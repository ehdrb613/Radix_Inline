using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace Radix
{
    /*
     * Init.cs : 장비 구동 직후 초기화 실행
     */

    public partial class Init : Form
    {
        private System.Threading.Timer timerCheck;
        private bool timerDoing = false;

        private bool btnInputConveyor_sel = false;
        private bool btnWorkConveyor_sel = false;
        private bool btnBufferConveyor_sel = false;
        private bool btnXAxis_sel = false;
        private bool btnYAxis_sel = false;
        private bool btnZAxis_sel = false;
        private bool btnWorkWidth_sel = false;
        private bool btnInputWidth_sel = false;
        private bool btnWorkTurn_sel = false;
        private bool btnPCBClamp_sel = false;
        private bool btnCleaner_sel = false;

        private bool xInited = false;
        private bool yInited = false;
        private bool rInited = false;
        private bool zInited = false;
        private bool iwInited = false;
        private bool wwInited = false;

        private bool workWidthInited = false;
        public Init()
        {
            InitializeComponent();
        }

        private void debug(string str)
        {
            //Util.Debug(str);
        }

        private void Init_Shown(object sender, EventArgs e)
        {
            GlobalVar.dlgOpened = true;
            // 타이머 시작
            TimerCallback CallBackCheck = new TimerCallback(TimerCheck);
            timerCheck = new System.Threading.Timer(CallBackCheck, false, 0, 1000);


            xInited = false;
            yInited = false;
            rInited = false;
            zInited = false;
            iwInited = false;
            wwInited = false;
            //GlobalVar.ServoInited = false;
            this.BringToFront();
        }

        private void Init_FormClosed(object sender, FormClosedEventArgs e)
        {
            //DIO.WriteDOData(FuncInline.enumDONames.Y02_0_Before_Tray_Input_Conveyor_Stopper_Upward, true);
            timerCheck.Dispose();
            GlobalVar.dlgOpened = false;
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

        private void TimerCheck(Object state)
        {
            try
            {
                if (timerDoing)
                {
                    return;
                }

                timerDoing = true;
                this.Invoke(new MethodInvoker(delegate ()
                {
                    if ((int)GlobalVar.SystemStatus >= (int)enumSystemStatus.AutoRun) // 작동 중에는 창 자동으로 닫기
                    {
                        timerDoing = false;
                        this.Close();
                        return;
                    }



                    int[] convSpeed = { 0, 0 };
                    /*
                    if (!GlobalVar.Simulation)
                    {
                        convSpeed = GlobalVar.WorkConv.GetDrvSpd(enumAxis.XY);
                    }
                    //*/
                    if (Math.Abs(convSpeed[0]) > 1 || Math.Abs(convSpeed[0]) > 1)
                    {
                        if (btnWorkConveyor_sel)
                        {
                            pbWorkConveyor.BackgroundImage = Radix.Properties.Resources.init_work_conv_yellow;
                        }
                        else
                        {
                            pbWorkConveyor.BackgroundImage = Radix.Properties.Resources.init_work_conv_normal;
                        }
                    }
                    else
                    {
                        pbWorkConveyor.BackgroundImage = Radix.Properties.Resources.init_work_conv_green;
                        btnWorkConveyor_sel = false;
                    }

                }));
                timerDoing = false;
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.ToString());
                //Console.WriteLine(ex.StackTrace);
            }
        }

    
        private void btnSelect_Click(object sender, EventArgs e)
        {
            btnInputConveyor_sel = true;
            btnWorkConveyor_sel = true;
            btnBufferConveyor_sel = true;
            btnXAxis_sel = true;
            btnYAxis_sel = true;
            btnZAxis_sel = true;
            btnWorkWidth_sel = true;
            btnInputWidth_sel = true;
            btnWorkTurn_sel = true;
            btnPCBClamp_sel = true;
        }

        private void btnUnselect_Click(object sender, EventArgs e)
        {
            btnInputConveyor_sel = false;
            btnWorkConveyor_sel = false;
            btnBufferConveyor_sel = false;
            btnXAxis_sel = false;
            btnYAxis_sel = false;
            btnZAxis_sel = false;
            btnWorkWidth_sel = false;
            btnInputWidth_sel = false;
            btnWorkTurn_sel = false;
            btnPCBClamp_sel = false;
        }

        private void btnInit_Click(object sender, EventArgs e)
        {
            #region Door Check
            /*
            if (GlobalVar.UseDoor &&
                (DIO.GetDIData(FuncInline.enumDINames.X00_1_Module1_Front_Door) ||
                    DIO.GetDIData(FuncInline.enumDINames.X00_2_Module1_Rear_Door)))
            {
                this.BringToFront();
                this.BringToFront();
                FuncWin.TopMessageBox("Can't initialize while doors are opened.");
                return;
            }
            //*/
            #endregion

            #region if 서보 disabled
            /*
            for (uint servoNum = 0; servoNum < GlobalVar.Axis_count; servoNum++)
            {
                if (GlobalVar.AxisStatus[servoNum].Errored ||
                        GlobalVar.AxisStatus[servoNum].Disabled ||
                        GlobalVar.AxisStatus[servoNum].ErrorStop)
                {
                    RTEX.ServoReset(servoNum);
                    RTEX.ServoOn(servoNum, true);
                }
            }
            //*/
            #endregion

            //if (//GlobalVar.ServoInited &&
            //         !DIO.GetDORead(FuncInline.enumDONames.Y01_0_))
            //{
            //    //GlobalVar.SystemInited = true;
            //    GlobalVar.SystemStatus = enumSystemStatus.Manual;
            //    GlobalVar.SystemMsg = "";
            //    this.BringToFront();
            //    FuncWin.TopMessageBox("System initialization finished.");
            //    return;
            //}

            this.BringToFront();
            FuncWin.TopMessageBox("System Initialization not finished.");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPass_Click(object sender, EventArgs e)
        {
            this.BringToFront();
            if (FuncWin.MessageBoxOK("Proceeding bypass initialization may cause serious system errors. \r\nDo you want to bypass the initialization process?"))
            {
                //GlobalVar.ServoInited = true;
                GlobalVar.SystemStatus = enumSystemStatus.Manual;
                this.Close();
            }
        }


        private void tmrCheck_Tick(object sender, EventArgs e)
        {
        
            if ((int)GlobalVar.SystemStatus >= (int)enumSystemStatus.AutoRun)
            {
                this.BringToFront();
                FuncWin.TopMessageBox("Can't initialization while system is running.");
                this.Close();
            }
        }

        private void btnInputConveyor_Click(object sender, EventArgs e)
        {
            btnInputConveyor_sel = !btnInputConveyor_sel;
        }

        private void btnWorkConveyor_Click(object sender, EventArgs e)
        {
            btnWorkConveyor_sel = !btnWorkConveyor_sel;
        }

        private void btnXAxis_Click(object sender, EventArgs e)
        {
            btnXAxis_sel = !btnXAxis_sel;
        }

        private void btnYAxis_Click(object sender, EventArgs e)
        {
            btnYAxis_sel = !btnYAxis_sel;
        }

        private void btnZAxis_Click(object sender, EventArgs e)
        {
            btnZAxis_sel = !btnZAxis_sel;
        }

        private void btnWorkWidth_Click(object sender, EventArgs e)
        {
            btnWorkWidth_sel = !btnWorkWidth_sel;
        }

        private void btnInputWidth_Click(object sender, EventArgs e)
        {
            btnInputWidth_sel = !btnInputWidth_sel;
        }

        private void btnWorkTurn_Click(object sender, EventArgs e)
        {
            btnWorkTurn_sel = !btnWorkTurn_sel;
        }
        
        private void pbPCBClamp_Click(object sender, EventArgs e)
        {
            btnPCBClamp_sel = !btnPCBClamp_sel;
        }

        private void btnBufferConveyor_Click(object sender, EventArgs e)
        {
            btnBufferConveyor_sel = !btnBufferConveyor_sel;
        }

        private void btnCleaner_Click(object sender, EventArgs e)
        {
            btnCleaner_sel = !btnCleaner_sel;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            /*
            if (GlobalVar.UseDoor &&
                (DIO.GetDIData(FuncInline.enumDINames.X00_1_Module1_Front_Door) ||
                    DIO.GetDIData(FuncInline.enumDINames.X00_2_Module1_Rear_Door)))
            {
                this.BringToFront();
                FuncWin.TopMessageBox("Can't initialize while doors are opened.");
                return;
            }
            //*/


            Loading dlgLoading = new Loading();
            dlgLoading.Show();

            #region if 서보 disabled
            /*
            for (uint servoNum = 0; servoNum < GlobalVar.Axis_count; servoNum++)
            {
                if (GlobalVar.AxisStatus[servoNum].Errored ||
                        GlobalVar.AxisStatus[servoNum].Disabled ||
                        GlobalVar.AxisStatus[servoNum].ErrorStop)
                {
                    RTEX.ServoReset(servoNum);
                    RTEX.ServoOn(servoNum, true);
                }
            }
            //*/
            #endregion

            //int axisNum = 0;
            /*
            if (btnXAxis_sel)
            {
                RTEX.MoveHome((uint)axisNum, false, 1000); // 일단 X부터 홈 구동시킴
            }
            //*/
            //if (btnInputConveyor_sel)
            //{
            //    DIO.WriteDOData(FuncInline.enumDONames.Y01_0_, false);
            //}
            //try
            //{
            //    dlgLoading.Close();
            //}
            //catch { }

            //int convSpeed = 0;
            if (!GlobalVar.Simulation)
            {
                //convSpeed = Math.Abs(GlobalVar.WorkConv.GetDrvSpd(enumAxis.X)[0]);
            }
            //if (!DIO.GetDORead(FuncInline.enumDONames.Y01_0_))
            //{
            //    //GlobalVar.SystemInited = true;
            //    GlobalVar.SystemStatus = enumSystemStatus.Manual;
            //    GlobalVar.SystemMsg = "";
            //    this.BringToFront();
            //    FuncWin.TopMessageBox("System initialization finished.");
            //    return;
            //}

            this.BringToFront();
            FuncWin.TopMessageBox("System Initialization not finished.");
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            this.BringToFront();
            if (FuncWin.MessageBoxOK("Proceeding bypass initialization may cause serious system errors. \r\nDo you want to bypass the initialization process?"))
            {
                //GlobalVar.ServoInited = true;
                GlobalVar.SystemStatus = enumSystemStatus.Manual;
                this.Close();
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            btnInputConveyor_sel = false;
            btnWorkConveyor_sel = false;
            btnBufferConveyor_sel = false;
            btnXAxis_sel = false;
            btnYAxis_sel = false;
            btnZAxis_sel = false;
            btnWorkWidth_sel = false;
            btnInputWidth_sel = false;
            btnWorkTurn_sel = false;
            btnPCBClamp_sel = false;
            btnCleaner_sel = false;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            btnInputConveyor_sel = true;
            btnWorkConveyor_sel = true;
            btnBufferConveyor_sel = true;
            btnXAxis_sel = true;
            btnYAxis_sel = true;
            btnZAxis_sel = true;
            btnWorkWidth_sel = true;
            btnInputWidth_sel = true;
            btnWorkTurn_sel = true;
            btnPCBClamp_sel = true;
            btnCleaner_sel = true;
        }

        private void pnCleaner_Paint(object sender, PaintEventArgs e)
        {
            btnCleaner_sel = !btnCleaner_sel;
        }

        private void pbCleaner_Click(object sender, EventArgs e)
        {
            btnCleaner_sel = !btnCleaner_sel;
        }

       
    }
}
