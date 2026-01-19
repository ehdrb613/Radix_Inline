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
        enum enumTabTrace
        {
            Pin,
            Communication,
            Defect
        }

        private string startDate;
        private string endDate;

        enumTabTrace traceTab = enumTabTrace.Defect;

        public Trace()
        {
            InitializeComponent();
        }

        private void debug(string str)
        {
            Util.Debug(str);
        }

        public void SetDate()
        {
            int nowHour = DateTime.Now.Hour;
            bool lastShift = false;
            if (!FuncInline.UseShiftC)
            {
                if (nowHour >= FuncInline.ShiftBHour ||
                    nowHour < FuncInline.ShiftAHour)
                {
                    lastShift = true;
                }
            }
            if (FuncInline.UseShiftC)
            {
                if (nowHour >= FuncInline.ShiftCHour ||
                    nowHour < FuncInline.ShiftAHour)
                {
                    lastShift = true;
                }
            }
            // 현재 조가 마지막 조면 이전날짜
            DateTime searchDate = DateTime.Now;
            if (lastShift)
            {
                if (nowHour < FuncInline.ShiftAHour)
                {
                    searchDate = searchDate.AddDays(-1);
                }
            }
            dateStart.Value = searchDate;
            dateEnd.Value = searchDate;

            if (nowHour >= FuncInline.ShiftAHour &&
                nowHour < FuncInline.ShiftBHour)
            {
                btnShiftA.BackColor = Color.Lime;
            }
            else
            {
                btnShiftA.BackColor = Color.White;
            }
            if (!FuncInline.UseShiftC &&
                (nowHour >= FuncInline.ShiftBHour ||
                    nowHour < FuncInline.ShiftAHour))
            {
                btnShiftB.BackColor = Color.Lime;
            }
            else if (FuncInline.UseShiftC &&
                nowHour >= FuncInline.ShiftBHour &&
                nowHour < FuncInline.ShiftCHour)
            {
                btnShiftB.BackColor = Color.Lime;
            }
            else
            {
                btnShiftB.BackColor = Color.White;
            }
            if (FuncInline.UseShiftC &&
                (nowHour >= FuncInline.ShiftCHour ||
                    nowHour < FuncInline.ShiftAHour))
            {
                btnShiftC.BackColor = Color.Lime;
            }
            else
            {
                btnShiftC.BackColor = Color.White;
            }
        }


        private void Manual_Shown(object sender, EventArgs e)
        {
            GlobalVar.dlgOpened = true;


            this.BringToFront();

            SetDate();

            LoadAllValue();

            //dataGridAllPin.DefaultCellStyle = GlobalVar.dataGridStyle;
            //dataGridAllPin.ColumnHeadersDefaultCellStyle = GlobalVar.dataGridStyle;
            dataGridAllTest.DefaultCellStyle = GlobalVar.dataGridStyle;
            dataGridAllTest.ColumnHeadersDefaultCellStyle = GlobalVar.dataGridStyle;

            System.Windows.Forms.DataGridViewCellStyle dataGridStyle = new System.Windows.Forms.DataGridViewCellStyle();
            dataGridStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridStyle.BackColor = System.Drawing.Color.White;
            dataGridStyle.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridStyle.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;

            dataGridDefectAll.DefaultCellStyle = dataGridStyle;
            dataGridDefectAll.ColumnHeadersDefaultCellStyle = dataGridStyle;
            dataGridDefectCode.DefaultCellStyle = dataGridStyle;
            dataGridDefectCode.ColumnHeadersDefaultCellStyle = dataGridStyle;
            dataGridDefectSite.DefaultCellStyle = dataGridStyle;
            dataGridDefectSite.ColumnHeadersDefaultCellStyle = dataGridStyle;
            //dataGridSiteComm.DefaultCellStyle = GlobalVar.dataGridStyle;
            //dataGridSiteComm.ColumnHeadersDefaultCellStyle = GlobalVar.dataGridStyle;

            dataGridCount.Rows.Clear();
            dataGridCount.Rows.Add("", "PBA", "PCB");
            dataGridCount.Rows.Add("Overall Input", "", "");
            dataGridCount.Rows.Add("Overall Pass", "", "");
            dataGridCount.Rows.Add("Overall Defect", "", "");
            dataGridCount.Rows.Add("Defect Rate", "", "");
            dataGridCount.Rows.Add("", "", "");
            dataGridCount.Rows[5].Selected = true;

            dataGridCount.DefaultCellStyle = dataGridStyle;

        }

        private void Manual_FormClosed(object sender, FormClosedEventArgs e)
        {
            //if (this.Parent != null)
            //{
            try
            {
                GlobalVar.dlgOpened = false;
                //this.Parent.BringToFront();
            }
            catch
            { }
            //}
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
        }

        private void pbDefect_Click(object sender, EventArgs e)
        {
            traceTab = enumTabTrace.Defect;
            clear_panel();
            pnDefect.Visible = true;
            pbDefect.BackgroundImage = Properties.Resources.defect_dark;
            refreshDefect();
        }

        private void pbCommunication_Click(object sender, EventArgs e)
        {
            SetDate();
            traceTab = enumTabTrace.Communication;
            clear_panel();
            pnComm.Visible = true;
            pbCommunication.BackgroundImage = Properties.Resources.communication_dark;
            refreshComm();
        }

        private void pbPinBlock_Click(object sender, EventArgs e)
        {
            traceTab = enumTabTrace.Pin;

            clear_panel();
            pnPin.Visible = true;
            pbPinBlock.BackgroundImage = Properties.Resources.PinBlock_dark;
            refreshPin();
        }

        private void pbRefresh_Click(object sender, EventArgs e)
        {
            if (btnShiftA.BackColor != Color.Lime &&
                btnShiftB.BackColor != Color.Lime &&
                btnShiftC.BackColor != Color.Lime)
            {
                FuncWin.TopMessageBox("Select one or more shift.");
                return;
            }

            if (traceTab == enumTabTrace.Communication)
            {
                if (dateStart.Value != dateEnd.Value)
                {
                    FuncWin.TopMessageBox("Only one day needed in Communication Trace.");
                    return;
                }
                int selectedCount = 0;
                if (btnShiftA.BackColor == Color.Lime)
                {
                    selectedCount++;
                }
                if (btnShiftB.BackColor == Color.Lime)
                {
                    selectedCount++;
                }
                if (btnShiftC.BackColor == Color.Lime)
                {
                    selectedCount++;
                }
                if (selectedCount > 1)
                {
                    FuncWin.TopMessageBox("Only one shift needed in Communication Trace.");
                    return;
                }
            }
            RefreshTrace();
        }

        public void RefreshTrace()
        {
            FuncInline.WaitMessage = "Searching Trace Log";
            //debug("RefreshTrace");

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

            FuncInline.WaitMessage = "";
        }

        private void calcDate()
        {
            startDate = dateStart.Value.Year.ToString() + Util.IntToString(dateStart.Value.Month, 2) + Util.IntToString(dateStart.Value.Day, 2);
            endDate = dateEnd.Value.Year.ToString() + Util.IntToString(dateEnd.Value.Month, 2) + Util.IntToString(dateEnd.Value.Day, 2);

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

            int totalInput = 0;
            int totalDefect = 0;
            int validCount = 0;

            //string sql = "select Site, ArrayNo, TestCount, NGCount " +
            //                "from PinLog " +
            //                "where Using='1' " +
            //                "order by Site,ArrayNo";
            //SqlDataReader rs = GlobalVar.Sql.Read(sql);
            string[,] rs = FuncInline.GetCurrentPinLog();
            if (rs != null)
            {
                //int rowNum = 0;
                dataGridAllPin.Rows.Clear();
                #region 사이트별 Row 만들기
                for (int i = 0; i < FuncInline.MaxSiteCount; i++)
                {
                    dataGridAllPin.Rows.Add((i + 1).ToString());
                    dataGridAllPin.Rows[i].DefaultCellStyle.BackColor = i % 2 > 0 ? Color.Cyan : Color.WhiteSmoke;
                }
                #endregion

                //while (rs.Read())
                for (int j = 0; j < rs.GetLength(0); j++)
                {
                    try
                    {
                        int site = int.Parse(rs[j, 0].ToString());
                        int pinNo = int.Parse(rs[j, 1].ToString());
                        int arrayNo = int.Parse(rs[j, 2].ToString());
                        int inputCount = int.Parse(rs[j, 3].ToString());
                        int ngCount = int.Parse(rs[j, 4].ToString());
                        double ngRate = 0;
                        if (inputCount != 0)
                        {
                            ngRate = (double)ngCount / (double)inputCount * 100;
                        }

                        // 핀번호로는 어레이인덱스를 계산할 수 없으므로
                        // 어레이 번호로 ColumnIndex 계산
                        int columnIndex = -1;
                        for (int i = 0; i < FuncInline.PinArray.Length; i++)
                        {
                            if (FuncInline.PinArray[i] == arrayNo)
                            {
                                columnIndex = i * 3 + 1;
                                break;
                            }
                        }
                        if (columnIndex == -1) // 어레이 순번 못 찾았으면 그냥 넘긴다.
                        {
                            continue;
                        }

                        dataGridAllPin.Rows[site - 1].Cells[columnIndex].Value = inputCount;
                        dataGridAllPin.Rows[site - 1].Cells[columnIndex + 1].Value = ngCount;
                        dataGridAllPin.Rows[site - 1].Cells[columnIndex + 2].Value = ngRate.ToString("F1");

                        //dataGridAllPin.Rows.Add(site, arrayNo, inputCount, ngCount, FuncInline.DefectLimit.ToString("F1"), ngRate.ToString("F1"));
                        //dataGridAllPin.Rows[rowNum].DefaultCellStyle.BackColor = rowNum % 2 > 0 ? Color.Cyan : Color.WhiteSmoke;
                        if (ngRate >= FuncInline.DefectLimit)
                        {
                            dataGridAllPin.Rows[site - 1].Cells[columnIndex].Style.BackColor = Color.Tomato;
                            dataGridAllPin.Rows[site - 1].Cells[columnIndex + 1].Style.BackColor = Color.Tomato;
                            dataGridAllPin.Rows[site - 1].Cells[columnIndex + 2].Style.BackColor = Color.Tomato;
                        }

                        totalInput += inputCount;
                        totalDefect += ngCount;
                        if (inputCount > 0)
                        {
                            validCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        //debug(ex.ToString());
                        //debug(ex.StackTrace);
                    }
                }
                //rs.Close();
            }

            lblPinOverallInput.Text = totalInput.ToString();
            if (validCount > 0)
            {
                lblPinAverageInputCount.Text = ((double)totalInput / validCount).ToString("F1");
                lblPinAverageNGCount.Text = ((double)totalDefect / validCount).ToString("F1");
            }
            else
            {
                lblPinAverageInputCount.Text = "0";
                lblPinAverageNGCount.Text = "0";
            }
            if (totalInput > 0)
            {
                lblAverageDefectRate.Text = ((double)totalDefect / (double)totalInput * 100).ToString("F1") + " %";
            }
            else
            {
                lblAverageDefectRate.Text = "0 %";
            }
        }

        private void SetGridColor(DataGridView grid)
        {
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                grid.Rows[i].DefaultCellStyle.BackColor = i % 2 > 0 ? Color.Cyan : Color.WhiteSmoke;
            }
        }

        private void refreshComm()
        {
            try
            {
                calcDate();

                if (!GlobalVar.Sql.connected)
                {
                    FuncWin.TopMessageBox("database not connected!");
                }

                FuncLog.WriteLog("Trace - Communication " + startDate + "-" + endDate + "," + (btnShiftA.BackColor == Color.Lime) + "," + (btnShiftB.BackColor == Color.Lime) + "," + (btnShiftC.BackColor == Color.Lime));

                #region 각 컬럼의 폭값이 기억해 둔다.
                int[] columnWidth = new int[dataGridSiteComm.ColumnCount];
                for (int i = 0; i < dataGridSiteComm.ColumnCount; i++)
                {
                    columnWidth[i] = dataGridSiteComm.Columns[i].Width;
                }
                #endregion

                #region 검색 일시 범위 지정
                string startTime = startDate;
                if (btnShiftA.BackColor == Color.Lime)
                {
                    startTime += Util.IntToString(FuncInline.ShiftAHour, 2) + ":" + Util.IntToString(FuncInline.ShiftAMin, 2) + ":00";
                }
                else if (btnShiftB.BackColor == Color.Lime)
                {
                    startTime += Util.IntToString(FuncInline.ShiftBHour, 2) + ":" + Util.IntToString(FuncInline.ShiftBMin, 2) + ":00";
                }
                else if (btnShiftC.BackColor == Color.Lime)
                {
                    startTime += Util.IntToString(FuncInline.ShiftCHour, 2) + ":" + Util.IntToString(FuncInline.ShiftCMin, 2) + ":00";
                }
                string endTime = endDate;
                #region 마지막조 포함시
                if ((!FuncInline.UseShiftC && btnShiftB.BackColor == Color.Lime) ||
                    (FuncInline.UseShiftC && btnShiftC.BackColor == Color.Lime))
                {
                    // 종료 날짜의 다음날 A조 시작 전 시간을 구한다.
                    int searchYear = int.Parse(endDate.Substring(0, 4));
                    int searchMonth = int.Parse(endDate.Substring(4, 2));
                    int searchDate = int.Parse(endDate.Substring(6, 2));
                    int searchHour = FuncInline.ShiftAHour;
                    int searchMin = FuncInline.ShiftAMin;
                    DateTime searchDateTime = new DateTime(searchYear, searchMonth, searchDate, searchHour, searchMin, 0);
                    searchDateTime = searchDateTime.AddDays(1);
                    searchDateTime = searchDateTime.AddSeconds(-1);
                    endTime = searchDateTime.ToString("yyyyMMddHH:mm:ss");
                }
                #endregion
                #region 마지막조 미포함시
                else
                {
                    // 종료 날짜의 선택된 마지막 조 종료 시간을 구한다.
                    int searchYear = int.Parse(endDate.Substring(0, 4));
                    int searchMonth = int.Parse(endDate.Substring(4, 2));
                    int searchDate = int.Parse(endDate.Substring(6, 2));
                    int searchHour = FuncInline.ShiftAHour;
                    int searchMin = FuncInline.ShiftAMin;
                    // A조 선택시 B조시간
                    if (btnShiftA.BackColor == Color.Lime)
                    {
                        searchHour = FuncInline.ShiftBHour;
                        searchMin = FuncInline.ShiftBMin;
                    }
                    // B조 선택시 C조시간
                    if (btnShiftB.BackColor == Color.Lime)
                    {
                        searchHour = FuncInline.ShiftCHour;
                        searchMin = FuncInline.ShiftCMin;
                    }
                    DateTime searchDateTime = new DateTime(searchYear, searchMonth, searchDate, searchHour, searchMin, 0);
                    searchDateTime = searchDateTime.AddSeconds(-1); // 1분을 빼서 조 마지막 시간으로 세팅
                    endTime = searchDateTime.ToString("yyyyMMddHH:mm:ss");
                }
                #endregion
                #endregion

                string sql = "select [Date],[Time],[Site],[Array],[Type],[Content],[Result] " +
                            "from CommLog " +
                            "where concat([Date],[Time]) >= '" + startTime + "' " +
                            "and concat([Date], [Time]) <= '" + endTime + "' " +
                            "order by Date desc , " +
                            "Time desc ";
                //debug(sql);
                GlobalVar.Sql.Read(sql, dataGridSiteComm);

                #region 그리드의 폭 정보가 날아가기 때문에 새로 지정해야 한다.
                for (int i = 0; i < dataGridSiteComm.ColumnCount; i++)
                {
                    if (i < dataGridSiteComm.Columns.Count &&
                        i < columnWidth.Length)
                    {
                        dataGridSiteComm.Columns[i].Width = columnWidth[i];
                    }
                }
                #endregion

                SetGridColor(dataGridSiteComm);

                /*
                string[,] rs = FuncInline.GetTestComm(startDate,
                                                        endDate,
                                                        btnShiftA.BackColor == Color.Lime,
                                                        btnShiftB.BackColor == Color.Lime,
                                                        btnShiftC.BackColor == Color.Lime,
                                                        "Date",
                                                        "Time",
                                                        true);
                if (rs != null)
                {
                    int rowNum = 0;
                    dataGridSiteComm.Rows.Clear();
                    //while (rs.Read())
                    for (int j = 0; j < rs.GetLength(0); j++)
                    {
                        dataGridSiteComm.Rows.Add(rs[j, 0].ToString(), rs[j, 1].ToString(), rs[j, 2].ToString(), rs[j, 3].ToString(), rs[j, 4].ToString(), rs[j, 5].ToString());
                        dataGridSiteComm.Rows[rowNum].DefaultCellStyle.BackColor = rowNum % 2 > 0 ? Color.Cyan : Color.WhiteSmoke;

                        rowNum++;
                    }
                    if (dataGridSiteComm.Rows.Count == 0)
                    {
                        dataGridSiteComm.Rows.Add("no", "data");
                    }
                    //rs.Close();
                }
                //*/

                #region 각 컬럼의 폭값이 기억해 둔다.
                columnWidth = new int[dataGridCommStatistics.ColumnCount];
                for (int i = 0; i < dataGridCommStatistics.ColumnCount; i++)
                {
                    columnWidth[i] = dataGridCommStatistics.Columns[i].Width;
                }
                #endregion

                sql = "select [Site],[Array],[Type],count(*) as count " +
                "from CommLog " +
                            "where concat([Date], [Time]) >= '" + startTime + "' " +
                            "and concat([Date], [Time]) <= '" + endTime + "' " +
                            "group by [Site],[Array],[Type] " +
                            "order by Site, " +
                            "Array";
                //debug(sql);
                GlobalVar.Sql.Read(sql, dataGridCommStatistics);

                #region 그리드의 폭 정보가 날아가기 때문에 새로 지정해야 한다.
                for (int i = 0; i < dataGridCommStatistics.ColumnCount; i++)
                {
                    dataGridCommStatistics.Columns[i].Width = columnWidth[i];
                }
                #endregion

                SetGridColor(dataGridCommStatistics);
                /*/ 
                string[,] rs1 = FuncInline.GetTestCommStatistics(startDate,
                                                                    endDate,
                                                                    btnShiftA.BackColor == Color.Lime,
                                                                    btnShiftB.BackColor == Color.Lime,
                                                                    btnShiftC.BackColor == Color.Lime,
                                                                    "Site",
                                                                    "Array",
                                                                    false);
                if (rs1 != null)
                {
                    int rowNum = 0;
                    dataGridCommStatistics.Rows.Clear();
                    //while (rs.Read())
                    for (int j = 0; j < rs1.GetLength(0); j++)
                    {
                        dataGridCommStatistics.Rows.Add(rs1[j, 0].ToString(), rs1[j, 1].ToString(), rs1[j, 2].ToString(), rs1[j, 3].ToString());
                        dataGridCommStatistics.Rows[rowNum].DefaultCellStyle.BackColor = rowNum % 2 > 0 ? Color.Cyan : Color.WhiteSmoke;

                        rowNum++;
                    }
                    if (dataGridCommStatistics.Rows.Count == 0)
                    {
                        dataGridCommStatistics.Rows.Add("no", "data");
                    }
                    //rs.Close();
                }
                //*/

                /*
                try
                {
                    string sql = "select [Date],[Time],[Site],[Array],[Type],[Content],[Result] " +
                                "from CommLog " +
                                "where [Date] >= '" + startDate + "' " +
                                "and [Date] <= '" + endDate + "' " +
                                "order by [Date] desc, [Time] desc";
                    //debug(sql);
                    string conStr = "server = " + GlobalVar.MsSQL_Server +
                                    "," + GlobalVar.MsSQL_Port +
                                    "; uid = " + GlobalVar.MsSQL_Id +
                                    "; pwd = " + GlobalVar.MsSQL_Pwd +
                                    "; database = " + GlobalVar.MsSQL_DB + ";";
                    SqlConnection con = new SqlConnection(conStr);
                    con.Open();

                    if (con == null ||
                        con.State == ConnectionState.Closed)
                    {
                        return;
                    }
                    SqlCommand com = new SqlCommand(sql, con);
                    com.CommandTimeout = 1;
                    com.Connection = con;
                    SqlDataReader reader = com.ExecuteReader();

                    if (reader != null)
                    {
                        int rowNum = 0;
                        dataGridSiteComm.Rows.Clear();
                        while (reader.Read())
                        {
                            dataGridSiteComm.Rows.Add(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString());
                            dataGridSiteComm.Rows[rowNum].DefaultCellStyle.BackColor = rowNum % 2 > 0 ? Color.Cyan : Color.WhiteSmoke;

                            rowNum++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //debug("GetTestComm : " + ex.ToString());
                    //debug(ex.StackTrace);
                }
                //*/
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        private void refreshTest()
        {
            calcDate();

            if (!GlobalVar.Sql.connected)
            {
                FuncWin.TopMessageBox("database not connected!");
            }

            FuncLog.WriteLog("Trace - Communication " + startDate + "-" + endDate);

            dataGridAllTest.Rows.Clear();
            for (int site = 1; site <= 12; site++)
            {
                string[,] rs = FuncInline.GetTestResult(startDate, endDate, site);
                if (rs != null)
                {
                    int totalTest = 0;
                    int totalPass = 0;
                    string[] arrayTest = new string[12];
                    //while (rs.Read())
                    for (int j = 0; j < rs.GetLength(0); j++)
                    {
                        try
                        {
                            int arrayNo = int.Parse(rs[j, 1].ToString());
                            int total = int.Parse(rs[j, 2].ToString());
                            int pass = int.Parse(rs[j, 3].ToString());
                            totalTest += total;
                            totalPass += pass;
                            arrayTest[arrayNo - 1] = pass.ToString() + " / " + total.ToString();
                        }
                        catch (Exception ex)
                        {
                            //debug("refreshTest : " + ex.ToString());
                            //debug(ex.StackTrace);
                        }
                    }
                    //rs.Close();
                    dataGridAllPin.Rows.Add("Site-" + site.ToString(), totalTest.ToString(), 
                                                "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0",
                                                "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0");
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
            try
            {
                calcDate();

                if (!GlobalVar.Sql.connected)
                {
                    FuncWin.TopMessageBox("database not connected!");
                    return;
                }

                int inputCount = 0; //FuncInline.GetTotalInputCount(startDate, endDate);
                int passCount = 0;

                string[,] rs = null;

                FuncLog.WriteLog("Trace - Defect " + startDate + "-" + endDate + "," + (btnShiftA.BackColor == Color.Lime) + "," + (btnShiftB.BackColor == Color.Lime) + "," + (btnShiftC.BackColor == Color.Lime));

                // 사이트별 defect 현황
                int defectCount = 0;
                dataGridDefectSite.Rows.Clear();
                //for (int site = 1; site <= 21; site++)
                //{
                //if (FuncInline.UseSite[site - 1])
                //{
                //for (int array = 1; array <= FuncInline.MaxArrayCount; array++)
                //{
                rs = FuncInline.GetAllDefectResultBySiteArray(startDate,
                                                                endDate,
                                                                (btnShiftA.BackColor == Color.Lime),
                                                                (btnShiftB.BackColor == Color.Lime),
                                                                (btnShiftC.BackColor == Color.Lime),
                                                                "Site",
                                                                "Array",
                                                                false);
                //bool exist = false;
                if (rs != null)
                {
                    //while (rs.Read())
                    for (int j = 0; j < rs.GetLength(0); j++)
                    {
                        try
                        {
                            //exist = true;
                            int site = int.Parse(rs[j, 0].ToString());
                            int array = int.Parse(rs[j, 1].ToString());
                            int total = int.Parse(rs[j, 2].ToString());
                            int defect = int.Parse(rs[j, 3].ToString());
                            int ratio = int.Parse(rs[j, 4].ToString());
                            int rowNum = dataGridDefectSite.Rows.Count;
                            dataGridDefectSite.Rows.Add(site.ToString(), array.ToString(), total.ToString(), defect.ToString(), ratio.ToString("F2"));
                            dataGridDefectSite.Rows[rowNum - 1].DefaultCellStyle.BackColor = (rowNum - 1) % 2 > 0 ? Color.Cyan : Color.WhiteSmoke;

                            inputCount += total;
                            defectCount += defect;
                        }
                        catch (Exception ex)
                        {
                            //debug("refreshDefect : " + ex.ToString());
                            //debug(ex.StackTrace);
                        }
                    }
                    //rs.Close();
                }
                //if (!exist)
                //{
                //    dataGridDefectSite.Rows.Add("Test-" + site.ToString(), array.ToString(), "-", "-", "-");
                //}
                //}
                //}
                if (dataGridDefectSite.Rows.Count == 0)
                {
                    dataGridDefectSite.Rows.Add("no", "data");
                }

                /*
                lblTotalInput.Text = inputCount.ToString();
                lblTotalDefect.Text = defectCount.ToString();
                if (inputCount == 0)
                {
                    lblDefectRate.Text = "-";
                }
                else
                {
                    lblDefectRate.Text = ((double)defectCount / (double)inputCount * 100).ToString("F1") + " %";
                }

                lblTotalPass.Text = (inputCount - defectCount).ToString();
                //*/

                rs = FuncInline.GetAllDefectResultByShift(startDate,
                                                        endDate,
                                                        (btnShiftA.BackColor == Color.Lime),
                                                        (btnShiftB.BackColor == Color.Lime),
                                                        (btnShiftC.BackColor == Color.Lime),
                                                        true);
                if (rs != null &&
                    rs.GetLength(0) > 0)
                {
                    int.TryParse(rs[0, 0], out inputCount);
                    int.TryParse(rs[0, 1], out passCount);
                    int.TryParse(rs[0, 2], out defectCount);
                }
                else
                {
                    inputCount = 0;
                    passCount = 0;
                    defectCount = 0;
                }
                dataGridCount.Rows[1].Cells[1].Value = inputCount;
                dataGridCount.Rows[2].Cells[1].Value = passCount;
                dataGridCount.Rows[3].Cells[1].Value = defectCount;
                dataGridCount.Rows[4].Cells[1].Value = ((double)defectCount / (double)inputCount * 100).ToString("F1") + " %";

                // PCB Count
                rs = FuncInline.GetAllDefectResultByShift(startDate,
                                                        endDate,
                                                        (btnShiftA.BackColor == Color.Lime),
                                                        (btnShiftB.BackColor == Color.Lime),
                                                        (btnShiftC.BackColor == Color.Lime),
                                                        false);
                if (rs != null &&
                    rs.GetLength(0) > 0)
                {
                    int.TryParse(rs[0, 0], out inputCount);
                    int.TryParse(rs[0, 1], out passCount);
                    int.TryParse(rs[0, 2], out defectCount);
                }
                else
                {
                    inputCount = 0;
                    passCount = 0;
                    defectCount = 0;
                }
                dataGridCount.Rows[1].Cells[2].Value = inputCount;
                dataGridCount.Rows[2].Cells[2].Value = passCount;
                dataGridCount.Rows[3].Cells[2].Value = defectCount;
                dataGridCount.Rows[4].Cells[2].Value = ((double)defectCount / (double)inputCount * 100).ToString("F1") + " %";

                // defect code 별 현황
                dataGridDefectCode.Rows.Clear();
                rs = FuncInline.GetDefectResultByCode(startDate, endDate, (btnShiftA.BackColor == Color.Lime), (btnShiftB.BackColor == Color.Lime), (btnShiftC.BackColor == Color.Lime));
                int totalDefect = 0;
                if (rs != null)
                {
                    int rowNum = 0;
                    //while (rs.Read())
                    for (int j = 0; j < rs.GetLength(0); j++)
                    {
                        try
                        {
                            string code = rs[j, 0].ToString();
                            string name = rs[j, 1].ToString();
                            int defect = int.Parse(rs[j, 2].ToString());
                            totalDefect += defect;
                            dataGridDefectCode.Rows.Add(code, name, defect.ToString(), "0");
                            dataGridDefectCode.Rows[rowNum].DefaultCellStyle.BackColor = (rowNum) % 2 > 0 ? Color.Cyan : Color.WhiteSmoke;
                            rowNum++;
                        }
                        catch (Exception ex)
                        {
                            //debug(ex.ToString());
                            //debug(ex.StackTrace);
                        }
                    }
                    //rs.Close();

                    // 비율 구하기
                    if (totalDefect > 0)
                    {
                        for (int i = 0; i < dataGridDefectCode.Rows.Count; i++)
                        {
                            try
                            {
                                if (dataGridDefectCode.Rows[i] != null &&
                                    dataGridDefectCode.Rows[i].Cells != null &&
                                    dataGridDefectCode.Rows[i].Cells.Count > 3 &&
                                    dataGridDefectCode.Rows[i].Cells[2] != null &&
                                    dataGridDefectCode.Rows[i].Cells[2].Value != null &&
                                    dataGridDefectCode.Rows[i].Cells[3] != null)
                                {
                                    dataGridDefectCode.Rows[i].Cells[3].Value = (double.Parse(dataGridDefectCode.Rows[i].Cells[2].Value.ToString()) / totalDefect * 100).ToString("F2");
                                }
                            }
                            catch { }
                        }
                    }
                }
                if (totalDefect == 0)
                {
                    dataGridDefectCode.Rows.Add("no", "data");
                }

                /*
                // 전체 사이트/어레이/코드별 defect 발생현황
                int[,,] defectList = new int[40, 6, 1000];
                rs = FuncInline.GetDefectResultBySiteArrayCode(startDate, endDate, btnShiftA.BackColor == Color.Lime, btnShiftB.BackColor == Color.Lime, btnShiftC.BackColor == Color.Lime);
                totalDefect = 0;
                if (rs != null)
                {
                    dataGridDefectAll.Rows.Clear();
                    //while (rs.Read())
                    for (int j = 0; j < rs.GetLength(0); j++)
                    {
                        try
                        {
                            if (rs[j,2].ToString() == "")
                            {
                                continue;
                            }
                            // a.[Site], a.[Array], a.[DefectCode], count(a.[DefectCode]) as DefectCount
                            int site = int.Parse(rs[j,0].ToString());
                            int array = int.Parse(rs[j,1].ToString());
                            int code = int.Parse(rs[j,2].ToString());
                            int count = int.Parse(rs[j,3].ToString());
                            defectList[site - 1, array - 1, code] = count;
                        }
                        catch (Exception ex)
                        {
                            //debug(ex.ToString());
                            //debug(ex.StackTrace);
                        }
                        totalDefect++;
                    }
                    //rs.Close();

                    // 어레이별 합계계산
                    int[,] arrayDefect = new int[40, 6];
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

                    //dataGridDefectAll.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    int rowIndex = 0;
                    for (int k = 0; k < defectList.GetLength(0); k++)
                    {
                        for (int j = 0; j < defectList.GetLength(1); j++)
                        {
                            try
                            {
                                if (arrayDefect[k, j] > 0)
                                {
                                    string site = (k + 1).ToString() + "-";
                                    string array = (j == 0 ? site : "") + (j + 1).ToString();
                                    dataGridDefectAll.Rows.Add(array, arrayDefect[k, j].ToString());
                                    for (int i = 0; i < defectList.GetLength(2); i++)
                                    {
                                        if (defectList[k, j, i] > 0)
                                        {
                                            if (i <= 100)
                                            {
                                                dataGridDefectAll.Rows[rowIndex].Cells[i + 2].Value = defectList[k, j, i].ToString();
                                            }
                                            else
                                            {
                                                dataGridDefectAll.Rows[rowIndex].Cells[i - 994 + 2].Value = defectList[k, j, i].ToString();
                                            }
                                        }
                                    }
                                    dataGridDefectAll.Rows[rowIndex].DefaultCellStyle.BackColor = (rowIndex) % 2 > 0 ? Color.Cyan : Color.WhiteSmoke;
                                    rowIndex++;
                                }
                            }
                            catch (Exception ex)
                            {
                                //debug(ex.ToString());
                                //debug(ex.StackTrace);
                            }
                        }
                    }
                }
                dataGridDefectAll.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                if (totalDefect == 0)
                {
                    dataGridDefectAll.Rows.Add("no", "data");
                }
                //*/
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        private void pbExcelAllPin_Click(object sender, EventArgs e)
        {
            FuncFile.WriteNewExcelFromDataGrid(dataGridAllPin);

            /*
            try
            {
                string[,] data = new string[dataGridAllPin.Rows.Count + 1, dataGridAllPin.Columns.Count]; // 헤더 정보를 더하기 때문에 행을 하나 더한다
                Func.SetDataGridViewToArray(dataGridAllPin, ref data); // dataGridView 에서 데이터 가져오기

                FuncFile.WriteNewExcelData(data);
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
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

                FuncFile.WriteNewExcelData(data);
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
            //*/
        }

        private void pbExcellAllComm_Click(object sender, EventArgs e)
        {
            FuncFile.WriteNewExcelFromDataGrid(dataGridCommStatistics);

            /*
            try
            {
                string[,] data = new string[dataGridAllComm.Rows.Count + 1, dataGridAllComm.Columns.Count]; // 헤더 정보를 더하기 때문에 행을 하나 더한다
                Func.SetDataGridViewToArray(dataGridAllComm, ref data); // dataGridView 에서 데이터 가져오기

                FuncFile.WriteNewExcelData(data);
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
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

                FuncFile.WriteNewExcelData(data);
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
            //*/
        }

        private void LoadAllValue()
        {
            try
            {
                //numPinLifeCount.Value = (decimal)FuncInline.PinLifeCount;
                //cbCheckPinLife.Checked = FuncInline.CheckPinLife;
                lblDefectLimit.Text = FuncInline.DefectLimit.ToString() + " %";
                lblLogPeriod.Text = FuncInline.PinLogTime.ToString() + " days";
                lblLogDirectory.Text = FuncInline.PinLogDirectory;
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }

        }

        private void btnInputClear_Click(object sender, EventArgs e)
        {
            if (FuncWin.MessageBoxOK("Clear Input Count at selected Arrays ?"))
            {
                FuncLog.WriteLog("Trace Clear Input Count");
                for (int i = 0; i < dataGridAllPin.SelectedCells.Count; i++)
                {
                    int siteNo = int.Parse(dataGridAllPin.Rows[dataGridAllPin.SelectedCells[i].RowIndex].Cells[0].Value.ToString());
                    if (dataGridAllPin.SelectedCells[i].ColumnIndex == 0)
                    {
                        // 전체 어레이
                        for (int j = 0; j < FuncInline.PinArray.Length; j++)
                        {
                            FuncInline.ClearPinTestCount(siteNo,
                                                         FuncInline.PinArray[j],
                                                         false);
                        }
                    }
                    else
                    {
                        // 특정 어레이
                        int pinIndex = (dataGridAllPin.SelectedCells[i].ColumnIndex - 1) / 3;
                        FuncInline.ClearPinTestCount(siteNo,
                                                     FuncInline.PinArray[pinIndex],
                                                     false);
                    }
                }


                //for (int i = 0; i < dataGridAllPin.SelectedRows.Count; i++)
                //{
                //    //string sql = "update PinLog " +
                //    //            "set TestCount = 0 " +
                //    //            "where site = " + dataGridAllPin.SelectedRows[i].Cells[0].Value.ToString().Replace("Test-","") + " " +
                //    //            "and ArrayNo = " + dataGridAllPin.SelectedRows[i].Cells[1].Value.ToString().Replace("Array-", "") + " " +
                //    //            "and Using = 1";
                //    //GlobalVar.Sql.Execute(sql);
                //    FuncInline.ClearPinTestCount(int.Parse(dataGridAllPin.SelectedRows[i].Cells[0].Value.ToString().Replace("Test-", "")),
                //                                 int.Parse(dataGridAllPin.SelectedRows[i].Cells[1].Value.ToString().Replace("Array-", "")),
                //                                 false);
                //}

                refreshPin();
                FuncWin.TopMessageBox("Input Count cleared");
            }
        }

        private void btnNGClear_Click(object sender, EventArgs e)
        {
            if (FuncWin.MessageBoxOK("Clear NG Count at selected Arrays ?"))
            {
                FuncLog.WriteLog("Trace Clear NG Count");
                for (int i = 0; i < dataGridAllPin.SelectedCells.Count; i++)
                {
                    int siteNo = int.Parse(dataGridAllPin.Rows[dataGridAllPin.SelectedCells[i].RowIndex].Cells[0].Value.ToString());
                    if (dataGridAllPin.SelectedCells[i].ColumnIndex == 0)
                    {
                        // 전체 어레이
                        for (int j = 0; j < FuncInline.PinArray.Length; j++)
                        {
                            FuncInline.ClearPinTestCount(siteNo,
                                                         FuncInline.PinArray[j],
                                                         true);
                            FuncInline.ClearPinTestCount(siteNo,
                                                         FuncInline.PinArray[j],
                                                         false);
                        }
                    }
                    else
                    {
                        // 특정 어레이
                        int pinIndex = (dataGridAllPin.SelectedCells[i].ColumnIndex - 1) / 3;
                        FuncInline.ClearPinTestCount(siteNo,
                                                     FuncInline.PinArray[pinIndex],
                                                     true);
                        FuncInline.ClearPinTestCount(siteNo,
                                                     FuncInline.PinArray[pinIndex],
                                                     false);
                    }
                }

                //for (int i = 0; i < dataGridAllPin.SelectedRows.Count; i++)
                //{

                //FuncInline.ClearPinTestCount(int.Parse(dataGridAllPin.SelectedRows[i].Cells[0].Value.ToString().Replace("Test-", "")),
                //                             int.Parse(dataGridAllPin.SelectedRows[i].Cells[1].Value.ToString().Replace("Array-", "")),
                //                             false);
                //}

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
                    FuncLog.WriteLog("Trace - Clear " + clearStr);

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

        #region 그리드뷰 다층 헤더 관련 이벤트 함수
        private void dataGridAllPin_Paint(object sender, PaintEventArgs e)
        {
            DataGridView gv = (DataGridView)sender;
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            for (int i = 0; i < FuncInline.MaxArrayCount; i++)
            {
                Rectangle r1 = gv.GetCellDisplayRectangle(i * 3 + 1, -1, false);
                int width1 = gv.GetCellDisplayRectangle(i * 3 + 2, -1, false).Width;
                int width2 = gv.GetCellDisplayRectangle(i * 3 + 3, -1, false).Width;
                r1.X += 1;
                r1.Y += (r1.Height / 3) + 1;
                r1.Width = r1.Width + width1 + width2 - 2;
                r1.Height = (r1.Height / 3) - 2;
                e.Graphics.DrawRectangle(new Pen(gv.ForeColor), r1);
                e.Graphics.FillRectangle(new SolidBrush(Color.White),
                        r1);
                e.Graphics.DrawString("Array" + FuncInline.PinArray[i], //strHeaders[0],
                        gv.ColumnHeadersDefaultCellStyle.Font,
                        new SolidBrush(gv.ColumnHeadersDefaultCellStyle.ForeColor),
                        r1,
                        format);
            }
            for (int i = 0; i < FuncInline.MaxArrayCount / 2; i++)
            {
                Rectangle r1 = gv.GetCellDisplayRectangle(i * 6 + 1, -1, false);
                int width1 = gv.GetCellDisplayRectangle(i * 6 + 2, -1, false).Width;
                int width2 = gv.GetCellDisplayRectangle(i * 6 + 3, -1, false).Width;
                int width3 = gv.GetCellDisplayRectangle(i * 6 + 4, -1, false).Width;
                int width4 = gv.GetCellDisplayRectangle(i * 6 + 5, -1, false).Width;
                int width5 = gv.GetCellDisplayRectangle(i * 6 + 6, -1, false).Width;
                r1.X += 1;
                r1.Y += 1;
                r1.Width = r1.Width + width1 + width2 + width3 + width4 + width5 - 2;
                r1.Height = (r1.Height / 3) - 2;
                e.Graphics.DrawRectangle(new Pen(gv.ForeColor), r1);
                e.Graphics.FillRectangle(new SolidBrush(Color.White),
                        r1);
                e.Graphics.DrawString("Pin" + (i + 1), //strHeaders[0],
                        gv.ColumnHeadersDefaultCellStyle.Font,
                        new SolidBrush(gv.ColumnHeadersDefaultCellStyle.ForeColor),
                        r1,
                        format);
            }
        }

        private void dataGridAllPin_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            DataGridView gv = (DataGridView)sender;
            Rectangle rtHeader = gv.DisplayRectangle;
            rtHeader.Height = gv.ColumnHeadersHeight / 3;
            gv.Invalidate(rtHeader);
        }

        private void dataGridAllPin_Scroll(object sender, ScrollEventArgs e)
        {
            DataGridView gv = (DataGridView)sender;
            Rectangle rtHeader = gv.DisplayRectangle;
            rtHeader.Height = gv.ColumnHeadersHeight / 3;
            gv.Invalidate(rtHeader);
            dataGridAllPin_Paint(dataGridAllPin, new PaintEventArgs(gv.CreateGraphics(), new Rectangle()));
        }

        private void dataGridAllPin_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex > -1)
            {
                Rectangle r = e.CellBounds;
                r.Y += e.CellBounds.Height / 3;
                r.Height = e.CellBounds.Height / 3;
                e.PaintBackground(r, true);
                e.PaintContent(r);
                e.Handled = true;
            }
        }
        #endregion

        private void timerUI_Tick(object sender, EventArgs e)
        {
            //timerUI.Enabled = false;

            if (FuncInline.TabMain != FuncInline.enumTabMain.Trace)
            {
                Thread.Sleep(GlobalVar.ThreadSleep);
                timerUI.Enabled = true;
                return;
            }
            #region 검색 옵션
            dateStart.Visible = traceTab != enumTabTrace.Pin;
            lblTime.Visible = traceTab != enumTabTrace.Pin;
            dateEnd.Visible = traceTab != enumTabTrace.Pin;
            btnShiftA.Visible = traceTab != enumTabTrace.Pin;
            btnShiftB.Visible = traceTab != enumTabTrace.Pin;
            btnShiftC.Visible = traceTab != enumTabTrace.Pin && FuncInline.UseShiftC;
            if (!FuncInline.UseShiftC)
            {
                btnShiftC.BackColor = Color.White;
            }
            #endregion

            #region 그리드 색상 안 되어 있으면 색상 표현
            if (traceTab == enumTabTrace.Communication)
            {
                #region CommLog
                //grid.Rows[i].DefaultCellStyle.BackColor = i % 2 > 0 ? Color.Cyan : Color.WhiteSmoke;
                if (dataGridSiteComm.Rows.Count >= 2)
                {
                    if (dataGridSiteComm.Rows[0].DefaultCellStyle.BackColor != Color.WhiteSmoke ||
                        dataGridSiteComm.Rows[1].DefaultCellStyle.BackColor != Color.Cyan)
                    {
                        SetGridColor(dataGridSiteComm);
                    }
                }
                #endregion
                #region CommLogStat
                //grid.Rows[i].DefaultCellStyle.BackColor = i % 2 > 0 ? Color.Cyan : Color.WhiteSmoke;
                if (dataGridCommStatistics.Rows.Count >= 2)
                {
                    if (dataGridCommStatistics.Rows[0].DefaultCellStyle.BackColor != Color.WhiteSmoke ||
                        dataGridCommStatistics.Rows[1].DefaultCellStyle.BackColor != Color.Cyan)
                    {
                        SetGridColor(dataGridCommStatistics);
                    }
                }
                #endregion
            }
            else if (traceTab == enumTabTrace.Defect)
            {
                #region dataGridDefectCode
                //grid.Rows[i].DefaultCellStyle.BackColor = i % 2 > 0 ? Color.Cyan : Color.WhiteSmoke;
                if (dataGridDefectCode.Rows.Count >= 2)
                {
                    if (dataGridDefectCode.Rows[0].DefaultCellStyle.BackColor != Color.WhiteSmoke ||
                        dataGridDefectCode.Rows[1].DefaultCellStyle.BackColor != Color.Cyan)
                    {
                        SetGridColor(dataGridDefectCode);
                    }
                }
                #endregion
                #region dataGridDefectSite
                //grid.Rows[i].DefaultCellStyle.BackColor = i % 2 > 0 ? Color.Cyan : Color.WhiteSmoke;
                if (dataGridDefectSite.Rows.Count >= 2)
                {
                    if (dataGridDefectSite.Rows[0].DefaultCellStyle.BackColor != Color.WhiteSmoke ||
                        dataGridDefectSite.Rows[1].DefaultCellStyle.BackColor != Color.Cyan)
                    {
                        SetGridColor(dataGridDefectSite);
                    }
                }
                #endregion
            }
            #endregion

            //if (!GlobalVar.GlobalStop)
            //{
            //    Thread.Sleep(GlobalVar.ThreadSleep);
            //    timerUI.Enabled = true;
            //}
        }

        private void btnShiftA_Click(object sender, EventArgs e)
        {
            btnShiftA.BackColor = btnShiftA.BackColor == Color.Lime ? Color.White : Color.Lime;
        }

        private void btnShiftB_Click(object sender, EventArgs e)
        {
            btnShiftB.BackColor = btnShiftB.BackColor == Color.Lime ? Color.White : Color.Lime;
        }

        private void btnShiftC_Click(object sender, EventArgs e)
        {
            btnShiftC.BackColor = btnShiftC.BackColor == Color.Lime ? Color.White : Color.Lime;
        }

        private void dataGridAllPin_Click(object sender, EventArgs e)
        {
            //Disable Sorting for DataGridView
            //foreach (DataGridViewColumn item in ((DataGridView)sender).Columns)
            //{
            //    item.SortMode = DataGridViewColumnSortMode.NotSortable;
            //}
        }

        private void dataGridDefectSite_Click(object sender, EventArgs e)
        {
            //Disable Sorting for DataGridView
            //foreach (DataGridViewColumn item in ((DataGridView)sender).Columns)
            //{
            //    item.SortMode = DataGridViewColumnSortMode.NotSortable;
            //}
        }

        private void dataGridDefectCode_Click(object sender, EventArgs e)
        {
            //Disable Sorting for DataGridView
            //foreach (DataGridViewColumn item in ((DataGridView)sender).Columns)
            //{
            //    item.SortMode = DataGridViewColumnSortMode.NotSortable;
            //}
        }

        private void dataGridDefectAll_Click(object sender, EventArgs e)
        {
            //Disable Sorting for DataGridView
            //foreach (DataGridViewColumn item in ((DataGridView)sender).Columns)
            //{
            //    item.SortMode = DataGridViewColumnSortMode.NotSortable;
            //}
        }

        private void dataGridSiteComm_Click(object sender, EventArgs e)
        {
            //Disable Sorting for DataGridView
            //foreach (DataGridViewColumn item in ((DataGridView)sender).Columns)
            //{
            //    item.SortMode = DataGridViewColumnSortMode.NotSortable;
            //}

        }

        private void dataGridCommStatistics_Click(object sender, EventArgs e)
        {
            ////Disable Sorting for DataGridView
            //foreach (DataGridViewColumn item in ((DataGridView)sender).Columns)
            //{
            //    item.SortMode = DataGridViewColumnSortMode.NotSortable;
            //}
        }

        private void btnDefectAscending_Click(object sender, EventArgs e)
        {
            //if (btnDefectAscending.Text == "Ascending")
            //{
            //    btnDefectAscending.Text = "Descending";
            //    btnDefectAscending.BackColor = Color.Lime;
            //}
            //else
            //{
            //    btnDefectAscending.Text = "Ascending";
            //    btnDefectAscending.BackColor = Color.White;
            //}
        }

        private void btnCommAscending_Click(object sender, EventArgs e)
        {
            //if (btnCommAscending.Text == "Ascending")
            //{
            //    btnCommAscending.Text = "Descending";
            //    btnCommAscending.BackColor = Color.Lime;
            //}
            //else
            //{
            //    btnCommAscending.Text = "Ascending";
            //    btnCommAscending.BackColor = Color.White;
            //}
        }

        private void btnCommStatAscending_Click(object sender, EventArgs e)
        {
            //if (btnCommStatAscending.Text == "Ascending")
            //{
            //    btnCommStatAscending.Text = "Descending";
            //    btnCommStatAscending.BackColor = Color.Lime;
            //}
            //else
            //{
            //    btnCommStatAscending.Text = "Ascending";
            //    btnCommStatAscending.BackColor = Color.White;
            //}
        }

        private void dataGridDefectSite_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            //if (e.Column.Name.Equals("Site") ||
            //    e.Column.Name.Equals("Array") ||
            //    e.Column.Name.Equals("Code"))
            //{
            double a = double.Parse(e.CellValue1.ToString()), b = double.Parse(e.CellValue2.ToString());
            e.SortResult = a.CompareTo(b);
            e.Handled = true;
            //}
        }

        private void dataGridDefectCode_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            try
            {
                if (e.Column.Name.Equals("Defect_Code") ||
                    e.Column.Name.Equals("Defect_Count") ||
                    e.Column.Name.Equals("Defect_Ratio"))
                {
                    double a = double.Parse(e.CellValue1.ToString()), b = double.Parse(e.CellValue2.ToString());
                    e.SortResult = a.CompareTo(b);
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        private void dataGridSiteComm_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            try
            {
                if (e.Column.Name.Equals("Site") ||
                    e.Column.Name.Equals("Array"))
                {
                    double a = double.Parse(e.CellValue1.ToString()), b = double.Parse(e.CellValue2.ToString());
                    e.SortResult = a.CompareTo(b);
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }

        }

        private void dataGridCommStatistics_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            try
            {
                if (e.Column.Name.Equals("Stat_Site") ||
                    e.Column.Name.Equals("Stat_Array") ||
                    e.Column.Name.Equals("Stat_Count"))
                {
                    double a = double.Parse(e.CellValue1.ToString()), b = double.Parse(e.CellValue2.ToString());
                    e.SortResult = a.CompareTo(b);
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }

        }
    }
}
