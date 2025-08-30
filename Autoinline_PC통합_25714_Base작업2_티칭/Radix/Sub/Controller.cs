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
    public static class Controller
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

        //MXP 초기화 변수
        public static UInt16 ProcState = 0; // MXP 초기화 제어용
        public static UInt32 Status = 0; // MXP 초기화 제어용
        public static ulong startTime = 0; // MXP 초기화 Timeout 체크용
        public static bool initFail = false; // 장치 초기화 체크용

        /** @brief 이더켓 마스터의 초기화 구동중 */
        public static bool MasterChecking = false; // 이더켓 마스터의 초기화 구동중
        /** @brief 이더켓 마스터의 초기화 완료됨 */
        public static bool MasterChecked = false; // 이더켓 마스터의 초기화 완료됨        
        /**
         * @brief class 내부콜용 로컬 debug 
         * @param 디버그 처리할 문자열
         * @return void
         */
        private static void debug(string str) // debug(문자열) class 내부콜용 로컬 debug 
        {
            //Util.Debug(str);
        }

        #region 제어기 초기화
        /**
         * @brief 제어기 마스터 초기화 진행
         *      main.cs 에서 shown 시 한 번 실행해서 체크한다.
         */
        public static bool MasterInit()
        {
            Controller.MasterChecked = false;
            Controller.MasterChecking = true;

            if (GlobalVar.Simulation)
            {
                Controller.Status = 0;
                Controller.MasterChecked = true;
                Controller.MasterChecking = false;
                return true;
            }
            else
            {
                /*
                if (GlobalVar.MasterType == enumMasterType.MXP)
                {
                    Controller.ProcState = (UInt16)MXP.MXP_KernelState.Init;
                }
                else if (GlobalVar.MasterType == enumMasterType.MXN)
                {
                    //MXN은 필요 없을 것 같다.
                }
                //*/
                //else
                 if (GlobalVar.MasterType == enumMasterType.AXL)
                {
                    //아진은 필요 없을 것 같다.
                }
                else if (GlobalVar.MasterType == enumMasterType.ADVANTECH)
                {
                    if (!FuncAdvantech.InitializeComponent())
                    {
                        FuncLog.WriteLog("Get Device Numbers Failed!");
                        //this.BringToFront();
                        MessageBox.Show("Get Device Numbers Failed!");
                        //initFail = true;
                        GlobalVar.GlobalStop = true;
                        //this.Close();
                        return false;
                    }

                    FuncAdvantech.OpenBoard();
                    FuncAdvantechDIO.InitializeComponent();
                }
                Controller.MasterChecked = false;
                Controller.MasterChecking = true;
            }

            startTime = GlobalVar.TickCount64;
            Application.DoEvents();

            // 체크 완료까지 대기
            /*
            if (GlobalVar.MasterType == enumMasterType.MXP)
            {
                while (//!GlobalVar.GlobalStop &&
                    Controller.MasterChecking &&
                    Controller.ProcState != (UInt16)MXP.MXP_KernelState.Runed &&
                    GlobalVar.TickCount64 - startTime < 60 * 1000)
                {
                    Thread.Sleep(1000);
                    Controller.CheckMXP();
                }
            }
            else if (GlobalVar.MasterType == enumMasterType.MXN)
            {
                Controller.MasterChecked = Controller.CheckMXN();
                Controller.MasterChecking = false;
            }
            //*/
            //else
         if (GlobalVar.MasterType == enumMasterType.AXL)
            {
                Controller.MasterChecked = Controller.CheckAXL();
                Controller.MasterChecking = false;
            }
            return Controller.MasterChecked;
        }

        #region MXP
        /**
         * @brief MXP 초기화 함수
         *      MasterInit() 에서 완료될 때까지 반복 호출한다.
         */
        public static void CheckMXP() // MXP 초기화 함수
        {
            debug("Environment.TickCount - startTime : " + (GlobalVar.TickCount64 - startTime));
            if (GlobalVar.TickCount64 - startTime > 60 * 1000)
            {
                Controller.MasterChecked = false;
                Controller.MasterChecking = false;
            }

            /*
            debug("ProcState : " + ((MXP.MXP_KernelState)ProcState).ToString());
            switch (ProcState)
            {

                case (UInt16)MXP.MXP_KernelState.Idle:
                    {
                        break;
                    }
                case (UInt16)MXP.MXP_KernelState.Init:
                    {
                        UInt32 status = 0;
                        Int32 InitError;

                        InitError = MXP.MXP_InitKernel_Developer(ref status);
                        Thread.Sleep(1000);

                        InitError = MXP.MXP_InitKernel_Developer(ref status);

                        if (InitError == MXP.MXP_ret.RET_NO_ERROR)
                        {
                            ProcState = (UInt16)MXP.MXP_KernelState.Initing;
                            //FuncWin.TopMessageBox("Succeed to initialize MXP.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            ProcState = (UInt16)MXP.MXP_KernelState.Idle;
                            //FuncWin.TopMessageBox("Fail to initialize MXP!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }
                        break;
                    }
                case (UInt16)MXP.MXP_KernelState.Initing:
                    {

                        UInt32 usNumOfSlave = 0;
                        if (MXP.MXP_GetSlaveCount(0, out usNumOfSlave) == MXP.MXP_ret.RET_NO_ERROR)
                        {
                            ProcState = (UInt16)MXP.MXP_KernelState.Inited;
                        }
                        break;
                    }
                case (UInt16)MXP.MXP_KernelState.Inited:
                    {
                        ProcState = (UInt16)MXP.MXP_KernelState.Run;
                        break;
                    }
                case (UInt16)MXP.MXP_KernelState.Run:
                    {
                        if (MXP.MXP_SystemRun() == MXP.MXP_ret.RET_NO_ERROR)
                        {
                            ProcState = (UInt16)MXP.MXP_KernelState.Running;
                            Status++;
                            if (Status > 3)
                            {
                                //FuncWin.TopMessageBox("Succeed to run MXP.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        else
                        {
                            //FuncWin.TopMessageBox("Fail to run MXP!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }
                        break;
                    }
                case (UInt16)MXP.MXP_KernelState.Running:
                    {
                        UInt32 usNumOfSlave = 0;
                        if (MXP.MXP_GetSlaveCount(0, out usNumOfSlave) == MXP.MXP_ret.RET_NO_ERROR)
                        {
                            ProcState = (UInt16)MXP.MXP_KernelState.Inited;
                        }

                        UInt32 status = 0;
                        MXP.MXP_GetOnlineMode(ref status);

                        //Ecat.ServoOnAll(true);
                        bool servoChecked = true;

                        for (ushort axis = 0; axis < GlobalVar.Axis_count; axis++)
                        {
                            MXP.MXP_READAXISINFO_IN inInfo = new MXP.MXP_READAXISINFO_IN { };
                            MXP.MXP_READAXISINFO_OUT outInfo = new MXP.MXP_READAXISINFO_OUT { };

                            MXP.MXP_READSTATUS_IN statIn = new MXP.MXP_READSTATUS_IN { };
                            MXP.MXP_READSTATUS_OUT statOut = new MXP.MXP_READSTATUS_OUT { };

                            ushort AxisNo = axis;

                            inInfo.Axis.AxisNo = axis;
                            inInfo.Enable = 1;

                            statIn.Axis.AxisNo = axis;
                            statIn.Enable = 1;

                            if (MXP.MXP_ReadAxisInfo(ref inInfo, out outInfo) != MXP.MXP_ret.RET_NO_ERROR ||
                                MXP.MXP_ReadStatus(ref statIn, out statOut) != MXP.MXP_ret.RET_NO_ERROR)
                            {
                                servoChecked = false;
                            }
                            if (!FuncMotion.ServoOn(axis, true))
                            {
                                servoChecked = false;
                            }
                        }
                        if (servoChecked)
                        {
                            ProcState = (UInt16)MXP.MXP_KernelState.Runed;
                            Controller.MasterChecked = true;
                            Controller.MasterChecking = false;
                        }
                        else
                        {
                            // 초기화 실패시 다시 초기화
                            ProcState = (UInt16)MXP.MXP_KernelState.Init;
                            Controller.MasterChecked = false;
                            Controller.MasterChecking = true;
                        }
                        break;
                    }
                case (UInt16)MXP.MXP_KernelState.Runed:
                    {
                        //러닝중
                        Status = 0;
                        Controller.MasterChecked = true;
                        Controller.MasterChecking = false;
                        break;
                    }
                case (UInt16)MXP.MXP_KernelState.Reset:
                    {
                        if (MXP.MXP_SystemReset() == MXP.MXP_ret.RET_NO_ERROR)
                        {
                            ProcState = (UInt16)MXP.MXP_KernelState.Running;
                            //FuncWin.TopMessageBox("Succeed to reset MXP.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            //FuncWin.TopMessageBox("Fail to reset MXP!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }
                        break;
                    }
                case (UInt16)MXP.MXP_KernelState.Close:
                    {
                        Int32 Status = 0;
                        MXP.MXP_GetKernelStatus(out Status);
                        if (Status >= MXP.MXP_SysStatus.Initialized)
                        {
                            if (MXP.MXP_SystemStop() == MXP.MXP_ret.RET_NO_ERROR)
                            {
                                ProcState = (UInt16)MXP.MXP_KernelState.Destory;
                            }
                            else if (Status == 0)
                            {
                                ProcState = (UInt16)MXP.MXP_KernelState.Idle;
                                //FuncWin.TopMessageBox("Already destroy MXP", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            }
                            else
                            {
                                //FuncWin.TopMessageBox("Fail to stop MXP!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            }
                        }
                        break;
                    }
                case (UInt16)MXP.MXP_KernelState.Destory:
                    {
                        if (MXP.MXP_Destroy() == MXP.MXP_ret.RET_NO_ERROR)
                        {
                            ProcState = (UInt16)MXP.MXP_KernelState.Idle;
                            //FuncWin.TopMessageBox("Succeed to close MXP.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            //FuncWin.TopMessageBox("Fail to close MXP!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }
                        break;
                    }

            }
            //*/
        }
        #endregion

        #region MXN
        /**
         * @brief MXN 초기화 함수
         *      MasterInit() 에서 완료될 때까지 반복 호출한다.
         */
        public static bool CheckMXN() // MXN 초기화 함수
        {
            if (GlobalVar.Simulation)
            {
                return true;
            }

            /*
            UInt16 usStatus;
            Int32 iRet;
            usStatus = 0;
            iRet = MXN.MXN_InitKernel(ref usStatus);
            if (iRet == MXN.KernelReturn.RET_NO_ERROR && usStatus >= MXN.KernelStatus.SYSTEM_INITED)
                //FuncWin.TopMessageBox("Success to load MXN API.", "SampleVC#", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            //*/
                //FuncWin.TopMessageBox("Fail to load MXN API.", "SampleVC#", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
        }
        #endregion

        #region AXL  

        /**
         * @brief AXL 초기화 함수
         *      MasterInit() 에서 완료될 때까지 반복 호출한다.
         */
        public static bool CheckAXL() // AXL 초기화 함수
        {
            if (GlobalVar.Simulation)
            {
                return true;
            }
            //++
            // Initialize library 
            uint uRetCode = (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS;
            uRetCode = CAXL.AxlOpen(7);
            if (uRetCode == (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
            {
                uint uStatus = 0;

                if (CAXD.AxdInfoIsDIOModule(ref uStatus) == (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                {
                    if ((AXT_EXISTENCE)uStatus == AXT_EXISTENCE.STATUS_EXIST)
                    {
                        #region DIO 초기화
                        int nModuleCount = 0;

                        if (CAXD.AxdInfoGetModuleCount(ref nModuleCount) == (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                        {
                            short i = 0;
                            int nBoardNo = 0;
                            int nModulePos = 0;
                            uint uModuleID = 0;
                            string strData = "";

                            for (i = 0; i < nModuleCount; i++)
                            {
                                if (CAXD.AxdInfoGetModule(i, ref nBoardNo, ref nModulePos, ref uModuleID) == (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                                {
                                    switch ((AXT_MODULE)uModuleID)
                                    {
                                        case AXT_MODULE.AXT_SIO_DI32: strData = String.Format("[{0:D2}:{1:D2}] SIO-DI32", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_DO32P: strData = String.Format("[{0:D2}:{1:D2}] SIO-DO32P", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_DB32P: strData = String.Format("[{0:D2}:{1:D2}] SIO-DB32P", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_DO32T: strData = String.Format("[{0:D2}:{1:D2}] SIO-DO32T", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_DB32T: strData = String.Format("[{0:D2}:{1:D2}] SIO-DB32T", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDI32: strData = String.Format("[{0:D2}:{1:D2}] SIO_RDI32", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDO32: strData = String.Format("[{0:D2}:{1:D2}] SIO_RDO32", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDB128MLII: strData = String.Format("[{0:D2}:{1:D2}] SIO-RDB128MLII", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RSIMPLEIOMLII: strData = String.Format("[{0:D2}:{1:D2}] SIO-RSIMPLEIOMLII", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDO16AMLII: strData = String.Format("[{0:D2}:{1:D2}] SIO-RDO16AMLII", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDO16BMLII: strData = String.Format("[{0:D2}:{1:D2}] SIO-RDO16BMLII", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDB96MLII: strData = String.Format("[{0:D2}:{1:D2}] SIO-RDB96MLII", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDO32RTEX: strData = String.Format("[{0:D2}:{1:D2}] SIO-RDO32RTEX", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDI32RTEX: strData = String.Format("[{0:D2}:{1:D2}] SIO-RDI32RTEX", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDB32RTEX: strData = String.Format("[{0:D2}:{1:D2}] SIO-RDB32RTEX", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_DI32_P: strData = String.Format("[{0:D2}:{1:D2}] SIO-DI32_P", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_DO32T_P: strData = String.Format("[{0:D2}:{1:D2}] SIO-DO32T_P", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDB32T: strData = String.Format("[{0:D2}:{1:D2}] SIO-RDB32T", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_ECAT_DIO:
                                            uint uModuleSubID = 0;
                                            var szModuleName = new System.Text.StringBuilder(50);
                                            var szModuleDesc = new System.Text.StringBuilder(80);
                                            //CAXD.AxdInfoGetModuleEx(i, ref uModuleSubID, szModuleName, szModuleDesc);
                                            //strData = String.Format("[{0:D2}:{1:D2}] {2}", nBoardNo, i, szModuleName);
                                            break;
                                        default:
                                            strData = String.Format("[{0:D2}:{1:D2}] Unknown", nBoardNo, i);
                                            break;
                                    }
                                    //comboModule.Items.Add(strData);
                                }
                            }
                            //comboModule.SelectedIndex = 0;
                        }
                        #endregion

                        #region  Motion 초기화
                        int m_lAxisCounts = 0;                // 제어 가능한 축갯수 선언 및 초기화
                        int m_lAxisNo = 0;                    // 제어할 축 번호 선언 및 초기화   
                        uint m_uModuleID = 0;                // 제어할 축의 모듈 I/O 선언 및 초기화
                        int m_lBoardNo = 0, m_lModulePos = 0;

                        String strAxis = "";

                        //++ 유효한 전체 모션축수를 반환합니다.
                        uint a = CAXM.AxmInfoGetAxisCount(ref m_lAxisCounts);
                        m_lAxisNo = 0;
                        //++ 지정한 축의 정보를 반환합니다.
                        // [INFO] 여러개의 정보를 읽는 함수 사용시 불필요한 정보는 NULL(0)을 입력하면 됩니다.
                        CAXM.AxmInfoGetAxis(m_lAxisNo, ref m_lBoardNo, ref m_lModulePos, ref m_uModuleID);
                        for (int i = 0; i < m_lAxisCounts; i++)
                        {
                            switch (m_uModuleID)
                            {
                                //++ 지정한 축의 정보를 반환합니다.
                                // [INFO] 여러개의 정보를 읽는 함수 사용시 불필요한 정보는 NULL(0)을 입력하면 됩니다.
                                case (uint)AXT_MODULE.AXT_SMC_4V04: strAxis = String.Format("{0:0}-(AXT_SMC_4V04)", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_R1V04: strAxis = String.Format("{0:0}-[AXT_SMC_R1V04]", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_2V04: strAxis = String.Format("{0:0}-[AXT_SMC_2V04]", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_R1V04MLIIPM: strAxis = String.Format("{0:0}-[AXT_SMC_R1V04MLIIPM]", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_R1V04PM2Q: strAxis = String.Format("{0:0}-[AXT_SMC_R1V04PM2Q]", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_R1V04PM2QE: strAxis = String.Format("{0:0}-[AXT_SMC_R1V04PM2QE]", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_R1V04MLIIIPM: strAxis = String.Format("{0:0}-(AXT_SMC_R1V04MLIIIPM)", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_R1V04MLIISV: strAxis = String.Format("{0:0}-[AXT_SMC_R1V04MLIISV]", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_R1V04A5: strAxis = String.Format("{0:0}-[AXT_SMC_R1V04A4]", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_R1V04A4: strAxis = String.Format("{0:0}-[AXT_SMC_R1V04MLIICL]", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_R1V04SIIIHMIV: strAxis = String.Format("{0:0}-[AXT_SMC_R1V04SIIIHMIV]", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_R1V04SIIIHMIV_R: strAxis = String.Format("{0:0}-[AXT_SMC_R1V04SIIIHMIV_R]", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_R1V04MLIIISV: strAxis = String.Format("{0:0}-[AXT_SMC_R1V04MLIIISV]", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_R1V04MLIIISV_MD: strAxis = String.Format("{0:0}-[AXT_SMC_R1V04MLIIISV_MD]", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_R1V04MLIIIS7S: strAxis = String.Format("{0:0}-[AXT_SMC_R1V04MLIIIS7S]", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_R1V04MLIIIS7W: strAxis = String.Format("{0:0}-[AXT_SMC_R1V04MLIIIS7W]", i); break;
                                case (uint)AXT_MODULE.AXT_ECAT_MOTION:
                                    uint uModuleSubID = 0;
                                    var szModuleName = new System.Text.StringBuilder(50);
                                    var szModuleDesc = new System.Text.StringBuilder(80);
                                    //CAXM.AxmInfoGetAxisEx(i, ref uModuleSubID, szModuleName, szModuleDesc);
                                    //strAxis = String.Format("{0:0}-[ECAT-{1}]", i, szModuleName);
                                    break;
                                default: strAxis = String.Format("{0:00}-[Unknown]", i); break;
                            }
                            //cboSelAxis.Items.Add(strAxis);
                        }
                        #endregion
                    }
                    else
                    {
                        FuncWin.TopMessageBox("Module not exist.");
                        return false;
                    }
                }
            }
            else
            {
                FuncWin.TopMessageBox("Open Error!");
                return false;
            }
            return true;
        }

        #endregion

        #endregion


        /**
         * @brief Ethercat Master 초기화 
         *      기본 연결과 체크만 실행되며, 구동은 MasterInit() 함수를 통해야 한다.
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
                /*
                case enumMasterType.MXP:
                default:
                    ret = Motion_Function.Init();
                    if (ret)
                    {
                        FuncMotion.ServoOnAll(true);
                    }
                    break;
                //*/
            }
            return ret;
        }


    }
}
