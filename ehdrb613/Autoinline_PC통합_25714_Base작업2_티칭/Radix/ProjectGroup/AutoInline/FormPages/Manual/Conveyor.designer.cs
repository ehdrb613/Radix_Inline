namespace Radix.Popup.Manual
{
    partial class Conveyor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Conveyor));
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnPosNG = new System.Windows.Forms.Button();
            this.btnPosOutConveyor = new System.Windows.Forms.Button();
            this.btnPosLift2Up = new System.Windows.Forms.Button();
            this.btnPosRearPassLine = new System.Windows.Forms.Button();
            this.btnPosLift1Up = new System.Windows.Forms.Button();
            this.btnPosFrontPassLine = new System.Windows.Forms.Button();
            this.btnPosInConveyor = new System.Windows.Forms.Button();
            this.btnStopConveyor = new System.Windows.Forms.Button();
            this.btnNGForward = new System.Windows.Forms.Button();
            this.btnNGStop = new System.Windows.Forms.Button();
            this.btnNGReward = new System.Windows.Forms.Button();
            this.btnRunConveyorCCW = new System.Windows.Forms.Button();
            this.btnRunConveyorCW = new System.Windows.Forms.Button();
            this.btnStopperDown = new System.Windows.Forms.Button();
            this.btnStopperUp = new System.Windows.Forms.Button();
            this.pnJog = new System.Windows.Forms.Panel();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.numSpeed = new System.Windows.Forms.NumericUpDown();
            this.btnSpeed = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.numPitch = new System.Windows.Forms.NumericUpDown();
            this.pbStop = new System.Windows.Forms.Button();
            this.btnPitch = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.lblTeachingWidth = new System.Windows.Forms.Label();
            this.pbAxisStatus = new System.Windows.Forms.PictureBox();
            this.pbJogWide = new System.Windows.Forms.Button();
            this.pbJogNarrow = new System.Windows.Forms.Button();
            this.pbHome = new System.Windows.Forms.Button();
            this.pbMove = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblCurrPos = new System.Windows.Forms.Label();
            this.numMovePos = new System.Windows.Forms.NumericUpDown();
            this.btnTeachingWidth = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnPosRearNGLine = new System.Windows.Forms.Button();
            this.btnPosInShuttle = new System.Windows.Forms.Button();
            this.btnPosOutShuttle = new System.Windows.Forms.Button();
            this.panel4.SuspendLayout();
            this.pnJog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPitch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAxisStatus)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMovePos)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.Transparent;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.btnPosOutShuttle);
            this.panel4.Controls.Add(this.btnPosInShuttle);
            this.panel4.Controls.Add(this.btnPosRearNGLine);
            this.panel4.Controls.Add(this.btnPosNG);
            this.panel4.Controls.Add(this.btnPosOutConveyor);
            this.panel4.Controls.Add(this.btnPosLift2Up);
            this.panel4.Controls.Add(this.btnPosRearPassLine);
            this.panel4.Controls.Add(this.btnPosLift1Up);
            this.panel4.Controls.Add(this.btnPosFrontPassLine);
            this.panel4.Controls.Add(this.btnPosInConveyor);
            this.panel4.Location = new System.Drawing.Point(21, 64);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1697, 255);
            this.panel4.TabIndex = 19;
            // 
            // btnPosNG
            // 
            this.btnPosNG.BackColor = System.Drawing.Color.White;
            this.btnPosNG.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPosNG.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.btnPosNG.Location = new System.Drawing.Point(1439, 128);
            this.btnPosNG.Name = "btnPosNG";
            this.btnPosNG.Size = new System.Drawing.Size(236, 84);
            this.btnPosNG.TabIndex = 136;
            this.btnPosNG.Text = "NG Buffer";
            this.btnPosNG.UseVisualStyleBackColor = false;
            this.btnPosNG.Click += new System.EventHandler(this.btnPosNG_Click);
            // 
            // btnPosOutConveyor
            // 
            this.btnPosOutConveyor.BackColor = System.Drawing.Color.White;
            this.btnPosOutConveyor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPosOutConveyor.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.btnPosOutConveyor.Location = new System.Drawing.Point(1439, 25);
            this.btnPosOutConveyor.Name = "btnPosOutConveyor";
            this.btnPosOutConveyor.Size = new System.Drawing.Size(236, 84);
            this.btnPosOutConveyor.TabIndex = 136;
            this.btnPosOutConveyor.Text = "Output Conveyor";
            this.btnPosOutConveyor.UseVisualStyleBackColor = false;
            this.btnPosOutConveyor.Click += new System.EventHandler(this.btnPosOutputConveyor_Click);
            // 
            // btnPosLift2Up
            // 
            this.btnPosLift2Up.BackColor = System.Drawing.Color.White;
            this.btnPosLift2Up.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPosLift2Up.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.btnPosLift2Up.Location = new System.Drawing.Point(1155, 25);
            this.btnPosLift2Up.Name = "btnPosLift2Up";
            this.btnPosLift2Up.Size = new System.Drawing.Size(236, 84);
            this.btnPosLift2Up.TabIndex = 136;
            this.btnPosLift2Up.Text = "Lift 2 UP";
            this.btnPosLift2Up.UseVisualStyleBackColor = false;
            this.btnPosLift2Up.Click += new System.EventHandler(this.btnPosLift2Up_Click);
            // 
            // btnPosRearPassLine
            // 
            this.btnPosRearPassLine.BackColor = System.Drawing.Color.White;
            this.btnPosRearPassLine.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPosRearPassLine.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.btnPosRearPassLine.Location = new System.Drawing.Point(871, 25);
            this.btnPosRearPassLine.Name = "btnPosRearPassLine";
            this.btnPosRearPassLine.Size = new System.Drawing.Size(236, 84);
            this.btnPosRearPassLine.TabIndex = 136;
            this.btnPosRearPassLine.Text = "RearPassLine";
            this.btnPosRearPassLine.UseVisualStyleBackColor = false;
            this.btnPosRearPassLine.Click += new System.EventHandler(this.btnPosPassLine2_Click);
            // 
            // btnPosLift1Up
            // 
            this.btnPosLift1Up.BackColor = System.Drawing.Color.White;
            this.btnPosLift1Up.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPosLift1Up.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.btnPosLift1Up.Location = new System.Drawing.Point(288, 136);
            this.btnPosLift1Up.Name = "btnPosLift1Up";
            this.btnPosLift1Up.Size = new System.Drawing.Size(236, 84);
            this.btnPosLift1Up.TabIndex = 136;
            this.btnPosLift1Up.Text = "Lift 1 UP";
            this.btnPosLift1Up.UseVisualStyleBackColor = false;
            this.btnPosLift1Up.Click += new System.EventHandler(this.btnPosLift1Up_Click);
            // 
            // btnPosFrontPassLine
            // 
            this.btnPosFrontPassLine.BackColor = System.Drawing.Color.White;
            this.btnPosFrontPassLine.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPosFrontPassLine.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.btnPosFrontPassLine.Location = new System.Drawing.Point(288, 24);
            this.btnPosFrontPassLine.Name = "btnPosFrontPassLine";
            this.btnPosFrontPassLine.Size = new System.Drawing.Size(236, 84);
            this.btnPosFrontPassLine.TabIndex = 136;
            this.btnPosFrontPassLine.Text = "PassLine 1";
            this.btnPosFrontPassLine.UseVisualStyleBackColor = false;
            this.btnPosFrontPassLine.Click += new System.EventHandler(this.btnPosPassLine1_Click);
            // 
            // btnPosInConveyor
            // 
            this.btnPosInConveyor.BackColor = System.Drawing.Color.Lime;
            this.btnPosInConveyor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPosInConveyor.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.btnPosInConveyor.Location = new System.Drawing.Point(19, 25);
            this.btnPosInConveyor.Name = "btnPosInConveyor";
            this.btnPosInConveyor.Size = new System.Drawing.Size(236, 84);
            this.btnPosInConveyor.TabIndex = 136;
            this.btnPosInConveyor.Text = "In Conveyor";
            this.btnPosInConveyor.UseVisualStyleBackColor = false;
            this.btnPosInConveyor.Click += new System.EventHandler(this.btnPosInputConveyor_Click);
            // 
            // btnStopConveyor
            // 
            this.btnStopConveyor.BackColor = System.Drawing.Color.White;
            this.btnStopConveyor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStopConveyor.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.btnStopConveyor.Location = new System.Drawing.Point(318, 251);
            this.btnStopConveyor.Name = "btnStopConveyor";
            this.btnStopConveyor.Size = new System.Drawing.Size(236, 84);
            this.btnStopConveyor.TabIndex = 18;
            this.btnStopConveyor.Text = "Conv. Stop";
            this.btnStopConveyor.UseVisualStyleBackColor = false;
            this.btnStopConveyor.Click += new System.EventHandler(this.btnStopConveyor_Click);
            // 
            // btnNGForward
            // 
            this.btnNGForward.BackColor = System.Drawing.Color.White;
            this.btnNGForward.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNGForward.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.btnNGForward.Location = new System.Drawing.Point(602, 251);
            this.btnNGForward.Name = "btnNGForward";
            this.btnNGForward.Size = new System.Drawing.Size(236, 84);
            this.btnNGForward.TabIndex = 18;
            this.btnNGForward.Text = "NG Forward";
            this.btnNGForward.UseVisualStyleBackColor = false;
            this.btnNGForward.Visible = false;
            this.btnNGForward.Click += new System.EventHandler(this.btnNGForward_Click);
            // 
            // btnNGStop
            // 
            this.btnNGStop.BackColor = System.Drawing.Color.White;
            this.btnNGStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNGStop.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.btnNGStop.Location = new System.Drawing.Point(602, 140);
            this.btnNGStop.Name = "btnNGStop";
            this.btnNGStop.Size = new System.Drawing.Size(236, 84);
            this.btnNGStop.TabIndex = 18;
            this.btnNGStop.Text = "NG  Move Stop";
            this.btnNGStop.UseVisualStyleBackColor = false;
            this.btnNGStop.Visible = false;
            this.btnNGStop.Click += new System.EventHandler(this.btnNGStop_Click);
            // 
            // btnNGReward
            // 
            this.btnNGReward.BackColor = System.Drawing.Color.White;
            this.btnNGReward.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNGReward.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.btnNGReward.Location = new System.Drawing.Point(602, 30);
            this.btnNGReward.Name = "btnNGReward";
            this.btnNGReward.Size = new System.Drawing.Size(236, 84);
            this.btnNGReward.TabIndex = 18;
            this.btnNGReward.Text = "NG Backward";
            this.btnNGReward.UseVisualStyleBackColor = false;
            this.btnNGReward.Visible = false;
            this.btnNGReward.Click += new System.EventHandler(this.btnNGReward_Click);
            // 
            // btnRunConveyorCCW
            // 
            this.btnRunConveyorCCW.BackColor = System.Drawing.Color.White;
            this.btnRunConveyorCCW.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRunConveyorCCW.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.btnRunConveyorCCW.Location = new System.Drawing.Point(318, 30);
            this.btnRunConveyorCCW.Name = "btnRunConveyorCCW";
            this.btnRunConveyorCCW.Size = new System.Drawing.Size(236, 84);
            this.btnRunConveyorCCW.TabIndex = 18;
            this.btnRunConveyorCCW.Text = "Conv. CCW";
            this.btnRunConveyorCCW.UseVisualStyleBackColor = false;
            this.btnRunConveyorCCW.Click += new System.EventHandler(this.btnRunConveyor_Click);
            // 
            // btnRunConveyorCW
            // 
            this.btnRunConveyorCW.BackColor = System.Drawing.Color.White;
            this.btnRunConveyorCW.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRunConveyorCW.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.btnRunConveyorCW.Location = new System.Drawing.Point(318, 140);
            this.btnRunConveyorCW.Name = "btnRunConveyorCW";
            this.btnRunConveyorCW.Size = new System.Drawing.Size(236, 84);
            this.btnRunConveyorCW.TabIndex = 18;
            this.btnRunConveyorCW.Text = "Conv. CW";
            this.btnRunConveyorCW.UseVisualStyleBackColor = false;
            this.btnRunConveyorCW.Click += new System.EventHandler(this.btnRunConveyor_Click);
            // 
            // btnStopperDown
            // 
            this.btnStopperDown.BackColor = System.Drawing.Color.White;
            this.btnStopperDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStopperDown.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.btnStopperDown.Location = new System.Drawing.Point(34, 140);
            this.btnStopperDown.Name = "btnStopperDown";
            this.btnStopperDown.Size = new System.Drawing.Size(236, 84);
            this.btnStopperDown.TabIndex = 18;
            this.btnStopperDown.Text = "Stopper Down";
            this.btnStopperDown.UseVisualStyleBackColor = false;
            this.btnStopperDown.Click += new System.EventHandler(this.btnStopperDownConveyor_Click);
            // 
            // btnStopperUp
            // 
            this.btnStopperUp.BackColor = System.Drawing.Color.White;
            this.btnStopperUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStopperUp.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.btnStopperUp.Location = new System.Drawing.Point(34, 30);
            this.btnStopperUp.Name = "btnStopperUp";
            this.btnStopperUp.Size = new System.Drawing.Size(236, 84);
            this.btnStopperUp.TabIndex = 18;
            this.btnStopperUp.Text = "Stopper Up";
            this.btnStopperUp.UseVisualStyleBackColor = false;
            this.btnStopperUp.Click += new System.EventHandler(this.btnStopperUpConveyor_Click);
            // 
            // pnJog
            // 
            this.pnJog.BackColor = System.Drawing.Color.Transparent;
            this.pnJog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnJog.Controls.Add(this.progressBar2);
            this.pnJog.Controls.Add(this.numSpeed);
            this.pnJog.Controls.Add(this.btnSpeed);
            this.pnJog.Controls.Add(this.label11);
            this.pnJog.Controls.Add(this.progressBar1);
            this.pnJog.Controls.Add(this.numPitch);
            this.pnJog.Controls.Add(this.pbStop);
            this.pnJog.Controls.Add(this.btnPitch);
            this.pnJog.Controls.Add(this.label10);
            this.pnJog.Controls.Add(this.lblTeachingWidth);
            this.pnJog.Controls.Add(this.pbAxisStatus);
            this.pnJog.Controls.Add(this.pbJogWide);
            this.pnJog.Controls.Add(this.pbJogNarrow);
            this.pnJog.Controls.Add(this.pbHome);
            this.pnJog.Controls.Add(this.pbMove);
            this.pnJog.Controls.Add(this.panel2);
            this.pnJog.Controls.Add(this.btnTeachingWidth);
            this.pnJog.Location = new System.Drawing.Point(21, 375);
            this.pnJog.Name = "pnJog";
            this.pnJog.Size = new System.Drawing.Size(795, 365);
            this.pnJog.TabIndex = 19;
            this.pnJog.Visible = false;
            // 
            // progressBar2
            // 
            this.progressBar2.Location = new System.Drawing.Point(540, 98);
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(161, 27);
            this.progressBar2.TabIndex = 179;
            this.progressBar2.Visible = false;
            // 
            // numSpeed
            // 
            this.numSpeed.DecimalPlaces = 2;
            this.numSpeed.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.numSpeed.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numSpeed.Location = new System.Drawing.Point(540, 59);
            this.numSpeed.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numSpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numSpeed.Name = "numSpeed";
            this.numSpeed.Size = new System.Drawing.Size(95, 37);
            this.numSpeed.TabIndex = 176;
            this.numSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numSpeed.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // btnSpeed
            // 
            this.btnSpeed.BackColor = System.Drawing.Color.Lime;
            this.btnSpeed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSpeed.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.btnSpeed.Location = new System.Drawing.Point(395, 51);
            this.btnSpeed.Name = "btnSpeed";
            this.btnSpeed.Size = new System.Drawing.Size(127, 69);
            this.btnSpeed.TabIndex = 177;
            this.btnSpeed.Text = "Speed";
            this.btnSpeed.UseVisualStyleBackColor = false;
            this.btnSpeed.Click += new System.EventHandler(this.btnSpeed_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.Transparent;
            this.label11.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.label11.Location = new System.Drawing.Point(636, 60);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(105, 29);
            this.label11.TabIndex = 178;
            this.label11.Text = "mm / sec";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(180, 101);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(161, 27);
            this.progressBar1.TabIndex = 175;
            this.progressBar1.Visible = false;
            // 
            // numPitch
            // 
            this.numPitch.DecimalPlaces = 2;
            this.numPitch.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.numPitch.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numPitch.Location = new System.Drawing.Point(180, 65);
            this.numPitch.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numPitch.Name = "numPitch";
            this.numPitch.Size = new System.Drawing.Size(95, 37);
            this.numPitch.TabIndex = 172;
            this.numPitch.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numPitch.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // pbStop
            // 
            this.pbStop.BackColor = System.Drawing.Color.Tomato;
            this.pbStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pbStop.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.pbStop.Location = new System.Drawing.Point(526, 251);
            this.pbStop.Name = "pbStop";
            this.pbStop.Size = new System.Drawing.Size(109, 69);
            this.pbStop.TabIndex = 148;
            this.pbStop.Text = "STOP";
            this.pbStop.UseVisualStyleBackColor = false;
            this.pbStop.Click += new System.EventHandler(this.pbStopConveyor_Click);
            // 
            // btnPitch
            // 
            this.btnPitch.BackColor = System.Drawing.Color.White;
            this.btnPitch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPitch.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.btnPitch.Location = new System.Drawing.Point(34, 51);
            this.btnPitch.Name = "btnPitch";
            this.btnPitch.Size = new System.Drawing.Size(127, 69);
            this.btnPitch.TabIndex = 173;
            this.btnPitch.Text = "Pitch";
            this.btnPitch.UseVisualStyleBackColor = false;
            this.btnPitch.Click += new System.EventHandler(this.btnPitch_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.Transparent;
            this.label10.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.label10.Location = new System.Drawing.Point(276, 66);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 29);
            this.label10.TabIndex = 174;
            this.label10.Text = "mm";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTeachingWidth
            // 
            this.lblTeachingWidth.AutoSize = true;
            this.lblTeachingWidth.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.lblTeachingWidth.Location = new System.Drawing.Point(244, 201);
            this.lblTeachingWidth.Name = "lblTeachingWidth";
            this.lblTeachingWidth.Size = new System.Drawing.Size(55, 29);
            this.lblTeachingWidth.TabIndex = 88;
            this.lblTeachingWidth.Text = "0.00";
            // 
            // pbAxisStatus
            // 
            this.pbAxisStatus.BackgroundImage = global::Radix.Properties.Resources.circle;
            this.pbAxisStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbAxisStatus.Location = new System.Drawing.Point(702, 186);
            this.pbAxisStatus.Name = "pbAxisStatus";
            this.pbAxisStatus.Size = new System.Drawing.Size(24, 25);
            this.pbAxisStatus.TabIndex = 150;
            this.pbAxisStatus.TabStop = false;
            // 
            // pbJogWide
            // 
            this.pbJogWide.BackColor = System.Drawing.Color.Transparent;
            this.pbJogWide.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pbJogWide.BackgroundImage")));
            this.pbJogWide.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbJogWide.FlatAppearance.BorderSize = 0;
            this.pbJogWide.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pbJogWide.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Bold);
            this.pbJogWide.ForeColor = System.Drawing.Color.Transparent;
            this.pbJogWide.Location = new System.Drawing.Point(33, 188);
            this.pbJogWide.Name = "pbJogWide";
            this.pbJogWide.Size = new System.Drawing.Size(128, 62);
            this.pbJogWide.TabIndex = 145;
            this.pbJogWide.Text = "Wide";
            this.pbJogWide.UseVisualStyleBackColor = false;
            this.pbJogWide.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbJogWideConveyor_MouseDown);
            this.pbJogWide.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbJogWideConveyor_MouseUp);
            // 
            // pbJogNarrow
            // 
            this.pbJogNarrow.BackColor = System.Drawing.Color.Transparent;
            this.pbJogNarrow.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pbJogNarrow.BackgroundImage")));
            this.pbJogNarrow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbJogNarrow.FlatAppearance.BorderSize = 0;
            this.pbJogNarrow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pbJogNarrow.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Bold);
            this.pbJogNarrow.ForeColor = System.Drawing.Color.Transparent;
            this.pbJogNarrow.Location = new System.Drawing.Point(33, 266);
            this.pbJogNarrow.Name = "pbJogNarrow";
            this.pbJogNarrow.Size = new System.Drawing.Size(128, 62);
            this.pbJogNarrow.TabIndex = 144;
            this.pbJogNarrow.Text = "Narrow";
            this.pbJogNarrow.UseVisualStyleBackColor = false;
            this.pbJogNarrow.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbJogNarrowConveyor_MouseDown);
            this.pbJogNarrow.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbJogNarrowConveyor_MouseUp);
            // 
            // pbHome
            // 
            this.pbHome.BackColor = System.Drawing.Color.DarkCyan;
            this.pbHome.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pbHome.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.pbHome.Location = new System.Drawing.Point(652, 251);
            this.pbHome.Name = "pbHome";
            this.pbHome.Size = new System.Drawing.Size(109, 69);
            this.pbHome.TabIndex = 147;
            this.pbHome.Text = "HOME";
            this.pbHome.UseVisualStyleBackColor = false;
            this.pbHome.Click += new System.EventHandler(this.pbHomeConveyor_Click);
            // 
            // pbMove
            // 
            this.pbMove.BackColor = System.Drawing.Color.LightSkyBlue;
            this.pbMove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pbMove.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.pbMove.Location = new System.Drawing.Point(395, 251);
            this.pbMove.Name = "pbMove";
            this.pbMove.Size = new System.Drawing.Size(109, 69);
            this.pbMove.TabIndex = 146;
            this.pbMove.Text = "MOVE";
            this.pbMove.UseVisualStyleBackColor = false;
            this.pbMove.Click += new System.EventHandler(this.pbMoveConveyor_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.lblCurrPos);
            this.panel2.Controls.Add(this.numMovePos);
            this.panel2.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.panel2.Location = new System.Drawing.Point(439, 182);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(259, 42);
            this.panel2.TabIndex = 149;
            // 
            // lblCurrPos
            // 
            this.lblCurrPos.AutoSize = true;
            this.lblCurrPos.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.lblCurrPos.Location = new System.Drawing.Point(28, 4);
            this.lblCurrPos.Name = "lblCurrPos";
            this.lblCurrPos.Size = new System.Drawing.Size(55, 29);
            this.lblCurrPos.TabIndex = 88;
            this.lblCurrPos.Text = "0.00";
            // 
            // numMovePos
            // 
            this.numMovePos.DecimalPlaces = 2;
            this.numMovePos.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.numMovePos.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numMovePos.Location = new System.Drawing.Point(133, 2);
            this.numMovePos.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numMovePos.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.numMovePos.Name = "numMovePos";
            this.numMovePos.Size = new System.Drawing.Size(124, 37);
            this.numMovePos.TabIndex = 106;
            this.numMovePos.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnTeachingWidth
            // 
            this.btnTeachingWidth.BackColor = System.Drawing.Color.White;
            this.btnTeachingWidth.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTeachingWidth.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.btnTeachingWidth.Location = new System.Drawing.Point(205, 251);
            this.btnTeachingWidth.Name = "btnTeachingWidth";
            this.btnTeachingWidth.Size = new System.Drawing.Size(145, 69);
            this.btnTeachingWidth.TabIndex = 18;
            this.btnTeachingWidth.Text = "Teaching Width";
            this.btnTeachingWidth.UseVisualStyleBackColor = false;
            this.btnTeachingWidth.Click += new System.EventHandler(this.pbNarrowPosConveyor_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(19, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(204, 29);
            this.label1.TabIndex = 110;
            this.label1.Text = "Conveyor Selection";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnStopperUp);
            this.panel1.Controls.Add(this.btnNGForward);
            this.panel1.Controls.Add(this.btnStopConveyor);
            this.panel1.Controls.Add(this.btnNGStop);
            this.panel1.Controls.Add(this.btnRunConveyorCCW);
            this.panel1.Controls.Add(this.btnNGReward);
            this.panel1.Controls.Add(this.btnRunConveyorCW);
            this.panel1.Controls.Add(this.btnStopperDown);
            this.panel1.Location = new System.Drawing.Point(857, 375);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(861, 365);
            this.panel1.TabIndex = 19;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(852, 343);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(178, 29);
            this.label2.TabIndex = 110;
            this.label2.Text = "Conveyor Action";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(19, 343);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 29);
            this.label3.TabIndex = 110;
            this.label3.Text = "Width Move";
            this.label3.Visible = false;
            // 
            // btnPosRearNGLine
            // 
            this.btnPosRearNGLine.BackColor = System.Drawing.Color.White;
            this.btnPosRearNGLine.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPosRearNGLine.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.btnPosRearNGLine.Location = new System.Drawing.Point(872, 128);
            this.btnPosRearNGLine.Name = "btnPosRearNGLine";
            this.btnPosRearNGLine.Size = new System.Drawing.Size(236, 84);
            this.btnPosRearNGLine.TabIndex = 137;
            this.btnPosRearNGLine.Text = "RearNGLine";
            this.btnPosRearNGLine.UseVisualStyleBackColor = false;
            // 
            // btnPosInShuttle
            // 
            this.btnPosInShuttle.BackColor = System.Drawing.Color.White;
            this.btnPosInShuttle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPosInShuttle.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.btnPosInShuttle.Location = new System.Drawing.Point(19, 136);
            this.btnPosInShuttle.Name = "btnPosInShuttle";
            this.btnPosInShuttle.Size = new System.Drawing.Size(236, 84);
            this.btnPosInShuttle.TabIndex = 138;
            this.btnPosInShuttle.Text = "In Shuttle";
            this.btnPosInShuttle.UseVisualStyleBackColor = false;
            // 
            // btnPosOutShuttle
            // 
            this.btnPosOutShuttle.BackColor = System.Drawing.Color.White;
            this.btnPosOutShuttle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPosOutShuttle.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.btnPosOutShuttle.Location = new System.Drawing.Point(1156, 128);
            this.btnPosOutShuttle.Name = "btnPosOutShuttle";
            this.btnPosOutShuttle.Size = new System.Drawing.Size(236, 84);
            this.btnPosOutShuttle.TabIndex = 139;
            this.btnPosOutShuttle.Text = "Out Shuttle";
            this.btnPosOutShuttle.UseVisualStyleBackColor = false;
            // 
            // Conveyor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(185)))), ((int)(((byte)(229)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1730, 890);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.pnJog);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Conveyor";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Manual";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Manual_FormClosed);
            this.Shown += new System.EventHandler(this.Manual_Shown);
            this.panel4.ResumeLayout(false);
            this.pnJog.ResumeLayout(false);
            this.pnJog.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPitch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAxisStatus)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMovePos)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button btnStopperUp;
        private System.Windows.Forms.Button btnRunConveyorCW;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnPosInConveyor;
        private System.Windows.Forms.Button btnPosOutConveyor;
        private System.Windows.Forms.Button btnPosLift2Up;
        private System.Windows.Forms.Button btnPosRearPassLine;
        private System.Windows.Forms.Button btnPosLift1Up;
        private System.Windows.Forms.Button btnPosFrontPassLine;
        private System.Windows.Forms.Button btnPosNG;
        private System.Windows.Forms.Button btnStopConveyor;
        private System.Windows.Forms.Button btnNGForward;
        private System.Windows.Forms.Button btnNGStop;
        private System.Windows.Forms.Button btnNGReward;
        private System.Windows.Forms.Button btnStopperDown;
        private System.Windows.Forms.Panel pnJog;
        private System.Windows.Forms.Button pbJogWide;
        private System.Windows.Forms.Button pbJogNarrow;
        private System.Windows.Forms.Button pbStop;
        private System.Windows.Forms.Button pbHome;
        private System.Windows.Forms.Button pbMove;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblCurrPos;
        private System.Windows.Forms.NumericUpDown numMovePos;
        private System.Windows.Forms.Button btnTeachingWidth;
        private System.Windows.Forms.Button btnRunConveyorCCW;
        private System.Windows.Forms.PictureBox pbAxisStatus;
        private System.Windows.Forms.Label lblTeachingWidth;
        private System.Windows.Forms.ProgressBar progressBar2;
        private System.Windows.Forms.NumericUpDown numSpeed;
        private System.Windows.Forms.Button btnSpeed;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.NumericUpDown numPitch;
        private System.Windows.Forms.Button btnPitch;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnPosRearNGLine;
        private System.Windows.Forms.Button btnPosInShuttle;
        private System.Windows.Forms.Button btnPosOutShuttle;
    }
}