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
using System.Collections.Concurrent; // ConcurrentQueue
using System.IO;

namespace Radix
{
    /*
     * Manual.cs : 각 파트 및 장비의 수동 운전
     */

    public partial class Select_Model : Form
    {
        private System.Threading.Timer timerCheck;
        private bool timerDoing = false;
        private double ModelHoleSize = 0;
        private double FixtureLength = 0;
        private int TrayNum = 0;
        private int TrayCount = 0;
        int count = 1; // 초기 숫자

        private Model _model;

      

        public Select_Model(int numerValue)
        {
            InitializeComponent();
            // ListBox 설정
            lbModel1.DrawMode = DrawMode.OwnerDrawFixed;
            lbModel1.ItemHeight = 50;  // 항목의 높이 (간격 조정)
            lbModel2.DrawMode = DrawMode.OwnerDrawFixed;
            lbModel2.ItemHeight = 50;  // 항목의 높이(간격 조정)
            lbHoleSize.DrawMode = DrawMode.OwnerDrawFixed;
            lbHoleSize.ItemHeight = 50;  // 항목의 높이 (간격 조정)
            lbFixtureLength.DrawMode = DrawMode.OwnerDrawFixed;
            lbFixtureLength.ItemHeight = 50;  // 항목의 높이 (간격 조정)
            // DrawItem 이벤트 핸들러 등록
            lbModel1.DrawItem += new DrawItemEventHandler(list_DrawItem);
            // DrawItem 이벤트 핸들러 등록
            lbModel2.DrawItem += new DrawItemEventHandler(list_DrawItem);
            // DrawItem 이벤트 핸들러 등록
            lbHoleSize.DrawItem += new DrawItemEventHandler(list_DrawItem);
            // DrawItem 이벤트 핸들러 등록
            lbFixtureLength.DrawItem += new DrawItemEventHandler(list_DrawItem);

            // 전달된 트레이 번호를 lb에 설정
            if(numerValue < 7)
            {
                if(numerValue == 6) lbTrayNum.Text = $"1번 트레이";
                if (numerValue == 5) lbTrayNum.Text = $"2번 트레이";
                if (numerValue == 4) lbTrayNum.Text = $"3번 트레이";
                if (numerValue == 3) lbTrayNum.Text = $"4번 트레이";
                if (numerValue == 2) lbTrayNum.Text = $"5번 트레이";
                if (numerValue == 1) lbTrayNum.Text = $"6번 트레이";
            }
            else
            {
                lbTrayNum.Text = $"작업위치 모델 정보";
            }
            TrayNum = numerValue -1;   //Tray Num 정보
        }

        // DrawItem 이벤트 핸들러
        private void list_DrawItem(object sender, DrawItemEventArgs e)
        {
            // ListBox 참조를 sender에서 가져옴
            ListBox lb = (ListBox)sender;

            // 항목 기본 배경 및 텍스트 설정
            e.DrawBackground();

            // 항목 텍스트 그리기
            e.Graphics.DrawString(lb.Items[e.Index].ToString(), e.Font, Brushes.Black, e.Bounds);

            // 항목 아래쪽에 구분선을 그리기
            using (Pen pen = new Pen(Color.Gray, 1))
            {
                e.Graphics.DrawLine(pen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
            }

            // 선택된 항목에 포커스 사각형 그리기
            e.DrawFocusRectangle();
        }

        private void debug(string str)
        {
            Util.Debug(" : " + str);
        }


        #region 초기화 관련

        private void Selct_Model_Shown(object sender, EventArgs e)
        {
            GlobalVar.dlgOpened = true;

            // 타이머 시작
            TimerCallback CallBackCheck = new TimerCallback(TimerCheck);
            timerCheck = new System.Threading.Timer(CallBackCheck, false, 0, 100);

            //dgModel2.Rows.Add( "+II", "+I","+", "+B", "1.8", "Null");
            //dgHoleSize.Rows.Add("35", "37", "38", "40", "43", "48", "53");
            //dgFixtureLength.Rows.Add("08", "09", "10", "11", "12","13","14","15", "16", "17","18");
            // "RFF", "AFS"

            UpdateDisplay();
            btnToday_Click(sender, e);
            this.BringToFront();

        }

        private void Selct_Model_FormClosed(object sender, FormClosedEventArgs e)
        {
            timerCheck.Dispose();
            GlobalVar.dlgOpened = false;
            if (this.Parent != null)
            {
                try
                {
                    timerCheck.Dispose();
                    this.Parent.BringToFront();
                }
                catch
                { }
            }
        }

        #endregion
 
        #region 타이머 함수
        private void TimerCheck(Object state)
        {
            if (FuncInline.TabMain != FuncInline.enumTabMain.Auto) // 메인 탭이 다른 곳에 있으면 실행 안 한다.
            {
                timerDoing = false;
                return;
            }
            try
            {
                if (timerDoing || this.IsDisposed || !this.IsHandleCreated)
                {
                    return;
                }

              

                timerDoing = true;



                this.Invoke((MethodInvoker)delegate
                {
                    ulong startTime = GlobalVar.TickCount64;


                    tbLot_Model.Text = GetSelectedItemsText();//모델정보 최신화
                    //길이 저장
                    if(lbFixtureLength.SelectedItem != null)
                    {
                        // 선택된 항목의 텍스트를 가져옴
                        string selectedText = lbFixtureLength.SelectedItem.ToString();

                        // 문자열을 double로 변환
                        double size;
                        bool isParsed = double.TryParse(selectedText, out size);

                        // 변환 성공 여부 확인
                        if (isParsed)
                        {
                            FixtureLength = size;
                        }
                      
                        lb1.Text = FixtureLength.ToString();
                    }

                    //HoleSize 저장
                    ConvertSelectedHoleSize();
                    //debug("manual ui time : " + (GlobalVar.TickCount64 - startTime).ToString());

                });

                timerDoing = false;

            }
            catch (Exception ex)
            {
                FuncLog.WriteLog(ex.ToString());
                FuncLog.WriteLog(ex.StackTrace);
            }
            timerDoing = false;
        }

        #endregion



        private string GetSelectedItemsText()
        {
            // 선택된 항목 텍스트를 저장할 리스트
            List<string> selectedTexts = new List<string>();

            // 각 ListBox에서 선택된 인덱스를 가져와 해당 항목의 텍스트를 리스트에 추가
            if (lbModel1.SelectedIndex != -1)
            {
                selectedTexts.Add(lbModel1.Items[lbModel1.SelectedIndex].ToString());
            }

            if (lbModel2.SelectedIndex != -1)
            {
                if (lbModel2.Items[lbModel2.SelectedIndex].ToString() == "Null")
                {
                    selectedTexts.Add("");
                }
                else
                {
                    selectedTexts.Add(lbModel2.Items[lbModel2.SelectedIndex].ToString());
                }

            }

            selectedTexts.Add("-");

            if (lbHoleSize.SelectedIndex != -1)
            {
                selectedTexts.Add(lbHoleSize.Items[lbHoleSize.SelectedIndex].ToString());
            }

            if (lbFixtureLength.SelectedIndex != -1)
            {
                selectedTexts.Add(lbFixtureLength.Items[lbFixtureLength.SelectedIndex].ToString());
            }

            // 선택된 항목들의 텍스트를 하나의 문자열로 합침
            string result = string.Join("", selectedTexts);


            return result;
        }

        private void btnSelect_Model_Colse_Click(object sender, EventArgs e)
        {
            timerCheck.Dispose();

            this.Close();
        }

        private void ConvertSelectedHoleSize()
        {
            // 선택된 항목이 있는지 확인
            if (lbHoleSize.SelectedItem != null)
            {
                // 선택된 항목의 텍스트를 가져옴
                string selectedText = lbHoleSize.SelectedItem.ToString();

                // 문자열을 int로 변환
                int size;
                bool isParsed = int.TryParse(selectedText, out size);

                // 변환 성공 여부 확인
                if (isParsed)
                {
                    // 소수점으로 변환 (10으로 나눠서 소수점 표현)
                    double convertedSize = size / 10.0;
                    ModelHoleSize = convertedSize;
                }
                else
                {

                }
            }
            else
            {

            }

        }

        private void btnSelectModel_Apply_Click(object sender, EventArgs e)
        {
            //// 선택된 행이 있는지 확인
            //if (dataGridModelList.SelectedRows.Count > 0)
            //{
            //    // 선택된 행의 첫 번째 셀 값(모델명)을 가져옴
            //    string selectedModelText = dataGridModelList.SelectedRows[0].Cells[1].Value.ToString();
            //    FuncSanding.Tray[TrayNum].Model = selectedModelText;

            //    if(TrayNum == 6)    //작업위치에 바로 적용하는경우, 픽스쳐 사용 X, Bypass모드일때
            //    {
            //        GlobalVar.ModelName = selectedModelText;
            //        FuncIni.LoadModelIni();
     
            //    }
            //}
            //else
            //{
            //    FuncWin.TopMessageBox("선택된 티칭모델이 없습니다.");
            //    return;
            //}

            //if (int.TryParse(tbCount.Text, out TrayCount))
            //{
            //    FuncSanding.Tray[TrayNum].Total = TrayCount;
            //}
            //else
            //{
            //    FuncWin.TopMessageBox("갯수 입력이 잘못되었습니다.");
            //    return;
            //}


            //if (lbModel1.SelectedIndex != -1 &&
            //    lbModel2.SelectedIndex != -1 &&
            //    lbHoleSize.SelectedIndex != -1 &&
            //    lbFixtureLength.SelectedIndex != -1)
            //{
            //    FuncSanding.Tray[TrayNum].Lot_Model1 = lbModel1.SelectedItem.ToString();
            //    FuncSanding.Tray[TrayNum].Lot_Model2 = lbModel2.SelectedItem.ToString();
            //    FuncSanding.Tray[TrayNum].Lot_Model3 = lbHoleSize.SelectedItem.ToString();
            //    FuncSanding.Tray[TrayNum].Lot_Model4 = lbFixtureLength.SelectedItem.ToString();
            //    FuncSanding.Tray[TrayNum].Lot_Model = tbLot_Model.Text;
            //}
            //else
            //{
            //    MessageBox.Show("LOT모델을 선택해 주세요");
             
            //    return;
            //}

            //if (tbLot_Num1.Text.Length == 6 && tbLot_Num2.Text.Length == 3)
            //{
            //    FuncSanding.Tray[TrayNum].LotNo =  "F"+ tbLot_Num1.Text.ToString().Trim()+ tbLot_Num2.Text.ToString().Trim();
            //}
            //else
            //{
            //    if(tbLot_Num1.Text.Length > 0)
            //    {
            //        MessageBox.Show("LOT 날짜 길이가 맞지 않습니다");
            //    }
            //    else if (tbLot_Num2.Text.Length > 0)
            //    {
            //        MessageBox.Show("LOT 번호 길이가 맞지 않습니다");
            //    }
            //    else
            //    {
            //        MessageBox.Show("LOT를 입력해 주세요");

            //    }
                

            //    return;
            //}
            //FuncSanding.Tray[TrayNum].TrayInfo = TrayNum; 
            //FuncSanding.Tray[TrayNum].isFixture = true;
            //FuncSanding.Tray[TrayNum].isTray = true;

            this.Close();
        }

        private void SelectSizeModel(object sender, EventArgs e)
        {
            try
            {
                if (lbHoleSize.SelectedIndex < 0 ||
                    lbHoleSize.SelectedIndex >= lbHoleSize.Items.Count ||
                    lbFixtureLength.SelectedIndex < 0 ||
                    lbFixtureLength.SelectedIndex >= lbFixtureLength.Items.Count)
                {
                    debug("선택 데이터 없음");
                    return;
                }
                string hole = lbHoleSize.Items[lbHoleSize.SelectedIndex].ToString();
                string length = lbFixtureLength.Items[lbFixtureLength.SelectedIndex].ToString();

                double hole_value = 0;
                double length_value = 0;
                double.TryParse(hole, out hole_value);
                double.TryParse(length, out length_value);

                if (hole_value == 0 ||
                    length_value == 0)
                {
                    debug("선택 값 없음");
                    return;
                }
                hole_value /= 10;

                dataGridModelList.Rows.Clear();

                string model_path = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.ModelPath;
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(model_path);
                int modelCount = 0;
                foreach (System.IO.FileInfo File in di.GetFiles())
                {
                    if (File.Extension.ToLower().CompareTo(".ini") == 0)
                    {
                        String FileNameOnly = File.Name.Substring(0, File.Name.Length - 4);

                        #region 모델정보 읽어서 홀과 길이 다르면 Pass
                        string IniPath = model_path + "\\" + FileNameOnly + ".ini";
                        string Section = GlobalVar.IniSection;

                        double hole_model = double.Parse(FuncIni.ReadIniFile(Section, "FixtureholeSize", IniPath, "0"));
                        double length_model = double.Parse(FuncIni.ReadIniFile(Section, "FixtureLength", IniPath, "0"));
                        if (hole_value != hole_model ||
                            length_value != length_model)
                        {
                            continue;
                        }
                        #endregion

                        modelCount++;
                        dataGridModelList.Rows.Add(modelCount.ToString(), FileNameOnly);
                        dataGridModelList.Rows[0].Selected = true; // 무조건 첫 번째 모델을 선택되게 한다.
                    }
                }

            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        private void btnFull_Click(object sender, EventArgs e)
        {
            tbCount.Text = "120";
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            //FuncSanding.Tray[TrayNum].Clear();
            this.Close();
        }

        private void Select_Model_Load(object sender, EventArgs e)
        {
            //tbCount.Text = FuncSanding.Tray[TrayNum].Total.ToString();
            //lbModel1.SelectedItem = FuncSanding.Tray[TrayNum].Lot_Model1;
            //lbModel2.SelectedItem = FuncSanding.Tray[TrayNum].Lot_Model2;
            //lbHoleSize.SelectedItem = FuncSanding.Tray[TrayNum].Lot_Model3;
            //lbFixtureLength.SelectedItem = FuncSanding.Tray[TrayNum].Lot_Model4;
            //foreach (DataGridViewRow row in dataGridModelList.Rows)
            //{
            //    if (row.Cells[1].Value != null && row.Cells[1].Value.ToString() == FuncSanding.Tray[TrayNum].Model)
            //    {
            //        row.Selected = true; // 해당 행을 선택
            //        dataGridModelList.FirstDisplayedScrollingRowIndex = row.Index; // 해당 행으로 스크롤
            //        break;
            //    }
            //}

        }

        private void btnToday_Click(object sender, EventArgs e)
        {
            tbLot_Num1.Text = DateTime.Now.ToString("yyMMdd");
        }

        private void btnNumUp_Click(object sender, EventArgs e)
        {
            //FuncSanding.LOTNum_Count++;
            UpdateDisplay();
        }


        private void btnNumDown_Click(object sender, EventArgs e)
        {
            //if (FuncSanding.LOTNum_Count > 1) // 1 이하로 내려가지 않도록 설정
            //{
            //    FuncSanding.LOTNum_Count--;
            //    UpdateDisplay();
            //}
        }

        private void UpdateDisplay()
        {
            //tbLot_Num2.Text = FuncSanding.LOTNum_Count.ToString("D3"); // 3자리 수로 표시 (001, 002 등)
        }

        private void Select_Model_Move(object sender, EventArgs e)
        {
            //여기 타나
        }
    }
}

