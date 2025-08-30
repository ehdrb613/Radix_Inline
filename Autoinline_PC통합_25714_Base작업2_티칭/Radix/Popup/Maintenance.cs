using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Radix
{
    public partial class Maintenance : Form
    {

        public Maintenance()
        {
            InitializeComponent();
        }



        private void btnIdle_Click(object sender, EventArgs e)
        {
            if (GlobalVar.SystemStatus < enumSystemStatus.AutoRun &&
                !GlobalVar.Secs.GetSECS_IdleState())
            {
                GlobalVar.Secs.SetSECS_IdleState(true, tbReason.Text);
                GlobalVar.Secs.S6F11_EventReport(enumECID.IdleReasonReport);
                btnIdle.Enabled = false;
                btnRun.Enabled = true;
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            if (GlobalVar.Secs.GetSECS_IdleState())
            {
                GlobalVar.Secs.SetSECS_IdleState(false, tbReason.Text);
                GlobalVar.Secs.S6F11_EventReport(enumECID.IdleReasonReport);
                btnIdle.Enabled = true;
                btnRun.Enabled = false;
                this.Close();
            }
        }
    }
}
