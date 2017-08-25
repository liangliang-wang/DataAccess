using System;

namespace DataAccess.Attributes
{
    /// <summary>
    /// 字段特性类
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DBFieldAttribute : Attribute
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public string FieldName { set; get; }
    }
}
