using M56X.Core.Common;
using System.Buffers;

namespace M56X.IIOT.Modbus.Common
{
    /// <summary>
    /// Modbus数据帧
    /// </summary>
    internal class ModbusFrameBuffer : IDisposable
    {
        public byte[] Buffer { get; }
        public BinaryWriterEx Writer { get; }
        public BinaryReaderEx Reader { get; }

        public ModbusFrameBuffer(int size)
        {
            Buffer = ArrayPool<byte>.Shared.Rent(size);

            Writer = new BinaryWriterEx(new MemoryStream(Buffer));
            Reader = new BinaryReaderEx(new MemoryStream(Buffer));
        }

        #region 销毁
        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Writer.Dispose();
                    Reader.Dispose();

                    ArrayPool<byte>.Shared.Return(Buffer);
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
