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

    public partial class Model : Form
    {
        private System.Threading.Timer timerUI; // Thread Timer
        private bool timerDoing = false;

     
        private int widthWorkingTick = Environment.TickCount;

     
        private int modelCount = 0;

        private string previewModel = "";

        
      
  
        public Model()
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

            tbModelName.Text = GlobalVar.ModelName;
            LoadAllValue();

            //DataGridViewRow test = dataGridModelList.CurrentRow;
            //dataGridModelList.Select("ModelName = "+ "'" + GlobalVar.ModelName +"'"+'"');
            //dataGridModelList.CurrentCell = dataGridModelList.Co
            #region 화면 제어용 쓰레드 타이머 시작
            //*
            TimerCallback CallBackUI = new TimerCallback(TimerUI);
            timerUI = new System.Threading.Timer(CallBackUI, false, 0, 100);
            //*/
            #endregion

        
         

            this.BringToFront();
        }

        public void LoadAllValue()
        {
            try
            {
                #region 현재모델 선택
                for (int i = 0; i < dataGridModelList.Rows.Count; i++)
                {
                    if (dataGridModelList.Rows[i].Cells[1].Value.ToString() == GlobalVar.ModelName)
                    {
                        dataGridModelList.Rows[i].Selected = true;
                        break;
                    }
                }
                #endregion

                tbModelName.Text = GlobalVar.ModelName;

                #region Raobot Offset 관련 셋팅
                numOffsetBeforeLX.Value = (decimal)FuncInline.Offset_BF_Tray_L.x;

                numOffsetBeforeLY.Value = (decimal)FuncInline.Offset_BF_Tray_L.y;
                numOffsetBeforeLZ.Value = (decimal)FuncInline.Offset_BF_Tray_L.z;

                numOffsetBeforeRX.Value = (decimal)FuncInline.Offset_BF_Tray_R.x;
                numOffsetBeforeRY.Value = (decimal)FuncInline.Offset_BF_Tray_R.y;
                numOffsetBeforeRZ.Value = (decimal)FuncInline.Offset_BF_Tray_R.z;

                numBF_YZ_Offset.Value = (decimal)FuncInline.BF_YZ_Offset;

                numOffsetSandingLX.Value = (decimal)FuncInline.Offset_Target_L.x;
                numOffsetSandingLY.Value = (decimal)FuncInline.Offset_Target_L.y;
                numOffsetSandingLZ.Value = (decimal)FuncInline.Offset_Target_L.z;

                numOffsetSandingRX.Value = (decimal)FuncInline.Offset_Target_R.x;
                numOffsetSandingRY.Value = (decimal)FuncInline.Offset_Target_R.y;
                numOffsetSandingRZ.Value = (decimal)FuncInline.Offset_Target_R.z;

                numOffsetAfterLX.Value = (decimal)FuncInline.Offset_AF_Tray_L.x;
                numOffsetAfterLY.Value = (decimal)FuncInline.Offset_AF_Tray_L.y;
                numOffsetAfterLZ.Value = (decimal)FuncInline.Offset_AF_Tray_L.z;

                numOffsetAfterRX.Value = (decimal)FuncInline.Offset_AF_Tray_R.x;
                numOffsetAfterRY.Value = (decimal)FuncInline.Offset_AF_Tray_R.y;
                numOffsetAfterRZ.Value = (decimal)FuncInline.Offset_AF_Tray_R.z;

                numAF_YZ_Offset.Value = (decimal)FuncInline.AF_YZ_Offset;

                numBAngel_offset.Value = (decimal)FuncInline.BF_Angle_Offset; //GSCHOI-230615
                numAAngel_offset.Value = (decimal)FuncInline.AF_Angle_Offset; //GSCHOI-230615

                //Kuka Tool 각도 보정 추가 by DGKim 230731
                numBTool_L_Angel_offset.Value = (decimal)FuncInline.BFTool_L_Angel_Offset; //샌딩 전 Left Tool 각도 보정 값
                numBTool_R_Angel_offset.Value = (decimal)FuncInline.BFTool_R_Angel_Offset; //샌딩 전 Right Tool 각도 보정 값
                numATool_L_Angel_offset.Value = (decimal)FuncInline.AFTool_L_Angel_Offset; //샌딩 후 Left Tool 각도 보정 값
                numATool_R_Angel_offset.Value = (decimal)FuncInline.AFTool_R_Angel_Offset; //샌딩 후 Right Tool 각도 보정 값

                #endregion

                #region Tray 관련 셋팅
               
                cbAfter_Tray_MatchingSort.Checked = FuncInline.After_Tray_MatchingSort;   //true 시 Before 트레이 After트레이 임플란트 위치 맞춤
            
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
           
                GlobalVar.ModelName = tbModelName.Text;

                #region Raobot Offset 관련 셋팅
                FuncInline.Offset_BF_Tray_L.x = (double)numOffsetBeforeLX.Value;
                FuncInline.Offset_BF_Tray_L.y = (double)numOffsetBeforeLY.Value;
                FuncInline.Offset_BF_Tray_L.z = (double)numOffsetBeforeLZ.Value;

                FuncInline.Offset_BF_Tray_R.x = (double)numOffsetBeforeRX.Value;
                FuncInline.Offset_BF_Tray_R.y = (double)numOffsetBeforeRY.Value;
                FuncInline.Offset_BF_Tray_R.z = (double)numOffsetBeforeRZ.Value;

                FuncInline.BF_YZ_Offset = (double)numBF_YZ_Offset.Value;

                FuncInline.Offset_Target_L.x = (double)numOffsetSandingLX.Value;
                FuncInline.Offset_Target_L.y = (double)numOffsetSandingLY.Value;
                FuncInline.Offset_Target_L.z = (double)numOffsetSandingLZ.Value;

                FuncInline.Offset_Target_R.x = (double)numOffsetSandingRX.Value;
                FuncInline.Offset_Target_R.y = (double)numOffsetSandingRY.Value;
                FuncInline.Offset_Target_R.z = (double)numOffsetSandingRZ.Value;

                FuncInline.Offset_AF_Tray_L.x = (double)numOffsetAfterLX.Value;
                FuncInline.Offset_AF_Tray_L.y = (double)numOffsetAfterLY.Value;
                FuncInline.Offset_AF_Tray_L.z = (double)numOffsetAfterLZ.Value;

                FuncInline.Offset_AF_Tray_R.x = (double)numOffsetAfterRX.Value;
                FuncInline.Offset_AF_Tray_R.y = (double)numOffsetAfterRY.Value;
                FuncInline.Offset_AF_Tray_R.z = (double)numOffsetAfterRZ.Value;

                FuncInline.AF_YZ_Offset = (double)numAF_YZ_Offset.Value;

                FuncInline.BF_Angle_Offset = (double)numBAngel_offset.Value;  //GSCHOI-230615
                FuncInline.AF_Angle_Offset = (double)numAAngel_offset.Value;  //GSCHOI-230615

                //Kuka Tool 각도 보정 추가 by DGKim 230731
                FuncInline.BFTool_L_Angel_Offset = (double)numBTool_L_Angel_offset.Value; //샌딩 전 Left Tool 각도 보정 값
                FuncInline.BFTool_R_Angel_Offset = (double)numBTool_R_Angel_offset.Value; //샌딩 전 Right Tool 각도 보정 값
                FuncInline.AFTool_L_Angel_Offset = (double)numATool_L_Angel_offset.Value; //샌딩 후 Left Tool 각도 보정 값
                FuncInline.AFTool_R_Angel_Offset = (double)numATool_R_Angel_offset.Value; //샌딩 후 Right Tool 각도 보정 값
                #endregion

                #region Tray 관련 셋팅 
              
                FuncInline.After_Tray_MatchingSort = (bool)cbAfter_Tray_MatchingSort.Checked;   //true 시 Before 트레이 After트레이 임플란트 위치 맞춤

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
            
            if (GlobalVar.GlobalStop ||
                GlobalVar.TabMain != enumTabMain.Model) // 메인 탭이 다른 곳에 있으면 실행 안 한다.
            {
                timerDoing = false;
                return;
            }
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
                    #region 현재 모델 배경색
                    for (int i = 0; i < dataGridModelList.Rows.Count; i++)
                    {
                        if (GlobalVar.ModelName == dataGridModelList.Rows[i].Cells[1].Value.ToString())
                        {
                            dataGridModelList.Rows[i].DefaultCellStyle.BackColor = Color.Lime;
                           
                        }
                        //else if (previewModel == dataGridModelList.Rows[i].Cells[1].Value.ToString())
                        //{
                        //    dataGridModelList.Rows[i].DefaultCellStyle.BackColor = Color.DarkCyan;
                        //}
                        //else
                        //{
                        //    dataGridModelList.Rows[i].DefaultCellStyle.BackColor = Color.White;
                        //}
                    }
                    #endregion
                    #region 미리보기 모델 배경색
                    //for (int i = 0; i < dataGridModelList.Rows.Count; i++)
                    //{
                    //    if (GlobalVar.ModelName == dataGridModelList.Rows[i].Cells[1].Value.ToString())
                    //    {
                    //        //dataGridModelList.Rows[i].DefaultCellStyle.BackColor = Color.Lime;
                    //    }
                    //    else if (previewModel == dataGridModelList.Rows[i].Cells[1].Value.ToString())
                    //    {
                    //        dataGridModelList.Rows[i].DefaultCellStyle.BackColor = Color.Cyan;
                    //    }
                    //    //else
                    //    //{
                    //    //    dataGridModelList.Rows[i].DefaultCellStyle.BackColor = Color.White;
                    //    //}
                    //}
                    #endregion
                    #region 기본 배경색
                    for (int i = 0; i < dataGridModelList.Rows.Count; i++)
                    {
                        if (GlobalVar.ModelName == dataGridModelList.Rows[i].Cells[1].Value.ToString())
                        {
                            //dataGridModelList.Rows[i].DefaultCellStyle.BackColor = Color.Lime;
                        }
                        else if (previewModel == dataGridModelList.Rows[i].Cells[1].Value.ToString())
                        {
                            //dataGridModelList.Rows[i].DefaultCellStyle.BackColor = Color.DarkCyan;
                        }
                        else
                        {
                            dataGridModelList.Rows[i].DefaultCellStyle.BackColor = Color.White;
                        }
                    }
                    #endregion

                    #region 위치값 표시
                 
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

        #region Model 관리
        private void loadModelList()
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

        private void pbLoad_Click(object sender, EventArgs e)
        {
            if (dataGridModelList.Rows.Count == 0 ||
                dataGridModelList.SelectedCells.Count == 0 ||
                dataGridModelList.SelectedCells[0].RowIndex < 0 ||
                dataGridModelList.SelectedCells[0].RowIndex >= modelCount)
            {
                FuncWin.TopMessageBox("Select Model first!");
                return;
            }
            //string modelName = dataGridModelList.SelectedRows[0].Cells[1].Value.ToString();
            //string modelName = tbModelName.Text;
            string modelName = previewModel;

            if(modelName == "")
            {
                modelName = tbModelName.Text;
            }

            if (FuncWin.MessageBoxOK("Load Model Setting named " + modelName))
            {
                GlobalVar.ModelName = modelName;
                previewModel = modelName;
                FuncIni.LoadModelIni();
                LoadAllValue();
            }
        }

        private void pbSave_Click(object sender, EventArgs e)
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
            if (FuncWin.MessageBoxOK("Save Model Setting named " + modelName))
            {
                GlobalVar.ModelName = modelName;
                ApplyAllValue();
                FuncIni.SaveModelIni();
                //Func.SaveTeachingIni();
            }
        }

        private void pbDelete_Click(object sender, EventArgs e)
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
            if (FuncWin.MessageBoxOK("Delete Model Setting named " + modelName))
            {
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
                        FuncIni.LoadModelIni();
                        LoadAllValue();
                    }
                    else
                    {
                        GlobalVar.ModelName = "";
                    }
                }
            }
        }

        private void pbAdd_Click(object sender, EventArgs e)
        {
            if (tbModelName.Text.Length == 0)
            {
                FuncWin.TopMessageBox("Input Model Name");
                return;
            }
            #region 중복체크
            for (int i = 0; i < dataGridModelList.Rows.Count; i++)
            {
                if (dataGridModelList.Rows[i].Cells[1].Value.ToString() == tbModelName.Text)
                {
                    FuncWin.TopMessageBox("Same Model Name already exists. Try another name.");
                    return;
                }
            }
            #endregion

            Console.WriteLine("add modelName : " + tbModelName.Text);
            GlobalVar.ModelName = tbModelName.Text;
            previewModel = tbModelName.Text;

            ApplyAllValue();
            FuncIni.SaveModelIni();
            //Func.SaveTeachingIni();

            loadModelList();
            dataGridModelList.CurrentCell = dataGridModelList.Rows[dataGridModelList.Rows.Count - 1].Cells[1];
        }

        #endregion

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
            buttonOk.Text = "확인";
            buttonCancel.Text = "취소";

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
     
        //클릭한 모델이름 미리보기 주석처리 by DG 241028
        private void dataGridModelList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridModelList.SelectedCells.Count > 0 &&
                dataGridModelList.SelectedCells[0].RowIndex >= 0 &&
                dataGridModelList.SelectedCells[0].RowIndex < dataGridModelList.Rows.Count)
            {
                string model = dataGridModelList.Rows[dataGridModelList.SelectedCells[0].RowIndex].Cells[1].Value.ToString();
                previewModel = model;
                //PreviewModel(model);
            }
        }

        public void PreviewModel(string model) // 클릭한 모델 미리보기. INI 읽어서 폼에만 표시
        {
            //tbModelName.Text = model;

            #region INI 읽기
            string IniPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.ModelPath + "\\" + model + ".ini";
            string Section = GlobalVar.IniSection;

            /*
            #region Robot Offset
            //for (int i = 0; i < RobotCount; i++)
            //{
            //    Offset_Robot[i].x = double.Parse(FuncIni.ReadIniFile(Section, "Offset_Robot_" + i + "_x", IniPath, "0"));
            //    Offset_Robot[i].y = double.Parse(FuncIni.ReadIniFile(Section, "Offset_Robot_" + i + "_y", IniPath, "0"));
            //    Offset_Robot[i].z = double.Parse(FuncIni.ReadIniFile(Section, "Offset_Robot_" + i + "_z", IniPath, "0"));
            //}

            int CartLoadingPositionSensor = int.Parse(FuncIni.ReadIniFile(Section, "CartLoadingPositionSensor", IniPath, "0"));
            int CartWeldingPositionSensor = int.Parse(FuncIni.ReadIniFile(Section, "CartWeldingPositionSensor", IniPath, "0"));

            int CartPositionLoading = int.Parse(FuncIni.ReadIniFile(Section, "CartPositionLoading", IniPath, "0"));
            int CartPositionWelding = int.Parse(FuncIni.ReadIniFile(Section, "CartPositionWelding", IniPath, "0"));
            int CartPositionUnloading = int.Parse(FuncIni.ReadIniFile(Section, "CartPositionUnloading", IniPath, "0"));
            int ModulerCheckDistance = int.Parse(FuncIni.ReadIniFile(Section, "ModulerCheckDistance", IniPath, "100"));

            int LoadingStopperPos = int.Parse(FuncIni.ReadIniFile(Section, "LoadingStopperPos", IniPath, "1"));
            int LoadingFrontGuidePos = int.Parse(FuncIni.ReadIniFile(Section, "LoadingFrontGuidePos", IniPath, "1"));
            int LoadingRearGuidePos = int.Parse(FuncIni.ReadIniFile(Section, "LoadingRearGuidePos", IniPath, "1"));
            int WeldingStopperPos = int.Parse(FuncIni.ReadIniFile(Section, "WeldingStopperPos", IniPath, "1"));
            int WeldingFrontGuidePos = int.Parse(FuncIni.ReadIniFile(Section, "WeldingFrontGuidePos", IniPath, "1"));
            int WeldingRearGuidePos = int.Parse(FuncIni.ReadIniFile(Section, "WeldingRearGuidePos", IniPath, "1"));


            //WeldUpOnly = FuncIni.ReadIniFile(Section, "WeldUpOnly", IniPath, "False") == "True";
            int RobotModel = int.Parse(FuncIni.ReadIniFile(Section, "RobotModel", IniPath, "1"));

            bool[,,] WeldUpDown = new bool[FuncWelding.RobotCount, 2, 2];
            for (int robot = 0; robot < FuncWelding.RobotCount; robot++)
            {
                for (int j = 0; j < 2; j++) // 상하부
                {
                    for (int i = 0; i < 2; i++) // 1/2차
                    {
                        WeldUpDown[robot, j, i] = FuncIni.ReadIniFile(Section, "WeldUpDown" + robot + "_" + j + "_" + i, IniPath, "False") == "True";
                    }
                }
            }

            bool[,] UsePusher = new bool[FuncWelding.UsePusher.GetLength(0), FuncWelding.UsePusher.GetLength(1)];
            for (int j = 0; j < FuncWelding.UsePusher.GetLength(0); j++)
            {
                for (int i = 0; i < FuncWelding.UsePusher.GetLength(1); i++)
                {
                    UsePusher[j, i] = FuncIni.ReadIniFile(Section, "UsePusher" + j + "_" + i, IniPath, "True") == "True";
                }
            }
            #endregion
            #endregion

            #region 현재모델 선택
            for (int i = 0; i < dataGridModelList.Rows.Count; i++)
            {
                if (dataGridModelList.Rows[i].Cells[1].Value.ToString() == GlobalVar.ModelName)
                {
                    dataGridModelList.Rows[i].Selected = true;
                    break;
                }
            }
            #endregion

            //tbModelName.Text = GlobalVar.ModelName;


            #region Raobot Offset 관련 셋팅
            //numOffsetRobot1X.Value = (decimal)FuncWelding.Offset_Robot[0].x;
            //numOffsetRobot1Y.Value = (decimal)FuncWelding.Offset_Robot[0].y;
            //numOffsetRobot1Z.Value = (decimal)FuncWelding.Offset_Robot[0].z;
            //numOffsetRobot2X.Value = (decimal)FuncWelding.Offset_Robot[1].x;
            //numOffsetRobot2Y.Value = (decimal)FuncWelding.Offset_Robot[1].y;
            //numOffsetRobot2Z.Value = (decimal)FuncWelding.Offset_Robot[1].z;
            //numOffsetRobot3X.Value = (decimal)FuncWelding.Offset_Robot[2].x;
            //numOffsetRobot3Y.Value = (decimal)FuncWelding.Offset_Robot[2].y;
            //numOffsetRobot3Z.Value = (decimal)FuncWelding.Offset_Robot[2].z;
            //numOffsetRobot4X.Value = (decimal)FuncWelding.Offset_Robot[3].x;
            //numOffsetRobot4Y.Value = (decimal)FuncWelding.Offset_Robot[3].y;
            //numOffsetRobot4Z.Value = (decimal)FuncWelding.Offset_Robot[3].z;
            //numOffsetRobot5X.Value = (decimal)FuncWelding.Offset_Robot[4].x;
            //numOffsetRobot5Y.Value = (decimal)FuncWelding.Offset_Robot[4].y;
            //numOffsetRobot5Z.Value = (decimal)FuncWelding.Offset_Robot[4].z;
            //numOffsetRobot6X.Value = (decimal)FuncWelding.Offset_Robot[5].x;
            //numOffsetRobot6Y.Value = (decimal)FuncWelding.Offset_Robot[5].y;
            //numOffsetRobot6Z.Value = (decimal)FuncWelding.Offset_Robot[5].z;

            btnLoadingCartPosition1.BackColor = CartLoadingPositionSensor == 0 ? Color.Lime : Color.White;
            btnLoadingCartPosition2.BackColor = CartLoadingPositionSensor == 1 ? Color.Lime : Color.White;
            btnLoadingCartPosition3.BackColor = CartLoadingPositionSensor == 2 ? Color.Lime : Color.White;
            btnLoadingCartPosition4.BackColor = CartLoadingPositionSensor == 3 ? Color.Lime : Color.White;
            btnWeldingCartPosition1.BackColor = CartWeldingPositionSensor == 0 ? Color.Lime : Color.White;
            btnWeldingCartPosition2.BackColor = CartWeldingPositionSensor == 1 ? Color.Lime : Color.White;
            btnWeldingCartPosition3.BackColor = CartWeldingPositionSensor == 2 ? Color.Lime : Color.White;
            btnWeldingCartPosition4.BackColor = CartWeldingPositionSensor == 3 ? Color.Lime : Color.White;


            numCartPositionLoading.Value = (decimal)CartPositionLoading;
            numCartPositionWelding.Value = (decimal)CartPositionWelding;
            numCartPositionUnloading.Value = (decimal)CartPositionUnloading;
            numModulerCheckDistance.Value = (decimal)ModulerCheckDistance;

            for (int i = 1; i <= 5; i++)
            {
                ((Button)(Controls.Find("btnWeldingStopperPos" + i, true)[0])).BackColor = WeldingStopperPos == i ? Color.Lime : Color.White;
                ((Button)(Controls.Find("btnWeldingFrontGuidePos" + i, true)[0])).BackColor = WeldingFrontGuidePos == i ? Color.Lime : Color.White;
                ((Button)(Controls.Find("btnWeldingRearGuidePos" + i, true)[0])).BackColor = WeldingRearGuidePos == i ? Color.Lime : Color.White;
                ((Button)(Controls.Find("btnLoadingStopperPos" + i, true)[0])).BackColor = LoadingStopperPos == i ? Color.Lime : Color.White;
                ((Button)(Controls.Find("btnLoadingFrontGuidePos" + i, true)[0])).BackColor = LoadingFrontGuidePos == i ? Color.Lime : Color.White;
                ((Button)(Controls.Find("btnLoadingRearGuidePos" + i, true)[0])).BackColor = LoadingRearGuidePos == i ? Color.Lime : Color.White;
            }


            //btnWeldDown1.BackColor = !FuncWelding.WeldUpOnly ? Color.Yellow : Color.White;
            //btnWeldDown2.BackColor = FuncWelding.WeldUpOnly ? Color.Yellow : Color.White;
            numRobotModel.Value = (decimal)RobotModel;
            for (int robot = 0; robot < FuncWelding.RobotCount; robot++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        try
                        {
                            ((Button)(Controls.Find("btnUpDown" + robot + "_" + j + "_" + i, true)[0])).BackColor = WeldUpDown[robot, j, i] ? Color.Lime : Color.White;
                        }
                        catch { }
                    }
                }
            }

            for (int j = 0; j < FuncWelding.UsePusher.GetLength(0); j++)
            {
                string strPos = "Front";
                if (j > 0)
                {
                    strPos = "Rear";
                }
                for (int i = 0; i < FuncWelding.UsePusher.GetLength(1); i++)
                {
                    ((Button)(Controls.Find("btnUsePusher" + strPos + (i + 1), true)[0])).BackColor = UsePusher[j, i] ? Color.Lime : Color.White;
                }
            }

            //*/
            #endregion
        }

        private void Model_VisibleChanged(object sender, EventArgs e)
        {
            LoadAllValue();
        }
    }
}
