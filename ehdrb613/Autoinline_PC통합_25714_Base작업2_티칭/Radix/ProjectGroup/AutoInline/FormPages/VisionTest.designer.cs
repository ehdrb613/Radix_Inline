namespace Radix
{
    partial class VisionTest
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
            this.btnVisionCheck = new System.Windows.Forms.Button();
            this.tbVisionData = new System.Windows.Forms.TextBox();
            this.numX = new System.Windows.Forms.NumericUpDown();
            this.numY = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numY)).BeginInit();
            this.SuspendLayout();
            // 
            // btnVisionCheck
            // 
            this.btnVisionCheck.Location = new System.Drawing.Point(94, 23);
            this.btnVisionCheck.Name = "btnVisionCheck";
            this.btnVisionCheck.Size = new System.Drawing.Size(83, 40);
            this.btnVisionCheck.TabIndex = 0;
            this.btnVisionCheck.Text = "Vision Check";
            this.btnVisionCheck.UseVisualStyleBackColor = true;
            this.btnVisionCheck.Click += new System.EventHandler(this.btnVisionCheck_Click);
            // 
            // tbVisionData
            // 
            this.tbVisionData.Location = new System.Drawing.Point(52, 83);
            this.tbVisionData.Name = "tbVisionData";
            this.tbVisionData.Size = new System.Drawing.Size(176, 21);
            this.tbVisionData.TabIndex = 1;
            // 
            // numX
            // 
            this.numX.DecimalPlaces = 3;
            this.numX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numX.Location = new System.Drawing.Point(52, 124);
            this.numX.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numX.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.numX.Name = "numX";
            this.numX.ReadOnly = true;
            this.numX.Size = new System.Drawing.Size(88, 21);
            this.numX.TabIndex = 2;
            // 
            // numY
            // 
            this.numY.DecimalPlaces = 3;
            this.numY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numY.Location = new System.Drawing.Point(52, 151);
            this.numY.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numY.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.numY.Name = "numY";
            this.numY.ReadOnly = true;
            this.numY.Size = new System.Drawing.Size(88, 21);
            this.numY.TabIndex = 2;
            // 
            // VisionTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.numY);
            this.Controls.Add(this.numX);
            this.Controls.Add(this.tbVisionData);
            this.Controls.Add(this.btnVisionCheck);
            this.Name = "VisionTest";
            this.Text = "VisionTest";
            ((System.ComponentModel.ISupportInitialize)(this.numX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numY)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnVisionCheck;
        private System.Windows.Forms.TextBox tbVisionData;
        private System.Windows.Forms.NumericUpDown numX;
        private System.Windows.Forms.NumericUpDown numY;
    }
}