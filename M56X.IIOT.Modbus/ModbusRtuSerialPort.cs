using System.IO.Ports;

namespace M56X.IIOT.Modbus
{
    public class ModbusRtuSerialPort : IModbusRtuSerialPort
    {
        private readonly SerialPort _serialPort;

        public string PortName => _serialPort.PortName;

        public bool IsOpen => _serialPort.IsOpen;

        public ModbusRtuSerialPort(SerialPort serialPort)
        {
            _serialPort = serialPort;
        }

        public void Open()
        {
            _serialPort.Open();
        }

        public void Close()
        {
            _serialPort.Close();
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return _serialPort.Read(buffer, offset, count);
        }

        public async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken token)
        {
            using var timeoutCts = new CancellationTokenSource(_serialPort.ReadTimeout);

            /* _serialPort.DiscardInBuffer is essential here to cancel the operation */
            using (timeoutCts.Token.Register(() =>
            {
                if (IsOpen)
                    _serialPort.DiscardInBuffer();
            }))
            using (token.Register(() => timeoutCts.Cancel()))
            {
                try
                {
                    return await _serialPort.BaseStream.ReadAsync(buffer, offset, count, timeoutCts.Token);
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    throw;
                }
                catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
                {
                    throw new TimeoutException("The asynchronous read operation timed out.");
                }
                catch (IOException) when (timeoutCts.IsCancellationRequested && !token.IsCancellationRequested)
                {
                    throw new TimeoutException("The asynchronous read operation timed out.");
                }
            }
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            _serialPort.Write(buffer, offset, count);
        }

        public async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken token)
        {
            using var timeoutCts = new CancellationTokenSource(_serialPort.WriteTimeout);

            /* _serialPort.DiscardInBuffer is essential here to cancel the operation */
            using (timeoutCts.Token.Register(() => _serialPort.DiscardOutBuffer()))
            using (token.Register(() => timeoutCts.Cancel()))
            {
                try
                {
                    await _serialPort.BaseStream.WriteAsync(buffer, offset, count, timeoutCts.Token);
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    throw;
                }
                catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
                {
                    throw new TimeoutException("The asynchronous write operation timed out.");
                }
                catch (IOException) when (timeoutCts.IsCancellationRequested && !token.IsCancellationRequested)
                {
                    throw new TimeoutException("The asynchronous write operation timed out.");
                }
            }
        }
    }
}
