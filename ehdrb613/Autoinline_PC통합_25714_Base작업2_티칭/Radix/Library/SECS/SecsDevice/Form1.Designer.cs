namespace SecsDevice {
    partial class Form1 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.GroupBox groupBox1;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.GroupBox groupBox4;
            System.Windows.Forms.GroupBox groupBox2;
            System.Windows.Forms.GroupBox groupBox5;
            System.Windows.Forms.GroupBox groupBox3;
            System.Windows.Forms.Button btnSendPrimary;
            System.Windows.Forms.Button btnReplySecondary;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.Label label6;
            System.Windows.Forms.Label label7;
            System.Windows.Forms.Label label8;
            System.Windows.Forms.Label label9;
            this.numDeviceId = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.lbStatus = new System.Windows.Forms.Label();
            this.btnDisable = new System.Windows.Forms.Button();
            this.btnEnable = new System.Windows.Forms.Button();
            this.numPort = new System.Windows.Forms.NumericUpDown();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.radioPassiveMode = new System.Windows.Forms.RadioButton();
            this.radioActiveMode = new System.Windows.Forms.RadioButton();
            this.txtRecvSecondary = new System.Windows.Forms.TextBox();
            this.txtSendPrimary = new System.Windows.Forms.TextBox();
            this.txtReplySeconary = new System.Windows.Forms.TextBox();
            this.txtRecvPrimary = new System.Windows.Forms.TextBox();
            this.lstUnreplyMsg = new System.Windows.Forms.ListBox();
            this.recvMessageBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnRUThere = new System.Windows.Forms.Button();
            this.btnEstablishCommunication = new System.Windows.Forms.Button();
            this.btnDisableEvent = new System.Windows.Forms.Button();
            this.btnDeleteAllReport = new System.Windows.Forms.Button();
            this.btnDefineReport = new System.Windows.Forms.Button();
            this.btnLinkEventReport = new System.Windows.Forms.Button();
            this.btnEnableAllEvent = new System.Windows.Forms.Button();
            this.btnTerminalMessage = new System.Windows.Forms.Button();
            this.btnEquipmentStatusRequest = new System.Windows.Forms.Button();
            this.btnDateTime = new System.Windows.Forms.Button();
            this.btnEnableAlarmReport = new System.Windows.Forms.Button();
            this.btnProcessProgramSend = new System.Windows.Forms.Button();
            this.btnCurrentEPPDRequest = new System.Windows.Forms.Button();
            this.btnDisableAlarmReport = new System.Windows.Forms.Button();
            this.btnRemoteCommandStart = new System.Windows.Forms.Button();
            this.btnRemoteCommandCancel = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.tbLotID = new System.Windows.Forms.TextBox();
            this.tbProductID = new System.Windows.Forms.TextBox();
            this.tbRecipeID = new System.Windows.Forms.TextBox();
            this.tbCancelCode = new System.Windows.Forms.TextBox();
            this.tbCancelText = new System.Windows.Forms.TextBox();
            groupBox1 = new System.Windows.Forms.GroupBox();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            groupBox4 = new System.Windows.Forms.GroupBox();
            groupBox2 = new System.Windows.Forms.GroupBox();
            groupBox5 = new System.Windows.Forms.GroupBox();
            groupBox3 = new System.Windows.Forms.GroupBox();
            btnSendPrimary = new System.Windows.Forms.Button();
            btnReplySecondary = new System.Windows.Forms.Button();
            label5 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDeviceId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
            groupBox4.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox5.SuspendLayout();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.recvMessageBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(this.numDeviceId);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(this.label4);
            groupBox1.Controls.Add(this.lbStatus);
            groupBox1.Controls.Add(this.btnDisable);
            groupBox1.Controls.Add(this.btnEnable);
            groupBox1.Controls.Add(this.numPort);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(this.txtAddress);
            groupBox1.Controls.Add(this.radioPassiveMode);
            groupBox1.Controls.Add(this.radioActiveMode);
            groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            groupBox1.Location = new System.Drawing.Point(0, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(1262, 70);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Config";
            // 
            // numDeviceId
            // 
            this.numDeviceId.Location = new System.Drawing.Point(472, 27);
            this.numDeviceId.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numDeviceId.Name = "numDeviceId";
            this.numDeviceId.Size = new System.Drawing.Size(50, 21);
            this.numDeviceId.TabIndex = 10;
            this.numDeviceId.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(407, 31);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(57, 12);
            label3.TabIndex = 9;
            label3.Text = "Device Id";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label4.Location = new System.Drawing.Point(1060, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(179, 37);
            this.label4.TabIndex = 8;
            this.label4.Text = "Equipment";
            // 
            // lbStatus
            // 
            this.lbStatus.AutoSize = true;
            this.lbStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbStatus.Location = new System.Drawing.Point(770, 25);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(114, 37);
            this.lbStatus.TabIndex = 8;
            this.lbStatus.Text = "Status";
            // 
            // btnDisable
            // 
            this.btnDisable.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDisable.Enabled = false;
            this.btnDisable.Location = new System.Drawing.Point(636, 25);
            this.btnDisable.Name = "btnDisable";
            this.btnDisable.Size = new System.Drawing.Size(87, 23);
            this.btnDisable.TabIndex = 7;
            this.btnDisable.Text = "Disable";
            this.btnDisable.UseVisualStyleBackColor = true;
            this.btnDisable.Click += new System.EventHandler(this.btnDisable_Click);
            // 
            // btnEnable
            // 
            this.btnEnable.Location = new System.Drawing.Point(541, 26);
            this.btnEnable.Name = "btnEnable";
            this.btnEnable.Size = new System.Drawing.Size(87, 23);
            this.btnEnable.TabIndex = 6;
            this.btnEnable.Text = "Enable";
            this.btnEnable.UseVisualStyleBackColor = true;
            this.btnEnable.Click += new System.EventHandler(this.btnEnable_Click);
            // 
            // numPort
            // 
            this.numPort.Location = new System.Drawing.Point(327, 27);
            this.numPort.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numPort.Minimum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numPort.Name = "numPort";
            this.numPort.Size = new System.Drawing.Size(61, 21);
            this.numPort.TabIndex = 5;
            this.numPort.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(292, 31);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(27, 12);
            label2.TabIndex = 4;
            label2.Text = "Port";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(93, 31);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(16, 12);
            label1.TabIndex = 3;
            label1.Text = "IP";
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(118, 27);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(166, 21);
            this.txtAddress.TabIndex = 2;
            this.txtAddress.Text = "192.168.10.101";
            // 
            // radioPassiveMode
            // 
            this.radioPassiveMode.AutoSize = true;
            this.radioPassiveMode.Location = new System.Drawing.Point(14, 42);
            this.radioPassiveMode.Name = "radioPassiveMode";
            this.radioPassiveMode.Size = new System.Drawing.Size(68, 16);
            this.radioPassiveMode.TabIndex = 1;
            this.radioPassiveMode.Text = "Passive";
            this.radioPassiveMode.UseVisualStyleBackColor = true;
            // 
            // radioActiveMode
            // 
            this.radioActiveMode.AutoSize = true;
            this.radioActiveMode.Checked = true;
            this.radioActiveMode.Location = new System.Drawing.Point(14, 20);
            this.radioActiveMode.Name = "radioActiveMode";
            this.radioActiveMode.Size = new System.Drawing.Size(57, 16);
            this.radioActiveMode.TabIndex = 0;
            this.radioActiveMode.TabStop = true;
            this.radioActiveMode.Text = "Active";
            this.radioActiveMode.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(this.txtRecvSecondary);
            groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox4.Location = new System.Drawing.Point(0, 173);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new System.Drawing.Size(428, 129);
            groupBox4.TabIndex = 5;
            groupBox4.TabStop = false;
            groupBox4.Text = "Received Secondary Message";
            // 
            // txtRecvSecondary
            // 
            this.txtRecvSecondary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRecvSecondary.Location = new System.Drawing.Point(3, 17);
            this.txtRecvSecondary.Multiline = true;
            this.txtRecvSecondary.Name = "txtRecvSecondary";
            this.txtRecvSecondary.ReadOnly = true;
            this.txtRecvSecondary.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtRecvSecondary.Size = new System.Drawing.Size(422, 109);
            this.txtRecvSecondary.TabIndex = 0;
            this.txtRecvSecondary.WordWrap = false;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(this.txtSendPrimary);
            groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            groupBox2.Location = new System.Drawing.Point(0, 0);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(428, 150);
            groupBox2.TabIndex = 3;
            groupBox2.TabStop = false;
            groupBox2.Text = "Send Primary Message";
            // 
            // txtSendPrimary
            // 
            this.txtSendPrimary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSendPrimary.Location = new System.Drawing.Point(3, 17);
            this.txtSendPrimary.Multiline = true;
            this.txtSendPrimary.Name = "txtSendPrimary";
            this.txtSendPrimary.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtSendPrimary.Size = new System.Drawing.Size(422, 130);
            this.txtSendPrimary.TabIndex = 1;
            this.txtSendPrimary.WordWrap = false;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(this.txtReplySeconary);
            groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox5.Location = new System.Drawing.Point(0, 175);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new System.Drawing.Size(472, 104);
            groupBox5.TabIndex = 2;
            groupBox5.TabStop = false;
            groupBox5.Text = "Reply Secondary Message";
            // 
            // txtReplySeconary
            // 
            this.txtReplySeconary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtReplySeconary.Location = new System.Drawing.Point(3, 17);
            this.txtReplySeconary.Multiline = true;
            this.txtReplySeconary.Name = "txtReplySeconary";
            this.txtReplySeconary.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtReplySeconary.Size = new System.Drawing.Size(466, 84);
            this.txtReplySeconary.TabIndex = 0;
            this.txtReplySeconary.WordWrap = false;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(this.txtRecvPrimary);
            groupBox3.Controls.Add(this.lstUnreplyMsg);
            groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
            groupBox3.Location = new System.Drawing.Point(0, 0);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new System.Drawing.Size(472, 175);
            groupBox3.TabIndex = 0;
            groupBox3.TabStop = false;
            groupBox3.Text = "Received Primary Message";
            // 
            // txtRecvPrimary
            // 
            this.txtRecvPrimary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRecvPrimary.Location = new System.Drawing.Point(241, 17);
            this.txtRecvPrimary.Multiline = true;
            this.txtRecvPrimary.Name = "txtRecvPrimary";
            this.txtRecvPrimary.ReadOnly = true;
            this.txtRecvPrimary.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtRecvPrimary.Size = new System.Drawing.Size(228, 155);
            this.txtRecvPrimary.TabIndex = 1;
            this.txtRecvPrimary.WordWrap = false;
            // 
            // lstUnreplyMsg
            // 
            this.lstUnreplyMsg.DataSource = this.recvMessageBindingSource;
            this.lstUnreplyMsg.DisplayMember = "Msg";
            this.lstUnreplyMsg.Dock = System.Windows.Forms.DockStyle.Left;
            this.lstUnreplyMsg.FormattingEnabled = true;
            this.lstUnreplyMsg.ItemHeight = 12;
            this.lstUnreplyMsg.Location = new System.Drawing.Point(3, 17);
            this.lstUnreplyMsg.Name = "lstUnreplyMsg";
            this.lstUnreplyMsg.Size = new System.Drawing.Size(238, 155);
            this.lstUnreplyMsg.TabIndex = 0;
            this.lstUnreplyMsg.SelectedIndexChanged += new System.EventHandler(this.lstUnreplyMsg_SelectedIndexChanged);
            // 
            // recvMessageBindingSource
            // 
            this.recvMessageBindingSource.DataSource = typeof(SecsDevice.RecvMessage);
            // 
            // btnSendPrimary
            // 
            btnSendPrimary.Dock = System.Windows.Forms.DockStyle.Top;
            btnSendPrimary.Location = new System.Drawing.Point(0, 150);
            btnSendPrimary.Name = "btnSendPrimary";
            btnSendPrimary.Size = new System.Drawing.Size(428, 23);
            btnSendPrimary.TabIndex = 4;
            btnSendPrimary.Text = "Send";
            btnSendPrimary.UseVisualStyleBackColor = true;
            btnSendPrimary.Click += new System.EventHandler(this.btnSendPrimary_Click);
            // 
            // btnReplySecondary
            // 
            btnReplySecondary.Dock = System.Windows.Forms.DockStyle.Bottom;
            btnReplySecondary.Location = new System.Drawing.Point(0, 279);
            btnReplySecondary.Name = "btnReplySecondary";
            btnReplySecondary.Size = new System.Drawing.Size(472, 23);
            btnReplySecondary.TabIndex = 1;
            btnReplySecondary.Text = "Reply";
            btnReplySecondary.UseVisualStyleBackColor = true;
            btnReplySecondary.Click += new System.EventHandler(this.btnReplySecondary_Click);
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(361, 394);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(37, 12);
            label5.TabIndex = 3;
            label5.Text = "Lot ID";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(335, 423);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(63, 12);
            label6.TabIndex = 3;
            label6.Text = "Product ID";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(339, 452);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(59, 12);
            label7.TabIndex = 3;
            label7.Text = "Recipe ID";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(319, 481);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(79, 12);
            label8.TabIndex = 3;
            label8.Text = "Cancel Code";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(324, 508);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(74, 12);
            label9.TabIndex = 3;
            label9.Text = "Cancel Text";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Location = new System.Drawing.Point(0, 70);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(groupBox4);
            this.splitContainer1.Panel1.Controls.Add(btnSendPrimary);
            this.splitContainer1.Panel1.Controls.Add(groupBox2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(groupBox5);
            this.splitContainer1.Panel2.Controls.Add(btnReplySecondary);
            this.splitContainer1.Panel2.Controls.Add(groupBox3);
            this.splitContainer1.Size = new System.Drawing.Size(905, 302);
            this.splitContainer1.SplitterDistance = 428;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 3;
            // 
            // btnRUThere
            // 
            this.btnRUThere.Location = new System.Drawing.Point(12, 417);
            this.btnRUThere.Name = "btnRUThere";
            this.btnRUThere.Size = new System.Drawing.Size(246, 23);
            this.btnRUThere.TabIndex = 4;
            this.btnRUThere.Text = "S1F1 Are You There Request";
            this.btnRUThere.UseVisualStyleBackColor = true;
            this.btnRUThere.Click += new System.EventHandler(this.btnRUThere_Click);
            // 
            // btnEstablishCommunication
            // 
            this.btnEstablishCommunication.Location = new System.Drawing.Point(12, 388);
            this.btnEstablishCommunication.Name = "btnEstablishCommunication";
            this.btnEstablishCommunication.Size = new System.Drawing.Size(246, 23);
            this.btnEstablishCommunication.TabIndex = 4;
            this.btnEstablishCommunication.Text = "S1F13 Establish Communication Request";
            this.btnEstablishCommunication.UseVisualStyleBackColor = true;
            this.btnEstablishCommunication.Click += new System.EventHandler(this.btnEstablishCommunication_Click);
            // 
            // btnDisableEvent
            // 
            this.btnDisableEvent.Location = new System.Drawing.Point(12, 513);
            this.btnDisableEvent.Name = "btnDisableEvent";
            this.btnDisableEvent.Size = new System.Drawing.Size(246, 23);
            this.btnDisableEvent.TabIndex = 4;
            this.btnDisableEvent.Text = "S2F37 Disable All Event";
            this.btnDisableEvent.UseVisualStyleBackColor = true;
            this.btnDisableEvent.Click += new System.EventHandler(this.btnDisableEvent_Click);
            // 
            // btnDeleteAllReport
            // 
            this.btnDeleteAllReport.Location = new System.Drawing.Point(12, 542);
            this.btnDeleteAllReport.Name = "btnDeleteAllReport";
            this.btnDeleteAllReport.Size = new System.Drawing.Size(246, 23);
            this.btnDeleteAllReport.TabIndex = 4;
            this.btnDeleteAllReport.Text = "S2F33 Delete All Report ID";
            this.btnDeleteAllReport.UseVisualStyleBackColor = true;
            this.btnDeleteAllReport.Click += new System.EventHandler(this.btnDeleteAllReport_Click);
            // 
            // btnDefineReport
            // 
            this.btnDefineReport.Location = new System.Drawing.Point(12, 608);
            this.btnDefineReport.Name = "btnDefineReport";
            this.btnDefineReport.Size = new System.Drawing.Size(246, 23);
            this.btnDefineReport.TabIndex = 4;
            this.btnDefineReport.Text = "S2F33 Define Report";
            this.btnDefineReport.UseVisualStyleBackColor = true;
            this.btnDefineReport.Click += new System.EventHandler(this.btnDefineReport_Click);
            // 
            // btnLinkEventReport
            // 
            this.btnLinkEventReport.Location = new System.Drawing.Point(12, 637);
            this.btnLinkEventReport.Name = "btnLinkEventReport";
            this.btnLinkEventReport.Size = new System.Drawing.Size(246, 23);
            this.btnLinkEventReport.TabIndex = 4;
            this.btnLinkEventReport.Text = "S2F35 Link Event Report";
            this.btnLinkEventReport.UseVisualStyleBackColor = true;
            this.btnLinkEventReport.Click += new System.EventHandler(this.btnLinkEventReport_Click);
            // 
            // btnEnableAllEvent
            // 
            this.btnEnableAllEvent.Location = new System.Drawing.Point(12, 666);
            this.btnEnableAllEvent.Name = "btnEnableAllEvent";
            this.btnEnableAllEvent.Size = new System.Drawing.Size(246, 23);
            this.btnEnableAllEvent.TabIndex = 4;
            this.btnEnableAllEvent.Text = "S2F37 Enable All Event";
            this.btnEnableAllEvent.UseVisualStyleBackColor = true;
            this.btnEnableAllEvent.Click += new System.EventHandler(this.btnEnableAllEvent_Click);
            // 
            // btnTerminalMessage
            // 
            this.btnTerminalMessage.Location = new System.Drawing.Point(12, 823);
            this.btnTerminalMessage.Name = "btnTerminalMessage";
            this.btnTerminalMessage.Size = new System.Drawing.Size(246, 23);
            this.btnTerminalMessage.TabIndex = 4;
            this.btnTerminalMessage.Text = "S10F3 Terminal Message";
            this.btnTerminalMessage.UseVisualStyleBackColor = true;
            this.btnTerminalMessage.Click += new System.EventHandler(this.btnTerminalMessage_Click);
            // 
            // btnEquipmentStatusRequest
            // 
            this.btnEquipmentStatusRequest.Location = new System.Drawing.Point(12, 446);
            this.btnEquipmentStatusRequest.Name = "btnEquipmentStatusRequest";
            this.btnEquipmentStatusRequest.Size = new System.Drawing.Size(246, 23);
            this.btnEquipmentStatusRequest.TabIndex = 4;
            this.btnEquipmentStatusRequest.Text = "S1F3 Eq Status Request";
            this.btnEquipmentStatusRequest.UseVisualStyleBackColor = true;
            this.btnEquipmentStatusRequest.Click += new System.EventHandler(this.btnEquipmentStatusRequest_Click);
            // 
            // btnDateTime
            // 
            this.btnDateTime.Location = new System.Drawing.Point(12, 475);
            this.btnDateTime.Name = "btnDateTime";
            this.btnDateTime.Size = new System.Drawing.Size(246, 23);
            this.btnDateTime.TabIndex = 4;
            this.btnDateTime.Text = "S2F31 Date Time Send";
            this.btnDateTime.UseVisualStyleBackColor = true;
            this.btnDateTime.Click += new System.EventHandler(this.btnDateTime_Click);
            // 
            // btnEnableAlarmReport
            // 
            this.btnEnableAlarmReport.Location = new System.Drawing.Point(12, 695);
            this.btnEnableAlarmReport.Name = "btnEnableAlarmReport";
            this.btnEnableAlarmReport.Size = new System.Drawing.Size(246, 23);
            this.btnEnableAlarmReport.TabIndex = 4;
            this.btnEnableAlarmReport.Text = "S5F3 Enable Alarm Report";
            this.btnEnableAlarmReport.UseVisualStyleBackColor = true;
            this.btnEnableAlarmReport.Click += new System.EventHandler(this.btnEnableAlarmReport_Click);
            // 
            // btnProcessProgramSend
            // 
            this.btnProcessProgramSend.Location = new System.Drawing.Point(12, 765);
            this.btnProcessProgramSend.Name = "btnProcessProgramSend";
            this.btnProcessProgramSend.Size = new System.Drawing.Size(246, 23);
            this.btnProcessProgramSend.TabIndex = 4;
            this.btnProcessProgramSend.Text = "S7F3 Process Program Send";
            this.btnProcessProgramSend.UseVisualStyleBackColor = true;
            this.btnProcessProgramSend.Click += new System.EventHandler(this.btnProcessProgramSend_Click);
            // 
            // btnCurrentEPPDRequest
            // 
            this.btnCurrentEPPDRequest.Location = new System.Drawing.Point(12, 794);
            this.btnCurrentEPPDRequest.Name = "btnCurrentEPPDRequest";
            this.btnCurrentEPPDRequest.Size = new System.Drawing.Size(246, 23);
            this.btnCurrentEPPDRequest.TabIndex = 4;
            this.btnCurrentEPPDRequest.Text = "S7F19 Current EPPD Request";
            this.btnCurrentEPPDRequest.UseVisualStyleBackColor = true;
            this.btnCurrentEPPDRequest.Click += new System.EventHandler(this.btnCurrentEPPDRequest_Click);
            // 
            // btnDisableAlarmReport
            // 
            this.btnDisableAlarmReport.Location = new System.Drawing.Point(12, 724);
            this.btnDisableAlarmReport.Name = "btnDisableAlarmReport";
            this.btnDisableAlarmReport.Size = new System.Drawing.Size(246, 23);
            this.btnDisableAlarmReport.TabIndex = 4;
            this.btnDisableAlarmReport.Text = "S5F3 Disable Alarm Report";
            this.btnDisableAlarmReport.UseVisualStyleBackColor = true;
            this.btnDisableAlarmReport.Click += new System.EventHandler(this.btnDisableAlarmReport_Click);
            // 
            // btnRemoteCommandStart
            // 
            this.btnRemoteCommandStart.Location = new System.Drawing.Point(327, 542);
            this.btnRemoteCommandStart.Name = "btnRemoteCommandStart";
            this.btnRemoteCommandStart.Size = new System.Drawing.Size(246, 23);
            this.btnRemoteCommandStart.TabIndex = 4;
            this.btnRemoteCommandStart.Text = "S2F41 Remote Command START";
            this.btnRemoteCommandStart.UseVisualStyleBackColor = true;
            this.btnRemoteCommandStart.Click += new System.EventHandler(this.btnRemoteCommandStart_Click);
            // 
            // btnRemoteCommandCancel
            // 
            this.btnRemoteCommandCancel.Location = new System.Drawing.Point(327, 571);
            this.btnRemoteCommandCancel.Name = "btnRemoteCommandCancel";
            this.btnRemoteCommandCancel.Size = new System.Drawing.Size(246, 23);
            this.btnRemoteCommandCancel.TabIndex = 4;
            this.btnRemoteCommandCancel.Text = "S2F41 Remote Command CANCEL";
            this.btnRemoteCommandCancel.UseVisualStyleBackColor = true;
            this.btnRemoteCommandCancel.Click += new System.EventHandler(this.btnRemoteCommandCancel_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 579);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(246, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "S2F35 UnLink Event Report";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(638, 388);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(246, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Unknown Steam Test";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(638, 417);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(246, 23);
            this.button3.TabIndex = 4;
            this.button3.Text = "Unknown Function Test";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // tbLotID
            // 
            this.tbLotID.Location = new System.Drawing.Point(407, 390);
            this.tbLotID.Name = "tbLotID";
            this.tbLotID.Size = new System.Drawing.Size(166, 21);
            this.tbLotID.TabIndex = 2;
            this.tbLotID.Text = "Lot1";
            // 
            // tbProductID
            // 
            this.tbProductID.Location = new System.Drawing.Point(407, 419);
            this.tbProductID.Name = "tbProductID";
            this.tbProductID.Size = new System.Drawing.Size(166, 21);
            this.tbProductID.TabIndex = 2;
            this.tbProductID.Text = "Product1";
            // 
            // tbRecipeID
            // 
            this.tbRecipeID.Location = new System.Drawing.Point(407, 448);
            this.tbRecipeID.Name = "tbRecipeID";
            this.tbRecipeID.Size = new System.Drawing.Size(166, 21);
            this.tbRecipeID.TabIndex = 2;
            this.tbRecipeID.Text = "Recipe1";
            // 
            // tbCancelCode
            // 
            this.tbCancelCode.Location = new System.Drawing.Point(407, 477);
            this.tbCancelCode.Name = "tbCancelCode";
            this.tbCancelCode.Size = new System.Drawing.Size(166, 21);
            this.tbCancelCode.TabIndex = 2;
            this.tbCancelCode.Text = "99";
            // 
            // tbCancelText
            // 
            this.tbCancelText.Location = new System.Drawing.Point(407, 504);
            this.tbCancelText.Name = "tbCancelText";
            this.tbCancelText.Size = new System.Drawing.Size(166, 21);
            this.tbCancelText.TabIndex = 2;
            this.tbCancelText.Text = "Cancel";
            // 
            // Form1
            // 
            this.AcceptButton = this.btnEnable;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnDisable;
            this.ClientSize = new System.Drawing.Size(1262, 961);
            this.Controls.Add(this.btnRemoteCommandCancel);
            this.Controls.Add(this.btnRemoteCommandStart);
            this.Controls.Add(this.btnEstablishCommunication);
            this.Controls.Add(this.btnDefineReport);
            this.Controls.Add(this.btnDeleteAllReport);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnLinkEventReport);
            this.Controls.Add(this.btnCurrentEPPDRequest);
            this.Controls.Add(label9);
            this.Controls.Add(label8);
            this.Controls.Add(label7);
            this.Controls.Add(label6);
            this.Controls.Add(label5);
            this.Controls.Add(this.btnProcessProgramSend);
            this.Controls.Add(this.tbCancelText);
            this.Controls.Add(this.tbCancelCode);
            this.Controls.Add(this.tbRecipeID);
            this.Controls.Add(this.tbProductID);
            this.Controls.Add(this.tbLotID);
            this.Controls.Add(this.btnDisableAlarmReport);
            this.Controls.Add(this.btnEnableAlarmReport);
            this.Controls.Add(this.btnTerminalMessage);
            this.Controls.Add(this.btnEnableAllEvent);
            this.Controls.Add(this.btnDisableEvent);
            this.Controls.Add(this.btnDateTime);
            this.Controls.Add(this.btnEquipmentStatusRequest);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnRUThere);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(groupBox1);
            this.Name = "Form1";
            this.Text = "SECS Device (HOST)";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDeviceId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.recvMessageBindingSource)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioPassiveMode;
        private System.Windows.Forms.RadioButton radioActiveMode;
        private System.Windows.Forms.Button btnDisable;
        private System.Windows.Forms.Button btnEnable;
        private System.Windows.Forms.NumericUpDown numPort;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txtSendPrimary;
        private System.Windows.Forms.TextBox txtRecvSecondary;
        private System.Windows.Forms.TextBox txtRecvPrimary;
        private System.Windows.Forms.ListBox lstUnreplyMsg;
        private System.Windows.Forms.TextBox txtReplySeconary;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.NumericUpDown numDeviceId;
        private System.Windows.Forms.BindingSource recvMessageBindingSource;
        private System.Windows.Forms.Button btnRUThere;
        private System.Windows.Forms.Button btnEstablishCommunication;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnDisableEvent;
        private System.Windows.Forms.Button btnDeleteAllReport;
        private System.Windows.Forms.Button btnDefineReport;
        private System.Windows.Forms.Button btnLinkEventReport;
        private System.Windows.Forms.Button btnEnableAllEvent;
        private System.Windows.Forms.Button btnTerminalMessage;
        private System.Windows.Forms.Button btnEquipmentStatusRequest;
        private System.Windows.Forms.Button btnDateTime;
        private System.Windows.Forms.Button btnEnableAlarmReport;
        private System.Windows.Forms.Button btnProcessProgramSend;
        private System.Windows.Forms.Button btnCurrentEPPDRequest;
        private System.Windows.Forms.Button btnDisableAlarmReport;
        private System.Windows.Forms.Button btnRemoteCommandStart;
        private System.Windows.Forms.Button btnRemoteCommandCancel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox tbLotID;
        private System.Windows.Forms.TextBox tbProductID;
        private System.Windows.Forms.TextBox tbRecipeID;
        private System.Windows.Forms.TextBox tbCancelCode;
        private System.Windows.Forms.TextBox tbCancelText;
    }
}

