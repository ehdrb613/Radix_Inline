namespace Radix
{
    partial class Manual
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
            this.btnLift_Manual = new System.Windows.Forms.Button();
            this.btnConveyor_Manual = new System.Windows.Forms.Button();
            this.hide_Panel = new System.Windows.Forms.Panel();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.btnVision_Manual = new System.Windows.Forms.Button();
            this.tcManual = new System.Windows.Forms.TabControl();
            this.tpRobot = new System.Windows.Forms.TabPage();
            this.tpSite = new System.Windows.Forms.TabPage();
            this.tpConveyor = new System.Windows.Forms.TabPage();
            this.tpVision = new System.Windows.Forms.TabPage();
            this.btnSite_Manual = new System.Windows.Forms.Button();
            this.label436 = new System.Windows.Forms.Label();
            this.btnFast = new System.Windows.Forms.Button();
            this.btnSlow = new System.Windows.Forms.Button();
            this.btnMiddle = new System.Windows.Forms.Button();
            this.btn1mm = new System.Windows.Forms.Button();
            this.btn01mm = new System.Windows.Forms.Button();
            this.hide_Panel.SuspendLayout();
            this.tcManual.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLift_Manual
            // 
            this.btnLift_Manual.BackColor = System.Drawing.Color.LightCyan;
            this.btnLift_Manual.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnLift_Manual.FlatAppearance.BorderSize = 2;
            this.btnLift_Manual.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLift_Manual.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLift_Manual.ForeColor = System.Drawing.Color.Black;
            this.btnLift_Manual.Location = new System.Drawing.Point(11, 11);
            this.btnLift_Manual.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnLift_Manual.Name = "btnLift_Manual";
            this.btnLift_Manual.Size = new System.Drawing.Size(136, 41);
            this.btnLift_Manual.TabIndex = 514;
            this.btnLift_Manual.Text = "Lift";
            this.btnLift_Manual.UseVisualStyleBackColor = false;
            this.btnLift_Manual.Click += new System.EventHandler(this.btnRobot_Manual_Click);
            // 
            // btnConveyor_Manual
            // 
            this.btnConveyor_Manual.BackColor = System.Drawing.Color.LightCyan;
            this.btnConveyor_Manual.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnConveyor_Manual.FlatAppearance.BorderSize = 2;
            this.btnConveyor_Manual.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConveyor_Manual.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold);
            this.btnConveyor_Manual.ForeColor = System.Drawing.Color.Black;
            this.btnConveyor_Manual.Location = new System.Drawing.Point(295, 11);
            this.btnConveyor_Manual.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnConveyor_Manual.Name = "btnConveyor_Manual";
            this.btnConveyor_Manual.Size = new System.Drawing.Size(136, 41);
            this.btnConveyor_Manual.TabIndex = 515;
            this.btnConveyor_Manual.Text = "Conveyor";
            this.btnConveyor_Manual.UseVisualStyleBackColor = false;
            this.btnConveyor_Manual.Click += new System.EventHandler(this.btnConveyor_Manual_Click);
            // 
            // hide_Panel
            // 
            this.hide_Panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(185)))), ((int)(((byte)(229)))));
            this.hide_Panel.Controls.Add(this.lblSpeed);
            this.hide_Panel.Location = new System.Drawing.Point(-13, -7);
            this.hide_Panel.Margin = new System.Windows.Forms.Padding(0);
            this.hide_Panel.Name = "hide_Panel";
            this.hide_Panel.Size = new System.Drawing.Size(2033, 71);
            this.hide_Panel.TabIndex = 513;
            // 
            // lblSpeed
            // 
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.BackColor = System.Drawing.Color.Transparent;
            this.lblSpeed.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpeed.Location = new System.Drawing.Point(1096, 28);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(59, 23);
            this.lblSpeed.TabIndex = 529;
            this.lblSpeed.Text = "Speed";
            this.lblSpeed.Visible = false;
            // 
            // btnVision_Manual
            // 
            this.btnVision_Manual.BackColor = System.Drawing.Color.LightCyan;
            this.btnVision_Manual.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnVision_Manual.FlatAppearance.BorderSize = 2;
            this.btnVision_Manual.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVision_Manual.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold);
            this.btnVision_Manual.ForeColor = System.Drawing.Color.Black;
            this.btnVision_Manual.Location = new System.Drawing.Point(437, 11);
            this.btnVision_Manual.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnVision_Manual.Name = "btnVision_Manual";
            this.btnVision_Manual.Size = new System.Drawing.Size(136, 41);
            this.btnVision_Manual.TabIndex = 516;
            this.btnVision_Manual.Text = "Vision";
            this.btnVision_Manual.UseVisualStyleBackColor = false;
            this.btnVision_Manual.Visible = false;
            this.btnVision_Manual.Click += new System.EventHandler(this.btnVision_Manual_Click);
            // 
            // tcManual
            // 
            this.tcManual.Controls.Add(this.tpRobot);
            this.tcManual.Controls.Add(this.tpSite);
            this.tcManual.Controls.Add(this.tpConveyor);
            this.tcManual.Controls.Add(this.tpVision);
            this.tcManual.Location = new System.Drawing.Point(-5, 40);
            this.tcManual.Margin = new System.Windows.Forms.Padding(0);
            this.tcManual.Name = "tcManual";
            this.tcManual.Padding = new System.Drawing.Point(0, 0);
            this.tcManual.SelectedIndex = 0;
            this.tcManual.Size = new System.Drawing.Size(1835, 998);
            this.tcManual.TabIndex = 521;
            // 
            // tpRobot
            // 
            this.tpRobot.Location = new System.Drawing.Point(4, 22);
            this.tpRobot.Margin = new System.Windows.Forms.Padding(0);
            this.tpRobot.Name = "tpRobot";
            this.tpRobot.Size = new System.Drawing.Size(1827, 972);
            this.tpRobot.TabIndex = 0;
            this.tpRobot.Text = "tabPage1";
            this.tpRobot.UseVisualStyleBackColor = true;
            // 
            // tpSite
            // 
            this.tpSite.Location = new System.Drawing.Point(4, 22);
            this.tpSite.Name = "tpSite";
            this.tpSite.Size = new System.Drawing.Size(1827, 972);
            this.tpSite.TabIndex = 3;
            this.tpSite.Text = "tabPage1";
            this.tpSite.UseVisualStyleBackColor = true;
            // 
            // tpConveyor
            // 
            this.tpConveyor.Location = new System.Drawing.Point(4, 22);
            this.tpConveyor.Margin = new System.Windows.Forms.Padding(0);
            this.tpConveyor.Name = "tpConveyor";
            this.tpConveyor.Size = new System.Drawing.Size(1827, 972);
            this.tpConveyor.TabIndex = 1;
            this.tpConveyor.Text = "tabPage2";
            this.tpConveyor.UseVisualStyleBackColor = true;
            // 
            // tpVision
            // 
            this.tpVision.Location = new System.Drawing.Point(4, 22);
            this.tpVision.Margin = new System.Windows.Forms.Padding(0);
            this.tpVision.Name = "tpVision";
            this.tpVision.Size = new System.Drawing.Size(1827, 972);
            this.tpVision.TabIndex = 2;
            this.tpVision.Text = "tabPage3";
            this.tpVision.UseVisualStyleBackColor = true;
            // 
            // btnSite_Manual
            // 
            this.btnSite_Manual.BackColor = System.Drawing.Color.LightCyan;
            this.btnSite_Manual.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnSite_Manual.FlatAppearance.BorderSize = 2;
            this.btnSite_Manual.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSite_Manual.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSite_Manual.ForeColor = System.Drawing.Color.Black;
            this.btnSite_Manual.Location = new System.Drawing.Point(153, 11);
            this.btnSite_Manual.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSite_Manual.Name = "btnSite_Manual";
            this.btnSite_Manual.Size = new System.Drawing.Size(136, 41);
            this.btnSite_Manual.TabIndex = 514;
            this.btnSite_Manual.Text = "Site";
            this.btnSite_Manual.UseVisualStyleBackColor = false;
            this.btnSite_Manual.Click += new System.EventHandler(this.btnSite_Manual_Click);
            // 
            // label436
            // 
            this.label436.AutoSize = true;
            this.label436.BackColor = System.Drawing.Color.Transparent;
            this.label436.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label436.Location = new System.Drawing.Point(582, 20);
            this.label436.Name = "label436";
            this.label436.Size = new System.Drawing.Size(59, 23);
            this.label436.TabIndex = 529;
            this.label436.Text = "Speed";
            this.label436.Visible = false;
            // 
            // btnFast
            // 
            this.btnFast.BackColor = System.Drawing.Color.White;
            this.btnFast.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFast.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Bold);
            this.btnFast.Location = new System.Drawing.Point(982, 10);
            this.btnFast.Name = "btnFast";
            this.btnFast.Size = new System.Drawing.Size(79, 44);
            this.btnFast.TabIndex = 526;
            this.btnFast.Text = "Fast";
            this.btnFast.UseVisualStyleBackColor = false;
            this.btnFast.Visible = false;
            this.btnFast.Click += new System.EventHandler(this.btnFast_Click);
            // 
            // btnSlow
            // 
            this.btnSlow.BackColor = System.Drawing.Color.White;
            this.btnSlow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSlow.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Bold);
            this.btnSlow.Location = new System.Drawing.Point(812, 10);
            this.btnSlow.Name = "btnSlow";
            this.btnSlow.Size = new System.Drawing.Size(79, 44);
            this.btnSlow.TabIndex = 527;
            this.btnSlow.Text = "Slow";
            this.btnSlow.UseVisualStyleBackColor = false;
            this.btnSlow.Visible = false;
            this.btnSlow.Click += new System.EventHandler(this.btnSlow_Click);
            // 
            // btnMiddle
            // 
            this.btnMiddle.BackColor = System.Drawing.Color.Lime;
            this.btnMiddle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMiddle.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Bold);
            this.btnMiddle.Location = new System.Drawing.Point(897, 10);
            this.btnMiddle.Name = "btnMiddle";
            this.btnMiddle.Size = new System.Drawing.Size(79, 44);
            this.btnMiddle.TabIndex = 528;
            this.btnMiddle.Text = "Middle";
            this.btnMiddle.UseVisualStyleBackColor = false;
            this.btnMiddle.Visible = false;
            this.btnMiddle.Click += new System.EventHandler(this.btnMiddle_Click);
            // 
            // btn1mm
            // 
            this.btn1mm.BackColor = System.Drawing.Color.White;
            this.btn1mm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn1mm.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Bold);
            this.btn1mm.Location = new System.Drawing.Point(727, 10);
            this.btn1mm.Name = "btn1mm";
            this.btn1mm.Size = new System.Drawing.Size(79, 44);
            this.btn1mm.TabIndex = 527;
            this.btn1mm.Text = "1mm";
            this.btn1mm.UseVisualStyleBackColor = false;
            this.btn1mm.Visible = false;
            this.btn1mm.Click += new System.EventHandler(this.btn1mm_Click);
            // 
            // btn01mm
            // 
            this.btn01mm.BackColor = System.Drawing.Color.White;
            this.btn01mm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn01mm.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Bold);
            this.btn01mm.Location = new System.Drawing.Point(642, 10);
            this.btn01mm.Name = "btn01mm";
            this.btn01mm.Size = new System.Drawing.Size(79, 44);
            this.btn01mm.TabIndex = 527;
            this.btn01mm.Text = "0.1mm";
            this.btn01mm.UseVisualStyleBackColor = false;
            this.btn01mm.Visible = false;
            this.btn01mm.Click += new System.EventHandler(this.btn01mm_Click);
            // 
            // Manual
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(185)))), ((int)(((byte)(229)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1751, 992);
            this.Controls.Add(this.btnSite_Manual);
            this.Controls.Add(this.btnFast);
            this.Controls.Add(this.btn01mm);
            this.Controls.Add(this.btn1mm);
            this.Controls.Add(this.label436);
            this.Controls.Add(this.btnSlow);
            this.Controls.Add(this.btnMiddle);
            this.Controls.Add(this.btnVision_Manual);
            this.Controls.Add(this.btnConveyor_Manual);
            this.Controls.Add(this.btnLift_Manual);
            this.Controls.Add(this.hide_Panel);
            this.Controls.Add(this.tcManual);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Manual";
            this.Text = "Setting";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Setting_FormClosed);
            this.Load += new System.EventHandler(this.Setting_Load);
            this.Shown += new System.EventHandler(this.Setting_Shown);
            this.hide_Panel.ResumeLayout(false);
            this.hide_Panel.PerformLayout();
            this.tcManual.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnLift_Manual;
        private System.Windows.Forms.Button btnConveyor_Manual;
        private System.Windows.Forms.Panel hide_Panel;
        private System.Windows.Forms.Button btnVision_Manual;
        private System.Windows.Forms.TabControl tcManual;
        private System.Windows.Forms.TabPage tpRobot;
        private System.Windows.Forms.TabPage tpConveyor;
        private System.Windows.Forms.TabPage tpVision;
        private System.Windows.Forms.TabPage tpSite;
        private System.Windows.Forms.Button btnSite_Manual;
        private System.Windows.Forms.Label label436;
        private System.Windows.Forms.Button btnFast;
        private System.Windows.Forms.Button btnSlow;
        private System.Windows.Forms.Button btnMiddle;
        private System.Windows.Forms.Button btn1mm;
        private System.Windows.Forms.Button btn01mm;
        private System.Windows.Forms.Label lblSpeed;
    }
}