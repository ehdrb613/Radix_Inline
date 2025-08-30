using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

using System.IO;
using System.IO.Ports;
using System.Diagnostics;   // Debug.WriteLine()

namespace Radix
{
    class SimpleSerial
    {
        private SerialPort _serialPort = new SerialPort();

        public bool OpenPort( string portName, int baudRate, Parity parity = Parity.None, int databits = 8, StopBits stopbits = StopBits.One, Handshake handshake = Handshake.None )
        {
            _serialPort.PortName = portName;        // COM1
            _serialPort.BaudRate = baudRate;        // 9600
            _serialPort.Parity = parity;            // None
            _serialPort.DataBits = databits;        // 8
            _serialPort.StopBits = stopbits;        // 1
            _serialPort.Handshake = handshake;      // default is None
            // default timeout set
            _serialPort.ReadTimeout = 100;
            _serialPort.WriteTimeout = 100;

            //_serialPort.ErrorReceived += OnError;

            try
            {
                _serialPort.Open();
            }
            catch(IOException e)
            {
                Debug.WriteLine("SerialPort {0} open error : {1}", portName, e.Message);
                return false;
            }
            catch(Exception ex)
            {
                Debug.WriteLine("SerialPort {0} open error : argument err", portName);
                return false;
            }
            return true;
        }

        void OnError(object sender, SerialErrorReceivedEventArgs e)
        {
            string msg = e.ToString();
        }

        public bool SetCommunicationTimeout( int readTimeout, int writeTimeout )
        {
            _serialPort.ReadTimeout = readTimeout;
            _serialPort.WriteTimeout = writeTimeout;
            return true;
        }



        public bool ReadBytes( Byte [] buff, int size )
        {
            int readSize = 0;
            try
            {
                readSize = _serialPort.Read(buff, 0, size);
            }
            catch(Exception e)
            {
                Debug.WriteLine("_serialPort.Read() exception : {0}", e.Message);
                return false;
            }

            return true;
        }

        public bool IsOpen()
        {
            if (_serialPort.IsOpen)
            {
                if (!_serialPort.DsrHolding)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public bool WriteBytes( Byte [] buff, int size )
        {
            try
            {
                _serialPort.Write(buff, 0, size);

            }
            catch (Exception e)
            {
                Debug.WriteLine("_serialPort.Write() exception : {0}", e.Message);
                return false;
            }

            return true;
        }

        public int BytesToRead()
        {
            return _serialPort.BytesToRead;
        }

        public void ClosePort()
        {
            _serialPort.Close();
        }

        public void DisCardInBuffer()
        {
            // 시리얼 수신 버퍼를 클리어
            if (_serialPort.IsOpen)
            {
                _serialPort.DiscardInBuffer();
            }
        }
    }
}
