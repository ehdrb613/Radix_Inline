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

namespace Radix.Popup.Teaching
{
    /*
     * Setting.cs : 각종 옵션 설정 관리
     */

    public partial class Width : Form
    {
        #region 로컬 변수
        private System.Threading.Timer timerUI = null; // Thread Timer
        private bool timerUIDoing = false;
        private System.Threading.Timer timerJog = null; // Thread Timer
        private bool timerJogDoing = false;

        private bool jogWidth = false;
        private bool jogWidthUp = false;
        private bool jogWidthDown = false;

        private FuncInline.enumPMCAxis activeStepAxis = FuncInline.enumPMCAxis.ST00_InShuttle_Width; // 스텝모터일 경우 순번
        private FuncInline.enumServoAxis activeServoAxis = FuncInline.enumServoAxis.SV00_In_Shuttle; // 서보모터일 경우 주축 순번
       
        bool activeServo = false; // 축이 서보로 지정되어 있는가?
        int teachingIndex = 0; // 티칭이 서보/스텝 섞여 있어 변수로 지정한다.
        //private string activePanelWidth = "pnWidthInputLift";
        //private double widthJogSpeed = 4;
        private int debugCount = 0;

        private double[] teachingWidth = new double[Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length + 2];
        private double[] offsetWidth = new double[Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length + 2];

        FuncInline.enumTabMain beforeTabMain = FuncInline.TabMain;
        FuncInline.enumTabTeaching beforeTabTeaching = FuncInline.TabTeaching;
        private bool valueChanged = false;

        //private Stopwatch jogWatch = new Stopwatch(); // 조그 정지 위해
        #endregion

        #region 초기화 함수
        public Width()
        {
            InitializeComponent();
        }


        private void Width_Load(object sender, EventArgs e)
        {
            //debug("load");
            try
            {
                #region 화면 제어용 쓰레드 타이머 시작
                //*
                TimerCallback CallBackUI = new TimerCallback(TimerUI);
                timerUI = new System.Threading.Timer(CallBackUI, false, 0, 100);

                //TimerCallback CallBackMotor = new TimerCallback(TimerMotor); // Invoke 과도 문제로 TimerUI로 통합
                //timerMotor = new System.Threading.Timer(CallBackMotor, false, 0, 100);

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

        private void Width_Leave(object sender, EventArgs e)
        {
            if (GlobalVar.SystemStatus < enumSystemStatus.AutoRun) // 운전중 아니면 모든 모터 정지
            {

                for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumServoAxis)).Length; i++)
                {
                    FuncMotion.MoveStop(i);
                }
                //for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length; i++)
                //{
                //    //PMCClass.Stop((FuncInline.enumPMCAxis)i);
                //}
            }
            //timerJog.Dispose();
            //timerMotor.Dispose();
            //timerUI.Dispose();

        }

        #endregion

        #region 타이머 함수
        private void TimerUI(Object state) // 화면 제어 쓰레드 타이머 함수
        {
            if (FuncInline.TabMain != FuncInline.enumTabMain.Teaching ||
                FuncInline.TabTeaching != FuncInline.enumTabTeaching.Width) // 메인 탭이 다른 곳에 있으면 실행 안 한다.
            {
                debugCount = 0;
                hiddenCount = 0;
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
                        //pnDebug.Visible = debugCount >= 5;
                        //pnHide.Visible = hiddenCount > 3 &&
                        //                teachingIndex >= 3;

                        lblTeachingWidth.Text = teachingWidth[teachingIndex].ToString("F2");

                        if (activeServo)
                        {
                            btnGantry.Visible = true;
                     
                        }
                        else
                        {
                            btnGantry.Visible = false;
                        }


                        #region 모터 현재 위치 표시

                        if (teachingIndex < Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length) // 스텝모터
                        {
                            pnDebug.Visible = false;
                            pbAxisStatus.Visible = false;
                            lblCurrentWidth.Text = (FuncInline.PMCStatus[(int)activeStepAxis].Position).ToString("F2");
                        }
                        else // 서보모터
                        {
                            if (debugCount >= 5)
                            {
                                //btnGantry.Visible = true;
                                pnDebug.Visible = true;
                                lblMasterPos.Text = (GlobalVar.AxisStatus[(int)activeServoAxis].Position).ToString("F0");
                                lblSlavePos.Text = (GlobalVar.AxisStatus[(int)activeServoAxis + 1].Position).ToString("F0");
                            }
                            else
                            {
                                pnDebug.Visible = false;
                            }
                            pbAxisStatus.Visible = true;
                            FuncForm.SetServoStateColor(pbAxisStatus, activeServoAxis);
                            lblCurrentWidth.Text = (FuncMotion.GetRealPosition((int)activeServoAxis)).ToString("F2");
                            pbAxisStatus.Image = GlobalVar.AxisStatus[(int)activeServoAxis].Errored || 
                                                        !GlobalVar.AxisStatus[(int)activeServoAxis].PowerOn ||
                                                        (btnGantry.BackColor == Color.Lime )
                                                ? Radix.Properties.Resources.circle_red
                                                : Radix.Properties.Resources.circle;

                        }
                        #endregion

                        #region 티칭값 표시
                        lblTeachingInConveyor.Text = teachingWidth[(int)FuncInline.enumPMCAxis.ST03_InConveyor_Width].ToString("F2");
                        lblTeachingInShuttle.Text = teachingWidth[(int)FuncInline.enumPMCAxis.ST00_InShuttle_Width].ToString("F2");
                        lblTeachingOutShuttle.Text = teachingWidth[(int)FuncInline.enumPMCAxis.ST01_OutShuttle_Width].ToString("F2");
                        lblTeachingOutConveyor.Text = teachingWidth[(int)FuncInline.enumPMCAxis.ST02_OutConveyor_Width].ToString("F2");
                        lblTeachingNGConveyor.Text = teachingWidth[(int)FuncInline.enumPMCAxis.ST04_NGBuffer].ToString("F2");

                        lblTeachingRack1.Text = teachingWidth[5].ToString("F2");
                        lblTeachingRack2.Text = teachingWidth[6].ToString("F2");
                        #endregion
                    }));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
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
            if (beforeTabMain == FuncInline.enumTabMain.Teaching && beforeTabTeaching == FuncInline.enumTabTeaching.Width &&
                (FuncInline.TabMain != FuncInline.enumTabMain.Teaching || FuncInline.TabTeaching != FuncInline.enumTabTeaching.Width))
            {
                FuncInlineMove.StopAllJog();
                //FuncInline.ClearAllSiteAction();

                #region 값 수정 상태로 창 떠나면 알림
                if (valueChanged)
                {
                    FuncInline.TabMain = FuncInline.enumTabMain.Teaching;
                    FuncInline.TabTeaching = FuncInline.enumTabTeaching.Width;

                    valueChanged = false;
                    if (FuncWin.MessageBoxOK("Width Teaching changed. Save?"))
                    {
                        ApplyAllValue();
                        Func.SaveTeachingWidthIni();
                    }
                }
                #endregion
            }
            beforeTabMain = FuncInline.TabMain;
            beforeTabTeaching = FuncInline.TabTeaching;
            #endregion

            if (FuncInline.TabMain != FuncInline.enumTabMain.Teaching ||
                FuncInline.TabTeaching != FuncInline.enumTabTeaching.Width) // 메인 탭이 다른 곳에 있으면 실행 안 한다.
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
                #region Master축
                if (jogWidth) //
                {
                    if (!jogWidthDown &&
                        !jogWidthUp)
                    {
                        if (GlobalVar.E_Stop ||
                            (activeServo && GlobalVar.AxisStatus[(int)activeServoAxis].Errored) ||
                            (!activeServo && FuncInline.PMCStatus[(int)activeStepAxis].Errored))
                        {
                            if (activeServo)
                            {
                                
                                FuncMotion.MoveStop((int)activeServoAxis);
                            }
                            else
                            {
                                //PMCClass.Stop(activeStepAxis);
                            }
                            jogWidth = false;
                        }
                        else if (activeServo && Math.Abs(GlobalVar.AxisStatus[(int)activeServoAxis].Velocity) > 0.1)
                        {
                            
                            FuncMotion.MoveStop((int)activeServoAxis);
                        }
                        else if (!activeServo && Math.Abs(FuncInline.PMCStatus[(int)activeStepAxis].Velocity) > 0.1)
                        {
                            //PMCClass.Stop(activeStepAxis);
                        }
                        else
                        {
                            jogWidth = false;
                        }
                    }
                }
                #endregion

                #endregion


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                //debug(ex.StackTrace);
            }

            timerJogDoing = false;
            //if (!GlobalVar.GlobalStop)
            //{
            //    Thread.Sleep(GlobalVar.ThreadSleep);
            //    timerJog = new System.Threading.Timer(new TimerCallback(TimerJog), false, 0, 100);
            //}

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
                for (int i = 0; i < FuncInline.WidthOffset.Length; i++)
                {
                    teachingWidth[i] = FuncInline.PCBWidth + FuncInline.WidthOffset[i];
                    FuncInline.TeachingWidth[i] = FuncInline.PCBWidth + FuncInline.WidthOffset[i];
                }

                lblTeachingWidth.Text = teachingWidth[(int)teachingIndex].ToString("F2");

                valueChanged = false;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }

        }

        public void ApplyAllValue()
        {
            try
            {
                for (int i = 0; i < FuncInline.WidthOffset.Length; i++)
                {
                    FuncInline.WidthOffset[i] = teachingWidth[i] - FuncInline.PCBWidth;
                    FuncInline.TeachingWidth[i] = FuncInline.PCBWidth; // apply건 load건 티칭계산이 다시 되었으면 티칭좌표에 적용
                    FuncInline.OffsetWidth[i] = offsetWidth[i];
                }

                valueChanged = false;

            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        #endregion

        #region 로컬함수
        private void debug(string str)
        {
            Util.Debug("frmWidth : " + str);
        }





        #endregion

        #region 버튼 클릭 이벤트

        private void pbWidthApply_Click(object sender, EventArgs e)
        {
            teachingWidth[(int)teachingIndex] = double.Parse(lblCurrentWidth.Text);
            lblTeachingWidth.Text = lblCurrentWidth.Text;
            offsetWidth[(int)teachingIndex] = (double)numOffsetWidth.Value;

            valueChanged = true;

        }

        private bool CheckPCBExist()
        {
            //IN Shuttle
            if (teachingIndex == 0 &&
                            (DIO.GetDIData(FuncInline.enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor) ||
                                    DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor) ||
                                    DIO.GetDIData(FuncInline.enumDINames.X303_4_In_Shuttle_Pcb_Interlock_Sensor)))
            {
                FuncWin.TopMessageBox("PCB detected in In Conveyor. Manual move disabled while PCB exist");
                return true;
            }
            //Out ShuttleUP
            if (teachingIndex == 1 &&
                (DIO.GetDIData(FuncInline.enumDINames.X302_3_Out_Shuttle_OK_PCB_In_Sensor) ||
                        DIO.GetDIData(FuncInline.enumDINames.X302_4_Out_Shuttle_OK_PCB_Stop_Sensor)||
                        DIO.GetDIData(FuncInline.enumDINames.X304_1_Out_Shuttle_Ok_Interlock_Sensor)))
            {
                FuncWin.TopMessageBox("PCB detected in Out Shuttle Up. Manual move disabled while PCB exist");
                return true;
            }
            //Out ShuttleDown
            if (teachingIndex == 1 &&
                (DIO.GetDIData(FuncInline.enumDINames.X402_0_Out_Shuttle_Ng_PCB_In_Sensor) ||
                        DIO.GetDIData(FuncInline.enumDINames.X04_2_Out_Shuttle_NG_PCB_Stop_Sensor) ||
                        DIO.GetDIData(FuncInline.enumDINames.X304_2_Out_Shuttle_Ng_Interlock_Sensor)))
            {
                FuncWin.TopMessageBox("PCB detected in Out Shuttle Down. Manual move disabled while PCB exist");
                return true;
            }

            //Out ConveyorUP
            if (teachingIndex == 2 &&
                (DIO.GetDIData(FuncInline.enumDINames.X02_3_Out_Conveyor_PASSLIne_PCB_Start_Sensor) ||
                        DIO.GetDIData(FuncInline.enumDINames.X02_4_Out_Conveyor_PASSLine_PCB_Stop_Sensor)))
            {
                FuncWin.TopMessageBox("PCB detected in Out Conveyor Up. Manual move disabled while PCB exist");
                return true;
            }
           
          
            //In Conveyor
            if (teachingIndex == 3 &&
                (DIO.GetDIData(FuncInline.enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor) ||
                        DIO.GetDIData(FuncInline.enumDINames.X303_4_In_Shuttle_Pcb_Interlock_Sensor)))
            {
                FuncWin.TopMessageBox("PCB detected In Conveyor. Manual move disabled while PCB exist");
                return true;
            }
            //NG buffer(OutConveyorDown)
            if (teachingIndex == 4 &&
                (DIO.GetDIData(FuncInline.enumDINames.X02_3_Out_Conveyor_PASSLIne_PCB_Start_Sensor) ||
                        DIO.GetDIData(FuncInline.enumDINames.X02_4_Out_Conveyor_PASSLine_PCB_Stop_Sensor)))
            {
                FuncWin.TopMessageBox("PCB detected in NG buffer. Manual move disabled while PCB exist");
                return true;
            }

            // Front Rack,Lift
            if (teachingIndex == 5)
            {
                // 1) Front 측 사이트(1~13) PCB 도크 센서로 체크
                var frontSites = new FuncInline.enumTeachingPos[]
                {
                    FuncInline.enumTeachingPos.Site1_F_DT1,
                    FuncInline.enumTeachingPos.Site2_F_DT2,
                    FuncInline.enumTeachingPos.Site3_F_DT3,
                    FuncInline.enumTeachingPos.Site4_F_DT4,
                    FuncInline.enumTeachingPos.Site5_F_DT5,
                    FuncInline.enumTeachingPos.Site6_F_DT6,
                    FuncInline.enumTeachingPos.Site7_F_DT7,
                    FuncInline.enumTeachingPos.Site8_F_DT8,
                    FuncInline.enumTeachingPos.Site9_F_DT9,
                    FuncInline.enumTeachingPos.Site10_F_DT10_FT4,
                    FuncInline.enumTeachingPos.Site11_F_FT1,
                    FuncInline.enumTeachingPos.Site12_F_FT2,
                    FuncInline.enumTeachingPos.Site13_F_FT3,
                };

                for (int i = 0; i < frontSites.Length; i++)
                {
                    var site = frontSites[i];
                    if (FuncInline.SiteIoMaps.TryGetPcbDockDI(site, out var di) && DIO.GetDIData(di))
                    {
                        //int siteNo = (int)site - (int)FuncInline.enumTeachingPos.Site1_F_DT1 + 1; // 1-based
                        string label = FuncInline.SiteDisplay.GetSiteDisplayName(site);

                        FuncWin.TopMessageBox($"PCB detected in Site #{label}. Manual move disabled while PCB exist");
                        return true;
                    }
                }

                // 2) Front Lift 컨베이어(상/하) PCB 감지
                if (DIO.GetDIData(FuncInline.enumDINames.X403_2_Front_Lift_Up_PCB_In_Sensor) ||
                    DIO.GetDIData(FuncInline.enumDINames.X403_5_Front_Lift_Up_PCB_Stop_Sensor) ||
                    DIO.GetDIData(FuncInline.enumDINames.X400_0_Front_Lift_Down_PCB_In_Sensor) ||
                    DIO.GetDIData(FuncInline.enumDINames.X400_2_Front_Lift_Down_PCB_Stop_Sensor))
                {
                    FuncWin.TopMessageBox("PCB detected in Front Lift Conveyor. Manual move disabled while PCB exist");
                    return true;
                }
            }

            // Rear Rack, Lift
            if (teachingIndex == 6)
            {
                // 1) Rear 측 사이트(14~26) PCB 도크 센서로 체크
                var rearSites = new FuncInline.enumTeachingPos[]
                {
                    FuncInline.enumTeachingPos.Site14_R_DT1,
                    FuncInline.enumTeachingPos.Site15_R_DT2,
                    FuncInline.enumTeachingPos.Site16_R_DT3,
                    FuncInline.enumTeachingPos.Site17_R_DT4,
                    FuncInline.enumTeachingPos.Site18_R_DT5,
                    FuncInline.enumTeachingPos.Site19_R_DT6,
                    FuncInline.enumTeachingPos.Site20_R_DT7,
                    FuncInline.enumTeachingPos.Site21_R_DT8,
                    FuncInline.enumTeachingPos.Site22_R_DT9,
                    FuncInline.enumTeachingPos.Site23_R_DT10_FT4,
                    FuncInline.enumTeachingPos.Site24_R_FT1,
                    FuncInline.enumTeachingPos.Site25_R_FT2,
                    FuncInline.enumTeachingPos.Site26_R_FT3,
                };

                for (int i = 0; i < rearSites.Length; i++)
                {
                    var site = rearSites[i];
                    if (FuncInline.SiteIoMaps.TryGetPcbDockDI(site, out var di) && DIO.GetDIData(di))
                    {
                        //int siteNo = (int)site - (int)FuncInline.enumTeachingPos.Site1_F_DT1 + 1; // 1-based
                        string label = FuncInline.SiteDisplay.GetSiteDisplayName(site);

                        FuncWin.TopMessageBox($"PCB detected in Site #{label}. Manual move disabled while PCB exist");
                        return true;
                    }
                }

                // 2) Rear Lift 컨베이어(상/하) PCB 감지
                if (DIO.GetDIData(FuncInline.enumDINames.X404_6_Rear_Lift_Up_PCB_In_Sensor) ||
                    DIO.GetDIData(FuncInline.enumDINames.X405_1_Rear_Lift_Up_PCB_Stop_Sensor) ||
                    DIO.GetDIData(FuncInline.enumDINames.X405_5_Rear_Lift_Down_PCB_In_Sensor) ||
                    DIO.GetDIData(FuncInline.enumDINames.X405_7_Rear_Lift_Down_PCB_Stop_Sensor))
                {
                    FuncWin.TopMessageBox("PCB detected in Lift #2 Conveyor. Manual move disabled while PCB exist");
                    return true;
                }
            }

            return false;
        }
        private void pbWIdthMove_Click(object sender, EventArgs e)
        {

            if (CheckPCBExist())
            {
                return;
            }
            
            #region 갠트리 해제시 주축 작동 금지
            /*
            if (!CheckGantry())
            {
                FuncWin.TopMessageBox("Set Gantry On First!");
            }
            //*/
            #endregion

            if (teachingIndex < Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length)
            {
                //PMCClass.ABSMove(activeStepAxis, 
                                //(double)numWidth.Value, 
                                //GlobalVar.WidthSpeed);
            }
            else
            {
                FuncMotion.MoveAbsolute((uint)activeServoAxis, 
                                    (double)numWidth.Value, 
                                    (double)numSpeed.Value);
            }
        }

        private void pbWIdthHome_Click(object sender, EventArgs e)
        {
         

            if (CheckPCBExist())
            {
                return;
            }

            if (teachingIndex < Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length)
            {
                //PMCClass.HomeRun(activeStepAxis, GlobalVar.WidthHomSpeed);
            }
            else
            {
                FuncInlineMove.MoveHome((uint)activeServoAxis);
            }
        }

        private void pbWIdthStop_Click(object sender, EventArgs e)
        {
            if (teachingIndex < Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length)
            {
                //PMCClass.Stop(activeStepAxis);
            }
            else
            {
                FuncMotion.MoveStop((int)activeServoAxis);
            }
        }

        private void pbWidthNarrow_MouseDown(object sender, MouseEventArgs e)
        {
            if (btnSpeed.BackColor == Color.Lime &&
                CheckPCBExist())
            {
                FuncWin.TopMessageBox("Can't move while PCB exists.");
                return;
            }

          



            FuncInlineMove.StopAllJog(true);

            double speed = 0 - (double)numSpeed.Value;
            if (btnPitch.BackColor == Color.Lime)
            {
                if (teachingIndex < Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length)
                {
                    //PMCClass.ABSMove(activeStepAxis, 
                        //FuncInline.PMCStatus[(int)activeStepAxis].Position - (double)numPitch.Value, 
                        //(double)numSpeed.Value);
                }
                else
                {
                   
                    FuncMotion.MoveAbsolute((uint)activeServoAxis,
                                        double.Parse(lblCurrentWidth.Text) - (double)numPitch.Value,
                                        (double)numSpeed.Value);
                  
               
                }
            }
            else
            {

                jogWidth = true;
                jogWidthDown = true;

                if (teachingIndex < Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length)
                {
                    //PMCClass.ContMove(activeStepAxis, speed);
                }
                else
                {
                    FuncMotion.MoveAbsolute((uint)activeServoAxis,
                                  -500,
                                    (double)numSpeed.Value);
                }
            }
        }

        private void pbWidthNarrow_MouseUp(object sender, MouseEventArgs e)
        {
            if (btnSpeed.BackColor == Color.Lime)
            {
                if (teachingIndex < Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length)
                {
                    //PMCClass.Stop(activeStepAxis);
                    Thread.Sleep(200);
                    //PMCClass.Stop(activeStepAxis);
                }
                else
                {
                    FuncMotion.MoveStop((int)activeServoAxis);
                }
                jogWidth = false;
            }
        }

    

        private void pbWidthWide_MouseDown(object sender, MouseEventArgs e)
        {

            if (btnSpeed.BackColor == Color.Lime &&
                CheckPCBExist())
            {
                FuncWin.TopMessageBox("Can't move while PCB exists.");
                return;
            }

            FuncInlineMove.StopAllJog(true);

         

            double speed = (double)numSpeed.Value;
            if (btnPitch.BackColor == Color.Lime)
            {
                if (teachingIndex < Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length)
                {
                    //PMCClass.ABSMove(activeStepAxis, FuncInline.PMCStatus[(int)activeStepAxis].Position + (double)numPitch.Value, (double)numSpeed.Value);
                }
                else
                {
                    
                    FuncMotion.MoveAbsolute((uint)activeServoAxis,
                                    double.Parse(lblCurrentWidth.Text) + (double)numPitch.Value,
                                    (double)numSpeed.Value);
                   
                }
            }
            else
            {

                jogWidth = true;
                jogWidthUp = true;

                if (teachingIndex < Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length)
                {
                    //PMCClass.ContMove(activeStepAxis, speed);
                }
                else
                {
                    FuncMotion.MoveAbsolute((uint)activeServoAxis,
                                  500,
                                    (double)numSpeed.Value);
                }
            }
        }

        private void pbWidthWide_MouseUp(object sender, MouseEventArgs e)
        {
            if (btnSpeed.BackColor == Color.Lime)
            {
                if (teachingIndex < Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length)
                {
                    //PMCClass.Stop(activeStepAxis);
                    Thread.Sleep(200);
                    //PMCClass.Stop(activeStepAxis);
                }
                else
                {
                    FuncMotion.MoveStop((int)activeServoAxis);
                }
                jogWidth = false;
            }
        }




        #endregion

        #region 값 변환 이벤트

        #endregion

        private void Width_Shown(object sender, EventArgs e)
        {
            //debug("shown");
            SetSpeed();
        }

        private void SetSpeed()
        {
            try
            {
                int index = -1;
                Array array = Enum.GetValues(typeof(FuncInline.enumPMCAxis));
                for (int i = 0; i < array.Length; i++)
                {
                    if (activeStepAxis == (FuncInline.enumPMCAxis)i)
                    {
                        index = i;
                        break;
                    }
                }
                if (index >= 0)
                {
                    FuncInline.enumPMCAxis axis = (FuncInline.enumPMCAxis)index;
                    //PMCClass.SetSpeed(axis, (double)numSpeed.Value);

                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        private void numWidth_ValueChanged(object sender, EventArgs e)
        {
            SetSpeed();
        }

        /*
        private void SetStepFlag()
        {
            if (GlobalVar.SystemStatus != enumSystemStatus.Initialize &&
                GlobalVar.SystemStatus < enumSystemStatus.AutoRun) // 초기화나 정지시만 조회 flag를 조정한다.
            {
                for (int i = 0; i < FuncInline.PMCStatus.Length; i += 2)
                {
                    bool flag = false;
                    int motionIndex = i / 2;
                    int manualMotion = (int)activeStepAxis / 2;

                    #region 콤보박스 중에 같은 Motion Index가 지정되어 있는 게 있는가?
                    if (motionIndex == manualMotion)
                    {
                        flag = true;
                    }
                    #endregion


                    FuncInline.PMCStatus[i].ReadFlag = flag;
                    if (i + 1 < FuncInline.PMCStatus.Length)
                    {
                        FuncInline.PMCStatus[i + 1].ReadFlag = flag;
                    }
                }
            }
        }
        //*/

        private void btnPosClick(object sender, EventArgs e)
        {
            string sender_name = ((Button)sender).Name;
            //debug("sender_name : " + sender_name);
            ((Button)sender).BackColor = Color.Lime;

            switch (sender_name)
            {
                case "btnPosInConveyor":
                    btnPosInConveyor.BackColor = Color.Lime;
                    btnPosInShuttle.BackColor = Color.White;
                    btnPosOutShuttle.BackColor = Color.White;
                    btnPosOutConveyor.BackColor = Color.White;
                    btnPosNGConveyor.BackColor = Color.White;
                    btnPosRack1.BackColor = Color.White;
                    btnPosRack2.BackColor = Color.White;

                    numWidth.Value = (decimal)teachingWidth[(int)FuncInline.enumPMCAxis.ST03_InConveyor_Width];
                    lblTeachingWidth.Text = teachingWidth[(int)FuncInline.enumPMCAxis.ST03_InConveyor_Width].ToString("F2");
                    numOffsetWidth.Value = (decimal)FuncInline.OffsetWidth[(int)FuncInline.enumPMCAxis.ST03_InConveyor_Width];
                    activeStepAxis = FuncInline.enumPMCAxis.ST03_InConveyor_Width;
                    activeServo = false; // 축이 서보로 지정되어 있는가?
                    teachingIndex = (int)FuncInline.enumPMCAxis.ST03_InConveyor_Width; // 티칭이 서보/스텝 섞여 있어 변수로 지정한다.
                    break;
                case "btnPosInShuttle":
                    btnPosInConveyor.BackColor = Color.White;
                    btnPosInShuttle.BackColor = Color.Lime;
                    btnPosOutShuttle.BackColor = Color.White;
                    btnPosOutConveyor.BackColor = Color.White;
                    btnPosNGConveyor.BackColor = Color.White;
                    btnPosRack1.BackColor = Color.White;
                    btnPosRack2.BackColor = Color.White;

                    numWidth.Value = (decimal)teachingWidth[(int)FuncInline.enumPMCAxis.ST00_InShuttle_Width];
                    lblTeachingWidth.Text = teachingWidth[(int)FuncInline.enumPMCAxis.ST00_InShuttle_Width].ToString("F2");
                    numOffsetWidth.Value = (decimal)FuncInline.OffsetWidth[(int)FuncInline.enumPMCAxis.ST00_InShuttle_Width];
                    activeStepAxis = FuncInline.enumPMCAxis.ST00_InShuttle_Width;
                    activeServo = false; // 축이 서보로 지정되어 있는가?
                    teachingIndex = (int)FuncInline.enumPMCAxis.ST00_InShuttle_Width; // 티칭이 서보/스텝 섞여 있어 변수로 지정한다.
                    break;
                case "btnPosOutShuttle":
                    btnPosInConveyor.BackColor = Color.White;
                    btnPosInShuttle.BackColor = Color.White;
                    btnPosOutShuttle.BackColor = Color.Lime;
                    btnPosOutConveyor.BackColor = Color.White;
                    btnPosNGConveyor.BackColor = Color.White;
                    btnPosRack1.BackColor = Color.White;
                    btnPosRack2.BackColor = Color.White;

                    numWidth.Value = (decimal)teachingWidth[(int)FuncInline.enumPMCAxis.ST01_OutShuttle_Width];
                    lblTeachingWidth.Text = teachingWidth[(int)FuncInline.enumPMCAxis.ST01_OutShuttle_Width].ToString("F2");
                    numOffsetWidth.Value = (decimal)FuncInline.OffsetWidth[(int)FuncInline.enumPMCAxis.ST01_OutShuttle_Width];
                    activeStepAxis = FuncInline.enumPMCAxis.ST01_OutShuttle_Width;
                    activeServo = false; // 축이 서보로 지정되어 있는가?
                    teachingIndex = (int)FuncInline.enumPMCAxis.ST01_OutShuttle_Width; // 티칭이 서보/스텝 섞여 있어 변수로 지정한다.
                    break;
                case "btnPosOutConveyor":
                    btnPosInConveyor.BackColor = Color.White;
                    btnPosInShuttle.BackColor = Color.White;
                    btnPosOutShuttle.BackColor = Color.White;
                    btnPosOutConveyor.BackColor = Color.Lime;
                    btnPosNGConveyor.BackColor = Color.White;
                    btnPosRack1.BackColor = Color.White;
                    btnPosRack2.BackColor = Color.White;

                    numWidth.Value = (decimal)teachingWidth[(int)FuncInline.enumPMCAxis.ST02_OutConveyor_Width];
                    lblTeachingWidth.Text = teachingWidth[(int)FuncInline.enumPMCAxis.ST02_OutConveyor_Width].ToString("F2");
                    numOffsetWidth.Value = (decimal)FuncInline.OffsetWidth[(int)FuncInline.enumPMCAxis.ST02_OutConveyor_Width];
                    activeStepAxis = FuncInline.enumPMCAxis.ST02_OutConveyor_Width;
                    activeServo = false; // 축이 서보로 지정되어 있는가?
                    teachingIndex = (int)FuncInline.enumPMCAxis.ST02_OutConveyor_Width; // 티칭이 서보/스텝 섞여 있어 변수로 지정한다.
                    break;
                case "btnPosNGConveyor":
                    btnPosInConveyor.BackColor = Color.White;
                    btnPosInShuttle.BackColor = Color.White;
                    btnPosOutShuttle.BackColor = Color.White;
                    btnPosOutConveyor.BackColor = Color.White;
                    btnPosNGConveyor.BackColor = Color.Lime;
                    btnPosRack1.BackColor = Color.White;
                    btnPosRack2.BackColor = Color.White;

                    numWidth.Value = (decimal)teachingWidth[(int)FuncInline.enumPMCAxis.ST04_NGBuffer];
                    lblTeachingWidth.Text = teachingWidth[(int)FuncInline.enumPMCAxis.ST04_NGBuffer].ToString("F2");
                    numOffsetWidth.Value = (decimal)FuncInline.OffsetWidth[(int)FuncInline.enumPMCAxis.ST04_NGBuffer];
                    activeStepAxis = FuncInline.enumPMCAxis.ST04_NGBuffer;
                    activeServo = false; // 축이 서보로 지정되어 있는가?
                    teachingIndex = (int)FuncInline.enumPMCAxis.ST04_NGBuffer; // 티칭이 서보/스텝 섞여 있어 변수로 지정한다.
                    break;
                case "btnPosRack1":
                    btnPosInConveyor.BackColor = Color.White;
                    btnPosInShuttle.BackColor = Color.White;
                    btnPosOutShuttle.BackColor = Color.White;
                    btnPosOutConveyor.BackColor = Color.White;
                    btnPosNGConveyor.BackColor = Color.White;
                    btnPosRack1.BackColor = Color.Lime;
                    btnPosRack2.BackColor = Color.White;

                    numWidth.Value = (decimal)teachingWidth[5];
                    lblTeachingWidth.Text = teachingWidth[5].ToString("F2");
                    numOffsetWidth.Value = (decimal)FuncInline.OffsetWidth[5];
                    activeServoAxis = FuncInline.enumServoAxis.SV03_Rack1_Width;
                 
                    activeServo = true; // 축이 서보로 지정되어 있는가?
                    teachingIndex = 5; // 티칭이 서보/스텝 섞여 있어 변수로 지정한다.
                    break;
                case "btnPosRack2":
                    btnPosInConveyor.BackColor = Color.White;
                    btnPosInShuttle.BackColor = Color.White;
                    btnPosOutShuttle.BackColor = Color.White;
                    btnPosOutConveyor.BackColor = Color.White;
                    btnPosNGConveyor.BackColor = Color.White;
                    btnPosRack1.BackColor = Color.White;
                    btnPosRack2.BackColor = Color.Lime;

                    numWidth.Value = (decimal)teachingWidth[6];
                    lblTeachingWidth.Text = teachingWidth[6].ToString("F2");
                    numOffsetWidth.Value = (decimal)FuncInline.OffsetWidth[6];
                    activeServoAxis = FuncInline.enumServoAxis.SV05_Rack2_Width;
                
                    activeServo = true; // 축이 서보로 지정되어 있는가?
                    teachingIndex = 6; // 티칭이 서보/스텝 섞여 있어 변수로 지정한다.
                    break;
            }

        }

        private void Width_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                //timerUI.Dispose();
                //timerJog.Dispose();
                //timerMotor.Dispose(); // 사용안함
            }
            catch { }
        }

        private void pbAxisStatus_Click(object sender, EventArgs e)
        {
            if (teachingIndex >= Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length)
            {
                
                Thread.Sleep(500);
              
                if (GlobalVar.AxisStatus[(int)activeServoAxis].PowerOn)
                {
                    FuncMotion.ServoOn((uint)activeServoAxis, false);
                }
             
                FuncMotion.ServoReset((uint)activeServoAxis);
             
                Thread.Sleep(100);
             
                if (!GlobalVar.AxisStatus[(int)activeServoAxis].PowerOn)
                {
                    FuncMotion.ServoOn((uint)activeServoAxis, true);
                }
            }
        }

        private int hiddenCount = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            if (btnGantry.BackColor != Color.Lime)
            {
                
            }
            else
            {
               
            }
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

        private void panel3_Click(object sender, EventArgs e)
        {
            debugCount++;
        }

        private void btnMatchGantry_Click(object sender, EventArgs e)
        {
          
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
    }
}
