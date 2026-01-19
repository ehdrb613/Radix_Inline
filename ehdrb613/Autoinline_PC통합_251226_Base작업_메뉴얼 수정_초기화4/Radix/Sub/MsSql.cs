using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Radix
{
    class MssqlClass
    {
        public bool connected = false;

        private string server = "127.0.0.1";
        private string port = "1433";
        private string uid = "sa";
        private string pwd = "radix5243";
        private string db = "radix";


        private SqlConnection con = null;

        public MssqlClass()
        {
            // 하드코딩된 연결 정보로 시작
        }

        public MssqlClass(string s, string p, string i, string w, string d)
        {
            server = s;
            port = p;
            uid = i;
            pwd = w;
            db = d;
        }

        public bool Connect()
        {
            try
            {
                string conStr = "server = " + server +
                                "," + port +
                                "; uid = " + uid +
                                "; pwd = " + pwd +
                                "; database = " + db + ";";
                con = new SqlConnection(conStr);
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
        }

        public void Close()
        {
            connected = false;
            con.Close();
        }

        public bool Excute(SqlCommand com)
        {
            if (!connected)
            {
                return false;
            }
            try
            {
                com.Connection = con;
                com.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public bool Execute(string sql)
        {
            if (!connected)
            {
                return false;
            }
            try
            {
                SqlCommand com = new SqlCommand(sql, con);
                com.Connection = con;
                com.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        /*
         * Parameter 전달 위해서는 sql문의 Parameter부분은 @p1,@p2... 순으로 명명되어야 함
        //*/

        public bool Execute(string sql, string p1)
        {
            if (!connected)
            {
                return false;
            }
            try
            {
                SqlCommand com = new SqlCommand(sql, con);
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public bool Execute(string sql, string p1, string p2)
        {
            if (!connected)
            {
                return false;
            }
            try
            {
                SqlCommand com = new SqlCommand(sql, con);
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.Parameters.AddWithValue("@p2", p2);
                com.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public bool Execute(string sql, string p1, string p2, string p3)
        {
            if (!connected)
            {
                return false;
            }
            try
            {
                SqlCommand com = new SqlCommand(sql, con);
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.Parameters.AddWithValue("@p2", p2);
                com.Parameters.AddWithValue("@p3", p3);
                com.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public bool Execute(string sql, string p1, string p2, string p3, string p4)
        {
            if (!connected)
            {
                return false;
            }
            try
            {
                SqlCommand com = new SqlCommand(sql, con);
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.Parameters.AddWithValue("@p2", p2);
                com.Parameters.AddWithValue("@p3", p3);
                com.Parameters.AddWithValue("@p4", p4);
                com.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public bool Execute(string sql, string p1, string p2, string p3, string p4, string p5)
        {
            if (!connected)
            {
                return false;
            }
            try
            {
                SqlCommand com = new SqlCommand(sql, con);
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.Parameters.AddWithValue("@p2", p2);
                com.Parameters.AddWithValue("@p3", p3);
                com.Parameters.AddWithValue("@p4", p4);
                com.Parameters.AddWithValue("@p5", p5);
                com.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public bool Execute(string sql, string p1, string p2, string p3, string p4, string p5, string p6)
        {
            if (!connected)
            {
                return false;
            }
            try
            {
                SqlCommand com = new SqlCommand(sql, con);
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.Parameters.AddWithValue("@p2", p2);
                com.Parameters.AddWithValue("@p3", p3);
                com.Parameters.AddWithValue("@p4", p4);
                com.Parameters.AddWithValue("@p5", p5);
                com.Parameters.AddWithValue("@p6", p6);
                com.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public bool Execute(string sql, string p1, string p2, string p3, string p4, string p5, string p6, string p7)
        {
            if (!connected)
            {
                return false;
            }
            try
            {
                SqlCommand com = new SqlCommand(sql, con);
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
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public bool Execute(string sql, string p1, string p2, string p3, string p4, string p5, string p6, string p7, string p8)
        {
            if (!connected)
            {
                return false;
            }
            try
            {
                SqlCommand com = new SqlCommand(sql, con);
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
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public bool Execute(string sql, string p1, string p2, string p3, string p4, string p5, string p6, string p7, string p8, string p9)
        {
            if (!connected)
            {
                return false;
            }
            try
            {
                SqlCommand com = new SqlCommand(sql, con);
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
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public bool Execute(string sql, string p1, string p2, string p3, string p4, string p5, string p6, string p7, string p8, string p9, string p10)
        {
            if (!connected)
            {
                return false;
            }
            try
            {
                SqlCommand com = new SqlCommand(sql, con);
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
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public SqlDataReader Read(SqlCommand com)
        {
            if (!connected)
            {
                return null;
            }
            try
            {
                com.Connection = con;
                return com.ExecuteReader();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public SqlDataReader Read(string sql)
        {
            if (!connected)
            {
                return null;
            }
            try
            {
                SqlCommand com = new SqlCommand(sql, con);
                com.Connection = con;
                SqlDataReader reader = com.ExecuteReader();
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
                return reader;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /*
        * Parameter 전달 위해서는 sql문의 Parameter부분은 @p1,@p2... 순으로 명명되어야 함
       //*/

        public SqlDataReader Read(string sql, string p1)
        {
            if (!connected)
            {
                return null;
            }
            try
            {
                SqlCommand com = new SqlCommand(sql, con);
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                return com.ExecuteReader();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public SqlDataReader Read(string sql, string p1, string p2)
        {
            if (!connected)
            {
                return null;
            }
            try
            {
                SqlCommand com = new SqlCommand(sql, con);
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.Parameters.AddWithValue("@p2", p2);
                return com.ExecuteReader();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public SqlDataReader Read(string sql, string p1, string p2, string p3)
        {
            if (!connected)
            {
                return null;
            }
            try
            {
                SqlCommand com = new SqlCommand(sql, con);
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.Parameters.AddWithValue("@p2", p2);
                com.Parameters.AddWithValue("@p3", p3);
                return com.ExecuteReader();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public SqlDataReader Read(string sql, string p1, string p2, string p3, string p4)
        {
            if (!connected)
            {
                return null;
            }
            try
            {
                SqlCommand com = new SqlCommand(sql, con);
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.Parameters.AddWithValue("@p2", p2);
                com.Parameters.AddWithValue("@p3", p3);
                com.Parameters.AddWithValue("@p4", p4);
                return com.ExecuteReader();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public SqlDataReader Read(string sql, string p1, string p2, string p3, string p4, string p5)
        {
            if (!connected)
            {
                return null;
            }
            try
            {
                SqlCommand com = new SqlCommand(sql, con);
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.Parameters.AddWithValue("@p2", p2);
                com.Parameters.AddWithValue("@p3", p3);
                com.Parameters.AddWithValue("@p4", p4);
                com.Parameters.AddWithValue("@p5", p5);
                return com.ExecuteReader();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public SqlDataReader Read(string sql, string p1, string p2, string p3, string p4, string p5, string p6)
        {
            if (!connected)
            {
                return null;
            }
            try
            {
                SqlCommand com = new SqlCommand(sql, con);
                com.Connection = con;
                com.Parameters.AddWithValue("@p1", p1);
                com.Parameters.AddWithValue("@p2", p2);
                com.Parameters.AddWithValue("@p3", p3);
                com.Parameters.AddWithValue("@p4", p4);
                com.Parameters.AddWithValue("@p5", p5);
                com.Parameters.AddWithValue("@p6", p6);
                return com.ExecuteReader();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public SqlDataReader Read(string sql, string p1, string p2, string p3, string p4, string p5, string p6, string p7)
        {
            if (!connected)
            {
                return null;
            }
            try
            {
                SqlCommand com = new SqlCommand(sql, con);
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
            catch (Exception e)
            {
                return null;
            }
        }

        public SqlDataReader Read(string sql, string p1, string p2, string p3, string p4, string p5, string p6, string p7, string p8)
        {
            if (!connected)
            {
                return null;
            }
            try
            {
                SqlCommand com = new SqlCommand(sql, con);
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
            catch (Exception e)
            {
                return null;
            }
        }

        public SqlDataReader Read(string sql, string p1, string p2, string p3, string p4, string p5, string p6, string p7, string p8, string p9)
        {
            if (!connected)
            {
                return null;
            }
            try
            {
                SqlCommand com = new SqlCommand(sql, con);
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
            catch (Exception e)
            {
                return null;
            }
        }

        public SqlDataReader Read(string sql, string p1, string p2, string p3, string p4, string p5, string p6, string p7, string p8, string p9, string p10)
        {
            if (!connected)
            {
                return null;
            }
            try
            {
                SqlCommand com = new SqlCommand(sql, con);
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
            catch (Exception e)
            {
                return null;
            }
        }

        public string[,] RsToArry(SqlDataReader rs)
        {
            string str = RsToCsv(rs);
            string[] strRow = str.Split(new char[] { '\n' });
            if (strRow.Length == 0)
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
                    rtn += rs[i] as string;
                }
                rowIndex++;
            }

            return rtn;
        }

    }
}
