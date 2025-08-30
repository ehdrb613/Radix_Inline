using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Radix
{

    // Install-Package WindowsAPICodePack --> 비정상
    // Install-Package WindowsAPICodePack-Shell -Version 1.1.1 --> 정상

    /**
     * @brief 자동 닫기 등의 기능을 가진 메시지 다이얼로그
     *      버그 해결 안 되었으므로 DialogForm을 사용할 것
     *      NuGet 패키지 설치 필요 : Install-Package WindowsAPICodePack-Shell -Version 1.1.1
     */
    public static class DialogMessage
    {
        /**
         * @brief 다이얼로그 창 처리 Thread Timer
         */
        private static System.Threading.Timer timerDialog = null; // Thread Timer
        /**
         * @brief 메시지창 처리 Thread Timer
         */
        private static System.Threading.Timer timerMessage = null; // Thread Timer
        /**
         * @brief 다이얼로그 창 감시 타이머
         */
        private static Stopwatch watchDialog = new Stopwatch();
        /**
         * @brief 메시지 창 감시 타이머
         */
        private static Stopwatch watchMessage = new Stopwatch();

        /**
         * @brief 로컬 디버그 출력 함수
         * @param str 디버그 처리할 문자열
         */
        private static void debug(string str)
        {
            Util.Debug("DialogMessage : " + str);
        }
        /**
         * @brief 메시지창 자동닫기 쓰레드 타이머 함수
         * @param state 
         */
        private static void TimerMessage(Object state) // 메시지창 자동닫기 쓰레드 타이머 함수
        {
            try
            {
                if (watchMessage == null ||
                    !watchMessage.IsRunning ||
                    watchMessage.ElapsedMilliseconds > 10 * 1000)
                {
                    timerMessage.Dispose();
                    if (GlobalVar.messageTask != null)
                    {
                        GlobalVar.messageTask.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        /**
         * @brief YES/NO 요구하지 않는 메시지창 출력
         * @param text 메시지창에 본문에 출력할 문자열
         * @param caption 메시지창 캡션에 출력할 문자열
         */
        public static void MessageTask(string text, string caption) // YES/NO 요구하지 않는 메시지창
        {
            // 기존 열려진 창이 있으면 닫는다
            try
            {
                if (GlobalVar.messageTask != null)
                {
                    GlobalVar.messageTask.Close();
                }
                if (timerMessage != null)
                {
                    timerMessage.Dispose();
                }
                Util.StartWatch(ref watchMessage);

                var yesButton = new TaskDialogButton("CloseTaskDialogButton", "OK")
                {
                    Default = true
                };

                //var dialog = new TaskDialog
                GlobalVar.messageTask = new TaskDialog
                {
                    Caption = caption,
                    //InstructionText = text,
                    //Icon = TaskDialogStandardIcon.Information,
                    Cancelable = true,
                    StartupLocation = TaskDialogStartupLocation.CenterOwner
                };
                GlobalVar.messageTask.Opened += (s1, e1) => { GlobalVar.messageTask.Icon = TaskDialogStandardIcon.Information; GlobalVar.messageTask.InstructionText = text; };

                GlobalVar.messageTask.Controls.Add(yesButton);

                GlobalVar.messageTask.OwnerWindowHandle = Form.ActiveForm.Handle;

                GlobalVar.messageTask.Opened += (senderObject, ea) =>
                {
                    var taskDialog = senderObject as TaskDialog;
                    taskDialog.Icon = taskDialog.Icon;
                };

                yesButton.Click += (e, a) =>
                {
                    //Console.WriteLine("Yes");
                    GlobalVar.messageTask.Close();
                    GlobalVar.messageTask = null;
                };

                //TaskDialogPage page = new TaskDialogPage()
                //{
                //    Caption = caption,
                //    SizeToContent = true,
                //    Heading = heading,
                //    Icon = TaskDialogIcon.Information,
                //    Buttons = buttons
                //};

                //timerMessage = new System.Threading.Timer(new TimerCallback(TimerMessage), false, 0, 100);

                GlobalVar.messageTask.Show();
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        /**
         * @brief YES/NO 요구하는 메시지창 출력
         * @param text 메시지창에 본문에 출력할 문자열
         * @param caption 메시지창 캡션에 출력할 문자열
         */
        public static void DialogTask(string text, string caption) // YES/NO 요구하는 메시지창
        {
            // 기존 열려진 창이 있으면 닫는다
            try
            {
                if (GlobalVar.dialogTask != null)
                {
                    GlobalVar.dialogTask.Close();
                }
                if (timerDialog != null)
                {
                    timerDialog.Dispose();
                }
                Util.StartWatch(ref watchDialog);


                var yesButton = new TaskDialogButton("CloseTaskDialogButton", "Yes")
                {
                    Default = true
                };
                var noButton = new TaskDialogButton("ProceedTaskDialogButton", "No");

                //var dialog = new TaskDialog
                GlobalVar.dialogTask = new TaskDialog
                {
                    Caption = caption,
                    //InstructionText = text,
                    //Icon = TaskDialogStandardIcon.Information,
                    Cancelable = false,
                    StartupLocation = TaskDialogStartupLocation.CenterOwner
                };
                GlobalVar.dialogTask.Opened += (s1, e1) => { GlobalVar.dialogTask.Icon = TaskDialogStandardIcon.Information; GlobalVar.dialogTask.InstructionText = text; };

                GlobalVar.dialogTask.Controls.Add(yesButton);
                GlobalVar.dialogTask.Controls.Add(noButton);

                //GlobalVar.dialogTask.OwnerWindowHandle = Form.ActiveForm.Handle;
                //초반에 안열리는 경우 있어서 변경함
                if (Form.ActiveForm == null ||
                    !Form.ActiveForm.IsAccessible ||
                    Form.ActiveForm.Handle == null ||
                    Form.ActiveForm.Handle == IntPtr.Zero)
                {
                    GlobalVar.dialogTask.OwnerWindowHandle = (new Form()).Handle;
                }
                else
                {
                    GlobalVar.dialogTask.OwnerWindowHandle = Form.ActiveForm.Handle;
                }



                GlobalVar.dialogTask.Opened += (senderObject, ea) =>
                {
                    var taskDialog = senderObject as TaskDialog;
                    taskDialog.Icon = taskDialog.Icon;
                };

                yesButton.Click += (e, a) =>
                {
                    //Console.WriteLine("Yes");
                    GlobalVar.MessageOK = true;
                    GlobalVar.MessageOKClick = true;
                    GlobalVar.dialogTask.Close(TaskDialogResult.Close);
                    GlobalVar.dialogTask = null;
                    //timerDialog.Dispose();
                };

                noButton.Click += (e, a) =>
                {
                    //Console.WriteLine("No");
                    GlobalVar.MessageOK = false;
                    GlobalVar.MessageOKClick = true;
                    GlobalVar.dialogTask.Close(TaskDialogResult.Close);
                    GlobalVar.dialogTask = null;
                    //timerDialog.Dispose();
                };

                //timerDialog = new System.Threading.Timer(new TimerCallback(TimerDialog), false, 0, 100);

                GlobalVar.dialogTask.Show();
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        /**
         * @brief Dialog 자동닫기 쓰레드 타이머 함수
         * @param state
         */
        private static void TimerDialog(Object state) // Dialog 자동닫기 쓰레드 타이머 함수
        {
            try
            {
                if (watchDialog == null ||
                    !watchDialog.IsRunning ||
                    watchDialog.ElapsedMilliseconds > 10 * 1000)
                {
                    timerDialog.Dispose();
                    if (GlobalVar.dialogTask != null)
                    {
                        GlobalVar.dialogTask.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }


    }
}