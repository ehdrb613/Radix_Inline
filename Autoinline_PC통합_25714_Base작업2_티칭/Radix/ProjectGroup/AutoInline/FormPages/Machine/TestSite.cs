using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace Radix.Popup.Machine
{
    public partial class TestSite : Form
    {
        private bool valueChanged = false;
        private FuncInline.enumTabMain beforeMain = FuncInline.enumTabMain.Auto;
        private FuncInline.enumTabMachine beforeMachine = FuncInline.enumTabMachine.Machine;
        private bool checkAll = false; // 전체 체크 이상 없는가? 여러 이벤트 연속실행시 이상 체크용
        private bool loaded = false; // 초기 로드시 변경 이벤트 방지용
        private bool allCheck = false; // PC단위 이상 이벤트 실행시 개별 이벤트 방지용

        private static string FTErrorFileName = "FTError.dat";
        private static string sectionErrorName = "FT_Error_Name";
        private static string sectionRetest = "Retest";
        private static string sectionRetestType = "Retest Type";

        // 사이트별 투입 순서. 글로벌에는 순서별로 사이트가 지정되어 있어 역파싱해서 계산한다.
        private int[] order = new int[FuncInline.MaxSiteCount];
        // 콤보박스 변경 이벤트가 반복 실행되는 것을 막기 위해
        private bool orderChange = false;

        public TestSite()
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
                loaded = false;

                btnUseSite1.BackColor = FuncInline.UseSite[0] ? Color.Lime : Color.White;
                btnUseSite2.BackColor = FuncInline.UseSite[1] ? Color.Lime : Color.White;
                btnUseSite3.BackColor = FuncInline.UseSite[2] ? Color.Lime : Color.White;
                btnUseSite4.BackColor = FuncInline.UseSite[3] ? Color.Lime : Color.White;
                btnUseSite5.BackColor = FuncInline.UseSite[4] ? Color.Lime : Color.White;
                btnUseSite6.BackColor = FuncInline.UseSite[5] ? Color.Lime : Color.White;
                btnUseSite7.BackColor = FuncInline.UseSite[6] ? Color.Lime : Color.White;
                btnUseSite8.BackColor = FuncInline.UseSite[7] ? Color.Lime : Color.White;
                btnUseSite9.BackColor = FuncInline.UseSite[8] ? Color.Lime : Color.White;
                btnUseSite10.BackColor = FuncInline.UseSite[9] ? Color.Lime : Color.White;
                btnUseSite11.BackColor = FuncInline.UseSite[10] ? Color.Lime : Color.White;
                btnUseSite12.BackColor = FuncInline.UseSite[11] ? Color.Lime : Color.White;
                btnUseSite13.BackColor = FuncInline.UseSite[12] ? Color.Lime : Color.White;
                btnUseSite14.BackColor = FuncInline.UseSite[13] ? Color.Lime : Color.White;
                btnUseSite15.BackColor = FuncInline.UseSite[14] ? Color.Lime : Color.White;
                btnUseSite16.BackColor = FuncInline.UseSite[15] ? Color.Lime : Color.White;
                btnUseSite17.BackColor = FuncInline.UseSite[16] ? Color.Lime : Color.White;
                btnUseSite18.BackColor = FuncInline.UseSite[17] ? Color.Lime : Color.White;
                btnUseSite19.BackColor = FuncInline.UseSite[18] ? Color.Lime : Color.White;
                btnUseSite20.BackColor = FuncInline.UseSite[19] ? Color.Lime : Color.White;

                //int leftIndex = (int)FuncInline.BuyerChangeSite[0] - (int)FuncInline.enumTeachingPos.Site1_F_DT1;
                //if (leftIndex >= 0 &&
                //    leftIndex < cmbBuyerChangeSite1.Items.Count)
                //{
                //    cmbBuyerChangeSite1.SelectedIndex = leftIndex;
                //}
                //int midIndex = (int)FuncInline.BuyerChangeSite[1] - (int)FuncInline.enumTeachingPos.Site8;
                //if (midIndex >= 0 &&
                //    midIndex < cmbBuyerChangeSite2.Items.Count)
                //{
                //    cmbBuyerChangeSite2.SelectedIndex = midIndex;
                //}
                //int rightIndex = (int)FuncInline.BuyerChangeSite[2] - (int)FuncInline.enumTeachingPos.Site15;
                //if (rightIndex >= 0 &&
                //    rightIndex < cmbBuyerChangeSite3.Items.Count)
                //{
                //    cmbBuyerChangeSite3.SelectedIndex = rightIndex;
                //}

                orderToSite();
                SetComboOrder();

                valueChanged = false;
                loaded = true;
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
                FuncInline.EnableSite(0, btnUseSite1.BackColor == Color.Lime);
                FuncInline.EnableSite(1, btnUseSite2.BackColor == Color.Lime);
                FuncInline.EnableSite(2, btnUseSite3.BackColor == Color.Lime);
                FuncInline.EnableSite(3, btnUseSite4.BackColor == Color.Lime);
                FuncInline.EnableSite(4, btnUseSite5.BackColor == Color.Lime);
                FuncInline.EnableSite(5, btnUseSite6.BackColor == Color.Lime);
                FuncInline.EnableSite(6, btnUseSite7.BackColor == Color.Lime);
                FuncInline.EnableSite(7, btnUseSite8.BackColor == Color.Lime);
                FuncInline.EnableSite(8, btnUseSite9.BackColor == Color.Lime);
                FuncInline.EnableSite(9, btnUseSite10.BackColor == Color.Lime);
                FuncInline.EnableSite(10, btnUseSite11.BackColor == Color.Lime);
                FuncInline.EnableSite(11, btnUseSite12.BackColor == Color.Lime);
                FuncInline.EnableSite(12, btnUseSite13.BackColor == Color.Lime);
                FuncInline.EnableSite(13, btnUseSite14.BackColor == Color.Lime);
                FuncInline.EnableSite(14, btnUseSite15.BackColor == Color.Lime);
                FuncInline.EnableSite(15, btnUseSite16.BackColor == Color.Lime);
                FuncInline.EnableSite(16, btnUseSite17.BackColor == Color.Lime);
                FuncInline.EnableSite(17, btnUseSite18.BackColor == Color.Lime);
                FuncInline.EnableSite(18, btnUseSite19.BackColor == Color.Lime);
                FuncInline.EnableSite(19, btnUseSite20.BackColor == Color.Lime);

                for (int i = 0; i < FuncInline.UseSite.Length; i++)
                {
                    //UseSite[i] = ((CheckBox)Controls.Find("cbUseSite" + (i + 1), true)[0]).Checked;
                    //debug("UseSite " + i + " = cbUseSite" + (i + 1) + ".checked " + FuncInline.UseSite[i]);
                    #region 사용으로 체크된 사이트의 모든 어레이 NG 카운트 초기화
                    if (FuncInline.UseSite[i])
                    {
                        //for (int j = 0; j < 21; j++) // 왜 전체 사이트로 해 놨을까?
                        //{
                        for (int k = 0; k < FuncInline.MaxArrayCount; k++)
                        {
                            FuncInline.ArrayNGCount[i, k] = 0;
                            FuncInline.ArrayNGCode[i, k] = -1;
                        }
                        //}
                    }
                    #endregion
                }
                //FuncInline.BuyerChangeSite[0] = FuncInline.enumTeachingPos.Site1_F_DT1 + cmbBuyerChangeSite1.SelectedIndex;
                //FuncInline.BuyerChangeSite[1] = FuncInline.enumTeachingPos.Site8 + cmbBuyerChangeSite2.SelectedIndex;
                //FuncInline.BuyerChangeSite[2] = FuncInline.enumTeachingPos.Site15 + cmbBuyerChangeSite3.SelectedIndex;

                siteToOrder();

                valueChanged = false;
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }
        #endregion

        public void ResetTestErrorCode()
        {
            dataGridErrorCode.Rows.Clear();
            FuncInline.GetAllTestError();
            for (int i = 0; i < FuncInline.TestErrorCode.Length; i++)
            {
                if (FuncInline.TestErrorCode[i].Length > 0)
                {
                    dataGridErrorCode.Rows.Add(Util.IntToString(i, 2), FuncInline.TestErrorCode[i], FuncInline.TestErrorRetest[i], FuncInline.TestErrorRetestType[i]);
                }
            }
        }

        private void pbErrorCodeRefresh_Click(object sender, EventArgs e)
        {
            ResetTestErrorCode();
        }

        private void dataGridErrorCode_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dataGridErrorCode.SelectedCells.Count > 0)
                //dataGridErrorCode.SelectedRows.Count > 0)
                {
                    int code = int.Parse(dataGridErrorCode.Rows[dataGridErrorCode.SelectedCells[0].RowIndex].Cells[0].Value.ToString());
                    string name = dataGridErrorCode.Rows[dataGridErrorCode.SelectedCells[0].RowIndex].Cells[1].Value.ToString();
                    bool retest = dataGridErrorCode.Rows[dataGridErrorCode.SelectedCells[0].RowIndex].Cells[2].Value.ToString() == "True";
                    string retestType = dataGridErrorCode.Rows[dataGridErrorCode.SelectedCells[0].RowIndex].Cells[3].Value.ToString();

                    if (name.Length > 0)
                    {
                        numErrorCode.Value = (decimal)code;
                        tbErrorCode.Text = name;
                        cbRetest.Checked = retest;
                        tbRetestType.Text = retestType;
                    }
                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        private void pbErrorCodeSave_Click(object sender, EventArgs e)
        {
            if (tbErrorCode.Text.Trim().Length == 0)
            {
                FuncWin.TopMessageBox("input Error Name");
                return;
            }
            if (GlobalVar.Sql == null ||
                !GlobalVar.Sql.connected)
            {
                return;
            }
            //string sql = "if exists (select * from TestErrorCode where ErrorCode=" + (int)numErrorCode.Value + ") " +
            //            "begin " +
            //            "    update TestErrorCode set ErrorName = '" + tbErrorCode.Text + "' where ErrorCode = " + (int)numErrorCode.Value + " " +
            //            "end " +
            //            "else " +
            //            " begin " +
            //            "    insert into TestErrorCode values(" + (int)numErrorCode.Value + ",'" + tbErrorCode.Text + "') " +
            //            "end";
            //GlobalVar.Sql.Execute(sql);
            FuncLog.WriteLog("Test Error Code Change : " + numErrorCode.Value + "," + tbErrorCode.Text + "," + cbRetest.Checked + "," + tbRetestType.Text);

            FuncInline.SaveTestErrorCode((int)numErrorCode.Value, tbErrorCode.Text, cbRetest.Checked, tbRetestType.Text);
            ResetTestErrorCode();
        }

        private void pbErrorCodeDelete_Click(object sender, EventArgs e)
        {
            if (GlobalVar.Sql == null || !GlobalVar.Sql.connected)
            {
                return;
            }
            //string sql = "    delete from TestErrorCode where ErrorCode = " + (int)numErrorCode.Value + " ";
            //GlobalVar.Sql.Execute(sql);
            FuncLog.WriteLog("Test Error Code Delete : " + numErrorCode.Value);
            FuncInline.DeleteErrorCode((int)numErrorCode.Value);
            ResetTestErrorCode();
        }

        /*
        private void cbPC1_CheckedChanged(object sender, EventArgs e)
        {
            if (!loaded)
            {
                return;
            }
            CheckBox cbPC = cbPC1;

            // Controls.Find가 무겁긴 하지만 상시가 아니라 체크 변경 이벤트에 쓰기 때문에 일단 놔둔다.
            allCheck = true;
            for (int i = 0; i < FuncInline.MaxSiteCount; i++)
            {
                int siteNo = i;
                if (cbPC.Checked &&
                    !FuncInline.Init_Site[siteNo])
                {
                    string siteName = "Site" + (siteNo + 1).ToString();
                    FuncWin.TopMessageBox(siteName + " not inited. Init site and try again.");
                    ((CheckBox)(Controls.Find("cbUseSite" + (siteNo + 1), true)[0])).Checked = false;
                    checkAll = false;
                    allCheck = false;
                    return;
                }
                else
                {
                    ((CheckBox)(Controls.Find("cbUseSite" + (siteNo + 1), true)[0])).Checked = cbPC.Checked;
                    valueChanged = true;
                }
            }
            checkAll = true;
            allCheck = false;
        }


        private void cbPC2_CheckedChanged(object sender, EventArgs e)
        {
            if (!loaded)
            {
                return;
            }
            CheckBox cbPC = cbPC2;
            // Controls.Find가 무겁긴 하지만 상시가 아니라 체크 변경 이벤트에 쓰기 때문에 일단 놔둔다.
            allCheck = true;
            for (int i = FuncInline.MaxSiteCount; i < FuncInline.MaxSiteCount * 2; i++)
            {
                int siteNo = i;
                if (cbPC.Checked &&
                    !FuncInline.Init_Site[siteNo])
                {
                    string siteName = "Site" + (siteNo + 1).ToString();
                    FuncWin.TopMessageBox(siteName + " not inited. Init site and try again.");
                    ((CheckBox)(Controls.Find("cbUseSite" + (siteNo + 1), true)[0])).Checked = false;
                    checkAll = false;
                    allCheck = false;
                    return;
                }
                else
                {
                    ((CheckBox)(Controls.Find("cbUseSite" + (siteNo + 1), true)[0])).Checked = cbPC.Checked;
                    valueChanged = true;
                }
            }
            checkAll = true;
            allCheck = false;
        }


        private void cbAllSite_CheckedChanged(object sender, EventArgs e)
        {
            cbPC1.Checked = cbAllSite.Checked;
            cbPC1_CheckedChanged(sender, e);
            if (!checkAll)
            {
                return;
            }
            cbPC2.Checked = cbAllSite.Checked;
            cbPC2_CheckedChanged(sender, e);
            if (!checkAll)
            {
                return;
            }
        }
        //*/

        private void cbUseSite_CheckedChanged(object sender, EventArgs e)
        {
            if (FuncInline.TabMain != FuncInline.enumTabMain.Machine ||
                FuncInline.TabMachine != FuncInline.enumTabMachine.TestSite)
            {
                return;
            }
            if (!loaded ||
                allCheck)
            {
                return;
            }
            CheckBox cbSite = (CheckBox)sender;
            int siteIndex = int.Parse(cbSite.Name.ToString().Replace("cbUseSite", "")) - 1;
            if (cbSite.Checked &&
                !FuncInline.InitialDone[(int)FuncInline.enumInitialize.Site1_F_DT1 + siteIndex])
            {
                string siteName = cbSite.Name.ToString().Replace("cbUse", "");
                FuncWin.TopMessageBox(siteName + " not inited. Init site and try again.");
                cbSite.Checked = false;
            }
            else
            {
               
                valueChanged = true;
            }
        }

        private void timerChange_Tick(object sender, EventArgs e)
        {
            //string IniPath = GlobalVar.FaPath;
            //if (!Directory.Exists(GlobalVar.IniPath))
            //{
            //    Directory.CreateDirectory(IniPath);
            //}
            //IniPath += "\\" + GlobalVar.SWName;
            //if (!Directory.Exists(GlobalVar.IniPath))
            //{
            //    Directory.CreateDirectory(IniPath);
            //}
            //IniPath += "\\" + GlobalVar.IniPath;
            //if (!Directory.Exists(GlobalVar.IniPath))
            //{
            //    Directory.CreateDirectory(IniPath);
            //}
            //IniPath += "\\" + FTErrorFileName;

            //lblSavePath.Text = "Save Path : " + IniPath;

            //timerChange.Enabled = false;

            if (FuncInline.TabMain != FuncInline.enumTabMain.Machine ||
                FuncInline.TabMachine != FuncInline.enumTabMachine.TestSite)
            {
                loaded = false;
            }

            #region 창 떠날 때 저장 확인
            if (valueChanged &&
                beforeMain == FuncInline.enumTabMain.Machine &&
                beforeMachine == FuncInline.enumTabMachine.TestSite &&
                (FuncInline.TabMain != FuncInline.enumTabMain.Machine ||
                        FuncInline.TabMachine != FuncInline.enumTabMachine.TestSite))
            {
                FuncInline.TabMain = FuncInline.enumTabMain.Machine;
                FuncInline.TabMachine = FuncInline.enumTabMachine.TestSite;

                valueChanged = false;
                if (FuncWin.MessageBoxOK("Test Site Setting changed. Save?"))
                {
                    ApplyAllValue();
                    Func.SaveTestSiteIni();
                }
            }
            beforeMain = FuncInline.TabMain;
            beforeMachine = FuncInline.TabMachine;
            #endregion

            //if (!GlobalVar.GlobalStop)
            //{
            //    Thread.Sleep(GlobalVar.ThreadSleep);
            //    timerChange.Enabled = true;
            //}
        }

        private void btnSaveFile_Click(object sender, EventArgs e)
        {
            FuncFile.WriteNewCSVFromDataGrid(dataGridErrorCode);
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            FuncFile.LoadCSVToDataGrid(dataGridErrorCode);
        }


        // 20230220 jhryu smd 개선요청 error_code ini 반영
        private void btnSaveINI_Click(object sender, EventArgs e)
        {
            if (FuncWin.MessageBoxOK("Save FTError.dat?"))
            {
                SaveTestErrorCodeIni(dataGridErrorCode);
            }
        }

        private void btnLoadINI_Click(object sender, EventArgs e)
        {
            if (FuncWin.MessageBoxOK("Load FTError.dat?"))
            {
                LoadTestErrorCodeIni(dataGridErrorCode);
            }
        }

        // jhryu : TestErrorCode Save to INI
        public static void SaveTestErrorCodeIni(DataGridView grid) // jhryu : Test Error Code Write to INI 
        {
            string IniPath = GlobalVar.FaPath;
            if (!Directory.Exists(GlobalVar.IniPath))
            {
                Directory.CreateDirectory(IniPath);
            }
            IniPath += "\\" + GlobalVar.SWName;
            if (!Directory.Exists(GlobalVar.IniPath))
            {
                Directory.CreateDirectory(IniPath);
            }
            IniPath += "\\" + GlobalVar.IniPath;
            if (!Directory.Exists(GlobalVar.IniPath))
            {
                Directory.CreateDirectory(IniPath);
            }
            IniPath += "\\" + FTErrorFileName;

            using (FileStream fs = File.Create(IniPath))    // 파일내용을 지우고 새로 만든다. (이유: 섹션이 섞이지 않게 깔끔하게 저장하기위해)
            {
                fs.Close();
            }
            #region test
            string[,] data = new string[grid.Rows.Count + 1, grid.Columns.Count]; // 헤더 정보를 더하기 때문에 행을 하나 더한다
            FuncForm.SetDataGridViewToArray(grid, ref data); // dataGridView 에서 데이터 가져오기

            int maxCount = data.GetLength(0);

            // FT_Error_Name 섹션
            for (int i = 1; i < maxCount; i++)
            {
                string key = sectionErrorName + '_' + int.Parse(data[i, 0]);
                string value = data[i, 1];
                FuncFile.WriteIniFile(sectionErrorName, key, value, IniPath);
            }

            // Retest 섹션 (Y/N)
            for (int i = 1; i < maxCount; i++)
            {
                string key = sectionErrorName + '_' + int.Parse(data[i, 0]);
                string value = data[i, 2];
                FuncFile.WriteIniFile(sectionRetest, key, value, IniPath);
            }
            // Retest Type 섹션 (F/A)
            for (int i = 1; i < maxCount; i++)
            {
                string key = sectionErrorName + '_' + int.Parse(data[i, 0]);
                string value = data[i, 3];
                FuncFile.WriteIniFile(sectionRetestType, key, value, IniPath);
            }

            #endregion test

            FuncWin.TopMessageBox("File saved to " + IniPath);
        }


        // jhryu : TestErrorCode INI Load to Grid
        public static void LoadTestErrorCodeIni(DataGridView grid)
        {
            string IniPath = GlobalVar.FaPath;
            if (!Directory.Exists(GlobalVar.IniPath))
            {
                Directory.CreateDirectory(IniPath);
            }
            IniPath += "\\" + GlobalVar.SWName;
            if (!Directory.Exists(GlobalVar.IniPath))
            {
                Directory.CreateDirectory(IniPath);
            }
            IniPath += "\\" + GlobalVar.IniPath;
            if (!Directory.Exists(GlobalVar.IniPath))
            {
                Directory.CreateDirectory(IniPath);
            }
            IniPath += "\\" + FTErrorFileName;

            if (!File.Exists(IniPath))
            {
                FuncWin.TopMessageBox(IniPath + " not found.");
                return;
            }

            int maxCount = 1000;    // 0~999
            string[,] data = new string[maxCount, 4];

            // ini에서 읽어서 data[]를 채운다
            int posCount = 0;
            for (int i = 0, pos = 0; i < maxCount; i++)
            {
                string key = sectionErrorName + '_' + i;
                string value = FuncFile.ReadIniFile(sectionErrorName, key, IniPath, "");
                if (value.Length == 0) continue;    // 에러네임이 없는것은 건너뛴다.

                data[pos, 0] = Util.IntToString(i, 2);
                data[pos, 1] = value.ToUpper();

                string retest = FuncFile.ReadIniFile(sectionRetest, key, IniPath, "True");
                string retestType = FuncFile.ReadIniFile(sectionRetestType, key, IniPath, "");
                data[pos, 2] = (retest.ToUpper() == "TRUE") ? "True" : "False";
                data[pos, 3] = retestType.ToUpper();
                pos++;
                posCount = pos;
            }

            // update grid
            grid.Rows.Clear();
            for (int j = 0; j < posCount; j++)
            {
                grid.Rows.Add();
                for (int i = 0; i < data.GetLength(1); i++)
                {
                    grid.Rows[j].Cells[i].Value = data[j, i];
                }
            }
        }

        // grid의 내용을 db에 전부 업데이트 한다.
        private void pbErrorCodeSaveAll_Click(object sender, EventArgs e)
        {
            /*
            if (tbErrorCode.Text.Trim().Length == 0)
            {
                FuncWin.TopMessageBox("input Error Name");
                return;
            }
            //*/
            if (GlobalVar.Sql == null ||
                !GlobalVar.Sql.connected)
            {
                return;
            }

            // get grid data
            var grid = dataGridErrorCode;
            string[,] data = new string[grid.Rows.Count + 1, grid.Columns.Count]; // 헤더 정보를 더하기 때문에 행을 하나 더한다
            FuncForm.SetDataGridViewToArray(grid, ref data); // dataGridView 에서 데이터 가져오기

            // save to db
            int maxCount = data.GetLength(0);
            for (int i = 1; i < maxCount; i++)  // 첫줄은 그리드의 헤더이므로 1부터 시작
            {
                int errNo = int.Parse(data[i, 0]);
                string errName = data[i, 1];
                bool retest = (data[i, 2] == "True") ? true : false;
                string retestType = data[i, 3];

                FuncInline.SaveTestErrorCode(errNo, errName, retest, retestType);
            }
            ResetTestErrorCode();
        }

        private void TestSite_Shown(object sender, EventArgs e)
        {
            string IniPath = GlobalVar.FaPath;
            if (!Directory.Exists(GlobalVar.IniPath))
            {
                Directory.CreateDirectory(IniPath);
            }
            IniPath += "\\" + GlobalVar.SWName;
            if (!Directory.Exists(GlobalVar.IniPath))
            {
                Directory.CreateDirectory(IniPath);
            }
            IniPath += "\\" + GlobalVar.IniPath;
            if (!Directory.Exists(GlobalVar.IniPath))
            {
                Directory.CreateDirectory(IniPath);
            }
            IniPath += "\\" + FTErrorFileName;

            lblSavePath.Text = "Save Path : " + IniPath;

            orderToSite();
            SetComboOrder();
        }

        #region 사이트 투입순서 관련
        // 글로벌에 순서별로 사이트 지정되어 있는 것을 사이트 순으로 역파싱한다.
        private void orderToSite()
        {
            try
            {
                for (int i = 0; i < FuncInline.SiteOrder.Length; i++)
                {
                    order[(int)FuncInline.SiteOrder[i] - (int)FuncInline.enumTeachingPos.Site1_F_DT1] = i;
                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        // 글로벌에 저장하기 위해 사이트순으로 된 것을 순서별로 사이트 열거하는 배열로 파싱한다.
        private void siteToOrder()
        {
            try
            {
                for (int i = 0; i < order.Length; i++)
                {
                    FuncInline.SiteOrder[(int)order[i]] = FuncInline.enumTeachingPos.Site1_F_DT1 + i;
                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        // 콤보박스들에 순서 출력
        private void SetComboOrder()
        {
            foreach (Control conCombo in pnOrder.Controls)
            {
                if (conCombo.GetType() == typeof(ComboBox))
                {
                    ComboBox combo = (ComboBox)conCombo;
                    if (combo.Name.Contains("cmbSite"))
                    {
                        int siteNo = -1;
                        int.TryParse(combo.Name.Replace("cmbSite", ""), out siteNo);
                        if (siteNo > 0)
                        {
                            combo.SelectedIndex = order[siteNo - 1];
                        }
                    }
                }
            }
        }

        private void cmbSite_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (orderChange)
            {
                orderChange = false;
                int siteNo = -1;
                int.TryParse(((ComboBox)sender).Name.Replace("cmbSite", ""), out siteNo);
                if (siteNo > 0)
                {
                    int selectedOrder = ((ComboBox)sender).SelectedIndex;
                    // 자신 이외
                    // 순서가 밀리면 이전순번에서 바꿀 순번 사이는 하나씩 감소
                    if (order[siteNo - 1] < selectedOrder)
                    {
                        for (int i = 0; i < order.Length; i++)
                        {
                            if (i != siteNo - 1 &&
                                order[i] >= order[siteNo - 1] &&
                                order[i] <= selectedOrder &&
                                order[i] > 0)
                            {
                                order[i]--;
                            }
                        }
                    }
                    // 순서가 당겨지면 이전순번에서 바꿀 순번 사이는 하나씩 증가
                    if (order[siteNo - 1] > selectedOrder)
                    {
                        for (int i = 0; i < order.Length; i++)
                        {
                            if (i != siteNo - 1 &&
                                order[i] >= selectedOrder &&
                                order[i] <= order[siteNo - 1])
                            {
                                order[i]++;
                            }
                        }
                    }
                    // 선택된 값을 지정한다.
                    order[siteNo - 1] = selectedOrder;
                    SetComboOrder();
                }
            }
        }

        private void cmbSite_Click(object sender, EventArgs e)
        {
            orderChange = true;
        }
        #endregion

        private void btnPC2_Click(object sender, EventArgs e)
        {
            if (!loaded)
            {
                return;
            }
            Button btnPC = btnPC2;
            if (allCheck)
            {
                btnPC.BackColor = btnAllSite.BackColor;
            }
            else
            {
                btnPC.BackColor = btnPC.BackColor == Color.Lime ? Color.White : Color.Lime;
            }

            // Controls.Find가 무겁긴 하지만 상시가 아니라 체크 변경 이벤트에 쓰기 때문에 일단 놔둔다.
            //allCheck = true;
            for (int i = 7; i <= 13; i++)
            {
                int siteNo = i;
                /*
                if (btnPC.BackColor == Color.Lime &&
                    !FuncInline.Init_Site[siteNo])
                {
                    string siteName = "Site" + (siteNo + 1).ToString();
                    FuncWin.TopMessageBox(siteName + " not inited. Init site and try again.");
                    ((Button)(Controls.Find("btnUseSite" + (siteNo + 1), true)[0])).BackColor = Color.White;
                    checkAll = false;
                    //allCheck = false;
                    return;
                }
                else
                {
                //*/
                ((Button)(Controls.Find("btnUseSite" + (siteNo + 1), true)[0])).BackColor = btnPC.BackColor;
                valueChanged = true;
                //}
            }
            checkAll = true;
            //allCheck = false;
        }

        private void btnPC1_Click(object sender, EventArgs e)
        {
            if (!loaded)
            {
                return;
            }
            Button btnPC = btnPC1;
            if (allCheck)
            {
                btnPC.BackColor = btnAllSite.BackColor;
            }
            else
            {
                btnPC.BackColor = btnPC.BackColor == Color.Lime ? Color.White : Color.Lime;
            }

            // Controls.Find가 무겁긴 하지만 상시가 아니라 체크 변경 이벤트에 쓰기 때문에 일단 놔둔다.
            //allCheck = true;
            for (int i = 0; i < 7; i++)
            {
                int siteNo = i;

                /*
                if (btnPC.BackColor == Color.Lime &&
                    !FuncInline.Init_Site[siteNo])
                {
                    string siteName = "Site" + (siteNo + 1).ToString();
                    FuncWin.TopMessageBox(siteName + " not inited. Init site and try again.");
                    ((Button)(Controls.Find("btnUseSite" + (siteNo + 1), true)[0])).BackColor = Color.White;
                    checkAll = false;
                    //allCheck = false;
                    return;
                }
                else
                {
                //*/
                ((Button)(Controls.Find("btnUseSite" + (siteNo + 1), true)[0])).BackColor = btnPC.BackColor;
                valueChanged = true;
                //}
            }
            checkAll = true;
            //allCheck = false;
        }

        private void btnAllSite_Click(object sender, EventArgs e)
        {
            btnAllSite.BackColor = btnAllSite.BackColor == Color.Lime ? Color.White : Color.Lime;
            allCheck = true;

            btnPC1.BackColor = btnAllSite.BackColor;
            btnPC1_Click(sender, e);

            btnPC2.BackColor = btnAllSite.BackColor;
            btnPC2_Click(sender, e);

            btnPC3.BackColor = btnAllSite.BackColor;
            btnPC3_Click(sender, e);

            allCheck = false;
        }

        private void btnUseSite_Click(object sender, EventArgs e)
        {
            Button btnSite = (Button)sender;
            btnSite.BackColor = btnSite.BackColor == Color.Lime ? Color.White : Color.Lime;
            valueChanged = true;
        }

        private void dataGridErrorCode_Click(object sender, EventArgs e)
        {
            //Disable Sorting for DataGridView
            foreach (DataGridViewColumn item in ((DataGridView)sender).Columns)
            {
                item.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void btnPC3_Click(object sender, EventArgs e)
        {
            if (!loaded)
            {
                return;
            }
            Button btnPC = btnPC3;
            if (allCheck)
            {
                btnPC.BackColor = btnAllSite.BackColor;
            }
            else
            {
                btnPC.BackColor = btnPC.BackColor == Color.Lime ? Color.White : Color.Lime;
            }

            // Controls.Find가 무겁긴 하지만 상시가 아니라 체크 변경 이벤트에 쓰기 때문에 일단 놔둔다.
            //allCheck = true;
            for (int i = 14; i <= 19; i++)
            {
                int siteNo = i;
                /*
                if (btnPC.BackColor == Color.Lime &&
                    !FuncInline.Init_Site[siteNo])
                {
                    string siteName = "Site" + (siteNo + 1).ToString();
                    FuncWin.TopMessageBox(siteName + " not inited. Init site and try again.");
                    ((Button)(Controls.Find("btnUseSite" + (siteNo + 1), true)[0])).BackColor = Color.White;
                    checkAll = false;
                    //allCheck = false;
                    return;
                }
                else
                {
                //*/
                ((Button)(Controls.Find("btnUseSite" + (siteNo + 1), true)[0])).BackColor = btnPC.BackColor;
                valueChanged = true;
                //}
            }
            checkAll = true;
            //allCheck = false;
        }
    }
}
