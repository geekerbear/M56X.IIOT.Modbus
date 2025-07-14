using M56X.Core.Helper;
using M56X.IIOT.Modbus;
using M56X.IIOT.Modbus.Enums;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

Span<byte> data;

var sleepTime = TimeSpan.FromMilliseconds(1000);
byte unitIdentifier = 0x01;
ushort startingAddress = 0;
ushort registerAddress = 0;

var client = new M56X.IIOT.Modbus.Client.ModbusRtuClient();
client.DataMonitor += Client_DataMonitor;

static void Client_DataMonitor(object? sender, M56X.IIOT.Modbus.EventHandlers.DataMonitorEventArgs e)
{
    Console.WriteLine($"{e.Type}: {e.Data.ToHex()}");
}

client.Connect("COM10");

var ddd = client.ReadWriteMultipleRegisters<float, float>(1, 0, 2, 0, [222.1f, 333.2f]);

Console.ReadLine();


//Console.WriteLine($"\nFC01 - ReadCoils: SlaveId: {unitIdentifier} StartingAddress: {startingAddress}");
//data = client.ReadCoils(unitIdentifier, startingAddress, 10);
//Thread.Sleep(sleepTime);

//Console.WriteLine($"\nFC02 - ReadDiscreteInputs: SlaveId: {unitIdentifier} StartingAddress: {startingAddress}");
//data = client.ReadDiscreteInputs(unitIdentifier, startingAddress, 10);
//Thread.Sleep(sleepTime);

//Console.WriteLine($"\nFC03 - ReadHoldingRegisters: SlaveId: {unitIdentifier} StartingAddress: {startingAddress}");
//data = client.ReadHoldingRegisters(unitIdentifier, startingAddress, 10);
//Thread.Sleep(sleepTime);

//Console.WriteLine($"\nFC04 - ReadInputRegisters: SlaveId: {unitIdentifier} StartingAddress: {startingAddress}");
//data = client.ReadInputRegisters(unitIdentifier, startingAddress, 10);
//Thread.Sleep(sleepTime);

//Console.WriteLine($"\nFC05 - WriteSingleCoil: SlaveId: {unitIdentifier} StartingAddress: {startingAddress}");
//client.WriteSingleCoil(unitIdentifier, registerAddress, true); ;
//Thread.Sleep(sleepTime);

//Console.WriteLine($"\nFC15 - WriteMultipleCoils: SlaveId: {unitIdentifier} StartingAddress: {startingAddress}");
//client.WriteMultipleCoils(unitIdentifier, registerAddress, [true, false, true, false]);
//Thread.Sleep(sleepTime);

//Console.WriteLine($"\nFC06 - WriteSingleRegister: SlaveId: {unitIdentifier} StartingAddress: {startingAddress}");
//client.WriteSingleRegister(unitIdentifier, registerAddress, [0x00, 0xff]);
//Thread.Sleep(sleepTime);

//Console.WriteLine($"\nFC16 - WriteMultipleRegisters: SlaveId: {unitIdentifier} StartingAddress: {startingAddress}");
//client.WriteMultipleRegisters(unitIdentifier, registerAddress, [0x00, 0xff, 0x00, 0xff]);
//Thread.Sleep(sleepTime);

//Console.WriteLine($"\nFC23 - ReadWriteMultipleRegisters: SlaveId: {unitIdentifier} StartingAddress: {startingAddress}");
//data = client.ReadWriteMultipleRegisters(unitIdentifier, startingAddress, 10, registerAddress, [0x00, 0xff, 0x00, 0xff]);
//Thread.Sleep(sleepTime);
