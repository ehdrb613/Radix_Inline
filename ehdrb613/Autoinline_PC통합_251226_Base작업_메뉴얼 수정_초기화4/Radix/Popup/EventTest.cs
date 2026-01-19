using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Radix
{
    public partial class EventTest : Form
    {
        private int num = 0;
        private bool up = false;
        private bool down = false;

        public EventTest()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (up)
            {
                num++;
            }
            if (down)
            {
                num--;
            }
            label1.Text = num.ToString();
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            up = true;
            down = false;
        }

        private void button2_MouseDown(object sender, MouseEventArgs e)
        {
            up = false;
            down = true;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            up = false;
            down = false;
        }

        private void button1_MouseUp(object sender, MouseEventArgs e)
        {
            up = false;
            down = false;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            up = false;
            down = false;
        }

        private void button2_MouseUp(object sender, MouseEventArgs e)
        {
            up = false;
            down = false;
        }
    }
}
