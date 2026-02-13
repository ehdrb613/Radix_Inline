namespace Radix
{
    partial class Maintenance
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbDepart = new System.Windows.Forms.TextBox();
            this.tbReason = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnIdle = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 56F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(288, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(309, 75);
            this.label1.TabIndex = 0;
            this.label1.Text = "점 검 중";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 56F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(83, 176);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(358, 75);
            this.label2.TabIndex = 0;
            this.label2.Text = "관계자 외";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("굴림", 56F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(516, 176);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(332, 75);
            this.label3.TabIndex = 0;
            this.label3.Text = "조작금지";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("굴림", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(57, 302);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(261, 38);
            this.label4.TabIndex = 1;
            this.label4.Text = "담당자(소속) :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("굴림", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.Location = new System.Drawing.Point(57, 365);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(226, 38);
            this.label5.TabIndex = 1;
            this.label5.Text = "연락처(TEL)";
            // 
            // tbDepart
            // 
            this.tbDepart.Font = new System.Drawing.Font("굴림", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbDepart.Location = new System.Drawing.Point(404, 298);
            this.tbDepart.Multiline = true;
            this.tbDepart.Name = "tbDepart";
            this.tbDepart.Size = new System.Drawing.Size(541, 207);
            this.tbDepart.TabIndex = 2;
            // 
            // tbReason
            // 
            this.tbReason.Font = new System.Drawing.Font("굴림", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbReason.Location = new System.Drawing.Point(156, 590);
            this.tbReason.Name = "tbReason";
            this.tbReason.Size = new System.Drawing.Size(501, 50);
            this.tbReason.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("굴림", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.Location = new System.Drawing.Point(57, 593);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(93, 38);
            this.label6.TabIndex = 1;
            this.label6.Text = "사유";
            // 
            // btnIdle
            // 
            this.btnIdle.Font = new System.Drawing.Font("굴림", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnIdle.Location = new System.Drawing.Point(680, 590);
            this.btnIdle.Name = "btnIdle";
            this.btnIdle.Size = new System.Drawing.Size(144, 49);
            this.btnIdle.TabIndex = 3;
            this.btnIdle.Text = "비가동";
            this.btnIdle.UseVisualStyleBackColor = true;
            this.btnIdle.Click += new System.EventHandler(this.btnIdle_Click);
            // 
            // btnRun
            // 
            this.btnRun.Font = new System.Drawing.Font("굴림", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRun.Location = new System.Drawing.Point(830, 590);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(144, 49);
            this.btnRun.TabIndex = 3;
            this.btnRun.Text = "가동";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // Maintenance
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1006, 721);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.btnIdle);
            this.Controls.Add(this.tbReason);
            this.Controls.Add(this.tbDepart);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Maintenance";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Maintenance";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbDepart;
        private System.Windows.Forms.TextBox tbReason;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnIdle;
        private System.Windows.Forms.Button btnRun;
    }
}