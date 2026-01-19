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
     * MotionTest.cs : 각 서보 모터의 작동 체크
     */

    public partial class MotionTest : Form
    {
        public MotionTest()
        {
            InitializeComponent();
        }

        private void btOn_Click(object sender, EventArgs e)
        {
            FuncMotion.ServoOn(Convert.ToUInt32(numAxis.Value), true);
        }

        private void btOff_Click(object sender, EventArgs e)
        {
            FuncMotion.ServoOn(Convert.ToUInt32(numAxis.Value), false);
        }

        private void btReset_Click(object sender, EventArgs e)
        {
            FuncMotion.ServoReset(Convert.ToUInt32(numAxis.Value));
        }

        private void btHome_Click(object sender, EventArgs e)
        {
            FuncMotion.MoveHome(Convert.ToUInt32(numAxis.Value), false, 0);
        }

        private void btAbsolute_Click(object sender, EventArgs e)
        {
            if (numAxis.Value == 2)  // 각도
            {
                FuncMotion.MoveAbsolute(Convert.ToUInt32(numAxis.Value),
                                    FuncMotion.DegreeToPulse(Convert.ToDouble(tbPos.Text), GlobalVar.ServoGearRatio[(int)numAxis.Value], GlobalVar.ServoRevPulse[(int)numAxis.Value]),
                                    FuncMotion.DegreeToPulse(Convert.ToDouble(tbVel.Text), GlobalVar.ServoGearRatio[(int)numAxis.Value], GlobalVar.ServoRevPulse[(int)numAxis.Value]),
                                    FuncMotion.DegreeToPulse(Convert.ToDouble(tbAcc.Text), GlobalVar.ServoGearRatio[(int)numAxis.Value], GlobalVar.ServoRevPulse[(int)numAxis.Value]),
                                    FuncMotion.DegreeToPulse(Convert.ToDouble(tbDec.Text), GlobalVar.ServoGearRatio[(int)numAxis.Value], GlobalVar.ServoRevPulse[(int)numAxis.Value]),
                                    FuncMotion.DegreeToPulse(Convert.ToDouble(tbJerk.Text), GlobalVar.ServoGearRatio[(int)numAxis.Value], GlobalVar.ServoRevPulse[(int)numAxis.Value]),
                                    tbWait.Text == "1",
                                    Convert.ToUInt32(tbTimeout.Text));
            }
            else // 거리
            {
                FuncMotion.MoveAbsolute(Convert.ToUInt32(numAxis.Value),
                                   FuncMotion.MMToPulse(Convert.ToDouble(tbPos.Text), GlobalVar.ServoGearRatio[(int)numAxis.Value], GlobalVar.ServoRevMM[(int)numAxis.Value], GlobalVar.ServoRevPulse[(int)numAxis.Value]),
                                   FuncMotion.MMToPulse(Convert.ToDouble(tbVel.Text), GlobalVar.ServoGearRatio[(int)numAxis.Value], GlobalVar.ServoRevMM[(int)numAxis.Value], GlobalVar.ServoRevPulse[(int)numAxis.Value]),
                                   FuncMotion.MMToPulse(Convert.ToDouble(tbAcc.Text), GlobalVar.ServoGearRatio[(int)numAxis.Value], GlobalVar.ServoRevMM[(int)numAxis.Value], GlobalVar.ServoRevPulse[(int)numAxis.Value]),
                                   FuncMotion.MMToPulse(Convert.ToDouble(tbDec.Text), GlobalVar.ServoGearRatio[(int)numAxis.Value], GlobalVar.ServoRevMM[(int)numAxis.Value], GlobalVar.ServoRevPulse[(int)numAxis.Value]),
                                   FuncMotion.MMToPulse(Convert.ToDouble(tbJerk.Text), GlobalVar.ServoGearRatio[(int)numAxis.Value], GlobalVar.ServoRevMM[(int)numAxis.Value], GlobalVar.ServoRevPulse[(int)numAxis.Value]),
                                   tbWait.Text == "1",
                                   Convert.ToUInt32(tbTimeout.Text));
            }
        }

        private void btVelocity_Click(object sender, EventArgs e)
        {
            FuncMotion.MoveVelocity(Convert.ToUInt32(numAxis.Value),
                                Convert.ToDouble(tbVel.Text),
                                Convert.ToDouble(tbAcc.Text),
                                Convert.ToDouble(tbDec.Text),
                                Convert.ToDouble(tbJerk.Text));
            /*
            RTEX.MoveVelocity(Convert.ToUInt32(numAxis.Value),
                                FuncMotion.MMToPulse(Convert.ToDouble(tbVel.Text), 10, 25.84, 2500) * 61,
                                FuncMotion.MMToPulse(Convert.ToDouble(tbAcc.Text), 10, 25.84, 2500) * 61,
                                FuncMotion.MMToPulse(Convert.ToDouble(tbDec.Text), 10, 25.84, 2500) * 61,
                                FuncMotion.MMToPulse(Convert.ToDouble(tbJerk.Text), 10, 25.84, 2500) * 61);
                                */
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            FuncMotion.MoveStop(Convert.ToInt32(numAxis.Value));
        }

        private void tmrSatus_Tick(object sender, EventArgs e)
        {
            /*
            MXP.MXP_READAXISINFO_IN inInfo = new MXP.MXP_READAXISINFO_IN { };
            MXP.MXP_READAXISINFO_OUT outInfo = new MXP.MXP_READAXISINFO_OUT { };

            MXP.MXP_READSTATUS_IN statIn = new MXP.MXP_READSTATUS_IN { };
            MXP.MXP_READSTATUS_OUT statOut = new MXP.MXP_READSTATUS_OUT { };

            ushort AxisNo = Convert.ToUInt16(numAxis.Value);

            inInfo.Axis.AxisNo = AxisNo;
            inInfo.Enable = 1;

            statIn.Axis.AxisNo = AxisNo;
            statIn.Enable = 1;

            if (MXP.MXP_ReadAxisInfo(ref inInfo, out outInfo) == MXP.MXP_ret.RET_NO_ERROR)
            {
                tbErrored.Text = outInfo.Error.ToString();
                tbErrorID.Text = outInfo.ErrorID.ToString();
                tbHome.Text = outInfo.HomeAbsSwitch.ToString();
                tbLimitPos.Text = outInfo.LimitSwitchPos.ToString();
                tbLimitNeg.Text = outInfo.LimitSwitchNeg.ToString();
                tbPowerOn.Text = outInfo.PowerOn.ToString();
                tbHomed.Text = outInfo.IsHomed.ToString();

            }
            if (MXP.MXP_ReadStatus(ref statIn, out statOut) == MXP.MXP_ret.RET_NO_ERROR)
            {
                tbErrorStop.Text = statOut.ErrorStop.ToString();
                tbDisabled.Text = statOut.Disabled.ToString();
                tbStopping.Text = statOut.Stopping.ToString();
                tbHoming.Text = statOut.Homing.ToString();
                tbStandStill.Text = statOut.Standstill.ToString();
            }
            if (numAxis.Value == 2)
            {
                tbPosition.Text = (FuncMotion.PulseToDegree(Motion_Function.MXP_MC_ReadActualPosition(AxisNo), GlobalVar.ServoGearRatio[(int)numAxis.Value], GlobalVar.ServoRevPulse[(int)numAxis.Value])).ToString();
                tbVelocity.Text = (FuncMotion.PulseToDegree(Motion_Function.MXP_MC_GroupReadActualVelocity(AxisNo), GlobalVar.ServoGearRatio[(int)numAxis.Value], GlobalVar.ServoRevPulse[(int)numAxis.Value])).ToString();
            }
            else
            {
                tbPosition.Text = (FuncMotion.PulseToMM((long)Motion_Function.MXP_MC_ReadActualPosition(AxisNo), GlobalVar.ServoGearRatio[(int)numAxis.Value], GlobalVar.ServoRevMM[(int)numAxis.Value], GlobalVar.ServoRevPulse[(int)numAxis.Value])).ToString();
                tbVelocity.Text = (FuncMotion.PulseToMM((long)Motion_Function.MXP_MC_GroupReadActualVelocity(AxisNo), GlobalVar.ServoGearRatio[(int)numAxis.Value], GlobalVar.ServoRevMM[(int)numAxis.Value], GlobalVar.ServoRevPulse[(int)numAxis.Value])).ToString();
            }
            //*/
        }

        private void btnRelative_Click(object sender, EventArgs e)
        {
            if (numAxis.Value == 2)  // 각도
            {
                FuncMotion.MoveAbsolute(Convert.ToUInt32(numAxis.Value),
                                    FuncMotion.DegreeToPulse(Convert.ToDouble(tbPos.Text) - Convert.ToDouble(tbPosition.Text), GlobalVar.ServoGearRatio[(int)numAxis.Value], GlobalVar.ServoRevPulse[(int)numAxis.Value]),
                                    FuncMotion.DegreeToPulse(Convert.ToDouble(tbVel.Text), GlobalVar.ServoGearRatio[(int)numAxis.Value], GlobalVar.ServoRevPulse[(int)numAxis.Value]),
                                    FuncMotion.DegreeToPulse(Convert.ToDouble(tbAcc.Text), GlobalVar.ServoGearRatio[(int)numAxis.Value], GlobalVar.ServoRevPulse[(int)numAxis.Value]),
                                    FuncMotion.DegreeToPulse(Convert.ToDouble(tbDec.Text), GlobalVar.ServoGearRatio[(int)numAxis.Value], GlobalVar.ServoRevPulse[(int)numAxis.Value]),
                                    FuncMotion.DegreeToPulse(Convert.ToDouble(tbJerk.Text), GlobalVar.ServoGearRatio[(int)numAxis.Value], GlobalVar.ServoRevPulse[(int)numAxis.Value]),
                                    tbWait.Text == "1",
                                    Convert.ToUInt32(tbTimeout.Text));
            }
            else // 거리
            {
                FuncMotion.MoveAbsolute(Convert.ToUInt32(numAxis.Value),
                                   FuncMotion.MMToPulse(Convert.ToDouble(tbPos.Text), GlobalVar.ServoGearRatio[(int)numAxis.Value], GlobalVar.ServoRevMM[(int)numAxis.Value], GlobalVar.ServoRevPulse[(int)numAxis.Value]),
                                   FuncMotion.MMToPulse(Convert.ToDouble(tbVel.Text), GlobalVar.ServoGearRatio[(int)numAxis.Value], GlobalVar.ServoRevMM[(int)numAxis.Value], GlobalVar.ServoRevPulse[(int)numAxis.Value]),
                                   FuncMotion.MMToPulse(Convert.ToDouble(tbAcc.Text), GlobalVar.ServoGearRatio[(int)numAxis.Value], GlobalVar.ServoRevMM[(int)numAxis.Value], GlobalVar.ServoRevPulse[(int)numAxis.Value]),
                                   FuncMotion.MMToPulse(Convert.ToDouble(tbDec.Text), GlobalVar.ServoGearRatio[(int)numAxis.Value], GlobalVar.ServoRevMM[(int)numAxis.Value], GlobalVar.ServoRevPulse[(int)numAxis.Value]),
                                   FuncMotion.MMToPulse(Convert.ToDouble(tbJerk.Text), GlobalVar.ServoGearRatio[(int)numAxis.Value], GlobalVar.ServoRevMM[(int)numAxis.Value], GlobalVar.ServoRevPulse[(int)numAxis.Value]),
                                   tbWait.Text == "1",
                                   Convert.ToUInt32(tbTimeout.Text));
            }
        }
    }
}
