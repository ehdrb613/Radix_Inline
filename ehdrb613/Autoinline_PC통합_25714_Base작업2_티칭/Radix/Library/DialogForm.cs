using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;

namespace Radix
{

    // Install-Package WindowsAPICodePack --> 비정상
    // Install-Package WindowsAPICodePack-Shell -Version 1.1.1 --> 정상

    /**
     * @brief 다양한 형식의 다이얼로그창 출력 클래스
     *      NuGet 패키지 설치 필요 : Install-Package WindowsAPICodePack-Shell -Version 1.1.1
     */
    public class DialogForm
    {
        /**
         * @brief 다이얼로그 본문에 출력할 문자열
         */
        private string text = "";
        /**
         * @brief 다이얼로그 캡션에 출력할 문자열
         */
        private string caption = "";
        /**
         * @brief Yes/No 버튼 표시 여부
         */
        private bool yesNo = false;
        /**
         * @brief 창 닫힐 때 Yes 버튼 클릭 여부
         */
        public bool YesClick = false;

        /**
         * @brief 로컬 디버그 출력 함수
         * @param str 디버그 처리할 문자열
         */
        private static void debug(string str)
        {
            Util.Debug("DialogForm : " + str);
        }

        /**
         * @brief 다이얼로그 폼 생성자
         * @param t 타이틀 문자열
         * @param c 캡션 문자열
         * @param yn Yes/No 버튼 표시 유무
         */
        public DialogForm(string t, string c, bool yn)
        {
            text = t;
            caption = c;
            yesNo = yn;
        }

        /**
         * @brief OK만 있는 다이얼로그 폼 을 띄운다.
         * @return void
         */
        public void ShowOk() // OK만 있는 창
        {
            // 기존 열려진 창이 있으면 닫는다
            try
            {
                //var dialog = new TaskDialog
                var task = new TaskDialog
                {
                    Caption = caption,
                    //InstructionText = text,
                    //Icon = TaskDialogStandardIcon.Information,
                    Cancelable = true,
                    StartupLocation = TaskDialogStartupLocation.CenterOwner
                };

                Form activeform = Form.ActiveForm;
                if (activeform == null ||
                   activeform.Handle == null ||
                   activeform.Handle == IntPtr.Zero)
                {

                    task.OwnerWindowHandle = (new Form()).Handle;
                }
                else
                {
                    task.OwnerWindowHandle = activeform.Handle;
                }

                task.Opened += (s1, e1) => { task.Icon = TaskDialogStandardIcon.Information; task.InstructionText = text; };

                task.Opened += (senderObject, ea) =>
                {
                    var taskDialog = senderObject as TaskDialog;
                    taskDialog.Icon = taskDialog.Icon;
                };

                var yesButton = new TaskDialogButton("CloseTaskDialogButton", "OK")
                {
                    Default = true
                };

                task.Controls.Add(yesButton);

                yesButton.Click += (e, a) =>
                {
                    //Console.WriteLine("Yes");
                    task.Close();
                };


                task.Show();
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        /**
         * @brief YES/NO 요구하는 메시지창 을 띄운다.
         * @return void
         */
        public void ShowYesNo()
        {
            var ui = Application.OpenForms.Cast<Form>().FirstOrDefault();
            if (ui != null && ui.InvokeRequired) { ui.Invoke((Action)ShowYesNo); return; }

            try
            {
                using (var task = new TaskDialog
                {
                    Caption = caption,
                    InstructionText = text,
                    Icon = TaskDialogStandardIcon.Information,
                    Cancelable = false,
                    // ⬇️ 커스텀 버튼 대신 표준 버튼 사용
                    StandardButtons = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No
                })
                {
                    var owner = Form.ActiveForm ?? Application.OpenForms.Cast<Form>().FirstOrDefault(f => f.Visible);
                    if (owner != null && owner.IsHandleCreated)
                    {
                        task.OwnerWindowHandle = owner.Handle;
                        task.StartupLocation = TaskDialogStartupLocation.CenterOwner;
                    }
                    else
                    {
                        task.StartupLocation = TaskDialogStartupLocation.CenterScreen;
                    }

                    var result = task.Show();                // OS 표준 Yes/No 표시
                    YesClick = (result == TaskDialogResult.Yes);
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
                // 급한 우회: 표준 MessageBox로 폴백
                var owner = Form.ActiveForm ?? Application.OpenForms.Cast<Form>().FirstOrDefault(f => f.Visible);
                var r = owner != null
                        ? MessageBox.Show(owner, text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Information)
                        : MessageBox.Show(text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                YesClick = (r == DialogResult.Yes);
            }
        }



    }
}