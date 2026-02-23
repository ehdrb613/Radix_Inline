using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Ports;    // SerialPort 클래스 사용을 위해서 추가
using System.Diagnostics;

namespace Radix
{
    /*
     * Setting.cs : 각종 옵션 설정 관리
     */

    public partial class ScanPosition : Form
    {
        #region 로컬 변수
        private System.Threading.Timer timerUI = null; // Thread Timer
        private bool timerUIDoing = false;
        //private System.Threading.Timer timerMotor; // Thread Timer
        //private bool timerMotorDoing = false;
        private System.Threading.Timer timerJog = null; // Thread Timer
        private bool timerJogDoing = false;

        private bool jog = false;
     

        private string activePanelPos = "pnPosPassLine1"; // 현재 선택한 위치
        private FuncInline.enumScanPos activePos = FuncInline.enumScanPos.UnKnown; // 티칭배열 인덱스로 사용할 위치값
        
        private const FuncInline.enumServoAxis actionAxisX = FuncInline.enumServoAxis.SV07_Scan_X; // 선택된 리프트 축
        private const FuncInline.enumServoAxis actionAxisY = FuncInline.enumServoAxis.SV06_Scan_Y; // 선택된 리프트 축
        private int activeNum = 0; // 선택된 축 순번
        private bool validPos = true; // 선택 조합이 맞는가?

        private int siteIndex = -1; //처음 선택은 PassLine1이기 때문에 사이트가 없다 //by DG

        FuncInline.enumTabMain beforeTabMain = FuncInline.TabMain;
        FuncInline.enumTabTeaching beforeTabTeaching = FuncInline.TabTeaching;
        bool valueChanged = false;

        //private double[,] ScanPos = new double[2, Enum.GetValues(typeof(FuncInline.enumLiftPos)).Length]; // 티칭값을 임시로 담아둘 변수. 저장 전에는 갖고만 있다가 저장시 이 값을 저장한다.
        private structPosition[] ScanPos = new structPosition[Enum.GetValues(typeof(FuncInline.enumScanPos)).Length]; // 좌표 티칭값

        private int debugCount = 0;
        #endregion


        #region 초기화 함수
        public ScanPosition()
        {
            InitializeComponent();
        }

        private void Position_Load(object sender, EventArgs e)
        {
            try
            {
                #region 화면 제어용 쓰레드 타이머 시작
                //*
                TimerCallback CallBackUI = new TimerCallback(TimerUI);
                timerUI = new System.Threading.Timer(CallBackUI, false, 0, 100);

                TimerCallback CallBackJog = new TimerCallback(TimerJog);
                timerJog = new System.Threading.Timer(CallBackJog, false, 0, 100);
                //*/
                #endregion

            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }

        }

        private void Position_Leave(object sender, EventArgs e)
        {
            if (GlobalVar.SystemStatus < enumSystemStatus.AutoRun) // 운전중 아니면 모든 모터 정지
            {

                for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumServoAxis)).Length; i++)
                {
                    FuncMotion.MoveStop(i);
                }
                for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumLetsAxis)).Length; i++)
                {
                    //FuncPMC.Stop((enumPMCAxis)i);
                }
            }
        }

        #endregion

        #region 타이머 함수
        private void TimerUI(Object state) // 화면 제어 쓰레드 타이머 함수
        {
            if (FuncInline.TabMain != FuncInline.enumTabMain.Model ||
                FuncInline.TabTeaching != FuncInline.enumTabTeaching.Scan) // 메인 탭이 다른 곳에 있으면 실행 안 한다.
            {
                debugCount = 0;
                timerUIDoing = false;
                return;
            }
            try
            {
                /* 화면 변경 timer */
                if (this == null)
                {
                    return;
                }
                if (timerUIDoing)
                {
                    return;
                }
                timerUIDoing = true;
                //timerUI.Dispose();

                if (!GlobalVar.GlobalStop &&
                    this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate ()
                {
                   

                    #region 관리자 여부에 따라 컨트롤 보이기/숨기기
                  
                    #endregion



                    FuncForm.SetServoStateColor(pbAxisStatusX, actionAxisX);
                    FuncForm.SetServoStateColor(pbAxisStatusY, actionAxisY);
#if DEBUG
                    
#endif
#if !DEBUG
                      
#endif
                    #region 모터 현재 위치 표시
                    lblPosX.Text = (GlobalVar.AxisStatus[(int)actionAxisX].Position).ToString("F2");
                    lblPosY.Text = (GlobalVar.AxisStatus[(int)actionAxisY].Position).ToString("F2");
                #endregion

                    Control[] panels = { pnScanPos }; 

                    #region 티칭값 표시
                    foreach (var panel in panels) // 사이트 묶음 전체
                    {
                        foreach (Control conPos in groupBox_Front.Controls) // Front
                        {
                            if (conPos.GetType() == typeof(Label))
                            {
                                Label label = (Label)conPos;
                                if (label.Name.Contains("lbArray_Teaching"))
                                {
                                    string str = label.Name.Replace("lbArray_Teaching", "");
                                    string[] idxs = str.Split('_');
                                    if (idxs.Length == 2)
                                    {
                                        string Axis = "";
                                        int ArrayIndex = -1;

                                        Axis = idxs[0];
                                        int.TryParse(idxs[1], out ArrayIndex);

                                        if (Axis == "X")
                                        {
                                            label.Text = ScanPos[ArrayIndex].x.ToString("F2");
                                        }
                                        if(Axis == "Y")
                                        {
                                            label.Text = ScanPos[ArrayIndex].y.ToString("F2");
                                        }
                                    }
                                }
                            }
                        }
                        foreach (Control conPos in groupBox_Rear.Controls) // Rear
                        {
                            if (conPos.GetType() == typeof(Label))
                            {
                                Label label = (Label)conPos;
                                if (label.Name.Contains("lbArray_Teaching"))
                                {
                                    string str = label.Name.Replace("lbArray_Teaching", "");
                                    string[] idxs = str.Split('_');
                                    if (idxs.Length == 2)
                                    {
                                        string Axis = "";
                                        int ArrayIndex = -1;

                                        Axis = idxs[0];
                                        int.TryParse(idxs[1], out ArrayIndex);

                                        if (Axis == "X")
                                        {
                                            label.Text = ScanPos[ArrayIndex].x.ToString("F2");
                                        }
                                        if (Axis == "Y")
                                        {
                                            label.Text = ScanPos[ArrayIndex].y.ToString("F2");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion


                }));
                }

            }
            catch (Exception ex)
            {
                FuncLog.WriteLog(ex.ToString());
                FuncLog.WriteLog(ex.StackTrace);
                //debug(ex.StackTrace);
            }

            timerUIDoing = false;
            //if (!GlobalVar.GlobalStop)
            //{
            //    Thread.Sleep(GlobalVar.ThreadSleep);
            //    timerUI = new System.Threading.Timer(new TimerCallback(TimerUI), false, 0, 100);
            //}
            if (GlobalVar.GlobalStop)
            {
                try
                {
                    timerUI.Dispose();
                }
                catch { }
            }
        }


        private void TimerJog(Object state) // 조그제어 쓰레드 타이머 함수
        {
            #region 창 떠나면 조그 멈추기
            if (beforeTabMain == FuncInline.enumTabMain.Model && beforeTabTeaching == FuncInline.enumTabTeaching.Scan &&
                (FuncInline.TabMain != FuncInline.enumTabMain.Model || FuncInline.TabTeaching != FuncInline.enumTabTeaching.Scan))
            {
                FuncInlineMove.StopAllJog();
                //FuncInline.ClearAllSiteAction();
                #region 값 수정 상태로 창 떠나면 알림
                if (valueChanged)
                {
                    FuncInline.TabMain = FuncInline.enumTabMain.Teaching;
                    FuncInline.TabTeaching = FuncInline.enumTabTeaching.LiftPosition;

                    valueChanged = false;
                    if (FuncWin.MessageBoxOK("Scan Teaching changed. Save?"))
                    {
                        ApplyAllValue();
                        Func.SaveModelIni();
                    }
                }
                #endregion
            }
            beforeTabMain = FuncInline.TabMain;
            beforeTabTeaching = FuncInline.TabTeaching;
            #endregion

            if (FuncInline.TabMain != FuncInline.enumTabMain.Teaching ||
                FuncInline.TabTeaching != FuncInline.enumTabTeaching.LiftPosition) // 메인 탭이 다른 곳에 있으면 실행 안 한다.
            {
                timerJogDoing = false;
                return;
            }
            if (this == null)
            {
                return;
            }

            try
            {
                if (timerJogDoing)
                {
                    return;
                }
                timerJogDoing = true;
                //timerJog.Dispose();

                #region JOG 멈추기
                #region 지정 리프트
                if (jog) // 축 두개
                {
                    if (GlobalVar.E_Stop ||
                            GlobalVar.AxisStatus[(int)actionAxisX].Errored)
                    {
                        FuncMotion.MoveStop((int)actionAxisX);
                        FuncMotion.MoveStop((int)actionAxisY);
                        jog = false;
                    }
                  
                    else
                    {
                        jog = false;
                    }
                }
                #endregion

                #endregion

            }
            catch (Exception ex)
            {
                FuncLog.WriteLog(ex.ToString());
                FuncLog.WriteLog(ex.StackTrace);
                //debug(ex.StackTrace);
            }
            timerJogDoing = false;
            if (GlobalVar.GlobalStop)
            {
                try
                {
                    timerJog.Dispose();
                }
                catch { }
            }
        }
        #endregion

        #region 설정 관련
        public void LoadAllValue()
        {
            try
            {              
                    for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumScanPos)).Length; i++)
                    {
                        // apply건 load건 다시 계산된 건 teaching 좌표에 적용
                        ScanPos[i].x = FuncInline.ScanTeachingPos[i].x;
                        ScanPos[i].y = FuncInline.ScanTeachingPos[i].y;
                    }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }

            valueChanged = false;
        }

        public void ApplyAllValue()
        {
            try
            {
                
                for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumScanPos)).Length; i++)
                {
                    
                    FuncInline.ScanTeachingPos[i].x = ScanPos[i].x;
                    FuncInline.ScanTeachingPos[i].y = ScanPos[i].y;
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }

            valueChanged = false;
        }

        #endregion

        #region 로컬함수
        private void debug(string str)
        {
            Util.Debug("frmPosition : " + str);
        }


        #endregion

        #region 조그버튼 처리

        private void pbJogUpRobot_MouseDown(object sender, MouseEventArgs e)
        {
            FuncInlineMove.StopAllJog(true);

            #region 마우스 이외의 방법으로 클릭시 동작 방지
            /*
            if (e.Button == MouseButtons.None)
            {
                FuncWin.TopMessageBox("Use mouse at jog action.");
                return;
            }
            //*/

            #endregion
      
            if (btnPitch.BackColor == Color.Lime)
            {
   
                FuncInlineMove.MoveAbsolute((uint)actionAxisY,
                double.Parse(lblPosY.Text) + (double)numPitch.Value,
                (double)numSpeed.Value);
    
            }
            else
            {
                jog = true;                   

                FuncInlineMove.MoveAbsolute((uint)actionAxisY, 10000, (double)numSpeed.Value);
            }
        }

        private void pbJogUpRobot_MouseUp(object sender, MouseEventArgs e)
        {

            if (btnSpeed.BackColor == Color.Lime)
            {
                FuncInlineMove.StopAllJog(true);
                Thread.Sleep(200);
                FuncInlineMove.StopAllJog(true);
                jog = false;
            }
        }

        private void pbJogDownRobot_MouseDown(object sender, MouseEventArgs e)
        {
            FuncInlineMove.StopAllJog();

            #region 마우스 이외의 방법으로 클릭시 동작 방지
            /*
            if (e.Button == MouseButtons.None)
            {
                FuncWin.TopMessageBox("Use mouse at jog action.");
                return;
            }
            //*/

            #endregion


            //axis = (int)FuncInline.enumServoAxis.RobotZ2;
            if (btnPitch.BackColor == Color.Lime)
            {
              
                FuncInlineMove.MoveAbsolute((uint)actionAxisY,
                double.Parse(lblPosY.Text) - (double)numPitch.Value,
                (double)numSpeed.Value);

            }
            else
            {
                jog = true;
                
                FuncInlineMove.MoveAbsolute((uint)actionAxisY, -200,(double)numSpeed.Value);
            
            }
        }

        private void pbJogDownRobot_MouseUp(object sender, MouseEventArgs e)
        {
            if (btnSpeed.BackColor == Color.Lime)
            {
                FuncInlineMove.StopAllJog(true);
                Thread.Sleep(200);
                FuncInlineMove.StopAllJog(true);
                
                jog = false;
               
            }
        }

        private void btnJogLeft_MouseDown(object sender, MouseEventArgs e)
        {
            FuncInlineMove.StopAllJog(true);

            #region 마우스 이외의 방법으로 클릭시 동작 방지
            /*
            if (e.Button == MouseButtons.None)
            {
                FuncWin.TopMessageBox("Use mouse at jog action.");
                return;
            }
            //*/

            #endregion

            if (btnPitch.BackColor == Color.Lime)
            {

                FuncInlineMove.MoveAbsolute((uint)actionAxisX,
                double.Parse(lblPosX.Text) + (double)numPitch.Value,
                (double)numSpeed.Value);

            }
            else
            {
                jog = true;

                FuncInlineMove.MoveAbsolute((uint)actionAxisX, 10000, (double)numSpeed.Value);
            }
        }
        private void btnJogLeft_MouseUp(object sender, MouseEventArgs e)
        {
            if (btnSpeed.BackColor == Color.Lime)
            {
                FuncInlineMove.StopAllJog(true);
                Thread.Sleep(200);
                FuncInlineMove.StopAllJog(true);
                jog = false;
            }
        }
        private void btnJogRigth_MouseDown(object sender, MouseEventArgs e)
        {
            FuncInlineMove.StopAllJog(true);

            #region 마우스 이외의 방법으로 클릭시 동작 방지
            /*
            if (e.Button == MouseButtons.None)
            {
                FuncWin.TopMessageBox("Use mouse at jog action.");
                return;
            }
            //*/

            #endregion

            if (btnPitch.BackColor == Color.Lime)
            {

                FuncInlineMove.MoveAbsolute((uint)actionAxisX,
                double.Parse(lblPosX.Text) - (double)numPitch.Value,
                (double)numSpeed.Value);

            }
            else
            {
                jog = true;

                FuncInlineMove.MoveAbsolute((uint)actionAxisX, -15, (double)numSpeed.Value);
            }
        }
        private void btnJogRigth_MouseUp(object sender, MouseEventArgs e)
        {
            if (btnSpeed.BackColor == Color.Lime)
            {
                FuncInlineMove.StopAllJog(true);
                Thread.Sleep(200);
                FuncInlineMove.StopAllJog(true);
                jog = false;
            }
        }

        #endregion

        #region 버튼 클릭 이벤트

        private void pbRobotApply_Click(object sender, EventArgs e)
        {
           

            try
            {
             
                ScanPos[(int)activePos].x = double.Parse(lblPosX.Text);
                ScanPos[(int)activePos].y = double.Parse(lblPosY.Text);
               
                lblTeachingX.Text = ScanPos[(int)activePos].x.ToString("F2");
                lblTeachingY.Text = ScanPos[(int)activePos].y.ToString("F2");

                FuncLog.WriteLog($"ScanPos X apply : {activePos} - {ScanPos[(int)activePos].x}");
                FuncLog.WriteLog($"ScanPos Y apply : {activePos} - {ScanPos[(int)activePos].y}");
             
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }

            valueChanged = true;



        }

        private void pbRobotMove_Click(object sender, EventArgs e)
        {
            
            if (!validPos)
            {
                FuncWin.TopMessageBox("Check Scan and Position Selection.");
                return;
            }

            if(!GlobalVar.AxisStatus[(uint)actionAxisX].isHomed)
            {
                FuncWin.TopMessageBox("Scan X - Origin first.");
                return;
            }
            if (!GlobalVar.AxisStatus[(uint)actionAxisY].isHomed)
            {
                FuncWin.TopMessageBox("Scan Y - Origin first.");
                return;
            }

            #region 초기화 전 실행 금지
            //if (GlobalVar.SystemStatus <= enumSystemStatus.Initialize)
            //{
            //    FuncWin.TopMessageBox("Origin first.");
            //    return;
            //}
            #endregion

            #region 도어 열림시 동작 금지
            if (GlobalVar.UseDoor &&
                    (DIO.GetDIData(FuncInline.enumDINames.X00_0_Door_Open_Front_Left) ||
                            DIO.GetDIData(FuncInline.enumDINames.X00_1_Door_Open_Front_Right) ||
                            DIO.GetDIData(FuncInline.enumDINames.X00_2_Door_Open_Rear_Left) ||
                            DIO.GetDIData(FuncInline.enumDINames.X02_0_Door_Open_Rear_Right)))
            {
                FuncWin.TopMessageBox("Can not move while doors are opened.");
                return;
            }
            #endregion

            FuncInlineMove.MoveAbsolute((uint)actionAxisX, (double)numPosX.Value,(double)numSpeed.Value);
            FuncInlineMove.MoveAbsolute((uint)actionAxisY, (double)numPosY.Value, (double)numSpeed.Value);

        }

        private void pbRobotHome_Click(object sender, EventArgs e)
        {
            if (!validPos)
            {
                FuncWin.TopMessageBox("Check Lift and Position Selection.");
                return;
            }
            #region 도어 열림시 동작 금지
            if (GlobalVar.UseDoor &&
                    (DIO.GetDIData(FuncInline.enumDINames.X00_0_Door_Open_Front_Left) ||
                            DIO.GetDIData(FuncInline.enumDINames.X00_1_Door_Open_Front_Right) ||
                            DIO.GetDIData(FuncInline.enumDINames.X00_2_Door_Open_Rear_Left) ||
                            DIO.GetDIData(FuncInline.enumDINames.X02_0_Door_Open_Rear_Right)))
            {
                FuncWin.TopMessageBox("Can not move while doors are opened.");
                return;
            }
            #endregion
           
            
            string name = ((Button)sender).Name;
            if (name.Contains("pbHomeX"))
            {
                FuncLog.WriteLog("Home Click : " + actionAxisX.ToString());
                FuncMotion.MoveHome((uint)actionAxisX);
                return;
            }
            if (name.Contains("pbHomeY"))
            {
                FuncLog.WriteLog("Home Click : " + actionAxisY.ToString());
                FuncMotion.MoveHome((uint)actionAxisY);

            }

        }

        private void pbRobotStop_Click(object sender, EventArgs e)
        {
            if (!validPos)
            {
                FuncWin.TopMessageBox("Check Scan Position Selection.");
                return;
            }
            FuncMotion.MoveStop((int)actionAxisX);
            FuncMotion.MoveStop((int)actionAxisY);
        }
        private void pbAxisStatusX_Click(object sender, EventArgs e)
        {
            FuncMotion.ServoOn((uint)actionAxisX, false);
            FuncMotion.ServoReset((uint)actionAxisX);
         
            FuncMotion.ServoOn((uint)actionAxisX, true);
            //}
        }
        private void pbAxisStatusY_Click(object sender, EventArgs e)
        {
            FuncMotion.ServoOn((uint)actionAxisY, false);
            FuncMotion.ServoReset((uint)actionAxisY);

            FuncMotion.ServoOn((uint)actionAxisY, true);
            //}
        }
      

        #endregion

        #region 값 변환 이벤트




        #endregion

        private void Position_Shown(object sender, EventArgs e)
        {
            LoadAllValue();

            numPosX.Value = (decimal)ScanPos[(int)activePos].x;

            numPosY.Value = (decimal)ScanPos[(int)activePos].y;

            for (int i = 0; i < 6; i++)
            {
                if (!FuncInline.ArrayUse[i])
                {
                    DisableArrayControls(i);
                }
                    
            }
            this.Text = GlobalVar.ModelName;

            ////사용 어래이만 활성화
            //if (!FuncInline.ArrayUse[0])
            //{
            //    //Front Array 1
            //    btnPosArray1.Enabled = false;
            //    lbArray_TeachingX_1.Enabled = false;
            //    lbArray_TeachingY_1.Enabled = false;

            //    //Rear Array 1
            //    btnPosArray7.Enabled = false;
            //    lbArray_TeachingX_7.Enabled = false;
            //    lbArray_TeachingY_7.Enabled = false;
            //}
            //if (!FuncInline.ArrayUse[1])
            //{
            //    //Front Array 2
            //    btnPosArray2.Enabled = false;
            //    lbArray_TeachingX_2.Enabled = false;
            //    lbArray_TeachingY_2.Enabled = false;

            //    //Rear Array 2
            //    btnPosArray8.Enabled = false;
            //    lbArray_TeachingX_8.Enabled = false;
            //    lbArray_TeachingY_8.Enabled = false;
            //}


        }

        //사용하지 않는 어레이를 비활성화
        private void DisableArrayControls(int idx)
        {
            int front = idx + 1;   // 1~6
            int rear = idx + 7;   // 7~12

            void Off(int n)
            {
                Button btn = this.Controls.Find($"btnPosArray{n}", true).FirstOrDefault() as Button;
                Label lx = this.Controls.Find($"lbArray_TeachingX_{n}", true).FirstOrDefault() as Label;
                Label ly = this.Controls.Find($"lbArray_TeachingY_{n}", true).FirstOrDefault() as Label;
                if (btn != null) btn.Enabled = false;
                if (lx != null) lx.Enabled = false;
                if (ly != null) ly.Enabled = false;
            }

            Off(front);
            Off(rear);
        }


        private void btnPos_Click(object sender, EventArgs e) // 버튼 눌렀을 때
        {
            try
            {
                bool isLift = false;
                bool isShuttle = false;
                string name = ((Button)sender).Name;
                if (name.Contains("btnPosArray"))
                {
                    int.TryParse(name.Replace("btnPosArray", ""), out siteIndex);
                    siteIndex--;
                }
                else
                {
                    siteIndex = -1;
                }
             

                activePanelPos = name;
                SetScanPosition();

                #region 티칭 좌표 선택
                if (validPos)
                {
                    lblTeachingX.Text = ScanPos[(int)activePos].x.ToString("F2");
                    numPosX.Value = (decimal)ScanPos[(int)activePos].x;
                    lblTeachingY.Text = ScanPos[(int)activePos].y.ToString("F2");
                    numPosY.Value = (decimal)ScanPos[(int)activePos].y;
                }
                #endregion
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }
        void PaintPosButtons(string activeName) //패널 그룹화 해서 컨트롤 찾기 인셔틀 추가
        {
            Control[] panels = { pnScanPos }; // ← 둘을 묶어서

            foreach (var panel in panels)
            {
                
                foreach (Control conButton in groupBox_Front.Controls)
                {
                    if (conButton.Name.StartsWith("btnPos"))
                    {
                        if (conButton.Name == activeName)
                        {
                            ((Button)conButton).BackColor = Color.Lime;
                        }
                        else
                        {
                            ((Button)conButton).BackColor = Color.White;
                        }
                    }
                }
                foreach (Control conButton in groupBox_Rear.Controls)
                {
                    if (conButton.Name.StartsWith("btnPos"))
                    {
                        if (conButton.Name == activeName)
                        {
                            ((Button)conButton).BackColor = Color.Lime;
                        }
                        else
                        {
                            ((Button)conButton).BackColor = Color.White;
                        }
                    }
                }
               
            }
          
        }

        private void SetScanPosition() // 위치, 리프트 선택 따라서 축 선택
        {
            #region 전체 버튼 색상 지정
         
            PaintPosButtons(activePanelPos);    //활성화 버튼 색상 변경

            #endregion
            #region Lift 부분
           
            if (siteIndex >= 0 &&
                siteIndex < 12) //Front Array
            {
                activePos = FuncInline.enumScanPos.FArray1 + siteIndex;
                validPos = true;
            }
          
            #endregion
    
        }


        private void btnSpeed_Click(object sender, EventArgs e)
        {
            btnPitch.BackColor = Color.White;
            btnSpeed.BackColor = Color.Lime;
        }

        private void btnPitch_Click(object sender, EventArgs e)
        {
            btnPitch.BackColor = Color.Lime;
            btnSpeed.BackColor = Color.White;
        }

        private void numPitch_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                trackBarPitch.Value = (int)((double)numPitch.Value * 100);
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        private void trackBarPitch_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                numPitch.Value = (decimal)Math.Max(0.01, (double)trackBarPitch.Value / 100);
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        private void trackBarSpeed_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                numSpeed.Value = trackBarSpeed.Value;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        private void numSpeed_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                trackBarSpeed.Value = (int)numSpeed.Value;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }



       
        private void ScanPosition_FormClosed(object sender, FormClosedEventArgs e)
        {
            timerUI.Dispose();
            // 4) 스캐너 재연결
            try
            {
                // API가 다르면 여길 프로젝트에 맞게 바꾸세요.
                // 예: FuncInline.Scanner.Connect() / Reconnect() / Open(port, baud) 등
                if (!FuncInline.Scanner.Connect())
                {
                    FuncWin.TopMessageBox($"Scanner Connect fail. PORT is COM { FuncInline.PortScanner} ");
                }
            }
            catch (Exception ex)
            {
                // 실패 시 로그/알림
                try { FuncLog.WriteLog("Scanner reconnect failed: " + ex.Message); } catch { }
                // 필요 시 사용자 알림:
                // MessageBox.Show("스캐너 재연결 실패: " + ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                // 1) 메인폼 복원(최소화 해제)
                var main = Application.OpenForms
                                      .OfType<Radix.frmMain_AutoInline_PC>()
                                      .FirstOrDefault();
                if (main != null)
                    main.WindowState = FormWindowState.Normal;
            }
            catch { /* 필요하면 로그 */ }

        }

        private void ScanPosition_FormClosing(object sender, FormClosingEventArgs e)
        {

            // 2) 외부 프로그램 종료 (살아있으면 먼저 종료)
            string exePath = @"C:\FA\AutoInline\Preview\Usercom_V2.exe";
            try
            {
                string procName = Path.GetFileNameWithoutExtension(exePath); // "Usercom_V2"

                foreach (var p in Process.GetProcessesByName(procName))
                {
                    try
                    {
                        if (p.HasExited) { p.Dispose(); continue; }

                        // (1) 정상 종료 시도
                        bool sent = p.CloseMainWindow();
                        if (sent)
                        {
                            if (p.WaitForExit(2000) == false)  // 2초 대기
                            {
                                // (2) 강제 종료
                                p.Kill();
                                p.WaitForExit(2000);
                            }
                        }
                        else
                        {
                            // 메인 윈도우가 없거나 메시지 못 보냈으면 바로 Kill
                            p.Kill();
                            p.WaitForExit(2000);
                        }
                    }
                    catch
                    {
                        try { if (!p.HasExited) p.Kill(); } catch { }
                    }
                    finally
                    {
                        try { p.Dispose(); } catch { }
                    }
                }
            }
            catch { /* 필요하면 로그 */ }

            // 3) 포트/리소스 정리 여유 (프로세스 종료 직후 시리얼 포트가 잠깐 바쁠 수 있음)
            try { Thread.Sleep(300); } catch { }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ApplyAllValue();
            Func.SaveModelIni();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadAllValue();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Scan ScanTeaching Close Click");
            this.Close();
        }
    }
}
