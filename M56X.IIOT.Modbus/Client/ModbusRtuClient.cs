using M56X.Core.Common;
using M56X.Core.Helper;
using M56X.IIOT.Modbus.Common;
using M56X.IIOT.Modbus.Enums;
using System.IO.Ports;

namespace M56X.IIOT.Modbus.Client
{
    public partial class ModbusRtuClient : ModbusClient
    {
        private IModbusRtuSerialPort? _serialPort;
        private ModbusFrameBuffer _frameBuffer = default!;

        public override bool IsConnected => _serialPort?.IsOpen ?? false;

        /// <summary>
        /// 波特率
        /// </summary>
        public int BaudRate { get; set; } = 9600;

        /// <summary>
        /// 获取或设置串行端口数据传输的握手协议
        /// </summary>
        public Handshake Handshake { get; set; } = Handshake.None;

        /// <summary>
        /// 校验设置
        /// </summary>
        public Parity Parity { get; set; } = Parity.None;

        /// <summary>
        /// 停止位
        /// </summary>
        public StopBits StopBits { get; set; } = StopBits.One;

        /// <summary>
        /// 读取超时
        /// </summary>
        public int ReadTimeout { get; set; } = 1000;

        /// <summary>
        /// 写入超时
        /// </summary>
        public int WriteTimeout { get; set; } = 1000;

        public ModbusRtuClient()
        {
            //
        }

        public void Connect(string port)
        {
            var serialPort = new ModbusRtuSerialPort(new SerialPort(port)
            {
                BaudRate = BaudRate,
                Handshake = Handshake,
                Parity = Parity,
                StopBits = StopBits,
                ReadTimeout = ReadTimeout,
                WriteTimeout = WriteTimeout
            });

            Initialize(serialPort);
        }

        public void Disconnect()
        {
            _serialPort?.Close();
            _frameBuffer?.Dispose();
        }

        private void Initialize(IModbusRtuSerialPort serialPort)
        {
            Disconnect();
            _frameBuffer = new ModbusFrameBuffer(256);

            _serialPort = serialPort;

            if (!serialPort.IsOpen)
                serialPort.Open();
        }

        protected override Span<byte> BuildFrame(byte unitIdentifier, FunctionCode functionCode, Action<BinaryWriterEx> extendFrame)
        {
            var frameBuffer = _frameBuffer;
            var writer = _frameBuffer.Writer;
            var reader = _frameBuffer.Reader;

            // build request
            if (!(0 <= unitIdentifier && unitIdentifier <= 247))
                throw new ModbusException(ErrorMessage.ModbusClient_InvalidUnitIdentifier);

            // special case: broadcast (only for write commands)
            if (unitIdentifier == 0)
            {
                switch (functionCode)
                {
                    case FunctionCode.WriteMultipleRegisters:
                    case FunctionCode.WriteSingleCoil:
                    case FunctionCode.WriteSingleRegister:
                    case FunctionCode.WriteMultipleCoils:
                    case FunctionCode.WriteFileRecord:
                    case FunctionCode.MaskWriteRegister:
                        break;
                    default:
                        throw new ModbusException(ErrorMessage.Modbus_InvalidUseOfBroadcast);
                }
            }

            writer.Seek(0, SeekOrigin.Begin);
            writer.Write(unitIdentifier);                                      // 00     Unit Identifier
            extendFrame(writer);

            var frameLength = (int)writer.BaseStream.Position;

            // add CRC
            var crc = ModbusUtils.CalculateCRC(frameBuffer.Buffer.AsMemory()[..frameLength]);
            writer.Write(crc);
            frameLength = (int)writer.BaseStream.Position;

            //发送的完整数据
            var reqData = frameBuffer.Buffer.ReadBytes(0, frameLength);
            SetDataMonitor(Core.Enums.DataDirection.TX, reqData.ReadBytes(0, reqData.Length));
            _serialPort?.Write(reqData, 0, frameLength);

            //接收数据
            // special case: broadcast (only for write commands)
            if (unitIdentifier == 0)
                return _frameBuffer.Buffer.AsSpan(0, 0);

            // wait for and process response
            frameLength = 0;
            reader.BaseStream.Seek(0, SeekOrigin.Begin);

            while (true)
            {
                frameLength += _serialPort!.Read(_frameBuffer.Buffer, frameLength, _frameBuffer.Buffer.Length - frameLength);

                if (ModbusUtils.DetectResponseFrame(unitIdentifier, _frameBuffer.Buffer.AsMemory()[..frameLength]))
                {
                    break;
                }
                else
                {
                    // reset length because one or more chunks of data were received and written to
                    // the buffer, but no valid Modbus frame could be detected and now the buffer is full
                    if (frameLength == _frameBuffer.Buffer.Length)
                        frameLength = 0;
                }
            }

            _ = _frameBuffer.Reader.ReadByte();
            var rawFunctionCode = _frameBuffer.Reader.ReadByte();

            //返回的完整数据
            var respData = frameBuffer.Buffer.ReadBytes(0, frameLength);
            SetDataMonitor(Core.Enums.DataDirection.RX, respData.ReadBytes(0, respData.Length));

            //解释错误代码
            if (rawFunctionCode == (byte)FunctionCode.Error + (byte)functionCode)
                ProcessError(functionCode, (ModbusExceptionCode)_frameBuffer.Buffer[2]);

            else if (rawFunctionCode != (byte)functionCode)
                throw new ModbusException(ErrorMessage.ModbusClient_InvalidResponseFunctionCode);

            return respData.AsSpan(1, frameLength - 3);
        }
    }
}
