using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms; //HJ 수정 200406 메시지 창을 띄우기 위해(Ecat.cs)
using Radix.Library.LestExplorer;

namespace Radix
{
    /**
     * @brief 모션 실행 및 체크 등 함수 선언
     */
    public static class FuncLetsMotion
    {
        /*
         * FuncMotion.cs : 모션 실행 및 체크 등 함수 선언
         */


        private static void debug(string str)
        {
            Util.Debug("FuncMotion : " + str);
        }

        #region 로컬 변수 ㅣets


        public static bool initialized = false; //스탭모터와 연결 여부 
        public static bool Scan_initialized = false; // 스캔 여부 
        private const string DefaultIp = "192.168.10.1"; //스탭모터 연결PC IP설정(PC고정 IP, 20,21,22
        public static Dictionary<string, List<int>> LetsIpAixs = new Dictionary<string, List<int>>();
        #endregion

        #region 모션 관련

       
        public static FuncInline.enumTeachingPos ErrorPartToTeachingPos(FuncInline.enumErrorPart part)
        {
            switch (part)
            {
                case FuncInline.enumErrorPart.InConveyor:
                    return FuncInline.enumTeachingPos.InConveyor;
                case FuncInline.enumErrorPart.InShuttle:
                    return FuncInline.enumTeachingPos.InShuttle;
                case FuncInline.enumErrorPart.NgBuffer:
                    return FuncInline.enumTeachingPos.NgBuffer;
                case FuncInline.enumErrorPart.OutShuttle_Up:
                    return FuncInline.enumTeachingPos.OutShuttle_Up;
                case FuncInline.enumErrorPart.OutConveyor:
                    return FuncInline.enumTeachingPos.OutConveyor;
                case FuncInline.enumErrorPart.FrontPassLine:
                    return FuncInline.enumTeachingPos.FrontPassLine;
                case FuncInline.enumErrorPart.RearPassLine:
                    return FuncInline.enumTeachingPos.RearPassLine;
                case FuncInline.enumErrorPart.RearNGLine:
                    return FuncInline.enumTeachingPos.RearNGLine;
                case FuncInline.enumErrorPart.Lift1_Up:
                    return FuncInline.enumTeachingPos.Lift1_Up;
                case FuncInline.enumErrorPart.Lift1_Down:
                    return FuncInline.enumTeachingPos.Lift1_Down;
                case FuncInline.enumErrorPart.Lift2_Up:
                    return FuncInline.enumTeachingPos.Lift2_Up;
                case FuncInline.enumErrorPart.Lift2_Down:
                    return FuncInline.enumTeachingPos.Lift2_Down;
                case FuncInline.enumErrorPart.FrontScanSite:
                    return FuncInline.enumTeachingPos.FrontScanSite;

                case FuncInline.enumErrorPart.Site1_F_DT1:
                case FuncInline.enumErrorPart.Site2_F_DT2:
                case FuncInline.enumErrorPart.Site3_F_DT3:
                case FuncInline.enumErrorPart.Site4_F_DT4:
                case FuncInline.enumErrorPart.Site5_F_DT5:
                case FuncInline.enumErrorPart.Site6_F_DT6:
                case FuncInline.enumErrorPart.Site7_F_DT7:
                case FuncInline.enumErrorPart.Site8_F_DT8:
                case FuncInline.enumErrorPart.Site9_F_DT9:
                case FuncInline.enumErrorPart.Site10_F_DT10_FT4:
                case FuncInline.enumErrorPart.Site11_F_FT1:
                case FuncInline.enumErrorPart.Site12_F_FT2:
                case FuncInline.enumErrorPart.Site13_F_FT3:
                case FuncInline.enumErrorPart.Site14_R_DT1:
                case FuncInline.enumErrorPart.Site15_R_DT2:
                case FuncInline.enumErrorPart.Site16_R_DT3:
                case FuncInline.enumErrorPart.Site17_R_DT4:
                case FuncInline.enumErrorPart.Site18_R_DT5:
                case FuncInline.enumErrorPart.Site19_R_DT6:
                case FuncInline.enumErrorPart.Site20_R_DT7:
                case FuncInline.enumErrorPart.Site21_R_DT8:
                case FuncInline.enumErrorPart.Site22_R_DT9:
                case FuncInline.enumErrorPart.Site23_R_DT10_FT4:
                case FuncInline.enumErrorPart.Site24_R_FT1:
                case FuncInline.enumErrorPart.Site25_R_FT2:
                case FuncInline.enumErrorPart.Site26_R_FT3:
                    return FuncInline.enumTeachingPos.Site1_F_DT1 + (int)part - (int)FuncInline.enumErrorPart.Site1_F_DT1;
                default:
                    return FuncInline.enumTeachingPos.None;
            }
        }

        public static FuncInline.enumErrorPart TeachingPosToErrorPart(FuncInline.enumTeachingPos pos)
        {
            switch (pos)
            {
                case FuncInline.enumTeachingPos.InConveyor:
                    return FuncInline.enumErrorPart.InConveyor;
                case FuncInline.enumTeachingPos.InShuttle:
                    return FuncInline.enumErrorPart.InShuttle;
                case FuncInline.enumTeachingPos.NgBuffer:
                    return FuncInline.enumErrorPart.NgBuffer;
                case FuncInline.enumTeachingPos.OutShuttle_Up:
                    return FuncInline.enumErrorPart.OutShuttle_Up;
                case FuncInline.enumTeachingPos.OutConveyor:
                    return FuncInline.enumErrorPart.OutConveyor;
                case FuncInline.enumTeachingPos.FrontPassLine:
                    return FuncInline.enumErrorPart.FrontPassLine;
                case FuncInline.enumTeachingPos.RearPassLine:
                    return FuncInline.enumErrorPart.RearPassLine;
                case FuncInline.enumTeachingPos.RearNGLine:
                    return FuncInline.enumErrorPart.RearNGLine;
                case FuncInline.enumTeachingPos.Lift1_Up:
                    return FuncInline.enumErrorPart.Lift1_Up;
                case FuncInline.enumTeachingPos.Lift1_Down:
                    return FuncInline.enumErrorPart.Lift1_Down;
                case FuncInline.enumTeachingPos.Lift2_Up:
                    return FuncInline.enumErrorPart.Lift2_Up;
                case FuncInline.enumTeachingPos.Lift2_Down:
                    return FuncInline.enumErrorPart.Lift2_Down;
                case FuncInline.enumTeachingPos.FrontScanSite:
                    return FuncInline.enumErrorPart.FrontScanSite;

                case FuncInline.enumTeachingPos.Site1_F_DT1:
                case FuncInline.enumTeachingPos.Site2_F_DT2:
                case FuncInline.enumTeachingPos.Site3_F_DT3:
                case FuncInline.enumTeachingPos.Site4_F_DT4:
                case FuncInline.enumTeachingPos.Site5_F_DT5:
                case FuncInline.enumTeachingPos.Site6_F_DT6:
                case FuncInline.enumTeachingPos.Site7_F_DT7:
                case FuncInline.enumTeachingPos.Site8_F_DT8:
                case FuncInline.enumTeachingPos.Site9_F_DT9:
                case FuncInline.enumTeachingPos.Site10_F_DT10_FT4:
                case FuncInline.enumTeachingPos.Site11_F_FT1:
                case FuncInline.enumTeachingPos.Site12_F_FT2:
                case FuncInline.enumTeachingPos.Site13_F_FT3:
                case FuncInline.enumTeachingPos.Site14_R_DT1:
                case FuncInline.enumTeachingPos.Site15_R_DT2:
                case FuncInline.enumTeachingPos.Site16_R_DT3:
                case FuncInline.enumTeachingPos.Site17_R_DT4:
                case FuncInline.enumTeachingPos.Site18_R_DT5:
                case FuncInline.enumTeachingPos.Site19_R_DT6:
                case FuncInline.enumTeachingPos.Site20_R_DT7:
                case FuncInline.enumTeachingPos.Site21_R_DT8:
                case FuncInline.enumTeachingPos.Site22_R_DT9:
                case FuncInline.enumTeachingPos.Site23_R_DT10_FT4:
                case FuncInline.enumTeachingPos.Site24_R_FT1:
                case FuncInline.enumTeachingPos.Site25_R_FT2:
                case FuncInline.enumTeachingPos.Site26_R_FT3:
                    return FuncInline.enumErrorPart.Site1_F_DT1 + (int)pos - (int)FuncInline.enumErrorPart.Site1_F_DT1;
                default:
                    return FuncInline.enumErrorPart.No_Error;


            }
        }

        /**
         * @brief Autonics 폭조절 모터 자체의 거리 계산값을 기구상의 실제 거리로 환산
         *          PCB 폭값과 같이 기구상에서는 사용자 좌표계로 사용하므로 사용자 거리값을 계산한다.
         * @param axis 축이름
         * @param pos 계산할 위치값
         * @return double 기구상 사용자 좌표
         */
        public static double CalcLetsWidthPos(int axis, double pos) // 일반 폭조절 모터 자체의 거리 계산값을 기구상의 실제 거리로 환산 - 위치값 확인할 때
        {
            // 축별 역방향 여부 확인
            bool rev = (axis >= 0 && axis < FuncInline.WidthDisplayReverse.Length) && FuncInline.WidthDisplayReverse[axis];
            double signedPos = rev ? -pos : pos; // 역방향이면 부호 반전

            switch (axis)
            {
                case 0: // In Shuttle
                case 1: // Out Shuttle
                case 2: //
                case 3: // Rack1
                case 4: // NG 버퍼
                    return FuncInline.DefaultPCBWidth + signedPos; // DefaultPCBWidth = 240
                default:
                    // 기존 else: pos 그대로 반환 → 역방향이면 -pos
                    return FuncInline.DefaultPCBWidth + signedPos;
            }
        }

        /**
         * @brief Autonics 폭조절 모터 실제 거리를 기구상의 모터 자체의 거리 계산값으로 환산
         *          PCB 폭과 같은 사용자 거리값을 모터 자체의 원점 기준 거리로 역환산한다.
         * @param axis 축이름
         * @param pos 계산할 사용자 위치값
         * @return double 원점 대비 모터상 거리값
         */
        public static double ReCalcLetsWidthPos(int axis, double pos) // 일반 폭조절 실제 거리를 기구상의 모터 자체의 거리 계산값으로 환산 - 지령 날릴 때
        {
            // pos: 화면에서 사용하는 "실제 거리" (예: 240 기준 표시값, Offset 미적용 값)
            bool rev = (axis >= 0 && axis < FuncInline.WidthDisplayReverse.Length) && FuncInline.WidthDisplayReverse[axis];

            switch (axis)
            {
                // 240(=DefaultPCBWidth) 기준으로 표시하는 축들
                case 0: // In Shuttle
                case 1: // Out Shuttle
                case 2:
                case 3: // Rack1
                case 4: // NG 버퍼
                    {
                        double delta = pos - FuncInline.DefaultPCBWidth; // (표시 - 240)
                        double motorMm = rev ? -delta : delta;           // 역방향이면 부호 반전
                        return motorMm;                                  // 모터에 보낼 "기구상의 mm"
                    }

             
                default:
                    {
                        double delta = pos - FuncInline.DefaultPCBWidth; // (표시 - 240)
                        double motorMm = rev ? -pos : pos;
                        return motorMm;
                    }
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
                    return pos;
                case FuncInline.enumServoAxis.SV01_Out_Shuttle:
                    return pos;
                case FuncInline.enumServoAxis.SV02_Lift1:
                    return pos;
                case FuncInline.enumServoAxis.SV03_Rack1_Width:
                    return pos;
                case FuncInline.enumServoAxis.SV04_Lift2:
                    return pos;
                case FuncInline.enumServoAxis.SV05_Rack2_Width:
                    return pos;
                case FuncInline.enumServoAxis.SV06_Scan_Y:
                    return pos;
                case FuncInline.enumServoAxis.SV07_Scan_X:
                    return pos;

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
                    return pos;
                case FuncInline.enumServoAxis.SV01_Out_Shuttle:
                    return pos;
                case FuncInline.enumServoAxis.SV02_Lift1:
                    return pos;
                case FuncInline.enumServoAxis.SV03_Rack1_Width:
                    return pos;
                case FuncInline.enumServoAxis.SV04_Lift2:
                    return pos;
                case FuncInline.enumServoAxis.SV05_Rack2_Width:
                    return pos;
                case FuncInline.enumServoAxis.SV06_Scan_Y:
                    return pos;
                case FuncInline.enumServoAxis.SV07_Scan_X:
                    return pos;

                default:
                    return pos;
            }
        }

        #region 스텝모터 위치값 환산

        /**
         * @brief 직교로봇 두 좌표간에 근접했나?
         * @param servoIndex 서보모터 순번
         * @param pos 체크할 위치값
         * @param gap 근접 간주할 거리
         * @return bool 범위 안 True
         *      범위 외 False
         */
        public static bool CheckNearPos(int axis, double pos, double gap) //CheckNearPos : 두 좌표간에 근접했나?
        {
            return Math.Abs(GlobalVar.LetsAxisStatus[axis].Position - pos) <= gap;
        }

        public static bool CheckNearPos(double pos1, double pos2, double gap)
        {
            return Math.Abs(pos1 - pos2) <= gap;
        }



        /**
         * @brief 직교로봇 z축 두개 인 경우 두 좌표간에 근접했나?
         * @param destPos 1번축
         * @param pos2 2번축
         * @param z2 모듈인라인 z축이 두개이므로 별도로 지정
         * @param gap 근접 간주할 거리
         * @return bool 범위 안 True
         *      범위 외 False
         */
        public static bool CheckNearPos(structPosition destPos, structPosition pos2, double z2, double gap) //CheckNearPos : 두 좌표간에 근접했나? 모듈인라인 z축이 두개
        {
            return Math.Abs(destPos.x - pos2.x) < gap &&
                        Math.Abs(destPos.y - pos2.y) < gap &&
                        Math.Abs(destPos.z - pos2.z) < gap &&
                        Math.Abs(destPos.z - z2) < gap;
        }


        /**
       * @brief 기구상 실제 속도값으로 실지령 pulse 계산
       * @param axis 축순번
       * @param speed 기구상 속도값
       * @return double 지령에 사용할 속도 펄스
       */
        public static double GetRealSpeed(FuncInline.enumLetsAxis LetsAxis, double speed) // 프로그램 UI연결할 실제 좌표값으로 실지령 pulse 계산
        {
            return MMToPulse((long)speed,
                                            GlobalVar.WidthGearRatio[(int)LetsAxis],
                                            GlobalVar.WidthRevMM[(int)LetsAxis],
                                            GlobalVar.WidthRevPulse[(int)LetsAxis]);
        }

        /**
         * @brief 프로그램 UI연결할 실제 좌표값으로부터 실지령 pulse 계산
         * @param axis 축순번
         * @param pos UI표시된 사용자 좌표값
         * @return double 범위 안 True
         *      범위 외 False
         */
        public static int GetRealPulse(FuncInline.enumLetsAxis LetsAxis, double pos)
        {
            return (int)FuncMotion.MMToPulse((double)ReCalcLetsWidthPos((int)LetsAxis, pos),
                                            GlobalVar.WidthGearRatio[(int)LetsAxis],
                                            GlobalVar.WidthRevMM[(int)LetsAxis],
                                            GlobalVar.WidthRevPulse[(int)LetsAxis]);
        }


        /**
         * @brief 지정 축 위치를 실제 사용자에게 표시할 좌표계로 환산 출력
         * @param axis 축순번
         * @return double 사용자 좌표 또는 거리값
         */
        public static double GetRealPos(FuncInline.enumLetsAxis LetsAxis, double pos) // PMCThread에서 이 함수 호출해서 환산된 실거리를 저장한다.
        {
            return CalcLetsWidthPos((int)LetsAxis,
                                     FuncMotion.PulseToMM((double)pos,
                                            GlobalVar.WidthGearRatio[(int)LetsAxis],
                                            GlobalVar.WidthRevMM[(int)LetsAxis],
                                            GlobalVar.WidthRevPulse[(int)LetsAxis]));
        }

        /**
         * @brief 펄스,기어비,회전당거리,회전당펄스로 서보가 이동할 거리 계산
         * @param Pulse 지령 펄스
         * @param GearRatio 기어비. 축회전/모터회전
         * @param mmPerRev 축 1회전시 이동 거리
         * @param pulsePerRev 모터 한 바퀴 돌리는 데 필요한 펄스
         * @return double 기구상 이동 거리
         */
        public static double PulseToMM(double Pulse, double GearRatio, double mmPerRev, double pulsePerRev) // PulseToMM(펄스, 기어비, 회전당mm, 회전당펄스) servo pulse를 mm로 환산
        {
            //Debug("PulseToMM pulse : " + Pulse.ToString());
            double pulse = (double)Pulse * mmPerRev / pulsePerRev / GearRatio;
            //Debug("result : " + pulse.ToString());
            return pulse;
        }

        /**
         * @brief 펄스,기어비,회전당거리,회전당펄스로 모터 지령에 사용할 펄스값 계산
         * @param MM 이동할 거리 또는 위치값
         * @param GearRatio 기어비. 축회전/모터회전
         * @param mmPerRev 축 1회전시 이동 거리
         * @param pulsePerRev 모터 한 바퀴 돌리는 데 필요한 펄스
         * @return double 모터상 지령 펄스
         */
        public static double MMToPulse(double MM, double GearRatio, double mmPerRev, double pulsePerRev) // MMToPulse(mm, 기어비, 회전당mm, 회전당펄스) mm를 servo pulse로 환산
        {
            return MM * GearRatio * pulsePerRev / mmPerRev;


        }

        /**
         * @brief 펄스,기어비,회전당펄스로 UI에 사용할 사용자 각도값 또는 실제 각도값 계산
         * @param Pulse 모터 위치값 펄스
         * @param GearRatio 기어비. 축회전/모터회전
         * @param pulsePerRev 모터 한 바퀴 돌리는 데 필요한 펄스
         * @return double 기구상 사용자 각도값
         */
        public static double PulseToDegree(double Pulse, double GearRatio, double pulsePerRev) // PulseToMM(펄스, 기어비, 회전당펄스) servo pulse를 각도로 환산
        {
            return Pulse * 360 / pulsePerRev / GearRatio;
        }

        /**
         * @brief 펄스,기어비,회전당펄스로 지정 각도 회전에 필요한 펄스 수 계산
         * @param Degree 기구상 이동할 각도
         * @param GearRatio 기어비. 축회전/모터회전
         * @param pulsePerRev 모터 한 바퀴 돌리는 데 필요한 펄스
         * @return double 모터 지령 펄스
         */
        public static double DegreeToPulse(double Degree, double GearRatio, double pulsePerRev) // MMToPulse(mm, 기어비, 회전당펄스) 각도를 servo pulse로 환산
        {
            return Degree * GearRatio * pulsePerRev / 360;
        }
        #endregion



       

        /**
         * @brief 모든 모션장치 정지
         *      서보 모터 및 스탭모터 전체 정지한다
         */
        public static void StopAllJog() // 모든 모션장치 정지
        {
            #region 모든 서보모터
            for (int i = 0; i < GlobalVar.AxisStatus.Length; i++)
            {
                FuncMotion.MoveStop(i);
            }
            #endregion

            #region 모든 스텝모터
            //for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumLetsAxis)).Length; i++)
            //{
            //    if (//PMCClass.GetCurrSpeed((FuncInline.enumLetsAxis)i) > 1)
            //    {
            //        //PMCClass.Stop((FuncInline.enumLetsAxis)i);
            //    }
            //}
            #endregion
        }





        #region 스텝 모터 제어
    


        public static bool HomeRun(int LetsAxis) // Homing 실행
        {
            try
            {
                int raw = GlobalVar.LetsAxis[(int)LetsAxis];

                int mask = 0;
                int mAll = 0;

                int mOrg = 0;
                int mMove = 0;
                double position = 0;
                // 상태 마스크
                mAll = LetsExplorerDll.GetAxisState_All();
                mOrg = LetsExplorerDll.GetAxisState_ORG();
                mMove = LetsExplorerDll.GetAxisState_MOVING();

                mask = LetsExplorerDll.GetState(raw, mAll, LetsExplorerDll.GetNodeType(raw));
                GlobalVar.LetsAxisStatus[LetsAxis].StandStill = (mask & mMove) == 0;  // 정지상태 여부
                //GlobalVar.LetsAxisStatus[LetsAxis].isHomed = (mask & mOrg) != 0 && GlobalVar.LetsAxisStatus[LetsAxis].StandStill;

                if (GlobalVar.Simulation)
                {
                    GlobalVar.LetsAxisStatus[(int)LetsAxis].StandStill = true;
                    GlobalVar.LetsAxisStatus[(int)LetsAxis].Velocity = 0;
                    GlobalVar.LetsAxisStatus[(int)LetsAxis].Position = FuncInline.DefaultPCBWidth - FuncInline.OffsetWidth[(int)LetsAxis];
                    GlobalVar.LetsAxisStatus[(int)LetsAxis].isHomed = true;
                    GlobalVar.LetsAxisStatus[(int)LetsAxis].Homing = false;
                    return true;
                }
               
                
                if (!GlobalVar.LetsAxisStatus[(int)LetsAxis].StandStill)
                {
                    FuncLog.WriteLog($"Lets Homeing Fail, Now Moveing");
                    return false;
                }
                //if (GlobalVar.LetsAxisStatus[(int)LetsAxis].Homing)
                //{
                //    FuncLog.WriteLog($"LetsMotion {LetsAxis.ToString()} Already homing");
                //    return false;
                //}


                

                //// 2축 보드만
                //int groupIndex = (raw >> 24) & 0xFF;
                //if (groupIndex >= 2)
                //{
                //    FuncLog.WriteLog($"LetsMotion Axis input Error");
                //    return false;
                //}

                // 2) 방향 결정
                int direction = 1; // +방향

                FuncInline.enumLetsAxis Name = (FuncInline.enumLetsAxis)LetsAxis;

                switch (LetsAxis)
                {
                    case (int)FuncInline.enumLetsAxis.ST00_InShuttle_Width:
                        direction = 0;  //-방향
                        break;
                    case (int)FuncInline.enumLetsAxis.ST01_OutShuttle_Width:
                        direction = 0;  //-방향
                        break;
                    case (int)FuncInline.enumLetsAxis.ST02_OutConveyor_Width:
                        direction = 0;  //-방향
                        break;
                    case (int)FuncInline.enumLetsAxis.ST03_InConveyor_Width:
                        direction = 0;  //-방향
                        break;
                    case (int)FuncInline.enumLetsAxis.ST04_NGBuffer:
                        direction = 0;  //-방향
                        break;
                    default:
                        break;
                }


                // 3) 속도·가속도 파싱
                double initVel = 1000;
                double endVel = initVel;
                double acc = initVel * 10;


                
                // 홈 시작 시 폴링 딜레이 증가 (예: 200ms 추가)
                LetsStatusThread.ExtraSleepMs = 1000;
                GlobalVar.LetsAxisStatus[(int)LetsAxis].isHomed = false;
                GlobalVar.LetsAxisStatus[(int)LetsAxis].Homing = true;

                //Thread.Sleep(500);
                FuncLog.WriteLog($"LetsMotion {LetsAxis.ToString()} raw=0x{raw:X8}, dir={direction}, init={initVel}, end={endVel}, acc={acc}");

                //int scanCount = LetsExplorerDll.ScanNodeList();
                //if (scanCount <= 0)
                //{
                //    Scan_initialized = false;
                //    MessageBox.Show($"Scan Fail: {scanCount}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                //    return false;
                //}

                //// 2) GetNodeList 호출 (이제 rawNodeInfo 리스트만 들어 있음)
                //int[] nodes = new int[scanCount];
                //int nodeCont = LetsExplorerDll.GetNodeList(nodes, scanCount);

                //int[] handles = new int[32];
                //int nodeCont = LetsExplorerDll.GetNodeList(handles, handles.Length);
                //for (int i = 0; i < nodeCont; i++)
                //{
                //    FuncLog.WriteLog($"등록된 handle[{i}]: 0x{handles[i]:X8}");
                //}

                int res = LetsExplorerDll.StartHome2Axis(raw, direction, initVel, endVel, acc, 0.0);

                //Thread.Sleep(100);

                if (res == 0)
                {
                    FuncLog.WriteLog($"Home Command OK Axis: {Name}");
                    //mask = 0;
                    //mAll = 0;

                    //mOrg = 0;
                    //mMove = 0;
                    //position = 0;
                    //// 상태 마스크
                    //mAll = LetsExplorerDll.GetAxisState_All();
                    //mOrg = LetsExplorerDll.GetAxisState_ORG();
                    //mMove = LetsExplorerDll.GetAxisState_MOVING();

                    //mask = LetsExplorerDll.GetState(raw, mAll, LetsExplorerDll.GetNodeType(raw));
                    //GlobalVar.LetsAxisStatus[LetsAxis].StandStill = (mask & mMove) == 0;  // 정지상태 여부
                    //GlobalVar.LetsAxisStatus[LetsAxis].isHomed = (mask & mOrg) != 0 && GlobalVar.LetsAxisStatus[LetsAxis].StandStill;

                    //if (GlobalVar.LetsAxisStatus[LetsAxis].isHomed) //센서 감지되면 완료
                    //{
                        GlobalVar.LetsAxisStatus[(int)LetsAxis].Homing = false;

                        switch (LetsAxis)
                        {
                            case 0:
                                FuncInline.InitialDone[(int)FuncInline.enumInitialize.InShuttle] = true;
                                break;
                            case 1:
                                FuncInline.InitialDone[(int)FuncInline.enumInitialize.OutShuttle] = true;
                                break;
                            case 2:
                                FuncInline.InitialDone[(int)FuncInline.enumInitialize.OutConveyor] = true;
                                break;
                            case 3:
                                FuncInline.InitialDone[(int)FuncInline.enumInitialize.InConveyor] = true;
                                break;
                            case 4:
                                FuncInline.InitialDone[(int)FuncInline.enumInitialize.NgBuffer] = true;
                                break;

                            default:
                                break;
                        }

                    //}



                }
                else
                {
                    Thread.Sleep(500);
                    FuncLog.WriteLog($"홈 실패: {Name}");
                    return HomeRun(LetsAxis);
                    return false;
                }

               

              
            }
            catch (Exception ex)
            {
                GlobalVar.LetsAxisStatus[(int)LetsAxis].isHomed = false;
                GlobalVar.LetsAxisStatus[(int)LetsAxis].Homing = false;
                FuncLog.WriteLog($"{ex}");
            }
            finally
            {
                // 홈 끝나면 원래 폴링 속도로 복귀
                //LetsStatusThread.ExtraSleepMs = 0;
                
            }
            return true;


        }

        public static bool INCMove(int letsAxis, double pos, double speed) // 상대좌표. 직접 호출하지 말고 PMCThread 통해서 할 것
        {
            if (GlobalVar.GlobalStop ||
               GlobalVar.E_Stop)
            {
                return false;
            }
            if (!GlobalVar.LetsAxisStatus[(int)letsAxis].StandStill)
            {
                return false;
            }
            //debug("ABSMove : " + pmcAxis.ToString() + " - " + pos + "," + speed);
            if (GlobalVar.Simulation)
            {
                GlobalVar.LetsAxisStatus[(int)letsAxis].Position = pos - FuncInline.OffsetWidth[(int)letsAxis];
                GlobalVar.LetsAxisStatus[(int)letsAxis].StandStill = true;
                GlobalVar.LetsAxisStatus[(int)letsAxis].Velocity = 0;
                GlobalVar.LetsAxisStatus[(int)letsAxis].isHomed = GlobalVar.LetsAxisStatus[(int)letsAxis].Homing;
                return true;
            }


            int raw = GlobalVar.LetsAxis[(int)letsAxis];

            double position, velocity, acceleration, jerk;
            position = GetRealPulse((FuncInline.enumLetsAxis)letsAxis, pos);
            velocity = GetRealSpeed((FuncInline.enumLetsAxis)letsAxis, speed);
            acceleration = speed * 10;
            jerk = acceleration * 10;

           
            int result = LetsExplorerDll.MoveRel(raw, position, velocity, acceleration, jerk);
            if (result == 0)
            {
            }
            else
            {
                FuncLog.WriteLog($"MoveINC Comand Fail");
                return false;
            }


        
            return true;
        }

        public static bool ABSMove(int letsAxis, double pos, double speed) // 절대좌표
        {
            if (GlobalVar.GlobalStop ||
                GlobalVar.E_Stop)
            {
                return false;
            }
            if (!GlobalVar.LetsAxisStatus[(int)letsAxis].StandStill)
            {
                return false;
            }
            //debug("ABSMove : " + pmcAxis.ToString() + " - " + pos + "," + speed);
            if (GlobalVar.Simulation)
            {
                GlobalVar.LetsAxisStatus[(int)letsAxis].Position = pos - FuncInline.OffsetWidth[(int)letsAxis];
                GlobalVar.LetsAxisStatus[(int)letsAxis].StandStill = true;
                GlobalVar.LetsAxisStatus[(int)letsAxis].Velocity = 0;
                GlobalVar.LetsAxisStatus[(int)letsAxis].isHomed = GlobalVar.LetsAxisStatus[(int)letsAxis].Homing;
                return true;
            }


            int raw = GlobalVar.LetsAxis[(int)letsAxis];

            double position, velocity, acceleration, jerk;
            position = GetRealPulse((FuncInline.enumLetsAxis)letsAxis, pos);
            velocity = GetRealSpeed((FuncInline.enumLetsAxis)letsAxis,speed);
            acceleration = velocity * 10;
            jerk = acceleration * 10;

            int result = LetsExplorerDll.MoveAbs(raw, position, velocity, acceleration, jerk);
          
            if (result == 0)
            {
                
            }
            else
            {
                FuncLog.WriteLog($"MoveAbs Comand Fail Axis :{letsAxis}");
                return false;
            }
       
 
            return true;
            //SetSpeed(pmcAxis, speed);
            //return ABSMove(pmcAxis, pos);
        }
     
       
        public static bool Stop(int LetsAxis) // 일반정지,Home정지
        {
            
            int raw = GlobalVar.LetsAxis[LetsAxis];

            int result = LetsExplorerDll.StopJog(raw);
            if (result < 0)
            {
                FuncLog.WriteLog($"Lets Stop Error : {result}");
                return false;
            }
       
            return true;
        }
        /**
         * @brief 서보 전체 정지        
         * @return void
         */
        public static void StopAll() //전체 서보모터 정지
        {
            for (int axis = 0; axis < GlobalVar.LetsAxis_count; axis++)
            {
                if (!GlobalVar.LetsAxisStatus[axis].StandStill)
                {
                    FuncLetsMotion.Stop(axis);
                }
            }
        }

        /**
        * @brief Jog 동작 전체 정지        
        * @return void
        */

        public static bool JogMoveStopAll()
        {
            uint uStatus = 0;
            for (int i = 0; i < GlobalVar.Axis_count; i++)
            {
                CAXM.AxmStatusReadInMotion(i, ref uStatus);
                if (uStatus != 0) CAXM.AxmMoveSStop(i);
            }
            return true;
        }




        #endregion





        #endregion

        //Lets모션컨트롤 스탭모터 초기화 관련 소스
        public static bool CheckLetsMotion()
        {
            try
            {
                int result = LetsExplorerDll.InitConnectionManual(DefaultIp);
                
                if (result <= 0)
                {
                    initialized = false;
                    FuncWin.TopMessageBox($"Lets Init  Fail: {result}");
                    return false;
                }

                initialized = true;
                return true;

            }
            catch (Exception ex)
            {
                initialized = false;
                FuncWin.TopMessageBox($"Lets Initialize  Error: {ex.Message}");
                return false;
            }
        }
        public static bool CheckLetsScanMotion()
        {
            try
            {
                // 0) 연결이 안 된 상태면 초기화
                if (!initialized)
                    return false;

                // 1) ScanNodeList 호출
                int scanCount = LetsExplorerDll.ScanNodeList();
                if (scanCount <= 0)
                {
                    Scan_initialized = false;
                    MessageBox.Show($"Scan Fail: {scanCount}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return false;
                }

                // 2) GetNodeList 호출 (이제 rawNodeInfo 리스트만 들어 있음)
                int[] nodes = new int[scanCount];
                int nodeCont = LetsExplorerDll.GetNodeList(nodes, scanCount);

                //FuncLog.WriteLog("=== Scan Debug Start ===");
                for (int i = 0; i < nodeCont; i++)
                {
                    int raw = nodes[i]; //IP주소 
                    int grp = (raw >> 24) & 0xFF;   //노드
                    //FuncLog.WriteLog($"[Scan Debug] raw=0x{raw:X8}, group={grp}");
                }
                //FuncLog.WriteLog("=== Scan Debug End ===");

                // 3) _ipMap 갱신: IP 문자열 → rawNodeInfo 리스트
                LetsIpAixs.Clear();
                var sbIp = new StringBuilder(16);//IP주소 
                for (int i = 0; i < nodeCont; i++)
                {
                    int raw = nodes[i];//IP주소 // 스캔으로 얻은 원시 노드 핸들

                    // IP 문자열 얻기 (하위 8비트)
                    sbIp.Clear();
                    LetsExplorerDll.GetIpFromNode(raw, sbIp);
                    string ip = sbIp.ToString();     // 이 raw 가 속한 IP 문자열
                    int grp = (raw >> 24) & 0xFF;   //노드(X,Y,Z,U)선택 Z,U축 사용안하려고, 섞어 사용하고있음

                    if (ip == "192.168.10.20" && grp == 0)
                    {
                        GlobalVar.LetsAxis[0] = raw;    //IP와 노드 정보 저장
                    }
                    else if (ip == "192.168.10.20" && grp == 1)
                    {
                        GlobalVar.LetsAxis[1] = raw;
                    }
                    else if (ip == "192.168.10.21" && grp == 0)
                    {
                        GlobalVar.LetsAxis[2] = raw;
                    }
                    else if (ip == "192.168.10.21" && grp == 1)
                    {
                        GlobalVar.LetsAxis[3] = raw;
                    }
                    else if (ip == "192.168.10.22" && grp == 0 && FuncInline.InlineType > FuncInline.enumInlineType.Gen5) //NG버퍼 사용할때
                    {
                        GlobalVar.LetsAxis[3] = raw;
                    }
                    else if (!(ip == "192.168.10.20" || ip == "192.168.10.21" || ip == "192.168.10.22"))    //해당 IP가 아니면 에러처리
                    {
                        FuncWin.TopMessageBox($"Lets Scanning Error - IP Error {ip}");
                        Scan_initialized = false;
                        return false;
                    }

                }
                Scan_initialized = true;
                return true;

            }
            catch (Exception ex)
            {
                Scan_initialized = false;
                FuncWin.TopMessageBox($"Lets Scanning Error: {ex.Message}");
                return false;
            }
        }
    }
}
