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
    public class FuncInlineAction
    {
        /*
        FuncInlineActin : Inline 장비 동작 관련 선언
        //*/
        private static void debug(string str)
        {
            //Util.Debug("FuncInlineAction + " + str);
            //FuncLog.WriteLog_Debug(str);
        }
        public static bool SitePCBLoad(FuncInline.enumTeachingPos site)
        {
            // 1) 사이트 범위 검사 (신규 enum: Site1_F_DT1 ~ Site26_R_FT3)
            if (site < FuncInline.enumTeachingPos.Site1_F_DT1 ||
                site > FuncInline.enumTeachingPos.Site26_R_FT3)
            {
                return true; // 사이트가 아니면 처리 불필요
            }

            // 2) 사이트 인덱스 계산 (0 ~ 25)
            int offset = (int)site - (int)FuncInline.enumTeachingPos.Site1_F_DT1;

            try
            {
                // 3) 해당 사이트의 DI/DO 매핑 가져오기
                //    - PCB 도크 센서(DI)
                //    - 컨택트 UP/DOWN 솔(핀 업/다운, DO)
                //    - 스토퍼(클램프) 솔(클램프/언클램프, DO)
                //    - 사이트 이송 모터 CW/CCW (컨베이어, DO)
                FuncInline.enumDINames dockDi;
                FuncInline.enumDONames updownDo, stopperDo, motorCw, motorCcw;

                if (!FuncInline.SiteIoMaps.TryGetPcbDockDI(site, out dockDi))
                    return false; // 매핑 없음 → 안전하게 실패 처리

                bool hasUpDown = FuncInline.SiteIoMaps.TryGetContactUpDownDO(site, out updownDo);
                bool hasStopper = FuncInline.SiteIoMaps.TryGetContactStopperDO(site, out stopperDo);
                bool hasMotors = FuncInline.SiteIoMaps.TryGetSiteTransportMotorPair(site, out motorCw, out motorCcw);

                // 4) 타이머 시작 (로딩 동작 타임아웃 관리)
                if (FuncInline.SiteCheckTime[offset] == null ||
                    !FuncInline.SiteCheckTime[offset].IsRunning)
                {
                    Util.StartWatch(ref FuncInline.SiteCheckTime[offset]);
                }

                // 5) 컨베이어가 한 번이라도 구동되었는지 추적
                if (hasMotors && (DIO.GetDORead(motorCw) || DIO.GetDORead(motorCcw)))
                {
                    FuncInline.SiteConvRun[offset] = true;

                    // 도크센서가 아직 OFF면, 로딩 대기시간을 리셋(밀어넣는 중이므로)
                    if (!DIO.GetDIData(dockDi))
                        Util.ResetWatch(ref FuncInline.SiteCheckTime[offset]);
                }

                // 6) 타임아웃 체크
                if (FuncInline.SiteCheckTime[offset] != null &&
                    FuncInline.SiteCheckTime[offset].IsRunning &&
                    FuncInline.SiteCheckTime[offset].ElapsedMilliseconds >
                        FuncInline.ConveyorTimeout * 1000)
                {
                    // 컨베이어 정지
                    if (hasMotors)
                    {
                        if (DIO.GetDORead(motorCw)) DIO.WriteDOData(motorCw, false);
                        if (DIO.GetDORead(motorCcw)) DIO.WriteDOData(motorCcw, false);
                    }

                    // 에러 리포트 (에러 파트: 사이트 인덱스에 매칭)
                    var part = (FuncInline.enumErrorPart)((int)FuncInline.enumErrorPart.Site1_F_DT1 + offset);

                    FuncInline.AddError(new FuncInline.structError(
                        DateTime.Now.ToString("yyyyMMdd"),
                        DateTime.Now.ToString("HH:mm:ss"),
                        part,
                        FuncInline.enumErrorCode.Site_PCB_Load_Timeout,
                        false,
                        "Site PCB Load Timed out. Check " + site.ToString() + " conveyor / clamp / contact."));

                    // 타이머 리셋
                    FuncInline.SiteCheckTime[offset].Stop();
                    FuncInline.SiteCheckTime[offset].Reset();
                    return false;
                }

                // 7) 완료조건 판정
                //    - BuyerChange(블루)일 때: 컨베이어를 한 번 이상 구동했고, PCB 도크 감지 & 모터 정지면 완료
                //    - 일반 테스트일 때:
                //         * TestWithSiteUnclamp == false : 도크 감지 & 모터 정지 & (클램프 ON) & (컨택트 DOWN) ⇒ 완료
                //         * TestWithSiteUnclamp == true  : 도크 감지 & 모터 정지 & (클램프 OFF) & (컨택트 DOWN) ⇒ 완료
                bool pcbDocked = DIO.GetDIData(dockDi);
                bool motorIdle = !hasMotors || (!DIO.GetDORead(motorCw) && !DIO.GetDORead(motorCcw));
                bool contactDown = FuncInline.IsContactDown(site);     // 핀 다운(접촉) 상태
                bool clampOn = FuncInline.CheckSiteClamp(site);    // 스토퍼(클램프) ON = 물림

                if (FuncInline.SiteConvRun[offset]) // 컨베이어를 한 번 이상 돌린 이후만 완료 판단
                {
                    if (FuncInline.PCBInfo[(int)site].BuyerChange)
                    {
                        // 바이어 체인지(블루) : PCB 감지 & 모터 정지면 완료
                        if (pcbDocked && motorIdle)
                            return true;
                    }
                    else
                    {
                        if (!FuncInline.TestWithSiteUnclamp)
                        {
                            // 클램프 상태로 테스트: PCB 감지 & 모터 정지 & 클램프 ON & 컨택트 DOWN
                            if (pcbDocked && motorIdle && clampOn && contactDown)
                                return true;
                        }
                        else
                        {
                            // 언클램프 상태로 테스트: PCB 감지 & 모터 정지 & 클램프 OFF & 컨택트 DOWN
                            if (pcbDocked && motorIdle && !clampOn && contactDown)
                                return true;
                        }
                    }
                }

                // 8) 공통 동작 시퀀스
                //    (1) 컨베이어 아직 안 돌렸다면 → 컨택트 UP + 언클램프 + 컨베이어 가동
                //    (2) 컨베이어 가동 중 PCB 감지되면 → (설정에 따라) 정지/핀다운/클램프
                //    (3) 모터/클램프/컨택트 조합은 설정(GlobalVar.AutoInline_*)에 맞춰 처리

                // (1) 컨베이어 미가동 상태에서 선행 조건 만들기: 컨택트 UP, 언클램프
                if (!FuncInline.SiteConvRun[offset])
                {
                    // 컨택트(핀) UP
                    if (hasUpDown && contactDown) // 이미 다운이면 올림
                    {
                        // 다운 → 업으로
                        DIO.WriteDOData(updownDo, false);
                    }

                    // 언클램프
                    if (hasStopper && clampOn) // 이미 클램프 ON이면 해제
                    {
                        DIO.WriteDOData(stopperDo, false);
                    }

                    // 컨베이어 가동 시작 (기본: CCW 사용, 반대축 OFF 보장)
                    if (hasMotors && !DIO.GetDORead(motorCw) && !DIO.GetDORead(motorCcw))
                    {
                        // 안전하게 CW OFF
                        DIO.WriteDOData(motorCw, false);
                        // CCW ON
                        DIO.WriteDOData(motorCcw, true);

                        // “컨베이어를 돌린 적 있음” 플래그
                        FuncInline.SiteConvRun[offset] = true;

                        // 타이머(머문시간) 다시 측정 시작
                        Util.StartWatch(ref FuncInline.SiteCheckTime[offset]);
                    }

                    return false; // 아직 완료 아님
                }

                // (2) 컨베이어 가동 중 → PCB 감지되면 지정된 시나리오 수행
                if (hasMotors && DIO.GetDORead(motorCcw)) // 현재 CCW 가동 가정
                {
                    if (pcbDocked)
                    {
                        // 클램프/핀다운 진행 순서 제어
                        // (a) FuncInline.PinDownAndClamp == true → 핀다운 먼저, 이후 클램프
                        // (b) false → 클램프 먼저, 이후 핀다운
                        if (!GlobalVar.Simulation)
                        {
                            // 약간의 지연(클램핑 위치 보정용)
                            Thread.Sleep(1000);
                        }

                        if (FuncInline.PCBInfo[(int)site].BuyerChange)
                        {
                            // 바이어체인지(블루) : 감지되면 바로 컨베이어 정지만 (핀/클램프는 유지)
                            DIO.WriteDOData(motorCw, false);
                            DIO.WriteDOData(motorCcw, false);
                        }
                        else
                        {
                            if (FuncInline.PinDownAndClamp)
                            {
                                // 핀다운 → (테스트가 언클램프면 스킵 / 아니면) 클램프
                                if (hasUpDown && !contactDown) DIO.WriteDOData(updownDo, true);

                                if (!FuncInline.TestWithSiteUnclamp && hasStopper)
                                    DIO.WriteDOData(stopperDo, true);
                            }
                            else
                            {
                                // 클램프 → (테스트가 언클램프면 스킵) → 핀다운
                                if (!FuncInline.TestWithSiteUnclamp && hasStopper && !clampOn)
                                    DIO.WriteDOData(stopperDo, true);

                                if (hasUpDown && !contactDown) DIO.WriteDOData(updownDo, true);
                            }

                            // 컨베이어 정지
                            DIO.WriteDOData(motorCw, false);
                            DIO.WriteDOData(motorCcw, false);
                        }

                        // 동작 이후 타이머는 유지(완료 조건 블록에서 판단)
                    }
                }

                // 아직 최종 완료는 아니므로 계속 false
                return false;
            }
            catch (Exception ex)
            {
                Util.Debug("SitePCBLoad : " + ex.ToString());
                Util.Debug(ex.StackTrace);
                return false;
            }
        }


        #region 부위별 오리진 관련
        // 부위별 오리진 여부 체크
        public static bool CheckOriginDone(FuncInline.enumInitialize part)
        {
            // ========= 사이트 공용 체크 (Site1_F_DT1 ~ Site26_R_FT3) =========
            if (part >= FuncInline.enumInitialize.Site1_F_DT1 &&
                part <= FuncInline.enumInitialize.Site26_R_FT3)
            {
                int siteIndex = (int)part - (int)FuncInline.enumInitialize.Site1_F_DT1;
                var sitePos = FuncInline.enumTeachingPos.Site1_F_DT1 + siteIndex;

                bool ok = true; //And 연산으로 함

                // 1) 컨택트 UP/DOWN SOL 은 OFF 상태여야 함
                if (FuncInline.SiteIoMaps.TryGetContactUpDownDO(sitePos, out var updownDo))
                {
                    ok &= !DIO.GetDORead(updownDo);
                }

                // 2) 컨택트 스토퍼 SOL 도 OFF 상태여야 함
                if (FuncInline.SiteIoMaps.TryGetContactStopperDO(sitePos, out var stopperDo))
                {
                    ok &= !DIO.GetDORead(stopperDo);
                }

                // 3) 사이트 이송 모터(정/역)도 OFF 상태여야 함
                if (FuncInline.SiteIoMaps.TryGetSiteTransportMotorPair(sitePos, out var cwDo, out var ccwDo))
                {
                    ok &= !DIO.GetDORead(cwDo) && !DIO.GetDORead(ccwDo);
                }

                return ok;
            }

            switch (part)
            {
                case FuncInline.enumInitialize.InConveyor:
                    return FuncInline.PMCStatus[(int)FuncInline.enumPMCAxis.ST03_InConveyor_Width].isHomed ;

                case FuncInline.enumInitialize.InShuttle:
                    return GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.SV00_In_Shuttle].isHomed &&
                                                                    //GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.SV00_In_Shuttle].HomeAbsSwitch &&
                                                                    FuncInline.PMCStatus[(int)FuncInline.enumPMCAxis.ST00_InShuttle_Width].isHomed &&
                                                                     !DIO.GetDIData(FuncInline.enumDINames.X302_2_In_Shuttle_Stopper_Cyl_Up_Sensor) &&
                                                                     
                                                                        !DIO.GetDORead((int)FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw) &&
                                                                        !DIO.GetDORead((int)FuncInline.enumDONames.Y400_3_In_Shuttle_Motor_Ccw)&&
                                                                        !DIO.GetDORead((int)FuncInline.enumDONames.Y4_0_IN_Shuttle_Turn_CW_Cylinder) &&
                                                                        DIO.GetDORead((int)FuncInline.enumDONames.Y412_0_IN_Shuttle_Turn_CCW_Cylinder); //원점은 CCW
               
                case FuncInline.enumInitialize.OutShuttle:
                    return GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.SV01_Out_Shuttle].isHomed &&                                                                
                                                                    FuncInline.PMCStatus[(int)FuncInline.enumPMCAxis.ST01_OutShuttle_Width].isHomed &&
                                                                    !DIO.GetDIData(FuncInline.enumDINames.X303_5_Out_Shuttle_Stopper_Cyl_IN_Sensor) &&
                                                                    !DIO.GetDIData(FuncInline.enumDINames.X302_5_Out_Shuttle_Stopper_Cyl_STOP_Sensor) &&
                                                                        !DIO.GetDORead((int)FuncInline.enumDONames.Y304_0_Out_Shuttle_Ok_Motor_Cw) &&
                                                                        !DIO.GetDORead((int)FuncInline.enumDONames.Y402_7_Out_Shuttle_Ok_Motor_Ccw) &&
                                                                         !DIO.GetDORead((int)FuncInline.enumDONames.Y400_5_Out_Shuttle_Ng_Motor_Cw) &&
                                                                        !DIO.GetDORead((int)FuncInline.enumDONames.Y400_7_Out_Shuttle_Ng_Motor_Ccw) &&
                                                                        !DIO.GetDORead((int)FuncInline.enumDONames.Y304_3_Out_Shuttle_Turn_Cw_Cylinder) &&
                                                                        DIO.GetDORead((int)FuncInline.enumDONames.Y304_4_Out_Shuttle_Turn_Ccw_Cylinder);    //원점은 CCW

                case FuncInline.enumInitialize.NgBuffer:
                    return FuncInline.PMCStatus[(int)FuncInline.enumPMCAxis.ST04_NGBuffer].isHomed &&                                                     
                                                                        !DIO.GetDORead((int)FuncInline.enumDONames.Y402_5_Out_Conveyor_Ng_Motor_Cw)&&
                                                                        FuncInline.InlineType >= FuncInline.enumInlineType.Gen5
                                                                        ? !DIO.GetDORead((int)FuncInline.enumDONames.Y412_6_Ngbuffer_Upper_cylinder_forward)
                                                                        : true &&
                                                                        FuncInline.InlineType >= FuncInline.enumInlineType.Gen5
                                                                        ? !DIO.GetDORead((int)FuncInline.enumDONames.Y412_7_NgBuffer_Lower_cylinder_forward)
                                                                        : true &&
                                                                        FuncInline.InlineType >= FuncInline.enumInlineType.Gen5
                                                                        ? DIO.GetDORead((int)FuncInline.enumDONames.Y412_5_Ngbuffer_Upper_cylinder_backward)
                                                                        : true &&
                                                                        FuncInline.InlineType >= FuncInline.enumInlineType.Gen5
                                                                        ? DIO.GetDORead((int)FuncInline.enumDONames.Y4_7_Ngbuffer_Lower_cylinder_backward)
                                                                        : true;
                case FuncInline.enumInitialize.FrontLift:
                    return GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.SV02_Lift1].isHomed &&
                                                                    GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.SV03_Rack1_Width].isHomed &&
                                                                    !DIO.GetDIData(FuncInline.enumDINames.X403_7_Front_Lift_Stopper_Cyl_Sensor) &&
                                                                    FuncInline.InlineType >= FuncInline.enumInlineType.Gen5
                                                                    ?!DIO.GetDORead((int)FuncInline.enumDONames.Y1_7_Front_PASSLINE_PCB_STOPPER_SOL)
                                                                    : true  &&
                                                                    !DIO.GetDORead((int)FuncInline.enumDONames.Y405_0_Front_Lift_Up_Motor_Cw) &&
                                                                    !DIO.GetDORead((int)FuncInline.enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw);
                case FuncInline.enumInitialize.Lift2:
                    return GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.SV04_Lift2].isHomed &&
                                                                    GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.SV05_Rack2_Width].isHomed &&
                                                                    DIO.GetDIData(FuncInline.enumDINames.X405_3_Rear_Lift_Stopper_Cyl_IN_UP_Sensor) &&
                                                                    FuncInline.InlineType >= FuncInline.enumInlineType.Gen5
                                                                    ? !DIO.GetDORead((int)FuncInline.enumDONames.Y1_6_Rear_OK_PassLine_CONTACT_STOPPER_SOL)
                                                                    : true &&
                                                                     FuncInline.InlineType >= FuncInline.enumInlineType.Gen5
                                                                    ? !DIO.GetDORead((int)FuncInline.enumDONames.Y4_5_Rear_NG_PassLine_CONTACT_STOPPER_SOL)
                                                                    : true &&
                                                                    !DIO.GetDORead((int)FuncInline.enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw) &&
                                                                    !DIO.GetDORead((int)FuncInline.enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw);

                case FuncInline.enumInitialize.Scan:
                    return GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.SV06_Scan_X].isHomed &&
                                                                    GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.SV07_Scan_Y].isHomed; 


            }
            return false;
        }

      

     
        // 부위별 오리진 실행
        public static void RunOrigin(FuncInline.enumInitialize part)
        {
            FuncInline.enumServoAxis servoAxis = FuncInline.enumServoAxis.SV00_In_Shuttle;
            FuncInline.enumPMCAxis stepAxis = FuncInline.enumPMCAxis.ST00_InShuttle_Width;
            int motion_index = 0;

            // ========= 사이트 공용 처리 (Site1_F_DT1 ~ Site26_R_FT3) =========
            if (part >= FuncInline.enumInitialize.Site1_F_DT1 &&
                part <= FuncInline.enumInitialize.Site26_R_FT3)
            {
                // enumInitialize → siteIndex → enumTeachingPos
                int siteIndex = (int)part - (int)FuncInline.enumInitialize.Site1_F_DT1;
                var sitePos = FuncInline.enumTeachingPos.Site1_F_DT1 + siteIndex;

                // 1) 컨택트 UP/DOWN 솔 OFF
                if (FuncInline.SiteIoMaps.TryGetContactUpDownDO(sitePos, out var updownDo))
                {
                    DIO.WriteDOData(updownDo, false);
                }

                // 2) 컨택트 스토퍼 솔 OFF
                if (FuncInline.SiteIoMaps.TryGetContactStopperDO(sitePos, out var stopperDo))
                {
                    DIO.WriteDOData(stopperDo, false);
                }

                // 3) 사이트 이송 모터(정/역) OFF
                if (FuncInline.SiteIoMaps.TryGetSiteTransportMotorPair(sitePos, out var cwDo, out var ccwDo))
                {
                    DIO.WriteDOData(cwDo, false);
                    DIO.WriteDOData(ccwDo, false);
                }

                // 사이트는 서보/PMC 홈 동작이 없는 구성이라면 여기서 종료
                return;
            }

            switch (part)
            {
                #region IN Shuttle
                //*
                case FuncInline.enumInitialize.InShuttle:
                    if (!DIO.GetDORead((int)FuncInline.enumDONames.Y4_0_IN_Shuttle_Turn_CW_Cylinder) &&
                        DIO.GetDORead((int)FuncInline.enumDONames.Y412_0_IN_Shuttle_Turn_CCW_Cylinder) &&
                        DIO.GetDIData(FuncInline.enumDINames.X302_7_In_Shuttle_Turn_Ccw_Cyl_Sensor))
                    {
                        #region 서보 호밍
                        servoAxis = FuncInline.enumServoAxis.SV00_In_Shuttle;
                        if (GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.SV01_Out_Shuttle].isHomed)
                        {

                            if (!GlobalVar.AxisStatus[(int)servoAxis].isHomed &&
                            GlobalVar.AxisStatus[(int)servoAxis].StandStill)
                            {
                                FuncInlineMove.MoveHome((uint)servoAxis);
                            }
                        }
                        #endregion
                        #region 폭조절
                        stepAxis = FuncInline.enumPMCAxis.ST00_InShuttle_Width;
                        motion_index = (int)((int)stepAxis / 2);
                        // 홈도 언클램프도 아니면 호밍
                        if (!FuncInline.PMCStatus[(int)stepAxis].isHomed &&
                            //!FuncInline.PMCStatus[(int)stepAxis].Home && // 홈 센서 감지 안 되면
                            FuncInline.PMCStatus[(int)stepAxis].StandStill)
                        {
                            //FuncInline.ComPMC[motion_index].HomeRun(stepAxis, GlobalVar.WidthHomSpeed);
                        }
                      
                        #endregion
                    }
                   
                    DIO.WriteDOData(FuncInline.enumDONames.Y302_2_IN_Shuttle_CONTACT_STOPPER_SOL, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y400_3_In_Shuttle_Motor_Ccw, false);
                    //턴 복동
                    DIO.WriteDOData(FuncInline.enumDONames.Y4_0_IN_Shuttle_Turn_CW_Cylinder, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y412_0_IN_Shuttle_Turn_CCW_Cylinder, true);
                    break;
                //*/
                #endregion
                #region NG Buffer
                //*
                //by DG 수정필요
                case FuncInline.enumInitialize.NgBuffer:
                    
                    #region 폭조절
                    stepAxis = FuncInline.enumPMCAxis.ST04_NGBuffer;
                    motion_index = (int)((int)stepAxis / 2);
                    // 홈도 언클램프도 아니면 호밍
                    if (//!FuncInline.CheckNearPos(stepAxis, false) && // 언클램프 위치
                        !FuncInline.PMCStatus[(int)stepAxis].isHomed && // 홈 위치 아니거나
                        FuncInline.PMCStatus[(int)stepAxis].StandStill)
                    {
                        ////FuncInline.ComPMC[motion_index].HomeRun(stepAxis, GlobalVar.WidthHomSpeed);
                    }
                  
                    #endregion
                    DIO.WriteDOData(FuncInline.enumDONames.Y402_5_Out_Conveyor_Ng_Motor_Cw, false);
                    if( FuncInline.InlineType >= FuncInline.enumInlineType.Gen5)
                    {
                        DIO.WriteDOData(FuncInline.enumDONames.Y412_6_Ngbuffer_Upper_cylinder_forward, false);
                        DIO.WriteDOData(FuncInline.enumDONames.Y412_7_NgBuffer_Lower_cylinder_forward, false);
                        DIO.WriteDOData(FuncInline.enumDONames.Y412_5_Ngbuffer_Upper_cylinder_backward, true);
                        DIO.WriteDOData(FuncInline.enumDONames.Y4_7_Ngbuffer_Lower_cylinder_backward, true);
                    }
               
                    break;
                //*/
                #endregion
                #region Out Shuttle
                ///*
                case FuncInline.enumInitialize.OutShuttle:
                    if (!DIO.GetDORead((int)FuncInline.enumDONames.Y304_3_Out_Shuttle_Turn_Cw_Cylinder) &&
                        DIO.GetDORead((int)FuncInline.enumDONames.Y304_4_Out_Shuttle_Turn_Ccw_Cylinder) &&
                        DIO.GetDIData(FuncInline.enumDINames.X303_2_Out_Shuttle_Turn_Ccw_Cyl_Sensor))
                    {
                        if (GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.SV01_Out_Shuttle].isHomed)
                        {
                            #region 서보 호밍
                            servoAxis = FuncInline.enumServoAxis.SV01_Out_Shuttle;
                            if (!GlobalVar.AxisStatus[(int)servoAxis].isHomed &&
                                GlobalVar.AxisStatus[(int)servoAxis].StandStill)
                            {
                                FuncInlineMove.MoveHome((uint)servoAxis);
                            }
                        }
                        #region 폭조절
                        stepAxis = FuncInline.enumPMCAxis.ST01_OutShuttle_Width;
                        motion_index = (int)((int)stepAxis / 2);
                        // 홈도 언클램프도 아니면 호밍
                        if (!FuncInline.PMCStatus[(int)stepAxis].isHomed &&
                            FuncInline.PMCStatus[(int)stepAxis].StandStill)
                        {
                            //FuncInline.ComPMC[motion_index].HomeRun(stepAxis, GlobalVar.WidthHomSpeed);
                        }


                        #endregion
                    }
                    #endregion

                    DIO.WriteDOData(FuncInline.enumDONames.Y300_2_Out_Shuttle_CONTACT_STOPPER_IN_SOL, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y302_1_Out_Shuttle_CONTACT_STOPPER_Out_SOL, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y304_0_Out_Shuttle_Ok_Motor_Cw, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y402_7_Out_Shuttle_Ok_Motor_Ccw, false);
                    break;
                //*/
                #endregion
                #region FrontLift
                //*
                case FuncInline.enumInitialize.FrontLift:
                    #region 서보 호밍
                    servoAxis = FuncInline.enumServoAxis.SV02_Lift1;
                    if (!GlobalVar.AxisStatus[(int)servoAxis].isHomed &&
                        GlobalVar.AxisStatus[(int)servoAxis].StandStill)
                    {
                        FuncInlineMove.MoveHome((uint)servoAxis);
                    }
                    #endregion
                    #region 폭조절
                    FuncInline.enumServoAxis widthAxis = FuncInline.enumServoAxis.SV02_Lift1;
           
                    // 홈도 언클램프도 아니면 호밍
                    if (!GlobalVar.AxisStatus[(int)widthAxis].isHomed &&
                        GlobalVar.AxisStatus[(int)widthAxis].StandStill)
                    {
                        FuncInlineMove.MoveHome((uint)widthAxis);
                    }
                   
                    #endregion
                    
                    DIO.WriteDOData(FuncInline.enumDONames.Y405_0_Front_Lift_Up_Motor_Cw, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y405_4_Front_Lift_Down_Motor_Cw, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y405_6_Front_Lift_Down_Motor_Ccw, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y300_1_Front_Lift_CONTACT_STOPPER_SOL, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y404_1_Front_Passline_Motor_Cw, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y1_7_Front_PASSLINE_PCB_STOPPER_SOL, false);
                    break;
                //*/
                #endregion
                #region Lift2
                //*
                case FuncInline.enumInitialize.Lift2:
                    #region 서보 호밍
                    servoAxis = FuncInline.enumServoAxis.SV04_Lift2;
                    if (!GlobalVar.AxisStatus[(int)servoAxis].isHomed &&
                        GlobalVar.AxisStatus[(int)servoAxis].StandStill)
                    {
                        FuncInlineMove.MoveHome((uint)servoAxis);
                    }
                    #endregion
                    #region 폭조절
                    widthAxis = FuncInline.enumServoAxis.SV05_Rack2_Width;  //Rear Lift, Rack

                    // 홈도 언클램프도 아니면 호밍
                    if (!GlobalVar.AxisStatus[(int)widthAxis].isHomed &&
                        GlobalVar.AxisStatus[(int)widthAxis].StandStill)
                    {
                        FuncInlineMove.MoveHome((uint)widthAxis);
                    }

                    #endregion

                    DIO.WriteDOData(FuncInline.enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y305_1_Rear_Lift_Down_Motor_Cw, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y304_1_Rear_Lift_Down_Motor_Ccw, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y302_0_Rear_Lift_CONTACT_STOPPER_IN_SOL, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y300_0_Rear_Lift_CONTACT_STOPPER_Out_SOL, false);

                    DIO.WriteDOData(FuncInline.enumDONames.Y1_6_Rear_OK_PassLine_CONTACT_STOPPER_SOL, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y4_5_Rear_NG_PassLine_CONTACT_STOPPER_SOL, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y305_4_Rear_PassLine_Motor_Cw, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y404_0_Rear_NgLine_Motor_Cw, false);
                    break;
                //*/
                #endregion
          
        
            }
        }

        #endregion


   




        public static bool InitAllWidth()
        {
            for (int i = 0; i < FuncInline.WidthStarted.Length; i++)
            {
                FuncInline.WidthStarted[i] = false;
                FuncInline.WidthDone[i] = false;
            }

            // 인터락 센서 감지시 폭조절 금지
            if (DIO.GetDIData(FuncInline.enumDINames.X114_7_Front_Rack_PCB_Interlock_Sensor) ||
                DIO.GetDIData(FuncInline.enumDINames.X405_2_Rear_Rack_PCB_Interlock_Sensor))
            {
                return false;
            }


            WidthInit dlgInit = new WidthInit();
            dlgInit.Show();

            Stopwatch watch = new Stopwatch();
            Util.StartWatch(ref watch);
            bool allWidthInited = false;
            try
            {
                while (watch.ElapsedMilliseconds < 9990 * 1000)
                {
                    if (GlobalVar.E_Stop ||
                        GlobalVar.DoorOpen ||
                        GlobalVar.LightCurtain ||
                        FuncInline.Loto_Switch)
                    {
                        StopAllMotor();
                        dlgInit.Close();
                        return false;
                    }

                    for (int i = 0; i < FuncInline.WidthDone.Length; i++)
                    {
                       
                        // 스텝모터
                        if (i < Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length)
                        {
                            FuncInline.enumPMCAxis pmcAxis = (FuncInline.enumPMCAxis)i;

                            if (!FuncInline.PMCStatus[i].StandStill)
                            {
                                //allInited = false;
                            }
                            else if (FuncInline.PMCStatus[i].StandStill &&
                                !FuncInlineMove.CheckNearPos(pmcAxis, false))
                            {

                                //by DG 수정필요
                                //debug(pmcAxis.ToString() + " 폭 조절 1");
                                ////PMCClass.ABSMove(pmcAxis, FuncInline.TeachingWidth[i] + FuncInline.WidthClampOffset, GlobalVar.WidthSpeed);
                                ////FuncInline.ComPMC[(int)((int)pmcAxis / 2)].ABSMove(pmcAxis, FuncInline.TeachingWidth[i] + FuncInline.WidthClampOffset, GlobalVar.WidthSpeed);
                                FuncInline.WidthStarted[i] = true;
                                //allInited = false;
                            }
                            else if (//!FuncInline.WidthDone[i] &&
                                FuncInline.PMCStatus[i].StandStill &&
                                FuncInlineMove.CheckNearPos(pmcAxis, false))
                            {
                                //debug(pmcAxis.ToString() + " 폭 조절 완료 2");
                                FuncInline.WidthDone[i] = true;
                            }
                        }
                        // 서보모터
                        else if (i >= Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length)    //서보모터2개
                        {
                            FuncInline.enumServoAxis servoAxis = 0;
                            if (i == 5)
                            {
                                servoAxis = FuncInline.enumServoAxis.SV03_Rack1_Width;
                            }
                            else if(i == 6)
                            {
                                servoAxis = FuncInline.enumServoAxis.SV05_Rack2_Width;
                            }
                            
                            if (!GlobalVar.AxisStatus[(int)servoAxis].StandStill)
                            {
                                //allInited = false;
                                FuncInline.WidthDone[i] = false;
                            }
                            // 이외 원점 위치면 지정폭으로 이동
                            else if (GlobalVar.AxisStatus[(int)servoAxis].StandStill &&
                                !FuncInline.WidthDone[i] &&
                                !FuncInlineMove.CheckNearPos(servoAxis, false)) // 티칭 위치 아니면
                            {
                                //debug(servoAxis.ToString() + " 이외 폭이 다르면 지정폭으로 이동 4");
                                FuncInlineMove.MoveAbsolute((uint)servoAxis, FuncInline.TeachingWidth[i] + FuncInline.WidthClampOffset, 20);
                                FuncInline.WidthStarted[i] = true;
                                FuncInline.WidthDone[i] = false;
                                //allInited = false;
                            }
                            // 초기화 완료
                            else if (//!FuncInline.WidthDone[i] &&
                                GlobalVar.AxisStatus[(int)servoAxis].StandStill &&
                                FuncInlineMove.CheckNearPos(servoAxis, false))
                            {
                                //debug(servoAxis.ToString() + " 폭 초기화 완료 5");
                                FuncInline.WidthDone[i] = true;
                            }
                            else
                            {
                                //allInited = false;
                            }
                            //else if (!Util.CheckNearPos(FuncInline.TeachingWidth[i] + FuncInline.WidthClampOffset,
                            //                       FuncMotion.GetRealPosition((int)servoAxis),
                            //                       0.1))
                            //{
                            //    allInited = false;
                            //    FuncInline.WidthDone[i] = false;
                            //}
                        }
               
                        else
                        {
                           

                        }
                        dlgInit.RefreshStep();
                    }
                    bool allInited = true;
                    for (int i = 0; i < FuncInline.WidthDone.Length; i++)
                    {
                        if (!FuncInline.WidthDone[i])
                        {
                            allInited = false;
                            break;
                        }
                    }
                    if (allInited)
                    {
                        allWidthInited = true;
                        break;
                    }

                    Thread.Sleep(GlobalVar.ThreadSleep);
                }

            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
            dlgInit.Close();
            return allWidthInited;

        }

        public static void StopAllMotor()
        {
            for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumServoAxis)).Length; i++)
            {
                FuncMotion.MoveStop(i);
            }
            for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length; i++)
            {
                // by DG 수정필요
                ////PMCClass.Stop((FuncInline.enumPMCAxis)i);
                ////FuncInline.ComPMC[(int)(i / 2)].Stop((FuncInline.enumPMCAxis)i);
            }
        }


    }
}
