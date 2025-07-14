namespace M56X.IIOT.Modbus.Enums
{
    /// <summary>
    /// Modbus功能码
    /// </summary>
    public enum FunctionCode
    {
        /// <summary>
        /// 读线圈 FC01
        /// </summary>
        ReadCoils = 0x01,
        /// <summary>
        /// 读离散输入 FC02
        /// </summary>
        ReadDiscreteInputs = 0x02,
        /// <summary>
        /// 读保持寄存器 FC03
        /// </summary>
        ReadHoldingRegisters = 0x03,
        /// <summary>
        /// 读输入寄存器 FC04
        /// </summary>
        ReadInputRegisters = 0x04,
        /// <summary>
        /// 写单个线圈 FC05
        /// </summary>
        WriteSingleCoil = 0x05,
        /// <summary>
        /// 写单个保持寄存器 FC06
        /// </summary>
        WriteSingleRegister = 0x06,
        /// <summary>
        /// 写多个线圈 FC15
        /// </summary>
        WriteMultipleCoils = 0x0F,
        /// <summary>
        /// 写多个保持寄存器 FC16
        /// </summary>
        WriteMultipleRegisters = 0x10,

        /// <summary>
        /// 
        /// </summary>
        ReadFileRecord = 0x14,              // FC20

        /// <summary>
        /// 
        /// </summary>
        WriteFileRecord = 0x15,             // FC21

        /// <summary>
        /// 屏蔽写寄存器 FC22
        /// </summary>
        MaskWriteRegister = 0x16,
        /// <summary>
        /// 读/写多个寄存器 FC23
        /// </summary>
        ReadWriteMultipleRegisters = 0x17,
        /// <summary>
        /// 文件记录操作 FC20
        /// </summary>
        FileRecord = 0x14,
        /// <summary>
        /// 设备诊断
        /// </summary>
        Diagnostic = 0x08,
        /// <summary>
        /// 设备操作
        /// </summary>
        DeviceIdentification = 0x2B,

        Error = 0x80                        // FC128
    }
}
