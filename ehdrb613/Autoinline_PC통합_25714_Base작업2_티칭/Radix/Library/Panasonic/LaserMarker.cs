using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Radix
{
    public enum enumMarkerGuideContent
    {
        Stop, // "0": 표시 정지
        Area, // "1": 마킹 영역 표시
        Object, // "2": 마킹 오브젝트 표시
        OffObject, // "3": 마킹 OFF 오브젝트 표시
        DualPointer // "4": 듀얼 포인터
    }

    public enum enumVerticalAlign
    {
        Base,
        Top,
        Center,
        Bottom
    }

    public enum enumHorizontalAlign
    {
        Left,
        Center,
        Right
    }

    public enum enumTextGap
    {
        Stick,
        Pro1,
        Pro2,
        Pro3,
        Same
    }

    public enum enumBarType
    {
        Code39 = 0,
        ITF = 1,
        Code128 = 2,
        NW7 = 3,
        JanUpc = 4,
        QRModel1 = 10,
        QRModel2 = 11,
        MicroQR = 12,
        SquareIQR = 13,
        RectangleIQR = 14,
        DataMatrix = 20,
        GS1DataMatrix = 21
    }

    public enum enumBarMode
    {
        Half,
        Full
    }

    public enum enumBarSkip
    {
        None,
        One,
        Tow
    }

    public enum enumObjectType
    {
        Char,
        Shape,
        Barcode
    }

    public enum enumDataPattern
    {
        WhiteBorder = 1,
        MarkModule = 2,
        SpaceModule = 3,
        Border = 4
    }

    public enum enumLaserStatus
    {
        BeforeInitialize,
        Idle,
        Waiting,
        Marking,
        Errored
    }


    public struct structMarkerStatus
    {
        public bool TriggerReady;
        public bool Marking;
        public bool Pumping;
        public bool ShutterOpen;
        public bool CmdReady;
        public bool Errored;

        public structMarkerStatus(bool trigger, bool marking, bool pumping, bool shutter, bool cmd, bool err)
        {
            TriggerReady = trigger;
            Marking = marking;
            Pumping = pumping;
            ShutterOpen = shutter;
            CmdReady = cmd;
            Errored = err;
        }
    }

    public class LaserMarker
    {
        private static TcpClient socketLaser = new TcpClient();
        private static NetworkStream streamLaser = default(NetworkStream);

        private string ip = "192.168.100.";
        private int port = 0;
        public bool connected = false;
        public bool cmdError = false;

        public LaserMarker()
        {

        }

        private void debug(string str)
        {
            //Util.Debug(str);
        }

        public LaserMarker(string Ip, int Port)
        {
            ip = Ip;
            port = Port;
        }

        public void Connect()
        {
            socketLaser.ReceiveTimeout = 500;
            socketLaser.SendTimeout = 500;
            try
            {
                socketLaser.Connect(ip, port);
                connected = socketLaser.Connected;
                if (!socketLaser.Connected)
                {
                    //GlobalVar.LaserStatus = enumLaserStatus.Errored;
                    //FuncWin.TopMessageBox("접속 실 패");
                    return;
                }
                streamLaser = socketLaser.GetStream();
            }
            catch (Exception ex)
            {
                //FuncWin.TopMessageBox("1서버가 실행중이 아닙니다.", "연결실패!");
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                return;
            }
            //GlobalVar.LaserStatus = enumLaserStatus.Idle;
        }

        public string LaserRequest(string cmd)
        {
            if (!socketLaser.Connected)
            {
                cmdError = true;
                return "";
            }

            try
            {
                byte[] bytes = new byte[cmd.Length + 2];
                bytes[0] = 0x02; // STX
                bytes[bytes.Length - 1] = 0x0D; //CR

                byte[] cmdBytes = Util.StringToByteArray(cmd);
                for (int i = 0; i < cmdBytes.Length; i++)
                {
                    bytes[i + 1] = cmdBytes[i];
                }

                debug("Send : " + Util.ByteArrayToString(bytes));

                streamLaser.Write(bytes, 0, bytes.Length);
                streamLaser.Flush();

                int BUFFERSIZE = socketLaser.ReceiveBufferSize;
                byte[] buffer = new byte[BUFFERSIZE];
                int rbytes = streamLaser.Read(buffer, 0, buffer.Length);

                if (rbytes < 5 ||
                    buffer[4] == 0x45) // E
                {
                    cmdError = true;
                }
                else
                {
                    cmdError = false;
                }

                string rtn = Encoding.ASCII.GetString(buffer, 0, rbytes);
                debug("Receive : " + rtn);
                return rtn;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
            return "";
        }

        public structMarkerStatus GetStatus()
        {
            structMarkerStatus status = new structMarkerStatus();

            string res = LaserRequest("STSR");
            debug("Laser : " + res);
            if (res.Length < 16)
            {
                return status;
            }
            byte[] bytes = Util.StringToByteArray(res);
            status.TriggerReady = bytes[5] == 0x31;
            status.Marking = bytes[7] == 0x31;
            status.Pumping = bytes[9] == 0x32;
            status.ShutterOpen = bytes[11] == 0x31;
            status.CmdReady = bytes[13] == 0x30;
            status.Errored = bytes[15] == 0x31;

            if (status.Errored)
            {
                //GlobalVar.LaserStatus = enumLaserStatus.Errored;
            }
            return status;
        }

        public void SetCommandReady(bool on) // 명령어 접수 허가 MKM
        {
            string cmd = "MKMS";
            if (on)
            {
                cmd += "0";
            }
            else
            {
                cmd += "1";
            }
            LaserRequest(cmd);
        }

        public void SetPumping(bool on) // 레이저 펌핑 LSR
        {
            string cmd = "LSRS";
            if (on)
            {
                cmd += "1";
            }
            else
            {
                cmd += "0";
            }
            LaserRequest(cmd);
            Util.USleep(1000);
        }

        public void SetShutter(bool on) // 셔터 개폐 SHT
        {
            string cmd = "SHTS";
            if (on)
            {
                cmd += "1";
            }
            else
            {
                cmd += "0";
            }
            LaserRequest(cmd);
        }

        public void SetTrigger() // 마킹 트리거 MRK
        {
            LaserRequest("MRKS");
        }

        public void SetGuideLaser(enumMarkerGuideContent content, int speed, double zRange) // 가이드 레이저 GID
        {
            string cmd = "GIDS";

            cmd += ((int)content).ToString() + ",";
            cmd += Util.IntToString(speed, 1) + ",";
            cmd += Util.DoubleToString(zRange, 1, 3);

            LaserRequest(cmd);
        }

        #region 문자 설정
        public void SetGlobalText(int ObjNum, string text) // (전체)문자 입력 STR
        {
            string cmd = "STRS";

            cmd += Util.IntToString(ObjNum, 4) + ",";
            cmd += text;

            LaserRequest(cmd);
        }

        public void SetText(int StrNum, string text) // (트리거마다)문자입력 SIN
        {
            string cmd = "SINS";

            cmd += Util.IntToString(StrNum, 1) + ",";
            cmd += text;

            LaserRequest(cmd);
        }

        public void SetTextCondition(int ObjectNum,
                                        double XPos,
                                        double YPos,
                                        double Height,
                                        double Width,
                                        enumHorizontalAlign HAlign,
                                        enumVerticalAlign VAlign,
                                        double BoldWidth,
                                        int FontNum,
                                        enumTextGap TextGap,
                                        int TextAlign,
                                        double Angle,
                                        double GapWidth,
                                        double LineGap,
                                        int LaserPower,
                                        int ScanSpeed) // 문자 오브젝트 조건 설정 STC
        {
            string cmd = "STCS";

            cmd += Util.IntToString(ObjectNum, 4) + ",";
            cmd += Util.DoubleToString(XPos, 1, 3) + ",";
            cmd += Util.DoubleToString(YPos, 1, 3) + ",";
            cmd += Util.DoubleToString(Height, 1, 3) + ",";
            cmd += Util.DoubleToString(Width, 1, 3) + ",";
            cmd += ((int)HAlign).ToString() + ",";
            cmd += ((int)VAlign).ToString() + ",";
            cmd += Util.DoubleToString(BoldWidth, 1, 3) + ",";
            cmd += Util.IntToString(FontNum, 1) + ",";
            cmd += ((int)TextGap).ToString() + ",";
            cmd += Util.IntToString(TextAlign, 1) + ",";
            cmd += Util.DoubleToString(Angle, 1, 3) + ",";
            cmd += Util.DoubleToString(GapWidth, 1, 3) + ",";
            cmd += Util.DoubleToString(LineGap, 1, 3) + ",";
            cmd += Util.DoubleToString(LaserPower, 1, 3) + ",";
            cmd += Util.DoubleToString(ScanSpeed, 1, 3);

            LaserRequest(cmd);
        }
        #endregion

        public void SetDataMatrix(int ObjectNum, 
                                    double XPos, 
                                    double YPos, 
                                    double Angle, 
                                    enumBarType CodeType,
                                    double ModuleHeight,
                                    double ModuleWidth,
                                    enumBarMode Mode,
                                    int HModule,
                                    int VModule,
                                    bool VisibleChar,
                                    bool Direction,
                                    enumBarSkip Skip) // 바코드 오브젝트 조건 BRF
        {
            string cmd = "BRFS";

            cmd += Util.IntToString(ObjectNum, 4) + ",";
            cmd += Util.DoubleToString(XPos, 1, 3) + ",";
            cmd += Util.DoubleToString(YPos, 1, 3) + ",";
            cmd += Util.DoubleToString(Angle, 1, 3) + ",";
            cmd += Util.IntToString((int)CodeType, 1) + ",";
            cmd += Util.DoubleToString(ModuleHeight, 1, 3) + ",";
            cmd += Util.DoubleToString(ModuleWidth, 1, 3) + ",";
            cmd += ((int)Mode).ToString() + ",";
            cmd += Util.IntToString(HModule, 1) + ",";
            cmd += Util.IntToString(VModule, 1) + ",";
            if (VisibleChar)
            {
                cmd += "1,";
            }
            else
            {
                cmd += "0,";
            }
            if (Direction)
            {
                cmd += "1,";
            }
            else
            {
                cmd += "0,";
            }
            cmd += ((int)Skip).ToString();

            LaserRequest(cmd);
        }


        public void CreateObject(enumObjectType ObjectType, int ObjectNum, int ObjectGroup) // 오브젝트 생성 CRE
        {
            string cmd = "CRES";

            cmd += ((int)ObjectType).ToString() + ",";
            cmd += Util.IntToString(ObjectNum, 4) + ",";
            cmd += Util.IntToString(ObjectGroup, 1);

            LaserRequest(cmd);
        }

        public void DeleteObject(int ObjectNum) // 오브젝트 삭제 DEL
        {
            string cmd = "DELS";

            cmd += Util.IntToString(ObjectNum, 4);
    
            LaserRequest(cmd);
        }

        public void SetMarkObject(bool ObjectGroup, int ObjectNum, bool Mark) // 오브젝트 마킹지정 ENA
        {
            string cmd = "ENAS";

            if (ObjectGroup)
            {
                cmd += "1,";
            }
            else
            {
                cmd += "0,";
            }
            cmd += Util.IntToString(ObjectNum, 4);
            if (Mark)
            {
                cmd += ",1";
            }
            else
            {
                cmd += ",0";
            }

            LaserRequest(cmd);
        }

        public void Set2DCondition(int ObjectNum,
                                    enumDataPattern Pattern, 
                                    bool MarkingOn,
                                    int LaserPower,
                                    int ScanSpeed) // 2D 코드 패턴별 레이저 조건 BRP
        {
            string cmd = "BRPS";
            cmd += Util.IntToString(ObjectNum, 4) + ",";
            cmd += ((int)Pattern).ToString() + ",";
            if (MarkingOn)
            {
                cmd += "1,";
            }
            else
            {
                cmd += "0,";
            }
            cmd += Util.IntToString(LaserPower, 1) + ",";
            cmd += Util.IntToString(ScanSpeed, 1);

            LaserRequest(cmd);
        }

        public void SetCondition(double LaserPower,
                                    int ScanSpeed,
                                    int LaserPrequency) // 레이저 조건 LSC
        {
            string cmd = "LSCS";
            cmd += Util.DoubleToString(LaserPower, 1, 1) + ",";
            cmd += Util.IntToString(ScanSpeed, 1) + ",";
            cmd += Util.IntToString(LaserPrequency, 1);

            LaserRequest(cmd);
        }

        public void SetFile(int FileNum) // 파일 전환 FNO
        {
            string cmd = "FNOS";
            cmd += Util.IntToString(FileNum, 1);

            LaserRequest(cmd);
        }

        public int GetError() // 에러코드 ENO
        {
            string res = LaserRequest("ENOS");
            if (res.Length < 8)
            {
                return 0;
            }
            try
            {
                return int.Parse(res.Substring(5, 3));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }

            return 0;
        }

        public void AlarmReset()
        {
            LaserRequest("ARSS");
        }

        public bool Init() // 접속 및 초기 동작부터 펌핑까지
        {
            ulong startTime = GlobalVar.TickCount64;
            while (!connected ||
                    !socketLaser.Connected)
            {
                if (GlobalVar.TickCount64 - startTime > 10000)
                {
                    debug("Laser connect fail");
                    //GlobalVar.LaserStatus = enumLaserStatus.Errored;
                    return false;
                }
                Thread.Sleep(100);
                Connect();
            }

            structMarkerStatus status = GetStatus();
            if (status.Errored)
            {
                AlarmReset();
                status = GetStatus();
            }

            startTime = GlobalVar.TickCount64;
            while (!status.Pumping)
            {
                if (GlobalVar.TickCount64 - startTime > 10000)
                {
                    debug("Pump Fail");
                    //GlobalVar.LaserStatus = enumLaserStatus.Errored;
                    return false;
                }
                SetPumping(true);
                Thread.Sleep(100);
                status = GetStatus();
            }

            status = GetStatus();
            if (status.Errored)
            {
                AlarmReset();
                status = GetStatus();
            }

            SetFile(1);
            status = GetStatus();
            if (status.Errored)
            {
                AlarmReset();
                status = GetStatus();
            }

            startTime = GlobalVar.TickCount64;
            while (!status.ShutterOpen)
            {
                if (GlobalVar.TickCount64 - startTime > 10000)
                {
                    debug("shutter fail");
                    //GlobalVar.LaserStatus = enumLaserStatus.Errored;
                    return false;
                }
                SetShutter(true);
                Thread.Sleep(100);
                status = GetStatus();
            }

            status = GetStatus();
            if (status.Errored)
            {
                AlarmReset();
                status = GetStatus();
            }

            if (status.Errored ||
                !status.Pumping ||
                !status.ShutterOpen)
            {
                debug("errored : " + status.Errored);
                debug("puming : " + status.Pumping);
                debug("shutter : " + status.ShutterOpen);
                //GlobalVar.LaserStatus = enumLaserStatus.Errored;
                return false;
            }

            //GlobalVar.LaserStatus = enumLaserStatus.Waiting;
            return true;
        }

        public bool ChangeObject(bool Mark2D, string Text, double Size, int ModuleCount, int LaserPower, int ScanSpeed)
        {
            // ObjNum은 2D : 1, Text : 2 로 고정
            if (!Init())
            {
                return false;
            }

            structMarkerStatus status = GetStatus();
            if (status.ShutterOpen)
            {
                SetShutter(false);
                status = GetStatus();
            }

            if (status.Errored ||
                 cmdError ||
                 status.ShutterOpen)
            {
                return false;
            }

            if (Mark2D)
            {
                SetDataMatrix(1, 0, 0, 0, enumBarType.DataMatrix, Size / ModuleCount, Size / ModuleCount, enumBarMode.Full, ModuleCount, ModuleCount, false, false, enumBarSkip.None);
                SetMarkObject(false, 1, true);
                SetMarkObject(false, 2, false);
            }
            else
            {
                SetTextCondition(2, 0, 0, Size, Size, enumHorizontalAlign.Center, enumVerticalAlign.Center, 2, 0, enumTextGap.Same, 0, 0, 0, 0, LaserPower, ScanSpeed);
                SetMarkObject(false, 1, false);
                SetMarkObject(false, 2, true);
            }
            status = GetStatus();
            if (status.Errored ||
                    cmdError)
            {
                return false;
            }

            if (!status.ShutterOpen)
            {
                SetShutter(true);
                status = GetStatus();
            }

            if (status.Errored ||
                 cmdError ||
                 !status.ShutterOpen)
            {
                return false;
            }

            return true;
        }
    }
}
