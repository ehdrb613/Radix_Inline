using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Concurrent; // ConcurrentQueue
using System.Windows.Forms;

namespace Radix
{
    /*
     * MsSQL.cs : MS SQL 서버 이용 함수 모음 - ADO가 한 Connection에 한 쿼리만 지원하므로 쿼리시 연결하고 바로 끊는 걸로
     * 
     * 사용법 : 
     * 
     * using System.Data.SqlClient; // 쿼리 결과를 파싱하기 위해 include 함
     * 
     * MsSQL mssql; // 클래스 로컬 변수로 클래스 object를 생성한다.
     * mssql = new MsSql(아이피,포트,아이디,비밀번호,database); // 접속정보로 접속한다.
     *         추후 접속여부 확인은 mssql.connected 를 확인하면 됨
     *         
     * bool result = mssql.Execute(sql문); // create,alter,insert,delete,update 등 결과값 받지 않는 쿼리 실행
     * 
     * // select 등 결과값을 받아야 하는 경우
     * SqlDataReader reader; // 쿼리 결과를 담을 object를 생성
     * try
     * {
     *     reader = mssql.Read(sql문);
     *     string[] arrResult = mssql.RsToArray(reader);
     *     // arrResult 배열로 필요한 처리 및 출력을 함
     * } catch (Exception ex)
     * {}
     * if (reader != null)
     * {
     *      reader.close();
     * } 
     * 
     * mssql.Close(); // 접속 해제
     */

    /**
     * @brief MSSql 관련 클래스
     *      연결은 Express 버전의 1유저 특성을 고려해서 실행시 즉시 연결하고 바로 끊는다.
     *      MsSQL mssql = new MsSQL(server, port, uid, pwd, db); 후
     *      mssql.Execute(string sql); 로 insert/update/delete 실행
     *      string[,] var = mssql.Read(string sql); 로 select 실행
     */

    /**
     *
     *  JHRYU 수정사항 2024.
     *  1. connected 변수를 살려서 접속테스트를 했으면 true, 통신 성공 했으면 true, 통신 실패 했으면 false 값으로 설정한다.
     *  2. 접속 테스트만 하는 Check_Connect() 함수 만듦
    */

    public class MsSQL
    {
        #region 전역변수
        /** @brief 접속됨. 매번 실행시 일회성으로 접속 후 끊으므로 사용할 필요 없다. */
        public bool connected = false;  // 컨넥트 풀이라도 사용함, DB 접속 및 쿼리 성공시 true, 실패시 false 된다.
        #endregion

#if (false)

#region 로컬변수
        /** @brief mssql 서버 아이피 */
        private string server = "127.0.0.1"; // mssql 서버 아이피
        /** @brief mssql 서버 포트 */
        private string port = "1433"; // mssql 서버 포트
        /** @brief mssql 접속 아이디 */
        private string uid = "sa"; // mssql 접속 아이디
        /** @brief mssql 접속 비밀번호 */
        private string pwd = "radix5243"; // mssql 접속 비밀번호
        /** @brief database */
        private string db = "radix"; // database
#endregion
#else
        private string server = "192.168.10.200"; // mssql 서버 아이피
        /** @brief mssql 서버 포트 */
        private string port = "1433"; // mssql 서버 포트
        /** @brief mssql 접속 아이디 */
        private string uid = "pack"; // mssql 접속 아이디
        /** @brief mssql 접속 비밀번호 */
        private string pwd = "Wkdrn1004!"; // mssql 접속 비밀번호
        /** @brief database */
        private string db = "snjMES"; // database
#endif

        object lock_obj = new object();


        //private SqlConnection con = null;

        /** 
         * @brief 클래스 생성자
         * @param s mssql 서버 아이피
         * @param p mssql 서버 포트
         * @param i mssql 접속 아이디
         * @param w mssql 접속 비밀번호
         * @param d database
         */
        public MsSQL(string s, string p, string i, string w, string d) // 생성자, 아이피/포트/아이디/비번/db 지정
        {
            server = s;
            port = p;
            uid = i;
            pwd = w;
            db = d;
        }

        public MsSQL()
        {
            // 위의 디폴트 값으로 설정 사용
            //server = s;
            //port = p;
            //uid = i;
            //pwd = w;
            //db = d;
        }


        /** 
         * @brief MsSQL 서버에 접속
         *      Express 버전 사용하므로 영구연결은 사용하지 않는다.
         *      정식 버전 사용을 위해 추후 주석해제 필요
         * @return bool 연결되면 true
         *      연결 실패하면 false
         */
        public bool Connect() // MsSQL 서버에 접속
        {
            return true;
            /*
            try
            {
                string conStr = "server = " + server +
                                "," + port +
                                "; uid = " + uid +
                                "; pwd = " + pwd +
                                "; database = " + db + ";";
                SqlConnection con = new SqlConnection(conStr);
                con.Open();
                connected = true;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
            return false;
            //*/
        }


        public bool Check_Connect()
        {
            try
            {
                string conStr = "server = " + server +
                                "," + port +
                                "; uid = " + uid +
                                "; pwd = " + pwd +
                                "; database = " + db + 
                                "; connect timeout=6" +     // 5초이상 설정할것
                                ";";
                SqlConnection con = new SqlConnection(conStr);
                con.Open();

                if (con == null || con.State == ConnectionState.Closed)
                {
                    connected = false;
                    return false;
                }
                else
                {
                    con.Close();
                    connected = true;
                    return true;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                connected = false;
                return false;
            }
        }

        /** 
         * @brief MsSQL 서버 연결 해제
         *      Express 버전 사용하므로 영구연결은 사용하지 않는다.
         *      정식 버전 사용을 위해 추후 주석해제 필요
         * @return bool 성공시 true
         *      실패하면 false. 예외발송 포함이므로 false 시도 연결해제 상태로 간주해도 됨
         */
        public bool Disconnect() // 연결 해제
        {
            return true;
            /*
            try
            {
                if (connected)
                {
                    con.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
            return false;
            //*/
        }

        /** 
         * @brief MsSQL 서버 연결 해제
         *      Express 버전 사용하므로 영구연결은 사용하지 않는다.
         *      정식 버전 사용을 위해 추후 주석해제 필요
         * @return void
         */
        public void Close() // MsSQL 서버 접속 해제
        {
            /*
            connected = false;
            con.Close();
            //*/
        }

        /** 
         * @brief SqlCommand를 이용, Result없는 쿼리, Insert/Update/Delete 등.
         *      string 형식의 sql문을 이용하는 것이 가시적이므로 주석 처리함
         * @param com 조합된 SqlCommand
         * @return bool 
         */
        public bool Excute(SqlCommand com) // SqlCommand를 이용, Result없는 쿼리, 
        {
            /*
            if (!connected)
            {
                return false;
            }
            try
            {
                if (con == null ||
                    con.State == ConnectionState.Closed)
                {
                    Connect();
                }
                com.Connection = con;
                com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                return false;
            }
            //*/
            return true;
        }

        /** 
         * @brief string 형식의 쿼리문을 이용, Result없는 쿼리, Insert/Update/Delete 등.
         *      Express 버전 이용으로 즉시 연결, 실행 후 연결해제하지만 추후 정식버전 대비 보강 필요
         * @param sql 실행할 쿼리 문자열
         * @return bool 실행 성공시 true
         *      실행 실패시 false
         */
        public bool Execute(string sql)
        {
            //if (!connected)
            //{
            //    return false;
            //}

            //GlobalVar.SQLExecuteQueue.Enqueue(sql);
            //*
            try
            {
                FuncLog.WriteLog_SQL(sql);

                string conStr = "server = " + server +
                                "," + port +
                                "; uid = " + uid +
                                "; pwd = " + pwd +
                                "; database = " + db + ";";
                SqlConnection con = new SqlConnection(conStr);
                con.Open();

                if (con == null ||
                    con.State == ConnectionState.Closed)
                {
                    return false;
                    //Connect();
                }
                SqlCommand com = new SqlCommand(sql, con);
                com.CommandTimeout = 10;
                com.Connection = con;
                com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString() + "\n" + sql);
                Console.WriteLine(ex.StackTrace);
                //FuncSql.WriteSqlLog(ex.ToString() + "\n" + sql);
                //FuncSql.WriteSqlLog(ex.StackTrace);
                Disconnect(); // 일단 연결 끊으면 다음 실행때 알아서 연결하겠지
                return false;
            }
            //*/
            return true;
        }

#region // JHRYU 안쓰는 함수

        /** 
         * @brief string 형식의 쿼리문을 이용, Result없는 쿼리, Insert/Update/Delete 등.
         *      Execute(string sql) 함수가 정식이며, 지워야 할듯
         * @return bool 실행 성공시 true
         *      실행 실패시 false
         */
        public bool ExecuteQuery(string sql)
        {
            //if (!connected)
            //{
            //    return false;
            //}

            /*
            try
            {
                FuncSql.WriteSqlLog(sql);
                if (con == null ||
                    con.State == ConnectionState.Closed)
                {
                    Connect();
                }
                SqlCommand com = new SqlCommand(sql, con);
                com.CommandTimeout = 1;
                com.Connection = con;
                int result = com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString() + "\n" + sql);
                Console.WriteLine(ex.StackTrace);
                //FuncSql.WriteSqlLog(ex.ToString() + "\n" + sql);
                //FuncSql.WriteSqlLog(ex.StackTrace);
                Disconnect(); // 일단 연결 끊으면 다음 실행때 알아서 연결하겠지
                return false;
            }
            //*/
            return true;
        }

        /*
         * Parameter 전달 위해서는 sql문의 Parameter부분은 @p1,@p2... 순으로 명명되어야 함
        //*/

        public bool Execute(string sql, string p1)
        {
            /*
            if (!connected)
            {
                return false;
            }
            try
            {
                if (con == null ||
                    con.State == ConnectionState.Closed)
                {
                    Connect();
                }
                SqlCommand com = new SqlCommand(sql, con);
                com.CommandTimeout = 1;
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                return false;
            }
            //*/
            return true;
        }

        public bool Execute(string sql, string p1, string p2)
        {
            /*
            if (!connected)
            {
                return false;
            }
            try
            {
                if (con == null ||
                    con.State == ConnectionState.Closed)
                {
                    Connect();
                }
                SqlCommand com = new SqlCommand(sql, con);
                com.CommandTimeout = 1;
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.Parameters.AddWithValue("@p2", p2);
                com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                return false;
            }
            //*/
            return true;
        }

        public bool Execute(string sql, string p1, string p2, string p3)
        {
            /*
            if (!connected)
            {
                return false;
            }
            try
            {
                if (con == null ||
                    con.State == ConnectionState.Closed)
                {
                    Connect();
                }
                SqlCommand com = new SqlCommand(sql, con);
                com.CommandTimeout = 1;
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.Parameters.AddWithValue("@p2", p2);
                com.Parameters.AddWithValue("@p3", p3);
                com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                return false;
            }
            //*/
            return true;
        }

        public bool Execute(string sql, string p1, string p2, string p3, string p4)
        {
            /*
            if (!connected)
            {
                return false;
            }
            try
            {
                if (con == null ||
                    con.State == ConnectionState.Closed)
                {
                    Connect();
                }
                SqlCommand com = new SqlCommand(sql, con);
                com.CommandTimeout = 1;
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.Parameters.AddWithValue("@p2", p2);
                com.Parameters.AddWithValue("@p3", p3);
                com.Parameters.AddWithValue("@p4", p4);
                com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                return false;
            }
            //*/
            return true;
        }

        public bool Execute(string sql, string p1, string p2, string p3, string p4, string p5)
        {
            /*
            if (!connected)
            {
                return false;
            }
            try
            {
                if (con == null ||
                    con.State == ConnectionState.Closed)
                {
                    Connect();
                }
                SqlCommand com = new SqlCommand(sql, con);
                com.CommandTimeout = 1;
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.Parameters.AddWithValue("@p2", p2);
                com.Parameters.AddWithValue("@p3", p3);
                com.Parameters.AddWithValue("@p4", p4);
                com.Parameters.AddWithValue("@p5", p5);
                com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                return false;
            }
            //*/
            return true;
        }

        public bool Execute(string sql, string p1, string p2, string p3, string p4, string p5, string p6)
        {
            /*
            if (!connected)
            {
                return false;
            }
            try
            {
                if (con == null ||
                    con.State == ConnectionState.Closed)
                {
                    Connect();
                }
                SqlCommand com = new SqlCommand(sql, con);
                com.CommandTimeout = 1;
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.Parameters.AddWithValue("@p2", p2);
                com.Parameters.AddWithValue("@p3", p3);
                com.Parameters.AddWithValue("@p4", p4);
                com.Parameters.AddWithValue("@p5", p5);
                com.Parameters.AddWithValue("@p6", p6);
                com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                return false;
            }
            //*/
            return true;
        }

        public bool Execute(string sql, string p1, string p2, string p3, string p4, string p5, string p6, string p7)
        {
            /*
            if (!connected)
            {
                return false;
            }
            try
            {
                if (con == null ||
                    con.State == ConnectionState.Closed)
                {
                    Connect();
                }
                SqlCommand com = new SqlCommand(sql, con);
                com.CommandTimeout = 1;
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.Parameters.AddWithValue("@p2", p2);
                com.Parameters.AddWithValue("@p3", p3);
                com.Parameters.AddWithValue("@p4", p4);
                com.Parameters.AddWithValue("@p5", p5);
                com.Parameters.AddWithValue("@p6", p6);
                com.Parameters.AddWithValue("@p7", p7);
                com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                return false;
            }
            //*/
            return true;
        }

        public bool Execute(string sql, string p1, string p2, string p3, string p4, string p5, string p6, string p7, string p8)
        {
            /*
            if (!connected)
            {
                return false;
            }
            try
            {
                if (con == null ||
                    con.State == ConnectionState.Closed)
                {
                    Connect();
                }
                SqlCommand com = new SqlCommand(sql, con);
                com.CommandTimeout = 1;
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.Parameters.AddWithValue("@p2", p2);
                com.Parameters.AddWithValue("@p3", p3);
                com.Parameters.AddWithValue("@p4", p4);
                com.Parameters.AddWithValue("@p5", p5);
                com.Parameters.AddWithValue("@p6", p6);
                com.Parameters.AddWithValue("@p7", p7);
                com.Parameters.AddWithValue("@p8", p8);
                com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                return false;
            }
            //*/
            return true;
        }

        public bool Execute(string sql, string p1, string p2, string p3, string p4, string p5, string p6, string p7, string p8, string p9)
        {
            /*
            if (!connected)
            {
                return false;
            }
            try
            {
                if (con == null ||
                    con.State == ConnectionState.Closed)
                {
                    Connect();
                }
                SqlCommand com = new SqlCommand(sql, con);
                com.CommandTimeout = 1;
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.Parameters.AddWithValue("@p2", p2);
                com.Parameters.AddWithValue("@p3", p3);
                com.Parameters.AddWithValue("@p4", p4);
                com.Parameters.AddWithValue("@p5", p5);
                com.Parameters.AddWithValue("@p6", p6);
                com.Parameters.AddWithValue("@p7", p7);
                com.Parameters.AddWithValue("@p8", p8);
                com.Parameters.AddWithValue("@p9", p9);
                com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                return false;
            }
            //*/
            return true;
        }

        public bool Execute(string sql, string p1, string p2, string p3, string p4, string p5, string p6, string p7, string p8, string p9, string p10)
        {
            /*
            if (!connected)
            {
                return false;
            }
            try
            {
                if (con == null ||
                    con.State == ConnectionState.Closed)
                {
                    Connect();
                }
                SqlCommand com = new SqlCommand(sql, con);
                com.CommandTimeout = 1;
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.Parameters.AddWithValue("@p2", p2);
                com.Parameters.AddWithValue("@p3", p3);
                com.Parameters.AddWithValue("@p4", p4);
                com.Parameters.AddWithValue("@p5", p5);
                com.Parameters.AddWithValue("@p6", p6);
                com.Parameters.AddWithValue("@p7", p7);
                com.Parameters.AddWithValue("@p8", p8);
                com.Parameters.AddWithValue("@p9", p9);
                com.Parameters.AddWithValue("@p10", p10);
                com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                return false;
            }
            //*/
            return true;
        }

        public SqlDataReader Read(SqlCommand com)
        {
            /*
            if (!connected)
            {
                return null;
            }
            try
            {
                if (con == null ||
                    con.State == ConnectionState.Closed)
                {
                    Connect();
                }
                com.Connection = con;
                return com.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                return null;
            }
            //*/
            return null;
        }

        // sql과 datagrid 개체를 받아 쿼리 결과를 datagrid에 출력한다.
        // grid의 스타일도 바뀌어 버리므로 성능 부담이 큰 Trace에만 적용한다.
        public bool Read(string sql, DataGridView grid)
        {
            string conStr = "server = " + server +
                                            "," + port +
                                            "; uid = " + uid +
                                            "; pwd = " + pwd +
                                            "; database = " + db + ";";
            SqlConnection con = new SqlConnection(conStr);
            try
            {
                con.Open();

                if (con == null ||
                    con.State == ConnectionState.Closed)
                {
                    return false;
                }
                SqlCommand com = new SqlCommand(sql, con);
                com.CommandTimeout = 10;
                com.Connection = con;
                SqlDataReader reader = com.ExecuteReader();


                grid.DataSource = null;
                grid.Rows.Clear();
                grid.ColumnCount = 0; // reader.FieldCount;

                /*
                int fieldCount = reader.FieldCount;
                while (reader.Read())
                {
                    string[] rsArr = new string[fieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        //Console.WriteLine(rs[i]);
                        rsArr[i] = reader[i].ToString();
                    }
                    grid.Rows.Add(rsArr);
                }
                //*/
                DataTable dt = new DataTable();
                dt.Load(reader);
                grid.DataSource = dt;

                if (reader != null)
                {
                    reader.Close();
                }
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }

            try
            {
                con.Close();
            }
            catch { }
            return false;
        }

        #endregion // JHRYU 안쓰는 함수


        public string[,] Read(string sql)
        {
            lock (lock_obj)     // DB 사용시 스레드 안전을 위해
            {
                try
                {
                    string conStr = "server = " + server +
                                    "," + port +
                                    "; uid = " + uid +
                                    "; pwd = " + pwd +
                                    "; database = " + db + ";";
                    SqlConnection con = new SqlConnection(conStr);
                    con.Open();

                    if (con == null ||
                        con.State == ConnectionState.Closed)
                    {
                        connected = false;
                        return null;
                        //Connect();
                    }
                    SqlCommand com = new SqlCommand(sql, con);
                    com.CommandTimeout = 3;
                    com.Connection = con;
                    SqlDataReader reader = com.ExecuteReader();
                    string[,] rtn = RsToArray(reader);
                    connected = true;
                    /*
                    int rowIndex = 0;
                    while (reader.Read())
                    {
                        Console.WriteLine("rowIndex : " + rowIndex);
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.WriteLine(reader[i]);
                            string col = reader[i] as string;
                            Console.WriteLine("rs " + rowIndex + "," + i + " : " + col);
                        }
                        rowIndex++;
                    }
                    //*/
                    if (reader != null)
                    {
                        reader.Close();
                    }
                    con.Close();
                    return rtn;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString() + "\n" + sql);
                    Console.WriteLine(ex.StackTrace);
                    Disconnect(); // 일단 연결 끊으면 다음 지령때 알아서 연결하겠지.
                    connected = false;
                    return null;
                }
            }
        }


        // JHRYU 20231113 DataSet Version
        public DataRowCollection Read_DataSet(string sql)
        {
            lock (lock_obj)     // DB 사용시 스레드 안전을 위해
            {
                try
                {
                    string conStr = "server = " + server +
                                    "," + port +
                                    "; uid = " + uid +
                                    "; pwd = " + pwd +
                                    "; database = " + db + ";";
                    SqlConnection con = new SqlConnection(conStr);
                    con.Open();

                    if (con == null ||
                        con.State == ConnectionState.Closed)
                    {
                        connected = false;
                        return null;
                        //Connect();
                    }
                    SqlCommand com = new SqlCommand(sql, con);
                    com.CommandTimeout = 10;
                    com.Connection = con;
                    SqlDataAdapter da = new SqlDataAdapter();

                    da.SelectCommand = com;

                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    con.Close();
                    connected = true;
                    return ds.Tables[0].Rows;
                }
                catch (Exception ex)
                {
                    FuncLog.WriteLog(ex.Message + "\n" + sql);
                    //Console.WriteLine(ex.StackTrace);
                    Disconnect(); // 일단 연결 끊으면 다음 지령때 알아서 연결하겠지.
                    connected = false;
                    return null;
                }

            }

        }


        /*
        * Parameter 전달 위해서는 sql문의 Parameter부분은 @p1,@p2... 순으로 명명되어야 함
       //*/

        public SqlDataReader Read(string sql, string p1)
        {
            /*
            if (!connected)
            {
                return null;
            }
            try
            {
                if (con == null ||
                    con.State == ConnectionState.Closed)
                {
                    Connect();
                }
                SqlCommand com = new SqlCommand(sql, con);
                com.CommandTimeout = 1;
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                return com.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                return null;
            }
            //*/
            return null;
        }

        public SqlDataReader Read(string sql, string p1, string p2)
        {
            /*
            if (!connected)
            {
                return null;
            }
            try
            {
                if (con == null ||
                    con.State == ConnectionState.Closed)
                {
                    Connect();
                }
                SqlCommand com = new SqlCommand(sql, con);
                com.CommandTimeout = 1;
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.Parameters.AddWithValue("@p2", p2);
                return com.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                return null;
            }
            //*/
            return null;
        }

        public SqlDataReader Read(string sql, string p1, string p2, string p3)
        {
            /*
            if (!connected)
            {
                return null;
            }
            try
            {
                if (con == null ||
                    con.State == ConnectionState.Closed)
                {
                    Connect();
                }
                SqlCommand com = new SqlCommand(sql, con);
                com.CommandTimeout = 1;
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.Parameters.AddWithValue("@p2", p2);
                com.Parameters.AddWithValue("@p3", p3);
                return com.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                return null;
            }
            //*/
            return null;
        }

        public SqlDataReader Read(string sql, string p1, string p2, string p3, string p4)
        {
            /*
            if (!connected)
            {
                return null;
            }
            try
            {
                if (con == null ||
                    con.State == ConnectionState.Closed)
                {
                    Connect();
                }
                SqlCommand com = new SqlCommand(sql, con);
                com.CommandTimeout = 1;
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.Parameters.AddWithValue("@p2", p2);
                com.Parameters.AddWithValue("@p3", p3);
                com.Parameters.AddWithValue("@p4", p4);
                return com.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                return null;
            }
            //*/
            return null;
        }

        public SqlDataReader Read(string sql, string p1, string p2, string p3, string p4, string p5)
        {
            /*
            if (!connected)
            {
                return null;
            }
            try
            {
                if (con == null ||
                    con.State == ConnectionState.Closed)
                {
                    Connect();
                }
                SqlCommand com = new SqlCommand(sql, con);
                com.CommandTimeout = 1;
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.Parameters.AddWithValue("@p2", p2);
                com.Parameters.AddWithValue("@p3", p3);
                com.Parameters.AddWithValue("@p4", p4);
                com.Parameters.AddWithValue("@p5", p5);
                return com.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                return null;
            }
            //*/
            return null;
        }

        public SqlDataReader Read(string sql, string p1, string p2, string p3, string p4, string p5, string p6)
        {
            /*
            if (!connected)
            {
                return null;
            }
            try
            {
                if (con == null ||
                    con.State == ConnectionState.Closed)
                {
                    Connect();
                }
                SqlCommand com = new SqlCommand(sql, con);
                com.CommandTimeout = 1;
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.Parameters.AddWithValue("@p2", p2);
                com.Parameters.AddWithValue("@p3", p3);
                com.Parameters.AddWithValue("@p4", p4);
                com.Parameters.AddWithValue("@p5", p5);
                com.Parameters.AddWithValue("@p6", p6);
                return com.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                return null;
            }
            //*/
            return null;
        }

        public SqlDataReader Read(string sql, string p1, string p2, string p3, string p4, string p5, string p6, string p7)
        {
            /*
            if (!connected)
            {
                return null;
            }
            try
            {
                if (con == null ||
                    con.State == ConnectionState.Closed)
                {
                    Connect();
                }
                SqlCommand com = new SqlCommand(sql, con);
                com.CommandTimeout = 1;
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.Parameters.AddWithValue("@p2", p2);
                com.Parameters.AddWithValue("@p3", p3);
                com.Parameters.AddWithValue("@p4", p4);
                com.Parameters.AddWithValue("@p5", p5);
                com.Parameters.AddWithValue("@p6", p6);
                com.Parameters.AddWithValue("@p7", p7);
                return com.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                return null;
            }
            //*/
            return null;
        }

        public SqlDataReader Read(string sql, string p1, string p2, string p3, string p4, string p5, string p6, string p7, string p8)
        {
            /*
            if (!connected)
            {
                return null;
            }
            try
            {
                if (con == null ||
                    con.State == ConnectionState.Closed)
                {
                    Connect();
                }
                SqlCommand com = new SqlCommand(sql, con);
                com.CommandTimeout = 1;
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.Parameters.AddWithValue("@p2", p2);
                com.Parameters.AddWithValue("@p3", p3);
                com.Parameters.AddWithValue("@p4", p4);
                com.Parameters.AddWithValue("@p5", p5);
                com.Parameters.AddWithValue("@p6", p6);
                com.Parameters.AddWithValue("@p7", p7);
                com.Parameters.AddWithValue("@p8", p8);
                return com.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                return null;
            }
            //*/
            return null;
        }

        public SqlDataReader Read(string sql, string p1, string p2, string p3, string p4, string p5, string p6, string p7, string p8, string p9)
        {
            /*
            if (!connected)
            {
                return null;
            }
            try
            {
                if (con == null ||
                    con.State == ConnectionState.Closed)
                {
                    Connect();
                }
                SqlCommand com = new SqlCommand(sql, con);
                com.CommandTimeout = 1;
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.Parameters.AddWithValue("@p2", p2);
                com.Parameters.AddWithValue("@p3", p3);
                com.Parameters.AddWithValue("@p4", p4);
                com.Parameters.AddWithValue("@p5", p5);
                com.Parameters.AddWithValue("@p6", p6);
                com.Parameters.AddWithValue("@p7", p7);
                com.Parameters.AddWithValue("@p8", p8);
                com.Parameters.AddWithValue("@p9", p9);
                return com.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                return null;
            }
            //*/
            return null;
        }

        public SqlDataReader Read(string sql, string p1, string p2, string p3, string p4, string p5, string p6, string p7, string p8, string p9, string p10)
        {
            /*
            if (!connected)
            {
                return null;
            }
            try
            {
                if (con == null ||
                    con.State == ConnectionState.Closed)
                {
                    Connect();
                }
                SqlCommand com = new SqlCommand(sql, con);
                com.CommandTimeout = 1;
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.Parameters.AddWithValue("@p2", p2);
                com.Parameters.AddWithValue("@p3", p3);
                com.Parameters.AddWithValue("@p4", p4);
                com.Parameters.AddWithValue("@p5", p5);
                com.Parameters.AddWithValue("@p6", p6);
                com.Parameters.AddWithValue("@p7", p7);
                com.Parameters.AddWithValue("@p8", p8);
                com.Parameters.AddWithValue("@p9", p9);
                com.Parameters.AddWithValue("@p10", p10);
                return com.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                return null;
            }
            //*/
            return null;
        }

        public string[,] RsToArray(SqlDataReader rs)
        {
            /*
            string str = RsToCsv(rs);
            string[] strRow = str.Split(new char[] { '\n' });
            if (strRow.Length == 0 ||
                str == "")
            {
                return new string[0, 0];
            }
            string[] strField = strRow[0].Split(new char[] { '\t' });
            string[,] rtn = new string[strRow.Length, strField.Length];
            for (int j = 0; j < strRow.Length; j++)
            {
                strField = strRow[j].Split(new char[] { '\t' });
                for (int i = 0; i < strField.Length; i++)
                {
                    rtn[j, i] = strField[i].Trim();
                }
            }
            //*/
            int fieldCount = rs.FieldCount;
            ConcurrentQueue<string[]> rsQueue = new ConcurrentQueue<string[]>(); // row count를 몰라 배열 갯수지정할 수 없기 때문에 일단 큐에 저장해 둔다

            int rowIndex = 0;
            while (rs.Read())
            {
                string[] rsArr = new string[fieldCount];
                //Console.WriteLine("rowIndex : " + rowIndex);
                for (int i = 0; i < rs.FieldCount; i++)
                {
                    //Console.WriteLine(rs[i]);
                    rsArr[i] = rs[i].ToString();
                }
                rsQueue.Enqueue(rsArr);
                rowIndex++;
            }

            string[,] rtn = new string[rsQueue.Count, fieldCount];
            string[] arr = new string[fieldCount];
            rowIndex = 0;
            while (rsQueue.TryDequeue(out arr))
            {
                for (int i = 0; i < fieldCount; i++)
                {
                    rtn[rowIndex, i] = arr[i];
                }
                rowIndex++;
            }
            return rtn;
        }

        public string RsToCsv(SqlDataReader rs)
        {
            string rtn = "";

            int rowIndex = 0;
            while (rs.Read())
            {
                //Console.WriteLine("rowIndex : " + rowIndex);
                if (rowIndex > 0)
                {
                    rtn += "\n";
                }
                for (int i = 0; i < rs.FieldCount; i++)
                {
                    //Console.WriteLine(rs[i]);
                    if (i > 0)
                    {
                        rtn += "\t";
                    }
                    rtn += rs[i].ToString();
                }
                rowIndex++;
            }

            return rtn;
        }

    }
}
