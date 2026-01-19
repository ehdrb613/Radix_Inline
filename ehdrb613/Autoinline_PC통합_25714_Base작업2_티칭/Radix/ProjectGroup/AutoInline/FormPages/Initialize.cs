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

    public partial class Initialize : Form
    {

        public Initialize()
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

                foreach (Control control in this.Controls) // 사이트 묶음 전체
                {
                    for (int i = 0; i < FuncInline.InitialStarted.Length; i++)
                    {
                        if (control.Name == "lbl" + ((FuncInline.enumInitialize)i).ToString())
                        {
                            ((Label)control).ForeColor = FuncInline.InitialStarted[i] ? Color.Black : Color.White;
                        }
                    }
                    for (int i = 0; i < FuncInline.InitialDone.Length; i++)
                    {
                        if (control.Name == "lblDone" + ((FuncInline.enumInitialize)i).ToString())
                        {
                            Label label = (Label)control;
                            if (i >= (int)FuncInline.enumInitialize.Site1_F_DT1 &&
                                i <= (int)FuncInline.enumInitialize.Site1_F_DT1 + FuncInline.MaxSiteCount - 1)
                            {
                                int siteIndex = i - (int)FuncInline.enumInitialize.Site1_F_DT1;
                                /*
                                if (!FuncInline.GetDISiteData(siteIndex, (int)FuncInline.enumDISite.X28_Module_Power))
                                {
                                    label.Visible = true;
                                    label.Text = "Power Off";
                                }
                                else if (!FuncInline.UseSite[siteIndex])
                                {
                                    label.Visible = true;
                                    label.Text = "Not Use";
                                }
                                //*/
                                if (FuncInline.SiteState[siteIndex] != FuncInline.enumSiteState.Valid)
                                {
                                    label.Visible = true;
                                    label.Text = FuncInline.SiteState[siteIndex].ToString();
                                }
                                else
                                {
                                    label.Visible = FuncInline.InitialDone[i];
                                    //FuncInline.Init_Site[i - (int)FuncInline.enumInitialize.Site7] = FuncInline.InitialDone[i];
                                    label.Text = "Done";
                                }
                            }
                            else
                            {
                                label.Visible = FuncInline.InitialDone[i];
                                label.Text = "Done";
                            }
                        }
                    }
                }

                /*
                for (int i = 0; i < FuncInline.InitialStarted.Length; i++)
                {
                    try
                    {
                        ((Label)(Controls.Find("lbl" + ((FuncInline.enumInitialize)i).ToString(), true)[0])).ForeColor = FuncInline.InitialStarted[i] ? Color.Black : Color.White;
                    }
                    catch (Exception xx)
                    {
                        //debug(xx.ToString());
                        //debug(xx.StackTrace);
                    }
                }
                for (int i = 0; i < FuncInline.InitialDone.Length; i++)
                {
                    try
                    {
                        Label label = ((Label)(Controls.Find("lblDone" + ((FuncInline.enumInitialize)i).ToString(), true)[0]));
                        if (i >= (int)FuncInline.enumInitialize.Site1 &&
                            i <= (int)FuncInline.enumInitialize.Site20)
                        {
                            int siteIndex = i - (int)FuncInline.enumInitialize.Site1;
                            if (!FuncInline.UseSite[siteIndex])
                            {
                                label.Visible = true;
                                label.Text = "Not Use";
                            }
                            else
                            {
                                label.Visible = FuncInline.InitialDone[i] || FuncInline.Init_Site[siteIndex];
                                //FuncInline.Init_Site[i - (int)FuncInline.enumInitialize.Site7] = FuncInline.InitialDone[i];
                                label.Text = "Done";
                            }
                        }
                        else
                        {
                            label.Visible = FuncInline.InitialDone[i];
                            label.Text = "Done";
                        }
                    }
                    catch (Exception xx)
                    {
                        //debug(xx.ToString());
                        //debug(xx.StackTrace);
                    }
                }
                //*/
                this.Refresh();
                //Application.DoEvents();
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        private void btnStopOrigin_Click(object sender, EventArgs e)
        {
            if (FuncWin.MessageBoxOK("Stop and Cancel Origin?"))
            {
                for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumServoAxis)).Length; i++)
                {
                    FuncMotion.MoveStop(i);
                }
                for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length; i++)
                {
                    //FuncInline.ComPMC[(int)(i / 2)].Stop((FuncInline.enumPMCAxis)i);
                }
                GlobalVar.SystemStatus = enumSystemStatus.BeforeInitialize;
                FuncWin.TopMessageBox("Origin canceled");
                this.Close();
            }
        }


    }

}
