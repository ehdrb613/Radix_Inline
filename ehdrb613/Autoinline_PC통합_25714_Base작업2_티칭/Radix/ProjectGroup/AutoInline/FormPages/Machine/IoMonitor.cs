using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Radix.Popup.Machine
{
    /*
     * IoMonitor.cs : 디지털 입력/출력 현황 조회 및 실행
     * YJ20221208 - Base 수정
     */

    public partial class IoMonitor : Form
    {
        int DiStart = 0;
        int DoStart = 0;

        //byte writeByte = 0;

        private void debug(string str)
        {
            //Util.Debug(str);
        }

        public IoMonitor()
        {
            InitializeComponent();
        }

        #region 초기화 관련
        private void IoMonitor_Shown(object sender, EventArgs e)
        {
            //DIO.SetDIOEnum();

            //Array dis = Enum.GetValues(typeof(enumDINames));
            //Console.WriteLine("di count : " + dis.Length);

            GlobalVar.dlgOpened = true;
            tmrIO.Start();
            this.BringToFront();
        }

        private void IoMonitor_Load(object sender, EventArgs e)
        {
            Console.WriteLine("load");
        }

        private void IoMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
        }


        private void IoMonitor_FormClosed(object sender, FormClosedEventArgs e)
        {
            //if (this.Parent != null)
            //{
            try
            {
                GlobalVar.dlgOpened = false;
                tmrIO.Stop();
                //this.Parent.BringToFront();
            }
            catch
            { }
            //}
        }
        #endregion

        #region 타이머 함수
        private void tmrIO_Tick(object sender, EventArgs e)
        {
            if (FuncInline.TabMain != FuncInline.enumTabMain.IO) // 메인 탭이 다른 곳에 있으면 실행 안 한다.
            {
                return;
            }
            tmrIO.Enabled = false;

            // 타이틀 표시
            int DiNum = DiStart / 8;
            if (GlobalVar.DiSize[0] + GlobalVar.DiSize[1] >= DiStart)
            {
                lblDiTitle0.Text = "X" + DiNum.ToString() + ".0 ~ X" + DiNum.ToString() + ".7";
            }
            if (GlobalVar.DiSize[0] + GlobalVar.DiSize[1] - 8 >= DiNum)
            {
                lblDiTitle1.Text = "X" + (DiNum + 1).ToString() + ".0 ~ X" + (DiNum + 1).ToString() + ".7";
            }
            int DoNum = DoStart / 8;
            if (GlobalVar.DoSize[0] + GlobalVar.DoSize[1] >= DoStart)
            {
                lblDoTitle0.Text = "Y" + DoNum.ToString() + ".0 ~ Y" + DoNum.ToString() + ".7";
            }
            if (GlobalVar.DoSize[0] + GlobalVar.DoSize[1] - 8 >= DoNum)
            {
                lblDoTitle1.Text = "Y" + (DoNum + 1).ToString() + ".0 ~ Y" + (DoNum + 1).ToString() + ".7";
            }

            // 버튼 제어
            pbDiPrev.Enabled = (DiStart >= 15);
            pbDiTop.Enabled = (DiStart >= 15);
            pbDiNext.Enabled = (DiStart <= (GlobalVar.DiSize[0] + GlobalVar.DiSize[1] - 17));
            pbDiBottom.Enabled = (DiStart <= (GlobalVar.DiSize[0] + GlobalVar.DiSize[1] - 17));
            pbDoPrev.Enabled = (DoStart >= 15);
            pbDoTop.Enabled = (DoStart >= 15);
            pbDoNext.Enabled = (DoStart <= (GlobalVar.DoSize[0] + GlobalVar.DoSize[1] - 17));
            pbDoBottom.Enabled = (DoStart <= (GlobalVar.DoSize[0] + GlobalVar.DoSize[1] - 17));

            for (int i = 0; i < 16; i++)
            {
                Control[] radio;
                Control[] label;

                int div = i / 8;
                int mod = i % 8;

                // DI
                radio = Controls.Find("radioDi" + div.ToString() + "_" + mod.ToString(), true);
                label = Controls.Find("lblDi" + div.ToString() + "_" + mod.ToString(), true);
                if (radio.Length > 0)
                {
                    //label[0].Text = ((enumDINames)(DiStart + i)).ToString();
                    //label[0].Text = DIO.getDiName(DiStart + i);
                    label[0].Text = DIO.GetDIName(DiStart + i);                    

                    radio[0].Enabled = (DiStart <= GlobalVar.DiSize[0] + GlobalVar.DiSize[1] - 1 - i); // 할당되지 않은 IO는 클랙 배제
                    ((RadioButton)(radio[0])).Checked = DIO.GetDIData(DiStart + i);
                }
                else
                {
                    label[0].Text = "";
                }

                // DO
                radio = Controls.Find("radioDo" + div.ToString() + "_" + mod.ToString(), true);
                label = Controls.Find("lblDo" + div.ToString() + "_" + mod.ToString(), true);
                if (radio.Length > 0)
                {
                    //label[0].Text = ((enumDONames)(DoStart + i)).ToString();
                    //label[0].Text = DIO.getDoName(DoStart + i);
                    label[0].Text = DIO.GetDOName(DoStart + i);

                    radio[0].Enabled = (DoStart <= GlobalVar.DoSize[0] + GlobalVar.DoSize[1] - 1 - i); // 할당되지 않은 IO는 클랙 배제
                    ((RadioButton)radio[0]).Checked = DIO.GetDORead(DoStart + i);
                    //Console.WriteLine(i +" " + DoStart);
                }
                else
                {
                    label[0].Text = "";
                }
            }

            if (!GlobalVar.GlobalStop)
            {
                tmrIO.Enabled = true;
            }
        }
        #endregion

        #region 버튼 이벤트
        private void btnDiPrev_Click(object sender, EventArgs e)
        {
            DiStart = Math.Max(DiStart - 16, 0);
        }

        private void btnDiNext_Click(object sender, EventArgs e)
        {
            DiStart = (int)Math.Min(DiStart + 16, GlobalVar.DiSize[0] + GlobalVar.DiSize[1] - 1);
        }

        private void btnDiTop_Click(object sender, EventArgs e)
        {
            DiStart = 0;
        }

        private void btnDiBottom_Click(object sender, EventArgs e)
        {
            int end = (int)(GlobalVar.DiSize[0] + GlobalVar.DiSize[1] - 1) / 16;
            DiStart = end * 16;
        }

        private void btnDoPrev_Click(object sender, EventArgs e)
        {
            DoStart = Math.Max(DoStart - 16, 0);
        }

        private void btnDoNext_Click(object sender, EventArgs e)
        {
            DoStart = (int)Math.Min(DoStart + 16, GlobalVar.DoSize[0] + GlobalVar.DoSize[1] - 1);
        }

        private void btnDoTop_Click(object sender, EventArgs e)
        {
            DoStart = 0;
        }

        private void btnDoBottom_Click(object sender, EventArgs e)
        {
            int end = (int)(GlobalVar.DoSize[0] + GlobalVar.DoSize[1] - 1) / 16;
            DoStart = end * 16;
        }


        private void radioDi_Click(object sender, EventArgs e)
        {
            try
            {
                //if (sender.GetType() == typeof(RadioButton) &&
                //    !((RadioButton)sender).Focused)
                //{
                //    return;    // 포커스가 없는 상태에서의 변경은 처리안함
                //}
                //if (sender.GetType() == typeof(Label) &&
                //    !((Label)sender).Focused)
                //{
                //    return;    // 포커스가 없는 상태에서의 변경은 처리안함
                //}
                Console.WriteLine("DI click");
                if (GlobalVar.Simulation)
                {
                    string name = "";
                    if (sender.GetType() == typeof(RadioButton))
                    {
                        name = ((RadioButton)sender).Name.Replace("radioDi", "");
                    }
                    else if (sender.GetType() == typeof(Label))
                    {
                        name = ((Label)sender).Name.Replace("lblDi", "");
                    }
                    if (name.Contains("_"))
                    {
                        string[] idx = name.Split('_');
                        int f = int.Parse(idx[0]);
                        int l = int.Parse(idx[1]);
                        DIO.WriteDIData(DiStart + f * 8 + l, !DIO.GetDIData(DiStart + f * 8 + l));
                    }
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        private void radioDo_Click(object sender, EventArgs e)
        {
            try
            {
                //if (sender.GetType() == typeof(RadioButton) &&
                //    !((RadioButton)sender).Focused)
                //{
                //    return;    // 포커스가 없는 상태에서의 변경은 처리안함
                //}
                //if (sender.GetType() == typeof(Label) &&
                //    !((Label)sender).Focused)
                //{
                //    return;    // 포커스가 없는 상태에서의 변경은 처리안함
                //}
                Console.WriteLine("DO click");
                if ((int)GlobalVar.SystemStatus < (int)enumSystemStatus.AutoRun)
                {
                    string name = "";
                    if (sender.GetType() == typeof(RadioButton))
                    {
                        name = ((RadioButton)sender).Name.Replace("radioDo", "");
                    }
                    else if (sender.GetType() == typeof(Label))
                    {
                        name = ((Label)sender).Name.Replace("lblDo", "");
                    }
                    if (name.Contains("_"))
                    {
                        string[] idx = name.Split('_');
                        int f = int.Parse(idx[0]);
                        int l = int.Parse(idx[1]);
                        DIO.WriteDOData(DoStart + f * 8 + l, !DIO.GetDORead(DoStart + f * 8 + l));
                    }
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }
        #endregion

    }
}
