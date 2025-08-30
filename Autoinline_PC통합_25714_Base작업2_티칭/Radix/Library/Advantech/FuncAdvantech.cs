using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

//HJ 2007 Advantech 추가 내용
using Advantech.Motion;//Common Motion API
using System.Windows.Forms;
using System.Diagnostics;

//using System.Runtime.InteropServices; //For Marshal
//using System.Diagnostics;





namespace Radix
{
    ////글로벌 변수 확인
    //bool m_bInit = false;
    //bool m_bServoOn = false;

    //public static bool Simulation = false;


    public static class FuncAdvantech
    {
        //글로벌 변수 확인
        public static bool m_bInit = false;
        public static bool m_bServoOn = false;
        public static uint m_ulAxisCount = 0;
        public static IntPtr[] m_Axishand = new IntPtr[32];
        public static Boolean VersionIsOk = false;
        public static uint DeviceNum = 0;
        public static IntPtr m_DeviceHandle = IntPtr.Zero;


        public static DEV_LIST[] CurAvailableDevs = new DEV_LIST[Motion.MAX_DEVICES];
        public static uint deviceCount = 0;



        #region  initialization

        public static Boolean GetDevCfgDllDrvVer()
        {
            string fileName = "";
            FileVersionInfo myFileVersionInfo;
            string FileVersion = "";
            fileName = Environment.SystemDirectory + "\\ADVMOT.dll";//SystemDirectory指System32 
            myFileVersionInfo = FileVersionInfo.GetVersionInfo(fileName);
            FileVersion = myFileVersionInfo.FileVersion;
            string DetailMessage;
            string[] strSplit = FileVersion.Split(',');
            if (Convert.ToUInt16(strSplit[0]) < 2)
            {

                DetailMessage = "The Driver Version  Is Too Low" + "\r\nYou can update the driver through the driver installation package ";
                DetailMessage = DetailMessage + "\r\nThe Current Driver Version Number is " + FileVersion;
                DetailMessage = DetailMessage + "\r\nYou need to update the driver to 2.0.0.0 version and above";
                //MessageBox.Show(DetailMessage, "CMove", MessageBoxButtons.OK, MessageBoxIcon.Error);
                FuncLog.WriteLog("FuncAdvantech -" + "GetDevCfgDllDrvVer() -" + DetailMessage);
                return false;
            }
            return true;
        }
        public static Boolean InitializeComponent()
        {
            VersionIsOk = GetDevCfgDllDrvVer(); //Get Driver Version Number, this step is not necessary

            int Result;
            string strTemp;
            if (VersionIsOk == false)
            {
                return false;
            }
            // Get the list of available device numbers and names of devices, of which driver has been loaded successfully 
            //If you have two/more board,the device list(m_avaDevs) may be changed when the slot of the boards changed,for example:m_avaDevs[0].szDeviceName to PCI-1245
            //m_avaDevs[1].szDeviceName to PCI-1245L,changing the slot，Perhaps the opposite
            Result = Motion.mAcm_GetAvailableDevs(CurAvailableDevs, Motion.MAX_DEVICES, ref deviceCount);
            if (Result != (int)ErrorCode.SUCCESS)
            {
                strTemp = "Get Device Numbers Failed With Error Code: [0x" + Convert.ToString(Result, 16) + "]";
                //ShowMessages(strTemp, (uint)Result);
                FuncLog.WriteLog("FuncAdvantech -" + "InitializeComponent() -" + strTemp);
                return false;
            }
            //If you want to get the device number of fixed equipment，you also can achieve it By adding the API:GetDevNum(UInt32 DevType, UInt32 BoardID, UInt32 MasterRingNo, UInt32 SlaveBoardID),
            //The API is defined and illustrates the way of using in this example,but it is not called,you can copy it to your program and
            //don't need to call Motion.mAcm_GetAvailableDevs(CurAvailableDevs, Motion.MAX_DEVICES, ref deviceCount)
            //GetDevNum(UInt32 DevType, UInt32 BoardID, UInt32 MasterRingNo, UInt32 SlaveBoardID) API Variables are stated below:
            //UInt32 DevType : Set Device Type ID of your motion card plug in PC. (Definition is in ..\Public\AdvMotDev.h)
            //UInt32 BoardID : Set Hardware Board-ID of your motion card plug in PC,you can get it from Utility
            //UInt32 MasterRingNo: PCI-Motion card, Always set to 0
            //UInt32 SlaveBoardID : PCI-Motion card,Always set to 0
            //CmbAvailableDevice.Items.Clear();         //내가 스킵
            //for (int i = 0; i < deviceCount; i++)
            //{
            //    CmbAvailableDevice.Items.Add(CurAvailableDevs[i].DeviceName);
            //}
            if (deviceCount > 0)
            {
                //CmbAvailableDevice.SelectedIndex = 0;
                DeviceNum = CurAvailableDevs[0].DeviceNum;
            }
            return true;

        }
        public static void OpenBoard()
        {
            uint Result;
            uint i = 0;
            uint[] slaveDevs = new uint[16];
            uint AxesPerDev = new uint();
            string strTemp;
            //Open a specified device to get device handle
            //you can call GetDevNum() API to get the devcie number of fixed equipment in this,as follow
            //DeviceNum = GetDevNum((uint)DevTypeID.PCI1285, 15, 0, 0);
            Result = Motion.mAcm_DevOpen(DeviceNum, ref m_DeviceHandle);
            if (Result != (uint)ErrorCode.SUCCESS)
            {
                strTemp = "Open Device Failed With Error Code: [0x" + Convert.ToString(Result, 16) + "]";
                //ShowMessages(strTemp, Result);
                FuncLog.WriteLog("FuncAdvantech -" + "OpenBoard() -" + "mAcm_DevOpen -" + strTemp);
                return;
            }
            //FT_DevAxesCount:Get axis number of this device.
            //if you device is fixed(for example: PCI-1245),You can not get FT_DevAxesCount property value
            //This step is not necessary
            //You can also use the old API: Motion.mAcm_GetProperty(m_DeviceHandle, (uint)PropertyID.FT_DevAxesCount, ref AxesPerDev, ref BufferLength);
            // UInt32 BufferLength;
            //BufferLength =4;  buffer size for the property
            Result = Motion.mAcm_GetU32Property(m_DeviceHandle, (uint)PropertyID.FT_DevAxesCount, ref AxesPerDev);
            if (Result != (uint)ErrorCode.SUCCESS)
            {
                strTemp = "Get Axis Number Failed With Error Code: [0x" + Convert.ToString(Result, 16) + "]";
                //ShowMessages(strTemp, Result);
                FuncLog.WriteLog("FuncAdvantech -" + "OpenBoard() -" + "mAcm_GetU32Property -" + strTemp);
                return;
            }
            m_ulAxisCount = AxesPerDev;
            //            CmbAxes.Items.Clear();                                        // 내가 스킵
            //if you device is fixed,for example: PCI-1245 m_ulAxisCount =4
            for (i = 0; i < m_ulAxisCount; i++)
            {
                //Open every Axis and get the each Axis Handle
                //And Initial property for each Axis 		
                //Open Axis 
                Result = Motion.mAcm_AxOpen(m_DeviceHandle, (UInt16)i, ref m_Axishand[i]);
                if (Result != (uint)ErrorCode.SUCCESS)
                {
                    strTemp = "Open Axis Failed With Error Code: [0x" + Convert.ToString(Result, 16) + "]";
                    //ShowMessages(strTemp, Result);
                    FuncLog.WriteLog("FuncAdvantech -" + "OpenBoard() -" + "mAcm_AxOpen -" + strTemp);
                    return;
                }
                //CmbAxes.Items.Add(String.Format("{0:d}-Axis", i));            //내가 스킵
                double cmdPosition = new double();
                cmdPosition = 0;
                //Set command position for the specified axis
                Motion.mAcm_AxSetCmdPosition(m_Axishand[i], cmdPosition);
                //Set actual position for the specified axis
                Motion.mAcm_AxSetActualPosition(m_Axishand[i], cmdPosition);
            }
            //CmbAxes.SelectedIndex = 0;                    //내가 스킵
            m_bInit = true;
            //timer1.Enabled = true;

        }

        #endregion


        #region Servo On/Off/Ararm Reset

        //개별 적인 On Off를 만들어야 할 것 같다.
        public static bool Servo_On(ushort Axis)
        {
            if (GlobalVar.Simulation)
            {
                // simulation
                GlobalVar.AxisStatus[Axis].PowerOn = true;
                return true;
            }

            //VersionIsOk = GetDevCfgDllDrvVer(); //Get Driver Version Number, this step is not necessary
            //InitializeComponent();
            //OpenBoard();

            UInt32 Result;
            string strTemp;
            //Check the servoOno flag to decide if turn on or turn off the ServoOn output.
            if (m_bInit != true)
            {
                return false;
            }

            // Set servo Driver ON,1: On                
            Result = Motion.mAcm_AxSetSvOn(m_Axishand[Axis], 1);
            if (Result != (uint)ErrorCode.SUCCESS)
            {
                strTemp = "Servo On Failed With Error Code: [0x" + Convert.ToString(Result, 16) + "]";
                //ShowMessages(strTemp, Result);
                FuncLog.WriteLog("FuncAdvantech -" + "Servo_On() -" + strTemp);
                return false;
            }
            else
            {
                GlobalVar.AxisStatus[Axis].PowerOn = true;
                return true;
            }
        }
        public static bool Servo_Off(ushort Axis)
        {
            if (GlobalVar.Simulation)
            {
                // simulation
                GlobalVar.AxisStatus[Axis].PowerOn = false;
                return true;
            }
            //VersionIsOk = GetDevCfgDllDrvVer(); //Get Driver Version Number, this step is not necessary
            //InitializeComponent();
            //OpenBoard();

            UInt32 Result;
            string strTemp;
            //Check the servoOno flag to decide if turn on or turn off the ServoOn output.
            if (m_bInit != true)
            {
                return false;
            }

            // Set servo Driver ON,1: On                
            Result = Motion.mAcm_AxSetSvOn(m_Axishand[Axis], 0);
            if (Result != (uint)ErrorCode.SUCCESS)
            {
                strTemp = "Servo On Failed With Error Code: [0x" + Convert.ToString(Result, 16) + "]";
                //ShowMessages(strTemp, Result);
                FuncLog.WriteLog("FuncAdvantech -" + "Servo_Off() -" + strTemp);
                return false;
            }
            else
            {
                GlobalVar.AxisStatus[Axis].PowerOn = false;
                return true;
            }
        }
        public static void Servo_ArarmReset(ushort Axis)
        {
            //UInt32 Result;
            //string strTemp;
            ////Reset the axis' state. If the axis is in ErrorStop state, the state will
            ////be changed to Ready after calling this function.
            //Result = Motion.mAcm_AxResetError(m_Axishand[Axis]);
            //if (Result != (uint)ErrorCode.SUCCESS)
            //{
            //    strTemp = "Reset axis's error failed With Error Code: [0x" + Convert.ToString(Result, 16) + "]";
            //    //ShowMessages(strTemp, Result);
            //    FuncLog.WriteLog("FuncAdvantech -" + "Servo_ArarmReset() -" + strTemp);
            //    return;
            //}
            Motion.mAcm_AxDoSetBit(m_Axishand[Axis], 7, 1);
            Thread.Sleep(500);
            Motion.mAcm_AxDoSetBit(m_Axishand[Axis], 7, 0);
        }




        public static void Servo_OnAll(byte on)
        {
            //VersionIsOk = GetDevCfgDllDrvVer(); //Get Driver Version Number, this step is not necessary
            //InitializeComponent();
            //OpenBoard();

            UInt32 AxisNum;
            UInt32 Result;
            string strTemp;
            //Check the servoOno flag to decide if turn on or turn off the ServoOn output.
            if (m_bInit != true)
            {
                return;
            }
            if (m_bServoOn == false)
            {
                for (AxisNum = 0; AxisNum < m_ulAxisCount; AxisNum++)
                {
                    if (GlobalVar.Simulation)
                    {
                        // simulation
                        GlobalVar.AxisStatus[AxisNum].PowerOn = true;
                    }
                    else
                    {
                        // Set servo Driver ON,1: On
                        Result = Motion.mAcm_AxSetSvOn(m_Axishand[AxisNum], 1);
                        if (Result != (uint)ErrorCode.SUCCESS)
                        {
                            strTemp = "Servo On Failed With Error Code: [0x" + Convert.ToString(Result, 16) + "]";
                            //ShowMessages(strTemp, Result);
                            FuncLog.WriteLog("FuncAdvantech -" + "Servo_OnAll() -" + strTemp);
                        }
                        else
                        {
                            m_bServoOn = true;
                        }
                        //BtnServo.Text = "Servo Off";
                    }
                }
            }
            else
            {
                for (AxisNum = 0; AxisNum < m_ulAxisCount; AxisNum++)
                {
                    if (GlobalVar.Simulation)
                    {
                        // simulation
                        GlobalVar.AxisStatus[AxisNum].PowerOn = false;
                    }
                    else
                    {
                        // Set servo Driver OFF,0: Off
                        Result = Motion.mAcm_AxSetSvOn(m_Axishand[AxisNum], 0);
                        if (Result != (uint)ErrorCode.SUCCESS)
                        {
                            strTemp = "Servo Off Failed With Error Code: [0x" + Convert.ToString(Result, 16) + "]";
                            //ShowMessages(strTemp, Result);
                            FuncLog.WriteLog("FuncAdvantech -" + "Servo_OnAll() -" + strTemp);
                        }
                        else
                        {
                            m_bServoOn = false;
                        }
                        //BtnServo.Text = "Servo On";
                    }
                }
            }
        }

        #endregion

        public enum Advantech_AXISSTATUS
        {
            Ready = 0,
            Alarm = 1,
            Positive_Limit = 2,
            Negative_Limit = 3
        }
        public enum Advantech_AXISMOTIONSTATUS
        {
            Stop = 0,
            Wait_ERC_Finished = 2,
            Correcting_Backlash = 4,
            Feeding_in_return_velocity = 6,
            Feeding_in_StrVel_speed = 7,
            Accelerating = 8,
            Feeding_in_MaxVel_speed = 9,
            Decelerating = 10,
            Waiting_for_INP_input = 11
        }


        #region 서보 체크
        public static uint ServoIOCheck(uint axis)
        {
            UInt32 AxisNum = axis;
            UInt32 Result, Result1;
            UInt32 IOStatus = new UInt32();
            UInt32 IOStatus1 = new UInt32();
            string strTemp;


            Result1 = Motion.mAcm_AxGetMotionStatus(m_Axishand[AxisNum], ref IOStatus1);

            Result = Motion.mAcm_AxGetMotionIO(m_Axishand[AxisNum], ref IOStatus);
            if (Result != (uint)ErrorCode.SUCCESS)
            {
                strTemp = "Servo Check Error Code: [0x" + Convert.ToString(IOStatus, 16) + "]";
                //ShowMessages(strTemp, Result);
                FuncLog.WriteLog("FuncAdvantech -" + "ServoIOCheck() -" + strTemp);
                return 0;
            }
            else
            {
                strTemp = "Servo Check Success Code: [0x" + Convert.ToString(IOStatus, 16) + "]" + "[0x" + Convert.ToString(IOStatus1, 16) + "]";
                //FuncLog.WriteLog("FuncAdvantech -" + "ServoIOCheck() -" + strTemp);
                return IOStatus;
            }

            //else if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_RDY) > 0)//RDY
            //{
            //    return (int)Advantech_AXISSTATUS.Ready;
            //}
            ////else if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_ALM) > 0)//ALM
            ////{

            ////}
            ////else if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_LMTP) > 0)//LMTP
            ////{

            ////}
            ////else if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_LMTN) > 0)//LMTN
            ////{

            ////}
            ////else if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_ORG) > 0)//ORG
            ////{

            ////}
            ////else if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_DIR) > 0)//DIR
            ////{

            ////}
            ////else if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_EMG) > 0)//EMG
            ////{

            ////}
            ////else if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_PCS) > 0)//PCS
            ////{

            ////}
            ////else if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_ERC) > 0)//ERC
            ////{

            ////}
            ////else if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_EZ) > 0)//EZ
            ////{

            ////}
            ////else if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_CLR) > 0)//CLR
            ////{

            ////}
            ////else if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_LTC) > 0)//LTC
            ////{

            ////}
            ////else if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_SD) > 0)//SD
            ////{

            ////}
            ////else if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_INP) > 0)//INP
            ////{

            ////}
            ////else if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_SVON) > 0)//SVON
            ////{

            ////}
            ////else if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_ALRM) > 0)//ALRM
            ////{

            ////}
            ////else if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_SLMTP) > 0)//SLMTP
            ////{

            ////}
            ////else if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_SLMTN) > 0)//SLMTN
            ////{

            ////}
            ////else if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_CMP) > 0)//CMP
            ////{

            ////}
            ////else if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_CAMDO) > 0)//CAMDO
            ////{

            ////}
            ////else if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_MAXTORLMT) > 0)//MAXTORLMT
            ////{

            ////}

            //return 99;

        }

        public static int ServoStateCheck(uint axis)
        {
            UInt16 AxState = new UInt16();
            uint Result;

            //Get the Axis's current state
            Result = Motion.mAcm_AxGetState(m_Axishand[axis], ref AxState);
            if (Result == (uint)ErrorCode.SUCCESS)
            {
                return AxState;
            }
            else
            {
                return -1;
            }
        }

        public static void GetMotionIOStatus(uint IOStatus)
        {
            //if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_ALM) > 0)//ALM
            //{
            //    pictureBoxALM.BackColor = Color.Red;
            //}
            //else
            //{
            //    pictureBoxALM.BackColor = Color.Gray;
            //}

            //if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_ORG) > 0)//ORG
            //{
            //    pictureBoxORG.BackColor = Color.Red;
            //}
            //else
            //{
            //    pictureBoxORG.BackColor = Color.Gray;
            //}

            //if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_LMTP) > 0)//+EL
            //{
            //    pictureBoxPosHEL.BackColor = Color.Red;
            //}
            //else
            //{
            //    pictureBoxPosHEL.BackColor = Color.Gray;
            //}

            //if ((IOStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_LMTN) > 0)//-EL
            //{
            //    pictureBoxNegHEL.BackColor = Color.Red;
            //}
            //else
            //{
            //    pictureBoxNegHEL.BackColor = Color.Gray;
            //}
        }

        public static double ServoPositionCheck(uint axis)
        {
            double CurCmd = new double();
            double CurPos = new double();

            //Get current command position of the specified axis
            Motion.mAcm_AxGetCmdPosition(m_Axishand[axis], ref CurCmd);
            //Get current actual position of the specified axis
            Motion.mAcm_AxGetActualPosition(m_Axishand[axis], ref CurPos);

            //return CurPos;//CurCmd
            return CurCmd;//CurCmd

        }
        public static double ServoVelocityCheck(uint axis)
        {
            double CurVel = new double();

            Motion.mAcm_AxGetCmdVelocity(m_Axishand[axis], ref CurVel);
            //textBoxVel.Text = Convert.ToString(CurVel);
            return CurVel;//CurCmd
        }
        #endregion

        #region Servo Set

        public static void Servo_Set(uint axis, double VelL, double VelH, double ACC, double DEC)
        {
            UInt32 AxisNum = axis;

            UInt32 Result;
            double AxVelLow;
            double AxVelHigh;
            double AxAcc;
            double AxDec;
            //double AxJerk;
            string strTemp;
            //AxVelLow = Convert.ToDouble(textBoxVelL.Text);
            AxVelLow = Convert.ToDouble(VelL);
            //Set low velocity (start velocity) of this axis (Unit: PPU/S).
            //This property value must be smaller than or equal to PAR_AxVelHigh
            //You can also use the old API:Motion.mAcm_SetProperty(m_Axishand[CmbAxes.SelectedIndex], (uint)PropertyID.PAR_AxVelLow, ref AxVelLow, BufferLength);
            // UInt32  BufferLength;
            //BufferLength =8; buffer size for the property
            Result = Motion.mAcm_SetF64Property(m_Axishand[AxisNum], (uint)PropertyID.PAR_AxVelLow, AxVelLow);
            if (Result != (uint)ErrorCode.SUCCESS)
            {
                strTemp = "Set low velocity failed with error code: [0x" + Convert.ToString(Result, 16) + "]";
                //ShowMessages(strTemp, Result);
                FuncLog.WriteLog("FuncAdvantech -" + "Servo_Set() -" + strTemp);
                return;
            }
            //AxVelHigh = Convert.ToDouble(textBoxVelH.Text);
            AxVelHigh = Convert.ToDouble(VelH);
            // Set high velocity (driving velocity) of this axis (Unit: PPU/s).
            //This property value must be smaller than CFG_AxMaxVel and greater than PAR_AxVelLow
            //You can also use the old API:Motion.mAcm_SetProperty(m_Axishand[CmbAxes.SelectedIndex], (uint)PropertyID.PAR_AxVelHigh,ref AxVelHigh,BufferLength)
            // UInt32  BufferLength;
            //BufferLength =8; buffer size for the property
            Result = Motion.mAcm_SetF64Property(m_Axishand[AxisNum], (uint)PropertyID.PAR_AxVelHigh, AxVelHigh);
            if (Result != (uint)ErrorCode.SUCCESS)
            {
                strTemp = "Set high velocity failed with error code: [0x" + Convert.ToString(Result, 16) + "]";
                //ShowMessages(strTemp, Result);
                FuncLog.WriteLog("FuncAdvantech -" + "Servo_Set() -" + strTemp);
                return;
            }
            //AxAcc = Convert.ToDouble(textBoxAcc.Text);
            AxAcc = Convert.ToDouble(ACC);
            // Set acceleration of this axis (Unit: PPU/s2).
            //This property value must be smaller than or equal to CFG_AxMaxAcc
            //You can also use the old API:Motion.mAcm_SetProperty(m_Axishand[CmbAxes.SelectedIndex], (uint)PropertyID.PAR_AxAcc,ref AxAcc,BufferLength)
            // UInt32  BufferLength;
            //BufferLength =8; buffer size for the property
            Result = Motion.mAcm_SetF64Property(m_Axishand[AxisNum], (uint)PropertyID.PAR_AxAcc, AxAcc);
            if (Result != (uint)ErrorCode.SUCCESS)
            {
                strTemp = "Set acceleration failed with error code: [0x" + Convert.ToString(Result, 16) + "]";
                //ShowMessages(strTemp, Result);
                FuncLog.WriteLog("FuncAdvantech -" + "Servo_Set() -" + strTemp);
                return;
            }
            //AxDec = Convert.ToDouble(textBoxDec.Text);
            AxDec = Convert.ToDouble(DEC);
            //Set deceleration of this axis (Unit: PPU/s2).
            //This property value must be smaller than or equal to CFG_AxMaxDec
            //You can also use the old API:Motion.mAcm_SetProperty(m_Axishand[CmbAxes.SelectedIndex], (uint)PropertyID.PAR_AxDcc,ref AxDec,BufferLength)
            // UInt32  BufferLength;
            //BufferLength =8; buffer size for the property
            Result = Motion.mAcm_SetF64Property(m_Axishand[AxisNum], (uint)PropertyID.PAR_AxDec, AxDec);
            if (Result != (uint)ErrorCode.SUCCESS)
            {
                strTemp = "Set deceleration failed with error code: [0x" + Convert.ToString(Result, 16) + "]";
                //ShowMessages(strTemp, Result);
                FuncLog.WriteLog("FuncAdvantech -" + "Servo_Set() -" + strTemp);
                return;
            }
            //if (rdb_T.Checked)
            //{
            //    AxJerk = 0;
            //}
            //else
            //{
            //    AxJerk = 1;
            //}
            //Set the type of velocity profile: t-curve or s-curve
            //You can also use the old API:Motion.mAcm_SetProperty(m_Axishand[CmbAxes.SelectedIndex], (uint)PropertyID.PAR_AxJerk,ref AxJerk,BufferLength)
            // UInt32  BufferLength;
            //BufferLength =8; buffer size for the property
            Result = Motion.mAcm_SetF64Property(m_Axishand[axis], (uint)PropertyID.PAR_AxJerk, 1);//S-Curver로 고정한다.
            if (Result != (uint)ErrorCode.SUCCESS)
            {
                strTemp = "Set the type of velocity profile failed with error code: [0x" + Convert.ToString(Result, 16) + "]";
                //ShowMessages(strTemp, Result);
                FuncLog.WriteLog("FuncAdvantech -" + "Servo_Set() -" + strTemp);
                return;
            }

            GetAxisVelParam(AxisNum); //Get Axis Velocity Param
        }

        private static void GetAxisVelParam(uint AxisNum)
        {

            double axvellow = new double();
            double axvelhigh = new double();
            double axacc = new double();
            double axdec = new double();
            UInt32 Result;
            string strTemp = "";
            //Get low velocity (start velocity) of this axis (Unit: PPU/S).
            //You can also use the old API:  Acm_GetProperty(m_Axishand[CmbAxes.SelectedIndex], (uint)PropertyID.PAR_AxVelLow, ref axvellow,ref BufferLength);
            // uint BufferLength;
            // BufferLength = 8; buffer size for the property
            Result = Motion.mAcm_GetF64Property(m_Axishand[AxisNum], (uint)PropertyID.PAR_AxVelLow, ref axvellow);
            if (Result != (uint)ErrorCode.SUCCESS)
            {
                strTemp = "Get low velocity failed with error code: [0x" + Convert.ToString(Result, 16) + "]";
                //ShowMessages(strTemp, Result);
                FuncLog.WriteLog("FuncAdvantech -" + "GetAxisVelParam() -" + strTemp);
                return;
            }
            //textBoxVelL.Text = Convert.ToString(axvellow);
            //get high velocity (driving velocity) of this axis (Unit: PPU/s).
            //You can also use the old API:  Acm_GetProperty(m_Axishand[CmbAxes.SelectedIndex], (uint)PropertyID.PAR_AxVelHigh, ref axvelhigh,ref BufferLength);
            // uint BufferLength;
            // BufferLength = 8; buffer size for the property
            Result = Motion.mAcm_GetF64Property(m_Axishand[AxisNum], (uint)PropertyID.PAR_AxVelHigh, ref axvelhigh);
            if (Result != (uint)ErrorCode.SUCCESS)
            {
                strTemp = "Get High velocity failed with error code: [0x" + Convert.ToString(Result, 16) + "]";
                //ShowMessages(strTemp, Result);
                FuncLog.WriteLog("FuncAdvantech -" + "GetAxisVelParam() -" + strTemp);
                return;
            }
            //textBoxVelH.Text = Convert.ToString(axvelhigh);
            //get acceleration of this axis (Unit: PPU/s2).
            //You can also use the old API:  Acm_GetProperty(m_Axishand[CmbAxes.SelectedIndex], (uint)PropertyID.PAR_AxAcc, ref axacc,ref BufferLength);
            // uint BufferLength;
            // BufferLength = 8; buffer size for the property
            Result = Motion.mAcm_GetF64Property(m_Axishand[AxisNum], (uint)PropertyID.PAR_AxAcc, ref axacc);
            if (Result != (uint)ErrorCode.SUCCESS)
            {
                strTemp = "Get acceleration  failed with error code: [0x" + Convert.ToString(Result, 16) + "]";
                //ShowMessages(strTemp, Result);
                FuncLog.WriteLog("FuncAdvantech -" + "GetAxisVelParam() -" + strTemp);
                return;
            }
            //textBoxAcc.Text = Convert.ToString(axacc);
            //get deceleration of this axis (Unit: PPU/s2).
            //You can also use the old API: Motion.mAcm_GetProperty(m_Axishand[CmbAxes.SelectedIndex], (uint)PropertyID.PAR_AxDec, ref axdec, ref BufferLength);
            // uint BufferLength;
            // BufferLength = 8; buffer size for the property
            Result = Motion.mAcm_GetF64Property(m_Axishand[AxisNum], (uint)PropertyID.PAR_AxDec, ref axdec);
            if (Result != (uint)ErrorCode.SUCCESS)
            {
                strTemp = "Get deceleration  failed with error code: [0x" + Convert.ToString(Result, 16) + "]";
                //ShowMessages(strTemp, Result);
                FuncLog.WriteLog("FuncAdvantech -" + "GetAxisVelParam() -" + strTemp);
                return;
            }
            //textBoxDec.Text = Convert.ToString(axdec);

            strTemp = "VelL : " + Convert.ToString(axvellow) + "VelH : " + Convert.ToString(axvelhigh) + "ACC : " + Convert.ToString(axacc) + "DEC : " + Convert.ToString(axdec);
            //ShowMessages(strTemp, Result);
            //FuncLog.WriteLog("FuncAdvantech -" + "GetAxisVelParam() -" + strTemp);
            return;
        }
        #endregion



        #region Servo Home
        public enum ServoHomeDrection
        {
            Positive_Direction = 0,
            Negative_Direction = 1
        }
        public enum ServoHomeMode
        {
            MODE1_Abs = 0,
            MODE2_Lmt = 1,
            MODE3_Ref = 2,
            MODE4_Abs_Ref = 3,
            MODE5_Abs_NegRef = 4,
            MODE6_Lmt_Ref = 5,
            MODE7_AbsSearch = 6,
            MODE8_LmtSearch = 7,
            MODE9_AbsSearch_Ref = 8,
            MODE10_AbsSearch_NegRef = 9,
            MODE11_LmtSearch_Ref = 10,
            MODE12_AbsSearchReFind = 11,
            MODE13_LmtSearchReFind = 12,
            MODE14_AbsSearchReFind_Ref = 13,
            MODE15_AbsSearchReFind_NegRef = 14,
            MODE16_LmtSearchReFind_Ref = 15
        }
        public enum ServoHomeSwitchMode
        {
            Level_On = 0,
            Level_Off = 1,
            Edge_On = 2,
            Edge_Off = 3
        }
        public enum ServoHomeCrossDistance
        {
            Low = 0,
            Middle = 100,
            High = 1000,
        }
        public static void Servo_Home(uint axis)
        {
            string strTemp;
            UInt32 Result;
            UInt32 PropertyVal = new UInt32();
            double CrossDistance = new double();
            UInt32 dir = new UInt32();

            if (axis == 0 ||
              axis == 1 ||
              axis == 2 ||
              axis == 3)
            {
                Servo_Set(axis, 10, 20, 10000, 10000);
            }

            //PropertyVal = (UInt32)comboBoxSwitchMode.SelectedIndex;
            PropertyVal = (UInt32)ServoHomeSwitchMode.Edge_On;

            //Setting the stopping condition of Acm_AxHomeEx
            //You can also use the old API: Motion.mAcm_SetProperty(m_Axishand[CmbAxes.SelectedIndex], (uint)PropertyID.PAR_AxHomeExSwitchMode, ref PropertyVal, (uint)Marshal.SizeOf(typeof(UInt32)));
            //Result = Motion.mAcm_SetU32Property(m_Axishand[axis], (uint)PropertyID.PAR_AxHomeExSwitchMode, PropertyVal);
            Result = Motion.mAcm_SetU32Property(m_Axishand[axis], (uint)PropertyID.PAR_AxHomeExSwitchMode, PropertyVal);
            if (Result != (uint)ErrorCode.SUCCESS)
            {
                strTemp = "Set Property-PAR_AxHomeExSwitchMode Failed With Error Code: [0x" + Convert.ToString(Result, 16) + "]";
                //ShowMessages(strTemp, Result);
                FuncLog.WriteLog("FuncAdvantech -" + "Servo_Home() -" + strTemp);
                return;
            }

            //CrossDistance = Convert.ToDouble(textBoxCross.Text);
            CrossDistance = Convert.ToDouble(ServoHomeCrossDistance.Middle);

            //Set the home cross distance (Unit: PPU). This property must be greater than 0. The default value is 10000
            //You can also use the old API: Motion.mAcm_SetProperty(m_Axishand[CmbAxes.SelectedIndex], (uint)PropertyID.PAR_AxHomeCrossDistance, ref CrossDistance, (uint)Marshal.SizeOf(typeof(double)));
            //Result = Motion.mAcm_SetF64Property(m_Axishand[axis], (uint)PropertyID.PAR_AxHomeCrossDistance, CrossDistance);
            Result = Motion.mAcm_SetF64Property(m_Axishand[axis], (uint)PropertyID.PAR_AxHomeCrossDistance, CrossDistance);
            if (Result != (uint)ErrorCode.SUCCESS)
            {
                strTemp = "Set Property-AxHomeCrossDistance Failed With Error Code: [0x" + Convert.ToString(Result, 16) + "]";
                //ShowMessages(strTemp, Result);
                FuncLog.WriteLog("FuncAdvantech -" + "Servo_Home() -" + strTemp);
                return;
            }
            //To command axis to start typical home motion. The 15 types of typical
            //home motion are composed of extended home
            //Result = Motion.mAcm_AxHome(m_Axishand[axis ], (UInt32)comboBoxMode.SelectedIndex, (UInt32)comboBoxDir.SelectedIndex);

            if (axis == 0 ||
             axis == 2)
            {
                dir = (UInt32)ServoHomeDrection.Positive_Direction;
            }
            else if (axis == 1 ||
             axis == 3)
            {
                dir = (UInt32)ServoHomeDrection.Negative_Direction;
            }
            else//4축
            {
                dir = (UInt32)ServoHomeDrection.Positive_Direction;
            }

            Result = Motion.mAcm_AxHome(m_Axishand[axis], (UInt32)ServoHomeMode.MODE7_AbsSearch, dir);
            if (Result != (uint)ErrorCode.SUCCESS)
            {
                strTemp = "AxHome Failed With Error Code: [0x" + Convert.ToString(Result, 16) + "]";
                //ShowMessages(strTemp, Result);
                FuncLog.WriteLog("FuncAdvantech -" + "Servo_Home() -" + strTemp);
            }
            return;
        }






        #endregion


        #region Servo Move
        public static void Servo_Stop(uint axis)
        {
            UInt32 AxisNum = axis;

            uint Result;
            string strTemp;
            if (m_bInit)
            {
                //To command axis to decelerate to stop.
                Result = Motion.mAcm_AxStopDec(m_Axishand[AxisNum]);
                if (Result != (uint)ErrorCode.SUCCESS)
                {
                    strTemp = "Axis To decelerate Stop Failed With Error Code: [0x" + Convert.ToString(Result, 16) + "]";
                    //ShowMessages(strTemp, Result);
                    FuncLog.WriteLog("FuncAdvantech -" + "Servo_Stop() -" + strTemp);
                    return;
                }
            }
            return;
        }

        public static void Servo_All_Stop(bool on)
        {
            //UInt32 AxisNum = axis;

            uint Result;
            string strTemp;
            if (m_bInit)
            {
                for (ushort axis = 0; axis < GlobalVar.Axis_count; axis++)
                {
                    //To command axis to decelerate to stop.
                    Result = Motion.mAcm_AxStopDec(m_Axishand[axis]);
                    if (Result != (uint)ErrorCode.SUCCESS)
                    {
                        strTemp = "Axis To decelerate ALL Stop Failed With Error Code: [0x" + Convert.ToString(Result, 16) + "]";
                        //ShowMessages(strTemp, Result);
                        FuncLog.WriteLog("FuncAdvantech -" + "Servo_Stop() -" + strTemp);
                    }
                }
            }
            //            return;
        }

        public static bool Servo_Move_Vel_Change(uint axis, double speed)
        {
            UInt32 AxisNum = axis;

            string strTemp;
            UInt32 Result;

            if (speed < 1)
            {
                GlobalVar.SystemMsg = "Vel Range Over. 1~100 Input .";
                return false;
            }

            //To command axis to change the velocity while axis is in velocity  motion
            if (axis == 2)
            {
                if (speed > 100)
                {
                    speed = 100;
                }
                Result = Motion.mAcm_AxChangeVel(m_Axishand[AxisNum], speed * 2000);
                if (Result != (uint)ErrorCode.SUCCESS)
                {
                    strTemp = "Change Velocity Failed With Error Code: [0x" + Convert.ToString(Result, 16) + "]";
                    //ShowMessages(strTemp, Result1);
                    FuncLog.WriteLog("FuncAdvantech -" + "Servo_Move_Vel_Left() -" + strTemp);
                    return false;
                }
            }
            return true;
        }


        //오른쪽이 필요하다면 인자를 한개 더 받으면 될 것 같다.
        public static bool Servo_Move_Vel_Left(uint axis, double speed)
        {
            UInt32 AxisNum = axis;

            string strTemp;
            UInt32 Result;

            if (axis == 4)
            {
                Servo_Set(AxisNum, 200, speed, 1000000, 1000000);
            }

            //To command axis to make a never ending movement with a specified velocity.1: Negative direction.
            Result = Motion.mAcm_AxMoveVel(m_Axishand[AxisNum], 0);
            if (Result != (uint)ErrorCode.SUCCESS)
            {
                strTemp = "Move Failed With Error Code[0x" + Convert.ToString(Result, 16) + "]";
                //ShowMessages(strTemp, Result);
                FuncLog.WriteLog("FuncAdvantech -" + "Servo_Move_Vel_Left() -" + strTemp);
                return false;
            }
            return true;
        }
        //오른쪽이 필요하다면 인자를 한개 더 받으면 될 것 같다.
        public static bool Servo_Move_Vel_Right(uint axis, double speed)
        {//마스크 이동하는 서버에서 홈 잡을 때 사용한다.
            UInt32 AxisNum = axis;

            string strTemp;
            UInt32 Result;

            //if (DIO.GetDIData(FuncInline.enumDINames.X0_20_Ear_Loop_Fold_Guide_Forward) ||
            //  DIO.GetDIData(FuncInline.enumDINames.X0_22_Ear_Loop_Fold_Forward_0) ||
            //  DIO.GetDIData(FuncInline.enumDINames.X0_24_Ear_Loop_Fold_Forward_1) ||
            //  DIO.GetDIData(FuncInline.enumDINames.X0_26_Ear_Loop_Fold_Push_Forward) ||
            //  DIO.GetDIData(FuncInline.enumDINames.X0_61_Ear_Loop_Fusion_Down) ||
            //  !DIO.GetDIData(FuncInline.enumDINames.X0_09_Mask_Moving_Safety_Check))//57, 59도 해야 되나?
            //{
            //    FuncLog.WriteLog("Mask Move Can not. Cylinder Check.");
            //    Func.AddError(enumError.Mask_Moving_Cannot_Cylinder_Check);
            //    return false;
            //}


            if (axis == 4)
            {
                Servo_Set(AxisNum, 2000, speed, 1000000, 1000000);
            }

            //To command axis to make a never ending movement with a specified velocity.1: Negative direction.
            Result = Motion.mAcm_AxMoveVel(m_Axishand[AxisNum], 1);
            if (Result != (uint)ErrorCode.SUCCESS)
            {
                strTemp = "Move Failed With Error Code[0x" + Convert.ToString(Result, 16) + "]";
                //ShowMessages(strTemp, Result);
                FuncLog.WriteLog("FuncAdvantech -" + "Servo_Move_Vel_Left() -" + strTemp);
                return false;
            }
            return true;
        }

        public static void Servo_Move_Abs(uint axis, double position, double VelL, double VelH, double ACC, double DEC)
        {
            UInt32 AxisNum = axis;

            UInt32 Result;
            string strTemp;
            if (m_bInit)
            {
                //if (axis == 4)
                //{
                //    if (!DIO.GetDIData(FuncInline.enumDINames.X0_27_Ear_Loop_Fold_Push_Backward) ||
                //       !DIO.GetDIData(FuncInline.enumDINames.X0_21_Ear_Loop_Fold_Guide_Backward) ||
                //       !DIO.GetDIData(FuncInline.enumDINames.X0_23_Ear_Loop_Fold_Backward_0) ||
                //       !DIO.GetDIData(FuncInline.enumDINames.X0_25_Ear_Loop_Fold_Backward_1) ||
                //       !DIO.GetDIData(FuncInline.enumDINames.X0_60_Ear_Loop_Fusion_Up) ||
                //       !DIO.GetDIData(FuncInline.enumDINames.X0_56_Ear_Loop_Position_Up_0) ||
                //       !DIO.GetDIData(FuncInline.enumDINames.X0_58_Ear_Loop_Position_Up_1))
                //    {
                //        FuncLog.WriteLog("Mask Move Can not. Cylinder Check.");
                //        Func.AddError(enumError.Mask_Moving_Cannot_Cylinder_Check);
                //        return;
                //    }
                //}

                Servo_Set(AxisNum, VelL, VelH, ACC, DEC);
                //Servo_Set(AxisNum, VelL, VelH, ACC, DEC);

                //Start single axis's absolute position motion.
                Result = Motion.mAcm_AxMoveAbs(m_Axishand[AxisNum], Convert.ToDouble(position));

                if (Result != (uint)ErrorCode.SUCCESS)
                {
                    strTemp = "PTP Move Failed With Error Code: [0x" + Convert.ToString(Result, 16) + "]";
                    //ShowMessages(strTemp, Result);
                    FuncLog.WriteLog("FuncAdvantech -" + "Servo_Move_Abs() -" + strTemp);
                }
            }
            return;
        }
        public static void Servo_Move_Rel(uint axis, double position, double VelL, double VelH, double ACC, double DEC)
        {//마스크 이동하는 서버에서 자동운전 중 사용 함.
            UInt32 AxisNum = axis;
            UInt32 Result;
            string strTemp;
            
            //if (DIO.GetDIData(FuncInline.enumDINames.X0_20_Ear_Loop_Fold_Guide_Forward) ||
            //    DIO.GetDIData(FuncInline.enumDINames.X0_22_Ear_Loop_Fold_Forward_0) ||
            //    DIO.GetDIData(FuncInline.enumDINames.X0_24_Ear_Loop_Fold_Forward_1) ||
            //    DIO.GetDIData(FuncInline.enumDINames.X0_26_Ear_Loop_Fold_Push_Forward) ||
            //    DIO.GetDIData(FuncInline.enumDINames.X0_61_Ear_Loop_Fusion_Down) ||
            //    !DIO.GetDIData(FuncInline.enumDINames.X0_09_Mask_Moving_Safety_Check))//57, 59도 해야 되나?
            //{
            //    FuncLog.WriteLog("Mask Move Can not. Cylinder Check.");
            //    Func.AddError(enumError.Mask_Moving_Cannot_Cylinder_Check);
            //    return ;
            //}

            if (m_bInit)
            {
                Servo_Set(AxisNum, VelL, VelH, ACC, DEC);
                //Servo_Set(AxisNum, VelL, VelH, ACC, DEC);

                Result = Motion.mAcm_AxMoveRel(m_Axishand[AxisNum], Convert.ToDouble(position));

                if (Result != (uint)ErrorCode.SUCCESS)
                {
                    strTemp = "PTP Move Failed With Error Code: [0x" + Convert.ToString(Result, 16) + "]";
                    //ShowMessages(strTemp, Result);
                    FuncLog.WriteLog("FuncAdvantech -" + "Servo_Move_Rel() -" + strTemp);
                }
            }
            return;
        }


        public static bool Servo_Move_A(uint axis, double position, uint speed)
        {
            UInt32 AxisNum = axis;

            UInt32 Result;
            string strTemp;
            double NewPos = new double();
            if (m_bInit)
            {
                Servo_Set(AxisNum, speed, speed * 30, speed * 30, speed * 30);
                //Servo_Set(AxisNum, VelL, VelH, ACC, DEC);

                NewPos = Convert.ToDouble(position);
                //This function will change the end position to specified position on current ptp motion
                //Result = Motion.mAcm_AxChangePos(m_Axishand[axis], NewPos);
                Result = Motion.mAcm_AxMoveAbs(m_Axishand[axis], position);
                if (Result != (uint)ErrorCode.SUCCESS)
                {
                    strTemp = "Change Position Failed With Error Code: [0x" + Convert.ToString(Result, 16) + "]";
                    //ShowMessages(strTemp, Result);
                    FuncLog.WriteLog("FuncAdvantech -" + "Servo_Move() -" + strTemp);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        #endregion




        #region Other(Message Box)
        private static void ShowMessages(string DetailMessage, uint errorCode)
        {
            StringBuilder ErrorMsg = new StringBuilder("", 100);
            //Get the error message according to error code returned from API
            Boolean res = Motion.mAcm_GetErrorMessage(errorCode, ErrorMsg, 100);
            string ErrorMessage = "";
            if (res)
                ErrorMessage = ErrorMsg.ToString();
            MessageBox.Show(DetailMessage + "\r\nError Message:" + ErrorMessage, "CMove", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion 


    }
}
