using M56X.Core.Enums;

namespace M56X.IIOT.Modbus.EventHandlers
{
    /// <summary>
    /// 数据监控
    /// </summary>
    public class DataMonitorEventArgs : EventArgs
    {
        /// <summary>
        /// 数据方向
        /// </summary>
        public readonly DataDirection Type;

        /// <summary>
        /// 数据
        /// </summary>
        public readonly byte[] Data;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public DataMonitorEventArgs(DataDirection type, byte[] data)
        {
            Type = type;
            Data = data;
        }
    }
}
