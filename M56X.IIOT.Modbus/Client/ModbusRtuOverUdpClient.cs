using M56X.IIOT.Modbus.Common;
using System.Net.Sockets;
using System.Net;
using M56X.Core.Common;
using M56X.Core.Helper;
using M56X.IIOT.Modbus.Enums;

namespace M56X.IIOT.Modbus.Client
{
    public class ModbusRtuOverUdpClient : ModbusClient
    {
        private UdpClient? _udpClient;
        private IPEndPoint? _remoteEndpoint;
        private ModbusFrameBuffer _frameBuffer = default!;

        public override bool IsConnected => true;

        public ModbusRtuOverUdpClient()
        {
        }

        public void Connect(string host = "127.0.0.1", int port = 502)
        {
            Initialize(new UdpClient(), new IPEndPoint(IPAddress.Parse(host), port));
        }

        public void Disconnect()
        {
            _udpClient?.Close();
            _udpClient?.Dispose();
            _frameBuffer?.Dispose();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="udpClient"></param>
        /// <param name="remoteEndpoint"></param>
        private void Initialize(UdpClient udpClient, IPEndPoint? remoteEndpoint)
        {
            Disconnect();

            _frameBuffer = new ModbusFrameBuffer(size: 260);

            remoteEndpoint ??= new IPEndPoint(IPAddress.Loopback, 502);

            _remoteEndpoint = remoteEndpoint;

            _udpClient = udpClient;


            //if (remoteEndpoint is not null && !udpClient.ConnectAsync(remoteEndpoint.Address, remoteEndpoint.Port).Wait(ConnectTimeout))
            //    throw new Exception("连接超时");
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
            _udpClient?.Send(reqData, _remoteEndpoint);

            //接收数据
            // special case: broadcast (only for write commands)
            if (unitIdentifier == 0)
                return _frameBuffer.Buffer.AsSpan(0, 0);

            // wait for and process response
            frameLength = 0;
            reader.BaseStream.Seek(0, SeekOrigin.Begin);

            while (true)
            {
                var data = _udpClient?.Receive(ref _remoteEndpoint);
                int len = data?.Length ?? 0;

                if (len == 0)
                    throw new InvalidOperationException("连接意外关闭");

                data?.CopyTo(_frameBuffer.Buffer, 0);

                frameLength += len;

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
