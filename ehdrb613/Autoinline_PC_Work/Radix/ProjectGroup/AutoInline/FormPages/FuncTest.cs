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
    public partial class FuncTest : Form
    {
        public FuncTest()
        {
            InitializeComponent();
        }

        private void debug(string str)
        {
            //Util.Debug(str);
        }

        private void FuncTest_Load(object sender, EventArgs e)
        {
           
        }

        private void btnPCBWidth_Click(object sender, EventArgs e)
        {
            tbPCBWidthResult.Text = Func.CheckPCBWidth((double)numSpeed.Value, (double)numPCBWidth.Value).ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tbPCBInputResult.Text = Func.InputPCB().ToString();
        }

        private void btnMoveStage_Click(object sender, EventArgs e)
        {
            //tbMoveStageResult.Text = Func.MoveStage((double)numSpeed.Value, (double)numMoveX.Value, (double)numMoveY.Value, cbTurn.Checked).ToString();
        }

        private void btnMark2D_Click(object sender, EventArgs e)
        {
            int startTime = Environment.TickCount;
            tbMark2DResult.Text = Func.LaserMarkCont(new structPosition(), false, true, tb2DText.Text).ToString();
            tbMark2DResult.Text += (Environment.TickCount - startTime).ToString();
        }

        private void btnWorkInput_Click(object sender, EventArgs e)
        {
            tbWorkInputResult.Text = Func.InputWork().ToString();
        }

        private void btnWorkOut_Click(object sender, EventArgs e)
        {
            tbWorkOutResult.Text = Func.OutputWork().ToString();
        }

        private void btnVisionAlign_Click(object sender, EventArgs e)
        {
            string str = Func.GetVisionResult(enumVisionCmd.Align);
            tbVisionAlignResult.Text = str;
            //tbVisionAlignX.Text = GlobalVar.VisionResultAlign.x.ToString();
            //tbVisionAlignY.Text = GlobalVar.VisionResultAlign.y.ToString();
        }

        private void btnVision2D_Click(object sender, EventArgs e)
        {
            string str = Func.GetVisionResult(enumVisionCmd.Code2D);
            tbVision2DResult.Text = str;
            //tbVision2DCode.Text = GlobalVar.VisionResult2D;
        }

        private void btnMarkText_Click(object sender, EventArgs e)
        {
            int startTime = Environment.TickCount;
            tbMarkTextResult.Text = Func.LaserMarkCont(new structPosition(), false, false, tbMarkText.Text).ToString();
            tbMarkTextResult.Text += (Environment.TickCount - startTime).ToString();
        }

        private void btnInitMark_Click(object sender, EventArgs e)
        {
            tbInitMarkResult.Text = Func.LaserMarkCont(new structPosition(), false, false, "A").ToString();
        }
    }
}
