namespace Radix
{
    partial class LogViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogViewer));
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnSql = new System.Windows.Forms.Panel();
            this.dataGridError = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridAll = new System.Windows.Forms.DataGridView();
            this.dataGridPart = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.pbExcelAllError = new System.Windows.Forms.PictureBox();
            this.panel32 = new System.Windows.Forms.Panel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.dateEnd = new System.Windows.Forms.DateTimePicker();
            this.dateStart = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            this.pnSql.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridError)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridAll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbExcelAllError)).BeginInit();
            this.panel32.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(185)))), ((int)(((byte)(229)))));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.pnSql);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.tbLog);
            this.panel1.Location = new System.Drawing.Point(-1, 43);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1732, 840);
            this.panel1.TabIndex = 17;
            // 
            // pnSql
            // 
            this.pnSql.BackColor = System.Drawing.Color.Transparent;
            this.pnSql.Controls.Add(this.dataGridError);
            this.pnSql.Controls.Add(this.dataGridAll);
            this.pnSql.Controls.Add(this.dataGridPart);
            this.pnSql.Controls.Add(this.label2);
            this.pnSql.Location = new System.Drawing.Point(490, 19);
            this.pnSql.Name = "pnSql";
            this.pnSql.Size = new System.Drawing.Size(1195, 747);
            this.pnSql.TabIndex = 5;
            // 
            // dataGridError
            // 
            this.dataGridError.AllowUserToAddRows = false;
            this.dataGridError.AllowUserToDeleteRows = false;
            this.dataGridError.BackgroundColor = System.Drawing.Color.White;
            this.dataGridError.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridError.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.Column5,
            this.dataGridViewTextBoxColumn3});
            this.dataGridError.Location = new System.Drawing.Point(768, 429);
            this.dataGridError.Name = "dataGridError";
            this.dataGridError.ReadOnly = true;
            this.dataGridError.RowHeadersVisible = false;
            this.dataGridError.RowTemplate.Height = 23;
            this.dataGridError.Size = new System.Drawing.Size(424, 315);
            this.dataGridError.TabIndex = 3;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "ErrorCode";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 120;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "ErrorName";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            this.Column5.Width = 200;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "ErrorCount";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridAll
            // 
            this.dataGridAll.AllowUserToAddRows = false;
            this.dataGridAll.AllowUserToDeleteRows = false;
            this.dataGridAll.BackgroundColor = System.Drawing.Color.White;
            this.dataGridAll.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridAll.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column6,
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column7,
            this.Column8});
            this.dataGridAll.Location = new System.Drawing.Point(3, 35);
            this.dataGridAll.Name = "dataGridAll";
            this.dataGridAll.ReadOnly = true;
            this.dataGridAll.RowHeadersVisible = false;
            this.dataGridAll.RowTemplate.Height = 23;
            this.dataGridAll.Size = new System.Drawing.Size(1189, 372);
            this.dataGridAll.TabIndex = 3;
            // 
            // dataGridPart
            // 
            this.dataGridPart.AllowUserToAddRows = false;
            this.dataGridPart.AllowUserToDeleteRows = false;
            this.dataGridPart.BackgroundColor = System.Drawing.Color.White;
            this.dataGridPart.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridPart.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn4});
            this.dataGridPart.Location = new System.Drawing.Point(446, 429);
            this.dataGridPart.Name = "dataGridPart";
            this.dataGridPart.ReadOnly = true;
            this.dataGridPart.RowHeadersVisible = false;
            this.dataGridPart.RowTemplate.Height = 23;
            this.dataGridPart.Size = new System.Drawing.Size(277, 315);
            this.dataGridPart.TabIndex = 3;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Part";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 120;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "ErrorCount";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 150;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Current Error Log";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "System Log";
            // 
            // tbLog
            // 
            this.tbLog.Location = new System.Drawing.Point(13, 42);
            this.tbLog.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbLog.Size = new System.Drawing.Size(471, 724);
            this.tbLog.TabIndex = 1;
            // 
            // pbExcelAllError
            // 
            this.pbExcelAllError.BackgroundImage = global::Radix.Properties.Resources.excel;
            this.pbExcelAllError.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbExcelAllError.Location = new System.Drawing.Point(1621, 7);
            this.pbExcelAllError.Name = "pbExcelAllError";
            this.pbExcelAllError.Size = new System.Drawing.Size(80, 30);
            this.pbExcelAllError.TabIndex = 4;
            this.pbExcelAllError.TabStop = false;
            this.pbExcelAllError.Visible = false;
            this.pbExcelAllError.Click += new System.EventHandler(this.pbExcelAllError_Click);
            // 
            // panel32
            // 
            this.panel32.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel32.BackgroundImage")));
            this.panel32.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel32.Controls.Add(this.btnRefresh);
            this.panel32.Controls.Add(this.pbExcelAllError);
            this.panel32.Controls.Add(this.dateEnd);
            this.panel32.Controls.Add(this.dateStart);
            this.panel32.Controls.Add(this.label3);
            this.panel32.Location = new System.Drawing.Point(0, 0);
            this.panel32.Name = "panel32";
            this.panel32.Size = new System.Drawing.Size(1728, 44);
            this.panel32.TabIndex = 16;
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.Transparent;
            this.btnRefresh.BackgroundImage = global::Radix.Properties.Resources.refresh;
            this.btnRefresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.btnRefresh.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Location = new System.Drawing.Point(265, 7);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(82, 33);
            this.btnRefresh.TabIndex = 17;
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // dateEnd
            // 
            this.dateEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateEnd.Location = new System.Drawing.Point(152, 12);
            this.dateEnd.Name = "dateEnd";
            this.dateEnd.Size = new System.Drawing.Size(101, 21);
            this.dateEnd.TabIndex = 16;
            // 
            // dateStart
            // 
            this.dateStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateStart.Location = new System.Drawing.Point(24, 12);
            this.dateStart.Name = "dateStart";
            this.dateStart.Size = new System.Drawing.Size(101, 21);
            this.dateStart.TabIndex = 16;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(132, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "~";
            // 
            // Column6
            // 
            this.Column6.HeaderText = "Date";
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            this.Column6.Width = 120;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Time";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Part";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Visible = false;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "ErrorCode";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "ErrorName";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.Width = 200;
            // 
            // Column7
            // 
            this.Column7.HeaderText = "Clear";
            this.Column7.Name = "Column7";
            this.Column7.ReadOnly = true;
            this.Column7.Width = 60;
            // 
            // Column8
            // 
            this.Column8.HeaderText = "Description";
            this.Column8.Name = "Column8";
            this.Column8.ReadOnly = true;
            this.Column8.Width = 610;
            // 
            // LogViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1713, 850);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel32);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "LogViewer";
            this.Text = "LogViewer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LogViewer_FormClosed);
            this.Shown += new System.EventHandler(this.LogViewer_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnSql.ResumeLayout(false);
            this.pnSql.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridError)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridAll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbExcelAllError)).EndInit();
            this.panel32.ResumeLayout(false);
            this.panel32.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.Panel panel32;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dataGridError;
        private System.Windows.Forms.DataGridView dataGridAll;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dateEnd;
        private System.Windows.Forms.DateTimePicker dateStart;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox pbExcelAllError;
        private System.Windows.Forms.Panel pnSql;
        private System.Windows.Forms.DataGridView dataGridPart;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
    }
}