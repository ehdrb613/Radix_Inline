using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO.Ports;    // SerialPort 클래스 사용을 위해서 추가
using System.IO; // MemoryStream

namespace Radix
{
    public enum EnumBaudRate
    {
        Baud1200 = 1200,
        Baud2400 = 2400,
        Baud4800 = 4800,
        Baud9600 = 9600,
        Baud19200 = 19200,
        Baud38400 = 38400,
        Baud57600 = 57600,
        Baud115200 = 115200
    }

    public struct StructRs232
    {
        public int rsPort;
        public EnumBaudRate rsBaudRate;
        public int rsDataLength;
        public Parity rsParity;
        public StopBits rsStopBits;

        public StructRs232(int port, EnumBaudRate baudRate, int dataLength, Parity parity, StopBits stopBits)
        {
            rsPort = port;
            rsBaudRate = baudRate;
            rsDataLength = dataLength;
            rsParity = parity;
            rsStopBits = stopBits;
        }
    }

    public struct StructEthernet
    {
        public string Ip;
        public int Port;

        public StructEthernet(string ip, int port)
        {
            Ip = ip;
            Port = port;
        }
    }

    class PanasonicLaser
    {
        private bool rs232Con = false;
        private bool ethernetCon = false;
        
        #region rs232 설정
        private int rsPort;
        private EnumBaudRate rsBaudRate;
        private int rsDataLength;
        private Parity rsParity;
        private StopBits rsStopBits;
        private SerialPort serialPort;    // 시리얼포트 선언
        #endregion

        #region ethernet 설정
        private string ethernetIp;
        private int ethernetPort;
        private TcpClient tcpClient = new TcpClient();
        private NetworkStream networkStream = default(NetworkStream);
        #endregion

        public PanasonicLaser(int Port, EnumBaudRate BaudRate, int DataLength, Parity Parity, StopBits StopBits)
        {
            rs232Con = true;
            ethernetCon = false;
            rsPort = Port;
            rsBaudRate = BaudRate;
            rsDataLength = DataLength;
            rsParity = Parity;
            rsStopBits = StopBits;
        }

        public PanasonicLaser(string EthernetIp, int EthernetPort)
        {
            rs232Con = false;
            ethernetCon = true;
            ethernetIp = EthernetIp;
            ethernetPort = EthernetPort;
        }

        public bool Connect()
        {
            try
            {
                if (rs232Con)
                {
                    serialPort = new SerialPort("COM" + rsPort.ToString(), (int)rsBaudRate, rsParity, rsDataLength, rsStopBits);
                    serialPort.ReadTimeout = 500;
                    serialPort.WriteTimeout = 500;
                    serialPort.Open();
                    if (!serialPort.IsOpen)
                    {
                        return false;
                    }
                }
                if (ethernetCon)
                {
                    tcpClient.Connect(ethernetIp, ethernetPort);
                    if (!tcpClient.Connected)
                    {
                        return false;
                    }
                    tcpClient.ReceiveTimeout = 500;
                    tcpClient.SendTimeout = 500;
                    networkStream = tcpClient.GetStream();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine(e.StackTrace);
            }
            return false;
        }

        private void Disconnect()
        {
            try
            {
                if (rs232Con)
                {
                    serialPort.Close();
                }
                if (ethernetCon)
                {
                    tcpClient.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine(e.StackTrace);
            }
        }

        private bool Send(string text)
        {
            try
            {
                if (rs232Con)
                {
                    serialPort.WriteLine(text);
                }
                if (ethernetCon)
                {
                    byte[] bytes = Util.StringToByteArray(text);
                    networkStream.Write(bytes, 0, bytes.Length);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine(e.StackTrace);
                return false;
            }

            return true;
        }

        private string Receive()
        {
            try
            {
                if (rs232Con)
                {
                    return serialPort.ReadLine();
                }
                if (ethernetCon)
                {
                    byte[] outbuf = new byte[1024];
                    int nbytes;
                    MemoryStream mem = new MemoryStream();
                    while ((nbytes = networkStream.Read(outbuf, 0, outbuf.Length)) > 0)
                    {
                        mem.Write(outbuf, 0, nbytes);
                    }
                    string str = mem.ToString();
                    mem.Close();
                    return str;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine(e.StackTrace);
            }

            return "";
        }


    }
}
