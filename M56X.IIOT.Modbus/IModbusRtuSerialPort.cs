namespace M56X.IIOT.Modbus
{
    public interface IModbusRtuSerialPort
    {
        /// <summary>
        /// 端口名称
        /// </summary>
        string PortName { get; }

        /// <summary>
        /// 是否已打开
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        int Read(byte[] buffer, int offset, int count);

        /// <summary>
        /// 异步读取
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken token);

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        void Write(byte[] buffer, int offset, int count);

        /// <summary>
        /// 异步写入
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken token);

        /// <summary>
        /// 打开
        /// </summary>
        void Open();

        /// <summary>
        /// 关闭
        /// </summary>
        void Close();
    }
}
