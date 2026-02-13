using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading;
using Radix.Library.LestExplorer;
using LetsStatusSnapshot = Radix.LetsStatusUI;

namespace Radix
{
    public partial class ServoPopUp : Form
    {
        private System.Threading.Timer timerUI; // Thread Timer
        private bool timerDoing = false;

        private bool _initialized = false; //스탭모터와 연결 여부 
        private const string DefaultIp = "192.168.10.1"; //스탭모터 연결PC IP설정
        private Dictionary<string, List<int>> LetsIpAixs = new Dictionary<string, List<int>>();

        private StatusThread _statusThread;
        private Task _statusTask;

        private LetsStatusThread _letsStatusThread;
        private Thread _letsStatusThreadRunner;

        #region Servo Speed Set 관련
        private double ServoSpeed = 0;
        private enum enumServoSpeedSet
        {
            Slow,
            Middle,
            High
        }
        private static enumServoSpeedSet ServoSpeedSet = enumServoSpeedSet.Slow;
        #endregion

        private static FuncInline.enumServoAxis ServoSelect = FuncInline.enumServoAxis.SV00_In_Shuttle;
        private static FuncInline.enumLetsAxis LetsSelect = FuncInline.enumLetsAxis.ST00_InShuttle_Width;


        public ServoPopUp()
        {
            InitializeComponent();

            lblStatus.Text = "준비 완료";
            cboIPlist.SelectedIndexChanged += cboIPlist_SelectedIndexChanged;
            txtEndVelocity.Text = "1250";
            txtInitVelocity.Text = "1250";
            txtAcceleration.Text = "12500";
            //txtVel.Text = "30";
            chkMinus.Checked = true;
            timer1.Interval = 1000;   // 0.2초마다 체크
            timer1.Tick += timer1_Tick;
            timer1.Start();

            cboGroup.SelectedIndexChanged += cboGroup_SelectedIndexChanged;
        }
        private void debug(string str)
        {
            Util.Debug(str);
        }
        private void ServoPopUp_Shown(object sender, EventArgs e)
        {
            #region 화면 제어용 쓰레드 타이머 시작            
            TimerCallback CallBackUI = new TimerCallback(TimerUI);
            timerUI = new System.Threading.Timer(CallBackUI, false, 0, 100);
            #endregion

            #region Robot Speed
            ServoSpeedSet = enumServoSpeedSet.Slow;
            #endregion
            ServoSelect = FuncInline.enumServoAxis.SV00_In_Shuttle;
            cbAxisSelect.Text = ServoSelect.ToString();
            LetsSelect = FuncInline.enumLetsAxis.ST00_InShuttle_Width;
            cboLetsAxis.SelectedItem = LetsSelect.ToString();

            timerDoing = true;
            //LetsStatusThread.SnapshotUpdated += StatusChangedUI;
            this.BringToFront();
        }
        private void ServoPopUp_FormClosed(object sender, FormClosedEventArgs e)
        {
            
            try
            {
                
                // LetsStatusThread.SnapshotUpdated -= StatusChangedUI;
            }
            catch
            { }
            if (this.Parent != null)
            {
                try
                {
                    this.Parent.BringToFront();
                }
                catch
                { }
            }
        }

        private void TimerUI(Object state) // 화면 제어 쓰레드 타이머 함수
        {
           
                //폼이 닫히는 중(Disposing)이거나, 핸들이 없으면 즉시 리턴
                // 이 코드가 없으면 폼 닫을 때 Invoke 에러가 터집니다.
                if (this.IsDisposed || !this.IsHandleCreated || timerUI == null)
                {
                    return;
                }
            try
            {
                if ((int)GlobalVar.SystemStatus == (int)enumSystemStatus.AutoRun)
                {
                    timerUI.Dispose();
                    this.Close();
                    return;
                }

                if (GlobalVar.GlobalStop)
                {
                    timerUI.Dispose();
                    this.Close();
                    return;
                }

                if (!timerDoing)
                {
                    return;
                }


                // Invoke 호출 전에도 한 번 더 체크하면 더 안전함
                if (this.IsDisposed || !this.IsHandleCreated) return;

                /* 화면 변경 timer */
                this.Invoke(new MethodInvoker(delegate ()
                {
                    #region Servo Speed 선택
                    if (ServoSpeedSet == enumServoSpeedSet.High)
                    {
                        FuncForm.SetButtonColor2(btnHighSpeed, true);
                        FuncForm.SetButtonColor2(btnMiddleSpeed, false);
                        FuncForm.SetButtonColor2(btnSlowSpeed, false);
                        //ServoSpeed = GlobalVar.ServoSpeed * (0.5);
                        GlobalVar.ServoManualSpeed = 40;
                    }
                    else if (ServoSpeedSet == enumServoSpeedSet.Middle)
                    {
                        FuncForm.SetButtonColor2(btnHighSpeed, false);
                        FuncForm.SetButtonColor2(btnMiddleSpeed, true);
                        FuncForm.SetButtonColor2(btnSlowSpeed, false);
                        //ServoSpeed = GlobalVar.ServoSpeed * (0.5);
                        GlobalVar.ServoManualSpeed = 20;
                    }
                    else if (ServoSpeedSet == enumServoSpeedSet.Slow)
                    {
                        FuncForm.SetButtonColor2(btnHighSpeed, false);
                        FuncForm.SetButtonColor2(btnMiddleSpeed, false);
                        FuncForm.SetButtonColor2(btnSlowSpeed, true);
                        //ServoSpeed = GlobalVar.ServoSpeed * (0.2);
                        GlobalVar.ServoManualSpeed = 5;
                    }
                    #endregion

                    #region Servo Current 
                    lbAxis.Text = ServoSelect.ToString();

                    lblServoPos.Text = GlobalVar.AxisStatus[(int)ServoSelect].Position.ToString("F3");//FuncMotion.GetRealPosition((int)ServoSelect).ToString(); //(GlobalVar.AxisStatus[(int)ServoSelect].Position / 1000).ToString();
                    FuncForm.SetServoStateColor(pbAxisStatus, (int)ServoSelect);

                    #region Z 축을 선택 할 경우 Up Down 표시, Jof 표시
                    if (ServoSelect == FuncInline.enumServoAxis.SV02_Lift1 ||
                    ServoSelect == FuncInline.enumServoAxis.SV04_Lift2)
                    {
                        pbJogUpServo.Visible = true;
                        lbJogUp.Visible = true;

                        lbJogUp.Text = "UP(+)";
                        lbJogDown.Text = "DOWN(-)";


                        pbJogDownServo.Visible = true;
                        lbJogDown.Visible = true;

                        pbJogFrontServo.Visible = false;
                        lbJogFront.Visible = false;
                        pbJogBackServo.Visible = false;
                        lbJogBack.Visible = false;
                    }
                    else
                    {
                        pbJogUpServo.Visible = false;
                        lbJogUp.Visible = false;
                        pbJogDownServo.Visible = false;
                        lbJogDown.Visible = false;
                        pbJogFrontServo.Visible = true;
                        lbJogFront.Visible = true;
                        pbJogBackServo.Visible = true;
                        lbJogBack.Visible = true;
                    }
                    #endregion

                    #endregion

                    btnConnect.Enabled = !FuncLetsMotion.initialized;   //한번 초기화 하고나서 다시 못하게 하기위해, 한번더하면 에러뜸 by DG
                    // Limit 보이기

                    bool IsLimitPlus = GlobalVar.AxisStatus[(int)ServoSelect].LimitSwitchPos;
                    bool IsLimitMinus = GlobalVar.AxisStatus[(int)ServoSelect].LimitSwitchNeg;
                    bool IsHomeOn = GlobalVar.AxisStatus[(int)ServoSelect].HomeAbsSwitch;
                    lbLimitPlus.BackColor = (IsLimitPlus) ? Color.Lime : Color.WhiteSmoke;
                    lbLimitMinus.BackColor = (IsLimitMinus) ? Color.Lime : Color.WhiteSmoke;
                    lbHome.BackColor = (IsHomeOn) ? Color.Lime : Color.WhiteSmoke;

                   
                        
                        
                    cbServoOn.Checked = GlobalVar.LetsAxisStatus[cboLetsAxis.SelectedIndex].PowerOn;
                    cbRun.Checked = !GlobalVar.LetsAxisStatus[cboLetsAxis.SelectedIndex].StandStill;
                    cbOrg.Checked = GlobalVar.LetsAxisStatus[cboLetsAxis.SelectedIndex].isHomed;

                    txtCurPosition.Text = GlobalVar.LetsAxisStatus[cboLetsAxis.SelectedIndex].Position.ToString("F2");

             
                }));

            }
            catch (ObjectDisposedException)
            {
                // 폼이 닫히는 중에 발생한 Invoke 에러는 자연스러운 현상이므로 
                // 그냥 무시하고 리턴합니다. (로그도 안 찍어도 됨)
                return;
            }
            catch (Exception ex)
            {
                FuncLog.WriteLog(ex.ToString());
                FuncLog.WriteLog(ex.StackTrace);
                //debug(ex.StackTrace);
            }
        }

        private void cbAxisSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAxisSelect.SelectedIndex >= (int)FuncInline.enumServoAxis.SV00_In_Shuttle && cbAxisSelect.SelectedIndex <= (int)FuncInline.enumServoAxis.SV07_Scan_X)
            {
                ServoSelect = (FuncInline.enumServoAxis)cbAxisSelect.SelectedIndex;
            }
        }

        #region Click 이벤트
        private void btnSlowSpeed_Click(object sender, EventArgs e)
        {
            ServoSpeedSet = enumServoSpeedSet.Slow;
        }

        private void btnMiddleSpeed_Click(object sender, EventArgs e)
        {
            ServoSpeedSet = enumServoSpeedSet.Middle;
        }

        private void btnHighSpeed_Click(object sender, EventArgs e)
        {
            ServoSpeedSet = enumServoSpeedSet.High;
        }
        private void pbAxisStatus_Click(object sender, EventArgs e)
        {
            FuncMotion.ServoReset((uint)ServoSelect);
            if (!GlobalVar.AxisStatus[(int)ServoSelect].PowerOn)
            {
                FuncMotion.ServoOn((uint)ServoSelect, true);
            }
        }

        bool IsConveyorAxis(int axis)
        {
            if (axis == (int)FuncInline.enumServoAxis.SV00_In_Shuttle)
                return true;
            return false;
        }


        private void pbRobotMove_Click(object sender, EventArgs e)
        {
            if (!GlobalVar.AxisStatus[(int)ServoSelect].StandStill)
            {
                FuncWin.TopMessageBox("Servo Moving!");
                return;
            }

            //FuncBoxPackingMove.MoveServo((int)ServoSelect, (double)numServo.Value, (double)GlobalVar.ServoManualSpeed, false);

            int axis = (int)ServoSelect;

            FuncInlineMove.MoveAbsolute((uint)ServoSelect, (double)numServo.Value,GlobalVar.ServoManualSpeed);

        }

        private void pbRobotStop_Click(object sender, EventArgs e)
        {
            FuncMotion.MoveStop((int)ServoSelect);
        }

        private void pbRobotHome_Click(object sender, EventArgs e)
        {
            if (!GlobalVar.AxisStatus[(int)ServoSelect].StandStill)
            {
                FuncWin.TopMessageBox("Servo Moving!");
                return;
            }
            // 인터락 체크
            if (!FuncInlineMove.ServoInterlockCheck((int)ServoSelect, 0))
            {
                FuncWin.TopMessageBox($"MoveAbsMM {(int)ServoSelect} ServoInterlockCheck failed!\n{FuncInline.Interlock_View}");
                return;
            }
            FuncMotion.MoveHome((uint)ServoSelect);

        }


        #endregion

        #region Jog 속도 및 거리 체크가 필요함.
        #region Up/Down
        private void pbJogUpServo_MouseDown(object sender, MouseEventArgs e)
        {
            //FuncMotion.StopAllJog();

            int axis = (int)ServoSelect;
            double speed = GlobalVar.ServoManualSpeed;

            if (ServoSelect == FuncInline.enumServoAxis.SV02_Lift1 ||
                         ServoSelect == FuncInline.enumServoAxis.SV04_Lift2)
            {
                FuncMotion.MoveRelative((uint)axis,
                 1000,
                 speed);
            }
            else
            {
                FuncMotion.MoveRelative((uint)axis,
                  -1000,
                  speed);
            }











            //FuncMotion.StopAllJog();

            //int axis = (int)ServoSelect;

            //RTEX.MoveRelative((uint)axis,
            //      -10000,
            //      GlobalVar.ServoManualSpeed);

            ////if ((int)ServoSelect <= (int)enum_BoxPacking_ServoAxis.Sanding_Before_X)
            ////{
            ////    FuncBoxPackingMove.JogMoveCheck_StateChange(0);
            ////}
            ////else if ((int)ServoSelect <= (int)enum_BoxPacking_ServoAxis.Sanding_After_X)
            ////{
            ////    FuncBoxPackingMove.JogMoveCheck_StateChange(1);
            ////}

        }
        private void pbJogUpServo_MouseUp(object sender, MouseEventArgs e)
        {
            //FuncMotion.StopAllJog();
            FuncMotion.JogMoveStopAll();
        }
        private void pbJogDownServo_MouseDown(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();

            int axis = (int)ServoSelect;
            double speed = GlobalVar.ServoManualSpeed;

            if (ServoSelect == FuncInline.enumServoAxis.SV02_Lift1 ||
                        ServoSelect == FuncInline.enumServoAxis.SV04_Lift2)
            {
                FuncMotion.MoveRelative((uint)axis,
                   -1000,
                   speed);
            }
            else
            {
                FuncMotion.MoveRelative((uint)axis,
                   1000,
                   speed);
            }






            //FuncMotion.StopAllJog();

            //int axis = (int)ServoSelect;

            //RTEX.MoveRelative((uint)axis,
            //      10000,
            //      GlobalVar.ServoManualSpeed);


            ////if ((int)ServoSelect <= (int)enum_BoxPacking_ServoAxis.Sanding_Before_X)
            ////{
            ////    FuncBoxPackingMove.JogMoveCheck_StateChange(0);
            ////}
            ////else if ((int)ServoSelect <= (int)enum_BoxPacking_ServoAxis.Sanding_After_X)
            ////{
            ////    FuncBoxPackingMove.JogMoveCheck_StateChange(1);
            ////}

        }
        private void pbJogDownServo_MouseUp(object sender, MouseEventArgs e)
        {
            FuncMotion.JogMoveStopAll();
        }
        #endregion
        #region Left/Right
        private void pbJogLeftServo_MouseDown(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();

            int axis = (int)ServoSelect;

            FuncMotion.MoveRelative((uint)axis,
                  -1000,
                  GlobalVar.ServoManualSpeed);
        }
        private void pbJogLeftServo_MouseUp(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();
        }
        private void pbJogRightServo_MouseDown(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();

            int axis = (int)ServoSelect;

            FuncMotion.MoveRelative((uint)axis,
                  1000,
                  GlobalVar.ServoManualSpeed);
        }
        private void pbJogRightServo_MouseUp(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();
        }
        #endregion
        #region Front/Back
        private void pbJogFrontServo_MouseDown(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();

            int axis = (int)ServoSelect;
            double speed = GlobalVar.ServoManualSpeed;  // mm/s



            //


            FuncMotion.MoveRelative((uint)axis,
                  1000,
                  speed);
            //GlobalVar.ServoManualSpeed);

            //if ((int)ServoSelect <= (int)enum_BoxPacking_ServoAxis.Sanding_Before_X)
            //{
            //    FuncBoxPackingMove.JogMoveCheck_StateChange(0);
            //}
            //else if ((int)ServoSelect <= (int)enum_BoxPacking_ServoAxis.Sanding_After_X)
            //{
            //    FuncBoxPackingMove.JogMoveCheck_StateChange(1);
            //}

        }
        private void pbJogFrontServo_MouseUp(object sender, MouseEventArgs e)
        {
            FuncMotion.JogMoveStopAll();
            //RTEX.MoveStop((int)ServoSelect);
        }

        private void pbJogBackServo_MouseDown(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();

            int axis = (int)ServoSelect;
            double speed = GlobalVar.ServoManualSpeed;  // mm/s


            FuncMotion.MoveRelative((uint)axis,
                  -1000,
                  speed);

            //if ((int)ServoSelect <= (int)enum_BoxPacking_ServoAxis.Sanding_Before_X)
            //{
            //    FuncBoxPackingMove.JogMoveCheck_StateChange(0);
            //}
            //else if ((int)ServoSelect <= (int)enum_BoxPacking_ServoAxis.Sanding_After_X)
            //{
            //    FuncBoxPackingMove.JogMoveCheck_StateChange(1);
            //}

        }
        private void pbJogBackServo_MouseUp(object sender, MouseEventArgs e)
        {

            FuncMotion.JogMoveStopAll();
        }



        #endregion

        #endregion

        private void test111_Click(object sender, EventArgs e)
        {
            double zpos = 911;
            double xpos = 19;

            //Bogan_Move((int)enum_BoxPacking_ServoAxis.Sanding_Before_Z1, zpos,
            //    (int)enum_BoxPacking_ServoAxis.Sanding_Before_X, xpos);
        }
        public void Bogan_Move(int axis_1, double ZPOS, int axis_2, double XPOS)
        {
            uint lSize = 2;
            int[] lAxesNo = { axis_1, axis_2 };
            ZPOS = FuncMotion.GetRealPulse((int)axis_1, ZPOS);
            XPOS = FuncMotion.GetRealPulse((int)axis_2, XPOS);
            double[] dPosition = { ZPOS, XPOS };
            double dMaxVelocity = FuncMotion.GetRealPulse((int)axis_1, GlobalVar.ServoSpeed); //100;
            double dMaxAccel = dMaxVelocity * 5;//200
            double dMaxDecel = dMaxVelocity * 5;//200
            int lCoordinate = 0;

            uint uAbsRelMode = 0;
            // 지정 축의 이동 거리 계산 모드를 설정한다.
            // uAbsRelMode : POS_ABS_MODE '0' - 절대 좌표계
            //               POS_REL_MODE '1' - 상대 좌표계

            uint uProfileMode = 3;
            // 지정 축의 구동 속도 프로파일 모드를 설정한다.
            // ProfileMode : SYM_TRAPEZOIDE_MODE    '0' - 대칭 Trapezode
            //               ASYM_TRAPEZOIDE_MODE   '1' - 비대칭 Trapezode
            //               QUASI_S_CURVE_MODE     '2' - 대칭 Quasi-S Curve
            //               SYM_S_CURVE_MODE       '3' - 대칭 S Curve
            //               ASYM_S_CURVE_MODE      '4' - 비대칭 S Curve
            //               SYM_TRAP_M3_SW_MODE    '5' - 대칭 Trapezode : MLIII 내부 S/W Profile
            //               ASYM_TRAP_M3_SW_MODE   '6' - 비대칭 Trapezode : MLIII 내부 S/W Profile
            //               SYM_S_M3_SW_MODE       '7' - 대칭 S Curve : MLIII 내부 S/W Profile
            //               ASYM_S_M3_SW_MODE      '8' - asymmetric S Curve : MLIII 내부 S/W Profile

            // 지정된 좌표계에 연속 보간 구동을 위해 저장된 내부 Queue를 모두 삭제하는 함수이다.
            CAXM.AxmContiWriteClear(lCoordinate);

            // 지정된 좌표계에 연속보간 축 맵핑을 설정한다.
            // (축맵핑 번호는 0 부터 시작))
            // 주의점: 축맵핑할때는 반드시 실제 축번호가 작은 숫자부터 큰숫자를 넣는다.
            //         가상축 맵핑 함수를 사용하였을 때 가상축번호를 실제 축번호가 작은 값 부터 lpAxesNo의 낮은 인텍스에 입력하여야 한다.
            //         가상축 맵핑 함수를 사용하였을 때 가상축번호에 해당하는 실제 축번호가 다른 값이라야 한다.
            //         같은 축을 다른 Coordinate에 중복 맵핑하지 말아야 한다.
            CAXM.AxmContiSetAxisMap(lCoordinate, lSize, lAxesNo);

            // 지정된 좌표계에 연속보간 축 절대/상대 모드를 설정한다.
            // (주의점 : 반드시 축맵핑 하고 사용가능)
            // 지정 축의 이동 거리 계산 모드를 설정한다.
            // uAbsRelMode : POS_ABS_MODE '0' - 절대 좌표계
            //               POS_REL_MODE '1' - 상대 좌표계
            CAXM.AxmContiSetAbsRelMode(lCoordinate, uAbsRelMode);// 상대위치구동으로설정

            // 시작점과 종료점을 지정하여 다축 직선 보간 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
            // AxmContiBeginNode, AxmContiEndNode와 같이사용시 지정된 좌표계에 시작점과 종료점을 지정하여 직선 보간 구동하는 Queue에 저장함수가된다. 
            // 직선 프로파일 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmContiStart함수를 사용해서 시작한다.
            uint duRetCode;
            duRetCode = CAXM.AxmLineMove(lCoordinate, dPosition, dMaxVelocity, dMaxAccel, dMaxDecel);
            if (duRetCode != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                MessageBox.Show(String.Format("AxmLineMove return error[Code:{0:d}]", duRetCode));
        }

        //스탭모터 관련 소스
        private bool EnsureInitialized(string ip)
        {
            try
            {
                int result = LetsExplorerDll.InitConnectionManual(ip);
                if (result <= 0)
                {
                    lblStatus.Text = $"Init  Fail: {result}";
                    MessageBox.Show(lblStatus.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                lblStatus.Text = $"Init Ok: {ip}";
                _initialized = true;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Initialize  Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            // 0) 연결이 안 된 상태면 초기화
            if (!_initialized && !FuncLetsMotion.CheckLetsMotion())
                return;

            lblStatus.Text = "Scanning...";

            if (FuncLetsMotion.CheckLetsScanMotion())
            {

            }
            else
            {

            }

            //// 1) ScanNodeList 호출
            //int scanCount = LetsExplorerDll.ScanNodeList();
            //if (scanCount <= 0)
            //{
            //    MessageBox.Show($"Scan Fail: {scanCount}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    lblStatus.Text = "Scan Fail";
            //    return;
            //}

            //// 2) GetNodeList 호출 (이제 rawNodeInfo 리스트만 들어 있음)
            //int[] nodes = new int[scanCount];
            //int got = LetsExplorerDll.GetNodeList(nodes, scanCount);


            //FuncLog.WriteLog("=== Scan Debug Start ===");
            //for (int i = 0; i < got; i++)
            //{
            //    int raw = nodes[i]; //IP주소 
            //    int grp = (raw >> 24) & 0xFF;   //노드
            //    FuncLog.WriteLog($"[Scan Debug] raw=0x{raw:X8}, group={grp}");
            //}
            //FuncLog.WriteLog("=== Scan Debug End ===");

            // 3) _ipMap 갱신: IP 문자열 → rawNodeInfo 리스트
            LetsIpAixs.Clear();
            var sbIp = new StringBuilder(16);//IP주소 
            for (int i = 0; i < GlobalVar.LetsAxis_count; i++)
            {
               int raw = GlobalVar.LetsAxis[(int)i]; ;//IP주소 // 스캔으로 얻은 원시 노드 핸들

                // IP 문자열 얻기 (하위 8비트)
                sbIp.Clear();
                LetsExplorerDll.GetIpFromNode(raw, sbIp);
                string ip = sbIp.ToString();     // 이 raw 가 속한 IP 문자열

                // 딕셔너리에 추가
                if (!LetsIpAixs.ContainsKey(ip))
                    LetsIpAixs[ip] = new List<int>();   // 해당 IP 최초 발견 시 리스트 생성
                LetsIpAixs[ip].Add(raw);
            }

            // 4) IP 콤보박스에 IP만 바인딩
            cboIPlist.DataSource = null;
            cboIPlist.DataSource = new List<string>(LetsIpAixs.Keys);
            lblStatus.Text = $"Scan Finish. IP Count: {LetsIpAixs.Count}";

        }

        private void LoadNodeListToUI(int count)
        {
            cboIPlist.Items.Clear();

            int[] nodes = new int[count];
            int got;
            try
            {
                got = LetsExplorerDll.GetNodeList(nodes, count);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"노드 리스트 가져오기 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            for (int i = 0; i < got; i++)
            {
                var sb = new StringBuilder(16);
                try
                {
                    LetsExplorerDll.GetIpFromNode(nodes[i], sb);
                    cboIPlist.Items.Add(sb.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"IP 생성 오류(node {nodes[i]}): {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (cboIPlist.Items.Count > 0)
                cboIPlist.SelectedIndex = 0;
            else
                lblStatus.Text = "연결 가능한 장비 없음";
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            EnsureInitialized(DefaultIp);
        }

        private void cboIPlist_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ip = cboIPlist.SelectedItem as string;
            if (ip == null || !LetsIpAixs.ContainsKey(ip))
            {
                cboGroup.DataSource = null;
                return;
            }

            var list = new List<KeyValuePair<string, int>>();
            foreach (int raw in LetsIpAixs[ip])
            {
                int grp = (raw >> 24) & 0xFF;
                string axisName;
                switch (grp)
                {
                    case 0: axisName = "X 축"; break;
                    case 1: axisName = "Y 축"; break;
                    case 2: axisName = "Z 축"; break;
                    case 3: axisName = "U 축"; break;
                    default: axisName = $"축{grp}"; break;
                }
                string label = $"{axisName} ({grp})";
                list.Add(new KeyValuePair<string, int>(label, raw));
            }

            cboGroup.DataSource = list;
            cboGroup.DisplayMember = "Key";
            cboGroup.ValueMember = "Value";
            if (cboGroup.Items.Count > 0)
            {
                cboGroup.SelectedIndex = 0;
            }
        }

        private void btnJogPlus_Click(object sender, EventArgs e)
        {
          
        }

        private void btnJogMinus_Click(object sender, EventArgs e)
        {
            if (cboGroup.SelectedItem == null) return;
            var sel = (KeyValuePair<string, int>)cboGroup.SelectedItem;
            int raw = sel.Value;

            double velocity = double.Parse(txtEndVelocity.Text);
            double acceleration = double.Parse(txtAcceleration.Text);

            int result = LetsExplorerDll.JogMinus(raw, velocity, acceleration);
            if (result < 0)
            {
                MessageBox.Show($"Jog– 시작 실패: {result}", "오류",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = $"Jog– 실패: {result}";
            }
            else
            {
                lblStatus.Text = $"Jog– 실행 중(V={velocity}, A={acceleration})";
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (cboLetsAxis.SelectedItem == null)
            {
                MessageBox.Show("먼저 축을 선택하세요.", "입력 오류",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
          
            FuncLetsMotion.Stop((int)cboLetsAxis.SelectedIndex);

            //if (cboGroup.SelectedItem == null) return;
            //var sel = (KeyValuePair<string, int>)cboGroup.SelectedItem;
            //int raw = sel.Value;

            //int result = LetsExplorerDll.StopJog(raw);
            //if (result < 0)
            //{
            //    MessageBox.Show($"정지 실패: {result}", "오류",
            //                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    lblStatus.Text = $"정지 실패: {result}";
            //}
            //else
            //{
            //    lblStatus.Text = "Jog 정지 완료";
            //}
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            // 1) 축 선택 확인
            if (cboGroup.SelectedItem == null)
            {
                MessageBox.Show("먼저 축을 선택하세요.", "입력 오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // 콤보박스에서 rawHandle 꺼내기
            var sel = (KeyValuePair<string, int>)cboGroup.SelectedItem;
            int raw = sel.Value;

            // 2) 방향 결정
            int direction;
            if (chkPlus.Checked)
            {
                direction = 1;  //+방향 홈무브
            }
            else if (chkMinus.Checked)
            {
                direction = 0;  //-방향 홈무브
            }
            else
            {
                MessageBox.Show("홈 무브 방향(+ 또는 –)을 선택하세요.", "입력 오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3) 속도·가속도 파싱 (VS2015 문법)
            double initV;
            if (!double.TryParse(txtInitVelocity.Text.Trim(), out initV))
            {
                MessageBox.Show("시작 속도를 올바르게 입력하세요.", "입력 오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtInitVelocity.Focus();
                return;
            }
            double endV;
            if (!double.TryParse(txtEndVelocity.Text.Trim(), out endV))
            {
                MessageBox.Show("종료 속도를 올바르게 입력하세요.", "입력 오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEndVelocity.Focus();
                return;
            }
            double acc;
            if (!double.TryParse(txtAcceleration.Text.Trim(), out acc))
            {
                MessageBox.Show("가속도를 올바르게 입력하세요.", "입력 오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAcceleration.Focus();
                return;
            }

            // 4) 디버그 로그 (string.Format 사용)
            FuncLog.WriteLog(string.Format(
                "[OrgHomeSensor] raw=0x{0:X8}, dir={1}, init={2}, end={3}, acc={4}",
                raw, direction, initV, endV, acc));

            // 5) DLL 호출
            int res = LetsExplorerDll.OrgHome(raw, direction, initV, endV, acc);

            // 6) 결과 표시 (string.Format 사용)
            if (res > 0)
            {
                lblStatus.Text = "Home(센서) 동작 시작";
                LetsExplorerDll.SetZero(raw, 0);
                timer1.Start();
            }
            else
            {
                lblStatus.Text = string.Format("Home 실패: {0}", res);
            }

        }

        private void btn2Home_Click(object sender, EventArgs e)
        {
            if (cboLetsAxis.SelectedItem == null)
            {
                MessageBox.Show("먼저 축을 선택하세요.", "입력 오류",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int raw = GlobalVar.LetsAxis[(int)cboLetsAxis.SelectedIndex];


            FuncLog.WriteLog($"{cboLetsAxis.SelectedItem} Home Move");
            //FuncInline.InitialStarted[(int)FuncInline.enumInitialize.InShuttle] = false;
            //FuncInline.InitialStarted[(int)FuncInline.enumInitialize.OutShuttle] = false;
            //FuncInline.InitialStarted[(int)FuncInline.enumInitialize.OutConveyor] = false;
            //FuncInline.InitialStarted[(int)FuncInline.enumInitialize.InConveyor] = false;
            //FuncInline.InitialDone[(int)FuncInline.enumInitialize.InShuttle] = false;
            //FuncInline.InitialDone[(int)FuncInline.enumInitialize.OutShuttle] = false;
            //FuncInline.InitialDone[(int)FuncInline.enumInitialize.OutConveyor] = false;
            //FuncInline.InitialDone[(int)FuncInline.enumInitialize.InConveyor] = false;
            //GlobalVar.LetsHoming = true;
            // FuncInline.InitialDone[(int)FuncInline.enumInitialize.InShuttle] = false;

            FuncLetsMotion.HomeRun((int)cboLetsAxis.SelectedIndex);
            //FuncLetsMotion.HomeRun((int)0);

            //FuncLetsMotion.HomeRun((int)1);

            //FuncLetsMotion.HomeRun((int)2);

            //FuncLetsMotion.HomeRun((int)3);


            //// 1) 축 선택 확인
            //if (cboLetsAxis.SelectedItem == null)
            //{
            //    MessageBox.Show("먼저 축을 선택하세요.", "입력 오류",
            //                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}
            ////var sel = (KeyValuePair<string, int>)cboGroup.SelectedItem;
            //int raw = GlobalVar.LetsAxis[(int)cboLetsAxis.SelectedIndex];

            //// 2축 보드만
            //int groupIndex = (raw >> 24) & 0xFF;
            //if (groupIndex >= 2)
            //{
            //    MessageBox.Show("2축 보드를 선택하세요.", "입력 오류",
            //                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}

            //// 2) 방향 결정
            //int direction;
            //if (chkPlus.Checked)
            //    direction = 1;
            //else if (chkMinus.Checked)
            //    direction = 0;
            //else
            //{
            //    MessageBox.Show("홈 무브 방향(+ 또는 –)을 선택하세요.", "입력 오류",
            //                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}

            //// 3) 속도·가속도 파싱
            //double initV;
            //if (!double.TryParse(txtInitVelocity.Text.Trim(), out initV))
            //{
            //    MessageBox.Show("시작 속도를 올바르게 입력하세요.", "입력 오류",
            //                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    txtInitVelocity.Focus();
            //    return;
            //}

            //double endV;
            //if (!double.TryParse(txtEndVelocity.Text.Trim(), out endV))
            //{
            //    MessageBox.Show("종료 속도를 올바르게 입력하세요.", "입력 오류",
            //                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    txtEndVelocity.Focus();
            //    return;
            //}

            //double acc;
            //if (!double.TryParse(txtAcceleration.Text.Trim(), out acc))
            //{
            //    MessageBox.Show("가속도를 올바르게 입력하세요.", "입력 오류",
            //                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    txtAcceleration.Focus();
            //    return;
            //}
            ////// 홈 시작 시 폴링 딜레이 증가 (예: 200ms 추가)
            ////LetsStatusThread.ExtraSleepMs = 1000;

            ////FuncLog.WriteLog(string.Format("[2축 Home] raw=0x{0:X8}, dir={1}, init={2}, end={3}, acc={4}",
            ////    raw, direction, initV, endV, acc));

            ////int[] handles = new int[32];
            ////int cnt = LetsExplorerDll.GetNodeList(handles, handles.Length);
            ////for (int i = 0; i < cnt; i++)
            ////{
            ////    FuncLog.WriteLog(string.Format("등록된 handle[{0}]: 0x{1:X8}", i, handles[i]));
            ////}

            ////int res = LetsExplorerDll.StartHome2Axis(raw, direction, initV, endV, acc, 0.0);
            ////// 4) 비동기 Task로 DLL 호출
            ////lblStatus.Text = "2축 센서 홈(StartHome) 동작 중...";
            //Task.Run(() =>
            //{
            //    try
            //    {
            //        // 홈 시작 시 폴링 딜레이 증가 (예: 200ms 추가)
            //        LetsStatusThread.ExtraSleepMs = 3000;

            //        FuncLog.WriteLog(string.Format("[2축 Home] raw=0x{0:X8}, dir={1}, init={2}, end={3}, acc={4}",
            //            raw, direction, initV, endV, acc));

            //        int[] handles = new int[32];
            //        int cnt = LetsExplorerDll.GetNodeList(handles, handles.Length);
            //        for (int i = 0; i < cnt; i++)
            //        {
            //            FuncLog.WriteLog(string.Format("등록된 handle[{0}]: 0x{1:X8}", i, handles[i]));
            //        }

            //        int res = LetsExplorerDll.StartHome2Axis(raw, direction, initV, endV, acc, 0.0);

            //        // 결과 UI에 반영
            //        this.BeginInvoke((MethodInvoker)delegate
            //        {
            //            if (res == 0)
            //            {
            //                lblStatus.Text = "2축 센서 홈 완료";
            //                LetsExplorerDll.SetZero(raw, 0);
            //            }
            //            else
            //            {
            //                lblStatus.Text = string.Format("2축 홈 실패: {0}", res);
            //            }
            //        });
            //    }
            //    finally
            //    {
            //        // 홈 끝나면 원래 폴링 속도로 복귀
            //        LetsStatusThread.ExtraSleepMs = 0;
            //    }
            //});
        }

        private void btnAbsMove_Click(object sender, EventArgs e)
        {
            if (cboLetsAxis.SelectedItem == null)
            {
                MessageBox.Show("Please select an axis first.", "Input Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int raw = GlobalVar.LetsAxis[(int)cboLetsAxis.SelectedIndex];

            double position, velocity;

            if (!double.TryParse(txtAbsPos.Value.ToString(), out position) ||
                !double.TryParse(txtVel.Value.ToString(), out velocity))
            {
                MessageBox.Show("Please check your input values.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if(FuncLetsMotion.ABSMove((int)cboLetsAxis.SelectedIndex, position, velocity))
            {

            }
            else
            {
                MessageBox.Show("Comand Error.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            //if (cboGroup.SelectedItem == null)
            //{
            //    MessageBox.Show("축을 선택하세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}
            //var sel = (KeyValuePair<string, int>)cboGroup.SelectedItem;
            //int raw = sel.Value;

            //double position, velocity, acceleration, jerk;
            //if (!double.TryParse(txtAbsPos.Text.Trim(), out position) ||
            //    !double.TryParse(txtEndVelocity.Text.Trim(), out velocity) ||
            //    !double.TryParse(txtAcceleration.Text.Trim(), out acceleration))
            //{
            //    MessageBox.Show("입력값을 확인하세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}

            //int result = LetsExplorerDll.MoveAbs(raw, position, velocity, acceleration, acceleration * 10);
            //lblStatus.Text = (result == 0) ? "절대이동 명령 성공" : $"이동 실패: {result}";
        }

        private void btnRelMove_Click(object sender, EventArgs e)
        {
            if (cboLetsAxis.SelectedItem == null)
            {
                MessageBox.Show("Please select an axis first.", "Input Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int raw = GlobalVar.LetsAxis[(int)cboLetsAxis.SelectedIndex];

            double position, velocity;

            if (!double.TryParse(txtRelPos.Value.ToString(), out position) ||
                !double.TryParse(txtVel.Value.ToString(), out velocity))
            {
                MessageBox.Show("Please check your input values.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (FuncLetsMotion.INCMove((int)cboLetsAxis.SelectedIndex, position, velocity))
            {

            }
            else
            {
                MessageBox.Show("Comand Error.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            //if (cboGroup.SelectedItem == null)
            //{
            //    MessageBox.Show("축을 선택하세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}
            //var sel = (KeyValuePair<string, int>)cboGroup.SelectedItem;
            //int raw = sel.Value;

            //double relDistance, velocity, acceleration, jerk;
            //if (!double.TryParse(txtRelPos.Text.Trim(), out relDistance))
            //{
            //    MessageBox.Show("상대이동 거리를 확인하세요.", "입력 오류");
            //    return;
            //}
            //if (!double.TryParse(txtEndVelocity.Text.Trim(), out velocity))
            //{
            //    MessageBox.Show("최대 속도를 확인하세요.", "입력 오류");
            //    return;
            //}
            //if (!double.TryParse(txtAcceleration.Text.Trim(), out acceleration))
            //{
            //    MessageBox.Show("가속도를 확인하세요.", "입력 오류");
            //    return;
            //}
            //if (!double.TryParse(txtJerk.Text.Trim(), out jerk))
            //{
            //    MessageBox.Show("Jerk 값을 확인하세요.", "입력 오류");
            //    return;
            //}

            //int result = LetsExplorerDll.MoveRel(raw, relDistance, velocity, acceleration, jerk);
            //lblStatus.Text = (result == 0) ? "상대이동 명령 성공" : $"이동 실패: {result}";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //// 현재 선택된 축이 없으면 위치 표시 안 함
            //if (cboGroup.SelectedItem == null)
            //{
            //    txtCurPosition.Text = "";
            //    return;
            //}

            //// 선택된 축의 rawHandle 가져오기
            //var sel = (KeyValuePair<string, int>)cboGroup.SelectedItem;
            //int raw = sel.Value;
            
            //byte nodeType = LetsExplorerDll.GetNodeType(raw);
            //int mAll = LetsExplorerDll.GetAxisState_All();
            //int mServo = LetsExplorerDll.GetAxisState_SERVO();
            //int mMove = LetsExplorerDll.GetAxisState_MOVING();
            //int mOrg = LetsExplorerDll.GetAxisState_ORG();
            //int mAlm = LetsExplorerDll.GetAxisState_ALM();
            //int mEmg = LetsExplorerDll.GetAxisState_EMG();
            ////GetStates로 현재 상태 읽기(모든 상태 OR로 요청함)
            //long mask = LetsExplorerDll.GetState(raw, mAll, nodeType);

            //cbServoOn.Checked = (mask & mServo) != 0; //서보온
            //cbRun.Checked = (mask & mMove) != 0;  //동작진행중
            //cbOrg.Checked = (mask & mOrg) != 0;  //오리진 위치(HOME위치)
            //cbALM.Checked = (mask & mAlm) != 0; //Alarm
            //cbEMG.Checked = (mask & mEmg) != 0; //Emergency

            //// 위치값 읽기 (DLL 호출)
            //double curPos = LetsExplorerDll.GetPosition(raw, 0); // 0 == CMD_POS, 1 == ENC_POS 등

            //// 에러 처리 (음수값 등)
            //if (curPos < -100000)
            //{
            //    txtCurPosition.Text = "Error";
            //}
            //else
            //{
            //    txtCurPosition.Text = curPos.ToString("F2"); // 소수점 2자리
            //}
        }

        private void btnSetZero_Click(object sender, EventArgs e)
        {
            //if (cboGroup.SelectedItem == null)
            //{
            //    MessageBox.Show("축을 선택하세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}
            //var sel = (KeyValuePair<string, int>)cboGroup.SelectedItem;
            //int raw = sel.Value;
            if (cboLetsAxis.SelectedItem == null)
            {
                MessageBox.Show("Please select an axis first.", "Input Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int raw = GlobalVar.LetsAxis[(int)cboLetsAxis.SelectedIndex];


            int result = LetsExplorerDll.SetZero(raw, 0); // 0 == CMD_POS, 1 == ENC_POS
            if (result == 0)
            {
                lblStatus.Text = "현재 위치가 0으로 리셋됨!";
            }
            else
            {
                lblStatus.Text = $"SetZero 실패: {result}";
            }
        }

        private void StatusChangedUI(LetsStatusSnapshot s)
        {
            if (!IsHandleCreated) return;
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => StatusChangedUI(s)));
                return;
            }

            cbServoOn.Checked = s.ServoOn;
            cbRun.Checked = s.Moving;
            cbOrg.Checked = s.Org;

            txtCurPosition.Text = (s.Position < -100000) ? "Error" : s.Position.ToString("F2");
        }

        private void cboGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            //var selected = cboGroup.SelectedItem;
            //if (selected != null)
            //{
            //    var kv = (KeyValuePair<string, int>)selected;
            //    int raw = kv.Value;
            //    LetsStatusThread.SelectedRawHandle = raw;
            //    LetsStatusThread.SelectedNodeType = LetsExplorerDll.GetNodeType(raw);
            //}
        }

      

        private void btnJogPlus_MouseDown(object sender, MouseEventArgs e)
        {
            if (cboLetsAxis.SelectedItem == null)
            {
                MessageBox.Show("먼저 축을 선택하세요.", "입력 오류",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int raw = GlobalVar.LetsAxis[(int)cboLetsAxis.SelectedIndex];

            // 텍스트박스에서 속도/가속도 읽기
            double velocity = double.Parse(txtEndVelocity.Text);
            double acceleration = double.Parse(txtAcceleration.Text);

            int result = LetsExplorerDll.JogPlus(raw, velocity, acceleration);
            if (result < 0)
            {
                MessageBox.Show($"Jog+ 시작 실패: {result}", "오류",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = $"Jog+ 실패: {result}";
            }
            else
            {
                lblStatus.Text = $"Jog+ 실행 중(V={velocity}, A={acceleration})";
            }
        }

        private void btnJogPlus_MouseUp(object sender, MouseEventArgs e)
        {

            FuncLetsMotion.Stop((int)cboLetsAxis.SelectedIndex);
        }

        private void btnJogMinus_MouseDown(object sender, MouseEventArgs e)
        {
            if (cboLetsAxis.SelectedItem == null)
            {
                MessageBox.Show("먼저 축을 선택하세요.", "입력 오류",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int raw = GlobalVar.LetsAxis[(int)cboLetsAxis.SelectedIndex];

            double velocity = double.Parse(txtEndVelocity.Text);
            double acceleration = double.Parse(txtAcceleration.Text);

            int result = LetsExplorerDll.JogMinus(raw, velocity, acceleration);
            if (result < 0)
            {
                MessageBox.Show($"Jog– 시작 실패: {result}", "오류",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = $"Jog– 실패: {result}";
            }
            else
            {
                lblStatus.Text = $"Jog– 실행 중(V={velocity}, A={acceleration})";
            }
        }

        private void btnJogMinus_MouseUp(object sender, MouseEventArgs e)
        {
            FuncLetsMotion.Stop((int)cboLetsAxis.SelectedIndex);
        }

        private void ServoPopUp_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 1. 플래그 먼저 내림
            timerDoing = false;

            // 2. 타이머 안전하게 정지 및 해제
            if (timerUI != null)
            {
                try
                {
                    // Dispose 하기 전에 타이머를 즉시 멈춥니다.
                    // Change(대기시간, 반복시간) -> 둘 다 Infinite면 정지됨
                    timerUI.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                    Thread.Sleep(200);
                    timerUI.Dispose();
                }
                catch { }
                finally
                {
                    timerUI = null;
                }
            }
        }
    }
}
