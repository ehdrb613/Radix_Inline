using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;    // SerialPort 클래스 사용을 위해서 추가
using System.IO; // MemoryStream
using System.Threading;
using System.Threading.Tasks;

namespace Radix
{

    public partial class SMDTest : Form
    {
        private System.Threading.Timer timerSMD; // Thread Timer

        private bool timerDoingSMD = false;


        public SMDTest()
        {
            InitializeComponent();

            #region 수신용 쓰레드 타이머 시작
            TimerCallback CallBackSMD = new TimerCallback(TimerSMD);
            timerSMD = new System.Threading.Timer(CallBackSMD, false, 0, 1000);
            #endregion
        }
        private void TimerSMD(Object state) // 화면 제어 쓰레드 타이머 함수
        {
            try
            {
                if (timerDoingSMD)
                {
                    return;
                }
                timerDoingSMD = true;

                if (!GlobalVar.GlobalStop &&
                    this.InvokeRequired)
                {
                    this.Invoke(new EventHandler(delegate
                {
                    for (int i = 0; i < 20; i++)
                    {
                        for (int j = 0; j < FuncInline.MaxArrayCount; j++)
                        {
                            // 0 --> 19, 19 --> 0, 20 --> 
                            if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + i].UserCancel)
                            {
                                dataGridSMD.Rows[((19 - i) * 2) + 1].Cells[j].Value = "Canceled";
                            }
                            else
                            {
                                dataGridSMD.Rows[((19 - i) * 2) + 1].Cells[j].Value = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + i].SMDStatus[j].ToString() + (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + i].SMDReady[j] ? " (R)" : "");
                            }
                        }
                    }
                }));
                }

            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
            timerDoingSMD = false;
            if (GlobalVar.GlobalStop)
            {
                try
                {
                    timerSMD.Dispose();
                }
                catch { }
            }
        }

        private void debug(string str)
        {
            Util.Debug(str);
        }

        private void SMDTest_Load(object sender, EventArgs e)
        {
            try
            {
                cmbSMDIndex.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }

        }

        private void SMDTest_Shown(object sender, EventArgs e)
        {
            long code = 1234567890123;
            #region PCB 정보 없으면 강제 할당
            for (int i =0; i < FuncInline.MaxSiteCount; i++)
            {
                for (int j = 0; j < FuncInline.MaxArrayCount; j++)
                {
                    if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + i].Barcode[j] == "")
                    {
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + i].Barcode[j] = (code++).ToString();
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + i].SMDStatus[j] = FuncInline.enumSMDStatus.Before_Command;
                    }
                }
                if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + i].PCBStatus == FuncInline.enumSMDStatus.UnKnown)
                {
                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + i].PCBStatus = FuncInline.enumSMDStatus.Before_Command;
                }
            }
            #endregion
            #region 그리드 기본 그리기
            for (int i = 20; i > 0; i--)
            {
                dataGridSMD.Rows.Add("Site" + i + "-1", "Site" + i + "-2", "Site" + i + "-3", "Site" + i + "-4", "Site" + i + "-5", "Site" + i + "-6", "Site" + (i) + "-7", "Site" + (i) + "-8", "Site" + (i) + "-9", "Site" + (i) + "-10", "Site" + (i) + "-11", "Site" + (i) + "-12");
                dataGridSMD.Rows[dataGridSMD.Rows.Count - 1].DefaultCellStyle.BackColor = Color.Cyan;
                dataGridSMD.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "");
                dataGridSMD.Rows[dataGridSMD.Rows.Count - 1].DefaultCellStyle.BackColor = Color.White;
            }
            #endregion
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FuncInline.UseSMDReady = true;
            for (int i = 0; i < FuncInline.MaxArrayCount; i++)
            {
                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + cmbSite.SelectedIndex].SMDReadySent[i] = false;
                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + cmbSite.SelectedIndex].SMDReady[i] = false;
            }
            FuncInline.ComSMD[cmbSMDIndex.SelectedIndex].SendStart(cmbSite.SelectedIndex);
        }

        private void btnTestStart_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < FuncInline.MaxArrayCount; i++)
            {
                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + cmbSite.SelectedIndex].SMDReady[i] = true;
            }
            FuncInline.ComSMD[cmbSMDIndex.SelectedIndex].SendStart(cmbSite.SelectedIndex);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FuncInline.ComSMD[cmbSMDIndex.SelectedIndex].SendStop(cmbSite.SelectedIndex);
        }
    }
}
