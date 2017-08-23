using System;

namespace DataAccess.Attributes
{
    /// <summary>
    /// 实体特性
    /// </summary>
    public class EntityAttribute : Attribute
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="tableName"></param>
        public EntityAttribute(string tableName)
        {
            this.TableName = tableName;
        }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; private set; }
    }
}
