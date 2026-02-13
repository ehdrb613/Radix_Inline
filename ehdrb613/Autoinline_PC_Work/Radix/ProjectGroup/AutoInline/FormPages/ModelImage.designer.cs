namespace Radix
{
    partial class ModelImage
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
            this.pbImage = new System.Windows.Forms.PictureBox();
            this.btnMake = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).BeginInit();
            this.SuspendLayout();
            // 
            // pbImage
            // 
            this.pbImage.Location = new System.Drawing.Point(0, -1);
            this.pbImage.Name = "pbImage";
            this.pbImage.Size = new System.Drawing.Size(1184, 863);
            this.pbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbImage.TabIndex = 0;
            this.pbImage.TabStop = false;
            this.pbImage.Paint += new System.Windows.Forms.PaintEventHandler(this.pbImage_Paint);
            this.pbImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbImage_MouseDown);
            this.pbImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbImage_MouseMove);
            this.pbImage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbImage_MouseUp);
            // 
            // btnMake
            // 
            this.btnMake.BackColor = System.Drawing.Color.White;
            this.btnMake.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnMake.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMake.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.btnMake.ForeColor = System.Drawing.Color.Black;
            this.btnMake.Location = new System.Drawing.Point(467, 872);
            this.btnMake.Margin = new System.Windows.Forms.Padding(2);
            this.btnMake.Name = "btnMake";
            this.btnMake.Size = new System.Drawing.Size(253, 54);
            this.btnMake.TabIndex = 551;
            this.btnMake.Text = "Make Model Image";
            this.btnMake.UseVisualStyleBackColor = false;
            this.btnMake.Click += new System.EventHandler(this.btnMake_Click);
            // 
            // ModelImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Turquoise;
            this.ClientSize = new System.Drawing.Size(1184, 932);
            this.Controls.Add(this.btnMake);
            this.Controls.Add(this.pbImage);
            this.Name = "ModelImage";
            this.Text = "ModelImage";
            this.Shown += new System.EventHandler(this.ModelImage_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbImage;
        private System.Windows.Forms.Button btnMake;
    }
}