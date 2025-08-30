namespace Radix
{
    partial class ServoPopUp
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
            this.pbAxisStatus = new System.Windows.Forms.PictureBox();
            this.lbJogBack = new System.Windows.Forms.Label();
            this.pbRobotStop = new System.Windows.Forms.PictureBox();
            this.pbRobotHome = new System.Windows.Forms.PictureBox();
            this.pbJogFrontServo = new System.Windows.Forms.PictureBox();
            this.pbJogBackServo = new System.Windows.Forms.PictureBox();
            this.pbRobotMove = new System.Windows.Forms.PictureBox();
            this.pbJogDownServo = new System.Windows.Forms.PictureBox();
            this.numServo = new System.Windows.Forms.NumericUpDown();
            this.lbAxis = new System.Windows.Forms.Label();
            this.lbJogUp = new System.Windows.Forms.Label();
            this.lblServoPos = new System.Windows.Forms.Label();
            this.pbJogUpServo = new System.Windows.Forms.PictureBox();
            this.lbJogFront = new System.Windows.Forms.Label();
            this.lbJogDown = new System.Windows.Forms.Label();
            this.btnSlowSpeed = new System.Windows.Forms.Button();
            this.btnMiddleSpeed = new System.Windows.Forms.Button();
            this.btnHighSpeed = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbAxisSelect = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.test111 = new System.Windows.Forms.Button();
            this.lbLimitPlus = new System.Windows.Forms.Label();
            this.lbLimitMinus = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lbHome = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbAxisStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRobotStop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRobotHome)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbJogFrontServo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbJogBackServo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRobotMove)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbJogDownServo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numServo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbJogUpServo)).BeginInit();
            this.SuspendLayout();
            // 
            // pbAxisStatus
            // 
            this.pbAxisStatus.BackgroundImage = global::Radix.Properties.Resources.circle;
            this.pbAxisStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbAxisStatus.Location = new System.Drawing.Point(383, 272);
            this.pbAxisStatus.Name = "pbAxisStatus";
            this.pbAxisStatus.Size = new System.Drawing.Size(25, 25);
            this.pbAxisStatus.TabIndex = 145;
            this.pbAxisStatus.TabStop = false;
            this.pbAxisStatus.Click += new System.EventHandler(this.pbAxisStatus_Click);
            // 
            // lbJogBack
            // 
            this.lbJogBack.AutoSize = true;
            this.lbJogBack.Location = new System.Drawing.Point(160, 142);
            this.lbJogBack.Name = "lbJogBack";
            this.lbJogBack.Size = new System.Drawing.Size(47, 12);
            this.lbJogBack.TabIndex = 81;
            this.lbJogBack.Text = "BWD(-)";
            // 
            // pbRobotStop
            // 
            this.pbRobotStop.BackColor = System.Drawing.Color.Transparent;
            this.pbRobotStop.BackgroundImage = global::Radix.Properties.Resources.motor_stop;
            this.pbRobotStop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbRobotStop.Location = new System.Drawing.Point(194, 308);
            this.pbRobotStop.Name = "pbRobotStop";
            this.pbRobotStop.Size = new System.Drawing.Size(67, 35);
            this.pbRobotStop.TabIndex = 135;
            this.pbRobotStop.TabStop = false;
            this.pbRobotStop.Click += new System.EventHandler(this.pbRobotStop_Click);
            // 
            // pbRobotHome
            // 
            this.pbRobotHome.BackColor = System.Drawing.Color.Transparent;
            this.pbRobotHome.BackgroundImage = global::Radix.Properties.Resources.motor_home;
            this.pbRobotHome.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbRobotHome.Location = new System.Drawing.Point(278, 308);
            this.pbRobotHome.Name = "pbRobotHome";
            this.pbRobotHome.Size = new System.Drawing.Size(67, 35);
            this.pbRobotHome.TabIndex = 136;
            this.pbRobotHome.TabStop = false;
            this.pbRobotHome.Click += new System.EventHandler(this.pbRobotHome_Click);
            // 
            // pbJogFrontServo
            // 
            this.pbJogFrontServo.BackgroundImage = global::Radix.Properties.Resources.right;
            this.pbJogFrontServo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbJogFrontServo.Location = new System.Drawing.Point(164, 205);
            this.pbJogFrontServo.Name = "pbJogFrontServo";
            this.pbJogFrontServo.Size = new System.Drawing.Size(56, 35);
            this.pbJogFrontServo.TabIndex = 85;
            this.pbJogFrontServo.TabStop = false;
            this.pbJogFrontServo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbJogFrontServo_MouseDown);
            this.pbJogFrontServo.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbJogFrontServo_MouseUp);
            // 
            // pbJogBackServo
            // 
            this.pbJogBackServo.BackgroundImage = global::Radix.Properties.Resources.left;
            this.pbJogBackServo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbJogBackServo.Location = new System.Drawing.Point(164, 158);
            this.pbJogBackServo.Name = "pbJogBackServo";
            this.pbJogBackServo.Size = new System.Drawing.Size(56, 35);
            this.pbJogBackServo.TabIndex = 84;
            this.pbJogBackServo.TabStop = false;
            this.pbJogBackServo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbJogBackServo_MouseDown);
            this.pbJogBackServo.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbJogBackServo_MouseUp);
            // 
            // pbRobotMove
            // 
            this.pbRobotMove.BackColor = System.Drawing.Color.Transparent;
            this.pbRobotMove.BackgroundImage = global::Radix.Properties.Resources.motor_move;
            this.pbRobotMove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbRobotMove.Location = new System.Drawing.Point(110, 308);
            this.pbRobotMove.Name = "pbRobotMove";
            this.pbRobotMove.Size = new System.Drawing.Size(67, 35);
            this.pbRobotMove.TabIndex = 94;
            this.pbRobotMove.TabStop = false;
            this.pbRobotMove.Click += new System.EventHandler(this.pbRobotMove_Click);
            // 
            // pbJogDownServo
            // 
            this.pbJogDownServo.BackgroundImage = global::Radix.Properties.Resources.down;
            this.pbJogDownServo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbJogDownServo.Location = new System.Drawing.Point(259, 203);
            this.pbJogDownServo.Name = "pbJogDownServo";
            this.pbJogDownServo.Size = new System.Drawing.Size(48, 35);
            this.pbJogDownServo.TabIndex = 87;
            this.pbJogDownServo.TabStop = false;
            this.pbJogDownServo.Click += new System.EventHandler(this.pbJogDownServo_Click);
            this.pbJogDownServo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbJogDownServo_MouseDown);
            this.pbJogDownServo.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbJogDownServo_MouseUp);
            // 
            // numServo
            // 
            this.numServo.DecimalPlaces = 2;
            this.numServo.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numServo.Location = new System.Drawing.Point(278, 274);
            this.numServo.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numServo.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.numServo.Name = "numServo";
            this.numServo.Size = new System.Drawing.Size(94, 21);
            this.numServo.TabIndex = 93;
            this.numServo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lbAxis
            // 
            this.lbAxis.Location = new System.Drawing.Point(5, 274);
            this.lbAxis.Name = "lbAxis";
            this.lbAxis.Size = new System.Drawing.Size(148, 18);
            this.lbAxis.TabIndex = 81;
            this.lbAxis.Text = "AXIS_______________";
            this.lbAxis.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbJogUp
            // 
            this.lbJogUp.AutoSize = true;
            this.lbJogUp.Location = new System.Drawing.Point(268, 142);
            this.lbJogUp.Name = "lbJogUp";
            this.lbJogUp.Size = new System.Drawing.Size(37, 12);
            this.lbJogUp.TabIndex = 81;
            this.lbJogUp.Text = "UP(-)";
            // 
            // lblServoPos
            // 
            this.lblServoPos.Location = new System.Drawing.Point(171, 277);
            this.lblServoPos.Name = "lblServoPos";
            this.lblServoPos.Size = new System.Drawing.Size(101, 18);
            this.lblServoPos.TabIndex = 90;
            this.lblServoPos.Text = "0.00";
            // 
            // pbJogUpServo
            // 
            this.pbJogUpServo.BackgroundImage = global::Radix.Properties.Resources.up;
            this.pbJogUpServo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbJogUpServo.Location = new System.Drawing.Point(259, 158);
            this.pbJogUpServo.Name = "pbJogUpServo";
            this.pbJogUpServo.Size = new System.Drawing.Size(48, 35);
            this.pbJogUpServo.TabIndex = 86;
            this.pbJogUpServo.TabStop = false;
            this.pbJogUpServo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbJogUpServo_MouseDown);
            this.pbJogUpServo.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbJogUpServo_MouseUp);
            // 
            // lbJogFront
            // 
            this.lbJogFront.AutoSize = true;
            this.lbJogFront.Location = new System.Drawing.Point(160, 247);
            this.lbJogFront.Name = "lbJogFront";
            this.lbJogFront.Size = new System.Drawing.Size(46, 12);
            this.lbJogFront.TabIndex = 81;
            this.lbJogFront.Text = "FWD(+)";
            // 
            // lbJogDown
            // 
            this.lbJogDown.AutoSize = true;
            this.lbJogDown.Location = new System.Drawing.Point(258, 247);
            this.lbJogDown.Name = "lbJogDown";
            this.lbJogDown.Size = new System.Drawing.Size(57, 12);
            this.lbJogDown.TabIndex = 81;
            this.lbJogDown.Text = "DOWN(+)";
            // 
            // btnSlowSpeed
            // 
            this.btnSlowSpeed.BackColor = System.Drawing.Color.White;
            this.btnSlowSpeed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSlowSpeed.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSlowSpeed.Location = new System.Drawing.Point(106, 82);
            this.btnSlowSpeed.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSlowSpeed.Name = "btnSlowSpeed";
            this.btnSlowSpeed.Size = new System.Drawing.Size(72, 46);
            this.btnSlowSpeed.TabIndex = 149;
            this.btnSlowSpeed.Text = "Slow";
            this.btnSlowSpeed.UseVisualStyleBackColor = false;
            this.btnSlowSpeed.Click += new System.EventHandler(this.btnSlowSpeed_Click);
            // 
            // btnMiddleSpeed
            // 
            this.btnMiddleSpeed.BackColor = System.Drawing.Color.White;
            this.btnMiddleSpeed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMiddleSpeed.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMiddleSpeed.Location = new System.Drawing.Point(188, 82);
            this.btnMiddleSpeed.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnMiddleSpeed.Name = "btnMiddleSpeed";
            this.btnMiddleSpeed.Size = new System.Drawing.Size(72, 46);
            this.btnMiddleSpeed.TabIndex = 148;
            this.btnMiddleSpeed.Text = "Middle";
            this.btnMiddleSpeed.UseVisualStyleBackColor = false;
            this.btnMiddleSpeed.Click += new System.EventHandler(this.btnMiddleSpeed_Click);
            // 
            // btnHighSpeed
            // 
            this.btnHighSpeed.BackColor = System.Drawing.Color.White;
            this.btnHighSpeed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHighSpeed.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHighSpeed.Location = new System.Drawing.Point(273, 82);
            this.btnHighSpeed.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnHighSpeed.Name = "btnHighSpeed";
            this.btnHighSpeed.Size = new System.Drawing.Size(72, 46);
            this.btnHighSpeed.TabIndex = 147;
            this.btnHighSpeed.Text = "High";
            this.btnHighSpeed.UseVisualStyleBackColor = false;
            this.btnHighSpeed.Click += new System.EventHandler(this.btnHighSpeed_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 99);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 148;
            this.label1.Text = "Speed";
            // 
            // cbAxisSelect
            // 
            this.cbAxisSelect.DropDownHeight = 115;
            this.cbAxisSelect.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbAxisSelect.FormattingEnabled = true;
            this.cbAxisSelect.IntegralHeight = false;
            this.cbAxisSelect.Items.AddRange(new object[] {
            "SV00_Input_Tray_Lift",
            "SV01_Output_Tray_Lift"});
            this.cbAxisSelect.Location = new System.Drawing.Point(106, 37);
            this.cbAxisSelect.Name = "cbAxisSelect";
            this.cbAxisSelect.Size = new System.Drawing.Size(183, 21);
            this.cbAxisSelect.TabIndex = 542;
            this.cbAxisSelect.SelectedIndexChanged += new System.EventHandler(this.cbAxisSelect_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(37, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 12);
            this.label2.TabIndex = 543;
            this.label2.Text = "Axis";
            // 
            // test111
            // 
            this.test111.BackColor = System.Drawing.Color.White;
            this.test111.Enabled = false;
            this.test111.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.test111.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.test111.Location = new System.Drawing.Point(32, 174);
            this.test111.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.test111.Name = "test111";
            this.test111.Size = new System.Drawing.Size(72, 46);
            this.test111.TabIndex = 547;
            this.test111.Text = "테스트중";
            this.test111.UseVisualStyleBackColor = false;
            this.test111.Visible = false;
            this.test111.Click += new System.EventHandler(this.test111_Click);
            // 
            // lbLimitPlus
            // 
            this.lbLimitPlus.AutoSize = true;
            this.lbLimitPlus.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lbLimitPlus.Location = new System.Drawing.Point(371, 161);
            this.lbLimitPlus.Name = "lbLimitPlus";
            this.lbLimitPlus.Size = new System.Drawing.Size(48, 12);
            this.lbLimitPlus.TabIndex = 548;
            this.lbLimitPlus.Text = "(+)Limit";
            // 
            // lbLimitMinus
            // 
            this.lbLimitMinus.AutoSize = true;
            this.lbLimitMinus.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lbLimitMinus.Location = new System.Drawing.Point(371, 193);
            this.lbLimitMinus.Name = "lbLimitMinus";
            this.lbLimitMinus.Size = new System.Drawing.Size(48, 12);
            this.lbLimitMinus.TabIndex = 549;
            this.lbLimitMinus.Text = "(-)Limit";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("굴림", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(44, 357);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(384, 11);
            this.label3.TabIndex = 550;
            this.label3.Text = "※ MOVE 사용시 해당축 Home 초기화 필요, 컨베이어는 상대이동처럼 동작";
            // 
            // lbHome
            // 
            this.lbHome.AutoSize = true;
            this.lbHome.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lbHome.Location = new System.Drawing.Point(379, 225);
            this.lbHome.Name = "lbHome";
            this.lbHome.Size = new System.Drawing.Size(38, 12);
            this.lbHome.TabIndex = 551;
            this.lbHome.Text = "Home";
            // 
            // ServoPopUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(465, 378);
            this.Controls.Add(this.lbHome);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbLimitMinus);
            this.Controls.Add(this.lbLimitPlus);
            this.Controls.Add(this.test111);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbAxisSelect);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSlowSpeed);
            this.Controls.Add(this.btnMiddleSpeed);
            this.Controls.Add(this.pbAxisStatus);
            this.Controls.Add(this.btnHighSpeed);
            this.Controls.Add(this.lbJogBack);
            this.Controls.Add(this.lbJogDown);
            this.Controls.Add(this.pbRobotStop);
            this.Controls.Add(this.pbRobotHome);
            this.Controls.Add(this.lbJogFront);
            this.Controls.Add(this.pbJogFrontServo);
            this.Controls.Add(this.pbJogUpServo);
            this.Controls.Add(this.pbJogBackServo);
            this.Controls.Add(this.pbRobotMove);
            this.Controls.Add(this.lblServoPos);
            this.Controls.Add(this.lbJogUp);
            this.Controls.Add(this.pbJogDownServo);
            this.Controls.Add(this.lbAxis);
            this.Controls.Add(this.numServo);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "ServoPopUp";
            this.Text = "ServoPopUp";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ServoPopUp_FormClosed);
            this.Shown += new System.EventHandler(this.ServoPopUp_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pbAxisStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRobotStop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRobotHome)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbJogFrontServo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbJogBackServo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRobotMove)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbJogDownServo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numServo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbJogUpServo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pbAxisStatus;
        private System.Windows.Forms.Label lbJogBack;
        private System.Windows.Forms.PictureBox pbRobotStop;
        private System.Windows.Forms.PictureBox pbRobotHome;
        private System.Windows.Forms.PictureBox pbJogFrontServo;
        private System.Windows.Forms.PictureBox pbJogBackServo;
        private System.Windows.Forms.PictureBox pbRobotMove;
        private System.Windows.Forms.PictureBox pbJogDownServo;
        private System.Windows.Forms.NumericUpDown numServo;
        private System.Windows.Forms.Label lbAxis;
        private System.Windows.Forms.Label lbJogUp;
        private System.Windows.Forms.Label lblServoPos;
        private System.Windows.Forms.PictureBox pbJogUpServo;
        private System.Windows.Forms.Label lbJogFront;
        private System.Windows.Forms.Label lbJogDown;
        private System.Windows.Forms.Button btnSlowSpeed;
        private System.Windows.Forms.Button btnMiddleSpeed;
        private System.Windows.Forms.Button btnHighSpeed;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbAxisSelect;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button test111;
        private System.Windows.Forms.Label lbLimitPlus;
        private System.Windows.Forms.Label lbLimitMinus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbHome;
    }
}