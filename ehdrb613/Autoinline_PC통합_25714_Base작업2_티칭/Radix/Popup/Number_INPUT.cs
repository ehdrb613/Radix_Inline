using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent; // ConcurrentQueue
using System.IO;

namespace Radix
{
    public partial class Number_INPUT : Form
    {
        public string ReturnValue { get; set; } // 부모 폼으로 보낼 값

        public Number_INPUT(double numerValue)
        {
            InitializeComponent();
            CreateNumberButtons();

            // 전달된 초기값을 TextBox에 설정
            txtInput.Text = numerValue.ToString();
        }
        public Number_INPUT()
        {
            InitializeComponent();
            CreateNumberButtons();

        }

        // 폼 로드 시 호출되는 메소드
        private void Number_INPUT_Load(object sender, EventArgs e)
        {
            // 폼 설정
            this.Text = "숫자 입력기";

            // TextBox에서 숫자만 입력되도록 KeyPress 이벤트 추가
            txtInput.KeyPress += TxtInput_KeyPress;
        }

        // TextBox에서 숫자와 백스페이스만 허용
        private void TxtInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 숫자, 백스페이스, 소수점 허용
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar != '.')
            {
                e.Handled = true; // 입력 무효화
            }

            // 소수점이 두 번 이상 들어가지 않도록 처리
            if (e.KeyChar == '.' && txtInput.Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        // 숫자 버튼 동적 생성
        private void CreateNumberButtons()
        {
            int buttonWidth = 90;
            int buttonHeight = 90;
            int margin = 10;
            int startX = 50;
            int startY = 100;

            // TextBox 생성 및 설정
            txtInput = new TextBox();
            txtInput.Location = new System.Drawing.Point(startX, startY - 70);
            txtInput.Width = buttonWidth * 3 + margin * 2;
            txtInput.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            this.Controls.Add(txtInput);

            // 숫자 버튼 생성
            for (int i = 1; i <= 9; i++)
            {
                Button numButton = new Button();
                numButton.Text = i.ToString();
                numButton.Width = buttonWidth;
                numButton.Height = buttonHeight;
                numButton.BackColor = Color.White;
                numButton.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
                // 버튼의 위치 계산
                int row = (i - 1) / 3;
                int col = (i - 1) % 3;
                numButton.Location = new System.Drawing.Point(startX + col * (buttonWidth + margin), startY + row * (buttonHeight + margin));

                // 숫자 버튼 클릭 이벤트 추가
                numButton.Click += NumButton_Click;
                this.Controls.Add(numButton);
            }

            // 0 버튼 생성
            Button zeroButton = new Button();
            zeroButton.Text = "0";
            zeroButton.Width = buttonWidth;
            zeroButton.Height = buttonHeight;
            zeroButton.Location = new System.Drawing.Point(startX + buttonWidth + margin, startY + 3 * (buttonHeight + margin));
            zeroButton.BackColor = Color.White;

            // 0 버튼 클릭 이벤트 추가
            zeroButton.Click += NumButton_Click;
            this.Controls.Add(zeroButton);

            // 지우기 버튼 (Backspace) 생성
            Button backspaceButton = new Button();
            backspaceButton.Text = "지우기";
            backspaceButton.Width = buttonWidth;
            backspaceButton.Height = buttonHeight;
            backspaceButton.Location = new System.Drawing.Point(startX + 2 * (buttonWidth + margin), startY + 3 * (buttonHeight + margin));
            backspaceButton.BackColor = Color.White;
            // 지우기 버튼 클릭 이벤트 추가
            backspaceButton.Click += BackspaceButton_Click;
            this.Controls.Add(backspaceButton);
           
            // 소수점 버튼 생성
            Button decimalButton = new Button();
            decimalButton.Text = ".";
            decimalButton.Width = buttonWidth;
            decimalButton.Height = buttonHeight;
            decimalButton.Location = new System.Drawing.Point(startX, startY + 3 * (buttonHeight + margin));
            decimalButton.BackColor = Color.White;
            // 소수점 버튼 클릭 이벤트 추가
            decimalButton.Click += DecimalButton_Click;
            this.Controls.Add(decimalButton);

            // 확인 버튼 생성
            Button confirmButton = new Button();
            confirmButton.Text = "확인";
            confirmButton.Width = buttonWidth * 3 + margin * 2;
            confirmButton.Height = buttonHeight;
            confirmButton.Location = new System.Drawing.Point(startX, startY + 4 * (buttonHeight + margin));
            confirmButton.BackColor = Color.White;
            // 확인 버튼 클릭 이벤트 추가
            confirmButton.Click += ConfirmButton_Click;
            this.Controls.Add(confirmButton);
        }

        // 숫자 버튼 클릭 이벤트 핸들러
        private void NumButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            txtInput.Text += clickedButton.Text;
        }

        // 지우기 버튼 클릭 이벤트 핸들러
        private void BackspaceButton_Click(object sender, EventArgs e)
        {
            if (txtInput.Text.Length > 0)
            {
                txtInput.Text = txtInput.Text.Substring(0, txtInput.Text.Length - 1);
            }
        }

        // 소수점 버튼 클릭 이벤트 핸들러
        private void DecimalButton_Click(object sender, EventArgs e)
        {
            if (!txtInput.Text.Contains("."))
            {
                txtInput.Text += ".";
            }
        }

        // 확인 버튼 클릭 이벤트 핸들러
        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            ReturnValue = txtInput.Text; // 입력된 값을 저장
            this.DialogResult = DialogResult.OK; // 폼이 정상적으로 닫혔음을 알림
            this.Close();
        }

        // TextBox 및 폼 내 버튼 필드 선언
        private TextBox txtInput;

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}





