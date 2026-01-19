using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

///////////////////////////////////////////////////////////////////////
// 이 클래스는 MXP 함수를 좀더 사용이 용이하도록 제공하는 함수입니다.//
// 사용자 임의로 변경이 가능하지만 그 경우 동작하지 않을 수 있습니다.//
// 이 클래스를 사용하지 않고 함수 원형을 사용하여 구현할 수 있습니다.//
//                                                                   //
// Read명령어를 제외하고 bool type의 y인자가 True인 경우는           //
// OutParam 함수만을 수행하게 됩니다.                                //
///////////////////////////////////////////////////////////////////////

namespace Radix
{
    class Motion_Function
    {
        //static UInt16 ProcState = 0;
        static UInt32 MaxAxis = 0;
        static UInt32 Status = 0;

        public static void ServoOnAll(byte on)
        {
            UInt32 Count = 0;
            MXP.MXP_GetSlaveCount(1, out Count);
            for (int i = (int)Count - 1; i >= 0 ; i--)
            {
                if (on == 1)
                {
                    MXP.MXP_RESET_OUT resetOut = new MXP.MXP_RESET_OUT { };
                    Motion_Function.MXP_MC_Reset((uint)i, IndexCal((UInt32)MXP.MXP_MotionBlockIndex.mcReset) + (uint)i, false, resetOut);

                    Thread.Sleep(100);
                }

                MXP.MXP_POWER_OUT powerOut = new MXP.MXP_POWER_OUT { };
                MXP_MC_Power((uint)i, IndexCal((UInt32)MXP.MXP_MotionBlockIndex.mcPower) + (uint)i, on, false, powerOut);

                Thread.Sleep(100);
            }
        }

        public static UInt32 IndexCal(UInt32 Index)
        {
            return Index * MaxAxis;
        }

        public static bool Init()
        {
            UInt32 status = 0;
            Int32 InitError;

            for (int i = 0; i < 3; i++)
            {
                InitError = MXP.MXP_InitKernel_Developer(ref status);
                Thread.Sleep(500);
            }
            InitError = MXP.MXP_InitKernel_Developer(ref status);
            if (InitError == MXP.MXP_ret.RET_NO_ERROR)
            {
                //ProcState = (UInt16)MXP.MXP_KernelState.Initing;
            }
            else
            {
                //ProcState = (UInt16)MXP.MXP_KernelState.Idle;
                //Util.Debug("mxp init idle");
                //return false;
            }

            for (int i = 0; i < 10; i++)
            {
                if (MXP.MXP_SystemRun() == MXP.MXP_ret.RET_NO_ERROR)
                {
                    //ProcState = (UInt16)MXP.MXP_KernelState.Running;
                    Status++;
                    if (Status > 3)
                    {
                        break;
                    }
                }
                Thread.Sleep(1000);
            }
            if (Status <= 3)
            {
                //Util.Debug("mxp init fail : " + Status.ToString());
                //return false;
            }

            UInt32 usNumOfSlave = 0;
            if (MXP.MXP_GetSlaveCount(0, out usNumOfSlave) == MXP.MXP_ret.RET_NO_ERROR)
            {
                MaxAxis = usNumOfSlave;

                int nAxisCnt = 0;

                // Set combobox items based on number of slave.
                for (int i = 0; i < usNumOfSlave; i++)
                {
                    UInt32 usNodeType = 0;

                    if (MXP.MXP_QueryNodeType((UInt32)i, out usNodeType) == MXP.MXP_ret.RET_NO_ERROR)
                    {
                        if (usNodeType == (UInt32)MXP.MXP_SlaveType.CiA402_ST)
                        {
                            nAxisCnt++;
                        }
                    }
                }
            }
            else
            {
                //Util.Debug("mxp slave fail");
                //return false;
            }

            if (MXP.MXP_IsSlaveOnline(0, out status) == MXP.MXP_ret.RET_NO_ERROR)
            {
                if (status != (UInt32)MXP.MXP_ONLINESTATE_ENUM.NET_STATE_OP) // 0x08: operation
                {
                    //Util.Debug("mxp slave offline");
                    //return false;
                }
            }

            //ProcState = (UInt16)MXP.MXP_KernelState.Run;

            //Util.Debug("Mxp init success");
            return true;
        }

        public static bool MXP_CheckMotionKernel(UInt32 AxisNo)
        {
            Int32 Status = 0;
            MXP.MXP_GetKernelStatus(out Status);
            if (Status >= MXP.MXP_SysStatus.Initialized)
            {
                UInt32 status = 0;
                if (MXP.MXP_GetOnlineMode(ref status) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    if (status != (UInt32)MXP.MXP_ONLINESTATE_ENUM.NET_STATE_OP)  // 0x08: operation
                        return false;
                }

                if (MXP.MXP_IsSlaveOnline(AxisNo, out status) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    if (status != (UInt32)MXP.MXP_ONLINESTATE_ENUM.NET_STATE_OP) // 0x08: operation
                        return false;
                }
            }
            return true;
        }

        public static int MXP_MC_Power(UInt32 AxisNo, UInt32 MotionIdx, Byte on, bool getY, MXP.MXP_POWER_OUT y)
        {
            //if (MXP_CheckMotionKernel(AxisNo) == true)  // check kernel is ready to receive command.
            //{
                MXP.MXP_POWER_IN x = new MXP.MXP_POWER_IN { };
                x.Axis.AxisNo = AxisNo;
                x.Enable = on;
                if (getY)
                {
                    y = new MXP.MXP_POWER_OUT { };
                    if (MXP.MXP_GetPowerOutParam(MotionIdx, out y) != MXP.MXP_ret.RET_NO_ERROR)
                        return -1;
                }
                else if (MXP.MXP_PowerCmd(MotionIdx, ref x) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    return MXP.MXP_ret.RET_NO_ERROR;
                }
            //}
            return -1;
        }
        public static int MXP_MC_Stop(UInt32 AxisNo, UInt32 MotionIdx, Single Deceleration, Single Jerk, bool getY, MXP.MXP_STOP_OUT y)
        {
            //Util.Debug("MXP_MC_Stop : " + AxisNo.ToString());
            MXP.MXP_STOP_IN x = new MXP.MXP_STOP_IN { };
            x.Axis.AxisNo = AxisNo;
            x.Deceleration = Deceleration;
            x.Jerk = Jerk;
            x.Execute = 1;
            //if (MXP_CheckMotionKernel(AxisNo) == true)  // check kernel is ready to receive command.
            //{
                if (getY)
                {
                    y = new MXP.MXP_STOP_OUT { };
                    if (MXP.MXP_GetStopOutParam(MotionIdx, out y) != MXP.MXP_ret.RET_NO_ERROR)
                        return -1;
                }
                else// if (MXP.MXP_StopCmd(MotionIdx, ref x) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    MXP.MXP_StopCmd(MotionIdx, ref x);
                    x.Execute = 0;
                    MXP.MXP_StopCmd(MotionIdx, ref x);
                    return MXP.MXP_ret.RET_NO_ERROR;
                }
            //}
            return -1;
        }
        public static int MXP_MC_Home(UInt32 AxisNo, UInt32 MotionIdx, MXP.MXP_BUFFERMODE_ENUM BufferMode, bool getY, MXP.MXP_HOME_OUT y)
        {
            //Util.Debug("MXP_MC_Hhome : " + AxisNo.ToString());
            MXP.MXP_HOME_IN x = new MXP.MXP_HOME_IN { };

            x.Axis.AxisNo = AxisNo;
            x.Execute = 0;
            x.Position = 0;
            x.BufferMode = BufferMode;

            //if (MXP_CheckMotionKernel(AxisNo) == true)  // check kernel is ready to receive command.
            //{
                if (getY)
                {
                    y = new MXP.MXP_HOME_OUT { };
                    if (MXP.MXP_GetHomeOutParam(MotionIdx, out y) != MXP.MXP_ret.RET_NO_ERROR)
                        return -1;
                }
                else// if (MXP.MXP_HomeCmd(MotionIdx, ref x) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    MXP.MXP_HomeCmd(MotionIdx, ref x);
                    x.Execute = 1;
                    MXP.MXP_HomeCmd(MotionIdx, ref x);
                    return MXP.MXP_ret.RET_NO_ERROR;
                }
            //}
            return -1;
        }
        public static int MXP_MC_Home(UInt32 AxisNo, UInt32 MotionIdx, MXP.MXP_BUFFERMODE_ENUM BufferMode, bool getY, MXP.MXP_HOME_OUT y, bool WaitDone, uint Timeout)
        {
            int ret = MXP_MC_Home(AxisNo, MotionIdx, BufferMode, getY, y);
            if (ret == MXP.MXP_ret.RET_NO_ERROR && 
                WaitDone)
            {
                // 이동 완료 대기
                MXP.MXP_READAXISINFO_IN inInfo = new MXP.MXP_READAXISINFO_IN { };
                MXP.MXP_READAXISINFO_OUT outInfo = new MXP.MXP_READAXISINFO_OUT { };

                inInfo.Axis.AxisNo = AxisNo;
                inInfo.Enable = 1;

                int chkTime = Environment.TickCount;
                while (Environment.TickCount - chkTime < Timeout)
                {
                    //if (MXP_CheckMotionKernel(AxisNo) == true)  // check kernel is ready to receive command.
                    //{
                        if (MXP.MXP_ReadAxisInfo(ref inInfo, out outInfo) == MXP.MXP_ret.RET_NO_ERROR)
                        {
                            if (outInfo.IsHomed == 1)
                            {
                                //Util.Debug("MoveHome RET_NO_ERROR");
                                return MXP.MXP_ret.RET_NO_ERROR;
                            }
                            if (outInfo.Error > 0)
                            {
                                //Util.Debug("MoveHome RET_ERROR_FUNCTION");
                                return MXP.MXP_ret.RET_ERROR_FUNCTION;
                            }
                        }
                    //}
                    Thread.Sleep(100);
                }
                //Util.Debug("MoveHome RET_ERROR_FUNCTION");
                return MXP.MXP_ret.RET_ERROR_FUNCTION;
            }
            return ret;
        }

        public static int MXP_MC_Halt(UInt32 AxisNo, UInt32 MotionIdx, Single Deceleration, Single Jerk, MXP.MXP_BUFFERMODE_ENUM BufferMode, bool getY, MXP.MXP_HALT_OUT y)
        {
            MXP.MXP_HALT_IN x = new MXP.MXP_HALT_IN { };

            x.Axis.AxisNo = AxisNo;
            x.Execute = 0;
            x.Deceleration = Deceleration;
            x.Jerk = Jerk;
            x.BufferMode = BufferMode;

            //if (MXP_CheckMotionKernel(AxisNo) == true)  // check kernel is ready to receive command.
            //{
                if (getY)
                {
                    y = new MXP.MXP_HALT_OUT { };
                    if (MXP.MXP_GetHaltOutParam(MotionIdx, out y) != MXP.MXP_ret.RET_NO_ERROR)
                        return -1;
                }
                else// if (MXP.MXP_HaltCmd(MotionIdx, ref x) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    MXP.MXP_HaltCmd(MotionIdx, ref x);
                    x.Execute = 1;
                    MXP.MXP_HaltCmd(MotionIdx, ref x);
                    return MXP.MXP_ret.RET_NO_ERROR;
                }
            //}
            return -1;
        }
        public static int MXP_MC_Reset(UInt32 AxisNo, UInt32 MotionIdx, bool getY, MXP.MXP_RESET_OUT y)
        {
            MXP.MXP_RESET_IN x = new MXP.MXP_RESET_IN { };

            x.Axis.AxisNo = AxisNo;
            x.Execute = 0;

            //if (MXP_CheckMotionKernel(AxisNo) == true)  // check kernel is ready to receive command.
            //{
                if (getY)
                {
                    y = new MXP.MXP_RESET_OUT { };
                    if (MXP.MXP_GetResetOutParam(MotionIdx, out y) != MXP.MXP_ret.RET_NO_ERROR)
                        return -1;
                }
                else// if (MXP.MXP_ResetCmd(MotionIdx, ref x) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    MXP.MXP_ResetCmd(MotionIdx, ref x);
                    x.Execute = 1;
                    MXP.MXP_ResetCmd(MotionIdx, ref x);
                    return MXP.MXP_ret.RET_NO_ERROR;
                }
            //}
            return -1;
        }
        public static int MXP_MC_MoveAbsolute(UInt32 AxisNo, UInt32 MotionIdx, Single Velocity, Single Position, Single Acceleration, Single Deceleration, Single Jerk, MXP.MXP_BUFFERMODE_ENUM BufferMode, MXP.MXP_DIRECTION_ENUM Direction, bool getY, MXP.MXP_MOVEABSOLUTE_OUT y)
        {
            MXP.MXP_MOVEABSOLUTE_IN x = new MXP.MXP_MOVEABSOLUTE_IN { };

            x.Axis.AxisNo = AxisNo;
            x.Execute = 0;
            x.ContinuousUpdate = 0;
            x.Velocity = Velocity;
            x.Position = Position;
            x.Acceleration = Acceleration;
            x.Deceleration = Deceleration;
            x.Jerk = Jerk;
            x.BufferMode = BufferMode;
            x.Direction = Direction;

            //if (MXP_CheckMotionKernel(AxisNo) == true)  // check kernel is ready to receive command.
            //{
                if (getY)
                {
                    y = new MXP.MXP_MOVEABSOLUTE_OUT { };
                    if (MXP.MXP_GetMoveAbsoluteOutParam(MotionIdx, out y) != MXP.MXP_ret.RET_NO_ERROR)
                        return -1;
                }
                else// if (MXP.MXP_MoveAbsoluteCmd(MotionIdx, ref x) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    MXP.MXP_MoveAbsoluteCmd(MotionIdx, ref x);
                    x.Execute = 1;
                    MXP.MXP_MoveAbsoluteCmd(MotionIdx, ref x);
                    return MXP.MXP_ret.RET_NO_ERROR;
                }
            //}
            return -1;
        }

        public static int MXP_MC_MoveAbsolute(UInt32 AxisNo, UInt32 MotionIdx, Single Velocity, Single Position, Single Acceleration, Single Deceleration, Single Jerk, MXP.MXP_BUFFERMODE_ENUM BufferMode, MXP.MXP_DIRECTION_ENUM Direction, bool getY, MXP.MXP_MOVEABSOLUTE_OUT y, bool WaitDone, uint Timeout)
        {
            int ret = MXP_MC_MoveAbsolute(AxisNo, MotionIdx, Velocity, Position, Acceleration, Deceleration, Jerk, BufferMode, Direction, getY, y);
            if (ret == MXP.MXP_ret.RET_NO_ERROR &&
                WaitDone)
            {
                // 이동 완료 대기
                MXP.MXP_READSTATUS_IN statIn = new MXP.MXP_READSTATUS_IN { };
                MXP.MXP_READSTATUS_OUT statOut = new MXP.MXP_READSTATUS_OUT { };

                statIn.Axis.AxisNo = AxisNo;
                statIn.Enable = 1;

                int chkTime = Environment.TickCount;
                while (Environment.TickCount - chkTime < Timeout)
                {
                    //if (MXP_CheckMotionKernel(AxisNo) == true)  // check kernel is ready to receive command.
                    //{
                    //if (MXP.MXP_ReadStatus(ref statIn, out statOut) == MXP.MXP_ret.RET_NO_ERROR)
                    //{
                    //if (statOut.Standstill == 1 &&
                    //    Motion_Function.MXP_MC_ReadActualPosition(AxisNo) > Position - 1 &&
                    //    Motion_Function.MXP_MC_ReadActualPosition(AxisNo) < Position + 1)
                        if (GlobalVar.AxisStatus[AxisNo].StandStill &&
                            GlobalVar.AxisStatus[AxisNo].Position > Position - 10 &&
                            GlobalVar.AxisStatus[AxisNo].Position < Position + 10)
                        {
                            return MXP.MXP_ret.RET_NO_ERROR;
                        }
                        //}
                    //}
                    Thread.Sleep(100);
                }
                return MXP.MXP_ret.RET_ERROR_FUNCTION;
            }
            return ret;
        }

        public static int MXP_MC_MoveRelative(UInt32 AxisNo, UInt32 MotionIdx, Single Velocity, Single Distance, Single Acceleration, Single Deceleration, Single Jerk, MXP.MXP_BUFFERMODE_ENUM BufferMode, bool getY, MXP.MXP_MOVERELATIVE_OUT y)
        {
            MXP.MXP_MOVERELATIVE_IN x = new MXP.MXP_MOVERELATIVE_IN { };

            x.Axis.AxisNo = AxisNo;
            x.Execute = 0;
            x.ContinuousUpdate = 0;
            x.Velocity = Velocity;
            x.Distance = Distance;
            x.Acceleration = Acceleration;
            x.Deceleration = Deceleration;
            x.Jerk = Jerk;
            x.BufferMode = BufferMode;

            //if (MXP_CheckMotionKernel(AxisNo) == true)  // check kernel is ready to receive command.
            //{
                if (getY)
                {
                    y = new MXP.MXP_MOVERELATIVE_OUT { };
                    if (MXP.MXP_GetMoveRelativeOutParam(MotionIdx, out y) != MXP.MXP_ret.RET_NO_ERROR)
                        return -1;
                }
                else// if (MXP.MXP_MoveRelativeCmd(MotionIdx, ref x) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    MXP.MXP_MoveRelativeCmd(MotionIdx, ref x);
                    x.Execute = 1;
                    MXP.MXP_MoveRelativeCmd(MotionIdx, ref x);
                    return MXP.MXP_ret.RET_NO_ERROR;
                }
            //}
            return -1;
        }
        public static int MXP_MC_MoveVelocity(UInt32 AxisNo, UInt32 MotionIdx, Single Velocity, Single Acceleration, Single Deceleration, Single Jerk, MXP.MXP_BUFFERMODE_ENUM BufferMode, MXP.MXP_DIRECTION_ENUM Direction, bool getY, MXP.MXP_MOVEVELOCITY_OUT y)
        {
            MXP.MXP_MOVEVELOCITY_IN x = new MXP.MXP_MOVEVELOCITY_IN { };

            x.Axis.AxisNo = AxisNo;
            x.Execute = 0;
            x.ContinuousUpdate = 0;
            x.Velocity = Velocity;
            x.Acceleration = Acceleration;
            x.Deceleration = Deceleration;
            x.Jerk = Jerk;
            x.Direction = Direction;
            x.BufferMode = BufferMode;

            //if (MXP_CheckMotionKernel(AxisNo) == true)  // check kernel is ready to receive command.
            //{
                if (getY)
                {
                    y = new MXP.MXP_MOVEVELOCITY_OUT { };
                    if (MXP.MXP_GetMoveVelocityOutParam(MotionIdx, out y) != MXP.MXP_ret.RET_NO_ERROR)
                        return -1;
                }
                else// if (MXP.MXP_MoveVelocityCmd(MotionIdx, ref x) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    MXP.MXP_MoveVelocityCmd(MotionIdx, ref x);
                    x.Execute = 1;
                    MXP.MXP_MoveVelocityCmd(MotionIdx, ref x);
                    return MXP.MXP_ret.RET_NO_ERROR;
                }
            //}
            return -1;
        }
        public static int MXP_MC_TorqueControl(UInt32 AxisNo, UInt32 MotionIdx, Single Torque, Single TorqueRamp, Single Velocity, Single Acceleration, Single Deceleration, Single Jerk, MXP.MXP_BUFFERMODE_ENUM BufferMode, MXP.MXP_DIRECTION_ENUM Direction, bool getY, MXP.MXP_TORQUECONTROL_OUT y)
        {
            MXP.MXP_TORQUECONTROL_IN x = new MXP.MXP_TORQUECONTROL_IN { };

            x.Axis.AxisNo = AxisNo;
            x.Execute = 0;
            x.ContinuousUpdate = 0;
            x.Velocity = Velocity;
            x.Acceleration = Acceleration;
            x.Deceleration = Deceleration;
            x.Jerk = Jerk;
            x.Direction = Direction;
            x.BufferMode = BufferMode;

            //if (MXP_CheckMotionKernel(AxisNo) == true)  // check kernel is ready to receive command.
            //{
                if (getY)
                {
                    y = new MXP.MXP_TORQUECONTROL_OUT { };
                    if (MXP.MXP_GetTorqueControlOutParam(MotionIdx, out y) != MXP.MXP_ret.RET_NO_ERROR)
                        return -1;
                }
                else// if (MXP.MXP_TorqueControlCmd(MotionIdx, ref x) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    MXP.MXP_TorqueControlCmd(MotionIdx, ref x);
                    x.Execute = 1;
                    MXP.MXP_TorqueControlCmd(MotionIdx, ref x);
                    return MXP.MXP_ret.RET_NO_ERROR;
                }
            //}
            return -1;
        }
        public static int MXP_MC_ReadAxisStatus(UInt32 AxisNo)
        {
            MXP.MXP_READSTATUS_IN x = new MXP.MXP_READSTATUS_IN { };
            MXP.MXP_READSTATUS_OUT y = new MXP.MXP_READSTATUS_OUT { };

            x.Axis.AxisNo = AxisNo;
            x.Enable = 1;

            //if (MXP_CheckMotionKernel(AxisNo) == true)  // check kernel is ready to receive command.
            //{
                if (MXP.MXP_ReadStatus(ref x, out y) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    int status = 0;

                    if (y.ErrorStop == 1)
                        //status = status | 0x01;
                        status = (int)MXP.MXP_AXISSTATUS.ERROR_STOP;
                    if (y.Disabled == 1)
                        status = (int)MXP.MXP_AXISSTATUS.DISABLED;
                    if (y.Stopping == 1)
                        status = (int)MXP.MXP_AXISSTATUS.STOPPING;
                    if (y.Homing == 1)
                        status = (int)MXP.MXP_AXISSTATUS.HOMING;
                    if (y.Standstill == 1)
                        status = (int)MXP.MXP_AXISSTATUS.STANDSTILL;
                    if (y.DiscreteMotion == 1)
                        status = (int)MXP.MXP_AXISSTATUS.DISCRETE_MOTION;
                    if (y.ContinuousMotion == 1)
                        status = (int)MXP.MXP_AXISSTATUS.CONTINUOUS_MOTION;
                    if (y.SynchronizedMotion == 1)
                        status = (int)MXP.MXP_AXISSTATUS.SYNCHRONIZED_MOTION;

                    return status;
                }
            //}
            return -1;
        }
        public static int MXP_MC_ReadAxisError(UInt32 AxisNo, bool bIsAux)
        {
            MXP.MXP_READAXISERROR_IN x = new MXP.MXP_READAXISERROR_IN { };
            MXP.MXP_READAXISERROR_OUT y = new MXP.MXP_READAXISERROR_OUT { };

            x.Axis.AxisNo = AxisNo;
            x.Enable = 1;

            //if (MXP_CheckMotionKernel(AxisNo) == true)  // check kernel is ready to receive command.
            //{
                if (MXP.MXP_ReadAxisError(ref x, out y) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    UInt16 usError = y.AxisErrorID;

                    if (bIsAux)
                        usError = y.AuxErrorID;

                    return usError;
                }
            //}
            return -1;
        }

        public static Single MXP_MC_ReadCommandPosition(UInt32 AxisNo)
        {
            MXP.MXP_READCOMMANDPOSITION_IN x;
            MXP.MXP_READCOMMANDPOSITION_OUT y;

            x.Axis.AxisNo = AxisNo;
            x.Enable = 1;

            //if (MXP_CheckMotionKernel(AxisNo) == true)  // check kernel is ready to receive command.
            //{
                if (MXP.MXP_ReadCommandPosition(ref x, out y) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    return y.CommandPosition;
                }
            //}
            return -1;
        }

        public static Single MXP_MC_ReadCommandVelocity(UInt32 AxisNo)
        {
            MXP.MXP_READCOMMANDVELOCITY_IN x;
            MXP.MXP_READCOMMANDVELOCITY_OUT y;

            x.Axis.AxisNo = AxisNo;
            x.Enable = 1;

            //if (MXP_CheckMotionKernel(AxisNo) == true)  // check kernel is ready to receive command.
            //{
                if (MXP.MXP_ReadCommandVelocity(ref x, out y) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    return y.CommandVelocity;
                }
            //}
            return -1; ;
        }

        public static int MXP_MC_ReadAxisInfo(UInt32 AxisNo)
        {
            MXP.MXP_READAXISINFO_IN x = new MXP.MXP_READAXISINFO_IN { };
            MXP.MXP_READAXISINFO_OUT y = new MXP.MXP_READAXISINFO_OUT { };

            x.Axis.AxisNo = AxisNo;
            x.Enable = 1;

            //if (MXP_CheckMotionKernel(AxisNo) == true)  // check kernel is ready to receive command.
            //{
                if (MXP.MXP_ReadAxisInfo(ref x, out y) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    UInt16 usInfo = 0;

                    if (y.AxisWarning == 1)
                        usInfo = ((usInfo) |= (1 << (0)));
                    if (y.CommunicationReady == 1)
                        usInfo = ((usInfo) |= (1 << (1)));
                    if (y.HomeAbsSwitch == 1)
                        usInfo = ((usInfo) |= (1 << (2)));
                    if (y.IsHomed == 1)
                        usInfo = ((usInfo) |= (1 << (3)));
                    if (y.LimitSwitchNeg == 1)
                        usInfo = ((usInfo) |= (1 << (4)));
                    if (y.LimitSwitchPos == 1)
                        usInfo = ((usInfo) |= (1 << (5)));
                    if (y.PowerOn == 1)
                        usInfo = ((usInfo) |= (1 << (6)));
                    if (y.ReadyForPowerOn == 1)
                        usInfo = ((usInfo) |= (1 << (7)));
                    if (y.Simulation == 1)
                        usInfo = ((usInfo) |= (1 << (8)));

                    return usInfo;
                }
            //}
            return -1;
        }
        public static int MXP_MC_ReadMotionState(UInt32 AxisNo)
        {
            MXP.MXP_READMOTIONSTATE_IN x = new MXP.MXP_READMOTIONSTATE_IN { };
            MXP.MXP_READMOTIONSTATE_OUT y = new MXP.MXP_READMOTIONSTATE_OUT { };

            x.Axis.AxisNo = AxisNo;
            x.Enable = 1;

            //if (MXP_CheckMotionKernel(AxisNo) == true)  // check kernel is ready to receive command.
            //{
                if (MXP.MXP_ReadMotionState(ref x, out y) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    UInt16 usInfo = 0;

                    if (y.ConstantVelocity == 1)
                        usInfo = ((usInfo) |= (1 << (0)));
                    if (y.Accelerating == 1)
                        usInfo = ((usInfo) |= (1 << (1)));
                    if (y.Decelerating == 1)
                        usInfo = ((usInfo) |= (1 << (2)));
                    if (y.DirectionPositive == 1)
                        usInfo = ((usInfo) |= (1 << (3)));
                    if (y.DirectionNegative == 1)
                        usInfo = ((usInfo) |= (1 << (4)));

                    return usInfo;
                }
            //}
            return -1;
        }
        public static Single MXP_MC_ReadParameter(UInt32 AxisNo, Int32 ParameterNumber)
        {
            MXP.MXP_READPARAMETER_IN x = new MXP.MXP_READPARAMETER_IN { };
            MXP.MXP_READPARAMETER_OUT y = new MXP.MXP_READPARAMETER_OUT { };

            x.Axis.AxisNo = AxisNo;
            x.Enable = 1;
            x.ParameterNumber = ParameterNumber;

            //if (MXP_CheckMotionKernel(AxisNo) == true)  // check kernel is ready to receive command.
            //{
                if (MXP.MXP_ReadParameter(ref x, out y) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    return y.Value;
                }
            //}
            return -1;
        }
        public static Single MXP_MC_ReadBoolParameter(UInt32 AxisNo, Int32 ParameterNumber)
        {
            MXP.MXP_READBOOLPARAMETER_IN x = new MXP.MXP_READBOOLPARAMETER_IN { };
            MXP.MXP_READBOOLPARAMETER_OUT y = new MXP.MXP_READBOOLPARAMETER_OUT { };

            x.Axis.AxisNo = AxisNo;
            x.Enable = 1;
            x.ParameterNumber = ParameterNumber;

            //if (MXP_CheckMotionKernel(AxisNo) == true)  // check kernel is ready to receive command.
            //{
                if (MXP.MXP_ReadBoolParameter(ref x, out y) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    return y.Value;
                }
            //}
            return -1;
        }
        public static int MXP_MC_WriteParameter(UInt32 AxisNo, UInt32 MotionIdx, Int32 ParameterNumber, Single Value, bool getY, MXP.MXP_WRITEPARAMETER_OUT y)
        {
            MXP.MXP_WRITEPARAMETER_IN x = new MXP.MXP_WRITEPARAMETER_IN { };

            x.Axis.AxisNo = AxisNo;
            x.Execute = 0;
            x.ParameterNumber = ParameterNumber;
            x.Value = Value;

            //if (MXP_CheckMotionKernel(AxisNo) == true)  // check kernel is ready to receive command.
            //{
                if (getY)
                {
                    y = new MXP.MXP_WRITEPARAMETER_OUT { };
                    if (MXP.MXP_GetWriteParameterOutParam(MotionIdx, out y) != MXP.MXP_ret.RET_NO_ERROR)
                        return -1;
                }
                else //if (MXP.MXP_WriteParameterCmd(MotionIdx, ref x) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    MXP.MXP_WriteParameterCmd(MotionIdx, ref x);
                    x.Execute = 1;
                    MXP.MXP_WriteParameterCmd(MotionIdx, ref x);
                    return MXP.MXP_ret.RET_NO_ERROR;
                }
            //}
            return -1;
        }
        public static int MXP_MC_WriteBoolParameter(UInt32 AxisNo, UInt32 MotionIdx, UInt16 ParameterNumber, Byte Value, bool getY, MXP.MXP_WRITEBOOLPARAMETER_OUT y)
        {
            MXP.MXP_WRITEBOOLPARAMETER_IN x = new MXP.MXP_WRITEBOOLPARAMETER_IN { };

            x.Axis.AxisNo = AxisNo;
            x.Execute = 0;
            x.ParameterNumber = ParameterNumber;
            x.Value = Value;

            //if (MXP_CheckMotionKernel(AxisNo) == true)  // check kernel is ready to receive command.
            //{
                if (getY)
                {
                    y = new MXP.MXP_WRITEBOOLPARAMETER_OUT { };
                    if (MXP.MXP_GetWriteBoolParameterOutParam(MotionIdx, out y) != MXP.MXP_ret.RET_NO_ERROR)
                        return -1;
                }
                else //if (MXP.MXP_WriteBoolParameterCmd(MotionIdx, ref x) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    MXP.MXP_WriteBoolParameterCmd(MotionIdx, ref x);
                    x.Execute = 1;
                    MXP.MXP_WriteBoolParameterCmd(MotionIdx, ref x);
                    return MXP.MXP_ret.RET_NO_ERROR;
                }
            //}
            return -1;
        }
        public static Single MXP_MC_ReadActualVelocity(UInt32 AxisNo)
        {
            MXP.MXP_READACTUALVELOCITY_IN x = new MXP.MXP_READACTUALVELOCITY_IN { };
            MXP.MXP_READACTUALVELOCITY_OUT y = new MXP.MXP_READACTUALVELOCITY_OUT { };

            x.Axis.AxisNo = AxisNo;
            x.Enable = 1;

            //if (MXP_CheckMotionKernel(AxisNo) == true)  // check kernel is ready to receive command.
            //{
                if (MXP.MXP_ReadActualVelocity(ref x, out y) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    return y.Velocity;
                }
            //}
            return -1;
        }
        public static Single MXP_MC_ReadActualPosition(UInt32 AxisNo)
        {
            MXP.MXP_READACTUALPOSITION_IN x = new MXP.MXP_READACTUALPOSITION_IN { };
            MXP.MXP_READACTUALPOSITION_OUT y = new MXP.MXP_READACTUALPOSITION_OUT { };

            x.Axis.AxisNo = AxisNo;
            x.Enable = 1;

            //if (MXP_CheckMotionKernel(AxisNo) == true)  // check kernel is ready to receive command.
            //{
                if (MXP.MXP_ReadActualPosition(ref x, out y) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    return y.Position;
                }
            //}
            return -1;
        }
        public static Single MXP_MC_ReadActualTorque(UInt32 AxisNo)
        {
            MXP.MXP_READACTUALTORQUE_IN x = new MXP.MXP_READACTUALTORQUE_IN { };
            MXP.MXP_READACTUALTORQUE_OUT y = new MXP.MXP_READACTUALTORQUE_OUT { };

            x.Axis.AxisNo = AxisNo;
            x.Enable = 1;

            //if (MXP_CheckMotionKernel(AxisNo) == true)  // check kernel is ready to receive command.
            //{
                if (MXP.MXP_ReadActualTorque(ref x, out y) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    return y.Torque;
                }
            //}
            return -1;
        }

        public static UInt32 MXP_MC_ReadInputs(UInt32 AxisNo)
        {
            MXP.MXP_READINPUTS_IN x = new MXP.MXP_READINPUTS_IN { };
            MXP.MXP_READINPUTS_OUT y = new MXP.MXP_READINPUTS_OUT { };
            Byte[] Data = new Byte[100];// Servo Inputs Register 4Byte 0x60FD 
            x.Input.SourceNo = AxisNo;
            x.Enable = 1;

            //if (MXP_CheckMotionKernel(AxisNo) == true)  // check kernel is ready to receive command.
            //{
                if (MXP.MXP_ReadInputs(ref x, out y, out Data[0]) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    Int32 reData = 0;

                    for (int i = 0; i < y.Size; i++)
                        reData += Data[i] << (8 * i);
                    return (UInt32)reData;
                }
            //}
            return 0;
        }
        public static UInt32 MXP_MC_ReadOutputs(UInt32 AxisNo)
        {
            MXP.MXP_READOUTPUTS_IN x = new MXP.MXP_READOUTPUTS_IN { };
            MXP.MXP_READOUTPUTS_OUT y = new MXP.MXP_READOUTPUTS_OUT { };
            Byte[] Data = new Byte[100]; // Servo Output Register 4Byte 0x60FE
            Int32 rtData = 0;
            x.Output.SourceNo = AxisNo;
            x.Enable = 1;

            //if (MXP_CheckMotionKernel(AxisNo) == true)  // check kernel is ready to receive command.
            //{
                if (MXP.MXP_ReadOutputs(ref x, out y, out Data[0]) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    for (int i = 0; i < y.Size; i++)
                        rtData += Data[i] << (8 * i);
                    return (UInt32)rtData;
                }
            //}
            return 0;
        }
        public static int MXP_MC_SetPosition(UInt32 AxisNo, UInt32 MotionIdx, Single Position, MXP.MXP_EXECUTIONMODE_ENUM ExecutionMode, bool getY, MXP.MXP_SETPOSITION_OUT y)
        {
            MXP.MXP_SETPOSITION_IN x = new MXP.MXP_SETPOSITION_IN { };

            x.Axis.AxisNo = AxisNo;
            x.Execute = 0;
            x.Position = Position;
            x.ExecutionMode = ExecutionMode;

            //if (MXP_CheckMotionKernel(AxisNo) == true)  // check kernel is ready to receive command.
            //{
                if (getY)
                {
                    y = new MXP.MXP_SETPOSITION_OUT { };
                    if (MXP.MXP_GetSetPositionOutParam(MotionIdx, out y) == MXP.MXP_ret.RET_NO_ERROR)
                        return MXP.MXP_ret.RET_NO_ERROR;
                }
                else //if (MXP.MXP_SetPositionCmd(MotionIdx, ref x) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    MXP.MXP_SetPositionCmd(MotionIdx, ref x);
                    x.Execute = 1;
                    MXP.MXP_SetPositionCmd(MotionIdx, ref x);
                    return MXP.MXP_ret.RET_NO_ERROR;
                }
            //}
            return -1;
        }
        public static int MXP_MC_SetOverride(UInt32 AxisNo, UInt32 MotionIdx, Single VelFactor, Single AccFactor, Single JerkFactor, bool getY, MXP.MXP_SETOVERRIDE_OUT y)
        {
            MXP.MXP_SETOVERRIDE_IN x = new MXP.MXP_SETOVERRIDE_IN { };

            x.Axis.AxisNo = AxisNo;
            x.Enable = 1;
            x.VelFactor = VelFactor;
            x.AccFactor = AccFactor;
            x.JerkFactor = JerkFactor;

            //if (MXP_CheckMotionKernel(AxisNo) == true)  // check kernel is ready to receive command.
            //{
                if (getY)
                {
                    y = new MXP.MXP_SETOVERRIDE_OUT { };
                    if (MXP.MXP_GetSetOverrideOutParam(MotionIdx, out y) == MXP.MXP_ret.RET_NO_ERROR)
                        return MXP.MXP_ret.RET_NO_ERROR;
                }
                else if (MXP.MXP_SetOverrideCmd(MotionIdx, ref x) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    return MXP.MXP_ret.RET_NO_ERROR;
                }
            //}
            return -1;
        }

        public static int MXP_MC_GearIn(UInt32 MotionIdx, UInt16 MasterAxis, UInt16 SlaveAxis, UInt16 RatioDenominator, UInt16 RatioNumerator, Single Acceleration, Single Deceleration, Single Jerk, MXP.MXP_SOURCE_ENUM MasterValueSource, MXP.MXP_BUFFERMODE_ENUM BufferMode, bool getY, MXP.MXP_GEARIN_OUT y)
        {
            MXP.MXP_GEARIN_IN x = new MXP.MXP_GEARIN_IN { };

            x.Master.AxisNo = MasterAxis;
            x.Slave.AxisNo = SlaveAxis;
            x.Execute = 0;
            x.ContinuousUpdate = 0;
            x.RatioDenominator = RatioDenominator;
            x.RatioNumerator = RatioNumerator;
            x.MasterValueSource = MasterValueSource;
            x.Acceleration = Acceleration;
            x.Deceleration = Deceleration;
            x.Jerk = Jerk;
            x.BufferMode = BufferMode;

            //if (MXP_CheckMotionKernel(SlaveAxis) == true)  // check kernel is ready to receive command.
            //{
                if (getY)
                {
                    y = new MXP.MXP_GEARIN_OUT { };
                    if (MXP.MXP_GetGearInOutParam(MotionIdx, out y) != MXP.MXP_ret.RET_NO_ERROR)
                        return -1;
                }
                else //if (MXP.MXP_GearInCmd(MotionIdx, ref x) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    MXP.MXP_GearInCmd(MotionIdx, ref x);
                    x.Execute = 1;
                    MXP.MXP_GearInCmd(MotionIdx, ref x);
                    return MXP.MXP_ret.RET_NO_ERROR;
                }
            //}
            return -1;
        }
        public static int MXP_MC_GearOut(UInt32 SlaveAxis, UInt32 MotionIdx, bool getY, MXP.MXP_GEAROUT_OUT y)
        {
            MXP.MXP_GEAROUT_IN x = new MXP.MXP_GEAROUT_IN { };

            x.Slave.AxisNo = SlaveAxis;
            x.Execute = 0;

            //if (MXP_CheckMotionKernel(SlaveAxis) == true)  // check kernel is ready to receive command.
            //{
                if (getY)
                {
                    y = new MXP.MXP_GEAROUT_OUT { };
                    if (MXP.MXP_GetGearOutOutParam(MotionIdx, out y) != MXP.MXP_ret.RET_NO_ERROR)
                        return -1;
                }
                else //if (MXP.MXP_GearOutCmd(MotionIdx, ref x) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    MXP.MXP_GearOutCmd(MotionIdx, ref x);
                    x.Execute = 1;
                    MXP.MXP_GearOutCmd(MotionIdx, ref x);
                    return MXP.MXP_ret.RET_NO_ERROR;
                }
            //}
            return -1;
        }
        public static int MXP_MC_GearInPos(UInt32 MotionIdx, UInt16 MasterAxis, UInt16 SlaveAxis, UInt16 RatioDenominator, Int16 RatioNumerator, Single MasterStartDistance, Single MasterSyncPosition, Single Velocity, Single Acceleration, Single Deceleration, Single Jerk, bool getY, MXP.MXP_GEARINPOS_OUT y)
        {
            MXP.MXP_GEARINPOS_IN x = new MXP.MXP_GEARINPOS_IN { };

            x.Master.AxisNo = MasterAxis;
            x.Slave.AxisNo = SlaveAxis;
            x.Execute = 0;
            x.RatioDenominator = RatioDenominator;
            x.RatioNumerator = RatioNumerator;
            x.MasterStartDistance = MasterStartDistance;
            x.MasterSyncPosition = MasterSyncPosition;
            x.Velocity = Velocity;
            x.Acceleration = Acceleration;
            x.Deceleration = Deceleration;
            x.Jerk = Jerk;
            x.MasterValueSource = 0;
            x.BufferMode = 0;

            //if (MXP_CheckMotionKernel(SlaveAxis) == true)  // check kernel is ready to receive command.
            //{
                if (getY)
                {
                    y = new MXP.MXP_GEARINPOS_OUT { };
                    if (MXP.MXP_GetGearInPosOutParam(MotionIdx, out y) != MXP.MXP_ret.RET_NO_ERROR)
                        return -1;
                }
                else// if (MXP.MXP_GearInPosCmd(MotionIdx, ref x) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    MXP.MXP_GearInPosCmd(MotionIdx, ref x);
                    x.Execute = 1;
                    MXP.MXP_GearInPosCmd(MotionIdx, ref x);
                    return MXP.MXP_ret.RET_NO_ERROR;
                }
            //}
            return -1;
        }
        public static int MXP_MC_CamTableSelect(UInt32 CamTable, UInt32 MotionIdx, Byte Periodic, Byte MasterAbsolute, Byte SlaveAbsolute, bool getY, MXP.MXP_CAMTABLESELECT_OUT y)
        {
            MXP.MXP_CAMTABLESELECT_IN x = new MXP.MXP_CAMTABLESELECT_IN { };

            x.CamTable.CamTable = CamTable;
            x.Periodic = Periodic;
            x.MasterAbsolute = MasterAbsolute;
            x.SlaveAbsolute = SlaveAbsolute;
            x.ExecutionMode = 0;    // Not supports
            x.Execute = 0;

            //if (MXP_CheckMotionKernel(0) == true)  // check kernel is ready to receive command.
            //{
                if (getY)
                {
                    y = new MXP.MXP_CAMTABLESELECT_OUT { };
                    if (MXP.MXP_GetCamTableSelectOutParam(MotionIdx, out y) != MXP.MXP_ret.RET_NO_ERROR)
                        return -1;
                }
                //if (MXP.MXP_CamTableSelectCmd(MotionIdx, ref x) == MXP.MXP_ret.RET_NO_ERROR)
                //{
                    MXP.MXP_CamTableSelectCmd(MotionIdx, ref x);
                    x.Execute = 1;
                    MXP.MXP_CamTableSelectCmd(MotionIdx, ref x);
                    return MXP.MXP_ret.RET_NO_ERROR;
                //}
            //}
            //return -1;
        }
        public static int MXP_MC_CamIn(UInt32 MotionIdx, UInt16 MasterAxis, UInt16 SlaveAxis, Single MasterOffset, Single SlaveOffset, Single MasterStartDistance, Single MasterSyncPosition, Single MasterScaling, Single SlaveScaling, UInt16 CamTableId, MXP.MXP_BUFFERMODE_ENUM BufferMode, bool getY, MXP.MXP_CAMIN_OUT y)
        {
            MXP.MXP_CAMIN_IN x = new MXP.MXP_CAMIN_IN { };

            x.Master.AxisNo = MasterAxis;
            x.Slave.AxisNo = SlaveAxis;
            x.Execute = 0;
            x.MasterOffset = MasterOffset;
            x.SlaveOffset = SlaveOffset;
            x.MasterStartDistance = MasterStartDistance;
            x.MasterSyncPosition = MasterSyncPosition;
            x.MasterScaling = MasterScaling;
            x.SlaveScaling = SlaveScaling;
            x.StartMode = 0;
            x.MasterValueSource = 0;
            x.CamTableID = CamTableId;
            x.BufferMode = BufferMode;

            //if (MXP_CheckMotionKernel(SlaveAxis) == true)  // check kernel is ready to receive command.
            //{
                if (getY)
                {
                    y = new MXP.MXP_CAMIN_OUT { };
                    if (MXP.MXP_GetCamInOutParam(MotionIdx, out y) != MXP.MXP_ret.RET_NO_ERROR)
                        return -1;
                }
                else// if (MXP.MXP_CamInCmd(MotionIdx, ref x) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    MXP.MXP_CamInCmd(MotionIdx, ref x);
                    x.Execute = 1;
                    MXP.MXP_CamInCmd(MotionIdx, ref x);
                    return MXP.MXP_ret.RET_NO_ERROR;
                }
            //}       
            return -1;
        }
        public static int MXP_MC_CamOut(UInt32 MotionIdx, UInt16 SlaveAxis, bool getY, MXP.MXP_CAMOUT_OUT y)
        {
            MXP.MXP_CAMOUT_IN x = new MXP.MXP_CAMOUT_IN { };

            x.Slave.AxisNo = SlaveAxis;
            x.Execute = 0;

            //if (MXP_CheckMotionKernel(SlaveAxis) == true)  // check kernel is ready to receive command.
            //{
                if (getY)
                {
                    y = new MXP.MXP_CAMOUT_OUT { };
                    if (MXP.MXP_GetCamOutOutParam(MotionIdx, out y) != MXP.MXP_ret.RET_NO_ERROR)
                        return -1;
                }
                else// if (MXP.MXP_CamOutCmd(MotionIdx, ref x) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    MXP.MXP_CamOutCmd(MotionIdx, ref x);
                    x.Execute = 1;
                    MXP.MXP_CamOutCmd(MotionIdx, ref x);
                    return MXP.MXP_ret.RET_NO_ERROR;
                }
            //}  
            return -1;
        }

        public static int MXP_MC_MoveLinearAbsolute(UInt32 AxesGroup, UInt32 MotionIdx, Single PositionX, Single PositionY, Single PositionZ, Single PositionU, Single PositionV, Single PositionW, Single PositionA, Single PositionB, Single PositionC, Single Velocity, Single Acceleration, Single Deceleration, Single Jerk, MXP.MXP_BUFFERMODE_ENUM BufferMode, bool getY, MXP.MXP_MOVELINEARABSOLUTE_OUT y)
        {
            MXP.MXP_MOVELINEARABSOLUTE_IN x = new MXP.MXP_MOVELINEARABSOLUTE_IN { };

            x.AxesGroup.AxesGroupNo = AxesGroup;
            x.Execute = 0;
            x.PositionX = PositionX;
            x.PositionY = PositionY;
            x.PositionZ = PositionZ;
            x.PositionU = PositionU;
            x.PositionV = PositionV;
            x.PositionW = PositionW;
            x.PositionA = PositionA;
            x.PositionB = PositionB;
            x.PositionC = PositionC;
            x.Velocity = Velocity;
            x.Acceleration = Acceleration;
            x.Deceleration = Deceleration;
            x.Jerk = Jerk;
            x.BufferMode = BufferMode;
            x.CoordSystem = 0;

            // 특정 그룹번호의 축번호를 읽을 수 없어 사용자의 재량으로 한다.
            // 따라서 다른 함수에 비해 축 상태를 체크하는 부분이 없다.
            if (getY)
            {
                y = new MXP.MXP_MOVELINEARABSOLUTE_OUT { };
                if (MXP.MXP_GetMoveLinearAbsoluteOutParam(MotionIdx, out y) != MXP.MXP_ret.RET_NO_ERROR)
                    return -1;
            }
            else if (MXP.MXP_MoveLinearAbsoluteCmd(MotionIdx, ref x) == MXP.MXP_ret.RET_NO_ERROR)
            {

                x.Execute = 1;
                MXP.MXP_MoveLinearAbsoluteCmd(MotionIdx, ref x);
                return MXP.MXP_ret.RET_NO_ERROR;
            }
            
            return -1;
        }

        public static int MXP_MC_MoveLinearRelative(UInt16 AxesGroup, UInt32 MotionIdx, Single DistanceX, Single DistanceY, Single DistanceZ, Single DistanceU, Single DistanceV, Single DistanceW, Single DistanceA, Single DistanceB, Single DistanceC, Single Velocity, Single Acceleration, Single Deceleration, Single Jerk, MXP.MXP_BUFFERMODE_ENUM BufferMode, bool getY, MXP.MXP_MOVELINEARRELATIVE_OUT y)
        {
            MXP.MXP_MOVELINEARRELATIVE_IN x = new MXP.MXP_MOVELINEARRELATIVE_IN { };

            x.AxesGroup.AxesGroupNo = AxesGroup;
            x.Execute = 0;
            x.DistanceX = DistanceX;
            x.DistanceY = DistanceY;
            x.DistanceZ = DistanceZ;
            x.DistanceU = DistanceU;
            x.DistanceV = DistanceV;
            x.DistanceW = DistanceW;
            x.DistanceA = DistanceA;
            x.DistanceB = DistanceB;
            x.DistanceC = DistanceC;
            x.Velocity = Velocity;
            x.Acceleration = Acceleration;
            x.Deceleration = Deceleration;
            x.Jerk = Jerk;
            x.BufferMode = BufferMode;
            x.CoordSystem = 0;

            // 특정 그룹번호의 축번호를 읽을 수 없어 사용자의 재량으로 한다.
            // 따라서 다른 함수에 비해 축 상태를 체크하는 부분이 없다.
            if (getY)
            {
                y = new MXP.MXP_MOVELINEARRELATIVE_OUT { };
                if (MXP.MXP_GetMoveLinearRelativeOutParam(MotionIdx, out y) != MXP.MXP_ret.RET_NO_ERROR)
                    return -1;
            }
            else if (MXP.MXP_MoveLinearRelativeCmd(MotionIdx, ref x) == MXP.MXP_ret.RET_NO_ERROR)
            {
                x.Execute = 1;
                MXP.MXP_MoveLinearRelativeCmd(MotionIdx, ref x);
                return MXP.MXP_ret.RET_NO_ERROR;
            }

            return -1;
        }
        public static int MXP_MC_MoveCircularAbsolute(UInt16 AxesGroup, UInt32 MotionIdx, Single Point1, Single Point2, Single Point3, Single Point4,
                                                      Int32 Plane1, Int32 Plane2, MXP.MXP_PATHCHOICE_ENUM Path,
                                                      Single Velocity, Single Acceleration, Single Deceleration, Single Jerk, MXP.MXP_BUFFERMODE_ENUM BufferMode, bool getY, MXP.MXP_MOVECIRCULARABSOLUTE_OUT y)
        {
            MXP.MXP_MOVECIRCULARABSOLUTE_IN x = new MXP.MXP_MOVECIRCULARABSOLUTE_IN { };

            x.AxesGroup.AxesGroupNo = AxesGroup;
            x.Execute = 0;
            x.AuxPoint1 = Point1;
            x.AuxPoint2 = Point2;
            x.EndPoint1 = Point3;
            x.EndPoint2 = Point4;
            x.Plane1 = Plane1;
            x.Plane2 = Plane2;
            x.PathChoice = Path;
            x.Velocity = Velocity;
            x.Acceleration = Acceleration;
            x.Deceleration = Deceleration;
            x.Jerk = Jerk;
            x.CoordSystem = 0;
            x.BufferMode = BufferMode;

            // 특정 그룹번호의 축번호를 읽을 수 없어 사용자의 재량으로 한다.
            // 따라서 다른 함수에 비해 축 상태를 체크하는 부분이 없다.
            if (getY)
            {
                y = new MXP.MXP_MOVECIRCULARABSOLUTE_OUT { };
                if (MXP.MXP_GetMoveCircularAbsoluteOutParam(MotionIdx, out y) != MXP.MXP_ret.RET_NO_ERROR)
                    return -1;
            }
            else if (MXP.MXP_MoveCircularAbsoluteCmd(MotionIdx, ref x) == MXP.MXP_ret.RET_NO_ERROR)
            {
                x.Execute = 1;
                MXP.MXP_MoveCircularAbsoluteCmd(MotionIdx, ref x);
                return MXP.MXP_ret.RET_NO_ERROR;
            }

            return -1;
        }
        public static int MXP_MC_MoveCircularRelative(UInt16 AxesGroup, UInt32 MotionIdx, Single Point1, Single Point2, Single Point3, Single Point4,
                                                      Int32 Plane1, Int32 Plane2, MXP.MXP_PATHCHOICE_ENUM Path,
                                                      Single Velocity, Single Acceleration, Single Deceleration, Single Jerk, MXP.MXP_BUFFERMODE_ENUM BufferMode, bool getY, MXP.MXP_MOVECIRCULARRELATIVE_OUT y)
        {
            MXP.MXP_MOVECIRCULARRELATIVE_IN x = new MXP.MXP_MOVECIRCULARRELATIVE_IN { };

            x.AxesGroup.AxesGroupNo = AxesGroup;
            x.Execute = 0;
            x.AuxPoint1 = Point1;
            x.AuxPoint2 = Point2;
            x.EndPoint1 = Point3;
            x.EndPoint2 = Point4;
            x.Plane1 = Plane1;
            x.Plane2 = Plane2;
            x.Velocity = Velocity;
            x.PathChoice = Path;
            x.Acceleration = Acceleration;
            x.Deceleration = Deceleration;
            x.Jerk = Jerk;
            x.CoordSystem = 0;
            x.BufferMode = BufferMode;

            // 특정 그룹번호의 축번호를 읽을 수 없어 사용자의 재량으로 한다.
            // 따라서 다른 함수에 비해 축 상태를 체크하는 부분이 없다.
            if (getY)
            {
                y = new MXP.MXP_MOVECIRCULARRELATIVE_OUT { };
                if (MXP.MXP_GetMoveCircularRelativeOutParam(MotionIdx, out y) != MXP.MXP_ret.RET_NO_ERROR)
                    return -1;
            }
            else if (MXP.MXP_MoveCircularRelativeCmd(MotionIdx, ref x) == MXP.MXP_ret.RET_NO_ERROR)
            {
                x.Execute = 1;
                MXP.MXP_MoveCircularRelativeCmd(MotionIdx, ref x);
                return MXP.MXP_ret.RET_NO_ERROR;
            }

            return -1;
        }

        public static int MXP_MC_GroupStop(UInt16 GroupNo, UInt32 MotionIdx, Single Deceleration, Single Jerk, bool getY, MXP.MXP_GROUPSTOP_OUT y)
        {
            MXP.MXP_GROUPSTOP_IN x = new MXP.MXP_GROUPSTOP_IN { };

            x.AxesGroup.AxesGroupNo = GroupNo;
            x.Execute = 0;
            x.Deceleration = Deceleration;
            x.Jerk = Jerk;

            // 특정 그룹번호의 축번호를 읽을 수 없어 사용자의 재량으로 한다.
            // 따라서 다른 함수에 비해 축 상태를 체크하는 부분이 없다.
            if (getY)
            {
                                        Thread.Sleep(50);
                        y = new MXP.MXP_GROUPSTOP_OUT { };
                        if (MXP.MXP_GetGroupStopOutParam(MotionIdx, out y) == MXP.MXP_ret.RET_NO_ERROR)
                            return MXP.MXP_ret.RET_NO_ERROR;
                        return MXP.MXP_ret.RET_NO_ERROR;
            }
            else if (MXP.MXP_GroupStopCmd(MotionIdx, ref x) == MXP.MXP_ret.RET_NO_ERROR)
            {
                x.Execute = 1;
                MXP.MXP_GroupStopCmd(MotionIdx, ref x);
            }
            return -1;
        }

        public static Single MXP_MC_GroupReadActualVelocity(UInt32 GroupNo)
        {
            MXP.MXP_GROUPREADACTUALVELOCITY_IN x = new MXP.MXP_GROUPREADACTUALVELOCITY_IN { };
            MXP.MXP_GROUPREADACTUALVELOCITY_OUT y = new MXP.MXP_GROUPREADACTUALVELOCITY_OUT { };

            x.AxesGroup.AxesGroupNo = GroupNo;
            x.Enable = 1;
            x.CoordSystem = 0; // Not supports

            if (MXP.MXP_GroupReadActualVelocity(ref x, out y) == MXP.MXP_ret.RET_NO_ERROR)
            {
                return y.PathVelocity;
            }
            return -1;
        }
        public static Single MXP_MC_GroupReadActualPosition(UInt32 GroupNo, Single X, Single Y, Single Z, Single U, Single V, Single W, Single A, Single B, Single C)
        {
            MXP.MXP_GROUPREADACTUALPOSITION_IN x = new MXP.MXP_GROUPREADACTUALPOSITION_IN { };
            MXP.MXP_GROUPREADACTUALPOSITION_OUT y = new MXP.MXP_GROUPREADACTUALPOSITION_OUT { };

            x.CoordSystem = 0;
            x.AxesGroup.AxesGroupNo = GroupNo;

            if (MXP.MXP_GroupReadActualPosition(ref x, out y) == MXP.MXP_ret.RET_NO_ERROR)
            {
                X = y.PositionX;
                Y = y.PositionY;
                Z = y.PositionZ;
                U = y.PositionU;
                V = y.PositionV;
                W = y.PositionW;
                A = y.PositionA;
                B = y.PositionB;
                C = y.PositionC;

                return MXP.MXP_ret.RET_NO_ERROR;
            }
            return -1;
        }
        public static Single MXP_MC_GroupReadCommandVelocity(UInt16 GroupNo)
        {
            MXP.MXP_GROUPREADCOMMANDVELOCITY_IN x = new MXP.MXP_GROUPREADCOMMANDVELOCITY_IN { };
            MXP.MXP_GROUPREADCOMMANDVELOCITY_OUT y = new MXP.MXP_GROUPREADCOMMANDVELOCITY_OUT { };

            x.AxesGroup.AxesGroupNo = GroupNo;

            if (MXP.MXP_GroupReadCommandVelocity(ref x, out y) == MXP.MXP_ret.RET_NO_ERROR)
            {
                return y.PathCommandVelocity;
            }
            return -1;
        }

        public static int MXP_ET_ReadParameter(UInt32 SlaveNo, UInt32 MotionIdx, UInt32 Index, UInt32 SubIndex, UInt32 Length)
        {
            MXP.MXP_ET_READPARAMETER_IN x = new MXP.MXP_ET_READPARAMETER_IN { };
            MXP.MXP_ET_READPARAMETER_OUT y = new MXP.MXP_ET_READPARAMETER_OUT { };

            x.SlaveNo = SlaveNo;
            x.Index = Index;
            x.SubIndex = SubIndex;
            x.BufLen = Length;

            //if (MXP_CheckMotionKernel(SlaveNo) == true)  // check kernel is ready to receive command.
            //{
                if (MXP.MXP_ET_ReadParameterCmd(MotionIdx, ref x) == MXP.MXP_ret.RET_NO_ERROR)
                {
                    Thread.Sleep(100);
                    if (MXP.MXP_ET_GetReadParameterOutParam(MotionIdx, out y) != MXP.MXP_ret.RET_NO_ERROR)
                        return -1;

                    return (int)((y.Data[0]) | (y.Data[1] << 8) | (y.Data[2] << 16) | (y.Data[3] << 24));
                }
            //}
            return -1;
        }
        public static int MXP_ET_WriteParameter(UInt32 SlaveNo, UInt32 MotionIdx, UInt32 Index, UInt32 SubIndex, UInt32 Length, UInt32 Data)
        {
            MXP.MXP_ET_WRITEPARAMETER_IN x = new MXP.MXP_ET_WRITEPARAMETER_IN { };
            MXP.MXP_ET_WRITEPARAMETER_OUT y = new MXP.MXP_ET_WRITEPARAMETER_OUT { };

            x.SlaveNo = SlaveNo;
            x.Index = Index;
            x.SubIndex = SubIndex;
            x.BufLen = Length;
            x.Data = Data;

            //if (MXP_CheckMotionKernel(SlaveNo) == true)  // check kernel is ready to receive command.
            //{
            int ret = MXP.MXP_ET_WriteParameterCmd(MotionIdx, ref x);
                if (ret == MXP.MXP_ret.RET_NO_ERROR)
                {
                    Thread.Sleep(50);
                    if (MXP.MXP_ET_GetWriteParameterOutParam(MotionIdx, out y) != MXP.MXP_ret.RET_NO_ERROR)
                        return -1;

                    return MXP.MXP_ret.RET_NO_ERROR;
                }
            //}
            return -1;
        }
    }
}
