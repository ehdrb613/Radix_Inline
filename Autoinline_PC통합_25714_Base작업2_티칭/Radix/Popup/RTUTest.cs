using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Radix
{
    public partial class RTUTest : Form
    {
        //PMCRTU Rtu = new PMCRTU();

        public RTUTest()
        {
            InitializeComponent();
        }

        private void RTUTest_Load(object sender, EventArgs e)
        {
            //-------------------PMC 파라미터 초기화--------------------//  
            AxisList.Items.Clear();
            AxisList.Items.Add("X Axis");
            AxisList.Items.Add("Y Axis");
            //AxisList.Items.Add("XY Axis"); // 한 축씩만 제어하도록 한다.
            AxisList.SelectedIndex = 0;
            SpeedX_Textbox.Text = "1";
            SpeedY_Textbox.Text = "1";
            GetposTextbox_X.Text = "0";
            GetposTextbox_Y.Text = "0";
            ABS_X_Textbox.Text = "10";
            ABS_Y_Textbox.Text = "10";
            INC_X_Textbox.Text = "10";
            INC_Y_Textbox.Text = "10";
            //-------------------PMC 파라미터 초기화--------------------//  

            //-------------------통신 파라미터 설정 구성--------------------//  
            string[] Comlist = System.IO.Ports.SerialPort.GetPortNames();
            if (Comlist.Length > 0)
            {
                Port_CB.Items.AddRange(Comlist);
                Port_CB.SelectedIndex = 0;
            }

            Baud_CB.Items.Add("9600");
            Baud_CB.Items.Add("19200");
            Baud_CB.Items.Add("38400");
            Baud_CB.Items.Add("57600");
            Baud_CB.Items.Add("115200");
            Baud_CB.SelectedIndex = 0;
            //-------------------통신 파라미터 설정 구성--------------------//  

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //if (Rtu.Connected())
            //{
            //    GetposTextbox_X.Text = FuncInline.PMCStatus[0].Position.ToString("F3");
            //    GetposTextbox_Y.Text = FuncInline.PMCStatus[1].Position.ToString("F3");
            //}
        }

        private void Open_Btn_Click(object sender, EventArgs e)
        {
            //Rtu.SetConnection(Port_CB.SelectedItem.ToString(), int.Parse(Baud_CB.SelectedItem.ToString()));
            //if (Rtu.Connect())
            //{
            //    PortStateTextbox.Text = "Success Port Open";
            //}
            //else
            //{
            //    PortStateTextbox.Text = "Fail Port Open";
            //}
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Rtu.Disconnect();
            PortStateTextbox.Text = "Port Close";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FuncInline.enumPMCAxis ax = (FuncInline.enumPMCAxis)AxisList.SelectedIndex;
            //Rtu.ContMove(ax, 0 - Math.Abs(Convert.ToDouble(SpeedY_Textbox.Text)));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FuncInline.enumPMCAxis ax = (FuncInline.enumPMCAxis)AxisList.SelectedIndex;
            //Rtu.ContMove(ax, Math.Abs(Convert.ToDouble(SpeedY_Textbox.Text)));
        }

        private void button8_Click(object sender, EventArgs e)
        {
            FuncInline.enumPMCAxis ax = (FuncInline.enumPMCAxis)AxisList.SelectedIndex;
            //Rtu.Stop(ax);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            FuncInline.enumPMCAxis ax = (FuncInline.enumPMCAxis)AxisList.SelectedIndex;
            //Rtu.Reset();
        }

        private void ABS_START_btn_Click(object sender, EventArgs e)
        {
            FuncInline.enumPMCAxis ax = (FuncInline.enumPMCAxis)AxisList.SelectedIndex;
            //if (ax == FuncInline.enumPMCAxis.Input)
            //{
            //    Rtu.ABSMove(ax, Convert.ToDouble(ABS_X_Textbox.Text), Math.Abs(Convert.ToDouble(SpeedX_Textbox.Text)));
            //}
            //else if (ax == FuncInline.enumPMCAxis.Output)
            //{
            //    Rtu.ABSMove(ax, Convert.ToDouble(ABS_Y_Textbox.Text), Math.Abs(Convert.ToDouble(SpeedY_Textbox.Text)));
            //}
        }

        private void INC_START_btn_Click(object sender, EventArgs e)
        {
            FuncInline.enumPMCAxis ax = (FuncInline.enumPMCAxis)AxisList.SelectedIndex;
            //Rtu.INCMove(ax, Convert.ToDouble(ABS_X_Textbox.Text), Math.Abs(Convert.ToDouble(SpeedX_Textbox.Text)));
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            FuncInline.enumPMCAxis ax = (FuncInline.enumPMCAxis)AxisList.SelectedIndex;
            //Rtu.ClearPos(ax);
        }
    }
}
