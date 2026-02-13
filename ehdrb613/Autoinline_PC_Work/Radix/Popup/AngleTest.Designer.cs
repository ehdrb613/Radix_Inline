namespace Radix
{
    partial class AngleTest
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
            this.numCenterX = new System.Windows.Forms.NumericUpDown();
            this.numCenterY = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numVisionX = new System.Windows.Forms.NumericUpDown();
            this.numVisionY = new System.Windows.Forms.NumericUpDown();
            this.button1 = new System.Windows.Forms.Button();
            this.lblAngle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numCenterX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCenterY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVisionX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVisionY)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Center";
            // 
            // numCenterX
            // 
            this.numCenterX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numCenterX.Location = new System.Drawing.Point(138, 33);
            this.numCenterX.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numCenterX.Name = "numCenterX";
            this.numCenterX.Size = new System.Drawing.Size(88, 25);
            this.numCenterX.TabIndex = 1;
            // 
            // numCenterY
            // 
            this.numCenterY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numCenterY.Location = new System.Drawing.Point(232, 33);
            this.numCenterY.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numCenterY.Name = "numCenterY";
            this.numCenterY.Size = new System.Drawing.Size(88, 25);
            this.numCenterY.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(35, 123);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "Vision Point";
            // 
            // numVisionX
            // 
            this.numVisionX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numVisionX.Location = new System.Drawing.Point(138, 121);
            this.numVisionX.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numVisionX.Name = "numVisionX";
            this.numVisionX.Size = new System.Drawing.Size(88, 25);
            this.numVisionX.TabIndex = 1;
            // 
            // numVisionY
            // 
            this.numVisionY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numVisionY.Location = new System.Drawing.Point(232, 121);
            this.numVisionY.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numVisionY.Name = "numVisionY";
            this.numVisionY.Size = new System.Drawing.Size(88, 25);
            this.numVisionY.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(461, 50);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "각도계산";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblAngle
            // 
            this.lblAngle.AutoSize = true;
            this.lblAngle.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblAngle.Location = new System.Drawing.Point(457, 100);
            this.lblAngle.Name = "lblAngle";
            this.lblAngle.Size = new System.Drawing.Size(20, 20);
            this.lblAngle.TabIndex = 0;
            this.lblAngle.Text = "0";
            // 
            // AngleTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(648, 508);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.numVisionY);
            this.Controls.Add(this.numVisionX);
            this.Controls.Add(this.numCenterY);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numCenterX);
            this.Controls.Add(this.lblAngle);
            this.Controls.Add(this.label1);
            this.Name = "AngleTest";
            this.Text = "AngleTest";
            ((System.ComponentModel.ISupportInitialize)(this.numCenterX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCenterY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVisionX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVisionY)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numCenterX;
        private System.Windows.Forms.NumericUpDown numCenterY;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numVisionX;
        private System.Windows.Forms.NumericUpDown numVisionY;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lblAngle;
    }
}