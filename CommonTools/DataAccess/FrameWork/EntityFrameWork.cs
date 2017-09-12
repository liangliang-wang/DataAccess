using DataAccess.Enums;
using DataAccess.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;

namespace DataAccess.FrameWork
{
    /// <summary>
    /// 实体查询框架
    /// </summary>
    /// <typeparam name="T">类型T</typeparam>
    public class EntityFrameWork<T> where T : BaseEntity<T>, new()
    {
        private DBType dbType;

        private Dictionary<DBType, Func<string, string, Dictionary<string, object>, IDbCommand>> commandDic = new Dictionary<DBType, Func<string, string, Dictionary<string, object>, IDbCommand>>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbType"></param>
        public EntityFrameWork(DBType dbType)
        {
            this.dbType = dbType;
            commandDic.Add(DBType.MYSQL, GetMySqlCommand);
            commandDic.Add(DBType.SQLSERVER, GetMSSqlCommand);

        }

        /// <summary>
        /// 查询单个实体
        /// </summary>
        /// <param name="item">框架实体</param>
        /// <returns>单个对象</returns>
        public T SelectModel(FrameWorkItem item)
        {
            IDataReader dr = null;
            try
            {
                dr = BuildDataReader(item);
                if (dr != null && dr.Read())
                {
                    return DynamicBuilderEntity<T>.CreateBuilder(dr).Build(dr);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
            return default(T);
        }

        /// <summary>
        /// 查询集合，带分页
        /// </summary>
        /// <param name="item">框架实体</param>
        /// <param name="dp">分页对象</param>
        /// <returns>对象集合</returns>
        public List<T> SelectList(FrameWorkItem item, DataPage dp = null)
        {
            IDataReader dr = null;
            try
            {
                dr = BuildDataReader(item, dp);

                List<T> list = new List<T>();
                if (dp != null && dp.PageSize > 0)
                {
                    int result = GetResult<int>(string.Format("SELECT COUNT(1) FROM ({0}) a", item.Sql), item.ConnectionString, item.SqlParam);
                    dp.RowCount = result;
                }
                while (dr != null && dr.Read())
                {
                    T tempT = new T();
                    tempT = DynamicBuilderEntity<T>.CreateBuilder(dr).Build(dr);
                    list.Add(tempT);
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
        }

        /// <summary>
        /// 查询表格数据
        /// </summary>
        /// <param name="item">框架实体</param>
        /// <param name="dp">分页对象</param>
        /// <returns></returns>
        public DataTable SelectDataTable(FrameWorkItem item, DataPage dp = null)
        {
            IDataReader dr = null;
            DataTable result = new DataTable();
            try
            {
                dr = BuildDataReader(item, dp);
                var datasSet = dr.ToDataSet();
                if (datasSet.Tables.Count > 0)
                {
                    result = datasSet.Tables[0];
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
        }

        /// <summary>
        /// 根据框架实体，条件式的选用sqlhelper
        /// </summary>
        /// <param name="item">框架实体</param>
        /// <param name="dp">分页</param>
        /// <returns>IDataReader</returns>
        private IDataReader BuildDataReader(FrameWorkItem item, DataPage dp = null)
        {
            var sql = DalAid.CreatePageQuerySql(item.Sql, dp, dbType);
            var connection = new MySqlConnection(item.ConnectionString);
            var cmd = GetCommand(sql, item.ConnectionString, item.SqlParam);
            var cnt = cmd.ExecuteReader();
            return cnt;
        }

        /// <summary>
        /// 获取单结果值
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="sql">SQL</param>
        /// <param name="dBConnectionString">数据库链接字符串</param>
        /// <param name="para">查询参数</param>
        /// <returns>结果</returns>
        public T GetResult<T>(string sql, string dBConnectionString, Dictionary<string, object> para)
        {
            T result;
            try
            {
                DataSet dataSet = ExecuteQuery(sql, dBConnectionString, para);
                bool flag2 = dataSet.Tables.Count == 0;
                if (flag2)
                {
                    result = default(T);
                }
                else
                {
                    bool flag3 = dataSet.Tables[0].Rows.Count == 0;
                    if (flag3)
                    {
                        result = default(T);
                    }
                    else
                    {
                        object value = dataSet.Tables[0].Rows[0][0];
                        result = (T)((object)Convert.ChangeType(value, typeof(T)));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\n" + sql);
            }
            return result;
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="dBConnectionString">数据库链接字符串</param>
        /// <param name="cmdParms">参数</param>
        /// <returns>结果</returns>
        public DataSet ExecuteQuery(string sql, string dBConnectionString, Dictionary<string, object> cmdParms)
        {
            DataSet dataSet = new DataSet();
            try
            {
                using (var cmd = GetCommand(sql, dBConnectionString, cmdParms))
                {
                    if (cmd != null)
                    {
                        using (IDataReader dataReader = cmd.ExecuteReader())
                        {
                            dataSet = dataReader.ToDataSet();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\n" + sql);
            }
            return dataSet;
        }

        /// <summary>
        /// 获取数据库cmd
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="dbConnectionString">数据库连接</param>
        /// <param name="cmdParms">参数</param>
        /// <returns>IDbCommand</returns>
        private IDbCommand GetCommand(string sql, string dbConnectionString, Dictionary<string, object> cmdParms)
        {
            if (commandDic.ContainsKey(dbType))
            {
                return commandDic[dbType](sql, dbConnectionString, cmdParms);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取mysql的cmd
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="dbConnectionString">链接字符串</param>
        /// <param name="cmdParms">参数</param>
        /// <returns>结果</returns>
        private MySqlCommand GetMySqlCommand(string sql, string dbConnectionString, Dictionary<string, object> cmdParms)
        {
            var connection = new MySqlConnection(dbConnectionString);
            var cmd = new MySqlCommand(sql, connection);
            if (cmdParms != null)
            {
                foreach (var item in cmdParms)
                {
                    cmd.Parameters.AddWithValue(item.Key, item.Value);
                }
            }
            if (connection.State != ConnectionState.Open)
                connection.Open();
            return cmd;
        }

        /// <summary>
        /// 获取sqlservice的cmd
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="dbConnectionString">链接字符串</param>
        /// <param name="cmdParms">参数</param>
        /// <returns>结果</returns>
        private SqlCommand GetMSSqlCommand(string sql, string dbConnectionString, Dictionary<string, object> cmdParms)
        {
            SqlConnection con = new SqlConnection(dbConnectionString);
            var cmd = new SqlCommand(sql, con);
            if (cmdParms != null)
            {
                foreach (var item in cmdParms)
                {
                    cmd.Parameters.AddWithValue(item.Key, item.Value);
                }
            }
            return cmd;
        }

        /// <summary>
        /// 执行sql 返回响应行数
        /// </summary>
        /// <param name="item">参数</param>
        /// <returns>结果</returns>
        public int ExecuteNonQuery(FrameWorkItem item)
        {
            int result = 0;
            try
            {
                using (var cmd = GetCommand(item.Sql, item.ConnectionString, item.SqlParam))
                {
                    if (cmd != null)
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\n" + item.Sql);
            }
            return result;
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。 忽略额外的列或行。
        /// </summary>
        /// <param name="item">参数</param>
        /// <returns>结果</returns>
        public object ExecuteScalar(FrameWorkItem item)
        {
            object result = null; ;
            try
            {
                using (var cmd = GetCommand(item.Sql, item.ConnectionString, item.SqlParam))
                {
                    if (cmd != null)
                    {
                        result = cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\n" + item.Sql);
            }
            return result;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <typeparam name="Tentity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="dbConnectionString"></param>
        /// <returns></returns>
        public int Add(T entity, string dbConnectionString)
        {
            var sql = entity.GetInsertSql();
            return ExecuteNonQuery(new FrameWorkItem { Sql = sql.Item1, ConnectionString = dbConnectionString, SqlParam = sql.Item2 });
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="Tentity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="dbConnectionString"></param>
        /// <returns></returns>
        public int Update(T entity, string dbConnectionString)
        {
            var sql = entity.GetUpdateSql();
            return ExecuteNonQuery(new FrameWorkItem { Sql = sql.Item1, ConnectionString = dbConnectionString, SqlParam = sql.Item2 });
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="Tentity"></typeparam>
        /// <param name="entity">实体</param>
        /// <param name="dbConnectionString">数据库连接字符串</param>
        /// <param name="onlyUpdate">仅更新那些字段</param>
        /// <returns></returns>
        public int Update(T entity, string dbConnectionString, List<string> onlyUpdate)
        {
            var sql = entity.GetUpdateSql(onlyUpdate);
            return ExecuteNonQuery(new FrameWorkItem { Sql = sql.Item1, ConnectionString = dbConnectionString, SqlParam = sql.Item2 });
        }

        /// <summary>
        /// 列表查询（单表）
        /// </summary>
        /// <param name="dbConnectionString"></param>
        /// <param name="param"></param>
        /// <param name="dp"></param>
        /// <returns></returns>
        public List<T> List(string dbConnectionString, Dictionary<string, object> param, DataPage dp = null)
        {
            var sql = new T().GetSelectSql();
            Dictionary<string, object> newParam = new Dictionary<string, object>();
            if (param != null && param.Count > 0)
            {
                sql += " where 1=1 ";
                int index = 0;
                foreach (var paramItem in param)
                {
                    var par = GetCondition(paramItem.Key);
                    var paramName = "@Param" + ++index;
                    sql += string.Format(" and {0} {1} {2}", par.Item2, par.Item1, paramName);
                    newParam.Add(paramName, paramItem.Value);
                }
            }
            FrameWorkItem item = new FrameWorkItem
            {
                Sql = sql,
                SqlParam = newParam,
                ConnectionString = dbConnectionString
            };
            return SelectList(item, dp);
        }

        /// <summary>
        /// 获取比较符
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static Tuple<string, string> GetCondition(string key)
        {
            var result = "=";
            var name = key;
            if (key[key.Length - 2] < 'A')
            {
                name = key.Substring(0, key.Length - 2);
                result = key.Substring(key.Length - 2);
            }
            else if (key[key.Length - 1] < 'A')
            {
                name = key.Substring(0, key.Length - 1);
                result = key.Substring(key.Length - 1);
            }
            return new Tuple<string, string>(result, name);
        }

        /// <summary>
        /// 根据主键硬删除
        /// </summary>
        /// <param name="pk"></param>
        /// <param name="dbConnectionString"></param>
        /// <returns></returns>
        public int DeleteByKey(string pk, string dbConnectionString)
        {
            var entity = new T();
            var sql = entity.GetDeleteSql(pk);
            return ExecuteNonQuery(new FrameWorkItem { Sql = sql.Item1, ConnectionString = dbConnectionString, SqlParam = sql.Item2 });
        }
    }
}
