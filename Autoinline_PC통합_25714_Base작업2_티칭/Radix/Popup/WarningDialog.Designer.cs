using System;

namespace Radix.Popup
{
    partial class WarningDialog
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
            this.Warning = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.WaringName = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.TimerUI = new System.Windows.Forms.Timer(this.components);
            this.timecheck = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Warning
            // 
            this.Warning.BackColor = System.Drawing.Color.Transparent;
            this.Warning.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Warning.Font = new System.Drawing.Font("Calibri", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Warning.ForeColor = System.Drawing.Color.Red;
            this.Warning.Location = new System.Drawing.Point(0, 40);
            this.Warning.Name = "Warning";
            this.Warning.Size = new System.Drawing.Size(816, 86);
            this.Warning.TabIndex = 0;
            this.Warning.Text = "Warning";
            this.Warning.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Silver;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Calibri", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(86, 350);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(270, 66);
            this.button1.TabIndex = 1;
            this.button1.Text = "설비정지";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.Stop_Click);
            // 
            // WaringName
            // 
            this.WaringName.BackColor = System.Drawing.Color.Transparent;
            this.WaringName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.WaringName.Font = new System.Drawing.Font("Calibri", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WaringName.ForeColor = System.Drawing.Color.Red;
            this.WaringName.Location = new System.Drawing.Point(-1, 126);
            this.WaringName.Name = "WaringName";
            this.WaringName.Size = new System.Drawing.Size(817, 193);
            this.WaringName.TabIndex = 2;
            this.WaringName.Text = "-";
            this.WaringName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Silver;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Calibri", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(453, 350);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(270, 66);
            this.button2.TabIndex = 3;
            this.button2.Text = "닫기";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // TimerUI
            // 
            this.TimerUI.Enabled = true;
            this.TimerUI.Interval = 1000;
            this.TimerUI.Tick += new System.EventHandler(this.TimerUI_Tick);
            // 
            // timecheck
            // 
            this.timecheck.AutoSize = true;
            this.timecheck.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timecheck.Location = new System.Drawing.Point(745, 21);
            this.timecheck.Name = "timecheck";
            this.timecheck.Size = new System.Drawing.Size(21, 19);
            this.timecheck.TabIndex = 4;
            this.timecheck.Text = "초";
            // 
            // WarningDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(818, 448);
            this.ControlBox = false;
            this.Controls.Add(this.timecheck);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.WaringName);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Warning);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "WarningDialog";
            this.Text = "Warring";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WarningDialog_FormClosing);
            this.Shown += new System.EventHandler(this.WarningDialog_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Warning;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label WaringName;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Timer TimerUI;
        private System.Windows.Forms.Label timecheck;
    }
}