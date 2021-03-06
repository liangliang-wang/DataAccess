﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace DataAccess.Tools
{
    /// <summary>枚举帮助类</summary>
    public static class EnumHelper
    {
        private static readonly ConcurrentDictionary<Type, Dictionary<string, EnumItem>> EnumAbout
         = new ConcurrentDictionary<Type, Dictionary<string, EnumItem>>();
        private static string GetName(Type t, object v)
        {
            try
            {
                return System.Enum.GetName(t, v);
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 根据传入的枚举值获取对应的枚举描述
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="v">枚举值</param>
        /// <returns>枚举描述</returns>
        public static string GetDescription<T>(this object v)
        {
            Type type = typeof(T);
            try
            {
                FieldInfo oFieldInfo = type.GetField(GetName(type, v));
                var attributes = (DescriptionAttribute[])oFieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                return (attributes.Length > 0) ? attributes[0].Description : GetName(type, v);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 根据传入的枚举描述获取对应的枚举值
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="description">枚举描述</param>
        /// <returns>枚举值，找不到则返回0</returns>
        public static int GetEnumName<T>(this string description)
        {
            Type _type = typeof(T);
            foreach(FieldInfo field in _type.GetFields())
            {
                DescriptionAttribute[] _curDesc = field.GetDescriptAttr();
                if(_curDesc != null && _curDesc.Length > 0)
                {
                    if(_curDesc[0].Description == description)
                        return (int)field.GetValue(null);
                }
                else
                {
                    if(field.Name == description)
                        return (int)field.GetValue(null);
                }
            }
            return 0;
        }

        private static DescriptionAttribute[] GetDescriptAttr(this FieldInfo fieldInfo)
        {
            if(fieldInfo != null)
            {
                return (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            }
            return null;
        }

        /// <summary>
        /// 枚举转换成数组
        /// </summary>
        /// <param name="t">枚举类型</param>
        /// <returns>枚举数组</returns>
        public static List<string> GetList(this Type t)
        {
            var result = new List<string>();
            if (t == null) return result;
            if (!t.IsEnum) return result;
            var array = System.Enum.GetValues(t);
            for (var i = 0; i < array.Length; i++)
            {
                result.Add(array.GetValue(i).ToString());
            }
            return result;
        }

        public static string GetDes(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            return ((DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))).Description;
        }

        public static Dictionary<string, EnumItem> GetItemList(this Enum value)
        {
            Type eType = value.GetType();
            if (!EnumAbout.ContainsKey(eType))
            {
                Type valueType = Enum.GetUnderlyingType(eType);
                var enums = Enum.GetValues(eType);
                Dictionary<string, EnumItem> tmpList = new Dictionary<string, EnumItem>();
                foreach (Enum e in enums)
                    tmpList.Add(Convert.ChangeType(e, valueType).ToString(), new EnumItem
                    {
                        ItemText = e.ToString(),
                        ItemValue = Convert.ChangeType(e, valueType).ToString(),
                        ItemDes = e.GetDes()
                    });
                EnumAbout.TryAdd(eType, tmpList);
            }
            return EnumAbout[eType];
        }

        public class EnumItem
        {
            public string ItemText { get; set; }
            public string ItemValue { get; set; }
            public string ItemDes { get; set; }
            public const string ItemValueField = "ItemValue";
            public const string ItemTextField = "ItemText";
            public const string ItemDesField = "ItemDes";
        }
    }
}
