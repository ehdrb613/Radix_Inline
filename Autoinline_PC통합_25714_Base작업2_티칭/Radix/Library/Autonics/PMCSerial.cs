using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;    // SerialPort 클래스 사용을 위해서 추가
using System.IO; // MemoryStream


namespace Radix
{
    public class PMCSerial
    {
        /*
         * 2축 제어용 autonic PMC Motion Controler rs232 통신 컨트롤 클래스
         */

        #region 접속정보
        private int rsPort;
        private EnumBaudRate rsBaudRate;
        private int rsDataLength;
        private Parity rsParity;
        private StopBits rsStopBits;
        private SerialPort serialPort;    // 시리얼포트 선언
        #endregion

        #region 상태정보
        public bool[] Stop0 = { false, false };
        public bool[] Stop1 = { false, false };
        public bool[] Stop2 = { false, false };
        public bool[] Inpos = { false, false };
        public bool[] PLimit = { false, false };
        public bool[] NLimit = { false, false };
        public bool[] Alarm = { false, false };
        public bool[] Emg = { false, false };
        public bool[] Drv = { false, false };
        public bool[] Err = { false, false };
        public bool[] Hom = { false, false };
        public bool[] Run = { false, false };
        public bool[] Pause = { false, false };
        public bool[] Out0 = { false, false };
        public bool[] Drive = { false, false };
        public bool[] Error = { false, false };
        #endregion

        public void debug(string str)
        {
            //Util.Debug(str);
        }

        public PMCSerial(int Port, EnumBaudRate BaudRate, int DataLength, Parity Parity, StopBits StopBits)
        {
            rsPort = Port;
            rsBaudRate = BaudRate;
            rsDataLength = DataLength;
            rsParity = Parity;
            rsStopBits = StopBits;
        }

        public bool Connect()
        {
            try
            {
                debug("connect");
                debug("port : COM" + rsPort.ToString());
                debug("baud rate : " + (int)rsBaudRate);
                debug("parity : " + (int)rsParity);
                debug("data length : " + rsDataLength);
                debug("stop bits : " + (int)rsStopBits);
                serialPort = new SerialPort("COM" + rsPort.ToString(), (int)rsBaudRate, rsParity, rsDataLength, rsStopBits);
                serialPort.ReadTimeout = 500;
                serialPort.WriteTimeout = 500;
                serialPort.Open();
                if (!serialPort.IsOpen)
                {
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine(e.StackTrace);
            }
            return false;
        }

    
        public void Disconnect()
        {
            try
            {
                serialPort.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine(e.StackTrace);
            }
        }

        public bool Send(string text)
        {
            try
            {
                serialPort.WriteLine(text + "\r");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine(e.StackTrace);
                return false;
            }

            return true;
        }

        public string Receive()
        {
            byte[] buffer = new byte[1024];
            try
            {
                if (serialPort.Read(buffer, 0, buffer.Length) == 0)
                {
                    return "";
                }
                return buffer.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine(e.StackTrace);
            }

            return "";
        }

        /*
         * 명령어
         * PRG : 내부 프로그램 번지 실행
         *** JOG : 연속모드 시작 
         *** PAB : 절대 위치로 드라이브
         *** PIC : 상대 위치로 드라이브
         *** CLL : 논리 위치 카운터 클리어
         *** CLR : 위치 카운터 클리어
         *** SPD : 현재 드라이브의 속도 변경 및 현재 각 축 동작 속도값 읽기
         *** POS : 현재 위치 좌표 돌려줌
         *** HOM : 원점 복귀
         *** STO : 감속정지
         * VER : 본체 버전정보
         * IDC : 동작중인 프로그램 번호
         * SSM : 축 속도 설정 1~4
         * INR : 입력 신호 및 커넥터(CN3) 신호를 bit 구성에 의한 16진수로 돌려줌
         * OUT : 출력신호 제어 16진수
         * RST : 모션컨트롤 IC 리셋
         * SCI : 통신포트의 조건을 고쳐 쓰기 또는 읽기
         *** OGE : 원점복귀 강제 종료
         * PSP : 프로그램,드라이브 스텝 일시 정지
         * EDP : 프로그램 강제 종료
         * PRS : 프로그램 다시 시작
         * PST : 프로그램 스텝 실행
         * ERD : 본체측의 에러상태
         * IHS : 2byte EEPROM 데이터 수정
         * IHR : 2byte EEPROM 데이터 읽기
         * IXS : 4byte EEPROM 데이터 수정
         * IXR : 4byte EEPROM 데이터 읽기
         */

        public void Jog(bool moveX, bool plusX, bool moveY, bool plusY)
        {
            string cmd = "JOG ";
            if (moveX)
            {
                if (plusX)
                {
                    cmd += "+";
                }
                else
                {
                    cmd += "-";
                }
                cmd += "X";
            }
            cmd += ",";
            if (moveY)
            {
                if (plusY)
                {
                    cmd += "+";
                }
                else
                {
                    cmd += "-";
                }
                cmd += "Y";
            }
            this.Send(cmd);
        }

        public void MoveAbsolute(int xPos, int yPos)
        {
            string cmd = "PAB ";
            if (xPos > 0)
            {
                cmd += xPos.ToString();
            }
            cmd += ",";
            if (yPos > 0)
            {
                cmd += yPos.ToString();
            }
            this.Send(cmd);
        }

        public void MoveRelative(int xPos, int yPos)
        {
            string cmd = "PIC ";
            if (xPos > 0)
            {
                cmd += xPos.ToString();
            }
            cmd += ",";
            if (yPos > 0)
            {
                cmd += yPos.ToString();
            }
            this.Send(cmd);
        }

        public void ClearCounter(bool x, bool y)
        {
            string cmd = "CLL ";
            if (x)
            {
                cmd += "X";
            }
            if (y)
            {
                cmd += "Y";
            }
            this.Send(cmd);
        }

        public void ClearPos(bool x, bool y)
        {
            string cmd = "CLR ";
            if (x)
            {
                cmd += "X";
            }
            if (y)
            {
                cmd += "Y";
            }
            this.Send(cmd);
        }

        public void SetSpeed(int xSpeed, int ySpeed)
        {
            /*
             * 드라이브속도 = 축속도 * 속도배율
             */
            string cmd = "SPD ";
            if (xSpeed > 0)
            {
                cmd += xSpeed.ToString();
            }
            cmd += ",";
            if (ySpeed > 0)
            {
                cmd += ySpeed.ToString();
            }
            this.Send(cmd);
        }

        public int GetSpeed(bool x)
        {
            string cmd = "SPD";
            this.Send(cmd);
            string str = this.Receive();
            if (str.Length > 4)
            {
                try
                {
                    string[] sp = str.Substring(4).Split(',');
                    if (x)
                    {
                        return int.Parse(sp[0].Trim());
                    }
                    return int.Parse(sp[1].Trim());
                    //return int.Parse(str.Substring(4));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace);
                    return 0;
                }
            }
            return 0;
        }

        public int GetPos(bool x)
        {
            string cmd = "POS";
            this.Send(cmd);
            string str = this.Receive();
            if (str.Length > 4)
            {
                try
                {
                    string[] sp = str.Substring(4).Split(',');
                    if (x)
                    {
                        return int.Parse(sp[0].Trim());
                    }
                    return int.Parse(sp[1].Trim());
                    //return int.Parse(str.Substring(4));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace);
                    return 0;
                }
            }
            return 0;
        }

        public void MoveHome(bool x, bool y)
        {
            string cmd = "HOM ";
            if (x)
            {
                cmd += "X";
            }
            if (y)
            {
                cmd += "Y";
            }
            this.Send(cmd);
        }

        public void StopMove(bool x, bool y)
        {
            string cmd = "STO ";
            if (x)
            {
                cmd += "X";
            }
            if (y)
            {
                cmd += "Y";
            }
            this.Send(cmd);
        }

        public void StopHome(bool x, bool y)
        {
            string cmd = "OGE ";
            if (x)
            {
                cmd += "X";
            }
            if (y)
            {
                cmd += "Y";
            }
            this.Send(cmd);
        }

        public void GetStatus()
        {
            string cmd = "INR";
            this.Send(cmd);
            string str = this.Receive();
            if (str.Length > 4)
            {
                try
                {
                    string[] sp = str.Substring(4).Split(',');
                    bool[] bools = new bool[16];  // Util.HexToBoolArray(str.Substring(4));

                    bools = Util.HexToBoolArray(sp[0].Trim());
                    Stop0[0] = bools[0];
                    Stop1[0] = bools[1];
                    Stop2[0] = bools[2];
                    Inpos[0] = bools[3];
                    PLimit[0] = bools[4];
                    NLimit[0] = bools[5];
                    Alarm[0] = bools[6];
                    Emg[0] = bools[7];
                    Drv[0] = bools[8];
                    Err[0] = bools[9];
                    Hom[0] = bools[10];
                    Run[0] = bools[11];
                    Pause[0] = bools[12];
                    Out0[0] = bools[13];
                    Drive[0] = bools[14];
                    Error[0] = bools[15];

                    bools = Util.HexToBoolArray(sp[1].Trim());
                    Stop0[1] = bools[0];
                    Stop1[1] = bools[1];
                    Stop2[1] = bools[2];
                    Inpos[1] = bools[3];
                    PLimit[1] = bools[4];
                    NLimit[1] = bools[5];
                    Alarm[1] = bools[6];
                    Emg[1] = bools[7];
                    Drv[1] = bools[8];
                    Err[1] = bools[9];
                    Hom[1] = bools[10];
                    Run[1] = bools[11];
                    Pause[1] = bools[12];
                    Out0[1] = bools[13];
                    Drive[1] = bools[14];
                    Error[1] = bools[15];

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.StackTrace);
                    Error[0] = true;
                    Error[1] = true;
                }
            }
        }
    }
}
