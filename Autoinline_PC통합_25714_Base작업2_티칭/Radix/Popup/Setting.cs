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

namespace Radix
{
    /*
     * Setting.cs : 각종 옵션 설정 관리
     */

    public partial class Setting : Form
    {
        private System.Threading.Timer timerUI; // Thread Timer
        private bool timerDoing = false;

        private bool jogX = false;
        private bool jogXUp = false;
        private bool jogXDown = false;

        private bool widthWorking = false;
        private ulong widthWorkingTick = GlobalVar.TickCount64;

        private bool remarking = false; // 리마킹 중복 실행 막기 위한 체크변수

        public Setting()
        {
            InitializeComponent();


        }

        private void debug(string str)
        {
            Util.Debug(str);
        }

        private void Setting_Shown(object sender, EventArgs e)
        {
            GlobalVar.dlgOpened = true;
            GlobalVar.PwdPass = true;

            loadModelList();
            //tbModel.Text = GlobalVar.ModelName;
            LoadAllValue();

            cbUseDoor.Checked = GlobalVar.UseDoor;

            #region 화면 제어용 쓰레드 타이머 시작
            //*
            TimerCallback CallBackUI = new TimerCallback(TimerUI);
            timerUI = new System.Threading.Timer(CallBackUI, false, 0, 100);
            //*/
            #endregion

            #region 비전 연동용 프로그램 구동
            /*
            if (FuncWin.FindWindowByName(GlobalVar.Vision_App_Title) == IntPtr.Zero)
            {
                FuncWin.RunProgram(GlobalVar.Vision_App_Path);
                Thread.Sleep(1000);
            }
            Func.SendVisionJob(enumVisionCmd.Viewer);
            //*/
            #endregion



            #region Robot Speed & Offset View
            //numOffset_Working_Put_8_Z.Visible = true;
            #endregion

            this.BringToFront();
        }

        private void LoadAllValue()
        {
            try
            {
          

                //tbPasswd.Text = GlobalVar.ManagePasswd;

                #region general
                //tbVersion.Text = GlobalVar.SwVersion;
                //tbAppName.Text = GlobalVar.AppName;
                //tbDepart.Text = GlobalVar.DepartName;
                //cbUseFluid.Checked = GlobalVar.Use_Fluid;

                //numNormalTimeout.Value = (decimal)GlobalVar.NormalTimeout;


                //cmbTransferTimeout.SelectedIndex = GlobalVar.Transfer_Timeout_Error ? 1 : 0;

                //for (int i = 0; i < cmbLoadcellPort.Items.Count; i++)
                //{
                //    if (GlobalVar.LoadcellPort == (string)cmbLoadcellPort.Items[i])
                //    {
                //        cmbLoadcellPort.SelectedIndex = i;
                //    }
                //}

                #endregion



       


                #region tower lamp
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        Controls.Find("btnTower" + i.ToString() + "_" + j.ToString(), true)[0].BackColor = GlobalVar.TowerAction[i, j, (int)enumTowerLampAction.Enable] ? Color.Yellow : Color.WhiteSmoke;
                        Controls.Find("btnTower" + i.ToString() + "_" + j.ToString(), true)[0].Text = GlobalVar.TowerAction[i, j, (int)enumTowerLampAction.Enable] ? "ON" : "OFF";
                        Controls.Find("btnBlink" + i.ToString() + "_" + j.ToString(), true)[0].BackColor = GlobalVar.TowerAction[i, j, (int)enumTowerLampAction.Blink] ? Color.Yellow : Color.WhiteSmoke;
                    }
                    ((NumericUpDown)(Controls.Find("numTower" + i.ToString(), true)[0])).Value = (decimal)GlobalVar.TowerTime[i];
                }
                #endregion

            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }

        }

        private void ApplyAllValue()
        {
            try
            {
        

                //GlobalVar.ManagePasswd = tbPasswd.Text;

                #region General
                //GlobalVar.SwVersion = tbVersion.Text;
                //GlobalVar.AppName = tbAppName.Text;
                //GlobalVar.DepartName = tbDepart.Text;
                //GlobalVar.Use_Fluid = cbUseFluid.Checked;


                //GlobalVar.NormalTimeout = (int)numNormalTimeout.Value;                       

                //GlobalVar.LoadcellPort = cmbLoadcellPort.Items[cmbLoadcellPort.SelectedIndex].ToString();
                //GlobalVar.LoadcellBaud = int.Parse(cmbLoadcellBaud.Items[cmbLoadcellBaud.SelectedIndex].ToString());
                #endregion



        


                #region tower lamp
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        GlobalVar.TowerAction[i, j, (int)enumTowerLampAction.Enable] = Controls.Find("btnTower" + i.ToString() + "_" + j.ToString(), true)[0].BackColor != SystemColors.Control &&
                                                         Controls.Find("btnTower" + i.ToString() + "_" + j.ToString(), true)[0].BackColor != Color.WhiteSmoke;
                        GlobalVar.TowerAction[i, j, (int)enumTowerLampAction.Blink] = Controls.Find("btnBlink" + i.ToString() + "_" + j.ToString(), true)[0].BackColor != SystemColors.Control &&
                                                         Controls.Find("btnBlink" + i.ToString() + "_" + j.ToString(), true)[0].BackColor != Color.WhiteSmoke;
                    }
                    GlobalVar.TowerTime[i] = (ulong)((NumericUpDown)(Controls.Find("numTower" + i.ToString(), true)[0])).Value;
                }
                #endregion
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            ///*
            this.BringToFront();
            if (FuncWin.MessageBoxOK(" Apply All Value?"))
            {
                //GlobalVar.ModelName = tbModel.Text; // YJ20210923 모델 적용시 모델명이 변경되지 않아서 추가함
                ApplyAllValue();
            }
            //*/
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //*
            this.BringToFront();
            if (FuncWin.MessageBoxOK(" Cancel All Value?"))
            {
                LoadAllValue();
            }
            //*/
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            this.BringToFront();
            if (FuncWin.MessageBoxOK(" Load All Values?"))
            {
                Func.LoadAllIni();

                LoadAllValue();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //*
            if (FuncWin.MessageBoxOK(" Save All Values?"))
            {
                //GlobalVar.ModelName = tbModel.Text;
                ApplyAllValue();

                Func.SaveAllIni();

                //if (!cmbModelList.Items.Contains(tbModel.Text))
                //{
                //    cmbModelList.Items.Add(tbModel.Text);
                //    cmbModelList.Text = tbModel.Text;
                //}
            }
            //*/
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.BringToFront();
            if (FuncWin.MessageBoxOK("Close?"))
            {
                this.Close();
            }
        }



        private void LoadTowerAction()
        {
            for (int i = 0; i < 8; i++)
            {
                Controls.Find("btnTower" + i.ToString() + "_0", true)[0].BackColor = GlobalVar.TowerAction[i, 0, (int)enumTowerLampAction.Enable] ? Color.Red : Color.WhiteSmoke;
                Controls.Find("btnBlink" + i.ToString() + "_0", true)[0].BackColor = GlobalVar.TowerAction[i, 0, (int)enumTowerLampAction.Blink] ? Color.Green : Color.WhiteSmoke;
                Controls.Find("btnTower" + i.ToString() + "_1", true)[0].BackColor = GlobalVar.TowerAction[i, 1, (int)enumTowerLampAction.Enable] ? Color.Yellow : Color.WhiteSmoke;
                Controls.Find("btnBlink" + i.ToString() + "_1", true)[0].BackColor = GlobalVar.TowerAction[i, 1, (int)enumTowerLampAction.Blink] ? Color.Green : Color.WhiteSmoke;
                Controls.Find("btnTower" + i.ToString() + "_2", true)[0].BackColor = GlobalVar.TowerAction[i, 2, (int)enumTowerLampAction.Enable] ? Color.LightGreen : Color.WhiteSmoke;
                Controls.Find("btnBlink" + i.ToString() + "_2", true)[0].BackColor = GlobalVar.TowerAction[i, 2, (int)enumTowerLampAction.Blink] ? Color.Green : Color.WhiteSmoke;
                Controls.Find("btnTower" + i.ToString() + "_3", true)[0].BackColor = GlobalVar.TowerAction[i, 3, (int)enumTowerLampAction.Enable] ? Color.DarkGray : Color.WhiteSmoke;
                Controls.Find("btnBlink" + i.ToString() + "_3", true)[0].BackColor = GlobalVar.TowerAction[i, 3, (int)enumTowerLampAction.Blink] ? Color.Green : Color.WhiteSmoke;
                ((NumericUpDown)(Controls.Find("numTower" + i.ToString(), true)[0])).Value = GlobalVar.TowerTime[i];
            }
        }

        private void Setting_FormClosed(object sender, FormClosedEventArgs e)
        {
            GlobalVar.SettingClose = true;
            timerUI.Dispose();
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

        private void TimerUI(Object state) // 화면 제어 쓰레드 타이머 함수
        {

            try
            {
                if (timerDoing)
                {
                    return;
                }

                timerDoing = true;

                /* 화면 변경 timer */
                this.Invoke(new MethodInvoker(delegate ()
                {

                    #region 관리자 여부에 따라 컨트롤 보이기/숨기기
                    cbUseDoor.Visible = GlobalVar.PwdPass;
                    //tbAppName.Enabled = GlobalVar.PwdPass;
                    //numMachineNum.Enabled = GlobalVar.PwdPass;
                    pbSave.Visible = GlobalVar.PwdPass;
                    //btnTower0_0.Enabled = GlobalVar.PwdPass;
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            Controls.Find("btnTower" + i.ToString() + "_" + j.ToString(), true)[0].Enabled = GlobalVar.PwdPass;
                            Controls.Find("btnBlink" + i.ToString() + "_" + j.ToString(), true)[0].Enabled = GlobalVar.PwdPass;
                        }
                    }
                    #endregion


                }));

                timerDoing = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                //debug(ex.StackTrace);
            }


        }

        private void btnTower_Click(object sender, EventArgs e)
        {
            ((Button)sender).BackColor = ((Button)sender).BackColor == Color.Yellow ? Color.WhiteSmoke : Color.Yellow;
        }

        private void cbUseDoor_CheckedChanged(object sender, EventArgs e)
        {
            GlobalVar.UseDoor = cbUseDoor.Checked;
            //HJ 수정 200408 도어 사용 유무를 체크에 따른 로그 남기기
            if (!cbUseDoor.Checked)
            {
                FuncLog.WriteLog("UserDoor Unchecked Click");
            }
            else
            {
                FuncLog.WriteLog("UserDoor checked Click");
            }

        }


        private void pbGeneral_Click(object sender, EventArgs e)
        {
            tcSetting.SelectedIndex = 0;
            pbGeneral.BackgroundImage = Radix.Properties.Resources.tab_general_info;
            pbTower.BackgroundImage = Radix.Properties.Resources.tab_tower_bright;
        }

        private void pbTower_Click(object sender, EventArgs e)
        {
            tcSetting.SelectedIndex = 1;
            pbGeneral.BackgroundImage = Radix.Properties.Resources.tab_general_bright;
            pbTower.BackgroundImage = Radix.Properties.Resources.tab_tower_lamp;
        }

        private void pbHidden_Click(object sender, EventArgs e)
        {
            if (GlobalVar.PwdPass)
            {
                return;
            }
            Password dlg = new Password();
            dlg.ShowDialog();
        }

        private void tpModel_Click(object sender, EventArgs e)
        {
            //Func.Remark();
        }

        private void Setting_Load(object sender, EventArgs e)
        {
            try
            {
                //cmbLoadcellPort.Items.AddRange(SerialPort.GetPortNames());
                //cmbLoadcellBaud.SelectedIndex = 3;

                //if (cmbLoadcellPort.Items.Count > 1)
                //{
                //    cmbLoadcellPort.SelectedIndex = 0;  // 컴포트 정보가 없을 경우 컴포트의 1번째를 사용
                //}
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }

        }

        #region Model 관리
        private void loadModelList()
        {
            //cmbModelList.Items.Clear();
            //String settingPath = GlobalVar.FaPath;
            //if (!Directory.Exists(settingPath))
            //{
            //    return;
            //}
            //settingPath += "\\" + GlobalVar.SWName;// + "\\" + GlobalVar.IniPath;
            //if (!Directory.Exists(settingPath))
            //{
            //    return;
            //}
            //settingPath += "\\" + GlobalVar.ModelPath;
            //if (!Directory.Exists(settingPath))
            //{
            //    return;
            //}
            //System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(settingPath);
            //foreach (System.IO.FileInfo File in di.GetFiles())
            //{
            //    if (File.Extension.ToLower().CompareTo(".ini") == 0)
            //    {
            //        String FileNameOnly = File.Name.Substring(0, File.Name.Length - 4);
            //        cmbModelList.Items.Add(FileNameOnly);
            //    }
            //}
        }
        private void cmbModelList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (FuncWin.MessageBoxOK(cmbModelList.SelectedText + " 모델 설정을 로드하시겠습니까?"))
            //{
            //    tbModel.Text = cmbModelList.SelectedText;
            //    GlobalVar.ModelName = tbModel.Text;
            //    Util.SaveAllIni();
            //    Util.LoadAllIni();
            //    LoadAllValue();
            //}
        }


        private void btnDelete_Click(object sender, EventArgs e) // YJ20210923 삭제코드 막혀 있어서 일단 살리고 SECS 통신 추가함. 삭제처리 디버깅 필요.
        {
            /*
            if (FuncWin.MessageBoxOK(cmbModelList.SelectedText + " 모델 설정을 삭제하시겠습니까?\n삭제시 설정을 복원할 수 없습니다."))
            {
                string settingPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\" + cmbModelList.Items[cmbModelList.SelectedIndex] + ".ini";
                if (File.Exists(settingPath))
                {
                    File.Delete(settingPath);
                    cmbModelList.Items.RemoveAt(cmbModelList.SelectedIndex);

                    settingPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\DefaultModel.ini";
                    if (File.Exists(settingPath))
                    {
                        // 기본 모델 로드
                        for (int i = 0; i < cmbModelList.Items.Count; i++)
                        {
                            if (cmbModelList.Items[i].ToString() == "DefaultModel")
                            {
                                cmbModelList.SelectedIndex = i;
                                break;
                            }
                        }
                        tbModel.Text = "DefaultModel";
                        GlobalVar.ModelName = tbModel.Text;
                        Util.LoadAllIni();
                        LoadAllValue();
                    }
                    else if (cmbModelList.Items.Count > 0)
                    {
                        // 삭제 후 첫 모델 로드
                        cmbModelList.SelectedIndex = 0;
                        tbModel.Text = cmbModelList.SelectedText;
                        GlobalVar.ModelName = tbModel.Text;
                        Util.LoadAllIni();
                        LoadAllValue();
                    }
                    GlobalVar.Secs.SendRecipeList(); // 20210723 모델목록 변경시 전송함
                }
            }
            //*/
        }


        #endregion




    }
}
