using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms; //HJ 수정 200406 메시지 창을 띄우기 위해(Ecat.cs)


namespace Radix
{
    /**
     * @brief 모션 실행 및 체크 등 함수 선언
     */
    public static class FuncMotion
    {
        /*
         * FuncMotion.cs : 모션 실행 및 체크 등 함수 선언
         */

        private static void debug(string str)
        {
            Util.Debug("FuncMotion : " + str);
        }

        #region 로컬 변수
        #region gantry 관련. 최대 축수를 일단 100개로 고정한다.
        public static uint[] duSlaveHmUse = new uint[100];
        public static uint[] duGantryOn = new uint[100];
        public static double[] dSlaveHmOffset = new double[100];
        public static double[] dSlaveHmRange = new double[100];
        #endregion
        #endregion

        #region 모션 관련

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
        public static double CalcWidthPos(int axis, double pos) // 일반 폭조절 모터 자체의 거리 계산값을 기구상의 실제 거리로 환산 - 위치값 확인할 때
        {
            switch (axis)
            {
                case 0: // In Shuttle
                    return FuncInline.DefaultPCBWidth + pos;
                case 1: // Out Shuttle
                    return FuncInline.DefaultPCBWidth + pos;
                case 2: // NG 컨베어
                    return FuncInline.DefaultPCBWidth + pos;
                case 3: // Rack1
                    return FuncInline.DefaultPCBWidth + pos;
                case 4: // Rack2
                    return FuncInline.DefaultPCBWidth + pos;
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
                    return pos - 235;
                case FuncInline.enumServoAxis.SV01_Out_Shuttle:
                    return pos - 235;
                case FuncInline.enumServoAxis.SV02_Lift1:
                    return pos - 235;
                case FuncInline.enumServoAxis.SV03_Rack1_Width:
                    return 600 - pos;
                case FuncInline.enumServoAxis.SV04_Lift2:
                    return 600 - pos;
                case FuncInline.enumServoAxis.SV05_Rack2_Width:
                    return pos; // 각도 환산 필요
                case FuncInline.enumServoAxis.SV06_Scan_X:
                    return pos; // 각도 환산 필요
                case FuncInline.enumServoAxis.SV07_Scan_Y:
                    return 600 - pos;
           
                default:
                    return pos;
            }
        }

        #region 서보 위치값 환산

        /**
         * @brief 직교로봇 두 좌표간에 근접했나?
         * @param servoIndex 서보모터 순번
         * @param pos 체크할 위치값
         * @param gap 근접 간주할 거리
         * @return bool 범위 안 True
         *      범위 외 False
         */
        public static bool CheckNearPos(int servoIndex, double pos, double gap) //CheckNearPos : 두 좌표간에 근접했나?
        {
            return Math.Abs(GlobalVar.AxisStatus[servoIndex].Position - pos) <= gap;
        }

        public static bool CheckNearPos(double pos1, double pos2, double gap)
        {
            return Math.Abs(pos1 - pos2) <= gap;
        }

        /**
         * @brief 직교로봇 두 좌표간에 근접했나?
         * @param pos1 1번축
         * @param pos2 2번축
         * @param gap 근접 간주할 거리
         * @return bool 범위 안 True
         *      범위 외 False
         */
        public static bool CheckNearPos(structPosition pos1, structPosition pos2, double gap) //CheckNearPos : 두 좌표간에 근접했나?
        {
            return Math.Abs(pos1.x - pos2.x) < gap &&
                        Math.Abs(pos1.y - pos2.y) < gap &&
                        Math.Abs(pos1.z - pos2.z) < gap;
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
         * @brief 서보모터 z축 두개 인 경우 두 좌표간에 근접했나?
         * @param destPos 1번축
         * @param x1 2번축 x 좌표
         * @param y1 2번축 y 좌표
         * @param z1 2번축 z 좌표
         * @param z2 모듈인라인 z축이 두개이므로 별도로 지정
         * @param gap 근접 간주할 거리
         * @return bool 범위 안 True
         *      범위 외 False
         */
        public static bool CheckNearPos(structPosition destPos, double x1, double y1, double z1, double z2, double gap) //CheckNearPos : 두 좌표간에 근접했나? 모듈인라인 z축이 두개
        {
            return Math.Abs(destPos.x - x1) < gap &&
                        Math.Abs(destPos.y - y1) < gap &&
                        Math.Abs(destPos.z - z1) < gap &&
                        Math.Abs(destPos.z - z2) < gap;
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
            return MMToPulse(FuncMotion.ReCalcAxisPos(axis, pos),
                                                       GlobalVar.ServoGearRatio[axis],
                                                       GlobalVar.ServoRevMM[axis],
                                                       GlobalVar.ServoRevPulse[axis]);
        }

        /**
         * @brief 기구상 실제 속도값으로 실지령 pulse 계산
         * @param axis 축순번
         * @param speed 기구상 속도값
         * @return double 지령에 사용할 속도 펄스
         */
        public static double GetRealSpeed(int axis, double speed) // 프로그램 UI연결할 실제 좌표값으로 실지령 pulse 계산
        {
            return MMToPulse((long)speed,
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
            return FuncMotion.CalcAxisPos(axis,
                                    PulseToMM((long)GlobalVar.AxisStatus[axis].Position,
                                                            GlobalVar.ServoGearRatio[axis],
                                                            GlobalVar.ServoRevMM[axis],
                                                            GlobalVar.ServoRevPulse[axis]));
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
                    Math.Abs(CalcAxisPos(0, PulseToMM((long)GlobalVar.AxisStatus[0].Position, GlobalVar.ServoGearRatio[0], GlobalVar.ServoRevMM[0], GlobalVar.ServoRevPulse[0])) - posX) < 0.1)
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
            //for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length; i++)
            //{
            //    if (//PMCClass.GetCurrSpeed((FuncInline.enumPMCAxis)i) > 1)
            //    {
            //        //PMCClass.Stop((FuncInline.enumPMCAxis)i);
            //    }
            //}
            #endregion
        }

        public static bool GantryCheck(int axis1)
        {
            if (GlobalVar.Simulation)
            {
                return true;
            }

            switch (GlobalVar.MasterType)
            {
                case enumMasterType.AXL:

                    //======== 겐트리 관련 함수==========================================================================================
                    // 모션모듈은 두 축이 기구적으로 Link되어있는 겐트리 구동시스템 제어를 지원한다. 
                    // 이 함수를 이용해 Master축을 겐트리 제어로 설정하면 해당 Slave축은 Master축과 동기되어 구동됩니다. 
                    // 만약 겐트리 설정 이후 Slave축에 구동명령이나 정지 명령등을 내려도 모두 무시됩니다.
                    // uSlHomeUse     : 슬레이축 홈사용 우뮤 ( 0 - 2)
                    //             (0 : 슬레이브축 홈을 사용안하고 마스터축을 홈을 찾는다.)
                    //             (1 : 마스터축 , 슬레이브축 홈을 찾는다. 슬레이브 dSlOffset 값 적용해서 보정함.)
                    //             (2 : 마스터축 , 슬레이브축 홈을 찾는다. 슬레이브 dSlOffset 값 적용해서 보정안함.)
                    // dSlOffset      : 슬레이브축 옵셋값
                    // dSlOffsetRange : 슬레이브축 옵셋값 레인지 설정
                    // 주의사항       : 갠트리 ENABLE시 슬레이브축은 모션중 AxmStatusReadMotion 함수로 확인하면 True(Motion 구동 중)로 확인되야 정상동작이다. 
                    //                  슬레이브축에 AxmStatusReadMotion로 확인했을때 InMotion 이 False이면 Gantry Enable이 안된것이므로 알람 혹은 리밋트 센서 등을 확인한다.
                    uint duRetCode;
                    //uint duSlaveHmUse = 0, duGantryOn = 0;
                    //double dSlaveHmOffset = 0.0, dSlaveHmRange = 1.0;

                    //++ 지정한 축의 겐트리제어 관련 설정값을 확인합니다.
                    CAXM.AxmGantryGetEnable(axis1, ref duSlaveHmUse[axis1], ref dSlaveHmOffset[axis1], ref dSlaveHmRange[axis1], ref duGantryOn[axis1]);
                    return duGantryOn[axis1] == 1;
            }
            return true;
        }

        public static bool GantrySetup(int axis1, int axis2, int On)
        {
            if (GlobalVar.Simulation)
            {
                return true;
            }

            switch (GlobalVar.MasterType)
            {
                case enumMasterType.AXL:

                    //======== 겐트리 관련 함수==========================================================================================
                    // 모션모듈은 두 축이 기구적으로 Link되어있는 겐트리 구동시스템 제어를 지원한다. 
                    // 이 함수를 이용해 Master축을 겐트리 제어로 설정하면 해당 Slave축은 Master축과 동기되어 구동됩니다. 
                    // 만약 겐트리 설정 이후 Slave축에 구동명령이나 정지 명령등을 내려도 모두 무시됩니다.
                    // uSlHomeUse     : 슬레이축 홈사용 우뮤 ( 0 - 2)
                    //             (0 : 슬레이브축 홈을 사용안하고 마스터축을 홈을 찾는다.)
                    //             (1 : 마스터축 , 슬레이브축 홈을 찾는다. 슬레이브 dSlOffset 값 적용해서 보정함.)
                    //             (2 : 마스터축 , 슬레이브축 홈을 찾는다. 슬레이브 dSlOffset 값 적용해서 보정안함.)
                    // dSlOffset      : 슬레이브축 옵셋값
                    // dSlOffsetRange : 슬레이브축 옵셋값 레인지 설정
                    // 주의사항       : 갠트리 ENABLE시 슬레이브축은 모션중 AxmStatusReadMotion 함수로 확인하면 True(Motion 구동 중)로 확인되야 정상동작이다. 
                    //                  슬레이브축에 AxmStatusReadMotion로 확인했을때 InMotion 이 False이면 Gantry Enable이 안된것이므로 알람 혹은 리밋트 센서 등을 확인한다.
                    uint duRetCode;
                    //uint duSlaveHmUse = 0, duGantryOn = 0;
                    //double dSlaveHmOffset = 0.0, dSlaveHmRange = 1.0;

                    //++ 지정한 축의 겐트리제어 관련 설정값을 확인합니다.
                    CAXM.AxmGantryGetEnable(axis1, ref duSlaveHmUse[axis1], ref dSlaveHmOffset[axis1], ref dSlaveHmRange[axis1], ref duGantryOn[axis1]);

                    //++ 지정한 Master축과 Slave축으로 겐트리 기능을 활성화 시킵니다.
                    //[INFO] 겐트리 제어 기능을 활성화 시키고 이후 Slave축에 구동 명령이나 정지 명령등을 내려도 모두 무시 됩니다.
                    if (duGantryOn[axis1] == 1 && On != 1)
                    {
                        duRetCode = CAXM.AxmGantrySetDisable(axis1, axis2);
                    }
                    else if (duGantryOn[axis1] == 0 && On == 1)
                    {
                        duRetCode = CAXM.AxmGantrySetEnable(axis1, axis2, duSlaveHmUse[axis1], dSlaveHmOffset[axis1], dSlaveHmRange[axis1]);

                        if (duRetCode != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                        {
                            FuncError.AddError(new FuncInline.structError(DateTime.Now.ToString("yyyyMMdd"),
                                                       DateTime.Now.ToString("HH:mm:ss"),
                                                       FuncInline.enumErrorPart.System,
                                                       FuncInline.enumErrorCode.E_Stop,
                                                       false,
                                                       "Servo Motor Gantry Setup Error."));
                            return false;
                        }
                    }

                    //++ 지정한 축의 겐트리제어 관련 설정값을 확인합니다.
                    CAXM.AxmGantryGetEnable(axis1, ref duSlaveHmUse[axis1], ref dSlaveHmOffset[axis1], ref dSlaveHmRange[axis1], ref duGantryOn[axis1]);
                    break;
            }
            return true;
        }


        #region 서보 모터 제어
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
                //*
                GlobalVar.AxisStatus[axis].PowerOn = on;
                GlobalVar.AxisStatus[axis].StandStill = true;
                //*/
                return true;
            }
            switch (GlobalVar.MasterType)
            {
                /*
                case enumMasterType.MXP:
                    MXP.MXP_POWER_OUT powerOut = new MXP.MXP_POWER_OUT { };
                    return Motion_Function.MXP_MC_Power(axis, Motion_Function.IndexCal((UInt32)MXP.MXP_MotionBlockIndex.mcPower) + axis, Convert.ToByte(on), false, powerOut) == MXP.MXP_ret.RET_NO_ERROR;
                //break;
                case enumMasterType.MXN:
                    MXN.MXN_Power(1);
                    return true;
                    //*/
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
                //*
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
                    /*
                    case enumMasterType.MXP:
                        Motion_Function.ServoOnAll(Convert.ToByte(on));
                        break;
                    case enumMasterType.MXN:
                        MXN.MXN_Power(1);
                        break;
                        //*/
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
                /*
                case enumMasterType.MXP:
                    MXP.MXP_RESET_OUT resetOut = new MXP.MXP_RESET_OUT { };
                    Motion_Function.MXP_MC_Reset(axis, Motion_Function.IndexCal((UInt32)MXP.MXP_MotionBlockIndex.mcReset) + axis, false, resetOut);
                    break;
                case enumMasterType.MXN:
                    MXN.MXN_Write_X(MXN.REG_BIT, 210, 7, 7, 1);
                    Thread.Sleep(200);
                    MXN.MXN_Write_X(MXN.REG_BIT, 210, 7, 7, 0);
                    break;
                    //*/
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
        * @brief  서보  Set Soft Limit 
        * @return void
        */
        
        public static bool SetSoftLimit(uint axisNo, double dPositivePos = 0.0, double dNegativePos = 0.0)
        {
            int axis = Convert.ToInt32(axisNo);
            uint duRetCode;
            uint duUse = 0, duStopMode = 0, duSelection = 0;

            //++ 지정한 축에 Software Limit기능을 확인합니다.
            //duRetCode = CAXM.AxmSignalGetSoftLimit(axis, ref duUse, ref duStopMode, ref duSelection, ref dPositivePos, ref dNegativePos);
           
            // 둘다 0 이면 
            if (Math.Abs(dPositivePos) < 0.0001 && Math.Abs(dNegativePos) < 0.0001)
            {
                duUse = 0;
            }

            //++ 지정 축의 소프트웨어 리미트를 설정합니다.
            // uUse       : (0)DISABLE        - 소프트웨어 리미트 기능을 사용하지 않습니다.
            //              (1)ENABLE         - 소프트웨어 리미트 기능을 사용합니다.
            // uStopMode  : (0)EMERGENCY_STOP - 소프트웨어 리미트 영역을 벗어날 경우 급정지합니다.
            //              (1)SLOWDOWN_STOP  - 소프트웨어 리미트 영역을 벗어날 경우 감속정지합니다.
            // uSelection : (0)COMMAND        - 기준위치를 지령위치로 합니다.
            //              (1)ACTUAL         - 기준위치를 엔코더 위치로 합니다.
            uint uiRet = CAXM.AxmSignalSetSoftLimit(axis, duUse, duStopMode, duSelection, dPositivePos, dNegativePos);
            if (uiRet != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
            {
                FuncWin.TopMessageBox(String.Format("AxmSignalSetLimit return error[Code:{0:d}]" + axisNo + " 축", uiRet));
                return false;
            }
            return true;
            
        }
       
        //아진제어기는 리미트 변경 안되는거 같음, 서보드라이버에서 직접 바꾸자
        public static bool SetEndLimitLevelAll(uint LowHigh)
        {
            if (GlobalVar.Simulation)
            {
             
                return true;
            }

            switch (GlobalVar.MasterType)
            {
                case enumMasterType.AXL:
                    for (int i = 0; i < GlobalVar.Axis_count; i++)
                    {
                        SetSoftLimit((uint)i, 0, 0);

                        // 지정 축의 end limit sensor의 사용 유무 및 신호의 입력 레벨을 설정한다. 
                        // end limit sensor 신호 입력 시 감속정지 또는 급정지에 대한 설정도 가능하다.
                        // uStopMode: EMERGENCY_STOP(0), SLOWDOWN_STOP(1)
                        // uPositiveLevel, uNegativeLevel : LOW(0), HIGH(1), UNUSED(2), USED(3)
                        uint uStopMode = 0;

                        // 리미트 사용 설정 (B 접점 : 1 , A접점 : 0 )
                        uint uiRet = CAXM.AxmSignalSetLimit((int)i, uStopMode, LowHigh, LowHigh);

                        if (uiRet != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                        {
                            FuncWin.TopMessageBox(String.Format("AxmSignalSetLimit return error[Code:{0:d}]" + i + " 축", uiRet));
                            return false;
                        }

                    }
                    break;
                default:
                    break;
            }
           
            return true;
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
                /*
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
                    //*/
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
                GlobalVar.AxisStatus[axis].Position = 0;
                GlobalVar.AxisStatus[axis].Velocity = 0;
                //*/
                return true;
            }

            //////////////////////////////////////////////////////////////////////
            switch (GlobalVar.MasterType)
            {

                #region MXP
                /*
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
                //*/
                /*
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
                       //*/
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


                    bool IsConveyor = false;    // axis 가 컨베이어 인가
                    bool IsPicker = false;      // axis 가 pick 동작인가

                    #region 앰플포장기용 초기화 세팅

                    if (false)   // 스누콘 앰플포장기용 홈 초기화
                    {
                        //int axis_conv1 = (int)FuncAmplePacking.enumServoAxis.SV00_Main_Conveyor;

                        //if (axis == axis_conv1)
                        //{
                        //    IsConveyor = true;

                        //    // 컨베이어는 +쪽으로 홈을 찾는게 맞을듯 한데.. (컨베어는 어떤 경우라도 뒤로 가는 경우는 없도록)
                        //    iHomeDir = 1;

                        //    double conv1_InitPos = FuncAmplePacking.ServoParamAll.sv0_Conveyor.init_pos; //Main 컨베이어


                        //    // 컨베어라서 초기화후 초기위치 보정을 해야한다.
                        //    double axis_conv1_Hoffset = FuncMotion.MMToPulse(conv1_InitPos, GlobalVar.ServoGearRatio[axis], GlobalVar.ServoRevMM[axis], GlobalVar.ServoRevPulse[axis]);


                        //    if (axis == axis_conv1)
                        //    {
                        //        dHomeOffset = axis_conv1_Hoffset;
                        //    }



                        //}
                        //int axis_pickX = (int)FuncAmplePacking.enumServoAxis.SV02_Fixture_Supply_X;
                        //int axis_pickY = (int)FuncAmplePacking.enumServoAxis.SV03_Fixture_Supply_Y;
                        //int axis_pickZ = (int)FuncAmplePacking.enumServoAxis.SV04_Fixture_Supply_Z;

                        //int axis_pick2 = (int)FuncAmplePacking.enumServoAxis.SV07_Print;
                        //int axis_OutTrayX = (int)FuncAmplePacking.enumServoAxis.SV08_Output_Tray_X;
                        //int axis_OutTrayZ = (int)FuncAmplePacking.enumServoAxis.SV09_Output_Tray_Z;

                        //if (axis == axis_pickX ||
                        //    axis == axis_pickY ||
                        //    axis == axis_pickZ ||
                        //    axis == axis_pick2 ||
                        //    axis == axis_OutTrayX ||
                        //    axis == axis_OutTrayZ)
                        //{
                        //    IsPicker = true;
                        //}

                        //int SV05_Fixture_Tray = (int)FuncAmplePacking.enumServoAxis.SV05_Fixture_Tray;

                        //if (axis == SV05_Fixture_Tray)
                        //{
                        //    //메거진 +방향은 아래임 
                        //    iHomeDir = 0;   //HmDir(홈 방향): DIR_CCW (0) -방향 , DIR_CW(1) +방향
                        //}
                    }
                    #endregion

                    //++ 지정한 축의 원점검색 방법을 변경합니다.
                    duRetCode = CAXM.AxmHomeSetMethod((int)axis, iHomeDir, duHomeSignal, duZPhaseUse, dHomeClrTime, dHomeOffset);
                    if (duRetCode != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                        FuncWin.TopMessageBox(String.Format("AxmHomeSetMethod return error[Code:{0:d}]" + axis + " 축", duRetCode));

                    //속도 지정
                    double dVelFirst, dVelSecond, dVelThird, dVelLast, dAccFirst, dAccSecond;

                    // 각각의 Edit 콘트롤에서 설정값을 가져옴

                    dVelFirst = 100000; //파나소닉 세팅 해상도 100000
                    //dVelFirst = 524288  //MXP기준

                    //dVelFirst = dVelFirst * 10;

                    dVelSecond = dVelFirst / 10;
                    dVelThird = dVelSecond / 10;
                    dVelLast = dVelThird / 20;
                    dAccFirst = dVelFirst * 10;
                    dAccSecond = dVelSecond * 10;

                    //++ 원점검색에 사용되는 단계별 속도를 설정합니다.
                    duRetCode = CAXM.AxmHomeSetVel((int)axis, dVelFirst, dVelSecond, dVelThird, dVelLast, dAccFirst, dAccSecond);
                    if (duRetCode != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                        FuncWin.TopMessageBox(String.Format("AxmHomeSetVel return error[Code:{0:d}]" + axis + " 축", duRetCode));

                    //원점 검색 시작ㄴ
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
            MoveAbsolute(axis, Pos, Vel, Vel * 5, Vel * 5, Vel * 25, false, 0);
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

            if (GlobalVar.E_Stop)
            {
                return;
            }

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
            double vel = FuncMotion.GetRealSpeed((int)axis, Vel);
            double acc = Acc == 0 ? vel * 10 : FuncMotion.GetRealSpeed((int)axis, Acc);
            double dec = Dec == 0 ? vel * 10 : FuncMotion.GetRealSpeed((int)axis, Dec);
            double jerk = Jerk == 0 ? vel * 50 : FuncMotion.GetRealSpeed((int)axis, Jerk);

            switch (GlobalVar.MasterType)
            {
                #region MXP
                /*
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
                    //*/
                #endregion
                #region MXN
                /*
  
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
  
//*/
                #endregion

                case enumMasterType.AXL:
                    uint duRetCode = 0;

                    //FuncLog.WriteLog("무브 지령 콜 - " + axis);

                    //++ 지정 축의 구동 좌표계를 설정합니다. 
                    // dwAbsRelMode : (0)POS_ABS_MODE - 현재 위치와 상관없이 지정한 위치로 절대좌표 이동합니다.
                    //                (1)POS_REL_MODE - 현재 위치에서 지정한 양만큼 상대좌표 이동합니다.
                    duRetCode = CAXM.AxmMotSetAbsRelMode((int)axis, 0);

                    //++ 지정한 축을 지정한 거리(또는 위치)/속도/가속도/감속도로 모션구동하고 모션 종료여부와 상관없이 함수를 빠져나옵니다.
                    //Console.WriteLine("MoveAbsolute : " + axis + " - " + pos);
                    if (true)
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

            MoveRelative(axis, Pos, Vel, Vel * 5, Vel * 5, Vel * 25, false, 0);
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

            if (GlobalVar.E_Stop)
            {
                return;
            }

            if (GlobalVar.Simulation)
            {
                // simulation
                debug("MoveRelative : " + axis.ToString() + " - " + Pos.ToString());
                Console.WriteLine("MoveRelative : " + axis.ToString() + " - " + Pos.ToString());
                //*
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
            double jerk = Jerk == 0 ? vel * 50 : FuncMotion.GetRealSpeed((int)axis, Jerk);

            switch (GlobalVar.MasterType)
            {
                #region MXP
                /*
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
                    //*/
                #endregion
                #region MXN
                /*
            case enumMasterType.MXN:
                MXN.MXN_MOVERELATIVE_IN InParam;
                InParam.uiAxisNo = axis + 1;
                InParam.iVelocity = (int)vel;
                InParam.iDistance = (int)pos;

                MXN.MXN_MoveRelative(ref InParam);
                break;
                //*/
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
                //*
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
                /*
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
                    //*/
                case enumMasterType.AXL:
                    //uint duRetCode = 0;
                    ////++ 지정한 축을 (+)방향으로 지정한 속도/가속도/감속도로 모션구동합니다.
                    //duRetCode = CAXM.AxmMoveVel((int)axis, Vel * GlobalVar.ServoSpeed_AXT, Math.Abs(acc * GlobalVar.ServoSpeed_AXT), Math.Abs(dec * GlobalVar.ServoSpeed_AXT));
                    //if (duRetCode != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                    //    FuncWin.TopMessageBox(String.Format("AxmMoveVel return error[Code:{0:d}]", duRetCode));
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
         * @brief 서보모터 긴급정지
         */
        public static void EmgStop(int axis) // MoveStop(축번호) 서보 정지 
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
                /*
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
                    //*/
                case enumMasterType.AXL:
                    CAXM.AxmMoveEStop(Convert.ToInt32(axis));
                    //CAXM.AxmMoveStop(Convert.ToInt32(axis), dec);

                    break;

                default:
                    break;
            }
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
                /*
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
                    //*/
                case enumMasterType.AXL:
                    CAXM.AxmMoveSStop(Convert.ToInt32(axis));
                    //CAXM.AxmMoveStop(Convert.ToInt32(axis), dec);

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
                    FuncMotion.MoveStop(axis);
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
        /**
         * @brief 홈 잡은 후 추가 옵셋 만큼 이동
         * @param axis 서보 순번
         * @return bool 이동 지령 수행 완료시 true
         *      지령 수행 실패시 false
         */
        public static bool HomeOffsetMove(uint axis) // 홈 잡은 후 추가 옵셋 만큼 이동
        {
       
            while (!GlobalVar.GlobalStop &&
                !GlobalVar.AxisStatus[axis].isHomed)
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
            /*
            if (FuncInline.TabMain == FuncInline.enumTabMain.Manual)
            {
                MXN.MXN_Write_R(MXN.REG_BIT, 700, 5, 5, 0);
            }
            //*/

            if (axis == (uint)enumServoAxis.Lift_PCBInPut)
            {
                if (GlobalVar.AxisStatus[(int)axis].StandStill)
                {



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
                //        FuncWin.TopMessageBox("Can't Turn Offest Moving2");
                //        return false;
                //    }

                //    //FuncMotion.MoveAbsolute((uint)axis, (5.6 * 27.7777777778), 20 * 1000);

                //}
                //else
                //{
                //    FuncWin.TopMessageBox("Can't Turn Offest Moving");
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

        public static void JogMoveCheck_StateChange(int position)   //0 = before 1= after
        {
            switch (GlobalVar.ProjectType)
            {
                case enumProject.AutoInline:
                    FuncInlineMove.JogMoveCheck_StateChange(position);
                    break;
                default:
                    break;
            }

        }

        /**
         * @brief 서보모터 갠트리(동기화) 여부 체크
         * @param Axis 체크할 축
         * @return bool 갠트리 설정 여부
         */
        public static bool CheckGantryEnabled(int Axis)
        {
            if (GlobalVar.Simulation)
            {
                return true;
            }
            switch (GlobalVar.MasterType)
            {
                /*
                case enumMasterType.MXP:
                    break;
                case enumMasterType.MXN:
                    break;
                    //*/
                case enumMasterType.AXL:
                    uint duSlaveHmUse = 0, duGantryOn = 0;
                    double dSlaveHmOffset = 0.0, dSlaveHmRange = 1.0;
                    CAXM.AxmGantryGetEnable(Axis, ref duSlaveHmUse, ref dSlaveHmOffset, ref dSlaveHmRange, ref duGantryOn);
                    return duGantryOn == 1;
                default:
                    break;
            }
            return false;
        }

        /**
         * @brief 서보모터 갠트리 세팅
         * @param Axis 체크할 축
         * @return bool 갠트리 설정 여부
         */
        public static bool SetGantryEnable(int Axis1, int Axis2, bool On)
        {
            switch (GlobalVar.MasterType)
            {
                /*
                case enumMasterType.MXN:
                    break;
                    //*/
                case enumMasterType.AXL:
                    if (GlobalVar.Simulation)
                    {
                        return true;
                    }
                    //======== 겐트리 관련 함수==========================================================================================
                    // 모션모듈은 두 축이 기구적으로 Link되어있는 겐트리 구동시스템 제어를 지원한다. 
                    // 이 함수를 이용해 Master축을 겐트리 제어로 설정하면 해당 Slave축은 Master축과 동기되어 구동됩니다. 
                    // 만약 겐트리 설정 이후 Slave축에 구동명령이나 정지 명령등을 내려도 모두 무시됩니다.
                    // uSlHomeUse     : 슬레이축 홈사용 우뮤 ( 0 - 2)
                    //             (0 : 슬레이브축 홈을 사용안하고 마스터축을 홈을 찾는다.)
                    //             (1 : 마스터축 , 슬레이브축 홈을 찾는다. 슬레이브 dSlOffset 값 적용해서 보정함.)
                    //             (2 : 마스터축 , 슬레이브축 홈을 찾는다. 슬레이브 dSlOffset 값 적용해서 보정안함.)
                    // dSlOffset      : 슬레이브축 옵셋값
                    // dSlOffsetRange : 슬레이브축 옵셋값 레인지 설정
                    // 주의사항       : 갠트리 ENABLE시 슬레이브축은 모션중 AxmStatusReadMotion 함수로 확인하면 True(Motion 구동 중)로 확인되야 정상동작이다. 
                    //                  슬레이브축에 AxmStatusReadMotion로 확인했을때 InMotion 이 False이면 Gantry Enable이 안된것이므로 알람 혹은 리밋트 센서 등을 확인한다.
                    uint duSlaveHmUse = 0, duGantryOn = 0;
                    double dSlaveHmOffset = 0.0, dSlaveHmRange = 1.0;

                    //++ 지정한 축의 겐트리제어 관련 설정값을 확인합니다.
                    CAXM.AxmGantryGetEnable(Axis1, ref duSlaveHmUse, ref dSlaveHmOffset, ref dSlaveHmRange, ref duGantryOn);

                    //++ 지정한 Master축과 Slave축으로 겐트리 기능을 활성화 시킵니다.
                    //[INFO] 겐트리 제어 기능을 활성화 시키고 이후 Slave축에 구동 명령이나 정지 명령등을 내려도 모두 무시 됩니다.
                    if (duGantryOn == 1 && !On)
                    {
                        CAXM.AxmGantrySetDisable(Axis1, Axis2);
                    }
                    else if (duGantryOn == 0 && On)
                    {
                        CAXM.AxmGantrySetEnable(Axis1, Axis2, duSlaveHmUse, dSlaveHmOffset, dSlaveHmRange);
                    }

                    return duGantryOn == 1;
                default:
                    break;
            }
            return false;
        }

        #endregion

        /**
         * @brief 서보모터를 이동시킬 수 있는 상황인지 체크
         *        위치에 따라서 각 실린더,로봇 및 센서 상황에 따라 인터락 판단
         * @param axis 체크할 축
         * @param pos 이동할 좌표
         * @return bool 이동 가능 여부
         */
        public static bool CheckMoveServoEnable(int axis, double pos)
        {
            /* 직접 콜하지 말고 프로젝트 소스(ex. FuncAmplePackingMove)의 함수를 직접 콜하도록 한다.
            switch (GlobalVar.ProjectType)
            {
                case enumProject.AmplePacking:
                    return true;
                    //return FuncAmplePackingMove.CheckMoveServoEnable(axis, pos);
            }
            //*/
            return false;
        }


        public static bool CheckServoInit()
        {
            try
            {
                if (GlobalVar.Simulation)
                {
                    return true;
                }
                /*
                if (!FuncMotion.GantryCheck((int)FuncInline.enumServoAxis.SV03_Rack1_Width) ||
                    !FuncMotion.GantryCheck((int)FuncInline.enumServoAxis.SV05_Rack2_Width))
                {
                    return false;
                }
                //*/
                for (int axis = 0; axis < GlobalVar.Axis_count; axis++)
                {
                    if (!GlobalVar.AxisStatus[axis].PowerOn)
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
                return false;
            }
            return true;
        }
        public static bool ServoAllInit()
        {
            if (GlobalVar.Simulation)
            {
                return true;
            }

            // Gantry 축에서 Gantry 해제하고 ServoOff 하면 틀어지므로
            // Gantry가 잡혀 있는 축은 리셋을 안 하도록 한다.

            for (uint i = 0; i < GlobalVar.Axis_count; i++)
            {
                FuncMotion.ServoReset(i);

                Thread.Sleep(100);

                ServoOn(i, true);

                Thread.Sleep(100);
            }
            //*/
            //Thread.Sleep(100);

            for (int axis = 0; axis < GlobalVar.Axis_count; axis++)
            {
                if (!GlobalVar.AxisStatus[axis].PowerOn)
                {
                    return false;
                }
            }
            return true;
        }


        // StandStill 은 타이머가 돌때까지 즉각적이지 않기때문에 보완적으로 사용한다.
        public static bool IsMoving(int axis)
        {
            if (GlobalVar.Simulation) return false;

            if (axis < 0 || axis >= GlobalVar.Axis_count) return false;
            uint uStatus = 0;
            CAXM.AxmStatusReadInMotion(axis, ref uStatus);
            if (uStatus == 0) return false;
            return true;
        }
        #endregion

    }
}
