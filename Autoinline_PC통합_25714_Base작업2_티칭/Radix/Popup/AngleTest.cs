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
    /*
     * AngleTest.cs : 비전 보정시 각도 계산 테스트용
     */

    public partial class AngleTest : Form
    {
        public AngleTest()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lblAngle.Text = Util.CheckAngle((double)numCenterX.Value, (double)numCenterY.Value, (double)numVisionX.Value, (double)numVisionY.Value).ToString();
        }
    }
}
