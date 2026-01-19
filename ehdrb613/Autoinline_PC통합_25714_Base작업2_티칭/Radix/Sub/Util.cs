using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Drawing;
using System.Collections.Concurrent; // ConcurrentQueue

using System.Runtime.InteropServices;

namespace Radix
{
    public static class Util
    {
        /*
         * Util.cs : 로직과 직접 관련 없는 함수들 선언
         * 일단 여기에 선언해 두고 특정기능별 분기가 필요시 분기
         */


        public static void Debug(string str) //Debug(문자열) 해당 문자열로 디버그 실행  
        {
            if (GlobalVar.Debug)
            {
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " : " + str);
                //WriteFile("C:\\FA\\Log\\debug_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt", DateTime.Now.ToString("yyyy-MM-dd,HH:mm:ss") + " : " + str);
                if (GlobalVar.Simulation)
                {
                    //System.Diagnostics.Trace.WriteLine(str);
                    //Console.WriteLine(str);
                    //WriteFile("C:\\FA\\Log\\debug_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt", str);
                }
                else
                {
                    //WriteFile("C:\\FA\\Log\\debug_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt", str);
                }
            }
        }


        #region 데이터 변환
        public static double TryParseDouble(string s)
        {
            double d = 0;
            Double.TryParse(s, out d);
            return d;
        }
        public static int TryParseInt(string s)
        {
            int i = 0;
            int.TryParse(s, out i);
            return i;
        }
        public static bool TryParseBool(string s)
        {
            bool b = false;
            Boolean.TryParse(s, out b);
            return b;
        }
        public static byte[] BitArrayToByteArray(bool[] bits) // BitArrayToByteArray(비트배열) 8개 비트를 바이트로 변환
        {
            byte[] ret = new byte[Math.Max(1, bits.Length / 8)];
            for (int i = 0; i < bits.Length / 8; i++)
            {
                byte bt = 0;
                for (int j = 0; j < Math.Min(8, bits.Length - 8 * i); j++)
                {
                    if (bits[i * 8 + j])
                    {
                        bt += (byte)Math.Pow(2, j);
                    }
                }
                ret[i] = bt;
            }
            //bits.CopyTo(ret, 0);
            return ret;
        }
        public static uint[] BitArrayToWordArray(bool[] bits) // BitArrayToWordArray(비트배열) 32개 비트를 워드로 변환
        {
            uint[] ret = new uint[Math.Max(1, bits.Length / 32)];
            for (int i = 0; i < bits.Length / 32; i++)
            {
                uint bt = 0;
                for (int j = 0; j < Math.Min(32, bits.Length - 32 * i); j++)
                {
                    if (bits[i * 32 + j])
                    {
                        bt += (uint)Math.Pow(2, j);
                    }
                }
                ret[i] = bt;
            }
            //bits.CopyTo(ret, 0);
            return ret;
        }

        public static bool[] ByteToBoolArray(byte b) // ByteToBoolArray(바이트) 바이트를 8개 비트로 변환
        {
            // prepare the return result
            bool[] result = new bool[8];

            try
            {
                // check each bit in the byte. if 1 set to true, if 0 set to false
                for (int i = 0; i < 8; i++)
                    result[7 - i] = (b & (1 << i)) == 0 ? false : true;

                // reverse the array
                Array.Reverse(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
            return result;
        }

        public static bool[] WordToBoolArray(uint b) // WordToBoolArray(바이트) 바이트를 32개 비트로 변환
        {
            // prepare the return result
            bool[] result = new bool[32];

            try
            {
                // check each bit in the byte. if 1 set to true, if 0 set to false
                for (int i = 0; i < 32; i++)
                    result[31 - i] = (b & (1 << i)) == 0 ? false : true;

                // reverse the array
                Array.Reverse(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
            return result;
        }

        public static string IntToHexString(int num, int size)
        {
            return num.ToString("X" + size.ToString());
        }

        public static bool[] HexToBoolArray(string hex) // 16진수 문자열을 16비트 배열로 변환
        {
            // prepare the return result
            bool[] result = new bool[16];

            try
            {
                ushort dec = Convert.ToUInt16(hex, 16);

                // check each bit in the byte. if 1 set to true, if 0 set to false
                for (int i = 0; i < 16; i++)
                    result[15 - i] = (dec & (1 << i)) == 0 ? false : true;

                // reverse the array
                Array.Reverse(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
            return result;
        }

        public static byte[] StringToByteArray(string str) // StringToByteArray(문자열) 문자열을 바이트 배열로 변환
        {
            return Encoding.ASCII.GetBytes(str);
        }

        public static string ByteArrayToString(byte[] bytes) // ByteArrayToString(바이트배열) 바이트 배열을 문자열로 변환
        {
            return Encoding.ASCII.GetString(bytes);
        }

        public static void SortArray(ref String[] arr) // SortArray(문자열배열참조) 문자열배열 정렬
        {
            Array.Sort(arr);
        }

        public static string IntToString(int num, int length)
        {
            return num.ToString("D" + length.ToString());
        }

        public static string DoubleToString(double num, int sLength, int eLength)
        {
            string fmt = "";
            for (int i = 0; i < sLength; i++)
            {
                fmt += "0";
            }
            fmt += ".";
            for (int i = 0; i < eLength; i++)
            {
                fmt += "#";
            }

            return num.ToString(fmt);
        }
        #endregion

        #region 콤보박스 관련
        public static bool CheckComboItem(ref ComboBox cmb, String str, bool sel) // CheckComboItem(콤보박스참조, 문자열, 선택여부) 콤보박스의 아이템 존재여부 확인
        {
            for (int i = 0; i < cmb.Items.Count; i++)
            {
                if ((String)cmb.Items[i] == str)
                {
                    if (sel)
                    {
                        cmb.SelectedIndex = i;
                    }
                    return true;
                }
            }
            return false;
        }

        public static void SortComboItem(ref ComboBox cmbbox) // SortComboItem(콤보박스참조) 콤보박스의 아이템 정렬 
        {
            if (cmbbox.Items.Count > 0)
            {
                string[] str = new string[cmbbox.Items.Count];
                for (int i = 0; i < cmbbox.Items.Count; i++)
                {
                    str[i] = (string)cmbbox.Items[i];
                }
                SortArray(ref str);
                for (int i = 0; i < cmbbox.Items.Count; i++)
                {
                    cmbbox.Items[i] = str[i];
                }
            }
        }

        public static void SetComboIndex(ref ComboBox cmbbox, string text) // 콤보박스에서 해당되는 아이템 찾아서 selected Index 수정
        {
            for (int i = 0; i < cmbbox.Items.Count; i++)
            {
                if (cmbbox.Items[i].ToString() == text)
                {
                    cmbbox.SelectedIndex = i;
                    return;
                }
            }
        }
        #endregion

        #region 문자열 관련
        public static string EraseFirstLine(String str) // EraseLastLine(문자열) 문자열 중 첫 줄 제거
        {
            try
            {
                string rtn = "";
                string[] lines = str.Split('\n');
                if (lines.Length <= 1)
                {
                    return str;
                }
                for (int i = 0; i < lines.Length - 1; i++)
                {
                    rtn += lines[i + 1];
                    if (i < lines.Length - 2)
                    {
                        rtn += "\n";
                    }
                }
                return rtn;
            }
            catch (Exception ex)
            {
                Debug(ex.ToString());
                Debug(ex.StackTrace);
            }
            return str;
        }

        public static string EraseLastLine(String str) // EraseLastLine(문자열) 문자열 중 마지막 줄 제거
        {
            try
            {
                string rtn = "";
                string[] lines = str.Split('\n');
                if (lines.Length <= 1)
                {
                    return str;
                }
                for (int i = 0; i < lines.Length - 1; i++)
                {
                    rtn += lines[i];
                    if (i < lines.Length - 2)
                    {
                        rtn += "\n";
                    }
                }
                return rtn;
            }
            catch (Exception ex)
            {
                Debug(ex.ToString());
                Debug(ex.StackTrace);
            }
            return str;
        }
        public static string GetLastLine(String str) // GetLastLine(문자열) 문자열 중 마지막 줄 
        {
            int startIdx = 0;
            try
            {
                startIdx = str.LastIndexOf("\n");
                if (startIdx >= 0)
                {
                    if (str.Substring(startIdx + 1).TrimEnd().Length > 0)
                    {
                        return str.Substring(startIdx + 1).TrimEnd();
                    }
                    else
                    {
                        string substr = str.Substring(0, startIdx - 1);
                        startIdx = str.LastIndexOf("\n");
                        if (startIdx >= 0)
                        {
                            return str.Substring(startIdx + 1).TrimEnd();
                        }
                        else
                        {
                            return str.TrimEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
            return str;
        }

        public static string GetFirstLine(String str) // GetFirstLine(문자열) 문자열 중 첫 줄 
        {
            int startIdx = 0;
            try
            {
                startIdx = str.LastIndexOf("\n");
                if (startIdx >= 0)
                {
                    if (str.Substring(startIdx + 1).TrimEnd().Length > 0)
                    {
                        return str.Substring(startIdx + 1).TrimEnd();
                    }
                    else
                    {
                        string substr = str.Substring(0, startIdx - 1);
                        startIdx = str.LastIndexOf("\n");
                        if (startIdx >= 0)
                        {
                            return str.Substring(startIdx + 1).TrimEnd();
                        }
                        else
                        {
                            return str.TrimEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
            return str;
        }

        public static string GetSecondLine(String str) // GetSecondLine(문자열) 문자열 중 두번째 줄 
        {
            int startIdx = 0;
            try
            {
                startIdx = str.LastIndexOf("\n");
                if (startIdx >= 0)
                {
                    if (str.Substring(startIdx + 1).TrimEnd().Length > 0)
                    {
                        return str.Substring(startIdx + 1).TrimEnd();
                    }
                    else
                    {
                        string substr = str.Substring(0, startIdx - 1);
                        startIdx = str.LastIndexOf("\n");
                        if (startIdx >= 0)
                        {
                            return str.Substring(startIdx + 1).TrimEnd();
                        }
                        else
                        {
                            return str.TrimEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
            return str;
        }

        public static string FillZero(int num, int size) // YJ추가 정수형 앞자리에 0 채우기
        {
            string str = num.ToString();

            for (int i = 0; i < size - str.Length; i++)
            {
                str = "0" + str;
            }

            return str;
        }
        #endregion

        public static void USleep(int time) // USleep(대기msec) 슬립 실행
        {
            Thread.Sleep(time);
            //Application.DoEvents();
        }

        public static Parity ParseParity(string str) // 문자열 형식에서 Parity 타입으로 변환
        {
            Array parity = Enum.GetValues(typeof(Parity));
            for (int i = 0; i < parity.Length; i++)
            {
                if (parity.GetValue(i).ToString() == str)
                {
                    return (Parity)i;
                }
            }
            return Parity.None;
        }

        public static StopBits ParseStopBits(string str) // 문자열 형식에서 Parity 타입으로 변환
        {
            Array stopBits = Enum.GetValues(typeof(StopBits));
            for (int i = 0; i < stopBits.Length; i++)
            {
                if (stopBits.GetValue(i).ToString() == str)
                {
                    return (StopBits)i;
                }
            }
            return StopBits.One;
        }

        public static double CheckAngle(double x1, double y1, double x2, double y2) // CheckAngle(x1,y1, x2,y2) x,y 두 좌표간의 각도 계산
        {
            double xDiff = x1 - x2;
            double yDiff = y2 - y1;
            return 0 - Math.Atan2(yDiff, xDiff) * (180 / Math.PI);
        }

        public static void StartWatch(ref Stopwatch watch)
        {
            if (watch == null)
            {
                watch = new Stopwatch();
            }
            if (watch.IsRunning)
            {
                watch.Stop();
            }
            //watch.Reset();
            watch.Start();
        }

        public static void ResetWatch(ref Stopwatch watch)
        {
            if (watch == null)
            {
                watch = new Stopwatch();
            }
            if (watch.IsRunning)
            {
                watch.Stop();
            }
            watch.Reset();
            watch.Start();
        }


        public static void InitWatch(ref Stopwatch watch)
        {
            if (watch == null)
            {
                watch = new Stopwatch();
            }
            if (watch.IsRunning)
            {
                watch.Stop();
            }
            watch.Reset();
        }
    }
}
