using M56X.Core.Common;
using M56X.Core.Enums;
using M56X.IIOT.Modbus.Common;
using M56X.IIOT.Modbus.Enums;
using M56X.IIOT.Modbus.EventHandlers;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;

namespace M56X.IIOT.Modbus.Client
{
    /// <summary>
    /// Modbus客户端基类
    /// </summary>
    public abstract partial class ModbusClient : IDisposable
    {
        private bool disposedValue;

        /// <summary>
        /// 是否连接
        /// </summary>
        public abstract bool IsConnected { get; }

        /// <summary>
        /// 数据报文监控
        /// </summary>
        public event EventHandler<DataMonitorEventArgs>? DataMonitor;

        /// <summary>
        /// 设置报文
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        internal void SetDataMonitor(DataDirection type, byte[] data)
        {
            DataMonitor?.Invoke(this, new DataMonitorEventArgs(type, data));
        }

        /// <summary>
        /// 构建数据帧
        /// </summary>
        /// <param name="unitIdentifier"></param>
        /// <param name="functionCode"></param>
        /// <param name="extendFrame"></param>
        /// <returns></returns>
        protected abstract Span<byte> BuildFrame(byte unitIdentifier, FunctionCode functionCode, Action<BinaryWriterEx> extendFrame);

        /// <summary>
        /// 处理错误代码
        /// </summary>
        /// <param name="functionCode"></param>
        /// <param name="exceptionCode"></param>
        internal void ProcessError(FunctionCode functionCode, ModbusExceptionCode exceptionCode)
        {
            
        }

        private static ushort ConvertSize<T>(ushort count)
        {
            var size = typeof(T) == typeof(bool) ? 1 : Marshal.SizeOf<T>();
            size = count * size;

            if (size % 2 != 0)
                throw new ArgumentOutOfRangeException(ErrorMessage.ModbusClient_QuantityMustBePositiveInteger);

            var quantity = (ushort)(size / 2);

            return quantity;
        }

        #region 读取

        #region 读取线圈
        /// <summary>
        /// 读取线圈
        /// </summary>
        /// <param name="unitIdentifier">从站ID</param>
        /// <param name="startingAddress">起始地址</param>
        /// <param name="quantity">数量</param>
        /// <returns></returns>
        /// <exception cref="ModbusException"></exception>
        public Span<bool> ReadCoils(byte unitIdentifier, ushort startingAddress, ushort quantity)
        {
            var buffer = BuildFrame(unitIdentifier, FunctionCode.ReadCoils, writer =>
            {
                writer.Write((byte)FunctionCode.ReadCoils);                               // 07     Function Code

                if (BitConverter.IsLittleEndian)
                {
                    writer.WriteReverse(startingAddress);                       // 08-09  Starting Address
                    writer.WriteReverse(quantity);                              // 10-11  Quantity of Coils
                }
                else
                {
                    writer.Write(startingAddress);                              // 08-09  Starting Address
                    writer.Write(quantity);                                     // 10-11  Quantity of Coils
                }
            });

            if (buffer.Length < (byte)Math.Ceiling((double)quantity / 8) + 2)
                throw new ModbusException(ErrorMessage.ModbusClient_InvalidResponseMessageLength);

            var data = buffer[2..];
            return data.ToBools();
        }
        #endregion

        #region 读取离散输入
        /// <summary>
        /// 读取离散输入
        /// </summary>
        /// <param name="unitIdentifier">从站ID</param>
        /// <param name="startingAddress">起始地址</param>
        /// <param name="quantity">数量</param>
        /// <returns></returns>
        /// <exception cref="ModbusException"></exception>
        public Span<bool> ReadDiscreteInputs(byte unitIdentifier, ushort startingAddress, ushort quantity)
        {
            var buffer = BuildFrame(unitIdentifier, FunctionCode.ReadDiscreteInputs, writer =>
            {
                writer.Write((byte)FunctionCode.ReadDiscreteInputs);                // 07     Function Code

                if (BitConverter.IsLittleEndian)
                {
                    writer.WriteReverse(startingAddress);                       // 08-09  Starting Address
                    writer.WriteReverse(quantity);                              // 10-11  Quantity of Coils
                }
                else
                {
                    writer.Write(startingAddress);                              // 08-09  Starting Address
                    writer.Write(quantity);                                     // 10-11  Quantity of Coils
                }
            });
            if (buffer.Length < (byte)Math.Ceiling((double)quantity / 8) + 2)
                throw new ModbusException(ErrorMessage.ModbusClient_InvalidResponseMessageLength);

            var data = buffer[2..];
            return data.ToBools();
        }
        #endregion

        #region 读取保持寄存器
        /// <summary>
        /// 读取保持寄存器
        /// </summary>
        /// <param name="unitIdentifier">从站ID</param>
        /// <param name="startingAddress">起始地址</param>
        /// <param name="quantity">数量</param>
        /// <returns></returns>
        /// <exception cref="ModbusException"></exception>
        public Span<byte> ReadHoldingRegisters(byte unitIdentifier, ushort startingAddress, ushort quantity)
        {
            var buffer = BuildFrame(unitIdentifier, FunctionCode.ReadHoldingRegisters, writer =>
            {
                writer.Write((byte)FunctionCode.ReadHoldingRegisters);              // 07     Function Code

                if (BitConverter.IsLittleEndian)
                {
                    writer.WriteReverse(startingAddress);                                 // 08-09  Starting Address
                    writer.WriteReverse(quantity);                                        // 10-11  Quantity of Input Registers
                }
                else
                {
                    writer.Write(startingAddress);                                        // 08-09  Starting Address
                    writer.Write(quantity);                                               // 10-11  Quantity of Input Registers
                }
            });

            if (buffer.Length < quantity * 2 + 2)
                throw new ModbusException(ErrorMessage.ModbusClient_InvalidResponseMessageLength);

            return buffer[2..];
        }

        /// <summary>
        /// 读取保持寄存器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="unitIdentifier">从站ID</param>
        /// <param name="startingAddress">起始地址</param>
        /// <param name="quantity">数量</param>
        /// <param name="swap">是否两两交换(CDAB或BADC)</param>
        /// <returns></returns>
        public Span<T> ReadHoldingRegisters<T>(byte unitIdentifier, ushort startingAddress, ushort quantity, ModbusEndianness endianness = ModbusEndianness.ABCD) where T : unmanaged
        {
            ushort count = ConvertSize<T>(quantity);
            var dataset = MemoryMarshal.Cast<byte, T>(
                ReadHoldingRegisters(unitIdentifier, startingAddress, count));

            var size = Marshal.SizeOf<T>();

            if (size >= 2 && ModbusUtils.SwapBytes(endianness))
                ModbusUtils.SwitchEndianness<T>(dataset);

            if (size >= 4 && (endianness == ModbusEndianness.CDAB || endianness == ModbusEndianness.BADC))
                ModbusUtils.SwitchTwoEndianness<T>(dataset);

            return dataset;
        }
        #endregion

        #region 读取输入寄存器
        /// <summary>
        /// 读取输入寄存器
        /// </summary>
        /// <param name="unitIdentifier">从站ID</param>
        /// <param name="startingAddress">起始地址</param>
        /// <param name="quantity">数量</param>
        /// <returns></returns>
        /// <exception cref="ModbusException"></exception>
        public Span<byte> ReadInputRegisters(byte unitIdentifier, ushort startingAddress, ushort quantity)
        {
            var buffer = BuildFrame(unitIdentifier, FunctionCode.ReadInputRegisters, writer =>
            {
                writer.Write((byte)FunctionCode.ReadInputRegisters);                // 07     Function Code

                if (BitConverter.IsLittleEndian)
                {
                    writer.WriteReverse(startingAddress);                                 // 08-09  Starting Address
                    writer.WriteReverse(quantity);                                        // 10-11  Quantity of Input Registers
                }
                else
                {
                    writer.Write(startingAddress);                                        // 08-09  Starting Address
                    writer.Write(quantity);                                               // 10-11  Quantity of Input Registers
                }
            });

            if (buffer.Length < quantity * 2 + 2)
                throw new ModbusException(ErrorMessage.ModbusClient_InvalidResponseMessageLength);

            return buffer[2..];
        }

        /// <summary>
        /// 读取输入寄存器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="unitIdentifier"></param>
        /// <param name="startingAddress"></param>
        /// <param name="quantity"></param>
        /// <param name="endianness"></param>
        /// <returns></returns>
        public Span<T> ReadInputRegisters<T>(byte unitIdentifier, ushort startingAddress, ushort quantity, ModbusEndianness endianness = ModbusEndianness.ABCD) where T : unmanaged
        {
            ushort count = ConvertSize<T>(quantity);
            var dataset = MemoryMarshal.Cast<byte, T>(
                ReadInputRegisters(unitIdentifier, startingAddress, count));

            var size = Marshal.SizeOf<T>();

            if (size >= 2 && ModbusUtils.SwapBytes(endianness))
                ModbusUtils.SwitchEndianness<T>(dataset);

            if (size >= 4 && (endianness == ModbusEndianness.CDAB || endianness == ModbusEndianness.BADC))
                ModbusUtils.SwitchTwoEndianness<T>(dataset);

            return dataset;
        }
        #endregion
        
        #endregion

        #region 写线圈
        /// <summary>
        /// 写单个线圈
        /// </summary>
        /// <param name="unitIdentifier"></param>
        /// <param name="startingAddress"></param>
        /// <param name="value"></param>
        public void WriteSingleCoil(byte unitIdentifier, ushort startingAddress, bool value)
        {
            BuildFrame(unitIdentifier, FunctionCode.WriteSingleCoil, writer =>
            {
                writer.Write((byte)FunctionCode.WriteSingleCoil);                   // 07     Function Code

                if (BitConverter.IsLittleEndian)
                {
                    writer.WriteReverse(startingAddress);                       // 08-09  Starting Address
                    writer.WriteReverse((ushort)(value ? 0xFF00 : 0x0000));               // 10-11  Value
                }
                else
                {
                    writer.Write(startingAddress);                              // 08-09  Starting Address
                    writer.Write((ushort)(value ? 0xFF00 : 0x0000));                      // 10-11  Value
                }
            });
        }

        /// <summary>
        /// 写多个线圈
        /// </summary>
        /// <param name="unitIdentifier"></param>
        /// <param name="startingAddress"></param>
        /// <param name="values"></param>
        public void WriteMultipleCoils(byte unitIdentifier, ushort startingAddress, bool[] values)
        {
            var quantityOfOutputs = values.Length;
            var byteCount = (quantityOfOutputs + 7) / 8;
            var convertedData = new byte[byteCount];

            new BitArray(values)
                .CopyTo(convertedData, 0);

            BuildFrame(unitIdentifier, FunctionCode.WriteMultipleCoils, writer =>
            {
                writer.Write((byte)FunctionCode.WriteMultipleCoils);                  // 07     Function Code

                if (BitConverter.IsLittleEndian)
                {
                    writer.WriteReverse(startingAddress);                         // 08-09  Starting Address
                    writer.WriteReverse((ushort)quantityOfOutputs);                         // 10-11  Quantity of Outputs
                }

                else
                {
                    writer.Write(startingAddress);                                // 08-09  Starting Address
                    writer.Write((ushort)quantityOfOutputs);                                // 10-11  Quantity of Outputs
                }

                writer.Write((byte)byteCount);                                              // 12     Byte Count = Outputs

                writer.Write(convertedData);
            });
        }
        #endregion

        #region 写单个保持寄存器
        /// <summary>
        /// 写单个保持寄存器
        /// </summary>
        /// <param name="unitIdentifier"></param>
        /// <param name="startingAddress"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void WriteSingleRegister(byte unitIdentifier, ushort startingAddress, byte[] value)
        {
            if (value.Length != 2)
                throw new ArgumentOutOfRangeException(ErrorMessage.ModbusClient_ArrayLengthMustBeEqualToTwo);

            BuildFrame(unitIdentifier, FunctionCode.WriteSingleRegister, writer =>
            {
                writer.Write((byte)FunctionCode.WriteSingleRegister);               // 07     Function Code

                if (BitConverter.IsLittleEndian)
                    writer.WriteReverse(startingAddress);                                 // 08-09  Starting Address
                else
                    writer.Write(startingAddress);                                        // 08-09  Starting Address

                writer.Write(value);                                                      // 10-11  Value
            });
        }

        /// <summary>
        /// 写单个保持寄存器
        /// </summary>
        /// <param name="unitIdentifier"></param>
        /// <param name="startingAddress"></param>
        /// <param name="value"></param>
        /// <param name="endianness"></param>
        public void WriteSingleRegister(byte unitIdentifier, ushort startingAddress, short value, ModbusEndianness endianness = ModbusEndianness.ABCD)
        {
            if (ModbusUtils.SwapBytes(endianness))
                value = ModbusUtils.SwitchEndianness(value);

            WriteSingleRegister(unitIdentifier, startingAddress, MemoryMarshal.Cast<short, byte>(new[] { value }).ToArray());
        }

        /// <summary>
        /// 写单个保持寄存器
        /// </summary>
        /// <param name="unitIdentifier"></param>
        /// <param name="registerAddress"></param>
        /// <param name="value"></param>
        /// <param name="endianness"></param>
        public void WriteSingleRegister(byte unitIdentifier, ushort startingAddress, ushort value, ModbusEndianness endianness = ModbusEndianness.ABCD)
        {
            if (ModbusUtils.SwapBytes(endianness))
                value = ModbusUtils.SwitchEndianness(value);

            WriteSingleRegister(unitIdentifier, startingAddress, MemoryMarshal.Cast<ushort, byte>(new[] { value }).ToArray());
        }
        #endregion

        #region 写多个保持寄存器
        /// <summary>
        /// 写多个保持寄存器
        /// </summary>
        /// <param name="unitIdentifier"></param>
        /// <param name="startingAddress"></param>
        /// <param name="dataset"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void WriteMultipleRegisters(byte unitIdentifier, ushort startingAddress, byte[] dataset)
        {
            if (dataset.Length < 2 || dataset.Length % 2 != 0)
                throw new ArgumentOutOfRangeException(ErrorMessage.ModbusClient_ArrayLengthMustBeGreaterThanTwoAndEven);

            var quantity = dataset.Length / 2;

            BuildFrame(unitIdentifier, FunctionCode.WriteMultipleRegisters, writer =>
            {
                writer.Write((byte)FunctionCode.WriteMultipleRegisters);            // 07     Function Code

                if (BitConverter.IsLittleEndian)
                {
                    writer.WriteReverse(startingAddress);                                 // 08-09  Starting Address
                    writer.WriteReverse((ushort)quantity);                                // 10-11  Quantity of Registers
                }

                else
                {
                    writer.Write(startingAddress);                                        // 08-09  Starting Address
                    writer.Write((ushort)quantity);                                       // 10-11  Quantity of Registers
                }

                writer.Write((byte)(quantity * 2));                                       // 12     Byte Count = Quantity of Registers * 2

                writer.Write(dataset, 0, dataset.Length);
            });
        }

        /// <summary>
        /// 写多个保持寄存器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="unitIdentifier"></param>
        /// <param name="startingAddress"></param>
        /// <param name="dataset"></param>
        /// <param name="endianness"></param>
        public void WriteMultipleRegisters<T>(byte unitIdentifier, ushort startingAddress, T[] dataset, ModbusEndianness endianness = ModbusEndianness.ABCD) where T : unmanaged
        {
            var size = Marshal.SizeOf<T>();

            var _dataset = dataset.AsSpan();
            if (size >= 2 && ModbusUtils.SwapBytes(endianness))
                ModbusUtils.SwitchEndianness<T>(_dataset);

            if (size >= 4 && (endianness == ModbusEndianness.CDAB || endianness == ModbusEndianness.BADC))
                ModbusUtils.SwitchTwoEndianness<T>(_dataset);

            WriteMultipleRegisters(unitIdentifier, startingAddress, MemoryMarshal.Cast<T, byte>(_dataset).ToArray());
        }
        #endregion

        /// <summary>
        /// 读/写多个寄存器(先写再读)
        /// </summary>
        /// <param name="unitIdentifier"></param>
        /// <param name="readStartingAddress"></param>
        /// <param name="readQuantity"></param>
        /// <param name="writeStartingAddress"></param>
        /// <param name="dataset"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ModbusException"></exception>
        public Span<byte> ReadWriteMultipleRegisters(byte unitIdentifier, ushort readStartingAddress, ushort readQuantity, ushort writeStartingAddress, byte[] dataset)
        {
            if (dataset.Length < 2 || dataset.Length % 2 != 0)
                throw new ArgumentOutOfRangeException(ErrorMessage.ModbusClient_ArrayLengthMustBeGreaterThanTwoAndEven);

            var writeQuantity = dataset.Length / 2;

            var buffer = BuildFrame(unitIdentifier, FunctionCode.ReadWriteMultipleRegisters, writer =>
            {
                writer.Write((byte)FunctionCode.ReadWriteMultipleRegisters);      // 07     Function Code

                if (BitConverter.IsLittleEndian)
                {
                    writer.WriteReverse(readStartingAddress);                           // 08-09  Read Starting Address
                    writer.WriteReverse(readQuantity);                                  // 10-11  Quantity to Read
                    writer.WriteReverse(writeStartingAddress);                          // 12-13  Read Starting Address
                    writer.WriteReverse((ushort)writeQuantity);                         // 14-15  Quantity to Write
                }
                else
                {
                    writer.Write(readStartingAddress);                                  // 08-09  Read Starting Address
                    writer.Write(readQuantity);                                         // 10-11  Quantity to Read
                    writer.Write(writeStartingAddress);                                 // 12-13  Read Starting Address
                    writer.Write((ushort)writeQuantity);                                // 14-15  Quantity to Write
                }

                writer.Write((byte)(writeQuantity * 2));                                // 16     Byte Count = Quantity to Write * 2

                writer.Write(dataset, 0, dataset.Length);
            });

            if (buffer.Length < readQuantity * 2 + 2)
                throw new ModbusException(ErrorMessage.ModbusClient_InvalidResponseMessageLength);

            return buffer[2..];
        }

        public Span<TRead> ReadWriteMultipleRegisters<TRead, TWrite>(byte unitIdentifier, ushort readStartingAddress, ushort readCount, ushort writeStartingAddress, TWrite[] dataset, ModbusEndianness endianness = ModbusEndianness.ABCD) where TRead : unmanaged                                                                                                                                                                   where TWrite : unmanaged
        {
            var size = Marshal.SizeOf<TWrite>();
            var _dataset = dataset.AsSpan();

            if (size >= 2 && ModbusUtils.SwapBytes(endianness))
                ModbusUtils.SwitchEndianness<TWrite>(_dataset);

            if (size >= 4 && (endianness == ModbusEndianness.CDAB || endianness == ModbusEndianness.BADC))
                ModbusUtils.SwitchTwoEndianness<TWrite>(_dataset);

            var readQuantity = ConvertSize<TRead>(readCount);
            var byteData = MemoryMarshal.Cast<TWrite, byte>(_dataset).ToArray();

            var dataset2 = MemoryMarshal.Cast<byte, TRead>(ReadWriteMultipleRegisters(unitIdentifier, readStartingAddress, readQuantity, writeStartingAddress, byteData));

            var size2 = Marshal.SizeOf<TRead>();

            if (size2 >= 2 && ModbusUtils.SwapBytes(endianness))
                ModbusUtils.SwitchEndianness<TRead>(dataset2);

            if (size2 >= 4 && (endianness == ModbusEndianness.CDAB || endianness == ModbusEndianness.BADC))
                ModbusUtils.SwitchTwoEndianness<TRead>(dataset2);

            return dataset2;
        }

        //public Span<bool> ReadHoldingRegistersBit(byte unitIdentifier, ushort address, byte bitIndex, byte quantity)
        //{
        //    if (bitIndex >= 16 || bitIndex < 0)
        //    {
        //        throw new ArgumentOutOfRangeException($"bitIndex({bitIndex})不合法(0~15)");
        //    }
        //}

        #region 释放
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~ModbusClient()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
