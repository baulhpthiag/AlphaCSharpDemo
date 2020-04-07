using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;

namespace MyHelper
{
    //需要安装 mysql.data

    public static class MySqlHelper
    {
        //如果没有配置文件，要手动创建一个
        //右键项目 - 添加 - 组件 - 应用程序配置文件
        //然后配置App.config里面的连接字符串
        private static string connectionString = ConfigurationManager.AppSettings["mysqldemo"];

        public static void TestConnectDb()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                }
                catch (MySqlException ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public static DataTable GetDataTable(string sql)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    // SqlDataAdapter是 DataSet和 SQL Server之间的桥接器，用于检索和保存数据。
                    //SqlDataAdapter通过对数据源使用适当的Transact - SQL语句映射 Fill
                    //（它可更改DataSet中的数据以匹配数据源中的数据）和 Update
                    //（它可更改数据源中的数据以匹配 DataSet中的数据）来提供这一桥接。
                    //当SqlDataAdapter填充 DataSet时，它为返回的数据创建必需的表和列
                    MySqlDataAdapter command = new MySqlDataAdapter(sql, connection);

                    command.Fill(ds, "ds");
                }
                catch (MySqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds.Tables[0];
            }
        }

        public static DataTable GetDataTable(string sql, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    DataSet ds = new DataSet();
                    try
                    {

                        PrepareCommand(cmd, connection, null, sql, cmdParms);
                        MySqlDataAdapter command = new MySqlDataAdapter(cmd);
                        command.Fill(ds, "ds");
                        return ds.Tables[0];
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (MySqlException E)
                    {
                        throw new Exception(E.Message);
                    }
                }
            }
        }

        private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, string cmdText, MySqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {
                foreach (MySqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

    }
}
