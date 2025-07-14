namespace M56X.IIOT.Modbus
{
    public static class SpanExtensions
    {
        public static Span<bool> ToBools(this Span<byte> data)
        {
            var response = new bool[10];
            for (int i = 0; i < 10; i++)
            {
                uint intData = data[i / 8];
                uint mask = Convert.ToUInt32(Math.Pow(2, (i % 8)));
                response[i] = Convert.ToBoolean((intData & mask) / mask);
            }
            return response;
        }
    }
}
