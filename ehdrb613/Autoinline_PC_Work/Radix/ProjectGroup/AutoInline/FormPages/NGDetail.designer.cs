namespace Radix
{
    partial class NGDetail
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
            this.lblSiteName = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.dataGridNG = new System.Windows.Forms.DataGridView();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Site = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Array = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Barcode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DefectCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DefectName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridNG)).BeginInit();
            this.SuspendLayout();
            // 
            // lblSiteName
            // 
            this.lblSiteName.BackColor = System.Drawing.Color.Transparent;
            this.lblSiteName.Font = new System.Drawing.Font("Calibri", 32F, System.Drawing.FontStyle.Bold);
            this.lblSiteName.ForeColor = System.Drawing.Color.OrangeRed;
            this.lblSiteName.Location = new System.Drawing.Point(-1, 12);
            this.lblSiteName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSiteName.Name = "lblSiteName";
            this.lblSiteName.Size = new System.Drawing.Size(822, 71);
            this.lblSiteName.TabIndex = 20;
            this.lblSiteName.Text = "Test History";
            this.lblSiteName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.White;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Bold);
            this.btnClose.Location = new System.Drawing.Point(332, 650);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(153, 53);
            this.btnClose.TabIndex = 144;
            this.btnClose.Text = "CLOSE";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // dataGridNG
            // 
            this.dataGridNG.BackgroundColor = System.Drawing.Color.White;
            this.dataGridNG.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridNG.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Date,
            this.Time,
            this.Site,
            this.Array,
            this.Barcode,
            this.DefectCode,
            this.DefectName});
            this.dataGridNG.Location = new System.Drawing.Point(17, 87);
            this.dataGridNG.Margin = new System.Windows.Forms.Padding(0);
            this.dataGridNG.MultiSelect = false;
            this.dataGridNG.Name = "dataGridNG";
            this.dataGridNG.ReadOnly = true;
            this.dataGridNG.RowHeadersVisible = false;
            this.dataGridNG.RowTemplate.Height = 23;
            this.dataGridNG.Size = new System.Drawing.Size(786, 552);
            this.dataGridNG.TabIndex = 145;
            this.dataGridNG.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dataGridNG_SortCompare);
            this.dataGridNG.Click += new System.EventHandler(this.dataGridNG_Click);
            // 
            // Date
            // 
            this.Date.HeaderText = "Date";
            this.Date.Name = "Date";
            this.Date.ReadOnly = true;
            // 
            // Time
            // 
            this.Time.HeaderText = "Time";
            this.Time.Name = "Time";
            this.Time.ReadOnly = true;
            this.Time.Width = 80;
            // 
            // Site
            // 
            this.Site.HeaderText = "Site";
            this.Site.Name = "Site";
            this.Site.ReadOnly = true;
            this.Site.Width = 60;
            // 
            // Array
            // 
            this.Array.HeaderText = "Array";
            this.Array.Name = "Array";
            this.Array.ReadOnly = true;
            this.Array.Width = 60;
            // 
            // Barcode
            // 
            this.Barcode.HeaderText = "Barcode";
            this.Barcode.Name = "Barcode";
            this.Barcode.ReadOnly = true;
            this.Barcode.Width = 200;
            // 
            // DefectCode
            // 
            this.DefectCode.HeaderText = "Code";
            this.DefectCode.Name = "DefectCode";
            this.DefectCode.ReadOnly = true;
            this.DefectCode.Width = 60;
            // 
            // DefectName
            // 
            this.DefectName.HeaderText = "Defect Name";
            this.DefectName.Name = "DefectName";
            this.DefectName.ReadOnly = true;
            this.DefectName.Width = 200;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // NGDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Turquoise;
            this.ClientSize = new System.Drawing.Size(822, 716);
            this.Controls.Add(this.dataGridNG);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblSiteName);
            this.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.ForeColor = System.Drawing.SystemColors.InfoText;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NGDetail";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Test History";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.SiteDetail_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridNG)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblSiteName;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridView dataGridNG;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn Time;
        private System.Windows.Forms.DataGridViewTextBoxColumn Site;
        private System.Windows.Forms.DataGridViewTextBoxColumn Array;
        private System.Windows.Forms.DataGridViewTextBoxColumn Barcode;
        private System.Windows.Forms.DataGridViewTextBoxColumn DefectCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn DefectName;
        private System.Windows.Forms.Timer timer1;
    }
}