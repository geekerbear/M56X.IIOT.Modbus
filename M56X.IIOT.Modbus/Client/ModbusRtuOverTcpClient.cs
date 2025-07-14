using M56X.Core.Common;
using M56X.Core.Helper;
using M56X.IIOT.Modbus.Common;
using M56X.IIOT.Modbus.Enums;
using System.Net;
using System.Net.Sockets;

namespace M56X.IIOT.Modbus.Client
{
    public class ModbusRtuOverTcpClient : ModbusClient
    {
        private TcpClient? _tcpClient;
        private NetworkStream _networkStream = default!;
        private ModbusFrameBuffer _frameBuffer = default!;

        public override bool IsConnected => _tcpClient?.Connected ?? false;

        /// <summary>
        /// 连接超时(默认: 1000毫秒)
        /// </summary>
        public int ConnectTimeout { get; set; } = ModbusTcpClient.DefaultConnectTimeout;

        /// <summary>
        /// 读取超时
        /// </summary>
        public int ReadTimeout { get; set; } = Timeout.Infinite;

        /// <summary>
        /// 写入超时
        /// </summary>
        public int WriteTimeout { get; set; } = Timeout.Infinite;

        /// <summary>
        /// 默认连接超时(默认: 1000毫秒)
        /// </summary>
        internal static int DefaultConnectTimeout { get; set; } = (int)TimeSpan.FromSeconds(1).TotalMilliseconds;

        public void Connect(string host = "127.0.0.1", int port = 502)
        {
            Initialize(new TcpClient(), new IPEndPoint(IPAddress.Parse(host), port));
        }

        public void Disconnect()
        {
            _networkStream?.Dispose();
            _tcpClient?.Close();
            _tcpClient?.Dispose();
            _frameBuffer?.Dispose();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="tcpClient"></param>
        /// <param name="remoteEndpoint"></param>
        /// <exception cref="Exception"></exception>
        private void Initialize(TcpClient tcpClient, IPEndPoint? remoteEndpoint)
        {
            Disconnect();

            _frameBuffer = new ModbusFrameBuffer(size: 260);

            remoteEndpoint ??= new IPEndPoint(IPAddress.Loopback, 502);

            _tcpClient = tcpClient;

            if (!tcpClient.ConnectAsync(remoteEndpoint.Address, remoteEndpoint.Port).Wait(ConnectTimeout))
                throw new Exception("连接超时");

            _networkStream = tcpClient.GetStream();

            _networkStream.ReadTimeout = ReadTimeout;
            _networkStream.WriteTimeout = WriteTimeout;
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
            _networkStream.Write(reqData);

            //接收数据
            // special case: broadcast (only for write commands)
            if (unitIdentifier == 0)
                return _frameBuffer.Buffer.AsSpan(0, 0);

            // wait for and process response
            frameLength = 0;
            reader.BaseStream.Seek(0, SeekOrigin.Begin);

            while (true)
            {
                frameLength += _networkStream.Read(frameBuffer.Buffer, frameLength, frameBuffer.Buffer.Length - frameLength);

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

            _ = reader.ReadByte();
            var rawFunctionCode = reader.ReadByte();

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
