using DataAccess.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Model
{
    /// <summary>
    /// 数据库实体基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseEntity<T> where T : class, new()
    {
        private string _tableName = string.Empty;

        /// <summary>
        /// 表名
        /// </summary>
        internal string TableName
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
        private string PkName
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

        private object _pkValue = null;

        /// <summary>
        /// 主键值
        /// </summary>
        private object PkValue
        {
            get
            {
                if (_pkValue == null)
                {
                    string result = string.Empty;
                    var type = this.GetType();
                    var propertise = type.GetProperties();
                    foreach (var item in propertise)
                    {
                        var attrs = item.GetCustomAttributes(typeof(PrimaryKeyAttribute), false);
                        if (attrs.Count() > 0)
                        {
                            _pkValue = item.GetValue(this);
                        }
                    }
                }
                return _pkValue;
            }
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
                var attrs = item.GetCustomAttributes(typeof(DBFieldAttribute), false);
                if (attrs.Count() > 0)
                {
                    var attr = attrs[0] as DBFieldAttribute;
                    var value = item.GetValue(this);
                    values.Add(attr.FieldName);
                    param.Add("@" + item.Name);
                    paramDic.Add("@" + item.Name, value);
                }
            }
            sql = string.Format("insert into {0} ({1}) values({2})", TableName, string.Join(",", values), string.Join(",", param));
            return new Tuple<string, Dictionary<string, object>>(sql, paramDic);
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

        public Tuple<string, Dictionary<string, object>> GetUpdateSql(List<string> onlyUpdate)
        {
            string sql = string.Empty;
            var setStr = string.Empty;
            var paramDic = new Dictionary<string, object>();
            var whereStr = string.Format("{0} = @{1}", PkName, PkName);
            paramDic.Add("@" + PkName, PkValue);
            var setTup = GetUpdateSet(onlyUpdate);
            setStr = setTup.Item1.Trim(',');
            foreach (var item in setTup.Item2)
            {
                paramDic.Add(item.Key,item.Value);
            }
            sql = string.Format("update {0} set {1} where {2}", TableName, setStr, whereStr);
            return new Tuple<string, Dictionary<string, object>>(sql, paramDic);

        }

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
                        if (attr.FieldName != PkName)
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
    }
}
