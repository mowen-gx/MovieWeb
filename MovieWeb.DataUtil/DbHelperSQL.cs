using System;
using System.Data;
using System.Data.SqlClient;

namespace MovieWeb.DataUtil
{
    /// <summary>
    /// 数据访问基础类
    /// 功能：封装底层数据访问
    /// 作者：莫文
    /// 时间：2011-11-1
    /// </summary>
    public class DbHelperSql
    {
        #region private fields

        /// <summary>
        /// 链接字符串
        /// </summary>
        private readonly string _ConnectionString;

        /// <summary>
        /// 超时时间(秒)
        /// </summary>
        private const int CommandTimeOut = 100;
        #endregion


        #region 构造函数
        public DbHelperSql(string conn)
        {
            _ConnectionString = conn;
        }
        #endregion

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parms">参数</param>
        /// <returns>影响记录数</returns>
        public int ExecuteSql(string sql, params SqlParameter[] parms)
        {
            return ExecuteNonQuery(this._ConnectionString, sql, parms);
        }

        public int ExecuteSql(SqlConnection conn, string sql, params SqlParameter[] parms)
        {
            return ExecuteNonQuery(conn, sql, parms);
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="sql">计算查询结果语句</param>
        /// <param name="parms">参数</param>
        /// <returns>查询结果（object）</returns>
        public object GetSingle(string sql, params SqlParameter[] parms)
        {
            return ExecuteScalar(this._ConnectionString, sql, parms);
        }

        public object GetSingle(SqlConnection conn, string sql, params SqlParameter[] parms)
        {
            return ExecuteScalar(conn, sql, parms);
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <param name="parms">参数</param>
        /// <returns>DataSet</returns>
        [Obsolete("不建议使用,此方法不支持Dataset更新操作,建议直接使用GetDataTable方法")]
        public DataSet Query(string sql, params SqlParameter[] parms)
        {
            using (SqlConnection conn = new SqlConnection(this._ConnectionString))
            {
                SqlCommand comm = conn.CreateCommand();
                comm.CommandTimeout = CommandTimeOut;
                comm.CommandType = CommandType.Text;
                comm.CommandText = sql;
                if (parms != null && parms.Length > 0)
                {
                    PrepareSqlParameter(parms);
                    comm.Parameters.AddRange(parms);
                }
                SqlDataAdapter adapter = new SqlDataAdapter(comm);

                DataSet ds = new DataSet();
                try
                {
                    adapter.Fill(ds);
                    return ds;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    adapter.Dispose();
                }
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parms">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public DataSet RunProcedure(string storedProcName, SqlParameter[] parms, string tableName)
        {
            using (SqlConnection conn = new SqlConnection(this._ConnectionString))
            {
                SqlCommand comm = conn.CreateCommand();
                comm.CommandTimeout = CommandTimeOut;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = storedProcName;
                if (parms != null && parms.Length > 0)
                {
                    PrepareSqlParameter(parms);
                    comm.Parameters.AddRange(parms);
                }
                SqlDataAdapter adapter = new SqlDataAdapter(comm);

                DataSet ds = new DataSet();
                try
                {
                    adapter.Fill(ds, tableName);

                    return ds;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    adapter.Dispose();
                }
            }
        }

        /// <summary>
        /// 获得分页数据
        /// </summary>
        public DataSet SelectDataByPages(int pageIndex, int pageSize, string querySqlorTable, string orderBySql, string queryWhere)
        {
            string sql =
                string.Format(
                    @" declare @recordCount int
                select * from {0} where id in (
                        select id from 
                            (select  id, row_number() over (order by {1})
                            scn from   {0}  {4}) t
                        where scn<={2} and scn>{3})  
                 select @recordCount=count(id)
                             from   {0}  {4}
                select @recordCount",
                    querySqlorTable, orderBySql, pageIndex * pageSize, (pageIndex - 1) * pageSize,
                    queryWhere != "" ? "where " + queryWhere : "");

            return Query(sql);
        }

        public DataTable GetPageDataTable(string sql, string order, int pageSize, int currentPage, string query)
        {
            string psql = string.Format(@"select * from(
            select ROW_NUMBER() over (order by {1}) RowNumber, * from
            (
	            {0}
            ) t  where 1=1 {4}
            )  t where RowNumber between {2} and {3}", sql, order, (currentPage - 1) * pageSize + 1, currentPage * pageSize, query);

            return Query(psql).Tables[0];
        }



        #region 实例方法

        /// <summary>
        /// 根据表名返回DataTable
        /// </summary>
        /// <param name="tableName">表名</param>
        public DataTable GetDataTableByTableName(string tableName)
        {
            return GetDataTable(this._ConnectionString, string.Format("select * from {0}", tableName));
        }

        /// <summary>
        /// 根据表名返回空DataTable
        /// </summary>
        /// <param name="tableName">表名</param>
        public DataTable GetEmptyDataTable(string tableName)
        {
            return GetDataTable(this._ConnectionString, string.Format("select * from {0} where 1=-1", tableName));
        }

        /// <summary>
        /// 根据SQL语句查询记录数据(支持分页)
        /// </summary>
        /// <param name="sql">传入的sql语句</param>
        /// <param name="order">排序字段（必须传，如:ID desc）</param>
        /// <param name="pageSize">每页页记录数</param>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="query">查询条件，可以为null（如:and id=1）</param>
        /// <returns>返回查询结果，以DataTable形式</returns>
        public DataTable GetDataTable(string sql, string order, int pageSize, int pageIndex, string query)
        {
            string psql = string.Format(@"select * from(
			select ROW_NUMBER() over (order by {1}) RowNumber, * from
			(
				{0}
			) t where 1=1 {4}
			)  t where RowNumber between {2} and {3} ", sql, order, (pageIndex - 1) * pageSize + 1, pageIndex * pageSize, query);

            return GetDataTable(this._ConnectionString, psql);
        }

        /// <summary>
        /// 根据SQL语句查询记录数据
        /// </summary>
        /// <param name="sql">传入的SQL语句</param>      
        /// <returns>返回查询结果，以DataTable形式</returns>
        public DataTable GetDataTable(string sql, params SqlParameter[] parms)
        {
            return GetDataTable(this._ConnectionString, sql, parms);
        }

        public DataRow GetDataRow(string sql)
        {
            DataTable dt = GetDataTable(this._ConnectionString, sql);
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        /// <summary>
        /// 根据SQL语句返回第一行查询数据
        /// </summary>
        /// <param name="sql">传入的SQL语句</param>      
        /// <returns>返回查询结果，以DataTable形式</returns>
        public DataRow GetDataRow(string sql, SqlParameter[] parms)
        {
            DataTable dt = GetDataTable(this._ConnectionString, sql, parms);
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行...
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, params SqlParameter[] parms)
        {
            return ExecuteScalar(this._ConnectionString, sql, parms);
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteNonQuery(string sql, params SqlParameter[] parms)
        {
            return ExecuteNonQuery(this._ConnectionString, sql, parms);
        }
        #endregion

        #region 静态方法

        /// <summary>
        /// 根据SQL语句查询记录数据
        /// </summary>
        /// <param name="sql">传入的SQL语句</param>
        /// <param name="parms">SQL参数</param>
        /// <returns>返回查询结果，以DataTable形式</returns>
        public static DataTable GetDataTable(string connString, string sql, params SqlParameter[] parms)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlCommand comm = conn.CreateCommand();
                comm.CommandTimeout = CommandTimeOut;
                comm.CommandType = CommandType.Text;
                comm.CommandText = sql;
                if (parms != null && parms.Length > 0)
                {
                    PrepareSqlParameter(parms);
                    comm.Parameters.AddRange(parms);
                }
                SqlDataAdapter adapter = new SqlDataAdapter(comm);

                DataTable dt = new DataTable();
                try
                {
                    adapter.Fill(dt);
                    if (sql.IndexOf("@") > 0)
                    {
                        sql = sql.ToLower();
                        int index = sql.IndexOf("where ");
                        if (index < 0)
                        {
                            index = sql.IndexOf("\nwhere");
                        }
                        if (index > 0)
                        {
                            dt.ExtendedProperties.Add("SQL", sql.Substring(0, index - 1));  //将获取的语句保存在表的一个附属数组里，方便更新时生成CommandBuilder
                        }
                        else
                        {
                            dt.ExtendedProperties.Add("SQL", sql);  //将获取的语句保存在表的一个附属数组里，方便更新时生成CommandBuilder
                        }
                    }
                    else
                    {
                        dt.ExtendedProperties.Add("SQL", sql);  //将获取的语句保存在表的一个附属数组里，方便更新时生成CommandBuilder
                    }
                    return dt;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    adapter.Dispose();
                }
            }
        }
        /// <summary>
        /// 批量更新数据(每批次500)
        /// </summary>
        /// <param name="dtDataTables">数据表记录</param>
        public static void Update(string connString, DataTable table)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand comm = conn.CreateCommand();
            comm.CommandTimeout = CommandTimeOut;
            comm.CommandType = CommandType.Text;
            SqlDataAdapter adapter = new SqlDataAdapter(comm);
            SqlCommandBuilder commandBulider = new SqlCommandBuilder(adapter);
            commandBulider.ConflictOption = ConflictOption.OverwriteChanges;
            try
            {
                conn.Open();
                //设置批量更新的每次处理条数
                adapter.UpdateBatchSize = 500;
                adapter.SelectCommand.Transaction = conn.BeginTransaction();/////////////////开始事务	
                if (table.ExtendedProperties["SQL"] != null)
                {
                    adapter.SelectCommand.CommandText = table.ExtendedProperties["SQL"].ToString();
                }
                adapter.Update(table);
                adapter.SelectCommand.Transaction.Commit();/////提交事务
            }
            catch (Exception ex)
            {
                if (adapter.SelectCommand != null && adapter.SelectCommand.Transaction != null)
                {
                    adapter.SelectCommand.Transaction.Rollback();
                }
                throw ex;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }
        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parms">参数</param>
        /// <returns>影响记录数</returns>
        public static int ExecuteNonQuery(string connString, string sql, params SqlParameter[] parms)
        {
            SqlConnection conn = new SqlConnection(connString);
            try
            {
                conn.Open();
                SqlCommand comm = new SqlCommand();
                comm.CommandTimeout = CommandTimeOut;
                comm.CommandType = CommandType.Text;
                comm.CommandText = sql;
                comm.Connection = conn;
                if (parms != null && parms.Length > 0)
                {
                    PrepareSqlParameter(parms);
                    comm.Parameters.AddRange(parms);
                }

                return comm.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }

        public static int ExecuteNonQuery(SqlConnection conn, string sql, params SqlParameter[] parms)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                SqlCommand comm = new SqlCommand();
                comm.CommandTimeout = CommandTimeOut;
                comm.CommandType = CommandType.Text;
                comm.CommandText = sql;
                comm.Connection = conn;
                if (parms != null && parms.Length > 0)
                {
                    PrepareSqlParameter(parms);
                    comm.Parameters.AddRange(parms);
                }

                return comm.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }


        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行...
        /// </summary>
        /// <param name="tran">事物</param>
        /// <param name="sql">sql</param>
        /// <param name="parms">参数</param>
        /// <returns></returns>
        public static object ExecuteScalar(string connString, string sql, params SqlParameter[] parms)
        {
            SqlConnection conn = new SqlConnection(connString);
            try
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandTimeout = CommandTimeOut;
                comm.CommandType = CommandType.Text;
                comm.CommandText = sql;
                if (parms != null && parms.Length > 0)
                {
                    PrepareSqlParameter(parms);
                    comm.Parameters.AddRange(parms);
                }

                object value = comm.ExecuteScalar();
                comm.Parameters.Clear();
                return value;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }

        }


        public static object ExecuteScalar(SqlConnection conn, string sql, params SqlParameter[] parms)
        {
            try
            {
                SqlCommand comm = conn.CreateCommand();
                comm.CommandTimeout = CommandTimeOut;
                comm.CommandType = CommandType.Text;
                comm.CommandText = sql;
                if (parms != null && parms.Length > 0)
                {
                    PrepareSqlParameter(parms);
                    comm.Parameters.AddRange(parms);
                }

                object value = comm.ExecuteScalar();
                return value;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

        }

        /// <summary>
        /// 批量执行Sql语句(事物控制)
        /// </summary>
        /// <param name="sqls">SQL语句数组</param>
        public static void ExecuteSQL(string connString, params string[] sqls)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand comm = new SqlCommand();
            try
            {
                conn.Open();
                comm.CommandTimeout = CommandTimeOut;
                comm.CommandType = CommandType.Text;
                comm.Connection = conn;
                comm.Transaction = conn.BeginTransaction();
                foreach (string sql in sqls)
                {
                    comm.CommandText = sql;
                    comm.ExecuteNonQuery();
                }
                comm.Transaction.Commit();
            }
            catch (Exception ex)
            {
                if (comm.Transaction != null)
                {
                    comm.Transaction.Rollback();
                    comm.Transaction.Dispose();
                }
                throw ex;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }
        /// <summary>
        /// 大批量插入数据
        /// </summary>
        /// <param name="connString">数据库链接字符串</param>
        /// <param name="tableName">数据库服务器上目标表名</param>
        /// <param name="dt">含有和目标数据库表结构完全一致(所包含的字段名完全一致即可)的DataTable</param>
        public static void BulkCopy(string connString, string tableName, DataTable dt)
        {
            using (SqlBulkCopy bulk = new SqlBulkCopy(connString))
            {
                bulk.BatchSize = 500;
                bulk.BulkCopyTimeout = CommandTimeOut;
                bulk.DestinationTableName = tableName;
                foreach (DataColumn col in dt.Columns)
                {
                    bulk.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                }
                bulk.WriteToServer(dt);
                bulk.Close();
            }
        }

        /// <summary>
        /// 根据表名返回空DataTable
        /// </summary>
        /// <param name="tableName">表名</param>
        public static DataTable GetEmptyDataTable(string connString, string tableName)
        {
            return GetDataTable(connString, string.Format("select * from {0} where 1=-1", tableName));
        }

        /// <summary>
        /// 过滤空参数
        /// </summary>
        /// <param name="parms"></param>
        private static void PrepareSqlParameter(params SqlParameter[] parms)
        {
            if (parms != null)
            {
                foreach (SqlParameter parameter in parms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) && (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                }
            }
        }
        #endregion
    }
}
