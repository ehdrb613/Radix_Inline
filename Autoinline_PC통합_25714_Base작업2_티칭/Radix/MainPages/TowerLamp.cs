using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Radix.Popup.Machine
{
    public partial class TowerLamp : UserControl
    {
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
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        Controls.Find("btnTower" + i.ToString() + "_" + j.ToString(), true)[0].BackColor = GlobalVar.TowerAction[i, j, (int)enumTowerLampAction.Enable] ? Color.Yellow : Color.WhiteSmoke;
                        Controls.Find("btnTower" + i.ToString() + "_" + j.ToString(), true)[0].Text = GlobalVar.TowerAction[i, j, (int)enumTowerLampAction.Enable] ? "ON" : "OFF";
                        Controls.Find("btnBlink" + i.ToString() + "_" + j.ToString(), true)[0].BackColor = GlobalVar.TowerAction[i, j, (int)enumTowerLampAction.Blink] ? Color.Yellow : Color.WhiteSmoke;
                    }
                    ((NumericUpDown)(Controls.Find("numTower" + i.ToString(), true)[0])).Value = (decimal)GlobalVar.TowerTime[i];
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }
        public void ApplyAllValue()
        {
            try
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        GlobalVar.TowerAction[i, j, 0] = Controls.Find("btnTower" + i.ToString() + "_" + j.ToString(), true)[0].BackColor != SystemColors.Control &&
                                                         Controls.Find("btnTower" + i.ToString() + "_" + j.ToString(), true)[0].BackColor != Color.WhiteSmoke;
                        GlobalVar.TowerAction[i, j, 1] = Controls.Find("btnBlink" + i.ToString() + "_" + j.ToString(), true)[0].BackColor != SystemColors.Control &&
                                                         Controls.Find("btnBlink" + i.ToString() + "_" + j.ToString(), true)[0].BackColor != Color.WhiteSmoke;
                    }
                    GlobalVar.TowerTime[i] = (ulong)((NumericUpDown)(Controls.Find("numTower" + i.ToString(), true)[0])).Value;
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }
        #endregion

        private void btnTower_Click(object sender, EventArgs e)
        {
            ((Button)sender).BackColor = ((Button)sender).BackColor == Color.Yellow ? Color.WhiteSmoke : Color.Yellow;
            if (((Button)sender).Name.StartsWith("btnTower"))
            {
                ((Button)sender).Text = ((Button)sender).BackColor == Color.Yellow ? "ON" : "OFF";
            }
        }

        private void TowerLamp_Load(object sender, EventArgs e)
        {
            try
            {
                //pnInOut.Visible = GlobalVar.UseInOutStop;
                for (int i = 0; i < Math.Min(8, Enum.GetValues(typeof(enumSystemStatus)).Length); i++)
                {
                    Label title = (Label)Controls.Find("lblTitle" + i, true)[0];
                    title.Text = ((enumSystemStatus)i).ToString();
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }
    }
}
