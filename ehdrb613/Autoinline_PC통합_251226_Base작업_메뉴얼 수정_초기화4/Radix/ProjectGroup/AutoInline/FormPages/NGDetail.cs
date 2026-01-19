using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace Radix
{

    public partial class NGDetail : Form
    {

        public NGDetail()
        {
            InitializeComponent();
        }

        private void debug(string str)
        {
            //Util.Debug("Initialize : " + str);
        }


        private void SiteDetail_Shown(object sender, EventArgs e)
        {
            if (FuncInline.NGDetailPos == FuncInline.enumTeachingPos.None)
            {
                lblSiteName.Text = "ALL NG " + " Test History";
            }
            else
            {
                lblSiteName.Text = FuncInline.NGDetailPos == FuncInline.enumTeachingPos.NgBuffer ? "NG" : "NG" + " Test History";
            }

            string[,] rs = null;
            if (FuncInline.NGDetailPos == FuncInline.enumTeachingPos.None) // 위치 지정되지 않은 경우 전체 NG 조회
            {
                rs = FuncInline.GetPCBHistory(null);
            }
            else if (FuncInline.PCBInfo[(int)FuncInline.NGDetailPos].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
            {
                rs = FuncInline.GetPCBHistory(FuncInline.PCBInfo[(int)FuncInline.NGDetailPos].Barcode);
            }

            if (rs != null)
            {
                dataGridNG.Rows.Clear();
                //while (rs.Read())
                for (int j = 0; j < rs.GetLength(0); j++)
                {
                    // 0 Date	1 Time	2 Site	3 Array	4 Barcode	5 Type	6 Command_Send	7 Command_Receive	8 Command_OK	9 Test_Finish	10 Test_Pass	11 Test_Cancel	12 User_Timeout	13 Finish	14 NG	15 DefectCode	16 TestTime
                    string date = rs[j,0].ToString();
                    string time = rs[j,1].ToString();
                    string site = rs[j,2].ToString();
                    string array = rs[j,3].ToString();
                    string barCode = rs[j,4].ToString();
                    string type = rs[j,5].ToString();
                    string defectCode = rs[j,15].ToString();
                    string testTime = rs[j,16].ToString();

                    string commandSend = rs[j,6].ToString();
                    string commandReceive = rs[j,7].ToString();
                    string commandOk = rs[j,8].ToString();
                    string testFinish = rs[j,9].ToString();
                    string testPass = rs[j,10].ToString();
                    string testCancel = rs[j,11].ToString();
                    string testTimeout = rs[j,12].ToString();
                    string finish = rs[j,13].ToString();
                    string ng = rs[j,14].ToString();

                    //string result = "";

                    string defectName = "";
                    try
                    {
                        if (defectCode.Length > 0)
                        {
                            defectName = FuncInline.TestErrorCode[int.Parse(defectCode)];
                        }
                    }
                    catch { }

                    if (defectName == "")
                    {
                        defectName = "TEST_PASS";
                    }

                    dataGridNG.Rows.Add(date, time, site, array, barCode, defectCode, defectName);
                }
                //rs.Close();
            }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridNG_Click(object sender, EventArgs e)
        {
            //Disable Sorting for DataGridView
            //foreach (DataGridViewColumn item in ((DataGridView)sender).Columns)
            //{
            //    item.SortMode = DataGridViewColumnSortMode.NotSortable;
            //}
        }

        private void dataGridNG_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Name.Equals("Site") ||
                e.Column.Name.Equals("Array") ||
                e.Column.Name.Equals("Code"))
            {
                int a = int.Parse(e.CellValue1.ToString()), b = int.Parse(e.CellValue2.ToString());
                e.SortResult = a.CompareTo(b);
                e.Handled = true;
            }
        }

        private void SetGridColor(DataGridView grid)
        {
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                grid.Rows[i].DefaultCellStyle.BackColor = i % 2 > 0 ? Color.Cyan : Color.WhiteSmoke;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            #region dataGridNG
            if (dataGridNG.Rows.Count >= 2)
            {
                if (dataGridNG.Rows[0].DefaultCellStyle.BackColor != Color.WhiteSmoke ||
                    dataGridNG.Rows[1].DefaultCellStyle.BackColor != Color.Cyan)
                {
                    SetGridColor(dataGridNG);
                }
            }
            #endregion
        }
    }

}
