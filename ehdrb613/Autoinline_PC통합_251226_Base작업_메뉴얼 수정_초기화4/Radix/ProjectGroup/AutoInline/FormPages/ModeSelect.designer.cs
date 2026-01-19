namespace Radix
{
    partial class ModeSelect
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
            this.btnAuto = new System.Windows.Forms.Button();
            this.btnPass = new System.Windows.Forms.Button();
            this.btnSimulation = new System.Windows.Forms.Button();
            this.btnDryRun = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnAuto
            // 
            this.btnAuto.BackColor = System.Drawing.Color.White;
            this.btnAuto.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAuto.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Bold);
            this.btnAuto.Location = new System.Drawing.Point(29, 30);
            this.btnAuto.Name = "btnAuto";
            this.btnAuto.Size = new System.Drawing.Size(140, 68);
            this.btnAuto.TabIndex = 148;
            this.btnAuto.Text = "Auto Mode";
            this.btnAuto.UseVisualStyleBackColor = false;
            this.btnAuto.Click += new System.EventHandler(this.btnAuto_Click);
            // 
            // btnPass
            // 
            this.btnPass.BackColor = System.Drawing.Color.White;
            this.btnPass.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPass.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Bold);
            this.btnPass.Location = new System.Drawing.Point(197, 30);
            this.btnPass.Name = "btnPass";
            this.btnPass.Size = new System.Drawing.Size(140, 68);
            this.btnPass.TabIndex = 148;
            this.btnPass.Text = "Pass Mode";
            this.btnPass.UseVisualStyleBackColor = false;
            this.btnPass.Click += new System.EventHandler(this.btnPass_Click);
            // 
            // btnSimulation
            // 
            this.btnSimulation.BackColor = System.Drawing.Color.White;
            this.btnSimulation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSimulation.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Bold);
            this.btnSimulation.Location = new System.Drawing.Point(29, 125);
            this.btnSimulation.Name = "btnSimulation";
            this.btnSimulation.Size = new System.Drawing.Size(140, 68);
            this.btnSimulation.TabIndex = 148;
            this.btnSimulation.Text = "Simulation Mode";
            this.btnSimulation.UseVisualStyleBackColor = false;
            this.btnSimulation.Click += new System.EventHandler(this.btnSimulation_Click);
            // 
            // btnDryRun
            // 
            this.btnDryRun.BackColor = System.Drawing.Color.White;
            this.btnDryRun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDryRun.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Bold);
            this.btnDryRun.Location = new System.Drawing.Point(197, 125);
            this.btnDryRun.Name = "btnDryRun";
            this.btnDryRun.Size = new System.Drawing.Size(140, 68);
            this.btnDryRun.TabIndex = 148;
            this.btnDryRun.Text = "DryRun Mode";
            this.btnDryRun.UseVisualStyleBackColor = false;
            this.btnDryRun.Click += new System.EventHandler(this.btnDryRun_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Coral;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Bold);
            this.btnClose.Location = new System.Drawing.Point(117, 223);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(140, 68);
            this.btnClose.TabIndex = 148;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // ModeSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(373, 315);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnDryRun);
            this.Controls.Add(this.btnSimulation);
            this.Controls.Add(this.btnPass);
            this.Controls.Add(this.btnAuto);
            this.Name = "ModeSelect";
            this.Text = "ModeSelect";
            this.Shown += new System.EventHandler(this.ModeSelect_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnAuto;
        private System.Windows.Forms.Button btnPass;
        private System.Windows.Forms.Button btnSimulation;
        private System.Windows.Forms.Button btnDryRun;
        private System.Windows.Forms.Button btnClose;
    }
}