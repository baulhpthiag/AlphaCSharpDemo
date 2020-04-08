using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Windows.Forms;
namespace HRPM.Utilities
{
    /// <summary>
    /// SqlHelper类提供很高的数据访问性能
    /// 使用SqlClient类的通/// <summary>
    /// SqlHelper类提供很高的数据访问性能,
    /// 使用SqlClient类的通用定义.
    /// </summary>
    public abstract class DbHelperSQL
    {

        //private static string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString; //定义读取App.config配置文件的数据库连接字符串   		
        public static string connectionString;

        // 数据链接字符串从配置文件中取得，这里注释掉
        // </summary>
        // <returns></returns>
        public static void GetConnectionString(string IP, string Port)
        {
            connectionString = "Server=" + IP + "," + Port + ";DataBase=HRPM;Uid=HRPMUser;Pwd =HRPMUser_123456";
        }
        //存贮Cache缓存的Hashtable集合
        private static readonly Hashtable parmCache = Hashtable.Synchronized(new Hashtable());
        /// <summary>
        /// 在缓存中新增参数数组
        /// </summary>
        /// <param name="cacheKey">参数的Key</param>
        /// <param name="cmdParms">参数数组</param>
        public static void CacheParameters(string cacheKey, params SqlParameter[] commandParameters)
        {
            parmCache[cacheKey] = commandParameters;
        }
        /// <summary>
        /// 提取缓存的参数数组
        /// </summary>
        /// <param name="cacheKey">查找缓存的key</param>
        /// <returns>返回被缓存的参数数组</returns>
        public static SqlParameter[] GetCachedParameters(string cacheKey)
        {
            SqlParameter[] cachedParms = (SqlParameter[])parmCache[cacheKey];
            if (cachedParms == null)
            {
                return null;
            }
            SqlParameter[] clonedParms = new SqlParameter[cachedParms.Length];
            for (int i = 0, j = cachedParms.Length; i < j; i++)
            {
                clonedParms[i] = (SqlParameter)((ICloneable)cachedParms[i]).Clone();
            }
            return clonedParms;
        }

        /// <summary>
        /// 提供一个SqlCommand对象的设置
        /// </summary>
        /// <param name="cmd">SqlCommand对象</param>
        /// <param name="conn">SqlConnection 对象</param>
        /// <param name="trans">SqlTransaction 对象</param>
        /// <param name="cmdType">CommandType 如存贮过程，T-SQL</param>
        /// <param name="cmdText">存贮过程名或查询串</param>
        /// <param name="cmdParms">命令中用到的参数集</param>
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
            {
                cmd.Transaction = trans;
            }
            cmd.CommandType = cmdType;
            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                {
                    cmd.Parameters.Add(parm);
                }
            }
        }
        public static DataSet GetDateSet(string sql, string TableName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    connection.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        using (DataSet ds = new DataSet())
                        {
                            try
                            {
                                da.Fill(ds, TableName);
                                connection.Close();
                            }
                            catch (System.Data.SqlClient.SqlException ex)
                            {
                                throw new Exception(ex.Message);
                            }
                            return ds;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static DataSet GetDateSet(string SQLString, string TableName, params SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, connection, null, CommandType.Text, SQLString, cmdParms);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    using (DataSet ds = new DataSet())
                    {
                        try
                        {
                            da.Fill(ds, TableName);
                            cmd.Parameters.Clear();
                        }
                        catch (System.Data.SqlClient.SqlException ex)
                        {
                            throw new Exception(ex.Message);
                        }
                        return ds;
                    }
                }
            }
        }
        /// <summary>
        /// 使用连接字符串，执行一个SqlCommand命令（没有记录返回）
        /// 使用提供的参数集.
        /// </summary>
        /// <remarks>
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的SqlConnection连接串</param>
        /// <param name="SQLString">存贮过程名称或是一个T-SQL语句串</param>
        /// <returns>受此命令影响的行数</returns>
        public static int ExecuteNonQuery(string SQLString)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    PrepareCommand(cmd, conn, null, CommandType.Text, SQLString, null);
                    int val = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    return val;
                }
            }
        }
        /// <summary>
        /// 使用连接字符串，执行一个SqlCommand命令（没有记录返回）
        /// 使用提供的参数集.
        /// </summary>
        /// <remarks>
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的SqlConnection连接串</param>
        /// <param name="SQLString">存贮过程名称或是一个T-SQL语句串</param>
        /// <param name="commandParameters">执行命令的参数集</param>
        /// <returns>受此命令影响的行数</returns>
        public static int ExecuteNonQuery(string SQLString, params SqlParameter[] commandParameters)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    PrepareCommand(cmd, conn, null, CommandType.Text, SQLString, commandParameters);
                    int val = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    return val;
                }
            }
        }
        /// <summary>
        /// 使用连接字符串，执行一个SqlCommand命令（没有记录返回）
        /// 使用提供的参数集.
        /// </summary>
        /// <remarks>
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的SqlConnection连接串</param>
        /// <param name="commandType">命令类型CommandType(stored procedure, text, etc.)</param>
        /// <param name="SQLString">存贮过程名称或是一个T-SQL语句串</param>
        /// <param name="commandParameters">执行命令的参数集</param>
        /// <returns>受此命令影响的行数</returns>
        public static int ExecuteNonQuery(CommandType cmdType, string SQLString, params SqlParameter[] commandParameters)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    PrepareCommand(cmd, conn, null, cmdType, SQLString, commandParameters);
                    int val = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    return val;
                }
            }
        }
        /// <summary>
        /// 在一个存在的连接上执行数据库的命令操作
        /// 使用提供的参数集.
        /// </summary>
        /// <remarks>
        ///  int result = ExecuteNonQuery(connection, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="conn">一个存在的数据库连接对象</param>
        /// <param name="commandType">命令类型CommandType (stored procedure, text, etc.)</param>
        /// <param name="SQLString">存贮过程名称或是一个T-SQL语句串</param>
        /// <param name="commandParameters">执行命令的参数集</param>
        /// <returns>受此命令影响的行数</returns>
        public static int ExecuteNonQuery(SqlConnection connection, CommandType cmdType, string SQLString, params SqlParameter[] commandParameters)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                PrepareCommand(cmd, connection, null, cmdType, SQLString, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 在一个连接串上执行一个命令，返回表中第一行，第一列的值
        /// 使用提供的参数.
        /// </summary>
        /// <remarks>
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="SQLString">存贮过程名称或是一个T-SQL语句串</param>  
        /// <returns>返回的对象，在使用时记得类型转换</returns>
        public static object ExecuteScalar(string SQLString)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    PrepareCommand(cmd, connection, null, CommandType.Text, SQLString, null);
                    object val = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    return val;
                }
            }
        }
        /// <summary>
        /// 在一个连接串上执行一个命令，返回表中第一行，第一列的值
        /// 使用提供的参数.
        /// </summary>
        /// <remarks>
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的SqlConnection连接串</param>
        /// <param name="SQLString">存贮过程名称或是一个T-SQL语句串</param>
        /// <param name="commandParameters">执行命令的参数集</param>        
        /// <returns>返回的对象，在使用时记得类型转换</returns>
        public static object ExecuteScalar(string SQLString, params SqlParameter[] commandParameters)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    PrepareCommand(cmd, connection, null, CommandType.Text, SQLString, commandParameters);
                    object val = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    return val;
                }
            }
        }

        /// <summary>
        /// 在一个连接串上执行一个命令，返回表中第一行，第一列的值
        /// 使用提供的参数.
        /// </summary>
        /// <remarks>
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的SqlConnection连接串</param>
        /// <param name="commandType">命令类型CommandType(stored procedure, text, etc.)</param>
        /// <param name="SQLString">存贮过程名称或是一个T-SQL语句串</param>
        /// <param name="commandParameters">执行命令的参数集</param>        
        /// <returns>返回的对象，在使用时记得类型转换</returns>
        public static object ExecuteScalar(CommandType cmdType, string SQLString, params SqlParameter[] commandParameters)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    PrepareCommand(cmd, connection, null, cmdType, SQLString, commandParameters);
                    object val = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    return val;
                }
            }
        }

        /// <summary>
        /// 在一个连接上执行一个命令，返回表中第一行，第一列的值
        /// 使用提供的参数.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的SqlConnection连接</param>
        /// <param name="commandType">命令类型CommandType(stored procedure, text, etc.)</param>
        /// <param name="SQLString">存贮过程名称或是一个T-SQL语句串</param>
        /// <param name="commandParameters">执行命令的参数集</param>        
        /// <returns>返回的对象，在使用时记得类型转换</returns>
        public static object ExecuteScalar(SqlConnection connection, CommandType cmdType, string SQLString, params SqlParameter[] commandParameters)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                PrepareCommand(cmd, connection, null, cmdType, SQLString, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 在一个事务的连接上执行数据库的命令操作
        /// 使用提供的参数集.
        /// </summary>
        /// <remarks>
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="trans">一个存在的事务</param>
        /// <param name="commandType">命令类型CommandType (stored procedure, text, etc.)</param>
        /// <param name="SQLString">存贮过程名称或是一个T-SQL语句串</param>
        /// <returns>受此命令影响的行数</returns>
        public static int ExecuteNonQuery(SqlTransaction trans, string SQLString)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                PrepareCommand(cmd, trans.Connection, trans, CommandType.Text, SQLString, null);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 在一个事务的连接上执行数据库的命令操作
        /// 使用提供的参数集.
        /// </summary>
        /// <remarks>
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="trans">一个存在的事务</param>
        /// <param name="commandType">命令类型CommandType (stored procedure, text, etc.)</param>
        /// <param name="SQLString">存贮过程名称或是一个T-SQL语句串</param>
        /// <param name="commandParameters">执行命令的参数集</param>
        /// <returns>受此命令影响的行数</returns>
        public static int ExecuteNonQuery(SqlTransaction trans, string SQLString, params SqlParameter[] commandParameters)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                PrepareCommand(cmd, trans.Connection, trans, CommandType.Text, SQLString, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 在一个事务的连接上执行数据库的命令操作
        /// 使用提供的参数集.
        /// </summary>
        /// <remarks>
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="trans">一个存在的事务</param>
        /// <param name="commandType">命令类型CommandType (stored procedure, text, etc.)</param>
        /// <param name="SQLString">存贮过程名称或是一个T-SQL语句串</param>
        /// <param name="commandParameters">执行命令的参数集</param>
        /// <returns>受此命令影响的行数</returns>
        public static int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, string SQLString, params SqlParameter[] commandParameters)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                PrepareCommand(cmd, trans.Connection, trans, cmdType, SQLString, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 在一个连接串上执行一个命令，返回一个SqlDataReader对象
        /// 使用提供的参数.
        /// </summary>
        /// <remarks>
        ///  SqlDataReader r = ExecuteReader(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="SQLString">存贮过程名称或是一个T-SQL语句串</param>
        /// <returns>一个结果集对象SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string SQLString)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                SqlConnection conn = new SqlConnection(connectionString);
                // 如果不存在要查询的对象，则发生异常
                // 连接要关闭
                // CommandBehavior.CloseConnection在异常时不发生作用
                try
                {
                    PrepareCommand(cmd, conn, null, CommandType.Text, SQLString, null);
                    SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    cmd.Parameters.Clear();
                    return rdr;
                }
                catch
                {
                    conn.Close();
                    throw;
                }
            }
        }

        /// <summary>
        /// 在一个连接串上执行一个命令，返回一个SqlDataReader对象
        /// 使用提供的参数.
        /// </summary>
        /// <remarks>
        ///  SqlDataReader r = ExecuteReader(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的SqlConnection连接串</param>
        /// <param name="SQLString">存贮过程名称或是一个T-SQL语句串</param>
        /// <param name="commandParameters">执行命令的参数集</param>
        /// <returns>一个结果集对象SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string SQLString, params SqlParameter[] commandParameters)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                SqlConnection conn = new SqlConnection(connectionString);
                // 如果不存在要查询的对象，则发生异常
                // 连接要关闭
                // CommandBehavior.CloseConnection在异常时不发生作用
                try
                {
                    PrepareCommand(cmd, conn, null, CommandType.Text, SQLString, commandParameters);
                    SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    cmd.Parameters.Clear();
                    return rdr;
                }
                catch
                {
                    conn.Close();
                    throw;
                }
            }
        }

        /// <summary>
        /// 在一个连接串上执行一个命令，返回一个SqlDataReader对象
        /// 使用提供的参数.
        /// </summary>
        /// <remarks>
        ///  SqlDataReader r = ExecuteReader(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="commandType">命令类型CommandType(stored procedure, text, etc.)</param>
        /// <param name="SQLString">存贮过程名称或是一个T-SQL语句串</param>
        /// <param name="commandParameters">执行命令的参数集</param>
        /// <returns>一个结果集对象SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(CommandType cmdType, string SQLString, params SqlParameter[] commandParameters)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                SqlConnection conn = new SqlConnection(connectionString);
                // 如果不存在要查询的对象，则发生异常
                // 连接要关闭
                // CommandBehavior.CloseConnection在异常时不发生作用
                try
                {
                    PrepareCommand(cmd, conn, null, cmdType, SQLString, commandParameters);
                    SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    cmd.Parameters.Clear();
                    return rdr;
                }
                catch
                {
                    conn.Close();
                    throw;
                }
            }
        }
        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(SQLString, connection))
                    {
                        da.Fill(ds, "ds");
                    }
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString, int Times)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(SQLString, connection))
                    {
                        da.SelectCommand.CommandTimeout = Times;
                        da.Fill(ds, "ds");
                    }
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString, params SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, connection, null, CommandType.Text, SQLString, cmdParms);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    using (DataSet ds = new DataSet())
                    {
                        try
                        {
                            da.Fill(ds, "ds");
                            cmd.Parameters.Clear();
                        }
                        catch (System.Data.SqlClient.SqlException ex)
                        {
                             throw new Exception(ex.Message);
                        }
                        return ds;
                    }
                }
            }
        }

        /// <summary>
        /// 返回某一列的最大值
        /// </summary>
        /// <param name="strID">列名称</param>
        /// <param name="TableName">表名称</param>
        /// <returns>列最大值</returns>
        public static int GetMaxID(string strField, string TableName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string strSQLField = "SELECT COUNT(*) FROM syscolumns WHERE id=OBJECT_ID(N'" + TableName + "') AND name= '" + strField + "'";
                object objField = ExecuteScalar(strSQLField);
                if (objField != null)
                {
                    string strSQL = "SELECT MAX(" + strField + ") FROM " + TableName;
                    object obj = ExecuteScalar(strSQL);
                    if (obj.ToString().Length > 0)
                    {
                        return Convert.ToInt32(obj);
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 使用连接字符串，执行一个SqlCommand命令（没有记录返回）
        /// 使用提供的参数集.
        /// </summary>
        /// <remarks>
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的SqlConnection连接串</param>
        /// <param name="SQLString">存贮过程名称或是一个T-SQL语句串</param>
        /// <returns>受此命令影响的行数</returns>
        public static bool Exists(string SQLString)
        {
            object obj = GetSingle(SQLString);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 使用连接字符串，执行一个SqlCommand命令（没有记录返回）
        /// 使用提供的参数集.
        /// </summary>
        /// <remarks>
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的SqlConnection连接串</param>
        /// <param name="SQLString">存贮过程名称或是一个T-SQL语句串</param>
        /// <param name="commandParameters">执行命令的参数集</param>
        /// <returns>受此命令影响的行数</returns>
        public static bool Exists(string SQLString, params SqlParameter[] commandParameters)
        {
            object obj = GetSingle(SQLString, commandParameters);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object GetSingle(string SQLString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Dispose();
                        connection.Close();
                    }
                }
            }
        }

        /// <summary> 
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object GetSingle(string SQLString, params SqlParameter[] commandParameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, conn, null, CommandType.Text, SQLString, commandParameters);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 在一个事务的连接上执行数据库的命令操作
        /// 使用提供的参数集.
        /// </summary>
        /// <remarks>
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="trans">一个存在的事务</param>
        /// <param name="commandType">命令类型CommandType (stored procedure, text, etc.)</param>
        /// <param name="SQLString">存贮过程名称或是一个T-SQL语句串</param>
        /// <param name="commandParameters">执行命令的参数集</param>
        /// <returns>受此命令影响的行数</returns>
        public static object GetSingle(SqlTransaction trans, string SQLString, params SqlParameter[] commandParameters)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                PrepareCommand(cmd, trans.Connection, trans, CommandType.Text, SQLString, commandParameters);
                object obj = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                {
                    return null;
                }
                else
                {
                    return obj;
                }
            }
        }
        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }
        /// <summary>
        /// 执行SQL语句带参数语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL带参数语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString, params SqlParameter[] commandParameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, conn, null, CommandType.Text, SQLString, commandParameters);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        throw e;
                    }
                }
            }
        }
        #region 获取对象泛型集合
        /// <summary>
        /// 获取对象泛型集合
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static List<T> ExecuteList<T>(IDataReader reader)
        {
            List<T> list = new List<T>();
            try
            {
                while (reader.Read())
                {
                    T obj = ExecuteDataReader<T>(reader);
                    list.Add(obj);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            reader.Close();
            return list;
        }

        /// <summary>
        /// 执行返回一条记录的泛型对象
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="reader">只进只读对象</param>
        /// <returns>泛型对象</returns>
        public static T ExecuteDataReader<T>(IDataReader reader)
        {
            T obj;
            try
            {
                Type type = typeof(T);
                obj = (T)Activator.CreateInstance(type);//从当前程序集里面通过反射的方式创建指定类型的对象
                //obj = (T)Assembly.Load(SQLHelper._assemblyName).CreateInstance(SQLHelper._assemblyName + "." + type.Name);//从另一个程序集里面通过反射的方式创建指定类型的对象
                PropertyInfo[] propertyInfos = type.GetProperties();//获取指定类型里面的所有属性
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string fieldName = reader.GetName(i);
                        if (fieldName.ToLower() == propertyInfo.Name.ToLower())
                        {
                            object val = reader[propertyInfo.Name];//读取表中某一条记录里面的某一列信息
                            if (val != null && val != DBNull.Value)
                                propertyInfo.SetValue(obj, val, null);//给对象的某一个属性赋值
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return obj;
        }

        /// <summary>
        /// DataTable转换为泛型集合 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static List<T> DataTableToIList<T>(DataTable table)
        {
            List<T> list = new List<T>();
            Type typeInfo = typeof(T);
            //得到T内所有的公共属性
            PropertyInfo[] propertys = typeInfo.GetProperties();
            foreach (DataRow rowItem in table.Rows)
            {
                //通过反射动态创建对象
                T objT = Activator.CreateInstance<T>();
                //给objT的所有属性赋值
                foreach (DataColumn columnItem in table.Columns)
                {
                    foreach (PropertyInfo property in propertys)
                    {
                        if (columnItem.ColumnName.ToLower().Equals(property.Name.ToLower()))
                        {
                            //获取指定单元格的值
                            object value = rowItem[columnItem.ColumnName];
                            if (value != DBNull.Value)
                            {
                                property.SetValue(objT, value, null);
                            }
                            break;
                        }
                    }
                }
                list.Add(objT);
            }
            return list;
        }

        /// <summary>
        /// 将泛型集合类转换成DataTable
        /// </summary>
        /// <typeparam name="T">集合项类型</typeparam>
        /// <param name="list">集合</param>
        /// <returns>数据集(表)</returns>
        public static DataTable IListToDataTable<T>(IList<T> list)
        {
            return IListToDataTable<T>(list, null);
        }

        /// <summary>
        /// 将泛型集合类转换成DataTable
        /// </summary>
        /// <typeparam name="T">集合项类型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="propertyName">需要返回的列的列名</param>
        /// <returns>数据集(表)</returns>
        public static DataTable IListToDataTable<T>(IList<T> list, params string[] propertyName)
        {
            List<string> propertyNameList = new List<string>();
            if (propertyName != null)
                propertyNameList.AddRange(propertyName);

            DataTable result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    if (propertyNameList.Count == 0)
                    {
                        result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                    else
                    {
                        if (propertyNameList.Contains(pi.Name))
                            result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                }

                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        if (propertyNameList.Count == 0)
                        {
                            object obj = pi.GetValue(list[i], null);
                            tempList.Add(obj);
                        }
                        else
                        {
                            if (propertyNameList.Contains(pi.Name))
                            {
                                object obj = pi.GetValue(list[i], null);
                                tempList.Add(obj);
                            }
                        }
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }

        #endregion

        #region 存储过程操作
        /// <summary>
        /// 执行存储过程，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlDataReader</returns>
        public static SqlDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlDataReader returnReader;
            connection.Open();
            using (SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters))
            {
                command.CommandType = CommandType.StoredProcedure;
                returnReader = command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            return returnReader;

        }
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                using (SqlDataAdapter sqlDA = new SqlDataAdapter
                {
                    SelectCommand = BuildQueryCommand(connection, storedProcName, parameters)
                })
                {
                    sqlDA.Fill(dataSet, tableName);
                }
                connection.Close();
                return dataSet;
            }
        }
        public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName, int Times)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                using (SqlDataAdapter sqlDA = new SqlDataAdapter
                {
                    SelectCommand = BuildQueryCommand(connection, storedProcName, parameters)
                })
                {
                    sqlDA.SelectCommand.CommandTimeout = Times;
                    sqlDA.Fill(dataSet, tableName);
                }
                connection.Close();
                return dataSet;
            }
        }
        /// <summary>
        /// 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlCommand</returns>
        private static SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(storedProcName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            foreach (SqlParameter parameter in parameters)
            {
                if (parameter != null)
                {
                    // 检查未分配值的输出参数,将其分配以DBNull.Value.
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    command.Parameters.Add(parameter);
                }
            }

            return command;
        }
        /// <summary>
        /// 执行存储过程，返回影响的行数		
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="rowsAffected">影响的行数</param>
        /// <returns></returns>
        public static int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                int result;
                connection.Open();
                using (SqlCommand command = BuildIntCommand(connection, storedProcName, parameters))
                {
                    rowsAffected = command.ExecuteNonQuery();
                    result = (int)command.Parameters["ReturnValue"].Value;
                }
                //Connection.Close();
                return result;
            }
        }
        /// <summary>
        /// 创建 SqlCommand 对象实例(用来返回一个整数值)	
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlCommand 对象实例</returns>
        private static SqlCommand BuildIntCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.Parameters.Add(new SqlParameter("ReturnValue",
                SqlDbType.Int, 4, ParameterDirection.ReturnValue,
                false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return command;
        }
        #endregion
        /// <summary>
        /// 调用存储过程
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="commandParameters">存储过程参数</param>
        /// <returns>返回DataTable对象</returns>
        public static DataTable InvokeProc_DataTable(string procName, params SqlParameter[] commandParameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand sqlCmd = new SqlCommand();
                PrepareCommand(sqlCmd, conn, null, CommandType.StoredProcedure, procName, commandParameters);
                using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                {
                    using (DataTable dt = new DataTable())
                    {
                        try
                        {
                            //填充ds
                            da.Fill(dt);
                            // 清除cmd的参数集合 
                            sqlCmd.Parameters.Clear();
                            //返回ds
                            return dt;
                        }
                        catch (Exception ex)
                        {

                            //关闭连接，抛出异常
                            conn.Close();
                            throw ex;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 调用存储过程。用指定的数据库连接执行一个命令并返回一个数据集的第一列
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="commandParameters"></param>
        /// <returns>返回一个数据集的第一列</returns>
        public static object InvokeProc_ExecuteScalar(string procName, params SqlParameter[] commandParameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand())
                {
                    PrepareCommand(sqlCmd, conn, null, CommandType.StoredProcedure, procName, commandParameters);
                    object flag = sqlCmd.ExecuteScalar();
                    sqlCmd.Parameters.Clear();
                    return flag;
                }
            }
        }
        /// <summary>
        /// 启动事件执行多条删除语句
        /// </summary>
        /// <param name="sqlList">多条sql语句的泛型集合</param>
        /// <param name="Param">多条sql语句所对应参数的泛型集合</param>
        /// <returns>返回事务是否成功</returns>
        public static bool DeleteByTran(List<string> sqlList, List<SqlParameter[]> Param)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            using (SqlCommand cmd = new SqlCommand
            {
                Connection = conn
            })
            {
                try
                {
                    conn.Open();
                    cmd.Transaction = conn.BeginTransaction();//开启事务          
                    int i = 0;
                    //循环执行SQL语句
                    foreach (var sqlStr in sqlList)
                    {
                        cmd.CommandText = sqlStr;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddRange(Param[i]);
                        cmd.ExecuteNonQuery();
                        i++;
                    }
                    cmd.Transaction.Commit();//提交事务（真正的从数据库中修改了数据）
                    return true;
                }
                catch (Exception ex)
                {
                    if (cmd.Transaction != null)
                        cmd.Transaction.Rollback();//回滚事务（撤销前面已经“正常完成”的任务）
                    throw new Exception("调用事务方法出现错误：" + ex.Message);
                }
                finally
                {
                    if (cmd.Transaction != null)
                    {
                        cmd.Transaction = null;//清空事务
                    }
                    conn.Close();
                }
            }
        }
        /// <summary>
        /// 启用事务执行多条SQL语句
        /// </summary>      
        /// <param name="sqlList">SQL语句列表</param>      
        /// <returns></returns>
        public static bool UpdateByTran(List<string> sqlList)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            using (SqlCommand cmd = new SqlCommand
            {
                Connection = conn
            })
            {
                try
                {
                    conn.Open();
                    cmd.Transaction = conn.BeginTransaction();   //开启事务
                    foreach (string itemSql in sqlList)//循环提交SQL语句
                    {
                        cmd.CommandText = itemSql;
                        cmd.ExecuteNonQuery();
                    }
                    cmd.Transaction.Commit();  //提交事务(同时自动清除事务)
                    return true;
                }
                catch (Exception ex)
                {
                    if (cmd.Transaction != null)
                        cmd.Transaction.Rollback();//回滚事务(同时自动清除事务)
                    throw new Exception("调用事务方法UpdateByTran(List<string> sqlList)时出现错误：" + ex.Message);
                }
                finally
                {
                    if (cmd.Transaction != null)
                        cmd.Transaction = null;
                    conn.Close();
                }
            }
        }
        /// <summary>
        /// 启用事务提交多条带参数的SQL语句
        /// </summary>
        /// <param name="mainSql">明细表SQL语句</param>
        /// <param name="List<SqlParameter[]> mainParam">明细表对应的参数集合</param>
        /// <returns>返回事务是否成功</returns>
        public static bool UpdateAllByTran(List<string> mainSql, List<SqlParameter[]> mainParam)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            using (SqlCommand cmd = new SqlCommand
            {
                Connection = conn
            })
            {
                try
                {
                    conn.Open();
                    cmd.Transaction = conn.BeginTransaction();//开启事务
                    int i = 0;
                    //循环执行SQL语句
                    foreach (var sqlStr in mainSql)
                    {
                        cmd.CommandText = sqlStr;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddRange(mainParam[i]);
                        cmd.ExecuteNonQuery();
                        i++;
                    }
                    cmd.Transaction.Commit();//提交事务
                    return true;
                }
                catch (Exception ex)
                {
                    if (cmd.Transaction != null)
                    {
                        cmd.Transaction.Rollback();//回滚事务
                    }

                    //抛出异常
                    throw ex;
                }
                finally
                {
                    if (cmd.Transaction != null)
                    {
                        cmd.Transaction = null;//清空事务
                    }
                    conn.Close();
                }
            }
        }
        /// <summary>
        /// 启用事务提交多条带参数的SQL语句
        /// </summary>
        /// <param name="mainSql">主表SQL</param>
        /// <param name="SqlParameter[] mainParam">主表对应的参数</param>
        /// <param name="detailSql">明细表SQL语句</param>
        /// <param name="List<SqlParameter[]> detailParam">明细表对应的参数集合</param>
        /// <returns>返回事务是否成功</returns>
        public static bool UpdateByTran(string mainSql, SqlParameter[] mainParam, string detailSql, List<SqlParameter[]> detailParam)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            using (SqlCommand cmd = new SqlCommand
            {
                Connection = conn
            })
            {
                try
                {
                    conn.Open();
                    cmd.Transaction = conn.BeginTransaction();//开启事务
                    if (mainSql != null && mainSql.Length != 0)
                    {
                        cmd.CommandText = mainSql;
                        cmd.Parameters.AddRange(mainParam);
                        cmd.ExecuteNonQuery();
                    }
                    foreach (SqlParameter[] param in detailParam)
                    {
                        cmd.CommandText = detailSql;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddRange(param);
                        cmd.ExecuteNonQuery();
                    }
                    cmd.Transaction.Commit();//提交事务
                    return true;
                }
                catch (Exception ex)
                {
                    if (cmd.Transaction != null)
                    {
                        cmd.Transaction.Rollback();//回滚事务
                    }
                    //抛出异常
                    throw ex;
                }
                finally
                {
                    if (cmd.Transaction != null)
                    {
                        cmd.Transaction = null;//清空事务
                    }
                    conn.Close();
                }
            }
        }
        /// <summary>
        /// 启用事务提交多条带参数的SQL语句
        /// </summary>
        /// <param name="mainSql">主表SQL</param>
        /// <param name="SqlParameter[] mainParam">主表对应的参数</param>
        /// <param name="List<string> detailSql">明细表多条SQL语句集合</param>
        /// <param name="List<SqlParameter[]> detailParam">明细表对应的参数集合</param>
        /// <returns>返回事务是否成功</returns>
        public static bool UpdateAllByTran(string mainSql, SqlParameter[] mainParam, List<string> detailSql, List<SqlParameter[]> detailParam)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            using (SqlCommand cmd = new SqlCommand
            {
                Connection = conn
            })
            {
                try
                {
                    conn.Open();
                    cmd.Transaction = conn.BeginTransaction();//开启事务
                    if (mainSql != null && mainSql.Length != 0)
                    {
                        cmd.CommandText = mainSql;
                        cmd.Parameters.AddRange(mainParam);
                        cmd.ExecuteNonQuery();
                    }
                    int i = 0;
                    //循环执行SQL语句
                    foreach (var sqlStr in detailSql)
                    {
                        cmd.CommandText = sqlStr;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddRange(detailParam[i]);
                        cmd.ExecuteNonQuery();
                        i++;
                    }
                    cmd.Transaction.Commit();//提交事务
                    return true;
                }
                catch (Exception ex)
                {
                    if (cmd.Transaction != null)
                    {
                        cmd.Transaction.Rollback();//回滚事务
                    }

                    //抛出异常
                    throw ex;
                }
                finally
                {
                    if (cmd.Transaction != null)
                    {
                        cmd.Transaction = null;//清空事务
                    }
                    conn.Close();
                }
            }
        }
    }
}