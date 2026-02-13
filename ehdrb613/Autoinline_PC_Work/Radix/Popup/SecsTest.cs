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

namespace Radix
{
    public partial class SecsTest : Form
    {
        public SecsTest()
        {
            InitializeComponent();
        }

        private void btnRUThere_Click(object sender, EventArgs e)
        {
            GlobalVar.Secs.S1F1_RUThere();
        }


        private void btnAlarmReport_Click(object sender, EventArgs e)
        {
            GlobalVar.Secs.S5F1_AlarmReportSend(cbAlarm.Checked, (enumAlarmCode)cmbAlarmCode.SelectedIndex, (uint)numAlarmID.Value, tbAlarmText.Text);
        }


        private void btnProcessProgramSend_Click(object sender, EventArgs e)
        {
            GlobalVar.Secs.ClearSECS_RecipeList();
            string[] lines = tbRecipeList.Text.Split('\n');
            for (int i = 0; i < lines.Length && i < 100; i++)
            {
                if (lines[i] != null &&
                    lines[i].Length > 0)
                {
                    GlobalVar.Secs.SetSECS_RecipeList(lines[i].Trim());
                }
            }
            //Console.WriteLine("S7F3 전송 전");
            GlobalVar.Secs.S7F3_ProcessProgramSend();
            //Console.WriteLine("S7F3 전송 완료");
        }

        private void btnCurrentEPPDReport_Click(object sender, EventArgs e)
        {
            if (tbCurrentEPPD.Text.Length > 0)
            {
                GlobalVar.Secs.SetSECS_RecipeID(tbCurrentEPPD.Text);
                GlobalVar.Secs.S7F19_CurrentEPPDRequestToHost();
            }
        }

        private void btnEstablishCommunication_Click(object sender, EventArgs e)
        {
            GlobalVar.Secs.S1F13_EstablishCommunication();
        }

        private void btnUnrecognizedDeviceID_Click(object sender, EventArgs e)
        {
            GlobalVar.Secs.S9F1_UnrecognizedDeviceID();
        }

        private void btnUnrecognizedStreamType_Click(object sender, EventArgs e)
        {
            GlobalVar.Secs.S9F3_UnrecognizedStreamType();
        }

        private void btnUnrecognizedFunctionType_Click(object sender, EventArgs e)
        {
            GlobalVar.Secs.S9F5_UnrecognizedFunctionType();
        }

        private void btnIllegalData_Click(object sender, EventArgs e)
        {
            GlobalVar.Secs.S9F7_IllegalData();
        }

        private void btnTransactionTimerTimeout_Click(object sender, EventArgs e)
        {
            GlobalVar.Secs.S9F9_TransactionTimerTimeout();
        }

        private void btnConversationTimeout_Click(object sender, EventArgs e)
        {
            GlobalVar.Secs.S9F13_ConversationTimeout();
        }

        private void btnControlStateChange_Click(object sender, EventArgs e)
        {
            // EQPID
            GlobalVar.Secs.SetSECS_EQPID(tbEQPId.Text);
            // Operator ID
            GlobalVar.Secs.SetSECS_OperatorID(tbOperatorID.Text);
            // Time
            // Control State
            GlobalVar.Secs.SetControlState((enumControlState)cmbControlState.SelectedIndex);
            GlobalVar.Secs.S6F11_EventReport(enumECID.ControlStateChange);
        }

        private void btnProcessStateChange_Click(object sender, EventArgs e)
        {
            // EQPID
            GlobalVar.Secs.SetSECS_EQPID(tbEQPId.Text);
            // Operator ID
            GlobalVar.Secs.SetSECS_OperatorID(tbOperatorID.Text);
            // Time
            // Process State
            GlobalVar.Secs.SetSECS_ProcessState((enumProcessState)cmbProcessState.SelectedIndex);
            // Lot ID
            GlobalVar.Secs.SetSECS_LotID(tbLOTID.Text);
            // Product ID
            GlobalVar.Secs.SetSECS_ProductID(tbProductID.Text);
            GlobalVar.Secs.S6F11_EventReport(enumECID.ProcessStateChange);
        }

        private void btnEquipmentStateChange_Click(object sender, EventArgs e)
        {
            // EQPID
            GlobalVar.Secs.SetSECS_EQPID(tbEQPId.Text);
            // Operator ID
            GlobalVar.Secs.SetSECS_OperatorID(tbOperatorID.Text);
            // Time
            // Equipment State
            GlobalVar.Secs.SetSECS_EquipmentState((enumEquipmentState)cmbEQPState.SelectedIndex);
            GlobalVar.Secs.S6F11_EventReport(enumECID.EquipmentStateChange);
        }

        private void btnTRSModeChange_Click(object sender, EventArgs e)
        {
            // EQPID
            GlobalVar.Secs.SetSECS_EQPID(tbEQPId.Text);
            // Operator ID
            GlobalVar.Secs.SetSECS_OperatorID(tbOperatorID.Text);
            // Time
            // TRS Mode
            GlobalVar.Secs.SetSECS_TRSMode((enumTransferControlMode)cmbTRSMode.SelectedIndex);
            GlobalVar.Secs.S6F11_EventReport(enumECID.TRSModeChange);
        }

        private void btnRecipeChange_Click(object sender, EventArgs e)
        {
            // EQPID
            GlobalVar.Secs.SetSECS_EQPID(tbEQPId.Text);
            // Operator ID
            GlobalVar.Secs.SetSECS_OperatorID(tbOperatorID.Text);
            // Time
            // Recipe ID
            GlobalVar.Secs.SetSECS_RecipeID(tbRecipeID.Text);
            GlobalVar.Secs.S6F11_EventReport(enumECID.RecipeChange);
        }

        private void btnPortStateChange_Click(object sender, EventArgs e)
        {
            // EQPID
            GlobalVar.Secs.SetSECS_EQPID(tbEQPId.Text);
            // Operator ID
            GlobalVar.Secs.SetSECS_OperatorID(tbOperatorID.Text);
            // Time
            // Port State List
            //GlobalVar.Secs.ClearSECS_Port_Stage_State();
            //GlobalVar.Secs.ClearSECS_Port_Stage_Count();
            //string[] lines = tbPortList.Text.Split('\n');
            //for (int i = 0; i < lines.Length && i < 100; i++)
            //{
            //    if (lines[i] != null &&
            //        lines[i].Length > 0)
            //    {
            //        string[] line = lines[i].Split(':');
            //        int idx1 = int.Parse(line[0].Trim());
            //        int idx2 = int.Parse(line[1].Trim());
            //        if (idx2 + 1 > GlobalVar.Secs.GetSECS_Port_Stage_Count((enumPortID)idx2))
            //        {
            //            GlobalVar.Secs.SetSECS_Port_Stage_Count((enumPortID)idx2, idx2 + 1);
            //        }
            //        GlobalVar.Secs.SetSECS_Port_Stage_State((enumPortID)idx1, idx2, line[2].Trim(), "", line[2].Trim());
            //    }
            //}
            GlobalVar.Secs.S6F11_EventReport(enumECID.PortStateChange);
        }

        private void btnRecipeListChange_Click(object sender, EventArgs e)
        {
            // EQPID
            GlobalVar.Secs.SetSECS_EQPID(tbEQPId.Text);
            // Operator ID
            GlobalVar.Secs.SetSECS_OperatorID(tbOperatorID.Text);
            // Time
            // Recipe List
            //*
            GlobalVar.Secs.ClearSECS_RecipeList();
            string[] lines = tbRecipeList.Text.Split('\n');
            for (int i = 0; i < lines.Length && i < 100; i++)
            {
                if (lines[i] != null &&
                    lines[i].Length > 0)
                {
                    GlobalVar.Secs.SetSECS_RecipeList(lines[i].Trim());
                }
            }
            //*/
            //GlobalVar.SECS_RecipeList[0] = "Recipe1";
            //GlobalVar.SECS_RecipeList[1] = "Recipe2";
            GlobalVar.Secs.S6F11_EventReport(enumECID.RecipeListChange);
        }

        private void btnProductProcessData_Click(object sender, EventArgs e)
        {
            GlobalVar.Secs.SetSECS_ProductID(GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.M, 0).ProductID); // YJ20210929 U에서 M으로 변경
            GlobalVar.Secs.SetSECS_RecipeID(GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.M, 0).RecipeID); // YJ20210929 U에서 M으로 변경
            //GlobalVar.Secs.SetSECS_TotalProductCount(GlobalVar.AXT_totoalProductCount); //YJ20210929 총생산량 직접지정에서 증가로 변경
            GlobalVar.Secs.SetSECS_TotalProductCountAdd(GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.M, 0).LotID); // YJ20210929 총생산량 직접지정에서 증가로 변경
            GlobalVar.Secs.SetSECS_ProcessJudge(enumProcessJudge.OK); // 검사기 이외에는 의미 없어서 OK로

            GlobalVar.Secs.ClearSECS_ProcessData();
            GlobalVar.Secs.SetSECS_ProcessData("Result", "OK");
            GlobalVar.Secs.SetSECS_ProcessData("Label_Data", GlobalVar.Label_Barcode);
            GlobalVar.Secs.S6F11_EventReport(enumECID.ProductProcessData);
            /*
                     // EQPID
                     GlobalVar.Secs.SetSECS_EQPID(tbEQPId.Text);
                     // Operator ID
                     GlobalVar.Secs.SetSECS_OperatorID(tbOperatorID.Text);
                     // Time
                     // Lot ID
                     GlobalVar.Secs.SetSECS_LotID(tbLOTID.Text);
                     // Product ID
                     GlobalVar.Secs.SetSECS_ProductID(tbOperatorID.Text);
                     // Recipe ID
                     GlobalVar.Secs.SetSECS_RecipeID(tbRecipeID.Text);
                     // Total Product Count            
                     GlobalVar.Secs.SetSECS_TotalProductCount(tbLOTID.Text, (uint)numTotalProductCount.Value); // YJ20210929 LotID 추가
                     // Process Judge
                     GlobalVar.Secs.SetSECS_ProcessJudge((enumProcessJudge)cmbProcessJudge.SelectedIndex);
                     // Process Data List
                     GlobalVar.Secs.ClearSECS_ProcessData();
                     string[] lines = tbProcessDataList.Text.Split('\n');
                     for (int i = 0; i < lines.Length && i < 100; i++)
                     {
                         if (lines[i] != null &&
                             lines[i].Trim().Length > 0)
                         {
                             string[] line = lines[i].Trim().Split(':');
                             GlobalVar.Secs.SetSECS_ProcessData(line[0].Trim(), line[1].Trim());
                         }
                     }
                     GlobalVar.Secs.S6F11_EventReport(enumECID.ProductProcessData);

                     GlobalVar.Secs.SetSECS_ProcessState(enumProcessState.Idle); // Process Data Report시 Host에서는 Process State 가 idle로 바뀌므로 Eq에서도 강제로 바꾼다.
                     */
        }

        private void btnScrap_Click(object sender, EventArgs e)
        {
            // EQPID
            GlobalVar.Secs.SetSECS_EQPID(tbEQPId.Text);
            // Operator ID
            GlobalVar.Secs.SetSECS_OperatorID(tbOperatorID.Text);
            // Time
            // Product ID
            GlobalVar.Secs.SetSECS_ProductID(tbProductID.Text);
            // Scrap Code
            GlobalVar.Secs.SetSECS_ScrapCode((enumScrapCode)cmbScrapCode.SelectedIndex);
            GlobalVar.Secs.S6F11_EventReport(enumECID.ScrapReport);
        }

        private void btnProcessAbort_Click(object sender, EventArgs e)
        {
            // EQPID
            GlobalVar.Secs.SetSECS_EQPID(tbEQPId.Text);
            // Operator ID
            GlobalVar.Secs.SetSECS_OperatorID(tbOperatorID.Text);
            // Time
            // Product ID
            GlobalVar.Secs.SetSECS_ProductID(tbProductID.Text);
            GlobalVar.Secs.S6F11_EventReport(enumECID.ProcessAbort);
        }

        private void btnProductProcessStart_Click(object sender, EventArgs e)
        {
            // EQPID
            GlobalVar.Secs.SetSECS_EQPID(tbEQPId.Text);
            // Operator ID
            GlobalVar.Secs.SetSECS_OperatorID(tbOperatorID.Text);
            // Time
            // Product ID
            GlobalVar.Secs.SetSECS_ProductID(tbProductID.Text);
            GlobalVar.Secs.S6F11_EventReport(enumECID.ProductProcessStart);
        }

        private void btnProcessCancel_Click(object sender, EventArgs e)
        {
            // EQPID
            GlobalVar.Secs.SetSECS_EQPID(tbEQPId.Text);
            // Operator ID
            GlobalVar.Secs.SetSECS_OperatorID(tbOperatorID.Text);
            // Time
            // Product ID
            GlobalVar.Secs.SetSECS_ProductID(tbProductID.Text);
            GlobalVar.Secs.S6F11_EventReport(enumECID.ProcessCancel);
        }

        private void btnProductID_Click(object sender, EventArgs e)
        {
            // EQPID
            GlobalVar.Secs.SetSECS_EQPID(tbEQPId.Text);
            // Operator ID
            GlobalVar.Secs.SetSECS_OperatorID(tbOperatorID.Text);
            // Time
            // Product ID
            GlobalVar.Secs.SetSECS_ProductID(tbProductID.Text);
            GlobalVar.Secs.S6F11_EventReport(enumECID.ProductID);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            GlobalVar.Secs.SendPrimary(tbSecsMessage.Text);
        }

        private void SecsTest_Shown(object sender, EventArgs e)
        {
            // 콤보박스 아이템 세팅
            cmbAlarmCode.Items.Clear();
            string[] enumNames = Enum.GetNames(typeof(enumAlarmCode));
            for (int i = 0; i < enumNames.Length; i++)
            {
                cmbAlarmCode.Items.Add(enumNames[i]);
            }
            if (cmbAlarmCode.Items.Count > 0)
            {
                cmbAlarmCode.SelectedIndex = 0;
            }


            cmbControlState.Items.Clear();
            enumNames = Enum.GetNames(typeof(enumControlState));
            for (int i = 0; i < enumNames.Length; i++)
            {
                cmbControlState.Items.Add(enumNames[i]);
            }
            if (cmbControlState.Items.Count > 0)
            {
                cmbControlState.SelectedIndex = 0;
            }

            cmbEQPState.Items.Clear();
            enumNames = Enum.GetNames(typeof(enumEquipmentState));
            for (int i = 0; i < enumNames.Length; i++)
            {
                cmbEQPState.Items.Add(enumNames[i]);
            }
            if (cmbEQPState.Items.Count > 0)
            {
                cmbEQPState.SelectedIndex = 0;
            }

            cmbProcessJudge.Items.Clear();
            enumNames = Enum.GetNames(typeof(enumProcessJudge));
            for (int i = 0; i < enumNames.Length; i++)
            {
                cmbProcessJudge.Items.Add(enumNames[i]);
            }
            if (cmbProcessJudge.Items.Count > 0)
            {
                cmbProcessJudge.SelectedIndex = 0;
            }

            cmbProcessState.Items.Clear();
            enumNames = Enum.GetNames(typeof(enumProcessState));
            for (int i = 0; i < enumNames.Length; i++)
            {
                cmbProcessState.Items.Add(enumNames[i]);
            }
            if (cmbProcessState.Items.Count > 0)
            {
                cmbProcessState.SelectedIndex = 0;
            }

            cmbScrapCode.Items.Clear();
            enumNames = Enum.GetNames(typeof(enumScrapCode));
            for (int i = 0; i < enumNames.Length; i++)
            {
                cmbScrapCode.Items.Add(enumNames[i]);
            }
            if (cmbScrapCode.Items.Count > 0)
            {
                cmbScrapCode.SelectedIndex = 0;
            }

            cmbTRSMode.Items.Clear();
            enumNames = Enum.GetNames(typeof(enumTransferControlMode));
            for (int i = 0; i < enumNames.Length; i++)
            {
                cmbTRSMode.Items.Add(enumNames[i]);
            }
            if (cmbTRSMode.Items.Count > 0)
            {
                cmbTRSMode.SelectedIndex = 0;
            }

            cmbSenario.Items.Clear();
            enumNames = Enum.GetNames(typeof(enumSecsSenario));
            for (int i = 0; i < enumNames.Length; i++)
            {
                cmbSenario.Items.Add(enumNames[i]);
            }
            if (cmbSenario.Items.Count > 0)
            {
                cmbSenario.SelectedIndex = 0;
            }


            cmbSetPort.Items.Clear();
            cmbFromPort.Items.Clear();
            cmbToPort.Items.Clear();
            cmbRotatePort.Items.Clear();
            enumNames = Enum.GetNames(typeof(enumPortID));
            for (int i = 0; i < enumNames.Length; i++)
            {
                cmbSetPort.Items.Add(enumNames[i]);
                cmbFromPort.Items.Add(enumNames[i]);
                cmbToPort.Items.Add(enumNames[i]);
                cmbRotatePort.Items.Add(enumNames[i]);
            }
            if (cmbSetPort.Items.Count > 0)
            {
                cmbSetPort.SelectedIndex = 0;
                cmbFromPort.SelectedIndex = 0;
                cmbToPort.SelectedIndex = 0;
                cmbRotatePort.SelectedIndex = 0;
            }
        }

        private void btnSenarioStart_Click(object sender, EventArgs e)
        {
            if (!GlobalVar.SenarioStart)
            {
                GlobalVar.Senario = (enumSecsSenario)(cmbSenario.SelectedIndex);
                GlobalVar.SenarioStart = true;

                SecsThread secsThread = new SecsThread();
                Thread auto = new Thread(secsThread.Run);
                auto.Start();
            }
        }

        private void btnSenarioStop_Click(object sender, EventArgs e)
        {
            GlobalVar.SenarioStart = false;
        }

        private void btnProductIDList_Click(object sender, EventArgs e)
        {
            // EQPID
            GlobalVar.Secs.SetSECS_EQPID(tbEQPId.Text);
            // Operator ID
            GlobalVar.Secs.SetSECS_OperatorID(tbOperatorID.Text);
            // Time
            // Recipe List
            //*
            GlobalVar.Secs.ClearSECS_ProductList();
            string[] lines = tbProductList.Text.Split('\n');
            for (int i = 0; i < lines.Length && i < 100; i++)
            {
                if (lines[i] != null &&
                    lines[i].Length > 0)
                {
                    GlobalVar.Secs.SetSECS_ProductList(lines[i].Trim());
                }
            }
            GlobalVar.Secs.S6F11_EventReport(enumECID.ProductIDList);
        }

        private void btnSetPort_Click(object sender, EventArgs e)
        {
            GlobalVar.Secs.SetSECS_Port_Stage_State((enumPortID)cmbSetPort.SelectedIndex, (int)numSetStage.Value, tbSetProductID.Text, tbSetLotID.Text, tbSetRecipeID.Text);
        }

        private void btnMovePort_Click(object sender, EventArgs e)
        {
            GlobalVar.Secs.SetSECS_Port_Stage_Move((enumPortID)cmbFromPort.SelectedIndex, (int)numFromStage.Value, (enumPortID)cmbToPort.SelectedIndex, (int)numToStage.Value);
        }

        private void btnRotatePort_Click(object sender, EventArgs e)
        {
            GlobalVar.Secs.SetSECS_Port_Stage_Rotate((enumPortID)cmbRotatePort.SelectedIndex, (int)numRotateCount.Value);
        }

        private void btnShowPort_Click(object sender, EventArgs e)
        {
            string txt = "";
            string[] names = Enum.GetNames(typeof(enumPortID));
            for ( int j = 0; j < names.Length; j++)
            {
                for (int i = 0; i < GlobalVar.Secs.GetSECS_Port_Stage_Count((enumPortID)j); i++)
                {
                    structProductInfo info = GlobalVar.Secs.GetSECS_Port_Stage_Product((enumPortID)j, i);
                    txt += names[j] + " - " + i + " : " + info.ProductID + "," + info.LotID + "," + info.RecipeID + "\r\n";
                }
            }
            tbPortState.Text = txt;
        }
    }
}
