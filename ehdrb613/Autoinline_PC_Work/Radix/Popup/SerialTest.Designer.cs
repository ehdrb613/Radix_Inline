
namespace Radix
{
    partial class SerialTest
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tbReceiveText = new System.Windows.Forms.TextBox();
            this.tbProbe1 = new System.Windows.Forms.TextBox();
            this.tbSendText2 = new System.Windows.Forms.TextBox();
            this.tbSendText1 = new System.Windows.Forms.TextBox();
            this.cmbStopBit = new System.Windows.Forms.ComboBox();
            this.cmbHandshake = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbParity = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbData = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbConnected = new System.Windows.Forms.CheckBox();
            this.cmbBaud = new System.Windows.Forms.ComboBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.btn_Disconnect = new System.Windows.Forms.Button();
            this.btn_Connect = new System.Windows.Forms.Button();
            this.cmbPortName = new System.Windows.Forms.ComboBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // tbReceiveText
            // 
            this.tbReceiveText.Location = new System.Drawing.Point(277, 74);
            this.tbReceiveText.Multiline = true;
            this.tbReceiveText.Name = "tbReceiveText";
            this.tbReceiveText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbReceiveText.Size = new System.Drawing.Size(224, 152);
            this.tbReceiveText.TabIndex = 64;
            // 
            // tbProbe1
            // 
            this.tbProbe1.Location = new System.Drawing.Point(277, 241);
            this.tbProbe1.Name = "tbProbe1";
            this.tbProbe1.Size = new System.Drawing.Size(79, 21);
            this.tbProbe1.TabIndex = 58;
            this.tbProbe1.Text = "02";
            // 
            // tbSendText2
            // 
            this.tbSendText2.Location = new System.Drawing.Point(310, 37);
            this.tbSendText2.Name = "tbSendText2";
            this.tbSendText2.Size = new System.Drawing.Size(27, 21);
            this.tbSendText2.TabIndex = 57;
            this.tbSendText2.Text = "52";
            // 
            // tbSendText1
            // 
            this.tbSendText1.Location = new System.Drawing.Point(277, 37);
            this.tbSendText1.Name = "tbSendText1";
            this.tbSendText1.Size = new System.Drawing.Size(27, 21);
            this.tbSendText1.TabIndex = 56;
            this.tbSendText1.Text = "01";
            // 
            // cmbStopBit
            // 
            this.cmbStopBit.FormattingEnabled = true;
            this.cmbStopBit.Items.AddRange(new object[] {
            "None",
            "One",
            "Two",
            "OnePointFive"});
            this.cmbStopBit.Location = new System.Drawing.Point(127, 148);
            this.cmbStopBit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbStopBit.Name = "cmbStopBit";
            this.cmbStopBit.Size = new System.Drawing.Size(78, 20);
            this.cmbStopBit.TabIndex = 55;
            // 
            // cmbHandshake
            // 
            this.cmbHandshake.FormattingEnabled = true;
            this.cmbHandshake.Items.AddRange(new object[] {
            "none",
            "Xon/Xoff",
            "request to send",
            "request to send Xon/Xoff"});
            this.cmbHandshake.Location = new System.Drawing.Point(127, 124);
            this.cmbHandshake.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbHandshake.Name = "cmbHandshake";
            this.cmbHandshake.Size = new System.Drawing.Size(78, 20);
            this.cmbHandshake.TabIndex = 54;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(46, 150);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 12);
            this.label1.TabIndex = 52;
            this.label1.Text = "StopBit";
            // 
            // cmbParity
            // 
            this.cmbParity.FormattingEnabled = true;
            this.cmbParity.Items.AddRange(new object[] {
            "none",
            "even",
            "mark",
            "odd",
            "space"});
            this.cmbParity.Location = new System.Drawing.Point(127, 102);
            this.cmbParity.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbParity.Name = "cmbParity";
            this.cmbParity.Size = new System.Drawing.Size(78, 20);
            this.cmbParity.TabIndex = 53;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(46, 126);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 12);
            this.label5.TabIndex = 51;
            this.label5.Text = "Handshake";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(46, 104);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 12);
            this.label6.TabIndex = 50;
            this.label6.Text = "Parity";
            // 
            // cmbData
            // 
            this.cmbData.FormattingEnabled = true;
            this.cmbData.Items.AddRange(new object[] {
            "8",
            "7",
            "6"});
            this.cmbData.Location = new System.Drawing.Point(127, 80);
            this.cmbData.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbData.Name = "cmbData";
            this.cmbData.Size = new System.Drawing.Size(78, 20);
            this.cmbData.TabIndex = 49;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(46, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 12);
            this.label4.TabIndex = 48;
            this.label4.Text = "Data";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(46, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 12);
            this.label3.TabIndex = 47;
            this.label3.Text = "Baudrate";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(46, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 12);
            this.label2.TabIndex = 46;
            this.label2.Text = "Port";
            // 
            // cbConnected
            // 
            this.cbConnected.AutoSize = true;
            this.cbConnected.Location = new System.Drawing.Point(25, 187);
            this.cbConnected.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cbConnected.Name = "cbConnected";
            this.cbConnected.Size = new System.Drawing.Size(15, 14);
            this.cbConnected.TabIndex = 45;
            this.cbConnected.UseVisualStyleBackColor = true;
            // 
            // cmbBaud
            // 
            this.cmbBaud.FormattingEnabled = true;
            this.cmbBaud.Items.AddRange(new object[] {
            "1200",
            "2400",
            "4800",
            "9600",
            "19200",
            "38400",
            "57600",
            "115200"});
            this.cmbBaud.Location = new System.Drawing.Point(127, 59);
            this.cmbBaud.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbBaud.Name = "cmbBaud";
            this.cmbBaud.Size = new System.Drawing.Size(78, 20);
            this.cmbBaud.TabIndex = 44;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(409, 38);
            this.btnSend.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(80, 23);
            this.btnSend.TabIndex = 40;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btn_Disconnect
            // 
            this.btn_Disconnect.Location = new System.Drawing.Point(120, 187);
            this.btn_Disconnect.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_Disconnect.Name = "btn_Disconnect";
            this.btn_Disconnect.Size = new System.Drawing.Size(83, 39);
            this.btn_Disconnect.TabIndex = 42;
            this.btn_Disconnect.Text = "DisConnect";
            this.btn_Disconnect.UseVisualStyleBackColor = true;
            this.btn_Disconnect.Click += new System.EventHandler(this.btn_Disconnect_Click);
            // 
            // btn_Connect
            // 
            this.btn_Connect.Location = new System.Drawing.Point(46, 187);
            this.btn_Connect.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_Connect.Name = "btn_Connect";
            this.btn_Connect.Size = new System.Drawing.Size(68, 38);
            this.btn_Connect.TabIndex = 41;
            this.btn_Connect.Text = "Connect1";
            this.btn_Connect.UseVisualStyleBackColor = true;
            this.btn_Connect.Click += new System.EventHandler(this.btn_Connect_Click);
            // 
            // cmbPortName
            // 
            this.cmbPortName.FormattingEnabled = true;
            this.cmbPortName.Location = new System.Drawing.Point(127, 38);
            this.cmbPortName.Name = "cmbPortName";
            this.cmbPortName.Size = new System.Drawing.Size(78, 20);
            this.cmbPortName.TabIndex = 38;
            // 
            // timer1
            // 
            this.timer1.Interval = 300;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // SerialTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tbReceiveText);
            this.Controls.Add(this.tbProbe1);
            this.Controls.Add(this.tbSendText2);
            this.Controls.Add(this.tbSendText1);
            this.Controls.Add(this.cmbStopBit);
            this.Controls.Add(this.cmbHandshake);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbParity);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cmbData);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbConnected);
            this.Controls.Add(this.cmbBaud);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.btn_Disconnect);
            this.Controls.Add(this.btn_Connect);
            this.Controls.Add(this.cmbPortName);
            this.Name = "SerialTest";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SerialTest_FormClosed_1);
            this.Load += new System.EventHandler(this.SerialTest_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbReceiveText;
        private System.Windows.Forms.TextBox tbProbe1;
        private System.Windows.Forms.TextBox tbSendText2;
        private System.Windows.Forms.TextBox tbSendText1;
        private System.Windows.Forms.ComboBox cmbStopBit;
        private System.Windows.Forms.ComboBox cmbHandshake;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbParity;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbData;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbConnected;
        private System.Windows.Forms.ComboBox cmbBaud;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btn_Disconnect;
        private System.Windows.Forms.Button btn_Connect;
        private System.Windows.Forms.ComboBox cmbPortName;
        private System.Windows.Forms.Timer timer1;
    }
}