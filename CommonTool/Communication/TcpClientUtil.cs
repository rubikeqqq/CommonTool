using HslCommunication.Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace CommonTool.Communication
{
    /// <summary>
    /// tcp客户端类
    /// <para>实现了心跳功能 心跳功能需手动开启</para>
    /// <para>实现了断线重连功能 断线重连功能默认启动</para>
    /// </summary>
    public class TcpClientUtil : IConnect
    {
        private Socket _socket;

        private int _listenPort;

        private IPEndPoint _remoteEP;

        private bool _isRun;

        private bool _enableHeartBeat;

        private int _heartBeatInterval = 1000;

        private int _reconnectInterval = 3000;

        private int _receiveBufferSize = 1024;

        private byte[] _heartBeatBytes = new byte[0];

        private readonly AutoResetEvent _heartBeatEvent = new AutoResetEvent(false);

        private readonly AutoResetEvent _reconnectEvent = new AutoResetEvent(false);

        private readonly AutoResetEvent _sendSyncEvent = new AutoResetEvent(false);

        private readonly SimpleHybirdLock _sendLock = new SimpleHybirdLock();

        /// <summary>
        /// 结束符
        /// </summary>
        public byte EndByte { get; set; } = 0x00;

        /// <summary>
        /// client客户端自己的端口号
        /// </summary>
        public int ListenPort => _listenPort;

        /// <summary>
        /// 心跳使能
        /// </summary>
        public bool EnableHeartBeat
        {
            get => _enableHeartBeat;
            set => _enableHeartBeat = value;
        }

        /// <summary>
        /// 心跳内容 默认无
        /// </summary>
        public byte[] HeartBeatBytes
        {
            get => _heartBeatBytes;
            set => _heartBeatBytes = value;
        }

        /// <summary>
        /// 接收数据字节大小 默认1024byte
        /// </summary>
        public int ReceiveBufferSize
        {
            get => _receiveBufferSize;
            set => _receiveBufferSize = value;
        }

        /// <summary>
        /// 心跳时间间隔 默认1000ms
        /// </summary>
        public int HeartBeatInterval
        {
            get => _heartBeatInterval;
            set => _heartBeatInterval = value;
        }

        /// <summary>
        /// 重连时间间隔 默认3000ms
        /// </summary>
        public int ReConnectInterval
        {
            get => _reconnectInterval;
            set => _reconnectInterval = value;
        }

        /// <summary>
        /// 接收数据事件
        /// </summary>
        public event ReceivedBytesEventHandler DataReceiveEvent;

        /// <summary>
        /// 连接的远程信息
        /// </summary>
        public string RemoteAddress => _remoteEP.ToString();

        /// <summary>
        /// 是否连接
        /// </summary>
        public bool IsConnected { get; private set; }

        public TcpClientUtil()
        {
        }

        /// <summary>
        /// Tcp构造函数
        /// </summary>
        /// <param name="remoteIP">ip</param>
        /// <param name="remotePort">port</param>
        /// <param name="localPort">自己的port</param>
        public TcpClientUtil(string remoteIP, int remotePort, int localPort = 0)
        {
            _remoteEP = new IPEndPoint(IPAddress.Parse(remoteIP), remotePort);
            _listenPort = localPort;
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void CloseConnect()
        {
            try
            {
                if (_socket != null)
                {
                    _isRun = false;
                    _enableHeartBeat = false;
                    if (_socket.Connected)
                    {
                        _socket.Disconnect(false);
                    }
                    _heartBeatEvent.Set();
                    _reconnectEvent.Set();
                    _sendSyncEvent.Set();
                    _socket.Close();
                    _socket = null;
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
        /// 打开连接
        /// </summary>
        /// <returns></returns>
        public bool OpenConnect()
        {
            _isRun = true;
            StartReceive();
            return true;
        }

        /// <summary>
        /// 开始心跳
        /// </summary>
        public void StartHeartBeat(byte[] heartBeat)
        {
            SetHeartBeat(heartBeat);
            Task.Run(() =>
            {
                while (_isRun && _enableHeartBeat)
                {
                    SendData(_heartBeatBytes);
                    _heartBeatEvent.WaitOne(_heartBeatInterval);
                }
            });
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>是否发送成功</returns>
        public bool SendData(byte[] data)
        {
            if (!IsConnected || !_isRun) return false;
            try
            {
                _sendLock.Enter();
                _socket.Send(data);
                return true;
            }
            catch (Exception)
            {
                if (_socket.Connected)
                {
                    _socket.Disconnect(false);
                }
                IsConnected = false;
                return false;
            }
            finally
            {
                _sendLock.Leave();
            }
        }

        /// <summary>
        /// 同步发送数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="timeOut">等待时间 如果<=0 会一直等待 默认=0</param>
        /// <returns>是否发送成功</returns>
        public bool SendDataSync(byte[] data, int timeOut = 0)
        {
            if (!IsConnected || !_isRun) return false;
            try
            {
                _sendLock.Enter();
                _socket.Send(data);
                //如果等待时间不设置的话 就一直等待回复
                if (timeOut <= 0)
                {
                    _sendSyncEvent.WaitOne();
                }
                else
                {
                    //如果设置了超时,如果超过时间还没收到回复 就会自动跳过waitone
                    if (!_sendSyncEvent.WaitOne(timeOut))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                if (_socket.Connected)
                {
                    _socket.Disconnect(false);
                }
                IsConnected = false;
                return false;
            }
            finally
            {
                _sendLock.Leave();
            }
        }

        /// <summary>
        /// 接收数据函数
        /// </summary>
        private void StartReceive()
        {
            Task.Run(() =>
            {
                byte[] buffer = new byte[_receiveBufferSize];
                while (_isRun)
                {
                    try
                    {
                        if (buffer.Length != _receiveBufferSize)
                        {
                            buffer = new byte[_receiveBufferSize];
                        }
                        if (_isRun && !IsConnected)
                        {
                            StartReConnect();
                        }
                        else
                        {
                            EndPoint ep = new IPEndPoint(_remoteEP.Address, _listenPort);
                            //如果没有消息 会一直在此等待？
                            int num = _socket.ReceiveFrom(buffer, ref ep);
                            if (num <= 0)
                            {
                                IsConnected = false;
                            }
                            else
                            {
                                List<byte> byteData = ApplyEndByte(buffer, num);

                                //数据发送事件函数
                                bool finished = false;
                                finished = OnDataReceive(ep, byteData.ToArray());
                                //同步发送的方法
                                if (finished)
                                {
                                    _sendSyncEvent.Set();
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        IsConnected = false;
                    }
                }
            });
        }

        /// <summary>
        /// 接收数据事件处理函数
        /// </summary>
        /// <param name="point"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private bool OnDataReceive(EndPoint point, byte[] buffer)
        {
            if (DataReceiveEvent != null)
            {
                var del = DataReceiveEvent;
                var e = new RemoteDataReceivedEventArgs(point.ToString(), buffer);
                return del.Invoke(e);
            }
            return false;
        }

        /// <summary>
        /// 结束符处理
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        private List<byte> ApplyEndByte(byte[] buffer, int num)
        {
            // 根据结束字符检测是否接收完成

            List<byte> byteData = new List<byte>();
            //是否检测到结束符
            bool foundEnd = false;
            for (int i = 0; i < num; i++)
            {
                if (!foundEnd)
                {
                    byteData.Add(buffer[i]);
                    if (byteData[i] == EndByte)
                    {
                        break;
                    }
                }
            }

            return byteData;
        }

        /// <summary>
        /// 重连
        /// </summary>
        private void StartReConnect()
        {
            if (_socket != null && _socket.Connected)
            {
                _socket.Disconnect(false);
                _socket.Close();
            }

            //服务器
            IPEndPoint localEP = new IPEndPoint(_remoteEP.Address, _listenPort);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.SendTimeout = 3000;
            _socket.Bind(localEP);

            int num = 1;

            while (_isRun)
            {
                try
                {
                    if (CheckNetState(_remoteEP.Address.ToString()))
                    {
                        _socket.Connect(_remoteEP);
                        IsConnected = true;
                        return;
                    }
                }
                catch (Exception)
                {
                }
                num++;
                _reconnectEvent.WaitOne(_reconnectInterval);
            }
        }

        /// <summary>
        /// 检查网络是否联通
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        private bool CheckNetState(string ipAddress)
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    PingOptions options = new PingOptions();
                    byte[] buffer = new byte[8];
                    PingReply pingReplay = ping.Send(ipAddress, 700, buffer, options);
                    if (pingReplay.Status == IPStatus.Success)
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        /// <summary>
        /// 设置心跳
        /// </summary>
        /// <param name="heartBeat"></param>
        private void SetHeartBeat(byte[] heartBeat)
        {
            _heartBeatBytes = heartBeat;
            _enableHeartBeat = true;
        }
    }
}