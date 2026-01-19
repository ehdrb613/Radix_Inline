using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace Radix
{
    /**
     * 
     * @brief 파일조작 및 Excel, CSV 처리 함수 선언
     * 
     */
    class FuncFile
    {

        #region Window API 함수 Import

        #region INI 관련
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal,
                                                        int size, string filePath);
        #endregion

        #endregion

        /**
         *
         *        @brief 클래스 내부 디버그 출력을 위한 함수
         *              Util.cs에 정의된 바와 같이 디버그 출력을 수행한다. 
         *              클래스 자체 디버그 막기 위해서는 출력부를 주석처리하면 된다.
         *        @param string str 디버그 출력할 문자열
         *        @return void
         *
         */
        private static void debug(string str) // 로컬 디버그
        {
            //Util.Debug("FuncExcel : " + str);
        }

        // INI 파일 쓰기
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
            //FuncLog.WriteLog("WriteIniFile failed : " + section + "," + key + "," + value + "," + path);
        }

        // INI 파일 읽기
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

        /**
         *
         *        @brief 테스트파일에 문자열 추가 저장 
         *        @param path 문자열 추가할 파일 경로
         *        @param text 추가할 문자열
         *        @return 없음
         *
         */
        public static void WriteFile(string path, string text) // WriteFile(경로,문자열) 테스트파일에 문자열 추가 저장 
        {
            //string path = @"c:\temp\MyTest.txt";
            // This text is added only once to the file.
            try
            {
                if (!File.Exists(path))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(text);
                        sw.Close();
                    }
                }
                else
                {
                    // This text is always added, making the file longer over time
                    // if it is not deleted.
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine(text);
                        sw.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
        }

        public static void WriteNewFile(string path, string text) // WriteFile(경로,문자열) 테스트파일에 문자열 덮어씌워서 저장 
        {
            //string path = @"c:\temp\MyTest.txt";
            // This text is added only once to the file.
            try
            {
                // 파일이 존재하면 삭제
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                // 새 파일을 생성하고 문자열을 쓴다.
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine(text);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
        }

        public static bool CreateDirectory(string path) // 디렉토리 문자열로 받아서 단계적으로 폴더 생성. 경로는 절대경로라야 함
        {
            if (path == null ||
                (!path.ToUpper().StartsWith("C") &&
                        !path.ToUpper().StartsWith("D") &&
                        !path.ToUpper().StartsWith("E") &&
                        !path.ToUpper().StartsWith("F") &&
                        !path.ToUpper().StartsWith("G")))
            {
                return false; // 경로문자열이 절대경로 아니면 거부
            }

            try
            {
                if (Directory.Exists(path))
                {
                    return true;
                }

                string[] subPath = path.Split('\\');
                if (subPath.Length < 2)
                {
                    return false; // 서브폴더 있지 않으면 거부
                }

                string fullPath = subPath[0]; // 일단 최상위 폴더로 지정
                for (int i = 1; i < subPath.Length; i++) // 서브폴더 각각
                {
                    fullPath += "\\" + subPath[i];
                    if (!Directory.Exists(fullPath))
                    {
                        Directory.CreateDirectory(fullPath);
                    }
                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
                return false;
            }

            return true;
        }


        #region EXCEL
        // 지정된 파일경로로 저장
        /**
         *        @brief 지정 Excel 파일에 배열 저장 실행
         *        @param excelPath 데이터 추가할 엑셀 파일 경로
         *        @param content 추가할 문자열 배열
         *        @return bool 저장 성공시 true
         *              저장 실패시 false
         */
        public static bool WriteExcelData(string excelPath, object[,] content) // 지정 Excel 파일에 배열 저장 실행
        {
            if (excelPath.Length <= 0)
            {
                return false;
            }

            Excel.Application excelApp = null;
            Excel.Workbook wb = null;
            Excel.Worksheet ws = null;
            try
            {
                int row = content.GetLength(0);
                int column = content.GetLength(1);
                excelApp = new Excel.Application();

                // 기존 파일이 있으면 덮어쓰기 확인 후 삭제
                if (File.Exists(excelPath))
                {
                    //if (FuncWin.MessageBoxOK("Overwrite to " + excelPath + " ?"))
                    //{
                    //    return false;
                    //}
                    File.Delete(excelPath);
                }

                //wb = excelApp.Workbooks.Open(excelPath); // 파일이 있을 때
                wb = excelApp.Workbooks.Add();
                // 엑셀파일을 엽니다.
                // ExcelPath 대신 문자열도 가능합니다
                // 예. Open(@"D:\test\test.xlsx");

                ws = wb.Worksheets.get_Item(1) as Excel.Worksheet;
                // 첫번째 Worksheet를 선택합니다.

                Excel.Range rng = ws.Range[ws.Cells[1, 1], ws.Cells[row, column]];
                // 해당 Worksheet에서 저장할 범위를 정합니다.
                // 지금은 저장할 행렬의 크기만큼 지정합니다.
                // 다른 예시 Excel.Range rng = ws.Range["B2", "G8"];

                object[,] data = new object[row, column];
                // 저장할 때 사용할 object 행렬


                // 내용부
                for (int r = 0; r < content.GetLength(0); r++)
                {
                    for (int c = 0; c < content.GetLength(1); c++)
                    {
                        data[r + 1, c] = content[r, c];
                    }
                }
                // for문이 아니더라도 object[,] 형으로 저장된다면 저장이 가능합니다.

                rng.Value = data;
                // data를 불러온 엑셀파일에 적용시킵니다. 아직 완료 X

                wb.SaveAs(excelPath);

                wb.Close();
                excelApp.Quit();
            }
            catch (Exception ex)
            {
                //throw ex;
                debug(ex.ToString());
                debug(ex.StackTrace);
                return false;
            }
            finally
            {
                ReleaseExcelObject(ws);
                ReleaseExcelObject(wb);
                ReleaseExcelObject(excelApp);
            }
            return true;
        }

        // 새 파일명을 입력받아서 저장
        /**
         *        @brief Excel 파일 저장시 새파일명 대화상자로 입력받아 저장
         *        @param content 추가할 문자열 배열
         *        @return bool 저장 성공시 true
         *              저장 실패시 false
         */
        public static bool WriteNewExcelFile(object[,] content) // Excel 파일 저장시 새파일명 대화상자로 입력받아 저장
        {
            string excelPath = "";
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Save Excel";
            dlg.Filter = "Excel|*.xlsx";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                excelPath = dlg.FileName;
            }
            if (excelPath.Length <= 0)
            {
                return false;
            }

            return WriteExcelData(excelPath, content);
        }

        /**
         *        @brief dataGridView 의 데이터를 새 엑셀파일 이름을 입력받아 저장한다.
         *        @param grid 데이터를 읽을 DataGridView 개체
         *        @return bool 저장 성공시 true
         *              저장 실패시 false
         */
        public static bool WriteNewExcelFromDataGrid(DataGridView grid)// dataGridView 의 데이터를 새 엑셀파일로 저장한다.
        {
            try
            {
                string[,] data = new string[grid.Rows.Count + 1, grid.Columns.Count]; // 헤더 정보를 더하기 때문에 행을 하나 더한다
                FuncForm.SetDataGridViewToArray(grid, ref data); // dataGridView 에서 데이터 가져오기

                return WriteNewExcelFile(data);
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        /**
         *        @brief Excel library 사용 해제
         *        @param obj 해제할 excel library 개체
         *        @return void
         */
        private static void ReleaseExcelObject(object obj) // Excel library 사용 해제
        {
            try
            {
                if (obj != null)
                {
                    Marshal.ReleaseComObject(obj);
                    obj = null;
                }
            }
            catch (Exception ex)
            {
                obj = null;
                //throw ex;
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            finally
            {
                GC.Collect();
            }
        }

        #endregion


        #region CSV
        /**
         *        @brief csv 포멧의 파일을 열어 2차원 배열로 읽는다
         *        @param excelPath 읽을 csv 파일 경로 문자열
         *        @return string[,] 읽은 데이터 배열
         */
        public static string[,] LoadCSVData(string excelPath) // 지정 CSV 파일로 저장 실행
        {
            if (excelPath.Length <= 0)
            {
                return null;
            }

            try
            {

                if (File.Exists(excelPath))
                {
                    string[] lines = File.ReadAllLines(excelPath);
                    if (lines.Length > 0)
                    {
                        int columns = lines[0].Split(',').Length;
                        string[,] csv = new string[lines.Length, columns];
                        for (int j = 0; j < lines.Length; j++)
                        {
                            string[] line = lines[j].Split(',');
                            for (int i = 0; i < Math.Min(columns, lines.Length); i++)
                            {
                                csv[j, i] = line[i];
                            }
                        }
                        return csv;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }

            return null;
        }

        /**
         *        @brief 새 CSV 파일명 입력받아 2차원 배열로 읽는다
         *        @return string[,] 읽은 데이터 배열
         */
        public static string[,] LoadCSVFile() // 새 CSV 파일명 입력받아 저장 수행
        {
            string excelPath = "";
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Save CSV";
            dlg.Filter = "CSV|*.csv";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                excelPath = dlg.FileName;
            }
            if (excelPath.Length <= 0)
            {
                return null;
            }

            return LoadCSVData(excelPath);
        }

        /**
         *        @brief CSV 파일명 입력받아 읽은 후 DataGrid에 출력
         *        @param grid 데이터를 출력할 DataGridView 개체
         *        @return void
         */
        public static void LoadCSVToDataGrid(DataGridView grid) // 새 CSV 파일명 입력받아 읽은 후 DataGrid에 출력
        {
            string[,] csv = LoadCSVFile();
            if (csv == null)
            {
                return;
            }
            grid.Rows.Clear();
            for (int j = 1; j < csv.GetLength(0); j++)
            {
                if (csv.GetLength(1) > 0)
                {
                    grid.Rows.Add(csv[j, 0]); // column이 몇 개인지 모르므로 일단 한 셀만 추가하고 나머지는 하나씩 변경
                    for (int i = 0; i < csv.GetLength(1); i++)
                    {
                        grid.Rows[j - 1].Cells[i].Value = csv[j, i];
                    }
                }
            }
        }

        /**
         *        @brief 배열 데이터를 지정 CSV 파일로 저장
         *        @param excelPath 데이터를 저장할 csv 파일
         *        @param content 데이터를 읽을 2차원 배열
         *        @return bool 저장 성공시 true
         *              저장 실패시 false
         */
        public static bool WriteCSVData(string excelPath, object[,] content) // 배열 데이터를 지정 CSV 파일로 저장
        {
            if (excelPath.Length <= 0)
            {
                return false;
            }

            try
            {

                // 기존 파일이 있으면 덮어쓰기 확인 후 삭제
                if (File.Exists(excelPath))
                {
                    //if (FuncWin.MessageBoxOK("Overwrite to " + excelPath + " ?"))
                    //{
                    //    return false;
                    //}
                    File.Delete(excelPath);
                }


                // 내용부
                for (int r = 0; r < content.GetLength(0); r++)
                {
                    string line = "";
                    for (int c = 0; c < content.GetLength(1); c++)
                    {
                        line += (line.Length > 0 ? "," : "") + content[r, c];
                    }
                    WriteFile(excelPath, line);
                }
                // for문이 아니더라도 object[,] 형으로 저장된다면 저장이 가능합니다.

            }
            catch (Exception ex)
            {
                //throw ex;
                debug(ex.ToString());
                debug(ex.StackTrace);
                return false;
            }

            return true;
        }

        /**
         *        @brief 새 CSV 파일명 입력받아 배열 데이터 저장
         *        @param content 데이터를 읽을 2차원 배열
         *        @return bool 저장 성공시 true
         *              저장 실패시 false
         */
        public static bool WriteNewCSVFile(object[,] content) // 새 CSV 파일명 입력받아 배열 데이터 저장
        {
            string excelPath = "";
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Save CSV";
            dlg.Filter = "CSV|*.csv";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                excelPath = dlg.FileName;
            }
            if (excelPath.Length <= 0)
            {
                return false;
            }

            return WriteCSVData(excelPath, content);
        }

        /**
         *        @brief 새 CSV 파일명 입력받아 DataGrid의 내용 저장
         *        @param grid 데이터를 읽을 DataGridView 개체
         *        @return bool 저장 성공시 true
         *              저장 실패시 false
         */
        public static bool WriteNewCSVFromDataGrid(DataGridView grid) // 새 CSV 파일명 입력받아 DataGrid의 내용 저장
        {
            try
            {
                string[,] data = new string[grid.Rows.Count + 1, grid.Columns.Count]; // 헤더 정보를 더하기 때문에 행을 하나 더한다
                FuncForm.SetDataGridViewToArray(grid, ref data); // dataGridView 에서 데이터 가져오기

                return WriteNewCSVFile(data);
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }
        #endregion

    }
}
