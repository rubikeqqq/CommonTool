using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using HslCommunication.Core;

namespace CommonTool.Communication
{
    /// <summary>
    /// 串口通讯类
    /// </summary>
    public class ComConnect : IConnect
    {
        private SerialPort _comPort = new SerialPort();

        /// <summary>
        /// 多线程等待锁
        /// </summary>
        private SimpleHybirdLock _sendLock = new SimpleHybirdLock();

        /// <summary>
        /// 同步写入方法中使用 写入后等待回复
        /// </summary>
        private readonly AutoResetEvent _autoEvent = new AutoResetEvent(false);

        /// <summary>
        /// 结束符
        /// </summary>
        public byte EndByte { get; set; } = 0x00;  //换行键 LF

        /// <summary>
        /// 串口号
        /// </summary>
        public string PortName { get; }

        /// <summary>
        /// 波特率
        /// </summary>
        public int BaudRate { get; }

        /// <summary>
        /// 奇偶校验位
        /// </summary>
        public Parity Parity { get; }

        /// <summary>
        /// 数据位
        /// </summary>
        public int DataBits { get; }

        /// <summary>
        /// 停止位
        /// </summary>
        public StopBits StopBits { get; }

        /// <summary>
        /// 串口是否连接
        /// </summary>
        public bool IsConnected => _comPort != null && _comPort.IsOpen;

        /// <summary>
        /// 数据处理事件
        /// <para>内部传送的数据是string</para>
        /// </summary>
        public event DataReceiveDelegate DataReceivedEvent;

        /// <summary>
        /// 连接状态改变事件
        /// </summary>
        public event EventHandler<bool> ConnectStatusChangedEvent;

        /// <summary>
        /// 参数构造函数（枚举）
        /// </summary>
        /// <param name="name">串口号</param>
        /// <param name="baudRates">波特率</param>
        /// <param name="parity">校验位</param>
        /// <param name="bits">数据位</param>
        /// <param name="stopBits">停止位</param>
        public ComConnect(string name, int baud, Parity parity, int dataBits, StopBits stopBits)
        {
            PortName = name;
            BaudRate = baud;
            Parity = parity;
            DataBits = dataBits;
            StopBits = stopBits;

            _comPort.DataReceived += PortDataReceivedCallback;
        }

        /// <summary>
        /// 参数构造函数（字符串）
        /// </summary>
        /// <param name="name">串口号</param>
        /// <param name="baud">波特率</param>
        /// <param name="parity">校验位</param>
        /// <param name="dataBits">数据位</param>
        /// <param name="stopBits">停止位</param>
        public ComConnect(string name, string baud, string parity, string dataBits, string stopBits)
        {
            PortName = name;
            BaudRate = int.Parse(baud);
            Parity = (Parity)Enum.Parse(typeof(Parity), parity);
            DataBits = int.Parse(dataBits);
            StopBits = (StopBits)Enum.Parse(typeof(StopBits), stopBits);

            _comPort.DataReceived += PortDataReceivedCallback;
        }

        /// <summary>
        /// 3个参数的构造函数
        /// <para>其他2个的默认:</para>
        /// <para>DataBits = 8</para>
        /// <para>StopBits = StopBits.One</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="baud"></param>
        /// <param name="parity"></param>
        public ComConnect(string name, int baud, Parity parity = Parity.None)
        {
            PortName = name;
            BaudRate = baud;
            Parity = parity;
            DataBits = 8;
            StopBits = StopBits.One;

            _comPort.DataReceived += PortDataReceivedCallback;
        }

        /// <summary>
        /// 连接串口
        /// </summary>
        public bool OpenConnect()
        {
            if (IsConnected)
            {
                _comPort.Close();
            }
            try
            {
                _comPort.PortName = PortName;
                _comPort.Parity = Parity;
                _comPort.BaudRate = BaudRate;
                _comPort.DataBits = DataBits;
                _comPort.StopBits = StopBits;

                _comPort.Open();
                OnConnectChanged(true);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        public void CloseConnect()
        {
            if (IsConnected)
            {
                _comPort.Close();
                _comPort.Dispose();
                _comPort = null;
                OnConnectChanged(false);
            }
        }

        /// <summary>
        /// 丢弃来自串行驱动程序的接收和发送缓冲区的数据
        /// </summary>
        public void DiscardBuffer()
        {
            _comPort.DiscardInBuffer();
            _comPort.DiscardOutBuffer();
        }

        /// <summary>
        /// 写入字节数组
        /// </summary>
        /// <param name="data">字节数组</param>
        public bool SendData(byte[] data)
        {
            if (!IsConnected) return false;
            try
            {
                _sendLock.Enter();
                _comPort.Write(data, 0, data.Length);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                _sendLock.Leave();
            }
        }

        /// <summary>
        ///  同步发送字节数据并等待回复
        /// </summary>
        /// <param name="data">发送的数据</param>
        /// <param name="timeOut">超时等待时间 默认为0 一直等待下去</param>
        public bool SendDataSync(byte[] data, int timeOut = 0)
        {
            if (!IsConnected)
            {
                return false;
            }
            try
            {
                _sendLock.Enter();
                _comPort.Write(data, 0, data.Length);
                //如果等待时间不设置的话 就一直等待回复
                if (timeOut <= 0)
                {
                    _autoEvent.WaitOne();
                }
                else
                {
                    //如果设置了超时,如果超过时间还没收到回复 就会自动跳过waitone
                    if (!_autoEvent.WaitOne(timeOut))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                _sendLock.Leave();
            }
        }

        /// <summary>
        /// 获取串口列表
        /// </summary>
        /// <returns>串口名称数组</returns>
        public static string[] GetPortNames()
        {
            return SerialPort.GetPortNames();
        }

        /// <summary>
        /// 数据接收处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PortDataReceivedCallback(object sender, SerialDataReceivedEventArgs e)
        {
            if (!IsConnected) return;
            if (_comPort.BytesToRead > 0)
            {
                byte[] buffer = new byte[_comPort.BytesToRead];
                int count = _comPort.Read(buffer, 0, buffer.Length);

                List<byte> byteData = ApplyEndByte(buffer, count);

                //触发事件处理的事件函数
                bool finished = false;
                if (DataReceivedEvent != null)
                {
                    finished = OnDataReceive(byteData.ToArray());
                }
                //同步锁释放
                if (finished)
                {
                    _autoEvent.Set();
                }
            }
        }

        /// <summary>
        /// 结束符处理
        /// </summary>
        /// <param name="buffer">所有数据</param>
        /// <param name="count">数据的长度</param>
        /// <returns>根据结束符获得的完整数据</returns>
        private List<byte> ApplyEndByte(byte[] buffer, int count)
        {
            // 根据结束字符检测是否接收完成

            List<byte> byteData = new List<byte>();
            //是否检测到结束符
            bool foundEnd = false;
            for (int i = 0; i < count; i++)
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
        /// 连接事件触发函数
        /// </summary>
        /// <param name="isConnect">是否连接</param>
        private void OnConnectChanged(bool isConnect)
        {
            if (ConnectStatusChangedEvent != null)
            {
                var del = ConnectStatusChangedEvent;
                del.Invoke(this, isConnect);
            }
        }

        /// <summary>
        /// 数据接收事件触发函数
        /// </summary>
        /// <param name="buffer">传送的数据</param>
        /// <returns>是否发送成功</returns>
        private bool OnDataReceive(byte[] buffer)
        {
            if (DataReceivedEvent != null)
            {
                var del = DataReceivedEvent;
                return del.Invoke(new DataReceivedEventArgs(buffer));
            }
            return false;
        }
    }
}