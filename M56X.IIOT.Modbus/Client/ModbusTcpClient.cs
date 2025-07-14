using M56X.Core.Common;
using M56X.Core.Helper;
using M56X.IIOT.Modbus.Common;
using M56X.IIOT.Modbus.Enums;
using System.Net;
using System.Net.Sockets;

namespace M56X.IIOT.Modbus.Client
{
    public partial class ModbusTcpClient : ModbusClient
    {
        private ushort _transactionIdentifierBase;
        private readonly Lock _transactionIdentifierLock;

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

        public ModbusTcpClient()
        {
            _transactionIdentifierBase = 0;
            _transactionIdentifierLock = new Lock();
        }

        private ushort GetTransactionIdentifier()
        {
            lock (_transactionIdentifierLock)
            {
                return _transactionIdentifierBase++;
            }
        }

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
            ushort pduLen = 0;

            var frameBuffer = _frameBuffer;
            var writer = _frameBuffer.Writer;
            var reader = _frameBuffer.Reader;

            //跳过帧头,写具体数据
            writer.Seek(7, SeekOrigin.Begin);
            extendFrame(writer);    //回调写具体数据
            var frameLength = (int)writer.BaseStream.Position; //获取当前流的当前位置，计算帧长度

            //转到流起始位置，写帧头
            writer.Seek(0, SeekOrigin.Begin);

            //00 00 00 00 00 06
            if (BitConverter.IsLittleEndian)
            {
                writer.Write(GetTransactionIdentifier().ToBytes(true));                 // 00-01  Transaction Identifier
                writer.Write(((ushort)0).ToBytes(true));                                // 02-03  Protocol Identifier
                writer.Write(((ushort)(frameLength - 6)).ToBytes(true));                // 04-05  Length
            }
            else
            {
                writer.Write(GetTransactionIdentifier().ToBytes());                     // 00-01  Transaction Identifier
                writer.Write(((ushort)0).ToBytes());                                    // 02-03  Protocol Identifier
                writer.Write(((ushort)(frameLength - 6)).ToBytes());                    // 04-05  Length
            }

            writer.Write(unitIdentifier);   //从站地址

            //发送的完整数据
            var reqData = frameBuffer.Buffer.ReadBytes(0, frameLength);
            SetDataMonitor(Core.Enums.DataDirection.TX, reqData.ReadBytes(0, reqData.Length));
            _networkStream.Write(reqData);

            //接收数据
            frameLength = 0;
            var isParsed = false;
            reader.BaseStream.Seek(0, SeekOrigin.Begin);

            while (true)
            {
                int len;
                len = _networkStream.Read(frameBuffer.Buffer, frameLength, frameBuffer.Buffer.Length - frameLength);

                if (len == 0)
                    throw new InvalidOperationException("连接意外关闭");

                frameLength += len;

                if (frameLength >= 7)
                {
                    if (!isParsed)
                    {
                        _ = reader.ReadUInt16Reverse();                                     // 00-01  Transaction Identifier
                        var protocolIdentifier = reader.ReadUInt16Reverse();                // 02-03  Protocol Identifier               
                        pduLen = reader.ReadUInt16Reverse();                                // 04-05  Length
                        _ = reader.ReadByte();                                              // 06     Unit Identifier

                        if (protocolIdentifier != 0)
                            throw new ModbusException(ErrorMessage.ModbusClient_InvalidProtocolIdentifier);

                        isParsed = true;
                    }

                    // full frame received
                    if (frameLength - 6 >= pduLen)
                        break;
                }
            }

            var rawFunctionCode = reader.ReadByte();

            //返回的完整数据
            var respData = frameBuffer.Buffer.ReadBytes(0, frameLength);
            SetDataMonitor(Core.Enums.DataDirection.RX, respData.ReadBytes(0, respData.Length));

            //解释错误代码
            if (rawFunctionCode == (byte)FunctionCode.Error + (byte)functionCode)
                ProcessError(functionCode, (ModbusExceptionCode)frameBuffer.Buffer[8]);

            else if (rawFunctionCode != (byte)functionCode)
                throw new ModbusException(ErrorMessage.ModbusClient_InvalidResponseFunctionCode);

            return respData.AsSpan(7, frameLength - 7);
        }
    }
}
