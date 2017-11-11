using System;

namespace DataAccess.Attributes
{
    /// <summary>
    /// 实体特性
    /// </summary>
    public class DataBaseAttribute : Attribute
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbName"></param>
        public DataBaseAttribute(string dbName)
        {
            this.DBName = dbName;
        }

        /// <summary>
        /// 表名
        /// </summary>
        public string DBName { get; private set; }
    }
}
