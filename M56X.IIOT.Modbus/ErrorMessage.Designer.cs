﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace M56X.IIOT.Modbus {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ErrorMessage {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ErrorMessage() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("M56X.IIOT.Modbus.ErrorMessage", typeof(ErrorMessage).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   重写当前线程的 CurrentUICulture 属性，对
        ///   使用此强类型资源类的所有资源查找执行重写。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似 Invalid use of broadcast: Unit identifier &apos;0&apos; can only be used for write operations. 的本地化字符串。
        /// </summary>
        internal static string Modbus_InvalidUseOfBroadcast {
            get {
                return ResourceManager.GetString("Modbus_InvalidUseOfBroadcast", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 该值无效。有效值在0-65535的范围内 的本地化字符串。
        /// </summary>
        internal static string Modbus_InvalidValueUShort {
            get {
                return ResourceManager.GetString("Modbus_InvalidValueUShort", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The function code received in the query is not an allowable action for the server. This may be because the function code is only applicable to newer devices, and was not implemented in the unit selected. It could also indicate that the server is in the wrong state to process a request of this type, for example because it is unconfigured and is being asked to return register values. 的本地化字符串。
        /// </summary>
        internal static string ModbusClient_0x01_IllegalFunction {
            get {
                return ResourceManager.GetString("ModbusClient_0x01_IllegalFunction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The data address received in the query is not an allowable address for the server. More specifically, the combination of reference number and transfer length is invalid. 的本地化字符串。
        /// </summary>
        internal static string ModbusClient_0x02_IllegalDataAddress {
            get {
                return ResourceManager.GetString("ModbusClient_0x02_IllegalDataAddress", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 A value contained in the query data field is not an allowable value for server. This indicates a fault in the structure of the remainder of a complex request, such as that the implied length is incorrect. 的本地化字符串。
        /// </summary>
        internal static string ModbusClient_0x03_IllegalDataValue {
            get {
                return ResourceManager.GetString("ModbusClient_0x03_IllegalDataValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The quantity of registers is out of range (1..123). Make sure to request a minimum of one register. If you use the generic overload methods, please note that a single register consists of 2 bytes. If, for example, 1 x int32 value is requested, this results in a read operation of 2 registers. 的本地化字符串。
        /// </summary>
        internal static string ModbusClient_0x03_IllegalDataValue_0x7B {
            get {
                return ResourceManager.GetString("ModbusClient_0x03_IllegalDataValue_0x7B", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The quantity of registers is out of range (1..125). Make sure to request a minimum of one register. If you use the generic overload methods, please note that a single register consists of 2 bytes. If, for example, 1 x int32 value is requested, this results in a read operation of 2 registers. 的本地化字符串。
        /// </summary>
        internal static string ModbusClient_0x03_IllegalDataValue_0x7D {
            get {
                return ResourceManager.GetString("ModbusClient_0x03_IllegalDataValue_0x7D", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The quantity of coils is out of range (1..2000). 的本地化字符串。
        /// </summary>
        internal static string ModbusClient_0x03_IllegalDataValue_0x7D0 {
            get {
                return ResourceManager.GetString("ModbusClient_0x03_IllegalDataValue_0x7D0", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 An unrecoverable error occurred while the server was attempting to perform the requested action. 的本地化字符串。
        /// </summary>
        internal static string ModbusClient_0x04_ServerDeviceFailure {
            get {
                return ResourceManager.GetString("ModbusClient_0x04_ServerDeviceFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The server has accepted the request and is processing it, but a long duration of time will be required to do so. 的本地化字符串。
        /// </summary>
        internal static string ModbusClient_0x05_Acknowledge {
            get {
                return ResourceManager.GetString("ModbusClient_0x05_Acknowledge", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The server is engaged in processing a long–duration program command. 的本地化字符串。
        /// </summary>
        internal static string ModbusClient_0x06_ServerDeviceBusy {
            get {
                return ResourceManager.GetString("ModbusClient_0x06_ServerDeviceBusy", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The server attempted to read record file, but detected a parity error in the memory. 的本地化字符串。
        /// </summary>
        internal static string ModbusClient_0x08_MemoryParityError {
            get {
                return ResourceManager.GetString("ModbusClient_0x08_MemoryParityError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The gateway was unable to allocate an internal communication path from the input port to the output port for processing the request. 的本地化字符串。
        /// </summary>
        internal static string ModbusClient_0x0A_GatewayPathUnavailable {
            get {
                return ResourceManager.GetString("ModbusClient_0x0A_GatewayPathUnavailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 No response was obtained from the target device 的本地化字符串。
        /// </summary>
        internal static string ModbusClient_0x0B_GatewayTargetDeviceFailedToRespond {
            get {
                return ResourceManager.GetString("ModbusClient_0x0B_GatewayTargetDeviceFailedToRespond", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Array length must be equal to two bytes. 的本地化字符串。
        /// </summary>
        internal static string ModbusClient_ArrayLengthMustBeEqualToTwo {
            get {
                return ResourceManager.GetString("ModbusClient_ArrayLengthMustBeEqualToTwo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Array length must be greater than two bytes and even. 的本地化字符串。
        /// </summary>
        internal static string ModbusClient_ArrayLengthMustBeGreaterThanTwoAndEven {
            get {
                return ResourceManager.GetString("ModbusClient_ArrayLengthMustBeGreaterThanTwoAndEven", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The exception code received from the server is invalid. 的本地化字符串。
        /// </summary>
        internal static string ModbusClient_InvalidExceptionCode {
            get {
                return ResourceManager.GetString("ModbusClient_InvalidExceptionCode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 协议标识符无效 的本地化字符串。
        /// </summary>
        internal static string ModbusClient_InvalidProtocolIdentifier {
            get {
                return ResourceManager.GetString("ModbusClient_InvalidProtocolIdentifier", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 返回的功能码无效 的本地化字符串。
        /// </summary>
        internal static string ModbusClient_InvalidResponseFunctionCode {
            get {
                return ResourceManager.GetString("ModbusClient_InvalidResponseFunctionCode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 响应消息长度无效 的本地化字符串。
        /// </summary>
        internal static string ModbusClient_InvalidResponseMessageLength {
            get {
                return ResourceManager.GetString("ModbusClient_InvalidResponseMessageLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 无效从站ID。有效的地址在0-247的范围内。使用地址“0”向所有可用服务器广播写入命令。 的本地化字符串。
        /// </summary>
        internal static string ModbusClient_InvalidUnitIdentifier {
            get {
                return ResourceManager.GetString("ModbusClient_InvalidUnitIdentifier", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Quantity must be a positive integer number. Choose the &apos;count&apos; parameter such that an even number of bytes is requested. 的本地化字符串。
        /// </summary>
        internal static string ModbusClient_QuantityMustBePositiveInteger {
            get {
                return ResourceManager.GetString("ModbusClient_QuantityMustBePositiveInteger", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The TCP connection closed unexpectedly. 的本地化字符串。
        /// </summary>
        internal static string ModbusClient_TcpConnectionClosedUnexpectedly {
            get {
                return ResourceManager.GetString("ModbusClient_TcpConnectionClosedUnexpectedly", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Could not connect within the specified time. 的本地化字符串。
        /// </summary>
        internal static string ModbusClient_TcpConnectTimeout {
            get {
                return ResourceManager.GetString("ModbusClient_TcpConnectTimeout", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Unknown {0} Modbus error received. 的本地化字符串。
        /// </summary>
        internal static string ModbusClient_Unknown_Error {
            get {
                return ResourceManager.GetString("ModbusClient_Unknown_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The unit identifier is invalid. Valid node addresses are in the range of 1 - 247. 的本地化字符串。
        /// </summary>
        internal static string ModbusServer_InvalidUnitIdentifier {
            get {
                return ResourceManager.GetString("ModbusServer_InvalidUnitIdentifier", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 No unit found for the specified unit identifier. 的本地化字符串。
        /// </summary>
        internal static string ModbusServer_UnitIdentifierNotFound {
            get {
                return ResourceManager.GetString("ModbusServer_UnitIdentifierNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 There is no valid request available. 的本地化字符串。
        /// </summary>
        internal static string ModbusTcpRequestHandler_NoValidRequestAvailable {
            get {
                return ResourceManager.GetString("ModbusTcpRequestHandler_NoValidRequestAvailable", resourceCulture);
            }
        }
    }
}
