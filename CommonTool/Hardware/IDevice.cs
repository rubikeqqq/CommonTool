namespace CommonTool.Hardware
{
    /// <summary>
    /// 设备接口类，所有的设备都应该实现此接口
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        /// 设备名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 打开设备
        /// </summary>
        /// <returns></returns>
        bool Open( );

        /// <summary>
        /// 关闭设备
        /// </summary>
        /// <returns></returns>
        bool Close( );
    }
}
