using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Threading.Tasks;
using System.IO;



namespace Radix
{
    /**
     * @brief 알람 및 에러 처리 관련 함수 선언
     */
    public static class FuncError
    {
        /*
         * FuncError.cs : 알람 및 에러 처리 관련 함수 선언
         */

        /**
         * @brief 로컬 디버그 함수
         * @param str 디버그 처리할 문자열
         */
        private static void debug(string str)
        {
            Util.Debug("FuncError : " + str);
        }

        //public static void AddError(FuncInline.enumError er) // AddError(에러명) 에러발생 큐에 추가
        //{
        //    //FuncLog.WriteLog("AddError(" + er.ToString() + ") : " + GlobalVar.SystemStatus.ToString());
        //    if (GlobalVar.SystemStatus == enumSystemStatus.BeforeInitialize &&
        //        er == FuncInline.enumError.SECS_Not_Connect)
        //    {
        //        return;
        //    }
        //    if (er == FuncInline.enumError.No_Error)
        //    {
        //        return;
        //    }
        //    if (!GlobalVar.SystemError[(int)er])
        //    {
        //        try
        //        {
        //            GlobalVar.SystemError[(int)er] = true;
        //            switch (er)
        //            {
        //                case FuncInline.enumError.No_Error:
        //                    break;
        //                case FuncInline.enumError.E_Stop:
        //                    GlobalVar.E_Stop = true;
        //                    if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
        //                    {
        //                        GlobalVar.SystemStatus = enumSystemStatus.ErrorStop;
        //                    }
        //                    else if (GlobalVar.SystemStatus <= enumSystemStatus.Initialize)
        //                    {
        //                        GlobalVar.SystemStatus = enumSystemStatus.BeforeInitialize;
        //                    }
        //                    else
        //                    {
        //                        GlobalVar.SystemStatus = enumSystemStatus.EmgStop;
        //                    }
        //                    #region 로봇 정지
        //                    FuncRobot.RobotHold(true);
        //                    #endregion


        //                    break;
        //                case FuncInline.enumError.System_Not_Inited:
        //                    GlobalVar.SystemStatus = enumSystemStatus.BeforeInitialize;
        //                    FuncLog.WriteLog("HJ 확인 - AddError Func.cs(System_Not_Inited)     enumSystemStatus.BeforeInitialize");
        //                    break;

        //                case FuncInline.enumError.Door_Opened:
        //                    //원본
        //                    //GlobalVar.SystemStatus = enumSystemStatus.ErrorStop;
        //                    //HJ 수정 200407 에러 발생시 스태이터스 변경 
        //                    if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
        //                    {
        //                        GlobalVar.SystemStatus = enumSystemStatus.ErrorStop;
        //                    }
        //                    GlobalVar.DoorOpen = true;
        //                    break;
        //                case FuncInline.enumError.Run_Stopped:
        //                    break;
        //                case FuncInline.enumError.Operator_Call:
        //                    break;
        //                case FuncInline.enumError.Digital_Input_Check:
        //                    break;
        //                case FuncInline.enumError.Digital_Output_Check:
        //                    break;

        //                case FuncInline.enumError.SECS_TimeOut:
        //                    if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
        //                    {
        //                        GlobalVar.SystemStatus = enumSystemStatus.ErrorStop;
        //                    }
        //                    break;
        //                case FuncInline.enumError.SECS_Not_Connect:
        //                    // 확인 해바야 된다.
        //                    break;              
        //                case FuncInline.enumError.Robot_Loading_Error:
        //                case FuncInline.enumError.Robot_UnLoading_Error:
        //                case FuncInline.enumError.Robot_Working_Error:

        //                    DNet.WriteDNetData(FuncInline.enumDnetONames.DO_017_Cycle_End, true);
        //                    DNet.WriteDNetData();
        //                    Thread.Sleep(300);

        //                    GlobalVar.SystemStatus = enumSystemStatus.Manual;

        //                    break;
        //                default:
        //                    GlobalVar.SystemStatus = enumSystemStatus.Manual;
        //                    break;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.ToString());
        //            Console.WriteLine(ex.StackTrace);
        //        }
        //        GlobalVar.SystemErrored = true;
        //        //GlobalVar.SystemErrorQueue.Enqueue(er);
        //        GlobalVar.SystemMsg = "Alarm : " + er.ToString();
        //        FuncLog.WriteLog("Alarm - " + er.ToString());
        //    }
        //}

        //public static void AddError(int er) // 프로젝트별로 에러 관리하기 위해 enum 처리 프로젝트별
        //{
        //    try
        //    {
        //        if (er < Enum.GetValues(typeof(FuncInline.enumError)).Length)
        //        {
        //            AddError((FuncInline.enumError)er);
        //        }      
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //        Console.WriteLine(ex.StackTrace);
        //    }
        //}

        //public static void AddError(FuncInline.enumErrorCode er, FuncInline.enumTeachingPos part) // AddError(에러명) 에러발생 큐에 추가
        //{
        //    if (er == FuncInline.enumErrorCode.No_Error)
        //    {
        //        return;
        //    }
        //    FuncInline.enumErrorPart.ePart = FuncInline.enumErrorPart.System;
        //    Array errors = Enum.GetValues(typeof(FuncInline.enumErrorPart.);
        //    for (int i = 0; i < errors.Length; i++)
        //    {
        //        if (((FuncInline.enumErrorPart)i).ToString() == part.ToString())
        //        {
        //            ePart = (FuncInline.enumErrorPart)i;
        //        }
        //    }
        //    structError sError = new FuncInline.structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                        DateTime.Now.ToString("HH:mm:ss"),
        //                                        ePart,
        //                                        er,
        //                                        false,
        //                                        "");
        //    AddError(sError);
        //}

        //public static void RemoveError(FuncInline.enumError er) // RemoveError(에러명) 발생된 에러 삭제
        //{
        //    GlobalVar.SystemError[(int)er] = false;
        //    bool errorExist = false;
        //    for (int i = 0; i < GlobalVar.SystemError.Length; i++)
        //    {
        //        if (GlobalVar.SystemError[i])
        //        {
        //            errorExist = true;
        //        }
        //    }
        //    GlobalVar.SystemErrored = errorExist;
        //    if (!GlobalVar.SystemErrored)
        //    {
        //        GlobalVar.SystemMsg = "";
        //    }
        //    DIO.WriteDOData(FuncInline.enumDONames.Y412_2_Tower_Lamp_Buzzer, false);

        //    HighGain_Router.Router_Reset(true);
        //    Thread.Sleep(500);
        //    HighGain_Router.Router_Reset(false);

        //    FuncLog.WriteLog("Alarm Clear - " + er.ToString());

        //}


        /**
         * @brief 발생된 에러 중 해당 에러코드에 해당하는 에러 삭제
         * @param er 삭제할 에러 코드
         */
        public static void RemoveError(object er) // RemoveError(에러명) 발생된 에러 삭제
        {
            for (int i = GlobalVar.SystemErrorQueue.Count - 1; i >= 0; i--)
            {
                if ((int)(((FuncInline.structError)GlobalVar.SystemErrorQueue.ElementAt(i)).ErrorCode) == (int)er)
                {
                    RemoveErrorQueue(i);

                    #region DataBase에 추가. 프로젝트별 에러 특성이 다를 수 있어 각 프로젝트 파일에 별도 선언해서 사용
                    FuncInline.ClearSystemErrorCode((int)er);
                    #endregion
                }
            }

            GlobalVar.SystemErrored = GlobalVar.SystemErrorQueue.Count > 0;
            if (!GlobalVar.SystemErrored)
            {
                GlobalVar.SystemMsg = "";
            }
            DIO.WriteDOData(FuncInline.enumDONames.Y412_2_Tower_Lamp_Buzzer, false);

            FuncLog.WriteLog("Alarm Clear - " + er.ToString());

        }



        /**
         * @brief 발생된 에러 중 해당 파트와 에러코드에 해당하는 에러 삭제
         * @param part 삭제할 에러 파트
         * @param er 삭제할 에러 코드
         */
        public static void RemoveError(object part, object er) // RemoveError(에러명) 발생된 에러 삭제
        {
            for (int i = GlobalVar.SystemErrorQueue.Count - 1; i >= 0; i--)
            {
                if ((int)(((FuncInline.structError)GlobalVar.SystemErrorQueue.ElementAt(i)).ErrorPart) == (int)part &&
                    (int)(((FuncInline.structError)GlobalVar.SystemErrorQueue.ElementAt(i)).ErrorCode) == (int)er)
                {
                    RemoveErrorQueue(i);

                    #region DataBase에 추가. 프로젝트별 에러 특성이 다를 수 있어 각 프로젝트 파일에 별도 선언해서 사용
                    FuncInline.ClearSystemError(part.ToString(), (int)er);
                    #endregion
                }
            }

            GlobalVar.SystemErrored = GlobalVar.SystemErrorQueue.Count > 0;
            if (!GlobalVar.SystemErrored)
            {
                GlobalVar.SystemMsg = "";
            }
            DIO.WriteDOData(FuncInline.enumDONames.Y412_2_Tower_Lamp_Buzzer, false);

            FuncLog.WriteLog("Alarm Clear - " + part.ToString() + "," + er.ToString());
        }

        /**
         * @brief 전체 알람 삭제
         */
        public static void RemoveAllError() // 전체 알람 삭제
        {
            while (GlobalVar.SystemErrorQueue.Count > 0)
            {
                FuncInline.structError er = new FuncInline.structError();
                GlobalVar.SystemErrorQueue.TryDequeue(out er);
            }
            while (GlobalVar.SystemErrorListQueue.Count > 0)
            {
                FuncInline.structError er = new FuncInline.structError();
                GlobalVar.SystemErrorListQueue.TryDequeue(out er);
            }
            if (GlobalVar.E_Stop ||
                GlobalVar.SystemStatus < enumSystemStatus.Manual)
            {
                GlobalVar.SystemStatus = enumSystemStatus.BeforeInitialize;
            }
            else
            {
                GlobalVar.SystemStatus = enumSystemStatus.Manual;
            }


            GlobalVar.E_Stop = false;
            GlobalVar.DoorOpen = false;
            GlobalVar.SystemErrored = GlobalVar.SystemErrorListQueue.Count > 0 ? true : false;

            #region DataBase에 추가
            //string sql = "update [SystemError] " +
            //                "set " +
            //                "[Clear] = '1' " +
            //                "where [Clear] = '0'";
            //GlobalVar.Sql.Execute(sql);
            FuncInline.ClearAllSystemError();
            #endregion
        }

        //public static bool CheckError(FuncInline.enumError er) // CheckError(에러명) 에러 발생 여부 확인
        //{
        //    try
        //    {
        //        return GlobalVar.SystemError[(int)er];
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //        Console.WriteLine(ex.StackTrace);
        //    }
        //    return false;
        //}

        /**
         * @brief 특정 파트에 에러 발생 여부 확인
         * @param part 검색할 에러 파트
         */
        public static bool CheckError(object part) // CheckError(파트) 에러 발생 여부 확인
        {
            try
            {
                for (int i = 0; i < GlobalVar.SystemErrorQueue.Count; i++)
                {
                    if ((int)(((FuncInline.structError)GlobalVar.SystemErrorQueue.ElementAt(i)).ErrorPart) == (int)part)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
            return false;
        }


        public static void AddError(FuncInline.structError er) // AddError(에러명) 에러발생 큐에 추가
        {
            //debug("AddError " + er.ErrorPart + " , " + er.ErrorCode.ToString() + " , " + er.Description);
            if (er.ErrorCode == FuncInline.enumErrorCode.No_Error)
            {
                return;
            }

            #region 시스템 정지 상태가 될 것이므로 스메마 신호 해제
            DIO.WriteDOData(FuncInline.enumDONames.Y4_2_SMEMA_Before_Ready, false);     //투입준비 완료 인셔틀
            DIO.WriteDOData(FuncInline.enumDONames.Y412_1_SMEMA_After_Ready, false);    //배출준비완료 아웃컨베이어
            #endregion

            //FuncInlineI.DoorControl(true);

            #region 사이트 경우 사이트 Open 해야 클리어 가능하게 세팅
            if (er.ErrorPart >= FuncInline.enumErrorPart.Site1_F_DT1 &&
                er.ErrorPart < FuncInline.enumErrorPart.Site1_F_DT1 + FuncInline.MaxSiteCount)
            {
                int siteIndex = (int)er.ErrorPart - (int)FuncInline.enumErrorPart.Site1_F_DT1;
                FuncInline.SiteClear[siteIndex] = true;
                FuncInline.SiteAction[siteIndex] = FuncInline.enumSiteAction.Waiting;
            }
            #endregion

            GlobalVar.EnableBuzzer = true;
            #region 발생 부위에 따라서 동작 삭제
            // 동작 삭제 코드 수정할 것
            /*
            switch (er.ErrorPart)
            {
                case FuncInline.enumErrorPart.InShuttle:
                    FuncInline.InShuttleAction = FuncInline.enumLiftAction.Waiting;
                    DIO.WriteDOData(FuncInline.enumDONames.Y4_2_SMEMA_Before_Ready, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y03_0_InShuttle_Conveyor_Run, false);
                    break;
                case FuncInline.enumErrorPart.Passline1:
                    FuncInline.Passline1Action = FuncInline.enumLiftAction.Waiting;
                    DIO.WriteDOData(FuncInline.enumDONames.Y05_5_Passline1_Conveyor_Forward, false);
                    break;
                case FuncInline.enumErrorPart.Lift1_Up:
                    FuncInline.Lift1Action = FuncInline.enumLiftAction.Waiting;
                    DIO.WriteDOData(FuncInline.enumDONames.Y405_0_Front_Lift_Up_Motor_Cw, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw, false);
                    FuncMotion.MoveStop((int)FuncInline.enumServoAxis.SV02_Lift1);
                    break;
                case FuncInline.enumErrorPart.Lift1_Down:
                    FuncInline.Lift1Action = FuncInline.enumLiftAction.Waiting;
                    DIO.WriteDOData(FuncInline.enumDONames.Y405_4_Front_Lift_Down_Motor_Cw, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y405_6_Front_Lift_Down_Motor_Ccw, false);
                    FuncMotion.MoveStop((int)FuncInline.enumServoAxis.SV02_Lift1);
                    break;
                case FuncInline.enumErrorPart.Passline2:
                    FuncInline.Passline2Action = FuncInline.enumLiftAction.Waiting;
                    DIO.WriteDOData(FuncInline.enumDONames.Y06_1_Passline2_Conveyor_Forward, false);
                    break;
                case FuncInline.enumErrorPart.Lift2_Up:
                    FuncInline.Lift2Action = FuncInline.enumLiftAction.Waiting;
                    DIO.WriteDOData(FuncInline.enumDONames.Y11_2_Lift2_Up_Conveyor_Forward, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y11_3_Lift2_Up_Conveyor_Backward, false);
                    FuncMotion.MoveStop((int)FuncInline.enumServoAxis.SV02_Lift1);
                    break;
                case FuncInline.enumErrorPart.Lift2_Down:
                    FuncInline.Lift2Action = FuncInline.enumLiftAction.Waiting;
                    DIO.WriteDOData(FuncInline.enumDONames.Y11_6_Lift2_Down_Conveyor_Forward, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y11_7_Lift2_Down_Conveyor_Backward, false);
                    FuncMotion.MoveStop((int)FuncInline.enumServoAxis.SV02_Lift1);
                    break;
                case FuncInline.enumErrorPart.OutShuttle:
                    FuncInline.OutShuttleAction = FuncInline.enumLiftAction.Waiting;
                    DIO.WriteDOData(FuncInline.enumDONames.Y412_1_SMEMA_After_Ready, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y06_1_OutShuttle_Conveyor_Forward, false);
                    break;
                case FuncInline.enumErrorPart.NgBuffer:
                    FuncInline.OutShuttleAction = FuncInline.enumLiftAction.Waiting;
                    DIO.WriteDOData(FuncInline.enumDONames.Y402_5_Out_Conveyor_Ng_Motor_Cw, false);
                    break;
            }
            //*/
            #endregion

            FuncInline.NeedPartClear[(int)er.ErrorPart] = true;

            if (!FuncError.CheckError(er))
            {
                FuncLog.WriteLog("AddError - " + er.ErrorPart + "," + er.ErrorCode.ToString() + "," + er.Description);

                try
                {
                    //GlobalVar.SystemError[(int)er.ErrorCode] = true;
                    switch (er.ErrorCode)
                    {
                        case FuncInline.enumErrorCode.E_Stop:
                            GlobalVar.E_Stop = true;
                            GlobalVar.SystemStatus = enumSystemStatus.BeforeInitialize;
                            break;
                        case FuncInline.enumErrorCode.System_Not_Inited:
                            GlobalVar.SystemStatus = enumSystemStatus.BeforeInitialize;
                           
                            break;

                        case FuncInline.enumErrorCode.Door_Opened:
                            //원본
                            //GlobalVar.SystemStatus = enumSystemStatus.ErrorStop;
                            //HJ 수정 200407 에러 발생시 스태이터스 변경 
                            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
                            {
                                GlobalVar.SystemStatus = enumSystemStatus.ErrorStop;
                            }
                            GlobalVar.DoorOpen = true;
                            break;

                        default:
                            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
                            {
                                GlobalVar.SystemStatus = enumSystemStatus.ErrorStop;
                            }
                            else
                            {
                                GlobalVar.SystemStatus = enumSystemStatus.Manual;
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                }
                GlobalVar.SystemErrored = true;
                GlobalVar.SystemErrorQueue.Enqueue(er);
                GlobalVar.SystemErrorListQueue.Enqueue(er);
                GlobalVar.SystemMsg = "Alarm : " + er.ToString();

                FuncLog.WriteLog("Alarm - " + er.ErrorCode.ToString());

                #region DataBase에 추가
                //string sql = "insert into [SystemError] " +
                //                "values (CONVERT(CHAR(8), getdate(), 112), " +
                //                    " CONVERT(CHAR(8), getdate(), 108), " +
                //                    "'" + er.ErrorPart.ToString() + "', " +
                //                    ((int)er.ErrorCode).ToString() + ", " +
                //                    "'" + er.ErrorCode.ToString() + "', " +
                //                    "'" + er.Description + "', " +
                //                    "'0')";
                //GlobalVar.Sql.Execute(sql);
                FuncInline.AddSystemError(er);

                #endregion

                //DIO.WriteDOData(FuncInline.enumDONames.Y00_6_Front_Door_Lock1, true);
                //DIO.WriteDOData(FuncInline.enumDONames.Y00_7_Front_Door_Lock2, true);
                //DIO.WriteDOData(FuncInline.enumDONames.Y01_1_Rear_Door_Lock1, true);
                //DIO.WriteDOData(FuncInline.enumDONames.Y01_2_Rear_Door_Lock2, true);

                for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumServoAxis)).Length; i++)
                {
                    FuncMotion.MoveStop(i);
                }
            }
        }

        public static void RemoveErrorQueue(int idx)
        {
            if (idx >= GlobalVar.SystemErrorQueue.Count)
            {
                return;
            }
            FuncInline.structError[] array = GlobalVar.SystemErrorQueue.ToArray();
            while (GlobalVar.SystemErrorQueue.Count > 0)
            {
                FuncInline.structError er = new FuncInline.structError();
                GlobalVar.SystemErrorQueue.TryDequeue(out er);
            }
            for (int i = 0; i < array.Length; i++)
            {
                if (i != idx)
                {
                    GlobalVar.SystemErrorQueue.Enqueue(array[i]);
                }
            }
        }

        public static void RemoveErrorListQueue(int idx)
        {
            if (idx >= GlobalVar.SystemErrorListQueue.Count)
            {
                return;
            }
            FuncInline.structError[] array = GlobalVar.SystemErrorListQueue.ToArray();
            while (GlobalVar.SystemErrorListQueue.Count > 0)
            {
                FuncInline.structError er = new FuncInline.structError();
                GlobalVar.SystemErrorListQueue.TryDequeue(out er);
            }
            for (int i = 0; i < array.Length; i++)
            {
                if (i != idx)
                {
                    GlobalVar.SystemErrorListQueue.Enqueue(array[i]);
                }
            }
        }

        public static void RemoveError(FuncInline.enumErrorCode er) // RemoveError(에러명) 발생된 에러 삭제
        {
            for (int i = GlobalVar.SystemErrorQueue.Count - 1; i >= 0; i--)
            {
                try
                {
                    if (((FuncInline.structError)GlobalVar.SystemErrorQueue.ElementAt(i)).ErrorCode == er)
                    {
                        RemoveErrorQueue(i);
                    }
                }
                catch (Exception ex)
                {
                    debug(ex.ToString());
                    debug(ex.StackTrace);
                }
            }
            for (int i = GlobalVar.SystemErrorListQueue.Count - 1; i >= 0; i--)
            {
                try
                {
                    if (((FuncInline.structError)GlobalVar.SystemErrorListQueue.ElementAt(i)).ErrorCode == er)
                    {
                        RemoveErrorListQueue(i);
                    }
                }
                catch (Exception ex)
                {
                    debug(ex.ToString());
                    debug(ex.StackTrace);
                }
            }
            #region DataBase에 추가
            FuncInline.ClearSystemErrorCode((int)er);
            #endregion

            GlobalVar.SystemErrored = GlobalVar.SystemErrorListQueue.Count > 0;
            if (!GlobalVar.SystemErrored)
            {
                GlobalVar.SystemMsg = "";
            }
            DIO.WriteDOData(FuncInline.enumDONames.Y412_2_Tower_Lamp_Buzzer, false);

            FuncLog.WriteLog("Alarm Clear - " + er.ToString());

        }

        public static void RemoveError(FuncInline.enumErrorPart part) // RemoveError(에러명) 발생된 에러 삭제
        {
            for (int i = GlobalVar.SystemErrorQueue.Count - 1; i >= 0; i--)
            {
                if (((FuncInline.structError)GlobalVar.SystemErrorQueue.ElementAt(i)).ErrorPart == part)
                {
                    RemoveErrorQueue(i);
                }
            }
            for (int i = GlobalVar.SystemErrorListQueue.Count - 1; i >= 0; i--)
            {
                if (((FuncInline.structError)GlobalVar.SystemErrorListQueue.ElementAt(i)).ErrorPart == part)
                {
                    RemoveErrorListQueue(i);
                }
            }
            #region DataBase에 추가
            //string sql = "update [SystemError] " +
            //                "set " +
            //                "[Clear] = '1' " +
            //                "where [ErrorPart] = " + part.ToString() + " " +
            //                "and [Clear] = '0'";
            //GlobalVar.Sql.Execute(sql);
            FuncInline.ClearSystemErrorPart(part.ToString());
            #endregion


            GlobalVar.SystemErrored = GlobalVar.SystemErrorListQueue.Count > 0;
            if (!GlobalVar.SystemErrored)
            {
                GlobalVar.SystemMsg = "";
            }
            DIO.WriteDOData(FuncInline.enumDONames.Y412_2_Tower_Lamp_Buzzer, false);

            FuncLog.WriteLog("Alarm Clear - " + part.ToString());

        }

        public static void RemoveError(FuncInline.enumErrorPart part, FuncInline.enumErrorCode er) // RemoveError(에러명) 발생된 에러 삭제
        {
            for (int i = GlobalVar.SystemErrorQueue.Count - 1; i >= 0; i--)
            {
                if (((FuncInline.structError)GlobalVar.SystemErrorQueue.ElementAt(i)).ErrorPart == part &&
                    ((FuncInline.structError)GlobalVar.SystemErrorQueue.ElementAt(i)).ErrorCode == er)
                {
                    RemoveErrorQueue(i);
                }
            }
            for (int i = GlobalVar.SystemErrorListQueue.Count - 1; i >= 0; i--)
            {
                if (((FuncInline.structError)GlobalVar.SystemErrorQueue.ElementAt(i)).ErrorPart == part &&
                    ((FuncInline.structError)GlobalVar.SystemErrorQueue.ElementAt(i)).ErrorCode == er)
                {
                    RemoveErrorListQueue(i);
                }
            }

            #region DataBase에 추가
            //string sql = "update [SystemError] " +
            //                "set " +
            //                "[Clear] = '1' " +
            //                "where [ErrorPart] = " + part.ToString() + " " +
            //                "and [ErrorCode] = " + ((int)er).ToString() + " " +
            //                "and [Clear] = '0'";
            //GlobalVar.Sql.Execute(sql);
            FuncInline.ClearSystemError(part.ToString(), (int)er);
            #endregion

            GlobalVar.SystemErrored = GlobalVar.SystemErrorListQueue.Count > 0;
            if (!GlobalVar.SystemErrored)
            {
                GlobalVar.SystemMsg = "";
            }
            DIO.WriteDOData(FuncInline.enumDONames.Y412_2_Tower_Lamp_Buzzer, false);

            FuncLog.WriteLog("Alarm Clear - " + part.ToString() + "," + er.ToString());
        }


        public static bool CheckError(FuncInline.enumErrorPart part, FuncInline.enumErrorCode code) // CheckError(에러명) 에러 발생 여부 확인
        {
            try
            {
                for (int i = 0; i < GlobalVar.SystemErrorListQueue.Count; i++)
                {
                    if (((FuncInline.structError)GlobalVar.SystemErrorQueue.ElementAt(i)).ErrorCode == code &&
                        ((FuncInline.structError)GlobalVar.SystemErrorQueue.ElementAt(i)).ErrorPart == part)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
            return false;
        }


        public static bool CheckError(FuncInline.structError er) // CheckError(에러명) 에러 발생 여부 확인
        {
            try
            {
                for (int i = 0; i < GlobalVar.SystemErrorListQueue.Count; i++)
                {
                    if (((FuncInline.structError)GlobalVar.SystemErrorQueue.ElementAt(i)).ErrorCode == er.ErrorCode &&
                        ((FuncInline.structError)GlobalVar.SystemErrorQueue.ElementAt(i)).ErrorPart == er.ErrorPart)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
            return false;
        }


        public static bool CheckError(FuncInline.enumErrorPart part) // CheckError(파트) 에러 발생 여부 확인
        {
            try
            {
                /*
                SqlDataReader errors = FuncInline.GetUnClearedSystemError();
                bool exist = false;
                while (errors.Read())
                {
                    if (errors[2].ToString() == part.ToString())
                    {
                        exist = true;
                    }
                }
                errors.Close();
                return exist;
                //*/

                for (int i = 0; i < GlobalVar.SystemErrorListQueue.Count; i++)
                {
                    if (((FuncInline.structError)GlobalVar.SystemErrorListQueue.ElementAt(i)).ErrorPart == part)
                    {
                        return true;
                    }
                }

                /*
                for (int i = 0; i < GlobalVar.SystemErrorQueue.Count; i++)
                {
                    if (GlobalVar.SystemErrorQueue.ElementAt(i).ErrorPart == part)
                    {
                        return true;
                    }
                }
                //*/
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
            return false;
        }

        public static bool CheckError(FuncInline.enumErrorCode er) // CheckError(에러명) 에러 발생 여부 확인
        {
            try
            {
                for (int i = 0; i < GlobalVar.SystemErrorListQueue.Count; i++)
                {
                    if (((FuncInline.structError)GlobalVar.SystemErrorQueue.ElementAt(i)).ErrorCode == er)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
            return false;
        }





    }
}
