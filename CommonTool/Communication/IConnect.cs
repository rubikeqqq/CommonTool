namespace CommonTool.Communication
{
    /// <summary>
    /// 通讯类的基接口
    /// </summary>
    public interface IConnect
    {
        /// <summary>
        /// 是否连接
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 打开连接
        /// </summary>
        bool OpenConnect();

        /// <summary>
        /// 关闭连接
        /// </summary>
        void CloseConnect();

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">数据</param>
        bool SendData(byte[] data);

        /// <summary>
        /// 同步发送数据
        /// </summary>
        /// <param name="data">数据</param>
        bool SendDataSync(byte[] data,int timeOut = 0);
    }
}