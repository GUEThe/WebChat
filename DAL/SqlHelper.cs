using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace DAL
{
    public static class  SqlHelper
    {
        public static readonly string connstr =
            ConfigurationManager.ConnectionStrings["connstr"].ConnectionString;

        public static SqlConnection OpenConnection()
        {
            SqlConnection conn = new SqlConnection(connstr);
            conn.Open();
            return conn;
        }

        public static int ExecuteNonQuery(string cmdText,
            params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = cmdText;
                    cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static object ExecuteScalar(string cmdText,
            params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = cmdText;
                    cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteScalar();
                }
            }
        }

        public static DataTable ExecuteDataTable(string cmdText,
            params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = cmdText;
                    cmd.Parameters.AddRange(parameters);
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public static DataSet ExecuteDataSet(string cmdText,
            params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = cmdText;
                    cmd.Parameters.AddRange(parameters);
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        return ds;
                    }
                }
            }
        }

        /////////////////////////分页//////////////////////////
        public static DataTable ExecuteDataTable(int page_count, int page_num, string cmdText,
            params SqlParameter[] parameters)
        {
            //page_count: 每页数目
            //page_num: 页码
            //必须有 id 列

            string[] cmdTextArr = cmdText.Split(new string[] { "select", "from", "where", "order by" }, StringSplitOptions.RemoveEmptyEntries);
            if (cmdTextArr.Length == 2)
            {
                cmdText = "select top " + page_count.ToString() + cmdTextArr[0] + "from" + cmdTextArr[1] + " where id not in"
                    + "(select top " + ((page_num - 1) * page_count).ToString() + " id from" + cmdTextArr[1] + ")";
            }
            else if (cmdTextArr.Length == 3)
            {
                if (cmdText.IndexOf("where") > -1)
                {
                    cmdText = "select top " + page_count.ToString() + cmdTextArr[0] + "from" + cmdTextArr[1] + "where" + cmdTextArr[2] + " and id not in"
                        + "(select top " + ((page_num - 1) * page_count).ToString() + " id from" + cmdTextArr[1] + "where" + cmdTextArr[2] + ")";
                }
                else if (cmdText.IndexOf("order by") > -1)
                {
                    cmdText = "select top " + page_count.ToString() + cmdTextArr[0] + "from" + cmdTextArr[1] + "where id not in"
                        + "(select top " + ((page_num - 1) * page_count).ToString() + " id from" + cmdTextArr[1] + "order by" + cmdTextArr[2] + ")"
                        + "order by" + cmdTextArr[2];
                }
            }
            else if (cmdTextArr.Length == 4)
            {
                cmdText = "select top " + page_count.ToString() + cmdTextArr[0] + "from" + cmdTextArr[1] + "where" + cmdTextArr[2] + " and id not in"
                    + "(select top " + ((page_num - 1) * page_count).ToString() + " id from" + cmdTextArr[1] + "where" + cmdTextArr[2] + "order by" + cmdTextArr[3] + ")"
                    + "order by" + cmdTextArr[3];
            }

            using (SqlConnection conn = new SqlConnection(connstr))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = cmdText;
                    cmd.Parameters.AddRange(parameters);
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public static object ToDBValue(this object value)
        {
            return value == null ? DBNull.Value : value;
        }

        public static object FromDBValue(this object dbValue)
        {
            return dbValue == DBNull.Value ? null : dbValue;
        }

        /////////////////////////事务//////////////////////////
        public class TranItem
        {
            public SqlConnection conn { get; set; }
            public SqlTransaction tran { get; set; }
            public SqlCommand cmd { get; set; }
        }
        public static TranItem TranConnection()
        {
            TranItem tran_item = new TranItem();
            tran_item.conn = new SqlConnection(connstr);
            tran_item.conn.Open();
            tran_item.tran = tran_item.conn.BeginTransaction();
            tran_item.cmd = tran_item.conn.CreateCommand();
            tran_item.cmd.Transaction = tran_item.tran;
            return tran_item;
        }
        public static object TranExecuteNonQuery(TranItem tran_item,
            string cmdText, params SqlParameter[] parameters)
        {
            tran_item.cmd.CommandText = cmdText;
            tran_item.cmd.Parameters.Clear();
            tran_item.cmd.Parameters.AddRange(parameters);
            return tran_item.cmd.ExecuteNonQuery();
        }
        public static object TranExecuteScalar(TranItem tran_item,
            string cmdText, params SqlParameter[] parameters)
        {
            tran_item.cmd.CommandText = cmdText;
            tran_item.cmd.Parameters.Clear();
            tran_item.cmd.Parameters.AddRange(parameters);
            return tran_item.cmd.ExecuteScalar();
        }
        public static void TranEnd(TranItem tran_item)
        {
            tran_item.tran.Commit();
            tran_item.conn.Close();
            tran_item.tran.Dispose();
        }
        /////////////////////////事务//////////////////////////
    }
}
