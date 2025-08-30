namespace Radix
{
    /*
     * CountLog.cs : 일자별 생산량 로그 확인
     */

    partial class CountLog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CountLog));
            this.gridCount = new System.Windows.Forms.DataGridView();
            this.date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PCB = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Array = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Side = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Result = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmbFiles = new System.Windows.Forms.ComboBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblSum = new System.Windows.Forms.Label();
            this.pbSaveCSV = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.textBox_Search = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.gridCount)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSaveCSV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // gridCount
            // 
            this.gridCount.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridCount.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.date,
            this.PCB,
            this.Array,
            this.Side,
            this.Code,
            this.Result});
            this.gridCount.Location = new System.Drawing.Point(33, 64);
            this.gridCount.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gridCount.Name = "gridCount";
            this.gridCount.RowTemplate.Height = 27;
            this.gridCount.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridCount.Size = new System.Drawing.Size(800, 510);
            this.gridCount.TabIndex = 1;
            // 
            // date
            // 
            this.date.HeaderText = "Date";
            this.date.Name = "date";
            // 
            // PCB
            // 
            this.PCB.HeaderText = "PCB Num.";
            this.PCB.Name = "PCB";
            this.PCB.Width = 130;
            // 
            // Array
            // 
            this.Array.HeaderText = "Array Num.";
            this.Array.Name = "Array";
            this.Array.Width = 130;
            // 
            // Side
            // 
            this.Side.HeaderText = "Side";
            this.Side.Name = "Side";
            // 
            // Code
            // 
            this.Code.HeaderText = "Code";
            this.Code.Name = "Code";
            // 
            // Result
            // 
            this.Result.HeaderText = "Result";
            this.Result.Name = "Result";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.Controls.Add(this.textBox_Search);
            this.panel1.Controls.Add(this.pictureBox3);
            this.panel1.Controls.Add(this.cmbFiles);
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(-1, -1);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(865, 100);
            this.panel1.TabIndex = 2;
            // 
            // cmbFiles
            // 
            this.cmbFiles.FormattingEnabled = true;
            this.cmbFiles.Location = new System.Drawing.Point(33, 34);
            this.cmbFiles.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbFiles.Name = "cmbFiles";
            this.cmbFiles.Size = new System.Drawing.Size(138, 23);
            this.cmbFiles.TabIndex = 4;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox2.BackgroundImage")));
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox2.Location = new System.Drawing.Point(189, 20);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(114, 51);
            this.pictureBox2.TabIndex = 3;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(695, 12);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(160, 68);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // panel2
            // 
            this.panel2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel2.BackgroundImage")));
            this.panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel2.Controls.Add(this.lblSum);
            this.panel2.Controls.Add(this.pbSaveCSV);
            this.panel2.Controls.Add(this.gridCount);
            this.panel2.Location = new System.Drawing.Point(-1, 86);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(865, 666);
            this.panel2.TabIndex = 3;
            // 
            // lblSum
            // 
            this.lblSum.AutoSize = true;
            this.lblSum.BackColor = System.Drawing.Color.Transparent;
            this.lblSum.Location = new System.Drawing.Point(31, 30);
            this.lblSum.Name = "lblSum";
            this.lblSum.Size = new System.Drawing.Size(0, 15);
            this.lblSum.TabIndex = 2;
            // 
            // pbSaveCSV
            // 
            this.pbSaveCSV.BackColor = System.Drawing.Color.Transparent;
            this.pbSaveCSV.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pbSaveCSV.BackgroundImage")));
            this.pbSaveCSV.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbSaveCSV.Location = new System.Drawing.Point(719, 590);
            this.pbSaveCSV.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pbSaveCSV.Name = "pbSaveCSV";
            this.pbSaveCSV.Size = new System.Drawing.Size(114, 51);
            this.pbSaveCSV.TabIndex = 3;
            this.pbSaveCSV.TabStop = false;
            this.pbSaveCSV.Click += new System.EventHandler(this.pbSaveCSV_Click);
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox3.BackgroundImage = global::Radix.Properties.Resources.search;
            this.pictureBox3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox3.Location = new System.Drawing.Point(512, 20);
            this.pictureBox3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(114, 51);
            this.pictureBox3.TabIndex = 5;
            this.pictureBox3.TabStop = false;
            this.pictureBox3.Click += new System.EventHandler(this.pictureBox3_Click);
            // 
            // textBox_Search
            // 
            this.textBox_Search.Location = new System.Drawing.Point(353, 34);
            this.textBox_Search.Name = "textBox_Search";
            this.textBox_Search.Size = new System.Drawing.Size(139, 25);
            this.textBox_Search.TabIndex = 6;
            // 
            // CountLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(864, 741);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "CountLog";
            this.Text = "CountLog";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CountLog_FormClosed);
            this.Shown += new System.EventHandler(this.CountLog_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.gridCount)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSaveCSV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView gridCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn inCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn outCount;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn date;
        private System.Windows.Forms.DataGridViewTextBoxColumn PCB;
        private System.Windows.Forms.DataGridViewTextBoxColumn Array;
        private System.Windows.Forms.DataGridViewTextBoxColumn Side;
        private System.Windows.Forms.DataGridViewTextBoxColumn Code;
        private System.Windows.Forms.DataGridViewTextBoxColumn Result;
        private System.Windows.Forms.ComboBox cmbFiles;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblSum;
        private System.Windows.Forms.PictureBox pbSaveCSV;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.TextBox textBox_Search;
    }
}