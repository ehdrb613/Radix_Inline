using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;

namespace Radix
{
    /**
     * @brief INI 파일 처리 함수 선언
     */
    class FuncIni
    {
        /*
         * FuncIni.cs : INI 파일 처리 함수 선언
         */

        #region Window API 함수 Import

        #region INI 관련
        [DllImport("kernel32")]
        /**
         * @brief Copies a string into the specified section of an initialization file.
         * @param section The name of the section to which the string will be copied. 
         *      If the section does not exist, it is created. 
         *      The name of the section is case-independent; the string can be any combination of uppercase and lowercase letters.
         * @param key The name of the key to be associated with a string. 
         *      If the key does not exist in the specified section, it is created. 
         *      If this parameter is NULL, the entire section, including all entries within the section, is deleted.
         * @param val A null-terminated string to be written to the file. 
         *      If this parameter is NULL, the key pointed to by the lpKeyName parameter is deleted.
         * @param filePath The name of the initialization file.
         *      If the file was created using Unicode characters, the function writes Unicode characters to the file. 
         *      Otherwise, the function writes ANSI characters.
         * @return long If the function successfully copies the string to the initialization file, the return value is nonzero.
         *      If the function fails, or if it flushes the cached version of the most recently accessed initialization file, the return value is zero.
         *      To get extended error information, call GetLastError.
         */
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        /**
         * @brief Retrieves a string from the specified section in an initialization file.
         * @param section The name of the section containing the key name. 
         *      If this parameter is NULL, the GetPrivateProfileString function copies all section names in the file to the supplied buffer.
         * @param key The name of the key whose associated string is to be retrieved. 
         *      If this parameter is NULL, all key names in the section specified by the lpAppName parameter are copied to the buffer specified by the lpReturnedString parameter.
         * @param def A default string. 
         *      If the lpKeyName key cannot be found in the initialization file, GetPrivateProfileString copies the default string to the lpReturnedString buffer.
         *      If this parameter is NULL, the default is an empty string, "".
         *      Avoid specifying a default string with trailing blank characters. 
         *      The function inserts a null character in the lpReturnedString buffer to strip any trailing blanks.
         * @param retVal A pointer to the buffer that receives the retrieved string.
         * @param size The size of the buffer pointed to by the lpReturnedString parameter, in characters.
         * @param filePath The name of the initialization file. 
         *      If this parameter does not contain a full path to the file, the system searches for the file in the Windows directory.
         * @return int The return value is the number of characters copied to the buffer, not including the terminating null character.
         *      If neither lpAppName nor lpKeyName is NULL and the supplied destination buffer is too small to hold the requested string, the string is truncated and followed by a null character, and the return value is equal to nSize minus one.
         *      If either lpAppName or lpKeyName is NULL and the supplied destination buffer is too small to hold all the strings, the last string is truncated and followed by two null characters. 
         *      In this case, the return value is equal to nSize minus two.
         *      In the event the initialization file specified by lpFileName is not found, or contains invalid values, calling GetLastError will return '0x2' (File Not Found). 
         *      To retrieve extended error information, call GetLastError.
         */
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal,
                                                        int size, string filePath);
        #endregion

        #endregion


        #region INI 파일 쓰기 / 읽기
        // INI 파일 쓰기
        /**
         * @brief INI 파일에 설정 쓰기 
         * @param section 구분 section
         * @param key 키값
         * @param value 키에 기록할 값
         * @param path ini 파일의 경로
         */
        public static void WriteIniFile(string section, string key, string value, string path) // WriteIniFile(섹션,키,값,경로) INI 설정 쓰기 
        {
            //Debug("writeinitfile : " + section + "," + key + "," + value + "," + path);
            for (int j = 0; j < 10; j++) // 20200319 10번까지 재시도
            {
                try
                {
                    string[] arrPath = path.Split('\\');
                    string tempPath = arrPath[0];
                    for (int i = 1; i < tempPath.Length - 1; i++)
                    {
                        tempPath += "\\" + tempPath[i];
                        if (!Directory.Exists(tempPath))
                        {
                            Directory.CreateDirectory(path);
                        }
                    }
                    WritePrivateProfileString(section, key, value, path);
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                }
            }
            FuncLog.WriteLog("WriteIniFile failed : " + section + "," + key + "," + value + "," + path);
        }

        // INI 파일 읽기
        /**
         * @brief INI 파일을 읽어 특정 키값 읽기
         * @param section 구분 section
         * @param key 키값
         * @param path ini 파일의 경로
         * @return string 해당 키에 기록된 값
         */
        public static string ReadIniFile(string section, string key, string path) // ReadIniFile(섹션,키,경로) INI 설정 읽기  
        {
            try
            {
                StringBuilder sb = new StringBuilder(1024);
                GetPrivateProfileString(section, key, "", sb, sb.Capacity, path);
                return sb.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
            return "";
        }
        /**
         * @brief INI 파일을 읽어 특정 키값 읽기
         *      키값이 존재하지 않을 때 기본값으로 읽어온다
         * @param section 구분 section
         * @param key 키값
         * @param path ini 파일의 경로
         * @param defaultStr 키값이 존재하지 않을 때 기본값
         * @return string 해당 키에 기록된 값
         */
        public static string ReadIniFile(string section, string key, string path, string defaultStr) // ReadIniFile(섹션,키,경로,기본값) INI 설정 읽기
        {
            StringBuilder sb = new StringBuilder(1024);
            GetPrivateProfileString(section, key, "", sb, sb.Capacity, path);
            string str = sb.ToString() == "" ? defaultStr : sb.ToString();
            //if (sb.ToString() == "") // 기본값으로 읽은 경우 그 기본값을 강제 저장한다.
            //{
            //    WriteIniFile(section, key, str, path);
            //}
            return str;
        }


        #endregion



        #region INI 실제 연결

        /**
         * @brief 시뮬레이션 설정 파일 읽어 시뮬레이션 여부 저장
         */
        public static void LoadSimulationIni() // 장비의 모든 설정 읽기
        {
            string IniPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\Simulation.ini";
            string Section = GlobalVar.IniSection;

            GlobalVar.Simulation = FuncIni.ReadIniFile(Section, "Simulation", IniPath, "False") == "True";
        }
        /**
         * @brief 프로그램 구동시 기본으로 읽을 모델 저장
         *      모델 변경시 최종 선택 모델을 기본값으로 저장한다.
         * @param modelName 기본값으로 저장할 모델명
         */
        public static void SaveDefaultModel(string modelName) // 프로그램 구동시 기본으로 읽을 모델 저장
        {
            string IniPath = GlobalVar.FaPath;
            if (!Directory.Exists(GlobalVar.IniPath))
            {
                Directory.CreateDirectory(IniPath);
            }
            IniPath += "\\" + GlobalVar.SWName;
            if (!Directory.Exists(GlobalVar.IniPath))
            {
                Directory.CreateDirectory(IniPath);
            }
            IniPath += "\\" + GlobalVar.IniPath;
            if (!Directory.Exists(GlobalVar.IniPath))
            {
                Directory.CreateDirectory(IniPath);
            }
            IniPath += "\\setting.ini";

            string Section = GlobalVar.IniSection;

            FuncIni.WriteIniFile(Section, "DefaultModel", modelName, IniPath);
        }


      

        #endregion


    }
}
