using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radix
{
    public class PMCDeviceServer
    {
        /*
         * 개발중 가상 Virtual COM이 지원되어서 개발 중단
         * TCPClient 대용으로 추후 사용해도 유용할 듯
        //*/

        #region 로컬 변수
        private static readonly System.Text.StringBuilder response =
            new System.Text.StringBuilder();

        TelnetSocket socket = new TelnetSocket();
        private static string host = "192.168.0.1";
        private static int port = 23;
        #endregion

        private static void
        DataReceived // 기본함수이긴 하지만 수신부 판단해야 하므로 별로로 작성함
        (
            string Data
        )
        {
            lock (response)
            {
                response.Append(Data);
            }

            return;
        }

        #region 통신 기본함수

        private static void
        ExceptionCaught
        (
            System.Exception Exception
        )
        {
            throw (Exception);
        }

        private static void
        WaitFor
        (
            string Prompt
        )
        {
            while (response.ToString().IndexOf(Prompt) == -1)
            {
                System.Threading.Thread.Sleep(100);
            }

            lock (response)
            {
                System.Console.Write(response);
                response.Length = 0;
            }

            return;
        }

        public PMCDeviceServer()
        {
            socket.OnDataReceived += DataReceived;
            socket.OnExceptionCaught += ExceptionCaught;
        }

        public PMCDeviceServer(string ip, int p)
        {
            socket.OnDataReceived += DataReceived;
            socket.OnExceptionCaught += ExceptionCaught;

            host = ip;
            port = p;
        }

        public void SetHost(string ip, int p)
        {
            host = ip;
            port = p;
        }

        public void SetHost(string ip)
        {
            host = ip;
        }

        public void SetPort(int p)
        {
            port = p;
        }

        public bool Connect() // 인증 없는 경우
        {
            try
            {
                socket.Connect(host, port);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public bool Connect(string id, string pwd) // 아이디와 비밀번호 입력해야 하는 경우
        {
            try
            {
                socket.Connect(host, port);
                WaitFor("Username:");
                socket.WriteLine(id);
                WaitFor("Password:");
                socket.WriteLine(pwd);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public void Disconnect()
        {
            try
            {
                socket.WriteLine("exit");
                WaitFor("logged out");
                socket.Close();
            }
            catch (Exception ex)
            {
                FuncLog.WriteLog(ex.ToString());
                FuncLog.WriteLog(ex.StackTrace);
            }
        }

        private void debug(String str)
        {
            Util.Debug("PMCDeviceServer : " + str);
        }
        #endregion


        #region 모션제어 함수 - rs232
        #endregion
    }
}
