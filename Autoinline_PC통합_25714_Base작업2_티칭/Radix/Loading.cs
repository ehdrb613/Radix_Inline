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
        bool Finsh;
        public bool getInitFinsh()
        {
            return Finsh;
        }
        public void setInitFinsh(bool finsh)
        {
            this.Finsh = finsh;
        }


        public Loading()
        {
            InitializeComponent();
            lbMsg.Text = "";
        }
       
       

        private void LoadingTimer_Tick(object sender, EventArgs e)
        {
            if (Finsh == true)
            {
                lbMsg.Text = "All done.";
            }


            if (!GlobalVar.Init_AutoRun_Finish)
            {
                /*
                if (!GlobalVar.Thread_AutoRun.Is_Init_AllDone())
                {
                    string msg = "Machine initalizing...";
                    lbMsg.Text = msg;
                }
                else
                {
                    string msg = "Machine init done.";
                    msg += "\nConveyor initalizing...";
                    lbMsg.Text = msg;
                }
                //*/
            }
            
        }
    }

}
