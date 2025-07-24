namespace M56X.IIOT.Modbus.Client
{
    public abstract class ModbusSerialPortClient : ModbusClient
    {
        public abstract void Connect(string port);

        public abstract void Disconnect();
    }
}
