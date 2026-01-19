using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Radix
{
    /*
     * test.cs : 개발중 특별한 동작에 대한 체크
     */

    public partial class test : Form
    {
        
        //private const bool result1 = true;
        //private const bool result2 = true;
        //private const bool result3 = true;

        public test()
        {
            InitializeComponent();

        }

        private bool test_func()
        {
            bool rs = true;
            if (!func1())
            {
                return false;
            }
            if (!func2())
            {
                return false;
            }
            return func3();
        }

        private bool func1()
        {
            //Console.WriteLine("1");
            //....
            return cbArray.Checked;
        }

        private bool func2()
        {
            //Console.WriteLine("2");
            //....
            return cbFront.Checked;
        }

        private bool func3()
        {
            //Console.WriteLine("3");

            //....
            return cbOk.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //FuncLaserMarker.UpProductCount(cbArray.Checked, (int)numIndex.Value, cbFront.Checked ? "FRONT" : "REAR", tbCode.Text, cbOk.Checked ? "OK" : "NG");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //FuncFile.WriteIniFile("error_desc", "1000", tbini.Text, GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\ErrorDesc.ini");
        }

        private void btnMessage_Click(object sender, EventArgs e)
        {
            //cbMessage.Checked = false;
            //FuncWin.AutoClosingMessageBox(tbMessage.Text, tbMessage.Text, (int)numMessage.Value * 1000);
            //cbMessage.Checked = true;

            /*
            FuncWin.TopMessageBox(tbMessage.Text);
            Thread.Sleep((int)numMessage.Value * 1000);
            IntPtr hWnd = FuncWin.FindWindowByName(tbMessage.Text);
            if (hWnd != IntPtr.Zero)
            {
                FuncWin.PostMessage(hWnd, FuncWin.WM_CLOSE, 0, 0);
            }
            //*/
        }
    }
}
