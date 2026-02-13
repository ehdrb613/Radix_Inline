using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Radix.Popup
{
    public partial class WarningDialog : Form
    {
        string setText;
        int sec = 0;
        public WarningDialog()
        {
            InitializeComponent();
        }
        public WarningDialog(string text)
        {
            InitializeComponent();
            setText = text;
        }
        public string SetText
        {
            get
            {
                return setText;
            }

            set
            {
                setText = value;
            }
        }

      

        private void Stop_Click(object sender, EventArgs e)
        {
            //GlobalVar.SystemStatus = enumSystemStatus.Manual;
            GlobalVar.Warning = false;
            setText = "";   //닫기 할때 MSG 초기화
            GlobalVar.dlgWarning_Msg = "";

            // 없으면 경고를 무시할 리스트에 등록
            if (!GlobalVar.WarningStatePre.Contains(GlobalVar.WarningState))
            {
                GlobalVar.WarningStatePre.Add(GlobalVar.WarningState);
            }
            GlobalVar.SystemStatus = enumSystemStatus.Manual;
            GlobalVar.WarningState = 0;
            Close();
        }

        private void WarningDialog_Shown(object sender, EventArgs e)
        {
            //GlobalVar.WarningStatePre = -1;
            //GlobalVar.WarningStatePre.Clear();
            WaringName.Text = setText;
            GlobalVar.Warning = true;
        }

        // CLOSE 버튼 처리
        private void button2_Click(object sender, EventArgs e)
        {
            //GlobalVar.SystemStatus = enumSystemStatus.AutoRun;
            GlobalVar.Warning = false;
            GlobalVar.WarningState = 0;
            Close();
        }

        private void TimerUI_Tick(object sender, EventArgs e)
        {
            sec = sec + 1;
            timecheck.Text = sec + "초";

            if (sec  % 2 == 0)
            {
                Warning.Visible = false;
                WaringName.Visible = false;
            }
            else
            {
                Warning.Visible = true;
                WaringName.Visible = true;
            }
            WaringName.Text = setText;

        }

        private void WarningDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            sec = 0;
            GlobalVar.dlgWarning_Msg = "";
            GlobalVar.Warning = false; 
        }
    }
}
