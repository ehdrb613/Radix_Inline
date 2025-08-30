using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Radix
{
    public  class Domino_Inkjet
    {
        public  TcpClient Inkjet_clientSocket = new TcpClient();
        public  NetworkStream Inkjet_stream = default(NetworkStream);

        public  bool Inkjet_ConnectCheck = false;
        public  string Inkjet_IP = "192.168.1.171";
        public  int Inkjet_Port = 7000;
        public  string vision_result = "";

        public Domino_Inkjet()
        {
        }
        public void Send(string ModelNo, string SerialNum)
        {
            #region 데이터 송신
            byte[] buffer;
            // 잉크젯 프린트 규칙
            // 모델번호: AF+B-4012
            // F + 년월일 + 일련번호(3자리) :F230327005

            byte[] ESC_byte = { 27 }; //<ESC> 
            byte[] EOT_byte = { 4 };  //<EOT>
            //ModelNo = "AF+B-4012";
            //string ProdDate = "F" + DateTime.Now.ToString("yyMMdd");//ex)2018-08-31 09:05:21
            //SerialNum = GlobalVar.SerialNum.ToString("00#");//ex)2018-08-31 09:05:21


            InkjetConnect();

            #region 테스트 및 참고용 송신문자열
            //buffer = Encoding.ASCII.GetBytes(Encoding.UTF8.GetString(ESC_byte) + "S001200115" + Encoding.UTF8.GetString(EOT_byte)); // 간단히 테스트 할때 사용
            //buffer = Encoding.ASCII.GetBytes(Encoding.UTF8.GetString(ESC_byte) + "S001" + ModelNo + Encoding.UTF8.GetString(EOT_byte));
            // 프린터에서 LOT번호를 처리하기 위한 송신문자
            // <ESC>OM03001AF+B-4012<ESC>r1F240826<ESC>j1N03001999001YN000100000N<EOT>
            //buffer = Encoding.ASCII.GetBytes(Encoding.UTF8.GetString(ESC_byte) + "OM03001" + ModelNo +
            //         Encoding.UTF8.GetString(ESC_byte) + "r1" +
            //         ProdDate +
            //         Encoding.UTF8.GetString(ESC_byte) + "j1N03001999001YN000100000N" + Encoding.UTF8.GetString(EOT_byte));
            #endregion

            //PC에서 LOT 번호를 조합하여 날릴 경우
            buffer = Encoding.ASCII.GetBytes(Encoding.UTF8.GetString(ESC_byte) + "S001" + ModelNo +
                                             Encoding.UTF8.GetString(ESC_byte) + "r" +              //줄바꿈 "r"
                                             SerialNum + Encoding.UTF8.GetString(EOT_byte));

            //Inkjet_stream = Inkjet_clientSocket.GetStream();

            Inkjet_stream.Write(buffer, 0, buffer.Length);
            Inkjet_stream.Flush();

            //for (int i = 0; i < 3; i++)
            //{
            //if (Vision_Recieve())
            //{
            //    return;
            //}
            //    Thread.Sleep(100);
            //}

            //return false;
            #endregion




        }
       
        public bool InkjetConnect()
        {
            Inkjet_clientSocket.ReceiveTimeout = 500;
            #region 트리거 전송 포트
            if (Inkjet_ConnectCheck == false)
            {
                try
                {
                    Inkjet_clientSocket.Connect(Inkjet_IP,Inkjet_Port);

                    if (!Inkjet_clientSocket.Connected)
                    {
                        MessageBox.Show("접속 실 패");
                        return false;
                    }
                    else
                    {
                        Inkjet_ConnectCheck = true;
                        Inkjet_stream = Inkjet_clientSocket.GetStream();
                    }
                    
                }
                catch (Exception e2)
                {
                    FuncLog.WriteLog("Inkjet Connection Exception Error" + e2);
                    return false;
                }

            }

            if (Inkjet_ConnectCheck == true )
            {

                return true;
            }
            else
            {
                return false;
            }

            #endregion



        }

        #region  Vision Recieve
        public static bool Vision_Recieve()
        {
            
            return false;
        }
        #endregion


    }
}
