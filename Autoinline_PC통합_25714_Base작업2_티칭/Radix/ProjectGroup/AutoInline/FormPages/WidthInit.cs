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

    public partial class WidthInit : Form
    {

        public WidthInit()
        {
            InitializeComponent();
        }

        private void debug(string str)
        {
            Util.Debug("Initialize : " + str);
        }


        public void RefreshStep()
        {
            //debug("RefeshStep");
            try
            {
                lblInputConveyor.ForeColor = FuncInline.WidthStarted[(int)FuncInline.enumPMCAxis.ST03_InConveyor_Width] ? Color.Black : Color.White;
                lblInShuttle.ForeColor = FuncInline.WidthStarted[(int)FuncInline.enumPMCAxis.ST00_InShuttle_Width] ? Color.Black : Color.White;
                lblOutShuttle.ForeColor = FuncInline.WidthStarted[(int)FuncInline.enumPMCAxis.ST01_OutShuttle_Width] ? Color.Black : Color.White;
                lblOutputConveyor.ForeColor = FuncInline.WidthStarted[(int)FuncInline.enumPMCAxis.ST02_OutConveyor_Width] ? Color.Black : Color.White;
               
                if (FuncInline.InlineType >= FuncInline.enumInlineType.Gen5) 
                {
                    lblNGBuffer.Visible = true;
                    lblNGBuffer.ForeColor = FuncInline.WidthStarted[(int)FuncInline.enumPMCAxis.ST04_NGBuffer] ? Color.Black : Color.White;
                    lblDoneNGBuffer.Visible = FuncInline.WidthDone[(int)FuncInline.enumPMCAxis.ST04_NGBuffer];
                    
                }
                else
                {
                    lblNGBuffer.Visible = false;
                }

                lblDoneInConveyor.Visible = FuncInline.WidthDone[(int)FuncInline.enumPMCAxis.ST03_InConveyor_Width];
                lblDoneInShuttle.Visible = FuncInline.WidthDone[(int)FuncInline.enumPMCAxis.ST00_InShuttle_Width];
                lblDoneOutShuttle.Visible = FuncInline.WidthDone[(int)FuncInline.enumPMCAxis.ST01_OutShuttle_Width];
                lblDoneOutConveyor.Visible = FuncInline.WidthDone[(int)FuncInline.enumPMCAxis.ST02_OutConveyor_Width];

                lblDoneRack1.Visible = FuncInline.WidthDone[5]; // 서보 폭 조절 로직 신규작성 필요
                lblDoneRack2.Visible = FuncInline.WidthDone[6]; // 서보 폭 조절 로직 신규작성 필요


                this.Refresh();
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }


    }

}
