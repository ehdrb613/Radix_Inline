using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Data.SqlClient;
using System.Threading;

namespace Radix
{
    /*
     * LogViewer.cs : 에러 로그를 일자별로 조회
     */

    public partial class LogViewer : Form
    {
        private void debug(string str)
        {
            Util.Debug("LogView : " + str);
        }

        public LogViewer()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LogViewer_Shown(object sender, EventArgs e)
        {
            #region MsSQL 로그 사용에 따른 UI 변경
            //pnSql.Visible = GlobalVar.UseMsSQL;
            #endregion

            GlobalVar.dlgOpened = true;
            #region 로그파일 목록
            //string logPath = GlobalVar.FaPath;
            //if (!Directory.Exists(logPath))
            //{
            //    Directory.CreateDirectory(logPath);
            //}
            //logPath += "\\" + GlobalVar.SWName;
            //if (!Directory.Exists(logPath))
            //{
            //    Directory.CreateDirectory(logPath);
            //}
            //logPath += "\\" + GlobalVar.LogPath;
            //if (!Directory.Exists(logPath))
            //{
            //    Directory.CreateDirectory(logPath);
            //}
            //System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(logPath);
            //foreach (System.IO.FileInfo File in di.GetFiles())
            //{
            //    if (File.Extension.ToLower().CompareTo(".log") == 0)
            //    {
            //        try
            //        {
            //            String FileNameOnly = File.Name.Substring(0, File.Name.Length - 4).Replace(logPath + "\\", "");
            //            cmbDate.Items.Add(FileNameOnly);
            //        }
            //        catch
            //        { }
            //    }
            //}
            #endregion

            string today = DateTime.Now.ToString("yyyyMMdd");
            #region 오늘 날짜 로그 조회
            //ViewLog(logPath + "\\" + today + ".log");
            #endregion

            //for (int i = 0; i < cmbDate.Items.Count; i++)
            //{
            //    if (cmbDate.Items[i].ToString() == today)
            //    {
            //        cmbDate.SelectedIndex = i;
            //    }
            //}

        }

        public void ShowFromMain()
        {
            dateStart.Value = DateTime.Now;
            dateEnd.Value = DateTime.Now;
            Refresh();
        }

        private void cmbDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            //string logPath = GlobalVar.FaPath;
            //if (!Directory.Exists(logPath))
            //{
            //    Directory.CreateDirectory(logPath);
            //}
            //logPath += "\\" + GlobalVar.SWName;
            //if (!Directory.Exists(logPath))
            //{
            //    Directory.CreateDirectory(logPath);
            //}
            //logPath += "\\" + GlobalVar.LogPath;
            //if (!Directory.Exists(logPath))
            //{
            //    Directory.CreateDirectory(logPath);
            //}
            //logPath += "\\" + cmbDate.SelectedText + ".log";
            //if (File.Exists(logPath))
            //{
            //    ViewLog(logPath);
            //}
        }

        private void ViewLog(string filename)
        {
            if (File.Exists(filename))
            {
                StreamReader objReader = new StreamReader(filename);
                if (objReader != null)
                {
                    string sLine = "";

                    while (sLine != null)
                    {
                        sLine = objReader.ReadLine();
                        if (sLine != null)
                        {
                            tbLog.Text = sLine + "\r\n" + tbLog.Text;
                        }
                    }
                    objReader.Close();
                }
            }
        }

        private string ReadLog(string filename)
        {
            string logPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.LogPath + "\\" + filename + ".log";
            string str = "";
            if (File.Exists(logPath))
            {
                StreamReader objReader = new StreamReader(logPath);
                if (objReader != null)
                {
                    long fileSize = objReader.BaseStream.Length;
                    long fileSeek = 200 * 1000; // 최근 로그 200k 만 보자
                    if (fileSize > fileSeek)
                    {
                        objReader.BaseStream.Seek(-fileSeek, SeekOrigin.End);
                        objReader.ReadLine();
                        str += "<이후 데이터는 파일에서 직접 참고>";
                    }

                    string sLine = "";

                    while (sLine != null)
                    {
                        sLine = objReader.ReadLine();
                        if (sLine != null)
                        {
                            str = sLine + "\r\n" + str;
                        }
                    }
                    objReader.Close();
                }
                str = filename + " :\r\n" + str;
            }
            return str;
        }

        private void LogViewer_FormClosed(object sender, FormClosedEventArgs e)
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

        private void Refresh()
        {
            string startDate = dateStart.Value.Year.ToString() + Util.IntToString(dateStart.Value.Month, 2) + Util.IntToString(dateStart.Value.Day, 2);
            string endDate = dateEnd.Value.Year.ToString() + Util.IntToString(dateEnd.Value.Month, 2) + Util.IntToString(dateEnd.Value.Day, 2);

            var start = Util.TryParseDouble(startDate);
            var end = Util.TryParseDouble(endDate);
            if ( start > end )
            {
                FuncWin.TopMessageBox("DateTime not valid");
                return;
            }


            bool finish = false;
            int year = dateEnd.Value.Year;
            int month = dateEnd.Value.Month;
            int date = dateEnd.Value.Day;

            #region Log File 검색
            string log_text = "";
            while (!finish)
            {
                string str_date = year.ToString() + Util.IntToString(month, 2) + Util.IntToString(date, 2);
                Console.WriteLine(str_date);

                log_text += ReadLog(str_date) + "\r\n\r\n";

                if (startDate == str_date)
                {
                    break;
                }

                date--;
                if (date <= 0)
                {
                    date = 31;
                    month--;
                }
                if (month <= 0)
                {
                    year--;
                    month = 12;
                }
            }
            tbLog.Text = log_text;
            #endregion


            #region if MsSQL 로그 검색
            if (GlobalVar.PartError)
            {
                // dataGridAll datetime / part / errorcode / errorname
                dataGridAll.Rows.Clear();
                //string sql = "select [Date],[Time],[Part],[ErrorCode],[ErrorName],[Clear] " +
                //                "from [SystemError] " +
                //                "where date >= '" + startDate + "' " +
                //                "and date <= '" + endDate + "' " +
                //                "order by date desc, time desc";
                //SqlDataReader rs = GlobalVar.Sql.Read(sql);
                string[,] rs = FuncInline.GetSystemError(startDate, endDate);
                int rowNum = 0;
                if (rs != null)
                {
                    for (int i = 0; i < rs.GetLength(0); i++)
                    {
                        string rdate = rs[i, 0].ToString();
                        string rtime = rs[i, 1].ToString();
                        string part = rs[i, 2].ToString();
                        string code = rs[i, 3].ToString();
                        string name = rs[i, 4].ToString();
                        string clear = rs[i, 5].ToString() == "1" ? "O" : "";

                        //if (rdate.Length != 6)
                        //{
                        //    continue;
                        //}
                        //string datetime = rdate.Substring(0, 2) + "-" + rdate.Substring(2, 2) + "-" + rdate.Substring(4, 2) + " " + rtime;
                        dataGridAll.Rows.Add(rdate, rtime, part, code, name, clear);
                        dataGridAll.Rows[rowNum].DefaultCellStyle.BackColor = rowNum % 2 > 0 ? Color.Cyan : Color.WhiteSmoke;

                        rowNum++;
                    }
                }

                // datagridpart part / errorcount
                dataGridPart.Rows.Clear();
                //sql = "select Part,count(Part) as count " +
                //                "from[SystemError] " +
                //                "where date >= '" + startDate + "' " +
                //                "and date <= '" + endDate + "' " +
                //                "group by part";
                //rs = GlobalVar.Sql.Read(sql);
                rs = FuncInline.GetSystemErrorPartCount(startDate, endDate);
                if (rs != null)
                {
                    rowNum = 0;
                    for (int i = 0; i < rs.GetLength(0); i++)
                    {
                        string part = rs[i, 0].ToString();
                        string count = rs[i, 1].ToString();

                        dataGridPart.Rows.Add(part, count);
                        dataGridPart.Rows[rowNum].DefaultCellStyle.BackColor = rowNum % 2 > 0 ? Color.Cyan : Color.WhiteSmoke;

                        rowNum++;
                    }
                }

                // datagriderror errorcode / errorname / errorcount
                dataGridError.Rows.Clear();
                //sql = "select ErrorCode,ErrorName, count(ErrorCode) as count " +
                //                "from[SystemError] " +
                //                "where date >= '" + startDate + "' " +
                //                "and date <= '" + endDate + "' " +
                //                "group by ErrorCode,ErrorName";
                //rs = GlobalVar.Sql.Read(sql);
                rs = FuncInline.GetSystemErrorCodeCount(startDate, endDate);
                if (rs != null)
                {
                    rowNum = 0;
                    for (int i = 0; i < rs.GetLength(0); i++)
                    {
                        string code = rs[i, 0].ToString();
                        string name = rs[i, 1].ToString();
                        string count = rs[i, 2].ToString();

                        dataGridError.Rows.Add(code, name, count);
                        dataGridError.Rows[rowNum].DefaultCellStyle.BackColor = rowNum % 2 > 0 ? Color.Cyan : Color.WhiteSmoke;

                        rowNum++;
                    }
                }
            }
            #endregion
            #region else 로그 히스토리 검색
            else
            {
                dataGridPart.Rows.Clear();

                dataGridError.Rows.Clear();

                dataGridAll.Rows.Clear();

                for (int i = GlobalVar.SystemErrorList.Count - 1; i >= 0; i--)
                {
                    dataGridAll.Rows.Add(GlobalVar.SystemErrorList[i].Date,
                                            GlobalVar.SystemErrorList[i].Time,
                                            GlobalVar.SystemErrorList[i].ErrorPart,
                                            ((int)GlobalVar.SystemErrorList[i].ErrorCode).ToString(),
                                            GlobalVar.SystemErrorList[i].ErrorCode,
                                            GlobalVar.SystemErrorList[i].Cure,
                                            GlobalVar.SystemErrorList[i].Description);
                    bool exist = false;
                    for (int j = 0; j < dataGridPart.Rows.Count; j++)
                    {
                        if (dataGridPart.Rows[j].Cells[0].Value.ToString().Equals(GlobalVar.SystemErrorList[j].ErrorPart.ToString()))
                        {
                            exist = true;
                            dataGridPart.Rows[j].Cells[1].Value = (int.Parse(dataGridPart.Rows[j].Cells[1].Value.ToString()) + 1).ToString();
                        }
                    }
                    if (!exist)
                    {
                        dataGridPart.Rows.Add(GlobalVar.SystemErrorList[i].ErrorPart.ToString(),
                                                "1");
                    }

                    exist = false;
                    for (int j = 0; j < dataGridError.Rows.Count; j++)
                    {
                        if (dataGridError.Rows[j].Cells[1].Value.ToString().Equals(GlobalVar.SystemErrorList[j].ErrorCode.ToString()))
                        {
                            exist = true;
                            dataGridError.Rows[j].Cells[2].Value = (int.Parse(dataGridError.Rows[j].Cells[2].Value.ToString()) + 1).ToString();
                        }
                    }
                    if (!exist)
                    {
                        dataGridError.Rows.Add(((int)GlobalVar.SystemErrorList[i].ErrorCode).ToString(),
                                                GlobalVar.SystemErrorList[i].ErrorCode.ToString(),
                                                "1");
                    }
                }

            }
            #endregion
        }

        private void pbExcelAllError_Click(object sender, EventArgs e)
        {
            FuncFile.WriteNewExcelFromDataGrid(dataGridAll);
            /*
            try
            {
                string[,] data = new string[dataGridAll.Rows.Count + 1, dataGridAll.Columns.Count]; // 헤더 정보를 더하기 때문에 행을 하나 더한다
                Func.SetDataGridViewToArray(dataGridAll, ref data); // dataGridView 에서 데이터 가져오기

                FuncFile.WriteNewExcelData(data);
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            //*/
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Refresh();
        }
    }
}
