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

    public partial class Loto : Form
    {



        public Loto()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void debug(string str)
        {
            //Util.Debug("Initialize : " + str);
        }


        private void tmrLoto_Tick(object sender, EventArgs e)
        {
            if (GlobalVar.E_Stop)
            {
                lblTitle.Text = "Alert";
                lblMessage.Text = "Emergency Stop";
            }
            //else if (FuncInline.Loto_Switch)
            //{
            //    lblTitle.Text = "No Operation";
            //    lblMessage.Text = "In Inner Work";
            //}
            //else if (GlobalVar.LightCurtain)
            //{
            //    lblTitle.Text = "Alert";
            //    lblMessage.Text = "Light Curtain Detected";
            //}
            else if (GlobalVar.DoorOpen)
            {
                lblTitle.Text = "Alert";
                lblMessage.Text = "Door Opened";
            }

            bool checkEMO = !GlobalVar.E_Stop;
            bool checkLC = !GlobalVar.LightCurtain;
            bool checkLOTO = !FuncInline.Loto_Switch &&
                                !FuncError.CheckError(FuncInline.enumErrorCode.Loto_Off);
            bool checkDOOR = !GlobalVar.DoorOpen;
            if (checkEMO &&
                        //checkLOTO &&
                        //checkLC &&
                        checkDOOR)
            {
                #region 서보모터 전원 끊었으면 다시 설정
                if (lblMessage.Text == "Emergency Stop")
                {
                    for (uint i = 0; i < Enum.GetValues(typeof(FuncInline.enumServoAxis)).Length; i++)
                    {
                        FuncMotion.ServoReset(i);
                    }
                    FuncMotion.ServoOnAll(true);
                }
                #endregion
                this.Close();
            }
        }

        private void Loto_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool checkEMO = !GlobalVar.E_Stop;
            bool checkLC = !GlobalVar.LightCurtain;
            bool checkLOTO = !FuncInline.Loto_Switch &&
                                !FuncError.CheckError(FuncInline.enumErrorCode.Loto_Off);
            bool checkDOOR = !GlobalVar.DoorOpen;


            if (!checkEMO ||
                //!checkLC ||
                //!checkLOTO ||
                !checkDOOR)
            {
                this.BringToFront();
                e.Cancel = true;
            }
        }


        private void Loto_Load(object sender, EventArgs e)
        {
            //bit = new Bitmap("sandglass.gif");
            //ImageAnimator.Animate(bit, new EventHandler(this.OnFrameChanged));
            //base.OnLoad(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //ImageAnimator.UpdateFrames();
            //Graphics g = pictureBox1.CreateGraphics();
            //g.DrawImage(this.bit, new Point(0, 0));
            //base.OnPaint(e);
        }

        private void OnFrameChanged(object sender, EventArgs e)
        {
            //this.Invalidate();
        }
    }

}
