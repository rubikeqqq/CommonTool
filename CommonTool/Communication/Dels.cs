using System;

namespace CommonTool.Communication
{
    public delegate bool ReceivedBytesEventHandler(RemoteDataReceivedEventArgs e);

    public delegate bool DataReceiveDelegate(DataReceivedEventArgs e);

    public delegate void ClientAddressEventHandler(ClientAddressEventArgs e);

    /// <summary>
    /// 通讯接收数据类
    /// </summary>
    public class DataReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// 发送的数据
        /// </summary>
        public byte[] Bytes { get; private set; }

        public DataReceivedEventArgs(byte[] bytes)
        {
            Bytes = bytes;
        }
    }

    /// <summary>
    /// 远程连接信息类
    /// </summary>
    public class RemoteDataReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// 远程连接的ip
        /// </summary>
        public string RemoteName { get; private set; }

        /// <summary>
        /// 发送的数据
        /// </summary>
        public byte[] Bytes { get; private set; }

        public RemoteDataReceivedEventArgs(string remote, byte[] bytes)
        {
            RemoteName = remote;
            Bytes = bytes;
        }
    }

    /// <summary>
    /// 服务器统计客户端ip的类
    /// </summary>
    public class ClientAddressEventArgs : EventArgs
    {
        /// <summary>
        /// 当前连接的客户端的ip数组
        /// </summary>
        public string[] Addresses { get; private set; }
        public ClientAddressEventArgs(string[] addresses)
        {
            Addresses = addresses;
        }
    }
    
}
