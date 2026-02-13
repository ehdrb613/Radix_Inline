using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Data.SqlClient;

namespace Radix
{
    /*
     * LogViewer.cs : 에러 로그를 일자별로 조회
     */

    public partial class NGViewer : Form
    {
        private void debug(string str)
        {
            Util.Debug("LogView : " + str);
        }

        public NGViewer()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void NGViewer_Shown(object sender, EventArgs e)
        {
            GlobalVar.dlgOpened = true;
        }

        private void NGViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            GlobalVar.dlgOpened = false;
            if (this.Parent != null)
            {
                try
                {
                    this.Parent.BringToFront();
                }
                catch
                { }
            }
        }

    }
}
