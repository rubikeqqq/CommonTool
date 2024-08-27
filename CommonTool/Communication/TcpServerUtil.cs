using HslCommunication.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace CommonTool.Communication
{
    /// <summary>
    /// tcp服务器类
    /// <para>实例化 使用有参构造 如果使用无参构造 还要设置属性ListenPort</para>
    /// <para>注意：此服务器类没有心跳功能 </para>
    /// <para>如果发送消息 需要先设置发送给哪个客户端 用属性SelectAddress来进行选择</para>
    /// <para>或者也可以使用Send的重载函数Send(string ipAddress, byte[] data)</para>
    /// </summary>
    public class TcpServerUtil : IConnect
    {
        private Dictionary<EndPoint, Socket> _clients = new Dictionary<EndPoint, Socket>();

        private Dictionary<EndPoint, AutoResetEvent> _autoEvents = new Dictionary<EndPoint, AutoResetEvent>();

        private Socket _sConn;

        private int _listenPort;

        private bool _isRun;

        private int _receiveBuffSize = 1024;

        private readonly SimpleHybirdLock _sendLock = new SimpleHybirdLock();

        /// <summary>
        /// 本地ip
        /// </summary>
        public string Address => _sConn.LocalEndPoint.ToString();

        /// <summary>
        /// 所有连接的远程ip
        /// </summary>
        public string[] RemoteAddresses => _clients.Keys.Select(x => x.ToString()).ToArray();

        /// <summary>
        /// 监听的port
        /// </summary>
        public int ListenPort => _listenPort;

        /// <summary>
        /// 是否连接
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// 选择的client地址
        /// </summary>
        public string SelectAddress { get; set; }

        /// <summary>
        /// 接收的缓存大小
        /// </summary>
        public int ReceiveBuffSize
        {
            get => _receiveBuffSize;
            set => _receiveBuffSize = value;
        }

        /// <summary>
        /// 数据接收事件
        /// </summary>
        public event ReceivedBytesEventHandler DataReceiveEvent;

        /// <summary>
        /// 当有客户端连接时触发的事件
        /// <para>注意：参数string 只是新连接的ip地址 不包含所有的ip地址</para>
        /// <para>如果想知道所有的ip地址 请使用RemoteAddresses属性 </para>
        /// <para>而且此服务器没有心跳功能 不能检测客户端是否已经断开连接</para>
        /// </summary>
        public event ClientAddressEventHandler ClientChangedEvent;

        /// <summary>
        /// 无参构造 如果调用需在OpenConnect之前指定ListenPort
        /// </summary>
        public TcpServerUtil()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="listenPort">监听的port</param>
        public TcpServerUtil(int listenPort)
        {
            _listenPort = listenPort;
        }

        /// <summary>
        /// 打开连接
        /// </summary>
        /// <returns></returns>
        public bool OpenConnect()
        {
            _isRun = true;
            return StartReceive();
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void CloseConnect()
        {
            try
            {
                _isRun = false;
                List<Socket> socketClients = GetAllSocketClients();
                foreach (Socket client in socketClients)
                {
                    try
                    {
                        if (client.Connected)
                        {
                            client.Close();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                ClearSocketClient();
                OnClientChanged();
                if (_sConn != null)
                {
                    _sConn.Close();
                    _sConn = null;
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                IsConnected = false;
            }
        }

        /// <summary>
        /// 发送数据 需设置SelectAddress来选择发送的客户端
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>是否发送成功</returns>
        public bool SendData(byte[] data)
        {
            if (!_isRun || !IsConnected) return false;

            var socket = GetSelectSocket();

            if (socket == null) return false;

            try
            {
                _sendLock.Enter();
                socket.Send(data);
                return true;
            }
            catch (Exception)
            {
                RemoveSocketClient(socket);
                return false;
            }
            finally
            {
                _sendLock.Leave();
            }
        }

        /// <summary>
        /// 发送数据 指定ip
        /// </summary>
        /// <param name="ipAddress">发送的地址</param>
        /// <param name="data">数据</param>
        /// <returns>是否发送成功</returns>
        public bool SendData(string ipAddress, byte[] data)
        {
            SelectAddress = ipAddress;
            return SendData(data);
        }

        /// <summary>
        /// 同步发送数据 需设置SelectAddress来选择发送的客户端
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="timeOut">等待时间 <=0 会一直等待 默认=0</param>
        /// <returns>是否发送成功并且返回数据</returns>
        public bool SendDataSync(byte[] data, int timeOut = 0)
        {
            if (!_isRun || !IsConnected) return false;
            var socket = GetSelectSocket();
            if (socket == null) return false;
            try
            {
                _sendLock.Enter();
                socket.Send(data);
                //如果等待时间不设置的话 就一直等待回复
                if (timeOut <= 0)
                {
                    _autoEvents[socket.RemoteEndPoint].WaitOne();
                }
                else
                {
                    //如果设置了超时,如果超过时间还没收到回复 就会自动跳过waitone
                    if (!_autoEvents[socket.RemoteEndPoint].WaitOne(timeOut))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                RemoveSocketClient(socket); return false;
            }
            finally
            {
                _sendLock.Leave();
            }
        }

        /// <summary>
        /// 同步发送数据 指定ip
        /// </summary>
        /// <param name="ipAddress">发送的地址</param>
        /// <param name="data">数据</param>
        /// <param name="timeOut">等待时间 <=0 会一直等待 默认=0</param>
        /// <returns>是否发送成功并且返回数据</returns>
        public bool SendDataSync(string ipAddress, byte[] data, int timeOut = 0)
        {
            SelectAddress = ipAddress;
            return SendDataSync(data, timeOut);
        }

        /// <summary>
        /// 获取当前的select socket
        /// </summary>
        /// <returns></returns>
        private Socket GetSelectSocket()
        {
            if (string.IsNullOrEmpty(SelectAddress))
            {
                return null;
            }

            return _clients.First(x => x.Key.ToString() == SelectAddress).Value ?? null;
        }

        /// <summary>
        /// 清除所有的client
        /// </summary>
        private void ClearSocketClient()
        {
            lock (_clients)
            {
                _clients.Clear();
                _autoEvents.Clear();
            }
        }

        /// <summary>
        /// 获取所有的socket
        /// </summary>
        /// <returns></returns>
        private List<Socket> GetAllSocketClients()
        {
            lock (_clients)
            {
                return _clients.Values.ToList();
            }
        }

        /// <summary>
        /// 开始监听
        /// </summary>
        /// <returns></returns>
        private bool StartReceive()
        {
            try
            {
                IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, _listenPort);
                _sConn = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _sConn.ReceiveBufferSize = _receiveBuffSize;
                _sConn.Bind(iPEndPoint);
                _sConn.Listen(100);

                //监听
                Task.Run(() =>
                {
                    while (_isRun)
                    {
                        try
                        {
                            Socket client = _sConn.Accept();
                            AddSocketClient(client);
                            ReceiveClientData(client);
                        }
                        catch (Exception)
                        {
                            IsConnected = false;
                        }
                    }
                });
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 接收数据线程
        /// </summary>
        /// <param name="client"></param>
        private void ReceiveClientData(Socket client)
        {
            Task.Run(() =>
            {
                byte[] recBuffer = new byte[_receiveBuffSize];
                while (_isRun)
                {
                    try
                    {
                        int num = client.Receive(recBuffer);
                        //当客户端断开的时候 会进入
                        if (num == 0)
                        {
                            RemoveSocketClient(client);
                            return;
                        }
                        byte[] copyBuffer = new byte[num];
                        Buffer.BlockCopy(recBuffer, 0, copyBuffer, 0, num);

                        //数据同步交互（和同步发送方法一起）
                        bool finished = false;
                        finished = OnDataReceive(client.RemoteEndPoint, copyBuffer);
                        //如果接收到完成的回复信息
                        if (finished)
                        {
                            _autoEvents[client.RemoteEndPoint].Set();
                        }
                    }
                    catch (Exception)
                    {
                        RemoveSocketClient(client);
                        return;
                    }
                }
            });
        }

        /// <summary>
        /// 接收数据事件处理函数
        /// </summary>
        /// <param name="remoteEndPoint"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool OnDataReceive(EndPoint remoteEndPoint, byte[] data)
        {
            if (DataReceiveEvent != null)
            {
                var del = DataReceiveEvent;
                var e = new RemoteDataReceivedEventArgs(remoteEndPoint.ToString(), data);
                return del.Invoke(e);
            }
            return false;
        }

        /// <summary>
        /// 客户端状态改变事件处理函数
        /// </summary>
        private void OnClientChanged()
        {
            if (ClientChangedEvent != null)
            {
                var del = ClientChangedEvent;
                del.Invoke(new ClientAddressEventArgs(RemoteAddresses));
            }
        }

        /// <summary>
        /// 移除客户端
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private bool RemoveSocketClient(Socket client)
        {
            lock (_clients)
            {
                try
                {
                    if (_clients.ContainsKey(client.RemoteEndPoint))
                    {
                        _clients.Remove(client.RemoteEndPoint);
                        OnClientChanged();
                        client.Close();
                        _autoEvents.Remove(client.RemoteEndPoint);
                        _autoEvents[client.RemoteEndPoint].Dispose();
                        IsConnected = _clients.Count > 0;
                        return true;
                    }
                }
                catch (Exception)
                {
                    IsConnected = _clients.Count > 0;
                }
                return false;
            }
        }

        /// <summary>
        /// 新增客户端
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private bool AddSocketClient(Socket client)
        {
            lock (_clients)
            {
                if (!_clients.ContainsKey(client.RemoteEndPoint))
                {
                    _clients.Add(client.RemoteEndPoint, client);
                    OnClientChanged();
                    _autoEvents.Add(client.RemoteEndPoint, new AutoResetEvent(false));
                    IsConnected = true;
                    return true;
                }
                return false;
            }
        }
    }
}