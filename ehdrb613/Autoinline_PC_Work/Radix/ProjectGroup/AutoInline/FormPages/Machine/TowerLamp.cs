using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Radix.Popup.Machine
{
    public partial class TowerLamp : Form
    {
        private bool valueChanged = false;
        private FuncInline.enumTabMain beforeMain = FuncInline.enumTabMain.Auto;
        private FuncInline.enumTabMachine beforeMachine = FuncInline.enumTabMachine.Machine;


        public TowerLamp()
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
                foreach (Control control in pnTower.Controls)
                {
                    //debug(control.Name);
                    if (control.Name.StartsWith("btnTower"))
                    {
                        int i = -1;
                        int j = -1;
                        string[] idx = control.Name.Replace("btnTower", "").Split('_');
                        if (idx.Length > 1)
                        {
                            int.TryParse(idx[0], out i);
                            int.TryParse(idx[1], out j);

                            if (i > -1 &&
                                j > -1)
                            {
                                ((Button)control).BackColor = GlobalVar.TowerAction[i, j, 0] ? Color.Yellow : Color.WhiteSmoke;
                                ((Button)control).Text = GlobalVar.TowerAction[i, j, 0] ? "ON" : "OFF";
                            }
                        }
                    }
                    else if (control.Name.StartsWith("btnBlink"))
                    {
                        int i = -1;
                        int j = -1;
                        string[] idx = control.Name.Replace("btnBlink", "").Split('_');
                        if (idx.Length > 1)
                        {
                            int.TryParse(idx[0], out i);
                            int.TryParse(idx[1], out j);

                            if (i > -1 &&
                                j > -1)
                            {
                                ((Button)control).BackColor = GlobalVar.TowerAction[i, j, 1] ? Color.Yellow : Color.WhiteSmoke;
                            }
                        }
                    }
                    else if (control.Name.StartsWith("numTower"))
                    {
                        int i = -1;
                        int.TryParse(control.Name.Replace("numTower", ""), out i);
                        if (i > -1)
                        {
                            ((NumericUpDown)control).Value = (decimal)GlobalVar.TowerTime[i];
                        }
                    }
                }
                /*
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        Controls.Find("btnTower" + i.ToString() + "_" + j.ToString(), true)[0].BackColor = GlobalVar.TowerAction[i, j, 0] ? Color.Yellow : Color.WhiteSmoke;
                        Controls.Find("btnTower" + i.ToString() + "_" + j.ToString(), true)[0].Text = GlobalVar.TowerAction[i, j, 0] ? "ON" : "OFF";
                        Controls.Find("btnBlink" + i.ToString() + "_" + j.ToString(), true)[0].BackColor = GlobalVar.TowerAction[i, j, 1] ? Color.Yellow : Color.WhiteSmoke;
                    }
                    ((NumericUpDown)(Controls.Find("numTower" + i.ToString(), true)[0])).Value = (decimal)GlobalVar.TowerTime[i];
                }
                //*/
                numInputStopTime.Value = (decimal)GlobalVar.InStopTime;
                numOutputStopTime.Value = (decimal)GlobalVar.OutStopTime;

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
                foreach (Control control in pnTower.Controls)
                {
                    //debug(control.Name);
                    if (control.Name.StartsWith("btnTower"))
                    {
                        int i = -1;
                        int j = -1;
                        string[] idx = control.Name.Replace("btnTower", "").Split('_');
                        if (idx.Length > 1)
                        {
                            int.TryParse(idx[0], out i);
                            int.TryParse(idx[1], out j);

                            if (i > -1 &&
                                j > -1)
                            {
                                GlobalVar.TowerAction[i, j, 0] = ((Button)control).BackColor != SystemColors.Control &&
                                                                 ((Button)control).BackColor != Color.WhiteSmoke;
                            }
                        }
                    }
                    else if (control.Name.StartsWith("btnBlink"))
                    {
                        int i = -1;
                        int j = -1;
                        string[] idx = control.Name.Replace("btnBlink", "").Split('_');
                        if (idx.Length > 1)
                        {
                            int.TryParse(idx[0], out i);
                            int.TryParse(idx[1], out j);

                            if (i > -1 &&
                                j > -1)
                            {
                                GlobalVar.TowerAction[i, j, 1] = ((Button)control).BackColor != SystemColors.Control &&
                                                                 ((Button)control).BackColor != Color.WhiteSmoke;
                            }
                        }
                    }
                    else if (control.Name.StartsWith("numTower"))
                    {
                        int i = -1;
                        int.TryParse(control.Name.Replace("numTower", ""), out i);
                        if (i > -1)
                        {
                            GlobalVar.TowerTime[i] = (ulong)(int)((NumericUpDown)control).Value;
                        }
                    }
                }
                /*
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        GlobalVar.TowerAction[i, j, 0] = Controls.Find("btnTower" + i.ToString() + "_" + j.ToString(), true)[0].BackColor != SystemColors.Control &&
                                                         Controls.Find("btnTower" + i.ToString() + "_" + j.ToString(), true)[0].BackColor != Color.WhiteSmoke;
                        GlobalVar.TowerAction[i, j, 1] = Controls.Find("btnBlink" + i.ToString() + "_" + j.ToString(), true)[0].BackColor != SystemColors.Control &&
                                                         Controls.Find("btnBlink" + i.ToString() + "_" + j.ToString(), true)[0].BackColor != Color.WhiteSmoke;
                    }
                    GlobalVar.TowerTime[i] = (int)((NumericUpDown)(Controls.Find("numTower" + i.ToString(), true)[0])).Value;
                }
                //*/
                GlobalVar.InStopTime = (int)numInputStopTime.Value;
                GlobalVar.OutStopTime = (int)numOutputStopTime.Value;

                valueChanged = false;
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }
        #endregion

        private void btnTower_Click(object sender, EventArgs e)
        {
            ((Button)sender).BackColor = ((Button)sender).BackColor == Color.Yellow ? Color.WhiteSmoke : Color.Yellow;
            valueChanged = true;
        }

        private void btnTower_Click_1(object sender, EventArgs e)
        {
            if (GlobalVar.SystemStatus < enumSystemStatus.AutoRun)
            {
                GlobalVar.EnableTower = !GlobalVar.EnableTower;
            }
        }

        private void tmrTower_Tick(object sender, EventArgs e)
        {
            //tmrTower.Enabled = false;

            FuncForm.SetButtonColor2(btnTower, GlobalVar.EnableTower);

            #region 값 수정 상태로 창 떠나면 알림
            if (valueChanged &&
                beforeMain == FuncInline.enumTabMain.Machine &&
                beforeMachine == FuncInline.enumTabMachine.TowerLamp &&
                (FuncInline.TabMain != FuncInline.enumTabMain.Machine ||
                        FuncInline.TabMachine != FuncInline.enumTabMachine.TowerLamp))
            {
                FuncInline.TabMain = FuncInline.enumTabMain.Machine;
                FuncInline.TabMachine = FuncInline.enumTabMachine.TowerLamp;

                valueChanged = false;
                if (FuncWin.MessageBoxOK("Tower Lamp Setting changed. Save?"))
                {
                    ApplyAllValue();
                    Func.SaveTowerIni();
                }
            }
            beforeMain = FuncInline.TabMain;
            beforeMachine = FuncInline.TabMachine;
            #endregion

            //if (!GlobalVar.GlobalStop)
            //{
            //    Thread.Sleep(GlobalVar.ThreadSleep);
            //    tmrTower.Enabled = true;
            //}
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            valueChanged = true;
        }

    }
}
