namespace Radix
{
    partial class test
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
            this.cbArray = new System.Windows.Forms.CheckBox();
            this.cbFront = new System.Windows.Forms.CheckBox();
            this.cbOk = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.tbCode = new System.Windows.Forms.TextBox();
            this.numIndex = new System.Windows.Forms.NumericUpDown();
            this.btnMessage = new System.Windows.Forms.Button();
            this.tbMessage = new System.Windows.Forms.TextBox();
            this.numMessage = new System.Windows.Forms.NumericUpDown();
            this.cbMessage = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMessage)).BeginInit();
            this.SuspendLayout();
            // 
            // cbArray
            // 
            this.cbArray.AutoSize = true;
            this.cbArray.Location = new System.Drawing.Point(31, 27);
            this.cbArray.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cbArray.Name = "cbArray";
            this.cbArray.Size = new System.Drawing.Size(68, 16);
            this.cbArray.TabIndex = 0;
            this.cbArray.Text = "is Array";
            this.cbArray.UseVisualStyleBackColor = true;
            // 
            // cbFront
            // 
            this.cbFront.AutoSize = true;
            this.cbFront.Location = new System.Drawing.Point(31, 94);
            this.cbFront.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cbFront.Name = "cbFront";
            this.cbFront.Size = new System.Drawing.Size(66, 16);
            this.cbFront.TabIndex = 0;
            this.cbFront.Text = "is Front";
            this.cbFront.UseVisualStyleBackColor = true;
            // 
            // cbOk
            // 
            this.cbOk.AutoSize = true;
            this.cbOk.Location = new System.Drawing.Point(31, 161);
            this.cbOk.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cbOk.Name = "cbOk";
            this.cbOk.Size = new System.Drawing.Size(55, 16);
            this.cbOk.TabIndex = 0;
            this.cbOk.Text = "is OK";
            this.cbOk.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(283, 159);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(66, 18);
            this.button1.TabIndex = 1;
            this.button1.Text = "up count";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tbCode
            // 
            this.tbCode.Location = new System.Drawing.Point(31, 125);
            this.tbCode.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tbCode.Name = "tbCode";
            this.tbCode.Size = new System.Drawing.Size(204, 21);
            this.tbCode.TabIndex = 2;
            this.tbCode.Text = "1234567890123";
            // 
            // numIndex
            // 
            this.numIndex.Location = new System.Drawing.Point(31, 58);
            this.numIndex.Name = "numIndex";
            this.numIndex.Size = new System.Drawing.Size(120, 21);
            this.numIndex.TabIndex = 3;
            // 
            // btnMessage
            // 
            this.btnMessage.Location = new System.Drawing.Point(543, 48);
            this.btnMessage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnMessage.Name = "btnMessage";
            this.btnMessage.Size = new System.Drawing.Size(66, 18);
            this.btnMessage.TabIndex = 1;
            this.btnMessage.Text = "messagebox";
            this.btnMessage.UseVisualStyleBackColor = true;
            this.btnMessage.Click += new System.EventHandler(this.btnMessage_Click);
            // 
            // tbMessage
            // 
            this.tbMessage.Location = new System.Drawing.Point(320, 45);
            this.tbMessage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tbMessage.Name = "tbMessage";
            this.tbMessage.Size = new System.Drawing.Size(143, 21);
            this.tbMessage.TabIndex = 2;
            this.tbMessage.Text = "message test";
            // 
            // numMessage
            // 
            this.numMessage.Location = new System.Drawing.Point(469, 45);
            this.numMessage.Name = "numMessage";
            this.numMessage.Size = new System.Drawing.Size(68, 21);
            this.numMessage.TabIndex = 3;
            this.numMessage.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // cbMessage
            // 
            this.cbMessage.AutoSize = true;
            this.cbMessage.Location = new System.Drawing.Point(616, 49);
            this.cbMessage.Name = "cbMessage";
            this.cbMessage.Size = new System.Drawing.Size(52, 16);
            this.cbMessage.TabIndex = 4;
            this.cbMessage.Text = "done";
            this.cbMessage.UseVisualStyleBackColor = true;
            // 
            // test
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(670, 470);
            this.Controls.Add(this.cbMessage);
            this.Controls.Add(this.numMessage);
            this.Controls.Add(this.numIndex);
            this.Controls.Add(this.tbMessage);
            this.Controls.Add(this.tbCode);
            this.Controls.Add(this.btnMessage);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cbOk);
            this.Controls.Add(this.cbFront);
            this.Controls.Add(this.cbArray);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "test";
            this.Text = "test";
            ((System.ComponentModel.ISupportInitialize)(this.numIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMessage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbArray;
        private System.Windows.Forms.CheckBox cbFront;
        private System.Windows.Forms.CheckBox cbOk;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tbCode;
        private System.Windows.Forms.NumericUpDown numIndex;
        private System.Windows.Forms.Button btnMessage;
        private System.Windows.Forms.TextBox tbMessage;
        private System.Windows.Forms.NumericUpDown numMessage;
        private System.Windows.Forms.CheckBox cbMessage;
    }
}