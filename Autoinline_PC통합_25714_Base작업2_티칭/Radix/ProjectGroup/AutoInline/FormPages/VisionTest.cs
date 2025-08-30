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
     * VisionTest.cs : 비전 개발 전 비전 관련 테스트
     */

    public partial class VisionTest : Form
    {
        public VisionTest()
        {
            InitializeComponent();
        }

        private void btnVisionCheck_Click(object sender, EventArgs e)
        {
            Func.VisitionConnectLeuze();
            /*
            tbVisionData.Text = GlobalVar.VisionResult;
            numX.Value = (decimal)GlobalVar.VisionPosition.x;
            numY.Value = (decimal)GlobalVar.VisionPosition.y;
            //*/
        }
    }
}
