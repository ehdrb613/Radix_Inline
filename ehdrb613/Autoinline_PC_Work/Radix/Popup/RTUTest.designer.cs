namespace Radix
{
    partial class RTUTest
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
            System.Windows.Forms.Button button9;
            System.Windows.Forms.Button button7;
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.Baud_CB = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.Port_CB = new System.Windows.Forms.ComboBox();
            this.INC_START_btn = new System.Windows.Forms.Button();
            this.ABS_START_btn = new System.Windows.Forms.Button();
            this.INC_Y_Textbox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.INC_X_Textbox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.ABS_Y_Textbox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.ABS_X_Textbox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SpeedY_Textbox = new System.Windows.Forms.TextBox();
            this.SpeedX_Textbox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.AxisList = new System.Windows.Forms.ComboBox();
            this.button8 = new System.Windows.Forms.Button();
            this.PortStateTextbox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.GetposTextbox_Y = new System.Windows.Forms.TextBox();
            this.button5 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.Open_Btn = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.GetposTextbox_X = new System.Windows.Forms.TextBox();
            this.button6 = new System.Windows.Forms.Button();
            button9 = new System.Windows.Forms.Button();
            button7 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button9
            // 
            button9.Location = new System.Drawing.Point(518, 374);
            button9.Name = "button9";
            button9.Size = new System.Drawing.Size(75, 23);
            button9.TabIndex = 49;
            button9.Text = "Clear Pos ";
            button9.UseVisualStyleBackColor = true;
            button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button7
            // 
            button7.Location = new System.Drawing.Point(518, 201);
            button7.Name = "button7";
            button7.Size = new System.Drawing.Size(75, 23);
            button7.TabIndex = 44;
            button7.Text = "Reset";
            button7.UseVisualStyleBackColor = true;
            button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label12.Location = new System.Drawing.Point(28, 139);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(73, 14);
            this.label12.TabIndex = 70;
            this.label12.Text = "Axis List";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("굴림", 20F, System.Drawing.FontStyle.Bold);
            this.label11.Location = new System.Drawing.Point(182, 22);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(339, 27);
            this.label11.TabIndex = 69;
            this.label11.Text = "<PMC Sample Program>";
            // 
            // Baud_CB
            // 
            this.Baud_CB.FormattingEnabled = true;
            this.Baud_CB.Location = new System.Drawing.Point(245, 80);
            this.Baud_CB.Name = "Baud_CB";
            this.Baud_CB.Size = new System.Drawing.Size(89, 20);
            this.Baud_CB.TabIndex = 68;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label10.Location = new System.Drawing.Point(180, 83);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(57, 14);
            this.label10.TabIndex = 67;
            this.label10.Text = "Baud :";
            // 
            // Port_CB
            // 
            this.Port_CB.FormattingEnabled = true;
            this.Port_CB.Location = new System.Drawing.Point(83, 80);
            this.Port_CB.Name = "Port_CB";
            this.Port_CB.Size = new System.Drawing.Size(89, 20);
            this.Port_CB.TabIndex = 66;
            // 
            // INC_START_btn
            // 
            this.INC_START_btn.Location = new System.Drawing.Point(437, 310);
            this.INC_START_btn.Name = "INC_START_btn";
            this.INC_START_btn.Size = new System.Drawing.Size(75, 23);
            this.INC_START_btn.TabIndex = 64;
            this.INC_START_btn.Text = "INC Start";
            this.INC_START_btn.UseVisualStyleBackColor = true;
            this.INC_START_btn.Click += new System.EventHandler(this.INC_START_btn_Click);
            // 
            // ABS_START_btn
            // 
            this.ABS_START_btn.Location = new System.Drawing.Point(437, 246);
            this.ABS_START_btn.Name = "ABS_START_btn";
            this.ABS_START_btn.Size = new System.Drawing.Size(75, 23);
            this.ABS_START_btn.TabIndex = 65;
            this.ABS_START_btn.Text = "ABS Start";
            this.ABS_START_btn.UseVisualStyleBackColor = true;
            this.ABS_START_btn.Click += new System.EventHandler(this.ABS_START_btn_Click);
            // 
            // INC_Y_Textbox
            // 
            this.INC_Y_Textbox.Location = new System.Drawing.Point(332, 310);
            this.INC_Y_Textbox.Name = "INC_Y_Textbox";
            this.INC_Y_Textbox.Size = new System.Drawing.Size(100, 21);
            this.INC_Y_Textbox.TabIndex = 63;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(241, 314);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 12);
            this.label7.TabIndex = 62;
            this.label7.Text = "Y INC PULSE";
            // 
            // INC_X_Textbox
            // 
            this.INC_X_Textbox.Location = new System.Drawing.Point(121, 310);
            this.INC_X_Textbox.Name = "INC_X_Textbox";
            this.INC_X_Textbox.Size = new System.Drawing.Size(100, 21);
            this.INC_X_Textbox.TabIndex = 61;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(30, 314);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(81, 12);
            this.label8.TabIndex = 60;
            this.label8.Text = "X INC PULSE";
            // 
            // ABS_Y_Textbox
            // 
            this.ABS_Y_Textbox.Location = new System.Drawing.Point(332, 246);
            this.ABS_Y_Textbox.Name = "ABS_Y_Textbox";
            this.ABS_Y_Textbox.Size = new System.Drawing.Size(100, 21);
            this.ABS_Y_Textbox.TabIndex = 59;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(241, 251);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 12);
            this.label6.TabIndex = 58;
            this.label6.Text = "Y ABS PULSE";
            // 
            // ABS_X_Textbox
            // 
            this.ABS_X_Textbox.Location = new System.Drawing.Point(121, 246);
            this.ABS_X_Textbox.Name = "ABS_X_Textbox";
            this.ABS_X_Textbox.Size = new System.Drawing.Size(100, 21);
            this.ABS_X_Textbox.TabIndex = 57;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(30, 251);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 12);
            this.label5.TabIndex = 56;
            this.label5.Text = "X ABS PULSE";
            // 
            // SpeedY_Textbox
            // 
            this.SpeedY_Textbox.Location = new System.Drawing.Point(332, 172);
            this.SpeedY_Textbox.Name = "SpeedY_Textbox";
            this.SpeedY_Textbox.Size = new System.Drawing.Size(100, 21);
            this.SpeedY_Textbox.TabIndex = 55;
            // 
            // SpeedX_Textbox
            // 
            this.SpeedX_Textbox.Location = new System.Drawing.Point(121, 172);
            this.SpeedX_Textbox.Name = "SpeedX_Textbox";
            this.SpeedX_Textbox.Size = new System.Drawing.Size(100, 21);
            this.SpeedX_Textbox.TabIndex = 54;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(241, 178);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 12);
            this.label4.TabIndex = 53;
            this.label4.Text = "Y Drive Speed";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label9.Location = new System.Drawing.Point(25, 83);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(50, 14);
            this.label9.TabIndex = 51;
            this.label9.Text = "Port :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 178);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 12);
            this.label3.TabIndex = 52;
            this.label3.Text = "X Drive Speed";
            // 
            // AxisList
            // 
            this.AxisList.FormattingEnabled = true;
            this.AxisList.Location = new System.Drawing.Point(107, 136);
            this.AxisList.Name = "AxisList";
            this.AxisList.Size = new System.Drawing.Size(75, 20);
            this.AxisList.TabIndex = 50;
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(518, 172);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(75, 23);
            this.button8.TabIndex = 48;
            this.button8.Text = "STOP";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // PortStateTextbox
            // 
            this.PortStateTextbox.Location = new System.Drawing.Point(508, 80);
            this.PortStateTextbox.Name = "PortStateTextbox";
            this.PortStateTextbox.ReadOnly = true;
            this.PortStateTextbox.Size = new System.Drawing.Size(163, 21);
            this.PortStateTextbox.TabIndex = 47;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(241, 377);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 12);
            this.label2.TabIndex = 46;
            this.label2.Text = "Y Current POS";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 377);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 12);
            this.label1.TabIndex = 45;
            this.label1.Text = "X Current POS";
            // 
            // GetposTextbox_Y
            // 
            this.GetposTextbox_Y.Location = new System.Drawing.Point(332, 374);
            this.GetposTextbox_Y.Name = "GetposTextbox_Y";
            this.GetposTextbox_Y.ReadOnly = true;
            this.GetposTextbox_Y.Size = new System.Drawing.Size(100, 21);
            this.GetposTextbox_Y.TabIndex = 43;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(599, 172);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 40;
            this.button5.Text = "CW";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(437, 172);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 39;
            this.button3.Text = "CCW";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(425, 79);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 38;
            this.button2.Text = "Close";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Open_Btn
            // 
            this.Open_Btn.Location = new System.Drawing.Point(342, 79);
            this.Open_Btn.Name = "Open_Btn";
            this.Open_Btn.Size = new System.Drawing.Size(75, 23);
            this.Open_Btn.TabIndex = 37;
            this.Open_Btn.Text = "OPEN";
            this.Open_Btn.UseVisualStyleBackColor = true;
            this.Open_Btn.Click += new System.EventHandler(this.Open_Btn_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // GetposTextbox_X
            // 
            this.GetposTextbox_X.Location = new System.Drawing.Point(121, 374);
            this.GetposTextbox_X.Name = "GetposTextbox_X";
            this.GetposTextbox_X.ReadOnly = true;
            this.GetposTextbox_X.Size = new System.Drawing.Size(100, 21);
            this.GetposTextbox_X.TabIndex = 42;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(437, 374);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(75, 23);
            this.button6.TabIndex = 41;
            this.button6.Text = "Get Pos";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // RTUTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(731, 444);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.Baud_CB);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.Port_CB);
            this.Controls.Add(this.INC_START_btn);
            this.Controls.Add(this.ABS_START_btn);
            this.Controls.Add(this.INC_Y_Textbox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.INC_X_Textbox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.ABS_Y_Textbox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.ABS_X_Textbox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.SpeedY_Textbox);
            this.Controls.Add(this.SpeedX_Textbox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.AxisList);
            this.Controls.Add(button9);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.PortStateTextbox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(button7);
            this.Controls.Add(this.GetposTextbox_Y);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.Open_Btn);
            this.Controls.Add(this.GetposTextbox_X);
            this.Controls.Add(this.button6);
            this.Name = "RTUTest";
            this.Text = "RTUTest";
            this.Load += new System.EventHandler(this.RTUTest_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox Baud_CB;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox Port_CB;
        private System.Windows.Forms.Button INC_START_btn;
        private System.Windows.Forms.Button ABS_START_btn;
        private System.Windows.Forms.TextBox INC_Y_Textbox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox INC_X_Textbox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox ABS_Y_Textbox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox ABS_X_Textbox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox SpeedY_Textbox;
        private System.Windows.Forms.TextBox SpeedX_Textbox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox AxisList;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.TextBox PortStateTextbox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox GetposTextbox_Y;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button Open_Btn;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox GetposTextbox_X;
        private System.Windows.Forms.Button button6;
    }
}