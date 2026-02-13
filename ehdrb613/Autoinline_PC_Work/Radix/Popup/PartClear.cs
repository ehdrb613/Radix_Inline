using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Radix
{
    /*
     * Manual.cs : 각 파트 및 장비의 수동 운전
     */

    public partial class PartClear : Form
    {
        #region 로컬변수
        private System.Threading.Timer timerCheck;
        private bool timerDoing = false;
        private Button activeButton = null;
        #endregion

        #region 초기화 관련
        public PartClear()
        {
            InitializeComponent();
        }

        private void debug(string str)
        {
            //Util.Debug(str);
        }




        private void Manual_Shown(object sender, EventArgs e)
        {
            GlobalVar.dlgOpened = true;


            // 타이머 시작
            TimerCallback CallBackCheck = new TimerCallback(TimerCheck);
            timerCheck = new System.Threading.Timer(CallBackCheck, false, 0, 100);

            this.BringToFront();

        }

        private void Manual_FormClosed(object sender, FormClosedEventArgs e)
        {
            timerCheck.Dispose();
            GlobalVar.dlgOpened = false;
            if (this.Parent != null)
            {
                try
                {
                    this.Parent.BringToFront();
                }
                catch
                { }
            }
        }

        #endregion


        #region 타이머 함수
        private void TimerCheck(Object state)
        {
            try
            {
                if (timerDoing)
                {
                    return;
                }

                timerDoing = true;

                this.Invoke(new MethodInvoker(delegate ()
                {
                    ulong startTime = GlobalVar.TickCount64;

                    #region 버튼 색상 표시
                    //Util.SetButtonColor3_2(btnInputLift,
                    //                    Func.CheckError(enumErrorPart.InputLift),
                    //                    DIO.GetDIData(enumDINames.X01_0_Input_Lift_Start_Sensor) ||
                    //                            DIO.GetDIData(enumDINames.X01_1_Input_Lift_Stop_Sensor) ||
                    //                            DIO.GetDIData(enumDINames.X01_2_Input_Lift_End_Sensor) ||
                    //                            GlobalVar.AutoInline_PCBInfo[(int)enumTeachingPos.InputLift].PCBStatus > enumSMDStatus.UnKnown);
                    //Util.SetButtonColor3_2(btnOutputLift,
                    //                    Func.CheckError(enumErrorPart.OutputLift),
                    //                    DIO.GetDIData(enumDINames.X01_5_Output_Lift_Start_Sensor) ||
                    //                            DIO.GetDIData(enumDINames.X01_6_Output_Lift_Stop_Sensor) ||
                    //                            DIO.GetDIData(enumDINames.X01_7_Output_Lift_End_Sensor) ||
                    //                            GlobalVar.AutoInline_PCBInfo[(int)enumTeachingPos.OutputLift].PCBStatus > enumSMDStatus.UnKnown);
                    //Util.SetButtonColor3_2(btnBufferConveyor,
                    //                    Func.CheckError(enumErrorPart.BufferConveyor),
                    //                    DIO.GetDIData(enumDINames.X02_2_Buffer_Conveyor_Start_Sensor) ||
                    //                            DIO.GetDIData(enumDINames.X02_3_Buffer_Conveyor_Stop_Sensor) ||
                    //                            DIO.GetDIData(enumDINames.X02_4_Buffer_Conveyor_End_Sensor) ||
                    //                            GlobalVar.AutoInline_PCBInfo[(int)enumTeachingPos.BufferConveyor].PCBStatus > enumSMDStatus.UnKnown);
                    //Util.SetButtonColor3_2(btnInputConveyor,
                    //                    Func.CheckError(enumErrorPart.InputConveyor),
                    //                    DIO.GetDIData(enumDINames.X02_5_Input_Conveyor_Start_Sensor) ||
                    //                            DIO.GetDIData(enumDINames.X02_6_Input_Conveyor_Stop_Sensor) ||
                    //                            DIO.GetDIData(enumDINames.X02_7_Input_Conveyor_End_Sensor) ||
                    //                            GlobalVar.AutoInline_PCBInfo[(int)enumTeachingPos.InputConveyor].PCBStatus > enumSMDStatus.UnKnown);
                    //Util.SetButtonColor3_2(btnLoadingConveyor,
                    //                    Func.CheckError(enumErrorPart.LoadingConveyor),
                    //                    DIO.GetDIData(enumDINames.X03_0_Loading_Conveyor_Start_Sensor) ||
                    //                            DIO.GetDIData(enumDINames.X03_1_Loading_Conveyor_Stop_Sensor) ||
                    //                            DIO.GetDIData(enumDINames.X03_2_Loading_Conveyor_End_Sensor) ||
                    //                            GlobalVar.AutoInline_PCBInfo[(int)enumTeachingPos.LoadingConveyor].PCBStatus > enumSMDStatus.UnKnown);
                    //Util.SetButtonColor3_2(btnUnloadingConveyor,
                    //                    Func.CheckError(enumErrorPart.UnloadingConveyor),
                    //                    DIO.GetDIData(enumDINames.X03_5_Unloading_Conveyor_Start_Sensor) ||
                    //                            DIO.GetDIData(enumDINames.X03_6_Unloading_Conveyor_Stop_Sensor) ||
                    //                            DIO.GetDIData(enumDINames.X03_7_Unloading_Conveyor_End_Sensor) ||
                    //                            GlobalVar.AutoInline_PCBInfo[(int)enumTeachingPos.UnloadingConveyor].PCBStatus > enumSMDStatus.UnKnown);
                    //Util.SetButtonColor3_2(btnInputJig,
                    //                    Func.CheckError(enumErrorPart.InputJig),
                    //                    GlobalVar.AutoInline_PCBInfo[(int)enumTeachingPos.InputJig].PCBStatus > enumSMDStatus.UnKnown);
                    //Util.SetButtonColor3_2(btnOutputJig,
                    //                    Func.CheckError(enumErrorPart.OutputJig),
                    //                    GlobalVar.AutoInline_PCBInfo[(int)enumTeachingPos.OutputJig].PCBStatus > enumSMDStatus.UnKnown);
                    //Util.SetButtonColor3_2(btnNg1,
                    //                    Func.CheckError(enumErrorPart.NGConveyor),
                    //                    DIO.GetDIData(enumDINames.X04_0_NG_Buffer_Start_Sensor) ||
                    //                            GlobalVar.AutoInline_PCBInfo[(int)enumTeachingPos.NG1].PCBStatus > enumSMDStatus.UnKnown);
                    //Util.SetButtonColor3_2(btnNg2,
                    //                    Func.CheckError(enumErrorPart.NGConveyor),
                    //                    DIO.GetDIData(enumDINames.X04_1_NG_Buffer_End_Sensor) ||
                    //                            GlobalVar.AutoInline_PCBInfo[(int)enumTeachingPos.NG2].PCBStatus > enumSMDStatus.UnKnown);
                    //for (int i = 1; i <= 21; i++)
                    //{
                    //    Util.SetButtonColor3_2(Controls.Find("btnSite" + i, true)[0],
                    //                        Func.CheckError((enumErrorPart)(enumErrorPart.Site1 + (i - 1))),
                    //                        DIO.GetDIData(enumDINames.X12_3_Module1_PCB_Sensor + (i - 1) * GlobalVar.DIModuleGap) ||
                    //                                GlobalVar.AutoInline_PCBInfo[(int)enumTeachingPos.Site1 + (i - 1)].PCBStatus > enumSMDStatus.UnKnown);
                    //}
                    if (activeButton != null &&
                        activeButton.BackColor == Color.Red)
                    {
                        activeButton.BackColor = Color.DarkRed;
                    }
                    else if (activeButton != null &&
                        activeButton.BackColor == Color.Lime)
                    {
                        activeButton.BackColor = Color.LimeGreen;
                    }
                    else if (activeButton != null &&
                        activeButton.BackColor == Color.WhiteSmoke)
                    {
                        activeButton.BackColor = Color.DarkGray;
                    }
                    #endregion

                    //Console.WriteLine("manual ui time : " + (GlobalVar.TickCount64 - startTime).ToString());
                }));

                timerDoing = false;

            }
            catch
            { }
            timerDoing = false;
        }
        #endregion


        private void Site_Click(object sender, EventArgs e)
        {
            //activeButton = (Button)sender;
            //enumTeachingPos pos = enumTeachingPos.InputJig;
            //string pcbDetect = "Empty";
            //switch (activeButton.Name)
            //{
            //    case "btnInputJig":
            //        pos = enumTeachingPos.InputJig;
            //        pcbDetect = "";
            //        break;
            //    case "btnOutputJig":
            //        pos = enumTeachingPos.OutputJig;
            //        pcbDetect = "";
            //        break;
            //    case "btnInputLift":
            //        pos = enumTeachingPos.BufferConveyor;
            //        pcbDetect = DIO.GetDIData(enumDINames.X01_0_Input_Lift_Start_Sensor) ||
            //                            DIO.GetDIData(enumDINames.X01_1_Input_Lift_Stop_Sensor) ||
            //                            DIO.GetDIData(enumDINames.X01_2_Input_Lift_End_Sensor) ?
            //                    "Detected" : "Not Detected";
            //        break;
            //    case "btnOutputLift":
            //        pos = enumTeachingPos.BufferConveyor;
            //        pcbDetect = DIO.GetDIData(enumDINames.X01_5_Output_Lift_Start_Sensor) ||
            //                            DIO.GetDIData(enumDINames.X01_6_Output_Lift_Stop_Sensor) ||
            //                            DIO.GetDIData(enumDINames.X01_7_Output_Lift_End_Sensor) ?
            //                    "Detected" : "Not Detected";
            //        break;
            //    case "btnBufferConveyor":
            //        pos = enumTeachingPos.InputConveyor;
            //        pcbDetect = DIO.GetDIData(enumDINames.X02_2_Buffer_Conveyor_Start_Sensor) ||
            //                            DIO.GetDIData(enumDINames.X02_3_Buffer_Conveyor_Stop_Sensor) ||
            //                            DIO.GetDIData(enumDINames.X02_4_Buffer_Conveyor_End_Sensor) ?
            //                    "Detected" : "Not Detected";
            //        break;
            //    case "btnInputConveyor":
            //        pos = enumTeachingPos.InputConveyor;
            //        pcbDetect = DIO.GetDIData(enumDINames.X02_5_Input_Conveyor_Start_Sensor) ||
            //                            DIO.GetDIData(enumDINames.X02_6_Input_Conveyor_Stop_Sensor) ||
            //                            DIO.GetDIData(enumDINames.X02_7_Input_Conveyor_End_Sensor) ?
            //                    "Detected" : "Not Detected";
            //        break;
            //    case "btnLoadingConveyor":
            //        pos = enumTeachingPos.LoadingConveyor;
            //        pcbDetect = DIO.GetDIData(enumDINames.X03_0_Loading_Conveyor_Start_Sensor) ||
            //                            DIO.GetDIData(enumDINames.X03_1_Loading_Conveyor_Stop_Sensor) ||
            //                            DIO.GetDIData(enumDINames.X03_2_Loading_Conveyor_End_Sensor) ?
            //                    "Detected" : "Not Detected";
            //        break;
            //    case "btnUnloadingConveyor":
            //        pos = enumTeachingPos.UnloadingConveyor;
            //        pcbDetect = DIO.GetDIData(enumDINames.X03_5_Unloading_Conveyor_Start_Sensor) ||
            //                            DIO.GetDIData(enumDINames.X03_6_Unloading_Conveyor_Stop_Sensor) ||
            //                            DIO.GetDIData(enumDINames.X03_7_Unloading_Conveyor_End_Sensor) ?
            //                    "Detected" : "Not Detected";
            //        break;
            //    case "btnNg1":
            //        pos = enumTeachingPos.NG1;
            //        pcbDetect = DIO.GetDIData(enumDINames.X04_0_NG_Buffer_Start_Sensor)  ?
            //                    "Detected" : "Not Detected";
            //        break;
            //    case "btnNg2":
            //        pos = enumTeachingPos.NG2;
            //        pcbDetect = DIO.GetDIData(enumDINames.X04_1_NG_Buffer_End_Sensor) ?
            //                    "Detected" : "Not Detected";
            //        break;
            //    case "btnSite1":
            //    case "btnSite2":
            //    case "btnSite3":
            //    case "btnSite4":
            //    case "btnSite5":
            //    case "btnSite6":
            //    case "btnSite7":
            //    case "btnSite8":
            //    case "btnSite9":
            //    case "btnSite10":
            //    case "btnSite11":
            //    case "btnSite12":
            //    case "btnSite13":
            //    case "btnSite14":
            //    case "btnSite15":
            //    case "btnSite16":
            //    case "btnSite17":
            //    case "btnSite18":
            //    case "btnSite19":
            //    case "btnSite20":
            //    case "btnSite21":
            //        // 테스트사이트는 묶어서 처리
            //        pos = (enumTeachingPos)((int)enumTeachingPos.Site1 + (int.Parse(activeButton.Name.Replace("btnSite", ""))) - 1);
            //        int diIndex = (int)(enumDINames.X12_3_Module1_PCB_Sensor + GlobalVar.DIModuleGap * (pos - enumTeachingPos.Site1));
            //        pcbDetect = DIO.GetDIData(diIndex) ?
            //                    "Detected" : "Not Detected";
            //        break;
            //    default: // 해당 사항 없음
            //        return;
            //}

            //string text = activeButton.Name.Replace("btn", "") + " Info : \r\n\r\n"; // 사이트 이름
            //if (pcbDetect.Length > 0) // 센서 감지 유무
            //{
            //    text += "PCB " + pcbDetect + "\r\n\r\n";
            //}
            //text += "PCB Status - " + GlobalVar.AutoInline_PCBInfo[(int)pos].PCBStatus.ToString() + "\r\n\r\n"; // PCB 처리 상태
            //if (GlobalVar.AutoInline_PCBInfo[(int)pos].PCBStatus >= enumSMDStatus.Before_Command &&
            //    GlobalVar.AutoInline_PCBInfo[(int)pos].PCBStatus != enumSMDStatus.No_Test) // Array 정보
            //{
            //    for (int i = 0; i < GlobalVar.AutoInline_PCBInfo[(int)pos].SMDStatus.Length; i++)
            //    {
            //        if (GlobalVar.AutoInline_PCBInfo[(int)pos].SMDStatus[i] >= enumSMDStatus.Before_Command &&
            //            GlobalVar.AutoInline_PCBInfo[(int)pos].SMDStatus[i] != enumSMDStatus.No_Test)
            //        {
            //            text += "Array " + (i + 1).ToString() + " - " + 
            //                    GlobalVar.AutoInline_PCBInfo[(int)pos].Barcode[i] + " " +
            //                    GlobalVar.AutoInline_PCBInfo[(int)pos].SMDStatus[i].ToString() + "\r\n";
            //        }
            //    }
            //}

            //tbSiteInfo.Text = text; 

        }

        private void btnSiteOpen_Click(object sender, EventArgs e)
        {
            if (activeButton == null)
            {
                return;
            }
            enumTeachingPos site = enumTeachingPos.Site1 + (int.Parse(activeButton.Name.Replace("btnSite", "")) - 1);
            FuncInline.SiteOpen(site, true);
        }

        private void btnPartClear_Click(object sender, EventArgs e)
        {
            //bool diCheck = false;
            //enumTeachingPos pos = enumTeachingPos.RobotWait;
            //enumErrorPart part = enumErrorPart.Program;
            //switch (activeButton.Name)
            //{
            //    case "btnInputJig":
            //        pos = enumTeachingPos.InputJig;
            //        part = enumErrorPart.InputJig;
            //        diCheck = false;
            //        break;
            //    case "btnOutputJig":
            //        pos = enumTeachingPos.OutputJig;
            //        part = enumErrorPart.OutputJig;
            //        diCheck = false;
            //        break;
            //    case "btnInputLift":
            //        pos = enumTeachingPos.InputLift;
            //        part = enumErrorPart.InputLift;
            //        diCheck = DIO.GetDIData(enumDINames.X01_0_Input_Lift_Start_Sensor) ||
            //                    DIO.GetDIData(enumDINames.X01_1_Input_Lift_Stop_Sensor) ||
            //                    DIO.GetDIData(enumDINames.X01_2_Input_Lift_End_Sensor);
            //        break;
            //    case "btnOutputLift":
            //        pos = enumTeachingPos.OutputLift;
            //        part = enumErrorPart.OutputLift;
            //        diCheck = DIO.GetDIData(enumDINames.X01_5_Output_Lift_Start_Sensor) ||
            //                    DIO.GetDIData(enumDINames.X01_6_Output_Lift_Stop_Sensor) ||
            //                    DIO.GetDIData(enumDINames.X01_7_Output_Lift_End_Sensor);
            //        break;
            //    case "btnBufferConveyor":
            //        pos = enumTeachingPos.BufferConveyor;
            //        part = enumErrorPart.BufferConveyor;
            //        diCheck = DIO.GetDIData(enumDINames.X02_2_Buffer_Conveyor_Start_Sensor) ||
            //                    DIO.GetDIData(enumDINames.X02_3_Buffer_Conveyor_Stop_Sensor) ||
            //                    DIO.GetDIData(enumDINames.X02_4_Buffer_Conveyor_End_Sensor);
            //        break;
            //    case "btnInputConveyor":
            //        pos = enumTeachingPos.InputConveyor;
            //        part = enumErrorPart.InputConveyor;
            //        diCheck = DIO.GetDIData(enumDINames.X02_5_Input_Conveyor_Start_Sensor) ||
            //                    DIO.GetDIData(enumDINames.X02_6_Input_Conveyor_Stop_Sensor) ||
            //                    DIO.GetDIData(enumDINames.X02_7_Input_Conveyor_End_Sensor);
            //        break;
            //    case "btnLoadingConveyor":
            //        pos = enumTeachingPos.LoadingConveyor;
            //        part = enumErrorPart.LoadingConveyor;
            //        diCheck = DIO.GetDIData(enumDINames.X03_0_Loading_Conveyor_Start_Sensor) ||
            //                    DIO.GetDIData(enumDINames.X03_1_Loading_Conveyor_Stop_Sensor) ||
            //                    DIO.GetDIData(enumDINames.X03_2_Loading_Conveyor_End_Sensor);
            //        break;
            //    case "btnUnloadingConveyor":
            //        pos = enumTeachingPos.UnloadingConveyor;
            //        part = enumErrorPart.UnloadingConveyor;
            //        diCheck = DIO.GetDIData(enumDINames.X03_5_Unloading_Conveyor_Start_Sensor) ||
            //                    DIO.GetDIData(enumDINames.X03_6_Unloading_Conveyor_Stop_Sensor) ||
            //                    DIO.GetDIData(enumDINames.X03_7_Unloading_Conveyor_End_Sensor);
            //        break;
            //    case "btnNg1":
            //        pos = enumTeachingPos.NG1;
            //        part = enumErrorPart.NGConveyor;
            //        diCheck = DIO.GetDIData(enumDINames.X04_0_NG_Buffer_Start_Sensor);
            //        break;
            //    case "btnNg2":
            //        pos = enumTeachingPos.NG2;
            //        part = enumErrorPart.NGConveyor;
            //        diCheck = DIO.GetDIData(enumDINames.X04_1_NG_Buffer_End_Sensor);
            //        break;
            //    case "btnSite1":
            //    case "btnSite2":
            //    case "btnSite3":
            //    case "btnSite4":
            //    case "btnSite5":
            //    case "btnSite6":
            //    case "btnSite7":
            //    case "btnSite8":
            //    case "btnSite9":
            //    case "btnSite10":
            //    case "btnSite11":
            //    case "btnSite12":
            //    case "btnSite13":
            //    case "btnSite14":
            //    case "btnSite15":
            //    case "btnSite16":
            //    case "btnSite17":
            //    case "btnSite18":
            //    case "btnSite19":
            //    case "btnSite20":
            //    case "btnSite21":
            //        // 테스트사이트는 묶어서 처리
            //        pos = (enumTeachingPos)((int)enumTeachingPos.Site1 + (int.Parse(activeButton.Name.Replace("btnSite", ""))) - 1);
            //        diCheck = DIO.GetDIData(enumDINames.X12_3_Module1_PCB_Sensor + GlobalVar.DIModuleGap * (pos - enumTeachingPos.Site1));
            //        break;
            //    default: // 해당 사항 없음
            //        return;
            //}

            //if (diCheck)
            //{
            //    Util.TopMessageBox("Can't clear part while PCB detected. Remove PCB and clear again.");
            //    return;
            //}

            //// PCB 정보 삭제
            //if (pos != enumTeachingPos.RobotWait)
            //{
            //    FuncInline.ClearPCBInfo(pos);
            //}

            //#region lift/conveyor action 삭제. 
            //// 앞뒤가 같이 묶여 있는데 같이 처리? 
            //// 앞쪽 먼저 처리하고 뒤쪽은 다시 판단하면 될까?
            //// 버튼 상태는 어떤 기준으로 표시?
            //if (activeButton.Name == "btnInputLift") // 투입 리프트
            //{
            //    if (GlobalVar.InputLiftAction == enumLiftAction.Output) // 배출시
            //    {

            //    }
            //    else if (GlobalVar.InputLiftAction == enumLiftAction.Input) // 공급시. 투입리프트는 앞쪽이 별개므로 공급시 판단을 독자로 함
            //    {
            //        // 공급시 제거는 스메마 끄고 제품 제거로. 
            //    } 
            //    GlobalVar.InputLiftAction = enumLiftAction.None;
            //}
            //// PCB 감지는 앞에서 거르고 있다. 그러므로 PCB 여부는 판단할 필요가 없지만. PCBInfo는 확인해야 함.
            //// 배출시는 OutputLIft 제외하고 PCB 상태 초기화하고 뒷라인 감지여부에 따라 뒷라인 세팅
            //// 투입시는 InputLIft 제외하고 앞쪽 상황 따라 다시 판단
            //#endregion

            //// 해당 파트 에러 삭제
            //if (part != enumErrorPart.Program)
            //{
            //    Func.RemoveError(part);
            //}
        }

        private void btnAlarmClear_Click(object sender, EventArgs e)
        {
            FuncError.RemoveAllError();
            btnRefresh_Click(sender, e);
        }

        private void btnSiteClose_Click(object sender, EventArgs e)
        {
            if (activeButton == null)
            {
                return;
            }
            enumTeachingPos site = enumTeachingPos.Site1 + (int.Parse(activeButton.Name.Replace("btnSite", "")) - 1);
            FuncInline.SiteClose(site);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //String sql = "select [Date],[Time],[Part],[ErrorCode],[ErrorName],[Description] " +
            //                "from [SystemError] " +
            //                "where [Clear] = '0' " +
            //                "order by [Date] desc, [Time] desc";
            //SqlDataReader rs = GlobalVar.Sql.Read(sql);
            string[,] rs = FuncInline.GetUnClearedSystemError();
            if (rs != null)
            {
                dataGridError.Rows.Clear();
                int rowNum = 0;
                for (int i = 0; i < rs.GetLength(0); i++)
                {
                    dataGridError.Rows.Add(rs[i,0].ToString(), rs[i,1].ToString(), rs[i,2].ToString(), rs[i,3].ToString(), rs[i,4].ToString(), rs[i,5].ToString());
                    dataGridError.Rows[rowNum].DefaultCellStyle.BackColor = rowNum % 2 > 0 ? Color.Cyan : Color.WhiteSmoke;

                    rowNum++;
                }
            }
        }
    }
}
