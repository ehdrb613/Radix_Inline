using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace Radix
{
    /*
     *  
     */

    public partial class CountLog : Form
    {
        private int pcbSum = 0;
        private int arraySum = 0;
        private int okSum = 0;
        private int ngSum = 0;
        private int unknownSum = 0;

        private bool first = true;

        public CountLog()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void refreshCount()
        {
            if (first)
            {
                first = false;
                return;
            }
            if (cmbFiles.SelectedIndex < 0)
            {
                this.BringToFront();
                FuncWin.TopMessageBox("Select date first");
                return;
            }
            first = false;

            pcbSum = 0;
            arraySum = 0;
            okSum = 0;
            ngSum = 0;
            unknownSum = 0;


            gridCount.Rows.Clear();

            string fileName = cmbFiles.Items[cmbFiles.SelectedIndex].ToString();
            string countPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.CountPath + "\\" + fileName + ".cnt";
            try
            {
                int pcbCount = int.Parse(FuncIni.ReadIniFile("count", "PCBCount", countPath, "0"));
                pcbSum = pcbCount;

                for (int i = 1; i <= pcbCount; i++)
                {
                    int arrayCount = int.Parse(FuncIni.ReadIniFile(i.ToString(), "ArrayCount", countPath, "0"));
                    arraySum += arrayCount;
                    for (int j = 1; j <= arrayCount; j++)
                    {
                        string front = FuncIni.ReadIniFile(i.ToString(), j + "_FRONT", countPath, "");
                        string rear = FuncIni.ReadIniFile(i.ToString(), j + "_REAR", countPath, "");

                        string frontCode;
                        string frontOk = "";
                        string rearCode;
                        string rearOk = "";

                        if (front.Length > 0 &&
                            front.Contains("_"))
                        {
                            string[] col = front.Split('_');
                            frontCode = col[0];
                            frontOk = col[1];
                            gridCount.Rows.Add(fileName, i, j, "FRONT", frontCode, frontOk);
                        }
                        if (rear.Length > 0 &&
                            rear.Contains("_"))
                        {
                            string[] col = rear.Split('_');
                            rearCode = col[0];
                            rearOk = col[1];
                            gridCount.Rows.Add(fileName, i, j, "REAR", rearCode, rearOk);
                        }

                        if (frontOk == "OK" &&
                            rearOk == "OK")
                        {
                            okSum++;
                        }
                        else if (frontOk == "NG" ||
                            rearOk == "NG")
                        {
                            ngSum++;
                        }
                        else
                        {
                            unknownSum++;
                        }
                    }
                }
            }
            catch { }
            lblSum.Text = "PCB : " + pcbSum.ToString() + 
                            "        ARRAY : " + arraySum.ToString() + 
                            "        OK : " + okSum.ToString() +
                            "        NG : " + ngSum.ToString() +
                            "        UNKNOWN : " + unknownSum.ToString();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            refreshCount();
        }

        private void CountLog_Shown(object sender, EventArgs e)
        {
            GlobalVar.dlgOpened = false;
            refreshList();
            refreshCount();
            this.BringToFront();
        }

        private void CountLog_FormClosed(object sender, FormClosedEventArgs e)
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

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            refreshCount();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void refreshList()
        {
            string countPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.CountPath;
            if (Directory.Exists(countPath))
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(countPath);
                foreach (System.IO.FileInfo File in di.GetFiles())
                {
                    if (File.Extension.ToLower().CompareTo(".cnt") == 0)
                    {
                        try
                        {
                            String FileNameOnly = File.Name.Substring(0, File.Name.Length - 4).Replace(countPath + "\\", "");
                            cmbFiles.Items.Add(FileNameOnly);
                        }
                        catch
                        { }
                    }
                }
            }
        }

        private void pbSaveCSV_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveDlg = new SaveFileDialog())
            {
                saveDlg.DefaultExt = "csv";
                saveDlg.Filter = "*.csv|*.csv";
                if (saveDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    File.Delete(saveDlg.FileName);

                    /*
                    if (File.Exists(saveDlg.FileName))
                    {
                        if (FuncWin.TopMessageBox(saveDlg.FileName + " exists. Overwrite ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            File.Delete(saveDlg.FileName);
                        }
                        else
                        {
                            return;
                        }
                    }
                    //*/
                    String header = "Date,PCB number,Array number,Side,Marked code,Result\n";
                    FuncFile.WriteFile(saveDlg.FileName, header);
                    for (int i = 0; i < gridCount.Rows.Count; i++)
                    {
                        try
                        {
                            string line = "";
                            if (gridCount.Rows[i].Cells.Count > 0)
                            {
                                line = gridCount.Rows[i].Cells[0].Value.ToString();
                            }
                            for (int j = 1; j < gridCount.Rows[i].Cells.Count; j++)
                            {
                                line += "," + gridCount.Rows[i].Cells[j].Value.ToString();
                            }
                            FuncFile.WriteFile(saveDlg.FileName, line);
                        }
                        catch
                        { }
                    }
                }
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            SeachCount();
        }
        //HJ 수정 200318 코드를 통한 생산이력 검색(CountLog.cs)

        [DllImport("kernel32.dll")]
        static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);
        private void SeachCount()
        {
            if (first)
            {
                first = false;
                return;
            }
            if (textBox_Search.Text == "" || textBox_Search.TextLength < 13)
            {
                this.BringToFront();
                FuncWin.TopMessageBox("Input Code");
                return;
            }

            first = false;

            gridCount.Rows.Clear();

            string countPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.CountPath + "\\";

            int count = 1;

            try
            {
                //string[] dir = Directory.GetDirectories(GlobalVar.FaPath);
                string[] files = Directory.GetFiles(countPath, $"*.{"cnt"}");

                foreach (string f in files)
                {// 이 곳에 해당 파일을 찾아서 처리할 코드를 삽입하면 된다.                     

                    Console.WriteLine((count++).ToString() + " 55 " + f + " 55 " + files);

                    try
                    {

                        // PCb 카운터를 서치하여 넣어 둠.
                        StringBuilder sb = new StringBuilder(1024);
                        GetPrivateProfileString("count", "PCBCount", "", sb, sb.Capacity, f);
                        string str = sb.ToString() == "" ? "0" : sb.ToString();

                        // PCb 카운터를 서치하여 넣어 둠.
                        //int pcbCount = int.Parse(FuncFile.ReadIniFile("count", "PCBCount", f, "0"));
                        int pcbCount = int.Parse(str);

                        //
                        for (int pcbnum = 0; pcbnum <= pcbCount; pcbnum++)
                        {
                            // Array 카운터를 서치하여 넣어 둠.
                            StringBuilder sb1 = new StringBuilder(1024);
                            GetPrivateProfileString(pcbnum.ToString(), "ArrayCount", "", sb1, sb1.Capacity, f);

                            string str1 = sb1.ToString() == "" ? "0" : sb1.ToString();

                            int arrayCount = int.Parse(str1);


                            for (int arraynum = 1; arraynum <= arrayCount; arraynum++)
                            {
                                // 결과값을 서치하여 넣어 둠.
                                StringBuilder sb2 = new StringBuilder(1024);
                                GetPrivateProfileString(pcbnum.ToString(), arraynum.ToString() + "_FRONT", "", sb2, sb2.Capacity, f);
                                StringBuilder sb3 = new StringBuilder(1024);
                                GetPrivateProfileString(pcbnum.ToString(), arraynum.ToString() + "_REAR", "", sb3, sb3.Capacity, f);

                                string str2 = sb2.ToString() == "" ? "0" : sb2.ToString();
                                string str3 = sb2.ToString() == "" ? "0" : sb2.ToString();

                                //
                                string filedate = f.Substring(22, 6);
                                string codedate_front = str2.Substring(0, 13);
                                string result_front = str2.Substring(14, 2);
                                string codedate_rear = str3.Substring(0, 13);
                                string result_rear = str3.Substring(14, 2);

                                if (str2.Contains(textBox_Search.Text))
                                {
                                    Console.WriteLine(filedate + " " + pcbnum + " " + arraynum + " " + "FRONT" + " " + codedate_front + " " + result_front);
                                    gridCount.Rows.Add(filedate, pcbnum, arraynum, "FRONT", codedate_front, result_front);
                                }
                                if (str3.Contains(textBox_Search.Text))
                                {
                                    Console.WriteLine(filedate + " " + pcbnum + " " + arraynum + " " + "REAR" + " " + codedate_rear + " " + result_rear);
                                    gridCount.Rows.Add(filedate, pcbnum, arraynum, "REAR", codedate_rear, result_rear);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
