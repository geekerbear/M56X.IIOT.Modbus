namespace M56X.IIOT.Modbus.Client
{
    public abstract class ModbusNetClient : ModbusClient
    {
        public abstract void Connect(string host = "127.0.0.1", int port = 502);

        public abstract void Disconnect();
    }
}
