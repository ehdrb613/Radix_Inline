using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using Modbus.Device;
using System.Threading;

namespace Radix
{
    public class SerialCom
    {
        /**
        * RS232/RS485 일반프로토콜 & RS485 ModBusRTU 동시처리 클래스
        * RTU 라이브러리상에서는 일반 프로토콜 해석이 안 되므로
        * RTU로 연결할 때는 일회성으로 연결, 데이터 처리 후 바로 연결을 해제한다.
        * RTU 모드가 아닌 항목들 처리할 때는 연결을 유지하고 처리가 끝나고 해제하면 된다.
        * RTU 모드 없이 일반 프로토콜로만 사용할 때는 연결을 해제할 필요가 없다.
        * 1. new SerialCom(연결 속성)
        * 2. RTU 모드 지령 (Connect, Disconnect 포함)
        */

        public bool ConstantConnection = false; // RTU 연결을 유지할지 여부. RTU만 쓸 경우는 true, 혼용일 경우는 false로 한다. RTU 쓰지 않는 경우는 외부에서 Connect/Disconnect 하므로 상관 없다.
        SerialPort _port;
        ModbusSerialMaster _master;
        public Exception exception = null;
        public bool Connected = false;
        public bool IsRTU = false;
        public string ReceiveBuffer = ""; // 불완전한 메시지 확인용
        public string ReceiveHex = ""; // 불완전한 메시지 확인용 (Hexa 문자열)

        // 연결 속성
        string Port;
        int BaudRate;
        Parity Parity;
        int DataBits;
        StopBits StopBits;

        private void debug(string str)
        {
            //Util.Debug("SerialCom : " + str);
        }

        // 생성자
        public SerialCom()
        {
            Port = "COM1";
            BaudRate = 9600;
            Parity = Parity.None;
            DataBits = 8;
            StopBits = StopBits.One;
            IsRTU = true;
        }

        public SerialCom(string port, int baudRate)
        {
            Port = port;
            BaudRate = baudRate;
            Parity = Parity.None;
            DataBits = 8;
            StopBits = StopBits.One;
            IsRTU = true;
        }

        public SerialCom(string port, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            Port = port;
            BaudRate = baudRate;
            Parity = parity;
            DataBits = dataBits;
            StopBits = stopBits;
        }

        // 소멸자
        ~SerialCom()
        {
            Disconnect();
        }

        // 연결 속성 지정
        public void SetConnection(string port, int baudRate, bool rtu)
        {
            Port = port;
            BaudRate = baudRate;
            IsRTU = rtu;
        }

        public void SetConnection(string port, int baudRate, Parity parity, int dataBits, StopBits stopBits, bool rtu)
        {
            Port = port;
            BaudRate = baudRate;
            Parity = parity;
            DataBits = dataBits;
            StopBits = stopBits;
            IsRTU = rtu;
        }

        // 기존 세팅된 연결속성으로 연결
        public bool Connect(bool rtu)
        {
            try
            {
                // RTU 속성과 같이 연결되어 있는 상태시 그대로
                if (IsRTU == rtu &&
                    _port != null &&
                    _port.IsOpen)
                {
                    return true;
                }

                // RTU 속성 다르게 연결되어 있다면 연결 해제
                if (IsRTU != rtu &&
                    _port != null &&
                    _port.IsOpen)
                {
                    Disconnect();
                }

                IsRTU = rtu;
                _port = new SerialPort(Port, BaudRate, Parity, DataBits, StopBits);
                _port.ReadTimeout = 100;
                _port.WriteTimeout = 100;
                _port.Open();

                if (rtu)
                {
                    _master = ModbusSerialMaster.CreateRtu(_port);
                }
                else
                {
                    _port.DataReceived += DataReceived;
                }
                exception = null;
            }
            catch (Exception ex)
            {
                //exception = ex;
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
            Connected = _port.IsOpen;
            return _port.IsOpen;
        }

        // 세팅된 정보로 연결
        public bool Connect()
        {
            try
            {
                // 연결되어 있다면 연결 해제
                if (_port != null &&
                    _port.IsOpen)
                {
                    Disconnect();
                }

                _port = new SerialPort(Port, BaudRate);
                _port.ReadTimeout = 100;
                _port.WriteTimeout = 100;
                _port.Open();

                if (IsRTU)
                {
                    _master = ModbusSerialMaster.CreateRtu(_port);
                }
                else
                {
                    _port.DataReceived += DataReceived;
                }
                exception = null;
            }
            catch (Exception ex)
            {
                //exception = ex;
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
            Connected = _port.IsOpen;
            return _port.IsOpen;
        }

        // 포트와 속도로 연결
        public bool Connect(string port, int baudRate, bool rtu)
        {
            try
            {
                // RTU 속성과 같이 연결되어 있는 상태시 그대로
                if (IsRTU == rtu &&
                    _port != null &&
                    _port.IsOpen)
                {
                    return true;
                }

                // RTU 속성 다르게 연결되어 있다면 연결 해제
                if (IsRTU != rtu &&
                    _port != null &&
                    _port.IsOpen)
                {
                    Disconnect();
                }

                IsRTU = rtu;
                _port = new SerialPort(port, baudRate);
                _port.ReadTimeout = 100;
                _port.WriteTimeout = 100;
                _port.Open();

                if (rtu)
                {
                    _master = ModbusSerialMaster.CreateRtu(_port);
                }
                else
                {
                    _port.DataReceived += DataReceived;
                }
                exception = null;
            }
            catch (Exception ex)
            {
                //exception = ex;
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
            Connected = _port.IsOpen;
            return _port.IsOpen;
        }

        // 세부 연결속성 전체로 연결
        public bool Connect(string port, int baudRate, Parity parity, int dataBits, StopBits stopBits, bool rtu)
        {
            try
            {
                // RTU 속성과 같이 연결되어 있는 상태시 그대로
                if (IsRTU == rtu &&
                    _port != null &&
                    _port.IsOpen)
                {
                    return true;
                }

                // RTU 속성 다르게 연결되어 있다면 연결 해제
                if (IsRTU != rtu &&
                    _port != null &&
                    _port.IsOpen)
                {
                    Disconnect();
                }

                IsRTU = rtu;
                _port = new SerialPort(port, baudRate, parity, dataBits, stopBits);
                _port.ReadTimeout = 100;
                _port.WriteTimeout = 100;
                _port.Open();

                if (rtu)
                {
                    _master = ModbusSerialMaster.CreateRtu(_port);
                }
                else
                {
                    _port.DataReceived += DataReceived;
                }
                exception = null;
            }
            catch (Exception ex)
            {
                //exception = ex;
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
            Connected = _port.IsOpen;
            return _port.IsOpen;
        }

        // 연결 해제
        public void Disconnect()
        {
            // Destroy serial port
            if (_port != null)
            {
                if (_port.IsOpen)
                {
                    if (IsRTU)
                    {
                        try
                        {
                            // Destroy Modbus master
                            if (_master != null)
                            {
                                _master.Dispose();
                            }
                            _master = null;
                            exception = null;
                        }
                        catch (Exception ex)
                        {
                            //exception = ex;
                            //debug(ex.ToString());
                            //debug(ex.StackTrace);
                        }
                    }
                    else
                    {
                        _port.DataReceived -= DataReceived;
                    }
                    _port.Close();
                }
                _port.Dispose();
                _port = null;
            }
            Connected = false;
        }

        // bit 여러 개 읽기
        public bool[] ReadCoils(byte slaveNo, ushort startCoil, ushort count)
        {
            try
            {
                bool[] coils = null;
                //if (Connect(true))
                //{
                if (_master != null)
                {
                    coils = _master.ReadCoils(slaveNo, startCoil, count);
                }
                //if (!ConstantConnection)
                //{
                //Disconnect();
                //}
                //}
                exception = null;
                return coils;
            }
            catch (Exception ex)
            {
                //exception = ex;
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
            return null;
        }

        // bit값 하나 쓰기
        public void WriteSingleCoil(byte slaveNo, ushort coilNo, bool val)
        {
            try
            {
                //if (Connect(true))
                //{
                if (_master != null)
                {
                    _master.WriteSingleCoil(slaveNo, coilNo, val);
                }
                //if (!ConstantConnection)
                //{
                //Disconnect();
                //}
                //}
                exception = null;
            }
            catch (Exception ex)
            {
                //exception = ex;
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        // bit값 여러개 쓰기
        public void WriteMultipleCoils(byte slaveNo, ushort startCoil, bool[] val)
        {
            try
            {
                //if (Connect(true))
                //{
                if (_master != null)
                {
                    _master.WriteMultipleCoils(slaveNo, startCoil, val);
                }
                //if (!ConstantConnection)
                //{
                //Disconnect();
                //}
                //}
                exception = null;
            }
            catch (Exception ex)
            {
                //exception = ex;
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        // word 여러 개 읽기
        public ushort[] ReadHoldingRegisters(byte slaveNo, ushort startCoil, ushort count)
        {
            try
            {
                ushort[] registers = null;
                //if (Connect(true))
                //{
                if (_master != null)
                {
                    //Thread.Sleep(50);
                    registers = _master.ReadHoldingRegisters(slaveNo, startCoil, count);
                }
                //if (!ConstantConnection)
                //{
                //Disconnect();
                //}
                //}
                exception = null;
                return registers;
            }
            catch (Exception ex)
            {
                //exception = ex;
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
            return null;
        }

        public ushort[] ReadInputRegisters(byte slaveNo, ushort startCoil, ushort count)
        {
            try
            {
                ushort[] registers = null;
                if (_master != null)
                {
                    registers = _master.ReadInputRegisters(slaveNo, startCoil, count);
                }
                exception = null;
                return registers;
            }
            catch (Exception ex)
            {
                //exception = ex;
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
            return null;
        }

        public void WriteSingleRegister(byte slaveNo, ushort startRegister, ushort val)
        {
            try
            {
                //if (Connect(true))
                //{
                if (_master != null)
                {
                    _master.WriteSingleRegister(slaveNo, startRegister, val);
                }
                //if (!ConstantConnection)
                //{
                //Disconnect();
                //}
                //}
                exception = null;
            }
            catch (Exception ex)
            {
                //exception = ex;
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        // work값 여러개 쓰기
        public void WriteMultipleRegisters(byte slaveNo, ushort startCoil, ushort[] val)
        {
            try
            {
                //if (Connect(true))
                //{
                if (_master != null)
                {
                    _master.WriteMultipleRegisters(slaveNo, startCoil, val);
                }
                //if (!ConstantConnection)
                //{
                //Disconnect();
                //}
                //}
                exception = null;
            }
            catch (Exception ex)
            {
                //exception = ex;
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        public void SendMessage(string msg)
        {
            try
            {
                if (_port.IsOpen)
                {
                    _port.Write(msg);
                }
                exception = null;
            }
            catch (Exception ex)
            {
                //exception = ex;
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        public bool Send(byte[] buffer)
        {
            try
            {
                // Send the hex data
                _port.Write(buffer, 0, buffer.Length);


            }
            catch (Exception ex)
            {
                //exception = ex;
                //debug(ex.ToString());
                //debug(ex.StackTrace);
                return false;
            }
            return true;
        }

        public void DataReceived(object sender, SerialDataReceivedEventArgs e) // 데이터 수신시
        {
            try
            {
                while (_port.BytesToRead != 0)
                {
                    int rByte = _port.ReadByte();
                    ReceiveHex += rByte.ToString("X2");
                    char[] tByte = new char[1];
                    tByte[0] = (char)rByte;
                    ReceiveBuffer += Util.ByteArrayToString(Encoding.ASCII.GetBytes(tByte));
                }
            }
            catch (Exception ex)
            {
                //exception = ex;
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        public void ClearException()
        {
            exception = null;
        }

        public void ClearBuffer()
        {
            ReceiveBuffer = "";
            ReceiveHex = "";
        }
        //Register를 32비트Float로 변환하는 메서드
        public float ConvertRegisterToFloat(ushort lowerRegister, ushort upperRegister)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)(lowerRegister & 0xFF);       // 하위 레지스터의 하위 바이트
            bytes[1] = (byte)((lowerRegister >> 8) & 0xFF); // 하위 레지스터의 상위 바이트
            bytes[2] = (byte)(upperRegister & 0xFF);       // 상위 레지스터의 하위 바이트
            bytes[3] = (byte)((upperRegister >> 8) & 0xFF); // 상위 레지스터의 상위 바이트

            // IEEE 754 Float로 변환
            return BitConverter.ToSingle(bytes, 0);
        }
    }
}
