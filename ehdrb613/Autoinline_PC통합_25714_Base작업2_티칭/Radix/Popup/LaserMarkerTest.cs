using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;


namespace Radix
{
    public partial class LaserMarkerTest : Form
    {
        private static TcpClient socketLaser = new TcpClient();
        private static NetworkStream streamLaser = default(NetworkStream);


        public LaserMarkerTest()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            /*
            GlobalVar.Laser = new LaserMarker(tbIp.Text, (int)numPort.Value);
            GlobalVar.Laser.Connect();
            cbConnnected.Checked = GlobalVar.Laser.connected;
            //*/
            /*
            socketLaser.ReceiveTimeout = 500;
            socketLaser.SendTimeout = 500;
            try
            {
                socketLaser.Connect(tbIp.Text, (int)numPort.Value);
                cbConnnected.Checked = socketLaser.Connected;
                if (!socketLaser.Connected)
                {
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
            }
            //*/
        }

        /*
         * 상기 명령어의 경우에는 아래와 같이 체크섬의 값을 산출합니다.
            시작 코드부터 체크섬 앞에 있는 데이터까지를 ASCII 코드표에 따라 16진수화하여 가산합니다. 합계한 값의 하위 2자리를 ASCII 코드 2문자로 변환한 값이 체크섬입니다.
            •
            02(HEX)＋ 46(HEX)＋ 4E(HEX)＋ 4F(HEX)＋ 53(HEX)＋ 39(HEX)＋ 39(HEX)＋ 39(HEX)＋ 39(HEX)＝21C(HEX)
            •
            하위 2자리 문자(하위 1Byte) 1C(HEX)를 ASCII 코드 2문자로 변환하면 31 43(HEX)
            //*/
        public byte[] GetMarkerCheckSum(byte[] bytes)
        {
            int sum = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                sum += bytes[i];
            }
            sum = sum % (16 * 16);
            byte[] rbyte = new byte[2];
            rbyte[0] = (byte)(sum / 16);
            rbyte[1] = (byte)(sum % 16);


            return rbyte;
        }

        public string LaserRequest(string cmd)
        {
            byte[] bytes = new byte[cmd.Length + 2];
            bytes[0] = 0x02; // STX
            bytes[bytes.Length - 1] = 0x0D; //CR

            byte[] cmdBytes = Util.StringToByteArray(cmd);
            for (int i = 0; i < cmdBytes.Length; i++)
            {
                bytes[i + 1] = cmdBytes[i];
            }

            string str = Util.ByteArrayToString(bytes);
            tbSend.Text = str;

            streamLaser.Write(bytes, 0, bytes.Length);
            streamLaser.Flush();

            int BUFFERSIZE = socketLaser.ReceiveBufferSize;
            byte[] buffer = new byte[BUFFERSIZE];
            int rbytes = streamLaser.Read(buffer, 0, buffer.Length);

            return Encoding.ASCII.GetString(buffer, 0, rbytes);
        }

        private void btnSTS_Click(object sender, EventArgs e)
        {
            /*
            structMarkerStatus status = GlobalVar.Laser.GetStatus();
            cbCmdError.Checked = GlobalVar.Laser.cmdError;
            cbTriggerReady.Checked = status.TriggerReady;
            cbMarking.Checked = status.Marking;
            cbPumping.Checked = status.Pumping;
            cbShutterOpen.Checked = status.ShutterOpen;
            cbCmdReady.Checked = status.CmdReady;
            cbErrored.Checked = status.Errored;

            if (status.Errored)
            {
                numErrorCode.Value = GlobalVar.Laser.GetError();
            }
            else
            {
                numErrorCode.Value = 0;
            }
            //*/

            /*
            byte[] bytes = new byte[6];
            bytes[0] = 0x02; // STX
            bytes[1] = 0x53; // S
            bytes[2] = 0x54; // T
            bytes[3] = 0x53; // S
            bytes[4] = 0x52; // R
            bytes[5] = 0x0D; // CR

            string str = Util.ByteArrayToString(bytes);
            tbSend.Text = str;

            streamLaser.Write(bytes, 0, bytes.Length);
            streamLaser.Flush();


            int BUFFERSIZE = socketLaser.ReceiveBufferSize;
            byte[] buffer = new byte[BUFFERSIZE];
            int rbytes = streamLaser.Read(buffer, 0, buffer.Length);

            string message = Encoding.ASCII.GetString(buffer, 0, rbytes);
            tbReceive.Text = message;
            //*/
        }

        private void btnLSROn_Click(object sender, EventArgs e)
        {
            /*
            GlobalVar.Laser.SetPumping(true);
            cbCmdError.Checked = GlobalVar.Laser.cmdError;
            //*/
            /*
            byte[] bytes = new byte[7];
            bytes[0] = 0x02; // STX
            bytes[1] = 0x4C; // L
            bytes[2] = 0x53; // S
            bytes[3] = 0x52; // R
            bytes[4] = 0x53; // S
            bytes[5] = 0x31; // ON
            bytes[6] = 0x0D; // CR

            string str = Util.ByteArrayToString(bytes);
            tbSend.Text = str;

            streamLaser.Write(bytes, 0, bytes.Length);
            streamLaser.Flush();


            int BUFFERSIZE = socketLaser.ReceiveBufferSize;
            byte[] buffer = new byte[BUFFERSIZE];
            int rbytes = streamLaser.Read(buffer, 0, buffer.Length);

            string message = Encoding.ASCII.GetString(buffer, 0, rbytes);
            tbReceive.Text = message;
            //*/
        }

        private void btnLSROff_Click(object sender, EventArgs e)
        {
            /*
            GlobalVar.Laser.SetPumping(false);
            cbCmdError.Checked = GlobalVar.Laser.cmdError;
            //*/

            /*
            byte[] bytes = new byte[7];
            bytes[0] = 0x02; // STX
            bytes[1] = 0x4C; // L
            bytes[2] = 0x53; // S
            bytes[3] = 0x52; // R
            bytes[4] = 0x53; // S
            bytes[5] = 0x30; // Off
            bytes[6] = 0x0D; // CR

            string str = Util.ByteArrayToString(bytes);
            tbSend.Text = str;

            streamLaser.Write(bytes, 0, bytes.Length);
            streamLaser.Flush();


            int BUFFERSIZE = socketLaser.ReceiveBufferSize;
            byte[] buffer = new byte[BUFFERSIZE];
            int rbytes = streamLaser.Read(buffer, 0, buffer.Length);

            string message = Encoding.ASCII.GetString(buffer, 0, rbytes);
            tbReceive.Text = message;
            //*/
        }

        private void btnSHTOpen_Click(object sender, EventArgs e)
        {
            /*
            GlobalVar.Laser.SetShutter(true);
            cbCmdError.Checked = GlobalVar.Laser.cmdError;
            //*/

            /*
            byte[] bytes = new byte[7];
            bytes[0] = 0x02; // STX
            bytes[1] = 0x53; // S
            bytes[2] = 0x48; // H
            bytes[3] = 0x54; // T
            bytes[4] = 0x53; // S
            bytes[5] = 0x31; // ON
            bytes[6] = 0x0D; // CR

            string str = Util.ByteArrayToString(bytes);
            tbSend.Text = str;

            streamLaser.Write(bytes, 0, bytes.Length);
            streamLaser.Flush();


            int BUFFERSIZE = socketLaser.ReceiveBufferSize;
            byte[] buffer = new byte[BUFFERSIZE];
            int rbytes = streamLaser.Read(buffer, 0, buffer.Length);

            string message = Encoding.ASCII.GetString(buffer, 0, rbytes);
            tbReceive.Text = message;
            //*/
        }

        private void btnSHTClose_Click(object sender, EventArgs e)
        {
            /*
            GlobalVar.Laser.SetShutter(false);
            cbCmdError.Checked = GlobalVar.Laser.cmdError;
            //*/

            /*
            byte[] bytes = new byte[7];
            bytes[0] = 0x02; // STX
            bytes[1] = 0x53; // S
            bytes[2] = 0x48; // H
            bytes[3] = 0x54; // T
            bytes[4] = 0x53; // S
            bytes[5] = 0x30; // Off
            bytes[6] = 0x0D; // CR

            string str = Util.ByteArrayToString(bytes);
            tbSend.Text = str;

            streamLaser.Write(bytes, 0, bytes.Length);
            streamLaser.Flush();


            int BUFFERSIZE = socketLaser.ReceiveBufferSize;
            byte[] buffer = new byte[BUFFERSIZE];
            int rbytes = streamLaser.Read(buffer, 0, buffer.Length);

            string message = Encoding.ASCII.GetString(buffer, 0, rbytes);
            tbReceive.Text = message;
            //*/
        }

        private void btnMKMOn_Click(object sender, EventArgs e)
        {
            /*
            GlobalVar.Laser.SetCommandReady(true);
            cbCmdError.Checked = GlobalVar.Laser.cmdError;
            //*/
        }

        private void btnMKMOff_Click(object sender, EventArgs e)
        {
            /*
            GlobalVar.Laser.SetCommandReady(false);
            cbCmdError.Checked = GlobalVar.Laser.cmdError;
            //*/
        }

        private void btnMRK_Click(object sender, EventArgs e)
        {
            /*
            GlobalVar.Laser.SetTrigger();
            cbCmdError.Checked = GlobalVar.Laser.cmdError;
            //*/
        }

        private void btnGID_Click(object sender, EventArgs e)
        {
            /*
            GlobalVar.Laser.SetGuideLaser((enumMarkerGuideContent)cmbGIDContent.SelectedIndex, (int)numGIDSpeed.Value, (double)numGIDRange.Value);
            cbCmdError.Checked = GlobalVar.Laser.cmdError;
            //*/
        }

        private void LaserMarkerTest_Shown(object sender, EventArgs e)
        {
            /*
            Stop, // "0": 표시 정지
        Area, // "1": 마킹 영역 표시
        Object, // "2": 마킹 오브젝트 표시
        OffObject, // "3": 마킹 OFF 오브젝트 표시
        DualPointer // "4": 듀얼 포인터
        //*/
            cmbGIDContent.Items.Clear();
            cmbGIDContent.Items.Add("Stop");
            cmbGIDContent.Items.Add("Area");
            cmbGIDContent.Items.Add("Object");
            cmbGIDContent.Items.Add("OffObject");
            cmbGIDContent.Items.Add("DualPointer");


            /*
             *         Left,
        Center,
        Right
             */
            cmbTextConditionHAlign.Items.Clear();
            cmbTextConditionHAlign.Items.Add("Left");
            cmbTextConditionHAlign.Items.Add("Center");
            cmbTextConditionHAlign.Items.Add("Right");


            /*
             *         Base,
        Top,
        Center,
        Bottom
             */
            cmbTextConditionVAlign.Items.Clear();
            cmbTextConditionVAlign.Items.Add("Base");
            cmbTextConditionVAlign.Items.Add("Top");
            cmbTextConditionVAlign.Items.Add("Center");
            cmbTextConditionVAlign.Items.Add("Bottom");


            /*
             *   Stick,
        Pro1,
        Pro2,
        Pro3,
        Same
             */
            cmbTextConditionGap.Items.Clear();
            cmbTextConditionGap.Items.Add("Stick");
            cmbTextConditionGap.Items.Add("Pro1");
            cmbTextConditionGap.Items.Add("Pro2");
            cmbTextConditionGap.Items.Add("Pro3");
            cmbTextConditionGap.Items.Add("Same");
        }

        private void btnGlobalText_Click(object sender, EventArgs e)
        {
            /*
            GlobalVar.Laser.SetGlobalText((int)numGlobalTextObjNum.Value, tbGlobalText.Text);
            cbCmdError.Checked = GlobalVar.Laser.cmdError;
            //*/
        }

        private void btnTriggerText_Click(object sender, EventArgs e)
        {
            /*
            GlobalVar.Laser.SetText((int)numStrNum.Value, tbTriggerText.Text);
            cbCmdError.Checked = GlobalVar.Laser.cmdError;
            //*/
        }

        private void btnTextCondition_Click(object sender, EventArgs e)
        {
            /*
            GlobalVar.Laser.SetTextCondition((int)numTextConditionObjNum.Value, 
                                                (double)numTextConditionX.Value,
                                                (double)numTextConditionY.Value,
                                                (double)numTextConditionH.Value,
                                                (double)numTextConditionW.Value,
                                                (enumHorizontalAlign)cmbTextConditionHAlign.SelectedIndex,
                                                (enumVerticalAlign)cmbTextConditionVAlign.SelectedIndex,
                                                (double)numTextConditionBoldWidth.Value,
                                                (int)numTextConditionFontNum.Value,
                                                (enumTextGap)cmbTextConditionGap.SelectedIndex,
                                                (int)numTextConditionCharAlign.Value,
                                                (double)numTextAlignAngle.Value,
                                                (double)numTextConditionGapWidth.Value,
                                                (double)numTextConditionLineHeight.Value,
                                                (int)numTextConditionLaserPower.Value,
                                                (int)numTextConditionSpeed.Value);
            cbCmdError.Checked = GlobalVar.Laser.cmdError;
            //*/
        }

        private void btnBarCondition_Click(object sender, EventArgs e)
        {
            int barTypeIndex = cmbBarConditionType.SelectedIndex;
            switch (barTypeIndex)
            {
                case 11:
                    barTypeIndex = 21;
                    break;
                case 10:
                    barTypeIndex = 20;
                    break;
                case 9:
                    barTypeIndex = 14;
                    break;
                case 8:
                    barTypeIndex = 13;
                    break;
                case 7:
                    barTypeIndex = 12;
                    break;
                case 6:
                    barTypeIndex = 10;
                    break;
                case 5:
                    barTypeIndex = 4;
                    break;
                case 4:
                    barTypeIndex = 3;
                    break;
                case 3:
                    barTypeIndex = 2;
                    break;
                case 2:
                    barTypeIndex = 1;
                    break;
                case 1:
                default:
                    barTypeIndex = 0;
                    break;
            }
            /*
            GlobalVar.Laser.SetDataMatrix((int)numBarCodeObjNum.Value,
                                                (double)numBarConditionX.Value,
                                                (double)numBarConditinY.Value,
                                                (double)numBarConditionAngle.Value,
                                                (enumBarType)barTypeIndex,
                                                (double)numBarConditionModuleHeight.Value,
                                                (double)numBarConditionModuleWidth.Value,
                                                (enumBarMode)cmbBarConditionMode.SelectedIndex,
                                                (int)numBarConditionHModuleCount.Value,
                                                (int)numBarConditionVModuleCount.Value,
                                                numBarConditionVisibleChar.Value == 1,
                                                numBarConditionDirection.Value == 1,
                                                (enumBarSkip)numBarConditionSkip.Value);
            cbCmdError.Checked = GlobalVar.Laser.cmdError;
            //*/
        }

        private void btnCreateObject_Click(object sender, EventArgs e)
        {
            /*
            GlobalVar.Laser.DeleteObject((int)numDelObjectNum.Value);
            cbCmdError.Checked = GlobalVar.Laser.cmdError;
            //*/
        }

        private void btnObjectMarking_Click(object sender, EventArgs e)
        {
            /*
            GlobalVar.Laser.SetMarkObject(cbMarkingGroup.Checked, (int)numMarkingObcNum.Value, cbMarkingOn.Checked);
            cbCmdError.Checked = GlobalVar.Laser.cmdError;
            //*/
        }

        private void btn2DCondition_Click(object sender, EventArgs e)
        {
            /*
            GlobalVar.Laser.Set2DCondition((int)num2DConditionObjNum.Value, 
                                            (enumDataPattern)(cmb2DConditionPattern.SelectedIndex + 1), 
                                            cb2DConditionMarking.Checked, 
                                            (int)num2DConditionPower.Value,
                                            (int)num2DConditionSpeed.Value);
            cbCmdError.Checked = GlobalVar.Laser.cmdError;
            //*/
        }

        private void btnSetCondition_Click(object sender, EventArgs e)
        {
            /*
            GlobalVar.Laser.SetCondition((double)numConditionPower.Value,
                                             (int)numConditionSpeed.Value,
                                            (int)numConditionPrequency.Value);
            cbCmdError.Checked = GlobalVar.Laser.cmdError;
            //*/
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*
            GlobalVar.Laser.AlarmReset();
            cbCmdError.Checked = GlobalVar.Laser.cmdError;
            //*/
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            //GlobalVar.Laser.SetFile((int)numFileNum.Value);
        }
    }
}
