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

    public partial class Model : Form
    {
        private System.Threading.Timer timerUI; // Thread Timer
        private bool timerUIDoing = false;

        //private bool jogX = false;
        //private bool jogXUp = false;
        //private bool jogXDown = false;

        //private bool widthWorking = false;
        //private int widthWorkingTick = Environment.TickCount;

        //private bool remarking = false; // 리마킹 중복 실행 막기 위한 체크변수
        private int modelCount = 0;
        private bool valueChanged = false;
        private FuncInline.enumTabMain beforeMain = FuncInline.enumTabMain.Auto;

        private bool firstShow = true; // 최초 실행시 컨트롤 값 변화 트래킹을 안 하기 위해
        private int debugCount = 0;

        public Model()
        {
            InitializeComponent();


        }

        private void debug(string str)
        {
            Util.Debug(str);
        }



        public void RefreshArrayImage()
        {
            FuncInline.LoadArrayImage();
            //string imagePath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.ArrayImagePath + "\\" + GlobalVar.ModelName + ".jpg";
            //FuncInline.ArrayBitmap = FuncScreen.LoadBitmap(imagePath);
            pbArrayImage.BackgroundImage = FuncInline.ArrayBitmap;
            /*
            if (GlobalVar.ArrayImagePath.Length > 0 &&
                File.Exists(imagePath))
            {
                pbArrayImage.BackgroundImage = Image.FromFile(imagePath);
                lblArrayImage.Text = FuncInline.ArrayImage;
            }
            else
            {
                pbArrayImage.BackgroundImage = Properties.Resources.no_image;
                lblArrayImage.Text = "";
            }
            //*/
        }

        private void Setting_Shown(object sender, EventArgs e)
        {
            try
            {
                GlobalVar.dlgOpened = true;
                //GlobalVar.PwdPass = true;

                loadModelList();
                //tbModel.Text = GlobalVar.ModelName;
                LoadAllValue();

                string imagePath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.ArrayImagePath + "\\" + GlobalVar.ModelName + ".bmp";
                if (GlobalVar.ArrayImagePath.Length > 0 &&
                    File.Exists(imagePath))
                {
                    pbArrayImage.BackgroundImage = Image.FromFile(imagePath);
                    lblArrayImage.Text = FuncInline.ArrayImage;
                }
                else
                {
                    pbArrayImage.BackgroundImage = Properties.Resources.no_image;
                    lblArrayImage.Text = "";
                }

                string tempPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.ArrayImagePath + "\\Array1.bmp";
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }

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
                    Util.USleep(1000);
                }
                Func.SendVisionJob(enumVisionCmd.Viewer);
                //*/
                #endregion



                #region Robot Speed & Offset View
                //numOffset_Working_Put_8_Z.Visible = true;
                #endregion


                for (int i = 0; i < dataGridModelList.Rows.Count; i++)
                {
                    if (dataGridModelList.Rows[i].Cells[1].ToString() == GlobalVar.ModelName)
                    {
                        dataGridModelList.Rows[i].Selected = true;
                        break;
                    }
                }

              

                this.BringToFront();
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        public void LoadAllValue()
        {
            try
            {

                numArrayCount.Value = (decimal)FuncInline.ArrayCount;
                //lblArrayImage.Text = FuncInline.ArrayImage;
                numPCBWidth.Value = (decimal)FuncInline.PCBWidth;
                numPCBLength.Value = (decimal)FuncInline.PCBLength;
                numTestTimeout.Value = (decimal)FuncInline.TestTimeout;

                cmbTestType.SelectedIndex = (int)FuncInline.TestType;

                cbPinUseFront.Checked = FuncInline.PinUseFront;
                cbPinUseRear.Checked = FuncInline.PinUseRear;

                /*
                for (int i = 0; i < FuncInline.ArrayUse.Length; i++)
                {
                    ((CheckBox)Controls.Find("cbArray" + (i + 1), true)[0]).Checked = FuncInline.ArrayUse[i];
                }
                for (int i = 0; i < FuncInline.XOut.Length; i++)
                {
                    ((CheckBox)Controls.Find("cbXOut" + (i + 1), true)[0]).Checked = FuncInline.XOut[i];
                }
                //*/
                btnArray1.BackColor = FuncInline.ArrayUse[0] ? Color.Lime : Color.White;
                btnArray2.BackColor = FuncInline.ArrayUse[1] ? Color.Lime : Color.White;
                btnArray3.BackColor = FuncInline.ArrayUse[2] ? Color.Lime : Color.White;
                btnArray4.BackColor = FuncInline.ArrayUse[3] ? Color.Lime : Color.White;
                btnArray5.BackColor = FuncInline.ArrayUse[4] ? Color.Lime : Color.White;
                btnArray6.BackColor = FuncInline.ArrayUse[5] ? Color.Lime : Color.White;
                btnXOut1.BackColor = FuncInline.XOut[0] ? Color.Lime : Color.White;
                btnXOut2.BackColor = FuncInline.XOut[1] ? Color.Lime : Color.White;
                btnXOut3.BackColor = FuncInline.XOut[2] ? Color.Lime : Color.White;
                btnXOut4.BackColor = FuncInline.XOut[3] ? Color.Lime : Color.White;
                btnXOut5.BackColor = FuncInline.XOut[4] ? Color.Lime : Color.White;
                btnXOut6.BackColor = FuncInline.XOut[5] ? Color.Lime : Color.White;

                btnXoutBySelection.BackColor = FuncInline.XoutBySelection ? Color.Lime : Color.White;
                btnXoutBySelectionNo.BackColor = !FuncInline.XoutBySelection ? Color.Lime : Color.White;
                btnXoutByVision.BackColor = FuncInline.XoutByVision ? Color.Lime : Color.White;
                btnXoutByVisionNo.BackColor = !FuncInline.XoutByVision ? Color.Lime : Color.White;
                btnUseBadMark.BackColor = FuncInline.UseBadMark ? Color.Lime : Color.White;
                btnUseBadMarkNo.BackColor = !FuncInline.UseBadMark ? Color.Lime : Color.White;
                //cbSkipWhenBadMarkExist.Checked = GlobalVar.AutoInline_BadMarkWhenExist;
                btnXOutToNG.BackColor = FuncInline.XoutToNG ? Color.Lime : Color.White;
                btnXOutToNGNo.BackColor = !FuncInline.XoutToNG ? Color.Lime : Color.White;
                btnBadMarkToNG.BackColor = FuncInline.BadMarkToNG ? Color.Lime : Color.White;
                btnBadMarkToNGNo.BackColor = !FuncInline.BadMarkToNG ? Color.Lime : Color.White;
                btnLongPCBTest.BackColor = FuncInline.LongPCBTest ? Color.Lime : Color.White;
                btnLongPCBTestNo.BackColor = !FuncInline.LongPCBTest ? Color.Lime : Color.White;
                

                #region 폭 티칭 계산
                for (int i = 0; i < FuncInline.OffsetWidth.Length; i++)
                {

                    FuncInline.TeachingWidth[i] = FuncInline.PCBWidth + FuncInline.OffsetWidth[i]; // apply건 load건 티칭계산이 다시 되었으면 티칭좌표에 적용
                }
                #endregion

                for (int i = 0; i < dataGridModelList.Rows.Count; i++)
                {
                    if (dataGridModelList.Rows[i].Cells[1].Value.ToString() == GlobalVar.ModelName)
                    {
                        dataGridModelList.Rows[i].Selected = true;
                        break;
                    }
                }

                firstShow = false;
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
                FuncInline.ArrayCount = (int)numArrayCount.Value;
                FuncInline.ArrayImage = lblArrayImage.Text;
                FuncInline.PCBWidth = (double)numPCBWidth.Value;
                FuncInline.PCBLength = (double)numPCBLength.Value;
                FuncInline.TestTimeout = (int)numTestTimeout.Value;
                FuncInline.TestType = (FuncInline.enumTestType)cmbTestType.SelectedIndex;

                FuncInline.PinUseFront = cbPinUseFront.Checked;
                FuncInline.PinUseRear = cbPinUseRear.Checked;

                /*
                for (int i = 0; i < FuncInline.ArrayUse.Length; i++)
                {
                    FuncInline.ArrayUse[i] = ((CheckBox)Controls.Find("cbArray" + (i + 1), true)[0]).Checked;
                }

                for (int i = 0; i < FuncInline.XOut.Length; i++)
                {
                    FuncInline.XOut[i] = ((CheckBox)Controls.Find("cbXOut" + (i + 1), true)[0]).Checked;
                }
                //*/
                FuncInline.ArrayUse[0] = btnArray1.BackColor == Color.Lime;
                FuncInline.ArrayUse[1] = btnArray2.BackColor == Color.Lime;
                FuncInline.ArrayUse[2] = btnArray3.BackColor == Color.Lime;
                FuncInline.ArrayUse[3] = btnArray4.BackColor == Color.Lime;
                FuncInline.ArrayUse[4] = btnArray5.BackColor == Color.Lime;
                FuncInline.ArrayUse[5] = btnArray6.BackColor == Color.Lime;
                FuncInline.XOut[0] = btnXOut1.BackColor == Color.Lime;
                FuncInline.XOut[1] = btnXOut2.BackColor == Color.Lime;
                FuncInline.XOut[2] = btnXOut3.BackColor == Color.Lime;
                FuncInline.XOut[3] = btnXOut4.BackColor == Color.Lime;
                FuncInline.XOut[4] = btnXOut5.BackColor == Color.Lime;
                FuncInline.XOut[5] = btnXOut6.BackColor == Color.Lime;


                FuncInline.XoutBySelection = btnXoutBySelection.BackColor == Color.Lime;
                FuncInline.XoutByVision = btnXoutByVision.BackColor == Color.Lime;
                FuncInline.UseBadMark = btnUseBadMark.BackColor == Color.Lime;
                //GlobalVar.AutoInline_BadMarkWhenExist = cbSkipWhenBadMarkExist.Checked;
                FuncInline.XoutToNG = btnXOutToNG.BackColor == Color.Lime;
                FuncInline.BadMarkToNG = btnBadMarkToNG.BackColor == Color.Lime;
                //FuncInline.CalcWidthOffset();
                FuncInline.LongPCBTest = btnLongPCBTest.BackColor == Color.Lime;

                #region 폭 티칭 계산
                for (int i = 0; i < FuncInline.OffsetWidth.Length; i++)
                {
                    FuncInline.TeachingWidth[i] = FuncInline.PCBWidth + FuncInline.OffsetWidth[i]; // apply건 load건 티칭계산이 다시 되었으면 티칭좌표에 적용
                }
                #endregion



            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }


        private void Setting_FormClosed(object sender, FormClosedEventArgs e)
        {
            //if (this.Parent != null)
            //{
            try
            {
                GlobalVar.SettingClose = true;
                GlobalVar.dlgOpened = false;
                //timerUI.Dispose();
                //this.Parent.BringToFront();
            }
            catch
            { }
            //}
        }

        private void TimerUI(Object state) // 화면 제어 쓰레드 타이머 함수
        {
            if (FuncInline.TabMain != FuncInline.enumTabMain.Model)
            {
                debugCount = 0;
            }
            #region 값 수정 상태로 창 떠나면 알림
            if (valueChanged &&
                beforeMain == FuncInline.enumTabMain.Model &&
                (FuncInline.TabMain != FuncInline.enumTabMain.Model))
            {
                FuncInline.TabMain = FuncInline.enumTabMain.Model;

                valueChanged = false;
                if (FuncWin.MessageBoxOK("Model Setting changed. Save?"))
                {
                    ApplyAllValue();
                    Func.SaveModelIni();
                    Func.SaveTeachingIni();
                    Func.SaveMachineIni(); // 이미지 영역 위해

                    // 모델이미지 등 데이터 날아가는 경우 있어 다시 로드한다.
                    Func.LoadModelIni();
                    //Func.LoadTeachingIni();
                    LoadAllValue();
                    RefreshArrayImage();
                }
            }
            beforeMain = FuncInline.TabMain;
            #endregion


            if (FuncInline.TabMain != FuncInline.enumTabMain.Model) // 메인 탭이 다른 곳에 있으면 실행 안 한다.
            {
                timerUIDoing = false;
                return;
            }
            try
            {
                if (timerUIDoing)
                {
                    return;
                }
                timerUIDoing = true;
                //timerUI.Dispose();

                Util.StartWatch(ref GlobalVar.PwdWatch); // 교시모드 타이머 연장
                /* 화면 변경 timer */
                if (!GlobalVar.GlobalStop &&
                    this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate ()
                {
                 

                    //tbAppName.Enabled = GlobalVar.PwdPass;
                    //numMachineNum.Enabled = GlobalVar.PwdPass;

                    //pbArrayImage.BackgroundImage = FuncInline.ArrayBitmap;

                    #region 현재 모델 배경색
                    for (int i = 0; i < dataGridModelList.Rows.Count; i++)
                    {
                        if (GlobalVar.ModelName == dataGridModelList.Rows[i].Cells[1].Value.ToString())
                        {
                            dataGridModelList.Rows[i].DefaultCellStyle.BackColor = Color.Lime;
                        }
                        else
                        {
                            dataGridModelList.Rows[i].DefaultCellStyle.BackColor = Color.White;
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





        #region Model 관리
        private void loadModelList()
        {
            try
            {
                dataGridModelList.Rows.Clear();
                String settingPath = GlobalVar.FaPath;
                if (!Directory.Exists(settingPath))
                {
                    return;
                }
                settingPath += "\\" + GlobalVar.SWName;// + "\\" + GlobalVar.IniPath;
                if (!Directory.Exists(settingPath))
                {
                    return;
                }
                settingPath += "\\" + GlobalVar.ModelPath;
                if (!Directory.Exists(settingPath))
                {
                    return;
                }
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(settingPath);

                modelCount = 0;
                foreach (System.IO.FileInfo File in di.GetFiles())
                {
                    if (File.Extension.ToLower().CompareTo(".ini") == 0)
                    {
                        modelCount++;
                        String FileNameOnly = File.Name.Substring(0, File.Name.Length - 4);
                        dataGridModelList.Rows.Add(modelCount.ToString(), FileNameOnly);
                    }
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }




        #endregion

        private void pbLoad_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridModelList.Rows.Count == 0 ||
                    dataGridModelList.SelectedCells.Count == 0 ||
                    dataGridModelList.SelectedCells[0].RowIndex < 0 ||
                    dataGridModelList.SelectedCells[0].RowIndex >= modelCount)
                {
                    FuncWin.TopMessageBox("Select Model first!");
                    return;
                }
                string modelName = dataGridModelList.SelectedRows[0].Cells[1].Value.ToString();

                #region PCB 존재하고 PCB 폭이 다르면 변경 금지
                if (Func.LoadModelWidth(modelName) != FuncInline.PCBWidth)
                {
                    if (FuncInline.CheckPCBInMachine())
                    {
                        FuncWin.TopMessageBox("Can't change model in different width while PCB in machine.");
                        return;
                    }
                    if (GlobalVar.SystemStatus <= enumSystemStatus.Initialize) // 초기화되어 있지 않으면 폭조절 금지
                    {
                        FuncWin.TopMessageBox("Run System Origin first to change PCB width.");
                        return;
                    }
                }
                #endregion

                #region PCB 폭이 다르면 폭 조절
                bool widthChanged = Func.LoadModelWidth(modelName) != FuncInline.PCBWidth;
                #endregion

                if (FuncWin.MessageBoxOK("Load Model Setting named " + modelName))
                {
                    FuncLog.WriteLog("Model - Load " + modelName);
                    GlobalVar.ModelName = modelName;
                    Func.LoadModelIni();
                    Func.LoadTeachingIni();
                    LoadAllValue();
                    RefreshArrayImage();


                    //string modelStr = "";
                    /*
                    for (int i = 1; i <= GlobalVar.AutoInline_MaxArrayCount; i++)
                    {
                        Control[] arrayControl = Controls.Find("cbArray" + i, true);
                        Control[] xoutControl = Controls.Find("cbXOut" + i, true);
                        //if (arrayControl.Length > 1 &&
                        //    xoutControl.Length > 1)
                        //{
                        try
                        {
                            if (((CheckBox)arrayControl[0]).Checked)
                            {
                                modelStr += "$" + i.ToString() + ",O,O";
                            }
                        }
                        catch (Exception ex)
                        {
                            debug(ex.ToString());
                            debug(ex.StackTrace);
                        }
                        //}
                    }
                    //*/
                    /*
                    if (btnArray1.BackColor == Color.Lime)
                    {
                        modelStr += "$" + 1.ToString() + ",O,O";
                    }
                    if (btnArray2.BackColor == Color.Lime)
                    {
                        modelStr += "$" + 2.ToString() + ",O,O";
                    }
                    if (btnArray3.BackColor == Color.Lime)
                    {
                        modelStr += "$" + 3.ToString() + ",O,O";
                    }
                    if (btnArray4.BackColor == Color.Lime)
                    {
                        modelStr += "$" + 4.ToString() + ",O,O";
                    }
                    if (btnArray5.BackColor == Color.Lime)
                    {
                        modelStr += "$" + 5.ToString() + ",O,O";
                    }
                    if (btnArray6.BackColor == Color.Lime)
                    {
                        modelStr += "$" + 6.ToString() + ",O,O";
                    }
                    //*/
                    string modelStr = "$" + (FuncInline.ScanInsertCheck ? "O" : "X"); // Fiducial
                    modelStr += "$" + (FuncInline.XoutByVision ? "O" : "X"); // XOUt
                    modelStr += "$X"; // OCR
                    modelStr += "$" + (FuncInline.UseBadMark ? "O" : "X"); // GoodMark
                    //modelStr += "$X"; // BadMark  // 통합될 예정
                    modelStr += "$" + (btnArray1.BackColor == Color.Lime ? "1" : "0");
                    modelStr += "$" + (btnArray2.BackColor == Color.Lime ? "2" : "0");
                    modelStr += "$" + (btnArray3.BackColor == Color.Lime ? "3" : "0");
                    modelStr += "$" + (btnArray4.BackColor == Color.Lime ? "4" : "0");
                    modelStr += "$" + (btnArray5.BackColor == Color.Lime ? "5" : "0");
                    modelStr += "$" + (btnArray6.BackColor == Color.Lime ? "6" : "0");
                    //FuncInline.Scan.ChangeModel(modelName + modelStr);

                    valueChanged = false;

                    #region PCB 폭이 다르면 폭 조절
                    //bool widthOk = false;
                    if (widthChanged &&
                        FuncWin.MessageBoxOK("PCB width changed. Run width change?"))
                    {
                        if (FuncInlineAction.InitAllWidth())
                        {
                            //widthOk = true;
                            FuncWin.TopMessageBox("Width Change OK");
                        }
                        else
                        {
                            FuncWin.TopMessageBox("Width Change Fail");
                            return;
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        private bool checkArrayCount()
        {
            int checkCount = 0;
            /*
            for (int i = 1; i <= GlobalVar.AutoInline_MaxArrayCount; i++)
            {
                if (((CheckBox)((Controls.Find("cbArray" + i, true))[0])).Checked)
                {
                    checkCount++;
                }
            }
            //*/
            if (btnArray1.BackColor == Color.Lime)
            {
                checkCount++;
            }
            if (btnArray2.BackColor == Color.Lime)
            {
                checkCount++;
            }
            if (btnArray3.BackColor == Color.Lime)
            {
                checkCount++;
            }
            if (btnArray4.BackColor == Color.Lime)
            {
                checkCount++;
            }
            if (btnArray5.BackColor == Color.Lime)
            {
                checkCount++;
            }
            if (btnArray6.BackColor == Color.Lime)
            {
                checkCount++;
            }
            return checkCount == (int)numArrayCount.Value;
        }

        private void pbSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridModelList.Rows.Count == 0 ||
                    dataGridModelList.SelectedCells.Count == 0 ||
                    dataGridModelList.SelectedCells[0].RowIndex < 0 ||
                    dataGridModelList.SelectedCells[0].RowIndex >= modelCount)
                {
                    FuncWin.TopMessageBox("Select Model first!");
                    return;
                }

                if (!checkArrayCount())
                {
                    FuncWin.TopMessageBox("Array count not match");
                    return;
                }

                string modelName = dataGridModelList.SelectedRows[0].Cells[1].Value.ToString();

                #region PCB 존재하고 PCB 폭이 다르면 변경 금지
                if (FuncInline.PCBWidth != (double)numPCBWidth.Value)
                {
                    if (FuncInline.CheckPCBInMachine())
                    {
                        FuncWin.TopMessageBox("Can't change PCB width while PCB in machine.");
                        return;
                    }
                    if (GlobalVar.SystemStatus <= enumSystemStatus.Initialize) // 초기화되어 있지 않으면 폭조절 금지
                    {
                        FuncWin.TopMessageBox("Run System Origin first to change PCB width.");
                        return;
                    }
                }
                #endregion

                if (FuncWin.MessageBoxOK("Save Model Setting named " + modelName))
                {
                    FuncLog.WriteLog("Model - Save " + modelName);

                    bool widthChanged = (double)numPCBWidth.Value != FuncInline.PCBWidth;

                    GlobalVar.ModelName = modelName;
                    ApplyAllValue();
                    Func.SaveModelIni();
                    Func.SaveTeachingIni();
                    Func.SaveMachineIni(); // 이미지 영역 위해

                    // 모델이미지 등 데이터 날아가는 경우 있어 다시 로드한다.
                    Func.LoadModelIni();
                    //Func.LoadTeachingIni();
                    LoadAllValue();
                    RefreshArrayImage();

                    //string modelStr = "";
                    /*
                    for (int i = 1; i <= GlobalVar.AutoInline_MaxArrayCount; i++)
                    {
                        Control[] arrayControl = Controls.Find("cbArray" + i, true);
                        Control[] xoutControl = Controls.Find("cbXOut" + i, true);
                        //if (arrayControl.Length > 1 &&
                        //    xoutControl.Length > 1)
                        //{
                        try
                        {
                            if (((CheckBox)arrayControl[0]).Checked)
                            {
                                modelStr += "$" + i.ToString() + ",O,O";
                            }
                        }
                        catch (Exception ex)
                        {
                            debug(ex.ToString());
                            debug(ex.StackTrace);
                        }
                        //}
                    }
                    //*/
                    /*
                    if (btnArray1.BackColor == Color.Lime)
                    {
                        modelStr += "$" + 1.ToString() + ",O,O";
                    }
                    if (btnArray2.BackColor == Color.Lime)
                    {
                        modelStr += "$" + 2.ToString() + ",O,O";
                    }
                    if (btnArray3.BackColor == Color.Lime)
                    {
                        modelStr += "$" + 3.ToString() + ",O,O";
                    }
                    if (btnArray4.BackColor == Color.Lime)
                    {
                        modelStr += "$" + 4.ToString() + ",O,O";
                    }
                    if (btnArray5.BackColor == Color.Lime)
                    {
                        modelStr += "$" + 5.ToString() + ",O,O";
                    }
                    if (btnArray6.BackColor == Color.Lime)
                    {
                        modelStr += "$" + 6.ToString() + ",O,O";
                    }
                    //*/

                    string modelStr = "$" + (FuncInline.ScanInsertCheck ? "O" : "X"); // Fiducial
                    modelStr += "$" + (FuncInline.XoutByVision ? "O" : "X"); // XOUt
                    modelStr += "$X"; // OCR
                    modelStr += "$" + (FuncInline.UseBadMark ? "O" : "X"); // GoodMark
                    //modelStr += "$X"; // BadMark  // 통합될 예정
                    modelStr += "$" + (btnArray1.BackColor == Color.Lime ? "1" : "0");
                    modelStr += "$" + (btnArray2.BackColor == Color.Lime ? "2" : "0");
                    modelStr += "$" + (btnArray3.BackColor == Color.Lime ? "3" : "0");
                    modelStr += "$" + (btnArray4.BackColor == Color.Lime ? "4" : "0");
                    modelStr += "$" + (btnArray5.BackColor == Color.Lime ? "5" : "0");
                    modelStr += "$" + (btnArray6.BackColor == Color.Lime ? "6" : "0");
                    //GlobalVar.Scan.ChangeModel(modelName + modelStr);

                    valueChanged = false;

                    #region PCB 폭이 다르면 폭 조절
                    if (widthChanged &&
                        FuncWin.MessageBoxOK("PCB width changed. Run width change?"))
                    {
                        if (FuncInlineAction.InitAllWidth())
                        {
                            FuncWin.TopMessageBox("Width Change OK");
                        }
                        else
                        {
                            FuncWin.TopMessageBox("Width Change Fail");
                            return;
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        private void pbDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridModelList.Rows.Count == 0 ||
                    dataGridModelList.SelectedCells.Count == 0 ||
                    dataGridModelList.SelectedCells[0].RowIndex < 0 ||
                    dataGridModelList.SelectedCells[0].RowIndex >= modelCount)
                {
                    FuncWin.TopMessageBox("Select Model first!");
                    return;
                }

                #region PCB 존재하고 PCB 폭이 다르면 변경 금지
                if (FuncInline.CheckPCBInMachine())
                {
                    FuncWin.TopMessageBox("Can't delete model while PCB in machine.");
                    return;
                }
                #endregion


                string modelName = dataGridModelList.SelectedRows[0].Cells[1].Value.ToString();
                if (FuncWin.MessageBoxOK("Delete Model Setting named " + modelName))
                {
                    FuncLog.WriteLog("Model - Delete " + modelName);

                    string IniPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.ModelPath + "\\" + modelName + ".ini";
                    if (File.Exists(IniPath))
                    {
                        File.Delete(IniPath);

                        loadModelList();
                        if (dataGridModelList.Rows.Count > 0)
                        {
                            dataGridModelList.CurrentCell = dataGridModelList.Rows[0].Cells[1];
                            modelName = dataGridModelList.Rows[0].Cells[1].Value.ToString();
                            GlobalVar.ModelName = modelName;
                            Func.LoadModelIni();
                            LoadAllValue();
                        }
                        else
                        {
                            GlobalVar.ModelName = "";
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        private void pbAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!checkArrayCount())
                {
                    FuncWin.TopMessageBox("Array count not match");
                }

                #region PCB 존재하고 PCB 폭이 다르면 변경 금지
                if ((double)numPCBWidth.Value != FuncInline.PCBWidth)
                {
                    if (FuncInline.CheckPCBInMachine())
                    {
                        FuncWin.TopMessageBox("Can't change model in different width while PCB in machine.");
                        return;
                    }
                    if (GlobalVar.SystemStatus <= enumSystemStatus.Initialize) // 초기화되어 있지 않으면 폭조절 금지
                    {
                        FuncWin.TopMessageBox("Run System Origin first to change PCB width.");
                        return;
                    }
                }
                #endregion


                string modelName = "";
                if (FuncWin.InputBox("New Model", "Type new Model Name to Add", ref modelName) == DialogResult.OK)
                {
                    if (modelName.Length == 0)
                    {
                        FuncWin.TopMessageBox("Input Model Name");
                        return;
                    }
                    //FuncLog.WriteLog("add modelName : " + modelName);
                    FuncLog.WriteLog("Model - Add " + modelName);

                    #region 중복체크
                    for (int i = 0; i < dataGridModelList.Rows.Count; i++)
                    {
                        if (dataGridModelList.Rows[i].Cells[1].Value.ToString() == modelName)
                        {
                            FuncWin.TopMessageBox("Same Model Name already exists. Try another name.");
                            return;
                        }
                    }
                    #endregion

                    bool widthChanged = (double)numPCBWidth.Value != FuncInline.PCBWidth;

                    #region 비전 티칭파일 복사. 티칭 안 된다 하여 삭제
                    /*
                    string visionPath = GlobalVar.FaPath + "\\" +
                                        GlobalVar.SWName + "\\" +
                                        GlobalVar.ScanPath +
                                        "\\model\\";
                    if (File.Exists(visionPath + GlobalVar.ModelName + ".mpp") &&
                        !File.Exists(visionPath + modelName + ".mpp"))
                    {
                        File.Copy(visionPath + GlobalVar.ModelName + ".mpp", visionPath + modelName + ".mpp");
                    }
                    //*/
                    #endregion

                    #region 모델이미지 복사
                    /*
                    string imagePath = GlobalVar.FaPath + "\\" +
                                        GlobalVar.SWName + "\\" +
                                        GlobalVar.ArrayImagePath + "\\";
                    if (File.Exists(imagePath + GlobalVar.ModelName + ".bmp") &&
                        !File.Exists(imagePath + modelName + ".bmp"))
                    {
                        File.Copy(imagePath + GlobalVar.ModelName + ".bmp", imagePath + modelName + ".bmp");
                    }
                    //*/
                    #endregion

                    GlobalVar.ModelName = modelName;

                    lblArrayImage.Text = modelName + ".bmp";
                    ApplyAllValue();

                    Func.SaveModelIni();
                    Func.SaveTeachingIni();
                    Func.SaveTeachingWidthIni();
                    Func.SaveMachineIni(); // 이미지 영역 위해

                    loadModelList();
                    dataGridModelList.CurrentCell = dataGridModelList.Rows[dataGridModelList.Rows.Count - 1].Cells[1];

                    #region 비전 연동
                    // [MModelName$Fiducial$XOut$OCR$Good$Bad$1$2$3$4$5$6]
                    string modelStr = "$" + (FuncInline.ScanInsertCheck ? "O" : "X"); // Fiducial
                    modelStr += "$" + (FuncInline.XoutByVision ? "O" : "X"); // XOUt
                    modelStr += "$X"; // OCR
                    modelStr += "$" + (FuncInline.UseBadMark ? "O" : "X"); // GoodMark
                    //modelStr += "$X"; // BadMark  // 통합될 예정
                    modelStr += "$" + (btnArray1.BackColor == Color.Lime ? "1" : "0");
                    modelStr += "$" + (btnArray2.BackColor == Color.Lime ? "2" : "0");
                    modelStr += "$" + (btnArray3.BackColor == Color.Lime ? "3" : "0");
                    modelStr += "$" + (btnArray4.BackColor == Color.Lime ? "4" : "0");
                    modelStr += "$" + (btnArray5.BackColor == Color.Lime ? "5" : "0");
                    modelStr += "$" + (btnArray6.BackColor == Color.Lime ? "6" : "0");
                    //GlobalVar.Scan.ChangeModel(modelName + modelStr);
                    #endregion

                    #region 폭 티칭 계산
                    for (int i = 0; i < FuncInline.OffsetWidth.Length; i++)
                    {

                        FuncInline.TeachingWidth[i] = FuncInline.PCBWidth + FuncInline.OffsetWidth[i]; // apply건 load건 티칭계산이 다시 되었으면 티칭좌표에 적용
                    }
                    #endregion

                    valueChanged = false;

                    #region PCB 폭이 다르면 폭 조절
                    if (widthChanged &&
                        FuncWin.MessageBoxOK("PCB width changed. Run width change?"))
                    {
                        if (FuncInlineAction.InitAllWidth())
                        {
                            FuncWin.TopMessageBox("Width Change OK");
                        }
                        else
                        {
                            FuncWin.TopMessageBox("Width Change Fail");
                            return;
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        /*
        public static DialogResult InputBox(string title, string content, ref string value)
        {
            Form form = new Form();
            PictureBox picture = new PictureBox();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.ClientSize = new Size(300, 200);
            form.Controls.AddRange(new Control[] { label, picture, textBox, buttonOk, buttonCancel });
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MaximizeBox = false;
            form.MinimizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            form.Text = title;
            picture.SizeMode = PictureBoxSizeMode.StretchImage;
            label.Text = content;
            textBox.Text = value;
            buttonOk.Text = "OK";
            buttonCancel.Text = "CANCEL";

            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            picture.SetBounds(10, 10, 50, 50);
            label.SetBounds(25, 17, 200, 100);
            textBox.SetBounds(25, 120, 220, 20);
            buttonOk.SetBounds(135, 170, 70, 20);
            buttonCancel.SetBounds(215, 170, 70, 20);

            DialogResult dialogResult = form.ShowDialog();

            value = textBox.Text;
            return dialogResult;
        }
        //*/

        private void pbArrayImageDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (lblArrayImage.Text.Length > 0)
                {
                    pbArrayImage.BackgroundImage = Properties.Resources.no_image;

                    string imagePath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.ArrayImagePath + "\\" + lblArrayImage.Text;
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                    lblArrayImage.Text = "";
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        private void pbArrayImage_Click(object sender, EventArgs e)
        {

            if (!Func.checkImageDirectory())
            {
                FuncWin.TopMessageBox("No uploaded Image. Add Layout Image first!");
                return;
            }

            string defaultPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.ArrayImagePath;
            //string imagePath = "";
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Select Uploaded Array Layout";
            //dlg.Filter = "*.*";
            dlg.InitialDirectory = defaultPath;
            dlg.FileOk += (s, cea) =>
            { // 지정 폴더 이외 선택 막기
                string selectedDir = System.IO.Path.GetDirectoryName(dlg.FileName);
                if (string.Compare(defaultPath, selectedDir, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    string msg = string.Format("Please select image in '{0}' folder", defaultPath);
                    FuncWin.TopMessageBox("Invalid folder selection. " + msg);
                    cea.Cancel = true;
                }
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                //pbArrayImage.BackgroundImage = Image.FromFile(dlg.FileName); // 이미지 파일 자체를 로드하면 사용중으로 되어서 추후 삭제 등을 할 수 없다.
                Bitmap image = FuncScreen.LoadBitmap(dlg.FileName);
                if (image == null)
                {
                    pbArrayImage.BackgroundImage = Properties.Resources.no_image;
                }
                else
                {
                    pbArrayImage.BackgroundImage = image;
                }
                lblArrayImage.Text = System.IO.Path.GetFileName(dlg.FileName);
            }
        }

        private void pbArrayImageAdd_Click(object sender, EventArgs e)
        {
            Func.checkImageDirectory();

            string defaultPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.ArrayImagePath;
            //string imagePath = "";
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Select Array Layout image";
            //dlg.Filter = "*.*";
            //dlg.InitialDirectory = defaultPath;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string destPath = defaultPath + "\\" + System.IO.Path.GetFileName(dlg.FileName);

                if (File.Exists(destPath))
                {
                    FuncWin.TopMessageBox("Same image file already exists!");
                    return;
                }

                if (File.Exists(dlg.FileName))
                {
                    File.Copy(dlg.FileName, destPath);
                }
                //pbArrayImage.BackgroundImage = Image.FromFile(destPath); // 이미지 파일 자체를 로드하면 사용중으로 되어서 추후 삭제 등을 할 수 없다.
                Bitmap image = FuncScreen.LoadBitmap(dlg.FileName);
                if (image == null)
                {
                    pbArrayImage.BackgroundImage = Properties.Resources.no_image;
                }
                else
                {
                    pbArrayImage.BackgroundImage = image;
                }
                lblArrayImage.Text = System.IO.Path.GetFileName(destPath);
            }
        }

        private void btnMakeImage_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Model - Make Array Image Click");
            //FuncInline.MakeArrayImage();

            ModelImage dlg = new ModelImage();
            dlg.ShowDialog();

            RefreshArrayImage();
        }

        private void cbXoutBySelection_CheckedChanged(object sender, EventArgs e)
        {
            //if (cbXoutBySelection.Checked)
            //{
            //    cbXoutByVision.Checked = false;
            //}
        }

        private void cbXoutByVision_CheckedChanged(object sender, EventArgs e)
        {
            //if (cbXoutByVision.Checked)
            //{
            //    cbXoutBySelection.Checked = false;
            //}
        }

        public void PreviewModel(string model)
        {
            try
            {
                #region INI 읽기
                string IniPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.ModelPath + "\\" + model + ".ini";
                string Section = GlobalVar.IniSection;

                numArrayCount.Value = Math.Max(1, Math.Min(6, int.Parse(FuncFile.ReadIniFile(Section, "ArrayCount", IniPath, "6"))));
                cbPinUseFront.Checked = FuncFile.ReadIniFile(Section, "PinUseFront", IniPath, "False") == "True";
                cbPinUseRear.Checked = FuncFile.ReadIniFile(Section, "PinUseRear", IniPath, "False") == "True";
                numPCBWidth.Value = (decimal)double.Parse(FuncFile.ReadIniFile(Section, "PCBWidth", IniPath, "70"));
                numPCBLength.Value = (decimal)double.Parse(FuncFile.ReadIniFile(Section, "PCBLength", IniPath, "100"));
                numTestTimeout.Value = int.Parse(FuncFile.ReadIniFile(Section, "TestTimeout", IniPath, "2000"));
                FuncInline.enumTestType TestType = (FuncInline.enumTestType)int.Parse(FuncFile.ReadIniFile(Section, "TestType", IniPath, "2"));
                for (int i = 0; i < cmbTestType.Items.Count; i++)
                {
                    if (cmbTestType.Items[i].ToString() == TestType.ToString())
                    {
                        cmbTestType.SelectedIndex = i;
                        break;
                    }
                }
                // bool[] ArrayUse = new bool[21];
                /*
                 for (int i = 0; i < FuncInline.ArrayUse.Length; i++)
                 {
                     ((CheckBox)(Controls.Find("cbArray" + (i + 1), true)[0])).Checked = FuncFile.ReadIniFile(Section, "ArrayUse_" + i, IniPath, "False") == "True";
                 }
                 bool[] XOut = new bool[21];
                 for (int i = 0; i < FuncInline.XOut.Length; i++)
                 {
                     ((CheckBox)(Controls.Find("cbXOut" + (i + 1), true)[0])).Checked = FuncFile.ReadIniFile(Section, "XOut_" + i, IniPath, "False") == "True";
                 }
                 //*/
                btnArray1.BackColor = FuncFile.ReadIniFile(Section, "ArrayUse_" + 0, IniPath, "False") == "True" ? Color.Lime : Color.White;
                btnArray2.BackColor = FuncFile.ReadIniFile(Section, "ArrayUse_" + 1, IniPath, "False") == "True" ? Color.Lime : Color.White;
                btnArray3.BackColor = FuncFile.ReadIniFile(Section, "ArrayUse_" + 2, IniPath, "False") == "True" ? Color.Lime : Color.White;
                btnArray4.BackColor = FuncFile.ReadIniFile(Section, "ArrayUse_" + 3, IniPath, "False") == "True" ? Color.Lime : Color.White;
                btnArray5.BackColor = FuncFile.ReadIniFile(Section, "ArrayUse_" + 4, IniPath, "False") == "True" ? Color.Lime : Color.White;
                btnArray6.BackColor = FuncFile.ReadIniFile(Section, "ArrayUse_" + 5, IniPath, "False") == "True" ? Color.Lime : Color.White;
                btnXOut1.BackColor = FuncFile.ReadIniFile(Section, "XOut_" + 0, IniPath, "False") == "True" ? Color.Lime : Color.White;
                btnXOut2.BackColor = FuncFile.ReadIniFile(Section, "XOut_" + 1, IniPath, "False") == "True" ? Color.Lime : Color.White;
                btnXOut3.BackColor = FuncFile.ReadIniFile(Section, "XOut_" + 2, IniPath, "False") == "True" ? Color.Lime : Color.White;
                btnXOut4.BackColor = FuncFile.ReadIniFile(Section, "XOut_" + 3, IniPath, "False") == "True" ? Color.Lime : Color.White;
                btnXOut5.BackColor = FuncFile.ReadIniFile(Section, "XOut_" + 4, IniPath, "False") == "True" ? Color.Lime : Color.White;
                btnXOut6.BackColor = FuncFile.ReadIniFile(Section, "XOut_" + 5, IniPath, "False") == "True" ? Color.Lime : Color.White;

                btnXoutBySelection.BackColor = FuncFile.ReadIniFile(Section, "XoutBySelection", IniPath, "False") == "True" ? Color.Lime : Color.White;
                btnXoutBySelectionNo.BackColor = btnXoutBySelection.BackColor != Color.Lime ? Color.Lime : Color.White;
                btnXoutByVision.BackColor = FuncFile.ReadIniFile(Section, "XoutByVision", IniPath, "False") == "True" ? Color.Lime : Color.White;
                btnXoutByVisionNo.BackColor = btnXoutByVision.BackColor != Color.Lime ? Color.Lime : Color.White;
                btnUseBadMark.BackColor = FuncFile.ReadIniFile(Section, "UseBadMark", IniPath, "False") == "True" ? Color.Lime : Color.White;
                btnUseBadMarkNo.BackColor = btnUseBadMark.BackColor != Color.Lime ? Color.Lime : Color.White;
                btnXOutToNG.BackColor = FuncFile.ReadIniFile(Section, "XoutToNG", IniPath, "False") == "True" ? Color.Lime : Color.White;
                btnXOutToNGNo.BackColor = btnXOutToNG.BackColor != Color.Lime ? Color.Lime : Color.White;
                btnBadMarkToNG.BackColor = FuncFile.ReadIniFile(Section, "BadMarkToNG", IniPath, "False") == "True" ? Color.Lime : Color.White;
                btnBadMarkToNGNo.BackColor = btnBadMarkToNG.BackColor != Color.Lime ? Color.Lime : Color.White;
                btnLongPCBTest.BackColor = FuncFile.ReadIniFile(Section, "LongPCBTest", IniPath, "False") == "True" ? Color.Lime : Color.White;
                btnLongPCBTestNo.BackColor = btnLongPCBTest.BackColor != Color.Lime ? Color.Lime : Color.White;

                string imagePath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.ArrayImagePath + "\\" + model + ".jpg";
                pbArrayImage.BackgroundImage = FuncScreen.LoadBitmap(imagePath);
                #endregion
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        private void dataGridModelList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dataGridModelList.SelectedCells.Count > 0 &&
                    dataGridModelList.SelectedCells[0].RowIndex >= 0 &&
                    dataGridModelList.SelectedCells[0].RowIndex < dataGridModelList.Rows.Count)
                {
                    string model = dataGridModelList.Rows[dataGridModelList.SelectedCells[0].RowIndex].Cells[1].Value.ToString();
                    PreviewModel(model);
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        private void btnArray_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.BackColor = button.BackColor == Color.Lime ? Color.White : Color.Lime;
        }

        private void btnXOut_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.BackColor = button.BackColor == Color.Lime ? Color.White : Color.Lime;
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.BackColor = button.BackColor == Color.Lime ? Color.White : Color.Lime;
            FuncLog.WriteLog("Model Value Change : " + button.Name + " ==> " + (button.BackColor == Color.Lime));
            valueChanged = true;
        }

        private void btnXoutBySelection_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Model Value Change : XOut by Selection ==> True");
            btnXoutBySelection.BackColor = Color.Lime;
            btnXoutBySelectionNo.BackColor = Color.White;
            valueChanged = true;
        }

        private void btnXoutBySelectionNo_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Model Value Change : XOut by Selection ==> False");
            btnXoutBySelection.BackColor = Color.White;
            btnXoutBySelectionNo.BackColor = Color.Lime;
            valueChanged = true;
        }

        private void btnXoutByVision_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Model Value Change : XOut by Vision ==> True");
            btnXoutByVision.BackColor = Color.Lime;
            btnXoutByVisionNo.BackColor = Color.White;
            valueChanged = true;
        }

        private void btnXoutByVisionNo_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Model Value Change : XOut by Vision ==> False");
            btnXoutByVision.BackColor = Color.White;
            btnXoutByVisionNo.BackColor = Color.Lime;
            valueChanged = true;
        }

        private void btnUseBadMark_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Model Value Change : BadMark by Vision ==> True");
            btnUseBadMark.BackColor = Color.Lime;
            btnUseBadMarkNo.BackColor = Color.White;
            valueChanged = true;
        }

        private void btnUseBadMarkNo_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Model Value Change : BadMark by Vision ==> False");
            btnUseBadMark.BackColor = Color.White;
            btnUseBadMarkNo.BackColor = Color.Lime;
            valueChanged = true;
        }

        private void btnXOutToNG_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Model Value Change : XOut to NG ==> True");
            btnXOutToNG.BackColor = Color.Lime;
            btnXOutToNGNo.BackColor = Color.White;
            valueChanged = true;
        }

        private void btnXOutToNGNo_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Model Value Change : XOut to NG ==> False");
            btnXOutToNG.BackColor = Color.White;
            btnXOutToNGNo.BackColor = Color.Lime;
            valueChanged = true;
        }

        private void btnBadMarkToNG_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Model Value Change : BadMark to NG ==> True");
            btnBadMarkToNG.BackColor = Color.Lime;
            btnBadMarkToNGNo.BackColor = Color.White;
            valueChanged = true;
        }

        private void btnBadMarkToNGNo_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Model Value Change : BadMark to NG ==> False");
            btnBadMarkToNG.BackColor = Color.White;
            btnBadMarkToNGNo.BackColor = Color.Lime;
            valueChanged = true;
        }

      

        private void numArrayCount_ValueChanged(object sender, EventArgs e)
        {
            if (firstShow)
            {
                return;
            }
            valueChanged = true;
        }

        private void cmbTestType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (firstShow)
            {
                return;
            }
            FuncLog.WriteLog("Model Value Change : TestType ==> " + cmbTestType.Text);
            valueChanged = true;
        }

        private void numPCBWidth_ValueChanged(object sender, EventArgs e)
        {
            if (firstShow)
            {
                return;
            }
            FuncLog.WriteLog("Model Value Change : PCB Width ==> " + numPCBWidth.Value);
            valueChanged = true;
        }

        private void numPCBLength_ValueChanged(object sender, EventArgs e)
        {
            if (firstShow)
            {
                return;
            }
            valueChanged = true;
        }

        private void numTestTimeout_ValueChanged(object sender, EventArgs e)
        {
            if (firstShow)
            {
                return;
            }
            FuncLog.WriteLog("Model Value Change : Test Timeout ==> " + numTestTimeout.Value);
            valueChanged = true;
        }

        private void dataGridModelList_Click(object sender, EventArgs e)
        {
            try
            {
                //Disable Sorting for DataGridView
                foreach (DataGridViewColumn item in ((DataGridView)sender).Columns)
                {
                    item.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {
            debugCount++;
        }

        private void btnScanTeaching_Click(object sender, EventArgs e)
        {
            try
            {
                if (valueChanged)
                {
                    valueChanged = false;
                    if (FuncWin.MessageBoxOK("Model Setting changed. Save?"))
                    {
                        ApplyAllValue();
                        Func.SaveModelIni();
                        Func.SaveTeachingIni();
                        Func.SaveMachineIni(); // 이미지 영역 위해

                        // 모델이미지 등 데이터 날아가는 경우 있어 다시 로드한다.
                        Func.LoadModelIni();
                        //Func.LoadTeachingIni();
                        LoadAllValue();
                        RefreshArrayImage();
                    }
                }

                // 0) 메인폼 참조 (탭/유저컨트롤 내부에서도 안전)
                var main = this.FindForm() as Radix.frmMain_AutoInline_PC
                           ?? Application.OpenForms.OfType<Radix.frmMain_AutoInline_PC>().FirstOrDefault();

                // 1) 스캐너 연결 해제 (예외 무시 안전처리)
                try
                {
                    if (FuncInline.Scanner != null)
                        FuncInline.Scanner.Disconnect();
                }
                catch { /* 필요시 로그 */ }

                // 2) 메인폼 "최소화"
                if (main != null)
                    main.WindowState = FormWindowState.Minimized;


                // 3) ScanPosition 폼 띄우기(최상단 유지)
                //    이미 떠있으면 재사용하고 앞으로 가져오기
                var scanForm = Application.OpenForms.OfType<ScanPosition>().FirstOrDefault();
                if (scanForm == null || scanForm.IsDisposed)
                    scanForm = new ScanPosition();

                scanForm.MdiParent = null;     // MDI 연결 금지(부모 최소화에 끌려가지 않도록)
                scanForm.TopMost = true;
                scanForm.StartPosition = FormStartPosition.Manual;

                // 현재 메인폼이 올라간 모니터의 작업 영역(작업표시줄 제외)
                Rectangle wa = Screen.FromControl(this).WorkingArea;

                // 요구 크기: 1920 x 363 (화면보다 크면 화면 크기로 클램프)
                int targetW = 1920;
                int targetH = 363;
                int w = Math.Min(targetW, wa.Width);
                int h = Math.Min(targetH, wa.Height);

                // 하단 정렬: X는 왼쪽부터, Y는 작업영역 하단에서 h만큼 위
                int x = wa.Left;                  // 좌측 정렬 (가로를 꽉 채우려면 이렇게 둡니다)
                int y = wa.Bottom - h;            // 하단 붙이기

                scanForm.Size = new Size(w, h);
                scanForm.Location = new Point(x, y);
                scanForm.WindowState = FormWindowState.Normal;

                // 표시
                scanForm.Show();
                scanForm.BringToFront();
                scanForm.Activate();

                FuncInline.TabTeaching = FuncInline.enumTabTeaching.Scan;

                // 4) 외부 프로그램 실행
                string exePath = @"C:\FA\AutoInline\Preview\Usercom_V2.exe";

                var running = Process.GetProcessesByName("Usercom_V2").Any();
                if (running)
                {
                    FuncWin.TopMessageBox($"Usercom_V2.exe is already running ");
                }

                else if (File.Exists(exePath))
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = exePath,
                        WorkingDirectory = Path.GetDirectoryName(exePath),
                        UseShellExecute = false,
                        CreateNoWindow = false
                    };
                    Process.Start(psi);
                }
                else
                {
                    MessageBox.Show($"Executable file not found.\n{exePath}",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while executing scan teaching : " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnLongPCBTest_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Model Value Change : LongPCBTest ==> True");
            btnLongPCBTest.BackColor = Color.Lime;
            btnLongPCBTestNo.BackColor = Color.White;
            valueChanged = true;
        }

        private void btnLongPCBTestNo_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Model Value Change : LongPCBTest ==> False");
            btnLongPCBTest.BackColor = Color.White;
            btnLongPCBTestNo.BackColor = Color.Lime;
            valueChanged = true;
        }
    }
}
