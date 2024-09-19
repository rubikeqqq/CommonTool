using System;
using System.Threading;

namespace CommonTool.Hardware.PLC
{
    public abstract class PlcBase : IDevice
    {
        #region Fields
        protected Mutex mAccessMutex;
        protected Thread mConnectThread;
        protected bool mIsConnected;
        #endregion

        /// <summary>
        /// 是否连接
        /// </summary>
        public bool IsConnected
        {
            get => mIsConnected;
            protected set
            {
                if (value != mIsConnected)
                {
                    mIsConnected = value;
                    ConnectedStateChanged?.Invoke( this , mIsConnected ? "plc连接成功！" : "plc断开连接" );
                }
            }
        }

        /// <summary>
        /// ip地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 延时
        /// </summary>
        public int DelayTime { get; set; }

        /// <summary>
        /// 心跳地址
        /// </summary>
        public string HeartBeatAddress { get; set; }

        /// <summary>
        /// 心跳发送时间(ms)
        /// </summary>
        public int HeartBeatTime { get; set; } = 1000;

        /// <summary>
        /// 是否开启心跳
        /// </summary>
        public bool IsOpenHeartBeat { get; set; }

        /// <summary>
        /// PLC设备名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 连接状态改变事件
        /// </summary>
        public event EventHandler<string> ConnectedStateChanged;

        /// <summary>
        /// 打开PLC
        /// </summary>
        /// <returns></returns>
        public abstract bool Open( );

        /// <summary>
        /// 关闭PLC
        /// </summary>
        /// <returns></returns>
        public abstract bool Close( );

        /// <summary>
        /// 写继电器
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns>写入结果</returns>
        public abstract bool WriteBool( string address , bool value );

        /// <summary>
        /// 读继电器
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns>读取结果</returns>
        public abstract bool ReadBool( string address , out bool value );

        /// <summary>
        /// 写16位寄存器
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns>写入结果</returns>
        public abstract bool WriteShort( string address , short value );

        /// <summary>
        /// 读取16位寄存器
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns>读取结果</returns>
        public abstract bool ReadShort( string address , out short value );

        /// <summary>
        /// 写入32位寄存器
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns>写入结果</returns>
        public abstract bool WriteInt( string address , int value );

        /// <summary>
        /// 读取32位寄存器
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns>读取结果</returns>
        public abstract bool ReadInt( string address , out int value );

        /// <summary>
        /// 写入浮点数
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns>写入结果</returns>
        public abstract bool WriteFloat( string address , float value );

        /// <summary>
        /// 读取浮点数
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns>读取结果</returns>
        public abstract bool ReadFloat( string address , out float value );

        /// <summary>
        /// 写入双精度浮点数
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns>写入结果</returns>
        public abstract bool WriteDouble( string address , double value );

        /// <summary>
        /// 读取双精度浮点数
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns>读取结果</returns>
        public abstract bool ReadDouble( string address , out double value );

        /// <summary>
        /// 写字符串
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns>写入结果</returns>
        public abstract bool WriteString( string address , string value );

        /// <summary>
        /// 读取字符串
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="len">读取长度</param>
        /// <returns>读取结果</returns>
        public abstract bool ReadString( string address , ushort len , out string value );

        /// <summary>
        /// 写多个继电器
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">值</param>
        /// <returns>写入结果</returns>
        public abstract bool WriteBoolArray( string address , bool[] Value );

        /// <summary>
        /// 读多个继电器
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="len">读取长度</param>
        /// <returns>读取结果</returns>
        public abstract bool ReadBoolArray( string address , ushort len , out bool[] Value );

        /// <summary>
        /// 写多个16位寄存器
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">值</param>
        /// <returns>写入结果</returns>
        public abstract bool WriteShrotArray( string address , short[] values );

        /// <summary>
        /// 读取多个16位寄存器
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="len">读取长度</param>
        /// <returns>读取结果</returns>
        public abstract bool ReadShortArray( string address , ushort len , out short[] value );

        /// <summary>
        /// 写入多个32位寄存器
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">值</param>
        /// <returns>写入结果</returns>
        public abstract bool WriteIntArray( string address , int[] values );

        /// <summary>
        /// 读取多个32位寄存器
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="len">长度</param>
        /// <returns>读取结果</returns>
        public abstract bool ReadIntArray( string address , ushort len , out int[] value );

        /// <summary>
        /// 写入多个浮点数
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">值</param>
        /// <returns>写入结果</returns>
        public abstract bool WriteFloatArray( string address , float[] values );

        /// <summary>
        /// 读取多个浮点数
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="len">读取长度</param>
        /// <returns>读取结果</returns>
        public abstract bool ReadFloatArray( string address , ushort len , out float[] value );

        /// <summary>
        /// 写入多个双精度浮点数
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">值</param>
        /// <returns>写入结果</returns>
        public abstract bool WriteDoubleArray( string address , double[] values );

        /// <summary>
        /// 读取多个双精度浮点数
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="len">读取长度</param>
        /// <returns>读取结果</returns>
        public abstract bool ReadDoubleArray( string address , ushort len , out double[] value );

        /// <summary>
        /// 断线重连线程
        /// </summary>
        protected void ConnectThread( )
        {
            //如果没有设置心跳 直接返回
            if( string.IsNullOrEmpty( HeartBeatAddress ) )
            {
                return;
            }
            while( IsOpenHeartBeat )
            {
                //循环次数
                int LoopCount = 0;
                int Index = 0;
                //连接PLC
                Open( );
                //已经连接上
                while( IsConnected && IsOpenHeartBeat )
                {
                    Thread.Sleep( 200 );
                    if( LoopCount < 5 )
                    {
                        if( Index != 1 )
                        {
                            IsConnected = WriteShort( HeartBeatAddress , 0 );
                            Index = 1;
                        }
                    }
                    else
                    {
                        if( Index != 0 )
                        {
                            IsConnected = WriteShort( HeartBeatAddress , 0 );
                            Index = 0;
                        }
                    }
                    LoopCount++;
                    LoopCount = LoopCount % 20;
                }
                Thread.Sleep( HeartBeatTime );
            }
        }
    }
}
