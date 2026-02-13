using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Radix
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            #region 중복실행 방지
            System.Diagnostics.Process[] processes = null;
            string strCurrentProcess = System.Diagnostics.Process.GetCurrentProcess().ProcessName.ToUpper();
            IntPtr intCurrentProcess = System.Diagnostics.Process.GetCurrentProcess().Handle;
            processes = System.Diagnostics.Process.GetProcessesByName(strCurrentProcess);
            if (processes.Length > 1)
            {
                FuncWin.TopMessageBox("Same program is already run. Close program and run again.");
                for (int i = 0; i < processes.Length; i++)
                {
                    if (intCurrentProcess != processes[i].Handle)
                    {
                        processes[i].Kill();
                    }
                }
                return;
            }
            #endregion

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                //Application.Run(new Loading());
                //Application.Run(new frmMain());
                //Application.Run(new frmMain_Sanding_Megagen());
                FuncIni.LoadSimulationIni();

                GlobalVar.MainForm = new frmMain_AutoInline_PC(); // 본프로그램으로 실행
                //GlobalVar.MainForm = new OutModule(); // 외모듈용으로 실행시
                Application.Run(GlobalVar.MainForm);
                //Application.Run(new frmMain_AutoInline_PC());

                Environment.Exit(Environment.ExitCode);//프로그램 남는거 때문에
            }
            catch (Exception ex)
            {
                FuncLog.WriteLog(ex.ToString());
                FuncLog.WriteLog(ex.StackTrace);
                GlobalVar.GlobalStop = true;
            }
        }
    }
}
