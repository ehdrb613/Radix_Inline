using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms; //HJ 수정 200406 메시지 창을 띄우기 위해(Ecat.cs)

namespace Radix
{
    /**
     * @brief 제어기 Master에 대한 기본 함수들을 선언
     *      EtherCat 타입에 대한 제어기만을 대상으로 하는 게 아니고 모든 제어기에 대해 선언할 것
     */
    public static class Ecat
    {
        /* Ecat.cs
            제어기 Master에 대한 기본 함수들을 선언
            MXP,RSA,Advantech,AXT 등에 따라 각각 선언해 둬야 함
        //*/

        //RSA NMC
        /** @brief RSA 제어기 모듈 ID. 사용하지 않는다 */
        public static ushort RSABoardID = 0; // RSA 모듈 ID 0부터
        /** @brief RSA 설정에서 축별 주소를 찾아 상수처리해야 함, 서보5 + IO 3 . 사용하지 않는다 */
        public static ushort[] RSAEcatAddr = { 0, 0, 0, 0, 0, 0, 0, 0 };  // RSA 설정에서 축별 주소를 찾아 상수처리해야 함, 서보5 + IO 3 

        /**
         * @brief class 내부콜용 로컬 debug 
         * @param 디버그 처리할 문자열
         * @return void
         */
        private static void debug(string str) // debug(문자열) class 내부콜용 로컬 debug 
        {
            //Util.Debug(str);
        }

        /**
         * @brief Ethercat Master 초기화 
         *      일회성으로 현재 상태에서 수행할 지령을 정의하며
         *      frmMain의 Shown 이벤트에서 제어기 초기화 루프를 만들어 콜한다.
         *      초기화 실패시는 빠른 처리를 위해 변경이 필요
         * @return bool 초기화 완료시 true
         *      초기화 실패 또는 초기화중이면 false
         */
        public static bool Init() // Ethercat Master 초기화 
        {
            bool ret = false;
            if (GlobalVar.Simulation)
            {
                // simulation
                return true;
            }
            switch (GlobalVar.MasterType)
            {
                case enumMasterType.MXP:
                default:
                    ret = Motion_Function.Init();
                    if (ret)
                    {
                        ServoOnAll(true);
                    }
                    break;
            }
            return ret;
        }
		/**
         * @brief 특정 서보 전원 On/Off 
         * @param axis 축 순번
         * @param on On 여부
         * @return void
         */
        public static bool ServoOn(uint axis, bool on) // ServoOn(축번호, On여부) 서보 전원 On/Off 
        {
            if (GlobalVar.Simulation)
            {
                // simulation
                /*
                GlobalVar.AxisStatus[axis].PowerOn = on;
                GlobalVar.AxisStatus[axis].StandStill = true;
                //*/
                return true;
            }
            switch (GlobalVar.MasterType)
            {
                case enumMasterType.MXP:
                    MXP.MXP_POWER_OUT powerOut = new MXP.MXP_POWER_OUT { };
                    return Motion_Function.MXP_MC_Power(axis, Motion_Function.IndexCal((UInt32)MXP.MXP_MotionBlockIndex.mcPower) + axis, Convert.ToByte(on), false, powerOut) == MXP.MXP_ret.RET_NO_ERROR;
                //break;
                case enumMasterType.MXN:
                    MXN.MXN_Power(1);
                    return true;
                case enumMasterType.AXL:
                    CAXM.AxmSignalServoOn(Convert.ToInt32(axis), Convert.ToUInt32(on));
                    return true;
                default:
                    return false;
            }
        }
 		/**
         * @brief 서보 전체 전원 On/Off 
         * @param on On 여부
         * @return void
         */
        public static void ServoOnAll(bool on) // ServoOnAll(On여부) 서보 전체 전원 On/Off 
        {
            if (GlobalVar.Simulation)
            {
                // simulation
                /*
                for (ushort axis = 0; axis < GlobalVar.Axis_count; axis++)
                {
                    GlobalVar.AxisStatus[axis].PowerOn = on;
                    GlobalVar.AxisStatus[axis].StandStill = true;
                }
                //*/
                return;
            }
            try
            {
                switch (GlobalVar.MasterType)
                {
                    case enumMasterType.MXP:
                        Motion_Function.ServoOnAll(Convert.ToByte(on));
                        break;
                    case enumMasterType.MXN:
                        MXN.MXN_Power(1);
                        break;
                    case enumMasterType.AXL:
                        for (int i = 0; i < GlobalVar.Axis_count; i++)
                        {
                            CAXM.AxmSignalServoOn(i, Convert.ToUInt32(on));
                            //sThread.Sleep(1000);
                        }

                        break;
                    default:
                        break;
                }
            }
            catch { }
        }
            
        /**
         * @brief 서보 에러 초기화 
         * @param axis 축 순번
         * @return void
         */
        public static void ServoReset(uint axis) // ServoReset(축번호) 서보 에러 초기화 
        {
            if (GlobalVar.Simulation)
            {
                // simulation
                /*
                GlobalVar.AxisStatus[axis].PowerOn = false;
                GlobalVar.AxisStatus[axis].Errored = false;
                GlobalVar.AxisStatus[axis].ErrorStop = false;
                //*/
                return;
            }
            switch (GlobalVar.MasterType)
            {
                case enumMasterType.MXP:
                    MXP.MXP_RESET_OUT resetOut = new MXP.MXP_RESET_OUT { };
                    Motion_Function.MXP_MC_Reset(axis, Motion_Function.IndexCal((UInt32)MXP.MXP_MotionBlockIndex.mcReset) + axis, false, resetOut);
                    break;
                case enumMasterType.MXN:
                    MXN.MXN_Write_X(MXN.REG_BIT, 210, 7, 7, 1);
                    Thread.Sleep(200);
                    MXN.MXN_Write_X(MXN.REG_BIT, 210, 7, 7, 0);
                    break;
                case enumMasterType.AXL:
                    uint a = CAXM.AxmSignalServoAlarmReset(Convert.ToInt32(axis), 1);
                    Thread.Sleep(500);
                    uint b = CAXM.AxmSignalServoAlarmReset(Convert.ToInt32(axis), 0);
                    break;
                default:
                    break;
            }
        }

        /**
         * @brief 모든 서보 에러 초기화 
         * @return void
         */
        public static void ServoReset_All()
        {
            for (uint axis = 0; axis < GlobalVar.Axis_count; axis++)
            {
                ServoReset(axis, 1);
            }
            Thread.Sleep(500);
            for (uint axis = 0; axis < GlobalVar.Axis_count; axis++)
            {
                ServoReset(axis, 0);
            }
        }

        /**
         * @brief 지정 서보 에러 초기화 
         * @param axis 서보 순번
         * @param on 리셋 지령 플래그
         *      MXP 경우 리셋 지령이 On 후 Off 해야 함
         * @return void
         */
        public static void ServoReset(uint axis, uint on) // ServoReset(축번호) 서보 에러 초기화 
        {
            if (GlobalVar.Simulation)
            {
                // simulation
                /*
                GlobalVar.AxisStatus[axis].PowerOn = false;
                GlobalVar.AxisStatus[axis].Errored = false;
                GlobalVar.AxisStatus[axis].ErrorStop = false;
                //*/
                return;
            }
            switch (GlobalVar.MasterType)
            {
                case enumMasterType.MXP:
                    if (on == 1)
                    {
                        MXP.MXP_RESET_OUT resetOut = new MXP.MXP_RESET_OUT { };
                        Motion_Function.MXP_MC_Reset(axis, Motion_Function.IndexCal((UInt32)MXP.MXP_MotionBlockIndex.mcReset) + axis, false, resetOut);
                    }
                    break;
                case enumMasterType.MXN:
                    MXN.MXN_Write_X(MXN.REG_BIT, 210, 7, 7, on);
                    break;
                case enumMasterType.AXL:
                    uint a = CAXM.AxmSignalServoAlarmReset(Convert.ToInt32(axis), on);
                    break;
                default:
                    break;
            }
        }

        /**
         * @brief 서보 Home 찾기 
         * @param axis 서보 순번
         * @return bool 지령 수행완료 여부. 홈찾기 완료 여부와 무관
         *      지령 수행완료시 true
         *      지령 실패시 false
         */
        public static bool MoveHome(uint axis) // MoveHome(축번호, 완료대기, 대기시간) 서보 Home 찾기 
        {
            return MoveHome(axis, false, 0);
        }

        /**
         * @brief 서보 Home 찾기 
         * @param axis 서보 순번
         * @param WaitDone Home 찾기 완료 여부 체크 여부
         * @param Timeout Home 찾기 완료 여부 체크시 체크 시간
         * @return bool 지령 수행완료 여부.
         *      WaitDone이 false 경우 홈찾기 완료 여부와 무관
         *      WaitDone이 false 경우 지령 수행완료시 true
         *      WaitDone이 false 경우 지령 실패시 false
         *      WaitDone이 true 경우 홈찾기 완료 여부 리턴
         *      WaitDone이 true 경우 홈찾기 수행완료시 true
         *      WaitDone이 true 경우 홈찾기 실패시 false
         */
        public static bool MoveHome(uint axis, bool WaitDone, uint Timeout) // MoveHome(축번호, 완료대기, 대기시간) 서보 Home 찾기 
        {
            debug("MoveHome : " + axis.ToString());
            if (GlobalVar.Simulation)
            {
                // simulation
                /*
                GlobalVar.AxisStatus[axis].HomeAbsSwitch = true;
                GlobalVar.AxisStatus[axis].isHomed = true;
                GlobalVar.AxisStatus[axis].StandStill = true;
                GlobalVar.AxisStatus[axis].Position = 0;
                GlobalVar.AxisStatus[axis].Velocity = 0;
                //*/
                return true;
            }

            //////////////////////////////////////////////////////////////////////
            switch (GlobalVar.MasterType)
            {
                #region MXP
                case enumMasterType.MXP:
                    MXP.MXP_HOME_OUT Out = new MXP.MXP_HOME_OUT { };
                    Motion_Function.MXP_MC_Home(axis,
                                                    Motion_Function.IndexCal((UInt32)MXP.MXP_MotionBlockIndex.mcHome) + axis,
                                                    MXP.MXP_BUFFERMODE_ENUM.MXP_ABORTING,
                                                    false,
                                                    Out,
                                                    WaitDone,
                                                    Timeout);
                    break;
                #endregion
                #region MXN
                case enumMasterType.MXN:

                    //if(GlobalVar.AxisStatus[axis].isHomed)
                    //{
                    //    break;                       
                    //}

                    MXN.MXN_Write_R(MXN.REG_BIT, 700, 5, 5, 1);
                    Thread.Sleep(200);

                    MXN.MXN_Write_R(MXN.REG_BIT, 701, (int)axis, (int)axis, 1);
                    Thread.Sleep(200);

                    if (axis == (uint)enumServoAxis.PCB_Width || axis == (uint)enumServoAxis.BoxOutPut_Push)
                    {//+ 방향으로 홈 잡음(특정 Servo)
                        MXN.MXN_Write_X(MXN.REG_BIT, 212, 21, 21, 1);
                    }
                    else
                    {//-방향으로 홈 잡음
                        MXN.MXN_Write_X(MXN.REG_BIT, 212, 20, 20, 1);
                    }
                    Thread.Sleep(GlobalVar.Home_Command_After_Sleep);

                    break;
                #endregion
                #region AXL
                case enumMasterType.AXL:

                    #region 안전 확인
                    FuncLog.WriteLog("홈 찾기 지령 콜 - " + axis);
                    if (GlobalVar.AxisStatus[(int)axis].Homing)
                    {
                        return false;
                    }
                    #endregion

                    //메쏘드 지정
                    uint duRetCode = 0;
                    int iHomeDir = 0;// HmDir(홈 방향): DIR_CCW (0) -방향 , DIR_CW(1) +방향
                    uint duHomeSignal = 4, duZPhaseUse = 0;
                    // HmSig : PosEndLimit(0) -> +Limit
                    //         NegEndLimit(1) -> -Limit
                    //         HomeSensor (4) -> 원점센서(범용 입력 0)
                    // uZphas: 1차 원점검색 완료 후 엔코더 Z상 검출 유무 설정  0: 사용안함 , 1: Hmdir과 반대 방향, 2: Hmdir과 같은 방향
                    double dHomeClrTime = 0.0, dHomeOffset = 0.0;
                    // HClrTim : HomeClear Time : 원점 검색 Encoder 값 Set하기 위한 대기시간         
                    // HOffset - 원점검출후 이동거리.     

                    //if (axis == 2)
                    //{
                    //    iHomeDir = 0;
                    //}

                    FuncLog.WriteLog("홈 찾기 지령 콜 - " + axis);
                    //++ 지정한 축의 원점검색 방법을 변경합니다.
                    duRetCode = CAXM.AxmHomeSetMethod((int)axis, iHomeDir, duHomeSignal, duZPhaseUse, dHomeClrTime, dHomeOffset);
                    if (duRetCode != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                        FuncWin.TopMessageBox(String.Format("AxmHomeSetMethod return error[Code:{0:d}]" + axis + " 축", duRetCode));

                    //속도 지정
                    double dVelFirst, dVelSecond, dVelThird, dVelLast, dAccFirst, dAccSecond;

                    // 각각의 Edit 콘트롤에서 설정값을 가져옴
                    
                    //MGG 서보홈 속도* 5  / 230720
                    if (axis == 0 || axis == 1 ||
                        axis == 3 || axis == 4 )
                    {
                        dVelFirst = 100000 * 5;//기능 검사 기준 값
                    }
                    else
                    {
                        dVelFirst = 100000;//기능 검사 기준 값
                    }                   

                    dVelFirst = dVelFirst * 10;

                    dVelSecond = dVelFirst / 2;
                    dVelThird = dVelSecond / 2;
                    dVelLast = dVelThird / 2;
                    dAccFirst = dVelFirst * 100;
                    dAccSecond = dVelFirst * 10;

                    //++ 원점검색에 사용되는 단계별 속도를 설정합니다.
                    duRetCode = CAXM.AxmHomeSetVel((int)axis, dVelFirst, dVelSecond, dVelThird, dVelLast, dAccFirst, dAccSecond);
                    if (duRetCode != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                        FuncWin.TopMessageBox(String.Format("AxmHomeSetVel return error[Code:{0:d}]" + axis + " 축", duRetCode));

                    //원점 검색 시작
                    CAXM.AxmHomeSetStart(Convert.ToInt32(axis));
                    Thread.Sleep(1000);
                    break;
                #endregion
                #region ADVANTECH
                case enumMasterType.ADVANTECH:
                    break;
                #endregion
                default:
                    break;
            }
            if (WaitDone)
            {
                //*
                int chkTime = Environment.TickCount;
                while (Environment.TickCount - chkTime < Timeout)
                {
                    if (GlobalVar.MasterType == enumMasterType.MXN)
                    {
                        if (GlobalVar.AxisStatus[axis].isHomed)
                        {

                            MXN.MXN_Write_R(MXN.REG_BIT, 701, (int)axis, (int)axis, 0);
                            if (axis == (uint)enumServoAxis.PCB_Width || axis == (uint)enumServoAxis.BoxOutPut_Push)
                            {//+ 방향으로 홈 잡음(특정 Servo)
                                MXN.MXN_Write_X(MXN.REG_BIT, 212, 21, 21, 0);
                            }
                            else
                            {//-방향으로 홈 잡음
                                MXN.MXN_Write_X(MXN.REG_BIT, 212, 20, 20, 0);
                            }

                            //메뉴얼, 모델 단독 Homeing 할때만 원점복귀모드 해제 by DG 20220801
                            if (GlobalVar.TabMain == enumTabMain.Manual ||
                                GlobalVar.TabMain == enumTabMain.Model)
                            {
                                MXN.MXN_Write_R(MXN.REG_BIT, 700, 5, 5, 0);
                            }
                            debug("servo " + axis.ToString() + " Home Stop");
                            Ecat.MoveStop((int)axis);

                            if (HomeOffsetMove(axis))
                            {
                                return true;
                            }
                            else
                            {
                                GlobalVar.HomeOffsetCanNotMove = true;
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if (GlobalVar.AxisStatus[axis].isHomed)
                        {
                            debug("servo " + axis.ToString() + " Home Stop");
                            Ecat.MoveStop((int)axis);

                            if (HomeOffsetMove(axis))
                            {
                                return true;
                            }
                        }
                    }
                    Thread.Sleep(100);
                }
                //*/                
            }
            else
            {
                int chkTime = Environment.TickCount;
                while (Environment.TickCount - chkTime < 5 * 1000)
                {
                    #region MXN
                    if (GlobalVar.MasterType == enumMasterType.MXN)
                    {
                        if (GlobalVar.AxisStatus[axis].isHomed)
                        {

                            MXN.MXN_Write_R(MXN.REG_BIT, 701, (int)axis, (int)axis, 0);
                            if (axis == (uint)enumServoAxis.PCB_Width || axis == (uint)enumServoAxis.BoxOutPut_Push)
                            {//+ 방향으로 홈 잡음(특정 Servo)
                                MXN.MXN_Write_X(MXN.REG_BIT, 212, 21, 21, 0);
                            }
                            else
                            {//-방향으로 홈 잡음
                                MXN.MXN_Write_X(MXN.REG_BIT, 212, 20, 20, 0);
                            }
                            //MXN.MXN_Write_R(MXN.REG_BIT, 700, 5, 5, 0);
                            debug("servo " + axis.ToString() + " Home Stop");
                            Ecat.MoveStop((int)axis);
                            Thread.Sleep(GlobalVar.Home_Command_After_Sleep);
                            if (HomeOffsetMove(axis))
                            {
                                return true;
                            }
                        }
                    }
                    #endregion
                    #region AXL
                    else if (GlobalVar.MasterType == enumMasterType.AXL)
                    {
                        //if (GlobalVar.AxisStatus[axis].isHomed)
                        //{
                        //    debug("servo " + axis.ToString() + " Home Stop");
                        //    Ecat.MoveStop((int)axis);

                        //    Thread.Sleep(1000);                            
                        //}
                    }
                    #endregion
                    else
                    {
                        if (!GlobalVar.AxisStatus[axis].isHomed)
                        {
                            debug("servo " + axis.ToString() + " Home Stop");
                            Ecat.MoveStop((int)axis);

                            Thread.Sleep(1000);

                            if (HomeOffsetMove(axis))
                            {
                                return true;
                            }
                        }
                    }
                    Thread.Sleep(100);
                }

                //Thread.Sleep(5000);
                //Ecat.MoveStop((int)axis);
                //HomeOffsetMove(axis);
                //return true;               

            }
            // homoing 후 축선택, 원점복귀모드, 조그 신호 초기화 By DG 220725
            if (GlobalVar.MasterType == enumMasterType.MXN)
            {


                MXN.MXN_Write_R(MXN.REG_BIT, 701, (int)axis, (int)axis, 0);
                if (axis == (uint)enumServoAxis.PCB_Width || axis == (uint)enumServoAxis.BoxOutPut_Push)
                {//+ 방향으로 홈 잡음(특정 Servo)
                    MXN.MXN_Write_X(MXN.REG_BIT, 212, 21, 21, 0);
                }
                else
                {//-방향으로 홈 잡음
                    MXN.MXN_Write_X(MXN.REG_BIT, 212, 20, 20, 0);
                }
                // MXN.MXN_Write_R(MXN.REG_BIT, 700, 5, 5, 0);
            }
            return false;
        }

        /**
         * @brief 서보 절대위치 이동 
         *      지령은 실제 거리 또는 속도값으로 실행하고
         *      동작시 pulse 환산하여 실제 지령을 서보드라이브에 전달한다.
         *      가감속 및 Jerk값은 즉시 응답에 준하는 값으로 내정됨
         * @param axis 서보 순번
         * @param Pos 이동할 위치값 mm
         * @param Vel 이동할 속도 mm/s
         * @return void
         */
        public static void MoveAbsolute(uint axis, double Pos, double Vel) // MoveAbsolute(축번호, 좌표, 속도) 서보 절대위치 이동  
        {
            MoveAbsolute(axis, Pos, Vel, Vel * 10, Vel * 10, Vel * 100, false, 0);
        }

        /**
         * @brief 서보 절대위치 이동 
         *      지령은 실제 거리 또는 속도값으로 실행하고
         *      동작시 pulse 환산하여 실제 지령을 서보드라이브에 전달한다.
         * @param axis 서보 순번
         * @param Pos 이동할 위치값 mm
         * @param Vel 이동할 속도 mm/s
         * @param Acc 가속값
         * @param Dec 감속값
         * @param Jerk Jerk값
         * @param WaitDone 이동 완료 체크 여부
         * @param Timeout 이동 완료 체크시 체크 시간
         * @return void
         */
        public static void MoveAbsolute(uint axis, double Pos, double Vel, double Acc, double Dec, double Jerk, bool WaitDone, uint Timeout) // MoveAbsolute(축번호, 좌표, 속도, 가속, 감속, 저크, 완료대기, 대기시간) 서보 절대위치 이동  
        {
            debug("MoveAbsolute(" + axis.ToString() + "," + Pos.ToString() + "," + Vel.ToString() + "," + Acc.ToString() + "," + Dec.ToString() + "," + Jerk.ToString() + "," + WaitDone.ToString() + "," + Timeout.ToString() + ",");
            if (GlobalVar.Simulation)
            {
                // simulation
                debug("MoveAbsoute : " + axis.ToString() + " - " + Pos.ToString());
                Console.WriteLine("MoveAbsoute : " + axis.ToString() + " - " + Pos.ToString());
                //*
                GlobalVar.AxisStatus[axis].HomeAbsSwitch = false;
                GlobalVar.AxisStatus[axis].StandStill = true;
                GlobalVar.AxisStatus[axis].Position = Pos;
                GlobalVar.AxisStatus[axis].Velocity = 0;
                //*/
                return;
            }
            double pos = FuncMotion.GetRealPulse((int)axis, Pos);
            double vel = FuncMotion.GetRealPulse((int)axis, Vel);
            double acc = Acc == 0 ? vel * 10 : FuncMotion.GetRealSpeed((int)axis, Acc);
            double dec = Dec == 0 ? vel * 10 : FuncMotion.GetRealSpeed((int)axis, Dec);
            double jerk = Jerk == 0 ? acc * 10 : FuncMotion.GetRealSpeed((int)axis, Jerk);

            switch (GlobalVar.MasterType)
            {
                #region MXP
                case enumMasterType.MXP:
                    MXP.MXP_MOVEABSOLUTE_OUT Out = new MXP.MXP_MOVEABSOLUTE_OUT { };
                    Motion_Function.MXP_MC_MoveAbsolute(axis,
                                                    Motion_Function.IndexCal((UInt32)MXP.MXP_MotionBlockIndex.mcMoveAbsolute) + axis,
                                                    (float)vel,
                                                    (float)pos,
                                                    (float)acc,
                                                    (float)dec,
                                                    (float)jerk,
                                                    MXP.MXP_BUFFERMODE_ENUM.MXP_ABORTING,
                                                    MXP.MXP_DIRECTION_ENUM.MXP_POSITIVE_DIRECTION,
                                                    false,
                                                    Out,
                                                    WaitDone,
                                                    Timeout);
                    break;
                #endregion
                #region MXN
                case enumMasterType.MXN:
                    MXN.MXN_MOVEABSOLUTE_IN InParam;
                    InParam.uiAxisNo = axis + 1;
                    InParam.iVelocity = (int)vel * 10;      //300000 리프트 기준 느림 by DG
                    if (axis == (uint)enumServoAxis.BaseRobot_T)
                    {
                        InParam.iVelocity = (int)vel;
                    }
                    InParam.iPosition = (int)pos;           //10000 = 10mm by DG

                    if (axis == (uint)enumServoAxis.PCB_Width ||
                        axis == (uint)enumServoAxis.BoxOutPut_Push)
                    {
                        InParam.iPosition = -(int)pos;
                    }

                    MXN.MXN_MoveAbsolute(ref InParam);
                    break;
                #endregion
                case enumMasterType.AXL:
                    uint duRetCode = 0;

                    //FuncLog.WriteLog("무브 지령 콜 - " + axis);

                    //++ 지정 축의 구동 좌표계를 설정합니다. 
                    // dwAbsRelMode : (0)POS_ABS_MODE - 현재 위치와 상관없이 지정한 위치로 절대좌표 이동합니다.
                    //                (1)POS_REL_MODE - 현재 위치에서 지정한 양만큼 상대좌표 이동합니다.
                    duRetCode = CAXM.AxmMotSetAbsRelMode((int)axis, 0);

                    //++ 지정한 축을 지정한 거리(또는 위치)/속도/가속도/감속도로 모션구동하고 모션 종료여부와 상관없이 함수를 빠져나옵니다.
                    Console.WriteLine("MoveAbsolute : " + axis + " - " + pos);
                    if (axis == 1)
                    {
                        FuncLog.WriteLog("MoveABCheck - " + pos + "Receive -" + Pos);
                    }
                    duRetCode = CAXM.AxmMoveStartPos((int)axis, pos, vel, acc, dec);
                    if (duRetCode != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                        //MoveStop(0);
                        FuncWin.TopMessageBox(String.Format("AxmMoveStartPos return error[Code:{0:d}] " + axis + " 시간 " + DateTime.Now.ToString("hh:mm:ss"), duRetCode));

                    break;
                default:
                    break;

            }
        }

        /**
         * @brief 서보 상대위치 이동 
         *      지령은 실제 거리 또는 속도값으로 실행하고
         *      동작시 pulse 환산하여 실제 지령을 서보드라이브에 전달한다.
         *      가감속 및 Jerk값은 즉시 응답에 준하는 값으로 내정됨
         * @param axis 서보 순번
         * @param Pos 이동할 위치값 mm
         * @param Vel 이동할 속도 mm/s
         * @return void
         */
        public static void MoveRelative(uint axis, double Pos, double Vel) // MoveAbsolute(축번호, 좌표, 속도) 서보 절대위치 이동  
        {
            MoveRelative(axis, Pos, Vel, Vel * 10, Vel * 10, Vel * 100, false, 0);
        }
        /**
         * @brief 서보 상대위치 이동 
         *      지령은 실제 거리 또는 속도값으로 실행하고
         *      동작시 pulse 환산하여 실제 지령을 서보드라이브에 전달한다.
         * @param axis 서보 순번
         * @param Pos 이동할 위치값 mm
         * @param Vel 이동할 속도 mm/s
         * @param Acc 가속값
         * @param Dec 감속값
         * @param Jerk Jerk값
         * @param WaitDone 이동 완료 체크 여부
         * @param Timeout 이동 완료 체크시 체크 시간
         * @return void
         */
        public static void MoveRelative(uint axis, double Pos, double Vel, double Acc, double Dec, double Jerk, bool WaitDone, uint Timeout) // MoveAbsolute(축번호, 좌표, 속도, 가속, 감속, 저크, 완료대기, 대기시간) 서보 절대위치 이동  
        {
            debug("MoveRelative(" + axis.ToString() + "," + Pos.ToString() + "," + Vel.ToString() + "," + Acc.ToString() + "," + Dec.ToString() + "," + Jerk.ToString() + "," + WaitDone.ToString() + "," + Timeout.ToString() + ",");
            if (GlobalVar.Simulation)
            {
                // simulation
                debug("MoveRelative : " + axis.ToString() + " - " + Pos.ToString());
                Console.WriteLine("MoveRelative : " + axis.ToString() + " - " + Pos.ToString());
                /*
                GlobalVar.AxisStatus[axis].HomeAbsSwitch = false;
                GlobalVar.AxisStatus[axis].StandStill = true;
                GlobalVar.AxisStatus[axis].Position = Pos;
                GlobalVar.AxisStatus[axis].Velocity = 0;
                //*/
                return;
            }
            double pos = FuncMotion.GetRealPulse((int)axis, Pos);
            double vel = FuncMotion.GetRealSpeed((int)axis, Vel);
            double acc = Acc == 0 ? vel * 10 : FuncMotion.GetRealSpeed((int)axis, Acc);
            double dec = Dec == 0 ? vel * 10 : FuncMotion.GetRealSpeed((int)axis, Dec);
            double jerk = Jerk == 0 ? acc * 10 : FuncMotion.GetRealSpeed((int)axis, Jerk);

            switch (GlobalVar.MasterType)
            {
                #region MXP
                case enumMasterType.MXP:// 사용 할 때 수정 해야 한다.
                    MXP.MXP_MOVEABSOLUTE_OUT Out = new MXP.MXP_MOVEABSOLUTE_OUT { };
                    Motion_Function.MXP_MC_MoveAbsolute(axis,
                                                    Motion_Function.IndexCal((UInt32)MXP.MXP_MotionBlockIndex.mcMoveAbsolute) + axis,
                                                    (float)vel,
                                                    (float)pos,
                                                    (float)acc,
                                                    (float)dec,
                                                    (float)jerk,
                                                    MXP.MXP_BUFFERMODE_ENUM.MXP_ABORTING,
                                                    MXP.MXP_DIRECTION_ENUM.MXP_POSITIVE_DIRECTION,
                                                    false,
                                                    Out,
                                                    WaitDone,
                                                    Timeout);
                    break;
                #endregion
                #region MXN
                case enumMasterType.MXN:
                    MXN.MXN_MOVERELATIVE_IN InParam;
                    InParam.uiAxisNo = axis + 1;
                    InParam.iVelocity = (int)vel;
                    InParam.iDistance = (int)pos;

                    MXN.MXN_MoveRelative(ref InParam);
                    break;
                #endregion
                #region AXL
                case enumMasterType.AXL:
                    uint duRetCode = 0;

                    //++ 지정 축의 구동 좌표계를 설정합니다. 
                    // dwAbsRelMode : (0)POS_ABS_MODE - 현재 위치와 상관없이 지정한 위치로 절대좌표 이동합니다.
                    //                (1)POS_REL_MODE - 현재 위치에서 지정한 양만큼 상대좌표 이동합니다.
                    duRetCode = CAXM.AxmMotSetAbsRelMode((int)axis, 1);

                    //++ 지정한 축을 지정한 거리(또는 위치)/속도/가속도/감속도로 모션구동하고 모션 종료여부와 상관없이 함수를 빠져나옵니다.
                    duRetCode = CAXM.AxmMoveStartPos((int)axis, pos, vel, acc, dec);
                    if (duRetCode != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                        //MoveStop(0);
                        FuncWin.TopMessageBox(String.Format("AxmMoveStartPos return error[Code:{0:d}]", duRetCode));

                    break;
                #endregion
                default:
                    break;

            }
        }

        /**
         * @brief 서보 속도모드 구동
         *      지령은 실제 속도값으로 실행하고
         *      동작시 pulse 환산하여 실제 지령을 서보드라이브에 전달한다.
         *      가감속 및 Jerk값은 즉시 응답에 준하는 값으로 내정됨
         * @param axis 서보 순번
         * @param Pos 이동할 위치값 mm
         * @param Vel 이동할 속도 mm/s
         * @return void
         */
        public static void MoveVelocity(uint axis, double Vel) // 정속 운동
        {
            MoveVelocity(axis, Vel, Vel * 10, Vel * 10, Vel * 10);
        }

        /**
         * @brief 서보 속도모드 구동
         *      지령은 실제 속도값으로 실행하고
         *      동작시 pulse 환산하여 실제 지령을 서보드라이브에 전달한다.
         * @param axis 서보 순번
         * @param Pos 이동할 위치값 mm
         * @param Vel 이동할 속도 mm/s
         * @param Acc 가속값
         * @param Dec 감속값
         * @param Jerk Jerk값
         * @param WaitDone 이동 완료 체크 여부
         * @param Timeout 이동 완료 체크시 체크 시간
         * @return void
         */
        public static void MoveVelocity(uint axis, double Vel, double Acc, double Dec, double Jerk) // MoveVelocity(축번호, 속도, 가속, 감속, 저크) 서보 속도모드 구동 
        {
            debug("MoveVelocity : " + axis.ToString() + " - " + Vel.ToString());
            if (GlobalVar.Simulation)
            {
                // simulation
                debug("MoveVelocity : " + axis.ToString() + " - " + Vel.ToString());
                /*
                GlobalVar.AxisStatus[axis].HomeAbsSwitch = false;
                GlobalVar.AxisStatus[axis].StandStill = false;
                GlobalVar.AxisStatus[axis].Velocity = Vel;
                    // 역방향 - 홈으로
                    if (Vel < 0)
                    {
                        GlobalVar.AxisStatus[axis].StandStill = true;
                        GlobalVar.AxisStatus[axis].HomeAbsSwitch = true;
                        GlobalVar.AxisStatus[axis].Velocity = 0;
                        GlobalVar.AxisStatus[axis].Position = 0;
                    }
                    else
                    if (Vel > 0)
                    {
                    }
                    //*/
                return;
            }


            if (Vel < 0)
            {
                /*
                if (GlobalVar.AxisStatus[axis].LimitSwitchNeg)
                {
                    debug("역방향 구동 금지 " + axis.ToString());
                    MoveStop((int)axis);
                    return;
                }
                //*/
            }
            else
            if (Vel > 0)
            {
                /*
                if (GlobalVar.AxisStatus[axis].LimitSwitchPos)
                {
                    debug("정방향 구동 금지 " + axis.ToString());
                    MoveStop((int)axis);
                    return;
                }
                //*/
            }

            double acc = Acc == 0 ? Vel * 10 : Acc;
            double dec = Dec == 0 ? Vel * 10 : Dec;
            double jerk = Jerk == 0 ? acc * 10 : Jerk;
            switch (GlobalVar.MasterType)
            {
                case enumMasterType.MXP:
                    MXP.MXP_MOVEVELOCITY_IN x = new MXP.MXP_MOVEVELOCITY_IN { };

                    x.Axis.AxisNo = axis;
                    x.Execute = 0;
                    x.ContinuousUpdate = 1;
                    x.Velocity = (float)Math.Abs(Vel);
                    x.Acceleration = (float)Math.Abs(acc);
                    x.Deceleration = (float)Math.Abs(dec);
                    x.Jerk = (float)Math.Abs(jerk);
                    x.Direction = Vel >= 0 ? MXP.MXP_DIRECTION_ENUM.MXP_POSITIVE_DIRECTION : MXP.MXP_DIRECTION_ENUM.MXP_NEGATIVE_DIRECTION;
                    x.BufferMode = MXP.MXP_BUFFERMODE_ENUM.MXP_ABORTING;

                    //if (MXP.Motion_Function.MXP_CheckMotionKernel(axis) == true)  // check kernel is ready to receive command.
                    //{

                    MXP.MXP_MoveVelocityCmd(Motion_Function.IndexCal((UInt32)MXP.MXP_MotionBlockIndex.mcMoveVelocity) + axis, ref x);
                    x.Execute = 1;
                    MXP.MXP_MoveVelocityCmd(Motion_Function.IndexCal((UInt32)MXP.MXP_MotionBlockIndex.mcMoveVelocity) + axis, ref x);
                    //}
                    break;
                case enumMasterType.AXL:
                    //uint duRetCode = 0;
                    ////++ 지정한 축을 (+)방향으로 지정한 속도/가속도/감속도로 모션구동합니다.
                    //duRetCode = CAXM.AxmMoveVel((int)axis, Vel * GlobalVar.ServoSpeed_AXT, Math.Abs(acc * GlobalVar.ServoSpeed_AXT), Math.Abs(dec * GlobalVar.ServoSpeed_AXT));
                    //if (duRetCode != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                    //    Util.TopMessageBox(String.Format("AxmMoveVel return error[Code:{0:d}]", duRetCode));
                    break;
                default:
                    break;
            }
        }

        /**
         * @brief 서보 2축 보간구동
         *      지령은 실제 속도값으로 실행하고
         *      동작시 pulse 환산하여 실제 지령을 서보드라이브에 전달한다.
         * @param axis_1 서보1 순번
         * @param POS_1 서보1 이동할 위치값 mm
         * @param axis_1 서보2 순번
         * @param POS_1 서보2 이동할 위치값 mm
         * @param Vel 이동할 속도 mm/s
         * @param boganNum 보간 세팅 번호
         * @return void
         */
        public static void Double_Axis_Move(int axis_1, double POS_1, int axis_2, double POS_2, double Vel, int boganNum) // 보간구동
        {
            uint lSize = 2;
            int[] lAxesNo = { axis_1, axis_2 };
            POS_1 = FuncMotion.GetRealPulse((int)axis_1, POS_1);
            POS_2 = FuncMotion.GetRealPulse((int)axis_2, POS_2);
            double[] dPosition = { POS_1, POS_2 };
            double dMaxVelocity = FuncMotion.GetRealPulse((int)axis_1, Vel); //100;
            double dMaxAccel = dMaxVelocity * 5;//200
            double dMaxDecel = dMaxVelocity * 5;//200
            int lCoordinate = boganNum;

            uint uAbsRelMode = 0;
            // 지정 축의 이동 거리 계산 모드를 설정한다.
            // uAbsRelMode : POS_ABS_MODE '0' - 절대 좌표계
            //               POS_REL_MODE '1' - 상대 좌표계

            uint uProfileMode = 3;
            // 지정 축의 구동 속도 프로파일 모드를 설정한다.
            // ProfileMode : SYM_TRAPEZOIDE_MODE    '0' - 대칭 Trapezode
            //               ASYM_TRAPEZOIDE_MODE   '1' - 비대칭 Trapezode
            //               QUASI_S_CURVE_MODE     '2' - 대칭 Quasi-S Curve
            //               SYM_S_CURVE_MODE       '3' - 대칭 S Curve
            //               ASYM_S_CURVE_MODE      '4' - 비대칭 S Curve
            //               SYM_TRAP_M3_SW_MODE    '5' - 대칭 Trapezode : MLIII 내부 S/W Profile
            //               ASYM_TRAP_M3_SW_MODE   '6' - 비대칭 Trapezode : MLIII 내부 S/W Profile
            //               SYM_S_M3_SW_MODE       '7' - 대칭 S Curve : MLIII 내부 S/W Profile
            //               ASYM_S_M3_SW_MODE      '8' - asymmetric S Curve : MLIII 내부 S/W Profile

            // 지정된 좌표계에 연속 보간 구동을 위해 저장된 내부 Queue를 모두 삭제하는 함수이다.
            CAXM.AxmContiWriteClear(lCoordinate);

            // 지정된 좌표계에 연속보간 축 맵핑을 설정한다.
            // (축맵핑 번호는 0 부터 시작))
            // 주의점: 축맵핑할때는 반드시 실제 축번호가 작은 숫자부터 큰숫자를 넣는다.
            //         가상축 맵핑 함수를 사용하였을 때 가상축번호를 실제 축번호가 작은 값 부터 lpAxesNo의 낮은 인텍스에 입력하여야 한다.
            //         가상축 맵핑 함수를 사용하였을 때 가상축번호에 해당하는 실제 축번호가 다른 값이라야 한다.
            //         같은 축을 다른 Coordinate에 중복 맵핑하지 말아야 한다.
            CAXM.AxmContiSetAxisMap(lCoordinate, lSize, lAxesNo);

            // 지정된 좌표계에 연속보간 축 절대/상대 모드를 설정한다.
            // (주의점 : 반드시 축맵핑 하고 사용가능)
            // 지정 축의 이동 거리 계산 모드를 설정한다.
            // uAbsRelMode : POS_ABS_MODE '0' - 절대 좌표계
            //               POS_REL_MODE '1' - 상대 좌표계
            CAXM.AxmContiSetAbsRelMode(lCoordinate, uAbsRelMode);// 상대위치구동으로설정

            // 시작점과 종료점을 지정하여 다축 직선 보간 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
            // AxmContiBeginNode, AxmContiEndNode와 같이사용시 지정된 좌표계에 시작점과 종료점을 지정하여 직선 보간 구동하는 Queue에 저장함수가된다. 
            // 직선 프로파일 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmContiStart함수를 사용해서 시작한다.
            uint duRetCode;
            duRetCode = CAXM.AxmLineMove(lCoordinate, dPosition, dMaxVelocity, dMaxAccel, dMaxDecel);
            if (duRetCode != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                MessageBox.Show(String.Format("AxmLineMove return error[Code:{0:d}]", duRetCode));
        }
        /**
         * @brief 서보 정지 
         * @param axis 서보 순번
         * @return void
         */
        public static void MoveStop(int axis) // MoveStop(축번호) 서보 정지 
        {
            //Util.Debug("MoveStop : " + axis.ToString());
            if (GlobalVar.Simulation)
            {
                // simulation
                GlobalVar.AxisStatus[axis].StandStill = true;
                GlobalVar.AxisStatus[axis].Velocity = 0;
                return;
            }
            float dec = Math.Abs((float)GlobalVar.AxisStatus[axis].Velocity) * 10;
            float jerk = Math.Abs((float)GlobalVar.AxisStatus[axis].Velocity) * 100;
            switch (GlobalVar.MasterType)
            {
                case enumMasterType.MXP:
                    MXP.MXP_STOP_OUT Out = new MXP.MXP_STOP_OUT { };
                    Motion_Function.MXP_MC_Stop((uint)axis, (uint)(Motion_Function.IndexCal((UInt32)MXP.MXP_MotionBlockIndex.mcStop) + axis), dec, jerk, false, Out);
                    break;
                case enumMasterType.MXN:
                    MXN.MXN_Stop((uint)axis + 1);
                    if (GlobalVar.SystemStatus == enumSystemStatus.AutoRun)
                    {
                        Thread.Sleep(GlobalVar.Servo_Stop_After_Sleep);
                    }
                    break;
                case enumMasterType.AXL:
                    //CAXM.AxmMoveSStop(Convert.ToInt32(axis));
                    CAXM.AxmMoveEStop(Convert.ToInt32(axis));

                    break;

                default:
                    break;
            }
        }
		/**
         * @brief 서보 전체 정지        
         * @return void
         */
        public static void MoveStopAll() //전체 서보모터 정지
        {
            for (int axis = 0; axis < GlobalVar.Axis_count; axis++)
            {
                if (!GlobalVar.AxisStatus[axis].StandStill)
                {
                    Ecat.MoveStop(axis);
                }
            }
        }
       
 	   	/**
         * @brief 홈 잡은 후 추가 옵셋 만큼 이동
         * @param axis 서보 순번
         * @return bool 이동 지령 수행 완료시 true
         *      지령 수행 실패시 false
         */
        public static bool HomeOffsetMove(uint axis) // 홈 잡은 후 추가 옵셋 만큼 이동
        {
            Thread.Sleep(GlobalVar.HighGain_Robot_Sleep);

            while (!GlobalVar.AxisStatus[axis].isHomed)
            {
                if (GlobalVar.SystemStatus != enumSystemStatus.Initialize)
                {
                    FuncWin.TopMessageBox("Can't Offest Moving - systemStatus Not Initialize");
                    return false;
                }
                debug("servo " + axis.ToString() + " Homeing");
                Thread.Sleep(100);
            }

            //메뉴얼, 모델 단독 Homeing 할때만 원점복귀모드 해제 by DG 20220801
            if (GlobalVar.TabMain == enumTabMain.Manual ||
                GlobalVar.TabMain == enumTabMain.Model)
            {
                MXN.MXN_Write_R(MXN.REG_BIT, 700, 5, 5, 0);
            }

            if (axis == (uint)enumServoAxis.Lift_PCBInPut)
            {
                if (GlobalVar.AxisStatus[(int)axis].StandStill)
                {

                    if (FuncHigain.MoveServo((int)axis, GlobalVar.HighGain_Lift_PCBinput, GlobalVar.ServoSpeed / GlobalVar.Magazine_LiftSpeed_check, true))
                    {
                        return true;
                    }
                    else
                    {
                        FuncWin.TopMessageBox("Can't PCB IN PUT Lift Offest Moving2");
                        return false;
                    }

                }
                else
                {
                    FuncWin.TopMessageBox("Can't PCB IN PUT Lift Offest Moving");
                    return false;
                }

            }
            else if (axis == (uint)enumServoAxis.Lift_BoxInPut)
            {
                if (GlobalVar.AxisStatus[(int)axis].StandStill)
                {

                    if (FuncHigain.MoveServo((int)axis, GlobalVar.HighGain_Lift_Boxinput, GlobalVar.ServoSpeed / GlobalVar.Magazine_LiftSpeed_check, true))
                    {
                        return true;
                    }
                    else
                    {
                        FuncWin.TopMessageBox("Can't Tray IN PUT Lift Offest Moving2");
                        return false;
                    }
                }
                else
                {
                    FuncWin.TopMessageBox("Can't Tray IN PUT Lift Offest Moving");
                    return false;
                }

            }
            else if (axis == (uint)enumServoAxis.Lift_BoxOutPut)
            {
                if (GlobalVar.AxisStatus[(int)axis].StandStill)
                {

                    if (FuncHigain.MoveServo((int)axis, GlobalVar.HighGain_Lift_BoxOutput, GlobalVar.ServoSpeed / GlobalVar.Magazine_LiftSpeed_check, true))
                    {
                        return true;
                    }
                    else
                    {
                        FuncWin.TopMessageBox("Can't PCB OUT PUT Lift Offest Moving2");
                        return false;
                    }
                }
                else
                {
                    FuncWin.TopMessageBox("Can't PCB OUT PUT Lift Offest Moving");
                    return false;
                }

            }

            else if (axis == (uint)enumServoAxis.BaseRobot_Z)
            {

                return true;
            }

            else if (axis == (uint)enumServoAxis.BaseRobot_T)
            {

                return true;

                //턴관련 옵셋 없앰 by DG 220913
                //if (GlobalVar.AxisStatus[(int)axis].StandStill)
                //{
                //    if (FuncHigain.MoveServo((int)axis, (5.6 * 27.7777777778), GlobalVar.ServoSpeed / GlobalVar.Magazine_Speed_check, true))
                //    {
                //        return true;
                //    }
                //    else
                //    {
                //        Util.TopMessageBox("Can't Turn Offest Moving2");
                //        return false;
                //    }

                //    //Ecat.MoveAbsolute((uint)axis, (5.6 * 27.7777777778), 20 * 1000);

                //}
                //else
                //{
                //    Util.TopMessageBox("Can't Turn Offest Moving");
                //    return false;
                //}


            }

            else if (axis == (uint)enumServoAxis.BaseRobot_X)
            {

                return true;
            }
            else if (axis == (uint)enumServoAxis.BaseRobot_Y)
            {

                return true;
            }

            else if (axis == (uint)enumServoAxis.PCB_Width)
            {
                if (GlobalVar.AxisStatus[(int)axis].StandStill)
                {
                    if (FuncHigain.MoveServo((int)axis, GlobalVar.HighGain_PCBWidth, GlobalVar.ServoSpeed, true))
                    {
                        return true;
                    }
                    else
                    {
                        FuncWin.TopMessageBox("Can't PCB_Width Offest Moving2");
                        return false;
                    }
                }
                else
                {
                    FuncWin.TopMessageBox("Can't PCB_Width Offest Moving");
                    return false;
                }

            }
            else if (axis == (uint)enumServoAxis.PCBInPut_Push)
            {
                if (GlobalVar.AxisStatus[(int)axis].StandStill)
                {
                    if (FuncHigain.MoveServo((int)axis, GlobalVar.HighGain_PCBInPut_PushWidth, GlobalVar.ServoSpeed, true))
                    {
                        return true;
                    }
                    else
                    {
                        FuncWin.TopMessageBox("Can't PCBInPut_Push Offest Moving2");
                        return false;
                    }
                }
                else
                {
                    FuncWin.TopMessageBox("Can't PCBInPut_Push Offest Moving");
                    return false;
                }

            }
            else if (axis == (uint)enumServoAxis.BoxInPut_Push)
            {
                if (GlobalVar.AxisStatus[(int)axis].StandStill)
                {
                    //-6.30

                    if (FuncHigain.MoveServo((int)axis, GlobalVar.HighGain_BoxInPut_PushWidth, GlobalVar.ServoSpeed, true))
                    {
                        return true;
                    }
                    else
                    {
                        FuncWin.TopMessageBox("Can't BoxInPut_Push Offest Moving2");
                        return false;
                    }
                }
                else
                {
                    FuncWin.TopMessageBox("Can't BoxInPut_Push Offest Moving");
                    return false;
                }

            }
            else if (axis == (uint)enumServoAxis.BoxOutPut_Push)
            {
                if (GlobalVar.AxisStatus[(int)axis].StandStill)
                {
                    //-6.30

                    if (FuncHigain.MoveServo((int)axis, GlobalVar.HighGain_BoxOutPut_PushWidth, GlobalVar.ServoSpeed, true))
                    {
                        return true;
                    }
                    else
                    {
                        FuncWin.TopMessageBox("Can't BoxOutPut_Push Offest Moving2");
                        return false;
                    }
                }
                else
                {
                    FuncWin.TopMessageBox("Can't BoxOutPut_Push Offest Moving");
                    return false;
                }

            }
            FuncWin.TopMessageBox("Can't Offest Moving");
            return false;
        }


    }
}
