namespace M56X.IIOT.Modbus.Enums
{
    /// <summary>
    /// 字节顺序类型
    /// </summary>
    public enum ModbusEndianness
    {
        /// <summary>
        /// 小端,Windows默认
        /// 格式: HGFEDCBA DCBA BA
        /// </summary>
        DCBA = 1,

        /// <summary>
        /// 大端,Modbus默认
        /// 格式: AB ABCD ABCDEFGH
        /// </summary>
        ABCD = 2,

        /// <summary>
        /// 小端字节交换
        /// 格式: GHEFCDAB CDAB
        /// </summary>
        CDAB = 3,

        /// <summary>
        /// 大端字节交换
        /// 格式: BADCFEHG BADC
        /// </summary>
        BADC = 4
    }
}
