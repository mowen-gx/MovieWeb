using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace MovieWeb.DataUtil
{
    public class FieldHelper
    {
        /// <summary>
        /// 利用反射获取字段的值
        /// 功能：数据库字段信息工具类
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        /// <param name="p">类型</param>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static object GetFieldValue(PropertyInfo p, Object obj)
        {
            object value = p.GetValue(obj, null);
            if (value == null)
                return null;
            return value;
        }

        /// <summary>
        /// 利用反射获取字段的值
        /// </summary>
        /// <param name="p">类型</param>
        /// <param name="attribute">attribute</param>
        /// <param name="obj">对象</param>
        /// <param name="index"> </param>
        /// <param name="order">参数顺序</param>
        /// <returns></returns>
        public static SqlParameter GetFieldSqlPara(PropertyInfo p,FieldAttribute attribute, Object obj,int index,int order)
        {
            object value = p.GetValue(obj, null);
            return GetParaByDbType(p.Name, GetFieldType(p, attribute), value, index, order);
        }

        /// <summary>
        /// 利用反射获取字段的值
        /// </summary>
        /// <param name="p">类型</param>
        /// <param name="fieldType">attribute</param>
        /// <param name="obj">对象</param>
        /// <param name="index"> </param>
        /// <param name="order">参数顺序</param>
        /// <returns></returns>
        public static SqlParameter GetFieldSqlPara(PropertyInfo p, string fieldType, Object obj, int index, int order)
        {
            object value = p.GetValue(obj, null);
            return GetParaByDbType(p.Name,fieldType, value,index, order);
        }

        /// <summary>
        /// 根据数据库类型创建数据库查询参数
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="dbType"></param>
        /// <param name="value"></param>
        /// <param name="order"></param>
        /// <param name="index"> </param>
        /// <returns></returns>
        public static SqlParameter GetParaByDbType(string fieldName,string dbType, object value,int index,int order)
        {
            switch (dbType)
            {
                case FieldType.Int:
                    int t;
                    if (int.TryParse(value.ToString(), out t))
                    {
                        return new SqlParameter("@" + fieldName + "_" + index+"_" + order, SqlDbType.Int, 4) { Value = t };
                    }
                    break;
                case FieldType.SmallInt:
                    Int16 t1;
                    if (Int16.TryParse(value.ToString(), out t1))
                    {
                        return new SqlParameter("@" + fieldName + "_" + index + "_" + order, SqlDbType.SmallInt, 2) { Value = t1 };
                    }
                    break;
                case FieldType.DateTime:
                    DateTime t3;
                    if (DateTime.TryParse(value.ToString(), out t3))
                    {
                        return new SqlParameter("@" + fieldName + "_" + index + "_" + order, SqlDbType.DateTime, 20) { Value = t3 };
                    }
                    break;
                case FieldType.Boolean:
                    Boolean t4;
                    if (Boolean.TryParse(value.ToString(), out t4))
                    {
                        return new SqlParameter("@" + fieldName + "_" + index + "_" + order, SqlDbType.SmallInt, 4) { Value = t4 ? "1" : "0" };
                    }
                    break;
                case FieldType.NVarChar:
                    string val = value.ToString().Trim();
                    if (val.EndsWith("like", StringComparison.OrdinalIgnoreCase))
                        val = val.Substring(0, val.Length - 5);
                    return new SqlParameter("@" + fieldName + "_" + index + "_" + order, SqlDbType.NVarChar, val.Length) { Value = val };
            }
            return new SqlParameter("@" + fieldName + "_" + index + "_" + order, SqlDbType.NVarChar, value.ToString().Length) { Value = value.ToString() };
        }

        /// <summary>
        /// 获取字段的C#数据类型
        /// </summary>
        /// <param name="p"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static string GetCSharpType(PropertyInfo p, FieldAttribute attribute)
        {
            if (attribute != null)
            {
                if (!string.IsNullOrEmpty(attribute.DbType))
                    return attribute.DbType;
            }
            string baseType;
            if (p.PropertyType.Name.Contains("Nullable`1") && !string.IsNullOrEmpty(p.PropertyType.FullName))
            {
                //System.Nullable`1[[System.Decimal, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]
                baseType = p.PropertyType.FullName.Replace("System.Nullable`1[[System.", "");
                baseType = baseType.Substring(0, baseType.IndexOf(","));
            }
            else
            {
                baseType = p.PropertyType.Name;
            }
            return baseType;
        }

        /// <summary>
        /// 获取字段类型
        /// </summary>
        /// <param name="p">字段属性</param>
        /// <param name="attribute">标签属性</param>
        /// <returns></returns>
        public static string GetFieldType(PropertyInfo p,FieldAttribute attribute)
        {
            switch (GetCSharpType(p,attribute))
            {
                case "Int32":
                    return FieldType.Int;
                case "Int16":
                    return FieldType.SmallInt;
                case "DateTime":
                    return FieldType.DateTime;
                case "String":
                    return FieldType.NVarChar;
                case "Byte":
                case "Bit":
                    return FieldType.Bit;
                case "Boolean":
                    return FieldType.Boolean;
                case "Decimal":
                    return FieldType.Decimal;
                default:
                    return string.Empty;
            }
        }
    }
}
