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

    public partial class BlockHistory : Form
    {

        public BlockHistory()
        {
            InitializeComponent();
        }

        private void debug(string str)
        {
            Util.Debug("BlockHistory : " + str);
        }


        private void SiteDetail_Shown(object sender, EventArgs e)
        {
            int site = ((int)FuncInline.DetailSite - (int)FuncInline.enumTeachingPos.Site1_F_DT1 + 1);
            lblSiteName.Text = "Site #" + site.ToString() + " Block History";

            string[,] rs = FuncInline.GetBlockHistory(site);

            if (rs != null)
            {
                dataGridBlock.Rows.Clear();
                //while (rs.Read())
                if (rs.GetLength(0) == 0)
                {
                    dataGridBlock.Rows.Add("No", "Data");
                }
                for (int j = 0; j < rs.GetLength(0); j++)
                {
                    // 0 Date	1 Time	2 Site	3 Array	4 Barcode	5 Type	6 Command_Send	7 Command_Receive	8 Command_OK	9 Test_Finish	10 Test_Pass	11 Test_Cancel	12 User_Timeout	13 Finish	14 NG	15 DefectCode	16 TestTime
                    string date = rs[j, 0].ToString();
                    string time = rs[j, 1].ToString();
                    string use = rs[j, 2].ToString() == "True" ? "Use" : "NotUse";
                    string content = rs[j, 3].ToString();
                    string comment = rs[j, 4].ToString();
                    dataGridBlock.Rows.Add(date, time, use, content, "delete",comment);
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
            foreach (DataGridViewColumn item in ((DataGridView)sender).Columns)
            {
                item.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            try
            {
                if (dataGridBlock.SelectedCells == null ||
                    dataGridBlock.SelectedCells.Count == 0)
                {
                    return;
                }
                int rowIndex = dataGridBlock.SelectedCells[0].RowIndex;
                int colIndex = dataGridBlock.SelectedCells[0].ColumnIndex;
                //debug("dataGridNG_Click : " + rowIndex + "," + colIndex);

                if (rowIndex >= dataGridBlock.Rows.Count ||
                    colIndex != 4)
                {
                    return;
                }

                int site = ((int)FuncInline.DetailSite - (int)FuncInline.enumTeachingPos.Site1_F_DT1 + 1);
                string date = dataGridBlock.Rows[rowIndex].Cells[0].Value.ToString();
                string time = dataGridBlock.Rows[rowIndex].Cells[1].Value.ToString();

                if (FuncWin.MessageBoxOK("Delete Site #" + site + " Block history data?" + " ( " + date + " " + time + " )"))
                {
                    string sql = "delete from BlockHistory " +
                                "Where Site = '" + site + "' " +
                                "And Date = '" + date + "' " +
                                "And Time = '" + time + "' ";
                    GlobalVar.Sql.Execute(sql);

                    SiteDetail_Shown(sender, e);
                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        private void dataGridBlock_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int site = ((int)FuncInline.DetailSite - (int)FuncInline.enumTeachingPos.Site1_F_DT1 + 1);
                int rowIndex = e.RowIndex;
                int columnIndex = e.ColumnIndex;
                string date = dataGridBlock.Rows[rowIndex].Cells[0].Value.ToString();
                string time = dataGridBlock.Rows[rowIndex].Cells[1].Value.ToString();
                string comment = dataGridBlock.Rows[rowIndex].Cells[5].Value.ToString();

                string sql = "update BlockHistory Set " +
                            "Comment = '" + comment.Replace("\n", "").Replace("'", "").Replace("\"", "") + "' " +
                            "Where Site = '" + site + "' " +
                            "And Date = '" + date + "' " +
                            "And Time = '" + time + "' ";
                GlobalVar.Sql.Execute(sql);
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }


    }

}
