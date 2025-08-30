using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Radix.Popup.Machine
{
    public partial class General : Form
    {
        #region 로컬변수
        //private System.Threading.Timer timerUI; // Thread Timer
        //private bool timerDoing = false;
        private bool valueChanged = false;
        private FuncInline.enumTabMain beforeMain = FuncInline.enumTabMain.Auto;
        private FuncInline.enumTabMachine beforeMachine = FuncInline.enumTabMachine.Machine;

        private bool firstShow = true; // 최초 실행시 컨트롤 값 변화 트래킹을 안 하기 위해
        public int debugCount = 0;

        #endregion
        public General()
        {
            InitializeComponent();
        }
        private void debug(string str)
        {
            Util.Debug(str);
        }
        #region 설정 관련
        public void LoadAllValue()
        {
            try
            {
                         
                #region general
                tbAppName.Text = GlobalVar.AppName;
                for (int i = 0; i < cmbCoCode.Items.Count; i++)
                {
                    if (cmbCoCode.Items[i].ToString().Substring(0, 1) == GlobalVar.CoCode)
                    {
                        cmbCoCode.SelectedIndex = i;
                        break;
                    }
                }
                numMachineNum.Value = (decimal)GlobalVar.MachineNumber;
                cmbMachineLeftRight.SelectedIndex = (int)GlobalVar.MachineLeftRight;
                cmbMachineFrontRear.SelectedIndex = (int)GlobalVar.MachineFrontRear;
                cmbMachineLinear.SelectedIndex = (int)GlobalVar.MachineLinear;
                numLogFileDelete.Value = (decimal)GlobalVar.LogFileDeleteDay;

                numLiftSpeed.Value = (decimal)FuncInline.LiftSpeed;
                numLiftAcc.Value = (decimal)FuncInline.LiftAccDec;
                numWidthHomeSpeed.Value = (decimal)GlobalVar.WidthHomSpeed;
                numWidthSpeed.Value = (decimal)GlobalVar.WidthSpeed;
                numWidthAcc.Value = (decimal)GlobalVar.WidthAccDec;

                numConveyorTimeout.Value = (decimal)FuncInline.ConveyorTimeout;
                numCommandTimeout.Value = (decimal)FuncInline.TestCommandTimeout;
                numCommandRetry.Value = (decimal)FuncInline.TestCommandRetry;
                numScanTimeout.Value = (decimal)FuncInline.ScanTimeout;
                numTestPassTime.Value = (decimal)FuncInline.TestPassTime;
                //cbNgAlarm.Checked = FuncInline.NgAlarm;
                numNgAlarmTime.Value = (decimal)FuncInline.NgAlarmTime;
                //numBlockNGCount.Value = (decimal)NGReEnterCount;
                btnBlockNGArray.BackColor = FuncInline.BlockNGArray ? Color.Lime : Color.White;
                btnBlockNGArrayNo.BackColor = !FuncInline.BlockNGArray ? Color.Lime : Color.White;
                numBlockNGCount.Value = (decimal)FuncInline.BlockNGCount;
                btnBlockDefectArray.BackColor = FuncInline.BlockDefectArray ? Color.Lime : Color.White;
                btnBlockDefectArrayNo.BackColor = !FuncInline.BlockDefectArray ? Color.Lime : Color.White;
                numDefectBlockMinIn.Value = (decimal)FuncInline.DefectBlockMinIn;
                numDefectBlockMinNG.Value = (decimal)FuncInline.DefectBlockMinNG;
                btnScanTwice.BackColor = FuncInline.ScanTwice ? Color.Lime : Color.White;
                btnScanTwiceNo.BackColor = !FuncInline.ScanTwice ? Color.Lime : Color.White;
                btnCheckCNDuplication.BackColor = FuncInline.CheckCNDuplication ? Color.Lime : Color.White;
                btnCheckCNDuplicationNo.BackColor = !FuncInline.CheckCNDuplication ? Color.Lime : Color.White;
                numCNDupCount.Value = (decimal)FuncInline.CheckCNDupeCount;
                btnCheckCNCross.BackColor = FuncInline.CheckCNCross ? Color.Lime : Color.White;
                btnCheckCNCrossNo.BackColor = !FuncInline.CheckCNCross ? Color.Lime : Color.White;
                btnScanInsertCheck.BackColor = FuncInline.ScanInsertCheck ? Color.Lime : Color.White;
                btnScanInsertCheckNo.BackColor = !FuncInline.ScanInsertCheck ? Color.Lime : Color.White;

                btnSelfRetest.BackColor = FuncInline.SelfRetest ? Color.Lime : Color.White;
                btnSelfRetestNo.BackColor = !FuncInline.SelfRetest ? Color.Lime : Color.White;
                btnOtherRetest.BackColor = FuncInline.OtherRetest ? Color.Lime : Color.White;
                btnOtherRetestNo.BackColor = !FuncInline.OtherRetest ? Color.Lime : Color.White;
                numSelfRetest.Value = (decimal)FuncInline.SelfRetestCount;
                numOtherRetest.Value = (decimal)FuncInline.OtherRetestCount;
                btnFailWhenNoEmpty.BackColor = FuncInline.FailWhenNoEmpty ? Color.Lime : Color.White;
                btnFailWhenNoEmptyNo.BackColor = !FuncInline.FailWhenNoEmpty ? Color.Lime : Color.White;
                btnNGToUnloading.BackColor = FuncInline.NGToUnloading ? Color.Lime : Color.White;
                btnNGToUnloadingNo.BackColor = !FuncInline.NGToUnloading ? Color.Lime : Color.White;
                btnPassToNG.BackColor = FuncInline.PassToNG ? Color.Lime : Color.White;
                btnPassToNGNo.BackColor = !FuncInline.PassToNG ? Color.Lime : Color.White;
                btnPinDownAndClamp.BackColor = FuncInline.PinDownAndClamp ? Color.Lime : Color.White;
                btnPinDownAndClampNo.BackColor = !FuncInline.PinDownAndClamp ? Color.Lime : Color.White;
                btnTestWithSiteUnclamp.BackColor = FuncInline.TestWithSiteUnclamp ? Color.Lime : Color.White;
                btnTestWithSiteUnclampNo.BackColor = !FuncInline.TestWithSiteUnclamp ? Color.Lime : Color.White;

                btnCoolingByTime.BackColor = FuncInline.CoolingByTime ? Color.Lime : Color.White;
                btnCoolingByTimeNo.BackColor = !FuncInline.CoolingByTime ? Color.Lime : Color.White;
                btnCoolingByTemperature.BackColor = FuncInline.CoolingByTemperature ? Color.Lime : Color.White;
                btnCoolingByTemperatureNo.BackColor = !FuncInline.CoolingByTemperature ? Color.Lime : Color.White;
                numCoolingTime.Value = (decimal)FuncInline.CoolingTime;
                numCoolingTemperature.Value = (decimal)FuncInline.CoolingTemperature;
                numCoolingMaxTime.Value = (decimal)FuncInline.CoolingMaxTime;

                DateTime dateTime = new DateTime(2022, 01, 01, FuncInline.ShiftAHour, FuncInline.ShiftAMin, 0);
                dateTimeShiftA.Value = dateTime;
                dateTime = new DateTime(2022, 01, 01, FuncInline.ShiftBHour, FuncInline.ShiftBMin, 0);
                dateTimeShiftB.Value = dateTime;
                dateTime = new DateTime(2022, 01, 01, FuncInline.ShiftCHour, FuncInline.ShiftCMin, 0);
                dateTimeShiftC.Value = dateTime;
                btnShiftC.BackColor = FuncInline.UseShiftC ? Color.Lime : Color.White;
                btnShiftCNo.BackColor = !FuncInline.UseShiftC ? Color.Lime : Color.White;

                numDefaultPCBWidth.Value = (decimal)FuncInline.DefaultPCBWidth;

                cbUseDoor.Checked = GlobalVar.UseDoor;

                // JHRYU
                tbSystemLogDirectory.Text = FuncInline.SystemLogPath;
                numSystemLogTime.Value = (decimal)FuncInline.SystemLogTime;
                #endregion

                numSiteClampDelay.Value = (decimal)FuncInline.SiteClampDelay;
                btnTestReady.BackColor = FuncInline.UseSMDReady ? Color.Lime : Color.White;
                btnTestReadyNo.BackColor = !FuncInline.UseSMDReady ? Color.Lime : Color.White;
                numWidthOffset.Value = (decimal)FuncInline.WidthClampOffset;

                btnPassModeBuffer.BackColor = FuncInline.PassModeBuffer ? Color.Lime : Color.White;
                btnPassModeBufferNo.BackColor = !FuncInline.PassModeBuffer ? Color.Lime : Color.White;
                btnLeaveOne.BackColor = FuncInline.LeaveOneSite ? Color.Lime : Color.White;
                btnLeaveOneNo.BackColor = !FuncInline.LeaveOneSite ? Color.Lime : Color.White;

                valueChanged = false;
                //firstShow = false;
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        public void ApplyAllValue()
        {
            try
            {
                
                #region General
                GlobalVar.AppName = tbAppName.Text;
                GlobalVar.CoCode = cmbCoCode.Text.Substring(0, 1);
                GlobalVar.MachineNumber = (int)numMachineNum.Value;
                GlobalVar.MachineLeftRight = (enumMachineLeftRight)cmbMachineLeftRight.SelectedIndex;
                GlobalVar.MachineFrontRear = (enumMachineFrontRear)cmbMachineFrontRear.SelectedIndex;
                GlobalVar.MachineLinear = (enumMachineLinear)cmbMachineLinear.SelectedIndex;
                GlobalVar.LogFileDeleteDay = (int)numLogFileDelete.Value;

                GlobalVar.WidthHomSpeed = (int)numWidthHomeSpeed.Value;
                GlobalVar.WidthSpeed = (int)numWidthSpeed.Value;
                GlobalVar.WidthAccDec = (int)numWidthAcc.Value;
                FuncInline.PinLifeCount = (int)numPinLifeCount.Value;
                FuncInline.LiftSpeed = (double)numLiftSpeed.Value;
                FuncInline.LiftAccDec = (double)numLiftAcc.Value;

                FuncInline.ConveyorTimeout = (int)numConveyorTimeout.Value;
                FuncInline.TestCommandTimeout = (int)numCommandTimeout.Value;
                FuncInline.TestCommandRetry = (int)numCommandRetry.Value;
                FuncInline.ScanTimeout = (int)numScanTimeout.Value;
                FuncInline.TestPassTime = (int)numTestPassTime.Value;
                FuncInline.NgAlarmTime = (int)numNgAlarmTime.Value;
                FuncInline.BlockNGArray = btnBlockNGArray.BackColor == Color.Lime;
                FuncInline.BlockNGCount = (int)numBlockNGCount.Value;
                FuncInline.BlockDefectArray = btnBlockDefectArray.BackColor == Color.Lime;
                FuncInline.DefectBlockMinIn = (int)numDefectBlockMinIn.Value;
                FuncInline.DefectBlockMinNG = (int)numDefectBlockMinNG.Value;
                FuncInline.ScanTwice = btnScanTwice.BackColor == Color.Lime;
                FuncInline.CheckCNDuplication = btnCheckCNDuplication.BackColor == Color.Lime;
                FuncInline.CheckCNDupeCount = (int)numCNDupCount.Value;
                FuncInline.CheckCNCross = btnCheckCNCross.BackColor == Color.Lime;
                FuncInline.ScanInsertCheck = btnScanInsertCheck.BackColor == Color.Lime;

                FuncInline.SelfRetest = btnSelfRetest.BackColor == Color.Lime;
                FuncInline.OtherRetest = btnOtherRetest.BackColor == Color.Lime;
                FuncInline.FailWhenNoEmpty = btnFailWhenNoEmpty.BackColor == Color.Lime;
                FuncInline.SelfRetestCount = (int)numSelfRetest.Value;
                FuncInline.OtherRetestCount = (int)numOtherRetest.Value;
                FuncInline.NGToUnloading = btnNGToUnloading.BackColor == Color.Lime;
                FuncInline.PassToNG = btnPassToNG.BackColor == Color.Lime;

                FuncInline.CoolingByTime = btnCoolingByTime.BackColor == Color.Lime;
                FuncInline.CoolingByTemperature = btnCoolingByTemperature.BackColor == Color.Lime;
                FuncInline.CoolingTime = (int)numCoolingTime.Value;
                FuncInline.CoolingTemperature = (int)numCoolingTemperature.Value;
                FuncInline.CoolingMaxTime = (int)numCoolingMaxTime.Value;

                FuncInline.ShiftAHour = dateTimeShiftA.Value.Hour;
                FuncInline.ShiftAMin = dateTimeShiftA.Value.Minute;
                FuncInline.ShiftBHour = dateTimeShiftB.Value.Hour;
                FuncInline.ShiftBMin = dateTimeShiftB.Value.Minute;
                FuncInline.ShiftCHour = dateTimeShiftC.Value.Hour;
                FuncInline.ShiftCMin = dateTimeShiftC.Value.Minute;

                FuncInline.DefaultPCBWidth = (double)numDefaultPCBWidth.Value;
                FuncInline.UseShiftC = btnShiftC.BackColor == Color.Lime;

                GlobalVar.UseDoor = cbUseDoor.Checked;

                FuncInline.SystemLogPath = tbSystemLogDirectory.Text;
                FuncInline.SystemLogTime = (int)numSystemLogTime.Value;

                FuncInline.SiteClampDelay = (double)numSiteClampDelay.Value;

                FuncInline.UseSMDReady = btnTestReady.BackColor == Color.Lime;
                FuncInline.WidthClampOffset = (double)numWidthOffset.Value;

                FuncInline.PassModeBuffer = btnPassModeBuffer.BackColor == Color.Lime;
                FuncInline.LeaveOneSite = btnLeaveOne.BackColor == Color.Lime;

                FuncInline.PinDownAndClamp = btnPinDownAndClamp.BackColor == Color.Lime;
                FuncInline.TestWithSiteUnclamp = btnTestWithSiteUnclamp.BackColor == Color.Lime;
                #endregion

                valueChanged = false;
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }
        #endregion

        private void General_Load(object sender, EventArgs e)
        {
            //debug("load");
        }

        private void General_Shown(object sender, EventArgs e)
        {
            //debug("shown");
            firstShow = false;
        }

        private void tmrPwd_Tick(object sender, EventArgs e)
        {
            //tmrPwd.Enabled = false;

            panel10.Visible = GlobalVar.PwdPass;
            //cbUseDoor.Visible = GlobalVar.PwdPass;
            //cbUseDoor.Checked = GlobalVar.UseDoor;
            lblMachineTitle.Text = "Machine Setting " + (GlobalVar.PwdPass ? "" : " - No Authority");

            #region 창 떠나면 관리자 권한 해제
            if (FuncInline.TabMain != FuncInline.enumTabMain.Machine ||
                FuncInline.TabMachine != FuncInline.enumTabMachine.Machine)
            {
                //debugCount = 0;
                //GlobalVar.PwdPass = false;
                //GlobalVar.PwdWatch.Stop();
                //GlobalVar.PwdWatch.Reset();
            }
            #endregion

            #region 값 수정 상태로 창 떠나면 알림
            if (valueChanged &&
                beforeMain == FuncInline.enumTabMain.Machine &&
                beforeMachine == FuncInline.enumTabMachine.Machine &&
                (FuncInline.TabMain != FuncInline.enumTabMain.Machine ||
                        FuncInline.TabMachine != FuncInline.enumTabMachine.Machine))
            {
                FuncInline.TabMain = FuncInline.enumTabMain.Machine;
                FuncInline.TabMachine = FuncInline.enumTabMachine.Machine;

                valueChanged = false;
                if (FuncWin.MessageBoxOK("Machine General Setting changed. Save?"))
                {
                    ApplyAllValue();
                    Func.SaveMachineIni();
                }
            }
            beforeMain = FuncInline.TabMain;
            beforeMachine = FuncInline.TabMachine;
            #endregion

            //if (!GlobalVar.GlobalStop)
            //{
            //    Thread.Sleep(GlobalVar.ThreadSleep);
            //    tmrPwd.Enabled = true;
            //}
            tbAppName.Enabled = debugCount >= 4;
            cmbCoCode.Enabled = debugCount >= 4;
            numMachineNum.Enabled = debugCount >= 4;
            cmbMachineLeftRight.Enabled = debugCount >= 4;

            numLiftSpeed.Enabled = debugCount >= 4;
            numLiftAcc.Enabled = debugCount >= 4;
            numConveyorTimeout.Enabled = debugCount >= 4;
            numDefaultPCBWidth.Enabled = debugCount >= 4;
        }

        private void cbUseDoor_CheckedChanged(object sender, EventArgs e)
        {
            //if (cbUseDoor.Checked)
            //{
            //    DIO.WriteDOData(FuncInline.enumDONames.Y00_6_Front_Door_Lock1, false);
            //    DIO.WriteDOData(FuncInline.enumDONames.Y00_7_Front_Door_Lock2, false);
            //    DIO.WriteDOData(FuncInline.enumDONames.Y01_0_Front_Door_Lock3, false);
            //}
            //if (!cbUseDoor.Checked)
            //{
            //    DIO.WriteDOData(FuncInline.enumDONames.Y00_6_Front_Door_Lock1, true);
            //    DIO.WriteDOData(FuncInline.enumDONames.Y00_7_Front_Door_Lock2, true);
            //    DIO.WriteDOData(FuncInline.enumDONames.Y01_0_Front_Door_Lock3, true);
            //}
            //GlobalVar.UseDoor = cbUseDoor.Checked;
        }

        private void cbCoolingByTime_CheckedChanged(object sender, EventArgs e)
        {
            /*
            if (cbCoolingByTime.Checked)
            {
                cbCoolingByTemperature.Checked = false;
            }
            //*/
        }

        private void cbCoolingByTemperature_CheckedChanged(object sender, EventArgs e)
        {
            /*
            if (cbCoolingByTemperature.Checked)
            {
                cbCoolingByTime.Checked = false;
            }
            //*/
        }
        
        private void ValueChanged(object sender, EventArgs e)
        {
            if (firstShow)
            {
                return;
            }
            if (sender.GetType() == typeof(NumericUpDown))
            {
                NumericUpDown ctr = (NumericUpDown)sender;
                FuncLog.WriteLog("Machine/General Value Change : " + ctr.Name + " ==> " + ctr.Value);
            }
            valueChanged = true;
        }

        private void tbPinLogDirectory_TextChanged(object sender, EventArgs e)
        {
            if (firstShow)
            {
                return;
            }
            FuncLog.WriteLog("Machine/General System Log Directory Change ==> " + tbSystemLogDirectory.Text);
            valueChanged = true;
        }

        private void tbPinLogDirectory_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                tbSystemLogDirectory.Text = dlg.SelectedPath;
            }
        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void btnTeachingMode_Click(object sender, EventArgs e)
        {
            string msg = "Set Teaching Mode ?";
            if (GlobalVar.TeachingMode)
            {
                msg = "Unset Teaching Mode ?";
            }
            if (FuncWin.MessageBoxOK(msg))
            {
                GlobalVar.TeachingMode = !GlobalVar.TeachingMode;
            }
            //btnTeachingMode.BackColor = GlobalVar.TeachingMode ? Color.Lime : Color.White;

            FuncInline.SetTeachingMode();
        }

        private void btnUse_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            if ((button.Name == "btnNGToUnloading" || button.Name == "btnpassToNG") &&
                btnNGToUnloading.BackColor == Color.Lime &&
                btnPassToNG.BackColor == Color.Lime)
            {
                FuncWin.TopMessageBox("Only one or no output option must be checked.");
                button.BackColor = Color.White;
                return;
            }

            button.BackColor = Color.Lime;

            Control[] controls = null;

            string use = "USE";
            // 사용안함
            if (button.Name.EndsWith("No"))
            {
                use = "NotUse";
                controls = Controls.Find(button.Name.Substring(0, button.Name.Length - 2), true);
            }
            // 사용함
            else
            {
                controls = Controls.Find(button.Name + "No", true);
            }
            if (controls.Length > 0)
            {
                ((Button)controls[0]).BackColor = Color.White;
            }

            if (sender.GetType() == typeof(Button))
            {
                Button ctr = (Button)sender;
                FuncLog.WriteLog("Machine/General Value Change : " + ctr.Name + " ==> " + use);
            }
            if (firstShow)
            {
                return;
            }
            valueChanged = true;
        }

        private void label8_Click(object sender, EventArgs e)
        {
            //debugCount = 1;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            if (debugCount == 1)
            {
                //debugCount = 2;
            }
            else
            {
                //debugCount = 0;
            }
        }

        private void label43_Click(object sender, EventArgs e)
        {
            if (debugCount == 2)
            {
                //debugCount = 3;
            }
            else
            {
                //debugCount = 0;
            }
        }

        private void label61_Click(object sender, EventArgs e)
        {
            if (debugCount == 3)
            {
                //debugCount = 4;
            }
            else
            {
                //debugCount = 0;
            }
        }

        private void panel3_Click(object sender, EventArgs e)
        {
            debugCount++;
        }


    }
}
