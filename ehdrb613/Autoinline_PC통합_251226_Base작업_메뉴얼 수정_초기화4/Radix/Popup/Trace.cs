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
using System.Data.SqlClient; // MsSQL

namespace Radix
{
    /*
     * Manual.cs : 각 파트 및 장비의 수동 운전
     */

    public partial class Trace : Form
    {

        private string startDate;
        private string endDate;

        public Trace()
        {
            InitializeComponent();
        }

        private void debug(string str)
        {
            //Util.Debug(str);
        }




        private void Manual_Shown(object sender, EventArgs e)
        {
            GlobalVar.dlgOpened = true;


            this.BringToFront();

            LoadAllValue();
        }

        private void Manual_FormClosed(object sender, FormClosedEventArgs e)
        {
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

        private void clear_panel()
        {
            pnComm.Visible = false;
            pnDefect.Visible = false;
            pnPin.Visible = false;
            pnTest.Visible = false;

            pbCommunication.BackgroundImage = Properties.Resources.communication;
            pbDefect.BackgroundImage = Properties.Resources.defect;
            pbPinBlock.BackgroundImage = Properties.Resources.PinBlock;
            pbTestResult.BackgroundImage = Properties.Resources.test_result;
        }

        private void pbDefect_Click(object sender, EventArgs e)
        {
            clear_panel();
            pnDefect.Visible = true;
            pbDefect.BackgroundImage = Properties.Resources.defect_dark;
            refreshDefect();
        }

        private void pbTestResult_Click(object sender, EventArgs e)
        {
            clear_panel();
            pnTest.Visible = true;
            pbTestResult.BackgroundImage = Properties.Resources.test_result_dark;
            refreshTest();
        }

        private void pbCommunication_Click(object sender, EventArgs e)
        {
            clear_panel();
            pnComm.Visible = true;
            pbCommunication.BackgroundImage = Properties.Resources.communication_dark;
            refreshComm();
        }

        private void pbPinBlock_Click(object sender, EventArgs e)
        {
            clear_panel();
            pnPin.Visible = true;
            pbPinBlock.BackgroundImage = Properties.Resources.PinBlock_dark;
            refreshPin();
        }

        private void pbRefresh_Click(object sender, EventArgs e)
        {
            if (pnPin.Visible)
            {
                refreshPin();
            }
            else if (pnComm.Visible)
            {
                refreshComm();
            }
            else if (pnTest.Visible)
            {
                refreshTest();
            }
            else if (pnDefect.Visible)
            {
                refreshDefect();
            }
        }

        private void calcDate()
        {
            string startDate = dateStart.Value.Year.ToString() + Util.IntToString(dateStart.Value.Month, 2) + Util.IntToString(dateStart.Value.Day, 2);
            string endDate = dateEnd.Value.Year.ToString() + Util.IntToString(dateEnd.Value.Month, 2) + Util.IntToString(dateEnd.Value.Day, 2);

            //bool finish = false;
            //int year = dateEnd.Value.Year;
            //int month = dateEnd.Value.Month;
            //int date = dateEnd.Value.Day;

            //string log_text = "";
            //while (!finish)
            //{
            //    string str_date = year.ToString() + Util.IntToString(month, 2) + Util.IntToString(date, 2);
            //    Console.WriteLine(str_date);

            //    log_text += ReadLog(str_date) + "\r\n\r\n";

            //    if (startDate == str_date)
            //    {
            //        break;
            //    }

            //    date--;
            //    if (date <= 0)
            //    {
            //        date = 31;
            //        month--;
            //    }
            //    if (month <= 0)
            //    {
            //        year--;
            //        month = 12;
            //    }
            //}

        }

        private void refreshPin()
        {
            calcDate();

            if (!GlobalVar.Sql.connected)
            {
                FuncWin.TopMessageBox("database not connected!");
            }

            //string sql = "select Site, ArrayNo, TestCount, NGCount " +
            //                "from PinLog " +
            //                "where Using='1' " +
            //                "order by Site,ArrayNo";
            //SqlDataReader rs = GlobalVar.Sql.Read(sql);
            string[,] rs = FuncInline.GetCurrentPinLog();
            if (rs != null)
            {
                int rowNum = 0;
                dataGridAllPin.Rows.Clear();
                for (int i = 0; i < rs.GetLength(0); i++)
                {
                    string site = "Test-" + rs[i,0].ToString();
                    string arrayNo = "Array-" + rs[i,1].ToString();
                    string inputCount = rs[i,2].ToString();
                    string ngCount = rs[i,3].ToString();
                    double ngRate = 0;
                    if (inputCount != "0")
                    {
                        ngRate = double.Parse(ngCount) / double.Parse(inputCount) * 100;
                    }

                    dataGridAllPin.Rows.Add(site, arrayNo, inputCount, ngCount, GlobalVar.AutoInline_DefectLimit.ToString("F1"), ngRate.ToString("F1"));
                    dataGridAllPin.Rows[rowNum].DefaultCellStyle.BackColor = rowNum % 2 > 0 ? Color.Cyan : Color.WhiteSmoke;
                    if (ngRate >= GlobalVar.AutoInline_DefectLimit)
                    {
                        dataGridAllPin.Rows[rowNum].Cells[5].Style.BackColor = Color.Tomato;
                    }

                    rowNum++;
                }
            }
        }

        private void refreshComm()
        {
            calcDate();

            if (!GlobalVar.Sql.connected)
            {
                FuncWin.TopMessageBox("database not connected!");
            }
            /*
            string[,] rs = FuncInline.GetTestComm(startDate, endDate, cmbCommSite.SelectedIndex, cmbCommArray.SelectedIndex);
            if (rs != null)
            {
                int rowNum = 0;
                dataGridSiteComm.Rows.Clear();
                while (rs.Read())
                {
                    string site = "Test-" + rs[2].ToString();
                    string arrayNo = "Array-" + rs[3].ToString();

                    dataGridAllPin.Rows.Add(rs[0].ToString(), rs[1].ToString(), site, arrayNo, rs[4].ToString(), rs[5].ToString());
                    dataGridAllPin.Rows[rowNum].DefaultCellStyle.BackColor = rowNum % 2 > 0 ? Color.Cyan : Color.WhiteSmoke;

                    rowNum++;
                }
                rs.Close();
            }
            rs = FuncInline.GetTestComm(startDate, endDate, 0, 0);
            if (rs != null)
            {
                int rowNum = 0;
                dataGridAllComm.Rows.Clear();
                while (rs.Read())
                {
                    string site = "Test-" + rs[2].ToString();
                    string arrayNo = "Array-" + rs[3].ToString();

                    dataGridAllComm.Rows.Add(rs[0].ToString(), rs[1].ToString(), site, arrayNo, rs[4].ToString(), rs[5].ToString());
                    dataGridAllComm.Rows[rowNum].DefaultCellStyle.BackColor = rowNum % 2 > 0 ? Color.Cyan : Color.WhiteSmoke;

                    rowNum++;
                }
                rs.Close();
            }
            //*/
        }

        private void refreshTest()
        {
            calcDate();

            if (!GlobalVar.Sql.connected)
            {
                FuncWin.TopMessageBox("database not connected!");
            }

            dataGridAllTest.Rows.Clear();
            for (int site = 1; site <= 12; site++)
            {
                string[,] rs = FuncInline.GetTestResult(startDate, endDate, site);
                if (rs != null)
                {
                    int totalTest = 0;
                    int totalPass = 0;
                    string[] arrayTest = new string[12];
                    for (int j = 0; j < rs.GetLength(0); j++)
                    {
                        try
                        {
                            int arrayNo = int.Parse(rs[j,1].ToString());
                            int total = int.Parse(rs[j,2].ToString());
                            int pass = int.Parse(rs[j,3].ToString());
                            totalTest += total;
                            totalPass += pass;
                            arrayTest[arrayNo - 1] = pass.ToString() + " / " + total.ToString();
                        }
                        catch (Exception ex)
                        {
                            debug(ex.ToString());
                            debug(ex.StackTrace);
                        }
                    }
                    dataGridAllPin.Rows.Add("Site-" + site.ToString(), totalTest.ToString(), "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0");
                    for (int i = 0; i < arrayTest.Length; i++)
                    {
                        dataGridAllPin.Rows[site - 1].Cells[i + 2].Value = arrayTest[i];
                    }
                    dataGridAllPin.Rows[site - 1].DefaultCellStyle.BackColor = (site - 1) % 2 > 0 ? Color.Cyan : Color.WhiteSmoke;
                }
            }
        }

        private void refreshDefect()
        {
            calcDate();

            if (!GlobalVar.Sql.connected)
            {
                FuncWin.TopMessageBox("database not connected!");
            }
            string[,] rs = null;

            // 사이트별 defect 현황
            dataGridDefectSite.Rows.Clear();
            for (int site = 1; site <= 21; site++)
            {
                //if (GlobalVar.AutoInline_UseSite[site - 1])
                //{
                rs = FuncInline.GetDefectResultBySite(startDate, endDate, site);
                bool exist = false;
                if (rs != null)
                {
                    if (rs.GetLength(0) > 0)
                    {
                        try
                        {
                            exist = true;
                            int total = int.Parse(rs[0,0].ToString());
                            int defect = int.Parse(rs[0,1].ToString());
                            double ratio = 0;
                            if (total > 0)
                            {
                                ratio = (double)defect / (double)total * 1000;
                            }
                            dataGridDefectSite.Rows.Add("Test-" + site.ToString(), total.ToString(), defect.ToString(), ratio.ToString("F2"));
                            dataGridDefectSite.Rows[site - 1].DefaultCellStyle.BackColor = (site - 1) % 2 > 0 ? Color.Cyan : Color.WhiteSmoke;
                        }
                        catch (Exception ex)
                        {
                            debug(ex.ToString());
                            debug(ex.StackTrace);
                        }
                    }
                }
                if (!exist)
                {
                    dataGridDefectSite.Rows.Add("Test-" + site.ToString(), "-", "-", "-");
                }
                //}
            }

            // defect code 별 현황
            dataGridDefectCode.Rows.Clear();
            rs = FuncInline.GetDefectResultByCode(startDate, endDate);
            int totalDefect = 0;
            if (rs != null)
            {
                int rowNum = 0;
                for (int i = 0; i < rs.GetLength(0); i++)
                {
                    try
                    {
                        string code = rs[i,0].ToString();
                        string name = rs[i,1].ToString();
                        int defect = int.Parse(rs[i,2].ToString());
                        totalDefect += defect;
                        dataGridDefectSite.Rows.Add(code, name, defect.ToString(), "0");
                        dataGridDefectSite.Rows[rowNum].DefaultCellStyle.BackColor = (rowNum) % 2 > 0 ? Color.Cyan : Color.WhiteSmoke;
                        rowNum++;
                    }
                    catch (Exception ex)
                    {
                        debug(ex.ToString());
                        debug(ex.StackTrace);
                    }
                }

                // 비율 구하기
                if (totalDefect > 0)
                {
                    for (int i = 0; i < dataGridDefectCode.Rows.Count; i++)
                    {
                        dataGridDefectCode.Rows[i].Cells[3].Value = (double.Parse(dataGridDefectCode.Rows[i].Cells[2].Value.ToString()) / totalDefect * 1000).ToString("F2");
                    }
                }
            }

            // 전체 사이트/어레이/코드별 defect 발생현황
            int[,,] defectList = new int[21, 12, 100];
            rs = FuncInline.GetDefectResultBySiteArrayCode(startDate, endDate);
            if (rs != null)
            {
                dataGridDefectAll.Rows.Clear();
                for (int i = 0; i < rs.GetLength(0); i++)
                {
                    try
                    {
                        int site = int.Parse(rs[i,0].ToString());
                        int array = int.Parse(rs[i,1].ToString());
                        int code = int.Parse(rs[i,2].ToString());
                        int count = int.Parse(rs[i,3].ToString());
                        defectList[site - 1, array - 1, code] = count;
                    }
                    catch (Exception ex)
                    {
                        debug(ex.ToString());
                        debug(ex.StackTrace);
                    }
                }

                // 어레이별 합계계산
                int[,] arrayDefect = new int[21, 12];
                for (int k = 0; k < defectList.GetLength(0); k++)
                {
                    for (int j = 0; j < defectList.GetLength(1); j++)
                    {
                        for (int i = 0; i < defectList.GetLength(2); i++)
                        {
                            arrayDefect[k, j] += defectList[k, j, i];
                        }
                    }
                }
                // 출력

                for (int k = 0; k < defectList.GetLength(0); k++)
                {
                    for (int j = 0; j < defectList.GetLength(1); j++)
                    {
                        try
                        {
                            string site = "Test" + (k + 1).ToString() + "-";
                            string array = (j == 0 ? site : "") + (j + 1).ToString();
                            dataGridDefectAll.Rows.Add(array, arrayDefect[k, j].ToString());
                            dataGridDefectAll.Rows[k * defectList.GetLength(1) + j].DefaultCellStyle.BackColor = (j) % 2 > 0 ? Color.Cyan : Color.WhiteSmoke;
                            for (int i = 0; i < defectList.GetLength(2); i++)
                            {
                                dataGridDefectAll.Rows[k * defectList.GetLength(1) + j].Cells[i + 2].Value = defectList[k, j, i].ToString();
                            }
                        }
                        catch (Exception ex)
                        {
                            debug(ex.ToString());
                            debug(ex.StackTrace);
                        }
                    }
                }
            }
            dataGridDefectAll.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridDefectAll.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void pbExcelAllPin_Click(object sender, EventArgs e)
        {
            FuncFile.WriteNewExcelFromDataGrid(dataGridAllPin);

            /*
            try
            {
                string[,] data = new string[dataGridAllPin.Rows.Count + 1, dataGridAllPin.Columns.Count]; // 헤더 정보를 더하기 때문에 행을 하나 더한다
                Func.SetDataGridViewToArray(dataGridAllPin, ref data); // dataGridView 에서 데이터 가져오기

                FuncExcel.WriteNewExcelData(data);
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            //*/
        }

        private void pbExcelAllDefect_Click(object sender, EventArgs e)
        {
            FuncFile.WriteNewExcelFromDataGrid(dataGridDefectAll);

            /*
            try
            {
                string[,] data = new string[dataGridDefectAll.Rows.Count + 1, dataGridDefectAll.Columns.Count]; // 헤더 정보를 더하기 때문에 행을 하나 더한다
                Func.SetDataGridViewToArray(dataGridDefectAll, ref data); // dataGridView 에서 데이터 가져오기

                FuncExcel.WriteNewExcelData(data);
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            //*/
        }

        private void pbExcellAllComm_Click(object sender, EventArgs e)
        {
            FuncFile.WriteNewExcelFromDataGrid(dataGridAllComm);

            /*
            try
            {
                string[,] data = new string[dataGridAllComm.Rows.Count + 1, dataGridAllComm.Columns.Count]; // 헤더 정보를 더하기 때문에 행을 하나 더한다
                Func.SetDataGridViewToArray(dataGridAllComm, ref data); // dataGridView 에서 데이터 가져오기

                FuncExcel.WriteNewExcelData(data);
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            //*/
        }

        private void pbExcelAllTest_Click(object sender, EventArgs e)
        {
            FuncFile.WriteNewExcelFromDataGrid(dataGridAllTest);

            /*
            try
            {
                string[,] data = new string[dataGridAllTest.Rows.Count + 1, dataGridAllTest.Columns.Count]; // 헤더 정보를 더하기 때문에 행을 하나 더한다
                Func.SetDataGridViewToArray(dataGridAllTest, ref data); // dataGridView 에서 데이터 가져오기

                FuncExcel.WriteNewExcelData(data);
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            //*/
        }

        private void LoadAllValue()
        {
            try
            {
                numPinLifeDate.Value = (decimal)GlobalVar.PinLifeDate;
                //numPinLifeCount.Value = (decimal)GlobalVar.PinLifeCount;
                //cbCheckPinLife.Checked = GlobalVar.CheckPinLife;
                numDefectLimit.Value = (decimal)GlobalVar.AutoInline_DefectLimit;
                numPinLogTime.Value = (decimal)GlobalVar.AutoInline_PinLogTime;
                tbPinLogDirectory.Text = GlobalVar.AutoInline_PinLogDirectory;

                for (int i = 1; i <= GlobalVar.AutoInline_MaxArrayCount; i++)
                {
                    ComboBox cmb = (ComboBox)Controls.Find("cmbPinArray" + i, true)[0];
                    Util.SetComboIndex(ref cmb, "Array" + GlobalVar.AutoInline_PinArray[i - 1]);
                }
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
                GlobalVar.PinLifeDate = (int)numPinLifeDate.Value;
                //GlobalVar.PinLifeCount = (int)numPinLifeCount.Value;
                //GlobalVar.CheckPinLife = cbCheckPinLife.Checked;
                GlobalVar.AutoInline_DefectLimit = (double)numDefectLimit.Value;
                GlobalVar.AutoInline_PinLogTime = (int)numPinLogTime.Value;
                GlobalVar.AutoInline_PinLogDirectory = tbPinLogDirectory.Text;

                for (int i = 1; i <= GlobalVar.AutoInline_MaxArrayCount; i++)
                {
                    ComboBox cmb = (ComboBox)Controls.Find("cmbPinArray" + i, true)[0];
                    GlobalVar.AutoInline_PinArray[i - 1] = cmb.SelectedIndex + 1;
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        private void pbCancel_Click(object sender, EventArgs e)
        {
            //*
            this.BringToFront();
            if (FuncWin.MessageBoxOK(" Cancel All Value?"))
            {
                LoadAllValue();
            }
            //*/}
        }

        private void pbApply_Click(object sender, EventArgs e)
        {
            this.BringToFront();
            if (FuncWin.MessageBoxOK(" Apply All Values?"))
            {
                ApplyAllValue();
            }
        }

        private void pbSave_Click(object sender, EventArgs e)
        {
            //*
            if (FuncWin.MessageBoxOK(" Save All Values?"))
            {
                //GlobalVar.ModelName = tbModel.Text;
                ApplyAllValue();

                FuncIni.SavePinIni();

                //if (!cmbModelList.Items.Contains(tbModel.Text))
                //{
                //    cmbModelList.Items.Add(tbModel.Text);
                //    cmbModelList.Text = tbModel.Text;
                //}
            }
            //*/
        }

        private void pbLoad_Click(object sender, EventArgs e)
        {
            this.BringToFront();
            if (FuncWin.MessageBoxOK(" Load All Values?"))
            {
                FuncIni.LoadPinIni();

                LoadAllValue();
            }
        }


      

        private void tbPinLogDirectory_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            //if (tbPinLogDirectory.Text.Length > 0)
            //{
            //}
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                tbPinLogDirectory.Text = dlg.SelectedPath;
            }
        }

        private void btnInputClear_Click(object sender, EventArgs e)
        {
            if (FuncWin.MessageBoxOK("Clear Input Count at selected Arrays ?"))
            {

                for (int i = 0; i < dataGridAllPin.SelectedRows.Count; i++)
                {
                    //string sql = "update PinLog " +
                    //            "set TestCount = 0 " +
                    //            "where site = " + dataGridAllPin.SelectedRows[i].Cells[0].Value.ToString().Replace("Test-","") + " " +
                    //            "and ArrayNo = " + dataGridAllPin.SelectedRows[i].Cells[1].Value.ToString().Replace("Array-", "") + " " +
                    //            "and Using = 1";
                    //GlobalVar.Sql.Execute(sql);
                    FuncInline.ClearPinTestCount(int.Parse(dataGridAllPin.SelectedRows[i].Cells[0].Value.ToString().Replace("Test-", "")),
                                                 int.Parse(dataGridAllPin.SelectedRows[i].Cells[1].Value.ToString().Replace("Array-", "")),
                                                 false);
                }

                refreshPin();
                FuncWin.TopMessageBox("Input Count cleared");
            }
        }

        private void btnNGClear_Click(object sender, EventArgs e)
        {
            if (FuncWin.MessageBoxOK("Clear NG Count at selected Arrays ?"))
            {
                for (int i = 0; i < dataGridAllPin.SelectedRows.Count; i++)
                {
                    //string sql = "update PinLog " +
                    //            "set NGCount = 0 " +
                    //            "where site = " + dataGridAllPin.SelectedRows[i].Cells[0].Value.ToString().Replace("Test-", "") + " " +
                    //            "and ArrayNo = " + dataGridAllPin.SelectedRows[i].Cells[1].Value.ToString().Replace("Array-", "") + " " +
                    //            "and Using = 1";
                    //GlobalVar.Sql.Execute(sql);
                    FuncInline.ClearPinTestCount(int.Parse(dataGridAllPin.SelectedRows[i].Cells[0].Value.ToString().Replace("Test-", "")),
                                                 int.Parse(dataGridAllPin.SelectedRows[i].Cells[1].Value.ToString().Replace("Array-", "")),
                                                 false);
                }

                refreshPin();
                FuncWin.TopMessageBox("NG Count cleared");
            }
        }

        private void btnAllClear_Click(object sender, EventArgs e)
        {
            bool inputClear = FuncWin.MessageBoxOK("Clear Input Count?");
            bool ngClear = FuncWin.MessageBoxOK("Clear NG Count?");
            if (inputClear || ngClear)
            {
                string clearStr = "All Input and NG Count";
                if (inputClear && !ngClear)
                {
                    clearStr = "Input Count Only";
                }
                else if (!inputClear && ngClear)
                {
                    clearStr = "NG Count Only";
                }

                if (FuncWin.MessageBoxOK("Clear " + clearStr + " at All Site?"))
                {
                    //string sql = "update PinLog " +
                    //            "set TestCount = 0, NGCount = 0 " +
                    //            "where Using = 1";
                    //GlobalVar.Sql.Execute(sql);
                    FuncInline.ClearAllPinCount();

                    refreshPin();
                    FuncWin.TopMessageBox(clearStr + " cleared");
                }
            }
        }
    }
}
