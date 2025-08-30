using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;

namespace Radix.Popup.Machine
{
    public partial class PinBlock : Form
    {
        private int beforeIndex = -1;
        private int siteIndex = 0;
        private int pinIndex = 0;
        private string beforeCode = "";

        private bool valueChanged = false;
        private FuncInline.enumTabMain beforeMain = FuncInline.enumTabMain.Auto;
        private FuncInline.enumTabMachine beforeMachine = FuncInline.enumTabMachine.Machine;

        public PinBlock()
        {
            InitializeComponent();
        }

        private void debug(string str)
        {
            Util.Debug(str);
        }

        #region 설정 관련
        public void LoadAllValue()
        {
            try
            {
                numPinLifeDate.Value = (decimal)FuncInline.PinLifeDate;
                //numPinLifeCount.Value = (decimal)FuncInline.PinLifeCount;
                //cbCheckPinLife.Checked = FuncInline.CheckPinLife;
                numDefectLimit.Value = (decimal)FuncInline.DefectLimit;
                numPinLogTime.Value = (decimal)FuncInline.PinLogTime;
                tbPinLogDirectory.Text = FuncInline.PinLogDirectory;

                /* // Controls.find가 무거워 풀어쓴다.
                for (int i = 1; i <= FuncInline.MaxArrayCount; i++)
                {
                    ComboBox cmb = (ComboBox)Controls.Find("cmbPinArray" + i, true)[0];
                    Util.SetComboIndex(ref cmb, "Array" + FuncInline.PinArray[i - 1]);
                }
                //*/
                Util.SetComboIndex(ref cmbPinArray1, "Array" + FuncInline.PinArray[0]);
                Util.SetComboIndex(ref cmbPinArray2, "Array" + FuncInline.PinArray[1]);
                Util.SetComboIndex(ref cmbPinArray3, "Array" + FuncInline.PinArray[2]);
                Util.SetComboIndex(ref cmbPinArray4, "Array" + FuncInline.PinArray[3]);
                Util.SetComboIndex(ref cmbPinArray5, "Array" + FuncInline.PinArray[4]);
                Util.SetComboIndex(ref cmbPinArray6, "Array" + FuncInline.PinArray[5]);
                Util.SetComboIndex(ref cmbPinArray7, "Array" + FuncInline.PinArray[6]);
                Util.SetComboIndex(ref cmbPinArray8, "Array" + FuncInline.PinArray[7]);
                Util.SetComboIndex(ref cmbPinArray9, "Array" + FuncInline.PinArray[8]);
                Util.SetComboIndex(ref cmbPinArray10, "Array" + FuncInline.PinArray[9]);
                Util.SetComboIndex(ref cmbPinArray11, "Array" + FuncInline.PinArray[10]);
                Util.SetComboIndex(ref cmbPinArray12, "Array" + FuncInline.PinArray[11]);

                valueChanged = false;
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }
        public void ApplyAllValue()
        {
            try
            {
                FuncInline.PinLifeDate = (int)numPinLifeDate.Value;
                //FuncInline.PinLifeCount = (int)numPinLifeCount.Value;
                //FuncInline.CheckPinLife = cbCheckPinLife.Checked;
                FuncInline.DefectLimit = (double)numDefectLimit.Value;
                FuncInline.PinLogTime = (int)numPinLogTime.Value;
                FuncInline.PinLogDirectory = tbPinLogDirectory.Text;

                for (int i = 1; i <= FuncInline.MaxArrayCount; i++)
                {
                    ComboBox cmb = (ComboBox)Controls.Find("cmbPinArray" + i, true)[0];
                    FuncInline.PinArray[i - 1] = cmb.SelectedIndex + 1;
                }

                valueChanged = false;
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }
        #endregion

        private void tbPinLogDirectory_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                tbPinLogDirectory.Text = dlg.SelectedPath;
            }
        }

        private void tmrCheck_Tick(object sender, EventArgs e)
        {
            //tmrCheck.Enabled = false;

            try
            {
                if (FuncInline.TabMain != FuncInline.enumTabMain.Machine ||
                    FuncInline.TabMachine != FuncInline.enumTabMachine.PinBlock)
                {
                    //tmrCheck.Enabled = true;
                    return;
                }




                

                #region 창 떠날 때 저장 확인
                if (valueChanged &&
                    beforeMain == FuncInline.enumTabMain.Machine &&
                    beforeMachine == FuncInline.enumTabMachine.PinBlock &&
                    (FuncInline.TabMain != FuncInline.enumTabMain.Machine ||
                            FuncInline.TabMachine != FuncInline.enumTabMachine.PinBlock))
                {
                    FuncInline.TabMain = FuncInline.enumTabMain.Machine;
                    FuncInline.TabMachine = FuncInline.enumTabMachine.PinBlock;

                    valueChanged = false;
                    if (FuncWin.MessageBoxOK("Pin Block Setting changed. Save?"))
                    {
                        ApplyAllValue();
                        Func.SavePinIni();
                    }
                }
                beforeMain = FuncInline.TabMain;
                beforeMachine = FuncInline.TabMachine;
                #endregion
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }

            //if (!GlobalVar.GlobalStop)
            //{
            //    Thread.Sleep(GlobalVar.ThreadSleep);
            //    tmrCheck.Enabled = true;
            //}
        }


        private void ValueChanged(object sender, EventArgs e)
        {
            valueChanged = true;
        }

    }
}
