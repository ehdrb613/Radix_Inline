using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics; // Stopwatch
using System.Reflection;
using System.Drawing;
using System.IO;
using System.Collections.Concurrent;


using System.Data.SqlClient;
using System.IO.Ports;    // SerialPort 클래스 사용을 위해서 추가
using System.Net;
using System.Net.Sockets;

namespace Radix
{
    /**
   * @brief 프로젝트에 관련된 Type, 변수, 함수를 선언한다.
   *        Type도 공통부에서 지정할 수 있도록 하고,
   *        추가로 공통함수에서 사용할 수 있도록 링크함수를 이용해서
   *        가급적 공통함수를 이용할 수 있도록 한다.
   */

    // Invoke Hadle Leak 방지용 확장 메서드 
    public static class ExtensionMethods
    {
        public static void InvokeAndClose(this Control self, MethodInvoker func)
        {
            IAsyncResult result = self.BeginInvoke(func);
            self.EndInvoke(result);
            result.AsyncWaitHandle.Close();
        }
    }

    public class FuncInline
    {
     


        #region enum 정의
        public enum enumDINames
        {
            X00_0_Door_Open_Front_Left,
            X00_1_Door_Open_Front_Right,
            X00_2_Door_Open_Rear_Left,
            X00_3_Emergency_Stop,
            X00_4_,
            X00_5_Emergency_Stop_Limit,
            X00_6_SMEMA_Before_Pass,
            X00_7_SMEMA_After_AutoInline,
            X01_0_,
            X01_1_OP_Start,
            X01_2_,
            X01_3_OP_Stop,
            X01_4_,
            X01_5_OP_CYCLE_1_STOP,
            X01_6_,
            X01_7_OP_Reset,
            X02_0_Door_Open_Rear_Right,
            X02_1_SMEMA_Before_Ready,
            X02_2_SMEMA_After_Ready,
            X02_3_Out_Conveyor_PASSLIne_PCB_Start_Sensor,
            X02_4_Out_Conveyor_PASSLine_PCB_Stop_Sensor,
            X02_5_MARKING_SITE_OK_UP_SENSOR,
            X02_6_MARKING_SITE_OK_DOWN_SENSOR,
            X02_7_,
            X03_0_NgBuffer_ForwardSwitch,
            X03_1_NgBuffer_UpperForwardSensor,
            X03_2_NgBuffer_UpperBackwardSensor,
            X03_3_Front_FT_3_PCB_Dock_Sensor,
            X03_4_Front_Scan_PCB_Dock_Sensor,
            X03_5_NgBuffer_LowerForwardSensor,
            X03_6_NgBuffer_LowerBackwardSensor,
            X03_7_NgBuffer_PCB_Stop_Sensor,
            X04_0_Front_FT_2_PCB_Dock_Sensor,
            X04_1_Front_FT_1_PCB_Dock_Sensor,
            X04_2_Out_Shuttle_NG_PCB_Stop_Sensor,
            X04_3_Out_Conveyor_NG_PCB_Stop_Sensor,
            X04_4_Front_DT_10_FT_4_PCB_Dock_Sensor,
            X04_5_Front_DT_9_PCB_Dock_Sensor,
            X04_6_Front_DT_8_PCB_Dock_Sensor,
            X04_7_Front_DT_7_PCB_Dock_Sensor,
            X114_0_Front_DT_6_PCB_Dock_Sensor,
            X114_1_Front_DT_5_PCB_Dock_Sensor,
            X114_2_Front_DT_4_PCB_Dock_Sensor,
            X114_3_Front_DT_3_PCB_Dock_Sensor,
            X114_4_Front_DT_2_PCB_Dock_Sensor,
            X114_5_Front_DT_1_PCB_Dock_Sensor,
            X114_6_Front_PASSLINE_PCB_Stop_Sensor,
            X114_7_Front_Rack_PCB_Interlock_Sensor,
            X300_0_,
            X300_1_,
            X300_2_,
            X300_3_,
            X300_4_,
            X300_5_,
            X300_6_,
            X300_7_,
            X301_0_,
            X301_1_,
            X301_2_,
            X301_3_,
            X301_4_,
            X301_5_,
            X301_6_,
            X301_7_,
            X302_0_In_Shuttle_Pcb_In_Sensor,
            X302_1_In_Shuttle_Pcb_Stop_Sensor,
            X302_2_In_Shuttle_Stopper_Cyl_Up_Sensor,
            X302_3_Out_Shuttle_OK_PCB_In_Sensor,
            X302_4_Out_Shuttle_OK_PCB_Stop_Sensor,
            X302_5_Out_Shuttle_Stopper_Cyl_STOP_Sensor, //OK,NG라인 동시 동작
            X302_6_In_Shuttle_Turn_Cw_Cyl_Sensor,
            X302_7_In_Shuttle_Turn_Ccw_Cyl_Sensor,
            X303_0_In_Shuttle_Turn_Position_Interlock,
            X303_1_Out_Shuttle_Turn_Cw_Cyl_Sensor,
            X303_2_Out_Shuttle_Turn_Ccw_Cyl_Sensor,
            X303_3_Out_Shuttle_Turn_Position_Interlock,
            X303_4_In_Shuttle_Pcb_Interlock_Sensor,
            X303_5_Out_Shuttle_Stopper_Cyl_IN_Sensor,   //OK,NG라인 동시 동작
            X303_6_,
            X303_7_,
            X304_0_In_Shuttle_Interlock_Sensor,
            X304_1_Out_Shuttle_Ok_Interlock_Sensor,
            X304_2_Out_Shuttle_Ng_Interlock_Sensor,
            X304_3_In_Shuttle_Turn_Motor_Alarm,
            X304_4_Out_Shuttle_Turn_Motor_Alarm,
            X304_5_In_Shuttle_Turn_Motor_Home_Complete,
            X304_6_Out_Shuttle_Turn_Motor_Home_Complete,
            X304_7_In_Shuttle_Pcb_Temp_Sensor,
            X305_0_,
            X305_1_,
            X305_2_,
            X305_3_,
            X305_4_,
            X305_5_,
            X305_6_,
            X305_7_,
            X400_1_Front_FT_3_Contact_Up_Sensor,
            X400_3_Front_FT_2_Contact_Up_Sensor,
            X400_5_Front_FT_1_Contact_Up_Sensor,
            X403_0_Front_DT_10_FT_4_Contact_Up_Sensor,
            X402_7_Front_DT_9_Contact_Up_Sensor,
            X402_5_Front_DT_8_Contact_Up_Sensor,
            X402_3_Front_DT_7_Contact_Up_Sensor,
            X402_1_Front_DT_6_Contact_Up_Sensor,
            X401_6_Front_DT_5_Contact_Up_Sensor,
            X401_4_Front_DT_4_Contact_Up_Sensor,
            X401_2_Front_DT_3_Contact_Up_Sensor,
            X401_0_Front_DT_2_Contact_Up_Sensor,
            X400_7_Front_DT_1_Contact_Up_Sensor,
            X403_2_Front_Lift_Up_PCB_In_Sensor,
            X403_5_Front_Lift_Up_PCB_Stop_Sensor,
            X403_7_Front_Lift_Stopper_Cyl_Sensor,
            X400_0_Front_Lift_Down_PCB_In_Sensor,
            X400_2_Front_Lift_Down_PCB_Stop_Sensor,
            X114_,
            X115_,
            X116_,
            X401_3_Rear_FT_3_PCB_Dock_Sensor,
            X401_5_Rear_FT_2_PCB_Dock_Sensor,
            X401_7_Rear_FT_1_PCB_Dock_Sensor,
            X402_0_Out_Shuttle_Ng_PCB_In_Sensor,
            X402_2_Out_Conveyor_Ng_PCB_In_Sensor,
            X402_4_Rear_DT_10_FT_4_PCB_Dock_Sensor,
            X402_6_Rear_DT_9_PCB_Dock_Sensor,
            X403_1_Rear_DT_8_PCB_Dock_Sensor,
            X403_3_Rear_DT_7_PCB_Dock_Sensor,
            X403_4_Rear_DT_6_PCB_Dock_Sensor,
            X403_6_Rear_DT_5_PCB_Dock_Sensor,
            X404_1_Rear_DT_4_PCB_Dock_Sensor,
            X404_3_Rear_DT_3_PCB_Dock_Sensor,
            X404_5_Rear_DT_2_PCB_Dock_Sensor,
            X404_7_Rear_DT_1_PCB_Dock_Sensor,
            X405_0_Rear_Pass_OkLine_PCB_In_Sensor,
            X405_2_Rear_Rack_PCB_Interlock_Sensor,
            X405_4_Rear_FT_3_Contact_Up_Sensor,
            X405_6_Rear_FT_2_Contact_Up_Sensor,
            X406_1_Rear_FT_1_Contact_Up_Sensor,
            X404_4_Rear_DT_10_FT_4_Contact_Up_Sensor,
            X404_2_Rear_DT_9_Contact_Up_Sensor,
            X404_0_Rear_DT_8_Contact_Up_Sensor,
            X407_7_Rear_DT_7_Contact_Up_Sensor,
            X407_5_Rear_DT_6_Contact_Up_Sensor,
            X407_2_Rear_DT_5_Contact_Up_Sensor,
            X407_0_Rear_DT_4_Contact_Up_Sensor,
            X406_7_Rear_DT_3_Contact_Up_Sensor,
            X406_5_Rear_DT_2_Contact_Up_Sensor,
            X406_3_Rear_DT_1_Contact_Up_Sensor,
            X404_6_Rear_Lift_Up_PCB_In_Sensor,
            X405_1_Rear_Lift_Up_PCB_Stop_Sensor,
            X405_3_Rear_Lift_Stopper_Cyl_IN_UP_Sensor,
            X405_5_Rear_Lift_Down_PCB_In_Sensor,
            X405_7_Rear_Lift_Down_PCB_Stop_Sensor,
            X152_,
            X406_2_Rear_Lift_Stopper_Cyl_Out_UP_Sensor,
            X406_4_Rear_Pass_NgLine_PCB_Stop_Sensor,
            X155_,
            X156_,
            X157_,
            X158_,
            X159_,




        }
        public enum enumDONames
        {
            Y2_6_Front_FT_3_CONTACT_STOPPER_SOL,
            Y0_6_Front_FT_2_CONTACT_STOPPER_SOL,
            Y2_5_Front_FT_1_CONTACT_STOPPER_SOL,
            Y0_5_Front_DT_10_FT4_CONTACT_STOPPER_SOL,
            Y2_4_Front_DT_9_CONTACT_STOPPER_SOL,
            Y0_4_Front_DT_8_CONTACT_STOPPER_SOL,
            Y2_3_Front_DT_7_CONTACT_STOPPER_SOL,
            Y0_3_Front_DT_6_CONTACT_STOPPER_SOL,
            Y2_2_Front_DT_5_CONTACT_STOPPER_SOL,
            Y0_2_Front_DT_4_CONTACT_STOPPER_SOL,
            Y2_1_Front_DT_3_CONTACT_STOPPER_SOL,
            Y0_1_Front_DT_2_CONTACT_STOPPER_SOL,
            Y2_0_Front_DT_1_CONTACT_STOPPER_SOL,
            Y1_7_Front_PASSLINE_PCB_STOPPER_SOL,
            Y3_7_Front_SCAN_STOPPER_SOL,
            Y300_3,
            Y3_5_Front_FT_3_CONTACT_UP_DOWN_SOL,
            Y1_5_Front_FT_2_CONTACT_UP_DOWN_SOL,
            Y3_4_Front_FT_1_CONTACT_UP_DOWN_SOL,
            Y1_4_Front_DT_10_FT_4_CONTACT_UP_DOWN_SOL,
            Y3_3_Front_DT_9_CONTACT_UP_DOWN_SOL,
            Y1_3_Front_DT_8_CONTACT_UP_DOWN_SOL,
            Y3_2_Front_DT_7_CONTACT_UP_DOWN_SOL,
            Y1_2_Front_DT_6_CONTACT_UP_DOWN_SOL,
            Y3_1_Front_DT_5_CONTACT_UP_DOWN_SOL,
            Y1_1_Front_DT_4_CONTACT_UP_DOWN_SOL,
            Y3_0_Front_DT_3_CONTACT_UP_DOWN_SOL,
            Y1_0_Front_DT_2_CONTACT_UP_DOWN_SOL,
            Y2_7_Front_DT_1_CONTACT_UP_DOWN_SOL,
            Y0_7_CNC_ON_Lamp,
            Y4_6_OP_Reset_Lamp,
            Y3_6_NgBuffer_ForwardSwitch_Lamp,
            Y412_7_NgBuffer_Lower_cylinder_forward,
            Y4_7_Ngbuffer_Lower_cylinder_backward,
            Y412_6_Ngbuffer_Upper_cylinder_forward,
            Y412_5_Ngbuffer_Upper_cylinder_backward,
            Y1_6_Rear_OK_PassLine_CONTACT_STOPPER_SOL,
            Y4_5_Rear_NG_PassLine_CONTACT_STOPPER_SOL,
            Y412_4,
            Y4_4_Tower_Lamp_Red,
            Y412_3_Tower_Lamp_Yellow,
            Y4_3_Tower_Lamp_Green,
            Y412_2_Tower_Lamp_Buzzer,
            Y4_2_SMEMA_Before_Ready,
            Y412_1_SMEMA_After_Ready,
            Y4_1_Emergency_Stop_Lamp,
            Y412_0_IN_Shuttle_Turn_CCW_Cylinder,
            Y4_0_IN_Shuttle_Turn_CW_Cylinder,
            Y0_0_Rear_FT_3_CONTACT_UP_DOWN_SOL,
            Y303_7_Rear_FT_2_CONTACT_UP_DOWN_SOL,
            Y301_7_Rear_FT_1_CONTACT_UP_DOWN_SOL,
            Y303_6_Rear_DT_10_FT_3_CONTACT_UP_DOWN_SOL,
            Y301_6_Rear_DT_9_CONTACT_UP_DOWN_SOL,
            Y303_5_Rear_DT_8_CONTACT_UP_DOWN_SOL,
            Y301_5_Rear_DT_7_CONTACT_UP_DOWN_SOL,
            Y303_4_Rear_DT_6_CONTACT_UP_DOWN_SOL,
            Y301_4_Rear_DT_5_CONTACT_UP_DOWN_SOL,
            Y303_3_Rear_DT_4_CONTACT_UP_DOWN_SOL,
            Y301_3_Rear_DT_3_CONTACT_UP_DOWN_SOL,
            Y303_2_Rear_DT_2_CONTACT_UP_DOWN_SOL,
            Y301_2_Rear_DT_1_CONTACT_UP_DOWN_SOL,
            Y302_2_IN_Shuttle_CONTACT_STOPPER_SOL,
            Y300_2_Out_Shuttle_CONTACT_STOPPER_IN_SOL,
            Y302_1_Out_Shuttle_CONTACT_STOPPER_Out_SOL,
            Y303_1_Rear_FT_3_CONTACT_STOPPER_SOL,
            Y301_1_Rear_FT_2_CONTACT_STOPPER_SOL,
            Y303_0_Rear_FT_1_CONTACT_STOPPER_SOL,
            Y301_0_Rear_DT_10_FT_4_CONTACT_STOPPER_SOL,
            Y302_7_Rear_DT_9_CONTACT_STOPPER_SOL,
            Y300_7_Rear_DT_8_CONTACT_STOPPER_SOL,
            Y302_6_Rear_DT_7_CONTACT_STOPPER_SOL,
            Y300_6_Rear_DT_6_CONTACT_STOPPER_SOL,
            Y302_5_Rear_DT_5_CONTACT_STOPPER_SOL,
            Y300_5_Rear_DT_4_CONTACT_STOPPER_SOL,
            Y302_4_Rear_DT_3_CONTACT_STOPPER_SOL,
            Y300_4_Rear_DT_2_CONTACT_STOPPER_SOL,
            Y302_3_Rear_DT_1_CONTACT_STOPPER_SOL,
            Y300_1_Front_Lift_CONTACT_STOPPER_SOL,
            Y302_0_Rear_Lift_CONTACT_STOPPER_IN_SOL,
            Y300_0_Rear_Lift_CONTACT_STOPPER_Out_SOL,
            Y305_7_Rear_DT_3_Motor_Cw,
            Y304_7_Rear_DT_3_Motor_Ccw,
            Y305_6_Rear_DT_2_Motor_Cw,
            Y304_6_Rear_DT_2_Motor_Ccw,
            Y305_5_Rear_DT_1_Motor_Cw,
            Y304_5_Rear_DT_1_Motor_Ccw,
            Y305_4_Rear_PassLine_Motor_Cw,
            Y87_,
            Y304_4_Out_Shuttle_Turn_Ccw_Cylinder,
            Y304_3_Out_Shuttle_Turn_Cw_Cylinder,
            Y305_2_Rear_Lift_Up_Motor_Cw,
            Y304_2_Rear_Lift_Up_Motor_Ccw,
            Y305_1_Rear_Lift_Down_Motor_Cw,
            Y304_1_Rear_Lift_Down_Motor_Ccw,
            Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw,
            Y304_0_Out_Shuttle_Ok_Motor_Cw,
            Y400_1_Out_Conveyor_Motor_Cw,
            Y400_3_In_Shuttle_Motor_Ccw,
            Y400_5_Out_Shuttle_Ng_Motor_Cw,
            Y400_7_Out_Shuttle_Ng_Motor_Ccw,
            Y401_0_Front_FT_3_Motor_Cw,
            Y401_2_Front_FT_3_Motor_Ccw,
            Y401_4_Front_FT_2_Motor_Cw,
            Y401_6_Front_FT_2_Motor_Ccw,
            Y402_1_Front_FT_1_Motor_Cw,
            Y402_3_Front_FT_1_Motor_Ccw,
            Y402_5_Out_Conveyor_Ng_Motor_Cw,
            Y402_7_Out_Shuttle_Ok_Motor_Ccw,
            Y403_0_Front_DT_10_FT4_Motor_Cw,
            Y403_2_Front_DT_10_FT4_Motor_Ccw,
            Y403_5_Front_DT_9_Motor_Cw,
            Y403_7_Front_DT_9_Motor_Ccw,
            Y400_0_Front_DT_8_Motor_Cw,
            Y400_2_Front_DT_8_Motor_Ccw,
            Y400_4_Front_DT_7_Motor_Cw,
            Y400_6_Front_DT_7_Motor_Ccw,
            Y401_1_Front_DT_6_Motor_Cw,
            Y401_3_Front_DT_6_Motor_Ccw,
            Y401_5_Front_DT_5_Motor_Cw,
            Y401_7_Front_DT_5_Motor_Ccw,
            Y402_0_Front_DT_4_Motor_Cw,
            Y402_2_Front_DT_4_Motor_Ccw,
            Y402_4_Front_DT_3_Motor_Cw,
            Y402_6_Front_DT_3_Motor_Ccw,
            Y403_1_Front_DT_2_Motor_Cw,
            Y403_3_Front_DT_2_Motor_Ccw,
            Y403_4_Front_DT_1_Motor_Cw,
            Y403_6_Front_DT_1_Motor_Ccw,
            Y404_1_Front_Passline_Motor_Cw,
            Y404_3_SMEMA_Before_AutoInline,
            Y404_5_SMEMA_After_Pass,
            Y404_7_,
            Y405_0_Front_Lift_Up_Motor_Cw,
            Y405_2_Front_Lift_Up_Motor_Ccw,
            Y405_4_Front_Lift_Down_Motor_Cw,
            Y405_6_Front_Lift_Down_Motor_Ccw,
            Y406_1_Front_SCAN_Motor_Cw,
            Y406_3_Front_SCAN_Motor_Ccw,
            Y406_5_Rear_FT_3_Motor_Cw,
            Y406_7_Rear_FT_3_Motor_Ccw,
            Y407_0_Rear_FT_2_Motor_Cw,
            Y407_2_Rear_FT_2_Motor_Ccw,
            Y407_5_Rear_FT_1_Motor_Cw,
            Y407_7_Rear_FT_1_Motor_Ccw,
            Y404_0_Rear_NgLine_Motor_Cw,
            Y404_2_,
            Y404_4_Rear_DT_10_FT_4_Motor_Cw,
            Y404_6_Rear_DT_10_FT_4_Motor_Ccw,
            Y405_1_Rear_DT_9_Motor_Cw,
            Y405_3_Rear_DT_9_Motor_Ccw,
            Y405_5_Rear_DT_8_Motor_Cw,
            Y405_7_Rear_DT_8_Motor_Ccw,
            Y406_0_Rear_DT_7_Motor_Cw,
            Y406_2_Rear_DT_7_Motor_Ccw,
            Y406_4_Rear_DT_6_Motor_Cw,
            Y406_6_Rear_DT_6_Motor_Ccw,
            Y407_1_Rear_DT_5_Motor_Cw,
            Y407_3_Rear_DT_5_Motor_Ccw,
            Y407_4_Rear_DT_4_Motor_Cw,
            Y407_6_Rear_DT_4_Motor_Ccw,


        }
        public enum enumServoAxis
        {
            SV00_In_Shuttle,
            SV01_Out_Shuttle,
            SV02_Lift1,
            SV03_Rack1_Width,
            SV04_Lift2,
            SV05_Rack2_Width,
            SV06_Scan_X,
            SV07_Scan_Y
        }
        /** @brief 에러 발생 파트  */
        public enum enumErrorPart// 에러 발생 파트
        {
            No_Error,
            System, // 안전 관련
            InConveyor,
            InShuttle,
            NgBuffer,   //아웃컨베이어 하단   
            OutShuttle_Up,
            OutShuttle_Down,
            OutConveyor,
            FrontPassLine,
            RearPassLine,
            RearNGLine,
            Lift1_Up,
            Lift1_Down,
            Lift2_Up,
            Lift2_Down,
            FrontScanSite,
            Site1_F_DT1,
            Site2_F_DT2,
            Site3_F_DT3,
            Site4_F_DT4,
            Site5_F_DT5,
            Site6_F_DT6,
            Site7_F_DT7,
            Site8_F_DT8,
            Site9_F_DT9,
            Site10_F_DT10_FT4,  // 5세대 FT4
            Site11_F_FT1,
            Site12_F_FT2,
            Site13_F_FT3,
            Site14_R_DT1,
            Site15_R_DT2,
            Site16_R_DT3,
            Site17_R_DT4,
            Site18_R_DT5,
            Site19_R_DT6,
            Site20_R_DT7,
            Site21_R_DT8,
            Site22_R_DT9,
            Site23_R_DT10_FT4,
            Site24_R_FT1,
            Site25_R_FT2,
            Site26_R_FT3
        }

        public enum enumInlineType // 구형인라인 세대별 타입설정
        {
            Gen1, //１세대
            Gen2, // ２세대
            Gen3, // ３세대
            Gen4, // ４세대
            Gen5, // 5세대
            Gen6    //6세대(통합모드일때)
        }

        public enum enumScanPos //스캔 티칭 위치
        {
            FArray1,
            FArray2,
            FArray3,
            FArray4,
            FArray5,
            FArray6,
            FArray7,
            FArray8,
            FArray9,
            FArray10,
            FArray11,
            FArray12,
            RArray1,
            RArray2,
            RArray3,
            RArray4,
            RArray5,
            RArray6,
            RArray7,
            RArray8,
            RArray9,
            RArray10,
            RArray11,
            RArray12
        }
        public enum enumTeachingPos // 티칭할 포인트들 이름
        {
            None,
            InConveyor,
            InShuttle,
            NgBuffer,   //아웃컨베이어 하단   
            OutShuttle_Up,
            OutShuttle_Down,
            OutConveyor,
            FrontPassLine,
            RearPassLine,
            RearNGLine,
            Lift1_Up,
            Lift1_Down,
            Lift2_Up,
            Lift2_Down,
            FrontScanSite,
            Site1_F_DT1,
            Site2_F_DT2,
            Site3_F_DT3,
            Site4_F_DT4,
            Site5_F_DT5,
            Site6_F_DT6,
            Site7_F_DT7,
            Site8_F_DT8,
            Site9_F_DT9,
            Site10_F_DT10_FT4,  // 5세대 FT4
            Site11_F_FT1,
            Site12_F_FT2,
            Site13_F_FT3,
            Site14_R_DT1,
            Site15_R_DT2,
            Site16_R_DT3,
            Site17_R_DT4,
            Site18_R_DT5,
            Site19_R_DT6,
            Site20_R_DT7,
            Site21_R_DT8,
            Site22_R_DT9,
            Site23_R_DT10_FT4,
            Site24_R_FT1,
            Site25_R_FT2,
            Site26_R_FT3
        }
        /** @brief 에러값 구분  */
        public enum enumErrorCode // 에러값 구분
        {
            #region 0~99 안전 시스템 관련
            No_Error,
            E_Stop,
            Door_Opened,
            Light_Curtain_Checked,
            Loto_Off,
            Fatal_System_Error,
            #endregion

            #region 100~199 시스템 정지해야 할 에러. 시스템 전반으로 중요도 높은 것.
            System_Init_Fail = 100,
            Vision_Connect_Fail,

            Robot_Not_Ext, // 로봇 외부모드 아님
            Robot_Not_Inited, // 로봇 초기화 되지 않음
            Robot_Init_Fail, // 로봇 초기화 실패
            Robot_Error, // 로봇 에러
            Robot_Run_Error, // 로봇 동작 에러
            Robot_Vacuum_Error, // 로봇 진공 에러
            Robot_PCB_Check_Error, // 로봇 PCB감지 에러
            Robot_Unknown_Error, // 확인 안 된 상황에서 일정시간 동작 안 하고 있을 때

            Robot_InterLock, // 로봇 동작중 충돌 위험 발생
            Servo_Axis_InterLock, // 로봇 이외 서보모터 이동중 충돌 위험

            #endregion

            #region 장비별 에러는 200~799 사이에서 정의
            #region 200~ 공통부
            Robot_Move_Timeout = 200, // 직교로봇 제품 이동 타임아웃
            Scan_Timeout, // 스캔 시간경과
            Scan_Fail, //
            Scan_Bad_Input, //
            Barcode_Scan_Duplicated,
            XOut_Scan_Mismatch,
            Scan_No_Test_Array, // PCB에 검사할 어레이가 없음
            Conveyor_Timeout, // 컨베이어 이송 시도 중 제품 도착 확인 안 됨
            Conveyor_Width_Mismatch,
            PCB_MissRun,
            PCB_OverRun,
            Axis_Disabled,
            PCB_Temperature_Over,
            PCB_Detect_Fail,
            PCB_Info_Not_Exist,
            PCB_Info_Move_Fail,
            Site_Open_Timeout,
            Site_Close_Timeout,
            Site_PCB_Load_Timeout,
            Site_Power_Off,
            Site_Unuse_Error,
            Site_Module_Insert_Check,
            NG_Buffer_Full,
            Width_Move_Timeout,
            Linear_Communication_Fail, // 직렬 연결시 뒷장비로 보내는 중 SmemaAuto 꺼졌을 경우
            CN_Duplicated,
            CN_Cross_Detected,
            Module_Clamp_Check,
            Site_Clamp_Forward_Error,
            Site_Clamp_Backward_Error,
            Pin_Clamp_Down_Error,
            Pin_Clamp_Up_Error,
            Stopper_Down_Error,
            Stopper_Up_Error,
            Site_Clamp_Sensor_Error,
            Pin_Clamp_Sensor_Error,
            Sol_Error,
            #endregion

            #region 300~ 테스트 통신 관련
            Test_Command_Timeout = 300, // 테스트로 테스트 시작 지령 응답 없음
            Test_Response_Timeout, // 테스트 결과 리턴 응답 없음
            Test_All_Fail, // 모든 테스트 결과 실패임
            #endregion

            #region 400~ 나머지 장치들 각 장치별로 세팅할 부분
            #endregion

            #region 600~ 운영 관련
            System_Not_Inited = 600, // 초기화 되지 않은 상태로 운영시도
            Robot_Jig_Vac_Check, // 예상되는 진공상태와 다름
            #endregion

            #region 700~ 세팅 관련
            Value_Range_Over = 700, // 세팅값이 유효범위 벗어남
            #endregion
            #endregion

            #region 800~ 사용자 호출 필요한 경우, 시스템은 정상 운영, 상황 해제시 자동 해제 --> 모든 알람은 무조건 정지로 수정됨
            Run_Stopped = 800, // 확인되지 않은 상태로 정지된 경우
            Operator_Call, // 사용자 호출
            #endregion

            #region 900~ 이외
            Digital_Input_Check = 900, // 센서 동작 확인
            Digital_Output_Check // 디지털 출력 확인
            #endregion
        }

        public enum enumCoCode
        {
            SEVT,
            SEV,
            SIEL,
            SEIN,
            SEDA_M,
            SEDA_C,
            SESC
        }

        public enum enumCustCode
        {
            M,  // - A015M
            G, // - A015G
            F, // - A015F
            V, // - A015V
            T, // - A015T1
            W, // - A015W
            A, // - A015A
            D, // - SM-S111DL
            Z, // - A015AZ
            R // - A015R4
        }

        public enum enumMoveAction // PCB 이송 동작 구분. Queue로 관리. 종속성(독립실행) 관련은 고민해 봐야함(아마도 쓰레드별로 따로 관리하면 될 듯). 사이트번호를 따로 할지, 한번에 늘어 놓을지...
        {
            Waiting, // 동작 없음
            BeforeToInputConveyor,
            InputConveyorToPass1,
            Pass1ToLift1Up,
            Lift1UpToPass2,
            Lift1DownToPass2,
            Pass2ToLift2Up,
            Pass2ToLift2Down,
            Lift2UpToOutput,
            Lift2DownToOutput,
            Lift2DownToNG,
            Lift1UpToSite1,
            Lift1UpToSite2,
            Lift1UpToSite3,
            Lift1UpToSite4,
            Lift1UpToSite5,
            Lift1UpToSite6,
            Lift1UpToSite7,
            Lift1UpToSite8,
            Lift1UpToSite9,
            Lift1UpToSite10,
            Lift1UpToSite11,
            Lift1UpToSite12,
            Lift1UpToSite13,
            Lift1UpToSite14,
            Lift1UpToSite15,
            Lift1UpToSite16,
            Lift1UpToSite17,
            Lift1UpToSite18,
            Lift1UpToSite19,
            Lift1UpToSite20,
            Lift1DownToSite1,
            Lift1DownToSite2,
            Lift1DownToSite3,
            Lift1DownToSite4,
            Lift1DownToSite5,
            Lift1DownToSite6,
            Lift1DownToSite7,
            Lift1DownToSite8,
            Lift1DownToSite9,
            Lift1DownToSite10,
            Lift1DownToSite11,
            Lift1DownToSite12,
            Lift1DownToSite13,
            Lift1DownToSite14,
            Lift1DownToSite15,
            Lift1DownToSite16,
            Lift1DownToSite17,
            Lift1DownToSite18,
            Lift1DownToSite19,
            Lift1DownToSite20,
            Lift2UpToSite21,
            Lift2UpToSite22,
            Lift2UpToSite23,
            Lift2UpToSite24,
            Lift2UpToSite25,
            Lift2UpToSite26,
            Lift2UpToSite27,
            Lift2UpToSite28,
            Lift2UpToSite29,
            Lift2UpToSite30,
            Lift2UpToSite31,
            Lift2UpToSite32,
            Lift2UpToSite33,
            Lift2UpToSite34,
            Lift2UpToSite35,
            Lift2UpToSite36,
            Lift2UpToSite37,
            Lift2UpToSite38,
            Lift2UpToSite39,
            Lift2UpToSite40,
            Lift2DownToSite21,
            Lift2DownToSite22,
            Lift2DownToSite23,
            Lift2DownToSite24,
            Lift2DownToSite25,
            Lift2DownToSite26,
            Lift2DownToSite27,
            Lift2DownToSite28,
            Lift2DownToSite29,
            Lift2DownToSite30,
            Lift2DownToSite31,
            Lift2DownToSite32,
            Lift2DownToSite33,
            Lift2DownToSite34,
            Lift2DownToSite35,
            Lift2DownToSite36,
            Lift2DownToSite37,
            Lift2DownToSite38,
            Lift2DownToSite39,
            Lift2DownToSite40,
            Site1ToLift1Down,
            Site2ToLift1Down,
            Site3ToLift1Down,
            Site4ToLift1Down,
            Site5ToLift1Down,
            Site6ToLift1Down,
            Site7ToLift1Down,
            Site8ToLift1Down,
            Site9ToLift1Down,
            Site10ToLift1Down,
            Site11ToLift1Down,
            Site12ToLift1Down,
            Site13ToLift1Down,
            Site14ToLift1Down,
            Site15ToLift1Down,
            Site16ToLift1Down,
            Site17ToLift1Down,
            Site18ToLift1Down,
            Site19ToLift1Down,
            Site20ToLift1Down,
            Site21ToLift2Down,
            Site22ToLift2Down,
            Site23ToLift2Down,
            Site24ToLift2Down,
            Site25ToLift2Down,
            Site26ToLift2Down,
            Site27ToLift2Down,
            Site28ToLift2Down,
            Site29ToLift2Down,
            Site30ToLift2Down,
            Site31ToLift2Down,
            Site32ToLift2Down,
            Site33ToLift2Down,
            Site34ToLift2Down,
            Site35ToLift2Down,
            Site36ToLift2Down,
            Site37ToLift2Down,
            Site38ToLift2Down,
            Site39ToLift2Down,
            Site40ToLift2Down
        }

        public enum enumSMDStatus // SMD 테스트 상태. PCB 상태만 지정
        {
            UnKnown, // 비어 있거나 확인 안 됨.
            Before_Scan, // 스캔 전이다
            Cooling, // 쿨링중이다
            No_Test, // 테스트를 사용하지 않는다.
            Before_Command, // PCB 안착했고 지령은 안 날렸다
            Command_Sent, // 지령 날렸고 응답 대기중
            Response_OK, // 지령 수신 정상. Testing으로 바로 간다
            Response_NG, // 지령 수신 실패. 재송신 필요
            Test_Cancel, // 테스트 취소
            Test_Timeout, // 타임아웃
            User_Cancel, // 사용자 취소
            ReTest, // 재검사 대기
            Testing, // 테스트중
            Test_Pass, // 양품 판정(통합)
            Test_Fail, // 비품 판정(통합)
            //==========================
            //D1,D2지그 DL,FT 분리형 전용
            DTest_Pass, // 양품 판정
            DTest_Fail, // 비품 판정
            FTest_Pass, // 양품 판정
            FTest_Fail, // 비품 판정
            //==========================

        }

        //사용 안함
        public enum enumETCPos // 기타 티칭 위치 저장 목록
        {
            InShuttleForwardPickup,
            InShuttleForwardOut,
            InShuttleBackwardIn,
            InShuttleBackwardOut,
            InShuttleScan,
            OutShuttleForwardPickup,
            OutShuttleForwardIn,
            OutShuttleBackwardIn,
            OutShuttleBackwardOut,
            NgShuttleForward,
            NgShuttleBackward,
            InPickupUp,
            InPickupDown,
            NgPickupUp,
            NgPickupDown,
            NgPickupForward,
            NgPickupBackward,
            OutPickupUp,
            OutPickupDown,
            InInverterRotate,
            InInverterReturn,
            OutInverterRotate,
            OutInverterReturn,
            JigBufferTop
        }

        public enum enumLiftName // 리프트 이름
        {
            FrontLift,
            RearLift
        }

        public enum enumLiftPos // 리프트 위치. Lift2 경우도 site1부터 할당
        {
            UnKnown = 0, // 표시를 위해 추가. 이 걸로 오류가 생기면 막을 것
            FrontPassLine, // Pass라인과 상단 일치
            FrontScanPos,    //Front스캔위치
            // ===== Front DT (1~10 / FT4) : UP =====
            Site1_F_DT1_Up = 3,
            Site2_F_DT2_Up,
            Site3_F_DT3_Up,
            Site4_F_DT4_Up,
            Site5_F_DT5_Up,
            Site6_F_DT6_Up,
            Site7_F_DT7_Up,
            Site8_F_DT8_Up,
            Site9_F_DT9_Up,
            Site10_F_DT10_FT4_Up,

            // ===== Front FT (1~3) : UP =====
            Site11_F_FT1_Up,
            Site12_F_FT2_Up,
            Site13_F_FT3_Up,

            // ===== Rear DT (1~10 / FT4) : UP =====
            Site14_R_DT1_Up,
            Site15_R_DT2_Up,
            Site16_R_DT3_Up,
            Site17_R_DT4_Up,
            Site18_R_DT5_Up,
            Site19_R_DT6_Up,
            Site20_R_DT7_Up,
            Site21_R_DT8_Up,
            Site22_R_DT9_Up,
            Site23_R_DT10_FT4_Up,

            // ===== Rear FT (1~3) : UP =====
            Site24_R_FT1_Up,
            Site25_R_FT2_Up,
            Site26_R_FT3_Up,

            OutShuttleUp = 29,
            OutShuttleDown = 30,
            RearPassLine, // Pass라인과 상당 일치
            RearNGLine, // NG라인과 상당 일치

            RearScanPos,    //Rear스캔위치

            // ===== Front DT (1~10 / FT4) : DOWN =====
            Site1_F_DT1_Down,
            Site2_F_DT2_Down,
            Site3_F_DT3_Down,
            Site4_F_DT4_Down,
            Site5_F_DT5_Down,
            Site6_F_DT6_Down,
            Site7_F_DT7_Down,
            Site8_F_DT8_Down,
            Site9_F_DT9_Down,
            Site10_F_DT10_FT4_Down,

            // ===== Front FT (1~3) : DOWN =====
            Site11_F_FT1_Down,
            Site12_F_FT2_Down,
            Site13_F_FT3_Down,

            // ===== Rear DT (1~10 / FT4) : DOWN =====
            Site14_R_DT1_Down,
            Site15_R_DT2_Down,
            Site16_R_DT3_Down,
            Site17_R_DT4_Down,
            Site18_R_DT5_Down,
            Site19_R_DT6_Down,
            Site20_R_DT7_Down,
            Site21_R_DT8_Down,
            Site22_R_DT9_Down,
            Site23_R_DT10_FT4_Down,

            // ===== Rear FT (1~3) : DOWN =====
            Site24_R_FT1_Down,
            Site25_R_FT2_Down,
            Site26_R_FT3_Down

        }

        public enum enumShuttleName
        {
            InShuttle,
            OutShuttle
        }

        public enum enumShuttlePos
        {
            UnKnown,
            InShuttle_InConveyorLoading,// 인컨베이어 투입
            InShuttle_FrontRackUnLoading,// 인셔틀 -> Front PassLine으로 배출
            InShuttle_TurnPosition,     //인셔틀 턴포지션
            InShuttle_RearLiftUnLoading,// 인셔틀 ->  RearLift로 배출
            OutShuttle_FrontLiftLoading, // Front Lift -> 아웃셔틀
            OutShuttle_TurnPosition,     //아웃셔틀 턴포지션
            OutShuttle_RearRackLoading, // RearRack->아웃셔틀
            OutShuttle_OutCovyUnLoading //아웃셔틀 -> 아웃컨베이어
        }

        public enum enumTestType // 테스트 방법 구분
        {
            DownloadOnly,
            FunctionOnly,
            All
        }

        public enum enumSiteAction // 사이트 자체 동작 구분
        {
            NotUse, // 사용 안 함
            Waiting, // 비어 있음. 닫혀 있는 상태
            Loading, // PCB 투입중. 리프트와 연계작업중
            Testing, // 테스트중
            Tested, // 테스트 완료
            ReOpen, // 자체 재테스트용 핀 올리기
            ReClose // 자체 재테스트용 핀 내리기
        }

        public enum enumInitialize // 초기화시 완료 확인 파트
        {
            InConveyor = 0,
            InShuttle,
            NgBuffer,   //아웃컨베이어 하단   
            OutShuttle,
            OutConveyor,
            FrontLift,
            Lift2,
            Scan,
            Site1_F_DT1,
            Site2_F_DT2,
            Site3_F_DT3,
            Site4_F_DT4,
            Site5_F_DT5,
            Site6_F_DT6,
            Site7_F_DT7,
            Site8_F_DT8,
            Site9_F_DT9,
            Site10_F_DT10_FT4,  // 5세대 FT4
            Site11_F_FT1,
            Site12_F_FT2,
            Site13_F_FT3,
            Site14_R_DT1,
            Site15_R_DT2,
            Site16_R_DT3,
            Site17_R_DT4,
            Site18_R_DT5,
            Site19_R_DT6,
            Site20_R_DT7,
            Site21_R_DT8,
            Site22_R_DT9,
            Site23_R_DT10_FT4,
            Site24_R_FT1,
            Site25_R_FT2,
            Site26_R_FT3
        }

        public enum enumMachineLinear // 직렬 연결 여부
        {
            StandAlone,
            LinearFirst,
            LinearSecond,
            LinearAuto
        }
        public enum enumBuyerChange
        {
            White, // 일반
            Yellow, // 투입막고 모든 PCB 배출되면 종료
            Blue, // 이전 PCB 모두 배출하고, 다음 PCB 투입후 테스트 안 함
            Orange  // 렉별로 한 개 사이트씩만 테스트 진행
        }

        public enum enumPMCMotion // Lets보드 모션 컨트롤러 순번
        {

            In_OutShuttle_Width,      //IP20
            Out_InConveyor_Width,     //IP21
            NGbuffer_Width            //IP22
        }

        public enum enumPMCAxis // Autonics PMC Motion 컨트롤러에 연결된 축 이름, 짝수면 x, 홀수면 y
        {
            ST00_InShuttle_Width,
            ST01_OutShuttle_Width,
            ST02_OutConveyor_Width,
            ST03_InConveyor_Width,
            ST04_NGBuffer
        }
        public enum enumPMCCommand
        {
            None,
            Connect,
            Disconnect,
            Reconnect,
            Home,
            AbsMove,
            IncMove,
            ContMove,
            EmgStop,
            Stop,
            Reset,
            SetSpeed,
            GetPosition,
            Step2Dir,
            ClearPos
        }

        public enum enumTabMachine // Machine 화면의 탭 구분
        {
            Machine,
            PinBlock,
            TestSite,
            SerialSet,
            TowerLamp,
            IoMonitor
        }

        public enum enumTabTeaching // Teaching 화면의 탭 구분
        {
            LiftPosition,
            Width,
            ETC,
            Scan
        }

        public enum enumTabManual // Manual 화면의 탭 구분
        {
            Lift,
            Site,
            Conveyor,
            Vision
        }

        public enum enumTabMain // 메인 탭의 목록
        {
            Auto = 0,
            Manual = 1,
            IO = 2,
            Model = 3,
            Defect = 4,
            PartClear = 5,
            Trace = 6,
            Errors = 7,
            Scan = 8,
            Machine = 9,
            Teaching = 10,
            Com = 11,
            Origin = 12
        }

        public enum enumLoading // 프로그램 로딩시 완료 확인 파트
        {
            WindowForms,
            MasterKernel,
            GlobalSettings,
            SerialPortConnection,
            SubForms,
            MotorInitialization,
            DeviceConnections
        }

        public enum enumLiftAction // 리프트 동작 구분
        {
            Waiting, // 해당 동작 없음, 감지된 것 없음
            Input, // 일반 장비 투입
            InputUp, // 리프트 상단으로 PCB 투입
            InputDown, // 리프트 하단으로 PCB 투입
            InputNG, // NG 컨베어 수령
            LoadingUp, // 리프트 상단 이용 사이트로 투입
            LoadingDown, // 리프트 하단 이용 사이트로 이동
            UnloadingUp, // 리프트 상단 이용 사이트에서 배출
            UnloadingDown, // 리프트 하단 이용 사이트에서 배출
            Output, // 일반 장비 배출
            OutShuttleUP, // 아웃 셔틀 상단 이용 컨베이어 배출
            OutShuttleDown, // 아웃 셔틀 하단 이용 뒷 장치로 배출
            OutputNG, // 리프트 하단 이용 NG로 배출
            Scan, // 스캔 실행
            Cooling, // PCB 쿨링
            Evade // 회피 위치 이동
        }
        public enum enumShuttleAction // 리프트 동작 구분
        {
            Waiting, // 해당 동작 없음, 감지된 것 없음
            Input, // 일반 장비 투입
            UnLoadingPassLine, // 인셔틀 -> Front PassLine으로 배출
            UnLoadingRearLift, // 인셔틀 ->  RearLift로 배출
            MoveTurnPos, // 셔틀 턴위치로 이동
            LoadingFrontLift, // Front Lift -> 아웃셔틀
            LoadingRearRack, // RearRack->아웃셔틀
            UnLoadingOutCovy,//아웃셔틀 -> 아웃컨베이어
            Cooling, // PCB 쿨링
            Evade // 회피 위치 이동
        }

        public enum enumNGType // Fail 처리시 종류. 배출 방법이 서로 다르다. Fail 판정시 PCBStatus 에다가 기록해 둔다.
        {
            OK,
            NormalFail,
            XOut,
            BadMark,
            TestCancel,
            Timeout,
            Unknown
        }

        public enum enumNGAction // NG 컨베어 동작 구분
        {
            Forward,
            Backward,
            Run,
            Stop
        }

        public enum enumConvAction // 컨베어 동작 구분
        {
            Forward,
            Backward,
            Stop
        }

        public enum enumSiteState // 사이트 연결 관련 상태
        {
            Valid, //정상
            NotUse, // 미사용
            PowerOff, // 전원 오프
            DisCon // 연결 안 됨
        }

        #endregion

      

        #region struct 정의 
        /** @brief 에러 추가 또는 조회시 사용하는 에러 정보 */
        public struct structError // 에러 추가 또는 조회시
        {
            public string Date;
            public string Time;
            public enumErrorPart ErrorPart;
            public enumErrorCode ErrorCode;
            public bool Cure;
            public string Description;

            public structError(string date, string time, enumErrorPart part, enumErrorCode code, bool cure, string desc)
            {
                Date = date;
                Time = time;
                ErrorPart = part;
                ErrorCode = code;
                Cure = cure;
                Description = desc;
            }
        }

        public struct structPMCStatus // PMC 모션 상태 조회시
        {
            public bool Connected;
            public double Position;
            public int Velocity;
            public bool Home;
            public bool POT;
            public bool NOT;
            public bool Errored;
            public int ErrorSt;
            public bool Homing;
            public bool isHomed; // 홈찾기 완료 후 계속 유지
            public bool StandStill;
        }

        public struct structPMCCommand // PMC Multi Thread 위해 Queue에 넣을 지령 구조체
        {
            public enumPMCMotion Motion;
            public enumAxis Axis;
            public enumPMCCommand Command;
            public double Position;
            public int Velocity;
            public int Acc;
            public int Dec;
            public int Jerk;
            public int Port;
            public int Node;
            public int BaudRate;
        }


        #endregion

        #region Servo class 정의 

        public class _ServoParam
        {
            public class _sv0  //샌딩 전 리프트
            {
                public double speed;
                public double init_1Stpos;  //1층 위치 Homeoffset으로 바로 이동
                public double Input_pos;  //작업자 트레이 투입위치
                public double step; // 한스텝 이동 거리

                public _sv0()
                {
                    speed = 0;
                    init_1Stpos = 0; // 기본값 0 설정
                    Input_pos = 0;
                    step = 0;
                }
            }

            public class _sv1  //샌딩 후 리프트
            {
                public double speed;
                public double init_1Stpos;  //1층 위치 Homeoffset으로 바로 이동
                public double Output_pos;  //작업자 트레이 배출위치
                public double step; // 한스텝 이동 거리

                public _sv1()
                {
                    speed = 0;
                    init_1Stpos = 0; // 기본값 0 설정
                    step = 0;
                }
            }





            // 메인 클래스 속성들
            public _sv0 sv0_BeforeLift = new _sv0();          //샌딩 전 리프트 속성
            public _sv1 sv1_AfterLift = new _sv1();         //샌딩 후 리프트 속성



        }
        #endregion
        #region Servo Parameter
        // Static 변수로 ServoParamAll 선언 및 초기화
        public static _ServoParam ServoParamAll = new _ServoParam();
        //public static _ServoParam ServoParamAll = new _ServoParam();

        #endregion

        #region Site 맵핑
        public static class SiteIoMaps
        {
            // === 1) 컨택트 UP 센서 (DI) ===
            public static bool TryGetContactUpDI(enumTeachingPos site, out enumDINames di)
            {
                switch (site)
                {
                    // Front FT
                    case enumTeachingPos.Site11_F_FT1: di = enumDINames.X400_5_Front_FT_1_Contact_Up_Sensor; return true;
                    case enumTeachingPos.Site12_F_FT2: di = enumDINames.X400_3_Front_FT_2_Contact_Up_Sensor; return true;
                    case enumTeachingPos.Site13_F_FT3: di = enumDINames.X400_1_Front_FT_3_Contact_Up_Sensor; return true;

                    // Front DT
                    case enumTeachingPos.Site10_F_DT10_FT4: di = enumDINames.X403_0_Front_DT_10_FT_4_Contact_Up_Sensor; return true;
                    case enumTeachingPos.Site9_F_DT9: di = enumDINames.X402_7_Front_DT_9_Contact_Up_Sensor; return true;
                    case enumTeachingPos.Site8_F_DT8: di = enumDINames.X402_5_Front_DT_8_Contact_Up_Sensor; return true;
                    case enumTeachingPos.Site7_F_DT7: di = enumDINames.X402_3_Front_DT_7_Contact_Up_Sensor; return true;
                    case enumTeachingPos.Site6_F_DT6: di = enumDINames.X402_1_Front_DT_6_Contact_Up_Sensor; return true;
                    case enumTeachingPos.Site5_F_DT5: di = enumDINames.X401_6_Front_DT_5_Contact_Up_Sensor; return true;
                    case enumTeachingPos.Site4_F_DT4: di = enumDINames.X401_4_Front_DT_4_Contact_Up_Sensor; return true;
                    case enumTeachingPos.Site3_F_DT3: di = enumDINames.X401_2_Front_DT_3_Contact_Up_Sensor; return true;
                    case enumTeachingPos.Site2_F_DT2: di = enumDINames.X401_0_Front_DT_2_Contact_Up_Sensor; return true;
                    case enumTeachingPos.Site1_F_DT1: di = enumDINames.X400_7_Front_DT_1_Contact_Up_Sensor; return true;

                    // Rear FT
                    case enumTeachingPos.Site24_R_FT1: di = enumDINames.X406_1_Rear_FT_1_Contact_Up_Sensor; return true;
                    case enumTeachingPos.Site25_R_FT2: di = enumDINames.X405_6_Rear_FT_2_Contact_Up_Sensor; return true;
                    case enumTeachingPos.Site26_R_FT3: di = enumDINames.X405_4_Rear_FT_3_Contact_Up_Sensor; return true;

                    // Rear DT
                    case enumTeachingPos.Site23_R_DT10_FT4: di = enumDINames.X404_4_Rear_DT_10_FT_4_Contact_Up_Sensor; return true;
                    case enumTeachingPos.Site22_R_DT9: di = enumDINames.X404_2_Rear_DT_9_Contact_Up_Sensor; return true;
                    case enumTeachingPos.Site21_R_DT8: di = enumDINames.X404_0_Rear_DT_8_Contact_Up_Sensor; return true;
                    case enumTeachingPos.Site20_R_DT7: di = enumDINames.X407_7_Rear_DT_7_Contact_Up_Sensor; return true;
                    case enumTeachingPos.Site19_R_DT6: di = enumDINames.X407_5_Rear_DT_6_Contact_Up_Sensor; return true;
                    case enumTeachingPos.Site18_R_DT5: di = enumDINames.X407_2_Rear_DT_5_Contact_Up_Sensor; return true;
                    case enumTeachingPos.Site17_R_DT4: di = enumDINames.X407_0_Rear_DT_4_Contact_Up_Sensor; return true;
                    case enumTeachingPos.Site16_R_DT3: di = enumDINames.X406_7_Rear_DT_3_Contact_Up_Sensor; return true;
                    case enumTeachingPos.Site15_R_DT2: di = enumDINames.X406_5_Rear_DT_2_Contact_Up_Sensor; return true;
                    case enumTeachingPos.Site14_R_DT1: di = enumDINames.X406_3_Rear_DT_1_Contact_Up_Sensor; return true;
                }
                di = default;
                return false;
            }

            // === 2) 컨택트 UP/DOWN 솔레노이드 (DO) ===
            public static bool TryGetContactUpDownDO(enumTeachingPos site, out enumDONames updown)
            {
                switch (site)
                {
                    // Front FT
                    case enumTeachingPos.Site11_F_FT1: updown =enumDONames.Y3_4_Front_FT_1_CONTACT_UP_DOWN_SOL; return true;
                    case enumTeachingPos.Site12_F_FT2: updown =enumDONames.Y1_5_Front_FT_2_CONTACT_UP_DOWN_SOL; return true;
                    case enumTeachingPos.Site13_F_FT3: updown =enumDONames.Y3_5_Front_FT_3_CONTACT_UP_DOWN_SOL; return true;

                    // Front DT
                    case enumTeachingPos.Site10_F_DT10_FT4: updown =enumDONames.Y1_4_Front_DT_10_FT_4_CONTACT_UP_DOWN_SOL; return true;
                    case enumTeachingPos.Site9_F_DT9: updown =enumDONames.Y3_3_Front_DT_9_CONTACT_UP_DOWN_SOL; return true;
                    case enumTeachingPos.Site8_F_DT8: updown =enumDONames.Y1_3_Front_DT_8_CONTACT_UP_DOWN_SOL; return true;
                    case enumTeachingPos.Site7_F_DT7: updown =enumDONames.Y3_2_Front_DT_7_CONTACT_UP_DOWN_SOL; return true;
                    case enumTeachingPos.Site6_F_DT6: updown =enumDONames.Y1_2_Front_DT_6_CONTACT_UP_DOWN_SOL; return true;
                    case enumTeachingPos.Site5_F_DT5: updown =enumDONames.Y3_1_Front_DT_5_CONTACT_UP_DOWN_SOL; return true;
                    case enumTeachingPos.Site4_F_DT4: updown =enumDONames.Y1_1_Front_DT_4_CONTACT_UP_DOWN_SOL; return true;
                    case enumTeachingPos.Site3_F_DT3: updown =enumDONames.Y3_0_Front_DT_3_CONTACT_UP_DOWN_SOL; return true;
                    case enumTeachingPos.Site2_F_DT2: updown =enumDONames.Y1_0_Front_DT_2_CONTACT_UP_DOWN_SOL; return true;
                    case enumTeachingPos.Site1_F_DT1: updown =enumDONames.Y2_7_Front_DT_1_CONTACT_UP_DOWN_SOL; return true;

                    // Rear FT
                    case enumTeachingPos.Site24_R_FT1: updown =enumDONames.Y301_7_Rear_FT_1_CONTACT_UP_DOWN_SOL; return true;
                    case enumTeachingPos.Site25_R_FT2: updown =enumDONames.Y303_7_Rear_FT_2_CONTACT_UP_DOWN_SOL; return true;
                    case enumTeachingPos.Site26_R_FT3: updown =enumDONames.Y0_0_Rear_FT_3_CONTACT_UP_DOWN_SOL; return true;

                    // Rear DT
                    case enumTeachingPos.Site23_R_DT10_FT4: updown =enumDONames.Y303_6_Rear_DT_10_FT_3_CONTACT_UP_DOWN_SOL; return true;
                    case enumTeachingPos.Site22_R_DT9: updown =enumDONames.Y301_6_Rear_DT_9_CONTACT_UP_DOWN_SOL; return true;
                    case enumTeachingPos.Site21_R_DT8: updown =enumDONames.Y303_5_Rear_DT_8_CONTACT_UP_DOWN_SOL; return true;
                    case enumTeachingPos.Site20_R_DT7: updown =enumDONames.Y301_5_Rear_DT_7_CONTACT_UP_DOWN_SOL; return true;
                    case enumTeachingPos.Site19_R_DT6: updown =enumDONames.Y303_4_Rear_DT_6_CONTACT_UP_DOWN_SOL; return true;
                    case enumTeachingPos.Site18_R_DT5: updown =enumDONames.Y301_4_Rear_DT_5_CONTACT_UP_DOWN_SOL; return true;
                    case enumTeachingPos.Site17_R_DT4: updown =enumDONames.Y303_3_Rear_DT_4_CONTACT_UP_DOWN_SOL; return true;
                    case enumTeachingPos.Site16_R_DT3: updown =enumDONames.Y301_3_Rear_DT_3_CONTACT_UP_DOWN_SOL; return true;
                    case enumTeachingPos.Site15_R_DT2: updown =enumDONames.Y303_2_Rear_DT_2_CONTACT_UP_DOWN_SOL; return true;
                    case enumTeachingPos.Site14_R_DT1: updown =enumDONames.Y301_2_Rear_DT_1_CONTACT_UP_DOWN_SOL; return true;
                }
                updown = default;
                return false;
            }

            // === 3) 컨택트 스토퍼 솔 (DO) — 클램프 상태 판단용 ===
            public static bool TryGetContactStopperDO(enumTeachingPos site, out enumDONames stopper)
            {
                switch (site)
                {
                    // Front FT
                    case enumTeachingPos.Site11_F_FT1: stopper =enumDONames.Y2_5_Front_FT_1_CONTACT_STOPPER_SOL; return true;
                    case enumTeachingPos.Site12_F_FT2: stopper =enumDONames.Y0_6_Front_FT_2_CONTACT_STOPPER_SOL; return true;
                    case enumTeachingPos.Site13_F_FT3: stopper =enumDONames.Y2_6_Front_FT_3_CONTACT_STOPPER_SOL; return true;

                    // Front DT
                    case enumTeachingPos.Site10_F_DT10_FT4: stopper =enumDONames.Y0_5_Front_DT_10_FT4_CONTACT_STOPPER_SOL; return true;
                    case enumTeachingPos.Site9_F_DT9: stopper =enumDONames.Y2_4_Front_DT_9_CONTACT_STOPPER_SOL; return true;
                    case enumTeachingPos.Site8_F_DT8: stopper =enumDONames.Y0_4_Front_DT_8_CONTACT_STOPPER_SOL; return true;
                    case enumTeachingPos.Site7_F_DT7: stopper =enumDONames.Y2_3_Front_DT_7_CONTACT_STOPPER_SOL; return true;
                    case enumTeachingPos.Site6_F_DT6: stopper =enumDONames.Y0_3_Front_DT_6_CONTACT_STOPPER_SOL; return true;
                    case enumTeachingPos.Site5_F_DT5: stopper =enumDONames.Y2_2_Front_DT_5_CONTACT_STOPPER_SOL; return true;
                    case enumTeachingPos.Site4_F_DT4: stopper =enumDONames.Y0_2_Front_DT_4_CONTACT_STOPPER_SOL; return true;
                    case enumTeachingPos.Site3_F_DT3: stopper =enumDONames.Y2_1_Front_DT_3_CONTACT_STOPPER_SOL; return true;
                    case enumTeachingPos.Site2_F_DT2: stopper =enumDONames.Y0_1_Front_DT_2_CONTACT_STOPPER_SOL; return true;
                    case enumTeachingPos.Site1_F_DT1: stopper =enumDONames.Y2_0_Front_DT_1_CONTACT_STOPPER_SOL; return true;

                    // Rear FT
                    case enumTeachingPos.Site26_R_FT3: stopper =enumDONames.Y303_1_Rear_FT_3_CONTACT_STOPPER_SOL; return true;
                    case enumTeachingPos.Site25_R_FT2: stopper =enumDONames.Y301_1_Rear_FT_2_CONTACT_STOPPER_SOL; return true;
                    case enumTeachingPos.Site24_R_FT1: stopper =enumDONames.Y303_0_Rear_FT_1_CONTACT_STOPPER_SOL; return true;

                    // Rear DT
                    case enumTeachingPos.Site23_R_DT10_FT4: stopper =enumDONames.Y301_0_Rear_DT_10_FT_4_CONTACT_STOPPER_SOL; return true;
                    case enumTeachingPos.Site22_R_DT9: stopper =enumDONames.Y302_7_Rear_DT_9_CONTACT_STOPPER_SOL; return true;
                    case enumTeachingPos.Site21_R_DT8: stopper =enumDONames.Y300_7_Rear_DT_8_CONTACT_STOPPER_SOL; return true;
                    case enumTeachingPos.Site20_R_DT7: stopper =enumDONames.Y302_6_Rear_DT_7_CONTACT_STOPPER_SOL; return true;
                    case enumTeachingPos.Site19_R_DT6: stopper =enumDONames.Y300_6_Rear_DT_6_CONTACT_STOPPER_SOL; return true;
                    case enumTeachingPos.Site18_R_DT5: stopper =enumDONames.Y302_5_Rear_DT_5_CONTACT_STOPPER_SOL; return true;
                    case enumTeachingPos.Site17_R_DT4: stopper =enumDONames.Y300_5_Rear_DT_4_CONTACT_STOPPER_SOL; return true;
                    case enumTeachingPos.Site16_R_DT3: stopper =enumDONames.Y302_4_Rear_DT_3_CONTACT_STOPPER_SOL; return true;
                    case enumTeachingPos.Site15_R_DT2: stopper =enumDONames.Y300_4_Rear_DT_2_CONTACT_STOPPER_SOL; return true;
                    case enumTeachingPos.Site14_R_DT1: stopper =enumDONames.Y302_3_Rear_DT_1_CONTACT_STOPPER_SOL; return true;
                }
                stopper = default;
                return false;
            }

            // === 4) 사이트 이송 모터(CW/CCW) — DT/FT 모터 쌍 (DO) ===
            public static bool TryGetSiteTransportMotorPair(enumTeachingPos site, out enumDONames cw, out enumDONames ccw)
            {
                switch (site)
                {
                    // Front DT
                    case enumTeachingPos.Site1_F_DT1: cw =enumDONames.Y403_4_Front_DT_1_Motor_Cw; ccw =enumDONames.Y403_6_Front_DT_1_Motor_Ccw; return true;
                    case enumTeachingPos.Site2_F_DT2: cw =enumDONames.Y403_1_Front_DT_2_Motor_Cw; ccw =enumDONames.Y403_3_Front_DT_2_Motor_Ccw; return true;
                    case enumTeachingPos.Site3_F_DT3: cw =enumDONames.Y402_4_Front_DT_3_Motor_Cw; ccw =enumDONames.Y402_6_Front_DT_3_Motor_Ccw; return true;
                    case enumTeachingPos.Site4_F_DT4: cw =enumDONames.Y402_0_Front_DT_4_Motor_Cw; ccw =enumDONames.Y402_2_Front_DT_4_Motor_Ccw; return true;
                    case enumTeachingPos.Site5_F_DT5: cw =enumDONames.Y401_5_Front_DT_5_Motor_Cw; ccw =enumDONames.Y401_7_Front_DT_5_Motor_Ccw; return true;
                    case enumTeachingPos.Site6_F_DT6: cw =enumDONames.Y401_1_Front_DT_6_Motor_Cw; ccw =enumDONames.Y401_3_Front_DT_6_Motor_Ccw; return true;
                    case enumTeachingPos.Site7_F_DT7: cw =enumDONames.Y400_4_Front_DT_7_Motor_Cw; ccw =enumDONames.Y400_6_Front_DT_7_Motor_Ccw; return true;
                    case enumTeachingPos.Site8_F_DT8: cw =enumDONames.Y400_0_Front_DT_8_Motor_Cw; ccw =enumDONames.Y400_2_Front_DT_8_Motor_Ccw; return true;
                    case enumTeachingPos.Site9_F_DT9: cw =enumDONames.Y403_5_Front_DT_9_Motor_Cw; ccw =enumDONames.Y403_7_Front_DT_9_Motor_Ccw; return true;
                    case enumTeachingPos.Site10_F_DT10_FT4: cw =enumDONames.Y403_0_Front_DT_10_FT4_Motor_Cw; ccw =enumDONames.Y403_2_Front_DT_10_FT4_Motor_Ccw; return true;

                    // Front FT
                    case enumTeachingPos.Site11_F_FT1: cw =enumDONames.Y402_1_Front_FT_1_Motor_Cw; ccw =enumDONames.Y402_3_Front_FT_1_Motor_Ccw; return true;
                    case enumTeachingPos.Site12_F_FT2: cw =enumDONames.Y401_4_Front_FT_2_Motor_Cw; ccw =enumDONames.Y401_6_Front_FT_2_Motor_Ccw; return true;
                    case enumTeachingPos.Site13_F_FT3: cw =enumDONames.Y401_0_Front_FT_3_Motor_Cw; ccw =enumDONames.Y401_2_Front_FT_3_Motor_Ccw; return true;

                    // Rear DT 1~3
                    case enumTeachingPos.Site14_R_DT1: cw =enumDONames.Y305_5_Rear_DT_1_Motor_Cw; ccw =enumDONames.Y304_5_Rear_DT_1_Motor_Ccw; return true;
                    case enumTeachingPos.Site15_R_DT2: cw =enumDONames.Y305_6_Rear_DT_2_Motor_Cw; ccw =enumDONames.Y304_6_Rear_DT_2_Motor_Ccw; return true;
                    case enumTeachingPos.Site16_R_DT3: cw =enumDONames.Y305_7_Rear_DT_3_Motor_Cw; ccw =enumDONames.Y304_7_Rear_DT_3_Motor_Ccw; return true;

                    // Rear DT 4~10/FT4
                    case enumTeachingPos.Site17_R_DT4: cw =enumDONames.Y407_4_Rear_DT_4_Motor_Cw; ccw =enumDONames.Y407_6_Rear_DT_4_Motor_Ccw; return true;
                    case enumTeachingPos.Site18_R_DT5: cw =enumDONames.Y407_1_Rear_DT_5_Motor_Cw; ccw =enumDONames.Y407_3_Rear_DT_5_Motor_Ccw; return true;
                    case enumTeachingPos.Site19_R_DT6: cw =enumDONames.Y406_4_Rear_DT_6_Motor_Cw; ccw =enumDONames.Y406_6_Rear_DT_6_Motor_Ccw; return true;
                    case enumTeachingPos.Site20_R_DT7: cw =enumDONames.Y406_0_Rear_DT_7_Motor_Cw; ccw =enumDONames.Y406_2_Rear_DT_7_Motor_Ccw; return true;
                    case enumTeachingPos.Site21_R_DT8: cw =enumDONames.Y405_5_Rear_DT_8_Motor_Cw; ccw =enumDONames.Y405_7_Rear_DT_8_Motor_Ccw; return true;
                    case enumTeachingPos.Site22_R_DT9: cw =enumDONames.Y405_1_Rear_DT_9_Motor_Cw; ccw =enumDONames.Y405_3_Rear_DT_9_Motor_Ccw; return true;
                    case enumTeachingPos.Site23_R_DT10_FT4: cw =enumDONames.Y404_4_Rear_DT_10_FT_4_Motor_Cw; ccw =enumDONames.Y404_6_Rear_DT_10_FT_4_Motor_Ccw; return true;

                    // Rear FT
                    case enumTeachingPos.Site24_R_FT1: cw =enumDONames.Y407_5_Rear_FT_1_Motor_Cw; ccw =enumDONames.Y407_7_Rear_FT_1_Motor_Ccw; return true;
                    case enumTeachingPos.Site25_R_FT2: cw =enumDONames.Y407_0_Rear_FT_2_Motor_Cw; ccw =enumDONames.Y407_2_Rear_FT_2_Motor_Ccw; return true;
                    case enumTeachingPos.Site26_R_FT3: cw =enumDONames.Y406_5_Rear_FT_3_Motor_Cw; ccw =enumDONames.Y406_7_Rear_FT_3_Motor_Ccw; return true;
                }
                cw = default; ccw = default;
                return false;
            }

            // === 5) PCB Dock 센서 (DI) — 각 사이트의 PCB 도크 감지 ===
            public static bool TryGetPcbDockDI(enumTeachingPos site, out enumDINames di)
            {
                switch (site)
                {
                    // Front FT
                    case enumTeachingPos.Site11_F_FT1: di = enumDINames.X04_1_Front_FT_1_PCB_Dock_Sensor; return true;
                    case enumTeachingPos.Site12_F_FT2: di = enumDINames.X04_0_Front_FT_2_PCB_Dock_Sensor; return true;
                    case enumTeachingPos.Site13_F_FT3: di = enumDINames.X03_3_Front_FT_3_PCB_Dock_Sensor; return true;

                    // Front DT
                    case enumTeachingPos.Site10_F_DT10_FT4: di = enumDINames.X04_4_Front_DT_10_FT_4_PCB_Dock_Sensor; return true;
                    case enumTeachingPos.Site9_F_DT9: di = enumDINames.X04_5_Front_DT_9_PCB_Dock_Sensor; return true;
                    case enumTeachingPos.Site8_F_DT8: di = enumDINames.X04_6_Front_DT_8_PCB_Dock_Sensor; return true;
                    case enumTeachingPos.Site7_F_DT7: di = enumDINames.X04_7_Front_DT_7_PCB_Dock_Sensor; return true;
                    case enumTeachingPos.Site6_F_DT6: di = enumDINames.X114_0_Front_DT_6_PCB_Dock_Sensor; return true;
                    case enumTeachingPos.Site5_F_DT5: di = enumDINames.X114_1_Front_DT_5_PCB_Dock_Sensor; return true;
                    case enumTeachingPos.Site4_F_DT4: di = enumDINames.X114_2_Front_DT_4_PCB_Dock_Sensor; return true;
                    case enumTeachingPos.Site3_F_DT3: di = enumDINames.X114_3_Front_DT_3_PCB_Dock_Sensor; return true;
                    case enumTeachingPos.Site2_F_DT2: di = enumDINames.X114_4_Front_DT_2_PCB_Dock_Sensor; return true;
                    case enumTeachingPos.Site1_F_DT1: di = enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor; return true;

                    // Rear FT
                    case enumTeachingPos.Site24_R_FT1: di = enumDINames.X401_7_Rear_FT_1_PCB_Dock_Sensor; return true;
                    case enumTeachingPos.Site25_R_FT2: di = enumDINames.X401_5_Rear_FT_2_PCB_Dock_Sensor; return true;
                    case enumTeachingPos.Site26_R_FT3: di = enumDINames.X401_3_Rear_FT_3_PCB_Dock_Sensor; return true;

                    // Rear DT
                    case enumTeachingPos.Site23_R_DT10_FT4: di = enumDINames.X402_4_Rear_DT_10_FT_4_PCB_Dock_Sensor; return true;
                    case enumTeachingPos.Site22_R_DT9: di = enumDINames.X402_6_Rear_DT_9_PCB_Dock_Sensor; return true;
                    case enumTeachingPos.Site21_R_DT8: di = enumDINames.X403_1_Rear_DT_8_PCB_Dock_Sensor; return true;
                    case enumTeachingPos.Site20_R_DT7: di = enumDINames.X403_3_Rear_DT_7_PCB_Dock_Sensor; return true;
                    case enumTeachingPos.Site19_R_DT6: di = enumDINames.X403_4_Rear_DT_6_PCB_Dock_Sensor; return true;
                    case enumTeachingPos.Site18_R_DT5: di = enumDINames.X403_6_Rear_DT_5_PCB_Dock_Sensor; return true;
                    case enumTeachingPos.Site17_R_DT4: di = enumDINames.X404_1_Rear_DT_4_PCB_Dock_Sensor; return true;
                    case enumTeachingPos.Site16_R_DT3: di = enumDINames.X404_3_Rear_DT_3_PCB_Dock_Sensor; return true;
                    case enumTeachingPos.Site15_R_DT2: di = enumDINames.X404_5_Rear_DT_2_PCB_Dock_Sensor; return true;
                    case enumTeachingPos.Site14_R_DT1: di = enumDINames.X404_7_Rear_DT_1_PCB_Dock_Sensor; return true;
                }
                di = default;
                return false;
            }

            // === 6) (보너스) Passline/NG/셔틀/리프트 등 사이트 외 포지션 매핑 ===
            public static bool TryGetSingleMotorDO(enumTeachingPos pos, out enumDONames cw)
            {
                switch (pos)
                {
                    case enumTeachingPos.FrontPassLine: cw =enumDONames.Y404_1_Front_Passline_Motor_Cw; return true;
                    case enumTeachingPos.RearPassLine: cw =enumDONames.Y305_4_Rear_PassLine_Motor_Cw; return true;
                    case enumTeachingPos.RearNGLine: cw =enumDONames.Y404_0_Rear_NgLine_Motor_Cw; return true;
                    case enumTeachingPos.OutConveyor: cw =enumDONames.Y400_1_Out_Conveyor_Motor_Cw; return true;
                    default: cw = default; return false;
                }
            }

            public static bool TryGetShuttleMotors(enumTeachingPos pos, out enumDONames cw, out enumDONames ccw)
            {
                switch (pos)
                {
                    case enumTeachingPos.InShuttle: cw =enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw; ccw =enumDONames.Y400_3_In_Shuttle_Motor_Ccw; return true;
                    case enumTeachingPos.OutShuttle_Up: cw =enumDONames.Y304_0_Out_Shuttle_Ok_Motor_Cw; ccw =enumDONames.Y402_7_Out_Shuttle_Ok_Motor_Ccw; return true;
                    case enumTeachingPos.OutShuttle_Down: cw =enumDONames.Y400_5_Out_Shuttle_Ng_Motor_Cw; ccw =enumDONames.Y400_7_Out_Shuttle_Ng_Motor_Ccw; return true;
                }
                cw = ccw = default;
                return false;
            }

            public static bool TryGetLiftMotors(enumTeachingPos pos, out enumDONames cw, out enumDONames ccw)
            {
                switch (pos)
                {
                    case enumTeachingPos.Lift1_Up: cw =enumDONames.Y405_0_Front_Lift_Up_Motor_Cw; ccw =enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw; return true;
                    case enumTeachingPos.Lift1_Down: cw =enumDONames.Y405_4_Front_Lift_Down_Motor_Cw; ccw =enumDONames.Y405_6_Front_Lift_Down_Motor_Ccw; return true;
                    case enumTeachingPos.Lift2_Up: cw =enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw; ccw =enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw; return true;
                    case enumTeachingPos.Lift2_Down: cw =enumDONames.Y305_1_Rear_Lift_Down_Motor_Cw; ccw =enumDONames.Y304_1_Rear_Lift_Down_Motor_Ccw; return true;
                }
                cw = ccw = default;
                return false;
            }
        }
        #endregion

        #region Site 네이밍 변경
        // 세대별 사이트 표기 유틸
        public static class SiteDisplay
        {
            // siteNo(1~26) → enumTeachingPos
            public static FuncInline.enumTeachingPos ToSite(int siteNo)
            {
                switch (siteNo)
                {
                    case 1: return FuncInline.enumTeachingPos.Site1_F_DT1;
                    case 2: return FuncInline.enumTeachingPos.Site2_F_DT2;
                    case 3: return FuncInline.enumTeachingPos.Site3_F_DT3;
                    case 4: return FuncInline.enumTeachingPos.Site4_F_DT4;
                    case 5: return FuncInline.enumTeachingPos.Site5_F_DT5;
                    case 6: return FuncInline.enumTeachingPos.Site6_F_DT6;
                    case 7: return FuncInline.enumTeachingPos.Site7_F_DT7;
                    case 8: return FuncInline.enumTeachingPos.Site8_F_DT8;
                    case 9: return FuncInline.enumTeachingPos.Site9_F_DT9;
                    case 10: return FuncInline.enumTeachingPos.Site10_F_DT10_FT4;
                    case 11: return FuncInline.enumTeachingPos.Site11_F_FT1;
                    case 12: return FuncInline.enumTeachingPos.Site12_F_FT2;
                    case 13: return FuncInline.enumTeachingPos.Site13_F_FT3;
                    case 14: return FuncInline.enumTeachingPos.Site14_R_DT1;
                    case 15: return FuncInline.enumTeachingPos.Site15_R_DT2;
                    case 16: return FuncInline.enumTeachingPos.Site16_R_DT3;
                    case 17: return FuncInline.enumTeachingPos.Site17_R_DT4;
                    case 18: return FuncInline.enumTeachingPos.Site18_R_DT5;
                    case 19: return FuncInline.enumTeachingPos.Site19_R_DT6;
                    case 20: return FuncInline.enumTeachingPos.Site20_R_DT7;
                    case 21: return FuncInline.enumTeachingPos.Site21_R_DT8;
                    case 22: return FuncInline.enumTeachingPos.Site22_R_DT9;
                    case 23: return FuncInline.enumTeachingPos.Site23_R_DT10_FT4;
                    case 24: return FuncInline.enumTeachingPos.Site24_R_FT1;
                    case 25: return FuncInline.enumTeachingPos.Site25_R_FT2;
                    case 26: return FuncInline.enumTeachingPos.Site26_R_FT3;
                    default: return FuncInline.enumTeachingPos.None;
                }
            }

            // enumTeachingPos → 1~26
            public static int SiteNumber(FuncInline.enumTeachingPos site)
            {
                int baseIdx = (int)FuncInline.enumTeachingPos.Site1_F_DT1;
                int idx = (int)site - baseIdx;
                return idx + 1;
            }

            /// <summary>
            /// 세대별 표기 규칙:
            /// Gen6 이상  : "Site1" ~ "Site26"
            /// Gen5       : DT1~9 → "DownLoad1~9", FT1~4 → "Funtion1~4"
            /// Gen1~Gen4  : DT1~10 → "DownLoad1~10", FT1~3 → "Funtion1~3"
            /// </summary>
            public static string GetSiteDisplayName(FuncInline.enumTeachingPos site)
            {
                // 유효 범위 아니면 enum 이름 그대로
                if (site < FuncInline.enumTeachingPos.Site1_F_DT1 || site > FuncInline.enumTeachingPos.Site26_R_FT3)
                    return site.ToString();

                // Gen6 이상: 그냥 Site번호
                if (FuncInline.InlineType >= FuncInline.enumInlineType.Gen6)
                {
                    int no = SiteNumber(site);
                    return $"Site{no}";
                }

                // Gen5: DT1~9 = DownLoad1~9, FT1~4 = Funtion1~4
                if (FuncInline.InlineType == FuncInline.enumInlineType.Gen5)
                {
                    if (TryGetGen5DownLoadNo(site, out int dlNo))
                        return $"DownLoad{dlNo}";
                    if (TryGetGen5FuntionNo(site, out int ftNo))
                        return $"Function{ftNo}";
                    // 안전망
                    int no = SiteNumber(site);
                    return $"Site{no}";
                }

                // Gen1~Gen4: DT1~10 = DownLoad1~10, FT1~3 = Funtion1~3
                if (TryGetGenLE4DownLoadNo(site, out int dlNoLE4))
                    return $"DownLoad{dlNoLE4}";
                if (TryGetGenLE4FuntionNo(site, out int ftNoLE4))
                    return $"Function{ftNoLE4}";

                // 안전망
                return $"Site{SiteNumber(site)}";
            }

            // ===== Gen5 규칙 =====
            private static bool TryGetGen5DownLoadNo(FuncInline.enumTeachingPos site, out int no)
            {
                // Front DT1~9
                if (site >= FuncInline.enumTeachingPos.Site1_F_DT1 && site <= FuncInline.enumTeachingPos.Site9_F_DT9)
                {
                    no = (int)site - (int)FuncInline.enumTeachingPos.Site1_F_DT1 + 1; // 1..9
                    return true;
                }
                // Rear DT1~9
                if (site >= FuncInline.enumTeachingPos.Site14_R_DT1 && site <= FuncInline.enumTeachingPos.Site22_R_DT9)
                {
                    no = (int)site - (int)FuncInline.enumTeachingPos.Site14_R_DT1 + 1; // 1..9
                    return true;
                }
                no = 0;
                return false;
            }

            private static bool TryGetGen5FuntionNo(FuncInline.enumTeachingPos site, out int no)
            {
                // Front FT1~3
                if (site >= FuncInline.enumTeachingPos.Site11_F_FT1 && site <= FuncInline.enumTeachingPos.Site13_F_FT3)
                {
                    no = (int)site - (int)FuncInline.enumTeachingPos.Site11_F_FT1 + 1; // 1..3
                    return true;
                }
                // Front/Rear FT4 (DT10_FT4 위치)
                if (site == FuncInline.enumTeachingPos.Site10_F_DT10_FT4 || site == FuncInline.enumTeachingPos.Site23_R_DT10_FT4)
                {
                    no = 4;
                    return true;
                }
                // Rear FT1~3
                if (site >= FuncInline.enumTeachingPos.Site24_R_FT1 && site <= FuncInline.enumTeachingPos.Site26_R_FT3)
                {
                    no = (int)site - (int)FuncInline.enumTeachingPos.Site24_R_FT1 + 1; // 1..3
                    return true;
                }
                no = 0;
                return false;
            }

            // ===== Gen1~Gen4 규칙 =====
            private static bool TryGetGenLE4DownLoadNo(FuncInline.enumTeachingPos site, out int no)
            {
                // Front DT1~10
                if (site >= FuncInline.enumTeachingPos.Site1_F_DT1 && site <= FuncInline.enumTeachingPos.Site10_F_DT10_FT4)
                {
                    no = (int)site - (int)FuncInline.enumTeachingPos.Site1_F_DT1 + 1; // 1..10
                    return true;
                }
                // Rear DT1~10
                if (site >= FuncInline.enumTeachingPos.Site14_R_DT1 && site <= FuncInline.enumTeachingPos.Site23_R_DT10_FT4)
                {
                    no = (int)site - (int)FuncInline.enumTeachingPos.Site14_R_DT1 + 1; // 1..10
                    return true;
                }
                no = 0;
                return false;
            }

            private static bool TryGetGenLE4FuntionNo(FuncInline.enumTeachingPos site, out int no)
            {
                // Front FT1~3
                if (site >= FuncInline.enumTeachingPos.Site11_F_FT1 && site <= FuncInline.enumTeachingPos.Site13_F_FT3)
                {
                    no = (int)site - (int)FuncInline.enumTeachingPos.Site11_F_FT1 + 1; // 1..3
                    return true;
                }
                // Rear FT1~3
                if (site >= FuncInline.enumTeachingPos.Site24_R_FT1 && site <= FuncInline.enumTeachingPos.Site26_R_FT3)
                {
                    no = (int)site - (int)FuncInline.enumTeachingPos.Site24_R_FT1 + 1; // 1..3
                    return true;
                }
                no = 0;
                return false;
            }
        }

        #endregion

        public static bool SettingChange = false; //Setting Load를 했을때Sanding_Before_Tray_LR_Pickup값이 변했으면 true, 처음엔 false
        public static frmMain_AutoInline_PC Mainform = Application.OpenForms.OfType<frmMain_AutoInline_PC>().FirstOrDefault();    //frmMain의 컨트롤을 사용하기 위해 by DGKim 230710
        public static int HiddenCount = 0;    //숨겨진 셋팅 활성화 by DG 250123
        //#endregion
        public static bool Init_Finish = false; // 초기화 완료
                                                //#endregion
        public static bool Conveyor_Ampule_check = false;   //컨베이어에 앰플이 없을때
        public static bool Print_OutAmpule_check = false;   //프린트쪽 스피드컨트롤쪽 앰플이 없을때

        public static int Nomal_Sleep = 100;
        public static int Cylinder_Sleep = 500;
        public static int Vac_Sleep = 300;
        public static int Conveyor_Sleep = 1000;
        public static int Feeder_Sleep = 1000;
        public static int Servo_Sleep = 300;
        public static int Connect_Sleep = 500;
        public static int CommandWait_Sleep = 2000;
        public static int Manual_testSpeed = 1;    //메뉴얼에서 서보 스피드를 테스트하기위해(초기단계에 사용) by DG 240912

        private static void debug(string str)
        {
            Util.Debug("FuncInline + " + str);
            //FuncLog.WriteLog_Debug(str);
        }

        public static enumDONames[] siteLastDO = new enumDONames[40]; // SitePCBLoad 시 마지막 출력한 값

        #region 로그 알람 관련
        public static string OpenDoorInfo1 = ""; //유닛1 열린 Door 정보 확인
        public static string OpenDoorInfo2 = ""; //유닛2 열린 Door 정보 확인
        public static string OpenDoorInfo3 = ""; //유닛3 열린 Door 정보 확인

        public static string Interlock_View = ""; //메뉴얼 Inter상태 표시
        #endregion

        #region StopWatch
        public static Stopwatch WorkFinish = new Stopwatch(); // 작업 종료 시간
        public static int WorkFinishTime = 60; // 작업 종료 타임  초
        #endregion


        #region 메인화면 UI에 로그를 표시하기위해 만듬
        public static void LogView(string msg)//, String name)//BeforeLog,RobotLog,AfterLog
        {
            if (Mainform != null)
            {
                Mainform.logView(msg);
            }
        }

        public static void LogView2(string msg)
        {
            if (Mainform != null)
            {
                Mainform.logView2(msg);
            }
        }


        #endregion


        #region Start / STop 함수화
        public static void Start_Button(bool op)
        {

            // Tick Overflow 체크
            // 64비트를 쓰면 경고할 필요가 없다.
            //if (GlobalVar.TickCount64 > (Int32.MaxValue - (8 * 3600 * 1000)))
            //{ // 틱오버플로우 8시간 전부터 경고한다.
            //    bool isOk = FuncWin.MessageBoxOK("시스템이 24일 이상 재부팅하지 않았습니다! \n프로그램을 종료하고 PC를 재부팅한 후 다시 실행하여 주십시오.\nNO를 선택하면 작업을 일시적으로 계속할 수 있습니다.");
            //    if (isOk)
            //    {
            //        return;
            //    }
            //}


            /*
            if (!GlobalVar.MesPass)
            {
                if (GlobalVar.Sql.Check_Connect() == false)
                {
                    FuncWin.MessageBoxOK("MES DB 접속이 실패하였습니다. 통신 확인후 다시 시작 하세요!");
                    return;
                }
            }
            //*/


            if (GlobalVar.SystemStatus == enumSystemStatus.Manual)
            {
                //if (op)
                //{
                //    // 버튼 누름시 메인페이지로 이동하고 메인탭이 선택되게 한다.
                //    ClearSubMenu();
                //    pbAuto.BackgroundImage = Radix.Properties.Resources.sub_main_sel;
                //    FuncInline.TabMain = FuncInline.enumTabMain.Auto;
                //    tcMain.SelectedIndex = (int)FuncInline.TabMain;
                //    Application.DoEvents();
                //}



                ((AutoInline_Class)GlobalVar.ProjectClass).runTime = GlobalVar.TickCount64;

                // 에러 발생으로 백업된 DO가 있으면 복원한다.
                // 박스포장기에서 사용하던 복원기능인데 일단 막는다 by DGkim 241111
                //DIO.DO_RestoreALL();

                //CycleStop = true이면 CycleStop으로 시작
                if (FuncInline.CycleStop == true)
                {
                    GlobalVar.SystemStatus = enumSystemStatus.CycleStop;

                }
                else
                {
                    GlobalVar.SystemStatus = enumSystemStatus.AutoRun;
                }


                // GlobalVar.Thread_AutoRun.Start();

            }
            else if (GlobalVar.SystemStatus == enumSystemStatus.ErrorStop)
            {
                if (op) // op 버튼으로 시작시는 에러차 있으면 안 되므로 그냥 리턴
                {
                    return;
                }
                else
                {
                    FuncWin.TopMessageBox("See error log first.");
                }
            }
            else if (GlobalVar.SystemErrored)
            {
                if (op) // op 버튼으로 시작시는 에러차 있으면 안 되므로 그냥 리턴
                {
                    return;
                }
                else
                {
                    FuncWin.TopMessageBox("Error Check.");
                }
            }
            else // BeforeInit
            {
                if (op) // op 버튼으로 시작시는 에러차 있으면 안 되므로 그냥 리턴
                {
                    FuncLog.WriteLog("Run system initialization first.");
                    return;
                }
                else
                {
                    FuncWin.TopMessageBox("Run system initialization first.");
                }
            }
            if (GlobalVar.dlgOpened)
            {
                return;
            }
        }
        public static void Stop_Button()
        {

            //if (GlobalVar.SystemStatus == enumSystemStatus.AutoRun || GlobalVar.SystemStatus == enumSystemStatus.CycleStop)
            //{
            //    GlobalVar.SystemStatus = enumSystemStatus.Manual;
            //}

            /*
            // 레이저 마킹기 대기상태로 집진기능 정지
            if (GlobalVar.LaserMark.connected)
            {
                //GlobalVar.LaserMark.SendCmd("RESETSYSTEM\r\nMARK STOP");
            }
            //*/

            if (GlobalVar.SystemStatus == enumSystemStatus.ErrorStop)
            {
                FuncLog.WriteLog("에러 상태에서는 리셋버튼을 누르십시오.");
                return;
            }


            if (GlobalVar.SystemStatus == enumSystemStatus.Manual || GlobalVar.SystemStatus == enumSystemStatus.AutoRun || GlobalVar.SystemStatus == enumSystemStatus.CycleStop)
            {
                GlobalVar.SystemStatus = enumSystemStatus.Manual;
            }
            else
            {
                GlobalVar.SystemStatus = enumSystemStatus.BeforeInitialize;
            }

            GlobalVar.SystemMsg = "Run stoped";
            ((AutoInline_Class)GlobalVar.ProjectClass).runTotal += (GlobalVar.TickCount64 - ((AutoInline_Class)GlobalVar.ProjectClass).runTime) / 1000;

            // 정지전에 작업했던 종이매뉴얼 슬롯 설정값을 연노랑으로 표시한다.(착오방지용)
            //if (GlobalVar.G_ManualType == 1 )
            //{
            //    btn_KorManual.BackColor = Color.LightYellow;
            //    btn_EngManual.BackColor = Color.White;
            //}
            //else if(GlobalVar.G_ManualType == 2)
            //{
            //    btn_KorManual.BackColor = Color.White;
            //    btn_EngManual.BackColor = Color.LightYellow;
            //}
            // 종이매뉴얼 설정 상태 리셋
            //GlobalVar.G_ManualType = 0;

            DIO.DO_BackupALL(false);

            // 모터를 강제로 정지해야할까? 
            // 서보 이동중 강제정지시 재시작해도 위치 도달 실패로 에러가 발생한다. 
            // 초기화 중에 누른다면 서보를 정지하고 자동 운전 중이라면 강제 정지는 필요 없을듯 하다.

            // 주의 : 홈 동작중에 강제 정지하면 매뉴얼 컨베이어와 투입기가 서로 간섭하는 위치가 되고
            //        투입기와 컨베이어가 서로 이동 하지 못하는 상황이 발생하므로 
            //        할수 없이 정지버튼을 사용해도 서보가 강제로 정지하지 않도록 하였다.

            //if (GlobalVar.SystemStatus <= enumSystemStatus.Manual)
            //{
            //    RTEX.MoveStopAll();
            //}
        }

        public static void Reset_Button()
        {


            // 리셋 버튼 처리 시작

            //!GlobalVar.Init_Finish 조건 추가 Init_Finish가 아니면 BeforeInitialize되도록 변경 by DG 250610
            if (GlobalVar.SystemStatus == enumSystemStatus.EmgStop || GlobalVar.SystemStatus < enumSystemStatus.Manual || !GlobalVar.Init_Finish)
            {
                FuncError.RemoveAllError();

                GlobalVar.SystemStatus = enumSystemStatus.BeforeInitialize;
                GlobalVar.E_Stop = false;
                GlobalVar.EnableTower = true;
                GlobalVar.EnableBuzzer = true;
                GlobalVar.SystemErrored = false;
                return;
            }

            if (GlobalVar.SystemStatus == enumSystemStatus.Manual)
            {
                FuncError.RemoveAllError();

                GlobalVar.SystemStatus = enumSystemStatus.Manual;
                GlobalVar.E_Stop = false;
                GlobalVar.EnableTower = true;
                GlobalVar.EnableBuzzer = true;
                GlobalVar.SystemErrored = false;
                return;
            }



            if (GlobalVar.SystemStatus == enumSystemStatus.ErrorStop)
            {
                //에러발생하면 해당상태의 IO값을 백업(복원 동작하려면 필요)
                //DIO.DO_BackupALL(true);

                // 에러시 리셋을 눌렀을때 문제있는 에러 부위의 실린더를 꺼주는 기능(필요하면 사용)

                //FuncInline.ReleaseCylinder(GlobalVar.G_ErrNo);

                //if (!GlobalVar.BoxPacking_Feeder.IsCycleEnd())
                //DIO_BoxPacking_enumDONames.Y00_0_OP_Start_Lamp

            }

            GlobalVar.G_ErrNo = 0;

            FuncError.RemoveAllError();

            GlobalVar.E_Stop = false;
            GlobalVar.EnableTower = true;
            GlobalVar.EnableBuzzer = true;
            GlobalVar.SystemErrored = false;

            // Clear Servo Alarm
            RTEX.ServoReset_All();

            RTEX.ServoOnAll(true);
        }

        /**
         * @brief 하부 클래스들의 동작상태를 초기화
         *          초기화 이후 첫 동작할 상태로 만든다.
         */
        public static void InitAllSubAction()
        {
            //여기서 초기화 후 처음 동작을 미리 지정한다. by DG
            //모두 Waiting으로 빼는게 맞을듯, 마지막 초기화 위치에서 대기하겠지

            ((AutoInline_Class)GlobalVar.ProjectClass).beforeLift01.Action = BeforeLiftClass.enumAction.Waiting;    //#1 샌딩전 트레이 투입 리프트
            ((AutoInline_Class)GlobalVar.ProjectClass).beforeWork02.Action = BeforeWorkClass.enumAction.Waiting;    //#2 샌딩전 트레이 작업위치


        }
        #endregion



        #region 공통함수에서 사용하기 위해 재정의하는 함수

        #region INI 관련
        #region Machine
        /**
         * @brief 장비설정 읽기
         *      프로젝트에 따라서 선택
         */
        public static void LoadMachinenIni() // 장비의 모든 설정 읽기
        {
            GlobalVar.ManagePasswd = FuncIni.ReadIniFile("manage", "pwd", GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\manage.ini", "1234");

            #region general
            string IniPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\setting.ini";
            string Section = GlobalVar.IniSection;

            GlobalVar.UseDoor = (FuncIni.ReadIniFile(GlobalVar.seting_Section, "UseDoor", GlobalVar.seting_IniPath, "true") == "true");

            GlobalVar.AppName = FuncIni.ReadIniFile(Section, "appName", IniPath, "SNUC Sanding System");
            GlobalVar.NormalTimeout = long.Parse(FuncIni.ReadIniFile(Section, "NormalTimeout", IniPath, "20"));
            #endregion



           

         


            #region Tower Lamp
            IniPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\Tower.ini";

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        GlobalVar.TowerAction[i, j, k] = FuncIni.ReadIniFile(i.ToString(), j.ToString() + "_" + k.ToString(), IniPath, "False") == "True";
                    }
                }
                GlobalVar.TowerTime[i] = ulong.Parse(FuncIni.ReadIniFile(i.ToString(), "time", IniPath, "0"));
            }
            #endregion
        }
      
        #endregion

    
        #endregion

        #region DIO.cs 에서 분기되어 재정의 해야 하는 함수
        /**
         * @brief DI 이름 조회
         */
        public static string GetDIName(int index)
        {
            if (index < Enum.GetValues(typeof(FuncInline.enumDINames)).Length)
            {
                return ((FuncInline.enumDINames)index).ToString();
            }
            return "";
        }
        /**
         * @brief DO 이름 조회
         */
        public static string GetDOName(int index)
        {
            if (index < Enum.GetValues(typeof(enumDONames)).Length)
            {
                return ((enumDONames)index).ToString();
            }
            return "";
        }
        /**
         * @brief DO 작동 전 인터락 체크
         * @return true 경우 동작, false 경우 동작 금지
         */
        public static bool CheckInterlock(int dn, bool data)
        {


            switch ((enumDONames)dn)
            {

                //case FuncAmplePacking.enumDONames.Y13_1_HighBuash_Exaust_Pinger_Right_Close:
                //    if (data &&
                //        (!DIO.GetDIData(FuncAmplePacking.enumDINames.X15_6_HighBush_Exaust_Up_Sensor) &&
                //        !DIO.GetDIData(FuncAmplePacking.enumDINames.X15_7_HighBush_Exaust_Down_Sensor)))
                //    {
                //        FuncLog.WriteLog("DIO Interlocak (" + ((FuncAmplePacking.enumDONames)dn).ToString() + "불량 배출 상승 하강 미확인");
                //        return false;
                //    }
                //    break;
                //case FuncAmplePacking.enumDONames.Y13_2_HighBuash_Exaust_Pinger_Left_Open:
                //    break;
                //case FuncAmplePacking.enumDONames.Y13_3_HighBuash_Exaust_Pinger_Left_Close:
                //    if (data &&
                //        (!DIO.GetDIData(FuncAmplePacking.enumDINames.X15_6_HighBush_Exaust_Up_Sensor) &&
                //        !DIO.GetDIData(FuncAmplePacking.enumDINames.X15_7_HighBush_Exaust_Down_Sensor)))
                //    {
                //        FuncLog.WriteLog("DIO Interlocak (" + ((FuncAmplePacking.enumDONames)dn).ToString() + "불량 배출 상승 하강 미확인");
                //        return false;
                //    }
                //    break;
                //case FuncAmplePacking.enumDONames.Y13_4_HighBuash_Exaust_TrayForward:
                //    break;
                //case FuncAmplePacking.enumDONames.Y14_4_Fixtrue_Tray_Forward:
                //    // 픽스처 리프트 동작중이거나, 트레이클램프 체결되어 있거나, 픽스처 공급 상승상태 아니면 금지
                //    if (!GlobalVar.AxisStatus[(int)FuncAmplePacking.enumServoAxis.SV05_Fixture_Tray].StandStill ||
                //        DIO.GetDIData(FuncAmplePacking.enumDINames.X19_1_Fixture_Tray_Clamp_Front_Forward_Sensor) ||
                //        DIO.GetDIData(FuncAmplePacking.enumDINames.X19_4_Fixture_Tray_Clamp_Rear_Forward_Sensor) ||
                //        GlobalVar.AxisStatus[(int)FuncAmplePacking.enumServoAxis.SV04_Fixture_Supply_Z].Position > Fixture_Supply_Z_supply_pos + 1) // 공급위치보다 낮게 있을때 취출위치니까 전진하면 안됨
                //    {
                //        FuncLog.WriteLog("DIO Interlocak (" + ((FuncAmplePacking.enumDONames)dn).ToString() + " 픽스처 리프트 동작중 또는 트레이클램프 체결중 또는 픽스처 공급 상승상태 미확인");
                //        return false;
                //    }
                //    break;

            }
            return true;
        }
        /**
         * 
         */
        public static void Simulate_DIO(object dn, bool data)
        {
            //switch ((enumDONames)dn)
            //{
            //    case enumDONames.Y02_3_After_Tray_WorkArea_Clamp_Forward:
            //        DIO.WriteDIData(enumDINames.X03_5_After_Tray_Clamp_Forward_Sensor, data);
            //        DIO.WriteDIData(enumDINames.X03_6_After_Tray_Clamp_Backward_Sensor, !data);
            //        break;

            //}
        }

        /** 
         * @brief  E_Stop OP 체크
         * @return bool B접점
         *      EMG Normal : True
         *      EMG Push : False
         */
        public static bool EMG_Check() // EMG Normal : True, EMG Push : False    //B접점
        {
            if (!DIO.GetDIData((int)FuncInline.enumDINames.X00_3_Emergency_Stop))
            {
                //GlobalVar.E_Stop = false;
                return true;
            }
            //GlobalVar.E_Stop = true;
            return false;
        }
        /** 
         * @brief 시뮬레이션용 E_Stop 입력 조작
         *      시뮬레이션시 OP가 없어 B 접점 신호가 없으므로 강제로 세팅한다
         * @param value 지정할 값
         * @return void
         */
        public static void EMG_Control(bool value)
        {
            DIO.WriteDIData((int)FuncInline.enumDINames.X00_3_Emergency_Stop, value);
        }
        /** 
         * @brief 도어 센서 체크
         *        안전 체크용
         *        일단 전체 도어로 작성함
         *        피더나 리프트쪽 도어 운영중 사용하려면 제외하고 각각 용도에 맞는 함수를 만들어야 함.
         * @return bool A접점
         *      DOOR Close : False 
         *      DOOR Open : True
         */
        public static bool Door_Check() // DOOR Close : False, DOOR Open : True //A접점
        {
            if (!DIO.GetDIData((int)FuncInline.enumDINames.X00_0_Door_Open_Front_Left))
            //!DIO.GetDIData((int)FuncAmplePacking.enumDINames.X31_7_3RD_Inner_Door)) //이건 수시로 열어야될 도어라.. 막자
            /*
                !DIO.GetDIData((int)FuncAmplePacking.enumDINames.X30_7_3RD_Front_Left_Door) ||
                !DIO.GetDIData((int)FuncAmplePacking.enumDINames.X31_0_3RD_Front_Right_Door) ||
                !DIO.GetDIData((int)FuncAmplePacking.enumDINames.X31_1_3RD_Outer_Left_Left_Door) ||
                !DIO.GetDIData((int)FuncAmplePacking.enumDINames.X31_2_3RD_Outer_Left_Right_Door) ||
                !DIO.GetDIData((int)FuncAmplePacking.enumDINames.X31_3_3RD_Outer_Right_Left_Door) ||
                !DIO.GetDIData((int)FuncAmplePacking.enumDINames.X31_4_3RD_Outer_Right_Right_Door) ||
                !DIO.GetDIData((int)FuncAmplePacking.enumDINames.X31_5_3RD_Rear_Left_Door) ||
                !DIO.GetDIData((int)FuncAmplePacking.enumDINames.X31_6_3RD_Rear_Right_Door)
                */
            {
                return true;
            }
            return false;
        }



        #endregion



        #endregion


        // 화면우측 AutoRun Step Status Update
        public static void StepPrint()
        {
            Mainform.listBoxStep.BeginUpdate();
            FuncInline.LogView2("");    // clear list
            //FuncBoxPacking.LogView2(GlobalVar.Thread_AutoRun.GetStepString());
            //FuncBoxPacking.LogView2(GlobalVar.Thread_SupplyBox.GetStepString());
            //FuncBoxPacking.LogView2(GlobalVar.Thread_SupplyManual.GetStepString());
            //FuncBoxPacking.LogView2(GlobalVar.Thread_InBlister.GetStepString());
            //FuncBoxPacking.LogView2(GlobalVar.Thread_BoxIn.GetStepString());
            //FuncBoxPacking.LogView2(GlobalVar.Thread_BoxReverse.GetStepString());
            //FuncBoxPacking.LogView2(GlobalVar.Thread_BoxLabel.GetStepString());
            //FuncBoxPacking.LogView2(GlobalVar.Thread_SortOut.GetStepString());
            //FuncBoxPacking.LogView2(GlobalVar.Thread_LaserMark.GetStepString());
            //FuncBoxPacking.LogView2(GlobalVar.Thread_BoxCheck.GetStepString());
            //FuncBoxPacking.LogView2(GlobalVar.Thread_MES.GetStepString());
            //FuncBoxPacking.LogView2(GlobalVar.Thread_OutBarcodes.GetStepString());

            Mainform.listBoxStep.EndUpdate();
        }
        public static void AddError(FuncInline.structError er) // AddError(에러명) 에러발생 큐에 추가
        {
            if (er.ErrorCode == FuncInline.enumErrorCode.No_Error)
            {
                return;
            }

            #region 시스템 정지 상태가 될 것이므로 스메마 신호 해제
            DIO.WriteDOData(FuncInline.enumDONames.Y4_2_SMEMA_Before_Ready, false);
            DIO.WriteDOData(FuncInline.enumDONames.Y412_1_SMEMA_After_Ready, false);
            #endregion

       

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

            FuncInline.NeedPartClear[(int)er.ErrorPart] = true;

            if (!FuncError.CheckError(er))
            {
                //debug("AddError " + er.ErrorPart + " , " + er.ErrorCode.ToString() + " , " + er.Description);
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
                            FuncLog.WriteLog("HJ 확인 - AddError Func.cs(System_Not_Inited)     enumSystemStatus.BeforeInitialize");
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
                            //else
                            //{
                            //    GlobalVar.SystemStatus = enumSystemStatus.Manual;
                            //}
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
        public static void AddError(enumErrorPart part, enumErrorCode code, string msg)
        {
            FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
                     DateTime.Now.ToString("HH:mm:ss"),
                     part,
                     code,
                     false,
                     msg));
        }

        public static void AddError(string msg)
        {
            FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
                     DateTime.Now.ToString("HH:mm:ss"),
                     enumErrorPart.System,
                     enumErrorCode.Sol_Error,
                     false,
                     msg));
        }

        public static void AddError(enumErrorCode errCode, string msg)
        {
            FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
                     DateTime.Now.ToString("HH:mm:ss"),
                     enumErrorPart.System,
                     errCode,
                     false,
                     msg));
        }

        // JHRYU 버전
        public static void AddError(int errCode, string msg)
        {
            FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
                     DateTime.Now.ToString("HH:mm:ss"),
                     enumErrorPart.System,
                     (enumErrorCode)errCode,
                     false,
                     msg));
        }




        public static void AddWarning(string msg)
        {
            //FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
            //         DateTime.Now.ToString("HH:mm:ss"),
            //         enumErrorPart.System,
            //         enumErrorCode.Sol_Error,
            //         false,
            //         msg));
        }

        public static void AddInfo(string msg)
        {
            //FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
            //         DateTime.Now.ToString("HH:mm:ss"),
            //         enumErrorPart.System,
            //         enumErrorCode.Sol_Error,
            //         false,
            //         msg));
        }


        #region Warning 창 발생
        public static void send_WarningMsg(String text, int State)
        {
            // JHRYU : 다중 알람을 처리하기위해 WarningStatePre 리스트로 변경

            if (GlobalVar.WarningState != 0) return;            // 경고 발생중이면 미처리

            if (!GlobalVar.WarningStatePre.Contains(State) || State > 99)     // 무시 리스트에 없는 경고거나 99번보다 크면
            {
                GlobalVar.WarningState = State;
                GlobalVar.dlgWarning_Check = true;   //작업중인 Box가 없다면 알람 또는 메시지창
                GlobalVar.dlgWarning_Msg = text;
            }
        }
        #endregion Warning 창 발생

        #region 전역 변수

        //public static structPosition SiteOffset = new structPosition(); // 티칭시 Site1 티칭하면 전체 Site 계산 Offset ==> 개별 offset으로 변경
        //public static structPosition[] PositionOffset = new structPosition[Enum.GetValues(typeof(enumTeachingPos)).Length];
        public static double[] WidthOffset = new double[Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length + 20 + 1]; // 스텝모터 15, 사이트20, 지그리프트. 생성시 변수로 지정하면 0이 되어 상수로 입력

        #region UI 구분
        public static FuncInline.enumTabMain TabMain = FuncInline.enumTabMain.Auto;
        public static FuncInline.enumTabMachine TabMachine = FuncInline.enumTabMachine.Machine;
        public static FuncInline.enumTabTeaching TabTeaching = FuncInline.enumTabTeaching.LiftPosition;
        public static FuncInline.enumTabManual TabManual = FuncInline.enumTabManual.Lift;
        //public static bool Machine_Machine = false; // Machine - Machine 화면
        //public static bool Machine_TowerLamp = false; // Machine - TowerLamp 화면
        //public static bool Machine_SerialSet = false; // Machine - SerialSet 화면
        public static string WaitMessage = ""; // 대기창 출력 메시지. ""일 때 창 닫고, 아닐 때 연다. Main 윈도 하부에서 사용
        public static string SiteMessage = ""; // 사이트 세부 정보창 동작시 대기창 출력 메시지. ""일 때 창 닫고, 아닐 때 연다. Main 윈도 하부에서 사용
        #endregion


        #region 모델별 설정
        public static int ScanRetryCount = 3; // 스캔 재시도 횟수
        public static int ArrayCount = 6; // 모델의 어레이 수
        public static string ArrayImage = ""; // PCB Layout Image
        public static int TestPassTime = 5 * 60; // DryRun, SimulationMode시 강제 테스트 통과시간
        public static enumSMDStatus TestPassMode = FuncInline.enumSMDStatus.Test_Pass; // 시뮬레이션 시간 경과시 처리 방법
        public static int TestPassSet = 0; // 시뮬레이션시 검사횟수 따라 불량처리할 횟수
        public static int TestPassCount = 0; // 시뮬레이션시 검사횟수 양품 처리한 횟수
        public static int TestFailCode = 1; // 시뮬레이션 불량 처리시
        public static int TestTimeout = 20 * 60; // 테스트 타웃아웃처리 시간
        public static int TestCommandTimeout = 10; // STA 등 전송 후 답신 타임아웃 처리 시간
        public static int TestCommandRetry = 10; // STA 등 전송 후 답신 재시도 횟수
        //public static int RobotTimeout = 60; // 직교로봇 이동 타임아웃 처리 초
        public static int ConveyorTimeout = 30; // 컨베어 이동 타임아웃 처리 초
        public static int ScanTimeout = 30; // 2D 스캔 이동 타임아웃 처리 초
        //public static int NGReEnterCount = 3; // NG PCB 재투입 알람 기준
        public static bool SelfRetest = false; // Test Fail 시 재 테스트 여부. 그 자리에서
        public static bool OtherRetest = false; // Test Fail 시 재 테스트 여부. 다른 사이트에서
        public static bool FailWhenNoEmpty = false; // 빈사이트 없을 경우 리사이트 없이 fail 처리
        public static int SelfRetestCount = 3; // Self Retest 시도 횟수
        public static int OtherRetestCount = 3; // Other Retest 시도 횟수
        public static bool NGToUnloading = false; // Test Fail된 PCB 일반 배출 여부, true일 때 Unloading, Fail이면 NG로
        public static bool PassToNG = false; // Test Pass된 PCB NG 배출 여부, true일 때 NG, Fail이면 Loading으로
        public static bool NgNgedOnly = true; // Test Pass된 PCB NG 배출 여부, false 때 양품도 NG로
        public static bool PinDownAndClamp = false; // 핀 내린 후 클램프 여부
        public static bool TestWithSiteUnclamp = false; // 언클램프 상태로 검사 여부
        //public static bool NgAlarm = true; // NG배출시 부저 및 경광등 조작 여부
        public static int NgAlarmTime = 5; // NG배출시 부저 및 경광등 조작 시간

        public static enumSiteState[] SiteState = new enumSiteState[MaxSiteCount]; // 각 사이트 연결 상태
        public static bool[] UseSite = new bool[MaxSiteCount]; // 각 사이트 사용 여부
        public static String[] BlockReason = new String[MaxSiteCount]; // 각 사이트 Block 사유
        public static bool[] ReTestSite = new bool[MaxSiteCount]; // 각 사이트 재검사용 사용 여부
        public static bool ReTestOnly = true; // 재검사용 사이트 재검사용으로만 사용
        public static enumSiteAction[] SiteAction = new enumSiteAction[MaxSiteCount]; // 각 사이트 동작구분
        public static bool[] SiteConvRun = new bool[MaxSiteCount]; // 사이트 컨베어 돌린 적이 있는가? PCB 감지 여부 상관없이 무조건 돌리기 위해
        public static bool[] SiteClear = new bool[MaxSiteCount]; // 각 사이트 Clear 해야 하는 상황 여부. 사이트 알람 발생시 true, 사이트 Open시 false, true일 때 클리어를 못하게 한다.

        public static bool[] ArrayUse = new bool[MaxArrayCount]; // 각 어레이 사용 정보
        public static bool[] XOut = new bool[MaxArrayCount]; // 각 어레이 XOut 정보
        public static bool BlockNGArray = false; // 동일 어레이 불량 누적또는 연속 발생시 Block 여부
        //public static bool BlockContinuous = false; // 어레이 불량 Block시 연속발생 체크 여부.
        public static int BlockNGCount = 3; // 어레이 불량 Block시 체크 카운트값
        public static bool BlockDefectArray = false; // 불량율 초과시 사이트 블럭 여부
        public static bool ScanTwice = false; // 스캔 두 번 해서 대조
        public static bool PassCNDuplication = false; // CN 중복 검사 1회 무시
        public static bool CheckCNDuplication = false; // CN 중복 검사 여부
        public static int CheckCNDupeCount = 5; // CN 중복 검사시 연속 투입 검사 수
        public static ConcurrentQueue<PCBInfoClass> DupeQueue = new ConcurrentQueue<PCBInfoClass>(); // CN 중복 검사 큐
        public static bool CheckCNCross = false; // CN Cross 검사 여부

        public static enumTeachingPos[] PCBInfoMoveFlag = new enumTeachingPos[Enum.GetValues(typeof(enumTeachingPos)).Length]; // PCB 정보 이전중. 중복 체크시 이동중인 건 배제하기 위함.
        public static enumTeachingPos[] DuplicatedPos = new enumTeachingPos[2]; // 중복 확인된 두 좌표

        public static int[,] ArrayNGCount = new int[MaxSiteCount, MaxArrayCount]; // 각 사이트/어레이 불량 발생 카운트.  누적 또는 연속으로 발생시 Block 위함
        public static int[,] ArrayNGCode = new int[MaxSiteCount, MaxArrayCount]; // 각 사이트/어레이 최종 불량 코드
        public static bool PinUseFront = true; // 전면 핀 사용 여부
        public static bool PinUseRear = true; // 후면 핀 사용 여부
        public static bool XoutBySelection = false; // 프로그램상 선택으로 Xout 여부
        public static bool XoutByVision = false; // Vision 검사로 Xout 여부
        public static bool UseBadMark = false; // BadMark 검사 유무
        //public static bool BadMarkWhenExist = false; // BardMark 있을 때 Skip
        public static bool XoutToNG = false; // XOut NG로 보내기. 선택에 따른 경우만
        public static bool BadMarkToNG = false; // BadMark NG로 보내기
        public static double PCBWidth = 70; // 모델의 PCB 폭
        public static double PCBLength = 100; // 모델의 PCB 길이
        public static enumTestType TestType = FuncInline.enumTestType.All; // 테스트 방법
        public static bool CarrierSeparation = false; // 캐리어 분리 여부. false 경우 캐리어 있건 없건 상관 않는다.
        public static bool PCBInverting = false; // PCB 반전 여부
        public static bool UseJigStop = false; // 사이트 내 중간정지 사용 여부
        public static bool ScanAfterInvert = false; // 스캔 실행 반전 전에 실행
        public static bool[,] PickupVaccum = new bool[3, 4]; // 픽업장치 진공 선택. In/Ng/Out Pickup순
        //public static double RobotSlowHeight = 10; // 로봇 동작시 목표 위치 상단 저속 구간
        //public static double RobotSpeed = 100; // 로봇 속도
        //public static double RobotSpeedSlow = 10; // 로봇 속도 저속 구간
        //public static double RobotZPitch = 10; // 로봇 z축 스크류 핏치. 개조전10, 개조후20
        //public static double RobotInputOutputOffsetX = 300; // 입력/출력 지그 간격
        //public static double RobotOutputSiteOffsetX = 20; // OutputJig로 투입시 배출좌표 기준 shift값
        //public static double RobotInputSiteOffsetX = 180; // InputJig로 배출시 투입좌표 기준 shift값
        //public static double CoolingLoadingOffsetX = 300; // Cooling 컨베어 취출시 로딩티칭 기준 shift값
        //public static bool InputJigResite = false; // 투입지그로 리사이트 여부
        //public static double RobotAccDec = 3; // 로봇 가감속
        //public static double RobotJerk = RobotAccDec * RobotAccDec; // 로봇 Jerk
        //public static double RobotInputVisionOffsetX = -300; // 투입지그와 티칭카메라의 X 위치 차이
        //public static double RobotInputVisionOffsetY = -300; // 투입지그와 티칭카메라의 Y 위치 차이
        //public static double RobotOffsetZ = 0; // Z1과 Z2의 거리 차이
        //public static double RobotGripShift = 10; // 그립 풀고 돌아나오는 이동 거리
        //public static double RobotWidthOffset = 10; // 그립 풀고 벌리는 이동 거리
        public static double LiftSpeed = 100; // 리프트 동작 속도
        public static double PickupSpeed = 100; // 픽업 상하 동작 속도
        public static double ShuttleSpeed = 100; // 셔틀 전후진 동작 속도
        public static double InverterSpeed = 10; // 반전기 회전 동작 속도
        public static double LiftHomeSpeed = 10000; // 리프트 호밍 동작 속도
        public static double LiftAccDec = 10; // 리프트 동작 가감속
        public static double LiftJerk = LiftAccDec * LiftAccDec; // 리프트 동작 Jerk
        //public static double SiteWidthOffset = 2; // 사이트에서 PCB 받을 때 클램프 전 폭
        //public static bool UseVac = true; // 그립에서 진공으로 개조 관련.

        public static bool CoolingByTime = false; // 시간으로 PCB 쿨링할지
        public static int CoolingTime = 10; // 시간 쿨링 경우 설정시간
        public static bool CoolingByTemperature = false; // 온도로 쿨링 여부
        public static int CoolingTemperature = 30; // PCB쿨링 설정 온도
        public static int CoolingMaxTime = 30; // PCB쿨링 최대 시간. 경과시 에러처리
        public static Stopwatch CoolingWatch = new Stopwatch(); // 쿨링 경과 시간
        public static bool CoolingFinish = false; // 해당 PCB 쿨링 완료 여부 
        public static bool USECooling = false; // 해당 PCB 쿨링 사용여부 by DG

        // 교대조 시간 설정
        public static int ShiftAHour = 9;
        public static int ShiftAMin = 0;
        public static int ShiftBHour = 15;
        public static int ShiftBMin = 0;
        public static int ShiftCHour = 23;
        public static int ShiftCMin = 0;
        public static string NowShift = "A";
        public static bool UseShiftC = true; // 3교대 유무

        public static Bitmap ArrayBitmap = null;
        #endregion

        #region AutoInline 전역변수
        #region 세대별 변수

        public static enumInlineType InlineType = enumInlineType.Gen3;  // 현재 세대 설정 변수

        #endregion  
        // 읽기 전용 계산 프로퍼티: InlineType 변경 시 자동 반영
        public static int MaxTestPCCount        // 각 Test PC의 갯수
        {
            get
            {
                switch (InlineType)
                {
                    case enumInlineType.Gen1:
                    case enumInlineType.Gen2:
                    case enumInlineType.Gen3:
                    case enumInlineType.Gen4:
                        return 7;   // 1~4세대
                    case enumInlineType.Gen5:
                        return 9;   // 5세대
                    case enumInlineType.Gen6:
                        return 1;   // 6세대
                                    // 지정 없으면 기본값(필요시 조정)
                    default:
                        return 7;   
                }
            }
        }
      
        // 세대별 통신COM 설정을 위한 맵핑
        /// <summary>
        /// siteIndex(0-base, Site1→0 … Site26→25)를 세대별 규칙으로 ComSMD 인덱스로 변환.
        /// - Gen6 이상 : 통합검사 → 항상 0 (DL/FT 구분 없음)
        /// - Gen5     : DL=0, FT 전용 사이트만 해당 FT 포트 인덱스 반환, 그 외는 0
        /// - Gen1~4   : DL=0, FT 전용 사이트만 해당 FT 포트 인덱스 반환, 그 외는 0
        /// </summary>
        public static int MapSmdIndex(int siteIndex)
        {
            if (siteIndex < 0) return 0;

            // Gen6+ : 통합검사 — COM 1개만 사용
            if (InlineType >= enumInlineType.Gen6)
                return 0;

            // Gen5 매핑
            if (InlineType == enumInlineType.Gen5)
            {
                // Site 번호는 0-base (Site1→0)
                // FT 전용 포트 맵
                //   [1] : Site11(10)  Front FT1
                //   [2] : Site12(11)  Front FT2
                //   [3] : Site13(12)  Front FT3
                //   [4] : Site10(9)   Front FT4
                //   [5] : Site24(23)  Rear  FT1
                //   [6] : Site25(24)  Rear  FT2
                //   [7] : Site26(25)  Rear  FT3
                //   [8] : Site23(22)  Rear  FT4
                switch (siteIndex)
                {
                    case 10: return 1; // Site11_F_FT1
                    case 11: return 2; // Site12_F_FT2
                    case 12: return 3; // Site13_F_FT3
                    case 9: return 4; // Site10_F_FT4
                    case 23: return 5; // Site24_R_FT1
                    case 24: return 6; // Site25_R_FT2
                    case 25: return 7; // Site26_R_FT3
                    case 22: return 8; // Site23_R_FT4
                    default: return 0; // 나머지는 DL 포트
                }
            }

            // Gen1~4 매핑
            //   [1] : Site11(10)  Front FT1
            //   [2] : Site12(11)  Front FT2
            //   [3] : Site13(12)  Front FT3
            //   [5] : Site24(23)  Rear  FT1
            //   [6] : Site25(24)  Rear  FT2
            //   [7] : Site26(25)  Rear  FT3
            // (빈 번호는 하드웨어 포트 구성상 공백)
            switch (siteIndex)
            {
                case 10: return 1; // Site11_F_FT1
                case 11: return 2; // Site12_F_FT2
                case 12: return 3; // Site13_F_FT3
                case 23: return 5; // Site24_R_FT1
                case 24: return 6; // Site25_R_FT2
                case 25: return 7; // Site26_R_FT3
                default: return 0; // 나머지는 DL 포트
            }
        }

        //public static int MaxTestPCCount = 7; // 각 Test PC의 갯수
        public static int MaxSiteCount = 26; // 전체 Test site 갯수
        public static int MaxDTSiteCount = 20; // 전체 Test site 갯수
        public static int MaxFTSiteCount = 6; // 전체 Test site 갯수

        public static int SitePerRack = 13; // 리프트 기준으로 렉 지정.
        public static int SiteDTPerRack = 10; // 리프트 기준으로 렉 지정. 다운로드테스트 층
        public static int SiteFTPerRack = 3; // 리프트 기준으로 렉 지정. 다운로드테스트 층

        public static int MaxArrayCount = 6; // 각 Site 의 array 갯수
        public static bool PassMode = false;
        public static bool PassModeBuffer = false; // PassMode시 사이트를 버퍼로 활용
        public static bool LeaveOneSite = false; // 빠른 Resite 위해 사이트 하나 비우기
        public static bool SimulationMode = false;
        public static bool CycleStop = false;
        public static enumBuyerChange BuyerChange = FuncInline.enumBuyerChange.White; // 시스템에 표시되는 모드
        public static enumBuyerChange BuyerChange_Before = FuncInline.enumBuyerChange.White; // 직전 가동했던 모드
        //public static enumBuyerChange BuyerChangeReal = FuncInline.enumBuyerChange.White; // 실제 작동에 관계되는 모드. white라도 orange로 남아 있는 PC 있을 수 있다.
        public static bool BuyerChangeBeforeCheck = false; // Blue 모드 전 거래선 완료 체크 여부
        public static bool BuyerChangeBeforeEnd = false; // Blue 모드 전 거래선 완료여부
        public static bool[] BuyerChangeOrange = new bool[MaxTestPCCount]; // 바이어체인지를 Orange로 진행시 PC별 불량 여부, 하나라도 양품되면 white로 변경.
        public static enumTeachingPos[] BuyerChangeSite = new enumTeachingPos[MaxTestPCCount]; // 바이어체인지 Orange시 테스트 진행할 지정 사이트
        public static bool[] BuyerChangeBlock = new bool[40]; // BuyerChange 중 Block 사유 발생. White 전환 후 Block 해야 함.
        public static double TestCycleTime = 0;
        public static string[] TestErrorCode = new string[1000];
        public static bool[] TestErrorRetest = new bool[1000];
        public static string[] TestErrorRetestType = new string[1000];
        public static double DefectLimit = 0;
        public static int DefectBlockMinIn = 100; // 불량율 블럭 적용시 최소 검사수
        public static int DefectBlockMinNG = 3; // 불량율 블럭 적용시 최소 NG수
        public static int PinLogTime = 10; // 핀로그 저장 분
        public static string PinLogDirectory = "D:\\SamsungMobile\\GlobalSIA\\Log"; // 핀로그 저장 경로

        public static int PinLifeDate = 1000;
        public static int PinLifeCount = 1000;
        public static bool CheckPinLife = false;

        //public static int ProductCanCount = 0;  // Can 생산 카운트
        //public static int ProductBoxCount = 0; // Box 생산 카운트
        public static int ArrayInputCount = 0;
        public static int ArrayPassCount = 0;
        public static int ArrayDefectCount = 0;
        public static int PCBInputCount = 0;
        public static int PCBPassCount = 0;
        public static int PCBDefectCount = 0;
        public static double DefectRate = 0;
        public static double ModulePitch = 210; // 모듈간 높이 간격
        public static double UpDownPitch = 60; // 리프트 상하 높이 간격
        public static double JigBufferPitch = 20; // 지그버퍼 리프트 층간 간격
        public static int JigBufferMax = 25; // 지그 버퍼 리프트 최대 층수

        // 삼성 시스템 로그용 저장경로
        public static string SystemLogPath = "C:\\FA";
        public static string SystemLogFileName = "System.dat";
        public static int SystemLogTime = 10; // 시스템 로그 저장 분
        public static bool SystemLogSave = false; // 시스템 로그 저장해야 할 경우

        public static bool[] InitStarted = new bool[Enum.GetValues(typeof(FuncInline.enumLoading)).Length]; // 시스템초기화창 진행 단계
        public static bool[] InitDone = new bool[Enum.GetValues(typeof(FuncInline.enumLoading)).Length]; // 시스템초기화창 진행 단계 완료 여부
        public static bool[] InitialStarted = new bool[Enum.GetValues(typeof(FuncInline.enumInitialize)).Length]; // Initialize 진행 단계
        public static bool[] InitialDone = new bool[Enum.GetValues(typeof(FuncInline.enumInitialize)).Length]; // Initialize 진행 단계 완료 여부
        public static bool[] WidthStarted = new bool[Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length + 2]; // 폭 Initialize 진행 단계, 오토닉스 + 사이트 + 지그버퍼폭. 생성시 변수로 지정하면 0이 되어 상수로 입력
        public static bool[] WidthDone = new bool[Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length + 2]; // Initialize 진행 단계 완료 여부, 오토닉스 + 사이트 + 지그버퍼폭. 생성시 변수로 지정하면 0이 되어 상수로 입력

        public static int[] PinArray = new int[MaxArrayCount];

        //public static enumDryRunMethod DryRunMethod = FuncInline.enumDryRunMethod.None;
        //public static int DryRunSite = 1;
        public static double DefaultSiteWidth = 250; // 파스텍 스텝모터 폭 계산할 때 원점 기준 기본 PCB 폭
        public static double DefaultPCBWidth = 250; // 오토닉 스텝모터 폭 계산할 때 원점 기준 기본 PCB 폭
        public static double DefaultPCBLength = 250; // 스텝모터 폭 계산할 때 원점 기준 기본 PCB 길이
        public static double SiteClampDelay = 1; // 사이트 클램프시 딜레이
        public static double WidthClampOffset = 2; // 티칭 대비 폭 넓히는 간격. 티칭은 PCB 폭에 타이트하게 하게, Run시 Offset만큼 더 벌려서 운영
        public static double ConveyorClampDelay = 1.5; // 컨베어 클램핑 전 컨베어 가동 딜레이
        public static double PCBCarrierAssembleDelay = 3; // 캐리어에 PCB 결합할 때 하강 후 누르는 시간
        public static double MinWidth = 50; // 폭 조절시 최소 폭
        public static double MaxWidth = 270; // 폭 조절시 최대 폭

        public static int ArraySeq = 1; // Simulation시 가상 코드 순번

        public static bool Temperature_Connect_Fail = false;//온도 확인 485 연결 확인
        public static bool HandScanner_Connect_Fail = false;//핸드 바코드 232 연결 확인

        public static double Temperature_NUX_PV = 0; // 온도 값(한영넉스)
        public static double Temperature_NUX_SV = 0; // 온도 값(한영넉스)
        public static double Temperature_NS10LT_PV = 0; // 온도 값(NS10LT)
        //public static string HandScanner = ""; // 핸드 스캐너 저장 값

        public static bool Loto_Use = true; // Loto Switch 사용 여부. false 일 때 DO/Servo/Step 지령 안 듣게
        public static bool Loto_Switch = false; // Loto Switch 입력 여부. true 일 때 DO/Servo/Step 지령 안 듣게

        //public static enumNGAlarmStatus NgAlarmStatus = FuncInline.enumNGAlarmStatus.None;
        public static Stopwatch NgAlarmWatch = new Stopwatch();
        public static enumTeachingPos NGDetailPos = enumTeachingPos.NgBuffer; // NG 상세정보창 조회할 위치

        public static Stopwatch PinLogWatch = new Stopwatch(); // Pin 로그 주기 체크용
        public static bool ManualStop = false; // 메뉴얼 동작 중 외부함수에서 인터락 등으로 멈출 경우 메뉴얼 타이머 멈추기 위한 변수

        public static bool Use_InAndOut = false; // 사이트 배출/투입 동시 처리

        public static int[] OCROrder = new int[MaxArrayCount]; // OCR 테스트 순번 변경 테스트 위해
        public static bool[] ScanBad = new bool[MaxArrayCount]; // GoodMark / BadMark 테트스 위해
        public static bool[] TestFail = new bool[MaxArrayCount]; // 어레이별 테스트 결과 시뮬레이션 위해



        #region Serial 통신 관련
        // --- Added: Scanner COM settings (separate from Test PCs) ---
        public static int PortScanner = 0;
        public static int BaudScanner = 115200;
        public static Parity ParityScanner = Parity.None;
        public static StopBits StopBitsScanner = StopBits.One;
        // -------------------------------------------------------------

        public static int ComSMDCount = MaxTestPCCount;  //0 - DL / 1,2,3,4 Front FT / 5,6,7,8 Rear FT
        public static SMDSerial[] ComSMD = new SMDSerial[ComSMDCount]; // TestPC의 통신 클래스
       //public static PMCClass[] ComPMC = new PMCClass[Enum.GetValues(typeof(FuncInline.enumPMCMotion)).Length]; // 일반컨베어 폭조절 모터 통신 클래스
        public static structPMCCommand ComCommand = new structPMCCommand(); // 스텝모터 지령. 큐에서 꺼내기 전에 먼저 확인한다.
        #endregion


        #region SMD Test 통신 설정
        public static int[] PortTest = new int[ComSMDCount]; // Test PC  통신 포트
        public static int[] PortPMC = new int[Enum.GetValues(typeof(FuncInline.enumPMCMotion)).Length]; // 컨베어 폭조절 모터 통신 포트
        public static int[] NodePMC = new int[Enum.GetValues(typeof(FuncInline.enumPMCMotion)).Length]; // 컨베어 폭조절 모터 Node ID
        public static int[] BaudTest = new int[ComSMDCount]; // Test PC  통신 속도
        public static int[] BaudPMC = new int[Enum.GetValues(typeof(FuncInline.enumPMCMotion)).Length]; // 컨베어 폭조절 모터  통신 속도
        public static Parity[] ParityTest = new Parity[ComSMDCount]; // Test PC  통신 Parity
        public static Parity[] ParityPMC = new Parity[Enum.GetValues(typeof(FuncInline.enumPMCMotion)).Length]; // 컨베어 폭조절 모터 통신 Parity
        public static StopBits[] StopBitsTest = new StopBits[ComSMDCount]; // Test PC  통신 Stopbit
        public static StopBits[] StopBitsPMC = new StopBits[Enum.GetValues(typeof(FuncInline.enumPMCMotion)).Length]; // 컨베어 폭조절 모터 통신 Stopbit
        public static int PortTemperature = 6; // 온도센서 통신 포트
        public static int BaudTemperature = 9600; // 온도센서 통신 속도 
        public static Parity ParityTemperature = Parity.Even;
        public static StopBits StopBitsTemperature = StopBits.One;

        #endregion

        #region 티칭 좌표
        public static structPosition[] ScanTeachingPos = new structPosition[Enum.GetValues(typeof(FuncInline.enumScanPos)).Length]; // 좌표 티칭값
        public static double[] TeachingWidth = new double[Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length + 2]; // 폭조절 티칭값, 스텝모터 + 사이트 + 서보. 생성시 변수로 지정하면 0이 되어 상수로 입력
        public static double[] OffsetWidth = new double[Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length + 2]; // 폭조절 옵셋값. 실제위치와 폭을 일치, 스텝모터 + 사이트 + 서보. 생성시 변수로 지정하면 0이 되어 상수로 입력
        public static double[,] LiftPos = new double[Enum.GetValues(typeof(FuncInline.enumLiftName)).Length, Enum.GetValues(typeof(FuncInline.enumLiftPos)).Length]; // 리프트 위치값
        public static double[,] ShuttlePos = new double[Enum.GetValues(typeof(FuncInline.enumShuttleName)).Length, Enum.GetValues(typeof(FuncInline.enumShuttlePos)).Length]; // 셔틀 위치값
        public static double[] EtcPos = new double[Enum.GetValues(typeof(FuncInline.enumETCPos)).Length]; // 기타 위치값
        #endregion

        #region PCB 이송 관련
        public static bool LiftPCBCheck = true; // 리프트에서 사용자 개입으로 PCB 조작시 에러처리할 것인가?

        public static PCBInfoClass[] PCBInfo = new PCBInfoClass[Enum.GetValues(typeof(enumTeachingPos)).Length]; // 위치별 PCB의 스캔값 및 처리 정보
        public static int[] TestTime = new int[26];

        public static ConcurrentQueue<enumLiftAction> MoveActionQueue = new ConcurrentQueue<enumLiftAction>(); // PCB 컨베어 이송 지령 큐, 메인라인의 PCB 이송 관련해서 큐로 관리하고 나머지는 쓰레드에서 직접 관리해야 할 듯
        public static enumLiftAction MoveAction = FuncInline.enumLiftAction.Waiting; // 현재 동작중인 이송 지령

        public static int[] SiteInputCount = new int[26]; // 각 사이트별 투입 카운트. 카운트가 가장 작은 사이트 중에 순서대로 투입한다. 보이지 않고 투입순서를 예측할 수 없는 부작용이 많아 삭제
        public static int SearchInputIndex = 0; // 투입 체크할 사이트 순번, 최종 투입한 사이트. 참조용. 좌우 전환 기준으로 동적으로 변화시킴
        public static int[] SiteInputIndex = new int[MaxTestPCCount]; //투입 체크할 렉별 사이트 순번.

        public static int SearchOutputIndex = 0; // 양품 배출 체크할 사이트 순번, 최종 배출한 사이트
        public static int[] SiteOutputIndex = new int[MaxTestPCCount]; //양품 배출 체크할 렉별 사이트 순번.
        public static int LeftInputIndex = 0; // 투입 체크할 좌측 사이트 순번.
        public static int RightInputIndex = 0; // 투입 체크할 우측 사이트 순번.

        public static int LeftOutputIndex = 0; // 렉1 양품 배출 체크할 사이트 순번
        public static int RightOutputIndex = 0; // 렉2 양품 배출 체크할 사이트 순번
        public static int SearchNGIndex = 0; // 불량 배출 체크할 사이트 순번, 최종 배출한 사이트
        public static int[] SiteNgIndex = new int[MaxTestPCCount]; //불량 배출 체크할 렉별 사이트 순번.
        public static int LeftNGIndex = 0; // 렉1 불량 배출 체크할 사이트 순번
        public static int RightNGIndex = 0; // 렉2 불량 배출 체크할 사이트 순번
        public static enumTeachingPos[] SiteOrder = // 투입 순서. 좌우 번갈아
        {
            enumTeachingPos.Site1_F_DT1,
            enumTeachingPos.Site14_R_DT1,
            enumTeachingPos.Site2_F_DT2,
            enumTeachingPos.Site15_R_DT2,
            enumTeachingPos.Site3_F_DT3,
            enumTeachingPos.Site16_R_DT3,
            enumTeachingPos.Site4_F_DT4,
            enumTeachingPos.Site17_R_DT4,
            enumTeachingPos.Site5_F_DT5,
            enumTeachingPos.Site18_R_DT5,
            enumTeachingPos.Site6_F_DT6,
            enumTeachingPos.Site19_R_DT6,
            enumTeachingPos.Site7_F_DT7,
            enumTeachingPos.Site20_R_DT7,
            enumTeachingPos.Site8_F_DT8,
            enumTeachingPos.Site21_R_DT8,
            enumTeachingPos.Site9_F_DT9,
            enumTeachingPos.Site22_R_DT9,
            enumTeachingPos.Site10_F_DT10_FT4,  // 5세대 FT4
            enumTeachingPos.Site23_R_DT10_FT4,
            enumTeachingPos.Site11_F_FT1,
            enumTeachingPos.Site24_R_FT1,
            enumTeachingPos.Site12_F_FT2,
            enumTeachingPos.Site25_R_FT2,
            enumTeachingPos.Site13_F_FT3,
            enumTeachingPos.Site26_R_FT3

        };
        public static enumTeachingPos[] SiteDTOrder = // 다운로드투입 순서. 좌우 번갈아
        {
            enumTeachingPos.Site1_F_DT1,
            enumTeachingPos.Site14_R_DT1,
            enumTeachingPos.Site2_F_DT2,
            enumTeachingPos.Site15_R_DT2,
            enumTeachingPos.Site3_F_DT3,
            enumTeachingPos.Site16_R_DT3,
            enumTeachingPos.Site4_F_DT4,
            enumTeachingPos.Site17_R_DT4,
            enumTeachingPos.Site5_F_DT5,
            enumTeachingPos.Site18_R_DT5,
            enumTeachingPos.Site6_F_DT6,
            enumTeachingPos.Site19_R_DT6,
            enumTeachingPos.Site7_F_DT7,
            enumTeachingPos.Site20_R_DT7,
            enumTeachingPos.Site8_F_DT8,
            enumTeachingPos.Site21_R_DT8,
            enumTeachingPos.Site9_F_DT9,
            enumTeachingPos.Site22_R_DT9,
            enumTeachingPos.Site10_F_DT10_FT4,  // 5세대 FT4
            enumTeachingPos.Site23_R_DT10_FT4




        };
        public static enumTeachingPos[] SiteFTOrder = // 펑션투입 순서. 좌우 번갈아
       {

            enumTeachingPos.Site10_F_DT10_FT4,  // 5세대 FT4
            enumTeachingPos.Site23_R_DT10_FT4,
            enumTeachingPos.Site11_F_FT1,
            enumTeachingPos.Site24_R_FT1,
            enumTeachingPos.Site12_F_FT2,
            enumTeachingPos.Site25_R_FT2,
            enumTeachingPos.Site13_F_FT3,
            enumTeachingPos.Site26_R_FT3

        };
        public static int FrontScanCount = 0; // 스캔한 횟수. 일정횟수 이상 경과시 스캔 실패로 처리
        public static int RearScanCount = 0; // 스캔한 횟수. 일정횟수 이상 경과시 스캔 실패로 처리
        public static enumLiftAction InConveyorAction = FuncInline.enumLiftAction.Waiting; // 투입 컨베어 동작
        public static enumShuttleAction InShuttleAction = FuncInline.enumShuttleAction.Waiting; // 인셔틀 동작
        public static enumLiftAction FrontPassLineAction = FuncInline.enumLiftAction.Waiting; // FrontPassLine 동작
        public static enumLiftAction FrontScanSiteAction = FuncInline.enumLiftAction.Waiting; // FrontScanSite 동작
    
        public static enumLiftAction Lift1Action = FuncInline.enumLiftAction.Waiting; // 리프트1 동작
        public static enumLiftAction Lift2Action = FuncInline.enumLiftAction.Waiting; // 리프트2 동작
        public static enumLiftAction RearPassLineAction = FuncInline.enumLiftAction.Waiting; // RearPassLine 동작
        public static enumLiftAction RearNGLineAction = FuncInline.enumLiftAction.Waiting; // RearNGLine 동작
        public static enumShuttleAction OutShuttleUpAction = FuncInline.enumShuttleAction.Waiting; // 배출셔틀 동작
        public static enumShuttleAction OutShuttleDownAction = FuncInline.enumShuttleAction.Waiting; // 배출셔틀 동작
        public static enumLiftAction OutConveyorAction = FuncInline.enumLiftAction.Waiting; // InPickup 동작
        public static enumLiftAction NGBufferAction = FuncInline.enumLiftAction.Waiting; // InPickup 동작
      
        public static enumLiftAction ScanAction = FuncInline.enumLiftAction.Waiting; // ScanX,Y 동작

        public static bool NGClear = false; // 시뮬레이션시 NG 정보 자동 삭제 여부
        public static bool NGOut = false; // NG1 에서 NG2로 이송중
       
        public static Stopwatch NGOutWatch = new Stopwatch(); // NG컨베어 이송 및 변경시 센서 감지 지연시간
        public static bool UseSiteClampSensor = false; // 사이트클램프 실린더 센서 사용 유무

        public static bool[] CheckPCB = new bool[Enum.GetValues(typeof(enumTeachingPos)).Length]; // 각 사이트 PCB 공급 유무
        public static Stopwatch[] SiteCheckTime = new Stopwatch[40]; // 각 사이트 PCB 공급 후 경과 시간. 동작시작 전에 start, 동작 완료 후 reset
        //public static Stopwatch[] SiteConvTime = new Stopwatch[Enum.GetValues(typeof(enumTeachingPos)).Length]; // 각 사이트 컨베어 구동 경과 시간
        public static bool[] NeedPartClear = new bool[Enum.GetValues(typeof(FuncInline.enumErrorPart)).Length]; // 에러난 경우 파트클리어 해야 시작할 수 있도록

        public static ConcurrentQueue<string> NgQueue = new ConcurrentQueue<string>(); // 메일 출력용 NG 리스트
        public static bool InputPCB = false; // 일회성으로 PCB를 투입할 수 있도록 한다. InputLift는 상단 대기. 로봇 인터락은 상황 따라 판단
        public static bool UseSMDReady = false; // RD 체크 사용할 것인가?

        public static bool[] InputStop = new bool[MaxTestPCCount]; // PC별 투입 정지
        public static bool OutputStop = false; // 뒷 라인으로 배출 금지

        public static double RobotJogSpeed = 50;
        public static double WidthJogSpeed = 5;


        #region StopWatch
  
        public static Stopwatch InConveyorActionWatch = new Stopwatch(); // 투입컨베어 동작 시간
        public static Stopwatch InShuttleActionWatch = new Stopwatch(); // 투입컨베어 동작 시간
        public static Stopwatch FrontPassLineActionWatch = new Stopwatch(); // 패스라인1 동작 시간
        public static Stopwatch FrontScanSiteActionWatch = new Stopwatch(); // 스캔 동작 시간
        public static Stopwatch Lift1ActionWatch = new Stopwatch(); // 리프트1 동작 시간
        public static Stopwatch Lift2ActionWatch = new Stopwatch(); // 리프트2 동작 시간
        public static Stopwatch RearPassLineActionWatch = new Stopwatch(); // 리어패스라인 동작 시간
        public static Stopwatch RearNGLineActionWatch = new Stopwatch(); // 리어NG라인 동작 시간
        public static Stopwatch OutShuttleUpActionWatch = new Stopwatch(); // 아웃셔틀 업 동작 시간
        public static Stopwatch OutShuttleDownActionWatch = new Stopwatch(); // 아웃셔틀 업 동작 시간
        public static Stopwatch OutConveyorActionWatch = new Stopwatch(); // 배출컨베어 동작 시간
        public static Stopwatch NGBufferActionWatch = new Stopwatch(); // NG배출 동작 시간
      
        public static Stopwatch ScanActionWatch = new Stopwatch(); // ScanX,Y 동작
        
        //public static Stopwatch[] StepFlagWatch = new Stopwatch[Enum.GetValues(typeof(FuncInline.enumPMCMotion)).Length]; // flag 확인용.// 접속 및 이동 지령이 실행되면 1분간 flag 유지
        public static enumTeachingPos DetailSite = enumTeachingPos.Site1_F_DT1;
        #endregion

        #endregion

        #region HandyScanner
        public static int Vid = 0x046D;
        public static int Pid = 0xC534;
        //public static UsbDeviceFinder MyUsbFinder = new UsbDeviceFinder(Vid, Pid);
        //public static UsbDevice MyUsbDevice = UsbDevice.OpenUsbDevice(MyUsbFinder);
        #endregion

        #region Vision Scan 관련
        //public static VisionScan Scan = new VisionScan();
        public static string Scan_Ip = "127.0.0.1";
        public static int Scan_Port = 4000;
        public static IntPtr ScanPtr = IntPtr.Zero; // 비전 티칭 프로그램 핸들
        public static string ScanPath = "MultiCode_Mark_Inspection"; // 비전스캔 프로그램 폴더 이름
        public static string ScanExeName = "MultiCode_Mark_Inspection"; // 비전 티칭 프로그램 실행파일명
        public static string ScanImagePath = "D:\\TestTempImage"; // 비전 스캔 결과 이미지 저장 폴더
        public static string ScanModelPath = "Models"; // 비전 스캔 모델 설정 저장 폴더. 모델 삭제시 같이 삭제되게 한다.
        public static int ScanProgramX = 185; // 스캔 프로그램 위치
        public static int ScanProgramY = 130; // 스캔 프로그램 위치
        public static structPosition ScanSize = new structPosition(1100, 0, 6000, 5120); //new structPosition(8192, 5460, 0, 0); // 이미지중 실제 PCB가 정렬되는 기준값으로 해야 계산이 정확하게 된다.
        public static structPosition VisionSize = new structPosition(8192, 5460, 0, 0); // 비전 스캐너 해상도
        public static bool ScanInsertCheck = false;
        #endregion

        #region 스텝모터 변수
        public static FuncInline.structPMCStatus[] PMCStatus = new FuncInline.structPMCStatus[Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length]; // Lets 스텝모터 축 상태 저장

        #endregion
        #endregion

        #endregion

        #region SQL 관련

        #region SMD 테스트 관련

        public static void DeleteSQLLog() // SQL 전체 로그를 삭제한다. 시뮬레이션시 데이터용량이 너무 많아 버벅이므로
        {
            try
            {

                string sql = "delete from CommLog";
                GlobalVar.Sql.Execute(sql);

                sql = "delete from DefectLog";
                GlobalVar.Sql.Execute(sql);

                sql = "delete from ModuleLog";
                GlobalVar.Sql.Execute(sql);

                sql = "delete from PinLog";
                GlobalVar.Sql.Execute(sql);

                sql = "delete from SystemError";
                GlobalVar.Sql.Execute(sql);

                sql = "delete from TestResult";
                GlobalVar.Sql.Execute(sql);

                sql = "delete from PCBCount";
                GlobalVar.Sql.Execute(sql);

                sql = "delete from BlockHistory";
                GlobalVar.Sql.Execute(sql);
            }
            catch (Exception ex)
            {
                debug("UpdateTestResult : " + ex.ToString());
                debug(ex.StackTrace);
            }
        }


        public static void InsertBlockHistory(int site, bool use, string content)
        {
            try
            {

                string sql = "insert into [BlockHistory] " +
                        " values (" +
                        "CONVERT(CHAR(8), getdate(), 112), " + // Date
                        "CONVERT(CHAR(8), getdate(), 108), " + // Time
                        "'" + site + "', " +
                        "'" + (use ? "1" : "0") + "', " +
                        "'" + content + "'," +
                        "'')";
                GlobalVar.Sql.Execute(sql);

            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        public static string[,] GetBlockHistory(int site)
        {
            if (GlobalVar.Sql == null ||
                !GlobalVar.Sql.connected)
            {
                return null;
            }

            if (!GlobalVar.UseMsSQL)
            {
                return null;
            }

            try
            {
                string sql = "Select [Date],[Time],[Use],[Content],[Comment] " +
                            "From BlockHistory " +
                            "Where [Site] = '" + site + "' " +
                            "Order by [Date] desc, [Time] desc";
                //debug(sql);
                return GlobalVar.Sql.Read(sql);
            }
            catch (Exception ex)
            {
                FuncLog.WriteLog_Debug("GetBlockHistory " + ex.ToString());
                FuncLog.WriteLog_Debug(ex.StackTrace);
            }
            return null;
        }

        public static void IncreasePCBCount(string date, string time, bool input, bool pass, bool ng, bool isPBA)
        {
            try
            {
                string shift = "A";
                //*
                #region 투입시간 기준 조 계산. A조 시작 전 시간대면 날짜에서 하루 뺀 날짜로 입력해야 한다.
                int aTime = FuncInline.ShiftAHour * 100 + FuncInline.ShiftAMin; // shift A 기준시간
                int bTime = FuncInline.ShiftBHour * 100 + FuncInline.ShiftBMin; // shift A 기준시간
                int cTime = FuncInline.ShiftCHour * 100 + FuncInline.ShiftCMin; // shift A 기준시간
                int inputTime = int.Parse(time.Replace(":", "").Substring(0, 4));
                if (inputTime >= aTime &&
                    inputTime < bTime)
                {
                    shift = "A";
                }
                else if (FuncInline.UseShiftC && // 3교대시 B조는 C조 시간 이전까지
                    inputTime >= bTime &&
                    inputTime < cTime)
                {
                    shift = "B";
                }
                else if (!FuncInline.UseShiftC && // 2교대시 B조는 B조 시간이후거나 A조 시간 이전
                    (inputTime >= bTime ||
                            inputTime < aTime))
                {
                    shift = "B";
                    #region 하루 전 날짜로
                    if (inputTime < aTime)
                    {
                        int year = int.Parse(date.Substring(0, 4));
                        int month = int.Parse(date.Substring(4, 2));
                        int day = int.Parse(date.Substring(6, 2));
                        date = (new DateTime(year, month, day)).AddDays(-1).ToString("yyyyMMdd");
                    }
                    #endregion
                }
                else
                {
                    shift = "C";
                    #region 하루 전 날짜로
                    if (inputTime < aTime)
                    {
                        int year = int.Parse(date.Substring(0, 4));
                        int month = int.Parse(date.Substring(4, 2));
                        int day = int.Parse(date.Substring(6, 2));
                        date = (new DateTime(year, month, day)).AddDays(-1).ToString("yyyyMMdd");
                    }
                    #endregion
                }
                #endregion

                // 데이터 있는지 확인
                bool exist = false;
                string sql = "select count(*) " +
                                "from [PCBCount] " +
                                "where [Date] = '" + date + "' " +
                                "and [Shift] = '" + shift + "' " +
                                "and [PBA] = '" + (isPBA ? "1" : "0") + "' ";
                string[,] res = GlobalVar.Sql.Read(sql);
                if (res != null &&
                    res.GetLength(0) > 0 &&
                    res.GetLength(1) > 0)
                {
                    int cnt = 0;
                    int.TryParse(res[0, 0], out cnt);
                    if (cnt > 0)
                    {
                        exist = true;
                    }
                }

                // 없으면 insert
                if (!exist)
                {
                    sql = "insert into [PCBCount] " +
                            " values (" +
                            "'" + date + "', " +
                            "'" + shift + "', " +
                            (input ? "1" : "0") + ", " +
                            (pass ? "1" : "0") + ", " +
                            (ng ? "1" : "0") + ", " +
                            (isPBA ? "1" : "0") + " " +
                            ")";
                    GlobalVar.Sql.Execute(sql);
                }
                else
                {
                    sql = "update [PCBCount] " +
                            " set " +
                            (input ? "[Input] = [Input] + 1" : "") + " " +
                            (pass ? "[Pass] = [Pass] + 1" : "") + " " +
                            (ng ? "[NG] = [NG] + 1" : "") + " " +
                            "where [Date] = '" + date + "' " +
                            "and [Shift] = '" + shift + "' " +
                            "and [PBA] = '" + (isPBA ? "1" : "0") + "' ";
                    GlobalVar.Sql.Execute(sql);
                }
                //*/
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        public static string[,] GetDefectResultBySiteArrayCode(string startDate, string endDate, bool shiftA, bool shiftB, bool shiftC) // SMD defect별 Defect 결과 조회
        {
            if (GlobalVar.Sql == null ||
                !GlobalVar.Sql.connected)
            {
                return null;
            }

            if (!GlobalVar.UseMsSQL)
            {
                return null;
            }

            try
            {
                // 2교대시 A조는 해당날짜, B조는 해당날짜 B조시간부터 다음날 A조시간 이전까지
                // 3교대시 A/B조는 해당날짜, C조는 해당날짜 C조시간부터 다음날 A조 시간 이전까지.
                // 검색시 날짜+시간 조합으로 시작날짜 가장 먼저 조 시작시간부터
                //                           끝조 미선택시 마지막날 마지막 시간범위
                //                           끝조 선택시 마지막 다음날 첫조 시간 이전까지 범위로 검색
                // Trace 폼으로 들어올 때 마지막조고 A조 시간 이전이면 이전날짜로 세팅해야 함

                #region 검색 일시 범위 지정
                string startTime = startDate;
                if (shiftA)
                {
                    startTime += Util.IntToString(FuncInline.ShiftAHour, 2) + ":" + Util.IntToString(FuncInline.ShiftAMin, 2) + ":00";
                }
                else if (shiftB)
                {
                    startTime += Util.IntToString(FuncInline.ShiftBHour, 2) + ":" + Util.IntToString(FuncInline.ShiftBMin, 2) + ":00";
                }
                else if (shiftC)
                {
                    startTime += Util.IntToString(FuncInline.ShiftCHour, 2) + ":" + Util.IntToString(FuncInline.ShiftCMin, 2) + ":00";
                }
                string endTime = endDate;
                #region 마지막조 포함시
                if ((!FuncInline.UseShiftC && shiftB) ||
                    (FuncInline.UseShiftC && shiftC))
                {
                    // 종료 날짜의 다음날 A조 시작 전 시간을 구한다.
                    int searchYear = int.Parse(endDate.Substring(0, 4));
                    int searchMonth = int.Parse(endDate.Substring(4, 2));
                    int searchDate = int.Parse(endDate.Substring(6, 2));
                    int searchHour = FuncInline.ShiftAHour;
                    int searchMin = FuncInline.ShiftAMin;
                    DateTime searchDateTime = new DateTime(searchYear, searchMonth, searchDate, searchHour, searchMin, 0);
                    searchDateTime = searchDateTime.AddDays(1);
                    searchDateTime = searchDateTime.AddSeconds(-1);
                    endTime = searchDateTime.ToString("yyyyMMddHH:mm:ss");
                }
                #endregion
                #region 마지막조 미포함시
                else
                {
                    // 종료 날짜의 선택된 마지막 조 종료 시간을 구한다.
                    int searchYear = int.Parse(endDate.Substring(0, 4));
                    int searchMonth = int.Parse(endDate.Substring(4, 2));
                    int searchDate = int.Parse(endDate.Substring(6, 2));
                    int searchHour = FuncInline.ShiftAHour;
                    int searchMin = FuncInline.ShiftAMin;
                    // A조 선택시 B조시간
                    if (shiftA)
                    {
                        searchHour = FuncInline.ShiftBHour;
                        searchMin = FuncInline.ShiftBMin;
                    }
                    // B조 선택시 C조시간
                    if (shiftB)
                    {
                        searchHour = FuncInline.ShiftCHour;
                        searchMin = FuncInline.ShiftCMin;
                    }
                    DateTime searchDateTime = new DateTime(searchYear, searchMonth, searchDate, searchHour, searchMin, 0);
                    searchDateTime = searchDateTime.AddSeconds(-1); // 1분을 빼서 조 마지막 시간으로 세팅
                    endTime = searchDateTime.ToString("yyyyMMddHH:mm:ss");
                }
                #endregion
                #endregion

                string sql = "select a.[Site], a.[Array], a.[DefectCode], count(a.[DefectCode]) as DefectCount " +
                            "from [TestResult] a " +
                            "where concat(a.[Date], a.[Time]) >= '" + startTime + "' " +
                            "and concat(a.[Date], a.[Time]) <= '" + endTime + "' " +
                            "and a.[Finish] = 1 " +
                            "and DefectCode <> '' " +
                            "and DefectCode <> '-1' " +
                            "group by a.[Site], a.[Array], a.[DefectCode]";
                //debug(sql);
                return GlobalVar.Sql.Read(sql);
            }
            catch (Exception ex)
            {
                debug("GetDefectResultBySiteArrayCode" + ex.ToString());
                debug(ex.StackTrace);

                FuncLog.WriteLog_Debug("GetDefectResultBySiteArrayCode" + ex.ToString());
                FuncLog.WriteLog_Debug(ex.StackTrace);
            }
            return null;
        }

        public static string[,] GetDefectResultBySiteArrayCode(string startDate, string endDate) // SMD defect별 Defect 결과 조회
        {
            if (GlobalVar.Sql == null ||
                !GlobalVar.Sql.connected)
            {
                return null;
            }

            if (!GlobalVar.UseMsSQL)
            {
                return null;
            }

            try
            {
                string sql = "select a.[Site], a.[Array], a.[DefectCode], count(a.[DefectCode]) as DefectCount " +
                            "from [TestResult] a " +
                            "where a.[Date] >= '" + startDate + "' " +
                            "and a.[Date] <= '" + endDate + "' " +
                            "and a.[Finish] = 1 " +
                            "group by a.[Site], a.[Array], a.[DefectCode]";
                //debug(sql);
                return GlobalVar.Sql.Read(sql);
            }
            catch (Exception ex)
            {
                debug("GetDefectResultBySiteArrayCode" + ex.ToString());
                debug(ex.StackTrace);

                FuncLog.WriteLog_Debug("GetDefectResultBySiteArrayCode" + ex.ToString());
                FuncLog.WriteLog_Debug(ex.StackTrace);
            }
            return null;
        }
        public static string[,] GetDefectResultByCode(string startDate, string endDate, bool shiftA, bool shiftB, bool shiftC) // SMD defect별 Defect 결과 조회
        {
            if (GlobalVar.Sql == null ||
                !GlobalVar.Sql.connected)
            {
                return null;
            }

            if (!GlobalVar.UseMsSQL)
            {
                return null;
            }

            try
            {
                // 2교대시 A조는 해당날짜, B조는 해당날짜 B조시간부터 다음날 A조시간 이전까지
                // 3교대시 A/B조는 해당날짜, C조는 해당날짜 C조시간부터 다음날 A조 시간 이전까지.
                // 검색시 날짜+시간 조합으로 시작날짜 가장 먼저 조 시작시간부터
                //                           끝조 미선택시 마지막날 마지막 시간범위
                //                           끝조 선택시 마지막 다음날 첫조 시간 이전까지 범위로 검색
                // Trace 폼으로 들어올 때 마지막조고 A조 시간 이전이면 이전날짜로 세팅해야 함

                #region 검색 일시 범위 지정
                string startTime = startDate;
                if (shiftA)
                {
                    startTime += Util.IntToString(FuncInline.ShiftAHour, 2) + ":" + Util.IntToString(FuncInline.ShiftAMin, 2) + ":00";
                }
                else if (shiftB)
                {
                    startTime += Util.IntToString(FuncInline.ShiftBHour, 2) + ":" + Util.IntToString(FuncInline.ShiftBMin, 2) + ":00";
                }
                else if (shiftC)
                {
                    startTime += Util.IntToString(FuncInline.ShiftCHour, 2) + ":" + Util.IntToString(FuncInline.ShiftCMin, 2) + ":00";
                }
                string endTime = endDate;
                #region 마지막조 포함시
                if ((!FuncInline.UseShiftC && shiftB) ||
                    (FuncInline.UseShiftC && shiftC))
                {
                    // 종료 날짜의 다음날 A조 시작 전 시간을 구한다.
                    int searchYear = int.Parse(endDate.Substring(0, 4));
                    int searchMonth = int.Parse(endDate.Substring(4, 2));
                    int searchDate = int.Parse(endDate.Substring(6, 2));
                    int searchHour = FuncInline.ShiftAHour;
                    int searchMin = FuncInline.ShiftAMin;
                    DateTime searchDateTime = new DateTime(searchYear, searchMonth, searchDate, searchHour, searchMin, 0);
                    searchDateTime = searchDateTime.AddDays(1);
                    searchDateTime = searchDateTime.AddSeconds(-1);
                    endTime = searchDateTime.ToString("yyyyMMddHH:mm:ss");
                }
                #endregion
                #region 마지막조 미포함시
                else
                {
                    // 종료 날짜의 선택된 마지막 조 종료 시간을 구한다.
                    int searchYear = int.Parse(endDate.Substring(0, 4));
                    int searchMonth = int.Parse(endDate.Substring(4, 2));
                    int searchDate = int.Parse(endDate.Substring(6, 2));
                    int searchHour = FuncInline.ShiftAHour;
                    int searchMin = FuncInline.ShiftAMin;
                    // A조 선택시 B조시간
                    if (shiftA)
                    {
                        searchHour = FuncInline.ShiftBHour;
                        searchMin = FuncInline.ShiftBMin;
                    }
                    // B조 선택시 C조시간
                    if (shiftB)
                    {
                        searchHour = FuncInline.ShiftCHour;
                        searchMin = FuncInline.ShiftCMin;
                    }
                    DateTime searchDateTime = new DateTime(searchYear, searchMonth, searchDate, searchHour, searchMin, 0);
                    searchDateTime = searchDateTime.AddSeconds(-1); // 1분을 빼서 조 마지막 시간으로 세팅
                    endTime = searchDateTime.ToString("yyyyMMddHH:mm:ss");
                }
                #endregion
                #endregion

                string sql = "select a.[DefectCode], b.ErrorName, count(a.[DefectCode]) as DefectCount " +
                "from [TestResult] a " +
                            "left outer join[TestErrorCode] b " +
                            "on a.[DefectCode] = b.[ErrorCode] " +
                            "where concat(a.[Date], a.[Time]) >= '" + startTime + "' " +
                            "and concat(a.[Date], a.[Time]) <= '" + endTime + "' " +
                            "and a.[Finish] = 1 " +
                            "and a.[DefectCode] <> '' " +
                            "and a.[DefectCode] <> '-1' " +
                            "group by a.[DefectCode], b.[ErrorName] " +
                            "order by a.[DefectCode]";
                //debug(sql);
                return GlobalVar.Sql.Read(sql);
            }
            catch (Exception ex)
            {
                debug("GetDefectResultByCode : " + ex.ToString());
                debug(ex.StackTrace);
                FuncLog.WriteLog_Debug("GetDefectResultByCode : " + ex.ToString());
                FuncLog.WriteLog_Debug(ex.StackTrace);
            }
            return null;
        }

        public static string[,] GetDefectResultByCode(string startDate, string endDate) // SMD defect별 Defect 결과 조회
        {
            if (GlobalVar.Sql == null ||
                !GlobalVar.Sql.connected)
            {
                return null;
            }

            if (!GlobalVar.UseMsSQL)
            {
                return null;
            }

            try
            {
                string sql = "select a.[DefectCode], b.ErrorName, count(a.[DefectCode]) as DefectCount " +
                            "from [TestResult] a " +
                            "left outer join[TestErrorCode] b " +
                            "on a.[DefectCode] = b.[ErrorCode] " +
                            "where a.[Date] >= '" + startDate + "' " +
                            "and a.[Date] <= '" + endDate + "' " +
                            "and a.[Finish] = 1 " +
                            "and a.[DefectCode] <> '' " +
                            "and a.[DefectCode] <> '-1' " +
                            "group by a.[DefectCode], b.[ErrorName]";
                //debug(sql);
                return GlobalVar.Sql.Read(sql);
            }
            catch (Exception ex)
            {
                debug("GetDefectResultByCode : " + ex.ToString());
                debug(ex.StackTrace);
                FuncLog.WriteLog_Debug("GetDefectResultByCode : " + ex.ToString());
                FuncLog.WriteLog_Debug(ex.StackTrace);
            }
            return null;
        }

        public static int GetTotalInputCount(string startDate, string endDate) // SMD 일자별 투입 수량 조회
        {
            if (GlobalVar.Sql == null ||
                !GlobalVar.Sql.connected)
            {
                return 0;
            }

            if (!GlobalVar.UseMsSQL)
            {
                return 0;
            }

            int count = 0;
            try
            {
                string sql = "select count(*) as count " +
                            "from[TestResult] " +
                            "where [Date] >= '" + startDate + "' " +
                            "and [Date] <= '" + endDate + "' ";
                string[,] rs = GlobalVar.Sql.Read(sql);


                if (rs != null)
                {
                    //if (rs.Read())
                    if (rs.GetLength(0) > 0)
                    {
                        count = int.Parse(rs[0, 0].ToString());
                    }
                    //rs.Close();
                }
                return count;
            }
            catch (Exception ex)
            {
                debug("GetTotalInputCount : " + ex.ToString());
                debug(ex.StackTrace);
                FuncLog.WriteLog_Debug("GetTotalInputCount : " + ex.ToString());
                FuncLog.WriteLog_Debug(ex.StackTrace);
            }
            return 0;
        }


        public static string[,] GetDefectResultBySite(string startDate, string endDate, int site) // SMD 사이트별 Defect 결과 조회
        {
            if (GlobalVar.Sql == null ||
                !GlobalVar.Sql.connected)
            {
                return null;
            }

            if (!GlobalVar.UseMsSQL)
            {
                return null;
            }

            try
            {
                string sql = "select (select count(*) " +
                            "from [TestResult] " +
                            "where [Date] >= '" + startDate + "' " +
                            "and [Date] <= '" + endDate + "' " +
                            "and [Finish] = 1 " +
                            "and [Site] = " + site + ") as TotalInput, " +
                            "(select count(*) " +
                            "from [TestResult] " +
                            "where [Date] >= '" + startDate + "' " +
                            "and [Date] <= '" + endDate + "' " +
                            "and [Finish] = 1 " +
                            "and [Site] = " + site + " " +
                            "and[NG] = 1) as TotalDefect";
                //debug(sql);
                return GlobalVar.Sql.Read(sql);
            }
            catch (Exception ex)
            {
                debug("GetDefectResultBySite : " + ex.ToString());
                debug(ex.StackTrace);
                FuncLog.WriteLog_Debug("GetDefectResultBySite : " + ex.ToString());
                FuncLog.WriteLog_Debug(ex.StackTrace);
            }
            return null;
        }

        public static string[,] GetAllDefectResultByShift(string startDate, string endDate, bool shiftA, bool shiftB, bool shiftC, bool isPBA) // SMD 전체 사이트별 Defect 결과 조회, 조별
        {
            if (GlobalVar.Sql == null ||
                !GlobalVar.Sql.connected)
            {
                return null;
            }

            if (!GlobalVar.UseMsSQL)
            {
                return null;
            }

            try
            {
                // 2교대시 A조는 해당날짜, B조는 해당날짜 B조시간부터 다음날 A조시간 이전까지
                // 3교대시 A/B조는 해당날짜, C조는 해당날짜 C조시간부터 다음날 A조 시간 이전까지.
                // 검색시 날짜+시간 조합으로 시작날짜 가장 먼저 조 시작시간부터
                //                           끝조 미선택시 마지막날 마지막 시간범위
                //                           끝조 선택시 마지막 다음날 첫조 시간 이전까지 범위로 검색
                // Trace 폼으로 들어올 때 마지막조고 A조 시간 이전이면 이전날짜로 세팅해야 함

                #region 검색 일시 범위 지정
                string shift = "";
                if (shiftA)
                {
                    shift += "'A'";
                }
                if (shiftB)
                {
                    if (shift.Length > 0)
                    {
                        shift += ",";
                    }
                    shift += "'B'";
                }
                if (shiftC)
                {
                    if (shift.Length > 0)
                    {
                        shift += ",";
                    }
                    shift += "'C'";
                }

                #endregion

                string sql = "select [Input],[Pass],[NG] " +
                                "from [PCBCount] " +
                                "where [Date] >= '" + startDate + "' " +
                                "and [Date] <= '" + endDate + "' " +
                                "and [Shift] in (" + shift + ") " +
                                "and [PBA] = '" + (isPBA ? "1" : "0") + "' ";

                //debug(sql);
                return GlobalVar.Sql.Read(sql);
            }
            catch (Exception ex)
            {
                debug("GetDefectResultBySite : " + ex.ToString());
                debug(ex.StackTrace);
                FuncLog.WriteLog_Debug("GetDefectResultBySite : " + ex.ToString());
                FuncLog.WriteLog_Debug(ex.StackTrace);
            }
            return null;
        }


        public static string[,] GetAllDefectResultBySiteArray(string startDate, string endDate, bool shiftA, bool shiftB, bool shiftC, string order, string order2, bool descending) // SMD 전체 사이트별 Defect 결과 조회, 조별
        {
            if (GlobalVar.Sql == null ||
                !GlobalVar.Sql.connected)
            {
                return null;
            }

            if (!GlobalVar.UseMsSQL)
            {
                return null;
            }

            try
            {
                // 2교대시 A조는 해당날짜, B조는 해당날짜 B조시간부터 다음날 A조시간 이전까지
                // 3교대시 A/B조는 해당날짜, C조는 해당날짜 C조시간부터 다음날 A조 시간 이전까지.
                // 검색시 날짜+시간 조합으로 시작날짜 가장 먼저 조 시작시간부터
                //                           끝조 미선택시 마지막날 마지막 시간범위
                //                           끝조 선택시 마지막 다음날 첫조 시간 이전까지 범위로 검색
                // Trace 폼으로 들어올 때 마지막조고 A조 시간 이전이면 이전날짜로 세팅해야 함

                #region 검색 일시 범위 지정
                string startTime = startDate;
                if (shiftA)
                {
                    startTime += Util.IntToString(FuncInline.ShiftAHour, 2) + ":" + Util.IntToString(FuncInline.ShiftAMin, 2) + ":00";
                }
                else if (shiftB)
                {
                    startTime += Util.IntToString(FuncInline.ShiftBHour, 2) + ":" + Util.IntToString(FuncInline.ShiftBMin, 2) + ":00";
                }
                else if (shiftC)
                {
                    startTime += Util.IntToString(FuncInline.ShiftCHour, 2) + ":" + Util.IntToString(FuncInline.ShiftCMin, 2) + ":00";
                }
                string endTime = endDate;
                #region 마지막조 포함시
                if ((!FuncInline.UseShiftC && shiftB) ||
                    (FuncInline.UseShiftC && shiftC))
                {
                    // 종료 날짜의 다음날 A조 시작 전 시간을 구한다.
                    int searchYear = int.Parse(endDate.Substring(0, 4));
                    int searchMonth = int.Parse(endDate.Substring(4, 2));
                    int searchDate = int.Parse(endDate.Substring(6, 2));
                    int searchHour = FuncInline.ShiftAHour;
                    int searchMin = FuncInline.ShiftAMin;
                    DateTime searchDateTime = new DateTime(searchYear, searchMonth, searchDate, searchHour, searchMin, 0);
                    searchDateTime = searchDateTime.AddDays(1);
                    searchDateTime = searchDateTime.AddSeconds(-1);
                    endTime = searchDateTime.ToString("yyyyMMddHH:mm:ss");
                }
                #endregion
                #region 마지막조 미포함시
                else
                {
                    // 종료 날짜의 선택된 마지막 조 종료 시간을 구한다.
                    int searchYear = int.Parse(endDate.Substring(0, 4));
                    int searchMonth = int.Parse(endDate.Substring(4, 2));
                    int searchDate = int.Parse(endDate.Substring(6, 2));
                    int searchHour = FuncInline.ShiftAHour;
                    int searchMin = FuncInline.ShiftAMin;
                    // A조 선택시 B조시간
                    if (shiftA)
                    {
                        searchHour = FuncInline.ShiftBHour;
                        searchMin = FuncInline.ShiftBMin;
                    }
                    // B조 선택시 C조시간
                    if (shiftB)
                    {
                        searchHour = FuncInline.ShiftCHour;
                        searchMin = FuncInline.ShiftCMin;
                    }
                    DateTime searchDateTime = new DateTime(searchYear, searchMonth, searchDate, searchHour, searchMin, 0);
                    searchDateTime = searchDateTime.AddSeconds(-1); // 1분을 빼서 조 마지막 시간으로 세팅
                    endTime = searchDateTime.ToString("yyyyMMddHH:mm:ss");
                }
                #endregion
                #endregion

                string sql = "select a.Site, " +
                                    "a.Array, " +
                                    "isnull(b.TotalInput,0) as Input, " +
                                    "isnull(c.TotalDefect,0) as Defect, " +
                                    "isnull(100 * c.TotalDefect / b.TotalInput,0) as DefectRatio " +
                                "from All_Site_Array a " +
                                "left outer join (select [Site],[Array], count(*) as TotalInput " +
                                                "from [TestResult] " +
                                                "where concat([Date],[Time]) >= '" + startTime + "' " +
                                                "and concat([Date],[Time]) <= '" + endTime + "' " +
                                                "and [Finish] = 1 " +
                                                "group by [Site],[Array]) as b " +
                                "on a.Site = b.Site " +
                                    "and a.Array = b.Array " +
                                "left outer join(select [Site],[Array], count(*) as TotalDefect " +
                                                "from [TestResult] " +
                                                "where concat([Date],[Time]) >= '" + startTime + "' " +
                                                "and concat([Date],[Time]) <= '" + endTime + "' " +
                                                "and [Finish] = 1 " +
                                                "and [NG] = 1 " +
                                                "group by [Site],[Array]) as c " +
                                "on a.Site = c.Site " +
                                    "and a.Array = c.Array " +
                                "order by " + order + (descending ? " desc" : "") + ", " +
                                order2 + (descending ? " desc" : "");
                //debug(sql);
                return GlobalVar.Sql.Read(sql);
            }
            catch (Exception ex)
            {
                debug("GetDefectResultBySite : " + ex.ToString());
                debug(ex.StackTrace);
                FuncLog.WriteLog_Debug("GetDefectResultBySite : " + ex.ToString());
                FuncLog.WriteLog_Debug(ex.StackTrace);
            }
            return null;
        }

        public static string[,] GetAllDefectResultBySiteArray(string startDate, string endDate) // SMD 전체 사이트별 Defect 결과 조회
        {
            if (GlobalVar.Sql == null ||
                !GlobalVar.Sql.connected)
            {
                return null;
            }

            if (!GlobalVar.UseMsSQL)
            {
                return null;
            }

            try
            {
                string sql = "select a.*, isnull(b.TotalInput,0) as TotalInput, isnull(c.TotalDefect,0) as TotalDefect " +
                                "from All_Site_Array a " +
                                "left outer join (select [Site],[Array], count(*) as TotalInput " +
                                                "from [TestResult] " +
                                                "where [Date] >= '" + startDate + "' " +
                                                "and [Date] <= '" + endDate + "' " +
                                                "and [Finish] = 1 " +
                                                "group by [Site],[Array]) as b " +
                                "on a.Site = b.Site " +
                                    "and a.Array = b.Array " +
                                "left outer join(select [Site],[Array], count(*) as TotalDefect " +
                                                "from [TestResult] " +
                                                "where [Date] >= '" + startDate + "' " +
                                                "and [Date] <= '" + endDate + "' " +
                                                "and [Finish] = 1 " +
                                                "and [NG] = 1 " +
                                                "group by [Site],[Array]) as c " +
                                "on a.Site = c.Site " +
                                    "and a.Array = c.Array " +
                                "order by a.Site, a.Array";
                //debug(sql);
                return GlobalVar.Sql.Read(sql);
            }
            catch (Exception ex)
            {
                debug("GetDefectResultBySite : " + ex.ToString());
                debug(ex.StackTrace);
                FuncLog.WriteLog_Debug("GetDefectResultBySite : " + ex.ToString());
                FuncLog.WriteLog_Debug(ex.StackTrace);
            }
            return null;
        }

        public static string[,] GetDefectResultBySiteArray(string startDate, string endDate, int site, int array) // SMD 사이트별 Defect 결과 조회
        {
            if (GlobalVar.Sql == null ||
                !GlobalVar.Sql.connected)
            {
                return null;
            }

            if (!GlobalVar.UseMsSQL)
            {
                return null;
            }

            try
            {
                string sql = "select (select count(*) " +
                            "from [TestResult] " +
                            "where [Date] >= '" + startDate + "' " +
                            "and [Date] <= '" + endDate + "' " +
                            "and [Finish] = 1 " +
                            "and [Site] = " + site + " " +
                            "and [Array] = " + array + ") as TotalInput, " +
                            "(select count(*) " +
                            "from [TestResult] " +
                            "where [Date] >= '" + startDate + "' " +
                            "and [Date] <= '" + endDate + "' " +
                            "and [Finish] = 1 " +
                            "and [Site] = " + site + " " +
                            "and [Array] = " + array + " " +
                            "and [NG] = 1) as TotalDefect";
                //debug(sql);
                return GlobalVar.Sql.Read(sql);
            }
            catch (Exception ex)
            {
                debug("GetDefectResultBySite : " + ex.ToString());
                debug(ex.StackTrace);
                FuncLog.WriteLog_Debug("GetDefectResultBySite : " + ex.ToString());
                FuncLog.WriteLog_Debug(ex.StackTrace);
            }
            return null;
        }

        public static void UpdateTestResult(int site, int array, string barcode, string testType, bool cmdSend, bool cmdReceive, bool cmdOK, bool testFinish, bool testPass, bool testCancel, bool testTimeout, bool finish, bool ng, string defectCode, double testTime) // PCB 를 사이트에 투입한 시점부터 검사 종료까지 상태값 변경
        {
            try
            {
                if (site < 1 ||
                    site > 21 ||
                    array < 1 ||
                    array > 12 ||
                    barcode.Length == 0 ||
                    testType.Length == 0 ||
                    (!cmdSend && !cmdReceive && !cmdOK && !testFinish && !testPass && !testCancel && !finish && !ng))
                {
                    return;
                }

                // sql 에러 등으로 row가 추가 안 되는 경우가 생길 수 있으므로
                // select 해서 있을 때까지 또는 몇 회 insert 시도해야 할 듯

                string sql = "";

                for (int i = 0; i < 5; i++)
                {
                    sql = "select count(*) from [TestResult] where finish <> 1 and barcode = '" + barcode + "'";
                    string[,] rs = GlobalVar.Sql.Read(sql);

                    int cnt = 0;
                    if (rs != null)
                    {
                        //if (rs.Read())
                        if (rs.GetLength(0) > 0)
                        {
                            try
                            {
                                cnt = int.Parse(rs[0, 0].ToString());
                            }
                            catch { }
                        }
                        //rs.Close();
                    }

                    if (cnt > 0)
                    {
                        break;
                    }

                    // 없으면 기본값 추가하는 쿼리 실행
                    sql = "if not exists (select * from [TestResult] where finish <> 1 and barcode = '" + barcode + "') " +
                                        "begin " +
                                        "insert into [TestResult] " +
                                        "([Date], [Time], [Site], [Array], [Barcode], [Type]) " +
                                        "values (" +
                                        "CONVERT(CHAR(8), getdate(), 112), " + // Date
                                        "CONVERT(CHAR(8), getdate(), 108), " + // Time
                                        site + ", " +
                                        array + ", " +
                                        "'" + barcode + "', " +
                                        "'" + testType + "', " +
                                        ") " +
                                        "end";
                    //debug(sql);
                    GlobalVar.Sql.Execute(sql);
                }


                // 각 패러미터 값이 true 인 것 update.
                if (cmdSend)
                {
                    sql = "update [TestResult] set [Command_Send] = 1 where finish <> 1 and barcode = '" + barcode + "'";
                    GlobalVar.Sql.Execute(sql);
                }
                if (cmdReceive)
                {
                    sql = "update [TestResult] set [Command_Receive] = 1 where finish <> 1 and barcode = '" + barcode + "'";
                    GlobalVar.Sql.Execute(sql);
                }
                if (cmdOK)
                {
                    sql = "update [TestResult] set [Command_OK] = 1 where finish <> 1 and barcode = '" + barcode + "'";
                    GlobalVar.Sql.Execute(sql);
                }
                if (testFinish)
                {
                    sql = "update [TestResult] set [Test_Finish] = 1 where finish <> 1 and barcode = '" + barcode + "'";
                    GlobalVar.Sql.Execute(sql);
                }
                if (testPass) // fail 은 ng로
                {
                    sql = "update [TestResult] set [Test_Pass] = 1 where finish <> 1 and barcode = '" + barcode + "'";
                    GlobalVar.Sql.Execute(sql);
                }
                if (testCancel)
                {
                    sql = "update [TestResult] set [Test_Cancel] = 1 where finish <> 1 and barcode = '" + barcode + "'";
                    GlobalVar.Sql.Execute(sql);
                }
                if (testTimeout)
                {
                    sql = "update [TestResult] set [Test_Timeout] = 1 where finish <> 1 and barcode = '" + barcode + "'";
                    GlobalVar.Sql.Execute(sql);
                }
                if (finish)
                {
                    sql = "update [TestResult] set [Finish] = 1 where finish <> 1 and barcode = '" + barcode + "'";
                    GlobalVar.Sql.Execute(sql);
                }
                if (ng)
                {
                    sql = "update [TestResult] set [NG] = 1 where finish <> 1 and barcode = '" + barcode + "'";
                    GlobalVar.Sql.Execute(sql);
                }
                if (defectCode.Length > 0)
                {
                    if (defectCode.Length == 1)
                    {
                        defectCode = '0' + defectCode;
                    }
                    sql = "update [TestResult] set [DefectCode] = '" + defectCode + "' where finish <> 1 and barcode = '" + barcode + "'";
                    GlobalVar.Sql.Execute(sql);
                }
                if (testTime > 0)
                {
                    sql = "update [TestResult] set [TestTime] = '" + testTime.ToString("F2") + "' where finish <> 1 and barcode = '" + barcode + "'";
                    GlobalVar.Sql.Execute(sql);
                }
            }
            catch (Exception ex)
            {
                debug("UpdateTestResult : " + ex.ToString());
                debug(ex.StackTrace);
                FuncLog.WriteLog_Debug("UpdateTestResult : " + ex.ToString());
                FuncLog.WriteLog_Debug(ex.StackTrace);
            }
        }

        public static string[,] GetTestResult(string startDate, string endDate, int site) // SMD Test 결과 조회
        {
            if (GlobalVar.Sql == null ||
                !GlobalVar.Sql.connected)
            {
                return null;
            }

            if (!GlobalVar.UseMsSQL)
            {
                return null;
            }

            try
            {
                string sql = "select [Site], [Array], isnull(count(*), 0) as Total, isnull(count(Test_Pass), 0) as Pass " +
                            "from [TestResult] " +
                            "where [Date] >= '" + startDate + "' " +
                            "and [Date] <= '" + endDate + "' " +
                            "and [Finish] = 1 " +
                            "and [Site] = " + site + " " +
                            "group by [Site], [Array]";
                return GlobalVar.Sql.Read(sql);
            }
            catch (Exception ex)
            {
                debug("GetTestResult : " + ex.ToString());
                debug(ex.StackTrace);
                FuncLog.WriteLog_Debug("GetTestResult : " + ex.ToString());
                FuncLog.WriteLog_Debug(ex.StackTrace);
            }
            return null;
        }
        public static string[,] GetTestComm(string startDate, string endDate, bool shiftA, bool shiftB, bool shiftC, string order, string order2, bool descending) // SMD Test 통신로그 조회
        {
            if (GlobalVar.Sql == null ||
                !GlobalVar.Sql.connected)
            {
                return null;
            }

            if (!GlobalVar.UseMsSQL)
            {
                return null;
            }

            try
            {
                // 2교대시 A조는 해당날짜, B조는 해당날짜 B조시간부터 다음날 A조시간 이전까지
                // 3교대시 A/B조는 해당날짜, C조는 해당날짜 C조시간부터 다음날 A조 시간 이전까지.
                // 검색시 날짜+시간 조합으로 시작날짜 가장 먼저 조 시작시간부터
                //                           끝조 미선택시 마지막날 마지막 시간범위
                //                           끝조 선택시 마지막 다음날 첫조 시간 이전까지 범위로 검색
                // Trace 폼으로 들어올 때 마지막조고 A조 시간 이전이면 이전날짜로 세팅해야 함

                #region 검색 일시 범위 지정
                string startTime = startDate;
                if (shiftA)
                {
                    startTime += Util.IntToString(FuncInline.ShiftAHour, 2) + ":" + Util.IntToString(FuncInline.ShiftAMin, 2) + ":00";
                }
                else if (shiftB)
                {
                    startTime += Util.IntToString(FuncInline.ShiftBHour, 2) + ":" + Util.IntToString(FuncInline.ShiftBMin, 2) + ":00";
                }
                else if (shiftC)
                {
                    startTime += Util.IntToString(FuncInline.ShiftCHour, 2) + ":" + Util.IntToString(FuncInline.ShiftCMin, 2) + ":00";
                }
                string endTime = endDate;
                #region 마지막조 포함시
                if ((!FuncInline.UseShiftC && shiftB) ||
                    (FuncInline.UseShiftC && shiftC))
                {
                    // 종료 날짜의 다음날 A조 시작 전 시간을 구한다.
                    int searchYear = int.Parse(endDate.Substring(0, 4));
                    int searchMonth = int.Parse(endDate.Substring(4, 2));
                    int searchDate = int.Parse(endDate.Substring(6, 2));
                    int searchHour = FuncInline.ShiftAHour;
                    int searchMin = FuncInline.ShiftAMin;
                    DateTime searchDateTime = new DateTime(searchYear, searchMonth, searchDate, searchHour, searchMin, 0);
                    searchDateTime = searchDateTime.AddDays(1);
                    searchDateTime = searchDateTime.AddSeconds(-1);
                    endTime = searchDateTime.ToString("yyyyMMddHH:mm:ss");
                }
                #endregion
                #region 마지막조 미포함시
                else
                {
                    // 종료 날짜의 선택된 마지막 조 종료 시간을 구한다.
                    int searchYear = int.Parse(endDate.Substring(0, 4));
                    int searchMonth = int.Parse(endDate.Substring(4, 2));
                    int searchDate = int.Parse(endDate.Substring(6, 2));
                    int searchHour = FuncInline.ShiftAHour;
                    int searchMin = FuncInline.ShiftAMin;
                    // A조 선택시 B조시간
                    if (shiftA)
                    {
                        searchHour = FuncInline.ShiftBHour;
                        searchMin = FuncInline.ShiftBMin;
                    }
                    // B조 선택시 C조시간
                    if (shiftB)
                    {
                        searchHour = FuncInline.ShiftCHour;
                        searchMin = FuncInline.ShiftCMin;
                    }
                    DateTime searchDateTime = new DateTime(searchYear, searchMonth, searchDate, searchHour, searchMin, 0);
                    searchDateTime = searchDateTime.AddSeconds(-1); // 1분을 빼서 조 마지막 시간으로 세팅
                    endTime = searchDateTime.ToString("yyyyMMddHH:mm:ss");
                }
                #endregion
                #endregion

                string sql = "select [Date],[Time],[Site],[Array],[Type],[Content],[Result] " +
                            "from CommLog " +
                            "where concat([Date],[Time]) >= '" + startTime + "' " +
                            "and concat([Date], [Time]) <= '" + endTime + "' " +
                            "order by " + order + (descending ? " desc" : "") + ", " +
                            order2 + (descending ? " desc" : "");
                //debug(sql);
                return GlobalVar.Sql.Read(sql);
            }
            catch (Exception ex)
            {
                debug("GetTestComm : " + ex.ToString());
                debug(ex.StackTrace);
            }
            return null;
        }

        public static string[,] GetTestComm(string startDate, string endDate) // SMD Test 통신로그 조회
        {
            if (GlobalVar.Sql == null ||
                !GlobalVar.Sql.connected)
            {
                return null;
            }

            if (!GlobalVar.UseMsSQL)
            {
                return null;
            }

            try
            {
                string sql = "select [Date],[Time],[Site],[Array],[Type],[Content],[Result] " +
                            "from CommLog " +
                            "where [Date] >= '" + startDate + "' " +
                            "and [Date] <= '" + endDate + "' " +
                            "order by [Date] desc, [Time] desc";
                //debug(sql);
                return GlobalVar.Sql.Read(sql);
            }
            catch (Exception ex)
            {
                debug("GetTestComm : " + ex.ToString());
                debug(ex.StackTrace);
            }
            return null;
        }

        public static string[,] GetTestCommStatistics(string startDate, string endDate, bool shiftA, bool shiftB, bool shiftC, string order, string order2, bool descending) // SMD Test 통신로그 조회
        {
            if (GlobalVar.Sql == null ||
                !GlobalVar.Sql.connected)
            {
                return null;
            }

            if (!GlobalVar.UseMsSQL)
            {
                return null;
            }

            try
            {
                // 2교대시 A조는 해당날짜, B조는 해당날짜 B조시간부터 다음날 A조시간 이전까지
                // 3교대시 A/B조는 해당날짜, C조는 해당날짜 C조시간부터 다음날 A조 시간 이전까지.
                // 검색시 날짜+시간 조합으로 시작날짜 가장 먼저 조 시작시간부터
                //                           끝조 미선택시 마지막날 마지막 시간범위
                //                           끝조 선택시 마지막 다음날 첫조 시간 이전까지 범위로 검색
                // Trace 폼으로 들어올 때 마지막조고 A조 시간 이전이면 이전날짜로 세팅해야 함

                #region 검색 일시 범위 지정
                string startTime = startDate;
                if (shiftA)
                {
                    startTime += Util.IntToString(FuncInline.ShiftAHour, 2) + ":" + Util.IntToString(FuncInline.ShiftAMin, 2) + ":00";
                }
                else if (shiftB)
                {
                    startTime += Util.IntToString(FuncInline.ShiftBHour, 2) + ":" + Util.IntToString(FuncInline.ShiftBMin, 2) + ":00";
                }
                else if (shiftC)
                {
                    startTime += Util.IntToString(FuncInline.ShiftCHour, 2) + ":" + Util.IntToString(FuncInline.ShiftCMin, 2) + ":00";
                }
                string endTime = endDate;
                #region 마지막조 포함시
                if ((!FuncInline.UseShiftC && shiftB) ||
                    (FuncInline.UseShiftC && shiftC))
                {
                    // 종료 날짜의 다음날 A조 시작 전 시간을 구한다.
                    int searchYear = int.Parse(endDate.Substring(0, 4));
                    int searchMonth = int.Parse(endDate.Substring(4, 2));
                    int searchDate = int.Parse(endDate.Substring(6, 2));
                    int searchHour = FuncInline.ShiftAHour;
                    int searchMin = FuncInline.ShiftAMin;
                    DateTime searchDateTime = new DateTime(searchYear, searchMonth, searchDate, searchHour, searchMin, 0);
                    searchDateTime = searchDateTime.AddDays(1);
                    searchDateTime = searchDateTime.AddSeconds(-1);
                    endTime = searchDateTime.ToString("yyyyMMddHH:mm:ss");
                }
                #endregion
                #region 마지막조 미포함시
                else
                {
                    // 종료 날짜의 선택된 마지막 조 종료 시간을 구한다.
                    int searchYear = int.Parse(endDate.Substring(0, 4));
                    int searchMonth = int.Parse(endDate.Substring(4, 2));
                    int searchDate = int.Parse(endDate.Substring(6, 2));
                    int searchHour = FuncInline.ShiftAHour;
                    int searchMin = FuncInline.ShiftAMin;
                    // A조 선택시 B조시간
                    if (shiftA)
                    {
                        searchHour = FuncInline.ShiftBHour;
                        searchMin = FuncInline.ShiftBMin;
                    }
                    // B조 선택시 C조시간
                    if (shiftB)
                    {
                        searchHour = FuncInline.ShiftCHour;
                        searchMin = FuncInline.ShiftCMin;
                    }
                    DateTime searchDateTime = new DateTime(searchYear, searchMonth, searchDate, searchHour, searchMin, 0);
                    searchDateTime = searchDateTime.AddSeconds(-1); // 1분을 빼서 조 마지막 시간으로 세팅
                    endTime = searchDateTime.ToString("yyyyMMddHH:mm:ss");
                }
                #endregion
                #endregion

                string sql = "select [Site],[Array],[Type],count(*) as count " +
                            "from CommLog " +
                            "where concat([Date], [Time]) >= '" + startTime + "' " +
                            "and concat([Date], [Time]) <= '" + endTime + "' " +
                            "group by [Site],[Array],[Type] " +
                            "order by " + order + (descending ? " desc" : "") + ", " +
                            order2 + (descending ? " desc" : "");
                //debug(sql);
                return GlobalVar.Sql.Read(sql);
            }
            catch (Exception ex)
            {
                debug("GetTestComm : " + ex.ToString());
                debug(ex.StackTrace);
            }
            return null;
        }

        public static string[,] GetTestCommStatistics(string startDate, string endDate) // SMD Test 통신로그 조회
        {
            if (GlobalVar.Sql == null ||
                !GlobalVar.Sql.connected)
            {
                return null;
            }

            if (!GlobalVar.UseMsSQL)
            {
                return null;
            }

            try
            {
                string sql = "select [Site],[Array],[Type],count(*) as count " +
                            "from CommLog " +
                            "where [Date] >= '" + startDate + "' " +
                            "and [Date] <= '" + endDate + "' " +
                            "group by [Site],[Array],[Type] " +
                            "order by  [Site],[Array],[Type]";
                //debug(sql);
                return GlobalVar.Sql.Read(sql);
            }
            catch (Exception ex)
            {
                debug("GetTestComm : " + ex.ToString());
                debug(ex.StackTrace);
            }
            return null;
        }

        public static void GetAllTestError() // SMD 테스트 에러 리스트 조회
        {
            if (GlobalVar.Sql == null ||
                !GlobalVar.Sql.connected)
            {
                return;
            }

            // 일단 배열 초기화
            for (int i = 0; i < FuncInline.TestErrorCode.Length; i++)
            {
                FuncInline.TestErrorCode[i] = "";
            }

            if (!GlobalVar.UseMsSQL)
            {
                return;
            }

            string sql = "select ErrorCode, ErrorName, Retest, RetestMethod " +
                        "from TestErrorCode " +
                        "order by ErrorCode";
            string[,] rs = GlobalVar.Sql.Read(sql);
            try
            {
                //while (rs.Read())
                for (int j = 0; j < rs.GetLength(0); j++)
                {
                    TestErrorCode[int.Parse(rs[j, 0].ToString())] = rs[j, 1].ToString();
                    TestErrorRetest[int.Parse(rs[j, 0].ToString())] = rs[j, 2].ToString() == "True";
                    TestErrorRetestType[int.Parse(rs[j, 0].ToString())] = rs[j, 3].ToString();
                }
                //rs.Close();
            }
            catch (Exception ex)
            {
                debug("GetAllTestError" + ex.ToString());
                debug(ex.StackTrace);
            }
        }

        public static void SaveTestErrorCode(int errorCode, string errorName, bool retest, string retestType) // SMD 에러코드 추가/수정
        {
            if (!GlobalVar.UseMsSQL)
            {
                return;
            }

            try
            {
                string strCode = (errorCode < 10 ? "0" : "") + errorCode;
                string sql = "if exists (select * from TestErrorCode where ErrorCode='" + strCode + "') " +
                "begin " +
                "    update TestErrorCode set ErrorName = '" + errorName + "', Retest = " + (retest ? 1 : 0) + ", RetestMethod = '" + retestType + "' where ErrorCode = '" + strCode + "' " +
                "end " +
                "else " +
                " begin " +
                "    insert into TestErrorCode (ErrorCode, ErrorName, Retest, RetestMethod) values('" + strCode + "','" + errorName + "', " + (retest ? 1 : 0) + ", '" + retestType + "') " +
                "end";
                //debug(sql);
                GlobalVar.Sql.Execute(sql);
            }
            catch (Exception ex)
            {
                debug("SaveTestErrorCode : " + ex.ToString());
                debug(ex.StackTrace);
            }
        }

        public static void DeleteErrorCode(int errorCode) // SMD 에러코드 삭제
        {
            if (!GlobalVar.UseMsSQL)
            {
                return;
            }

            try
            {
                string sql = "    delete from TestErrorCode where ErrorCode = " + errorCode + " ";
                GlobalVar.Sql.Execute(sql);
            }
            catch (Exception ex)
            {
                debug("DeleteErrorCode : " + ex.ToString());
                debug(ex.StackTrace);
            }
        }

        public static string[,] GetPCBHistory(string[] code) // PCB 테스트 결과 히스토리 리스트 조회
        {
            // 일단 배열 초기화
            if (!GlobalVar.UseMsSQL)
            {
                return null;
            }

            if (GlobalVar.Sql == null ||
                !GlobalVar.Sql.connected)
            {
                return null;
            }

            string codes = "";
            //bool empty = true;
            if (code != null)
            {
                for (int i = 0; i < code.Length; i++)
                {
                    if (code[i].Length > 0)
                    {
                        codes += "'" + code[i] + "',";
                        //empty = false;
                    }
                }
            }
            if (codes.Length > 0)
            {
                codes = codes.Substring(0, codes.Length - 1); // 마지막 , 없앤다.
            }

            //if (empty) // 유효 데이터가 없다. 해당shift의 전체 ng 목록을 가져오게
            //{
            //    return null;
            //}

            try
            {
                string sql = "select * " +
                            "from TestResult " +
                            "where Barcode in (" + codes + ")" +
                            "order by Date desc,Time desc";
                if (codes.Length == 0) // 코드가 지정되지 않았으면 해당 shift의 전체 ng 목록 가져오기
                {
                    //return null;// 코드가 지정되지 않았으면 해당 shift의 전체 ng 목록 가져오기로 변경

                    // 2교대시 B조는 이전날짜 오후부터 오늘날짜 오전까지
                    // 3교대시 C조는 이전날짜 저녁부터 오늘날짜 오전까지

                    string nowDate = DateTime.Now.ToString("yyyyMMdd");
                    string shiftATime = Util.IntToString(FuncInline.ShiftAHour, 2) + ":" + Util.IntToString(FuncInline.ShiftAMin, 2) + ":00";
                    string shiftBTime = Util.IntToString(FuncInline.ShiftBHour, 2) + ":" + Util.IntToString(FuncInline.ShiftBMin, 2) + ":00";
                    string shiftCTime = Util.IntToString(FuncInline.ShiftCHour, 2) + ":" + Util.IntToString(FuncInline.ShiftCMin, 2) + ":00";
                    string nowTime = DateTime.Now.ToString("HH:mm:ss");
                    string yesterDay = DateTime.Today.AddDays(-1).ToString("yyyyMMdd");
                    // 현재 시간이 ShiftA보다 적으면 이전날짜 shiftC 보다 날짜/시간 조합이 큰 경우로 읽어야 함.
                    switch (NowShift)
                    {
                        case "A":
                            sql = "select * " +
                                        "from TestResult " +
                                        "where [DefectCode] <> '' " +
                                        "and [DefectCode] <> '-1' " +
                                        "and [Date] >= '" + nowDate + "' " +
                                        "and [Time] >= '" + shiftATime + "' " +
                                        "order by Date desc,Time desc";
                            break;
                        case "B":
                            if (FuncInline.UseShiftC)
                            {
                                sql = "select * " +
                                            "from TestResult " +
                                            "where [DefectCode] <> '' " +
                                            "and [DefectCode] <> '-1' " +
                                            "and [Date] >= '" + nowDate + "' " +
                                            "and [Time] >= '" + shiftBTime + "' " +
                                            "order by Date desc,Time desc";
                            }
                            else
                            {
                                if (int.Parse(nowTime.Replace(":", "")) >= int.Parse(shiftBTime.Replace(":", ""))) // 저녁이면 오늘 날짜만 고려
                                {
                                    sql = "select * " +
                                                "from TestResult " +
                                                "where [DefectCode] <> '' " +
                                                "and [DefectCode] <> '-1' " +
                                                "and [Date] >= '" + nowDate + "' " +
                                                "and [Time] >= '" + shiftBTime + "' " +
                                                "order by Date desc,Time desc";
                                }
                                else // 새벽 시간이면 이전날짜 C조부터
                                {
                                    sql = "select * " +
                                                "from TestResult " +
                                                "where [DefectCode] <> '' " +
                                                "and [DefectCode] <> '-1' " +
                                                "and (([Date] >= '" + yesterDay + "' " +
                                                "          and [Time] >= '" + shiftBTime + "') " +
                                                "      or ([Date] >= '" + nowDate + "')) " +
                                                "order by Date desc,Time desc";
                                }
                            }
                            break;
                        case "C":
                            if (int.Parse(nowTime.Replace(":", "")) >= int.Parse(shiftCTime.Replace(":", ""))) // 저녁이면 오늘 날짜만 고려
                            {
                                sql = "select * " +
                                            "from TestResult " +
                                            "where [DefectCode] <> '' " +
                                            "and [DefectCode] <> '-1' " +
                                           "and [Date] >= '" + nowDate + "' " +
                                            "and [Time] >= '" + shiftCTime + "' " +
                                            "order by Date desc,Time desc";
                            }
                            else // 새벽 시간이면 이전날짜 C조부터
                            {
                                sql = "select * " +
                                            "from TestResult " +
                                            "where [DefectCode] <> '' " +
                                            "and [DefectCode] <> '-1' " +
                                            "and (([Date] >= '" + yesterDay + "' " +
                                            "          and [Time] >= '" + shiftCTime + "') " +
                                            "      or ([Date] >= '" + nowDate + "')) " +
                                            "order by Date desc,Time desc";
                            }
                            break;
                        default: // shift 계산이 안 된 경우는 null로 리턴
                            return null;
                    }
                }
                //debug(sql);
                return GlobalVar.Sql.Read(sql);
            }
            catch (Exception ex)
            {
                debug("GetPCBHistory : " + ex.ToString());
                debug(ex.StackTrace);
                FuncLog.WriteLog_Debug("GetPCBHistory : " + ex.ToString());
                FuncLog.WriteLog_Debug(ex.StackTrace);
            }

            return null;
        }

        #endregion


        #region PIN 관련
        public static void IncreaseInputCount(int site, int array) // PIN 투입 카운트 증가
        {
            IncreaseInputCount(Util.IntToString(site, 2), array);
        }

        public static void IncreaseInputCount(string site, int array) // PIN 투입 카운트 증가
        {
            if (!GlobalVar.UseMsSQL)
            {
                return;
            }



            try
            {
                //debug("핀 투입 카운트 추가 " + site + "," + array);
                string sql = "update PinLog " +
                            "set TestCount = TestCount + 1 " +
                            "where Site = '" + site + "' " +
                            "and ArrayNo = '" + array + "' " +
                            "and Using = '1' " +
                            "IF @@ROWCOUNT=0 " +
                            "insert into PinLog values (" +
                            "CONVERT(CHAR(8), getdate(), 112), " +
                            " CONVERT(CHAR(8), getdate(), 108), " +
                            site + ", " +
                            "'FRONT', " +
                            GetPinNo(array - 1) + ", " +
                            "'', " +
                            array + ", " +
                            "0, " +
                            "0, " +
                            "1)";
                GlobalVar.Sql.Execute(sql);
            }
            catch (Exception ex)
            {
                debug("IncreaseInputCount : " + ex.ToString());
                debug(ex.StackTrace);
            }
        }

        public static void IncreaseNGCount(int site, int array) // PIN NG 카운트 증가
        {
            IncreaseNGCount(Util.IntToString(site, 2), array);
        }

        public static void IncreaseNGCount(string site, int array) // PIN NG 카운트 증가
        {
            if (!GlobalVar.UseMsSQL)
            {
                return;
            }

            try
            {
                //debug("PIN NG 카운트 추가 : " + site + "," + array);
                string sql = "update PinLog " +
                            "set NGCount = NGCount + 1 " +
                            "where Site = '" + site + "' " +
                            "and ArrayNo = '" + array + "' " +
                            "and Using = '1' " +
                            "IF @@ROWCOUNT=0 " +
                            "insert into PinLog values (" +
                            "CONVERT(CHAR(8), getdate(), 112), " +
                            " CONVERT(CHAR(8), getdate(), 108), " +
                            site + ", " +
                            "'FRONT', " +
                            GetPinNo(array - 1) + ", " +
                            "'', " +
                            array + ", " +
                            "0, " +
                            "0, " +
                            "1)";
                GlobalVar.Sql.Execute(sql);
            }
            catch (Exception ex)
            {
                debug("IncreaseNGCount : " + ex.ToString());
                debug(ex.StackTrace);
            }
        }

        // 불량율에 따른 사이트 블럭시 해당 어레이 반환
        public static int CheckDefectRateBlock(int site)
        {
            if (!GlobalVar.UseMsSQL)
            {
                return 0;
            }

            //FuncLog.WriteLog("Trace - PinLog Click");
            try
            {
                string sql = "select Site, ArrayNo, isnull(TestCount, 0), isnull(NGCount, 0) " +
                            "from PinLog " +
                            "where Using='1' " +
                            "and Site = '" + site + "' " +
                            "order by Site,ArrayNo";
                //debug(sql);
                string[,] result = GlobalVar.Sql.Read(sql);
                if (result != null &&
                    result.GetLength(0) > 0 &&
                    result.GetLength(1) > 1)
                {
                    for (int j = 0; j < result.GetLength(0); j++)
                    {
                        int array = 0;
                        int input = 0;
                        int ng = 0;
                        int.TryParse(result[0, 1], out array);
                        int.TryParse(result[0, 2], out input);
                        int.TryParse(result[0, 3], out ng);
                        if (input == 0 ||
                            input < FuncInline.DefectBlockMinIn || // 최소 투입수 미달
                            ng < FuncInline.DefectBlockMinNG) // 최소 불량수 미달
                        {
                            return 0;
                        }
                        else
                        {
                            return array;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                debug("CheckDefectRateBlock : " + ex.ToString());
                debug(ex.StackTrace);
            }
            return 0;
        }

        // 불량율에 따른 사이트 블럭 여부 검색
        public static bool CheckDefectRateBlock(int site, int array)
        {
            if (!GlobalVar.UseMsSQL)
            {
                return false;
            }

            //FuncLog.WriteLog("Trace - PinLog Click");
            try
            {
                string sql = "select isnull(TestCount, 0), isnull(NGCount, 0) " +
                            "from PinLog " +
                            "where Using='1' " +
                            "and Site = '" + site + "' " +
                            "and ArrayNo = '" + array + "' " +
                            "order by Site,ArrayNo";
                //debug(sql);
                string[,] result = GlobalVar.Sql.Read(sql);
                if (result != null &&
                    result.GetLength(0) > 0 &&
                    result.GetLength(1) > 1)
                {
                    int input = 0;
                    int ng = 0;
                    int.TryParse(result[0, 0], out input);
                    int.TryParse(result[0, 1], out ng);
                    if (input == 0 ||
                        input < FuncInline.DefectBlockMinIn || // 최소 투입수 미달
                        ng < FuncInline.DefectBlockMinNG) // 최소 불량수 미달
                    {
                        return false;
                    }
                    else
                    {
                        return ng / input > FuncInline.DefectLimit;
                    }
                }
            }
            catch (Exception ex)
            {
                debug("CheckDefectRateBlock : " + ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public static string[,] GetCurrentPinLog() // 현재 사용중인 PIN 목록 조회
        {
            if (!GlobalVar.UseMsSQL)
            {
                return null;
            }

            //FuncLog.WriteLog("Trace - PinLog Click");
            try
            {
                //string sql = "select isnull(a.Site, 0), " +
                //            "   isnull(b.PinNo, 0), " + // 어레이번호가 아닌 핀 번호로 매치하기 위해 매칭 테이블 맞들어서 조인함
                //            "   isnull(a.ArrayNo, 0), " +
                //            "   isnull(a.TestCount, 0), " +
                //            "   isnull(a.NGCount, 0) " +
                //            "from PinLog a " +
                //            "left outer join ArrayPin b " +
                //            "   on a.ArrayNo = b.ArrayNo " +
                //            "where a.Using = '1' " +
                //            "order by a.Site ,b.PinNo ";
                string sql = "select isnull(Site, 0), isnull(PinNo, 0), isnull(ArrayNo, 0), isnull(TestCount, 0), isnull(NGCount, 0) " +
                            "from PinLog " +
                            "where Using='1' " +
                            "order by Site,ArrayNo";
                //debug(sql);
                return GlobalVar.Sql.Read(sql);
            }
            catch (Exception ex)
            {
                debug("GetCurrentPinLog : " + ex.ToString());
                debug(ex.StackTrace);
            }
            return null;
        }

        public static bool CheckCurrentPin(int site, int pinNo, string pinCode) // 현재 사용중인 핀인가 조회
        {
            if (!GlobalVar.UseMsSQL)
            {
                if (GlobalVar.SystemStatus < enumSystemStatus.AutoRun)
                {
                    FuncWin.TopMessageBox("Database Use disabled");
                }
                return false;
            }
            bool exists = false;
            try
            {
                string sql = "select PinID, TestCount, NGCount " +
                    "from PinLog " +
                    "where Using = '1' " +
                    "and Site = " + site + " " +
                    "and PinNo = " + pinNo + " " +
                    "and PinID = '" + pinCode + "'";
                string[,] rs = GlobalVar.Sql.Read(sql);
                //if (rs != null)
                if (rs.GetLength(0) > 0)
                {
                    //if (rs.Read())
                    if (rs.GetLength(1) > 0)
                    {
                        exists = true;
                    }

                    //rs.Close();
                }
            }
            catch (Exception ex)
            {
                debug("CheckCurrentPin : " + ex.ToString());
                debug(ex.StackTrace);
            }
            return exists;
        }


        public static void DisablePinCode(int site, int pinNo) // 현재 사용중인 핀 사용해제
        {
            if (!GlobalVar.UseMsSQL)
            {
                return;
            }
            try
            {
                string sql = "update PinLog " +
                    "set Using = '0' " +
                    "where Using = '1' " +
                    "and Site = " + site + " " +
                    "and PinNo = " + pinNo;
                GlobalVar.Sql.Execute(sql);
            }
            catch (Exception ex)
            {
                debug("DisablePinCode : " + ex.ToString());
                debug(ex.StackTrace);
            }
        }


        public static void InsertCommLog(int site, int array, string type, string content, double testTime, int defectCode, string defectName) // 테스트 통신 로그 추가
        {
            //debug("InsertCommLog : " + site + "," + array + "," + type + "," + content);
            if (!GlobalVar.UseMsSQL)
            {
                return;
            }
            try
            {
                #region 통신로그 저장
                string sql = "insert into CommLog values (" +
                    "CONVERT(CHAR(8), getdate(), 112), " +
                    " CONVERT(CHAR(8), getdate(), 108), " +
                    site + ", " +
                    array + ", " +
                    "'" + type + "', " +
                    "'" + content.Replace("\n", "") + "', " +
                    "'')";
                GlobalVar.Sql.Execute(sql);
                //debug(sql);
                #endregion

                //*
                string code = "";
                //string defectCode = "";
                if (content.Contains("["))
                {
                    string line = content.Split('[')[0];
                    //if (line.Length > 6)
                    //{
                    //    defectCode = line.Substring(6, line.Length - 6);
                    //}
                    if (content.Contains("]"))
                    {
                        code = content.Split('[')[1].Split(']')[0];
                    }
                }
                //*/
                #region Defect Log 저장
                if (defectCode != -1 &&
                    (type == "FL" ||
                            type == "NG" ||
                            type == "SS" ||
                            type == "STS"))
                {
                    //FL210184[A1303014A9S07]
                    sql = "insert into DefectLog values (" +
                            "CONVERT(CHAR(8), getdate(), 112), " +
                            " CONVERT(CHAR(8), getdate(), 108), " +
                            site + ", " +
                            array + ", " +
                            "'" + Util.IntToString(defectCode, 2) + "', " +
                            "'" + defectName + "')";
                    //debug(sql);
                    GlobalVar.Sql.Execute(sql);
                }
                #endregion

                #region 테스트 결과 통계 저장
                // TestResult
                /*
SELECT [Date]
      ,[Time]
      ,[Site]
      ,[Array]
      ,[Barcode]
      ,[Type]
      ,[Command_Send]
      ,[Command_Receive]
      ,[Command_OK]
      ,[Test_Finish]
      ,[Test_Pass]
      ,[Test_Cancel]
      ,[User_Timeout]
      ,[Finish]
      ,[NG]
      ,[DefectCode]
      ,[TestTime]
  FROM [AutoInline].[dbo].[TestResult]

  첨에 ST 보내고 Insert, 나머지는 Update
  Update시 Type에 따라서 각 필드 update
  OK : Command_Receive T, Command_OK T
  NG : NG T
  STS : Test_Cancel T, NG T
  SS : Test_Cancel T, NG T                   
  PS : Test_Finish T, Test_Pass T, Finish T, TestTime                 
  FL : Test_Finish T, Finish T, NG T, DefectCode, TestTime
  //*/
                switch (type)
                {
                    case "ST":
                    case "STA":   // STA0701[A1303014A9N02]
                    case "STD":   // STD1502[K22023E1B40G2]
                    case "STF":
                        sql = "insert into [TestResult] " +
                              "([Date], [Time], [Site], [Array], [Barcode], [Type], [Command_Send], [DefectCode]) " +
                              "values " +
                              "(CONVERT(CHAR(8), getdate(), 112), " +  // Date
                              " CONVERT(CHAR(8), getdate(), 108), " +  // Time
                              site + ", " +
                              array + ", " +
                              "'" + code + "', " +
                              "'" + type + "', " +
                              "1, " +  // Command_Send
                              "'')"; // DefectCode
                        GlobalVar.Sql.Execute(sql);
                        break;
                    case "STS": // STS0701[A1303014A9N02]     Test_Cancel T, NG T
                        sql = "update TestResult " +
                                "set Test_Finish = 1, " +
                                        "Test_Cancel = 1, " +
                                        "NG = 1, " +
                                        "DefectCode = '998', " +
                                        "TestTime = '" + testTime.ToString("F2") + "' " +
                                "where site = " + site + " " +
                                "and array = " + array + " " +
                                "and Barcode = '" + code + "' " +
                                "and (DefectCode = '' or DefectCode = '-1') " +
                                "and NG = '0' " +
                                "and Finish = '0' " +
                                "and [Date] = (select max([Date]) " +
                                            "from TestResult " +
                                            "where site = " + site + " " +
                                            "and array = " + array + " " +
                                            "and Barcode = '" + code + "') ";// +
                                                                             //"and [Time] = (select max([Time]) " +
                                                                             //            "from TestResult " +
                                                                             //            "where site = " + site + " " +
                                                                             //            "and array = " + array + " " +
                                                                             //            "and Barcode = '" + code + "')";
                                                                             //debug(sql);
                        GlobalVar.Sql.Execute(sql);
                        break;
                    case "OK": // OK2101 Command_Receive T, Command_OK T
                        sql = "update TestResult " +
                                "set Command_Receive = 1, " +
                                        "Command_OK = 1 " +
                                "where site = " + site + " " +
                                "and array = " + array + " " +
                                //"and Barcode = '" + code + "' " +
                                "and [Date] = (select max([Date]) " +
                                            "from TestResult " +
                                            "where site = " + site + " " +
                                            "and array = " + array + ") ";// +
                                                                          //"and Barcode = '" + code + "') " +
                                                                          //"and [Time] = (select max([Time]) " +
                                                                          //            "from TestResult " +
                                                                          //            "where site = " + site + " " +
                                                                          //            "and array = " + array + ") ";// +
                                                                          //"and Barcode = '" + code + "')";
                                                                          //debug(sql);
                        GlobalVar.Sql.Execute(sql);
                        break;
                    case "NG": // NG0701 Command_Receive T, NG T
                        sql = "update TestResult " +
                                "set Test_Finish = 1, " +
                                        "Command_Receive = 1, " +
                                        "DefectCode = '" + Util.IntToString(defectCode, 2) + "', " +
                                        "NG = 1 " +
                                "where site = " + site + " " +
                                "and array = " + array + " " +
                                //"and Barcode = '" + code + "' " +
                                "and [Date] = (select max([Date]) " +
                                            "from TestResult " +
                                            "where site = " + site + " " +
                                            "and array = " + array + ") ";// +
                                                                          //"and Barcode = '" + code + "') " +
                                                                          //"and [Time] = (select max([Time]) " +
                                                                          //            "from TestResult " +
                                                                          //            "where site = " + site + " " +
                                                                          //            "and array = " + array + ") ";// +
                                                                          //"and Barcode = '" + code + "')";
                        GlobalVar.Sql.Execute(sql);
                        break;
                    case "SS": // SS0701 Test_Cancel T, NG T       
                        sql = "update TestResult " +
                                "set Test_Finish = 1, " +
                                        "Test_Cancel = 1, " +
                                        "NG = 1, " +
                                        "TestTime = '" + testTime.ToString("F2") + "' " +
                                "where site = " + site + " " +
                                "and array = " + array + " " +
                                //"and Barcode = '" + code + "' " +
                                "and [Date] = (select max([Date]) " +
                                            "from TestResult " +
                                            "where site = " + site + " " +
                                            "and array = " + array + ") ";// +
                                                                          //"and Barcode = '" + code + "') " +
                                                                          //"and [Time] = (select max([Time]) " +
                                                                          //            "from TestResult " +
                                                                          //            "where site = " + site + " " +
                                                                          //            "and array = " + array + ") ";// +
                                                                          //"and Barcode = '" + code + "')";
                        GlobalVar.Sql.Execute(sql);
                        break;
                    case "PS": // PS0704[K09044F0J09E07]  Test_Finish T, Test_Pass T, Finish T, TestTime                 
                        sql = "update TestResult " +
                                "set Test_Finish = 1, " +
                                        "Test_Pass = 1, " +
                                        "Finish = 1, " +
                                        "TestTime = '" + testTime.ToString("F2") + "' " +
                                "where site = " + site + " " +
                                "and array = " + array + " " +
                                "and Barcode = '" + code + "' " +
                                "and [Date] = (select max([Date]) " +
                                            "from TestResult " +
                                            "where site = " + site + " " +
                                            "and array = " + array + " " +
                                            "and Barcode = '" + code + "') ";// +
                                                                             //"and [Time] = (select max([Time]) " +
                                                                             //            "from TestResult " +
                                                                             //            "where site = " + site + " " +
                                                                             //            "and array = " + array + " " +
                                                                             //            "and Barcode = '" + code + "')";
                        GlobalVar.Sql.Execute(sql);
                        break;
                    case "FL": // FL0701100[K09044F0J09E07]  Test_Finish T, Finish T, NG T, DefectCode, TestTime

                        sql = "update TestResult " +
                                "set Test_Finish = 1, " +
                                        "Finish = 1, " +
                                        "NG = 1, " +
                                        "DefectCode = '" + Util.IntToString(defectCode, 2) + "', " +
                                        "TestTime = '" + testTime.ToString("F2") + "' " +
                                "where site = " + site + " " +
                                "and array = " + array + " " +
                                "and Barcode = '" + code + "' " +
                                "and [Date] = (select max([Date]) " +
                                            "from TestResult " +
                                            "where site = " + site + " " +
                                            "and array = " + array + " " +
                                            "and Barcode = '" + code + "') ";// +
                                                                             //"and [Time] = (select max([Time]) " +
                                                                             //            "from TestResult " +
                                                                             //            "where site = " + site + " " +
                                                                             //            "and array = " + array + " " +
                                                                             //            "and Barcode = '" + code + "')";
                        GlobalVar.Sql.Execute(sql);
                        break;
                }
                #endregion
            }
            catch (Exception ex)
            {
                debug("InsertCommLog : " + ex.ToString());
                debug(ex.StackTrace);
                FuncLog.WriteLog_Debug("InsertCommLog : " + ex.ToString());
                FuncLog.WriteLog_Debug(ex.StackTrace);
            }
        }

        public static string GetModuleCodeFromSiteNo(int siteNo)
        {
            if (!GlobalVar.UseMsSQL)
            {
                return "";
            }
            string[,] reader = null;
            try
            {
                // 사용중인 모듈 조회
                string sql = "select ModuleID " +
                    "from ModuleLog " +
                    "where Site = '" + siteNo + "' " +
                    "and using = '1'";
                reader = GlobalVar.Sql.Read(sql);
                //if (reader != null)
                if (reader.GetLength(0) > 0)
                {
                    //if (reader.Read())
                    if (reader.GetLength(1) > 0)
                    {
                        string code = reader[0, 0].ToString();

                        //reader.Close();
                        return code;
                    }

                    //reader.Close();
                }
            }
            catch (Exception ex)
            {
                debug("CheckPinCodeUsing : " + ex.ToString());
                debug(ex.StackTrace);
            }
            //if (reader != null)
            //{
            //    reader.Close();
            //}
            return "";
        }

        public static string[,] GetPinCodeFromSiteNo(int siteNo)
        {
            if (!GlobalVar.UseMsSQL)
            {
                return null;
            }
            string[,] reader = null;
            try
            {
                // 사용중인 모듈 조회
                string sql = "select PinNo, PinID " +
                    "from PinLog " +
                    "where Site = '" + siteNo + "' " +
                    "and using = '1' " +
                    "order by PinNo";
                reader = GlobalVar.Sql.Read(sql);
            }
            catch (Exception ex)
            {
                debug("CheckPinCodeUsing : " + ex.ToString());
                debug(ex.StackTrace);
            }
            return reader;
        }

        public static int GetSiteNoFromModuleCode(string moduleCode) // 지정 Module코드가 사용중인 사이트 가져오기?
        {
            if (!GlobalVar.UseMsSQL)
            {
                return -1;
            }
            string[,] reader = null;
            try
            {
                // 사용중인 모듈 조회
                string sql = "select Site " +
                    "from ModuleLog " +
                    "where ModuleID = '" + moduleCode + "' " +
                    "and using = '1'";
                reader = GlobalVar.Sql.Read(sql);
                //if (reader != null)
                if (reader.GetLength(0) > 0)
                {
                    //if (reader.Read())
                    if (reader.GetLength(1) > 0)
                    {
                        int site = int.Parse(reader[0, 0].ToString());

                        //reader.Close();
                        return site;
                    }

                    //reader.Close();
                }
            }
            catch (Exception ex)
            {
                debug("CheckPinCodeUsing : " + ex.ToString());
                debug(ex.StackTrace);
            }
            //if (reader != null)
            //{
            //    reader.Close();
            //}
            return -1;
        }

        public static void InsertModuleLog(int site, string moduleCode) // 모듈 사용로그 추가, 모듈 교체시. 코드 같으면 무시
        {
            if (!GlobalVar.UseMsSQL)
            {
                return;
            }
            try
            {
                string beforeCode = "";
                // 사용중인 핀코드 조회
                string sql = "select moduleID " +
                    "from ModuleLog " +
                    "where site = " + site + " " +
                    "and using = '1'";
                string[,] reader = GlobalVar.Sql.Read(sql);
                //if (reader != null)
                if (reader.GetLength(0) > 0)
                {
                    //if (reader.Read())
                    if (reader.GetLength(1) > 0)
                    {
                        beforeCode = reader[0, 0].ToString();
                    }

                    //reader.Close();
                }

                if (beforeCode == moduleCode) // 기존 사용중인 코드와 같으면 무시
                {
                    return;
                }

                // 기존 사용중인 로그 사용중지
                sql = "update ModuleLog " +
                    "set Using = '0' " +
                    "where site = " + site;
                GlobalVar.Sql.Execute(sql);

                // 새 핀 정보 추가
                sql = "insert into ModuleLog values (" +
                    "CONVERT(CHAR(8), getdate(), 112), " +
                    " CONVERT(CHAR(8), getdate(), 108), " +
                    site + ", " +
                    "'" + moduleCode + "', " +
                    "0, " +
                    "1)";
                GlobalVar.Sql.Execute(sql);
            }
            catch (Exception ex)
            {
                debug("InsertPinLog : " + ex.ToString());
                debug(ex.StackTrace);
            }
        }


        public static bool GetPinCodeSite(string pinCode, ref int site, ref int pinNo) // 지정 pin코드가 사용중인 사이트와 어레이 가져오기?
        {
            if (!GlobalVar.UseMsSQL)
            {
                return false;
            }
            string[,] reader = null;
            try
            {
                // 사용중인 핀코드 조회
                string sql = "select Site, PinNo " +
                    "from PinLog " +
                    "where pinID = '" + pinNo + "' " +
                    "and using = '1'";
                reader = GlobalVar.Sql.Read(sql);
                //if (reader != null)
                if (reader.GetLength(0) > 0)
                {
                    if (reader.GetLength(1) > 1)
                    {
                        site = int.Parse(reader[0, 0].ToString());
                        pinNo = int.Parse(reader[0, 1].ToString());

                        //reader.Close();
                        return true;
                    }

                    //reader.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                debug("CheckPinCodeUsing : " + ex.ToString());
                debug(ex.StackTrace);
            }
            //if (reader != null)
            //{
            //    reader.Close();
            //}
            return false;
        }

        public static bool CheckPinCodeUsing(int site, int pinNo, string pinCode) // 지정 pin이 같은 코드로 사용중인가?
        {
            if (!GlobalVar.UseMsSQL)
            {
                return false;
            }
            try
            {
                string beforeCode = "";
                // 사용중인 핀코드 조회
                string sql = "select pinCode " +
                    "from PinLog " +
                    "where site = " + site + " " +
                    "and pinNo = " + pinNo + " " +
                    "and using = '1'";
                string[,] reader = GlobalVar.Sql.Read(sql);
                //if (reader != null)
                if (reader.GetLength(0) > 0)
                {
                    //if (reader.Read())
                    if (reader.GetLength(1) > 0)
                    {
                        beforeCode = reader[0, 0].ToString();
                    }

                    //reader.Close();
                }
                return pinCode == beforeCode;
            }
            catch (Exception ex)
            {
                debug("CheckPinCodeUsing : " + ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public static bool CheckPinCodeUsingInOther(int site, int pinNo, string pinCode) // 지정 pin이 다른 사이트나 핀에 사용중인가?
        {
            if (!GlobalVar.UseMsSQL)
            {
                return false;
            }
            try
            {
                if (pinCode != "")
                {
                    return false;
                }

                string beforeCode = "";
                // 사용중인 핀코드 조회
                string sql = "select pinCode " +
                    "from PinLog " +
                    "where (site <> " + site + " " +
                    "or pinNo <> " + pinNo + ") " +
                    "and pinCode = '" + pinCode + "' " +
                    "and using = '1'";
                string[,] reader = GlobalVar.Sql.Read(sql);
                //if (reader != null)
                if (reader.GetLength(0) > 0)
                {
                    //if (reader.Read())
                    if (reader.GetLength(1) > 0)
                    {
                        beforeCode = reader[0, 0].ToString();
                    }

                    //reader.Close();
                }
                return pinCode == beforeCode;
            }
            catch (Exception ex)
            {
                debug("CheckPinCodeUsing : " + ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public static void PlusPinUseByPin(int site, int pinNo, bool insert, bool ng) // 핀번호 따라 핀사용값 증가
        {
            if (!GlobalVar.UseMsSQL)
            {
                return;
            }
            try
            {
                string sql = "update PinLog " +
                    "set Using = '1', " +
                    (insert ? "TestCount = TestCount + 1," : "") +
                    (ng ? "NGCount = NGCount + 1," : "") +
                    "where site = " + site + " " +
                    "and PinNo = " + pinNo + " " +
                    "and Using = '1'";
                GlobalVar.Sql.Execute(sql);

            }
            catch (Exception ex)
            {
                debug("InsertPinLog : " + ex.ToString());
                debug(ex.StackTrace);
            }
        }

        public static void PlusPinUseByArray(int site, int arrayNo, bool insert, bool ng) // 어레이번호 따라 핀사용값 증가
        {
            if (!GlobalVar.UseMsSQL)
            {
                return;
            }
            try
            {
                if (!insert &&
                    !ng)
                {
                    return;
                }

                string sql = "update PinLog " +
                    "set Using = '1', " +
                    (insert ? "TestCount = TestCount + 1" : "") +
                    (insert && ng ? "," : "") +
                    (ng ? "NGCount = NGCount + 1" : "") +
                    "where site = " + site + " " +
                    "and ArrayNo = " + arrayNo + " " +
                    "and Using = '1'";
                //debug(sql);
                GlobalVar.Sql.Execute(sql);

            }
            catch (Exception ex)
            {
                debug("InsertPinLog : " + ex.ToString());
                debug(ex.StackTrace);
            }
        }

        public static void InsertPinLog(int site, int side, int pinNo, string pinCode, int arrayNo) // 핀 사용로그 추가, 핀 교체시. 코드 같으면 무시
        {
            if (!GlobalVar.UseMsSQL)
            {
                return;
            }
            try
            {
                string beforeCode = "";
                // 사용중인 핀코드 조회
                string sql = "select pinID " +
                    "from PinLog " +
                    "where site = " + site + " " +
                    "and pinNo = " + pinNo + " " +
                    "and arrayNo = " + arrayNo + " " +
                    "and using = '1'";
                string[,] reader = GlobalVar.Sql.Read(sql);

                //if (reader != null)
                if (reader.GetLength(0) > 0)
                {
                    //if (reader.Read())
                    if (reader.GetLength(1) > 0)
                    {
                        beforeCode = reader[0, 0].ToString();
                    }

                    //reader.Close();
                }

                if (beforeCode == pinCode) // 기존 사용중인 코드와 같으면 무시
                {
                    return;
                }

                // 기존 사용중인 로그 사용중지
                sql = "update PinLog " +
                    "set Using = '0' " +
                    "where site = " + site + " " +
                    "and PinNo = " + pinNo + " " +
                    "and ArrayNo = " + arrayNo;
                //debug(sql);
                GlobalVar.Sql.Execute(sql);

                // 새 핀 정보 추가
                sql = "insert into PinLog values (" +
                    "CONVERT(CHAR(8), getdate(), 112), " +
                    " CONVERT(CHAR(8), getdate(), 108), " +
                    site + ", " +
                    side + ", " +
                    pinNo + ", " +
                    "'" + pinCode + "', " +
                    arrayNo + ", " +
                    "0, " +
                    "0, " +
                    "1)";
                //debug(sql);
                GlobalVar.Sql.Execute(sql);
            }
            catch (Exception ex)
            {
                debug("InsertPinLog : " + ex.ToString());
                debug(ex.StackTrace);
            }
        }

        public static void ClearPinTestCount(int site, int arrayNo, bool ng) // 해당 핀의 핀 투입/NG카운트 초기화
        {
            //debug("ClearPinTestCount " + site + "," + arrayNo + "," + ng);
            if (!GlobalVar.UseMsSQL)
            {
                return;
            }
            try
            {
                string sql = "update PinLog " +
                "set " + (ng ? "NGCount" : "TestCount") + " = 0 " +
                "where site = " + site + " " +
                (arrayNo > 0 ? "and ArrayNo = " + arrayNo + " " : "") +
                "and Using = 1";
                GlobalVar.Sql.Execute(sql);
            }
            catch (Exception ex)
            {
                debug("ClearPinTestCount : " + ex.ToString());
                debug(ex.StackTrace);
            }
        }

        public static void ClearAllPinCount() // 모든 핀 투입/NG카운트 초기화
        {
            if (!GlobalVar.UseMsSQL)
            {
                return;
            }
            try
            {
                string sql = "update PinLog " +
                            "set TestCount = 0, NGCount = 0 " +
                            "where Using = 1";
                GlobalVar.Sql.Execute(sql);
            }
            catch (Exception ex)
            {
                debug("ClearAllPinCount : " + ex.ToString());
                debug(ex.StackTrace);
            }
        }

        #endregion


        #region 시스템 에러 관련
        public static string[,] GetUnClearedSystemError() // 클리어 안 된 알람 조회
        {
            if (!GlobalVar.UseMsSQL)
            {
                return null;
            }

            try
            {
                String sql = "select [Date],[Time],[Part],[ErrorCode],[ErrorName],[Description] " +
                    "from [SystemError] " +
                    "where [Clear] <> '1' " +
                    "order by [Date] desc, [Time] desc";
                //debug(sql);
                return GlobalVar.Sql.Read(sql);
            }
            catch (Exception ex)
            {
                debug("GetUnClearedSystemError : " + ex.ToString());
                debug(ex.StackTrace);
            }
            return null;
        }

        public static string[,] GetSystemError(string startDate, string endDate, bool shiftA, bool shiftB, bool shiftC) // 두 날짜 사이의 시스템로그 조회
        {
            if (!GlobalVar.UseMsSQL)
            {
                return null;
            }

            try
            {
                // 2교대시 A조는 해당날짜, B조는 해당날짜 B조시간부터 다음날 A조시간 이전까지
                // 3교대시 A/B조는 해당날짜, C조는 해당날짜 C조시간부터 다음날 A조 시간 이전까지.
                // 검색시 날짜+시간 조합으로 시작날짜 가장 먼저 조 시작시간부터
                //                           끝조 미선택시 마지막날 마지막 시간범위
                //                           끝조 선택시 마지막 다음날 첫조 시간 이전까지 범위로 검색
                // Trace 폼으로 들어올 때 마지막조고 A조 시간 이전이면 이전날짜로 세팅해야 함

                #region 검색 일시 범위 지정
                string startTime = startDate;
                if (shiftA)
                {
                    startTime += Util.IntToString(FuncInline.ShiftAHour, 2) + ":" + Util.IntToString(FuncInline.ShiftAMin, 2) + ":00";
                }
                else if (shiftB)
                {
                    startTime += Util.IntToString(FuncInline.ShiftBHour, 2) + ":" + Util.IntToString(FuncInline.ShiftBMin, 2) + ":00";
                }
                else if (shiftC)
                {
                    startTime += Util.IntToString(FuncInline.ShiftCHour, 2) + ":" + Util.IntToString(FuncInline.ShiftCMin, 2) + ":00";
                }
                string endTime = endDate;
                #region 마지막조 포함시
                if ((!FuncInline.UseShiftC && shiftB) ||
                    (FuncInline.UseShiftC && shiftC))
                {
                    // 종료 날짜의 다음날 A조 시작 전 시간을 구한다.
                    int searchYear = int.Parse(endDate.Substring(0, 4));
                    int searchMonth = int.Parse(endDate.Substring(4, 2));
                    int searchDate = int.Parse(endDate.Substring(6, 2));
                    int searchHour = FuncInline.ShiftAHour;
                    int searchMin = FuncInline.ShiftAMin;
                    DateTime searchDateTime = new DateTime(searchYear, searchMonth, searchDate, searchHour, searchMin, 0);
                    searchDateTime = searchDateTime.AddDays(1);
                    searchDateTime = searchDateTime.AddSeconds(-1);
                    endTime = searchDateTime.ToString("yyyyMMddHH:mm:ss");
                }
                #endregion
                #region 마지막조 미포함시
                else
                {
                    // 종료 날짜의 선택된 마지막 조 종료 시간을 구한다.
                    int searchYear = int.Parse(endDate.Substring(0, 4));
                    int searchMonth = int.Parse(endDate.Substring(4, 2));
                    int searchDate = int.Parse(endDate.Substring(6, 2));
                    int searchHour = FuncInline.ShiftAHour;
                    int searchMin = FuncInline.ShiftAMin;
                    // A조 선택시 B조시간
                    if (shiftA)
                    {
                        searchHour = FuncInline.ShiftBHour;
                        searchMin = FuncInline.ShiftBMin;
                    }
                    // B조 선택시 C조시간
                    if (shiftB)
                    {
                        searchHour = FuncInline.ShiftCHour;
                        searchMin = FuncInline.ShiftCMin;
                    }
                    DateTime searchDateTime = new DateTime(searchYear, searchMonth, searchDate, searchHour, searchMin, 0);
                    searchDateTime = searchDateTime.AddSeconds(-1); // 1분을 빼서 조 마지막 시간으로 세팅
                    endTime = searchDateTime.ToString("yyyyMMddHH:mm:ss");
                }
                #endregion
                #endregion

                string sql = "select [Date],[Time],[Part],[ErrorCode],[ErrorName],[Clear],[Description] " +
                "from [SystemError] " +
                                "where concat(date, time) >= '" + startTime + "' " +
                                "and concat(date, time) <= '" + endTime + "' " +
                                "order by date desc, time desc";
                //debug(sql);
                return GlobalVar.Sql.Read(sql);
            }
            catch (Exception ex)
            {
                debug("GetSystemError : " + ex.ToString());
                debug(ex.StackTrace);
            }
            return null;
        }

        public static string[,] GetSystemError(string startDate, string endDate) // 두 날짜 사이의 시스템로그 조회
        {
            if (!GlobalVar.UseMsSQL)
            {
                return null;
            }

            try
            {
                string sql = "select [Date],[Time],[Part],[ErrorCode],[ErrorName],[Clear],[Description] " +
                                "from [SystemError] " +
                                "where date >= '" + startDate + "' " +
                                "and date <= '" + endDate + "' " +
                                "order by date desc, time desc";
                return GlobalVar.Sql.Read(sql);
            }
            catch (Exception ex)
            {
                debug("GetSystemError : " + ex.ToString());
                debug(ex.StackTrace);
            }
            return null;
        }
        public static string[,] GetSystemErrorCodeCount(string startDate, string endDate) // 두 날짜 사이의 코드별 알람 카운트 조회
        {
            try
            {
                string sql = "select ErrorCode,ErrorName, count(ErrorCode) as count " +
                            "from[SystemError] " +
                            "where date >= '" + startDate + "' " +
                            "and date <= '" + endDate + "' " +
                            "group by ErrorCode,ErrorName";
                return GlobalVar.Sql.Read(sql);
            }
            catch (Exception ex)
            {
                debug("GetSystemErrorCode : " + ex.ToString());
                debug(ex.StackTrace);
            }
            return null;
        }

        public static string[,] GetSystemErrorPartCount(string startDate, string endDate) // 두 날짜 사이의 파트별 알람 카운트 조회
        {
            try
            {
                string sql = "select Part,count(Part) as count " +
                                    "from[SystemError] " +
                                    "where date >= '" + startDate + "' " +
                                    "and date <= '" + endDate + "' " +
                                    "group by part";
                return GlobalVar.Sql.Read(sql);
            }
            catch (Exception ex)
            {
                debug("GetSystemErrorPartCount : " + ex.ToString());
                debug(ex.StackTrace);
            }
            return null;
        }
        public static string[,] GetSystemErrorPartCount(string startDate, string endDate, bool shiftA, bool shiftB, bool shiftC) // 두 날짜 사이의 파트별 알람 카운트 조회
        {
            try
            {
                #region 검색 일시 범위 지정
                string startTime = startDate;
                if (shiftA)
                {
                    startTime += Util.IntToString(FuncInline.ShiftAHour, 2) + ":" + Util.IntToString(FuncInline.ShiftAMin, 2) + ":00";
                }
                else if (shiftB)
                {
                    startTime += Util.IntToString(FuncInline.ShiftBHour, 2) + ":" + Util.IntToString(FuncInline.ShiftBMin, 2) + ":00";
                }
                else if (shiftC)
                {
                    startTime += Util.IntToString(FuncInline.ShiftCHour, 2) + ":" + Util.IntToString(FuncInline.ShiftCMin, 2) + ":00";
                }
                string endTime = endDate;
                #region 마지막조 포함시
                if ((!FuncInline.UseShiftC && shiftB) ||
                    (FuncInline.UseShiftC && shiftC))
                {
                    // 종료 날짜의 다음날 A조 시작 전 시간을 구한다.
                    int searchYear = int.Parse(endDate.Substring(0, 4));
                    int searchMonth = int.Parse(endDate.Substring(4, 2));
                    int searchDate = int.Parse(endDate.Substring(6, 2));
                    int searchHour = FuncInline.ShiftAHour;
                    int searchMin = FuncInline.ShiftAMin;
                    DateTime searchDateTime = new DateTime(searchYear, searchMonth, searchDate, searchHour, searchMin, 0);
                    searchDateTime = searchDateTime.AddDays(1);
                    searchDateTime = searchDateTime.AddSeconds(-1);
                    endTime = searchDateTime.ToString("yyyyMMddHH:mm:ss");
                }
                #endregion
                #region 마지막조 미포함시
                else
                {
                    // 종료 날짜의 선택된 마지막 조 종료 시간을 구한다.
                    int searchYear = int.Parse(endDate.Substring(0, 4));
                    int searchMonth = int.Parse(endDate.Substring(4, 2));
                    int searchDate = int.Parse(endDate.Substring(6, 2));
                    int searchHour = FuncInline.ShiftAHour;
                    int searchMin = FuncInline.ShiftAMin;
                    // A조 선택시 B조시간
                    if (shiftA)
                    {
                        searchHour = FuncInline.ShiftBHour;
                        searchMin = FuncInline.ShiftBMin;
                    }
                    // B조 선택시 C조시간
                    if (shiftB)
                    {
                        searchHour = FuncInline.ShiftCHour;
                        searchMin = FuncInline.ShiftCMin;
                    }
                    DateTime searchDateTime = new DateTime(searchYear, searchMonth, searchDate, searchHour, searchMin, 0);
                    searchDateTime = searchDateTime.AddSeconds(-1); // 1분을 빼서 조 마지막 시간으로 세팅
                    endTime = searchDateTime.ToString("yyyyMMddHH:mm:ss");
                }
                #endregion
                #endregion

                string sql = "select Part,count(Part) as count " +
                                    "from[SystemError] " +
                                    "where concat(date, time) >= '" + startTime + "' " +
                                    "and concat(date, time) <= '" + endTime + "' " +
                                    "group by part";
                return GlobalVar.Sql.Read(sql);
            }
            catch (Exception ex)
            {
                debug("GetSystemErrorPartCount : " + ex.ToString());
                debug(ex.StackTrace);
            }
            return null;
        }

        public static string[,] GetSystemErrorCodeCount(string startDate, string endDate, bool shiftA, bool shiftB, bool shiftC) // 두 날짜 사이의 코드별 알람 카운트 조회
        {
            try
            {
                #region 검색 일시 범위 지정
                string startTime = startDate;
                if (shiftA)
                {
                    startTime += Util.IntToString(FuncInline.ShiftAHour, 2) + ":" + Util.IntToString(FuncInline.ShiftAMin, 2) + ":00";
                }
                else if (shiftB)
                {
                    startTime += Util.IntToString(FuncInline.ShiftBHour, 2) + ":" + Util.IntToString(FuncInline.ShiftBMin, 2) + ":00";
                }
                else if (shiftC)
                {
                    startTime += Util.IntToString(FuncInline.ShiftCHour, 2) + ":" + Util.IntToString(FuncInline.ShiftCMin, 2) + ":00";
                }
                string endTime = endDate;
                #region 마지막조 포함시
                if ((!FuncInline.UseShiftC && shiftB) ||
                    (FuncInline.UseShiftC && shiftC))
                {
                    // 종료 날짜의 다음날 A조 시작 전 시간을 구한다.
                    int searchYear = int.Parse(endDate.Substring(0, 4));
                    int searchMonth = int.Parse(endDate.Substring(4, 2));
                    int searchDate = int.Parse(endDate.Substring(6, 2));
                    int searchHour = FuncInline.ShiftAHour;
                    int searchMin = FuncInline.ShiftAMin;
                    DateTime searchDateTime = new DateTime(searchYear, searchMonth, searchDate, searchHour, searchMin, 0);
                    searchDateTime = searchDateTime.AddDays(1);
                    searchDateTime = searchDateTime.AddSeconds(-1);
                    endTime = searchDateTime.ToString("yyyyMMddHH:mm:ss");
                }
                #endregion
                #region 마지막조 미포함시
                else
                {
                    // 종료 날짜의 선택된 마지막 조 종료 시간을 구한다.
                    int searchYear = int.Parse(endDate.Substring(0, 4));
                    int searchMonth = int.Parse(endDate.Substring(4, 2));
                    int searchDate = int.Parse(endDate.Substring(6, 2));
                    int searchHour = FuncInline.ShiftAHour;
                    int searchMin = FuncInline.ShiftAMin;
                    // A조 선택시 B조시간
                    if (shiftA)
                    {
                        searchHour = FuncInline.ShiftBHour;
                        searchMin = FuncInline.ShiftBMin;
                    }
                    // B조 선택시 C조시간
                    if (shiftB)
                    {
                        searchHour = FuncInline.ShiftCHour;
                        searchMin = FuncInline.ShiftCMin;
                    }
                    DateTime searchDateTime = new DateTime(searchYear, searchMonth, searchDate, searchHour, searchMin, 0);
                    searchDateTime = searchDateTime.AddSeconds(-1); // 1분을 빼서 조 마지막 시간으로 세팅
                    endTime = searchDateTime.ToString("yyyyMMddHH:mm:ss");
                }
                #endregion
                #endregion

                string sql = "select ErrorCode,ErrorName, count(ErrorCode) as count " +
                            "from[SystemError] " +
                            "where concat(date, time) >= '" + startTime + "' " +
                            "and concat(date, time) <= '" + endTime + "' " +
                            "group by ErrorCode,ErrorName";
                return GlobalVar.Sql.Read(sql);
            }
            catch (Exception ex)
            {
                debug("GetSystemErrorCode : " + ex.ToString());
                debug(ex.StackTrace);
            }
            return null;
        }

        public static void ClearSystemErrorCode(int code) // 해당코드의 알람 클리어
        {
            try
            {
                string sql = "update [SystemError] " +
                    "set " +
                    "[Clear] = '1' " +
                    "where [ErrorCode] = " + code + " " +
                    "and [Clear] = '0'";
                GlobalVar.Sql.Execute(sql);
            }
            catch (Exception ex)
            {
                debug("ClearSystemErrorCode : " + ex.ToString());
                debug(ex.StackTrace);
            }
        }

        public static void ClearSystemErrorPart(string part) // 해당 파트의 알람 클리어
        {
            try
            {
                string sql = "update [SystemError] " +
                    "set " +
                    "[Clear] = '1' " +
                    "where [Part] = '" + part + "' " +
                    "and [Clear] = '0'";
                GlobalVar.Sql.Execute(sql);
            }
            catch (Exception ex)
            {
                debug("ClearSystemErrorPart : " + ex.ToString());
                debug(ex.StackTrace);
            }
        }

        public static void ClearSystemError(string part, int code) // 해당 파트/코드의 알람 클리어
        {
            try
            {
                string sql = "update [SystemError] " +
                "set " +
                "[Clear] = '1' " +
                "where [Part] = " + part + " " +
                "and [ErrorCode] = " + code + " " +
                "and [Clear] = '0'";
                GlobalVar.Sql.Execute(sql);
            }
            catch (Exception ex)
            {
                debug("ClearSystemError : " + ex.ToString());
                debug(ex.StackTrace);
            }
        }

        public static void ClearAllSystemError() // 전체 알람 초기화
        {
            try
            {
                string sql = "update [SystemError] " +
                                "set " +
                                "[Clear] = '1' " +
                                "where [Clear] = '0'";
                GlobalVar.Sql.Execute(sql);
            }
            catch (Exception ex)
            {
                debug("ClearAllSystemError : " + ex.ToString());
                debug(ex.StackTrace);
            }
        }

        public static void AddSystemError(structError er) // 알람발생 AddError시 데이터베이스에 추가
        {
            if (!GlobalVar.UseMsSQL)
            {
                return;
            }
            string sql = "";
            try
            {
                string desc = er.Description;
                if (desc == null)
                {
                    desc = "";
                }
                if (desc.Length > 200)
                {
                    desc = desc.Substring(0, 200);
                }
                sql = "insert into [SystemError] " +
                                "values (CONVERT(CHAR(8), getdate(), 112), " +
                                    " CONVERT(CHAR(8), getdate(), 108), " +
                                    "'" + er.ErrorPart.ToString() + "', " +
                                    ((int)er.ErrorCode).ToString() + ", " +
                                    "'" + er.ErrorCode.ToString() + "', " +
                                    "'" + desc + "', " +
                                    "'0')";
                GlobalVar.Sql.Execute(sql);
                //debug(sql);
            }
            catch (Exception ex)
            {
                debug("AddSystemError : " + ex.ToString() + sql);
                debug(ex.StackTrace);
            }
        }

        #endregion

        #endregion

        #region PCB 운영 관련
        public static bool CheckPCBInMachine() // 장비 전체 PCB 있는가?
        {
            try
            {
                for (int i = 0; i < MaxSiteCount; i++)
                {
                    if (FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
                    {
                        return true;
                    }

                }

                bool exists = PCBInfo[(int)enumTeachingPos.InConveyor].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                    PCBInfo[(int)enumTeachingPos.InShuttle].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                    PCBInfo[(int)enumTeachingPos.NgBuffer].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                    PCBInfo[(int)enumTeachingPos.OutShuttle_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                    PCBInfo[(int)enumTeachingPos.OutConveyor].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                    PCBInfo[(int)enumTeachingPos.FrontPassLine].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                    PCBInfo[(int)enumTeachingPos.RearPassLine].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                    PCBInfo[(int)enumTeachingPos.RearNGLine].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                    PCBInfo[(int)enumTeachingPos.Lift1_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                    PCBInfo[(int)enumTeachingPos.Lift1_Down].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                    PCBInfo[(int)enumTeachingPos.Lift2_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                    PCBInfo[(int)enumTeachingPos.Lift2_Down].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                    PCBInfo[(int)enumTeachingPos.FrontScanSite].PCBStatus != FuncInline.enumSMDStatus.UnKnown;

                for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumDINames)).Length; i++)
                {
                    string di_name = ((FuncInline.enumDINames)i).ToString();
                    if (di_name.Contains("PCB_Dock_Sensor") ||
                        di_name.Contains("PCB_Start_Sensor") ||
                        di_name.Contains("Pcb_In_Sensor") ||
                        di_name.Contains("PCB_Stop_Sensor"))
                    {
                        if (DIO.GetDIData(i))
                        {
                            exists = true;
                            break;
                        }
                    }
                }
                return exists;
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
            return true;

          
        }

        public static bool CheckNoPCB() // OutputStop 적용시 처리중인 PCB 있는지 체크
        {
            try
            {
                for (int i = 0; i < MaxSiteCount; i++)
                {
                    if (PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
                    {
                        return false;
                    }
                }
                if (PCBInfo[(int)enumTeachingPos.InConveyor].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                    PCBInfo[(int)enumTeachingPos.InShuttle].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                    PCBInfo[(int)enumTeachingPos.Lift1_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                    PCBInfo[(int)enumTeachingPos.Lift1_Down].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                    PCBInfo[(int)enumTeachingPos.FrontPassLine].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                    PCBInfo[(int)enumTeachingPos.Lift2_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                    PCBInfo[(int)enumTeachingPos.Lift2_Down].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                    PCBInfo[(int)enumTeachingPos.RearNGLine].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                    PCBInfo[(int)enumTeachingPos.RearPassLine].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                    PCBInfo[(int)enumTeachingPos.OutShuttle_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
                {
                    return false;
                }
                if (FuncInline.PassMode)
                {
                    if (FuncInline.PCBInfo[(int)enumTeachingPos.InShuttle].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                        PCBInfo[(int)enumTeachingPos.Lift1_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                        PCBInfo[(int)enumTeachingPos.FrontPassLine].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                        PCBInfo[(int)enumTeachingPos.OutShuttle_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
            return true;
        }

        public static void ClearPCBInfo(enumTeachingPos pos) // 해당 위치 PCB 정보 삭제
        {


            //debug("ClearPCBInfo : " + pos.ToString());
            try
            {
                PCBInfo[(int)pos] = new PCBInfoClass();
            }
            catch (Exception ex)
            {
                //debug("ClearPCBInfo : " + ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        public static FuncInline.enumErrorPart TeachingPosToErrorPart(enumTeachingPos pos)
        {
            switch (pos)
            {
                case enumTeachingPos.InConveyor:
                    return FuncInline.enumErrorPart.InConveyor;
                case enumTeachingPos.InShuttle:
                    return FuncInline.enumErrorPart.InShuttle;
                case enumTeachingPos.NgBuffer:
                    return FuncInline.enumErrorPart.NgBuffer;
                case enumTeachingPos.OutShuttle_Up:
                    return FuncInline.enumErrorPart.OutShuttle_Up;
                case enumTeachingPos.OutShuttle_Down:
                    return FuncInline.enumErrorPart.OutShuttle_Down;
                case enumTeachingPos.OutConveyor:
                    return FuncInline.enumErrorPart.OutConveyor;
                case enumTeachingPos.FrontPassLine:
                    return FuncInline.enumErrorPart.FrontPassLine;
                case enumTeachingPos.RearPassLine:
                    return FuncInline.enumErrorPart.RearPassLine;
                case enumTeachingPos.RearNGLine:
                    return FuncInline.enumErrorPart.RearNGLine;
                case enumTeachingPos.Lift1_Up:
                    return FuncInline.enumErrorPart.Lift1_Up;
                case enumTeachingPos.Lift1_Down:
                    return FuncInline.enumErrorPart.Lift1_Down;
                case enumTeachingPos.Lift2_Up:
                    return FuncInline.enumErrorPart.Lift2_Up;
                case enumTeachingPos.Lift2_Down:
                    return FuncInline.enumErrorPart.Lift2_Down;
                case enumTeachingPos.FrontScanSite:
                    return FuncInline.enumErrorPart.FrontScanSite;

                case enumTeachingPos.Site1_F_DT1:
                case enumTeachingPos.Site2_F_DT2:
                case enumTeachingPos.Site3_F_DT3:
                case enumTeachingPos.Site4_F_DT4:
                case enumTeachingPos.Site5_F_DT5:
                case enumTeachingPos.Site6_F_DT6:
                case enumTeachingPos.Site7_F_DT7:
                case enumTeachingPos.Site8_F_DT8:
                case enumTeachingPos.Site9_F_DT9:
                case enumTeachingPos.Site10_F_DT10_FT4:
                case enumTeachingPos.Site11_F_FT1:
                case enumTeachingPos.Site12_F_FT2:
                case enumTeachingPos.Site13_F_FT3:
                case enumTeachingPos.Site14_R_DT1:
                case enumTeachingPos.Site15_R_DT2:
                case enumTeachingPos.Site16_R_DT3:
                case enumTeachingPos.Site17_R_DT4:
                case enumTeachingPos.Site18_R_DT5:
                case enumTeachingPos.Site19_R_DT6:
                case enumTeachingPos.Site20_R_DT7:
                case enumTeachingPos.Site21_R_DT8:
                case enumTeachingPos.Site22_R_DT9:
                case enumTeachingPos.Site23_R_DT10_FT4:
                case enumTeachingPos.Site24_R_FT1:
                case enumTeachingPos.Site25_R_FT2:
                case enumTeachingPos.Site26_R_FT3:
                    return FuncInline.enumErrorPart.Site1_F_DT1 + (int)pos - (int)FuncInline.enumErrorPart.Site1_F_DT1;

            }
            return FuncInline.enumErrorPart.System;

        }


        // 포인터로 치환하려면 포인터의 사이즈 정보가 미리 정해져야 하는데
        // structPCBInfo 구조체가 너무 복잡해서 형식의 사이즈 정보를 가져올 수 없어 포인터를 이용할 수 없다.
        public static void MovePCBInfo(enumTeachingPos startPos, enumTeachingPos endPos) // 두 위치 간 PCB 정보 이전
        {
            try
            {


                //debug("MovePCBInfo " + startPos.ToString() + " ==> " + endPos.ToString());
                //debug("TestTime : " + FuncInline.PCBInfo[(int)startPos].TestTime.ToString());
                //debug("TestPass : " + FuncInline.PCBInfo[(int)startPos].TestPass);
                //debug("PCBStatus : " + FuncInline.PCBInfo[(int)startPos].PCBStatus);
                // 이전할 정보가 있을 경우에만 실행
                if (FuncInline.PCBInfo[(int)startPos].PCBStatus == FuncInline.enumSMDStatus.UnKnown)
                {
                    //debug("no pcb info : " + startPos.ToString() + " ==> " + endPos.ToString());
                    return;
                }
                if (FuncInline.PCBInfo[(int)endPos].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
                {
                    //debug("pcb info already exist : " + startPos.ToString() + " ==> " + endPos.ToString());
                    // 시작 위치 파트로

                    FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
                                DateTime.Now.ToString("HH:mm:ss"),
                                TeachingPosToErrorPart(startPos), // 일단 system으로 해 두고 조건 확인해서 변경
                                enumErrorCode.PCB_Info_Move_Fail,
                                false,
                                "PCB Info move tried while " + endPos.ToString() + " PCB info already exist."));
                    FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
                                DateTime.Now.ToString("HH:mm:ss"),
                                TeachingPosToErrorPart(endPos), // 일단 system으로 해 두고 조건 확인해서 변경
                                enumErrorCode.PCB_Info_Move_Fail,
                                false,
                                "PCB Info move tried while " + endPos.ToString() + " PCB info already exist."));
                    return;
                }

                FuncInline.PCBInfoMoveFlag[(int)startPos] = endPos;
                FuncInline.PCBInfoMoveFlag[(int)endPos] = startPos;

                string codes = "";
                for (int i = 0; i < FuncInline.PCBInfo[(int)startPos].Barcode.Length; i++)
                {
                    if (FuncInline.PCBInfo[(int)startPos].Barcode[i] != null &&
                        FuncInline.PCBInfo[(int)startPos].Barcode[i].Length > 0)
                    {
                        codes += (codes.Length > 0 ? "," : "") + ((i + 1).ToString()) + ":" + FuncInline.PCBInfo[(int)startPos].Barcode[i];
                    }
                }
                FuncLog.WriteLog_Tester("PCB Move " + startPos.ToString() + " ==> " + endPos.ToString() + " : " + codes);

                FuncInline.PCBInfo[(int)endPos] = FuncInline.PCBInfo[(int)startPos];
                ClearPCBInfo(startPos);
                return;


                if (GlobalVar.Simulation)
                {
                    // 시뮬레이션시 언로딩 센서 강제감지
                    //if (startPos == enumTeachingPos.OutputJig &&
                    //    endPos == enumTeachingPos.LoadingConveyor)
                    //{
                    //    DIO.WriteDIData(enumDINames.X03_0_Loading_Conveyor_Start_Sensor, true);
                    //}
                }

                FuncInline.PCBInfo[(int)endPos].TestPass = FuncInline.PCBInfo[(int)startPos].TestPass;
                //FuncInline.PCBInfo[(int)endPos].PCBStatus = FuncInline.PCBInfo[(int)startPos].PCBStatus;
                if (FuncInline.PCBInfo[(int)endPos].Barcode == null)
                {
                    FuncInline.PCBInfo[(int)endPos].Barcode = new string[FuncInline.MaxArrayCount];
                }
                for (int i = 0; i < Math.Min(FuncInline.PCBInfo[(int)startPos].Barcode.Length, FuncInline.MaxArrayCount); i++)
                {
                    //debug("barcode " + i + " : " + FuncInline.PCBInfo[(int)startPos].Barcode[i]);
                    FuncInline.PCBInfo[(int)endPos].Barcode[i] = FuncInline.PCBInfo[(int)startPos].Barcode[i];
                }
                for (int i = 0; i < Math.Min(FuncInline.PCBInfo[(int)startPos].Xout.Length, FuncInline.MaxArrayCount); i++)
                {
                    //debug("barcode " + i + " : " + FuncInline.PCBInfo[(int)startPos].Barcode[i]);
                    FuncInline.PCBInfo[(int)endPos].Xout[i] = FuncInline.PCBInfo[(int)startPos].Xout[i];
                }
                for (int i = 0; i < Math.Min(FuncInline.PCBInfo[(int)startPos].BadMark.Length, FuncInline.MaxArrayCount); i++)
                {
                    //debug("barcode " + i + " : " + FuncInline.PCBInfo[(int)startPos].Barcode[i]);
                    FuncInline.PCBInfo[(int)endPos].BadMark[i] = FuncInline.PCBInfo[(int)startPos].BadMark[i];
                }
                if (FuncInline.PCBInfo[(int)endPos].SMDStatus == null)
                {
                    FuncInline.PCBInfo[(int)endPos].SMDStatus = new enumSMDStatus[FuncInline.MaxArrayCount];
                }
                for (int i = 0; i < Math.Min(FuncInline.PCBInfo[(int)startPos].SMDStatus.Length, FuncInline.MaxArrayCount); i++)
                {
                    if (endPos >= enumTeachingPos.Site1_F_DT1 &&
                        endPos <= enumTeachingPos.Site26_R_FT3 &&
                        FuncInline.PCBInfo[(int)startPos].SMDStatus[i] != FuncInline.enumSMDStatus.No_Test &&
                        FuncInline.PCBInfo[(int)startPos].SMDStatus[i] != FuncInline.enumSMDStatus.Test_Pass &&
                        !FuncInline.PCBInfo[(int)startPos].Xout[i] &&
                        !FuncInline.PCBInfo[(int)startPos].BadMark[i])
                    {
                        FuncInline.PCBInfo[(int)endPos].SMDReady[i] = false;
                        FuncInline.PCBInfo[(int)endPos].SMDReadySent[i] = false;
                        FuncInline.PCBInfo[(int)endPos].SMDStatus[i] = FuncInline.enumSMDStatus.Before_Command;
                    }
                    else
                    {
                        //debug("SMDStatus " + i + " : " + FuncInline.PCBInfo[(int)startPos].SMDStatus[i]);
                        FuncInline.PCBInfo[(int)endPos].SMDStatus[i] = FuncInline.PCBInfo[(int)startPos].SMDStatus[i];
                    }
                }
                if (FuncInline.PCBInfo[(int)endPos].ErrorCode == null)
                {
                    FuncInline.PCBInfo[(int)endPos].ErrorCode = new int[FuncInline.MaxArrayCount];
                }
                FuncInline.PCBInfo[(int)endPos].NgType = FuncInline.PCBInfo[(int)startPos].NgType;
                for (int i = 0; i < Math.Min(FuncInline.PCBInfo[(int)startPos].ErrorCode.Length, FuncInline.MaxArrayCount); i++)
                {
                    //debug("SMDStatus " + i + " : " + FuncInline.PCBInfo[(int)startPos].SMDStatus[i]);
                    FuncInline.PCBInfo[(int)endPos].ErrorCode[i] = FuncInline.PCBInfo[(int)startPos].ErrorCode[i];
                }
                if (FuncInline.PCBInfo[(int)endPos].BeforeCode == null)
                {
                    FuncInline.PCBInfo[(int)endPos].BeforeCode = new int[FuncInline.MaxArrayCount];
                }
                for (int i = 0; i < Math.Min(FuncInline.PCBInfo[(int)startPos].BeforeCode.Length, FuncInline.MaxArrayCount); i++)
                {
                    //debug("SMDStatus " + i + " : " + FuncInline.PCBInfo[(int)startPos].SMDStatus[i]);
                    FuncInline.PCBInfo[(int)endPos].BeforeCode[i] = FuncInline.PCBInfo[(int)startPos].BeforeCode[i];
                }
                FuncInline.PCBInfo[(int)endPos].TempCheck = FuncInline.PCBInfo[(int)startPos].TempCheck;
                if (FuncInline.PCBInfo[(int)endPos].Xout == null)
                {
                    FuncInline.PCBInfo[(int)endPos].Xout = new bool[FuncInline.MaxArrayCount];
                }
                for (int i = 0; i < FuncInline.PCBInfo[(int)startPos].Xout.Length; i++)
                {
                    //debug("Xout " + i + " : " + FuncInline.PCBInfo[(int)startPos].Xout[i]);
                    FuncInline.PCBInfo[(int)endPos].Xout[i] = FuncInline.PCBInfo[(int)startPos].Xout[i];
                }
                FuncInline.PCBInfo[(int)endPos].Destination = FuncInline.PCBInfo[(int)startPos].Destination;

                //if (FuncInline.PCBInfo[(int)endPos].StopWatch == null)
                //{
                //    FuncInline.PCBInfo[(int)endPos].StopWatch = new System.Diagnostics.Stopwatch();
                //}
                #region 앞 위치는 타이머 리셋, 뒤 위치는 타이머 정지
                if (FuncInline.PCBInfo[(int)startPos].StopWatch == null)
                {
                    FuncInline.PCBInfo[(int)startPos].StopWatch = new Stopwatch();
                }
                FuncInline.PCBInfo[(int)startPos].StopWatch.Stop();
                FuncInline.PCBInfo[(int)endPos].StopWatch = FuncInline.PCBInfo[(int)startPos].StopWatch;
                FuncInline.PCBInfo[(int)endPos].TestTime = FuncInline.PCBInfo[(int)startPos].TestTime;
                //FuncInline.PCBInfo[(int)startPos].StopWatch.Reset();
                #endregion
                //if (FuncInline.PCBInfo[(int)endPos].TestWatch == null)
                //{
                //    FuncInline.PCBInfo[(int)endPos].TestWatch = new System.Diagnostics.Stopwatch[FuncInline.MaxArrayCount];
                //    for (int i = 0; i < FuncInline.PCBInfo[(int)endPos].TestWatch.Length; i++)
                //    {
                //        if (FuncInline.PCBInfo[(int)endPos].TestWatch[i] == null)
                //        {
                //            FuncInline.PCBInfo[(int)endPos].TestWatch[i] = new System.Diagnostics.Stopwatch();
                //        }
                //        if (FuncInline.PCBInfo[(int)endPos].TestWatch[i].IsRunning)
                //        {
                //            FuncInline.PCBInfo[(int)endPos].TestWatch[i].Stop();
                //        }
                //    }
                //}
                //FuncInline.PCBInfo[(int)endPos].TestWatch = FuncInline.PCBInfo[(int)startPos].TestWatch;
                for (int i = 0; i < FuncInline.PCBInfo[(int)startPos].TestWatch.Length; i++)
                {
                    if (FuncInline.PCBInfo[(int)startPos].TestWatch[i] == null)
                    {
                        FuncInline.PCBInfo[(int)startPos].TestWatch[i] = new Stopwatch();
                    }
                    FuncInline.PCBInfo[(int)startPos].TestWatch[i].Stop();
                    FuncInline.PCBInfo[(int)endPos].TestWatch[i] = FuncInline.PCBInfo[(int)startPos].TestWatch[i];
                    FuncInline.PCBInfo[(int)startPos].TestWatch[i].Reset();
                }
                FuncInline.PCBInfo[(int)endPos].CommandRetry = FuncInline.PCBInfo[(int)startPos].CommandRetry;

                FuncInline.PCBInfo[(int)endPos].SelfRetestCount = FuncInline.PCBInfo[(int)startPos].SelfRetestCount;
                FuncInline.PCBInfo[(int)endPos].OtherRetestCount = FuncInline.PCBInfo[(int)startPos].OtherRetestCount;
                FuncInline.PCBInfo[(int)endPos].UserCancel = FuncInline.PCBInfo[(int)startPos].UserCancel;
                //FuncInline.PCBInfo[(int)endPos].TestSite = FuncInline.PCBInfo[(int)startPos].TestSite;
                if (FuncInline.PCBInfo[(int)endPos].TestSite == null)
                {
                    FuncInline.PCBInfo[(int)endPos].TestSite = new int[10];
                }
                if (FuncInline.PCBInfo[(int)startPos].TestSite != null)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        FuncInline.PCBInfo[(int)endPos].TestSite[i] = FuncInline.PCBInfo[(int)startPos].TestSite[i];
                    }
                }
                FuncInline.PCBInfo[(int)endPos].BuyerChange = FuncInline.PCBInfo[(int)startPos].BuyerChange;
                //FuncInline.PCBInfo[(int)endPos].SMDReady = !FuncInline.SMDReady;
                FuncInline.PCBInfo[(int)endPos].SMDReady = new bool[FuncInline.MaxArrayCount];
                if (!FuncInline.UseSMDReady)
                {
                    for (int i = 0; i < FuncInline.PCBInfo[(int)endPos].SMDReady.Length; i++)
                    {
                        FuncInline.PCBInfo[(int)endPos].SMDReady[i] = true;
                    }
                }
                FuncInline.PCBInfo[(int)endPos].SMDReadySent = new bool[FuncInline.MaxArrayCount];

                if (endPos >= enumTeachingPos.Site1_F_DT1 &&
                    endPos <= enumTeachingPos.Site26_R_FT3)
                {
                    FuncInline.PCBInfo[(int)endPos].OtherReTest = false;
                    FuncInline.PCBInfo[(int)endPos].SelfReTest = false;
                }
                FuncInline.PCBInfo[(int)endPos].Destination = FuncInline.PCBInfo[(int)startPos].Destination;
                /*
                enumMoveAction action = enumMoveAction.Waiting;
                if (FuncInline.PCBInfo[(int)endPos].MoveActionQueue == null)
                {
                    FuncInline.PCBInfo[(int)endPos].MoveActionQueue = new ConcurrentQueue<enumMoveAction>();
                }
                // 왜 end에서 빼서 start로 넣지?
                while (FuncInline.PCBInfo[(int)startPos].MoveActionQueue != null &&
                    FuncInline.PCBInfo[(int)startPos].MoveActionQueue.Count > 0)
                {
                    FuncInline.PCBInfo[(int)startPos].MoveActionQueue.TryDequeue(out action);
                    FuncInline.PCBInfo[(int)endPos].MoveActionQueue.Enqueue(action);
                }
                //*/

                #region 목적지가 사이트,Output,NG인 경우 목적지 삭제
                if (endPos >= enumTeachingPos.Site1_F_DT1 &&
                    endPos <= enumTeachingPos.Site26_R_FT3)
                {
                    FuncInline.PCBInfo[(int)endPos].Destination = enumTeachingPos.None;
                }
                if (endPos == enumTeachingPos.OutConveyor ||
                    endPos == enumTeachingPos.NgBuffer)
                {
                    FuncInline.PCBInfo[(int)endPos].Destination = enumTeachingPos.None;
                }
                #endregion
                FuncInline.PCBInfo[(int)endPos].SiteInputDate = FuncInline.PCBInfo[(int)startPos].SiteInputDate;
                FuncInline.PCBInfo[(int)endPos].SiteInputTime = FuncInline.PCBInfo[(int)startPos].SiteInputTime;
                FuncInline.PCBInfo[(int)endPos].PCBStatus = FuncInline.PCBInfo[(int)startPos].PCBStatus;
                //FuncInline.PCBInfo[(int)endPos].CycleStartTime = FuncInline.PCBInfo[(int)startPos].CycleStartTime;

                //FuncInline.PCBInfo[(int)endPos] = FuncInline.PCBInfo[(int)startPos];
                ClearPCBInfo(startPos);
            }
            catch (Exception ex)
            {
                debug("MovePCBInfo : " + ex.ToString());
                debug(ex.StackTrace);
            }
        }

        // 배출 상황 상관없이 리사이트 대기중인 사이트가 있는가?
        public static bool CheckResiteNeed(int rack)
        {
            try
            {
                for (enumTeachingPos pos = enumTeachingPos.Site1_F_DT1; pos <= enumTeachingPos.Site26_R_FT3; pos++)
                {
                    if (rack == 0 &&
                        pos >= enumTeachingPos.Site14_R_DT1)
                    {
                        continue;
                    }
                    if (rack == 1 &&
                        pos <= enumTeachingPos.Site13_F_FT3)
                    {
                        continue;
                    }
                    if (FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.ReTest)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public static bool CheckSiteFull(bool reTest)
        {
            bool full = true; // 일단 full로 체크해 두고 하나라도 비어 있으면 false 시킴

            try
            {
                #region 일반 사이트
                if (!reTest)
                {
                    for (int i = (int)enumTeachingPos.Site1_F_DT1; i <= (int)enumTeachingPos.Site26_R_FT3; i++)
                    {
                        //if (!GlobalVar.AutoInline_ReTestOnly ||
                        //    !FuncInline.ReTestSite[i - (int)enumTeachingPos.Site1_F_DT1])
                        //{
                        if (FuncInline.UseSite[i - (int)enumTeachingPos.Site1_F_DT1] &&
                            FuncInline.PCBInfo[i].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
                            !DIO.GetDIData(enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + (i - (int)enumTeachingPos.Site1_F_DT1)))
                        {
                            return false;
                        }
                        //}
                    }
                }
                #endregion
                #region 재검사 사이트
                else
                {
                    for (int i = (int)enumTeachingPos.Site1_F_DT1; i <= (int)enumTeachingPos.Site26_R_FT3; i++)
                    {
                        if (FuncInline.ReTestSite[i - (int)enumTeachingPos.Site1_F_DT1])
                        {
                            if (FuncInline.UseSite[i - (int)enumTeachingPos.Site1_F_DT1] &&
                                FuncInline.PCBInfo[i].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
                                !DIO.GetDIData(enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + (i - (int)enumTeachingPos.Site1_F_DT1) ))
                            {
                                return false;
                            }
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                debug("CheckSiteFull : " + ex.ToString());
                debug(ex.StackTrace);
            }

            return full;
        }

        public static enumTeachingPos GetSiteNoToPos(int no) // 좌하부터 가로순으로 된 번호를 enumTeachingPos로
        {
            int col = (no - 1) % FuncInline.MaxTestPCCount;
            int row = (int)((no - 1) / FuncInline.MaxTestPCCount);

            return enumTeachingPos.Site1_F_DT1 + col * FuncInline.MaxTestPCCount + row;
        }

        public static int GetSitePosToNo(enumTeachingPos pos) // enumTeachingPos를 좌하부터 가로순으로 된 번호로
        {
            int no = pos - enumTeachingPos.Site1_F_DT1 + 1;
            int row = (int)((no - 1) % FuncInline.MaxTestPCCount);
            int col = (int)((no - 1) / FuncInline.MaxTestPCCount);

            return row * FuncInline.MaxTestPCCount + col + 1; // 1부터
        }

        public static bool CheckResitePosible(int siteIndex)
        {
            int lack = siteIndex / 20;

            for (int i = 0 + lack * 20; i < 20 + lack * 20; i++)
            {
                // 미사용은 패쓰
                if (!FuncInline.UseSite[i])
                {
                    continue;
                }

                // 해당사이트 투입된적 있으면 Not OK
                bool reEnter = false;
                for (int j = 0; j < FuncInline.PCBInfo[siteIndex].TestSite.Length; j++)
                {
                    if (FuncInline.PCBInfo[siteIndex].TestSite[j] == i + 1)
                    {
                        reEnter = true;
                        break;
                    }
                }
                if (reEnter)
                {
                    continue;
                }

                // 빈사이트 OK
                if (CheckPosEmpty(enumTeachingPos.Site1_F_DT1 + i) &&
                    CheckDestination(enumTeachingPos.Site1_F_DT1 + i) == enumTeachingPos.None)
                {
                    return true;
                }

                // 검사중 OK
                if (FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].PCBStatus == FuncInline.enumSMDStatus.Testing)
                {
                    return true;
                }
                // 배출예정 OK
                if (!FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].OtherReTest &&
                    (FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].PCBStatus == FuncInline.enumSMDStatus.Test_Pass ||
                            FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].PCBStatus == FuncInline.enumSMDStatus.Response_NG ||
                            FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].PCBStatus == FuncInline.enumSMDStatus.Test_Fail ||
                            FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].PCBStatus == FuncInline.enumSMDStatus.Test_Cancel ||
                            FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].PCBStatus == FuncInline.enumSMDStatus.User_Cancel ||
                            FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].PCBStatus == FuncInline.enumSMDStatus.Test_Timeout))
                {
                    return true;
                }
            }

            return false;
        }

        public static int GetEmptySiteCount(int siteIndex)
        {
            int count = 0;
            int nonBlueCount = 0;


            try
            {
                int pcNum = siteIndex / FuncInline.MaxSiteCount; // PC당 사이트수로 나눈 몫
                for (int i = pcNum * FuncInline.MaxSiteCount; i < pcNum * FuncInline.MaxSiteCount + FuncInline.MaxSiteCount; i++)
                {
                    if (FuncInline.UseSite[i] &&
                        !DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + i) &&
                        FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].PCBStatus == FuncInline.enumSMDStatus.UnKnown) // 빈 사이트 카운트 증가
                    {
                        count++;
                    }
                    // Blue가 아니더라도 리사이트할 예정이 없는 PCB는 nonBlue 카운트를 증가시키지 않는다.
                    if (DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + i * GlobalVar.DIModuleGap) &&
                        (FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].PCBStatus != FuncInline.enumSMDStatus.UnKnown && !FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].BuyerChange) &&
                        FuncInline.OtherRetest &&
                        FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].OtherRetestCount < FuncInline.OtherRetestCount)
                    {
                        nonBlueCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }

            /* blue 모드에서도 무조건 한 사이트를 남기기 위해 막는다.
            if (count == 1 &&
                nonBlueCount == 0)
            {
                return 2; // 기존보드 다 빠져나간 후에는 꽉 채우기 위해
            }
            //*/

            return count;
        }


        public static enumTeachingPos CheckEmptySite(int rack, bool destCheck) // 렉별로 빈 사이트 찾기
        {
            // BuyerChange.Blue 경우 좌우 각각 한 사이트만 남았으면 배제해야 한다.
            // 순서를 열별로가 아닌 행순서로 해야 하지 않을까? 열별로 한 사이트 남겨야 한다.
            try
            {
                #region 투입중인 보드가 있으면 대기
                #region Front
                if (rack != 1)
                {
                    if (FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].PCBStatus == FuncInline.enumSMDStatus.Before_Command)
                    {
                        return enumTeachingPos.None;
                    }
                }
                #endregion
                #region Rear
                if (rack != 0)
                {
                    if ( FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Up].PCBStatus == FuncInline.enumSMDStatus.Before_Command)
                    {
                        return enumTeachingPos.None;
                    }
                }
                #endregion
                #endregion

                // SiteOrder 를 쓰기 때문에 전체 범위 SiteOrder 순대로 돌다가 해당렉에 해당하는 경우 통과

                // BuyerChange.Blue에 준하는 경우
                // Oragne 모드, 렉별 AutoInline_BuyerChangeOrange 이 true인 경우

                bool leftOrange = false; // 좌측 지정사이트 비어 있는가?
                bool rightOrange = false; // 우측 지정사이트 비어 있는가?
                #region 지정 렉이 Orange모드면 지정사이트가 비어 있는가만 판단
                //*
                //if (rack != 1 &&
                //    (FuncInline.BuyerChange != enumBuyerChange.White ||
                //            FuncInline.BuyerChangeOrange[0]))
                //{
                //    if (FuncInline.UseSite[(int)FuncInline.BuyerChangeSite[0] - (int)enumTeachingPos.Site1_F_DT1] &&
                //        FuncInline.PCBInfo[(int)FuncInline.BuyerChangeSite[0]].PCBStatus == FuncInline.enumSMDStatus.UnKnown)
                //    {
                //        leftOrange = true;
                //    }
                //}
                //if (rack != 0 &&
                //    (FuncInline.BuyerChange != enumBuyerChange.White ||
                //            FuncInline.BuyerChangeOrange[1]))
                //{
                //    if (FuncInline.UseSite[(int)FuncInline.BuyerChangeSite[1] - (int)enumTeachingPos.Site1_F_DT1] &&
                //        FuncInline.PCBInfo[(int)FuncInline.BuyerChangeSite[1]].PCBStatus == FuncInline.enumSMDStatus.UnKnown)
                //    {
                //        rightOrange = true;
                //    }
                //}
                //*/
                #endregion

                #region 이동중인 PCB 있으면 통과. 상황별로 별도 판단해야 한다.
                /*
                if (destCheck &&
                    CheckMovingPCB())
                {
                    return enumTeachingPos.None;
                }
                //*/

                // destination으로 체크하면 빈 틈이 생긴다.
                // PCB 정보에서 사이트 투입중인 것 체크해야 한다.
                bool leftMoving = false; // 좌측 투입 동작에 걸리는 동선이 있는가?
                bool rightMoving = false; // 우측 투입 동작에 걸리는 동선이 있는가?
                if (destCheck)
                {
                    /*
                    // PC별 투입금지 경우
                    if (GlobalVar.InputStop[0])
                    {
                        leftMoving = true;
                    }
                    if (GlobalVar.InputStop[1])
                    {
                        rightMoving = true;
                    }
                    //*/
                    #region 바이어체인지 빈사이트 하나 이하일 때
                    if ((FuncInline.BuyerChange != enumBuyerChange.White ||
                                FuncInline.BuyerChangeOrange[0]) &&
                        GetEmptySiteCount(0) <= 1)
                    {
                        leftMoving = true;
                    }
                    if ((FuncInline.BuyerChange != enumBuyerChange.White ||
                                FuncInline.BuyerChangeOrange[1]) &&
                        GetEmptySiteCount(20) <= 1)
                    {
                        rightMoving = true;
                    }
                    #endregion

                    // 좌측 동선 체크. 좌측사이트 배출 또는 Pass1/Lift1Up 동선 있을 때
                    if (FuncInline.PCBInfo[(int)enumTeachingPos.FrontPassLine].Destination != enumTeachingPos.None ||
                        FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination != enumTeachingPos.None)
                    {
                        leftMoving = true;
                    }
                    if (FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].PCBStatus == FuncInline.enumSMDStatus.Before_Command)
                    {
                        leftMoving = true;
                    }
                    // 목적지 정해진 사이트 있을 때
                    if (!leftMoving)
                    {
                        for (enumTeachingPos site = enumTeachingPos.Site1_F_DT1; site <= enumTeachingPos.Site13_F_FT3; site++)
                        {
                            if (FuncInline.PCBInfo[(int)site].Destination != enumTeachingPos.None)
                            {
                                leftMoving = true;
                                break;
                            }
                        }
                    }
                    #region 리사이트 있으면 동선에 걸린다 판단
                    if (destCheck &&
                        !leftMoving) // &&
                                     //(FuncInline.BuyerChange == FuncInline.enumBuyerChange.Blue))
                    {
                        if (FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].OtherReTest ||
                            FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                            GetEmptySiteCount(0) <= (FuncInline.LeaveOneSite || FuncInline.BuyerChange == FuncInline.enumBuyerChange.Blue || FuncInline.BuyerChange == FuncInline.enumBuyerChange.Orange ? 1 : 0))
                        {
                            leftMoving = true;
                        }
                        //*
                        for (int i = 0; i < 20; i++)
                        {
                            if (FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].PCBStatus > FuncInline.enumSMDStatus.Before_Command &&
                                FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].OtherReTest &&
                                GetEmptySiteCount(0) <= 2)
                            {
                                leftMoving = true;
                                break;
                            }
                        }
                        //*/
                    }
                    #endregion
                    // 바이어 체인지 아니더라도 한 사이트 비워두기 옵션경우 체크
                    if (FuncInline.LeaveOneSite &&
                        destCheck &&
                        !leftMoving)
                    {
                        if (GetEmptySiteCount(0) <= 1 ||
                                FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination != enumTeachingPos.None ||
                                FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].OtherReTest ||
                                FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
                        {
                            leftMoving = true;
                        }
                        //*
                        for (int i = 0; i < 20; i++)
                        {
                            if (FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].PCBStatus > FuncInline.enumSMDStatus.Before_Command &&
                                FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].OtherReTest &&
                                GetEmptySiteCount(0) <= 2)
                            {
                                leftMoving = true;
                                break;
                            }
                        }
                        //*/
                    }


                    // 우측 동선 체크. 전사이트 배출 또는 Pass1/Lift1Up/Pass2/Lift2Up 동선 있을 떄
                    if (FuncInline.PCBInfo[(int)enumTeachingPos.FrontPassLine].Destination != enumTeachingPos.None ||
                        FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination != enumTeachingPos.None ||
                        FuncInline.PCBInfo[(int)enumTeachingPos.RearPassLine].Destination != enumTeachingPos.None ||
                        FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Up].Destination != enumTeachingPos.None)
                    {
                        rightMoving = true;
                    }
                    if (FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].PCBStatus == FuncInline.enumSMDStatus.Before_Command ||
                        FuncInline.PCBInfo[(int)enumTeachingPos.RearPassLine].PCBStatus == FuncInline.enumSMDStatus.Before_Command ||
                        FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Up].PCBStatus == FuncInline.enumSMDStatus.Before_Command)
                    {
                        rightMoving = true;
                    }
                    if (RearPassLineAction != FuncInline.enumLiftAction.Waiting ||
                        Lift2Action != FuncInline.enumLiftAction.Waiting)
                    {
                        rightMoving = true;
                    }
                    if (!rightMoving)
                    {
                        for (enumTeachingPos site = enumTeachingPos.Site1_F_DT1; site <= enumTeachingPos.Site26_R_FT3; site++)
                        {
                            if (FuncInline.PCBInfo[(int)site].Destination != enumTeachingPos.None &&
                                GetEmptySiteCount(0) > 0)
                            {
                                rightMoving = true;
                                break;
                            }
                        }
                    }
                    #region 리사이트 있으면 두칸이라도 동선에 걸린다 판단
                    if (destCheck &&
                        !rightMoving) // &&
                                      //(FuncInline.BuyerChange == FuncInline.enumBuyerChange.Blue))
                    {
                        if (FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Up].OtherReTest ||
                            FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                            GetEmptySiteCount(20) <= (FuncInline.LeaveOneSite || FuncInline.BuyerChange == FuncInline.enumBuyerChange.Blue || FuncInline.BuyerChange == FuncInline.enumBuyerChange.Orange ? 1 : 0))
                        {
                            rightMoving = true;
                        }
                        //*
                        for (int i = 20; i < 40; i++)
                        {
                            if (FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].PCBStatus > FuncInline.enumSMDStatus.Before_Command &&
                                FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].OtherReTest &&
                                GetEmptySiteCount(20) <= 2)
                            {
                                rightMoving = true;
                                break;
                            }
                        }
                        //*/
                    }
                    #endregion
                    // 바이어 체인지 아니더라도 한 사이트 비워두기 옵션경우 체크
                    if (FuncInline.LeaveOneSite &&
                        destCheck &&
                        !rightMoving)
                    {
                        if (GetEmptySiteCount(20) <= 1 ||
                            FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Up].Destination != enumTeachingPos.None ||
                            FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Up].OtherReTest ||
                            FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
                        {
                            rightMoving = true;
                        }
                        //*
                        for (int i = 20; i < 40; i++)
                        {
                            if (FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].PCBStatus > FuncInline.enumSMDStatus.Before_Command &&
                                FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].OtherReTest &&
                                GetEmptySiteCount(20) <= 2)
                            {
                                rightMoving = true;
                                break;
                            }
                        }
                        //*/
                    }
                }
                #endregion

                #region 지정사이트 우선 판단
                if (rack != 1 &&
                    leftOrange &&
                    !leftMoving)
                {
                  
                    return FuncInline.BuyerChangeSite[0];
                }
                if (rack != 0 &&
                    rightOrange &&
                    !rightMoving)
                {
                    return FuncInline.BuyerChangeSite[1];
                }
                #endregion

                #region 이동중인 다음 사이트가 같은 렉이고 연속이고 비어 있다면 빈 사이트 없다고 판단
                /*
                if (GlobalVar.SearchInputIndexCurrent > -1)
                {
                    enumTeachingPos currentSite = FuncInline.SiteOrder[GlobalVar.SearchInputIndexCurrent];
                    int nextIndex = GlobalVar.SearchInputIndexCurrent + 1;
                    if (nextIndex >= FuncInline.SiteOrder.Length)
                    {
                        nextIndex = 0;
                    }
                    enumTeachingPos nextSite = FuncInline.SiteOrder[nextIndex];
                    bool currentLeft = currentSite <= enumTeachingPos.Site13_F_FT3;
                    bool nextLeft = nextSite <= enumTeachingPos.Site13_F_FT3;
                    debug("current site : " + currentSite);
                    debug("next site : " + nextSite);
                    debug("currentLeft : " + currentLeft);
                    debug("nextLeft : " + nextLeft);
                    debug("FuncInline.PCBInfo[(int)nextSite].PCBStatus : " + FuncInline.PCBInfo[(int)nextSite].PCBStatus);
                    debug("FuncInline.PCBInfo[(int)currentSite].PCBStatus : " + FuncInline.PCBInfo[(int)currentSite].PCBStatus);
                    debug("FuncInline.PCBInfo[(int)enumTeachingPos.FrontPassline].Destination : " + FuncInline.PCBInfo[(int)enumTeachingPos.FrontPassline].Destination);
                    debug("FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination : " + FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination);
                    debug("FuncInline.PCBInfo[(int)enumTeachingPos.FrontPassline].Destination : " + FuncInline.PCBInfo[(int)enumTeachingPos.FrontPassline].Destination);
                    debug("FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Up].Destination : " + FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Up].Destination);
                    if (GlobalVar.SearchInputIndexCurrent != nextIndex &&
                        (currentLeft && nextLeft ||
                                !currentLeft && !nextLeft))
                    {
                        if (FuncInline.PCBInfo[(int)nextSite].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
                            FuncInline.PCBInfo[(int)currentSite].PCBStatus == FuncInline.enumSMDStatus.UnKnown) // && // 현재 이동중
                        //(FuncInline.PCBInfo[(int)enumTeachingPos.FrontPassline].Destination == currentSite ||
                        //        FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination == currentSite ||
                        //        FuncInline.PCBInfo[(int)enumTeachingPos.FrontPassline].Destination == currentSite ||
                        //        FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Up].Destination == currentSite)) 
                        {
                            return enumTeachingPos.None;
                        }
                    }
                }
                //*/
                #endregion

                #region 최소/최대 투입 카운트를 구한다.
                /*
                int minInputCount = 99999999;
                int maxInputCount = 0;
                for (int i = 0; i < FuncInline.SiteInputCount.Length; i++)
                {
                    if (FuncInline.SiteInputCount[i] < minInputCount)
                    {
                        minInputCount = FuncInline.SiteInputCount[i];
                    }
                    if (FuncInline.SiteInputCount[i] > maxInputCount)
                    {
                        maxInputCount = FuncInline.SiteInputCount[i];
                    }
                }
                //debug("최소 투입 카운트 : " + minInputCount);
                //debug("최대 투입 카운트 : " + maxInputCount);
                //*/
                #endregion

                /*
                같은 카운트 내 루프 순서
                LeftInputIndex와 RightInputIndex 사용
                먼저 검색할 쪽 순서 돌면서 검색 후
                없으면 반대쪽 검색
                //*/

                #region 최소 투입카운트부터 1씩 증가시키면서 투입순서대로 빈 사이트 검색
                //for (int cnt = minInputCount; cnt <= maxInputCount; cnt++)
                //{
                bool leftFirst = FuncInline.SiteOrder[SearchInputIndex] <= enumTeachingPos.Site13_F_FT3;
                int firstIndex = leftFirst ? LeftInputIndex : RightInputIndex;
                int secondIndex = !leftFirst ? LeftInputIndex : RightInputIndex;

                #region 우선 검색 랙 검색
                /*
                if (leftFirst &&
                    GlobalVar.InputStop[0])
                {
                    // 좌측렉 투입 중지
                }
                else if (!leftFirst &&
                    GlobalVar.InputStop[1])
                {
                    // 우측렉 투입 중지
                }
                else
                {
                //*/
                #region 검색 순번부터 끝까지 검색. 목적지가 등록된 사이트는 배제해야 한다.
                for (int i = firstIndex; i < FuncInline.SiteOrder.Length; i++)
                {
                    // 지정렉 확인
                    enumTeachingPos sitePos = FuncInline.SiteOrder[i];
                    if (rack >= 0 &&
                        (sitePos < enumTeachingPos.Site1_F_DT1 + rack * FuncInline.MaxSiteCount ||
                                sitePos > enumTeachingPos.Site13_F_FT3 + rack * FuncInline.MaxSiteCount))
                    {
                        continue;
                    }
                    if (leftFirst &&
                        (sitePos < enumTeachingPos.Site1_F_DT1 ||
                                sitePos > enumTeachingPos.Site13_F_FT3))
                    {
                        continue;
                    }
                    if (!leftFirst &&
                        (sitePos < enumTeachingPos.Site14_R_DT1 ||
                                sitePos > enumTeachingPos.Site26_R_FT3))
                    {
                        continue;
                    }

                    int siteIndex = (int)sitePos - (int)enumTeachingPos.Site1_F_DT1;

                    // 좌측 사이트 동선체크에 걸리면 통과
                    if (leftMoving &&
                        siteIndex < FuncInline.MaxSiteCount)
                    {
                        continue;
                    }
                    // 우측 사이트 동선체크에 걸리면 통과. ==> 투입대상인데 동선에 걸리면 대기
                    /*
                    if (rightMoving &&
                        siteIndex >= FuncInline.MaxSiteCount)
                    {
                        continue;
                    }
                    //*/

                    // 투입카운트 확인
                    /*
                    if (FuncInline.SiteInputCount[siteIndex] != cnt)
                    {
                        continue;
                    }
                    //*/

                    if (FuncInline.UseSite[siteIndex] && // 사이트 사용체크되고
                        !DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + siteIndex * GlobalVar.DIModuleGap) && // PCB 확인 안 되고
                        FuncInline.PCBInfo[(int)sitePos].PCBStatus == FuncInline.enumSMDStatus.UnKnown && // PCB 상태 없고
                        FuncInline.SiteAction[siteIndex] == FuncInline.enumSiteAction.Waiting) // 사이트 동작 없으면
                    {
                        // 우선사이트 고려 경우와 아닌 경우로 구분해서 판단해야 한다.
                        // 우선사이트 비고려시 : White모드 해당 렉 Orange 모드 아닌 경우
                        //                      동선 비고려시 한 자리 이상, 동선 고려시 두 자리 이상 빈 자리
                        // 우선사이트 고려시 : White 모드 하니거나 렉 Orange 모드인 경우
                        //            
                        #region 일반 white 모드          
                        if (//FuncInline.BuyerChange != enumBuyerChange.Blue &&
                            GetEmptySiteCount(siteIndex) > 0 &&
                            CheckDestination(sitePos) == enumTeachingPos.None)
                        {
                            if (siteIndex < 20 &&
                                !FuncInline.BuyerChangeOrange[0] &&
                                FuncInline.BuyerChange == FuncInline.enumBuyerChange.White)
                            {
                                return enumTeachingPos.Site1_F_DT1 + siteIndex;
                            }
                            if (siteIndex >= 20 &&
                                !FuncInline.BuyerChangeOrange[1] &&
                                FuncInline.BuyerChange == FuncInline.enumBuyerChange.White)
                            {
                                if (rightMoving)
                                {
                                    return enumTeachingPos.None;
                                }
                                return enumTeachingPos.Site1_F_DT1 + siteIndex;
                            }
                        }
                        #endregion
                        #region 일반 white 모드 이외 경우. 좌측 빈 사이트를 리턴하는 현상 발생
                        if (//(FuncInline.BuyerChange == FuncInline.enumBuyerChange.Blue) &&
                            ((destCheck && GetEmptySiteCount(siteIndex) > 1) || (!destCheck && GetEmptySiteCount(siteIndex) > 0)) &&
                            CheckDestination(sitePos) == enumTeachingPos.None)
                        {
                            if (siteIndex < 20 &&
                                (FuncInline.BuyerChangeOrange[0] ||
                                        FuncInline.BuyerChange != enumBuyerChange.White))
                            {
                                return enumTeachingPos.Site1_F_DT1 + siteIndex;
                            }
                            if (siteIndex >= 20 &&
                                (FuncInline.BuyerChangeOrange[1] ||
                                        FuncInline.BuyerChange != enumBuyerChange.White))
                            {
                                if (rightMoving)
                                {
                                    return enumTeachingPos.None;
                                }
                                return enumTeachingPos.Site1_F_DT1 + siteIndex;
                            }
                        }
                        #endregion
                    }
                }
                #endregion

                #region 처음부터 검색 순서까지 검색
                for (int i = 0; i < firstIndex; i++)
                {
                    enumTeachingPos sitePos = FuncInline.SiteOrder[i];
                    // 지정 렉 확인
                    if (rack >= 0 &&
                        (sitePos < enumTeachingPos.Site1_F_DT1 + rack * FuncInline.MaxSiteCount ||
                                sitePos > enumTeachingPos.Site13_F_FT3 + rack * FuncInline.MaxSiteCount))
                    {
                        continue;
                    }
                    if (leftFirst &&
                        (sitePos < enumTeachingPos.Site1_F_DT1 ||
                                sitePos > enumTeachingPos.Site13_F_FT3))
                    {
                        continue;
                    }
                    if (!leftFirst &&
                        (sitePos < enumTeachingPos.Site14_R_DT1 ||
                                sitePos > enumTeachingPos.Site26_R_FT3))
                    {
                        continue;
                    }

                    int siteIndex = (int)sitePos - (int)enumTeachingPos.Site1_F_DT1;

                    // 투입카운트 확인
                    /*
                    if (FuncInline.SiteInputCount[siteIndex] != cnt)
                    {
                        continue;
                    }
                    //*/

                    // 좌측 사이트 동선체크에 걸리면 통과
                    if (leftMoving &&
                        siteIndex < FuncInline.MaxSiteCount)
                    {
                        continue;
                    }
                    // 우측 사이트 동선체크에 걸리면 통과
                    /*
                    if (rightMoving &&
                        siteIndex >= FuncInline.MaxSiteCount)
                    {
                        continue;
                    }
                    //*/
                    if (FuncInline.UseSite[siteIndex] &&
                        !DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + siteIndex * GlobalVar.DIModuleGap) &&
                        FuncInline.PCBInfo[(int)sitePos].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
                        FuncInline.SiteAction[siteIndex] == FuncInline.enumSiteAction.Waiting)
                    {
                        // 우선사이트 고려 경우와 아닌 경우로 구분해서 판단해야 한다.
                        // 우선사이트 비고려시 : White모드 해당 렉 Orange 모드 아닌 경우
                        //                      동선 비고려시 한 자리 이상, 동선 고려시 두 자리 이상 빈 자리
                        // 우선사이트 고려시 : White 모드 하니거나 렉 Orange 모드인 경우
                        //            
                        #region 일반 white 모드          
                        if (//FuncInline.BuyerChange != enumBuyerChange.Blue &&
                            GetEmptySiteCount(siteIndex) > 0 &&
                            CheckDestination(sitePos) == enumTeachingPos.None)
                        {
                            if (siteIndex < 20 &&
                                !FuncInline.BuyerChangeOrange[0] &&
                                FuncInline.BuyerChange == FuncInline.enumBuyerChange.White)
                            {
                                return enumTeachingPos.Site1_F_DT1 + siteIndex;
                            }
                            if (siteIndex >= 20 &&
                                !FuncInline.BuyerChangeOrange[1] &&
                                FuncInline.BuyerChange == FuncInline.enumBuyerChange.White)
                            {
                                if (rightMoving)
                                {
                                    return enumTeachingPos.None;
                                }
                                return enumTeachingPos.Site1_F_DT1 + siteIndex;
                            }
                        }
                        #endregion
                        #region 일반 white 모드 이외 경우
                        if (//(FuncInline.BuyerChange == FuncInline.enumBuyerChange.Blue) &&
                            ((destCheck && GetEmptySiteCount(siteIndex) > 1) || (!destCheck && GetEmptySiteCount(siteIndex) > 0)) &&
                            CheckDestination(sitePos) == enumTeachingPos.None)
                        {
                            if (siteIndex < 20 &&
                                (FuncInline.BuyerChangeOrange[0] ||
                                        FuncInline.BuyerChange != enumBuyerChange.White))
                            {
                                return enumTeachingPos.Site1_F_DT1 + siteIndex;
                            }
                            if (siteIndex >= 20 &&
                                (FuncInline.BuyerChangeOrange[1] ||
                                        FuncInline.BuyerChange != enumBuyerChange.White))
                            {
                                if (rightMoving)
                                {
                                    return enumTeachingPos.None;
                                }
                                return enumTeachingPos.Site1_F_DT1 + siteIndex;
                            }
                        }
                        #endregion
                    }
                }
                #endregion
                //}
                #endregion

                #region 다음 검색 랙 검색
                /*
                if (!leftFirst &&
                    GlobalVar.InputStop[0])
                {
                    // 좌측렉 투입 중지
                }
                else if (leftFirst &&
                    GlobalVar.InputStop[1])
                {
                    // 우측렉 투입 중지
                }
                else
                {
                //*/
                #region 검색 순번부터 끝까지 검색. 목적지가 등록된 사이트는 배제해야 한다.
                for (int i = secondIndex; i < FuncInline.SiteOrder.Length; i++)
                {
                    // 지정렉 확인
                    enumTeachingPos sitePos = FuncInline.SiteOrder[i];
                    if (rack >= 0 &&
                        (sitePos < enumTeachingPos.Site1_F_DT1 + rack * FuncInline.MaxSiteCount ||
                                sitePos > enumTeachingPos.Site13_F_FT3 + rack * FuncInline.MaxSiteCount))
                    {
                        continue;
                    }
                    if (leftFirst &&
                        (sitePos < enumTeachingPos.Site14_R_DT1 ||
                                sitePos > enumTeachingPos.Site26_R_FT3))
                    {
                        continue;
                    }
                    if (!leftFirst &&
                        (sitePos < enumTeachingPos.Site1_F_DT1 ||
                                sitePos > enumTeachingPos.Site13_F_FT3))
                    {
                        continue;
                    }
                    int siteIndex = (int)sitePos - (int)enumTeachingPos.Site1_F_DT1;

                    // 좌측 사이트 동선체크에 걸리면 통과
                    if (//leftFirst &&
                        leftMoving &&
                        siteIndex < FuncInline.MaxSiteCount)
                    {
                        continue;
                    }
                    // 우측 사이트 동선체크에 걸리면 통과
                    /*
                    if (//!leftFirst &&
                        rightMoving &&
                        siteIndex >= FuncInline.MaxSiteCount)
                    {
                        continue;
                    }
                    //*/

                    // 투입카운트 확인
                    /*
                    if (FuncInline.SiteInputCount[siteIndex] != cnt)
                    {
                        continue;
                    }
                    //*/

                    if (FuncInline.UseSite[siteIndex] && // 사이트 사용체크되고
                        !DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + siteIndex * GlobalVar.DIModuleGap) && // PCB 확인 안 되고
                        FuncInline.PCBInfo[(int)sitePos].PCBStatus == FuncInline.enumSMDStatus.UnKnown && // PCB 상태 없고
                        FuncInline.SiteAction[siteIndex] == FuncInline.enumSiteAction.Waiting) // 사이트 동작 없으면
                    {
                        // 우선사이트 고려 경우와 아닌 경우로 구분해서 판단해야 한다.
                        // 우선사이트 비고려시 : White모드 해당 렉 Orange 모드 아닌 경우
                        //                      동선 비고려시 한 자리 이상, 동선 고려시 두 자리 이상 빈 자리
                        // 우선사이트 고려시 : White 모드 하니거나 렉 Orange 모드인 경우
                        //            
                        #region 일반 white 모드          
                        if (//FuncInline.BuyerChange != enumBuyerChange.Blue &&
                            GetEmptySiteCount(siteIndex) > 0 &&
                            CheckDestination(sitePos) == enumTeachingPos.None)
                        {
                            if (siteIndex < 20 &&
                                !FuncInline.BuyerChangeOrange[0] &&
                                FuncInline.BuyerChange == FuncInline.enumBuyerChange.White)
                            {
                                return enumTeachingPos.Site1_F_DT1 + siteIndex;
                            }
                            if (siteIndex >= 20 &&
                                !FuncInline.BuyerChangeOrange[1] &&
                                FuncInline.BuyerChange == FuncInline.enumBuyerChange.White)
                            {
                                if (rightMoving)
                                {
                                    return enumTeachingPos.None;
                                }
                                return enumTeachingPos.Site1_F_DT1 + siteIndex;
                            }
                        }
                        #endregion
                        #region 일반 white 모드 이외 경우. 좌측 빈 사이트를 리턴하는 현상 발생
                        if (//(FuncInline.BuyerChange == FuncInline.enumBuyerChange.Blue) &&
                            ((destCheck && GetEmptySiteCount(siteIndex) > 1) || (!destCheck && GetEmptySiteCount(siteIndex) > 0)) &&
                            CheckDestination(sitePos) == enumTeachingPos.None)
                        {
                            if (siteIndex < 20 &&
                                (FuncInline.BuyerChangeOrange[0] ||
                                        FuncInline.BuyerChange != enumBuyerChange.White))
                            {
                                return enumTeachingPos.Site1_F_DT1 + siteIndex;
                            }
                            if (siteIndex >= 20 &&
                                (FuncInline.BuyerChangeOrange[1] ||
                                        FuncInline.BuyerChange != enumBuyerChange.White))
                            {
                                if (rightMoving)
                                {
                                    return enumTeachingPos.None;
                                }
                                return enumTeachingPos.Site1_F_DT1 + siteIndex;
                            }
                        }
                        #endregion
                    }
                }
                #endregion

                #region 처음부터 검색 순서까지 검색
                for (int i = 0; i < secondIndex; i++)
                {
                    enumTeachingPos sitePos = FuncInline.SiteOrder[i];
                    // 지정 렉 확인
                    if (rack >= 0 &&
                        (sitePos < enumTeachingPos.Site1_F_DT1 + rack * FuncInline.MaxSiteCount ||
                                sitePos > enumTeachingPos.Site13_F_FT3 + rack * FuncInline.MaxSiteCount))
                    {
                        continue;
                    }
                    if (leftFirst &&
                        (sitePos < enumTeachingPos.Site14_R_DT1 ||
                                sitePos > enumTeachingPos.Site26_R_FT3))
                    {
                        continue;
                    }
                    if (!leftFirst &&
                        (sitePos < enumTeachingPos.Site1_F_DT1 ||
                                sitePos > enumTeachingPos.Site13_F_FT3))
                    {
                        continue;
                    }

                    int siteIndex = (int)sitePos - (int)enumTeachingPos.Site1_F_DT1;

                    // 투입카운트 확인
                    /*
                    if (FuncInline.SiteInputCount[siteIndex] != cnt)
                    {
                        continue;
                    }
                    //*/

                    // 좌측 사이트 동선체크에 걸리면 통과
                    if (//leftFirst &&
                        leftMoving &&
                        siteIndex < FuncInline.MaxSiteCount)
                    {
                        continue;
                    }
                    /*
                    // 우측 사이트 동선체크에 걸리면 통과
                    if (//!leftFirst &&
                        rightMoving &&
                        siteIndex >= FuncInline.MaxSiteCount)
                    {
                        continue;
                    }
                    //*/
                    if (FuncInline.UseSite[siteIndex] &&
                        !DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + siteIndex * GlobalVar.DIModuleGap) &&
                        FuncInline.PCBInfo[(int)sitePos].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
                        FuncInline.SiteAction[siteIndex] == FuncInline.enumSiteAction.Waiting)
                    {
                        // 우선사이트 고려 경우와 아닌 경우로 구분해서 판단해야 한다.
                        // 우선사이트 비고려시 : White모드 해당 렉 Orange 모드 아닌 경우
                        //                      동선 비고려시 한 자리 이상, 동선 고려시 두 자리 이상 빈 자리
                        // 우선사이트 고려시 : White 모드 하니거나 렉 Orange 모드인 경우
                        //            
                        #region 일반 white 모드          
                        if (//FuncInline.BuyerChange != enumBuyerChange.Blue &&
                            GetEmptySiteCount(siteIndex) > 0 &&
                            CheckDestination(sitePos) == enumTeachingPos.None)
                        {
                            if (siteIndex < 20 &&
                                !FuncInline.BuyerChangeOrange[0] &&
                                FuncInline.BuyerChange == FuncInline.enumBuyerChange.White)
                            {
                                return enumTeachingPos.Site1_F_DT1 + siteIndex;
                            }
                            if (siteIndex >= 20 &&
                                !FuncInline.BuyerChangeOrange[1] &&
                                FuncInline.BuyerChange == FuncInline.enumBuyerChange.White)
                            {
                                if (rightMoving)
                                {
                                    return enumTeachingPos.None;
                                }
                                return enumTeachingPos.Site1_F_DT1 + siteIndex;
                            }
                        }
                        #endregion
                        #region 일반 white 모드 이외 경우
                        if (//(FuncInline.BuyerChange == FuncInline.enumBuyerChange.Blue) &&
                            ((destCheck && GetEmptySiteCount(siteIndex) > 1) || (!destCheck && GetEmptySiteCount(siteIndex) > 0)) &&
                            CheckDestination(sitePos) == enumTeachingPos.None)
                        {
                            if (siteIndex < 20 &&
                                (FuncInline.BuyerChangeOrange[0] ||
                                        FuncInline.BuyerChange != enumBuyerChange.White))
                            {
                                return enumTeachingPos.Site1_F_DT1 + siteIndex;
                            }
                            if (siteIndex >= 20 &&
                                (FuncInline.BuyerChangeOrange[1] ||
                                        FuncInline.BuyerChange != enumBuyerChange.White))
                            {
                                if (rightMoving)
                                {
                                    return enumTeachingPos.None;
                                }
                                return enumTeachingPos.Site1_F_DT1 + siteIndex;
                            }
                        }
                        #endregion
                    }
                }
                #endregion
                //}
                #endregion
                //}
                #endregion
            }
            catch (Exception ex)
            {
                debug("CheckEmptySite : " + ex.ToString());
                debug(ex.StackTrace);
            }
            return enumTeachingPos.None;
        }

        public static enumTeachingPos CheckEmptySite(int rack) // 동선은 상관 없이 목적지 지정 안 된 빈 사이트 찾기
        {
            // 순서를 열별로가 아닌 행순서로 해야 하지 않을까? 열별로 한 사이트 남겨야 한다.
            try
            {
                #region 지정 렉이 Orange모드면 지정사이트가 비어 있는가만 판단
                /*
                if (FuncInline.BuyerChangeOrange[rack])
                {
                    if (FuncInline.PCBInfo[(int)FuncInline.BuyerChangeSite[rack]].PCBStatus == FuncInline.enumSMDStatus.UnKnown)
                    {
                        return FuncInline.BuyerChangeSite[rack];
                    }
                    return enumTeachingPos.None;
                }
                //*/
                #endregion

                #region 최소/최대 투입 카운트를 구한다.
                /*
                int minInputCount = 99999999;
                int maxInputCount = 0;
                for (int i = 0; i < FuncInline.SiteInputCount.Length; i++)
                {
                    if (FuncInline.SiteInputCount[i] < minInputCount)
                    {
                        minInputCount = FuncInline.SiteInputCount[i];
                    }
                    if (FuncInline.SiteInputCount[i] > maxInputCount)
                    {
                        maxInputCount = FuncInline.SiteInputCount[i];
                    }
                }
                //*/
                #endregion

                #region 최소 투입카운트부터 1씩 증가시키면서 투입순서대로 빈 사이트 검색
                //for (int cnt = minInputCount; cnt <= maxInputCount; cnt++)
                //{
                // 목적지가 등록된 사이트는 배제해야 한다.
                for (int i = SearchInputIndex; i < FuncInline.SiteOrder.Length; i++)
                {
                    enumTeachingPos sitePos = FuncInline.SiteOrder[i];
                    if (rack >= 0 &&
                        (sitePos < enumTeachingPos.Site1_F_DT1 + rack * FuncInline.MaxSiteCount ||
                                sitePos > enumTeachingPos.Site13_F_FT3 + rack * FuncInline.MaxSiteCount))
                    {
                        continue;
                    }
                    int siteIndex = (int)sitePos - (int)enumTeachingPos.Site1_F_DT1;
                    if (//FuncInline.SiteInputCount[siteIndex] == cnt &&
                        FuncInline.UseSite[siteIndex] && // 사이트 사용체크되고
                        !DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + siteIndex * GlobalVar.DIModuleGap) && // PCB 확인 안 되고
                        FuncInline.PCBInfo[(int)sitePos].PCBStatus == FuncInline.enumSMDStatus.UnKnown && // PCB 상태 없고
                        FuncInline.SiteAction[siteIndex] == FuncInline.enumSiteAction.Waiting) // 사이트 동작 없으면
                    {
                        if ((//(FuncInline.BuyerChange != enumBuyerChange.Blue) ||
                                GetEmptySiteCount(siteIndex) > 0) &&
                            CheckDestination(sitePos) == enumTeachingPos.None)
                        {
                            return enumTeachingPos.Site1_F_DT1 + siteIndex;
                        }
                    }
                }
                for (int i = 0; i < SearchInputIndex; i++)
                {
                    enumTeachingPos sitePos = FuncInline.SiteOrder[i];
                    if (rack >= 0 &&
                        (sitePos < enumTeachingPos.Site1_F_DT1 + rack * FuncInline.MaxSiteCount ||
                                sitePos > enumTeachingPos.Site13_F_FT3 + rack * FuncInline.MaxSiteCount))
                    {
                        continue;
                    }
                    int siteIndex = (int)sitePos - (int)enumTeachingPos.Site1_F_DT1;
                    if (//FuncInline.SiteInputCount[siteIndex] == cnt &&
                        FuncInline.UseSite[siteIndex] &&
                        !DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + siteIndex * GlobalVar.DIModuleGap) &&
                        FuncInline.PCBInfo[(int)sitePos].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
                        FuncInline.SiteAction[siteIndex] == FuncInline.enumSiteAction.Waiting)
                    {
                        if ((//(FuncInline.BuyerChange != enumBuyerChange.Blue) ||
                                GetEmptySiteCount(siteIndex) > 0) &&
                            CheckDestination(sitePos) == enumTeachingPos.None)
                        {
                            return enumTeachingPos.Site1_F_DT1 + siteIndex;
                        }
                    }
                }
                //}
                #endregion
            }
            catch (Exception ex)
            {
                debug("CheckEmptySite : " + ex.ToString());
                debug(ex.StackTrace);
            }
            return enumTeachingPos.None;
        }

        public static bool reservedSite(enumTeachingPos site) // 배출 -> 투입 연계 동작 예정사이트인가?
        {
            return FuncInline.PCBInfo[(int)enumTeachingPos.FrontPassLine].Destination == site ||
                    FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination == site ||
                    FuncInline.PCBInfo[(int)enumTeachingPos.RearPassLine].Destination == site;
        }

        public static enumTeachingPos CheckFailedSite(int rack, bool destCheck) // NG처리되어 배출 가능한 사이트를 검색.
        {

            // xout, badmark 경우 양품으로 보내는 옵션이면 양품으로 판단하므로 배제해야 함.
            // 이 경우 테스트 단계에서 양품처리(?) 되므로 무시?
            // fail 처리는 그대로 하고, 별도로 xout,badmark 때문인지로 판단해야 할 듯

            // NG 컨베어 동작중이면 통과
            if (destCheck &&
                NGOut)
            {
                return enumTeachingPos.None;
            }

            // NG 버퍼 PCB 있으면 통과. 버퍼 쓰더라도 NG는 버퍼에 포함하지 않아야 한다.
            if (FuncInline.PCBInfo[(int)enumTeachingPos.NgBuffer].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                DIO.GetDIData(enumDINames.X04_3_Out_Conveyor_NG_PCB_Stop_Sensor) ||
                DIO.GetDIData(enumDINames.X402_2_Out_Conveyor_Ng_PCB_In_Sensor) ||
                NGOut)
            {
                return enumTeachingPos.None;
            }

            // 양품또는 NG 배출 목표지인 동선이 등록되어 있으면 양품 배출 불가
            //*
            if (//CheckDestination(enumTeachingPos.OutConveyor) != enumTeachingPos.None ||
                CheckDestination(enumTeachingPos.NgBuffer) != enumTeachingPos.None)
            {
                return enumTeachingPos.None;
            }
            //*/


            #region 이동중인 PCB 있으면 통과. 좌우 각각 상황에 따라 판단해야 함. 
            bool leftMoving = false; // 좌측 사이트에서 배출에 걸리는 동선이 있는가?
            bool rightMoving = false; // 우측 사이트에서 배출에 걸리는 동선이 있는가?
            /*
            if (destCheck)
            {
                // 좌우측 공통조건이라도 분리해야 할 경우 있으므로 각각 코딩한다.
                // 좌측. 전사이트, Pass1(투입등록시도 동선에 걸린다), Lift1Down, Pass2, Lift2Down 에 동선 등록시. 배출카운트 경우 배출컨베어 비어 있으면 상관 없고, 차 있다면 Lift2Down 버퍼로 쓸 경우 하나만 있어야 한다.
                if (FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Down].Destination == enumTeachingPos.OutConveyor ||
                    FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Down].Destination == enumTeachingPos.NgBuffer ||
                    FuncInline.PCBInfo[(int)enumTeachingPos.RearPassline].Destination == enumTeachingPos.NgBuffer ||
                    FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Down].Destination == enumTeachingPos.NgBuffer ||
                    FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Down].Destination == enumTeachingPos.NgBuffer)
                {
                    leftMoving = true;
                }
                else
                {
                    for (enumTeachingPos site = enumTeachingPos.Site1_F_DT1; site <= enumTeachingPos.Site26_R_FT3; site++)
                    {
                        if (FuncInline.PCBInfo[(int)site].Destination == enumTeachingPos.NgBuffer)
                        {
                            leftMoving = true;
                            break;
                        }
                    }
                }
                // 우측, 전사이트, Pass1(우측 사이트로 동선 등록시만), Lift1Down, Pass2, Lift2Down 에 동선 등록시. 출카운트 경우 배출컨베어 비어 있으면 상관 없고, 차 있다면 Lift2Down 버퍼로 쓸 경우 하나만 있어야 한다.
                if (//FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Down].Destination == enumTeachingPos.OutConveyor ||
                    FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Down].Destination == enumTeachingPos.NgBuffer ||
                    //FuncInline.PCBInfo[(int)enumTeachingPos.RearPassline].Destination == enumTeachingPos.OutConveyor ||
                    FuncInline.PCBInfo[(int)enumTeachingPos.RearPassline].Destination == enumTeachingPos.NgBuffer ||
                    FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Down].Destination == enumTeachingPos.OutConveyor ||
                    FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Down].Destination == enumTeachingPos.NgBuffer)
                {
                    rightMoving = true;
                }
                else
                {
                    for (enumTeachingPos site = enumTeachingPos.Site1_F_DT1; site <= enumTeachingPos.Site26_R_FT3; site++)
                    {
                        if (FuncInline.PCBInfo[(int)site].Destination == enumTeachingPos.NgBuffer)
                        {
                            rightMoving = true;
                            break;
                        }
                    }
                }
            }
            //*/
            // 좌우 구분 없이 NG 배출가능 여부로 판단
            if (!CheckOutputEnable(true))
            {
                return enumTeachingPos.None;
            }
            /*
            if (CheckMovingPCB())
            {
                return enumTeachingPos.None;
            }
            //*/
            #endregion


            #region 내려놓을 자리 없으면 통과
            /*
            if (destCheck &&
                (FuncInline.PCBInfo[(int)enumTeachingPos.NgBuffer].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                        FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Down].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                        (rack == 0 && FuncInline.PCBInfo[(int)enumTeachingPos.RearPassline].PCBStatus != FuncInline.enumSMDStatus.UnKnown) ||
                        (rack == 0 && FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Down].PCBStatus != FuncInline.enumSMDStatus.UnKnown) ||
                        DIO.GetDIData(enumDINames.X08_5_NG_Buffer_Start_Sensor) ||
                        DIO.GetDIData(enumDINames.X08_7_NG_Buffer_End_Sensor)))
            {
                return enumTeachingPos.None; // NG 내려놓을 자리 없으면 failed PCB 없는 걸로
            }
            //*/
            #endregion


            /*
             * 0. Retest는 무시
             * 1. PCB 상태 무조건 NG로 보내야 하는 경우, timeout,cancel
             * 2. Xout 해당 어레이 있고 XOut2NG true 경우, BadMark 해당 어레이 있고 BadMark2NG true 경우
             * 3. 이외 Fail 어레이 있고 fail2unload false 경우
             * 4. 이외 모든 어레이 Pass고 Pass2NG true 경우
             * 5. 나머지 무시
             * xout 해당 NG 면 무조건 NG
             * xout 해당 unload 면 나머지 fail ng 경우 ng
             //*/

            try
            {

                #region NotUse 사이트 먼저 검색. 검색 순번부터 마지막까지
                //for (enumTeachingPos site = enumTeachingPos.Site1_F_DT1; site <= enumTeachingPos.Site26_R_FT3; site++)
                for (int i = FuncInline.SearchNGIndex; i < FuncInline.SiteOrder.Length; i++)
                {
                    enumTeachingPos site = FuncInline.SiteOrder[i];
                    // 완료 아니면 통과. 0.Retest 무시
                    if (FuncInline.PCBInfo[(int)site].PCBStatus <= FuncInline.enumSMDStatus.Testing ||
                        FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.ReTest)
                    {
                        continue;
                    }

                    int siteIndex = (int)site - (int)enumTeachingPos.Site1_F_DT1;
                    if (rack >= 0 &&
                        (site < enumTeachingPos.Site1_F_DT1 + rack * FuncInline.MaxSiteCount ||
                                site > enumTeachingPos.Site13_F_FT3 + rack * FuncInline.MaxSiteCount))
                    {
                        continue;
                    }
                    // 좌측 사이트 동선체크에 걸리면 통과
                    if (leftMoving &&
                        siteIndex < FuncInline.MaxSiteCount)
                    {
                        continue;
                    }
                    // 우측 사이트 동선체크에 걸리면 통과
                    if (rightMoving &&
                        siteIndex >= FuncInline.MaxSiteCount)
                    {
                        continue;
                    }

                    // 목적지 있는 경우 배제
                    if (FuncInline.PCBInfo[(int)site].Destination != enumTeachingPos.None)
                    {
                        continue;
                    }


                    if (!FuncInline.UseSite[siteIndex])
                    {
                        // 1. 무조건 NG
                        if (FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Response_NG ||
                            FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Cancel ||
                            FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Timeout ||
                            FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.User_Cancel)
                        {
                            return site;
                        }

                        // 양품이건 불량이건 배출방향 NG로 지정된 경우
                        if ((FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Pass ||
                                    FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Fail) &
                            FuncInline.PCBInfo[(int)site].OutputNG)
                        {
                            return site;
                        }

                        #region xout,badmark,나머지 ng 있는지 확인
                        bool isXout = false;
                        bool isBadMark = false;
                        bool isNg = false;
                        for (int array = 0; array < FuncInline.MaxArrayCount; array++)
                        {
                            if (ArrayUse[array])
                            {
                                //*
                                if (FuncInline.PCBInfo[(int)site].Xout[array])
                                {
                                    isXout = true;
                                }
                                if (FuncInline.PCBInfo[(int)site].BadMark[array])
                                {
                                    isBadMark = true;
                                }
                                //*/
                                if (FuncInline.PCBInfo[(int)site].SMDStatus[array] == FuncInline.enumSMDStatus.Test_Fail)
                                {
                                    isNg = true;
                                }
                            }
                        }
                        #endregion

                        #region 2. xout 해당 NG 보내기면 무조건 NG
                        //*
                        if (isXout)
                        {
                            if (FuncInline.XoutToNG)
                            {
                                return site;
                            }
                            else if (isNg &&
                                !FuncInline.NGToUnloading)
                            {
                                return site;
                            }
                        }
                        //*/
                        #endregion

                        #region 2. badmark 해당 NG 보내기면 무조건 NG
                        //*
                        if (isBadMark)
                        {
                            if (FuncInline.BadMarkToNG)
                            {
                                return site;
                            }
                            else if (isNg &&
                                !FuncInline.NGToUnloading)
                            {
                                return site;
                            }
                        }
                        //*/
                        #endregion

                        #region 3. 이외 Fail 어레이 있고 fail2unload false 경우
                        if (!isXout &&
                            !isBadMark &&
                            isNg &&
                            !FuncInline.NGToUnloading)
                        {
                            return site;
                        }
                        #endregion

                        #region 4. 이외 모든 어레이 Pass고 Pass2NG true 경우
                        if (!isXout &&
                            !isBadMark &&
                            !isNg &&
                            FuncInline.PassToNG)
                        {
                            return site;
                        }
                        #endregion


                    }
                }
                #endregion
                #region NotUse 사이트 먼저 검색. 첨부터 검색순번 전까지
                //for (enumTeachingPos site = enumTeachingPos.Site1_F_DT1; site <= enumTeachingPos.Site26_R_FT3; site++)
                for (int i = 0; i < FuncInline.SearchNGIndex; i++)
                {
                    enumTeachingPos site = FuncInline.SiteOrder[i];
                    // 완료 아니면 통과. 0.Retest 무시
                    if (FuncInline.PCBInfo[(int)site].PCBStatus <= FuncInline.enumSMDStatus.Testing ||
                        FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.ReTest)
                    {
                        continue;
                    }

                    int siteIndex = (int)site - (int)enumTeachingPos.Site1_F_DT1;
                    if (rack >= 0 &&
                        (site < enumTeachingPos.Site1_F_DT1 + rack * FuncInline.MaxSiteCount ||
                                site > enumTeachingPos.Site13_F_FT3 + rack * FuncInline.MaxSiteCount))
                    {
                        continue;
                    }
                    // 좌측 사이트 동선체크에 걸리면 통과
                    if (leftMoving &&
                        siteIndex < FuncInline.MaxSiteCount)
                    {
                        continue;
                    }
                    // 우측 사이트 동선체크에 걸리면 통과
                    if (rightMoving &&
                        siteIndex >= FuncInline.MaxSiteCount)
                    {
                        continue;
                    }

                    // 목적지 있는 경우 배제
                    if (FuncInline.PCBInfo[(int)site].Destination != enumTeachingPos.None)
                    {
                        continue;
                    }


                    if (!FuncInline.UseSite[siteIndex])
                    {
                        // 1. 무조건 NG
                        if (FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Response_NG ||
                            FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Cancel ||
                            FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Timeout ||
                            FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.User_Cancel)
                        {
                            return site;
                        }

                        // 양품이건 불량이건 배출방향 NG로 지정된 경우
                        if ((FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Pass ||
                                    FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Fail) &
                            FuncInline.PCBInfo[(int)site].OutputNG)
                        {
                            return site;
                        }

                        #region xout,badmark,나머지 ng 있는지 확인
                        bool isXout = false;
                        bool isBadMark = false;
                        bool isNg = false;
                        for (int array = 0; array < FuncInline.MaxArrayCount; array++)
                        {
                            if (ArrayUse[array])
                            {
                                //*
                                if (FuncInline.PCBInfo[(int)site].Xout[array])
                                {
                                    isXout = true;
                                }
                                if (FuncInline.PCBInfo[(int)site].BadMark[array])
                                {
                                    isBadMark = true;
                                }
                                //*/
                                if (FuncInline.PCBInfo[(int)site].SMDStatus[array] == FuncInline.enumSMDStatus.Test_Fail)
                                {
                                    isNg = true;
                                }
                            }
                        }
                        #endregion

                        #region 2. xout 해당 NG 보내기면 무조건 NG
                        //*
                        if (isXout)
                        {
                            if (FuncInline.XoutToNG)
                            {
                                return site;
                            }
                            else if (isNg &&
                                !FuncInline.NGToUnloading)
                            {
                                return site;
                            }
                        }
                        //*/
                        #endregion

                        #region 2. badmark 해당 NG 보내기면 무조건 NG
                        //*
                        if (isBadMark)
                        {
                            if (FuncInline.BadMarkToNG)
                            {
                                return site;
                            }
                            else if (isNg &&
                                !FuncInline.NGToUnloading)
                            {
                                return site;
                            }
                        }
                        //*/
                        #endregion

                        #region 3. 이외 Fail 어레이 있고 fail2unload false 경우
                        if (!isXout &&
                            !isBadMark &&
                            isNg &&
                            !FuncInline.NGToUnloading)
                        {
                            return site;
                        }
                        #endregion

                        #region 4. 이외 모든 어레이 Pass고 Pass2NG true 경우
                        if (!isXout &&
                            !isBadMark &&
                            !isNg &&
                            FuncInline.PassToNG)
                        {
                            return site;
                        }
                        #endregion


                    }
                }
                #endregion

                #region Use Site 검색. 검색 순번부터 마지막까지
                //for (enumTeachingPos site = enumTeachingPos.Site1_F_DT1; site <= enumTeachingPos.Site26_R_FT3; site++)
                for (int i = FuncInline.SearchNGIndex; i < FuncInline.SiteOrder.Length; i++)
                {
                    enumTeachingPos site = FuncInline.SiteOrder[i];
                    // 완료 아니면 통과. 0.Retest 무시
                    if (FuncInline.PCBInfo[(int)site].PCBStatus <= FuncInline.enumSMDStatus.Testing ||
                        FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.ReTest)
                    {
                        continue;
                    }

                    int siteIndex = (int)site - (int)enumTeachingPos.Site1_F_DT1;
                    if (rack >= 0 &&
                        (site < enumTeachingPos.Site1_F_DT1 + rack * FuncInline.MaxSiteCount ||
                                site > enumTeachingPos.Site13_F_FT3 + rack * FuncInline.MaxSiteCount))
                    {
                        continue;
                    }
                    // 좌측 사이트 동선체크에 걸리면 통과
                    if (leftMoving &&
                        siteIndex < FuncInline.MaxSiteCount)
                    {
                        continue;
                    }
                    // 우측 사이트 동선체크에 걸리면 통과
                    if (rightMoving &&
                        siteIndex >= FuncInline.MaxSiteCount)
                    {
                        continue;
                    }

                    // 목적지 있는 경우 배제
                    if (FuncInline.PCBInfo[(int)site].Destination != enumTeachingPos.None)
                    {
                        continue;
                    }


                    if (FuncInline.UseSite[siteIndex])
                    {
                        // 1. 무조건 NG
                        if (FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Response_NG ||
                            FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Cancel ||
                            FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Timeout ||
                            FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.User_Cancel)
                        {
                            return site;
                        }

                        // 양품이건 불량이건 배출방향 NG로 지정된 경우
                        if ((FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Pass ||
                                    FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Fail) &
                            FuncInline.PCBInfo[(int)site].OutputNG)
                        {
                            return site;
                        }

                        #region xout,badmark,나머지 ng 있는지 확인
                        bool isXout = false;
                        bool isBadMark = false;
                        bool isNg = false;
                        for (int array = 0; array < FuncInline.MaxArrayCount; array++)
                        {
                            if (ArrayUse[array])
                            {
                                //*
                                if (FuncInline.PCBInfo[(int)site].Xout[array])
                                {
                                    isXout = true;
                                }
                                if (FuncInline.PCBInfo[(int)site].BadMark[array])
                                {
                                    isBadMark = true;
                                }
                                //*/
                                if (FuncInline.PCBInfo[(int)site].SMDStatus[array] == FuncInline.enumSMDStatus.Test_Fail)
                                {
                                    isNg = true;
                                }
                            }
                        }
                        #endregion

                        #region 2. xout 해당 NG 보내기면 무조건 NG
                        //*
                        if (isXout)
                        {
                            if (FuncInline.XoutToNG)
                            {
                                return site;
                            }
                            else if (isNg &&
                                !FuncInline.NGToUnloading)
                            {
                                return site;
                            }
                        }
                        //*/
                        #endregion

                        #region 2. badmark 해당 NG 보내기면 무조건 NG
                        //*
                        if (isBadMark)
                        {
                            if (FuncInline.BadMarkToNG)
                            {
                                return site;
                            }
                            else if (isNg &&
                                !FuncInline.NGToUnloading)
                            {
                                return site;
                            }
                        }
                        //*/
                        #endregion

                        #region 3. 이외 Fail 어레이 있고 fail2unload false 경우
                        if (!isXout &&
                            !isBadMark &&
                            isNg &&
                            !FuncInline.NGToUnloading)
                        {
                            return site;
                        }
                        #endregion

                        #region 4. 이외 모든 어레이 Pass고 Pass2NG true 경우
                        if (!isXout &&
                            !isBadMark &&
                            !isNg &&
                            FuncInline.PassToNG)
                        {
                            return site;
                        }
                        #endregion


                    }
                }
                #endregion
                #region Use Site 검색. 첨부터 검색순번까지
                //for (enumTeachingPos site = enumTeachingPos.Site1_F_DT1; site <= enumTeachingPos.Site26_R_FT3; site++)
                for (int i = 0; i < FuncInline.SearchNGIndex; i++)
                {
                    enumTeachingPos site = FuncInline.SiteOrder[i];
                    // 완료 아니면 통과. 0.Retest 무시
                    if (FuncInline.PCBInfo[(int)site].PCBStatus <= FuncInline.enumSMDStatus.Testing ||
                        FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.ReTest)
                    {
                        continue;
                    }

                    int siteIndex = (int)site - (int)enumTeachingPos.Site1_F_DT1;
                    if (rack >= 0 &&
                        (site < enumTeachingPos.Site1_F_DT1 + rack * FuncInline.MaxSiteCount ||
                                site > enumTeachingPos.Site13_F_FT3 + rack * FuncInline.MaxSiteCount))
                    {
                        continue;
                    }
                    // 좌측 사이트 동선체크에 걸리면 통과
                    if (leftMoving &&
                        siteIndex < FuncInline.MaxSiteCount)
                    {
                        continue;
                    }
                    // 우측 사이트 동선체크에 걸리면 통과
                    if (rightMoving &&
                        siteIndex >= FuncInline.MaxSiteCount)
                    {
                        continue;
                    }

                    // 목적지 있는 경우 배제
                    if (FuncInline.PCBInfo[(int)site].Destination != enumTeachingPos.None)
                    {
                        continue;
                    }


                    if (FuncInline.UseSite[siteIndex])
                    {
                        // 1. 무조건 NG
                        if (FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Response_NG ||
                            FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Cancel ||
                            FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Timeout ||
                            FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.User_Cancel)
                        {
                            return site;
                        }

                        // 양품이건 불량이건 배출방향 NG로 지정된 경우
                        if ((FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Pass ||
                                    FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Fail) &
                            FuncInline.PCBInfo[(int)site].OutputNG)
                        {
                            return site;
                        }

                        #region xout,badmark,나머지 ng 있는지 확인
                        bool isXout = false;
                        bool isBadMark = false;
                        bool isNg = false;
                        for (int array = 0; array < FuncInline.MaxArrayCount; array++)
                        {
                            if (ArrayUse[array])
                            {
                                //*
                                if (FuncInline.PCBInfo[(int)site].Xout[array])
                                {
                                    isXout = true;
                                }
                                if (FuncInline.PCBInfo[(int)site].BadMark[array])
                                {
                                    isBadMark = true;
                                }
                                //*/
                                if (FuncInline.PCBInfo[(int)site].SMDStatus[array] == FuncInline.enumSMDStatus.Test_Fail)
                                {
                                    isNg = true;
                                }
                            }
                        }
                        #endregion

                        #region 2. xout 해당 NG 보내기면 무조건 NG
                        //*
                        if (isXout)
                        {
                            if (FuncInline.XoutToNG)
                            {
                                return site;
                            }
                            else if (isNg &&
                                !FuncInline.NGToUnloading)
                            {
                                return site;
                            }
                        }
                        //*/
                        #endregion

                        #region 2. badmark 해당 NG 보내기면 무조건 NG
                        //*
                        if (isBadMark)
                        {
                            if (FuncInline.BadMarkToNG)
                            {
                                return site;
                            }
                            else if (isNg &&
                                !FuncInline.NGToUnloading)
                            {
                                return site;
                            }
                        }
                        //*/
                        #endregion

                        #region 3. 이외 Fail 어레이 있고 fail2unload false 경우
                        if (!isXout &&
                            !isBadMark &&
                            isNg &&
                            !FuncInline.NGToUnloading)
                        {
                            return site;
                        }
                        #endregion

                        #region 4. 이외 모든 어레이 Pass고 Pass2NG true 경우
                        if (!isXout &&
                            !isBadMark &&
                            !isNg &&
                            FuncInline.PassToNG)
                        {
                            return site;
                        }
                        #endregion


                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return enumTeachingPos.None;
        }

        public static enumTeachingPos CheckDestination(enumTeachingPos dest) // NG 또는 Output으로 보내고 있는 PCB 존재 확인용
        {
            try
            {
                for (int i = 0; i < FuncInline.PCBInfo.Length; i++)
                {
                    if ((enumTeachingPos)i != dest &&
                        FuncInline.PCBInfo[i].Destination == dest)
                    {
                        return (enumTeachingPos)i;
                    }
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }

            return enumTeachingPos.None;
        }

        public static enumTeachingPos CheckPCBDestination(enumTeachingPos pos) // 해당 위치의 PCB가 보내 져야 할 곳 확인. 사이트에서는 호출하지 말고 중간 경우에만 확인한다.
        {
            try
            {
                #region 검사 전이면 사이트
                if (FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Before_Command)
                {
                    if (FuncInline.PCBInfo[(int)pos].Destination >= enumTeachingPos.Site1_F_DT1 &&
                        FuncInline.PCBInfo[(int)pos].Destination <= enumTeachingPos.Site26_R_FT3)
                    {
                        // 목적지 있으면 해당 목적지
                        return FuncInline.PCBInfo[(int)pos].Destination;
                    }
                    else
                    {
                        // 좌우 사이트를 확인해야 하는데. 일단 Site1로 리턴해 둠
                        return enumTeachingPos.Site1_F_DT1;
                    }
                }
                #endregion
                #region 재검사 대상이면 사이트
                if (FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.ReTest)
                {
                    if (FuncInline.PCBInfo[(int)pos].Destination >= enumTeachingPos.Site1_F_DT1 &&
                        FuncInline.PCBInfo[(int)pos].Destination <= enumTeachingPos.Site26_R_FT3)
                    {
                        // 목적지 있으면 해당 목적지
                        return FuncInline.PCBInfo[(int)pos].Destination;
                    }
                    else
                    {
                        // 좌우 사이트를 확인해야 하는데. 일단 Site1로 리턴해 둠
                        return enumTeachingPos.Site1_F_DT1;
                    }
                }
                #endregion

                #region 1.PCB 상태 무조건 NG로 보내야 하는 경우
                if (FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Response_NG ||
                    FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Cancel ||
                    FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Timeout ||
                    FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.User_Cancel)
                {
                    return enumTeachingPos.NgBuffer;
                }
                #endregion

                #region 무조건 배출 방향 NG로 정해진 경우
                if ((FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Pass ||
                            FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Fail) &&
                    FuncInline.PCBInfo[(int)pos].OutputNG)
                {
                    return enumTeachingPos.NgBuffer;
                }
                #endregion

                #region xout,badmark,나머지 ng 있는지 확인
                bool isXout = false;
                bool isBadMark = false;
                bool isNg = false;
                for (int array = 0; array < FuncInline.MaxArrayCount; array++)
                {
                    if (ArrayUse[array])
                    {
                        ///*
                        if (FuncInline.PCBInfo[(int)pos].Xout[array])
                        {
                            isXout = true;
                        }
                        if (FuncInline.PCBInfo[(int)pos].BadMark[array])
                        {
                            isBadMark = true;
                        }
                        //*/
                        if (FuncInline.PCBInfo[(int)pos].SMDStatus[array] == FuncInline.enumSMDStatus.Test_Fail)
                        {
                            isNg = true;
                        }
                    }
                }
                #endregion

                #region 2.Xout 해당 어레이 있고 XOut2NG false 경우. 배출컨베어
                //xout 해당 unload고 pass / fail 시 NG 보내는 조건 없으면 해당
                //*
                if (isXout &&
                    !FuncInline.XoutToNG)
                {
                    if (!isNg ||
                        FuncInline.NGToUnloading)
                    {
                        return enumTeachingPos.OutConveyor;
                    }
                }
                //*/
                #endregion
                #region 2. xout 해당 NG 보내기면 무조건 NG
                //*
                if (isXout)
                {
                    if (FuncInline.XoutToNG)
                    {
                        return enumTeachingPos.NgBuffer;
                    }
                    else if (isNg &&
                        !FuncInline.NGToUnloading)
                    {
                        return enumTeachingPos.NgBuffer;
                    }
                }
                //*/
                #endregion

                #region 2.BadMark 해당 어레이 있고 BadMark 해당 어레이 있고 BadMark2NG false 경우. 배출 컨베어
                //*
                if (isBadMark &&
                    !FuncInline.BadMarkToNG)
                {
                    if (!isNg ||
                        FuncInline.NGToUnloading)
                    {
                        return enumTeachingPos.OutConveyor;
                    }
                }
                //*/
                #endregion

                #region 3.이외 모든 어레이 Pass고 Pass2NG false 경우
                if (!isXout &&
                    !isBadMark &&
                    !isNg &&
                    !FuncInline.PassToNG)
                {
                    return enumTeachingPos.OutConveyor;
                }
                #endregion
                #region 3. 이외 Fail 어레이 있고 fail2unload false 경우
                if (!isXout &&
                    !isBadMark &&
                    isNg &&
                    !FuncInline.NGToUnloading)
                {
                    return enumTeachingPos.NgBuffer;
                }
                #endregion

                #region 4.이외 Fail 어레이 있고 fail2unload true 경우
                if (!isXout &&
                    !isBadMark &&
                    isNg &&
                    FuncInline.NGToUnloading)
                {
                    return enumTeachingPos.OutConveyor;
                }
                #endregion
                #region 4. 이외 모든 어레이 Pass고 Pass2NG true 경우
                if (!isXout &&
                    !isBadMark &&
                    !isNg &&
                    FuncInline.PassToNG)
                {
                    return enumTeachingPos.NgBuffer;
                }
                #endregion

                #region 5.나머지 xout,badmark,ng 모두 아니면 양품
                if (!isXout &&
                    !isBadMark &&
                    !isNg &&
                    !FuncInline.PassToNG)
                {
                    return enumTeachingPos.OutConveyor;
                }
                #endregion
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return enumTeachingPos.None;
        }

        public static bool CheckPassedPCB(enumTeachingPos pos)
        {
            try
            {
                if (FuncInline.PCBInfo[(int)pos].PCBStatus <= FuncInline.enumSMDStatus.Testing)
                {
                    return false;
                }

                #region 1.PCB 상태 무조건 NG로 보내야 하는 경우 배제
                if (FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Response_NG ||
                    FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Cancel ||
                    FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Timeout ||
                    FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.User_Cancel)
                {
                    return false;
                }
                #endregion

                #region 무조건 NG로 보내는 경우 배제
                if ((FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Pass ||
                            FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Fail) &&
                    FuncInline.PCBInfo[(int)pos].OutputNG)
                {
                    return false;
                }
                #endregion

                #region xout,badmark,나머지 ng 있는지 확인
                bool isXout = false;
                bool isBadMark = false;
                bool isNg = false;
                for (int array = 0; array < FuncInline.MaxArrayCount; array++)
                {
                    if (ArrayUse[array])
                    {
                        //*
                        if (FuncInline.PCBInfo[(int)pos].Xout[array])
                        {
                            isXout = true;
                        }
                        if (FuncInline.PCBInfo[(int)pos].BadMark[array])
                        {
                            isBadMark = true;
                        }
                        //*/
                        if (FuncInline.PCBInfo[(int)pos].SMDStatus[array] == FuncInline.enumSMDStatus.Test_Fail)
                        {
                            isNg = true;
                        }
                    }
                }
                #endregion

                #region 2.Xout 해당 어레이 있고 XOut2NG false 경우
                //xout 해당 unload고 pass / fail 시 NG 보내는 조건 없으면 해당
                //*
                if (isXout &&
                    !FuncInline.XoutToNG)
                {
                    if (!isNg ||
                        FuncInline.NGToUnloading)
                    {
                        return true;
                    }
                }
                //*/
                #endregion
                #region 2.BadMark 해당 어레이 있고 BadMark 해당 어레이 있고 BadMark2NG false 경우
                //*
                if (isBadMark &&
                    !FuncInline.BadMarkToNG)
                {
                    if (!isNg ||
                        FuncInline.NGToUnloading)
                    {
                        return true;
                    }
                }
                //*/
                #endregion

                #region 3.이외 모든 어레이 Pass고 Pass2NG false 경우
                if (!isXout &&
                    !isBadMark &&
                    !isNg &&
                    !FuncInline.PassToNG)
                {
                    return true;
                }
                #endregion

                #region 4.이외 Fail 어레이 있고 fail2unload true 경우
                if (!isXout &&
                    !isBadMark &&
                    isNg &&
                    FuncInline.NGToUnloading)
                {
                    return true;
                }
                #endregion

                #region 5.나머지 xout,badmark,ng 모두 아니면 양품
                //if (!isXout &&
                //    !isBadMark &&
                //    !isNg)
                //{
                //    return enumTeachingPos.Site1_F_DT1 + siteIndex;
                //}
                #endregion
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        // 최종 양품 배출한 사이트를 기억한다.
        // 배출 검색시 투입순서에 따라 최종배출한 사이트부터 검색하게 된다.
        public static void NextOutputIndex(enumTeachingPos site)
        {
            #region 리사이트 배출은 무시
            if (FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.ReTest)
            {
                return;
            }
            #endregion

            #region 불량 배출 검색 순서
            if (FuncInline.CheckNGPCB(site))
            {
                // 배출한 사이트 기준으로 현재 순번을 구하고
                // 다음 순번을 찾아
                // 서로 다른 렉이면 반대편 렉의 순번으로 지정, 기존 렉의 순번 증가
                // 같은 렉이면 다음 순번으로 지정

                #region 현재 순번 구하기
                int currentIndex = -1;
                for (int i = 0; i < FuncInline.SiteOrder.Length; i++)
                {
                    if (FuncInline.SiteOrder[i] == site)
                    {
                        currentIndex = i;
                        break;
                    }
                }
                if (currentIndex == -1)
                {
                    return;
                }
                bool currentLeft = FuncInline.SiteOrder[currentIndex] < enumTeachingPos.Site14_R_DT1;
                if (currentLeft)
                {
                    FuncInline.LeftNGIndex = currentIndex;
                }
                else
                {
                    FuncInline.RightNGIndex = currentIndex;
                }
                #endregion

                #region 다음 순번 확인
                int nextIndex = currentIndex + 1;
                if (nextIndex >= FuncInline.SiteOrder.Length)
                {
                    nextIndex = 0;
                }
                bool nextLeft = FuncInline.SiteOrder[nextIndex] < enumTeachingPos.Site14_R_DT1;
                #endregion

                #region 서로 다른 렉이면 반대편 렉의 순번으로 지정
                if (currentLeft != nextLeft)
                {
                    if (nextLeft)
                    {
                        FuncInline.SearchNGIndex = FuncInline.LeftNGIndex;
                    }
                    else
                    {
                        FuncInline.SearchNGIndex = FuncInline.RightNGIndex;
                    }

                    #region 기존 렉의 순번 변경시켜 둬야 중복이 되지 않는다.
                    if (currentLeft)
                    {
                        bool checkIndex = false;
                        for (int i = FuncInline.LeftNGIndex + 1; i < FuncInline.SiteOrder.Length; i++)
                        {
                            if (FuncInline.SiteOrder[i] >= enumTeachingPos.Site1_F_DT1 &&
                                FuncInline.SiteOrder[i] <= enumTeachingPos.Site13_F_FT3)
                            {
                                FuncInline.LeftNGIndex = i;
                                checkIndex = true;
                                break;
                            }
                        }
                        if (!checkIndex)
                        {
                            for (int i = 0; i < FuncInline.LeftNGIndex; i++)
                            {
                                if (FuncInline.SiteOrder[i] >= enumTeachingPos.Site1_F_DT1 &&
                                    FuncInline.SiteOrder[i] <= enumTeachingPos.Site13_F_FT3)
                                {
                                    FuncInline.LeftNGIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        bool checkIndex = false;
                        for (int i = FuncInline.RightNGIndex + 1; i < FuncInline.SiteOrder.Length; i++)
                        {
                            if (FuncInline.SiteOrder[i] >= enumTeachingPos.Site14_R_DT1 &&
                                FuncInline.SiteOrder[i] <= enumTeachingPos.Site26_R_FT3)
                            {
                                FuncInline.RightNGIndex = i;
                                checkIndex = true;
                                break;
                            }
                        }
                        if (!checkIndex)
                        {
                            for (int i = 0; i < FuncInline.RightNGIndex; i++)
                            {
                                if (FuncInline.SiteOrder[i] >= enumTeachingPos.Site14_R_DT1 &&
                                    FuncInline.SiteOrder[i] <= enumTeachingPos.Site26_R_FT3)
                                {
                                    FuncInline.RightNGIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                    #endregion
                }
                #endregion

                #region 같은 렉이면 다음 순번으로 지정
                else
                {
                    FuncInline.SearchNGIndex = nextIndex;
                }
                #endregion

                /*
                FuncInline.SearchNGIndex++;
                if (FuncInline.SearchNGIndex >= FuncInline.SiteOrder.Length)
                {
                    FuncInline.SearchNGIndex = 0;
                }
                //*/
            }

            #endregion
            #region 양품 배출 검색 순서
            else
            {
                // 배출한 사이트 기준으로 현재 순번을 구하고
                // 다음 순번을 찾아
                // 서로 다른 렉이면 반대편 렉의 순번으로 지정, 기존 렉의 순번 증가
                // 같은 렉이면 다음 순번으로 지정

                #region 현재 순번 구하기
                int currentIndex = -1;
                for (int i = 0; i < FuncInline.SiteOrder.Length; i++)
                {
                    if (FuncInline.SiteOrder[i] == site)
                    {
                        currentIndex = i;
                        break;
                    }
                }
                if (currentIndex == -1)
                {
                    return;
                }
                bool currentLeft = FuncInline.SiteOrder[currentIndex] < enumTeachingPos.Site14_R_DT1;
                if (currentLeft)
                {
                    FuncInline.LeftOutputIndex = currentIndex;
                }
                else
                {
                    FuncInline.RightOutputIndex = currentIndex;
                }
                #endregion

                #region 다음 순번 확인
                int nextIndex = currentIndex + 1;
                if (nextIndex >= FuncInline.SiteOrder.Length)
                {
                    nextIndex = 0;
                }
                bool nextLeft = FuncInline.SiteOrder[nextIndex] < enumTeachingPos.Site14_R_DT1;
                #endregion

                #region 서로 다른 렉이면 반대편 렉의 순번으로 지정
                if (currentLeft != nextLeft)
                {
                    if (nextLeft)
                    {
                        FuncInline.SearchOutputIndex = FuncInline.LeftOutputIndex;
                    }
                    else
                    {
                        FuncInline.SearchOutputIndex = FuncInline.RightOutputIndex;
                    }

                    #region 기존 렉의 순번 변경시켜 둬야 중복이 되지 않는다.
                    if (currentLeft)
                    {
                        bool checkIndex = false;
                        for (int i = FuncInline.LeftOutputIndex + 1; i < FuncInline.SiteOrder.Length; i++)
                        {
                            if (FuncInline.SiteOrder[i] >= enumTeachingPos.Site1_F_DT1 &&
                                FuncInline.SiteOrder[i] <= enumTeachingPos.Site13_F_FT3)
                            {
                                FuncInline.LeftOutputIndex = i;
                                checkIndex = true;
                                break;
                            }
                        }
                        if (!checkIndex)
                        {
                            for (int i = 0; i < FuncInline.LeftOutputIndex; i++)
                            {
                                if (FuncInline.SiteOrder[i] >= enumTeachingPos.Site1_F_DT1 &&
                                    FuncInline.SiteOrder[i] <= enumTeachingPos.Site13_F_FT3)
                                {
                                    FuncInline.LeftOutputIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        bool checkIndex = false;
                        for (int i = FuncInline.RightOutputIndex + 1; i < FuncInline.SiteOrder.Length; i++)
                        {
                            if (FuncInline.SiteOrder[i] >= enumTeachingPos.Site14_R_DT1 &&
                                FuncInline.SiteOrder[i] <= enumTeachingPos.Site26_R_FT3)
                            {
                                FuncInline.RightOutputIndex = i;
                                checkIndex = true;
                                break;
                            }
                        }
                        if (!checkIndex)
                        {
                            for (int i = 0; i < FuncInline.RightOutputIndex; i++)
                            {
                                if (FuncInline.SiteOrder[i] >= enumTeachingPos.Site14_R_DT1 &&
                                    FuncInline.SiteOrder[i] <= enumTeachingPos.Site26_R_FT3)
                                {
                                    FuncInline.RightOutputIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                    #endregion
                }
                #endregion

                #region 같은 렉이면 다음 순번으로 지정
                else
                {
                    FuncInline.SearchOutputIndex = nextIndex;
                }
                #endregion

                /*
                FuncInline.SearchOutputIndex++;
                if (FuncInline.SearchOutputIndex >= FuncInline.SiteOrder.Length)
                {
                    FuncInline.SearchOutputIndex = 0;
                }
                //*/
            }
            #endregion
        }

        // 최종 불량 배출한 사이트를 기억한다.
        // 배출 검색시 투입순서에 따라 최종배출한 사이트부터 검색하게 된다.
        // NextOutputIndex에서 통합판단한다.
        public static void NextNGIndex(enumTeachingPos site)
        {
            /*
            for (int i = 0; i < FuncInline.SiteOrder.Length; i++)
            {
                if (FuncInline.SiteOrder[i] == site)
                {
                    FuncInline.SearchOutputIndex = i;
                }
            }
            //*/
            FuncInline.SearchNGIndex++;
            if (FuncInline.SearchNGIndex >= FuncInline.SiteOrder.Length)
            {
                FuncInline.SearchNGIndex = 0;
            }
        }

        public static bool CheckOututPCB(enumTeachingPos pos) // 지정 위치 PCB가 양품 배출 대상인가?
        {
            try
            {
                if (FuncInline.PCBInfo[(int)pos].PCBStatus <= FuncInline.enumSMDStatus.Testing)
                {
                    return false;
                }

                #region 1.PCB 상태 무조건 NG로 보내야 하는 경우 배제
                if (FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Response_NG ||
                    FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Cancel ||
                    FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Timeout ||
                    FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.User_Cancel)
                {
                    return false;
                }
                #endregion

                // 양품이건 불량이건 배출방향 NG로 지정된 경우
                if ((FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Pass ||
                            FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Fail) &
                    FuncInline.PCBInfo[(int)pos].OutputNG)
                {
                    return false;
                }

                #region xout,badmark,나머지 ng 있는지 확인
                bool isXout = false;
                bool isBadMark = false;
                bool isNg = false;
                for (int array = 0; array < FuncInline.MaxArrayCount; array++)
                {
                    if (ArrayUse[array])
                    {
                        //*
                        if (FuncInline.PCBInfo[(int)pos].Xout[array])
                        {
                            isXout = true;
                        }
                        if (FuncInline.PCBInfo[(int)pos].BadMark[array])
                        {
                            isBadMark = true;
                        }
                        //*/
                        if (FuncInline.PCBInfo[(int)pos].SMDStatus[array] == FuncInline.enumSMDStatus.Test_Fail)
                        {
                            isNg = true;
                        }
                    }
                }
                #endregion

                #region 2.Xout 해당 어레이 있고 XOut2NG false 경우
                //xout 해당 unload고 pass / fail 시 NG 보내는 조건 없으면 해당
                //*
                if (isXout &&
                    !FuncInline.XoutToNG)
                {
                    if (!isNg ||
                        FuncInline.NGToUnloading)
                    {
                        return true;
                    }
                }
                //*/
                #endregion
                #region 2.BadMark 해당 어레이 있고 BadMark 해당 어레이 있고 BadMark2NG false 경우
                //*
                if (isBadMark &&
                    !FuncInline.BadMarkToNG)
                {
                    if (!isNg ||
                        FuncInline.NGToUnloading)
                    {
                        return true;
                    }
                }
                //*/
                #endregion

                #region 3.이외 모든 어레이 Pass고 Pass2NG false 경우
                if (!isXout &&
                    !isBadMark &&
                    !isNg &&
                    !FuncInline.PassToNG)
                {
                    return true;
                }
                #endregion

                #region 4.이외 Fail 어레이 있고 fail2unload true 경우
                if (!isXout &&
                    !isBadMark &&
                    isNg &&
                    FuncInline.NGToUnloading)
                {
                    return true;
                }
                #endregion

                #region 5.나머지 xout,badmark,ng 모두 아니면 양품
                //if (!isXout &&
                //    !isBadMark &&
                //    !isNg)
                //{
                //    return enumTeachingPos.Site1_F_DT1 + siteIndex;
                //}
                #endregion
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }

            return false;
        }

        // 해당 위치 PCB가 NG배출 대상인가?
        public static bool CheckNGPCB(enumTeachingPos pos)
        {
            try
            {
                if (FuncInline.PCBInfo[(int)pos].PCBStatus <= FuncInline.enumSMDStatus.Testing)
                {
                    return false;
                }
                // 1. 무조건 NG
                if (FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Response_NG ||
                    FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Cancel ||
                    FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Timeout ||
                    FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.User_Cancel)
                {
                    return true;
                }

                // 양품이건 불량이건 배출방향 NG로 지정된 경우
                if ((FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Pass ||
                            FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Fail) &
                    FuncInline.PCBInfo[(int)pos].OutputNG)
                {
                    return true;
                }

                #region xout,badmark,나머지 ng 있는지 확인
                bool isXout = false;
                bool isBadMark = false;
                bool isNg = false;
                for (int array = 0; array < FuncInline.MaxArrayCount; array++)
                {
                    if (ArrayUse[array])
                    {
                        //*
                        if (FuncInline.PCBInfo[(int)pos].Xout[array])
                        {
                            isXout = true;
                        }
                        if (FuncInline.PCBInfo[(int)pos].BadMark[array])
                        {
                            isBadMark = true;
                        }
                        //*/
                        if (FuncInline.PCBInfo[(int)pos].SMDStatus[array] == FuncInline.enumSMDStatus.Test_Fail)
                        {
                            isNg = true;
                        }
                    }
                }
                #endregion

                #region 2. xout 해당 NG 보내기면 무조건 NG
                //*
                if (isXout)
                {
                    if (FuncInline.XoutToNG)
                    {
                        return true;
                    }
                    else if (isNg &&
                        !FuncInline.NGToUnloading)
                    {
                        return true;
                    }
                }
                //*/
                #endregion

                #region 2. badmark 해당 NG 보내기면 무조건 NG
                //*
                if (isBadMark)
                {
                    if (FuncInline.BadMarkToNG)
                    {
                        return true;
                    }
                    else if (isNg &&
                        !FuncInline.NGToUnloading)
                    {
                        return true;
                    }
                }
                //*/
                #endregion

                #region 3. 이외 Fail 어레이 있고 fail2unload false 경우
                if (!isXout &&
                    !isBadMark &&
                    isNg &&
                    !FuncInline.NGToUnloading)
                {
                    return true;
                }
                #endregion

                #region 4. 이외 모든 어레이 Pass고 Pass2NG true 경우
                if (!isXout &&
                    !isBadMark &&
                    !isNg &&
                    FuncInline.PassToNG)
                {
                    return true;
                }
                #endregion
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public static enumTeachingPos CheckPassedSite(int rack, bool destCheck) // 로봇이 취출할 Test_Pass 끝난 사이트 찾기. ==> 취출 후 배출 위치 배출 가능
        {

            // loading/unloading으로 배출 가능한지만 판단
            // xout, badmark 경우 각각 설정에 따라 판단하므로 해당 사이트는 각 설정에 따라 판단한다.
            // 목적지가 정해진 사이트는 배제

            // 배출할 곳 모두 PCB 있다면 배출 불가능하다.
            /*
            if (destCheck)
            {
                return enumTeachingPos.None;
            }
            //*/

            #region 불량 배출 우선 위해 불량 배출 대기 사이트 있으면 없다고 판단
            if (CheckFailedSite(0, false) != enumTeachingPos.None ||
                CheckFailedSite(1, false) != enumTeachingPos.None)
            {
                return enumTeachingPos.None;
            }
            #endregion

            #region 양품 배출 카운트 체크
            if (!CheckOutputEnable(false))
            {
                return enumTeachingPos.None;
            }
            #endregion

            #region 이동중인 PCB 있으면 통과. 좌우 각각 상황에 따라 판단해야 함. 
            bool leftMoving = false; // 좌측 사이트에서 배출에 걸리는 동선이 있는가?
            bool rightMoving = false; // 우측 사이트에서 배출에 걸리는 동선이 있는가?
            if (destCheck)// &&
                          //FuncInline.PCBInfo[(int)enumTeachingPos.OutConveyor].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
            {
                // 좌우측 공통조건이라도 분리해야 할 경우 있으므로 각각 코딩한다.
                // 배출 컨베어 비어 있으면 OK로 처리해서 리프트를 배출 버퍼로 사용하도록 한다.
                // 좌측. 전사이트, Pass1(투입등록시도 동선에 걸린다), Lift1Down, Pass2, Lift2Down 에 동선 등록시. 배출카운트 경우 배출컨베어 비어 있으면 상관 없고, 차 있다면 Lift2Down 버퍼로 쓸 경우 하나만 있어야 한다.
                // Out,Lift1Down,Lift2Down PCB 있으면 불가
                if (//FuncInline.PCBInfo[(int)enumTeachingPos.OutConveyor].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                    //FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Down].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                    FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Down].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
                {
                    leftMoving = true;
                }
                // 배출컨베어 PCB 있다면, 배출 예약된 곳이 있다면 불가. 우측 리사이트는 배제해서 배출하도록 해야 함.
                else if (FuncInline.PCBInfo[(int)enumTeachingPos.OutConveyor].PCBStatus != FuncInline.enumSMDStatus.UnKnown &&
                    (FuncInline.PCBInfo[(int)enumTeachingPos.FrontPassLine].Destination == enumTeachingPos.OutConveyor ||
                            FuncInline.PCBInfo[(int)enumTeachingPos.FrontPassLine].Destination == enumTeachingPos.NgBuffer ||
                            FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Down].Destination == enumTeachingPos.OutConveyor ||
                            FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Down].Destination == enumTeachingPos.NgBuffer ||
                            FuncInline.PCBInfo[(int)enumTeachingPos.RearPassLine].Destination == enumTeachingPos.OutConveyor ||
                            FuncInline.PCBInfo[(int)enumTeachingPos.RearPassLine].Destination == enumTeachingPos.NgBuffer ||
                            FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Down].Destination == enumTeachingPos.OutConveyor ||
                            FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Down].Destination == enumTeachingPos.NgBuffer))
                {
                    leftMoving = true;
                }
                else if (FuncInline.PCBInfo[(int)enumTeachingPos.OutConveyor].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
                {
                    for (enumTeachingPos site = enumTeachingPos.Site1_F_DT1; site <= enumTeachingPos.Site26_R_FT3; site++)
                    {
                        if (FuncInline.PCBInfo[(int)site].Destination == enumTeachingPos.OutConveyor ||
                            FuncInline.PCBInfo[(int)site].Destination == enumTeachingPos.NgBuffer)
                        {
                            leftMoving = true;
                            break;
                        }
                    }
                }

                // 우측, 전사이트, Pass1(우측 사이트로 동선 등록시만), Lift1Down, Pass2, Lift2Down 에 동선 등록시. 출카운트 경우 배출컨베어 비어 있으면 상관 없고, 차 있다면 Lift2Down 버퍼로 쓸 경우 하나만 있어야 한다.
                // Out,Lift2Down PCB 있으면 불가
                /* 우측 리프트는 항시 바빠서 우선순위가 무조건 왼쪽보다 밀려 버린다.
                if (//FuncInline.PCBInfo[(int)enumTeachingPos.OutConveyor].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                    FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Down].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
                {
                    rightMoving = true;
                }
                //*/
                // 배출컨베어 PCB 있다면, 배출 예약된 곳이 있다면 불가
                if (FuncInline.PCBInfo[(int)enumTeachingPos.OutConveyor].PCBStatus != FuncInline.enumSMDStatus.UnKnown &&
                    (FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Down].Destination == enumTeachingPos.OutConveyor ||
                            FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Down].Destination == enumTeachingPos.NgBuffer ||
                            FuncInline.PCBInfo[(int)enumTeachingPos.RearPassLine].Destination == enumTeachingPos.OutConveyor ||
                            FuncInline.PCBInfo[(int)enumTeachingPos.RearPassLine].Destination == enumTeachingPos.NgBuffer ||
                            FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Down].Destination == enumTeachingPos.OutConveyor ||
                            FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Down].Destination == enumTeachingPos.NgBuffer))
                {
                    rightMoving = true;
                }
                else if (FuncInline.PCBInfo[(int)enumTeachingPos.OutConveyor].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
                {
                    for (enumTeachingPos site = enumTeachingPos.Site1_F_DT1; site <= enumTeachingPos.Site26_R_FT3; site++)
                    {
                        if (FuncInline.PCBInfo[(int)site].Destination != enumTeachingPos.None)
                        {
                            rightMoving = true;
                            break;
                        }
                    }
                }
            }

            /*
            if (CheckMovingPCB())
            {
                return enumTeachingPos.None;
            }
            //*/
            #endregion

            /*
            if (destCheck &&
                (FuncInline.PCBInfo[(int)enumTeachingPos.OutConveyor].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                        FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Down].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                        (rack == 0 && FuncInline.PCBInfo[(int)enumTeachingPos.RearPassline].PCBStatus != FuncInline.enumSMDStatus.UnKnown) ||
                        (rack == 0 && FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Down].PCBStatus != FuncInline.enumSMDStatus.UnKnown)))
            {
                return enumTeachingPos.None; // 로딩으로 내려놓을 자리 없으면 검사 완료 아닌 걸로
            }
            //*/

            // 동선상에서 Output과 NG로 등록된 게 없어야 한다.

            /*
             * 0. Retest는 무시
             * 1. PCB 상태 무조건 NG로 보내야 하는 경우 배제
             * 2. Xout 해당 어레이 있고 XOut2NG false 경우, BadMark 해당 어레이 있고 BadMark2NG false 경우
             * 4. 이외 모든 어레이 Pass고 Pass2NG false 경우
             * 3. 이외 Fail 어레이 있고 fail2unload true 경우
             * 5. 나머지 무시
             * xout 해당 unload고 pass/fail 시 NG 보내는 조건 없으면 해당
             //*/

            // 취출 후 배출로 바로 연결할 것이므로 양품경우 Unloading, NG경우 NG컨베어 자리가 있어야 함 ==> 양품 위치만 본다.
            try
            {
                #region NG unloading 보내는 경우 ng 사이트 먼저 검색
                if (FuncInline.NGToUnloading)
                {
                    for (int i = 0; i < FuncInline.SiteOrder.Length; i++)
                    {
                        enumTeachingPos site = FuncInline.SiteOrder[i];

                        #region 0. 완료 아니면 통과. 0.Retest 무시
                        if (FuncInline.PCBInfo[(int)site].PCBStatus <= FuncInline.enumSMDStatus.Testing ||
                            FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.ReTest)
                        {
                            continue;
                        }
                        #endregion

                        int siteIndex = (int)site - (int)enumTeachingPos.Site1_F_DT1;
                        if (rack >= 0 &&
                            (site < enumTeachingPos.Site1_F_DT1 + rack * FuncInline.MaxSiteCount ||
                                    site > enumTeachingPos.Site13_F_FT3 + rack * FuncInline.MaxSiteCount))
                        {
                            continue;
                        }
                        // 좌측 사이트 동선체크에 걸리면 통과
                        if (leftMoving &&
                            siteIndex < FuncInline.MaxSiteCount)
                        {
                            continue;
                        }
                        // 우측 사이트 동선체크에 걸리면 통과
                        if (rightMoving &&
                            siteIndex >= FuncInline.MaxSiteCount)
                        {
                            continue;
                        }

                        // 목적지 있는 경우 배제
                        if (FuncInline.PCBInfo[(int)site].Destination != enumTeachingPos.None)
                        {
                            continue;
                        }

                        #region 1.PCB 상태 무조건 NG로 보내야 하는 경우 배제
                        if (FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Response_NG ||
                            FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Cancel ||
                            FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Timeout ||
                            FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.User_Cancel)
                        {
                            //return site;
                            continue;
                        }
                        #endregion

                        // 양품이건 불량이건 배출방향 NG로 지정된 경우
                        if ((FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Pass ||
                                    FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Fail) &
                            FuncInline.PCBInfo[(int)site].OutputNG)
                        {
                            continue;
                        }

                        #region xout,badmark,나머지 ng 있는지 확인
                        bool isXout = false;
                        bool isBadMark = false;
                        bool isNg = false;
                        for (int array = 0; array < FuncInline.MaxArrayCount; array++)
                        {
                            if (ArrayUse[array])
                            {
                                //*
                                if (FuncInline.PCBInfo[(int)site].Xout[array])
                                {
                                    isXout = true;
                                }
                                if (FuncInline.PCBInfo[(int)site].BadMark[array])
                                {
                                    isBadMark = true;
                                }
                                //*/
                                if (FuncInline.PCBInfo[(int)site].SMDStatus[array] == FuncInline.enumSMDStatus.Test_Fail)
                                {
                                    isNg = true;
                                }
                            }
                        }
                        #endregion



                        #region 4.이외 Fail 어레이 있고 fail2unload true 경우
                        if (!isXout &&
                            !isBadMark &&
                            isNg &&
                            FuncInline.NGToUnloading)
                        {
                            return enumTeachingPos.Site1_F_DT1 + siteIndex;
                        }
                        #endregion

                        #region 5.나머지 xout,badmark,ng 모두 아니면 양품
                        //if (!isXout &&
                        //    !isBadMark &&
                        //    !isNg)
                        //{
                        //    return enumTeachingPos.Site1_F_DT1 + siteIndex;
                        //}
                        #endregion

                    }
                }
                #endregion

                #region NotUse 사이트 먼저 검색
                for (int i = 0; i < FuncInline.SiteOrder.Length; i++)
                {
                    enumTeachingPos site = FuncInline.SiteOrder[i];

                    #region 0. 완료 아니면 통과. 0.Retest 무시
                    if (FuncInline.PCBInfo[(int)site].PCBStatus <= FuncInline.enumSMDStatus.Testing ||
                        FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.ReTest)
                    {
                        continue;
                    }
                    #endregion

                    int siteIndex = (int)site - (int)enumTeachingPos.Site1_F_DT1;
                    if (rack >= 0 &&
                        (site < enumTeachingPos.Site1_F_DT1 + rack * FuncInline.MaxSiteCount ||
                                site > enumTeachingPos.Site13_F_FT3 + rack * FuncInline.MaxSiteCount))
                    {
                        continue;
                    }
                    // 좌측 사이트 동선체크에 걸리면 통과
                    if (leftMoving &&
                        siteIndex < FuncInline.MaxSiteCount)
                    {
                        continue;
                    }
                    // 우측 사이트 동선체크에 걸리면 통과
                    if (rightMoving &&
                        siteIndex >= FuncInline.MaxSiteCount)
                    {
                        continue;
                    }

                    // 목적지 있는 경우 배제
                    if (FuncInline.PCBInfo[(int)site].Destination != enumTeachingPos.None)
                    {
                        continue;
                    }

                    if (!FuncInline.UseSite[siteIndex])
                    {
                        #region 1.PCB 상태 무조건 NG로 보내야 하는 경우 배제
                        if (FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Response_NG ||
                            FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Cancel ||
                            FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Timeout ||
                            FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.User_Cancel)
                        {
                            //return site;
                            continue;
                        }
                        #endregion

                        // 양품이건 불량이건 배출방향 NG로 지정된 경우
                        if ((FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Pass ||
                                    FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Fail) &
                            FuncInline.PCBInfo[(int)site].OutputNG)
                        {
                            continue;
                        }

                        #region xout,badmark,나머지 ng 있는지 확인
                        bool isXout = false;
                        bool isBadMark = false;
                        bool isNg = false;
                        for (int array = 0; array < FuncInline.MaxArrayCount; array++)
                        {
                            if (ArrayUse[array])
                            {
                                //*
                                if (FuncInline.PCBInfo[(int)site].Xout[array])
                                {
                                    isXout = true;
                                }
                                if (FuncInline.PCBInfo[(int)site].BadMark[array])
                                {
                                    isBadMark = true;
                                }
                                //*/
                                if (FuncInline.PCBInfo[(int)site].SMDStatus[array] == FuncInline.enumSMDStatus.Test_Fail)
                                {
                                    isNg = true;
                                }
                            }
                        }
                        #endregion

                        #region 2.Xout 해당 어레이 있고 XOut2NG false 경우
                        //xout 해당 unload고 pass / fail 시 NG 보내는 조건 없으면 해당
                        //*
                        if (isXout &&
                            !FuncInline.XoutToNG)
                        {
                            if (!isNg ||
                                FuncInline.NGToUnloading)
                            {
                                return enumTeachingPos.Site1_F_DT1 + siteIndex;
                            }
                        }
                        //*/
                        #endregion
                        #region 2.BadMark 해당 어레이 있고 BadMark 해당 어레이 있고 BadMark2NG false 경우
                        //*
                        if (isBadMark &&
                            !FuncInline.BadMarkToNG)
                        {
                            if (!isNg ||
                                FuncInline.NGToUnloading)
                            {
                                return enumTeachingPos.Site1_F_DT1 + siteIndex;
                            }
                        }
                        //*/
                        #endregion

                        #region 3.이외 모든 어레이 Pass고 Pass2NG false 경우
                        if (!isXout &&
                            !isBadMark &&
                            !isNg &&
                            !FuncInline.PassToNG)
                        {
                            return enumTeachingPos.Site1_F_DT1 + siteIndex;
                        }
                        #endregion

                        #region 4.이외 Fail 어레이 있고 fail2unload true 경우
                        if (!isXout &&
                            !isBadMark &&
                            isNg &&
                            FuncInline.NGToUnloading)
                        {
                            return enumTeachingPos.Site1_F_DT1 + siteIndex;
                        }
                        #endregion

                        #region 5.나머지 xout,badmark,ng 모두 아니면 양품
                        //if (!isXout &&
                        //    !isBadMark &&
                        //    !isNg)
                        //{
                        //    return enumTeachingPos.Site1_F_DT1 + siteIndex;
                        //}
                        #endregion

                    }
                }
                #endregion

                // 순차적으로 골고루 우선순위를 주기 위해 SearchOutputIndex 기준으로 뒤쪽과 앞쪽까지 두 번으로 나눠 검색.
                for (int i = FuncInline.SearchOutputIndex; i < FuncInline.SiteOrder.Length; i++)
                {
                    enumTeachingPos site = FuncInline.SiteOrder[i];

                    #region 0. 완료 아니면 통과. 0.Retest 무시
                    if (FuncInline.PCBInfo[(int)site].PCBStatus <= FuncInline.enumSMDStatus.Testing ||
                        FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.ReTest)
                    {
                        continue;
                    }
                    #endregion
                    int siteIndex = (int)site - (int)enumTeachingPos.Site1_F_DT1;
                    if (rack >= 0 &&
                        (site <enumTeachingPos.Site1_F_DT1 + rack * FuncInline.MaxSiteCount ||
                                site > enumTeachingPos.Site13_F_FT3 + rack * FuncInline.MaxSiteCount))
                    {
                        continue;
                    }
                    // 좌측 사이트 동선체크에 걸리면 통과
                    if (leftMoving &&
                        siteIndex < FuncInline.MaxSiteCount)
                    {
                        continue;
                    }
                    // 우측 사이트 동선체크에 걸리면 통과
                    if (rightMoving &&
                        siteIndex >= FuncInline.MaxSiteCount)
                    {
                        continue;
                    }

                    // 목적지 있는 경우 배제
                    if (FuncInline.PCBInfo[(int)site].Destination != enumTeachingPos.None)
                    {
                        continue;
                    }

                    if (FuncInline.UseSite[siteIndex])
                    {
                        #region 1.PCB 상태 무조건 NG로 보내야 하는 경우 배제
                        if (FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Response_NG ||
                            FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Cancel ||
                            FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Timeout ||
                            FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.User_Cancel)
                        {
                            //return site;
                            continue;
                        }
                        #endregion

                        // 양품이건 불량이건 배출방향 NG로 지정된 경우
                        if ((FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Pass ||
                                    FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Fail) &
                            FuncInline.PCBInfo[(int)site].OutputNG)
                        {
                            continue;
                        }

                        #region xout,badmark,나머지 ng 있는지 확인
                        bool isXout = false;
                        bool isBadMark = false;
                        bool isNg = false;
                        for (int array = 0; array < FuncInline.MaxArrayCount; array++)
                        {
                            if (ArrayUse[array])
                            {
                                //*
                                if (FuncInline.PCBInfo[(int)site].Xout[array])
                                {
                                    isXout = true;
                                }
                                if (FuncInline.PCBInfo[(int)site].BadMark[array])
                                {
                                    isBadMark = true;
                                }
                                //*/
                                if (FuncInline.PCBInfo[(int)site].SMDStatus[array] == FuncInline.enumSMDStatus.Test_Fail)
                                {
                                    isNg = true;
                                }
                            }
                        }
                        #endregion

                        #region 2.Xout 해당 어레이 있고 XOut2NG false 경우
                        //xout 해당 unload고 pass / fail 시 NG 보내는 조건 없으면 해당
                        //*
                        if (isXout &&
                            !FuncInline.XoutToNG)
                        {
                            if (!isNg ||
                                FuncInline.NGToUnloading)
                            {
                                return enumTeachingPos.Site1_F_DT1 + siteIndex;
                            }
                        }
                        //*/
                        #endregion
                        #region 2.BadMark 해당 어레이 있고 BadMark 해당 어레이 있고 BadMark2NG false 경우
                        //*
                        if (isBadMark &&
                            !FuncInline.BadMarkToNG)
                        {
                            if (!isNg ||
                                FuncInline.NGToUnloading)
                            {
                                return enumTeachingPos.Site1_F_DT1 + siteIndex;
                            }
                        }
                        //*/
                        #endregion

                        #region 3.이외 모든 어레이 Pass고 Pass2NG false 경우
                        if (!isXout &&
                            !isBadMark &&
                            !isNg &&
                            !FuncInline.PassToNG)
                        {
                            return enumTeachingPos.Site1_F_DT1 + siteIndex;
                        }
                        #endregion

                        #region 4.이외 Fail 어레이 있고 fail2unload true 경우
                        if (!isXout &&
                            !isBadMark &&
                            isNg &&
                            FuncInline.NGToUnloading)
                        {
                            return enumTeachingPos.Site1_F_DT1 + siteIndex;
                        }
                        #endregion

                        #region 5.나머지 xout,badmark,ng 모두 아니면 양품
                        //if (!isXout &&
                        //    !isBadMark &&
                        //    !isNg)
                        //{
                        //    return enumTeachingPos.Site1_F_DT1 + siteIndex;
                        //}
                        #endregion
                    }
                }
                for (int i = 0; i < FuncInline.SearchOutputIndex; i++)
                {
                    enumTeachingPos site = FuncInline.SiteOrder[i];

                    #region 0. 완료 아니면 통과. 0.Retest 무시
                    if (FuncInline.PCBInfo[(int)site].PCBStatus <= FuncInline.enumSMDStatus.Testing ||
                        FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.ReTest)
                    {
                        continue;
                    }
                    #endregion
                    int siteIndex = (int)site - (int)enumTeachingPos.Site1_F_DT1;
                    if (FuncInline.UseSite[siteIndex])
                    {
                        #region 1.PCB 상태 무조건 NG로 보내야 하는 경우 배제
                        if (FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Response_NG ||
                            FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Cancel ||
                            FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Timeout ||
                            FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.User_Cancel)
                        {
                            continue;
                        }
                        #endregion

                        // 양품이건 불량이건 배출방향 NG로 지정된 경우
                        if ((FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Pass ||
                                    FuncInline.PCBInfo[(int)site].PCBStatus == FuncInline.enumSMDStatus.Test_Fail) &
                            FuncInline.PCBInfo[(int)site].OutputNG)
                        {
                            continue;
                        }

                        #region xout,badmark,나머지 ng 있는지 확인
                        bool isXout = false;
                        bool isBadMark = false;
                        bool isNg = false;
                        for (int array = 0; array < FuncInline.MaxArrayCount; array++)
                        {
                            if (ArrayUse[array])
                            {
                                //*
                                if (FuncInline.PCBInfo[(int)site].Xout[array])
                                {
                                    isXout = true;
                                }
                                if (FuncInline.PCBInfo[(int)site].BadMark[array])
                                {
                                    isBadMark = true;
                                }
                                //*/
                                if (FuncInline.PCBInfo[(int)site].SMDStatus[array] == FuncInline.enumSMDStatus.Test_Fail)
                                {
                                    isNg = true;
                                }
                            }
                        }
                        #endregion

                        #region 2.Xout 해당 어레이 있고 XOut2NG false 경우
                        //xout 해당 unload고 pass / fail 시 NG 보내는 조건 없으면 해당
                        //*
                        if (isXout &&
                            !FuncInline.XoutToNG)
                        {
                            if (!isNg ||
                                FuncInline.NGToUnloading)
                            {
                                return enumTeachingPos.Site1_F_DT1 + siteIndex;
                            }
                        }
                        //*/
                        #endregion
                        #region 2.BadMark 해당 어레이 있고 BadMark 해당 어레이 있고 BadMark2NG false 경우
                        //*
                        if (isBadMark &&
                            !FuncInline.BadMarkToNG)
                        {
                            if (!isNg ||
                                FuncInline.NGToUnloading)
                            {
                                return enumTeachingPos.Site1_F_DT1 + siteIndex;
                            }
                        }
                        //*/
                        #endregion

                        #region 3.이외 모든 어레이 Pass고 Pass2NG false 경우
                        if (!isXout &&
                            !isBadMark &&
                            !isNg &&
                            !FuncInline.PassToNG)
                        {
                            return enumTeachingPos.Site1_F_DT1 + siteIndex;
                        }
                        #endregion

                        #region 4.이외 Fail 어레이 있고 fail2unload true 경우
                        if (!isXout &&
                            !isBadMark &&
                            isNg &&
                            FuncInline.NGToUnloading)
                        {
                            return enumTeachingPos.Site1_F_DT1 + siteIndex;
                        }
                        #endregion

                        #region 5.나머지 xout,badmark,ng 모두 아니면 양품
                        //if (!isXout &&
                        //    !isBadMark &&
                        //    !isNg)
                        //{
                        //    return enumTeachingPos.Site1_F_DT1 + siteIndex;
                        //}
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                debug("CheckPassedSite : " + ex.ToString());
                debug(ex.StackTrace);
            }
            return enumTeachingPos.None;
        }


        public static enumTeachingPos CheckOtherRetestSite(int rack, bool destCheck) // 타사이트 재검사가 필요한 사이트 검색
        {
            try
            {
                #region 이동중인 PCB 있으면 통과
                bool leftMoving = false;
                bool rightMoving = false;
                if (destCheck)
                {
                    // 좌측. Pass1(FrontLift을 움직여야 하기 때문에 배제),lift1Up,좌측 사이트
                    if (FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                        FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination != enumTeachingPos.None)
                    {
                        leftMoving = true;
                    }
                    else
                    {
                        for (enumTeachingPos site = enumTeachingPos.Site1_F_DT1; site <= enumTeachingPos.Site13_F_FT3; site++)
                        {
                            if (site != FuncInline.PCBInfo[(int)site].Destination &&
                                FuncInline.PCBInfo[(int)site].Destination >= enumTeachingPos.Site1_F_DT1 &&
                                FuncInline.PCBInfo[(int)site].Destination <= enumTeachingPos.Site13_F_FT3)
                            {
                                leftMoving = true;
                                break;
                            }
                        }
                    }

                    // 우측, pass1(목표지가 좌측사이트 아닌 경우), lift1Up(목표지가 좌측사이트 아닌 경우), pass2,lift2Up, 전체사이트(lift2 움직여야 하기 때문)
                    if (//FuncInline.PCBInfo[(int)enumTeachingPos.RearPassline].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                        FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                        //FuncInline.PCBInfo[(int)enumTeachingPos.RearPassline].Destination != enumTeachingPos.None ||
                        FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Up].Destination != enumTeachingPos.None)
                    {
                        rightMoving = true;
                    }
                    /*
                    else if (//FuncInline.PCBInfo[(int)enumTeachingPos.FrontPassline].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                        FuncInline.PCBInfo[(int)enumTeachingPos.FrontPassline].Destination == enumTeachingPos.OutConveyor ||
                        (FuncInline.PCBInfo[(int)enumTeachingPos.FrontPassline].Destination >= enumTeachingPos.Site14_R_DT1 &&
                                FuncInline.PCBInfo[(int)enumTeachingPos.FrontPassline].Destination <= enumTeachingPos.Site26_R_FT3))
                    {
                        rightMoving = true;
                    }
                    else if (FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                        FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination == enumTeachingPos.OutConveyor ||
                        (FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination >= enumTeachingPos.Site14_R_DT1 &&
                                FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination <= enumTeachingPos.Site26_R_FT3))
                    {
                        rightMoving = true;
                    }
                    //*/
                    else
                    {
                        for (enumTeachingPos site = enumTeachingPos.Site1_F_DT1; site <= enumTeachingPos.Site26_R_FT3; site++)
                        {
                            if (FuncInline.PCBInfo[(int)site].Destination >= enumTeachingPos.Site14_R_DT1 &&
                                FuncInline.PCBInfo[(int)site].Destination <= enumTeachingPos.Site26_R_FT3)
                            {
                                rightMoving = true;
                                break;
                            }
                        }
                    }
                }
                /*
                if (CheckMovingPCB())
                {
                    return enumTeachingPos.None;
                }
                //*/
                #endregion

                // 빈사이트 없을 경우에는 재테스트 사이트 검색 배제
                if (destCheck &&
                    GetEmptySiteCount(0) == 0)
                //CheckEmptySite(0, false) == enumTeachingPos.None)
                {
                    leftMoving = true;
                    //return enumTeachingPos.None;
                }
                if (destCheck &&
                    GetEmptySiteCount(20) == 0)
                //CheckEmptySite(1, false) == enumTeachingPos.None)
                {
                    rightMoving = true;
                    //return enumTeachingPos.None;
                }



                for (int i = 0; i < FuncInline.MaxTestPCCount * FuncInline.MaxSiteCount; i++)
                {
                    // 같은 렉에 빈 사이트가 있을 경우에 검색한다.
                    enumTeachingPos sitePos = enumTeachingPos.Site1_F_DT1 + i;
                    if (rack >= 0 &&
                        (sitePos < enumTeachingPos.Site1_F_DT1 + rack * FuncInline.MaxSiteCount ||
                                sitePos > enumTeachingPos.Site13_F_FT3 + rack * FuncInline.MaxSiteCount))
                    {
                        continue;
                    }
                    int siteIndex = (int)sitePos - (int)enumTeachingPos.Site1_F_DT1;
                    // 좌측 사이트 동선체크에 걸리면 통과
                    if (leftMoving &&
                        siteIndex < FuncInline.MaxSiteCount)
                    {
                        continue;
                    }
                    // 우측 사이트 동선체크에 걸리면 통과
                    if (rightMoving &&
                        siteIndex >= FuncInline.MaxSiteCount)
                    {
                        continue;
                    }

                    // 목적지 있는 경우 배제
                    if (enumTeachingPos.Site1_F_DT1 + i != FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].Destination &&
                        FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].Destination != enumTeachingPos.None)
                    {
                        continue;
                    }


                    if (//CheckEmptySite(rack, false) != enumTeachingPos.None && // 같은 렉에 빈 사이트 있고
                        GetEmptySiteCount(rack * 20) > 0 &&
                        FuncInline.PCBInfo[(int)sitePos].PCBStatus == FuncInline.enumSMDStatus.ReTest)
                    {
                        return sitePos;
                    }
                }
            }
            catch (Exception ex)
            {
                debug("CheckOtherRetestSite : " + ex.ToString());
                debug(ex.StackTrace);
            }
            return enumTeachingPos.None;
        }

        #endregion


        #region 사이트 관련
        public static bool CheckSiteTestDone(int siteIndex) // 해당 사이트가 검사완료 했는가?
        {
            return CheckSiteTestDone(enumTeachingPos.Site1_F_DT1 + siteIndex);
        }
        public static bool CheckSiteTestDone(enumTeachingPos pos) // 해당 위치 검사완료 했는가?
        {
            try
            {
                if (pos < enumTeachingPos.Site1_F_DT1 ||
                    pos > enumTeachingPos.Site26_R_FT3)
                {
                    return false;
                }

                int siteIndex = (int)pos - (int)enumTeachingPos.Site1_F_DT1;

                if (!DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + siteIndex * GlobalVar.DIModuleGap)) // PCB 감지 안 되면 false
                {
                    return false;
                }

                if (FuncInline.PCBInfo[(int)pos].PCBStatus != FuncInline.enumSMDStatus.Response_NG &&
                    FuncInline.PCBInfo[(int)pos].PCBStatus != FuncInline.enumSMDStatus.Test_Cancel &&
                    FuncInline.PCBInfo[(int)pos].PCBStatus != FuncInline.enumSMDStatus.Test_Fail &&
                    FuncInline.PCBInfo[(int)pos].PCBStatus != FuncInline.enumSMDStatus.Test_Pass &&
                    FuncInline.PCBInfo[(int)pos].PCBStatus != FuncInline.enumSMDStatus.Test_Timeout &&
                    FuncInline.PCBInfo[(int)pos].PCBStatus != FuncInline.enumSMDStatus.ReTest) // PCB 상태 검사 완료 아니면 미완료
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return true;
        }
        public static void NextEmptySite(enumTeachingPos pos)
        {
            try
            {
                #region 카운트가 10 이상이면 전체 카운트 초기화
                /*
                if (FuncInline.SiteInputCount[(int)pos - (int)enumTeachingPos.Site1_F_DT1] >= 10)
                {
                    for (int i = 0; i < FuncInline.SiteInputCount.Length; i++)
                    {
                        FuncInline.SiteInputCount[i] = 0;
                    }
                }
                //*/
                #endregion

                FuncInline.SiteInputCount[(int)pos - (int)enumTeachingPos.Site1_F_DT1]++; // 현재 투입한 사이트 카운트 추가

                #region 최소/최대 투입 카운트를 구한다.
                /*
                int minInputCount = 99999999;
                int maxInputCount = 0;
                for (int i = 0; i < FuncInline.SiteInputCount.Length; i++)
                {
                    if (FuncInline.SiteInputCount[i] < minInputCount)
                    {
                        minInputCount = FuncInline.SiteInputCount[i];
                    }
                    if (FuncInline.SiteInputCount[i] > maxInputCount)
                    {
                        maxInputCount = FuncInline.SiteInputCount[i];
                    }
                }
                //debug("최소 투입 카운트 : " + minInputCount);
                //debug("최대 투입 카운트 : " + maxInputCount);
                //*/
                #endregion

                //enumTeachingPos indexSite = FuncInline.SiteOrder[SearchInputIndex];
                //int siteIndex = (int)pos - (int)enumTeachingPos.Site1_F_DT1;

                int currentIndex = SearchInputIndex; // 현재 투입 순번

                int inputIndex = -1; // 현재 투입한 사이트의 순번

                for (int i = 0; i < FuncInline.SiteOrder.Length; i++)
                {
                    if (FuncInline.SiteOrder[i] == pos)
                    {
                        inputIndex = i;
                        break;
                    }
                }

                #region 투입한 사이트와 투입할 인덱스 사이 빈 사이트 있으면 유지
                /*
                #region 순차로 있는 경우
                if (currentIndex <= inputIndex)
                {
                    for (int i = currentIndex; i <= inputIndex; i++)
                    {
                        enumTeachingPos site = FuncInline.SiteOrder[i];
                        if (CheckPosEmpty(site))
                        {
                            return;
                        }
                    }
                }
                #endregion
                #region 역순인 경우, 마지막까지 + 첨부터
                else
                {
                    for (int i = currentIndex; i < FuncInline.SiteOrder.Length; i++)
                    {
                        enumTeachingPos site = FuncInline.SiteOrder[i];
                        if (CheckPosEmpty(site))
                        {
                            return;
                        }
                    }
                    for (int i = 0; i <= inputIndex; i++)
                    {
                        enumTeachingPos site = FuncInline.SiteOrder[i];
                        if (CheckPosEmpty(site))
                        {
                            return;
                        }
                    }
                }
                #endregion
                //*/
                #endregion

                currentIndex = inputIndex;

                /*
                현재 순번 사이트가 차 있거나 투입카운트가 최소가 아니면,
                현재 순번에 해당하는 렉의 투입순번 기준으로 증가시킨 인덱스가 같은 인덱스면 해당 렉의 인덱스로 지정.
                렉이 바뀌면 바뀐 렉의 인덱스로 지정, 기존 렉의 인덱스는 증가시켜 둔다.
                리사이트 경우는 전체 인덱스는 같은 렉일 경우에만 이동시키고, 해당 렉의 인덱스를 이동시킨다.
                //*/
                //if (!CheckPosEmpty(indexSite) ||
                //    FuncInline.SiteInputCount[siteIndex] > minInputCount)
                //{
                bool currentLeft = pos <= enumTeachingPos.Site13_F_FT3;
                int nextIndex = inputIndex + 1;
                if (nextIndex >= FuncInline.SiteOrder.Length)
                {
                    nextIndex = 0;
                }
                bool nextLeft = FuncInline.SiteOrder[nextIndex] <= enumTeachingPos.Site13_F_FT3;
                if (currentLeft == nextLeft)
                {
                    // 같은 렉이면 같은 렉의 인덱스 증가
                    if (currentLeft)
                    {
                        LeftInputIndex = nextIndex;
                    }
                    else
                    {
                        RightInputIndex = nextIndex;
                    }
                    if (FuncInline.PCBInfo[(int)pos].PCBStatus != FuncInline.enumSMDStatus.ReTest)
                    {
                        SearchInputIndex = nextIndex;
                    }
                }
                else// if (FuncInline.PCBInfo[(int)pos].PCBStatus != FuncInline.enumSMDStatus.ReTest)
                {
                    // 다른 렉이면 리사이트 아닌 경우 다른 렉의 인덱스로 지정, 
                    if (FuncInline.PCBInfo[(int)pos].PCBStatus != FuncInline.enumSMDStatus.ReTest)
                    {
                        if (nextLeft)
                        {
                            SearchInputIndex = LeftInputIndex;
                        }
                        else
                        {
                            SearchInputIndex = RightInputIndex;
                        }
                    }

                    // 기존 렉의 인덱스는 증가시켜 둬야 리사이트자리 다시 가는 일이 없다.
                    if (currentLeft)
                    {
                        bool checkIndex = false;
                        for (int i = LeftInputIndex + 1; i < FuncInline.SiteOrder.Length; i++)
                        {
                            if (FuncInline.SiteOrder[i] >= enumTeachingPos.Site1_F_DT1 &&
                                FuncInline.SiteOrder[i] <= enumTeachingPos.Site13_F_FT3)
                            {
                                LeftInputIndex = i;
                                checkIndex = true;
                                break;
                            }
                        }
                        if (!checkIndex)
                        {
                            for (int i = 0; i < LeftInputIndex; i++)
                            {
                                if (FuncInline.SiteOrder[i] >= enumTeachingPos.Site1_F_DT1 &&
                                    FuncInline.SiteOrder[i] <= enumTeachingPos.Site13_F_FT3)
                                {
                                    LeftInputIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        bool checkIndex = false;
                        for (int i = RightInputIndex + 1; i < FuncInline.SiteOrder.Length; i++)
                        {
                            if (FuncInline.SiteOrder[i] >= enumTeachingPos.Site14_R_DT1 &&
                                FuncInline.SiteOrder[i] <= enumTeachingPos.Site26_R_FT3)
                            {
                                RightInputIndex = i;
                                checkIndex = true;
                                break;
                            }
                        }
                        if (!checkIndex)
                        {
                            for (int i = 0; i < RightInputIndex; i++)
                            {
                                if (FuncInline.SiteOrder[i] >= enumTeachingPos.Site14_R_DT1 &&
                                    FuncInline.SiteOrder[i] <= enumTeachingPos.Site26_R_FT3)
                                {
                                    RightInputIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                }
                //}

                /*
                // 현재 순번 사이트가 차 있거나 투입카운트가 최소가 아니면,
                // 최소 투입카운트인 다음 검색 사이트로 지정한다.
                //for (int cnt = minInputCount; cnt <= maxInputCount; cnt++)
                //{
                    if (!CheckPosEmpty(indexSite) ||
                        FuncInline.SiteInputCount[siteIndex] > minInputCount)
                    {
                        for (int i = SearchInputIndex + 1; i < FuncInline.SiteOrder.Length; i++)
                        {
                            enumTeachingPos searchSite = FuncInline.SiteOrder[i];
                            int searchIndex = (int)searchSite - (int)enumTeachingPos.Site1_F_DT1;
                            if (//CheckPosEmpty(searchSite) &&
                                FuncInline.SiteInputCount[searchIndex] == minInputCount)
                            {
                                //debug("SearchInputIndex 변경 " + indexSite.ToString() + " ==> " + FuncInline.SiteOrder[i] + "(" + i + ")");
                                SearchInputIndex = i;
                                return;
                            }
                        }
                        for (int i = 0; i < SearchInputIndex; i++)
                        {
                            enumTeachingPos searchSite = FuncInline.SiteOrder[i];
                            int searchIndex = (int)searchSite - (int)enumTeachingPos.Site1_F_DT1;
                            if (//CheckPosEmpty(searchSite) &&
                                FuncInline.SiteInputCount[searchIndex] == minInputCount)
                            {
                                //debug("SearchInputIndex 변경 " + indexSite.ToString() + " ==> " + FuncInline.SiteOrder[i] + "(" + i + ")");
                                SearchInputIndex = i;
                                return;
                            }
                        }
                    }
                //}

                FuncInline.SiteInputCount[siteIndex]++; // 현재 투입한 사이트 카운트 추가
                /*

                if (!CheckPosEmpty(indexSite))
                {
                    SearchInputIndex++;
                    if (SearchInputIndex >= FuncInline.SiteOrder.Length)
                    {
                        SearchInputIndex = 0;
                    }
                }
                //*/


                //debug("SearchInputIndex 변경 " + indexSite.ToString() + " ==> " + SearchInputIndex + "(" + FuncInline.SiteOrder[SearchInputIndex] + ")");
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }


        public static void PartClear(enumTeachingPos pos)
        {
            try
            {
                #region 사이트 경우 테스트중인 array에 STS전송
                if (pos >= enumTeachingPos.Site1_F_DT1 &&
                    pos <= enumTeachingPos.Site26_R_FT3)
                {
                    if (FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Testing)
                    {
                        int siteIndex = (int)pos - (int)enumTeachingPos.Site1_F_DT1;
                        int smdIndex = siteIndex / FuncInline.MaxSiteCount;
                        FuncInline.ComSMD[smdIndex].SendStop(siteIndex);
                    }
                }
                #endregion

                // PCB 정보 삭제
                if (pos != enumTeachingPos.None)// &&
                                                //(pos != enumTeachingPos.NG1 || !NGOut))
                {
                    FuncInline.ClearPCBInfo(pos);
                }

                #region lift/conveyor action 삭제. 
                // 앞뒤가 같이 묶여 있는데 같이 처리? 
                // 앞쪽 먼저 처리하고 뒤쪽은 다시 판단하면 될까?
                // 버튼 상태는 어떤 기준으로 표시?
                FuncInline.PCBInfo[(int)pos].StopWatch.Stop();
                FuncInline.PCBInfo[(int)pos].StopWatch.Reset();
                switch (pos)
                {
                    case enumTeachingPos.InConveyor:
                        FuncInline.InConveyorAction = FuncInline.enumLiftAction.Waiting;
                        FuncError.RemoveError(FuncInline.enumErrorPart.InConveyor);
                        FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.InConveyor] = false;
                        break;
                   
                    case enumTeachingPos.Site1_F_DT1:
                 
                    case enumTeachingPos.Site13_F_FT3:
                    case enumTeachingPos.Site14_R_DT1:
                        int siteIndex = (int)pos - (int)enumTeachingPos.Site1_F_DT1;
                        //FuncLog.WriteLog("PartClear : SiteAction Change - " + pos.ToString() + " Waiting");
                        if (FuncInline.SiteAction[siteIndex] != FuncInline.enumSiteAction.Waiting)
                        {
                            FuncLog.WriteLog_Tester(pos.ToString() + " action change : " + FuncInline.SiteAction[siteIndex].ToString() + " ==> Waiting");
                        }
                        FuncInline.SiteAction[siteIndex] = FuncInline.enumSiteAction.Waiting;
                        break;
                }
                // PCB 감지는 앞에서 거르고 있다. 그러므로 PCB 여부는 판단할 필요가 없지만. PCBInfo는 확인해야 함.
                // 배출시는 OutputLIft 제외하고 PCB 상태 초기화하고 뒷라인 감지여부에 따라 뒷라인 세팅
                // 투입시는 InputLIft 제외하고 앞쪽 상황 따라 다시 판단
                #endregion

                // 해당 파트 에러 삭제
                //if (part != FuncInline.enumErrorPart.Program)
                //{
                FuncError.RemoveAllError();

                //}
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

       

        public static void StopTest(int siteNo)
        {
            try
            {
                int siteIndex = siteNo - 1;
                int smdIndex = siteIndex / FuncInline.MaxSiteCount;

                //if (FuncWin.MessageBoxOK("Cancel test at Site" + siteNo))
                //{
                FuncInline.ComSMD[smdIndex].SendStop(siteIndex);
                for (int i = 0; i < FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + siteIndex].SMDStatus.Length; i++)
                {
                    if (ArrayUse[i] &&
                        FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + siteIndex].SMDStatus[i] >= FuncInline.enumSMDStatus.Command_Sent &&
                        FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + siteIndex].SMDStatus[i] <= FuncInline.enumSMDStatus.Testing)
                    {
                        if (FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + siteIndex].ErrorCode[i] == -1)
                        {
                            FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + siteIndex].ErrorCode[i] = 998;
                            FuncLog.WriteLog_Tester("AutoInline_PCBInfo " + (enumTeachingPos.Site1_F_DT1 + siteIndex).ToString() + " .SMDStatus " + (i + 1) + " = FuncInline.enumSMDStatus.User_Cancel");
                            string msg = "STS" + Util.IntToString(siteIndex + 1, 2) + Util.IntToString(i + 1, 2) + "[" + FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + siteIndex].Barcode[i] + "]";
                            FuncInline.InsertCommLog(siteIndex + 1, i + 1, "STS", msg, 0, 998, "USER_CANCEL"); // USER_CANCEL
                        }
                        FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + siteIndex].SMDStatus[i] = FuncInline.enumSMDStatus.Test_Fail; // User_Cancel 하지 말고 바로 fail 처리한다.
                    }
                }
                if (FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + siteIndex].PCBStatus <= FuncInline.enumSMDStatus.Testing)
                {
                    FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + siteIndex].PCBStatus = FuncInline.enumSMDStatus.User_Cancel;
                }
                FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + siteIndex].NgType = enumNGType.TestCancel;
                //debug((enumTeachingPos.Site1_F_DT1 + siteIndex).ToString() + "==> TestCancel");
                //}
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        public static void StartTest(int siteNo)
        {
            try
            {
                int siteIndex = siteNo - 1;
                int smdIndex = siteIndex / 7;
                //if (FuncWin.MessageBoxOK("Start test at Site" + siteNo))
                //{
                // 시작 안 된 어레이 하나라도 없으면 통과
                int cmdCount = 0;
                for (int i = 0; i < FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + siteIndex].SMDStatus.Length; i++)
                {
                    if (FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + siteIndex].SMDStatus[i] == FuncInline.enumSMDStatus.Before_Command)
                    {
                        cmdCount++;
                    }
                }
                if (cmdCount > 0)
                {
                    FuncInline.ComSMD[smdIndex].SendStart(siteIndex);
                }
                //}
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        





        public static bool SiteReOpen(enumTeachingPos site) // 자체 재검사 위해 핀 올리기
        {
            if (site < enumTeachingPos.Site1_F_DT1 ||
                site > enumTeachingPos.Site26_R_FT3) // 사이트가 아닌 경우 패쓰
            {
                return true;
            }

            int offset = (int)site - (int)enumTeachingPos.Site1_F_DT1;
            try
            {

                #region PCB 체크 시간 경과시 에러처리
                //*
                if (FuncInline.SiteCheckTime[offset] == null ||
                    !FuncInline.SiteCheckTime[offset].IsRunning)
                {
                    Util.StartWatch(ref FuncInline.SiteCheckTime[offset]);
                }
                else if (!GlobalVar.Simulation &&
                    FuncInline.SiteCheckTime[offset] != null &&
                    FuncInline.SiteCheckTime[offset].IsRunning &&
                    FuncInline.SiteCheckTime[offset].ElapsedMilliseconds > GlobalVar.NormalTimeout * 1000)
                {
                    //debug("SiteReOpen PCB 체크 시간 경과시 에러처리");

                    #region 실패 사유 조합
                    string reason = "";
                    #endregion

                    if (reason.Length > 0)
                    {
                        Util.InitWatch(ref FuncInline.SiteCheckTime[offset]);
 
                        switch (site)
                        {
                            case enumTeachingPos.Site1_F_DT1:
                            case enumTeachingPos.Site2_F_DT2:
                            case enumTeachingPos.Site3_F_DT3:
                            case enumTeachingPos.Site4_F_DT4:
                            case enumTeachingPos.Site5_F_DT5:
                            case enumTeachingPos.Site6_F_DT6:
                            case enumTeachingPos.Site7_F_DT7:
                            case enumTeachingPos.Site8_F_DT8:
                            case enumTeachingPos.Site9_F_DT9:
                            case enumTeachingPos.Site10_F_DT10_FT4:
                                DIO.WriteDOData(enumDONames.Y403_4_Front_DT_1_Motor_Cw - (offset * 2), false);
                                DIO.WriteDOData(enumDONames.Y403_6_Front_DT_1_Motor_Ccw - (offset * 2), false);
                                break;
                            case enumTeachingPos.Site11_F_FT1:
                            case enumTeachingPos.Site12_F_FT2:
                            case enumTeachingPos.Site13_F_FT3:
                                DIO.WriteDOData(enumDONames.Y402_1_Front_FT_1_Motor_Cw - (offset * 2), false);
                                DIO.WriteDOData(enumDONames.Y402_3_Front_FT_1_Motor_Ccw - (offset * 2), false);
                                
                                break;
                            case enumTeachingPos.Site14_R_DT1:
                            case enumTeachingPos.Site15_R_DT2:
                            case enumTeachingPos.Site16_R_DT3:
                                DIO.WriteDOData(enumDONames.Y305_5_Rear_DT_1_Motor_Cw - (offset * 2), false);
                                DIO.WriteDOData(enumDONames.Y304_5_Rear_DT_1_Motor_Ccw - (offset * 2), false);
                                break;
                            case enumTeachingPos.Site17_R_DT4:
                            case enumTeachingPos.Site18_R_DT5:
                            case enumTeachingPos.Site19_R_DT6:
                            case enumTeachingPos.Site20_R_DT7:
                            case enumTeachingPos.Site21_R_DT8:
                            case enumTeachingPos.Site22_R_DT9:
                            case enumTeachingPos.Site23_R_DT10_FT4:
                                DIO.WriteDOData(enumDONames.Y407_4_Rear_DT_4_Motor_Cw - (offset * 2), false);
                                DIO.WriteDOData(enumDONames.Y407_6_Rear_DT_4_Motor_Ccw - (offset * 2), false);
                                break;
                            case enumTeachingPos.Site24_R_FT1:
                            case enumTeachingPos.Site25_R_FT2:
                            case enumTeachingPos.Site26_R_FT3:
                                DIO.WriteDOData(enumDONames.Y407_5_Rear_FT_1_Motor_Cw - (offset * 2), false);
                                DIO.WriteDOData(enumDONames.Y407_7_Rear_FT_1_Motor_Ccw - (offset * 2), false);
                                break;
                            default:
                                break;
                        }
                   
                        FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
                                                        DateTime.Now.ToString("HH:mm:ss"),
                                                        FuncInline.enumErrorPart.Site1_F_DT1 + offset,
                                                        enumErrorCode.Site_Open_Timeout,
                                                        false,
                                                        "Site ReOpen Timed out"));
                        return false;
                    }
                }
                //*/
                #endregion

                // 1) 컨베이어 정지
                if (StopConveyorIfRunning(site))
                {
                    // 정지 조치 했으면 여기서 한 턴 종료하거나 다음 사이클로
                }
                #region else if 핀 올려져 있고 언클램프 상태면 완료
                else if (TryGetContactUpDI(site, out var diUp) &&
                          DIO.GetDIData(diUp) &&
                          (!TryGetContactStopperDO(site, out var doStopper) ||    // STOPPER가 없으면 생략
                           !DIO.GetDORead(doStopper)))                           // STOPPER_SOL==false ⇒ 언클램프
                {
                    return true;
                }
                #endregion
                #region else if 핀 내려져 있으면 올리기
                else if (TryGetContactUpDI(site, out diUp) && !DIO.GetDIData(diUp))
                {
                    // 3) 내려가 있으면 올리기
                    if (TryGetContactUpDownDO(site, out var doUpDown))
                        DIO.WriteDOData(doUpDown, false); 
                }
                #endregion
                #region else if 클램핑 되어 있으면 언클램프
                else if (TryGetContactStopperDO(site, out doStopper) &&
                        DIO.GetDORead(doStopper))                 // STOPPER_SOL==true ⇒ 클램프 ON 상태
                {
                    DIO.WriteDOData(doStopper, false);             // 언클램프
                }
                #endregion

               
            }
            catch (Exception ex)
            {
                debug("SiteReOpen : " + ex.ToString());
                debug(ex.StackTrace);
            }

            return false;
        }


        
        // site에 해당하는 컨베이어 모터(CW/CCW) DO를 찾아준다.
        public static bool TryGetConveyorMotorPair(enumTeachingPos site, out enumDONames cw, out enumDONames ccw)
        {
            switch (site)
            {
                // ===== Front DT =====
                case enumTeachingPos.Site1_F_DT1: cw =enumDONames.Y403_4_Front_DT_1_Motor_Cw; ccw =enumDONames.Y403_6_Front_DT_1_Motor_Ccw; return true;
                case enumTeachingPos.Site2_F_DT2: cw =enumDONames.Y403_1_Front_DT_2_Motor_Cw; ccw =enumDONames.Y403_3_Front_DT_2_Motor_Ccw; return true;
                case enumTeachingPos.Site3_F_DT3: cw =enumDONames.Y402_4_Front_DT_3_Motor_Cw; ccw =enumDONames.Y402_6_Front_DT_3_Motor_Ccw; return true;
                case enumTeachingPos.Site4_F_DT4: cw =enumDONames.Y402_0_Front_DT_4_Motor_Cw; ccw =enumDONames.Y402_2_Front_DT_4_Motor_Ccw; return true;
                case enumTeachingPos.Site5_F_DT5: cw =enumDONames.Y401_5_Front_DT_5_Motor_Cw; ccw =enumDONames.Y401_7_Front_DT_5_Motor_Ccw; return true;
                case enumTeachingPos.Site6_F_DT6: cw =enumDONames.Y401_1_Front_DT_6_Motor_Cw; ccw =enumDONames.Y401_3_Front_DT_6_Motor_Ccw; return true;
                case enumTeachingPos.Site7_F_DT7: cw =enumDONames.Y400_4_Front_DT_7_Motor_Cw; ccw =enumDONames.Y400_6_Front_DT_7_Motor_Ccw; return true;
                case enumTeachingPos.Site8_F_DT8: cw =enumDONames.Y400_0_Front_DT_8_Motor_Cw; ccw =enumDONames.Y400_2_Front_DT_8_Motor_Ccw; return true;
                case enumTeachingPos.Site9_F_DT9: cw =enumDONames.Y403_5_Front_DT_9_Motor_Cw; ccw =enumDONames.Y403_7_Front_DT_9_Motor_Ccw; return true;
                case enumTeachingPos.Site10_F_DT10_FT4: cw =enumDONames.Y403_0_Front_DT_10_FT4_Motor_Cw; ccw =enumDONames.Y403_2_Front_DT_10_FT4_Motor_Ccw; return true;

                // ===== Front FT =====
                case enumTeachingPos.Site11_F_FT1: cw =enumDONames.Y402_1_Front_FT_1_Motor_Cw; ccw =enumDONames.Y402_3_Front_FT_1_Motor_Ccw; return true;
                case enumTeachingPos.Site12_F_FT2: cw =enumDONames.Y401_4_Front_FT_2_Motor_Cw; ccw =enumDONames.Y401_6_Front_FT_2_Motor_Ccw; return true;
                case enumTeachingPos.Site13_F_FT3: cw =enumDONames.Y401_0_Front_FT_3_Motor_Cw; ccw =enumDONames.Y401_2_Front_FT_3_Motor_Ccw; return true;

                // ===== Rear DT 1~3 (별도 어드레스군) =====
                case enumTeachingPos.Site14_R_DT1: cw =enumDONames.Y305_5_Rear_DT_1_Motor_Cw; ccw =enumDONames.Y304_5_Rear_DT_1_Motor_Ccw; return true;
                case enumTeachingPos.Site15_R_DT2: cw =enumDONames.Y305_6_Rear_DT_2_Motor_Cw; ccw =enumDONames.Y304_6_Rear_DT_2_Motor_Ccw; return true;
                case enumTeachingPos.Site16_R_DT3: cw =enumDONames.Y305_7_Rear_DT_3_Motor_Cw; ccw =enumDONames.Y304_7_Rear_DT_3_Motor_Ccw; return true;

                // ===== Rear DT 4~10 =====
                case enumTeachingPos.Site17_R_DT4: cw =enumDONames.Y407_4_Rear_DT_4_Motor_Cw; ccw =enumDONames.Y407_6_Rear_DT_4_Motor_Ccw; return true;
                case enumTeachingPos.Site18_R_DT5: cw =enumDONames.Y407_1_Rear_DT_5_Motor_Cw; ccw =enumDONames.Y407_3_Rear_DT_5_Motor_Ccw; return true;
                case enumTeachingPos.Site19_R_DT6: cw =enumDONames.Y406_4_Rear_DT_6_Motor_Cw; ccw =enumDONames.Y406_6_Rear_DT_6_Motor_Ccw; return true;
                case enumTeachingPos.Site20_R_DT7: cw =enumDONames.Y406_0_Rear_DT_7_Motor_Cw; ccw =enumDONames.Y406_2_Rear_DT_7_Motor_Ccw; return true;
                case enumTeachingPos.Site21_R_DT8: cw =enumDONames.Y405_5_Rear_DT_8_Motor_Cw; ccw =enumDONames.Y405_7_Rear_DT_8_Motor_Ccw; return true;
                case enumTeachingPos.Site22_R_DT9: cw =enumDONames.Y405_1_Rear_DT_9_Motor_Cw; ccw =enumDONames.Y405_3_Rear_DT_9_Motor_Ccw; return true;
                case enumTeachingPos.Site23_R_DT10_FT4: cw =enumDONames.Y404_4_Rear_DT_10_FT_4_Motor_Cw; ccw =enumDONames.Y404_6_Rear_DT_10_FT_4_Motor_Ccw; return true;

                // ===== Rear FT =====
                case enumTeachingPos.Site24_R_FT1: cw =enumDONames.Y407_5_Rear_FT_1_Motor_Cw; ccw =enumDONames.Y407_7_Rear_FT_1_Motor_Ccw; return true;
                case enumTeachingPos.Site25_R_FT2: cw =enumDONames.Y407_0_Rear_FT_2_Motor_Cw; ccw =enumDONames.Y407_2_Rear_FT_2_Motor_Ccw; return true;
                case enumTeachingPos.Site26_R_FT3: cw =enumDONames.Y406_5_Rear_FT_3_Motor_Cw; ccw =enumDONames.Y406_7_Rear_FT_3_Motor_Ccw; return true;
            }

            cw = default;
            ccw = default;
            return false;
        }

        // offset 없는 Stop 함수
        public static bool StopConveyorIfRunning(enumTeachingPos site)
        {
            if (!TryGetConveyorMotorPair(site, out var cw, out var ccw))
                return false;

            bool running = DIO.GetDORead(cw) || DIO.GetDORead(ccw);
            if (running)
            {
                DIO.WriteDOData(cw, false);
                DIO.WriteDOData(ccw, false);
            }
            return running;
        }

        // site별 Contact Up 센서(DI) 찾기
        public static bool TryGetContactUpDI(enumTeachingPos site, out enumDINames di)
        {
            switch (site)
            {
                // Front FT
                case enumTeachingPos.Site11_F_FT1: di = enumDINames.X400_5_Front_FT_1_Contact_Up_Sensor; return true;
                case enumTeachingPos.Site12_F_FT2: di = enumDINames.X400_3_Front_FT_2_Contact_Up_Sensor; return true;
                case enumTeachingPos.Site13_F_FT3: di = enumDINames.X400_1_Front_FT_3_Contact_Up_Sensor; return true;

                // Front DT
                case enumTeachingPos.Site10_F_DT10_FT4: di = enumDINames.X403_0_Front_DT_10_FT_4_Contact_Up_Sensor; return true;
                case enumTeachingPos.Site9_F_DT9: di = enumDINames.X402_7_Front_DT_9_Contact_Up_Sensor; return true;
                case enumTeachingPos.Site8_F_DT8: di = enumDINames.X402_5_Front_DT_8_Contact_Up_Sensor; return true;
                case enumTeachingPos.Site7_F_DT7: di = enumDINames.X402_3_Front_DT_7_Contact_Up_Sensor; return true;
                case enumTeachingPos.Site6_F_DT6: di = enumDINames.X402_1_Front_DT_6_Contact_Up_Sensor; return true;
                case enumTeachingPos.Site5_F_DT5: di = enumDINames.X401_6_Front_DT_5_Contact_Up_Sensor; return true;
                case enumTeachingPos.Site4_F_DT4: di = enumDINames.X401_4_Front_DT_4_Contact_Up_Sensor; return true;
                case enumTeachingPos.Site3_F_DT3: di = enumDINames.X401_2_Front_DT_3_Contact_Up_Sensor; return true;
                case enumTeachingPos.Site2_F_DT2: di = enumDINames.X401_0_Front_DT_2_Contact_Up_Sensor; return true;
                case enumTeachingPos.Site1_F_DT1: di = enumDINames.X400_7_Front_DT_1_Contact_Up_Sensor; return true;

                // Rear FT
                case enumTeachingPos.Site24_R_FT1: di = enumDINames.X406_1_Rear_FT_1_Contact_Up_Sensor; return true;
                case enumTeachingPos.Site25_R_FT2: di = enumDINames.X405_6_Rear_FT_2_Contact_Up_Sensor; return true;
                case enumTeachingPos.Site26_R_FT3: di = enumDINames.X405_4_Rear_FT_3_Contact_Up_Sensor; return true;

                // Rear DT
                case enumTeachingPos.Site23_R_DT10_FT4: di = enumDINames.X404_4_Rear_DT_10_FT_4_Contact_Up_Sensor; return true;
                case enumTeachingPos.Site22_R_DT9: di = enumDINames.X404_2_Rear_DT_9_Contact_Up_Sensor; return true;
                case enumTeachingPos.Site21_R_DT8: di = enumDINames.X404_0_Rear_DT_8_Contact_Up_Sensor; return true;
                case enumTeachingPos.Site20_R_DT7: di = enumDINames.X407_7_Rear_DT_7_Contact_Up_Sensor; return true;
                case enumTeachingPos.Site19_R_DT6: di = enumDINames.X407_5_Rear_DT_6_Contact_Up_Sensor; return true;
                case enumTeachingPos.Site18_R_DT5: di = enumDINames.X407_2_Rear_DT_5_Contact_Up_Sensor; return true;
                case enumTeachingPos.Site17_R_DT4: di = enumDINames.X407_0_Rear_DT_4_Contact_Up_Sensor; return true;
                case enumTeachingPos.Site16_R_DT3: di = enumDINames.X406_7_Rear_DT_3_Contact_Up_Sensor; return true;
                case enumTeachingPos.Site15_R_DT2: di = enumDINames.X406_5_Rear_DT_2_Contact_Up_Sensor; return true;
                case enumTeachingPos.Site14_R_DT1: di = enumDINames.X406_3_Rear_DT_1_Contact_Up_Sensor; return true;
            }
            di = default;
            return false;
        }

        public static bool TryGetContactStopperDO(enumTeachingPos site, out enumDONames stopper)
        {
            switch (site)
            {
                // Front FT
                case enumTeachingPos.Site11_F_FT1: stopper =enumDONames.Y2_5_Front_FT_1_CONTACT_STOPPER_SOL; return true;
                case enumTeachingPos.Site12_F_FT2: stopper =enumDONames.Y0_6_Front_FT_2_CONTACT_STOPPER_SOL; return true;
                case enumTeachingPos.Site13_F_FT3: stopper =enumDONames.Y2_6_Front_FT_3_CONTACT_STOPPER_SOL; return true;

                // Front DT
                case enumTeachingPos.Site10_F_DT10_FT4: stopper =enumDONames.Y0_5_Front_DT_10_FT4_CONTACT_STOPPER_SOL; return true;
                case enumTeachingPos.Site9_F_DT9: stopper =enumDONames.Y2_4_Front_DT_9_CONTACT_STOPPER_SOL; return true;
                case enumTeachingPos.Site8_F_DT8: stopper =enumDONames.Y0_4_Front_DT_8_CONTACT_STOPPER_SOL; return true;
                case enumTeachingPos.Site7_F_DT7: stopper =enumDONames.Y2_3_Front_DT_7_CONTACT_STOPPER_SOL; return true;
                case enumTeachingPos.Site6_F_DT6: stopper =enumDONames.Y0_3_Front_DT_6_CONTACT_STOPPER_SOL; return true;
                case enumTeachingPos.Site5_F_DT5: stopper =enumDONames.Y2_2_Front_DT_5_CONTACT_STOPPER_SOL; return true;
                case enumTeachingPos.Site4_F_DT4: stopper =enumDONames.Y0_2_Front_DT_4_CONTACT_STOPPER_SOL; return true;
                case enumTeachingPos.Site3_F_DT3: stopper =enumDONames.Y2_1_Front_DT_3_CONTACT_STOPPER_SOL; return true;
                case enumTeachingPos.Site2_F_DT2: stopper =enumDONames.Y0_1_Front_DT_2_CONTACT_STOPPER_SOL; return true;
                case enumTeachingPos.Site1_F_DT1: stopper =enumDONames.Y2_0_Front_DT_1_CONTACT_STOPPER_SOL; return true;

                // Rear FT
                case enumTeachingPos.Site26_R_FT3: stopper =enumDONames.Y303_1_Rear_FT_3_CONTACT_STOPPER_SOL; return true;
                case enumTeachingPos.Site25_R_FT2: stopper =enumDONames.Y301_1_Rear_FT_2_CONTACT_STOPPER_SOL; return true;
                case enumTeachingPos.Site24_R_FT1: stopper =enumDONames.Y303_0_Rear_FT_1_CONTACT_STOPPER_SOL; return true;

                // Rear DT
                case enumTeachingPos.Site23_R_DT10_FT4: stopper =enumDONames.Y301_0_Rear_DT_10_FT_4_CONTACT_STOPPER_SOL; return true;
                case enumTeachingPos.Site22_R_DT9: stopper =enumDONames.Y302_7_Rear_DT_9_CONTACT_STOPPER_SOL; return true;
                case enumTeachingPos.Site21_R_DT8: stopper =enumDONames.Y300_7_Rear_DT_8_CONTACT_STOPPER_SOL; return true;
                case enumTeachingPos.Site20_R_DT7: stopper =enumDONames.Y302_6_Rear_DT_7_CONTACT_STOPPER_SOL; return true;
                case enumTeachingPos.Site19_R_DT6: stopper =enumDONames.Y300_6_Rear_DT_6_CONTACT_STOPPER_SOL; return true;
                case enumTeachingPos.Site18_R_DT5: stopper =enumDONames.Y302_5_Rear_DT_5_CONTACT_STOPPER_SOL; return true;
                case enumTeachingPos.Site17_R_DT4: stopper =enumDONames.Y300_5_Rear_DT_4_CONTACT_STOPPER_SOL; return true;
                case enumTeachingPos.Site16_R_DT3: stopper =enumDONames.Y302_4_Rear_DT_3_CONTACT_STOPPER_SOL; return true;
                case enumTeachingPos.Site15_R_DT2: stopper =enumDONames.Y300_4_Rear_DT_2_CONTACT_STOPPER_SOL; return true;
                case enumTeachingPos.Site14_R_DT1: stopper =enumDONames.Y302_3_Rear_DT_1_CONTACT_STOPPER_SOL; return true;
            }
            stopper = default;
            return false;
        }


        // site별 Contact Up/Down 솔레노이드(DO) 찾기
        public static bool TryGetContactUpDownDO(enumTeachingPos site, out enumDONames updown)
        {
            switch (site)
            {
                // Front FT
                case enumTeachingPos.Site11_F_FT1: updown =enumDONames.Y3_4_Front_FT_1_CONTACT_UP_DOWN_SOL; return true;
                case enumTeachingPos.Site12_F_FT2: updown =enumDONames.Y1_5_Front_FT_2_CONTACT_UP_DOWN_SOL; return true;
                case enumTeachingPos.Site13_F_FT3: updown =enumDONames.Y3_5_Front_FT_3_CONTACT_UP_DOWN_SOL; return true;

                // Front DT
                case enumTeachingPos.Site10_F_DT10_FT4: updown =enumDONames.Y1_4_Front_DT_10_FT_4_CONTACT_UP_DOWN_SOL; return true;
                case enumTeachingPos.Site9_F_DT9: updown =enumDONames.Y3_3_Front_DT_9_CONTACT_UP_DOWN_SOL; return true;
                case enumTeachingPos.Site8_F_DT8: updown =enumDONames.Y1_3_Front_DT_8_CONTACT_UP_DOWN_SOL; return true;
                case enumTeachingPos.Site7_F_DT7: updown =enumDONames.Y3_2_Front_DT_7_CONTACT_UP_DOWN_SOL; return true;
                case enumTeachingPos.Site6_F_DT6: updown =enumDONames.Y1_2_Front_DT_6_CONTACT_UP_DOWN_SOL; return true;
                case enumTeachingPos.Site5_F_DT5: updown =enumDONames.Y3_1_Front_DT_5_CONTACT_UP_DOWN_SOL; return true;
                case enumTeachingPos.Site4_F_DT4: updown =enumDONames.Y1_1_Front_DT_4_CONTACT_UP_DOWN_SOL; return true;
                case enumTeachingPos.Site3_F_DT3: updown =enumDONames.Y3_0_Front_DT_3_CONTACT_UP_DOWN_SOL; return true;
                case enumTeachingPos.Site2_F_DT2: updown =enumDONames.Y1_0_Front_DT_2_CONTACT_UP_DOWN_SOL; return true;
                case enumTeachingPos.Site1_F_DT1: updown =enumDONames.Y2_7_Front_DT_1_CONTACT_UP_DOWN_SOL; return true;

                // Rear FT 
                case enumTeachingPos.Site24_R_FT1: updown =enumDONames.Y301_7_Rear_FT_1_CONTACT_UP_DOWN_SOL; return true;
                case enumTeachingPos.Site25_R_FT2: updown =enumDONames.Y303_7_Rear_FT_2_CONTACT_UP_DOWN_SOL; return true;
                case enumTeachingPos.Site26_R_FT3: updown =enumDONames.Y0_0_Rear_FT_3_CONTACT_UP_DOWN_SOL; return true;

                // Rear DT
                case enumTeachingPos.Site23_R_DT10_FT4: updown =enumDONames.Y303_6_Rear_DT_10_FT_3_CONTACT_UP_DOWN_SOL; return true;
                case enumTeachingPos.Site22_R_DT9: updown =enumDONames.Y301_6_Rear_DT_9_CONTACT_UP_DOWN_SOL; return true;
                case enumTeachingPos.Site21_R_DT8: updown =enumDONames.Y303_5_Rear_DT_8_CONTACT_UP_DOWN_SOL; return true;
                case enumTeachingPos.Site20_R_DT7: updown =enumDONames.Y301_5_Rear_DT_7_CONTACT_UP_DOWN_SOL; return true;
                case enumTeachingPos.Site19_R_DT6: updown =enumDONames.Y303_4_Rear_DT_6_CONTACT_UP_DOWN_SOL; return true;
                case enumTeachingPos.Site18_R_DT5: updown =enumDONames.Y301_4_Rear_DT_5_CONTACT_UP_DOWN_SOL; return true;
                case enumTeachingPos.Site17_R_DT4: updown =enumDONames.Y303_3_Rear_DT_4_CONTACT_UP_DOWN_SOL; return true;
                case enumTeachingPos.Site16_R_DT3: updown =enumDONames.Y301_3_Rear_DT_3_CONTACT_UP_DOWN_SOL; return true;
                case enumTeachingPos.Site15_R_DT2: updown =enumDONames.Y303_2_Rear_DT_2_CONTACT_UP_DOWN_SOL; return true;
                case enumTeachingPos.Site14_R_DT1: updown =enumDONames.Y301_2_Rear_DT_1_CONTACT_UP_DOWN_SOL; return true;
            }
            updown = default;
            return false;
        }
        public static bool SiteUnclamp(enumTeachingPos site) // 검사 끝난 사이트 언클램프/핀업
        {
            try
            {
                // 아직 검사 진행/대기면 아무 것도 하지 않음
                if (FuncInline.PCBInfo[(int)site].PCBStatus <= FuncInline.enumSMDStatus.Testing)
                    return false;

                // 1) 현재 상태 읽기
                bool upOk = false;
                if (TryGetContactUpDI(site, out var diUp))
                    upOk = DIO.GetDIData(diUp);                 // 컨택트 UP 센서 (DI)

                bool unclampOk = true;                          // STOPPER 없으면 생략(=언클램프 OK로 간주)
                if (TryGetContactStopperDO(site, out var doStopper))
                    unclampOk = !DIO.GetDORead(doStopper);      // STOPPER DO==true 면 클램프 상태

                // 둘 다 만족하면 완료
                if (upOk && unclampOk)
                    return true;

                // 2) 액션: 핀(컨택트) 올리기
                if (!upOk && TryGetContactUpDownDO(site, out var doUpDown))
                {
                   
                    DIO.WriteDOData(doUpDown, false);
                }

                // 3) 액션: 클램프 해제(언클램프)
                if (!unclampOk && TryGetContactStopperDO(site, out doStopper))
                {
                    // STOPPER 솔 OFF = 언클램프
                    DIO.WriteDOData(doStopper, false);
                }

                // 아직 목표상태가 아니면 false (다음 사이클에서 재평가)
                return false;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
                return false;
            }
        }


      

        #endregion

       


        #region 타임아웃 관련
        public static void StopAllWatch() // 전체 타임아웃 체크 타이머 정지
        {
            //debug("StopAllWatch");
            //GlobalVar.BufferConveyorActionWatch.Stop();
            //GlobalVar.CoolingConveyorActionWatch.Stop();
            //GlobalVar.InputLiftActionWatch.Stop();
            //GlobalVar.LoadingConveyorActionWatch.Stop();
            //GlobalVar.OutputLiftActionWatch.Stop();
            //GlobalVar.RobotActionWatch.Stop();
            //GlobalVar.UnloadingConveyorActionWatch.Stop();
            for (int i = 0; i < FuncInline.SiteCheckTime.Length; i++)
            {
                FuncInline.SiteCheckTime[i].Stop();
            }
        }

        public static void ResetAllWatch() // 전체 타임아웃 체크 변수 초기화
        {
            //debug("ResetAllWatch");
            //GlobalVar.BufferConveyorActionWatch.Reset();
            //GlobalVar.CoolingConveyorActionWatch.Reset();
            //GlobalVar.InputLiftActionWatch.Reset();
            //GlobalVar.LoadingConveyorActionWatch.Reset();
            //GlobalVar.OutputLiftActionWatch.Reset();
            //GlobalVar.RobotActionWatch.Reset();
            //GlobalVar.UnloadingConveyorActionWatch.Reset();
            for (int i = 0; i < FuncInline.SiteCheckTime.Length; i++)
            {
                FuncInline.SiteCheckTime[i].Reset();
            }
        }

        public static void RestartAllWatch() // 전체 타임아웃 체크 타이머 재시작
        {
            //debug("RestartAllWatch");
            //GlobalVar.BufferConveyorActionWatch.Restart();
            //GlobalVar.CoolingConveyorActionWatch.Restart();
            //GlobalVar.InputLiftActionWatch.Restart();
            //GlobalVar.LoadingConveyorActionWatch.Restart();
            //GlobalVar.OutputLiftActionWatch.Restart();
            //GlobalVar.RobotActionWatch.Restart();
            //GlobalVar.UnloadingConveyorActionWatch.Restart();
            for (int i = 0; i < FuncInline.SiteCheckTime.Length; i++)
            {
                Util.StartWatch(ref FuncInline.SiteCheckTime[i]);
            }
        }

        #endregion

        #region 컨베어 운영 관련
        public static void StopAllConveyor()
        {
            // 새 IO 리스트 기준: 모든 컨베이어/리프트/패스라인/셔틀 모터 OFF
            var outs = new enumDONames[]
            {
        // ===== Front DT (1~10/FT4) =====
        enumDONames.Y403_4_Front_DT_1_Motor_Cw,      enumDONames.Y403_6_Front_DT_1_Motor_Ccw,
        enumDONames.Y403_1_Front_DT_2_Motor_Cw,      enumDONames.Y403_3_Front_DT_2_Motor_Ccw,
        enumDONames.Y402_4_Front_DT_3_Motor_Cw,      enumDONames.Y402_6_Front_DT_3_Motor_Ccw,
        enumDONames.Y402_0_Front_DT_4_Motor_Cw,      enumDONames.Y402_2_Front_DT_4_Motor_Ccw,
        enumDONames.Y401_5_Front_DT_5_Motor_Cw,      enumDONames.Y401_7_Front_DT_5_Motor_Ccw,
        enumDONames.Y401_1_Front_DT_6_Motor_Cw,      enumDONames.Y401_3_Front_DT_6_Motor_Ccw,
        enumDONames.Y400_4_Front_DT_7_Motor_Cw,      enumDONames.Y400_6_Front_DT_7_Motor_Ccw,
        enumDONames.Y400_0_Front_DT_8_Motor_Cw,      enumDONames.Y400_2_Front_DT_8_Motor_Ccw,
        enumDONames.Y403_5_Front_DT_9_Motor_Cw,      enumDONames.Y403_7_Front_DT_9_Motor_Ccw,
        enumDONames.Y403_0_Front_DT_10_FT4_Motor_Cw, enumDONames.Y403_2_Front_DT_10_FT4_Motor_Ccw,

        // ===== Front FT (1~3) =====
        enumDONames.Y402_1_Front_FT_1_Motor_Cw,      enumDONames.Y402_3_Front_FT_1_Motor_Ccw,
        enumDONames.Y401_4_Front_FT_2_Motor_Cw,      enumDONames.Y401_6_Front_FT_2_Motor_Ccw,
        enumDONames.Y401_0_Front_FT_3_Motor_Cw,      enumDONames.Y401_2_Front_FT_3_Motor_Ccw,

        // ===== Rear DT (1~3: 별도 어드레스군) =====
        enumDONames.Y305_5_Rear_DT_1_Motor_Cw,       enumDONames.Y304_5_Rear_DT_1_Motor_Ccw,
        enumDONames.Y305_6_Rear_DT_2_Motor_Cw,       enumDONames.Y304_6_Rear_DT_2_Motor_Ccw,
        enumDONames.Y305_7_Rear_DT_3_Motor_Cw,       enumDONames.Y304_7_Rear_DT_3_Motor_Ccw,

        // ===== Rear DT (4~10/FT4) =====
        enumDONames.Y407_4_Rear_DT_4_Motor_Cw,       enumDONames.Y407_6_Rear_DT_4_Motor_Ccw,
        enumDONames.Y407_1_Rear_DT_5_Motor_Cw,       enumDONames.Y407_3_Rear_DT_5_Motor_Ccw,
        enumDONames.Y406_4_Rear_DT_6_Motor_Cw,       enumDONames.Y406_6_Rear_DT_6_Motor_Ccw,
        enumDONames.Y406_0_Rear_DT_7_Motor_Cw,       enumDONames.Y406_2_Rear_DT_7_Motor_Ccw,
        enumDONames.Y405_5_Rear_DT_8_Motor_Cw,       enumDONames.Y405_7_Rear_DT_8_Motor_Ccw,
        enumDONames.Y405_1_Rear_DT_9_Motor_Cw,       enumDONames.Y405_3_Rear_DT_9_Motor_Ccw,
        enumDONames.Y404_4_Rear_DT_10_FT_4_Motor_Cw, enumDONames.Y404_6_Rear_DT_10_FT_4_Motor_Ccw,

        // ===== Rear FT (1~3) =====
        enumDONames.Y407_5_Rear_FT_1_Motor_Cw,       enumDONames.Y407_7_Rear_FT_1_Motor_Ccw,
        enumDONames.Y407_0_Rear_FT_2_Motor_Cw,       enumDONames.Y407_2_Rear_FT_2_Motor_Ccw,
        enumDONames.Y406_5_Rear_FT_3_Motor_Cw,       enumDONames.Y406_7_Rear_FT_3_Motor_Ccw,

        // ===== 패스라인/인아웃/셔틀/NG 라인 =====
        enumDONames.Y404_1_Front_Passline_Motor_Cw,      // (단일방향)
        enumDONames.Y305_4_Rear_PassLine_Motor_Cw,       // (단일방향)
        enumDONames.Y400_1_Out_Conveyor_Motor_Cw,        // Out conveyor
        enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw, // In conveyor
        enumDONames.Y400_3_In_Shuttle_Motor_Ccw,         // In shuttle ccw
        enumDONames.Y404_0_Rear_NgLine_Motor_Cw,         // Rear NG line
        enumDONames.Y402_5_Out_Conveyor_Ng_Motor_Cw,     // Out NG conveyor
        enumDONames.Y304_0_Out_Shuttle_Ok_Motor_Cw,      // Out shuttle OK
        enumDONames.Y400_5_Out_Shuttle_Ng_Motor_Cw,      // Out shuttle NG (cw)
        enumDONames.Y400_7_Out_Shuttle_Ng_Motor_Ccw,     // Out shuttle NG (ccw)
        enumDONames.Y402_7_Out_Shuttle_Ok_Motor_Ccw,     // Out shuttle OK (ccw)

        // ===== 리프트(Front/Rear Up & Down) =====
        enumDONames.Y405_0_Front_Lift_Up_Motor_Cw,   enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw,
        enumDONames.Y405_4_Front_Lift_Down_Motor_Cw, enumDONames.Y405_6_Front_Lift_Down_Motor_Ccw,
        enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw,    enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw,
        enumDONames.Y305_1_Rear_Lift_Down_Motor_Cw,  enumDONames.Y304_1_Rear_Lift_Down_Motor_Ccw,
            };

            foreach (var o in outs)
                DIO.WriteDOData(o, false);
        }


        public static bool MoveNGSylinder(FuncInline.enumNGAction action) // 인터락 처리된 NG 실린더 동작
        {


            bool forward = action == FuncInline.enumNGAction.Backward ? false : true;
            bool on = action == FuncInline.enumNGAction.Run ? true : false;
            switch (action)
            {
                case FuncInline.enumNGAction.Run:
                case FuncInline.enumNGAction.Stop:
                    DIO.WriteDOData(enumDONames.Y402_5_Out_Conveyor_Ng_Motor_Cw, on);
                    return DIO.GetDORead(enumDONames.Y402_5_Out_Conveyor_Ng_Motor_Cw) == on;
                case FuncInline.enumNGAction.Forward:
                case FuncInline.enumNGAction.Backward:
                    if (action == FuncInline.enumNGAction.Backward &&
                        InlineType == enumInlineType.Gen5)
                    {
                        DIO.DoubleSol(enumDONames.Y412_6_Ngbuffer_Upper_cylinder_forward, forward);
                    
                        if (DIO.GetDIData(FuncInline.enumDINames.X03_2_NgBuffer_UpperBackwardSensor))
                        {
                            DIO.DoubleSol(enumDONames.Y412_7_NgBuffer_Lower_cylinder_forward, forward);
                        }

                    }
                    if (action == FuncInline.enumNGAction.Forward &&
                        InlineType == enumInlineType.Gen5)
                    {
                        DIO.DoubleSol(enumDONames.Y412_6_Ngbuffer_Upper_cylinder_forward, forward);

                        if (DIO.GetDIData(FuncInline.enumDINames.X03_1_NgBuffer_UpperForwardSensor))
                        {
                            DIO.DoubleSol(enumDONames.Y412_7_NgBuffer_Lower_cylinder_forward, forward);
                        }

                        return DIO.GetDIData(enumDINames.X03_1_NgBuffer_UpperForwardSensor) &&
                                DIO.GetDIData(enumDINames.X03_5_NgBuffer_LowerForwardSensor);
                    }
                    return DIO.GetDIData(enumDINames.X03_2_NgBuffer_UpperBackwardSensor) &&
                            DIO.GetDIData(enumDINames.X03_6_NgBuffer_LowerBackwardSensor);
            }
            return false;
        }

        #endregion


        public static void ResetInOutTime() // InputStop/OutputStop 체크 시간을 초기화한다.
        {
            if (GlobalVar.SystemStatus == enumSystemStatus.InputStop ||
                GlobalVar.SystemStatus == enumSystemStatus.OutputStop)
            {
                GlobalVar.SystemStatus = enumSystemStatus.AutoRun;
            }
            GlobalVar.InputTime = GlobalVar.TickCount64;
            GlobalVar.OutputTime = GlobalVar.TickCount64;
        }

        public static void LoadArrayImage()
        {
            string imagePath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.ArrayImagePath + "\\" + GlobalVar.ModelName + ".bmp";
            FuncInline.ArrayBitmap = FuncScreen.LoadBitmap(imagePath);
        }

        public static void MakeArrayImage() // 수동으로 이미지 지정해서 현재 모델의 실사 이미지를 생성한다.
        {
            try
            {
                FuncInline.ArrayBitmap = null;

                int originStartX = (int)FuncInline.ScanSize.x; // 원본 이미지에서 PCB 유효영역의 시작점 X
                int originStartY = (int)FuncInline.ScanSize.y; // 원본 이미지에서 PCB 유효영역의 시작점 Y
                int originEndX = (int)FuncInline.ScanSize.z + originStartX; // 원본 이미지에서 PCB 유효영역의 끝점 X
                int originEndY = (int)FuncInline.ScanSize.a + originStartY; // 원본 이미지에서 PCB 유효영역의 끝점 Y
                int maxPCBWidth = (int)FuncInline.DefaultPCBWidth; // PCB의 최대 넓이
                int maxPCBLength = (int)FuncInline.DefaultPCBLength; // PCB의 최대 길이
                double modelWidth = FuncInline.PCBWidth; // 해당 모델의 폭
                double modelLength = FuncInline.PCBLength; // 해당 모델의 길이
                int zoom = 20;

                string defaultPath = FuncInline.ScanImagePath; //"D:\\TestTempImage";
                                                              //string imagePath = "";
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Title = "Select Full Size image";
                //dlg.Filter = "*.*";
                dlg.InitialDirectory = defaultPath;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    // zoom 먼저하면 계산이 어려워지므로 먼저 자르고 다음으로 zoom해서 저장한다. 메모리 문제로 실패
                    // 알고 보니 메모리 문제가 아니라 clone시 범위를 벗어나면 out of memory exception이 발생한다.
                    Bitmap image = FuncScreen.LoadBitmap(dlg.FileName);// ("C:\\FA\\AutoInline\\ArrayImage\\SM-S901B_Big.bmp");
                    Size resize = new Size(image.Width / zoom, image.Height / zoom);
                    Bitmap resizeImage = new Bitmap(image, resize);
                    //resizeImage.Save("C:\\FA\\AutoInline\\ArrayImage\\resize.bmp");
                    image.Dispose();
                    int cloneStartX = Math.Max(0, (int)(originStartX + (maxPCBLength - modelLength) * (originEndX - originStartX) / maxPCBLength) / zoom);
                    int cloneStartY = Math.Max(0, (int)(originStartY + (maxPCBWidth - modelWidth) * (originEndY - originStartY) / maxPCBWidth) / zoom);
                    int cloneEndX = Math.Min(originEndX / zoom, resizeImage.Size.Width - 1);
                    int cloneEndY = Math.Min(originEndY / zoom, resizeImage.Size.Height - 1);
                    /*
                    int totalWidth = rect.Left + rect.Width; //think -the same as Right property

                    int allowableWidth = localImage.Width - rect.Left;
                    int finalWidth = 0;

                    if (totalWidth > allowableWidth){
                       finalWidth = allowableWidth;
                    } else {
                       finalWidth = totalWidth;
                    }

                    rect.Width = finalWidth;

                    int totalHeight = rect.Top + rect.Height; //think same as Bottom property
                    int allowableHeight = localImage.Height - rect.Top;
                    int finalHeight = 0;

                    if (totalHeight > allowableHeight){
                       finalHeight = allowableHeight;
                    } else {
                       finalHeight = totalHeight;
                    }

                    rect.Height = finalHeight;
                    cropped = ((Bitmap)localImage).Clone(rect,    System.Drawing.Imaging.PixelFormat.DontCare);
                    //*/
                    //cloneStartX = 0;
                    //cloneStartY = 0;
                    //cloneEndX = resizeImage.Size.Width - 1;
                    //cloneEndY = resizeImage.Size.Height - 1;

                    Bitmap cloneImage = resizeImage.Clone(
                        new Rectangle(cloneStartX,
                            cloneStartY,
                            cloneEndX - cloneStartX,
                            cloneEndY - cloneStartY),
                        System.Drawing.Imaging.PixelFormat.DontCare);
                    resizeImage.Dispose();
                    string savePath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\ArrayImage\\" + GlobalVar.ModelName + ".bmp";
                    //if (File.Exists(savePath))
                    //{
                    //    File.Delete(savePath);
                    //}

                    cloneImage.Save(savePath);

                    cloneImage.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
        }

        public static bool CheckCNDuplicationNG() // 장비 내 PCB 간에 CN 중복 여부 체크. 
        {
            // MovePCBInfo시 이전중인 건 어떻게 하지?
            // MovePCBInfo시 출발지/도착지 플래그 켜고, Clear 할 때 끄면 되나? Clear때 도착지는 어떻게 알지?
            // Flag를 enumTeachingPos 로 해서 Clear시 상대 위치 플래그도 같이 끌 수 있다.
            // Flag가 RobotWait면 중복판단 배제
            int fromStart = (int)enumTeachingPos.Site1_F_DT1;
            int fromEnd = (int)enumTeachingPos.Site26_R_FT3;
            int toStart = (int)enumTeachingPos.InConveyor;
            int toEnd = (int)enumTeachingPos.OutConveyor;
            for (int i = fromStart; i <= fromEnd; i++)
            {
                for (int j = toStart; j <= toEnd; j++)
                {
                    // 같은 파트끼리 비교는 배제
                    if (i == j)
                    {
                        continue;
                    }

                    if (FuncInline.PCBInfo[i].PCBStatus != FuncInline.enumSMDStatus.UnKnown && // i PCB 정보 있고
                        FuncInline.PCBInfoMoveFlag[i] != enumTeachingPos.None && // i 정보 이전중 아니고
                        FuncInline.PCBInfo[j].PCBStatus != FuncInline.enumSMDStatus.UnKnown && // j PCB 정보 있고
                        FuncInline.PCBInfoMoveFlag[j] != enumTeachingPos.None) // j 정보 이전중 아니고
                    {
                        // 전체 어레이 CN 대조
                        for (int m = 0; m < FuncInline.PCBInfo[i].Barcode.Length; m++)
                        {
                            for (int n = 0; n < FuncInline.PCBInfo[j].Barcode.Length; n++)
                            {
                                if (FuncInline.PCBInfo[i].Barcode[m].Length > 0 &&
                                    FuncInline.PCBInfo[j].Barcode[n].Length > 0 &&
                                    FuncInline.PCBInfo[i].Barcode[m] == FuncInline.PCBInfo[j].Barcode[n])
                                {
                                    FuncInline.DuplicatedPos[0] = (enumTeachingPos)i;
                                    FuncInline.DuplicatedPos[1] = (enumTeachingPos)j;
                                    FuncLog.WriteLog(FuncInline.PCBInfo[i].Barcode[m] + "DUPLICATED. " + ((enumTeachingPos)i).ToString() + " & " + ((enumTeachingPos)j).ToString());
                                    return true; // 중복 확인됨. 이후 에러처리 위해 중복 위치들 리턴해야 하지 않나?
                                }
                            }
                        }
                    }
                }
            }

            FuncInline.DuplicatedPos[0] = enumTeachingPos.None;
            FuncInline.DuplicatedPos[1] = enumTeachingPos.None;
            return false;
        }


        public static void WritePinLog() // 핀 사용 로그 저장
        {
            try
            {
                WritePinLog_Samsung();  // jhryu 삼성요구포멧 저장

            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        public static void WriteSystemLog() // 시스템 사용 로그 저장
        {
            try
            {
                WriteSystemLog_Samsung();  // jhryu 삼성요구포멧 저장

            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }



        // JHRYU 20230223 기능없어서 만듦
        public static string[,] GetSiteInputCount() // 사이트별 인풋 카운트
        {
            if (!GlobalVar.UseMsSQL)
            {
                return null;
            }

            try
            {
                string sql = "select distinct Site, MAX(TestCount) OVER (PARTITION BY Site)  from PinLog where Using='1' order by Site";
                return GlobalVar.Sql.Read(sql);
            }
            catch (Exception ex)
            {
                debug("GetSiteInputCount : " + ex.ToString());
                debug(ex.StackTrace);
            }
            return null;
        }

        public static string[,] GetSiteNGCount() // 사이트별 NG 카운트
        {
            if (!GlobalVar.UseMsSQL)
            {
                return null;
            }

            try
            {
                string sql = "select distinct Site, MAX(NGCount) OVER (PARTITION BY Site)  from PinLog where Using='1'order by Site";
                return GlobalVar.Sql.Read(sql);
            }
            catch (Exception ex)
            {
                debug("GetSiteInputCount : " + ex.ToString());
                debug(ex.StackTrace);
            }
            return null;
        }

        public static int GetPinNo(int arrayNo)
        {
            for (int i = 0; i < FuncInline.PinArray.Length; i++)
            {
                if (FuncInline.PinArray[i] == arrayNo)
                {
                    return i + 1;
                }
            }
            return 0;
        }

        public static void WriteSystemLog_Samsung() // jhryu 20230220 삼성요구
        {
            string logPath = "";
            try
            {
                string fileName = SystemLogFileName;
                if (!Directory.Exists(FuncInline.SystemLogPath))
                {
                    FuncFile.CreateDirectory(FuncInline.SystemLogPath);
                }
                logPath = FuncInline.SystemLogPath + "\\" + fileName;

                // 저장 시작

                //string key = "";
                string value = "";
                string section = "";
                string NewLine = "\n";
                if (File.Exists(logPath))
                {
                    NewLine = "";
                }
             
                section = "System Data";
                FuncFile.WriteIniFile(section, "Model Name", GlobalVar.ModelName, logPath);
                value = (FuncInline.OtherRetest) ? "1" : "0";
                FuncFile.WriteIniFile(section, "OtherSiteRetest", value, logPath);
                value = OtherRetestCount.ToString();
                FuncFile.WriteIniFile(section, "Test Retry Count", value, logPath);
                value = BadMarkToNG.ToString();
                FuncFile.WriteIniFile(section, "BadMarkToNG", value, logPath);
                value = PassToNG ? "1" :
                        NGToUnloading ? "2" :
                        "0";
                FuncFile.WriteIniFile(section, "UnloadOp", value, logPath);
                FuncFile.WriteIniFile(section, "CVFixedPosition", "0", logPath);
                FuncFile.WriteIniFile(section, "FlowDirection", "0" + NewLine, logPath);


                section = "Time Data";
                value = "90";       // 초기화버튼시 현재 90초까지로 상수 설정되어있다.
                FuncFile.WriteIniFile(section, "OriginLimitTime", value, logPath);
                //value = RobotTimeout.ToString();
                value = "30";
                FuncFile.WriteIniFile(section, "MotionMoveLimitTime", value, logPath);
                value = GlobalVar.NormalTimeout.ToString();
                FuncFile.WriteIniFile(section, "CylinderMoveLimitTime", value, logPath);
                value = TestTimeout.ToString();
                FuncFile.WriteIniFile(section, "TestLimitTime", value, logPath);
                value = TestCycleTime.ToString();
                FuncFile.WriteIniFile(section, "CycleTime", value + NewLine, logPath);

                int maxModuleCount = UseSite.Length;   //21
                int maxArrayCount = MaxArrayCount;     //12

                section = "Product Management";
                FuncFile.WriteIniFile(section, "ProductCount", "0" + NewLine, logPath);
             

                string[,] rs = GetSiteInputCount();

                for (int i = 0; i < rs.GetLength(0); i++)
                {
                    string siteNo = rs[i, 0];
                    value = rs[i, 1];

                    FuncFile.WriteIniFile(section, "Module" + siteNo + " Input Count", value, logPath);
                }

                rs = GetSiteNGCount();
                for (int i = 0; i < rs.GetLength(0); i++)
                {
                    string siteNo = rs[i, 0];
                    value = rs[i, 1];

                    FuncFile.WriteIniFile(section, "Module" + siteNo + " NG Count", value, logPath);
                    //if (i + 1 < rs.GetLength(0))
                    //{
                    //    FuncFile.WriteIniFile(section, "Module" + siteNo + " NG Count", value, logPath);
                    //}
                    //else
                    //{
                    //    FuncFile.WriteIniFile(section, "Module" + siteNo + " NG Count", value + NewLine, logPath);
                    //}
                }

                for (int i = 0; i < maxModuleCount; i++)
                {
                    value = (FuncInline.UseSite[i]) ? "1" : "0";
                    FuncFile.WriteIniFile(section, "Module" + (i + 1) + "_Used", value + (i == maxModuleCount - 1 ? NewLine : ""), logPath);
                }

                //int ngBuffCount = 0;
                //if (FuncInline.PCBInfo[(int)enumTeachingPos.NgShuttle1].PCBStatus != FuncInline.enumSMDStatus.UnKnown) ngBuffCount++;
                //if (FuncInline.PCBInfo[(int)enumTeachingPos.NgShuttle2].PCBStatus != FuncInline.enumSMDStatus.UnKnown) ngBuffCount++;
                //value = ngBuffCount.ToString();
                //FuncFile.WriteIniFile(section, "NG Buffer Count", value, logPath);

                //for (int i = 0, cnt = 0; i < maxModuleCount; i++)
                //{
                //    if (FuncInline.UseSite[i]) cnt++;
                //    value = cnt.ToString();
                //}
                //FuncFile.WriteIniFile(section, "Module Use Count", value, logPath);


                //// 테스트 타임은 글로벌에 변수 만들어서 모듈마다 테스트가 끝날때마다 시간을 업데이트하고 그 시간(초) 값을 여기에 적어준다.
                //for (int i = 0; i < maxModuleCount; i++)
                //{
                //    value = pcbInfo[i].TestTime.ToString();
                //    FuncFile.WriteIniFile(section, "Module" + (i + 1) + " Test Time", value, logPath);
                //}

                // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                section = "Pin Data";
                // PinLifeCount는 c:\FA\AutoInline\Setting\setting.ini 의 값을 가져오는 모양이다.
                // 값이 이상하다면 setting.ini 를 수정할것!
                value = PinLifeCount.ToString();
                FuncFile.WriteIniFile(section, "PinLimit", value + NewLine, logPath);



                //
                rs = GetCurrentPinLog();


                // 모듈별 핀 사용횟수
                for (int i = 1; i <= MaxTestPCCount * MaxSiteCount; i++)
                {
                    for (int j = 0; j < rs.GetLength(0); j++)
                    {
                        int site = int.Parse(rs[j, 0].ToString());
                        if (site != i)
                        {
                            continue;
                        }
                        int pinNo = int.Parse(rs[j, 1].ToString());
                        int arrayNo = int.Parse(rs[j, 2].ToString());
                        int inputCount = int.Parse(rs[j, 3].ToString());
                        int ngCount = int.Parse(rs[j, 4].ToString());
                        double ngRate = 0;
                        if (inputCount != 0)
                        {
                            ngRate = (double)ngCount / (double)inputCount * 100;
                        }

                        FuncFile.WriteIniFile(section, "Module" + Util.IntToString(site, 2) + "_Pin" + Util.IntToString(arrayNo, 2), (inputCount).ToString(), logPath);
                    }

                    // 모듈별 핀 에러횟수
                    for (int j = 0; j < rs.GetLength(0); j++)
                    {
                        int site = int.Parse(rs[j, 0].ToString());
                        if (site != i)
                        {
                            continue;
                        }
                        int pinNo = int.Parse(rs[j, 1].ToString());
                        int arrayNo = int.Parse(rs[j, 2].ToString());
                        int inputCount = int.Parse(rs[j, 3].ToString());
                        int ngCount = int.Parse(rs[j, 4].ToString());
                        double ngRate = 0;
                        if (inputCount != 0)
                        {
                            ngRate = (double)ngCount / (double)inputCount * 100;
                        }

                        if (j + 1 < rs.GetLength(0))
                        {
                            FuncFile.WriteIniFile(section, "Module" + Util.IntToString(site, 2) + "_Pin" + Util.IntToString(arrayNo, 2) + "_NG", (ngCount).ToString(), logPath);
                        }
                        else
                        {
                            FuncFile.WriteIniFile(section, "Module" + Util.IntToString(site, 2) + "_Pin" + Util.IntToString(arrayNo, 2) + "_NG", (ngCount).ToString() + NewLine, logPath);
                        }
                    }
                }


                // 알람
                section = "Alarm";
                value = (GlobalVar.SystemErrored) ? "Error" : "None";
                //if (GlobalVar.SystemErrored)
                {
                    FuncFile.WriteIniFile(section, "AlarmStatus", value + NewLine, logPath);
                }


                // JHRYU 요청에 따라 Equipment 맨뒤로
                section = "Equipment";
                FuncFile.WriteIniFile(section, "Maker", "Radix", logPath);
                FuncFile.WriteIniFile(section, "EQ Model", GlobalVar.SWName, logPath);
                FuncFile.WriteIniFile(section, "SW Ver", Application.ProductVersion, logPath);
                // 설비 가동상태
                value = (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun) ? "Run" : "Stop";
                FuncFile.WriteIniFile(section, "EW Running", value, logPath);

                value = "";
                value = DIO.GetDORead((int)enumDONames.Y4_3_Tower_Lamp_Green) ? "1" : "0";
                value += DIO.GetDORead((int)enumDONames.Y412_3_Tower_Lamp_Yellow) ? ", 1" : ", 0";
                value += DIO.GetDORead((int)enumDONames.Y4_4_Tower_Lamp_Red) ? ", 1" : ", 0";
                FuncFile.WriteIniFile(section, "Tower Lamp", value + NewLine, logPath);

                // 시스템 설정값
                section = "Machine Setting";
                FuncFile.WriteIniFile(section, "Lift Speed mm per sec", LiftSpeed.ToString("F0"), logPath);
                FuncFile.WriteIniFile(section, "Lift Acc/Dec", LiftAccDec.ToString("F0"), logPath);
                FuncFile.WriteIniFile(section, "Conveyor Timeout", ConveyorTimeout.ToString(), logPath);
                FuncFile.WriteIniFile(section, "Conveyor Default Width mm", DefaultPCBWidth.ToString("F2"), logPath);
                FuncFile.WriteIniFile(section, "Cooling By Time", CoolingByTime ? "Use" : "Not Use", logPath);
                FuncFile.WriteIniFile(section, "Cooling By Time sec", CoolingTime.ToString(), logPath);
                FuncFile.WriteIniFile(section, "Cooling By Temperature", CoolingByTemperature ? "Use" : "Not Use", logPath);
                FuncFile.WriteIniFile(section, "Cooling By Temperature degree", CoolingTemperature.ToString(), logPath);
                FuncFile.WriteIniFile(section, "Cooling By Temperature Timeout sec", CoolingMaxTime.ToString(), logPath);
                FuncFile.WriteIniFile(section, "Shift A Start Hour", ShiftAHour.ToString(), logPath);
                FuncFile.WriteIniFile(section, "Shift A Start Minute", ShiftAMin.ToString(), logPath);
                FuncFile.WriteIniFile(section, "Shift B Start Hour", ShiftBHour.ToString(), logPath);
                FuncFile.WriteIniFile(section, "Shift B Start Minute", ShiftBMin.ToString(), logPath);
                FuncFile.WriteIniFile(section, "Shift C Active", UseShiftC ? "Use" : "Not Use", logPath);
                FuncFile.WriteIniFile(section, "Shift C Start Hour", ShiftCHour.ToString(), logPath);
                FuncFile.WriteIniFile(section, "Shift C Start Minute", ShiftCMin.ToString(), logPath);
                FuncFile.WriteIniFile(section, "Output NG to Unloading", NGToUnloading ? "Use" : "Not Use", logPath);
                FuncFile.WriteIniFile(section, "Output Pass to NG", PassToNG ? "Use" : "Not Use", logPath);
                FuncFile.WriteIniFile(section, "Pin Down Conveyor Stop", PinDownAndClamp ? "Use" : "Not Use", logPath);
                FuncFile.WriteIniFile(section, "Test with Site Unclamp", TestWithSiteUnclamp ? "Use" : "Not Use", logPath);
                FuncFile.WriteIniFile(section, "Site Clamp Delay sec", SiteClampDelay.ToString(), logPath);
                FuncFile.WriteIniFile(section, "Leave One Site Empty", LeaveOneSite ? "Use" : "Not Use", logPath);
                FuncFile.WriteIniFile(section, "Fail when Resite Imposible", FailWhenNoEmpty ? "Use" : "Not Use", logPath);
                FuncFile.WriteIniFile(section, "PassMode Site Buffer", PassModeBuffer ? "Use" : "Not Use", logPath);
                FuncFile.WriteIniFile(section, "Self Retest", SelfRetest ? "Use" : "Not Use", logPath);
                FuncFile.WriteIniFile(section, "Self Retest times", SelfRetestCount.ToString(), logPath);
                FuncFile.WriteIniFile(section, "Resite", OtherRetest ? "Use" : "Not Use", logPath);
                FuncFile.WriteIniFile(section, "Resite times", OtherRetestCount.ToString(), logPath);
                FuncFile.WriteIniFile(section, "Scan Twice and Compare", ScanTwice ? "Use" : "Not Use", logPath);
                FuncFile.WriteIniFile(section, "Scan Timeout sec", ScanTimeout.ToString(), logPath);
                FuncFile.WriteIniFile(section, "Check PCB Input Miss", FuncInline.ScanInsertCheck ? "Use" : "Not Use", logPath);
                FuncFile.WriteIniFile(section, "Test Ready Command", UseSMDReady ? "Use" : "Not Use", logPath);
                FuncFile.WriteIniFile(section, "Test Command Timeout sec", TestCommandTimeout.ToString(), logPath);
                FuncFile.WriteIniFile(section, "Test Command Retry times", TestCommandRetry.ToString(), logPath);
                FuncFile.WriteIniFile(section, "Simulation Pass sec", TestPassTime.ToString(), logPath);
                FuncFile.WriteIniFile(section, "Check CN Duplication", CheckCNDuplication ? "Use" : "Not Use", logPath);
                FuncFile.WriteIniFile(section, "Check CN Duplication times", CheckCNDupeCount.ToString(), logPath);
                FuncFile.WriteIniFile(section, "Check CN Cross", CheckCNCross ? "Use" : "Not Use", logPath);
                FuncFile.WriteIniFile(section, "NG Alarm sec", NgAlarmTime.ToString(), logPath);
                FuncFile.WriteIniFile(section, "Block NG Site", BlockNGArray ? "Use" : "Not Use", logPath);
                FuncFile.WriteIniFile(section, "Block NG Site times", BlockNGCount.ToString(), logPath);
                FuncFile.WriteIniFile(section, "Pin Lift times", PinLifeCount.ToString(), logPath);
                FuncFile.WriteIniFile(section, "Defect Limit percent", DefectLimit.ToString(), logPath);
                FuncFile.WriteIniFile(section, "Defect Limit Block", BlockDefectArray ? "Use" : "Not Use", logPath);
                FuncFile.WriteIniFile(section, "Defect Limit Block minimum Input count", DefectBlockMinIn.ToString(), logPath);
                FuncFile.WriteIniFile(section, "Defect Limit Block minimum NG count", DefectBlockMinNG.ToString(), logPath);
                FuncFile.WriteIniFile(section, "System Log Directory", FuncInline.SystemLogPath.ToString(), logPath);
                FuncFile.WriteIniFile(section, "System Log Period minute", SystemLogTime.ToString(), logPath);
                FuncFile.WriteIniFile(section, "Log File Delete Day", GlobalVar.LogFileDeleteDay.ToString() + NewLine, logPath);

            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }

            // JHRYU : 생성된 PinLog 파일을 OverWrite 복사 해놓는 기능
            //try
            //{
            //    string srcfile = logPath;
            //    string dstfile = FuncInline.SystemLogPath + "\\" + GlobalVar.SystemLogFileName;
            //    System.IO.File.Copy(srcfile, dstfile, true);
            //}
            //catch (Exception ex)
            //{
            //}



        }
        public static void WritePinLog_Samsung() // jhryu 20230220 삼성요구
        {
            string logPath = "";
            try
            {
                //string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                //string logPath = PinLogDirectory + "\\" + fileName;

                //string logPath = "D:\\SamsungMobile";
                //if (!Directory.Exists(logPath))
                //{
                //    Directory.CreateDirectory(logPath);
                //}
                //logPath += "\\" + "GlobalSIA";
                //if (!Directory.Exists(logPath))
                //{
                //    Directory.CreateDirectory(logPath);
                //}
                //logPath += "\\" + "Log";
                //if (!Directory.Exists(logPath))
                //{
                //    Directory.CreateDirectory(logPath);
                //}
                //logPath += "\\" + fileName;
                logPath = PinLogDirectory + "\\" + SystemLogFileName;

                // 저장 시작

                //string key = "";
                string value = "";
                string section = "";
                string NewLine = "\n";
                if (File.Exists(logPath))
                {
                    NewLine = "";
                }
                /*
                string section = "Equipment";
                FuncFile.WriteIniFile(section, "Maker", "Radix", logPath);
                FuncFile.WriteIniFile(section, "EQ Model", "ModuleAutoInline", logPath);
                FuncFile.WriteIniFile(section, "SW Ver", Application.ProductVersion, logPath);
                // 설비 가동상태
                value = (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun) ? "Run" : "Stop";
                FuncFile.WriteIniFile(section, "EW Running", value, logPath);

                value = "";
                value = DIO.GetDORead((int)enumDONames.Y4_3_Tower_Lamp_Green) ? "1" : "0";
                value += DIO.GetDORead((int)enumDONames.Y412_3_Tower_Lamp_Yellow) ? ", 1" : ", 0";
                value += DIO.GetDORead((int)enumDONames.Y4_4_Tower_Lamp_Red) ? ", 1" : ", 0";
                FuncFile.WriteIniFile(section, "Tower Lamp", value, logPath);
                */
                section = "System Data";
                FuncFile.WriteIniFile(section, "Model Name", GlobalVar.ModelName, logPath);
                value = (FuncInline.OtherRetest) ? "1" : "0";
                FuncFile.WriteIniFile(section, "OtherSiteRetest", value, logPath);
                value = OtherRetestCount.ToString();
                FuncFile.WriteIniFile(section, "Test Retry Count", value, logPath);
                value = BadMarkToNG.ToString();
                FuncFile.WriteIniFile(section, "BadMarkToNGCV", value, logPath);
                value = PassToNG ? "1" :
                        NGToUnloading ? "2" :
                        "0";
                FuncFile.WriteIniFile(section, "UnloadOp", value, logPath);
                FuncFile.WriteIniFile(section, "CVFixedPosition", "0", logPath);
                FuncFile.WriteIniFile(section, "FlowDirection", "0" + NewLine, logPath);


                section = "Time Data";
                value = "90";       // 초기화버튼시 현재 90초까지로 상수 설정되어있다.
                FuncFile.WriteIniFile(section, "OriginLimitTime", value, logPath);
                value = "30"; // RobotTimeout.ToString();
                FuncFile.WriteIniFile(section, "MotionMoveLimitTime", value, logPath);
                value = GlobalVar.NormalTimeout.ToString();
                FuncFile.WriteIniFile(section, "CylinderMoveLimitTime", value, logPath);
                value = TestTimeout.ToString();
                FuncFile.WriteIniFile(section, "TestLimitTime", value + NewLine, logPath);


                int maxModuleCount = UseSite.Length;   //21
                int maxArrayCount = MaxArrayCount;     //12

                //structPCBInfo[] pcbInfo = new structPCBInfo[maxModuleCount];

                //for (int i = 0; i < MaxTestPCCount * MaxSiteCount; i++)
                //{
                //    pcbInfo[i] = PCBInfo[(int)enumTeachingPos.Site1_F_DT1 * i];
                //}

                section = "Product Management";
                FuncFile.WriteIniFile(section, "ProductCount", "0" + NewLine, logPath);
                //FuncFile.WriteIniFile(section, "ToTal Product Count", value, logPath);
                //value = FuncInline.PCBInputCount.ToString();
                //FuncFile.WriteIniFile(section, "ToTal Input Count", value, logPath);
                //value = FuncInline.PCBPassCount.ToString();
                //FuncFile.WriteIniFile(section, "ToTal Good Count", value, logPath);
                //value = FuncInline.PCBDefectCount.ToString();
                //FuncFile.WriteIniFile(section, "ToTal NG Count", value, logPath);

                string[,] rs = GetSiteInputCount();

                for (int i = 0; i < rs.GetLength(0); i++)
                {
                    string siteNo = rs[i, 0];
                    value = rs[i, 1];

                    FuncFile.WriteIniFile(section, "Module" + siteNo + " Input Count", value, logPath);
                }

                rs = GetSiteNGCount();
                for (int i = 0; i < rs.GetLength(0); i++)
                {
                    string siteNo = rs[i, 0];
                    value = rs[i, 1];

                    FuncFile.WriteIniFile(section, "Module" + siteNo + " NG Count", value, logPath);
                    //if (i + 1 < rs.GetLength(0))
                    //{
                    //    FuncFile.WriteIniFile(section, "Module" + siteNo + " NG Count", value, logPath);
                    //}
                    //else
                    //{
                    //    FuncFile.WriteIniFile(section, "Module" + siteNo + " NG Count", value + NewLine, logPath);
                    //}
                }

                for (int i = 0; i < maxModuleCount; i++)
                {
                    value = (FuncInline.UseSite[i]) ? "1" : "0";
                    FuncFile.WriteIniFile(section, "Module" + (i + 1) + "_Used", value + (i == maxModuleCount - 1 ? NewLine : ""), logPath);
                }

                //int ngBuffCount = 0;
                //if (FuncInline.PCBInfo[(int)enumTeachingPos.NgShuttle1].PCBStatus != FuncInline.enumSMDStatus.UnKnown) ngBuffCount++;
                //if (FuncInline.PCBInfo[(int)enumTeachingPos.NgShuttle2].PCBStatus != FuncInline.enumSMDStatus.UnKnown) ngBuffCount++;
                //value = ngBuffCount.ToString();
                //FuncFile.WriteIniFile(section, "NG Buffer Count", value, logPath);

                //for (int i = 0, cnt = 0; i < maxModuleCount; i++)
                //{
                //    if (FuncInline.UseSite[i]) cnt++;
                //    value = cnt.ToString();
                //}
                //FuncFile.WriteIniFile(section, "Module Use Count", value, logPath);


                //// 테스트 타임은 글로벌에 변수 만들어서 모듈마다 테스트가 끝날때마다 시간을 업데이트하고 그 시간(초) 값을 여기에 적어준다.
                //for (int i = 0; i < maxModuleCount; i++)
                //{
                //    value = pcbInfo[i].TestTime.ToString();
                //    FuncFile.WriteIniFile(section, "Module" + (i + 1) + " Test Time", value, logPath);
                //}

                // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                section = "Pin Data";
                // PinLifeCount는 c:\FA\AutoInline\Setting\setting.ini 의 값을 가져오는 모양이다.
                // 값이 이상하다면 setting.ini 를 수정할것!
                value = PinLifeCount.ToString();
                FuncFile.WriteIniFile(section, "PinLimit", value + NewLine, logPath);



                //
                rs = GetCurrentPinLog();


                // 모듈별 핀 사용횟수
                for (int i = 1; i <= MaxTestPCCount * MaxSiteCount; i++)
                {
                    for (int j = 0; j < rs.GetLength(0); j++)
                    {
                        int site = int.Parse(rs[j, 0].ToString());
                        if (site != i)
                        {
                            continue;
                        }
                        int pinNo = int.Parse(rs[j, 1].ToString());
                        int arrayNo = int.Parse(rs[j, 2].ToString());
                        int inputCount = int.Parse(rs[j, 3].ToString());
                        int ngCount = int.Parse(rs[j, 4].ToString());
                        double ngRate = 0;
                        if (inputCount != 0)
                        {
                            ngRate = (double)ngCount / (double)inputCount * 100;
                        }

                        FuncFile.WriteIniFile(section, "Module" + Util.IntToString(site, 2) + "_Pin" + Util.IntToString(arrayNo, 2), (inputCount).ToString(), logPath);
                    }

                    // 모듈별 핀 에러횟수
                    for (int j = 0; j < rs.GetLength(0); j++)
                    {
                        int site = int.Parse(rs[j, 0].ToString());
                        if (site != i)
                        {
                            continue;
                        }
                        int pinNo = int.Parse(rs[j, 1].ToString());
                        int arrayNo = int.Parse(rs[j, 2].ToString());
                        int inputCount = int.Parse(rs[j, 3].ToString());
                        int ngCount = int.Parse(rs[j, 4].ToString());
                        double ngRate = 0;
                        if (inputCount != 0)
                        {
                            ngRate = (double)ngCount / (double)inputCount * 100;
                        }

                        if (j + 1 < rs.GetLength(0))
                        {
                            FuncFile.WriteIniFile(section, "Module" + Util.IntToString(site, 2) + "_Pin" + Util.IntToString(arrayNo, 2) + "_NG", (ngCount).ToString(), logPath);
                        }
                        else
                        {
                            FuncFile.WriteIniFile(section, "Module" + Util.IntToString(site, 2) + "_Pin" + Util.IntToString(arrayNo, 2) + "_NG", (ngCount).ToString() + NewLine, logPath);
                        }
                    }
                }


                // 알람
                section = "Alarm";
                value = (GlobalVar.SystemErrored) ? "Error" : "None";
                //if (GlobalVar.SystemErrored)
                {
                    FuncFile.WriteIniFile(section, "AlarmStatus", value + NewLine, logPath);
                }


                // JHRYU 요청에 따라 Equipment 맨뒤로
                section = "Equipment";
                FuncFile.WriteIniFile(section, "Maker", "Radix", logPath);
                FuncFile.WriteIniFile(section, "EQ Model", GlobalVar.SWName, logPath);
                FuncFile.WriteIniFile(section, "SW Ver", Application.ProductVersion, logPath);
                // 설비 가동상태
                value = (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun) ? "Run" : "Stop";
                FuncFile.WriteIniFile(section, "EW Running", value, logPath);

                value = "";
                value = DIO.GetDORead((int)enumDONames.Y4_3_Tower_Lamp_Green) ? "1" : "0";
                value += DIO.GetDORead((int)enumDONames.Y412_3_Tower_Lamp_Yellow) ? ", 1" : ", 0";
                value += DIO.GetDORead((int)enumDONames.Y4_4_Tower_Lamp_Red) ? ", 1" : ", 0";
                FuncFile.WriteIniFile(section, "Tower Lamp", value + NewLine, logPath);

            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }

            // JHRYU : 생성된 PinLog 파일을 OverWrite 복사 해놓는 기능
            //try
            //{
            //    string srcfile = logPath;
            //    string dstfile = FuncInline.SystemLogPath + "\\" + GlobalVar.SystemLogFileName;
            //    System.IO.File.Copy(srcfile, dstfile, true);
            //}
            //catch (Exception ex)
            //{
            //}



        }

        public static void CheckFailType(enumTeachingPos pos) // 지정 위치의 검사 결과값 조회해서 fail 처리 구분 계산
        {
            // 테스트 완료 아니면 패쓰
            if (FuncInline.PCBInfo[(int)pos].PCBStatus <= FuncInline.enumSMDStatus.Testing)
            {
                return;
            }

            try
            {
                // ng로 보내는 경우를 우선해서 판단해야 함.
                // 1. 무조건 ng로 보내는 경우 우선 (cancel, timeout)
                // 2. xout,badmark 해당이면 해당 설정에 따라
                // 3. 일반 fail 경우 해당 설정 따라
                // 4. 나머지 양품이면 해당 설정 따라

                // 에러코드 없는 것 있으면 바로 NG로
                for (int i = 0; i < FuncInline.PCBInfo[(int)pos].ErrorCode.Length; i++)
                {
                    if (FuncInline.PCBInfo[(int)pos].ErrorCode[i] >= 0 &&
                        (FuncInline.TestErrorCode[FuncInline.PCBInfo[(int)pos].ErrorCode[i]] == null ||
                                FuncInline.TestErrorCode[FuncInline.PCBInfo[(int)pos].ErrorCode[i]] == ""))
                    {
                        FuncInline.PCBInfo[(int)pos].NgType = enumNGType.Unknown;
                        return;
                    }
                }

                #region 1. 무조건 ng로 보내야 하는 경우 무조건 NG쪽으로
                if (FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Response_NG ||
                    FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Timeout) // 
                {
                    FuncInline.PCBInfo[(int)pos].NgType = enumNGType.Timeout;
                    return;
                }
                if (FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Cancel ||
                    FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.User_Cancel) // 
                {
                    FuncInline.PCBInfo[(int)pos].NgType = enumNGType.TestCancel;
                    return;
                }
                #endregion

                #region 2. Xout, BadMark 경우 해당 설정 따라
                for (int i = 0; i < FuncInline.PCBInfo[(int)pos].SMDStatus.Length; i++) // 전체 어레이 
                {
                    if (ArrayUse[i]) // 사용하는 어레이 경우
                    {
                        if (FuncInline.PCBInfo[(int)pos].Xout[i]) // XOut
                        {
                            FuncInline.PCBInfo[(int)pos].NgType = enumNGType.XOut;
                            return;
                        }
                        if (FuncInline.PCBInfo[(int)pos].BadMark[i]) // BadMark
                        {
                            FuncInline.PCBInfo[(int)pos].NgType = enumNGType.BadMark;
                            return;
                        }
                    }
                }
                #endregion

                #region 3. 일반 Fail 경우 해당 설정 따라
                if (!FuncInline.NGToUnloading &&
                    FuncInline.PCBInfo[(int)pos].PCBStatus != FuncInline.enumSMDStatus.ReTest)
                {
                    for (int i = 0; i < FuncInline.PCBInfo[(int)pos].SMDStatus.Length; i++) // 전체 어레이 
                    {
                        if (ArrayUse[i] &&
                            !FuncInline.PCBInfo[(int)pos].Xout[i] &&
                            !FuncInline.PCBInfo[(int)pos].BadMark[i]) // 사용하는 어레이 경우
                        {
                            if (FuncInline.PCBInfo[(int)pos].ErrorCode[i] > -1 &&
                                FuncInline.PCBInfo[(int)pos].ErrorCode[i] < 900) // Normal
                            {
                                FuncInline.PCBInfo[(int)pos].NgType = enumNGType.NormalFail;
                                return;
                            }
                        }
                    }
                }
                #endregion


                /*
                // 이외 fail 확인
                for (int i = 0; i < FuncInline.PCBInfo[(int)pos].SMDStatus.Length; i++) // 전체 어레이 
                {
                    if (ArrayUse[i]) // 사용하는 어레이 경우
                    {
                        switch (FuncInline.PCBInfo[(int)pos].SMDStatus[i])
                        {
                            case FuncInline.enumSMDStatus.Response_NG:
                                FuncInline.PCBInfo[(int)pos].NgType = enumNGType.Timeout;
                                return;
                            case FuncInline.enumSMDStatus.ReTest:
                                FuncInline.PCBInfo[(int)pos].NgType = enumNGType.NormalFail;
                                return;
                            case FuncInline.enumSMDStatus.Test_Cancel:
                                FuncInline.PCBInfo[(int)pos].NgType = enumNGType.TestCancel;
                                return;
                            case FuncInline.enumSMDStatus.Test_Fail:
                                // Xout,BadMark 별도 확인
                                switch (FuncInline.PCBInfo[(int)pos].ErrorCode[i])
                                {
                                    case 994:
                                        FuncInline.PCBInfo[(int)pos].NgType = enumNGType.XOut;
                                        return;
                                    case 995:
                                        FuncInline.PCBInfo[(int)pos].NgType = enumNGType.BadMark;
                                        return;
                                    default:
                                        FuncInline.PCBInfo[(int)pos].NgType = enumNGType.NormalFail;
                                        return;
                                }
                            case FuncInline.enumSMDStatus.Test_Timeout:
                                FuncInline.PCBInfo[(int)pos].NgType = enumNGType.Timeout;
                                return;
                            case FuncInline.enumSMDStatus.User_Cancel:
                                FuncInline.PCBInfo[(int)pos].NgType = enumNGType.TestCancel;
                                return;
                        }
                    }
                }
                //*/

                // 4. 양품 경우 해당 설정 따라
                FuncInline.PCBInfo[(int)pos].NgType = enumNGType.OK;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }

        }


        // 배출컨베어에서 다음 장비로 PCB 이송
        public static bool MoveOutputConveyorToNextMachine()
        {
            try
            {
                FuncInline.ResetInOutTime();
                #region PCB 정보 없으면 액션 삭제
                /*
                if (FuncInline.PCBInfo[(int)enumTeachingPos.OutConveyor].PCBStatus == FuncInline.enumSMDStatus.UnKnown)
                {
                    //debug("MoveOutputConveyorToNextMachine PCB 정보 없으면 액션 삭제");
                    Util.ResetWatch(ref FuncInline.OutConveyorActionWatch);
                    FuncInline.OutConveyorAction = FuncInline.enumLiftAction.Waiting;
                    DIO.WriteDOData(enumDONames.Y400_1_Out_Conveyor_Motor_Cw, false);
                    return true;
                }
                //*/
                #endregion
                #region PCB 정보 배출대상 아니면 액션 삭제. 직렬 경우 No_Test
                if (FuncInline.PCBInfo[(int)enumTeachingPos.OutConveyor].PCBStatus == FuncInline.enumSMDStatus.Before_Command)
                {
                    //debug("MoveOutputConveyorToNextMachine PCB 정보 배출대상 아니면 액션 삭제");
                    Util.ResetWatch(ref FuncInline.OutConveyorActionWatch);
                    FuncInline.OutConveyorAction = FuncInline.enumLiftAction.Waiting;
                    DIO.WriteDOData(enumDONames.Y400_1_Out_Conveyor_Motor_Cw, false);
                    return false;
                }
                #endregion
                #region if 모든 센서 확인 안 되면 정보 넘기고 종료
                if (!DIO.GetDIData(enumDINames.X02_3_Out_Conveyor_PASSLIne_PCB_Start_Sensor) &&
                        !DIO.GetDIData(enumDINames.X02_4_Out_Conveyor_PASSLine_PCB_Stop_Sensor)&&
                        !DIO.GetDIData(enumDINames.X02_1_SMEMA_Before_Ready))   //다음 설비에서 받으면 false될테니까 by DG
                                                                                
                {
                    //debug("if 모든 센서 확인 안 되면 종료");
                    DIO.WriteDOData(enumDONames.Y412_1_SMEMA_After_Ready, false); // 스메마 출력 가동
                    DIO.WriteDOData(enumDONames.Y400_1_Out_Conveyor_Motor_Cw, false);
                    //GlobalVar.DryRunMethod = enumDryRunMethod.None; // 드라이런 method 초기화
                    FuncInline.OutConveyorActionWatch.Stop();
                    FuncInline.OutConveyorActionWatch.Reset();
                    FuncInline.ClearPCBInfo(enumTeachingPos.OutConveyor);
                    return true;
                }
                #endregion
            
                else if (FuncInline.OutConveyorActionWatch.IsRunning &&
                    FuncInline.OutConveyorActionWatch.ElapsedMilliseconds > FuncInline.ConveyorTimeout * 1000)
                {
                    FuncInline.OutConveyorActionWatch.Stop();
                    FuncInline.OutConveyorActionWatch.Reset();
                    DIO.WriteDOData(enumDONames.Y400_1_Out_Conveyor_Motor_Cw, false);

                   
                        //FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
                        //    DateTime.Now.ToString("HH:mm:ss"),
                        //    FuncInline.enumErrorPart.OutConveyor,
                        //    enumErrorCode.Stopper_Down_Error,
                        //    false,
                        //    "OutputConveyor Stopper Down Sensor not detected."));
                 
                    FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
                                                DateTime.Now.ToString("HH:mm:ss"),
                                                FuncInline.enumErrorPart.OutConveyor,
                                                enumErrorCode.Conveyor_Timeout,
                                                false,
                                                "OutputConveyor output action timed out"));
                   
                    return false;
                }

                //다음설비가 오토인라인이면 패스신호 ON 여부 by DG
                if (DIO.GetDIData(enumDINames.X00_7_SMEMA_After_AutoInline))
                {
                    // 다음라인 패쓰 여부는 이 장비에서 테스트 했는가
                    DIO.WriteDOData(enumDONames.Y404_5_SMEMA_After_Pass, FuncInline.PCBInfo[(int)enumTeachingPos.OutConveyor].PCBStatus >= FuncInline.enumSMDStatus.Test_Pass);
                }
             

                #region if 모든 센서 확인 안 되면 종료
                if (//!GlobalVar.Simulation &&
                    //!GlobalVar.DryRun &&
                    !DIO.GetDIData(enumDINames.X02_3_Out_Conveyor_PASSLIne_PCB_Start_Sensor) &&
                        !DIO.GetDIData(enumDINames.X02_4_Out_Conveyor_PASSLine_PCB_Stop_Sensor))// &&
                                                                                  //FuncInline.PCBInfo[(int)enumTeachingPos.OutputLift].PCBStatus == FuncInline.enumSMDStatus.UnKnown)
                {
                    DIO.WriteDOData(enumDONames.Y412_1_SMEMA_After_Ready, false); // 스메마 출력 가동
                    DIO.WriteDOData(enumDONames.Y400_1_Out_Conveyor_Motor_Cw, false);
                    //debug("if 모든 센서 확인 안 되면 종료");
                    FuncInline.OutConveyorAction = FuncInline.enumLiftAction.Waiting;
                    //GlobalVar.DryRunMethod = enumDryRunMethod.None; // 드라이런 method 초기화
                    FuncInline.OutConveyorActionWatch.Stop();
                    FuncInline.OutConveyorActionWatch.Reset();
                    FuncInline.ClearPCBInfo(enumTeachingPos.OutConveyor);
                    Util.ResetWatch(ref FuncInline.OutConveyorActionWatch);
                    return true;
                }
                #endregion

             
                #region else if 스메마 입력되면 컨베어 가동
                else if (!DIO.GetDORead(enumDONames.Y400_1_Out_Conveyor_Motor_Cw) &&
                        (DIO.GetDIData(enumDINames.X02_2_SMEMA_After_Ready) ||   // 임시 뒷셔틀 수정 전까지
                            GlobalVar.DryRun ||
                            GlobalVar.SystemStatus < enumSystemStatus.AutoRun))  // 스메마 입력되면 컨베어 가동
                {
                    //debug("else if 스메마 입력되면 컨베어 가동");
                    DIO.WriteDOData(enumDONames.Y412_1_SMEMA_After_Ready, true); // 스메마 출력 가동
                    DIO.WriteDOData(enumDONames.Y400_1_Out_Conveyor_Motor_Cw, true);
                }

                #endregion else 타임아웃 체크
                else // 이송중 Timeout check
                {
                    if (!FuncInline.OutConveyorActionWatch.IsRunning)
                    {
                        Util.StartWatch(ref FuncInline.OutConveyorActionWatch);
                    }
                    if (GlobalVar.DryRun && // 드라이런은 시뮬레이션모드와 동일하지만 PCB 감지와 SMEMA만 가상화
                        FuncInline.OutConveyorActionWatch.IsRunning &&
                        FuncInline.OutConveyorActionWatch.ElapsedMilliseconds > 3000)
                    {
                        DIO.WriteDOData(enumDONames.Y412_1_SMEMA_After_Ready, false); // 스메마 출력 가동
                        DIO.WriteDOData(enumDONames.Y400_1_Out_Conveyor_Motor_Cw, false);
                        //debug("if dryrun 시간 경과시 이동 완료");
                        FuncInline.OutConveyorAction = FuncInline.enumLiftAction.Waiting;
                        //GlobalVar.DryRunMethod = enumDryRunMethod.None; // 드라이런 method 초기화
                        FuncInline.ClearPCBInfo(enumTeachingPos.OutConveyor);
                        Util.ResetWatch(ref FuncInline.OutConveyorActionWatch);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }


            return false;
        }

    
        // 전 장비에서 투입컨베어로 수령
        public static bool InShuttle_Input()
        {
            try
            {
                FuncInline.ResetInOutTime();

                #region PCB 정보 있으면 액션 삭제
                if (FuncInline.PCBInfo[(int)enumTeachingPos.InConveyor].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
                {
                    //debug("InputConveyorInput PCB 정보 있으면 액션 삭제");
                    Util.ResetWatch(ref FuncInline.InConveyorActionWatch);
                    FuncInline.InConveyorAction = FuncInline.enumLiftAction.Waiting;
                    DIO.WriteDOData(enumDONames.Y4_2_SMEMA_Before_Ready, false);
                    DIO.WriteDOData(enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw, false);
                    return false;
                }
                #endregion
                FuncInline.ResetInOutTime();
                DIO.WriteDOData(enumDONames.Y4_2_SMEMA_Before_Ready, !FuncInline.CycleStop &&
                                                                        FuncInline.BuyerChange != enumBuyerChange.Yellow &&
                                                                        //!GlobalVar.InputLiftInputChecked &&
                                                                        FuncInlineMove.CheckNearPMCWidth(FuncInline.enumPMCAxis.ST00_InShuttle_Width) &&
                                                                        !DIO.GetDIData(enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor) &&
                                                                        !DIO.GetDIData(enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor) &&
                                                                        DIO.GetDIData(enumDINames.X02_1_SMEMA_Before_Ready)); // 스메마 출력 가동

                #region else 수령 위치로 이동 되면
                if (false) // !CheckNearWidth(FuncInline.enumPMCAxis.ST00_InShuttle_Width, true)) // 폭조절은 시작 전에
                {
                    ////PMCClass.ABSMove(FuncInline.enumPMCAxis.ST00_InShuttle_Width, FuncInline.TeachingWidth[(int)FuncInline.enumPMCAxis.ST00_InShuttle_Width], GlobalVar.WidthSpeed); // 폭조절은 시작 전에
                }
                else
                {

                    FuncInline.PCBInfo[(int)enumTeachingPos.InConveyor].TestPass = DIO.GetDIData(enumDINames.X00_6_SMEMA_Before_Pass) ||
                                                                                            FuncInline.PassMode; // pass 여부 수신

                    if (GlobalVar.Simulation &&
                        DIO.GetDIData(enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor) &&
                        !DIO.GetDIData(enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor) &&
                        !DIO.GetDIData(enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor) &&
                        DIO.GetDORead(enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw))
                    {
                        DIO.WriteDIData(enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor, false);
                        DIO.WriteDIData(enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor, true);
                        DIO.WriteDIData(enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor, true);
                    }

                    #region if 스토퍼 안 올라가 있으면 스토퍼 상승
                    if (!DIO.GetDIData(enumDINames.X302_2_In_Shuttle_Stopper_Cyl_Up_Sensor))  // 스토퍼 안 올라가 있으면 스토퍼 상승
                    {
                        //debug("Input if 스토퍼 안 올라가 있으면 스토퍼 상승");
                        DIO.WriteDOData(enumDONames.Y302_2_IN_Shuttle_CONTACT_STOPPER_SOL, false);
                    }
                    #endregion if 스토퍼 안 올라가 있으면 스토퍼 상승

                    #region else if 스메마 입력되면 컨베어 가동
                    else if (//(FuncInline.InputPCB || DIO.GetDIData(enumDINames.X02_1_SMEMA_Before_Ready)) &&
                        !DIO.GetDORead(enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw))  // 스메마 입력되면 컨베어 가동
                    {
                        //debug("Input else if 스메마 입력되면 컨베어 가동");
                        DIO.WriteDOData(enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw, true);
                    }
                    #endregion else if 스메마 입력되면 컨베어 가동

                    #region else if 시작/중간 센서 감지되고 최종 센서 확인 안 되면 컨베어 가동
                    /*
                    else if (DIO.GetDIData(enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor) &&
                        !DIO.GetDIData(enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor) &&
                        !DIO.GetDIData(enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor) &&
                        !DIO.GetDORead(enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw)) // 시작/중간 센서 감지되고 최종 센서 확인 안 되면 컨베어 가동
                    {
                        //debug("Input else if 시작/중간 센서 감지되고 최종 센서 확인 안 되면 컨베어 가동");
                        DIO.WriteDOData(enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw, true);
                    }
                    //*/
                    #endregion else if 시작/중간 센서 감지되고 최종 센서 확인 안 되면 컨베어 가동

                    #region else if 최종 센서 확인되면 투입 종료 - Cooling로 변경
                    else if (!DIO.GetDIData(enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor) &&
                        (DIO.GetDIData(enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor)))
                    {
                        //debug("Input else if 최종 센서 확인되면 투입 종료 - Scan으로 변경");
                        if (FuncInline.PassMode) //  패스모드일 경우 투입수량 증가. 패스 아니면 로봇이 투입시
                        {
                            FuncInline.PCBInputCount++;
                        }

                        //GlobalVar.InputLiftInputChecked = false; // 투입센서 감지되면 true, 종단에 도착하면 false.  True 동안 smema 보내면 안 된다.

                        DIO.WriteDOData(enumDONames.Y4_2_SMEMA_Before_Ready, false);
                        Thread.Sleep(500);
                        DIO.WriteDOData(enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw, false);
                        GlobalVar.InputTime = GlobalVar.TickCount64;
                        if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun &&
                            GlobalVar.SystemStatus != enumSystemStatus.ErrorStop)
                        {
                            GlobalVar.SystemStatus = enumSystemStatus.AutoRun;
                        }
                        #region if Pass할 PCB면 종료
                        if (FuncInline.PCBInfo[(int)enumTeachingPos.InConveyor].TestPass)
                        {
                            //debug("Input if Pass할 PCB면 종료");
                            //GlobalVar.InputLiftAction = FuncInline.enumLiftAction.Waiting;
                            if (GlobalVar.SMDLog &&
                                FuncInline.PCBInfo[(int)enumTeachingPos.InConveyor].PCBStatus != FuncInline.enumSMDStatus.No_Test)
                            {
                                FuncLog.WriteLog_Tester("AutoInline_PCBInfo InputLift .PCBStatus = FuncInline.enumSMDStatus.No_Test");
                            }
                            FuncInline.PCBInfo[(int)enumTeachingPos.InConveyor].PCBStatus = FuncInline.enumSMDStatus.No_Test;
                            FuncInline.PCBInfo[(int)enumTeachingPos.InConveyor].NgType = enumNGType.OK;
                            Util.ResetWatch(ref FuncInline.InConveyorActionWatch);
                            DIO.WriteDOData(enumDONames.Y4_2_SMEMA_Before_Ready, false);
                            DIO.WriteDOData(enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw, false);
                            return true;
                        }
                        #endregion
                        #region else Pass할 PCB 아니면 스캔 동작으로 연결
                        else if (FuncInline.PCBInfo[(int)enumTeachingPos.InConveyor].PCBStatus == FuncInline.enumSMDStatus.UnKnown)
                        {
                         
                       
                            //FuncInline.InConveyorAction = FuncInline.enumLiftAction.Scan;
                            if (GlobalVar.SMDLog &&
                                FuncInline.PCBInfo[(int)enumTeachingPos.InConveyor].PCBStatus != FuncInline.enumSMDStatus.Before_Scan)
                            {
                                FuncLog.WriteLog_Tester("AutoInline_PCBInfo InputLift .PCBStatus = FuncInline.enumSMDStatus.Before_Scan");
                            }
                            FuncInline.PCBInfo[(int)enumTeachingPos.InConveyor].PCBStatus = FuncInline.enumSMDStatus.Cooling;
                            FuncInline.PCBInfo[(int)enumTeachingPos.InConveyor].NgType = enumNGType.OK;
                        }
                        #endregion
                        Util.ResetWatch(ref FuncInline.InConveyorActionWatch);
                        DIO.WriteDOData(enumDONames.Y4_2_SMEMA_Before_Ready, false);
                        DIO.WriteDOData(enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw, false);
                        return true;
                    }
                    //debug("!DIO.GetDIData(enumDINames.X01_0_Input_Lift_Start_Sensor) : " + !DIO.GetDIData(enumDINames.X01_0_Input_Lift_Start_Sensor));
                    //debug("DIO.GetDIData(enumDINames.X01_1_Input_Lift_Stop_Sensor) : " + DIO.GetDIData(enumDINames.X01_1_Input_Lift_Stop_Sensor));
                    //debug("DIO.GetDIData(enumDINames.X01_2_Input_Lift_End_Sensor) : " + DIO.GetDIData(enumDINames.X01_2_Input_Lift_End_Sensor));
                    //debug("FuncInline.PCBInfo[(int)enumTeachingPos.InputLift].PCBStatus == FuncInline.enumSMDStatus.UnKnown) : " + (FuncInline.PCBInfo[(int)
                    //    enumTeachingPos.InputLift].PCBStatus == FuncInline.enumSMDStatus.UnKnown));
                    //debug("DIO.GetDORead(enumDONames.Y02_1_Input_Lift_Conveyor_Run) : " + DIO.GetDORead(enumDONames.Y02_1_Input_Lift_Conveyor_Run));
                    #endregion else if 최종 센서 확인되면 투입 종료 - Scan으로 변경
                }
                #endregion else 수령 위치면

                #region 투입 타임아웃
                if (FuncInline.InConveyorActionWatch.IsRunning &&
                    FuncInline.InConveyorActionWatch.ElapsedMilliseconds > FuncInline.ConveyorTimeout * 1000)
                {
                    FuncInline.InConveyorActionWatch.Stop();
                    FuncInline.InConveyorActionWatch.Reset();
                    DIO.WriteDOData(enumDONames.Y4_2_SMEMA_Before_Ready, false);
                    DIO.WriteDOData(enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw, false);
                    if (!DIO.GetDIData(enumDINames.X302_2_In_Shuttle_Stopper_Cyl_Up_Sensor))
                    {
                        FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
                                                    DateTime.Now.ToString("HH:mm:ss"),
                                                    FuncInline.enumErrorPart.InShuttle,
                                                    enumErrorCode.Stopper_Down_Error,
                                                    false,
                                                    "In Shuttle Stopper Up Sensor not detected"));
                    }
                    else
                    {
                        FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
                                                    DateTime.Now.ToString("HH:mm:ss"),
                                                    FuncInline.enumErrorPart.InShuttle,
                                                    enumErrorCode.Conveyor_Timeout,
                                                    false,
                                                    "In Shuttle PCB Input Timed out"));
                    }
                    return false;
                }
                #endregion
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }

            return false;
        }

        #region === 공통 헬퍼 (SiteIoMaps 사용) ===

        enum LiftSide { Front, Rear }
        enum LiftLevel { Up, Down }

        // 사이트 인덱스(리프트 포지션 계산용 — LiftPos enum이 연속값이라는 기존 전제 유지)
        static int GetSiteIndex(enumTeachingPos site)
        {
            int first = (int)enumTeachingPos.Site1_F_DT1;
            int last = (int)enumTeachingPos.Site26_R_FT3; // 마지막 사이트 이름으로 맞춰주세요

            int value = (int)site;
            if (value < first || value > last)
                return -1; // 또는 throw new ArgumentOutOfRangeException(nameof(site));

            return value - first;
        }

        // 리프트 이젝션 포지션 계산 (기존 enumLiftPos.Sitexx_Up/Down가 연속이라는 전제)
        static FuncInline.enumLiftPos GetLiftEjectPos(LiftSide side, LiftLevel level, enumTeachingPos site)
        {
            var idx = GetSiteIndex(site);
            return (level == LiftLevel.Up)
                ? (FuncInline.enumLiftPos.Site1_F_DT1_Up + idx)
                : (FuncInline.enumLiftPos.Site1_F_DT1_Down + idx);
        }

        static enumLiftName GetLiftName(LiftSide side)
            => (side == LiftSide.Front ? enumLiftName.FrontLift : enumLiftName.RearLift);

        // Lift 구간 PCB 유무 확인
        static bool LiftHasPcb(LiftSide side, LiftLevel level)
        {
            if (side == LiftSide.Front && level == LiftLevel.Up)
                return DIO.GetDIData(enumDINames.X403_2_Front_Lift_Up_PCB_In_Sensor) ||
                       DIO.GetDIData(enumDINames.X403_5_Front_Lift_Up_PCB_Stop_Sensor);

            if (side == LiftSide.Front && level == LiftLevel.Down)
                return DIO.GetDIData(enumDINames.X400_0_Front_Lift_Down_PCB_In_Sensor) ||
                       DIO.GetDIData(enumDINames.X400_2_Front_Lift_Down_PCB_Stop_Sensor);

            if (side == LiftSide.Rear && level == LiftLevel.Up)
                return DIO.GetDIData(enumDINames.X404_6_Rear_Lift_Up_PCB_In_Sensor) ||
                       DIO.GetDIData(enumDINames.X405_1_Rear_Lift_Up_PCB_Stop_Sensor);

            // Rear Down
            return DIO.GetDIData(enumDINames.X405_5_Rear_Lift_Down_PCB_In_Sensor) ||
                   DIO.GetDIData(enumDINames.X405_7_Rear_Lift_Down_PCB_Stop_Sensor);
        }

        static bool LiftEmpty(LiftSide side, LiftLevel level)
        {
            bool hasPcb = LiftHasPcb(side, level);
            return !hasPcb;
        }

        // Lift 스토퍼 Up 센서/명령 (Front는 센서 1점, Rear는 IN/OUT 2코일)
        static bool IsLiftStopperUp(LiftSide side)
        {
            if (side == LiftSide.Front)
                return DIO.GetDIData(enumDINames.X403_7_Front_Lift_Stopper_Cyl_Sensor); // (원문 철자)
            return DIO.GetDIData(enumDINames.X405_3_Rear_Lift_Stopper_Cyl_IN_UP_Sensor);
        }

        static void CommandLiftStopper(LiftSide side, bool up)
        {
            if (side == LiftSide.Front)
            {
                // Front: 단일 코일 (ON=UP 가정)
                DIO.WriteDOData(enumDONames.Y300_1_Front_Lift_CONTACT_STOPPER_SOL, up);
            }
            else
            {
                // Rear: IN=UP, OUT=DOWN
                DIO.WriteDOData(enumDONames.Y302_0_Rear_Lift_CONTACT_STOPPER_IN_SOL, up);
                DIO.WriteDOData(enumDONames.Y300_0_Rear_Lift_CONTACT_STOPPER_Out_SOL, !up);
            }
        }

        // 사이트 컨택트가 올라왔는지(센서 있으면 DI, 없으면 Up/Down 코일 출력으로 판단)
        static bool EnsureSiteContactUp(enumTeachingPos site)
        {
            if (SiteIoMaps.TryGetContactUpDI(site, out var diUp))
                return DIO.GetDIData(diUp);

            // DI가 없으면 Up/Down DO로 판단/명령
            if (SiteIoMaps.TryGetContactUpDownDO(site, out var doUpDown))
            {
                // 출력상태 읽기가 가능하다고 가정 (없으면 항상 ON으로 명령 후 true 반환)
                if (!DIO.GetDORead(doUpDown))
                    DIO.WriteDOData(doUpDown, true);
                return true;
            }
            return true; // 마지막 수단: 안전측(Up 가정)
        }

        // 사이트 컨택트 Up 명령(필요시)
        static void CommandSiteContactUp(enumTeachingPos site)
        {
            if (SiteIoMaps.TryGetContactUpDownDO(site, out var doUpDown))
                DIO.WriteDOData(doUpDown, true);
        }

        // 사이트 스토퍼(클램프) 상태 — 센서가 없고 “Y 출력값”으로 확인 (네 요청 반영)
        static bool IsSiteStopperUp(enumTeachingPos site)
        {
            if (SiteIoMaps.TryGetContactStopperDO(site, out var doStopper))
                return DIO.GetDORead(doStopper); // ON=UP 가정
            return true; // 맵핑 없으면 안전측
        }

        static void StartMotorPair(enumDONames cw, enumDONames ccw, bool runCw, bool runCcw)
        {
            DIO.WriteDOData(cw, runCw);
            DIO.WriteDOData(ccw, runCcw);
        }
        static void StopMotorPair(enumDONames cw, enumDONames ccw)
        {
            DIO.WriteDOData(cw, false);
            DIO.WriteDOData(ccw, false);
        }

        // 공통: 사이트 → 리프트
        static bool Move_Site_To_Lift(enumTeachingPos destSite, LiftSide side, LiftLevel level,
                                      System.Diagnostics.Stopwatch actionWatch, bool passMode)
        {
            try
            {
                FuncInline.ResetInOutTime();

                // 맵핑
                if (!SiteIoMaps.TryGetSiteTransportMotorPair(destSite, out var siteCw, out var siteCcw)) return false;
                if (!SiteIoMaps.TryGetLiftMotors(level == LiftLevel.Up ? (side == LiftSide.Front ? enumTeachingPos.Lift1_Up : enumTeachingPos.Lift2_Up)
                                                                       : (side == LiftSide.Front ? enumTeachingPos.Lift1_Down : enumTeachingPos.Lift2_Down),
                                                 out var liftCw, out var liftCcw)) return false;

                // 타임아웃
                if (actionWatch.IsRunning && actionWatch.ElapsedMilliseconds > FuncInline.ConveyorTimeout * 1000)
                {
                    StopMotorPair(siteCw, siteCcw);
                    StopMotorPair(liftCw, liftCcw);
                    Util.InitWatch(ref actionWatch);
                    FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
                                                       DateTime.Now.ToString("HH:mm:ss"),
                                                       (side == LiftSide.Front ? FuncInline.enumErrorPart.Lift1_Up : FuncInline.enumErrorPart.Lift2_Up),
                                                       enumErrorCode.Conveyor_Timeout, false,
                                                       "Site→Lift 동작 타임아웃"));
                    return false;
                }

                // 완료조건: 사이트 비었고 Lift 구간은 PCB 감지
                bool siteDock = SiteIoMaps.TryGetPcbDockDI(destSite, out var diDock) && DIO.GetDIData(diDock);
                bool liftHas = LiftHasPcb(side, level);
                if (!siteDock && liftHas)
                {
                    StopMotorPair(siteCw, siteCcw);
                    StopMotorPair(liftCw, liftCcw);
                    Util.ResetWatch(ref actionWatch);
                    return true;
                }

                // 리프트 이젝션 포지션 정렬
                var ejectPos = GetLiftEjectPos(side, level, destSite);
                if (!CheckLiftPos(GetLiftName(side), ejectPos, 0.1))
                {
                    MoveLift(GetLiftName(side), ejectPos);
                    return false;
                }

                // Lift 스토퍼 UP
                if (!IsLiftStopperUp(side))
                {
                    CommandLiftStopper(side, true);
                    return false;
                }

                // 사이트 컨택트 UP (센서 없으면 Y 출력으로 강제 확인)
                if (!EnsureSiteContactUp(destSite))
                {
                    CommandSiteContactUp(destSite);
                    return false;
                }

                // PassMode면 Dock 감지 즉시 정지
                if (passMode && siteDock &&
                    (DIO.GetDORead(siteCw) || DIO.GetDORead(siteCcw) || DIO.GetDORead(liftCw) || DIO.GetDORead(liftCcw)))
                {
                    StopMotorPair(siteCw, siteCcw);
                    StopMotorPair(liftCw, liftCcw);
                    return false;
                }

                // 이송: 사이트→리프트는 CW/CW로 통일
                StartMotorPair(siteCw, siteCcw, true, false);
                StartMotorPair(liftCw, liftCcw, true, false);
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        // 공통: 리프트 → 사이트
        static bool Move_Lift_To_Site(enumTeachingPos destSite, LiftSide side, LiftLevel level,
                                      System.Diagnostics.Stopwatch actionWatch, bool passMode)
        {
            try
            {
                FuncInline.ResetInOutTime();

                if (!SiteIoMaps.TryGetSiteTransportMotorPair(destSite, out var siteCw, out var siteCcw)) return false;
                if (!SiteIoMaps.TryGetLiftMotors(level == LiftLevel.Up ? (side == LiftSide.Front ? enumTeachingPos.Lift1_Up : enumTeachingPos.Lift2_Up)
                                                                       : (side == LiftSide.Front ? enumTeachingPos.Lift1_Down : enumTeachingPos.Lift2_Down),
                                                 out var liftCw, out var liftCcw)) return false;

                // 타임아웃
                if (actionWatch.IsRunning && actionWatch.ElapsedMilliseconds > FuncInline.ConveyorTimeout * 1000)
                {
                    StopMotorPair(siteCw, siteCcw);
                    StopMotorPair(liftCw, liftCcw);
                    Util.InitWatch(ref actionWatch);
                    FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
                                                       DateTime.Now.ToString("HH:mm:ss"),
                                                       (side == LiftSide.Front ? FuncInline.enumErrorPart.Lift1_Up : FuncInline.enumErrorPart.Lift2_Up),
                                                       enumErrorCode.Conveyor_Timeout, false,
                                                       "Lift→Site 동작 타임아웃"));
                    return false;
                }

                // 완료조건: 사이트 Dock 감지 & Lift 구간 비움
                bool siteDock = SiteIoMaps.TryGetPcbDockDI(destSite, out var diDock) && DIO.GetDIData(diDock);
                bool liftEmpty = LiftEmpty(side, level);
                if (siteDock && liftEmpty)
                {
                    StopMotorPair(siteCw, siteCcw);
                    StopMotorPair(liftCw, liftCcw);
                    Util.ResetWatch(ref actionWatch);
                    return true;
                }

                // 리프트 이젝션 포지션 정렬
                var ejectPos = GetLiftEjectPos(side, level, destSite);
                if (!CheckLiftPos(GetLiftName(side), ejectPos, 0.1))
                {
                    MoveLift(GetLiftName(side), ejectPos);
                    return false;
                }

                // Lift 스토퍼 UP
                if (!IsLiftStopperUp(side))
                {
                    CommandLiftStopper(side, true);
                    return false;
                }

                // PassMode면 Dock 감지시 모터 정지
                if (passMode && siteDock &&
                    (DIO.GetDORead(siteCw) || DIO.GetDORead(siteCcw) || DIO.GetDORead(liftCw) || DIO.GetDORead(liftCcw)))
                {
                    StopMotorPair(siteCw, siteCcw);
                    StopMotorPair(liftCw, liftCcw);
                    return false;
                }

                // 이송: 리프트→사이트는 CCW/CCW로 통일
                StartMotorPair(siteCw, siteCcw, false, true);
                StartMotorPair(liftCw, liftCcw, false, true);
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        #endregion


        #region === 공개 함수 (6개) ===

        // site14~26 → Lift2 상단
        public static bool MoveSiteToLift2Up(enumTeachingPos destSite)
            => Move_Site_To_Lift(destSite, LiftSide.Rear, LiftLevel.Up, FuncInline.Lift2ActionWatch, FuncInline.PassMode);

        // site14~26 → Lift2 하단
        public static bool MoveSiteToLift2Down(enumTeachingPos destSite)
            => Move_Site_To_Lift(destSite, LiftSide.Rear, LiftLevel.Down, FuncInline.Lift2ActionWatch, FuncInline.PassMode);

        // site1~13 → FrontLift 상단
        public static bool MoveSiteToLift1Up(enumTeachingPos destSite)
            => Move_Site_To_Lift(destSite, LiftSide.Front, LiftLevel.Up, FuncInline.Lift1ActionWatch, FuncInline.PassMode);

        // site1~13 → FrontLift 하단
        public static bool MoveSiteToLift1Down(enumTeachingPos destSite)
            => Move_Site_To_Lift(destSite, LiftSide.Front, LiftLevel.Down, FuncInline.Lift1ActionWatch, FuncInline.PassMode);

        // Lift2 상단 → site14~26
        public static bool MoveLift2UpToSite(enumTeachingPos destSite)
            => Move_Lift_To_Site(destSite, LiftSide.Rear, LiftLevel.Up, FuncInline.Lift2ActionWatch, FuncInline.PassMode);

        // FrontLift 상단 → site1~13
        public static bool MoveLift1UpToSite(enumTeachingPos destSite)
            => Move_Lift_To_Site(destSite, LiftSide.Front, LiftLevel.Up, FuncInline.Lift1ActionWatch, FuncInline.PassMode);

        #endregion


        #region 사이트 관련 동작
        
        //// site14~26 에서  Lift2상단으로 PCB 이송
        //public static bool MoveSiteToLift2Up(enumTeachingPos destSite)
        //{
        //    try
        //    {
        //        FuncInline.ResetInOutTime();

        //        #region 목적지 확인
        //        if (destSite < enumTeachingPos.Site14_R_DT1 ||
        //            destSite > enumTeachingPos.Site26_R_FT3)
        //        {
        //            //debug("MoveSiteToLift2Up 목적지 사이트 아님 " + destSite.ToString());
        //            Util.ResetWatch(ref FuncInline.Lift2ActionWatch);
        //            FuncInline.Lift2Action = FuncInline.enumLiftAction.Waiting;
        //            return false;
        //        }
        //        #endregion

        //        int siteIndex = (int)destSite - (int)enumTeachingPos.Site1_F_DT1;          

        //        #region 타임아웃 처리
        //        if (//(FuncInline.PCBInfo[(int)destSite].StopWatch.IsRunning &&
        //            //        FuncInline.PCBInfo[(int)destSite].StopWatch.ElapsedMilliseconds > FuncInline.ConveyorTimeout * 1000) ||
        //            (FuncInline.Lift2ActionWatch.IsRunning &&
        //                    FuncInline.Lift2ActionWatch.ElapsedMilliseconds > FuncInline.ConveyorTimeout * 1000))
        //        {
        //            //debug("time : " + FuncInline.Lift2ActionWatch.ElapsedMilliseconds);
        //            Util.InitWatch(ref FuncInline.PCBInfo[(int)destSite].StopWatch);
        //            Util.InitWatch(ref FuncInline.Lift2ActionWatch);
        //            DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw, false);
        //            DIO.WriteDOData(enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw, false);

        //            // 사이트 핀업
        //            if (DIO.GetDIData(enumDINames.X12_2_Site1_PinClamp_Left_Down_Sensor + siteIndex * GlobalVar.DIModuleGap) ||
        //                DIO.GetDIData(enumDINames.X12_3_Site1_PinClamp_Right_Down_Sensor + siteIndex * GlobalVar.DIModuleGap))
        //            {
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex,
        //                                            enumErrorCode.Pin_Clamp_Up_Error,
        //                                            false,
        //                                            (FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex).ToString() + " pin down sensor detected."));
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Lift2_Up,
        //                                            enumErrorCode.Conveyor_Timeout,
        //                                            false,
        //                                            "Lift2 Up Side input action timed out"));
        //            }
        //            // 사이트 언클램프
        //            else if (!DIO.GetDIData(enumDINames.X12_1_Site1_Clamp_Backward_Sensor + siteIndex * GlobalVar.DIModuleGap))
        //            {
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex,
        //                                            enumErrorCode.Site_Clamp_Backward_Error,
        //                                            false,
        //                                            (FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex).ToString() + " clamp Backward sensor not detected."));
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Lift2_Up,
        //                                            enumErrorCode.Conveyor_Timeout,
        //                                            false,
        //                                            "Lift2 Up Side input action timed out"));
        //            }
        //            // 리프트 스토퍼
        //            else if (!DIO.GetDIData(enumDINames.X05_3_Rack1_DownLift_Stopper_Up_Sensor))
        //            {
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex,
        //                                            enumErrorCode.Conveyor_Timeout,
        //                                            false,
        //                                            (FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex).ToString() + " output action timed out"));
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Lift2_Up,
        //                                            enumErrorCode.Stopper_Up_Error,
        //                                            false,
        //                                            "Lift2 Up Side Stopper Up Sensor not detected"));
        //            }
        //            // 나머지
        //            else
        //            {
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex,
        //                                            enumErrorCode.Conveyor_Timeout,
        //                                            false,
        //                                            (FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex).ToString() + " output action timed out"));
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Lift2_Up,
        //                                            enumErrorCode.Conveyor_Timeout,
        //                                            false,
        //                                            "Lift2 Up Side input action timed out"));
        //            }
        //            return false;
        //        }
        //        #endregion

        //        #region 동작 완료
        //        if (!DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + siteIndex * GlobalVar.DIModuleGap) &&
        //            (DIO.GetDIData(enumDINames.X405_1_Rear_Lift_Up_PCB_Stop_Sensor) ||
        //                    DIO.GetDIData(enumDINames.X06_2_Rack2_UpLift_End_Sensor)))// &&
        //                                                                              //!DIO.GetDORead(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) &&
        //                                                                              //!DIO.GetDORead(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) &&
        //                                                                              //!DIO.GetDORead(enumDONames.Y305_1_Rear_Lift_Down_Motor_Cw) &&
        //                                                                              //!DIO.GetDORead(enumDONames.Y304_1_Rear_Lift_Down_Motor_Ccw))
        //        {
        //            //debug("동작 완료");
        //            DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw, false);
        //            DIO.WriteDOData(enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw, false);
        //            Util.ResetWatch(ref FuncInline.PCBInfo[(int)destSite].StopWatch);
        //            Util.ResetWatch(ref FuncInline.Lift2ActionWatch);
        //            return true;
        //        }
        //        #endregion

        //        #region 리프트 컨베어 동작중 리프트 동작중이라면 정지
        //        bool conv_run = DIO.GetDORead(enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw) ||
        //                            DIO.GetDORead(enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw);
        //        if (conv_run &&
        //            !GlobalVar.AxisStatus[(int)enumServoAxis.SV04_Lift2].StandStill)
        //        {
        //            DIO.WriteDOData(enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw, false);
        //            DIO.WriteDOData(enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw, false);
        //            FuncMotion.MoveStop((int)enumServoAxis.SV04_Lift2);
        //        }
        //        #endregion
        //        #region 사이트 컨베어 동작중 리프트 위치 확인. 다른 위치에서 컨베어 정방향으로 돌고 있다면 추락방지 위해 정지
        //        for (int i = 20; i < 40; i++)
        //        {
        //            if (!CheckLiftPos(enumLiftName.RearLift, FuncInline.enumLiftPos.Site1_F_DT1_Up + siteIndex, 0.1) &&
        //                (DIO.GetDORead(enumDONames.Y08_0_Site1_CW_Conveyor_Run + i * GlobalVar.DOModuleGap)))
        //            {
        //                DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + i * GlobalVar.DOModuleGap, false);
        //            }
        //        }
        //        #endregion

        //        #region 리프트 배출 위치 아니면 이동
        //        if (!CheckLiftPos(enumLiftName.RearLift, FuncInline.enumLiftPos.Site1_F_DT1_Up + siteIndex, 0.1))
        //        {
        //            MoveLift(enumLiftName.RearLift, FuncInline.enumLiftPos.Site1_F_DT1_Up + siteIndex);
        //        }
        //        #endregion
        //        #region 리프트2상단 스토퍼 상승 아니면 상승
        //        else if (CheckLiftPos(enumLiftName.RearLift, FuncInline.enumLiftPos.Site1_F_DT1_Up + siteIndex, 0.1) &&
        //            !DIO.GetDIData(enumDINames.X06_3_Rack2_UpLift_Stopper_Up_Sensor))
        //        {
        //            DIO.WriteDOData(enumDONames.Y05_2_Rack2_UpLift_Stopper_Up, true);
        //        }
        //        #endregion
        //        #region 사이트  핀클램핑상태면 언클램프
        //        else if (CheckLiftPos(enumLiftName.RearLift, FuncInline.enumLiftPos.Site1_F_DT1_Up + siteIndex, 0.1) &&
        //            (DIO.GetDIData(enumDINames.X12_2_Site1_PinClamp_Left_Down_Sensor + siteIndex * GlobalVar.DIModuleGap) ||
        //                    DIO.GetDIData(enumDINames.X12_3_Site1_PinClamp_Right_Down_Sensor + siteIndex * GlobalVar.DIModuleGap)))
        //        {
        //            #region 찐빠 확인시 내렸다가 다시 올린다
        //            /*
        //            if (!DIO.GetDORead(enumDONames.Y08_3_Site1_PinClamp_Down + siteIndex * GlobalVar.DOModuleGap) &&
        //                ((DIO.GetDIData(enumDINames.X12_2_Site1_PinClamp_Left_Down_Sensor + siteIndex * GlobalVar.DIModuleGap) &&
        //                        !DIO.GetDIData(enumDINames.X12_3_Site1_PinClamp_Right_Down_Sensor + siteIndex * GlobalVar.DIModuleGap)) ||
        //                    (!DIO.GetDIData(enumDINames.X12_2_Site1_PinClamp_Left_Down_Sensor + siteIndex * GlobalVar.DIModuleGap) &&
        //                        DIO.GetDIData(enumDINames.X12_3_Site1_PinClamp_Right_Down_Sensor + siteIndex * GlobalVar.DIModuleGap))))
        //            {
        //                DIO.WriteDOData(enumDONames.Y08_3_Site1_PinClamp_Down + siteIndex * GlobalVar.DOModuleGap, true);
        //                Thread.Sleep(GlobalVar.ThreadSleep * 2);
        //            }
        //            //*/
        //            #endregion
        //            DIO.WriteDOData(enumDONames.Y08_3_Site1_PinClamp_Down + siteIndex * GlobalVar.DOModuleGap, false);
        //        }
        //        #endregion
        //        #region 사이트 클램핑상태면 언클램프
        //        else if (CheckLiftPos(enumLiftName.RearLift, FuncInline.enumLiftPos.Site1_F_DT1_Up + siteIndex, 0.1) &&
        //            ((FuncInline.UseSiteClampSensor && !DIO.GetDIData(enumDINames.X12_1_Site1_Clamp_Backward_Sensor + siteIndex * GlobalVar.DIModuleGap)) ||
        //                    (!FuncInline.UseSiteClampSensor && DIO.GetDORead(enumDONames.Y08_2_Site1_Clamp_Forward + siteIndex * GlobalVar.DOModuleGap))))
        //        {
        //            DIO.WriteDOData(enumDONames.Y08_2_Site1_Clamp_Forward + siteIndex * GlobalVar.DOModuleGap, false);
        //        }
        //        #endregion
        //        #region 컨베어 두개 동작 아니면 동작
        //        else if (CheckLiftPos(enumLiftName.RearLift, FuncInline.enumLiftPos.Site1_F_DT1_Up + siteIndex, 0.1) &&
        //            (!DIO.GetDORead(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) ||
        //                    DIO.GetDORead(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) ||
        //                    !DIO.GetDORead(enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw) ||
        //                    DIO.GetDORead(enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw)))
        //        {
        //            DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, true);
        //            DIO.WriteDOData(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw, true);
        //            DIO.WriteDOData(enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw, false);
        //        }
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        debug(ex.ToString());
        //        debug(ex.StackTrace);
        //    }

        //    return false;
        //}

        //// site14~26 에서  Lift2하단으로 PCB 이송
        //public static bool MoveSiteToLift2Down(enumTeachingPos destSite)
        //{
        //    try
        //    {
        //        FuncInline.ResetInOutTime();

               
        //        if (destSite < enumTeachingPos.Site14_R_DT1 ||
        //            destSite > enumTeachingPos.Site26_R_FT3)
        //        {
        //            //debug("MoveSiteToLift2Down 목적지 사이트 아님 " + destSite.ToString());
        //            Util.ResetWatch(ref FuncInline.Lift2ActionWatch);
        //            FuncInline.Lift2Action = FuncInline.enumLiftAction.Waiting;
        //            return false;
        //        }

        //        int siteIndex = (int)destSite - (int)enumTeachingPos.Site1_F_DT1;

        //        #region PCB 정보 없으면 액션 삭제
        //        /*
        //        if (FuncInline.PCBInfo[(int)destSite].PCBStatus == FuncInline.enumSMDStatus.UnKnown)
        //        {
        //            //debug("MoveSiteToLift2Down PCB 정보 없으면 액션 삭제");
        //            Util.ResetWatch(ref FuncInline.PCBInfo[(int)destSite].StopWatch);
        //            Util.ResetWatch(ref FuncInline.Lift2ActionWatch);
        //            FuncInline.Lift2Action = FuncInline.enumLiftAction.Waiting;
        //            DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y305_1_Rear_Lift_Down_Motor_Cw, false);
        //            DIO.WriteDOData(enumDONames.Y304_1_Rear_Lift_Down_Motor_Ccw, false);
        //            return false;
        //        }
        //        //*/
        //        #endregion
               

        //        // 타임아웃 처리
        //        if (//(FuncInline.PCBInfo[(int)destSite].StopWatch.IsRunning &&
        //            //        FuncInline.PCBInfo[(int)destSite].StopWatch.ElapsedMilliseconds > FuncInline.ConveyorTimeout * 1000) ||
        //            (FuncInline.Lift2ActionWatch.IsRunning &&
        //                    FuncInline.Lift2ActionWatch.ElapsedMilliseconds > FuncInline.ConveyorTimeout * 1000))
        //        {
        //            //debug("time : " + FuncInline.Lift2ActionWatch.ElapsedMilliseconds);
        //            Util.InitWatch(ref FuncInline.PCBInfo[(int)destSite].StopWatch);
        //            Util.InitWatch(ref FuncInline.Lift2ActionWatch);
        //            DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y305_1_Rear_Lift_Down_Motor_Cw, false);
        //            DIO.WriteDOData(enumDONames.Y304_1_Rear_Lift_Down_Motor_Ccw, false);

        //            // 사이트 핀업
        //            if (DIO.GetDIData(enumDINames.X12_2_Site1_PinClamp_Left_Down_Sensor + siteIndex * GlobalVar.DIModuleGap) ||
        //                DIO.GetDIData(enumDINames.X12_3_Site1_PinClamp_Right_Down_Sensor + siteIndex * GlobalVar.DIModuleGap))
        //            {
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex,
        //                                            enumErrorCode.Pin_Clamp_Up_Error,
        //                                            false,
        //                                            (FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex).ToString() + " pin down sensor detected."));
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Lift2_Down,
        //                                            enumErrorCode.Conveyor_Timeout,
        //                                            false,
        //                                            "Lift2 Down Side input action timed out"));
        //            }
        //            // 사이트 언클램프
        //            else if (!DIO.GetDIData(enumDINames.X12_1_Site1_Clamp_Backward_Sensor + siteIndex * GlobalVar.DIModuleGap))
        //            {
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex,
        //                                            enumErrorCode.Site_Clamp_Backward_Error,
        //                                            false,
        //                                            (FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex).ToString() + " clamp Backward sensor not detected."));
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Lift2_Down,
        //                                            enumErrorCode.Conveyor_Timeout,
        //                                            false,
        //                                            "Lift2 Down Side input action timed out"));
        //            }
        //            // 리프트 스토퍼
        //            else if (!DIO.GetDIData(enumDINames.X05_3_Rack1_DownLift_Stopper_Up_Sensor))
        //            {
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex,
        //                                            enumErrorCode.Conveyor_Timeout,
        //                                            false,
        //                                            (FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex).ToString() + " output action timed out"));
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Lift2_Down,
        //                                            enumErrorCode.Stopper_Up_Error,
        //                                            false,
        //                                            "Lift2 Down Side Stopper Up Sensor not detected"));
        //            }
        //            // 나머지
        //            else
        //            {
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex,
        //                                            enumErrorCode.Conveyor_Timeout,
        //                                            false,
        //                                            (FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex).ToString() + " output action timed out"));
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Lift2_Down,
        //                                            enumErrorCode.Conveyor_Timeout,
        //                                            false,
        //                                            "Lift2 Down Side input action timed out"));
        //            }
        //            return false;
        //        }

        //        // 동작 완료
        //        if (!DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + siteIndex) &&
        //            (DIO.GetDIData(enumDINames.X405_7_Rear_Lift_Down_PCB_Stop_Sensor) ||
        //                    DIO.GetDIData(enumDINames.X07_2_Rack2_DownLift_End_Sensor)))// &&
        //                                                                                //!DIO.GetDORead(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) &&
        //                                                                                //!DIO.GetDORead(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) &&
        //                                                                                //!DIO.GetDORead(enumDONames.Y305_1_Rear_Lift_Down_Motor_Cw) &&
        //                                                                                //!DIO.GetDORead(enumDONames.Y304_1_Rear_Lift_Down_Motor_Ccw))
        //        {
        //            DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y305_1_Rear_Lift_Down_Motor_Cw, false);
        //            DIO.WriteDOData(enumDONames.Y304_1_Rear_Lift_Down_Motor_Ccw, false);
        //            Util.ResetWatch(ref FuncInline.PCBInfo[(int)destSite].StopWatch);
        //            Util.ResetWatch(ref FuncInline.Lift2ActionWatch);
        //            return true;
        //        }

        //        #region 리프트 컨베어 동작중 리프트 동작중이라면 정지
        //        bool conv_run = DIO.GetDORead(enumDONames.Y305_1_Rear_Lift_Down_Motor_Cw) ||
        //                            DIO.GetDORead(enumDONames.Y304_1_Rear_Lift_Down_Motor_Ccw);
        //        if (conv_run &&
        //            !GlobalVar.AxisStatus[(int)enumServoAxis.SV04_Lift2].StandStill)
        //        {
        //            DIO.WriteDOData(enumDONames.Y305_1_Rear_Lift_Down_Motor_Cw, false);
        //            DIO.WriteDOData(enumDONames.Y304_1_Rear_Lift_Down_Motor_Ccw, false);
        //            FuncMotion.MoveStop((int)enumServoAxis.SV04_Lift2);
        //        }
        //        #endregion
        //        #region 사이트 컨베어 동작중 리프트 위치 확인. 다른 위치에서 컨베어 정방향으로 돌고 있다면 추락방지 위해 정지
        //        for (int i = 20; i < 40; i++)
        //        {
        //            if (!CheckLiftPos(enumLiftName.RearLift, FuncInline.enumLiftPos.Site1_F_DT1_Down + siteIndex, 0.1) &&
        //                (DIO.GetDORead(enumDONames.Y08_0_Site1_CW_Conveyor_Run + i * GlobalVar.DOModuleGap)))
        //            {
        //                DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + i * GlobalVar.DOModuleGap, false);
        //            }
        //        }
        //        #endregion

        //        // 리프트 배출 위치 아니면 이동
        //        if (!CheckLiftPos(enumLiftName.RearLift, FuncInline.enumLiftPos.Site1_F_DT1_Down + siteIndex, 0.1))
        //        {
        //            MoveLift(enumLiftName.RearLift, FuncInline.enumLiftPos.Site1_F_DT1_Down + siteIndex);
        //        }
        //        // 리프트2하단 스토퍼 상승 아니면 상승
        //        else if (CheckLiftPos(enumLiftName.RearLift, FuncInline.enumLiftPos.Site1_F_DT1_Down + siteIndex, 0.1) &&
        //            !DIO.GetDIData(enumDINames.X07_3_Rack2_DownLift_Stopper_Up_Sensor))
        //        {
        //            DIO.WriteDOData(enumDONames.Y05_6_Rack2_DownLift_Stopper_Up, true);
        //        }
        //        // 사이트  핀클램핑상태면 언클램프
        //        else if (CheckLiftPos(enumLiftName.RearLift, FuncInline.enumLiftPos.Site1_F_DT1_Down + siteIndex, 0.1) &&
        //            (DIO.GetDIData(enumDINames.X12_2_Site1_PinClamp_Left_Down_Sensor + siteIndex * GlobalVar.DIModuleGap) ||
        //                    DIO.GetDIData(enumDINames.X12_3_Site1_PinClamp_Right_Down_Sensor + siteIndex * GlobalVar.DIModuleGap)))
        //        {
                    
        //            DIO.WriteDOData(enumDONames.Y08_3_Site1_PinClamp_Down + siteIndex * GlobalVar.DOModuleGap, false);
        //        }
        //        // 사이트 클램핑상태면 언클램프
        //        else if (CheckLiftPos(enumLiftName.RearLift, FuncInline.enumLiftPos.Site1_F_DT1_Down + siteIndex, 0.1) &&
        //            (FuncInline.UseSiteClampSensor && !DIO.GetDIData(enumDINames.X12_1_Site1_Clamp_Backward_Sensor + siteIndex * GlobalVar.DIModuleGap)) ||
        //                    (!FuncInline.UseSiteClampSensor && DIO.GetDORead(enumDONames.Y08_2_Site1_Clamp_Forward + siteIndex * GlobalVar.DOModuleGap)))
        //        {
        //            DIO.WriteDOData(enumDONames.Y08_2_Site1_Clamp_Forward + siteIndex * GlobalVar.DOModuleGap, false);
        //        }
        //        // 컨베어 두개 동작 아니면 동작
        //        else if (CheckLiftPos(enumLiftName.RearLift, FuncInline.enumLiftPos.Site1_F_DT1_Down + siteIndex, 0.1) &&
        //            (!DIO.GetDORead(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) ||
        //                    DIO.GetDORead(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) ||
        //                    !DIO.GetDORead(enumDONames.Y305_1_Rear_Lift_Down_Motor_Cw) ||
        //                    DIO.GetDORead(enumDONames.Y304_1_Rear_Lift_Down_Motor_Ccw)))
        //        {
        //            DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, true);
        //            DIO.WriteDOData(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y305_1_Rear_Lift_Down_Motor_Cw, true);
        //            DIO.WriteDOData(enumDONames.Y304_1_Rear_Lift_Down_Motor_Ccw, false);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        debug(ex.ToString());
        //        debug(ex.StackTrace);
        //    }

        //    return false;
        //}

        //// site1~13 에서  FrontLift상단으로 PCB 이송
        //public static bool MoveSiteToLift1Up(enumTeachingPos destSite)
        //{
        //    try
        //    {
        //        FuncInline.ResetInOutTime();

        //        #region 목적지 확인
        //        if (destSite < enumTeachingPos.Site1_F_DT1 ||
        //            destSite > enumTeachingPos.Site13_F_FT3)
        //        {
        //            //debug("MoveSiteToLift1Up 목적지 사이트 아님 " + destSite.ToString());
        //            Util.ResetWatch(ref FuncInline.Lift1ActionWatch);
        //            FuncInline.Lift1Action = FuncInline.enumLiftAction.Waiting;
        //            return false;
        //        }
        //        #endregion

        //        int siteIndex = (int)destSite - (int)enumTeachingPos.Site1_F_DT1;

        //        #region PCB 정보 없으면 액션 삭제
        //        /*
        //        if (FuncInline.PCBInfo[(int)destSite].PCBStatus == FuncInline.enumSMDStatus.UnKnown)
        //        {
        //            //debug("MoveSiteToLift1Up PCB 정보 없으면 액션 삭제");
        //            Util.ResetWatch(ref FuncInline.PCBInfo[(int)destSite].StopWatch);
        //            Util.ResetWatch(ref FuncInline.Lift2ActionWatch);
        //            FuncInline.Lift1Action = FuncInline.enumLiftAction.Waiting;
        //            DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y405_0_Front_Lift_Up_Motor_Cw, false);
        //            DIO.WriteDOData(enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw, false);
        //            return false;
        //        }
        //        //*/
        //        #endregion
        //        #region PCB 정보 있으면 액션 삭제
        //        /*
        //        if (FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
        //        {
        //            //debug("MoveSiteToLift1Up PCB 정보 있으면 액션 삭제");
        //            Util.ResetWatch(ref FuncInline.PCBInfo[(int)destSite].StopWatch);
        //            Util.ResetWatch(ref FuncInline.Lift2ActionWatch);
        //            FuncInline.Lift1Action = FuncInline.enumLiftAction.Waiting;
        //            DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y405_0_Front_Lift_Up_Motor_Cw, false);
        //            DIO.WriteDOData(enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw, false);
        //            return false;
        //        }
        //        //*/
        //        #endregion

        //        #region 타임아웃 처리
        //        if (//(FuncInline.PCBInfo[(int)destSite].StopWatch.IsRunning &&
        //            //        FuncInline.PCBInfo[(int)destSite].StopWatch.ElapsedMilliseconds > FuncInline.ConveyorTimeout * 1000) ||
        //            (FuncInline.Lift1ActionWatch.IsRunning &&
        //                    FuncInline.Lift1ActionWatch.ElapsedMilliseconds > FuncInline.ConveyorTimeout * 1000))
        //        {
        //            //debug("time : " + FuncInline.Lift1ActionWatch.ElapsedMilliseconds);
        //            Util.InitWatch(ref FuncInline.PCBInfo[(int)destSite].StopWatch);
        //            Util.InitWatch(ref FuncInline.Lift1ActionWatch);
        //            DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y405_0_Front_Lift_Up_Motor_Cw, false);
        //            DIO.WriteDOData(enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw, false);

        //            // 사이트 핀업
        //            if (DIO.GetDIData(enumDINames.X12_2_Site1_PinClamp_Left_Down_Sensor + siteIndex * GlobalVar.DIModuleGap) ||
        //                DIO.GetDIData(enumDINames.X12_3_Site1_PinClamp_Right_Down_Sensor + siteIndex * GlobalVar.DIModuleGap))
        //            {
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex,
        //                                            enumErrorCode.Pin_Clamp_Up_Error,
        //                                            false,
        //                                            (FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex).ToString() + " pin down sensor detected."));
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Lift1_Up,
        //                                            enumErrorCode.Conveyor_Timeout,
        //                                            false,
        //                                            "FrontLift Up Side input action timed out"));
        //            }
        //            // 사이트 언클램프
        //            else if (!DIO.GetDIData(enumDINames.X12_1_Site1_Clamp_Backward_Sensor + siteIndex * GlobalVar.DIModuleGap))
        //            {
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex,
        //                                            enumErrorCode.Site_Clamp_Backward_Error,
        //                                            false,
        //                                            (FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex).ToString() + " clamp Backward sensor not detected."));
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Lift1_Up,
        //                                            enumErrorCode.Conveyor_Timeout,
        //                                            false,
        //                                            "FrontLift Up Side input action timed out"));
        //            }
        //            // 리프트 스토퍼
        //            else if (!DIO.GetDIData(enumDINames.X05_3_Rack1_DownLift_Stopper_Up_Sensor))
        //            {
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex,
        //                                            enumErrorCode.Conveyor_Timeout,
        //                                            false,
        //                                            (FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex).ToString() + " output action timed out"));
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Lift1_Up,
        //                                            enumErrorCode.Stopper_Up_Error,
        //                                            false,
        //                                            "FrontLift Up Side Stopper Up Sensor not detected"));
        //            }
        //            // 나머지
        //            else
        //            {
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex,
        //                                            enumErrorCode.Conveyor_Timeout,
        //                                            false,
        //                                            (FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex).ToString() + " output action timed out"));
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Lift1_Up,
        //                                            enumErrorCode.Conveyor_Timeout,
        //                                            false,
        //                                            "FrontLift Up Side input action timed out"));
        //            }
        //            return false;
        //        }
        //        #endregion

        //        #region 동작 완료
        //        if (!DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + siteIndex * GlobalVar.DIModuleGap) &&
        //            (DIO.GetDIData(enumDINames.X04_1_Rack1_UpLift_Stop_Sensor) ||
        //                    DIO.GetDIData(enumDINames.X403_5_Front_Lift_Up_PCB_Stop_Sensor))) // &&
        //                                                                               //!DIO.GetDORead(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) &&
        //                                                                               //!DIO.GetDORead(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) &&
        //                                                                               //!DIO.GetDORead(enumDONames.Y405_4_Front_Lift_Down_Motor_Cw) &&
        //                                                                               //!DIO.GetDORead(enumDONames.Y405_6_Front_Lift_Down_Motor_Ccw))
        //        {
        //            DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y405_0_Front_Lift_Up_Motor_Cw, false);
        //            DIO.WriteDOData(enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw, false);
        //            Util.ResetWatch(ref FuncInline.PCBInfo[(int)destSite].StopWatch);
        //            Util.ResetWatch(ref FuncInline.Lift1ActionWatch);
        //            return true;
        //        }
        //        #endregion

        //        #region 리프트 컨베어 동작중 리프트 동작중이라면 정지
        //        bool conv_run = DIO.GetDORead(enumDONames.Y405_0_Front_Lift_Up_Motor_Cw) ||
        //                            DIO.GetDORead(enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw);
        //        if (conv_run &&
        //            !GlobalVar.AxisStatus[(int)enumServoAxis.SV02_Lift1].StandStill)
        //        {
        //            DIO.WriteDOData(enumDONames.Y405_0_Front_Lift_Up_Motor_Cw, false);
        //            DIO.WriteDOData(enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw, false);
        //            FuncMotion.MoveStop((int)enumServoAxis.SV02_Lift1);
        //        }
        //        #endregion
        //        #region 사이트 컨베어 동작중 리프트 위치 확인. 다른 위치에서 컨베어 정방향으로 돌고 있다면 추락방지 위해 정지
        //        for (int i = 0; i < 20; i++)
        //        {
        //            if (!CheckLiftPos(enumLiftName.FrontLift, FuncInline.enumLiftPos.Site1_F_DT1_Up + siteIndex, 0.1) &&
        //                (DIO.GetDORead(enumDONames.Y08_0_Site1_CW_Conveyor_Run + i * GlobalVar.DOModuleGap)))
        //            {
        //                DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + i * GlobalVar.DOModuleGap, false);
        //            }
        //        }
        //        #endregion

        //        #region 리프트 배출 위치 아니면 이동
        //        if (!CheckLiftPos(enumLiftName.FrontLift, FuncInline.enumLiftPos.Site1_F_DT1_Up + siteIndex, 0.1))
        //        {
        //            MoveLift(enumLiftName.FrontLift, FuncInline.enumLiftPos.Site1_F_DT1_Up + siteIndex);
        //        }
        //        #endregion
        //        #region 리프트1상단 스토퍼 상승 아니면 상승
        //        else if (CheckLiftPos(enumLiftName.FrontLift, FuncInline.enumLiftPos.Site1_F_DT1_Up + siteIndex, 0.1) &&
        //            !DIO.GetDIData(enumDINames.X04_3_Rack1_UpLift_Stopper_Up_Sensor))
        //        {
        //            DIO.WriteDOData(enumDONames.Y04_2_Rack1_UpLift_Stopper_Up, true);
        //        }
        //        #endregion
        //        #region 사이트  핀클램핑상태면 언클램프
        //        else if (CheckLiftPos(enumLiftName.FrontLift, FuncInline.enumLiftPos.Site1_F_DT1_Up + siteIndex, 0.1) &&
        //            (DIO.GetDIData(enumDINames.X12_2_Site1_PinClamp_Left_Down_Sensor + siteIndex * GlobalVar.DIModuleGap) ||
        //                    DIO.GetDIData(enumDINames.X12_3_Site1_PinClamp_Right_Down_Sensor + siteIndex * GlobalVar.DIModuleGap)))
        //        {
        //            #region 찐빠 확인시 내렸다가 다시 올린다
        //            /*
        //            if (!DIO.GetDORead(enumDONames.Y08_3_Site1_PinClamp_Down + siteIndex * GlobalVar.DOModuleGap) &&
        //                ((DIO.GetDIData(enumDINames.X12_2_Site1_PinClamp_Left_Down_Sensor + siteIndex * GlobalVar.DIModuleGap) &&
        //                        !DIO.GetDIData(enumDINames.X12_3_Site1_PinClamp_Right_Down_Sensor + siteIndex * GlobalVar.DIModuleGap)) ||
        //                    (!DIO.GetDIData(enumDINames.X12_2_Site1_PinClamp_Left_Down_Sensor + siteIndex * GlobalVar.DIModuleGap) &&
        //                        DIO.GetDIData(enumDINames.X12_3_Site1_PinClamp_Right_Down_Sensor + siteIndex * GlobalVar.DIModuleGap))))
        //            {
        //                DIO.WriteDOData(enumDONames.Y08_3_Site1_PinClamp_Down + siteIndex * GlobalVar.DOModuleGap, true);
        //                Thread.Sleep(GlobalVar.ThreadSleep * 2);
        //            }
        //            //*/
        //            #endregion
        //            DIO.WriteDOData(enumDONames.Y08_3_Site1_PinClamp_Down + siteIndex * GlobalVar.DOModuleGap, false);
        //        }
        //        #endregion
        //        #region 사이트 클램핑상태면 언클램프
        //        else if (CheckLiftPos(enumLiftName.FrontLift, FuncInline.enumLiftPos.Site1_F_DT1_Up + siteIndex, 0.1) &&
        //            (FuncInline.UseSiteClampSensor && !DIO.GetDIData(enumDINames.X12_1_Site1_Clamp_Backward_Sensor + siteIndex * GlobalVar.DIModuleGap)) ||
        //                    (!FuncInline.UseSiteClampSensor && DIO.GetDORead(enumDONames.Y08_2_Site1_Clamp_Forward + siteIndex * GlobalVar.DOModuleGap)))
        //        {
        //            DIO.WriteDOData(enumDONames.Y08_2_Site1_Clamp_Forward + siteIndex * GlobalVar.DOModuleGap, false);
        //        }
        //        #endregion
        //        #region 컨베어 두개 동작 아니면 동작
        //        else if (CheckLiftPos(enumLiftName.FrontLift, FuncInline.enumLiftPos.Site1_F_DT1_Up + siteIndex, 0.1) &&
        //            (!DIO.GetDORead(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) ||
        //                    DIO.GetDORead(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) ||
        //                    !DIO.GetDORead(enumDONames.Y405_0_Front_Lift_Up_Motor_Cw) ||
        //                    DIO.GetDORead(enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw)))
        //        {
        //            DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, true);
        //            DIO.WriteDOData(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y405_0_Front_Lift_Up_Motor_Cw, true);
        //            DIO.WriteDOData(enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw, false);
        //        }
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        debug(ex.ToString());
        //        debug(ex.StackTrace);
        //    }

        //    return false;
        //}

        //// site1~13 에서  FrontLift하단으로 PCB 이송
        //public static bool MoveSiteToLift1Down(enumTeachingPos destSite)
        //{
        //    try
        //    {
        //        FuncInline.ResetInOutTime();

        //        #region 목적지 확인
        //        if (destSite < enumTeachingPos.Site1_F_DT1 ||
        //            destSite > enumTeachingPos.Site13_F_FT3)
        //        {
        //            //debug("MoveSiteToLift1Down 목적지 사이트 아님 " + destSite.ToString());
        //            Util.ResetWatch(ref FuncInline.Lift1ActionWatch);
        //            FuncInline.Lift1Action = FuncInline.enumLiftAction.Waiting;
        //            return false;
        //        }
        //        #endregion

        //        int siteIndex = (int)destSite - (int)enumTeachingPos.Site1_F_DT1;

        //        #region PCB 정보 없으면 액션 삭제
        //        /*
        //        if (FuncInline.PCBInfo[(int)destSite].PCBStatus == FuncInline.enumSMDStatus.UnKnown)
        //        {
        //            //debug("MoveSiteToLift1Down PCB 정보 없으면 액션 삭제");
        //            Util.ResetWatch(ref FuncInline.PCBInfo[(int)destSite].StopWatch);
        //            Util.ResetWatch(ref FuncInline.Lift2ActionWatch);
        //            FuncInline.Lift1Action = FuncInline.enumLiftAction.Waiting;
        //            DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y405_4_Front_Lift_Down_Motor_Cw, false);
        //            DIO.WriteDOData(enumDONames.Y405_6_Front_Lift_Down_Motor_Ccw, false);
        //            return false;
        //        }
        //        //*/
        //        #endregion
        //        #region PCB 정보 있으면 액션 삭제
        //        /*
        //        if (FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Down].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
        //        {
        //            //debug("MoveSiteToLift1Down PCB 정보 있으면 액션 삭제");
        //            Util.ResetWatch(ref FuncInline.PCBInfo[(int)destSite].StopWatch);
        //            Util.ResetWatch(ref FuncInline.Lift2ActionWatch);
        //            FuncInline.Lift1Action = FuncInline.enumLiftAction.Waiting;
        //            DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y405_4_Front_Lift_Down_Motor_Cw, false);
        //            DIO.WriteDOData(enumDONames.Y405_6_Front_Lift_Down_Motor_Ccw, false);
        //            return false;
        //        }
        //        //*/
        //        #endregion


        //        #region 타임아웃 처리
        //        if (//(FuncInline.PCBInfo[(int)destSite].StopWatch.IsRunning &&
        //            //        FuncInline.PCBInfo[(int)destSite].StopWatch.ElapsedMilliseconds > FuncInline.ConveyorTimeout * 1000) ||
        //            (FuncInline.Lift1ActionWatch.IsRunning &&
        //                    FuncInline.Lift1ActionWatch.ElapsedMilliseconds > FuncInline.ConveyorTimeout * 1000))
        //        {
        //            //debug("time : " + FuncInline.Lift1ActionWatch.ElapsedMilliseconds);
        //            Util.InitWatch(ref FuncInline.PCBInfo[(int)destSite].StopWatch);
        //            Util.InitWatch(ref FuncInline.Lift1ActionWatch);
        //            DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y405_4_Front_Lift_Down_Motor_Cw, false);
        //            DIO.WriteDOData(enumDONames.Y405_6_Front_Lift_Down_Motor_Ccw, false);

        //            // 사이트 핀업
        //            if (DIO.GetDIData(enumDINames.X12_2_Site1_PinClamp_Left_Down_Sensor + siteIndex * GlobalVar.DIModuleGap) ||
        //                DIO.GetDIData(enumDINames.X12_3_Site1_PinClamp_Right_Down_Sensor + siteIndex * GlobalVar.DIModuleGap))
        //            {
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex,
        //                                            enumErrorCode.Pin_Clamp_Up_Error,
        //                                            false,
        //                                            (FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex).ToString() + " pin down sensor detected."));
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Rack1_Lift_Down,
        //                                            enumErrorCode.Conveyor_Timeout,
        //                                            false,
        //                                            "FrontLift Down Side input action timed out"));
        //            }
        //            // 사이트 언클램프
        //            else if (!DIO.GetDIData(enumDINames.X12_1_Site1_Clamp_Backward_Sensor + siteIndex * GlobalVar.DIModuleGap))
        //            {
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex,
        //                                            enumErrorCode.Site_Clamp_Backward_Error,
        //                                            false,
        //                                            (FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex).ToString() + " clamp Backward sensor not detected."));
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Rack1_Lift_Down,
        //                                            enumErrorCode.Conveyor_Timeout,
        //                                            false,
        //                                            "FrontLift Down Side input action timed out"));
        //            }
        //            // 리프트 스토퍼
        //            else if (!DIO.GetDIData(enumDINames.X05_3_Rack1_DownLift_Stopper_Up_Sensor))
        //            {
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex,
        //                                            enumErrorCode.Conveyor_Timeout,
        //                                            false,
        //                                            (FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex).ToString() + " output action timed out"));
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Rack1_Lift_Down,
        //                                            enumErrorCode.Stopper_Up_Error,
        //                                            false,
        //                                            "FrontLift Down Side Stopper Up Sensor not detected"));
        //            }
        //            // 나머지
        //            else
        //            {
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex,
        //                                            enumErrorCode.Conveyor_Timeout,
        //                                            false,
        //                                            (FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex).ToString() + " output action timed out"));
        //                FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                            DateTime.Now.ToString("HH:mm:ss"),
        //                                            FuncInline.enumErrorPart.Rack1_Lift_Down,
        //                                            enumErrorCode.Conveyor_Timeout,
        //                                            false,
        //                                            "FrontLift Down Side input action timed out"));
        //            }
        //            return false;
        //        }
        //        #endregion

        //        #region 동작 완료
        //        if (!DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + siteIndex * GlobalVar.DIModuleGap) &&
        //            (DIO.GetDIData(enumDINames.X400_2_Front_Lift_Down_PCB_Stop_Sensor) ||
        //                    DIO.GetDIData(enumDINames.X05_2_Rack1_DownLift_End_Sensor))) // &&
        //                                                                                 //!DIO.GetDORead(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) &&
        //                                                                                 //!DIO.GetDORead(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) &&
        //                                                                                 //!DIO.GetDORead(enumDONames.Y405_4_Front_Lift_Down_Motor_Cw) &&
        //                                                                                 //!DIO.GetDORead(enumDONames.Y405_6_Front_Lift_Down_Motor_Ccw))
        //        {
        //            DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y405_4_Front_Lift_Down_Motor_Cw, false);
        //            DIO.WriteDOData(enumDONames.Y405_6_Front_Lift_Down_Motor_Ccw, false);
        //            Util.ResetWatch(ref FuncInline.PCBInfo[(int)destSite].StopWatch);
        //            Util.ResetWatch(ref FuncInline.Lift1ActionWatch);
        //            return true;
        //        }
        //        #endregion

        //        #region 리프트 컨베어 동작중 리프트 동작중이라면 정지
        //        bool conv_run = DIO.GetDORead(enumDONames.Y405_4_Front_Lift_Down_Motor_Cw) ||
        //                            DIO.GetDORead(enumDONames.Y405_6_Front_Lift_Down_Motor_Ccw);
        //        if (conv_run &&
        //            !GlobalVar.AxisStatus[(int)enumServoAxis.SV02_Lift1].StandStill)
        //        {
        //            DIO.WriteDOData(enumDONames.Y405_4_Front_Lift_Down_Motor_Cw, false);
        //            DIO.WriteDOData(enumDONames.Y405_6_Front_Lift_Down_Motor_Ccw, false);
        //            FuncMotion.MoveStop((int)enumServoAxis.SV02_Lift1);
        //        }
        //        #endregion
        //        #region 사이트 컨베어 동작중 리프트 위치 확인. 다른 위치에서 컨베어 정방향으로 돌고 있다면 추락방지 위해 정지
        //        for (int i = 0; i < 20; i++)
        //        {
        //            if (!CheckLiftPos(enumLiftName.FrontLift, FuncInline.enumLiftPos.Site1_F_DT1_Down + siteIndex, 0.1) &&
        //                (DIO.GetDORead(enumDONames.Y08_0_Site1_CW_Conveyor_Run + i * GlobalVar.DOModuleGap)))
        //            {
        //                DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + i * GlobalVar.DOModuleGap, false);
        //            }
        //        }
        //        #endregion

        //        #region 리프트 배출 위치 아니면 이동
        //        if (!CheckLiftPos(enumLiftName.FrontLift, FuncInline.enumLiftPos.Site1_F_DT1_Down + siteIndex, 0.1))
        //        {
        //            MoveLift(enumLiftName.FrontLift, FuncInline.enumLiftPos.Site1_F_DT1_Down + siteIndex);
        //        }
        //        #endregion
        //        #region 리프트1하단 스토퍼 상승 아니면 상승
        //        else if (CheckLiftPos(enumLiftName.FrontLift, FuncInline.enumLiftPos.Site1_F_DT1_Down + siteIndex, 0.1) &&
        //            CheckLiftPos(enumLiftName.FrontLift, FuncInline.enumLiftPos.Site1_F_DT1_Down + siteIndex, 0.1) &&
        //            !DIO.GetDIData(enumDINames.X05_3_Rack1_DownLift_Stopper_Up_Sensor))
        //        {
        //            DIO.WriteDOData(enumDONames.Y04_6_Rack1_DownLift_Stopper_Up, true);
        //        }
        //        #endregion
        //        #region 사이트  핀클램핑상태면 언클램프
        //        else if (CheckLiftPos(enumLiftName.FrontLift, FuncInline.enumLiftPos.Site1_F_DT1_Down + siteIndex, 0.1) &&
        //            (DIO.GetDIData(enumDINames.X12_2_Site1_PinClamp_Left_Down_Sensor + siteIndex * GlobalVar.DIModuleGap) ||
        //                    DIO.GetDIData(enumDINames.X12_3_Site1_PinClamp_Right_Down_Sensor + siteIndex * GlobalVar.DIModuleGap)))
        //        {
        //            #region 찐빠 확인시 내렸다가 다시 올린다
        //            /*
        //            if (!DIO.GetDORead(enumDONames.Y08_3_Site1_PinClamp_Down + siteIndex * GlobalVar.DOModuleGap) &&
        //                ((DIO.GetDIData(enumDINames.X12_2_Site1_PinClamp_Left_Down_Sensor + siteIndex * GlobalVar.DIModuleGap) &&
        //                        !DIO.GetDIData(enumDINames.X12_3_Site1_PinClamp_Right_Down_Sensor + siteIndex * GlobalVar.DIModuleGap)) ||
        //                    (!DIO.GetDIData(enumDINames.X12_2_Site1_PinClamp_Left_Down_Sensor + siteIndex * GlobalVar.DIModuleGap) &&
        //                        DIO.GetDIData(enumDINames.X12_3_Site1_PinClamp_Right_Down_Sensor + siteIndex * GlobalVar.DIModuleGap))))
        //            {
        //                DIO.WriteDOData(enumDONames.Y08_3_Site1_PinClamp_Down + siteIndex * GlobalVar.DOModuleGap, true);
        //                Thread.Sleep(GlobalVar.ThreadSleep * 2);
        //            }
        //            //*/
        //            #endregion
        //            DIO.WriteDOData(enumDONames.Y08_3_Site1_PinClamp_Down + siteIndex * GlobalVar.DOModuleGap, false);
        //        }
        //        #endregion
        //        #region 사이트 클램핑상태면 언클램프
        //        else if (CheckLiftPos(enumLiftName.FrontLift, FuncInline.enumLiftPos.Site1_F_DT1_Down + siteIndex, 0.1) &&
        //            (FuncInline.UseSiteClampSensor && !DIO.GetDIData(enumDINames.X12_1_Site1_Clamp_Backward_Sensor + siteIndex * GlobalVar.DIModuleGap)) ||
        //                    (!FuncInline.UseSiteClampSensor && DIO.GetDORead(enumDONames.Y08_2_Site1_Clamp_Forward + siteIndex * GlobalVar.DOModuleGap)))
        //        {
        //            DIO.WriteDOData(enumDONames.Y08_2_Site1_Clamp_Forward + siteIndex * GlobalVar.DOModuleGap, false);
        //        }
        //        #endregion
        //        #region 컨베어 두개 동작 아니면 동작
        //        else if (CheckLiftPos(enumLiftName.FrontLift, FuncInline.enumLiftPos.Site1_F_DT1_Down + siteIndex, 0.1) &&
        //            (!DIO.GetDORead(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) ||
        //                    DIO.GetDORead(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) ||
        //                    !DIO.GetDORead(enumDONames.Y405_4_Front_Lift_Down_Motor_Cw) ||
        //                    DIO.GetDORead(enumDONames.Y405_6_Front_Lift_Down_Motor_Ccw)))
        //        {
        //            DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, true);
        //            DIO.WriteDOData(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y405_4_Front_Lift_Down_Motor_Cw, true);
        //            DIO.WriteDOData(enumDONames.Y405_6_Front_Lift_Down_Motor_Ccw, false);
        //        }
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        debug(ex.ToString());
        //        debug(ex.StackTrace);
        //    }

        //    return false;
        //}

       

        //// Lift2상단 에서 site14~26 으로 PCB 이송
        //public static bool MoveLift2UpToSite(enumTeachingPos destSite)
        //{
        //    try
        //    {
        //        FuncInline.ResetInOutTime();

        //        if (destSite < enumTeachingPos.Site14_R_DT1 ||
        //            destSite > enumTeachingPos.Site26_R_FT3)
        //        {
        //            //debug("MoveLift2UpToSite not site dest " + destSite.ToString());
        //            Util.ResetWatch(ref FuncInline.Lift2ActionWatch);
        //            FuncInline.Lift2Action = FuncInline.enumLiftAction.Waiting;
        //            return false;
        //        }

        //        int siteIndex = (int)destSite - (int)enumTeachingPos.Site1_F_DT1;

        //        #region PCB 정보 없으면 액션 삭제
        //        /*
        //        if (FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Up].PCBStatus == FuncInline.enumSMDStatus.UnKnown)
        //        {
        //            //debug("MoveLift2UpToSite no PCB action delete");
        //            Util.ResetWatch(ref FuncInline.PCBInfo[(int)destSite].StopWatch);
        //            Util.ResetWatch(ref FuncInline.Lift2ActionWatch);
        //            FuncInline.Lift2Action = FuncInline.enumLiftAction.Waiting;
        //            DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw, false);
        //            DIO.WriteDOData(enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw, false);
        //            return false;
        //        }
        //        //*/
        //        #endregion
        //        #region PCB 정보 있으면 액션 삭제
        //        /*
        //        if (FuncInline.PCBInfo[(int)destSite].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
        //        {
        //            //debug("MoveLift2UpToSite PCB action delete");
        //            Util.ResetWatch(ref FuncInline.PCBInfo[(int)destSite].StopWatch);
        //            Util.ResetWatch(ref FuncInline.Lift2ActionWatch);
        //            FuncInline.Lift2Action = FuncInline.enumLiftAction.Waiting;
        //            DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw, false);
        //            DIO.WriteDOData(enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw, false);
        //            return false;
        //        }
        //        //*/
        //        #endregion

        //        // 타임아웃 처리
        //        if (//(FuncInline.PCBInfo[(int)destSite].StopWatch.IsRunning &&
        //            //        FuncInline.PCBInfo[(int)destSite].StopWatch.ElapsedMilliseconds > FuncInline.ConveyorTimeout * 1000) ||
        //            (FuncInline.Lift2ActionWatch.IsRunning &&
        //                    FuncInline.Lift2ActionWatch.ElapsedMilliseconds > FuncInline.ConveyorTimeout * 1000))
        //        {
        //            //debug("time : " + FuncInline.Lift2ActionWatch.ElapsedMilliseconds);
        //            Util.InitWatch(ref FuncInline.PCBInfo[(int)destSite].StopWatch);
        //            Util.InitWatch(ref FuncInline.Lift2ActionWatch);
        //            DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw, false);
        //            DIO.WriteDOData(enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw, false);

        //            FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                        DateTime.Now.ToString("HH:mm:ss"),
        //                                        FuncInline.enumErrorPart.Lift2_Up,
        //                                        enumErrorCode.Conveyor_Timeout,
        //                                        false,
        //                                        "Lift2 Up Side output action timed out"));
        //            FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                        DateTime.Now.ToString("HH:mm:ss"),
        //                                        FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex,
        //                                        enumErrorCode.Conveyor_Timeout,
        //                                        false,
        //                                        (FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex).ToString() + " input action timed out"));
        //            return false;
        //        }

        //        // 동작 완료
        //        if (!FuncInline.PassMode &&
        //            DIO.GetDORead(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) &&
        //            DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + siteIndex * GlobalVar.DIModuleGap) &&
        //            (!DIO.GetDIData(enumDINames.X404_6_Rear_Lift_Up_PCB_In_Sensor) &&
        //                    !DIO.GetDIData(enumDINames.X405_1_Rear_Lift_Up_PCB_Stop_Sensor) &&
        //                    !DIO.GetDIData(enumDINames.X06_2_Rack2_UpLift_End_Sensor))) // &&
        //                                                                                //!DIO.GetDORead(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) &&
        //                                                                                //!DIO.GetDORead(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) &&
        //                                                                                //!DIO.GetDORead(enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw) &&
        //                                                                                //!DIO.GetDORead(enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw))
        //        {
        //            //debug("MoveLift2UpToSite reset");
        //            Util.ResetWatch(ref FuncInline.PCBInfo[(int)destSite].StopWatch);
        //            Util.ResetWatch(ref FuncInline.Lift2ActionWatch);
        //            return true;
        //        }
        //        if (FuncInline.PassMode &&
        //            !DIO.GetDORead(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) &&
        //            DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + siteIndex * GlobalVar.DIModuleGap) &&
        //            (!DIO.GetDIData(enumDINames.X404_6_Rear_Lift_Up_PCB_In_Sensor) &&
        //                    !DIO.GetDIData(enumDINames.X405_1_Rear_Lift_Up_PCB_Stop_Sensor) &&
        //                    !DIO.GetDIData(enumDINames.X06_2_Rack2_UpLift_End_Sensor))) // &&
        //                                                                                //!DIO.GetDORead(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) &&
        //                                                                                //!DIO.GetDORead(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) &&
        //                                                                                //!DIO.GetDORead(enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw) &&
        //                                                                                //!DIO.GetDORead(enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw))
        //        {
        //            //debug("MoveLift2UpToSite finish");
        //            Util.ResetWatch(ref FuncInline.PCBInfo[(int)destSite].StopWatch);
        //            Util.ResetWatch(ref FuncInline.Lift2ActionWatch);
        //            return true;
        //        }

        //        // 리프트 배출 위치 아니면 이동
        //        if (!CheckLiftPos(enumLiftName.RearLift, FuncInline.enumLiftPos.Site1_F_DT1_Up + siteIndex, 0.1))
        //        {
        //            //debug("MoveLift2UpToSite lift");
        //            MoveLift(enumLiftName.RearLift, FuncInline.enumLiftPos.Site1_F_DT1_Up + siteIndex);
        //        }
        //        // 사이트  핀클램핑상태면 언클램프
        //        else if (CheckLiftPos(enumLiftName.RearLift, FuncInline.enumLiftPos.Site1_F_DT1_Up + siteIndex, 0.1) &&
        //            (DIO.GetDIData(enumDINames.X12_2_Site1_PinClamp_Left_Down_Sensor + siteIndex * GlobalVar.DIModuleGap) ||
        //                    DIO.GetDIData(enumDINames.X12_3_Site1_PinClamp_Right_Down_Sensor + siteIndex * GlobalVar.DIModuleGap)))
        //        {
        //            //debug("MoveLift2UpToSite unclamp 1");
        //            #region 찐빠 확인시 내렸다가 다시 올린다
        //            /*
        //            if (!DIO.GetDORead(enumDONames.Y08_3_Site1_PinClamp_Down + siteIndex * GlobalVar.DOModuleGap) &&
        //                ((DIO.GetDIData(enumDINames.X12_2_Site1_PinClamp_Left_Down_Sensor + siteIndex * GlobalVar.DIModuleGap) &&
        //                        !DIO.GetDIData(enumDINames.X12_3_Site1_PinClamp_Right_Down_Sensor + siteIndex * GlobalVar.DIModuleGap)) ||
        //                    (!DIO.GetDIData(enumDINames.X12_2_Site1_PinClamp_Left_Down_Sensor + siteIndex * GlobalVar.DIModuleGap) &&
        //                        DIO.GetDIData(enumDINames.X12_3_Site1_PinClamp_Right_Down_Sensor + siteIndex * GlobalVar.DIModuleGap))))
        //            {
        //                DIO.WriteDOData(enumDONames.Y08_3_Site1_PinClamp_Down + siteIndex * GlobalVar.DOModuleGap, true);
        //                Thread.Sleep(GlobalVar.ThreadSleep * 2);
        //            }
        //            //*/
        //            #endregion
        //            DIO.WriteDOData(enumDONames.Y08_3_Site1_PinClamp_Down + siteIndex * GlobalVar.DOModuleGap, false);
        //        }
        //        // 사이트 클램핑상태면 언클램프
        //        else if (CheckLiftPos(enumLiftName.RearLift, FuncInline.enumLiftPos.Site1_F_DT1_Up + siteIndex, 0.1) &&
        //            (FuncInline.UseSiteClampSensor && !DIO.GetDIData(enumDINames.X12_1_Site1_Clamp_Backward_Sensor + siteIndex * GlobalVar.DIModuleGap)) ||
        //                    (!FuncInline.UseSiteClampSensor && DIO.GetDORead(enumDONames.Y08_2_Site1_Clamp_Forward + siteIndex * GlobalVar.DOModuleGap)))
        //        {
        //            //debug("MoveLift2UpToSite unclamp 2");
        //            DIO.WriteDOData(enumDONames.Y08_2_Site1_Clamp_Forward + siteIndex * GlobalVar.DOModuleGap, false);
        //        }
        //        // PassMode PCB 감지되고 컨베어 돌고 있으면 정지
        //        else if (FuncInline.PassMode &&
        //            DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + siteIndex * GlobalVar.DIModuleGap) &&
        //            (DIO.GetDORead(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) ||
        //                    DIO.GetDORead(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) ||
        //                    DIO.GetDORead(enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw) ||
        //                    DIO.GetDORead(enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw)))
        //        {
        //            //debug("MoveLift2UpToSite passmode stop");
        //            DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw, false);
        //            DIO.WriteDOData(enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw, false);
        //        }
        //        // 컨베어 두개 동작 아니면 동작
        //        else if (CheckLiftPos(enumLiftName.RearLift, FuncInline.enumLiftPos.Site1_F_DT1_Up + siteIndex, 0.1) &&
        //            (DIO.GetDORead(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) ||
        //                    !DIO.GetDORead(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) ||
        //                    DIO.GetDORead(enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw) ||
        //                    !DIO.GetDORead(enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw)))
        //        {
        //            //debug("MoveLift2UpToSite conv run");
        //            DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, true);
        //            DIO.WriteDOData(enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw, false);
        //            DIO.WriteDOData(enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw, true);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        debug(ex.ToString());
        //        debug(ex.StackTrace);
        //    }

        //    return false;
        //}

        //// FrontLift상단 에서 site1~13 으로 PCB 이송
        //public static bool MoveLift1UpToSite(enumTeachingPos destSite)
        //{
        //    try
        //    {
        //        FuncInline.ResetInOutTime();

        //        if (destSite < enumTeachingPos.Site1_F_DT1 ||
        //            destSite > enumTeachingPos.Site13_F_FT3)
        //        {
        //            //debug("MoveLift1UpToSite 목적지 사이트 아님 " + destSite.ToString());
        //            Util.ResetWatch(ref FuncInline.Lift1ActionWatch);
        //            FuncInline.Lift1Action = FuncInline.enumLiftAction.Waiting;
        //            return false;
        //        }

        //        int siteIndex = (int)destSite - (int)enumTeachingPos.Site1_F_DT1;

        //        #region PCB 정보 없으면 액션 삭제
        //        /*
        //        if (FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].PCBStatus == FuncInline.enumSMDStatus.UnKnown)
        //        {
        //            //debug("MoveLift1UpToSite PCB 정보 없으면 액션 삭제");
        //            Util.ResetWatch(ref FuncInline.PCBInfo[(int)destSite].StopWatch);
        //            Util.ResetWatch(ref FuncInline.Lift1ActionWatch);
        //            FuncInline.Lift1Action = FuncInline.enumLiftAction.Waiting;
        //            DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y405_0_Front_Lift_Up_Motor_Cw, false);
        //            DIO.WriteDOData(enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw, false);
        //            return false;
        //        }
        //        //*/
        //        #endregion
        //        #region PCB 정보 있으면 액션 삭제
        //        /*
        //        if (FuncInline.PCBInfo[(int)destSite].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
        //        {
        //            //debug("MoveLift1UpToSite PCB 정보 있으면 액션 삭제");
        //            Util.ResetWatch(ref FuncInline.PCBInfo[(int)destSite].StopWatch);
        //            Util.ResetWatch(ref FuncInline.Lift2ActionWatch);
        //            FuncInline.Lift2Action = FuncInline.enumLiftAction.Waiting;
        //            DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y405_0_Front_Lift_Up_Motor_Cw, false);
        //            DIO.WriteDOData(enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw, false);
        //            return false;
        //        }
        //        //*/
        //        #endregion


        //        // 타임아웃 처리
        //        if (//(FuncInline.PCBInfo[(int)destSite].StopWatch.IsRunning &&
        //            //        FuncInline.PCBInfo[(int)destSite].StopWatch.ElapsedMilliseconds > FuncInline.ConveyorTimeout * 1000) ||
        //            (FuncInline.Lift1ActionWatch.IsRunning &&
        //                    FuncInline.Lift1ActionWatch.ElapsedMilliseconds > FuncInline.ConveyorTimeout * 1000))
        //        {
        //            //debug("time : " + FuncInline.Lift1ActionWatch.ElapsedMilliseconds);
        //            Util.InitWatch(ref FuncInline.PCBInfo[(int)destSite].StopWatch);
        //            Util.InitWatch(ref FuncInline.Lift1ActionWatch);
        //            DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y405_0_Front_Lift_Up_Motor_Cw, false);
        //            DIO.WriteDOData(enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw, false);

        //            FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                        DateTime.Now.ToString("HH:mm:ss"),
        //                                        FuncInline.enumErrorPart.Lift1_Up,
        //                                        enumErrorCode.Conveyor_Timeout,
        //                                        false,
        //                                        "FrontLift Up Side output action timed out"));
        //            FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
        //                                        DateTime.Now.ToString("HH:mm:ss"),
        //                                        FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex,
        //                                        enumErrorCode.Conveyor_Timeout,
        //                                        false,
        //                                        (FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex).ToString() + " input action timed out"));
        //            return false;
        //        }

        //        // 동작 완료
        //        if (DIO.GetDORead(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) &&
        //            DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + siteIndex * GlobalVar.DIModuleGap) &&
        //            (!DIO.GetDIData(enumDINames.X403_2_Front_Lift_Up_PCB_In_Sensor) &&
        //                    !DIO.GetDIData(enumDINames.X04_1_Rack1_UpLift_Stop_Sensor) &&
        //                    !DIO.GetDIData(enumDINames.X403_5_Front_Lift_Up_PCB_Stop_Sensor))) // &&
        //                                                                                //!DIO.GetDORead(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) &&
        //                                                                                //!DIO.GetDORead(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) && // 사이트 로딩 연결하기 위해 구동하도록 남겨 둔다.
        //                                                                                //!DIO.GetDORead(enumDONames.Y405_0_Front_Lift_Up_Motor_Cw) &&
        //                                                                                //!DIO.GetDORead(enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw))
        //        {

        //            Util.ResetWatch(ref FuncInline.PCBInfo[(int)destSite].StopWatch);
        //            Util.ResetWatch(ref FuncInline.Lift1ActionWatch);
        //            return true;
        //        }

        //        // 리프트 배출 위치 아니면 이동
        //        if (!CheckLiftPos(enumLiftName.FrontLift, FuncInline.enumLiftPos.Site1_F_DT1_Up + siteIndex, 0.1))
        //        {
        //            MoveLift(enumLiftName.FrontLift, FuncInline.enumLiftPos.Site1_F_DT1_Up + siteIndex);
        //        }
        //        // 사이트  핀클램핑상태면 언클램프
        //        else if (CheckLiftPos(enumLiftName.FrontLift, FuncInline.enumLiftPos.Site1_F_DT1_Up + siteIndex, 0.1) &&
        //            (DIO.GetDIData(enumDINames.X12_2_Site1_PinClamp_Left_Down_Sensor + siteIndex * GlobalVar.DIModuleGap) ||
        //                    DIO.GetDIData(enumDINames.X12_3_Site1_PinClamp_Right_Down_Sensor + siteIndex * GlobalVar.DIModuleGap)))
        //        {
        //            #region 찐빠 확인시 내렸다가 다시 올린다
        //            /*
        //            if (!DIO.GetDORead(enumDONames.Y08_3_Site1_PinClamp_Down + siteIndex * GlobalVar.DOModuleGap) &&
        //                ((DIO.GetDIData(enumDINames.X12_2_Site1_PinClamp_Left_Down_Sensor + siteIndex * GlobalVar.DIModuleGap) &&
        //                        !DIO.GetDIData(enumDINames.X12_3_Site1_PinClamp_Right_Down_Sensor + siteIndex * GlobalVar.DIModuleGap)) ||
        //                    (!DIO.GetDIData(enumDINames.X12_2_Site1_PinClamp_Left_Down_Sensor + siteIndex * GlobalVar.DIModuleGap) &&
        //                        DIO.GetDIData(enumDINames.X12_3_Site1_PinClamp_Right_Down_Sensor + siteIndex * GlobalVar.DIModuleGap))))
        //            {
        //                DIO.WriteDOData(enumDONames.Y08_3_Site1_PinClamp_Down + siteIndex * GlobalVar.DOModuleGap, true);
        //                Thread.Sleep(GlobalVar.ThreadSleep * 2);
        //            }
        //            //*/
        //            #endregion
        //            DIO.WriteDOData(enumDONames.Y08_3_Site1_PinClamp_Down + siteIndex * GlobalVar.DOModuleGap, false);
        //        }
        //        // 사이트 클램핑상태면 언클램프
        //        else if (CheckLiftPos(enumLiftName.FrontLift, FuncInline.enumLiftPos.Site1_F_DT1_Up + siteIndex, 0.1) &&
        //            (FuncInline.UseSiteClampSensor && !DIO.GetDIData(enumDINames.X12_1_Site1_Clamp_Backward_Sensor + siteIndex * GlobalVar.DIModuleGap)) ||
        //                    (!FuncInline.UseSiteClampSensor && DIO.GetDORead(enumDONames.Y08_2_Site1_Clamp_Forward + siteIndex * GlobalVar.DOModuleGap)))
        //        {
        //            DIO.WriteDOData(enumDONames.Y08_2_Site1_Clamp_Forward + siteIndex * GlobalVar.DOModuleGap, false);
        //        }
        //        // 컨베어 두개 동작 아니면 동작
        //        else if (CheckLiftPos(enumLiftName.FrontLift, FuncInline.enumLiftPos.Site1_F_DT1_Up + siteIndex, 0.1) &&
        //            (DIO.GetDORead(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) ||
        //                    !DIO.GetDORead(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap) ||
        //                    DIO.GetDORead(enumDONames.Y405_0_Front_Lift_Up_Motor_Cw) ||
        //                    !DIO.GetDORead(enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw)))
        //        {
        //            DIO.WriteDOData(enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        //            DIO.WriteDOData(enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, true);
        //            DIO.WriteDOData(enumDONames.Y405_0_Front_Lift_Up_Motor_Cw, false);
        //            DIO.WriteDOData(enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw, true);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        debug(ex.ToString());
        //        debug(ex.StackTrace);
        //    }

        //    return false;
        //}

        
        #endregion

        // 리프트가 지정 위치에 있는가?
        public static bool CheckLiftPos(enumLiftName lift, FuncInline.enumLiftPos pos, double gap)
        {
            try
            {
                int axis = lift == enumLiftName.RearLift ? (int)enumServoAxis.SV04_Lift2 : (int)enumServoAxis.SV02_Lift1;
                double axis_pos = FuncMotion.GetRealPosition(axis);
                double lift_pos = FuncInline.LiftPos[(int)lift, (int)pos];
                return Math.Abs(axis_pos - lift_pos) <= gap;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public static bool MoveLift(enumLiftName lift, FuncInline.enumLiftPos pos) // 투입리프트 동작. 설정속도로 동작
        {
            return MoveLift(lift, pos, FuncInline.LiftSpeed);
        }

        public static bool MoveLift(enumLiftName lift, FuncInline.enumLiftPos pos, double speed) // 투입리프트 동작. 지정속도로
        {
            try
            {
                int axis = lift == enumLiftName.RearLift ? (int)enumServoAxis.SV04_Lift2 : (int)enumServoAxis.SV02_Lift1;

                if (GlobalVar.AxisStatus[axis].StandStill &&
                     CheckLiftPos(lift, pos, 0.1))
                {
                    return true;
                }
                #region 렉센서 인터락
                if (lift == enumLiftName.FrontLift &&
                    DIO.GetDIData(enumDINames.X114_7_Front_Rack_PCB_Interlock_Sensor))
                {
                    FuncMotion.MoveStop((int)enumServoAxis.SV02_Lift1);
                    return false;
                }
                if (lift == enumLiftName.RearLift &&
                    DIO.GetDIData(enumDINames.X405_2_Rear_Rack_PCB_Interlock_Sensor))
                {
                    FuncMotion.MoveStop((int)enumServoAxis.SV04_Lift2);
                    return false;
                }
                #endregion
                #region 컨베어 동작중에는 이동금지
                if (lift == enumLiftName.FrontLift &&
                    (DIO.GetDORead(enumDONames.Y405_0_Front_Lift_Up_Motor_Cw) ||
                            DIO.GetDORead(enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw) ||
                            DIO.GetDORead(enumDONames.Y405_4_Front_Lift_Down_Motor_Cw) ||
                            DIO.GetDORead(enumDONames.Y405_6_Front_Lift_Down_Motor_Ccw)))
                {
                    FuncMotion.MoveStop((int)enumServoAxis.SV02_Lift1);
                    return false;
                }
                if (lift == enumLiftName.RearLift &&
                    (DIO.GetDORead(enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw) ||
                            DIO.GetDORead(enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw) ||
                            DIO.GetDORead(enumDONames.Y305_1_Rear_Lift_Down_Motor_Cw) ||
                            DIO.GetDORead(enumDONames.Y304_1_Rear_Lift_Down_Motor_Ccw)))
                {
                    FuncMotion.MoveStop((int)enumServoAxis.SV04_Lift2);
                    return false;
                }
                #endregion
                #region Lift2 배출 또는 NG 컨베어 걸침 가능성 있으면 이동 금지
                //if (lift == enumLiftName.RearLift &&
                //    (CheckOutputPCBStopAtStart() ||
                //            CheckNGPCBStopAtStart()))
                //{
                //    FuncMotion.MoveStop((int)enumServoAxis.SV04_Lift2);
                //    return false;
                //}
                #endregion
                #region 사이트 배출동작 걸린 거 있으면 이동 금지
                if (lift == enumLiftName.FrontLift)
                {
                    enumTeachingPos[] sites = GetFrontSites();
                    foreach (enumTeachingPos site in sites)
                    {
                        enumDONames cw, ccw;
                        if (SiteIoMaps.TryGetSiteTransportMotorPair(site, out cw, out ccw)
                            && DIO.GetDORead(ccw)) // 배출은 CCW 기준
                        {
                            FuncMotion.MoveStop((int)enumServoAxis.SV02_Lift1);
                            return false;
                        }
                    }
                }
                else if (lift == enumLiftName.RearLift)
                {
                    enumTeachingPos[] sites = GetRearSites();
                    foreach (enumTeachingPos site in sites)
                    {
                        enumDONames cw, ccw;
                        if (SiteIoMaps.TryGetSiteTransportMotorPair(site, out cw, out ccw)
                            && DIO.GetDORead(ccw)) // 배출은 CW 기준
                        {
                            FuncMotion.MoveStop((int)enumServoAxis.SV04_Lift2);
                            return false;
                        }
                    }
                }
                #endregion
                if (GlobalVar.AxisStatus[axis].StandStill)
                {
                    //debug("리프트 정지시 이동 " + lift.ToString() + "," + pos.ToString());
                    FuncMotion.MoveAbsolute(Convert.ToUInt32(axis),
                                   FuncInline.LiftPos[(int)lift, (int)pos],
                                    speed);
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }
        // 전면(Front) 사이트들
        private static enumTeachingPos[] GetFrontSites()
        {
            return new enumTeachingPos[]
            {
                enumTeachingPos.Site1_F_DT1,
                enumTeachingPos.Site2_F_DT2,
                enumTeachingPos.Site3_F_DT3,
                enumTeachingPos.Site4_F_DT4,
                enumTeachingPos.Site5_F_DT5,
                enumTeachingPos.Site6_F_DT6,
                enumTeachingPos.Site7_F_DT7,
                enumTeachingPos.Site8_F_DT8,
                enumTeachingPos.Site9_F_DT9,
                enumTeachingPos.Site10_F_DT10_FT4,
                enumTeachingPos.Site11_F_FT1,
                enumTeachingPos.Site12_F_FT2,
                enumTeachingPos.Site13_F_FT3
            };
        }

        // 후면(Rear) 사이트들
        private static enumTeachingPos[] GetRearSites()
        {
            return new enumTeachingPos[]
            {
                enumTeachingPos.Site14_R_DT1,
                enumTeachingPos.Site15_R_DT2,
                enumTeachingPos.Site16_R_DT3,
                enumTeachingPos.Site17_R_DT4,
                enumTeachingPos.Site18_R_DT5,
                enumTeachingPos.Site19_R_DT6,
                enumTeachingPos.Site20_R_DT7,
                enumTeachingPos.Site21_R_DT8,
                enumTeachingPos.Site22_R_DT9,
                enumTeachingPos.Site23_R_DT10_FT4,
                enumTeachingPos.Site24_R_FT1,
                enumTeachingPos.Site25_R_FT2,
                enumTeachingPos.Site26_R_FT3
            };
        }
        // 바이어체인지 Orange 모드시 렉별로 테스트 진행할 사이트를 구한다.
        // 지정 사이트로 변경시 지정사이트 체크만 하면 된다.
        public static enumTeachingPos GetBuyerChangeTestSite(int rack)
        {
            // 지정사이트 리턴
            return FuncInline.BuyerChangeSite[rack];
            // 나머지는 사용할 필요 없다.
            /*
            #region 사용중이고 NonBlue인 사이트 있으면 None
            for (enumTeachingPos site = enumTeachingPos.Site1_F_DT1 + rack * FuncInline.MaxSiteCount; site < enumTeachingPos.Site1_F_DT1 + (rack + 1) * FuncInline.MaxSiteCount; site++)
            {
                if (FuncInline.UseSite[(int)site - (int)enumTeachingPos.Site1_F_DT1] &&
                    FuncInline.PCBInfo[(int)site].PCBStatus != FuncInline.enumSMDStatus.UnKnown &&
                    !FuncInline.PCBInfo[(int)site].BuyerChange)
                {
                    return enumTeachingPos.None;
                }
            }
            #endregion
            #region 리사이트 이동중인 PCB 있으면 None
            if (rack == 0 &&
                FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
            {
                return enumTeachingPos.None;
            }
            if (rack == 1 &&
                FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
            {
                return enumTeachingPos.None;
            }
            #endregion
            #region 투입인덱스 다음부터 사이트 검색
            for (int i = SearchInputIndex; i < FuncInline.SiteOrder.Length; i++)
            {
                enumTeachingPos sitePos = FuncInline.SiteOrder[i];
                if (sitePos < enumTeachingPos.Site1_F_DT1 + rack * FuncInline.MaxSiteCount ||
                            sitePos > enumTeachingPos.Site13_F_FT3 + rack * FuncInline.MaxSiteCount)
                {
                    continue;
                }
                int siteIndex = (int)sitePos - (int)enumTeachingPos.Site1_F_DT1;
                if (FuncInline.PCBInfo[(int)sitePos].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
                {
                    return sitePos;
                }
            }
            for (int i = 0; i < SearchInputIndex; i++)
            {
                enumTeachingPos sitePos = FuncInline.SiteOrder[i];
                if (sitePos < enumTeachingPos.Site1_F_DT1 + rack * FuncInline.MaxSiteCount ||
                            sitePos > enumTeachingPos.Site13_F_FT3 + rack * FuncInline.MaxSiteCount)
                {
                    continue;
                }
                int siteIndex = (int)sitePos - (int)enumTeachingPos.Site1_F_DT1;
                if (FuncInline.PCBInfo[(int)sitePos].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
                {
                    return sitePos;
                }
            }
            #endregion
            return enumTeachingPos.None;
            //*/
        }


        public static void EnableSite(int siteIndex, bool enable)
        {
            if (enable != FuncInline.UseSite[siteIndex])
            {
                if (enable)
                {
                    FuncLog.WriteLog("Site #" + (siteIndex + 1).ToString() + " Enabled by User");
                    FuncInline.BlockReason[siteIndex] = "";
                    FuncInline.InsertBlockHistory(siteIndex + 1, true, "Enabled by User");
                }
                else
                {
                    FuncLog.WriteLog("Site #" + (siteIndex + 1).ToString() + " Blocked by User");
                    FuncInline.BlockReason[siteIndex] = "User Block";
                    FuncInline.InsertBlockHistory(siteIndex + 1, false, "Blocked by User");
                }
            }

            FuncInline.UseSite[siteIndex] = enable;


            if (enable)
            {
                /*
                for (int i = 0; i < FuncInline.SiteInputCount.Length; i++)
                {
                    FuncInline.SiteInputCount[i] = 0;
                }
                //*/

                /*
                #region 최소 투입카운트 구한다
                int minInputLeft = 99999;
                int minInputRight = 99999;
                for (int i = 0; i < FuncInline.SiteInputCount.Length; i++)
                {
                    if (FuncInline.UseSite[i] &&
                        i < 20 &&
                        FuncInline.SiteInputCount[i] < minInputLeft)
                    {
                        minInputLeft = FuncInline.SiteInputCount[i];
                    }
                    else if (FuncInline.UseSite[i] &&
                        i >= 20 &&
                        FuncInline.SiteInputCount[i] < minInputRight)
                    {
                        minInputRight = FuncInline.SiteInputCount[i];
                    }
                }
                #endregion

                // 사용중인 사이트 중 최소 투입카운트로 지정
                FuncInline.SiteInputCount[siteIndex] = siteIndex < 20 ? minInputLeft : minInputRight;

                int blockArray = CheckDefectRateBlock(siteIndex + 1); // 불량율로 블럭된 어레이
                if (blockArray > 0)
                {
                    if (FuncWin.MessageBoxOK("Site " + (siteIndex + 1) + " is blocked by DefectRate.\nClear Pin in and ng count?\nIf not cleared this Site will be blocked with one NG!"))
                    {
                        ClearPinTestCount(siteIndex + 1, blockArray, false);
                        ClearPinTestCount(siteIndex + 1, blockArray, true);
                    }
                }
                //*/
            }
        }

        // 좌우측 렉에 배출 가능한 사이트 검색 (PassMode용)
        public static enumTeachingPos CheckPassModeSiteOut(int rackIndex)
        {
            try
            {
                for (enumTeachingPos site = enumTeachingPos.Site1_F_DT1 + rackIndex * 20; site <= enumTeachingPos.Site13_F_FT3 + rackIndex * 20; site++)
                {
                    int siteIndex = (int)site - (int)enumTeachingPos.Site1_F_DT1;
                    if (FuncInline.PCBInfo[(int)site].PCBStatus != FuncInline.enumSMDStatus.UnKnown &&
                        DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + GlobalVar.DIModuleGap * siteIndex))
                    {
                        return site;
                    }
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return enumTeachingPos.None;
        }

        public static void SetTeachingMode()
        {
            if (GlobalVar.TeachingMode)
            {
                //DIO.WriteDOData(enumDONames.Y01_3_Safety_Mute, true);
                //Thread.Sleep(1000);
            }

            //FuncInline.DoorControl(GlobalVar.TeachingMode);
   

            if (!GlobalVar.TeachingMode)
            {
                //Thread.Sleep(1000);
                //DIO.WriteDOData(enumDONames.Y01_3_Safety_Mute, false);
            }

        }

        public static void DoorControl(bool open)
        {
            if (open)
            {
                GlobalVar.UseDoor = false;
            }
          
            if (!open)
            {
                Thread.Sleep(200);
                GlobalVar.UseDoor = true;
            }
        }

        public static void SetMute(bool mute)
        {
            if (GlobalVar.Muting == mute)
            {
                return;
            }
            GlobalVar.Muting = mute;
            //if (!mute) // 뮤팅 해제시 도어를 먼전 락한다.
            //{
            //    DIO.WriteDOData(enumDONames.Y00_6_Front_Door_Lock1, false);
            //    DIO.WriteDOData(enumDONames.Y00_7_Front_Door_Lock2, false);
            //    DIO.WriteDOData(enumDONames.Y01_1_Rear_Door_Lock1, false);
            //    DIO.WriteDOData(enumDONames.Y01_2_Rear_Door_Lock2, false);
            //    Thread.Sleep(1000); // 도어 잠긴 후에 뮤트를 끄기 위해
            //}
            //DIO.WriteDOData(enumDONames.Y01_3_Safety_Mute, mute);
            //DIO.WriteDOData(enumDONames.Y01_4_Mute_Lamp, mute);
            //if (mute) // 뮤팅 설정시 뮤트를 먼저 설정하고 도어를 연다
            //{
            //    Thread.Sleep(1000); // 도어 잠긴 후에 뮤트를 끄기 위해
            //    DIO.WriteDOData(enumDONames.Y00_6_Front_Door_Lock1, true);
            //    DIO.WriteDOData(enumDONames.Y00_7_Front_Door_Lock2, true);
            //    DIO.WriteDOData(enumDONames.Y01_1_Rear_Door_Lock1, true);
            //    DIO.WriteDOData(enumDONames.Y01_2_Rear_Door_Lock2, true);
            //}
        }

        // 렉별로 목적지가 배출또는 NG 방향인가? 리프트 동작 중복지정 막기위해
        public static bool CheckSiteOutput(int rack, bool pass, bool ng)
        {
            try
            {
                for (int i = 0; i < 40; i++)
                {
                    // 좌측 렉 배제
                    if (rack == 1 &
                        i < 20)
                    {
                        continue;
                    }
                    // 우측 렉 배제
                    if (rack == 0 &
                        i >= 20)
                    {
                        continue;
                    }
                    if ((pass &&
                                FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].Destination == enumTeachingPos.OutConveyor) ||
                        (ng &&
                                FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].Destination == enumTeachingPos.NgBuffer))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        // 배출 가능 확인
        public static bool CheckOutputEnable(bool isNG)
        {

            try
            {
                // 양품 경우 배출컨베어+리프트2하단 있으면 금지
                if (!isNG &&
                    !CheckPosEmpty(enumTeachingPos.OutConveyor) &&
                    !CheckPosEmpty(enumTeachingPos.Lift2_Down))
                {
                    return false;
                }
                // 이외 위치에서 배출컨베어 목적지 있으면 금지. destination으로는 체크가 되지 않는다.
                //if (!isNG &&
                //    (FuncInline.PCBInfo[(int)enumTeachingPos.RearPassline].Destination == enumTeachingPos.OutConveyor ||
                //    FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination == enumTeachingPos.OutConveyor ||
                //    FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Down].Destination == enumTeachingPos.OutConveyor ||
                //    FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Up].Destination == enumTeachingPos.OutConveyor))
                //{
                //    return false;
                //}
                if (!isNG &&
                    (CheckPassedPCB(enumTeachingPos.RearPassLine) ||
                            CheckPassedPCB(enumTeachingPos.Lift1_Up) |
                            CheckPassedPCB(enumTeachingPos.Lift1_Down) ||
                            CheckPassedPCB(enumTeachingPos.RearPassLine)))
                {
                    return false;
                }
                for (int i = 0; i < 40; i++)
                {
                    if (FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].Destination == enumTeachingPos.OutConveyor)
                    {
                        return false;
                    }
                }

                // 불량 경우 NG컨베어 있으면 금지
                if (isNG &&
                    !CheckPosEmpty(enumTeachingPos.NgBuffer))
                {
                    return false;
                }
                // 이외 위치에서 NG목적지 있으면 금지
                // destinatino으로는 체크가 되지 않는다.
                //if (isNG &&
                //    (FuncInline.PCBInfo[(int)enumTeachingPos.RearPassline].Destination == enumTeachingPos.NgBuffer ||
                //    FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination == enumTeachingPos.NgBuffer ||
                //    FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Down].Destination == enumTeachingPos.NgBuffer ||
                //    FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Up].Destination == enumTeachingPos.NgBuffer ||
                //    FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Down].Destination == enumTeachingPos.NgBuffer))
                //{
                //    return false;
                //}
                if (isNG &&
                    (CheckNGPCB(enumTeachingPos.RearPassLine) ||
                      CheckNGPCB(enumTeachingPos.Lift1_Up) ||
                      CheckNGPCB(enumTeachingPos.Lift1_Down) ||
                      CheckNGPCB(enumTeachingPos.RearPassLine)))
                {
                    return false;
                }

                for (int i = 0; i < 40; i++)
                {
                    if (FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].Destination == enumTeachingPos.NgBuffer)
                    {
                        return false;
                    }
                }
                // 이외는 정상
                return true;


                // 아래는 여러 개 동시에 작동하려고 하니 복잡하고 오류도 많다.
                // Pass,NG 카운트 각각 산정해서 종합적으로 판단
                int passCount = 0; // 배출 컨베어 포함 배출 예정 포함
                int ngCount = 0; // NG 컨베어 제외 NG 배출 예정

                #region NG 경우 먼저 판단. NG 배출 대상이 하나라도 있거나, 정상 배출 두개 이상인 경우 불가
                //if (isNG)
                //{
                #region NG 배출 카운트
                /*
                if (!CheckPosEmpty(enumTeachingPos.NgBuffer))
                {
                    ngCount++;
                }
                //*/
                //if (FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Down].Destination == enumTeachingPos.NgBuffer)
                if (CheckNGPCB(enumTeachingPos.Lift2_Up))
                {
                    ngCount++;
                }
                if (CheckNGPCB(enumTeachingPos.Lift2_Down))
                {
                    ngCount++;
                }
                //if (FuncInline.PCBInfo[(int)enumTeachingPos.RearPassline].Destination == enumTeachingPos.NgBuffer)
                if (CheckNGPCB(enumTeachingPos.RearPassLine))
                {
                    ngCount++;
                }
                //if (FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Down].Destination == enumTeachingPos.NgBuffer)
                if (CheckNGPCB(enumTeachingPos.Lift1_Up))
                {
                    ngCount++;
                }
                if (CheckNGPCB(enumTeachingPos.Lift1_Down))
                {
                    ngCount++;
                }
                for (int i = 0; i < 40; i++)
                {
                    if (FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].Destination == enumTeachingPos.NgBuffer)
                    {
                        ngCount++;
                    }
                }
                #endregion
                //}
                #endregion

                #region 배출 컨베어 포함 양품 배출 예정 카운트
                //if (FuncInline.PCBInfo[(int)enumTeachingPos.OutConveyor].PCBStatus != FuncInline.enumSMDStatus.UnKnown ||
                //DIO.GetDIData(enumDINames.X02_3_Out_Conveyor_PASSLIne_PCB_Start_Sensor) ||
                //DIO.GetDIData(enumDINames.X02_4_Out_Conveyor_PASSLine_PCB_Stop_Sensor) ||
                //DIO.GetDIData(enumDINames.X08_2_Out_Conveyor_End_Sensor))
                if (!CheckPosEmpty(enumTeachingPos.OutConveyor))
                {
                    passCount++;
                }
                //if (FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Down].Destination == enumTeachingPos.OutConveyor)
                if (//CheckPassedPCB(enumTeachingPos.Lift2_Down) ||
                    !CheckPosEmpty(enumTeachingPos.Lift2_Down))
                {
                    passCount++;
                }
                //if (FuncInline.PCBInfo[(int)enumTeachingPos.RearPassline].Destination == enumTeachingPos.OutConveyor)
                if (//CheckPassedPCB(enumTeachingPos.RearPassline) ||
                    !CheckPosEmpty(enumTeachingPos.RearPassLine))
                {
                    passCount++;
                }
                //if (FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Down].Destination == enumTeachingPos.OutConveyor)
                if (//CheckPassedPCB(enumTeachingPos.Lift1_Down) ||
                    !CheckPosEmpty(enumTeachingPos.Lift1_Down))
                {
                    passCount++;
                }
                // 리프트1 하단
                if (FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Down].Destination >= enumTeachingPos.Site1_F_DT1 &&
                    FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Down].Destination <= enumTeachingPos.Site13_F_FT3)
                {
                    passCount++;
                }
                // 리프트2 하단
                if (FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Down].Destination >= enumTeachingPos.Site14_R_DT1 &&
                    FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Down].Destination <= enumTeachingPos.Site26_R_FT3)
                {
                    passCount++;
                }
                // 사이트에서는 목적지만 검사한다.
                for (int i = 0; i < 40; i++)
                {
                    if (FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + i].Destination == enumTeachingPos.OutConveyor)
                    {
                        passCount++;
                    }
                }
                #endregion

                //debug("Pass Count : " + passCount);
                //debug("NG Count : " + ngCount);

                #region NG 배출 가능 여부 판단
                if (isNG)
                {
                    return CheckPosEmpty(enumTeachingPos.NgBuffer) && ngCount == 0;// &&
                                                                             //passCount < 2;
                }
                #endregion

                // Pass 경우 배출 컨베어 + 배출 예정 한 개 미만일 때
                return passCount + ngCount <= 1;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }


        //여러 센서를 한 번에 체크
        static bool AllSensorsFalse(params enumDINames[] sensors)
        {
            bool ok = true;
            foreach (var s in sensors)
                ok &= !DIO.GetDIData(s);
            return ok;
        }
        static bool AnySensorTrue(params enumDINames[] sensors)
        {
            foreach (var s in sensors)
                if (DIO.GetDIData(s)) return true;
            return false;
        }

        // PCB 정보도 없고 감지되는 센서도 없는가?
        public static bool CheckPosEmpty(enumTeachingPos pos)
        {
            try
            {
                var unknown = FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.UnKnown;

                switch (pos)
                {
                    case enumTeachingPos.InConveyor:
                        // In/Shuttle 구간 센서 사용
                        return unknown && AllSensorsFalse(
                            enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor,
                            enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor
                        );

                    case enumTeachingPos.FrontPassLine:
                        // 프론트 패스라인 Stop 센서
                        return unknown && AllSensorsFalse(
                            enumDINames.X114_6_Front_PASSLINE_PCB_Stop_Sensor
                        );

                    case enumTeachingPos.Lift1_Up:
                        // Front Lift - Up 라인
                        return unknown && AllSensorsFalse(
                            enumDINames.X403_2_Front_Lift_Up_PCB_In_Sensor,
                            enumDINames.X403_5_Front_Lift_Up_PCB_Stop_Sensor
                        );

                    case enumTeachingPos.Lift1_Down:
                        // Front Lift - Down 라인
                        return unknown && AllSensorsFalse(
                            enumDINames.X400_0_Front_Lift_Down_PCB_In_Sensor,
                            enumDINames.X400_2_Front_Lift_Down_PCB_Stop_Sensor
                        );

                    case enumTeachingPos.RearPassLine:
                        // 리어 패스라인(OK/NG 라인 모두 비어야 'Empty')
                        return unknown && AllSensorsFalse(
                            enumDINames.X405_0_Rear_Pass_OkLine_PCB_In_Sensor,
                            enumDINames.X406_4_Rear_Pass_NgLine_PCB_Stop_Sensor
                        );

                    case enumTeachingPos.Lift2_Up:
                        // Rear Lift - Up 라인
                        return unknown && AllSensorsFalse(
                            enumDINames.X404_6_Rear_Lift_Up_PCB_In_Sensor,
                            enumDINames.X405_1_Rear_Lift_Up_PCB_Stop_Sensor
                        );

                    case enumTeachingPos.Lift2_Down:
                        // Rear Lift - Down 라인
                        return unknown && AllSensorsFalse(
                            enumDINames.X405_5_Rear_Lift_Down_PCB_In_Sensor,
                            enumDINames.X405_7_Rear_Lift_Down_PCB_Stop_Sensor
                        );

                    case enumTeachingPos.OutConveyor:
                        // 아웃 컨베이어 (PASSLine Start/Stop)
                        return unknown && AllSensorsFalse(
                            enumDINames.X02_3_Out_Conveyor_PASSLIne_PCB_Start_Sensor,
                            enumDINames.X02_4_Out_Conveyor_PASSLine_PCB_Stop_Sensor
                        );

                    case enumTeachingPos.NgBuffer:
                        // NG 버퍼: 존재 감지(Detection) 및 상/하단 전후 센서 모두 OFF면 빈 상태로 판단
                        return unknown && AllSensorsFalse(
                            enumDINames.X03_7_NgBuffer_PCB_Stop_Sensor,
                            enumDINames.X03_1_NgBuffer_UpperForwardSensor,
                            enumDINames.X03_5_NgBuffer_LowerForwardSensor
                        );

                    default:
                        // 사이트 구간(Front/Rear DT/FT)
                        if (pos >= enumTeachingPos.Site1_F_DT1 && pos <= enumTeachingPos.Site26_R_FT3)
                        {
                            if (SiteIoMaps.TryGetPcbDockDI(pos, out var diDock))
                                return unknown && !DIO.GetDIData(diDock);
                            // 매핑이 없으면 센서가 없다고 보고 PCB정보만으로 판단
                            return unknown;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        // PCB 정보도 있고 센서도 감지되는가?
        public static bool CheckPosNotEmpty(enumTeachingPos pos)
        {
            try
            {
                var known = FuncInline.PCBInfo[(int)pos].PCBStatus != FuncInline.enumSMDStatus.UnKnown;

                switch (pos)
                {
                    case enumTeachingPos.InConveyor:
                        return known && AnySensorTrue(
                            enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor,
                            enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor
                        );

                    case enumTeachingPos.FrontPassLine:
                        return known && AnySensorTrue(
                            enumDINames.X114_6_Front_PASSLINE_PCB_Stop_Sensor
                        );

                    case enumTeachingPos.Lift1_Up:
                        return known && AnySensorTrue(
                            enumDINames.X403_2_Front_Lift_Up_PCB_In_Sensor,
                            enumDINames.X403_5_Front_Lift_Up_PCB_Stop_Sensor
                        );

                    case enumTeachingPos.Lift1_Down:
                        return known && AnySensorTrue(
                            enumDINames.X400_0_Front_Lift_Down_PCB_In_Sensor,
                            enumDINames.X400_2_Front_Lift_Down_PCB_Stop_Sensor
                        );

                    case enumTeachingPos.RearPassLine:
                        return known && AnySensorTrue(
                            enumDINames.X405_0_Rear_Pass_OkLine_PCB_In_Sensor,
                            enumDINames.X406_4_Rear_Pass_NgLine_PCB_Stop_Sensor
                        );

                    case enumTeachingPos.Lift2_Up:
                        return known && AnySensorTrue(
                            enumDINames.X404_6_Rear_Lift_Up_PCB_In_Sensor,
                            enumDINames.X405_1_Rear_Lift_Up_PCB_Stop_Sensor
                        );

                    case enumTeachingPos.Lift2_Down:
                        return known && AnySensorTrue(
                            enumDINames.X405_5_Rear_Lift_Down_PCB_In_Sensor,
                            enumDINames.X405_7_Rear_Lift_Down_PCB_Stop_Sensor
                        );

                    case enumTeachingPos.OutConveyor:
                        return known && AnySensorTrue(
                            enumDINames.X02_3_Out_Conveyor_PASSLIne_PCB_Start_Sensor,
                            enumDINames.X02_4_Out_Conveyor_PASSLine_PCB_Stop_Sensor
                        );

                    case enumTeachingPos.NgBuffer:
                        return known && AnySensorTrue(
                            enumDINames.X03_7_NgBuffer_PCB_Stop_Sensor,
                            enumDINames.X03_1_NgBuffer_UpperForwardSensor,                          
                            enumDINames.X03_5_NgBuffer_LowerForwardSensor
                        );

                    default:
                        // 사이트 구간(Front/Rear DT/FT)
                        if (pos >= enumTeachingPos.Site1_F_DT1 && pos <= enumTeachingPos.Site26_R_FT3)
                        {
                            if (SiteIoMaps.TryGetPcbDockDI(pos, out var diDock))
                                return known && DIO.GetDIData(diDock);
                            return false;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

      

        // 배열 전체 뒤져서 해당 사이트 없으면 등록된 모든 사이트 정보 하나씩 밀고
        //첫 자리에 사이트 등록
        public static void SetTestSite(int siteIndex)
        {
            bool exist = false;
            /*
            for (int j = 0; j < FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + siteIndex].TestSite.Length; j++)
            {
                if (FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + siteIndex].TestSite[j] > 0 &&
                    FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + siteIndex].TestSite[j] == siteIndex + 1)
                {
                    exist = true;
                    break;
                }
            }
            //*/
            exist = FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + siteIndex].TestSite[0] == siteIndex + 1; // 같은 사이트 또 들어갈 수 있으므로 첫 자리만 확인
            if (!exist)
            {
                // 한칸씩 뒤로 민다.
                for (int j = FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + siteIndex].TestSite.Length - 1; j > 0; j--)
                {
                    FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + siteIndex].TestSite[j] = FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + siteIndex].TestSite[j - 1];
                }
                FuncInline.PCBInfo[(int)enumTeachingPos.Site1_F_DT1 + siteIndex].TestSite[0] = siteIndex + 1;
            }
        }

     

        // 필요시: 클램프(물림) 여부도 함께
        public static bool CheckSiteClamp(enumTeachingPos site)
        {
            if (SiteIoMaps.TryGetContactStopperDO(site, out var stopperDo))
                return DIO.GetDORead(stopperDo); // ON ⇒ 클램프(물림)

            return false;
        }


        // 업 커맨드 극성: true면 DO = true 가 "DOWN 명령" (일반적)

        private const bool DO_PIN_UP_SOL = false;

        // 사이트 핀(컨택트)이 DOWN 상태인가?
        public static bool IsContactDown(enumTeachingPos site)
        {
            // 1) 사이트별 업센서/업다운 DO 찾기
            enumDINames diPinUp;
            enumDONames doPinDownSol;
            bool hasUpPin = SiteIoMaps.TryGetContactUpDI(site, out diPinUp);
            bool hasDownPinSol = SiteIoMaps.TryGetContactUpDownDO(site, out doPinDownSol);

            // 2) 센서/DO 현재 상태 읽기
            bool upSensorOn = false; // true == 실제로 UP 감지
            if (hasUpPin)
            {
                upSensorOn = DIO.GetDIData(diPinUp);
            }
              

            bool doOn = false;       // true/false == 명령 라인 상태
            bool commandKnown = false;
            if (hasDownPinSol)
            {
                doOn = DIO.GetDORead(doPinDownSol); //true면 다운상태
                commandKnown = true;
            }

            // 3) 판정 로직
            //    - 업센서가 있다면 센서를 최우선으로 신뢰
            //    - 업센서 OFF 이면서, 가능하면 DO가 DOWN을 가리킬 때 DOWN 확정
            //    - 업센서만 있고 DO를 못읽으면, 업센서 OFF를 DOWN으로 본다(보수적).
            //    - 업센서가 없고 DO만 있으면 극성 기준으로 DOWN 판정
            if (hasUpPin)
            {
                if (upSensorOn)
                    return false; // 센서가 UP이면 DOWN 아님

                if (commandKnown)
                {
                    bool commandIsUp = (doOn == DO_PIN_UP_SOL); //true 조건이면 업상태
                    bool commandIsDown = !commandIsUp;
                    return commandIsDown; // 센서 OFF + DOWN 명령 => DOWN
                }

                // 센서 OFF인데 DO 정보가 없으면 DOWN 으로 간주(현장 상황에 맞게 조정 가능)
                return true;
            }
            else if (commandKnown)
            {
                // 센서가 없으면 DO 극성으로만 판단
                bool commandIsUp = (doOn == DO_PIN_UP_SOL);
                return !commandIsUp; // UP 명령이 아니면 DOWN으로 간주
            }

            // 둘 다 없으면 안전을 위해 DOWN 아님 처리
            return false;
        }


        // 리사이트 판단위해 이미 거쳐간 사이트에 포함되지 않았는지 확인
        public static bool CheckResiteSite(enumTeachingPos resiteSite, enumTeachingPos emptySite)
        {
            try
            {
                if (FuncInline.PCBInfo[(int)resiteSite].TestSite != null)
                {
                    // 첫 사이트는 현재 사이트로 넣으므로 통과
                    for (int i = 0; i < FuncInline.PCBInfo[(int)resiteSite].TestSite.Length; i++)
                    {
                        if (FuncInline.PCBInfo[(int)resiteSite].TestSite[i] == (int)emptySite - (int)enumTeachingPos.Site1_F_DT1
                             + 1)
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }

            return true;
        }

        public static bool CheckCurrentShift(string date, string time)
        {
            try
            {
                string currentDate = DateTime.Now.ToString("yyyyMMdd"); // 현재 날짜
                string yesterDay = DateTime.Today.AddDays(-1).ToString("yyyyMMdd"); // 어제 날짜
                string currentTime = DateTime.Now.ToString("HH:mm:ss"); // 현재 시간
                int aTime = FuncInline.ShiftAHour * 100 + FuncInline.ShiftAMin;
                int bTime = FuncInline.ShiftBHour * 100 + FuncInline.ShiftBMin;
                int cTime = FuncInline.ShiftCHour * 100 + FuncInline.ShiftCMin;
                //NowShift)
                int inTime = int.Parse(time.Replace(":", "").Substring(0, 4));
                string inShift = "C";
                if (inTime >= aTime &&
                    inTime < bTime)
                {
                    inShift = "A";
                }
                else if (FuncInline.UseShiftC && // 3교대시 B조는 C조 시간 이전까지
                    inTime >= bTime &&
                    inTime < cTime)
                {
                    inShift = "B";
                }
                else if (!FuncInline.UseShiftC && // 2교대시 B조는 B조 시간이후거나 A조 시간 이전
                    (inTime >= bTime ||
                            inTime < aTime))
                {
                    inShift = "B";
                }
                else
                {
                    inShift = "C";
                }

                return inShift == NowShift;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }

            return false;
        }

        public static void ResetCount()
        {
            GlobalVar.TackStart = GlobalVar.TickCount64;

            //lblRunningTime.Text = "-";
            //lblTotalTime.Text = "-";

            ArrayInputCount = 0;
            ArrayPassCount = 0;
            ArrayDefectCount = 0;
            PCBInputCount = 0;
            PCBPassCount = 0;
            PCBDefectCount = 0;
            DefectRate = 0;
            //GlobalVar.AutoInline_TestCycleTime = 0;
        }

        public static void ResetInputCount()
        {
            //*
            for (int siteIndex = 0; siteIndex < 40; siteIndex++)
            {
                FuncInline.SiteInputCount[siteIndex] = 0;
            }
            //*/
        }
        // 공통 유틸: 사이트 인덱스(i: 0-base) → Dock DI 상태 읽기
        public static bool GetDockState(int i)
        {
            var pos = (FuncInline.enumTeachingPos)((int)FuncInline.enumTeachingPos.Site1_F_DT1 + i);
            return FuncInline.SiteIoMaps.TryGetPcbDockDI(pos, out var di) && DIO.GetDIData(di);
        }


        public static bool CheckOutputPCBStopAtStart()
        {
            if (DIO.GetDIData(enumDINames.X02_3_Out_Conveyor_PASSLIne_PCB_Start_Sensor) &&
                !DIO.GetDIData(enumDINames.X02_4_Out_Conveyor_PASSLine_PCB_Stop_Sensor))
            {
                return true;
            }

            return false;
        }

        public static bool CheckNGPCBStopAtStart()
        {
            if (!DIO.GetDIData(enumDINames.X04_2_Out_Shuttle_NG_PCB_Stop_Sensor))
            {
                return true;
            }

            return false;
        }

        public static void SaveVersionInfo()
        {
            string version_path = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\Program\\Versions";
            string version_str = "1.0.0.1";
            #region 1.0.0.1
            if (!File.Exists(version_path + "\\" + version_str + ".txt"))
            {
                using (FileStream fs =
                               new FileStream(version_path + "\\" + version_str + ".txt", FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        string lines = "Initial Production Version";
                        sw.Write(lines);

                        sw.Flush();
                        sw.Close();
                    }
                }
            }
            #endregion

            #region 1.0.0.2
            //version_str = "1.0.0.2";
            //if (!File.Exists(version_path + "\\" + version_str + ".txt"))
            //{
            //    using (FileStream fs =
            //                   new FileStream(version_path + "\\" + version_str + ".txt", FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
            //    {
            //        using (StreamWriter sw = new StreamWriter(fs))
            //        {
            //            string lines = "1.Show Timeout to All History\r\n" +
            //                            "2.Correct test time display in NG Viewer\r\n" +
            //                            "3.Timeout Added to Site Block\r\n" +
            //                            "4.Correct dual insert of RD Command in Communication Trace\r\n" +
            //                            "5.Timeout When no response to RD Command\r\n" +
            //                            "6.Correct different result in Errors\r\n" +
            //                            "7.Main UI change, adjust Lift width to text\r\n" +
            //                            "8.Sort selection added to Trace\r\n" +
            //                            "9.Correct exception, Click Door Open Button while Origin and Width move\r\n" +
            //                            "10.Site Block History added to Site Detail\r\n" +
            //                            "11.GoodMark and BadMark are combined in Vision Teaching\r\n" +
            //                            "12.Correct OCR function in Vision Teaching\r\n" +
            //                            "13.Change Array Index from Vision OCR Teaching\r\n";
            //            sw.Write(lines);

            //            sw.Flush();
            //            sw.Close();
            //        }
            //    }
            //}
            #endregion

          
        }

        public static void InitSimulation()
        {
            if (!GlobalVar.Simulation)
            {
                return;
            }

            #region 서보 초기화
            for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumServoAxis)).Length; i++)
            {
                GlobalVar.AxisStatus[i].StandStill = true;
                GlobalVar.AxisStatus[i].Position = 0;
            }
            #endregion

            #region 실린더 초기화
            //DIO.WriteDIData(FuncAmplePacking.enumDINames.X04_1_Conveyor_Clamp_Left_Reward, true);
            //DIO.WriteDIData(FuncAmplePacking.enumDINames.X04_3_Conveyor_Clamp_Right_Reward, true);
            //DIO.WriteDIData(FuncAmplePacking.enumDINames.X06_1_Ample_Body_Return_Sensor, true);


            #endregion
        }
    }

   
}
