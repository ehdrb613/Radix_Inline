namespace Radix
{
    partial class PCBLoaderTest
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
            this.btn_Connect = new System.Windows.Forms.Button();
            this.btn_Disconnect = new System.Windows.Forms.Button();
            this.cmbBaud = new System.Windows.Forms.ComboBox();
            this.numNodeNum = new System.Windows.Forms.NumericUpDown();
            this.cbConnected = new System.Windows.Forms.CheckBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btn_SendMessage = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Label_LoadDataReceive = new System.Windows.Forms.Label();
            this.textBox_Loder_Communication = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbPort = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbData = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbHandshake = new System.Windows.Forms.ComboBox();
            this.cmbParity = new System.Windows.Forms.ComboBox();
            this.btn_Unload_SendMessage = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numNodeNum)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Connect
            // 
            this.btn_Connect.Location = new System.Drawing.Point(28, 138);
            this.btn_Connect.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_Connect.Name = "btn_Connect";
            this.btn_Connect.Size = new System.Drawing.Size(68, 38);
            this.btn_Connect.TabIndex = 0;
            this.btn_Connect.Text = "Connect1";
            this.btn_Connect.UseVisualStyleBackColor = true;
            this.btn_Connect.Click += new System.EventHandler(this.btn_Connect_Click);
            // 
            // btn_Disconnect
            // 
            this.btn_Disconnect.Location = new System.Drawing.Point(28, 182);
            this.btn_Disconnect.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_Disconnect.Name = "btn_Disconnect";
            this.btn_Disconnect.Size = new System.Drawing.Size(120, 39);
            this.btn_Disconnect.TabIndex = 1;
            this.btn_Disconnect.Text = "DisConnect";
            this.btn_Disconnect.UseVisualStyleBackColor = true;
            this.btn_Disconnect.Click += new System.EventHandler(this.btn_Disconnect_Click);
            // 
            // cmbBaud
            // 
            this.cmbBaud.FormattingEnabled = true;
            this.cmbBaud.Items.AddRange(new object[] {
            "Baud1200",
            "Baud2400",
            "Baud4800",
            "Baud9600",
            "Baud19200",
            "Baud38400",
            "Baud57600",
            "Baud115200"});
            this.cmbBaud.Location = new System.Drawing.Point(101, 41);
            this.cmbBaud.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbBaud.Name = "cmbBaud";
            this.cmbBaud.Size = new System.Drawing.Size(78, 20);
            this.cmbBaud.TabIndex = 6;
            // 
            // numNodeNum
            // 
            this.numNodeNum.Location = new System.Drawing.Point(293, 378);
            this.numNodeNum.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.numNodeNum.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numNodeNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numNodeNum.Name = "numNodeNum";
            this.numNodeNum.Size = new System.Drawing.Size(78, 21);
            this.numNodeNum.TabIndex = 5;
            this.numNodeNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // cbConnected
            // 
            this.cbConnected.AutoSize = true;
            this.cbConnected.Location = new System.Drawing.Point(183, 151);
            this.cbConnected.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cbConnected.Name = "cbConnected";
            this.cbConnected.Size = new System.Drawing.Size(15, 14);
            this.cbConnected.TabIndex = 7;
            this.cbConnected.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(28, 224);
            this.textBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(164, 21);
            this.textBox1.TabIndex = 8;
            // 
            // btn_SendMessage
            // 
            this.btn_SendMessage.Location = new System.Drawing.Point(28, 256);
            this.btn_SendMessage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_SendMessage.Name = "btn_SendMessage";
            this.btn_SendMessage.Size = new System.Drawing.Size(82, 58);
            this.btn_SendMessage.TabIndex = 9;
            this.btn_SendMessage.Text = "Loder_Send_Message";
            this.btn_SendMessage.UseVisualStyleBackColor = true;
            this.btn_SendMessage.Click += new System.EventHandler(this.btn_SendMessage_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 386);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "Receive : ";
            // 
            // Label_LoadDataReceive
            // 
            this.Label_LoadDataReceive.AutoSize = true;
            this.Label_LoadDataReceive.Location = new System.Drawing.Point(105, 386);
            this.Label_LoadDataReceive.Name = "Label_LoadDataReceive";
            this.Label_LoadDataReceive.Size = new System.Drawing.Size(17, 12);
            this.Label_LoadDataReceive.TabIndex = 11;
            this.Label_LoadDataReceive.Text = "^^";
            // 
            // textBox_Loder_Communication
            // 
            this.textBox_Loder_Communication.Location = new System.Drawing.Point(230, 18);
            this.textBox_Loder_Communication.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox_Loder_Communication.Multiline = true;
            this.textBox_Loder_Communication.Name = "textBox_Loder_Communication";
            this.textBox_Loder_Communication.Size = new System.Drawing.Size(302, 272);
            this.textBox_Loder_Communication.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 12);
            this.label2.TabIndex = 13;
            this.label2.Text = "Port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 12);
            this.label3.TabIndex = 14;
            this.label3.Text = "Baudrate";
            // 
            // cmbPort
            // 
            this.cmbPort.FormattingEnabled = true;
            this.cmbPort.Location = new System.Drawing.Point(101, 18);
            this.cmbPort.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbPort.Name = "cmbPort";
            this.cmbPort.Size = new System.Drawing.Size(78, 20);
            this.cmbPort.TabIndex = 15;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 12);
            this.label4.TabIndex = 16;
            this.label4.Text = "Data";
            // 
            // cmbData
            // 
            this.cmbData.FormattingEnabled = true;
            this.cmbData.Items.AddRange(new object[] {
            "8",
            "7",
            "6"});
            this.cmbData.Location = new System.Drawing.Point(101, 62);
            this.cmbData.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbData.Name = "cmbData";
            this.cmbData.Size = new System.Drawing.Size(78, 20);
            this.cmbData.TabIndex = 17;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 108);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 12);
            this.label5.TabIndex = 19;
            this.label5.Text = "Handshake";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 86);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 12);
            this.label6.TabIndex = 18;
            this.label6.Text = "Parity";
            // 
            // cmbHandshake
            // 
            this.cmbHandshake.FormattingEnabled = true;
            this.cmbHandshake.Items.AddRange(new object[] {
            "none",
            "Xon/Xoff",
            "request to send",
            "request to send Xon/Xoff"});
            this.cmbHandshake.Location = new System.Drawing.Point(101, 106);
            this.cmbHandshake.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbHandshake.Name = "cmbHandshake";
            this.cmbHandshake.Size = new System.Drawing.Size(78, 20);
            this.cmbHandshake.TabIndex = 21;
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
            this.cmbParity.Location = new System.Drawing.Point(101, 84);
            this.cmbParity.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbParity.Name = "cmbParity";
            this.cmbParity.Size = new System.Drawing.Size(78, 20);
            this.cmbParity.TabIndex = 20;
            // 
            // btn_Unload_SendMessage
            // 
            this.btn_Unload_SendMessage.Location = new System.Drawing.Point(116, 256);
            this.btn_Unload_SendMessage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_Unload_SendMessage.Name = "btn_Unload_SendMessage";
            this.btn_Unload_SendMessage.Size = new System.Drawing.Size(82, 58);
            this.btn_Unload_SendMessage.TabIndex = 22;
            this.btn_Unload_SendMessage.Text = "UnLoder_Send_Message";
            this.btn_Unload_SendMessage.UseVisualStyleBackColor = true;
            this.btn_Unload_SendMessage.Click += new System.EventHandler(this.btn_Unload_SendMessage_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(110, 138);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(67, 38);
            this.button1.TabIndex = 23;
            this.button1.Text = "Connect2";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // PCBLoaderTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 527);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btn_Unload_SendMessage);
            this.Controls.Add(this.cmbHandshake);
            this.Controls.Add(this.cmbParity);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cmbData);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbPort);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_Loder_Communication);
            this.Controls.Add(this.Label_LoadDataReceive);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_SendMessage);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.cbConnected);
            this.Controls.Add(this.cmbBaud);
            this.Controls.Add(this.numNodeNum);
            this.Controls.Add(this.btn_Disconnect);
            this.Controls.Add(this.btn_Connect);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "PCBLoaderTest";
            this.Text = "PCBLoderTest";
            this.Load += new System.EventHandler(this.PCBLoaderTest_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numNodeNum)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Connect;
        private System.Windows.Forms.Button btn_Disconnect;
        private System.Windows.Forms.ComboBox cmbBaud;
        private System.Windows.Forms.NumericUpDown numNodeNum;
        private System.Windows.Forms.CheckBox cbConnected;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btn_SendMessage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label Label_LoadDataReceive;
        private System.Windows.Forms.TextBox textBox_Loder_Communication;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbData;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbHandshake;
        private System.Windows.Forms.ComboBox cmbParity;
        private System.Windows.Forms.Button btn_Unload_SendMessage;
        private System.Windows.Forms.Button button1;
    }
}