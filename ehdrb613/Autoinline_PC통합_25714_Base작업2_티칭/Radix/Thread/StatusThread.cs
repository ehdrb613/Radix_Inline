using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace Radix
{
    /**
     * Input/Output 데이터 처리 및 서보 모션 상태 데이터를 모아서 처리
     *      1. Digital Input
     *      2. Digital Output
     *      3. Motion Status
     */
    class StatusThread
    {
        /*
         * StatusThread.cs : Input/Output 데이터 처리 및 서보 모션 상태 데이터를 모아서 처리/
         *      1. Digital Input
         *      2. Digital Output
         *      3. Motion Status
         */
        
            //아진 모듈 설정
        public static int MAX_DI = (32 * GlobalVar.Inputmodule);       // Global 설정값 맞출것
        public static int MAX_DO = (32 * GlobalVar.Outputmodule);
        public static bool[] DI = new bool[MAX_DI];    // RTEX 읽은 현재 DI 값
        public static bool[] DO = new bool[MAX_DO];    // RTEX 읽은 현재 DO 값

        /** 
         * @brief 클래스 내부에서 Call 하는 로컬 Debug
         * @param str 디버그 처리할 문자열
         * @return void
         */
        private void debug(string str) // 클래스 내부에서 Call 하는 로컬 Debug
        {
            Util.Debug(str);
        }

        /** 
         * @brief 쓰레드 메인 함수
         *      Servo Motor Status
         *      Digital Input Read,
         *      Digital Output Read/Output
         *      Master Type에 따라 분기됨
         * @return void
         */
        public void Run()
        {
            int nStatusSleep = GlobalVar.ThreadSleep;

            ulong chkTime = GlobalVar.TickCount64; // 상태 쓰레드 수행 시간 체크용
            while (GlobalVar.GlobalStop == false) // 프로그램 종료 전까지
            {
                try
                {
                    switch (GlobalVar.MasterType)
                    {

                        #region AXL
                        case enumMasterType.AXL:

                            #region Servo Motion Status

                            for (ushort axis = 0; axis < GlobalVar.Axis_count; axis++) // 전체 서보 모터 갯수대로
                            {

                                #region 서보 온 확인
                                uint duOnOff = 0;//서버 온오프 상태 확인
                                                 //++ 지정한 축의 서보온 상태를 반환합니다.      
                                CAXM.AxmSignalIsServoOn(axis, ref duOnOff);
                                GlobalVar.AxisStatus[axis].PowerOn = duOnOff == 1;// 서보 온 여부
                                #endregion

                                #region 홈센서 및 홈 검색 관련 확인
                                uint duState = 0;
                                //++ 지정한 축의 원점신호의 상태를 확인합니다.                        
                                CAXM.AxmHomeReadSignal(axis, ref duState);
                                GlobalVar.AxisStatus[axis].HomeAbsSwitch = duState == 1; // 홈 센서

                                //홈 검색 관련 확인
                                CAXM.AxmHomeGetResult(axis, ref duState);
                                switch (duState)
                                {
                                    case (uint)AXT_MOTION_HOME_RESULT.HOME_SUCCESS:
                                        //GlobalVar.AxisStatus[axis].isHomed = true; // 홈 찾기 완료 여부
                                        //GlobalVar.AxisStatus[axis].Homing = false; // 서보 홈찾기중
                                        if (GlobalVar.AxisStatus[axis].Homing && GlobalVar.AxisStatus[axis].HomedTime == 0)
                                        {
                                            GlobalVar.AxisStatus[axis].isHomed = false;
                                            GlobalVar.AxisStatus[axis].HomedTime = GlobalVar.TickCount64;
                                        }
                                        break;
                                    case (uint)AXT_MOTION_HOME_RESULT.HOME_SEARCHING:
                                        GlobalVar.AxisStatus[axis].isHomed = false; // 홈 찾기 완료 여부
                                        GlobalVar.AxisStatus[axis].Homing = true; // 서보 홈찾기중
                                        GlobalVar.AxisStatus[axis].HomedTime = 0;
                                        break;
                                    default:
                                        GlobalVar.AxisStatus[axis].isHomed = false; // 홈 찾기 완료 여부
                                        GlobalVar.AxisStatus[axis].Homing = false; // 서보 홈찾기중
                                        GlobalVar.AxisStatus[axis].HomedTime = 0;
                                        break;
                                }
                                #endregion

                                // 홈완료 1초 지나서 홈 펄스값 초기화 설정
                                if (!GlobalVar.AxisStatus[axis].isHomed && GlobalVar.AxisStatus[axis].HomedTime != 0)
                                {
                                    if (GlobalVar.AxisStatus[axis].HomedTime + 1000 < GlobalVar.TickCount64)
                                    {
                                        // 현재 위치를 원점으로 설정한다. 
                                        CAXM.AxmStatusSetActPos((int)axis, 0.0);
                                        CAXM.AxmStatusSetCmdPos((int)axis, 0.0);

                                        GlobalVar.AxisStatus[axis].isHomed = true; // 홈 찾기 완료 여부
                                        GlobalVar.AxisStatus[axis].Homing = false; // 서보 홈찾기중
                                        GlobalVar.AxisStatus[axis].HomedTime = 0;

                                        // 현재 위치 정보 기록하는 컨베이어 등을 위해 위치 펄스값 세팅 
                                        FuncInlineMove.SetAbsConveyorPos(axis, 0.0);
                                    }
                                }



                                #region 리미트 센서 확인
                                uint upPositiveStatus = 0, upNegativeStatus = 0;
                                //++ (+/-)End Limit신호의 상태를 확인합니다.
                                CAXM.AxmSignalReadLimit(axis, ref upPositiveStatus, ref upNegativeStatus);
                                GlobalVar.AxisStatus[axis].LimitSwitchPos = upPositiveStatus == 1; // 포지티브 리미트
                                GlobalVar.AxisStatus[axis].LimitSwitchNeg = upNegativeStatus == 1; // 네거티브 리미트
                                #endregion

                                #region 서보 알람 확인
                                //++ Servo Alarm 신호의 상태를 확인합니다.                            
                                CAXM.AxmSignalReadServoAlarm(axis, ref duState);
                                GlobalVar.AxisStatus[axis].Errored = duState == 1;  // 에러 여부
                                                                                    //GlobalVar.AxisStatus[axis].ErrorID = outInfo.ErrorID;  // 에러 아이디        
                                #endregion

                                #region 서보 정지 상태 확인
                                CAXM.AxmStatusReadInMotion(axis, ref duState);
                                GlobalVar.AxisStatus[axis].StandStill = duState == 0; // 서보 정지상태
                                #endregion

                                #region 서보 위치 속도
                                double dActPosition = 0.0;
                                double dCmdVelocity = 0.0;
                                CAXM.AxmStatusGetActPos(axis, ref dActPosition);
                                GlobalVar.AxisStatus[axis].Position = FuncMotion.PulseToMM((long)dActPosition, GlobalVar.ServoGearRatio[(int)axis], GlobalVar.ServoRevMM[(int)axis], GlobalVar.ServoRevPulse[(int)axis]);//dActPosition; // 서보 포지션
                                CAXM.AxmStatusReadVel(axis, ref dCmdVelocity);
                                GlobalVar.AxisStatus[axis].Velocity = FuncMotion.PulseToMM((long)dCmdVelocity, GlobalVar.ServoGearRatio[(int)axis], GlobalVar.ServoRevMM[(int)axis], GlobalVar.ServoRevPulse[(int)axis]);//dCmdVelocity; // 서보 구동 속도
                                #endregion
                            }
                            #endregion

                            // DIO Update
                            UpdateStatus();

                            break;
                        #endregion


                        #region Advantech
                        case enumMasterType.ADVANTECH:

                            #region Servo Motion Status
                            for (ushort axis = 0; axis < GlobalVar.Axis_count; axis++) // 전체 서보 모터 갯수대로
                            {
                                #region Servo On
                                //bool Servo_on = FuncAdvantech.Servo_On((byte)axis);
                                //GlobalVar.AxisStatus[axis].PowerOn = Servo_on == true; // 서보 온 여부
                                #endregion

                                #region ServoState
                                int status = FuncAdvantech.ServoStateCheck(axis);

                                GlobalVar.AxisStatus[axis].Disabled = status == (int)Advantech.Motion.AxisState.STA_AX_DISABLE;  // 사용하지 않을 경우
                                GlobalVar.AxisStatus[axis].StandStill = status == (int)Advantech.Motion.AxisState.STA_AX_READY;  // 준비 상황
                                GlobalVar.AxisStatus[axis].ErrorStop = status == (int)Advantech.Motion.AxisState.STA_AX_ERROR_STOP;  // 에러 여부
                                GlobalVar.AxisStatus[axis].Homing = status == (int)Advantech.Motion.AxisState.STA_AX_HOMING; // 홈 모션 실행 중

                                #endregion

                                #region ServoIOState
                                uint iostatus = FuncAdvantech.ServoIOCheck(axis);
                                //내 방식
                                //if ((iostatus & (uint)Advantech.Motion.Ax_Motion_IO.AX_MOTION_IO_ORG) > 0)
                                //{ GlobalVar.AxisStatus[axis].HomeAbsSwitch = true;}
                                //else  { GlobalVar.AxisStatus[axis].HomeAbsSwitch = false ;}
                                //금수석님 방식

                                GlobalVar.AxisStatus[axis].PowerOn = (iostatus & (uint)Advantech.Motion.Ax_Motion_IO.AX_MOTION_IO_SVON) > 0;// 서보 온                            

                                GlobalVar.AxisStatus[axis].HomeAbsSwitch = (iostatus & (uint)Advantech.Motion.Ax_Motion_IO.AX_MOTION_IO_ORG) > 0;// 홈 센서                            
                                GlobalVar.AxisStatus[axis].LimitSwitchPos = (iostatus & (uint)Advantech.Motion.Ax_Motion_IO.AX_MOTION_IO_LMTP) > 0; // 포지티브 리미트
                                GlobalVar.AxisStatus[axis].LimitSwitchNeg = (iostatus & (uint)Advantech.Motion.Ax_Motion_IO.AX_MOTION_IO_LMTN) > 0; // 네거티브 리미트
                                GlobalVar.AxisStatus[axis].Errored = ((iostatus & (uint)Advantech.Motion.Ax_Motion_IO.AX_MOTION_IO_ALM) > 0); //알람 센서                            


                                //if (axis == 2 && GlobalVar.AxisStatus[axis].Errored)
                                //{
                                //    FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
                                //                          DateTime.Now.ToString("HH:mm:ss"),
                                //                          FuncInline.enumErrorPart.System,
                                //                          enumErrorCode.Axis_Disabled,
                                //                          false,
                                //                          "서보 축 diable 됨. " + axis.ToString()));
                                //    //FuncError.AddError(enumError.Axis_Disabled);
                                //}
                                #endregion

                                //GlobalVar.AxisStatus[axis].ErrorID = outInfo.ErrorID;  // 에러 아이디                                                        
                                //GlobalVar.AxisStatus[axis].isHomed = outInfo.IsHomed == 1; // 홈 찾기 완료 여부

                                //if (MXP.MXP_ReadAxisInfo(ref inInfo, out outInfo) == MXP.MXP_ret.RET_NO_ERROR)
                                //{
                                //    GlobalVar.AxisStatus[axis].Errored = outInfo.Error == 1;  // 에러 여부
                                //    GlobalVar.AxisStatus[axis].ErrorID = outInfo.ErrorID;  // 에러 아이디
                                //    GlobalVar.AxisStatus[axis].HomeAbsSwitch = outInfo.HomeAbsSwitch == 1; // 홈 센서
                                //    GlobalVar.AxisStatus[axis].LimitSwitchPos = outInfo.LimitSwitchPos == 1; // 포지티브 리미트
                                //    GlobalVar.AxisStatus[axis].LimitSwitchNeg = outInfo.LimitSwitchNeg == 1; // 네거티브 리미트
                                //    GlobalVar.AxisStatus[axis].PowerOn = outInfo.PowerOn == 1; // 서보 온 여부
                                //    GlobalVar.AxisStatus[axis].isHomed = outInfo.IsHomed == 1; // 홈 찾기 완료 여부

                                //}
                                //if (MXP.MXP_ReadStatus(ref statIn, out statOut) == MXP.MXP_ret.RET_NO_ERROR)
                                //{
                                //    GlobalVar.AxisStatus[axis].ErrorStop = statOut.ErrorStop == 1; // 에러 정지 여부
                                //    GlobalVar.AxisStatus[axis].Disabled = statOut.Disabled == 1; // 서보 불능 여부
                                //    GlobalVar.AxisStatus[axis].Stopping = statOut.Stopping == 1; // 서보 정지중
                                //    GlobalVar.AxisStatus[axis].Homing = statOut.Homing == 1; // 서보 홈찾기중
                                //    GlobalVar.AxisStatus[axis].StandStill = statOut.Standstill == 1; // 서보 정지상태
                                //}
                                //GlobalVar.AxisStatus[axis].Position = Motion_Function.MXP_MC_ReadActualPosition(axis); // 서보 포지션
                                //GlobalVar.AxisStatus[axis].Velocity = Motion_Function.MXP_MC_ReadActualVelocity(axis); // 서보 구동 속도
                            }
                            #endregion

                            #region DI Data Read
                            /*
                            // DI Read Status
                            // 각 모듈별 전체 byte                         
                            Byte[] Ad_diData = new Byte[(uint)((Enum.GetValues(typeof(enumDINames))).Length)];// Servo Inputs Register 4Byte 0x60FD                         

                            if (frmMain.instantDiCtrl1.Read(0, (int)(GlobalVar.AD_DiSize / 8), Ad_diData) == ErrorCode.Success)
                            {
                                int midx = ((Enum.GetValues(typeof(enumDINames))).Length);
                                for (int j = 0; j < midx; j++)
                                {
                                    try
                                    {
                                        bool[] arrData = Util.ByteToBoolArray(Ad_diData[j]);
                                        for (int k = 0; k < 8; k++)
                                        {
                                            GlobalVar.DI_Array[j * 8 + k] = arrData[k];
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.ToString());
                                        Console.WriteLine(ex.StackTrace);
                                    }
                                }
                            }
                            //*/
                            #endregion

                            #region DO Data Read
                            /*
                            // DO Read Status
                            // 각 모듈별 전체 byte                         
                            Byte[] Ad_doReadData = new Byte[(uint)((Enum.GetValues(typeof(enumDONames))).Length)];

                            if (frmMain.instantDoCtrl1.Read(0, (int)(GlobalVar.DoSize / 8), Ad_doReadData) == ErrorCode.Success)
                            {
                                for (int j = 0; j < Ad_doReadData.Length; j++)
                                {
                                    bool[] arrData = Util.ByteToBoolArray(Ad_doReadData[j]);
                                    for (int k = 0; k < 8; k++)
                                    {
                                        GlobalVar.DO_Read[j * 8 + k] = arrData[k];
                                    }
                                }
                            }
                            //*/
                            #endregion

                            #region DO Data Write
                            /*
                            // DO Read Status
                            // 각 모듈별 전체 byte                         
                            //Byte[] Ad_doWriteData = new Byte[GlobalVar.DIO_Size[1]];

                            byte[] Ad_DoWriteData = new byte[GlobalVar.DIO_Size[1]];
                            for (int i = 0; i < GlobalVar.DIO_Size[1]; i++)
                            {
                                try
                                {
                                    // 8bit씩 잘라서
                                    bool[] bits = new bool[8];
                                    for (int j = 0; j < bits.Length; j++)
                                    {
                                        bits[j] = GlobalVar.DO_Array[i * 8 + j];
                                    }
                                    // 각각 byte로 환산
                                    byte data = Util.BitArrayToByteArray(bits)[0];
                                    Ad_DoWriteData[i] = data;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.ToString());
                                    Console.WriteLine(ex.StackTrace);
                                }
                            }

                            frmMain.instantDoCtrl1.Write(0, (int)(GlobalVar.DoSize / 8), Ad_DoWriteData);

                            //*/
                            #endregion

                            break;
                        #endregion


                        default:
                            break;
                    }

                }
                catch (Exception ex)
                {
                    debug(ex.ToString());
                    debug(ex.StackTrace);
                }
                Thread.Sleep(nStatusSleep);
                //Util.Debug("status check time : " + (GlobalVar.TickCount64 - chkTime).ToString());
                chkTime = GlobalVar.TickCount64;
                //Application.DoEvents();                
            }

            FuncMotion.ServoOnAll(false);
            #region 종료시 모든 DO 초기화
            switch (GlobalVar.MasterType)
            {
                case enumMasterType.AXL:
                    for (int i = 0 + GlobalVar.OutputStartNodeID; i < GlobalVar.OutputStartNodeID + GlobalVar.Outputmodule; i++)
                    {
                        CAXD.AxdoWriteOutportDword(i, 0, 0x0000);
                    }
                    CAXL.AxlClose();
                    break;
                case enumMasterType.ADVANTECH:
                    //DIO.WriteDOData(FuncInline.enumDONames.Y0_04_AC_On, false);
                    break;

                default:
                    break;
            }

            #endregion

            Environment.Exit(Environment.ExitCode);//프로그램 남는거 때문에
        }

        // JHRYU: IO Update 함수

        static bool IsBusy = false;
        public static long UpdateStatus()
        {
            if (IsBusy) return 0;

            try
            {
                IsBusy = true;

                switch (GlobalVar.MasterType)
                {

                    /*
                    case enumMasterType.MXP:
                        break;
                        //*/

                    case enumMasterType.AXL:

                        // WRITE DO
                        DOutWrite(GlobalVar.DO_Array);

                        // READ DI, DO
                        #region DI Data Read

                        uint wInput = 0;
                        for (int i = 0 + GlobalVar.InputStartNodeID; i < GlobalVar.InputStartNodeID + GlobalVar.Inputmodule; i++)
                        {
                            uint result = CAXD.AxdiReadInportDword(i, 0, ref wInput);
                            if (result != 0)
                            {
                                //_SCAN_RESULT result1 = new _SCAN_RESULT();
                                //uint aa = CAXL.AxlScanStartSIIIH(ref result1);
                                if (GlobalVar.G_ErrNo != (int)FuncInline.enumErrorCode.Fatal_System_Error)
                                {
                                    Func.StatusPrint("AxdiReadInportDword Failed!");
                                    FuncInline.AddError(FuncInline.enumErrorCode.Fatal_System_Error, "IO 통신 불가로 시스템이 정지되었습니다!.\n시스템 전원을 확인후 프로그램을 재시작 하시기 바랍니다.");
                                }
                            }

                            try
                            {
                                bool[] arrData = Util.WordToBoolArray(wInput);
                                for (int k = 0; k < 32; k++)
                                {
                                    DI[(i - GlobalVar.InputStartNodeID) * 32 + k] = arrData[k];
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                                Console.WriteLine(ex.StackTrace);
                            }
                        }
                        #endregion

                        #region DO Data Read
                        uint wOutput = 0;
                        for (int i = 0 + GlobalVar.OutputStartNodeID; i < GlobalVar.OutputStartNodeID + GlobalVar.Outputmodule; i++)
                        {
                            uint result = CAXD.AxdoReadOutportDword(i, 0, ref wOutput);
                            //Console.WriteLine(i + " " + wOutput);
                            try
                            {
                                bool[] arrData = Util.WordToBoolArray(wOutput);
                                for (int k = 0; k < 32; k++)
                                {
                                    DO[(i - GlobalVar.OutputStartNodeID) * 32 + k] = arrData[k];
                                    //Console.WriteLine(k + " "+ arrData[k]);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                                Console.WriteLine(ex.StackTrace);
                            }
                        }
                        #endregion

                        // UPDATE GLOBAL
                        for (int i = 0; i < GlobalVar.DI_Array.Length; i++)
                        {
                            GlobalVar.DI_Array[i] = DI[i];
                        }
                        for (int i = 0; i < GlobalVar.DO_Read.Length; i++)
                        {
                            GlobalVar.DO_Read[i] = DO[i];
                        }

                        if (!GlobalVar.E_Stop &&
                       DIO.EMG_Check())
                        {
                            GlobalVar.E_Stop = true;



                            FuncError.AddError(new FuncInline.structError(DateTime.Now.ToString("yyyyMMdd"),
                                                        DateTime.Now.ToString("HH:mm:ss"),
                                                        FuncInline.enumErrorPart.System,
                                                        FuncInline.enumErrorCode.E_Stop,
                                                        false,
                                                        "Emergency Stop Button Pressed. Release Button and Initialize system."));
                            //#region Normal Error
                            //if (GlobalVar.UseNormalError)
                            //{
                            //    FuncError.AddError(enumError.E_Stop);
                            //}
                            //#endregion
                            //#region Part Error
                            //if (GlobalVar.PartError)
                            //{
                            //    FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
                            //                                DateTime.Now.ToString("HH:mm:ss"),
                            //                                FuncInline.enumErrorPart.System,
                            //                                enumErrorCode.E_Stop,
                            //                                false,
                            //                                ""));
                            //}
                            //#endregion                    
                        }
                        break;


                    default:
                        break;
                }

            }
            catch (Exception)
            {

            }
            finally
            {
                IsBusy = false;
            }

            return 0;
        }


        // array : gloabl bool array
        public static long DOutWrite(bool[] array)
        {
            try
            {

                // 각각 word로 환산
                uint[] data = Util.BitArrayToWordArray(array);

                for (int i = 0; i < data.Length; i++)
                {
                    //uint result = CAXD.AxdoWriteOutportDword(i, 0, data[i]);
                    uint result = CAXD.AxdoWriteOutportDword(GlobalVar.OutputStartNodeID + i, 0, data[i]);
                    if (result != 0)
                    {
                        //_SCAN_RESULT result1 = new _SCAN_RESULT();
                        //uint aa = CAXL.AxlScanStartSIIIH(ref result1);

                        if (GlobalVar.G_ErrNo != (int)enumErrorCode.Fatal_System_Error)
                        {

                            Func.StatusPrint("AxdoWriteOutportDword Failed!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Func.StatusPrint("Exception DOutWrite! - " + ex.ToString());
            }

            return 0;                   // 정상
        }
    }
}