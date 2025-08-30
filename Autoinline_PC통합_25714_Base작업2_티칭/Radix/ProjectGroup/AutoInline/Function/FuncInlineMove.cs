using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;    // SerialPort 클래스 사용을 위해서 추가
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace Radix
{
    public class FuncInlineMove
    {
        /*
        FuncInlineMotion : Inline 장비 모터 관련 정의. Servo + Step
        //*/

        private static void debug(string str)
        {
            Util.Debug("FuncInlineMotion + " + str);
            //FuncLog.WriteLog_Debug(str);
        }

        /*
        public static bool CheckNearWidth(FuncInline.enumPMCAxis axis) // 폭조절 스텝모터가 해당 폭 위치에 있는가?
        {
            double teachingWidth = FuncInline.TeachingWidth[(int)axis];
            double actualWidth = FuncInline.PMCStatus[(int)axis].Position;

            return Math.Abs(actualWidth - teachingWidth) <= 0.1;
        }
        //*/

        #region 컨베어 운영 관련
        public static void StopAllJog()
        {
            StopAllJog(false);
        }

        public static void StopAllJog(bool servoOnly)
        {
            #region 모든 서보모터
            for (int i = 0; i < GlobalVar.AxisStatus.Length; i++)
            {
                //if (!GlobalVar.Axis_Sync ||
                //    (i != (int)FuncInline.enumServoAxis.SV02_Lift1 &&
                //            i != (int)FuncInline.enumServoAxis.SV05_Rack2_Width))
                //{
                FuncMotion.MoveStop(i);
                //}
            }
            #endregion

            #region 모든 스텝모터
            if (!servoOnly)
            {
                for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length; i++)
                {
                    if (!FuncInline.PMCStatus[i].StandStill)
                    {
                        ////FuncInline.ComPMC[(int)(i / 2)].Stop((FuncInline.enumPMCAxis)i);
                    }
                }
            }
            #endregion

            // 모든 사이트 모션 추가 필요.
        }

        public static void StopAllConveyor()
        {
            //debug("Stop All Conveyor");
            // 모든 컨베어 정지
            for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumDONames)).Length; i++)
            {
                if (((FuncInline.enumDONames)i).ToString().Contains("Motor"))
                {
                    DIO.WriteDOData(i, false);
                }
            }


        }



        #endregion


        /**
         * @brief PMC Motion 컨트롤러에 연결된 축 이름으로 x/y 확인
         *      모션 컨트롤러 하나에 축이 두개씩 연결되므로 enum으로 선언된 축이름으로 모션컨트롤러 내 x/y 축을 환산
         * @param axis 축이름
         */
        public static enumAxis GetPMCAxis(FuncInline.enumPMCAxis axis) // PMC Motion 컨트롤러에 연결된 축 이름으로 x/y 확인
        {
            int mod = (int)axis % 2;
            if (mod == 0)
            {
                return enumAxis.X;
            }
            return enumAxis.Y;
        }





        /**
         * @brief Autonics 폭조절 모터 자체의 거리 계산값을 기구상의 실제 거리로 환산
         *          PCB 폭값과 같이 기구상에서는 사용자 좌표계로 사용하므로 사용자 거리값을 계산한다.
         * @param axis 축이름
         * @param pos 계산할 위치값
         * @return double 기구상 사용자 좌표
         */
        public static double CalcWidthPos(FuncInline.enumPMCAxis axis, double pos) // 일반 폭조절 모터 자체의 거리 계산값을 기구상의 실제 거리로 환산 - 위치값 확인할 때
        {
            switch (axis)
            {
                case FuncInline.enumPMCAxis.ST00_InShuttle_Width:
                    return 300 - pos;
                case FuncInline.enumPMCAxis.ST01_OutShuttle_Width:
                    return 300 - pos;
                case FuncInline.enumPMCAxis.ST02_OutConveyor_Width:
                    return 300 - pos;
                case FuncInline.enumPMCAxis.ST03_InConveyor_Width:
                    return 300 - pos;
                case FuncInline.enumPMCAxis.ST04_NGBuffer:
                    return 300 - pos;
                default:
                    return pos;
            }
        }

        /**
         * @brief Autonics 폭조절 모터 실제 거리를 기구상의 모터 자체의 거리 계산값으로 환산
         *          PCB 폭과 같은 사용자 거리값을 모터 자체의 원점 기준 거리로 역환산한다.
         * @param axis 축이름
         * @param pos 계산할 사용자 위치값
         * @return double 원점 대비 모터상 거리값
         */
        public static double ReCalcWidthPos(int axis, double pos) // 일반 폭조절 실제 거리를 기구상의 모터 자체의 거리 계산값으로 환산 - 지령 날릴 때
        {
            switch (axis)
            {
                case 0:
                    return pos - FuncInline.DefaultPCBWidth;
                case 1:
                    return pos - FuncInline.DefaultPCBWidth;
                case 2:
                    return pos - FuncInline.DefaultPCBWidth;
                case 3:
                    return pos - FuncInline.DefaultPCBWidth;
                case 4:
                    return pos - FuncInline.DefaultPCBWidth;
                default:
                    return pos;
            }
        }

        /**
         * @brief 서보 모터 실제 거리를 기구상의 사용자 거리 계산값으로 환산
         *          축별로 사용자 인지좌표로 사용하므로 UI상에 표시 위해서 환산한다.
         * @param axis 축순번
         * @param pos 모터의 원점 대비 거리값
         * @return double 기구상 실제 사용자 좌표
         */
        public static double CalcAxisPos(int axis, double pos) // 모터 자체의 거리 계산값을 기구상의 실제 거리로 환산
        {
            return CalcAxisPos((FuncInline.enumServoAxis)axis, pos);
        }

        /**
         * @brief 서보 모터 실제 거리를 기구상의 사용자 거리 계산값으로 환산
         *          축별로 사용자 인지좌표로 사용하므로 UI상에 표시 위해서 환산한다.
         * @param axis 축이름
         * @param pos 모터의 원점 대비 거리값
         * @return double 기구상 실제 사용자 좌표
         */
        public static double CalcAxisPos(FuncInline.enumServoAxis axis, double pos) // 모터 자체의 거리 계산값을 기구상의 실제 거리로 환산
        {
            switch (axis)
            {
                case FuncInline.enumServoAxis.SV00_In_Shuttle:
                    return 235 + pos;
                case FuncInline.enumServoAxis.SV01_Out_Shuttle:
                    return 235 + pos;
                case FuncInline.enumServoAxis.SV02_Lift1:
                    return 235 + pos;
                case FuncInline.enumServoAxis.SV03_Rack1_Width:
                    return 600 - pos;
                case FuncInline.enumServoAxis.SV04_Lift2:
                    return 600 - pos;
                case FuncInline.enumServoAxis.SV05_Rack2_Width:
                    return pos;
                case FuncInline.enumServoAxis.SV06_Scan_X:
                    return pos;
                case FuncInline.enumServoAxis.SV07_Scan_Y:
                    return 600 - pos;

                default:
                    return pos;
            }
        }

        /**
         * @brief 사용자 거리 계산값을 서보 모터 실제 거리로 환산
         *          축별로 사용자 인지좌표로 사용하므로 UI상에 표시된 좌표를 지령과 연결하기 위해 사용
         * @param axis 축순번
         * @param pos 모터의 원점 대비 거리값
         * @return double 모터상 원점 대비 거리값
         */
        public static double ReCalcAxisPos(int axis, double pos) // 기구상의 실제 거리를모터 자체의 거리 계산값으로 환산
        {
            return ReCalcAxisPos((FuncInline.enumServoAxis)axis, pos);
        }

        /**
         * @brief 사용자 거리 계산값을 서보 모터 실제 거리로 환산
         *          축별로 사용자 인지좌표로 사용하므로 UI상에 표시된 좌표를 지령과 연결하기 위해 사용
         * @param axis 축이름
         * @param pos 모터의 원점 대비 거리값
         * @return double 모터상 원점 대비 거리값
         */
        public static double ReCalcAxisPos(FuncInline.enumServoAxis axis, double pos) // 기구상의 실제 거리를모터 자체의 거리 계산값으로 환산
        {
            switch (axis)
            {
                case FuncInline.enumServoAxis.SV00_In_Shuttle:
                    return 235 + pos;
                case FuncInline.enumServoAxis.SV01_Out_Shuttle:
                    return 235 + pos;
                case FuncInline.enumServoAxis.SV02_Lift1:
                    return 235 + pos;
                case FuncInline.enumServoAxis.SV03_Rack1_Width:
                    return 600 - pos;
                case FuncInline.enumServoAxis.SV04_Lift2:
                    return 600 - pos;
                case FuncInline.enumServoAxis.SV05_Rack2_Width:
                    return pos;
                case FuncInline.enumServoAxis.SV06_Scan_X:
                    return pos;
                case FuncInline.enumServoAxis.SV07_Scan_Y:
                    return 600 - pos;

                default:
                    return pos;
            }
        }

        /**
         * @brief 프로그램 UI연결할 실제 좌표값으로부터 실지령 pulse 계산
         * @param axis 축순번
         * @param pos UI표시된 사용자 좌표값
         * @return double 범위 안 True
         *      범위 외 False
         */
        public static double GetRealPulse(int axis, double pos) // 프로그램 UI연결할 실제 좌표값으로 실지령 pulse 계산
        {
            return FuncMotion.MMToPulse(FuncInlineMove.ReCalcAxisPos((FuncInline.enumServoAxis)axis, pos),
                                                       GlobalVar.ServoGearRatio[axis],
                                                       GlobalVar.ServoRevMM[axis],
                                                       GlobalVar.ServoRevPulse[axis]);
        }

        /**
         * @brief 지정 축 위치를 실제 사용자에게 표시할 좌표계로 환산 출력
         * @param axis 축순번
         * @return double 사용자 좌표 또는 거리값
         */
        public static double GetRealPosition(int axis) // 프로그램 UI연결할 실제 좌표값 계산
        {
            return GlobalVar.AxisStatus[axis].Position;
            //return FuncMotion.CalcAxisPos((FuncInline.enumServoAxis)axis,
            //                        PulseToMM((long)GlobalVar.AxisStatus[axis].Position,
            //                                                GlobalVar.ServoGearRatio[axis],
            //                                                GlobalVar.ServoRevMM[axis],
            //                                                GlobalVar.ServoRevPulse[axis]));
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
            //debug("MoveHome : " + axis.ToString());
            if (GlobalVar.E_Stop)
            {
                return false;
            }
            if (GlobalVar.Simulation)
            {
                // simulation
                //*
                GlobalVar.AxisStatus[axis].HomeAbsSwitch = true;
                GlobalVar.AxisStatus[axis].isHomed = true;
                GlobalVar.AxisStatus[axis].StandStill = true;
                GlobalVar.AxisStatus[axis].Position = FuncInline.DefaultPCBWidth;
                GlobalVar.AxisStatus[axis].Velocity = 0;
                //*/
                return true;
            }

            return FuncMotion.MoveHome(axis, WaitDone, Timeout);
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
            //debug("MoveAbsolute(" + axis.ToString() + "," + Pos.ToString() + "," + Vel.ToString() + "," + Acc.ToString() + "," + Dec.ToString() + "," + Jerk.ToString() + "," + WaitDone.ToString() + "," + Timeout.ToString() + ",");

            if (GlobalVar.E_Stop)
            {
                return;
            }

            if (GlobalVar.Simulation)
            {
                // simulation
                //debug("MoveAbsoute : " + axis.ToString() + " - " + Pos.ToString());
                //Console.WriteLine("MoveAbsoute : " + axis.ToString() + " - " + Pos.ToString());
                //*
                GlobalVar.AxisStatus[axis].HomeAbsSwitch = false;
                GlobalVar.AxisStatus[axis].StandStill = true;
                GlobalVar.AxisStatus[axis].Position = Pos;
                GlobalVar.AxisStatus[axis].Velocity = 0;
                //*/
                return;
            }




            FuncMotion.MoveAbsolute(axis,
                                    GetRealPulse((int)axis, Pos),
                                    FuncMotion.GetRealSpeed((int)axis, Vel),
                                    FuncMotion.GetRealSpeed((int)axis, Vel) * Acc,
                                    FuncMotion.GetRealSpeed((int)axis, Vel) * Dec,
                                    FuncMotion.GetRealSpeed((int)axis, Vel) * Jerk,
                                    WaitDone,
                                    Timeout);
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
            //debug("MoveRelative(" + axis.ToString() + "," + Pos.ToString() + "," + Vel.ToString() + "," + Acc.ToString() + "," + Dec.ToString() + "," + Jerk.ToString() + "," + WaitDone.ToString() + "," + Timeout.ToString() + ",");

            if (GlobalVar.E_Stop)
            {
                return;
            }

            if (GlobalVar.Simulation)
            {
                // simulation
                //debug("MoveRelative : " + axis.ToString() + " - " + Pos.ToString());
                Console.WriteLine("MoveRelative : " + axis.ToString() + " - " + Pos.ToString());
                //*
                GlobalVar.AxisStatus[axis].HomeAbsSwitch = false;
                GlobalVar.AxisStatus[axis].StandStill = true;
                GlobalVar.AxisStatus[axis].Position = Pos;
                GlobalVar.AxisStatus[axis].Velocity = 0;
                //*/
                return;
            }

            FuncMotion.MoveRelative(axis,
                        GetRealPulse((int)axis, Pos),
                        FuncMotion.GetRealSpeed((int)axis, Vel),
                        FuncMotion.GetRealSpeed((int)axis, Vel) * Acc,
                        FuncMotion.GetRealSpeed((int)axis, Vel) * Dec,
                        FuncMotion.GetRealSpeed((int)axis, Vel) * Jerk,
                        WaitDone,
                        Timeout);

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
            //debug("MoveVelocity : " + axis.ToString() + " - " + Vel.ToString());
            if (GlobalVar.Simulation)
            {
                /*
                // 시뮬레이션에서는 지속적으로 돌리지 않으므로 무시한다.
                // simulation
                //debug("MoveVelocity : " + axis.ToString() + " - " + Vel.ToString());
                GlobalVar.AxisStatus[axis].HomeAbsSwitch = false;
                GlobalVar.AxisStatus[axis].StandStill = false;
                GlobalVar.AxisStatus[axis].Velocity = Vel;
                // 역방향 - 홈으로
                if (Vel < 0)
                {
                    GlobalVar.AxisStatus[axis].StandStill = true;
                    GlobalVar.AxisStatus[axis].HomeAbsSwitch = true;
                    GlobalVar.AxisStatus[axis].Velocity = 0;
                    //GlobalVar.AxisStatus[axis].Position = 0;
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
                    //debug("역방향 구동 금지 " + axis.ToString());
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
                    //debug("정방향 구동 금지 " + axis.ToString());
                    MoveStop((int)axis);
                    return;
                }
                //*/
            }

            FuncMotion.MoveVelocity(axis,
                        FuncMotion.GetRealSpeed((int)axis, Vel),
                        FuncMotion.GetRealSpeed((int)axis, Vel) * Acc,
                        FuncMotion.GetRealSpeed((int)axis, Vel) * Dec,
                        FuncMotion.GetRealSpeed((int)axis, Vel) * Jerk);

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
            FuncMotion.Double_Axis_Move(axis_1,
                                        GetRealPulse((int)axis_1, POS_1),
                                        axis_2,
                                        GetRealPulse((int)axis_2, POS_2),
                                        FuncMotion.GetRealSpeed((int)axis_1, Vel),
                                        boganNum);
        }

        /**
         * @brief XYT 스테이지 이동
         *      레이저 마킹기 사용 후 Base 작업중 작동부 주석처리됨
         * @param speed 회전 사용자 속도
         * @param posX 이동할 사용자 X 좌표
         * @param posY 이동할 사용자 Y 좌표
         * @param turn 반전여부.
         * @return bool 이동 완료 여부
         */
        public static bool MoveStage(double speed, double posX, double posY, bool turn) // XYZT 스테이지 이동
        {
            return MoveStage(speed, posX, posY, turn, 10);
        }

        public static bool MoveAbsolute(FuncInline.enumServoAxis axis, double pos, double vel, double acc)
        {


            if (GlobalVar.AxisStatus[(int)axis].StandStill &&
                !FuncMotion.CheckNearPos(GetRealPosition((int)axis), pos, 0.1))
            {
                //FuncMotion.MoveAbsolute(0,
                //                    FuncMotion.MMToPulse(Func.CalcAxisPos(0, pos), GlobalVar.ServoGearRatio[0], GlobalVar.ServoRevMM[0], GlobalVar.ServoRevPulse[0]),
                //                    FuncMotion.MMToPulse(vel, GlobalVar.ServoGearRatio[0], GlobalVar.ServoRevMM[0], GlobalVar.ServoRevPulse[0]),
                //                    FuncMotion.MMToPulse(vel, GlobalVar.ServoGearRatio[0], GlobalVar.ServoRevMM[0], GlobalVar.ServoRevPulse[0]) * acc,
                //                    FuncMotion.MMToPulse(vel, GlobalVar.ServoGearRatio[0], GlobalVar.ServoRevMM[0], GlobalVar.ServoRevPulse[0]) * acc,
                //                    FuncMotion.MMToPulse(vel, GlobalVar.ServoGearRatio[0], GlobalVar.ServoRevMM[0], GlobalVar.ServoRevPulse[0]) * acc * acc,
                //                    false,
                //                    0);
                FuncMotion.MoveAbsolute((uint)axis,
                                    pos,
                                    vel,
                                    vel * acc,
                                    vel * acc,
                                    vel * acc * acc,
                                    false,
                                    0);
            }
            return GlobalVar.AxisStatus[(int)axis].StandStill &&
                FuncMotion.CheckNearPos(GetRealPosition((int)axis), pos, 0.1);
        }


        /**
         * @brief XYT 스테이지 이동
         *      레이저 마킹기 사용 후 Base 작업중 작동부 주석처리됨
         * @param speed 회전 사용자 속도
         * @param posX 이동할 사용자 X 좌표
         * @param posY 이동할 사용자 Y 좌표
         * @param turn 반전여부.
         * @param vel 작동 속도.
         * @return bool 이동 완료 여부
         */
        public static bool MoveStage(double speed, double posX, double posY, bool turn, double vel) // XYZT 스테이지 이동
        {
            bool moveX = false;
            bool moveY = false;
            bool moveT = false;
            bool moveZ = false; // z축 이동 완료 여부
            bool upZ = false; // z축 상승 완료 여부
            bool needZ = false; // z축 이동 필요 여부

            ulong startTime = GlobalVar.TickCount64;
            /*
            double turnAngle = GlobalVar.WorkInputPos.a; //GlobalVar.WorkAngle[0];

            //double posZ = GlobalVar.AxisStatus[3].Position;
            if (turn)
            {
                turnAngle = GlobalVar.WorkOutputPos.a; //GlobalVar.WorkAngle[1];
            }
            if (Math.Abs(Util.PulseToDegree(GlobalVar.AxisStatus[2].Position, GlobalVar.GearRatio[2], GlobalVar.RevPulse[2]) - turnAngle) < 5) // 턴이 필요한 상황
            {
                moveT = true;
                moveZ = true;
                upZ = true;
                needZ = false;
            }
            else
            {
                moveZ = false;
                upZ = false;
                needZ = true;
            }
            //*/

            while (!GlobalVar.GlobalStop)
            {
                if (GlobalVar.TickCount64 - startTime > (ulong)GlobalVar.NormalTimeout * 1000)
                {
                    //FuncMotion.MoveStop(1);
                    //FuncMotion.MoveStop(2);
                    //FuncMotion.MoveStop(3);
                    //FuncMotion.MoveStop(4);
                    return false;
                }

                #region X
                if (GlobalVar.AxisStatus[0].StandStill &&
                    Math.Abs(CalcAxisPos(0, FuncMotion.PulseToMM(GlobalVar.AxisStatus[0].Position, GlobalVar.ServoGearRatio[0], GlobalVar.ServoRevMM[0], GlobalVar.ServoRevPulse[0])) - posX) < 0.1)
                {
                    moveX = true;
                }
                //else if (GlobalVar.AxisStatus[0].Errored ||
                //            GlobalVar.AxisStatus[0].ErrorStop)
                //{
                //    FuncMotion.ServoReset(0);
                //}
                //else if (!GlobalVar.AxisStatus[0].PowerOn)
                //{
                //    FuncMotion.ServoOn(0, true);
                //}
                //else if (GlobalVar.AxisStatus[0].StandStill)
                //{
                //    FuncMotion.MoveAbsolute(0,
                //                        FuncMotion.MMToPulse(Func.CalcAxisPos(0, posX), GlobalVar.GearRatio[0], GlobalVar.RevMM[0], GlobalVar.RevPulse[0]),
                //                        FuncMotion.MMToPulse(speed, GlobalVar.GearRatio[0], GlobalVar.RevMM[0], GlobalVar.RevPulse[0]),
                //                        FuncMotion.MMToPulse(speed, GlobalVar.GearRatio[0], GlobalVar.RevMM[0], GlobalVar.RevPulse[0]) * vel,
                //                        FuncMotion.MMToPulse(speed, GlobalVar.GearRatio[0], GlobalVar.RevMM[0], GlobalVar.RevPulse[0]) * vel,
                //                        FuncMotion.MMToPulse(speed, GlobalVar.GearRatio[0], GlobalVar.RevMM[0], GlobalVar.RevPulse[0]) * vel * vel,
                //                        false,
                //                        0);
                //}
                #endregion
                if (moveX && moveY && moveT && moveZ)
                {
                    return true;
                }

                Thread.Sleep(GlobalVar.ThreadSleep);
            }
            return false;
        }

        // 리프트가 지정 위치에 있는가? CheckNearPos로 통합
        /*
        public static bool CheckLiftPos(FuncInline.enumLiftName lift, FuncInline.enumLiftPos pos, double gap)
        {
            try
            {
                int axis = lift == FuncInline.enumLiftName.Lift2 ? (int)FuncInline.enumServoAxis.SV04_Lift2 : (int)FuncInline.enumServoAxis.SV02_Lift1;
                double axis_pos = GlobalVar.AxisStatus[axis].Position;
                double lift_pos = FuncInline.LiftPos[(int)lift, (int)pos];
                return Math.Abs(axis_pos - lift_pos) <= gap;
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
            return false;
        }
        //*/



        public static bool MoveLift(FuncInline.enumLiftName lift, FuncInline.enumLiftPos pos) // 투입리프트 동작. 설정속도로 동작
        {
            return MoveLift(lift, pos, FuncInline.LiftSpeed);
        }

        public static bool MoveLift(FuncInline.enumLiftName lift, FuncInline.enumLiftPos pos, double speed) // 투입리프트 동작. 지정속도로
        {
            try
            {
                debug("MoveLift : " + lift.ToString() + " ==> " + pos);
                int axis = lift == FuncInline.enumLiftName.RearLift ? (int)FuncInline.enumServoAxis.SV04_Lift2 : (int)FuncInline.enumServoAxis.SV02_Lift1;

                if (GlobalVar.AxisStatus[axis].StandStill &&
                     FuncInlineMove.CheckNearPos(lift, pos))
                {
                    return true;
                }

                #region 컨베어 동작중에는 이동금지
                if (lift == FuncInline.enumLiftName.FrontLift &&
                    (DIO.GetDORead((int)FuncInline.enumDONames.Y405_0_Front_Lift_Up_Motor_Cw) ||
                            DIO.GetDORead((int)FuncInline.enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw) ||
                            DIO.GetDORead((int)FuncInline.enumDONames.Y405_4_Front_Lift_Down_Motor_Cw) ||
                            DIO.GetDORead((int)FuncInline.enumDONames.Y405_6_Front_Lift_Down_Motor_Ccw)))
                {
                    FuncMotion.MoveStop((int)FuncInline.enumServoAxis.SV02_Lift1);
                    return false;
                }
                if (lift == FuncInline.enumLiftName.RearLift &&
                    (DIO.GetDORead((int)FuncInline.enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw) ||
                            DIO.GetDORead((int)FuncInline.enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw) ||
                            DIO.GetDORead((int)FuncInline.enumDONames.Y305_1_Rear_Lift_Down_Motor_Cw) ||
                            DIO.GetDORead((int)FuncInline.enumDONames.Y304_1_Rear_Lift_Down_Motor_Ccw)))
                {
                    FuncMotion.MoveStop((int)FuncInline.enumServoAxis.SV04_Lift2);
                    return false;
                }
                #endregion
                #region Lift2 배출 또는 NG 컨베어 걸침 가능성 있으면 이동 금지
                //if (lift == FuncInline.enumLiftName.RearLift &&
                //    (FuncInline.CheckOutputPCBStopAtStart() ||
                //            FuncInline.CheckNGPCBStopAtStart()))
                //{
                //    FuncMotion.MoveStop((int)FuncInline.enumServoAxis.SV04_Lift2);
                //    return false;
                //}
                #endregion

                if (GlobalVar.AxisStatus[axis].StandStill)
                {
                    //debug("리프트 정지시 이동 " + lift.ToString() + "," + pos.ToString());
                    FuncInlineMove.MoveAbsolute(Convert.ToUInt32(axis),
                                    FuncInline.LiftPos[(int)lift, (int)pos],
                                    speed);
                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
            return false;
        }

        // 지정 렉 리프트가 위치해 있는 사이트 리턴
        public static FuncInline.enumLiftPos GetLiftPos(int rack)
        {
            double current = 0;
            FuncInline.enumLiftName lift = FuncInline.enumLiftName.FrontLift;
            if (rack == 2)
            {
                lift = FuncInline.enumLiftName.RearLift;
            }
            switch (rack)
            {
                case 0:
                    current = GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.SV02_Lift1].Position;

                    // site1up~site7up, site1Down~site7Down, Passline1
                    if (CheckNearPos(lift, FuncInline.enumLiftPos.FrontPassLine))
                    {
                        return FuncInline.enumLiftPos.FrontPassLine;
                    }
                    if (CheckNearPos(lift, FuncInline.enumLiftPos.FrontScanPos))
                    {
                        return FuncInline.enumLiftPos.FrontScanPos;
                    }
                    for (FuncInline.enumLiftPos pos = FuncInline.enumLiftPos.Site1_F_DT1_Up; pos <= FuncInline.enumLiftPos.Site13_F_FT3_Up; pos++)
                    {
                        if (CheckNearPos(lift, pos))
                        {
                            return pos;
                        }
                    }
                    for (FuncInline.enumLiftPos pos = FuncInline.enumLiftPos.Site1_F_DT1_Down; pos <= FuncInline.enumLiftPos.Site13_F_FT3_Down; pos++)
                    {
                        if (CheckNearPos(lift, pos))
                        {
                            return pos;
                        }
                    }

                    return FuncInline.enumLiftPos.UnKnown;
                case 1:
                    current = GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.SV02_Lift1].Position;

                    // site8up~site14up, site8Down~site14Down, passline2
                    if (CheckNearPos(lift, FuncInline.enumLiftPos.RearPassLine))
                    {
                        return FuncInline.enumLiftPos.RearPassLine;
                    }
                    if (CheckNearPos(lift, FuncInline.enumLiftPos.RearNGLine))
                    {
                        return FuncInline.enumLiftPos.RearNGLine;
                    }
                    if (CheckNearPos(lift, FuncInline.enumLiftPos.RearScanPos))
                    {
                        return FuncInline.enumLiftPos.RearScanPos;
                    }

                    for (FuncInline.enumLiftPos pos = FuncInline.enumLiftPos.Site14_R_DT1_Up; pos <= FuncInline.enumLiftPos.Site26_R_FT3_Up; pos++)
                    {
                        if (CheckNearPos(lift, pos))
                        {
                            return pos;
                        }
                    }
                    for (FuncInline.enumLiftPos pos = FuncInline.enumLiftPos.Site14_R_DT1_Down; pos <= FuncInline.enumLiftPos.Site26_R_FT3_Down; pos++)
                    {
                        if (CheckNearPos(lift, pos))
                        {
                            return pos;
                        }
                    }
                    return FuncInline.enumLiftPos.UnKnown;


            }
            return FuncInline.enumLiftPos.UnKnown;

        }

        // 지정 리프트가 위치해 있는 위치를 찾는다.
        public static FuncInline.enumLiftPos GetLiftPos(FuncInline.enumLiftName lift)
        {
            double current = 0;
            #region 리프트1
            if (lift == FuncInline.enumLiftName.FrontLift)
            {
                current = GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.SV02_Lift1].Position;

                // site1up~site14up, site1Down~site14Down, Passline1, passline2
                if (CheckNearPos(lift, FuncInline.enumLiftPos.FrontPassLine))
                {
                    return FuncInline.enumLiftPos.FrontPassLine;
                }
                if (CheckNearPos(lift, FuncInline.enumLiftPos.FrontScanPos))
                {
                    return FuncInline.enumLiftPos.FrontScanPos;
                }
                if (CheckNearPos(lift, FuncInline.enumLiftPos.RearPassLine))
                {
                    return FuncInline.enumLiftPos.RearPassLine;
                }
                if (CheckNearPos(lift, FuncInline.enumLiftPos.RearNGLine))
                {
                    return FuncInline.enumLiftPos.RearNGLine;
                }
                for (FuncInline.enumLiftPos pos = FuncInline.enumLiftPos.Site1_F_DT1_Up; pos <= FuncInline.enumLiftPos.Site13_F_FT3_Up; pos++)
                {
                    if (CheckNearPos(lift, pos))
                    {
                        return pos;
                    }
                }
                for (FuncInline.enumLiftPos pos = FuncInline.enumLiftPos.Site1_F_DT1_Down; pos <= FuncInline.enumLiftPos.Site13_F_FT3_Down; pos++)
                {
                    if (CheckNearPos(lift, pos))
                    {
                        return pos;
                    }
                }

                return FuncInline.enumLiftPos.UnKnown;
            }
            #endregion

            #region 리프트2
            else
            {


                current = GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.SV04_Lift2].Position;


                if (CheckNearPos(lift, FuncInline.enumLiftPos.RearPassLine))
                {
                    return FuncInline.enumLiftPos.RearPassLine;
                }
                if (CheckNearPos(lift, FuncInline.enumLiftPos.RearNGLine))
                {
                    return FuncInline.enumLiftPos.RearNGLine;
                }
                if (CheckNearPos(lift, FuncInline.enumLiftPos.RearScanPos))
                {
                    return FuncInline.enumLiftPos.RearNGLine;
                }

                for (FuncInline.enumLiftPos pos = FuncInline.enumLiftPos.Site13_F_FT3_Up; pos <= FuncInline.enumLiftPos.Site26_R_FT3_Up; pos++)
                {
                    if (CheckNearPos(lift, pos))
                    {
                        return pos;
                    }
                }
                for (FuncInline.enumLiftPos pos = FuncInline.enumLiftPos.Site13_F_FT3_Down; pos <= FuncInline.enumLiftPos.Site26_R_FT3_Down; pos++)
                {
                    if (CheckNearPos(lift, pos))
                    {
                        return pos;
                    }
                }

                return FuncInline.enumLiftPos.UnKnown;
            }
            #endregion

        }

        // 리프트 가까운 위치 확인 by DG 수정 필요
        public static FuncInline.enumLiftPos CheckNearLiftPos(FuncInline.enumLiftName lift)
        {
            FuncInline.enumLiftPos pos = FuncInline.enumLiftPos.UnKnown;
            double minDist = 9999;
            double current = 0;
            #region Front 리프트
            if (lift == FuncInline.enumLiftName.FrontLift)
            {
                current = GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.SV02_Lift1].Position;

                // site1up~site14up, site1Down~site14Down, Passline1, passline2
                if (Math.Abs(FuncInline.LiftPos[(int)FuncInline.enumLiftName.FrontLift, (int)FuncInline.enumLiftPos.FrontPassLine] - current) < minDist)
                {
                    minDist = Math.Abs(FuncInline.LiftPos[(int)FuncInline.enumLiftName.FrontLift, (int)FuncInline.enumLiftPos.FrontPassLine] - current);
                    pos = FuncInline.enumLiftPos.FrontPassLine;
                }
                if (Math.Abs(FuncInline.LiftPos[(int)FuncInline.enumLiftName.FrontLift, (int)FuncInline.enumLiftPos.FrontScanPos] - current) < minDist)
                {
                    minDist = Math.Abs(FuncInline.LiftPos[(int)FuncInline.enumLiftName.FrontLift, (int)FuncInline.enumLiftPos.FrontScanPos] - current);
                    pos = FuncInline.enumLiftPos.FrontScanPos;
                }
               
                for (FuncInline.enumLiftPos site = FuncInline.enumLiftPos.Site1_F_DT1_Up; site <= FuncInline.enumLiftPos.Site13_F_FT3_Up; site++)
                {
                    if (Math.Abs(FuncInline.LiftPos[(int)FuncInline.enumLiftName.FrontLift, (int)site] - current) < minDist)
                    {
                        minDist = Math.Abs(FuncInline.LiftPos[(int)FuncInline.enumLiftName.FrontLift, (int)site] - current);
                        pos = site;
                    }
                }
                for (FuncInline.enumLiftPos site = FuncInline.enumLiftPos.Site1_F_DT1_Down; site <= FuncInline.enumLiftPos.Site13_F_FT3_Down; site++)
                {
                    if (Math.Abs(FuncInline.LiftPos[(int)FuncInline.enumLiftName.FrontLift, (int)site] - current) < minDist)
                    {
                        minDist = Math.Abs(FuncInline.LiftPos[(int)FuncInline.enumLiftName.FrontLift, (int)site] - current);
                        pos = site;
                    }
                }

                return pos;
            }
            #endregion
            #region Rear 리프트
            else
            {
                current = GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.SV04_Lift2].Position;

                if (Math.Abs(FuncInline.LiftPos[(int)FuncInline.enumLiftName.RearLift, (int)FuncInline.enumLiftPos.RearPassLine] - current) < minDist)
                {
                    minDist = Math.Abs(FuncInline.LiftPos[(int)FuncInline.enumLiftName.FrontLift, (int)FuncInline.enumLiftPos.RearPassLine] - current);
                    pos = FuncInline.enumLiftPos.RearPassLine;
                }
                if (Math.Abs(FuncInline.LiftPos[(int)FuncInline.enumLiftName.RearLift, (int)FuncInline.enumLiftPos.RearNGLine] - current) < minDist)
                {
                    minDist = Math.Abs(FuncInline.LiftPos[(int)FuncInline.enumLiftName.FrontLift, (int)FuncInline.enumLiftPos.RearNGLine] - current);
                    pos = FuncInline.enumLiftPos.RearNGLine;
                }
                if (Math.Abs(FuncInline.LiftPos[(int)FuncInline.enumLiftName.RearLift, (int)FuncInline.enumLiftPos.RearScanPos] - current) < minDist)
                {
                    minDist = Math.Abs(FuncInline.LiftPos[(int)FuncInline.enumLiftName.FrontLift, (int)FuncInline.enumLiftPos.RearScanPos] - current);
                    pos = FuncInline.enumLiftPos.RearScanPos;
                }

                for (FuncInline.enumLiftPos site = FuncInline.enumLiftPos.Site14_R_DT1_Up; site <= FuncInline.enumLiftPos.Site26_R_FT3_Up; site++)
                {
                    if (Math.Abs(FuncInline.LiftPos[(int)FuncInline.enumLiftName.RearLift, (int)site] - current) < minDist)
                    {
                        minDist = Math.Abs(FuncInline.LiftPos[(int)FuncInline.enumLiftName.RearLift, (int)site] - current);
                        pos = site;
                    }
                }
                for (FuncInline.enumLiftPos site = FuncInline.enumLiftPos.Site14_R_DT1_Down; site <= FuncInline.enumLiftPos.Site26_R_FT3_Down; site++)
                {
                    if (Math.Abs(FuncInline.LiftPos[(int)FuncInline.enumLiftName.RearLift, (int)site] - current) < minDist)
                    {
                        minDist = Math.Abs(FuncInline.LiftPos[(int)FuncInline.enumLiftName.RearLift, (int)site] - current);
                        pos = site;
                    }
                }
            }
            

            #endregion

            return pos;
        }

        #region 위치 검증 코드


        // 리프트 지정 위치 인가?
        public static bool CheckNearPos(FuncInline.enumLiftName lift, FuncInline.enumLiftPos pos)
        {
            FuncInline.enumServoAxis axis = FuncInline.enumServoAxis.SV02_Lift1 + (int)lift;
            return GlobalVar.AxisStatus[(int)axis].StandStill &&
                    FuncMotion.CheckNearPos(GlobalVar.AxisStatus[(int)axis].Position,
                                     FuncInline.LiftPos[(int)lift, (int)pos],
                                     0.1);
        }


        public static bool CheckNearPMCWidth(FuncInline.enumPMCAxis axis) // 폭조절 스텝모터가 해당 폭 위치에 있는가?
        {
            double teachingWidth = FuncInline.TeachingWidth[(int)axis];
            double actualWidth = FuncInline.PMCStatus[(int)axis].Position;

            return Math.Abs(actualWidth - teachingWidth) <= 0.1;
        }
        // 오토닉스 스탭모터 위치 판단
        public static bool CheckNearPos(FuncInline.enumPMCAxis axis, bool clamp)
        {
            return FuncInline.PMCStatus[(int)axis].StandStill &&
                    FuncMotion.CheckNearPos(FuncInline.PMCStatus[(int)axis].Position,
                                     FuncInline.TeachingWidth[(int)axis] + (clamp ? 0 : FuncInline.WidthClampOffset), // 
                                     0.1);
        }



        // 지그버퍼 리프트 폭 위치 판단
        public static bool CheckNearPos(FuncInline.enumServoAxis axis, bool clamp)
        {
            return GlobalVar.AxisStatus[(int)axis].StandStill &&
                    FuncMotion.CheckNearPos(GlobalVar.AxisStatus[(int)axis].Position,
                                     FuncInline.TeachingWidth[Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length + 20] + (clamp ? 0 : FuncInline.WidthClampOffset), // 
                                     0.1);
        }


        #endregion



        // 서보 폭조절 모터 초기화되었는가? 위치값이 언클램프 위치
        public static bool CheckWidthInit(FuncInline.enumServoAxis axis)
        {
            return GlobalVar.AxisStatus[(int)axis].StandStill &&
                FuncMotion.CheckNearPos(GlobalVar.AxisStatus[(int)axis].Position,
                                FuncInline.TeachingWidth[Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length + 2] + FuncInline.WidthClampOffset, //??? 이거 확인좀
                                0.1);
        }

        // 오토닉스 스탭모터 초기화 되었는가? 위치값이 언클램프 위치
        public static bool CheckWidthInit(FuncInline.enumPMCAxis axis)
        {
            return CheckNearPos(axis, false);
        }






        public static Stopwatch BeforeTrayInWatch = new Stopwatch(); // 샌딩 전 트레이 공급 시간
        public static Stopwatch AfterTrayInWatch = new Stopwatch(); // 샌딩 후 트레이 공급 시간
        public static frmMain_AutoInline_PC Mainform = Application.OpenForms.OfType<frmMain_AutoInline_PC>().FirstOrDefault();    //frmMain의 컨트롤을 사용하기 위해 by DGKim 230710



        #region 공용 함수. FuncMotion.cs 에서 분기되어 재정의 해야 하는 함수
        static double[] AbsConveyorPos = new double[13];    // 서보별 펄스값 저장용

        public static void SetAbsConveyorPos(int axis, double pos)
        {
            if (axis >= 0 && axis < GlobalVar.Axis_count)
            {
                AbsConveyorPos[axis] = pos;
            }
        }

        #region MoveServo
        public static bool MoveAbsMM(int axis, double dPosMM)
        {
            bool IsStandStill = GlobalVar.AxisStatus[axis].StandStill;

            if (FuncMotion.IsMoving(axis) || !IsStandStill)
            {
                FuncLog.WriteLog($"MoveAbsMM {axis} is not standstill");
                return false;
            }

            if (!GlobalVar.AxisStatus[axis].isHomed)
            {
                FuncLog.WriteLog($"MoveAbsMM {axis} is not Homed");
                return false;
            }

            // 인터락 체크
            if (!ServoInterlockCheck(axis, dPosMM))
            {

                FuncLog.WriteLog($"MoveAbsMM {axis} ServoInterlockCheck failed!\n{FuncInline.Interlock_View}");
                return false;
            }

            // 



            #region 속도 지정
            //#4 하부시 조립 속도
            double speed = 1;   //mm/s
                                //속도 통합
            if (axis == (int)FuncInline.enumServoAxis.SV00_In_Shuttle)
            {
                speed = FuncInline.ServoParamAll.sv0_BeforeLift.speed;
                //if (dPosMM == FuncAmplePacking.ServoParamAll.sv1_LowBush_Z.assemble_pos &&
                //    DIO.GetDORead(FuncAmplePacking.FuncInline.enumDONames.Y09_0_LowBush_Forward))  //전진상태일때
                //{
                //    speed = FuncMotion.MMToPulse(FuncAmplePacking.ServoParamAll.sv1_LowBush_Z.assembly_speed, GlobalVar.ServoGearRatio[(int)axis], GlobalVar.ServoRevMM[(int)axis], GlobalVar.ServoRevPulse[(int)axis]);
                //}
                //else
                //{
                //    speed = FuncMotion.MMToPulse(FuncAmplePacking.ServoParamAll.sv1_LowBush_Z.speed, GlobalVar.ServoGearRatio[(int)axis], GlobalVar.ServoRevMM[(int)axis], GlobalVar.ServoRevPulse[(int)axis]);
                //}
            }
            else if (axis == (int)FuncInline.enumServoAxis.SV01_Out_Shuttle)
            {
                speed = FuncInline.ServoParamAll.sv1_AfterLift.speed;
            }





            // 글로벌 감속비 적용
            speed = speed * (GlobalVar.ServoSpeed / 100.0);
            #endregion //속도 지정

            // 위치 지정
            double PosMM = dPosMM;

            FuncMotion.MoveAbsolute((uint)axis, PosMM, speed);
            return true;
        }


        public static bool MoveBogan(FuncInline.enumServoAxis axisX, FuncInline.enumServoAxis axisY, structPosition pos, double speed, bool wait)
        {

            #region simulation 이면 도착으로 체크
            if (GlobalVar.Simulation)
            {
                GlobalVar.AxisStatus[(int)axisX].Position = pos.x;
                GlobalVar.AxisStatus[(int)axisY].Position = pos.y;
                return true;
            }
            #endregion

            #region if 서보 상태 확인
            if (GlobalVar.AxisStatus[(int)axisX].Errored || //서보 축 에러 있으면 return false
                !GlobalVar.AxisStatus[(int)axisX].StandStill || //이동중인 축이 있으면 return false
                GlobalVar.AxisStatus[(int)axisY].Errored || //서보 축 에러 있으면 return false
                !GlobalVar.AxisStatus[(int)axisY].StandStill) //이동중인 축이 있으면 return false
            {
                return false;
            }
            #endregion

            double xpos = GlobalVar.AxisStatus[(int)axisX].Position;
            double ypos = GlobalVar.AxisStatus[(int)axisY].Position;


            #region 동작 가능 여부
            if (!ServoInterlockCheck((int)axisX, pos.x) ||
                !ServoInterlockCheck((int)axisY, pos.y))
            {
                return false;
            }
            #endregion

            FuncMotion.Double_Axis_Move((int)axisX, pos.x, (int)axisY, pos.y, speed, 1);

            #region 기다리면서 도착확인 (사용안함)
            //if (wait)
            //{
            //    Thread.Sleep(FuncAmplePacking.Servo_Sleep);

            //    ulong chkTime = GlobalVar.TickCount64;
            //    while (GlobalVar.TickCount64 - chkTime < GlobalVar.NormalTimeout * 1000)
            //    {
            //        if (GlobalVar.AxisStatus[(int)axisX].StandStill &&
            //            GlobalVar.AxisStatus[(int)axisY].StandStill)
            //        {
            //            if (FuncMotion.CheckNearPos((int)axisX, xpos, 0.1) &&
            //                FuncMotion.CheckNearPos((int)axisY, ypos, 0.1))
            //            {
            //                //Thread.Sleep(100);
            //                FuncLog.WriteLog("축" + axisX + "," + axisY + " :" + pos.x + "," + pos.y);
            //                //FuncMotion.MoveStop((int)axis);
            //                return true;
            //            }
            //            else
            //            {
            //                FuncMotion.Double_Axis_Move((int)axisX, pos.x, (int)axisY, pos.y, speed, 1);
            //            }
            //        }
            //        Thread.Sleep(100);
            //    }
            //}

            #endregion

            return true; // 이하 return false;        
        }

        // JHRYU : 컨베이어 이동 전용 함수
        public static bool MoveConveyorMM(FuncInline.enumServoAxis eAxis, double step)
        {
            int axis = (int)eAxis;
            bool IsStandStill = GlobalVar.AxisStatus[axis].StandStill;

            if (FuncMotion.IsMoving(axis) || !IsStandStill)
            {
                FuncLog.WriteLog($"MoveConveyorMM {axis} is not standstill");
                return false;
            }

            if (!GlobalVar.AxisStatus[axis].isHomed)
            {
                FuncLog.WriteLog($"MoveConveyorMM {axis} is not Homed");
                return false;
            }

            // 인터락 체크
            if (!ServoInterlockCheck(axis, step))
            {
                FuncLog.WriteLog($"MoveConveyorMM {axis} ServoInterlockCheck failed!\n{FuncInline.Interlock_View}");
                return false;
            }



            // 속도 지정
            double speed = 0;
            //if (eAxis == FuncAmplePacking.enumServoAxis.SV00_Main_Conveyor)
            //{
            //    //FuncAmplePacking.ServoParamAll.sv0_Conveyor.speed = 10;
            //    speed = FuncMotion.MMToPulse(FuncAmplePacking.ServoParamAll.sv0_Conveyor.speed, GlobalVar.ServoGearRatio[(int)eAxis], GlobalVar.ServoRevMM[(int)eAxis], GlobalVar.ServoRevPulse[(int)eAxis]);
            //}
            //else
            //{
            //    FuncLog.WriteLog("MoveConveyor axis is not Conveyor");
            //    return false;
            //}

            double dActPos = 0.0;
            double current = AbsConveyorPos[axis];
            // MM 단위를 펄스로 환산
            step = FuncMotion.MMToPulse(step, GlobalVar.ServoGearRatio[axis], GlobalVar.ServoRevMM[axis], GlobalVar.ServoRevPulse[axis]);

            double nextPos = current + step;

            //// 안전 장치
            //CAXM.AxmStatusGetActPos(axis, ref dActPos);
            //// step 의 1/10 이상 차이나면 위치 에러상태로 판단
            //if ( Math.Abs( current - dActPos ) > (step / 10.0) )
            //{
            //    FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
            //     DateTime.Now.ToString("HH:mm:ss"),
            //     FuncInline.enumErrorPart.System,
            //     enumErrorCode.Conveyor_Position_Error,
            //     false,
            //     "FuncBoxPackingMove.MoveConveyor Position Error"));
            //    return false;
            //}

            if (!IsConveyorArrived(eAxis))
            {
                // 컨베이어가 1mm 이상 틀어졌다.
                string msg = eAxis.ToString() + " : 컨베이어가 정위치가 아닙니다. 재초기화 필요";
                //테스트 때문에 막음 by DG 240821
                FuncInline.AddError((int)enumErrorCode.Conveyor_Position_Error, msg);
                return false;
            }

            // conveyor position overflow 방지 => 이 코드는 위험하다. 테스트 필요함
            //if (nextPos > Int32.MaxValue)

            if (nextPos > (290 * 1600000))     // 대충 290번 스탭하면 포지션을 초기화한다. 10바퀴 돌면 한번 초기화
            {
                // 현재 위치를 원점으로 설정한다. 
                CAXM.AxmStatusSetActPos(axis, 0.0);
                CAXM.AxmStatusSetCmdPos(axis, 0.0);

                CAXM.AxmStatusGetActPos(axis, ref dActPos);     // dActPos 가 0.0 이 정상
                nextPos = dActPos + step;

                FuncLog.WriteLog($"MoveConveyor Axis{axis} ACTPOS RESET 0 dActPos={dActPos} next={nextPos}");
            }

            //AbsConveyorPos[axis] = nextPos;
            SetAbsConveyorPos(axis, nextPos);



            // 글로벌 감속비 적용
            speed = speed * (GlobalVar.ServoSpeed / 100.0);
            //MAX_VELOCITY.        =700000
            double vel = speed;

            FuncMotion.MoveAbsolute((uint)axis, nextPos, vel);
            return true;
        }

        #endregion MoveServo


        public static bool IsConveyorArrived(FuncInline.enumServoAxis eAxis)
        {
            int axis = (int)eAxis;
            if (GlobalVar.Simulation) return true;

            if (FuncMotion.IsMoving(axis)) return false;

            //if (eAxis != FuncAmplePacking.enumServoAxis.SV00_Main_Conveyor)
            //    return false;

            double current = GlobalVar.AxisStatus[axis].Position;
            double targetPulse = AbsConveyorPos[axis];
            double target = FuncMotion.PulseToMM((long)targetPulse, GlobalVar.ServoGearRatio[(int)axis], GlobalVar.ServoRevMM[(int)axis], GlobalVar.ServoRevPulse[(int)axis]);
            bool IsStandStill = GlobalVar.AxisStatus[axis].StandStill;
            double diff = Math.Abs(current - target);

            if (IsStandStill && (diff < 0.01)) return true;
            return false;
        }
        // axis 축이 posMM 위치에 도달했는지 확인하는 함수
        public static bool IsArrived(int axis, double posMM)
        {
            if (GlobalVar.Simulation) return true;

            if (FuncMotion.IsMoving(axis)) return false;
            bool IsStandStill = GlobalVar.AxisStatus[axis].StandStill;
            double diff = Math.Abs(posMM - GlobalVar.AxisStatus[axis].Position);
            if (IsStandStill && (diff < 0.01)) return true;
            return false;
        }

        public static bool IsSamePos(int axis, double posMM)
        {
            double diff = Math.Abs(posMM - GlobalVar.AxisStatus[axis].Position);
            if (diff < 0.01)
            {
                return true;
            }
            return false;
        }
        #endregion 공용 함수


        #region 실린더 에러 위치 체크후 Process 리셋
        public static void CylinderProcessReset()
        {
        }
        #endregion 실린더 에러 위치 체크후 Process 리셋


        #region 메뉴얼 동작시 상태 체크하여 Servo 위치를 조정했으면 위치 복귀를 위해 그전 상태로 돌림
        public static void JogMoveCheck_StateChange(int position)   //0 = before 1= after
        {


        }
        #endregion


        // JHRYU : 컨베이어 이동 전용 함수
        public static bool ServoInterlockCheck(int axis, double dPosMM)
        {
            FuncInline.Interlock_View = "";
            if (GlobalVar.Simulation) return true;
            // 인터락 체크

            // 안전상 이동 금지
            if (GlobalVar.SystemStatus == enumSystemStatus.EmgStop ||
                GlobalVar.E_Stop ||
                GlobalVar.DoorOpen ||
                GlobalVar.ProjectClass == null)
            {
                FuncInline.Interlock_View = "E_Stop / 도어오픈 에러 상태";
                return false;
            }

            switch ((FuncInline.enumServoAxis)axis)
            {
                #region SV00샌딩전 리프트
                case FuncInline.enumServoAxis.SV00_In_Shuttle:
                    List<string> Interlock = new List<string>();
                    FuncInline.Interlock_View = "";

                    //int SV00_Input_Tray_Lift = (int)FuncInline.enumServoAxis.SV00_Input_Tray_Lift;
                    //double current_pos = GlobalVar.AxisStatus[SV00_Input_Tray_Lift].Position;

                    //int currentFloor = ((AutoInline_Class)GlobalVar.ProjectClass).beforeLift01.currentFloor;

                    ////컨베이어 클램프 인터락 체크
                    //if (DIO.GetDOData((int)FuncInline.enumDONames.Y01_0_Before_Tray_Lift_Conveyor_CW))
                    //{
                    //    Interlock.Add("SV00_Input_Tray_Lift 이동불가 - 컨베이어 동작중(Y01_0)");
                    //    //FuncAmplePacking.LogView("SV00_Main_Conveyor 이동불가 - X04_1_#0왼쪽 컨베이어 클램프 센서 확인");
                    //}


                    //동작전 (1~10층)정위치일때 리프트의 트레이 센서 감지되면 무조건 정지

                    if (Interlock.Count > 0)
                    {
                        // 줄바꿈(\n)으로 구분하여 문자열로 반환
                        FuncInline.Interlock_View = string.Join("\n", Interlock);
                        return false;
                    }

                    break;
                    #endregion




            }

            return true;
        }

    }
}
