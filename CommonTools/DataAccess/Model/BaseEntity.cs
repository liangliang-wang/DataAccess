using DataAccess.Attributes;
using DataAccess.Enums;
using DataAccess.FrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DataAccess.Model
{
    /// <summary>
    /// 数据库实体基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseEntity<T> where T : class, new()
    {
        private string _dbName = string.Empty;
        /// <summary>
        /// 库名
        /// </summary>
        public string DBName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_dbName))
                {
                    var type = this.GetType();
                    var attribute = type.GetCustomAttributes(typeof(DataBaseAttribute), false).FirstOrDefault();
                    if (attribute != null)
                    {
                        _dbName = ((DataBaseAttribute)attribute).DBName;
                    }
                }
                return _dbName;
            }
        }

        private string _tableName = string.Empty;
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_tableName))
                {
                    var type = this.GetType();
                    var attribute = type.GetCustomAttributes(typeof(EntityAttribute), false).FirstOrDefault();
                    if (attribute != null)
                    {
                        _tableName = ((EntityAttribute)attribute).TableName;
                    }
                }
                return _tableName;
            }
        }

        private string _pkName = string.Empty;
        /// <summary>
        /// 主键名
        /// </summary>
        internal string PkName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_pkName))
                {
                    string result = string.Empty;
                    var type = this.GetType();
                    var propertise = type.GetProperties();
                    foreach (var item in propertise)
                    {
                        var attrs = item.GetCustomAttributes(typeof(PrimaryKeyAttribute), false);
                        if (attrs.Count() > 0)
                        {
                            var fileAttrs = item.GetCustomAttributes(typeof(DBFieldAttribute), false);
                            if (fileAttrs.Count() > 0)
                            {
                                var fileAttr = fileAttrs[0] as DBFieldAttribute;
                                _pkName = fileAttr.FieldName;
                            }
                        }
                    }
                }
                return _pkName;
            }
        }

        private string _pkFileName = string.Empty;
        /// <summary>
        /// 主键名属性名
        /// </summary>
        public string PkFileName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_pkFileName))
                {
                    string result = string.Empty;
                    var type = this.GetType();
                    var propertise = type.GetProperties();
                    foreach (var item in propertise)
                    {
                        var attrs = item.GetCustomAttributes(typeof(PrimaryKeyAttribute), false);
                        if (attrs.Count() > 0)
                        {
                            _pkFileName = item.Name;
                        }
                    }
                }
                return _pkFileName;
            }
        }

        private object _pkValue = null;

        /// <summary>
        /// 主键值
        /// </summary>
        public object PkValue
        {
            get
            {
                if (_pkValue == null)
                {
                    if (pkPropertyInfo == null)
                    {
                        var type = this.GetType();
                        var propertise = type.GetProperties();
                        foreach (var item in propertise)
                        {
                            var attrs = item.GetCustomAttributes(typeof(PrimaryKeyAttribute), false);
                            if (attrs.Count() > 0)
                            {
                                pkPropertyInfo = item;
                                break;
                            }
                        }
                    }
                    _pkValue = pkPropertyInfo.GetValue(this);
                }
                return _pkValue;
            }
        }

        private PropertyInfo pkPropertyInfo = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void SetPpValue(object value)
        {
            if (pkPropertyInfo == null)
            {
                var type = this.GetType();
                var propertise = type.GetProperties();
                foreach (var item in propertise)
                {
                    var attrs = item.GetCustomAttributes(typeof(PrimaryKeyAttribute), false);
                    if (attrs.Count() > 0)
                    {
                        pkPropertyInfo = item;
                        break;
                    }
                }
            }
            pkPropertyInfo.SetValue(this, value);
        }

        /// <summary>
        /// 获取插入sql
        /// </summary>
        /// <returns></returns>
        public Tuple<string, Dictionary<string, object>> GetInsertSql()
        {
            string sql = string.Empty;
            List<string> values = new List<string>();
            List<string> param = new List<string>();
            var paramDic = new Dictionary<string, object>();
            var type = this.GetType();
            var propertise = type.GetProperties();
            foreach (var item in propertise)
            {
                if (IsPkIdentity(item))
                {
                    continue;
                }
                var attrs = item.GetCustomAttributes(typeof(DBFieldAttribute), false);
                if (attrs.Count() > 0)
                {
                    var attr = attrs[0] as DBFieldAttribute;
                    var value = item.GetValue(this);
                    if (value != null && values.Contains(attr.FieldName) == false)
                    {
                        values.Add(attr.FieldName);
                        param.Add("@" + item.Name);
                        paramDic.Add("@" + item.Name, value);
                    }
                }
            }
            sql = string.Format("insert into {0} ({1}) values({2})", TableName, string.Join(",", values), string.Join(",", param));
            return new Tuple<string, Dictionary<string, object>>(sql, paramDic);
        }

        /// <summary>
        /// 是否主键自增
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool IsPkIdentity(PropertyInfo item)
        {
            var attrs = item.GetCustomAttributes(typeof(PrimaryKeyAttribute), false);
            if (attrs.Count() > 0)
            {
                var attr = attrs[0] as PrimaryKeyAttribute;
                if (attr.IsIdentity)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取更新sql
        /// </summary>
        /// <returns></returns>
        public Tuple<string, Dictionary<string, object>> GetUpdateSql()
        {
            string sql = string.Empty;
            var setStr = string.Empty;
            var paramDic = new Dictionary<string, object>();
            var whereStr = string.Format("{0} = @{1}", PkName, PkName);
            paramDic.Add("@" + PkName, PkValue);
            var setTup = GetUpdateSet();
            setStr = setTup.Item1.Trim(',');
            foreach (var item in setTup.Item2)
            {
                paramDic.Add(item.Key, item.Value);
            }
            sql = string.Format("update {0} set {1} where {2}", TableName, setStr, whereStr);
            return new Tuple<string, Dictionary<string, object>>(sql, paramDic);
        }

        /// <summary>
        /// 获取更新Ssql
        /// </summary>
        /// <param name="onlyUpdate">只更新哪些字段</param>
        /// <returns></returns>
        public Tuple<string, Dictionary<string, object>> GetUpdateSql(List<string> onlyUpdate)
        {
            string sql = string.Empty;
            var setStr = string.Empty;
            var paramDic = new Dictionary<string, object>();
            var whereStr = string.Format("{0} = @{1}", PkName, PkFileName);
            paramDic.Add("@" + PkFileName, PkValue);
            var setTup = GetUpdateSet(onlyUpdate);
            setStr = setTup.Item1.Trim(',');
            foreach (var item in setTup.Item2)
            {
                paramDic.Add(item.Key, item.Value);
            }
            sql = string.Format("update {0} set {1} where {2}", TableName, setStr, whereStr);
            return new Tuple<string, Dictionary<string, object>>(sql, paramDic);

        }

        /// <summary>
        /// 获取更新的字段Sql段
        /// </summary>
        /// <param name="onlyUpdate"></param>
        /// <returns></returns>
        private Tuple<string, Dictionary<string, object>> GetUpdateSet(List<string> onlyUpdate = null)
        {
            var setStr = string.Empty;
            var paramDic = new Dictionary<string, object>();
            var type = this.GetType();
            var propertise = type.GetProperties();
            foreach (var item in propertise)
            {
                if (onlyUpdate == null || onlyUpdate.Contains(item.Name))
                {
                    var attrs = item.GetCustomAttributes(typeof(DBFieldAttribute), false);
                    if (attrs.Count() > 0)
                    {
                        var attr = attrs[0] as DBFieldAttribute;
                        if (item.Name != PkFileName)
                        {
                            var value = item.GetValue(this);
                            setStr += string.Format("{0} = @{1},", attr.FieldName, item.Name);
                            paramDic.Add("@" + item.Name, value);
                        }
                    }
                }
            }
            setStr = setStr.Trim(',');
            return new Tuple<string, Dictionary<string, object>>(setStr, paramDic);
        }

        /// <summary>
        /// 获取查询Sql
        /// </summary>
        /// <returns></returns>
        public string GetSelectSql()
        {
            var sql = string.Empty;
            var fields = new List<string>();
            var type = this.GetType();
            var propertise = type.GetProperties();
            foreach (var item in propertise)
            {
                var attrs = item.GetCustomAttributes(typeof(DBFieldAttribute), false);
                if (attrs.Count() > 0)
                {
                    var attr = attrs[0] as DBFieldAttribute;
                    var value = string.Format(" {0} as {1}", attr.FieldName, item.Name);
                    fields.Add(value);
                }
            }
            sql = string.Format(" select * from (select {0} from {1}) a1", string.Join(",", fields), TableName);
            return sql;
        }

        /// <summary>
        /// 获取硬删除sql
        /// </summary>
        /// <returns></returns>
        public Tuple<string, Dictionary<string, object>> GetDeleteSql(string pk)
        {
            var sql = string.Format(" delete from {0} where {1} = @{1}", TableName, PkName);
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("@" + PkName, pk);
            return new Tuple<string, Dictionary<string, object>>(sql, param);
        }
    }
}
