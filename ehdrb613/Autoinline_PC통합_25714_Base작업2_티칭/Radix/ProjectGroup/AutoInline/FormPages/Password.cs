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
     * Password.cs : 관리자 기능 활성화용 비밀번호 입력창
     */

    public partial class Password : Form
    {
        public Password()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.BringToFront();
            pbOk.Focus();
            if (tbPassword.Text == FuncFile.ReadIniFile("manage", "pwd", GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\manage.ini", "1234"))
            {
                GlobalVar.PwdPass = true;
                Util.StartWatch(ref GlobalVar.PwdWatch);
                GlobalVar.ManagePasswd = tbPassword.Text;
                this.Close();
            }
            else
            {
                this.BringToFront();
                FuncWin.TopMessageBox("Password Dismatched!");
            }
        }

        private void Password_Shown(object sender, EventArgs e)
        {
            tbPassword.Focus();
            this.BringToFront();
            FuncKeyboard.OpenKeyboard();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            GlobalVar.PwdPass = false;
            this.Close();
        }

        private void Password_FormClosed(object sender, FormClosedEventArgs e)
        {
            FuncKeyboard.CloseKeyboard();
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

        private void btnOK_Click_1(object sender, EventArgs e)
        {
            if (tbPassword.Text == FuncFile.ReadIniFile("manage", "pwd", GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\manage.ini", "1234"))
            {
                GlobalVar.PwdPass = true;
                GlobalVar.ManagePasswd = tbPassword.Text;
                this.Close();
            }
            else
            {
                this.BringToFront();
                FuncWin.TopMessageBox("Password Dismatched!");
            }
        }

        private void tbPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                btnOk_Click(sender, e);
            }
        }
    }
}
