namespace Radix.Popup.Machine
{
    partial class PinBlock
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel5 = new System.Windows.Forms.Panel();
            this.cmbPinArray6 = new System.Windows.Forms.ComboBox();
            this.cmbPinArray5 = new System.Windows.Forms.ComboBox();
            this.cmbPinArray4 = new System.Windows.Forms.ComboBox();
            this.cmbPinArray3 = new System.Windows.Forms.ComboBox();
            this.cmbPinArray2 = new System.Windows.Forms.ComboBox();
            this.cmbPinArray1 = new System.Windows.Forms.ComboBox();
            this.label33 = new System.Windows.Forms.Label();
            this.numPinSite = new System.Windows.Forms.NumericUpDown();
            this.label39 = new System.Windows.Forms.Label();
            this.tbPinLogDirectory = new System.Windows.Forms.TextBox();
            this.label36 = new System.Windows.Forms.Label();
            this.numPinLogTime = new System.Windows.Forms.NumericUpDown();
            this.label23 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.numDefectLimit = new System.Windows.Forms.NumericUpDown();
            this.label37 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label13 = new System.Windows.Forms.Label();
            this.numPinLifeDate = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.tmrCheck = new System.Windows.Forms.Timer(this.components);
            this.cmbPinArray7 = new System.Windows.Forms.ComboBox();
            this.cmbPinArray8 = new System.Windows.Forms.ComboBox();
            this.cmbPinArray9 = new System.Windows.Forms.ComboBox();
            this.cmbPinArray10 = new System.Windows.Forms.ComboBox();
            this.cmbPinArray11 = new System.Windows.Forms.ComboBox();
            this.cmbPinArray12 = new System.Windows.Forms.ComboBox();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPinSite)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPinLogTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDefectLimit)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPinLifeDate)).BeginInit();
            this.SuspendLayout();
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.Transparent;
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel5.Controls.Add(this.cmbPinArray12);
            this.panel5.Controls.Add(this.cmbPinArray11);
            this.panel5.Controls.Add(this.cmbPinArray6);
            this.panel5.Controls.Add(this.cmbPinArray10);
            this.panel5.Controls.Add(this.cmbPinArray5);
            this.panel5.Controls.Add(this.cmbPinArray9);
            this.panel5.Controls.Add(this.cmbPinArray4);
            this.panel5.Controls.Add(this.cmbPinArray8);
            this.panel5.Controls.Add(this.cmbPinArray3);
            this.panel5.Controls.Add(this.cmbPinArray7);
            this.panel5.Controls.Add(this.cmbPinArray2);
            this.panel5.Controls.Add(this.cmbPinArray1);
            this.panel5.Location = new System.Drawing.Point(49, 81);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(969, 217);
            this.panel5.TabIndex = 142;
            // 
            // cmbPinArray6
            // 
            this.cmbPinArray6.FormattingEnabled = true;
            this.cmbPinArray6.Items.AddRange(new object[] {
            "Array1",
            "Array2",
            "Array3",
            "Array4",
            "Array5",
            "Array6",
            "Array7",
            "Array8",
            "Array9",
            "Array10",
            "Array11",
            "Array12"});
            this.cmbPinArray6.Location = new System.Drawing.Point(823, 39);
            this.cmbPinArray6.Name = "cmbPinArray6";
            this.cmbPinArray6.Size = new System.Drawing.Size(112, 37);
            this.cmbPinArray6.TabIndex = 1;
            this.cmbPinArray6.Text = "Array6";
            this.cmbPinArray6.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // cmbPinArray5
            // 
            this.cmbPinArray5.FormattingEnabled = true;
            this.cmbPinArray5.Items.AddRange(new object[] {
            "Array1",
            "Array2",
            "Array3",
            "Array4",
            "Array5",
            "Array6",
            "Array7",
            "Array8",
            "Array9",
            "Array10",
            "Array11",
            "Array12"});
            this.cmbPinArray5.Location = new System.Drawing.Point(666, 39);
            this.cmbPinArray5.Name = "cmbPinArray5";
            this.cmbPinArray5.Size = new System.Drawing.Size(112, 37);
            this.cmbPinArray5.TabIndex = 1;
            this.cmbPinArray5.Text = "Array5";
            this.cmbPinArray5.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // cmbPinArray4
            // 
            this.cmbPinArray4.FormattingEnabled = true;
            this.cmbPinArray4.Items.AddRange(new object[] {
            "Array1",
            "Array2",
            "Array3",
            "Array4",
            "Array5",
            "Array6",
            "Array7",
            "Array8",
            "Array9",
            "Array10",
            "Array11",
            "Array12"});
            this.cmbPinArray4.Location = new System.Drawing.Point(509, 39);
            this.cmbPinArray4.Name = "cmbPinArray4";
            this.cmbPinArray4.Size = new System.Drawing.Size(112, 37);
            this.cmbPinArray4.TabIndex = 1;
            this.cmbPinArray4.Text = "Array4";
            this.cmbPinArray4.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // cmbPinArray3
            // 
            this.cmbPinArray3.FormattingEnabled = true;
            this.cmbPinArray3.Items.AddRange(new object[] {
            "Array1",
            "Array2",
            "Array3",
            "Array4",
            "Array5",
            "Array6",
            "Array7",
            "Array8",
            "Array9",
            "Array10",
            "Array11",
            "Array12"});
            this.cmbPinArray3.Location = new System.Drawing.Point(352, 39);
            this.cmbPinArray3.Name = "cmbPinArray3";
            this.cmbPinArray3.Size = new System.Drawing.Size(112, 37);
            this.cmbPinArray3.TabIndex = 1;
            this.cmbPinArray3.Text = "Array3";
            this.cmbPinArray3.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // cmbPinArray2
            // 
            this.cmbPinArray2.FormattingEnabled = true;
            this.cmbPinArray2.Items.AddRange(new object[] {
            "Array1",
            "Array2",
            "Array3",
            "Array4",
            "Array5",
            "Array6",
            "Array7",
            "Array8",
            "Array9",
            "Array10",
            "Array11",
            "Array12"});
            this.cmbPinArray2.Location = new System.Drawing.Point(195, 39);
            this.cmbPinArray2.Name = "cmbPinArray2";
            this.cmbPinArray2.Size = new System.Drawing.Size(112, 37);
            this.cmbPinArray2.TabIndex = 1;
            this.cmbPinArray2.Text = "Array2";
            this.cmbPinArray2.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // cmbPinArray1
            // 
            this.cmbPinArray1.FormattingEnabled = true;
            this.cmbPinArray1.Items.AddRange(new object[] {
            "Array1",
            "Array2",
            "Array3",
            "Array4",
            "Array5",
            "Array6",
            "Array7",
            "Array8",
            "Array9",
            "Array10",
            "Array11",
            "Array12"});
            this.cmbPinArray1.Location = new System.Drawing.Point(38, 39);
            this.cmbPinArray1.Name = "cmbPinArray1";
            this.cmbPinArray1.Size = new System.Drawing.Size(112, 37);
            this.cmbPinArray1.TabIndex = 1;
            this.cmbPinArray1.Text = "Array1";
            this.cmbPinArray1.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(163, 581);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(50, 29);
            this.label33.TabIndex = 0;
            this.label33.Text = "Site";
            this.label33.Visible = false;
            // 
            // numPinSite
            // 
            this.numPinSite.Location = new System.Drawing.Point(219, 579);
            this.numPinSite.Maximum = new decimal(new int[] {
            21,
            0,
            0,
            0});
            this.numPinSite.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPinSite.Name = "numPinSite";
            this.numPinSite.Size = new System.Drawing.Size(68, 37);
            this.numPinSite.TabIndex = 141;
            this.numPinSite.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numPinSite.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPinSite.Visible = false;
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(47, 47);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(236, 29);
            this.label39.TabIndex = 140;
            this.label39.Text = "PIN Block Array Match";
            // 
            // tbPinLogDirectory
            // 
            this.tbPinLogDirectory.Location = new System.Drawing.Point(169, 101);
            this.tbPinLogDirectory.Name = "tbPinLogDirectory";
            this.tbPinLogDirectory.Size = new System.Drawing.Size(763, 37);
            this.tbPinLogDirectory.TabIndex = 157;
            this.tbPinLogDirectory.Click += new System.EventHandler(this.tbPinLogDirectory_Click);
            this.tbPinLogDirectory.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(6, 105);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(145, 29);
            this.label36.TabIndex = 152;
            this.label36.Text = "Log Directory";
            // 
            // numPinLogTime
            // 
            this.numPinLogTime.Location = new System.Drawing.Point(169, 57);
            this.numPinLogTime.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numPinLogTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPinLogTime.Name = "numPinLogTime";
            this.numPinLogTime.Size = new System.Drawing.Size(68, 37);
            this.numPinLogTime.TabIndex = 150;
            this.numPinLogTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numPinLogTime.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPinLogTime.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(33, 61);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(118, 29);
            this.label23.TabIndex = 153;
            this.label23.Text = "Log Period";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(243, 61);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(52, 29);
            this.label35.TabIndex = 154;
            this.label35.Text = "min";
            // 
            // numDefectLimit
            // 
            this.numDefectLimit.DecimalPlaces = 1;
            this.numDefectLimit.Location = new System.Drawing.Point(169, 15);
            this.numDefectLimit.Name = "numDefectLimit";
            this.numDefectLimit.Size = new System.Drawing.Size(68, 37);
            this.numDefectLimit.TabIndex = 151;
            this.numDefectLimit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numDefectLimit.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numDefectLimit.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(18, 17);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(133, 29);
            this.label37.TabIndex = 155;
            this.label37.Text = "Defect Limit";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(243, 21);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(31, 29);
            this.label38.TabIndex = 156;
            this.label38.Text = "%";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(47, 321);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(180, 29);
            this.label1.TabIndex = 140;
            this.label1.Text = "PIN Block Setting";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label13);
            this.panel2.Controls.Add(this.tbPinLogDirectory);
            this.panel2.Controls.Add(this.numPinLifeDate);
            this.panel2.Controls.Add(this.label14);
            this.panel2.Controls.Add(this.numDefectLimit);
            this.panel2.Controls.Add(this.label36);
            this.panel2.Controls.Add(this.label38);
            this.panel2.Controls.Add(this.numPinLogTime);
            this.panel2.Controls.Add(this.label37);
            this.panel2.Controls.Add(this.label23);
            this.panel2.Controls.Add(this.label35);
            this.panel2.Location = new System.Drawing.Point(49, 355);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(966, 201);
            this.panel2.TabIndex = 142;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(10, 149);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(141, 29);
            this.label13.TabIndex = 145;
            this.label13.Text = "PIN Life Date";
            this.label13.Visible = false;
            // 
            // numPinLifeDate
            // 
            this.numPinLifeDate.Location = new System.Drawing.Point(169, 145);
            this.numPinLifeDate.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numPinLifeDate.Name = "numPinLifeDate";
            this.numPinLifeDate.Size = new System.Drawing.Size(68, 37);
            this.numPinLifeDate.TabIndex = 144;
            this.numPinLifeDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numPinLifeDate.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPinLifeDate.Visible = false;
            this.numPinLifeDate.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(243, 149);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(61, 29);
            this.label14.TabIndex = 146;
            this.label14.Text = "Days";
            this.label14.Visible = false;
            // 
            // tmrCheck
            // 
            this.tmrCheck.Enabled = true;
            this.tmrCheck.Tick += new System.EventHandler(this.tmrCheck_Tick);
            // 
            // cmbPinArray7
            // 
            this.cmbPinArray7.FormattingEnabled = true;
            this.cmbPinArray7.Items.AddRange(new object[] {
            "Array1",
            "Array2",
            "Array3",
            "Array4",
            "Array5",
            "Array6",
            "Array7",
            "Array8",
            "Array9",
            "Array10",
            "Array11",
            "Array12"});
            this.cmbPinArray7.Location = new System.Drawing.Point(35, 103);
            this.cmbPinArray7.Name = "cmbPinArray7";
            this.cmbPinArray7.Size = new System.Drawing.Size(112, 37);
            this.cmbPinArray7.TabIndex = 1;
            this.cmbPinArray7.Text = "Array7";
            this.cmbPinArray7.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // cmbPinArray8
            // 
            this.cmbPinArray8.FormattingEnabled = true;
            this.cmbPinArray8.Items.AddRange(new object[] {
            "Array1",
            "Array2",
            "Array3",
            "Array4",
            "Array5",
            "Array6",
            "Array7",
            "Array8",
            "Array9",
            "Array10",
            "Array11",
            "Array12"});
            this.cmbPinArray8.Location = new System.Drawing.Point(192, 103);
            this.cmbPinArray8.Name = "cmbPinArray8";
            this.cmbPinArray8.Size = new System.Drawing.Size(112, 37);
            this.cmbPinArray8.TabIndex = 1;
            this.cmbPinArray8.Text = "Array8";
            this.cmbPinArray8.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // cmbPinArray9
            // 
            this.cmbPinArray9.FormattingEnabled = true;
            this.cmbPinArray9.Items.AddRange(new object[] {
            "Array1",
            "Array2",
            "Array3",
            "Array4",
            "Array5",
            "Array6",
            "Array7",
            "Array8",
            "Array9",
            "Array10",
            "Array11",
            "Array12"});
            this.cmbPinArray9.Location = new System.Drawing.Point(349, 103);
            this.cmbPinArray9.Name = "cmbPinArray9";
            this.cmbPinArray9.Size = new System.Drawing.Size(112, 37);
            this.cmbPinArray9.TabIndex = 1;
            this.cmbPinArray9.Text = "Array9";
            this.cmbPinArray9.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // cmbPinArray10
            // 
            this.cmbPinArray10.FormattingEnabled = true;
            this.cmbPinArray10.Items.AddRange(new object[] {
            "Array1",
            "Array2",
            "Array3",
            "Array4",
            "Array5",
            "Array6",
            "Array7",
            "Array8",
            "Array9",
            "Array10",
            "Array11",
            "Array12"});
            this.cmbPinArray10.Location = new System.Drawing.Point(506, 103);
            this.cmbPinArray10.Name = "cmbPinArray10";
            this.cmbPinArray10.Size = new System.Drawing.Size(112, 37);
            this.cmbPinArray10.TabIndex = 1;
            this.cmbPinArray10.Text = "Array10";
            this.cmbPinArray10.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // cmbPinArray11
            // 
            this.cmbPinArray11.FormattingEnabled = true;
            this.cmbPinArray11.Items.AddRange(new object[] {
            "Array1",
            "Array2",
            "Array3",
            "Array4",
            "Array5",
            "Array6",
            "Array7",
            "Array8",
            "Array9",
            "Array10",
            "Array11",
            "Array12"});
            this.cmbPinArray11.Location = new System.Drawing.Point(663, 103);
            this.cmbPinArray11.Name = "cmbPinArray11";
            this.cmbPinArray11.Size = new System.Drawing.Size(112, 37);
            this.cmbPinArray11.TabIndex = 1;
            this.cmbPinArray11.Text = "Array11";
            this.cmbPinArray11.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // cmbPinArray12
            // 
            this.cmbPinArray12.FormattingEnabled = true;
            this.cmbPinArray12.Items.AddRange(new object[] {
            "Array1",
            "Array2",
            "Array3",
            "Array4",
            "Array5",
            "Array6",
            "Array7",
            "Array8",
            "Array9",
            "Array10",
            "Array11",
            "Array12"});
            this.cmbPinArray12.Location = new System.Drawing.Point(820, 103);
            this.cmbPinArray12.Name = "cmbPinArray12";
            this.cmbPinArray12.Size = new System.Drawing.Size(112, 37);
            this.cmbPinArray12.TabIndex = 1;
            this.cmbPinArray12.Text = "Array12";
            this.cmbPinArray12.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // PinBlock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(185)))), ((int)(((byte)(229)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1730, 890);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label39);
            this.Controls.Add(this.numPinSite);
            this.Controls.Add(this.label33);
            this.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "PinBlock";
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numPinSite)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPinLogTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDefectLimit)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPinLifeDate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.ComboBox cmbPinArray6;
        private System.Windows.Forms.ComboBox cmbPinArray5;
        private System.Windows.Forms.ComboBox cmbPinArray4;
        private System.Windows.Forms.ComboBox cmbPinArray3;
        private System.Windows.Forms.ComboBox cmbPinArray2;
        private System.Windows.Forms.ComboBox cmbPinArray1;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.NumericUpDown numPinSite;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.TextBox tbPinLogDirectory;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.NumericUpDown numPinLogTime;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.NumericUpDown numDefectLimit;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown numPinLifeDate;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Timer tmrCheck;
        private System.Windows.Forms.ComboBox cmbPinArray12;
        private System.Windows.Forms.ComboBox cmbPinArray11;
        private System.Windows.Forms.ComboBox cmbPinArray10;
        private System.Windows.Forms.ComboBox cmbPinArray9;
        private System.Windows.Forms.ComboBox cmbPinArray8;
        private System.Windows.Forms.ComboBox cmbPinArray7;
    }
}
