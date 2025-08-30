using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;

namespace Radix
{
    public class PanasonicVision
    {
        private  bool Vision_ConnectCheck1 = false;
        private  bool Vision_ConnectCheck2 = false;

        public  TcpClient Vision_clientSocket1 = new TcpClient();
        public  NetworkStream Vision_stream1 = default(NetworkStream);
        public  TcpClient Vision_clientSocket2 = new TcpClient();
        public  NetworkStream Vision_stream2 = default(NetworkStream);

        private string vision_IP = "192.168.1.6";
        private int vision_Trigger_Port = 8604;
        private int vision_Recieve_Port = 8601;

        public string vision_result = "";   // 비젼 결과값

        public string VisionResult_W;   //홀사이즈
        public string VisionResult_H;   //길이

        private string VisonType = "";

        public string VisionResult_Total;  //종합판정
        public string VisionResult_1 = "";      //노이즈 검사
        public string VisionResult_2 = "";      //하단 직선도검사
        public string VisionResult_3 = "";      //소재유무


        public PanasonicVision(string ip, int trigger_port, int recieve_port)
        {
            vision_IP = ip;
            vision_Trigger_Port = trigger_port;
            vision_Recieve_Port = recieve_port;
        }



        public bool CheckVision(string VisonTypeP)// Vision 연결 후 트리거 발송 및 데이터 수신
        {

            VisonType = VisonTypeP;
            Vision_clientSocket1.ReceiveTimeout = 500;
            Vision_clientSocket2.ReceiveTimeout = 500;
          
            #region 트리거 전송 포트
            if (Vision_ConnectCheck1 == false)
            {
                try
                {
                    Vision_clientSocket1.Connect(vision_IP, vision_Trigger_Port);
                    if (!Vision_clientSocket1.Connected)
                    {
                        FuncLog.WriteLog("Panasonic Vision Connect - 실패");
                        Vision_ConnectCheck1 = false;
                        //connectCheckTrigger = false;
                        return false;
                    }
                    else

                    {
                        Vision_ConnectCheck1 = true;
                        //connectCheckTrigger = true;
                        Vision_stream1 = Vision_clientSocket1.GetStream();
                    }
                }


                catch (Exception e2)
                {
                    FuncLog.WriteLog("Vision1(외관검사) Connection Error" + e2);
                    return false;
                }

            }
            #endregion

            #region 데이터 수신 포트
            
            if (Vision_ConnectCheck2 == false)
            {
                //MessageBox.Show("Receive 시도");
                try
                {
                    Vision_clientSocket2.Connect(vision_IP, vision_Recieve_Port);
                    if (!Vision_clientSocket2.Connected)
                    {
                        //MessageBox.Show("접속 실 패");
                        FuncLog.WriteLog("Panasonic Vision Connect - 실패");
                        Vision_ConnectCheck2 = false;
                        //connectCheckRecieve = false;
                        return false;
                    }
                    else

                    {
                        Vision_ConnectCheck2 = true;
                        Vision_stream2 = Vision_clientSocket2.GetStream();
                    }
                }
                catch (Exception e2)
                {
                    FuncLog.WriteLog("Vision2((치수검사) Connection Error" + e2);
                    return false;
                }
            }
            #endregion


            #region 데이터 송신
            byte[] buffer;
            buffer = Encoding.ASCII.GetBytes("%S76\r");

            //tbPayload.Text = Encoding.Default.GetString(buffer); // byte 형을 string 형으로 변환

            //            tbPayload.Text = buffer.ToString();

            Vision_stream1.Write(buffer, 0, buffer.Length);
            Vision_stream1.Flush();

            if (Recieve())
            {
                return true;
            }
            else
            {
                return false;
            }

            #endregion

        }

        private bool Recieve()
        {
            string message1 = string.Empty;
            if (Vision_ConnectCheck1 == false)
            {
                try
                {
                    //Vision_clientSocket1.Connect(vision_IP, vision_Trigger_Port);  // 송신후 성공여부를 수신하기 위함

                    if (!Vision_clientSocket1.Connected)
                    {
                        //MessageBox.Show("접속이 끊어짐");
                        return false;
                    }
                    else
                    {
                        Vision_ConnectCheck2 = true;
                    }
                    Vision_stream1 = Vision_clientSocket1.GetStream();
                }
                catch (Exception e2)
                {
                    FuncLog.WriteLog("Vision1(외관검사) Connection Error" + e2);
                    return false;
                }
            }

            string message2 = string.Empty;
            if (Vision_ConnectCheck2 == false)
            {
                try
                {
                    //Vision_clientSocket2.Connect(vision_IP, vision_Trigger_Port); // 송신후 성공여부를 수신하기 위함

                    if (!Vision_clientSocket2.Connected)
                    {
                        //MessageBox.Show("접속이 끊어짐");
                        return false;
                    }
                    else
                    {
                        Vision_ConnectCheck2 = true;
                    }
                    Vision_stream2 = Vision_clientSocket2.GetStream();
                }
                catch (Exception e2)
                {
                    FuncLog.WriteLog("Vision2(치수검사) Connection Error" + e2);
                    return false;
                }
            }


            Thread.Sleep(100);

            try
            {
                // 1차 8604 포트를 통해서는 요청전문에 대한 성공 실패에 대한 메세지가 표시된다.
                int BUFFERSIZE1 = Vision_clientSocket1.ReceiveBufferSize;
                byte[] buffer1 = new byte[BUFFERSIZE1];
                int bytes1 = Vision_stream1.Read(buffer1, 0, buffer1.Length);
                message1 = Encoding.ASCII.GetString(buffer1, 0, bytes1);

                if (message1.Contains("%S$52"))
                {

                    // MessageBox.Show(message1);
                    FuncLog.WriteLog("Vision1 수신메세지 - " + message1);
                }
                else
                {
                    if (message1.Contains("%S!110"))
                    {
                        FuncLog.WriteLog("Vision 상태 Error : " + "110 파라미터의 자리수가 잘못되었습니다.");
                        return false;
                    }
                    else if (message1.Contains("%S!111"))
                    {
                        FuncLog.WriteLog("Vision 상태 Error : " + "111 접수 가능한 커맨드 수의 상한을 초과하였습니다.");
                        return false;
                    }
                    else if (message1.Contains("%S!113"))
                    {
                        FuncLog.WriteLog("Vision 상태 Error : " + "113 SV가 SV Works와 접속 중이므로 실행할 수 없습니다.");
                        return false;
                    }
                    else if (message1.Contains("%S!114"))
                    {
                        FuncLog.WriteLog("Vision 상태 Error : " + "114 SV가 SV Works와 접속 중이므로 실행할 수 없습니다.");
                        return false;
                    }
                    else if (message1.Contains("%S!200"))
                    {
                        FuncLog.WriteLog("Vision 상태 Error : " + "200 SV가 공장 출하 시의 상태이므로 실행할 수 없습니다.");
                        return false;
                    }
                    else if (message1.Contains("%S!202"))
                    {
                        FuncLog.WriteLog("Vision 상태 Error : " + "202 SV가 SV Works와 접속 중이므로 실행할 수 없습니다.");
                        return false;
                    }
                    else if (message1.Contains("%S!204"))
                    {
                        FuncLog.WriteLog("Vision 상태 Error : " + "204 SV가 반복 실행 중이므로 실행할 수 없습니다.");
                        return false;
                    }
                    else if (message1.Contains("%S!251"))
                    {
                        FuncLog.WriteLog("Vision 상태 Error : " + "251 지정한 템플렛 화상이 프로젝트 데이터에 존재하지 않습니다.");
                        return false;
                    }
                    else if (message1.Contains("%S!252"))
                    {
                        FuncLog.WriteLog("Vision 상태 Error : " + "252 시스템 에러: SV의 이상으로 생각됩니다.");
                        return false;
                    }
                    else if (message1.Contains("%S!275"))
                    {
                        FuncLog.WriteLog("Vision 상태 Error : " + "275 지정한 워크 화상이 존재하지 않습니다.");
                        return false;
                    }
                    else if (message1.Contains("%S!278"))
                    {
                        FuncLog.WriteLog("Vision 상태 Error : " + "278 지정한 템플렛 화상은 사용할 수 없습니다..");
                        return false;
                    }
                    else if (message1.Contains("%S!410"))
                    {
                        FuncLog.WriteLog("Vision 상태 Error : " + "410 특징이 없으므로 템플렛 재등록이 불가능합니다..");
                        return false;
                    }
                    else if (message1.Contains("%S!435"))
                    {
                        FuncLog.WriteLog("Vision 상태 Error : " + "435 한 번도 검사를 실행하지 않았으므로 실행할 수 없습니다..");
                        return false;
                    }
                    else if (message1.Contains("%S!453"))
                    {
                        FuncLog.WriteLog("Vision 상태 Error : " + "453 템플렛 등록 시의 위치가 촬영 영역의 범위 밖이므로 템플렛을 재등록할 수 없습니다.");
                        return false;
                    }

                }

                // 2차 8601 포트를 통해서는 요청전문 결과값에 대한 메세지가 표시된다.
                int BUFFERSIZE2 = Vision_clientSocket2.ReceiveBufferSize;
                byte[] buffer2 = new byte[BUFFERSIZE2];
                int bytes2 = Vision_stream2.Read(buffer2, 0, buffer2.Length);
                message2 = Encoding.ASCII.GetString(buffer2, 0, bytes2);

                // FuncLog.WriteLog("Vision2 수신메세지 - " + message2);

                if (message2 != null)
                {
                    switch (VisonType)
                    {
                        case "Size":
                            VisionResult_W = message2.Substring(1, 2) + "." + message2.Substring(3, 2);
                            VisionResult_H = message2.Substring(5, 2) + "." + message2.Substring(7, 2);
                            break;

                        case "Print":
                            VisionResult_Total = message2.Substring(0, 1);
                            VisionResult_1 = message2.Substring(1, 1);
                            VisionResult_2 = message2.Substring(2, 1);
                            VisionResult_3 = message2.Substring(3, 1);
                            break;
                    }

                    return true;

                }
                else
                {
                    return false;
                }

            }
            catch (Exception e2)
            {
                FuncLog.WriteLog(" Recieve() 메세지에서 예외발생" + e2);
            }
            //}

            return false;

        }

    }
}
