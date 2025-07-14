using M56X.IIOT.Modbus.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M56X.IIOT.Modbus.Common
{
    /// <summary>
    /// Modbus错误
    /// </summary>
    public class ModbusException : Exception
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        public ModbusExceptionCode ExceptionCode { get; }

        internal ModbusException(string message) : base(message)
        {
            ExceptionCode = (ModbusExceptionCode)255;
        }

        internal ModbusException(ModbusExceptionCode exceptionCode, string message) : base(message)
        {
            ExceptionCode = exceptionCode;
        }
    }
}
