using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Radix
{
    public partial class ModeSelect : Form
    {
        public ModeSelect()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAuto_Click(object sender, EventArgs e)
        {
            if (FuncWin.MessageBoxOK("Change Mode to Auto Mode?"))
            {
                GlobalVar.DryRun = false;
                FuncInline.SimulationMode = false;
                FuncInline.PassMode = false;
                this.Close();
            }
        }

        private void btnPass_Click(object sender, EventArgs e)
        {
            if (FuncWin.MessageBoxOK("Change Mode to Pass Mode?"))
            {
                GlobalVar.DryRun = false;
                FuncInline.SimulationMode = false;
                FuncInline.PassMode = true;
                this.Close();
            }
        }

        private void btnSimulation_Click(object sender, EventArgs e)
        {
            if (FuncWin.MessageBoxOK("Change Mode to Simulation Mode?"))
            {
                GlobalVar.DryRun = false;
                FuncInline.SimulationMode = true;
                FuncInline.PassMode = false;
                this.Close();
            }
        }

        private void btnDryRun_Click(object sender, EventArgs e)
        {
            if (FuncWin.MessageBoxOK("Change Mode to DryRun Mode?"))
            {
                GlobalVar.DryRun = true;
                FuncInline.SimulationMode = false;
                FuncInline.PassMode = false;
                this.Close();
            }
        }

        private void ModeSelect_Shown(object sender, EventArgs e)
        {
            btnAuto.BackColor = !GlobalVar.DryRun && !FuncInline.SimulationMode && !FuncInline.PassMode ? Color.Lime : Color.White;
            btnPass.BackColor = FuncInline.PassMode ? Color.Lime : Color.White;
            btnDryRun.BackColor = GlobalVar.DryRun ? Color.Lime : Color.White;
            btnSimulation.BackColor = FuncInline.SimulationMode ? Color.Lime : Color.White;
        }
    }
}
