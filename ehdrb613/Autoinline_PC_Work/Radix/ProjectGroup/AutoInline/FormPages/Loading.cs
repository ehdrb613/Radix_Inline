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

    public partial class Loading : Form
    {

        public Loading()
        {
            InitializeComponent();
        }

        private void debug(string str)
        {
            Util.Debug("Loading : " + str);
        }


        public void RefreshStep()
        {
            //debug("RefeshStep");
            try
            {
                for (int i = 0; i < FuncInline.InitStarted.Length; i++)
                {
                    ((Label)(Controls.Find("lblInitStep" + i, true)[0])).ForeColor = FuncInline.InitStarted[i] ? Color.Black : Color.White;
                }
                for (int i = 0; i < FuncInline.InitDone.Length; i++)
                {
                    ((Label)(Controls.Find("lblDone" + i, true)[0])).Visible = FuncInline.InitDone[i];
                }
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
