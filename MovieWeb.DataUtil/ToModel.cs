using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MovieWeb.DataUtil
{
    /// <summary>
    /// 功能：通过反射将数据转换成实体
    /// 作者：莫文
    /// 时间：2011-11-1
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ToModel<T>  where T : class
    {
        /// <summary>
        /// 获取当前类型的字段信息
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string,Field> GetFields()
        {
            TypeParser parser = new TypeParser(typeof(T));
            return parser.Result.Fields ?? new Dictionary<string, Field>();
        }

        /// <summary>
        /// 将数据行转化成实体对象
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private static T RowToModel(DataRow row)
        {
            string id = string.Empty;
            return RowToModel(row,string.Empty,ref id);
        }

        /// <summary>
        /// 将数据行转化成实体对象
        /// </summary>
        /// <param name="row"></param>
        /// <param name="key"></param>
        /// <param name="identity"></param>
        /// <returns></returns>
        private static T RowToModel(DataRow row,string key,ref string identity)
        {
            if (row != null)
            {
                Type type = typeof(T);
                T model = (T)Activator.CreateInstance(type);
                foreach (KeyValuePair<string, Field> fields in GetFields())
                {
                    if (fields.Value == null)
                        continue;

                    if (row[fields.Value.Name] != null)
                    {
                        if (fields.Value.IsPrimary && string.IsNullOrEmpty(key))
                            identity = row[fields.Value.Name].ToString();
                        if (fields.Value.Name.Trim().ToLower() == (key??string.Empty).Trim().ToLower())
                            identity = row[fields.Value.Name].ToString();
                        switch (fields.Value.DbType)
                        {
                            case FieldType.Int:
                                int t;
                                if (int.TryParse(row[fields.Value.Name].ToString(), out t) && fields.Value.Type == "Int32")
                                {
                                    fields.Value.Property.SetValue(model, t, null);
                                }

                                if (fields.Value.Type == "Boolean")
                                {
                                    fields.Value.Property.SetValue(model, row[fields.Value.Name].ToString() == "1" ? true : false, null);
                                }
                                break;
                            case FieldType.Decimal:
                                decimal td;
                                if (Decimal.TryParse(row[fields.Value.Name].ToString(), out td))
                                {
                                    fields.Value.Property.SetValue(model, td, null);
                                }
                                break;
                            case FieldType.SmallInt:
                                Int16 t1;
                                if (Int16.TryParse(row[fields.Value.Name].ToString(), out t1)
                                    && (fields.Value.Type == "Int32"
                                    || fields.Value.Type == "Int16"))
                                {
                                    fields.Value.Property.SetValue(model, t1, null);
                                }

                                if (fields.Value.Type == "Boolean")
                                {
                                    fields.Value.Property.SetValue(model, row[fields.Value.Name].ToString() == "1" ? true : false, null);
                                }

                                break;
                            case FieldType.DateTime:
                                DateTime t3;
                                if (DateTime.TryParse(row[fields.Value.Name].ToString(), out t3))
                                {
                                    fields.Value.Property.SetValue(model, t3, null);
                                }
                                break;
                            case FieldType.Boolean:
                                bool t4 = row[fields.Value.Name].ToString() == "1" ? true : false;
                                fields.Value.Property.SetValue(model, t4, null);
                                break;
                            case FieldType.Bit:
                                if (fields.Value.Type == "Boolean")
                                {
                                    bool t5 = row[fields.Value.Name].ToString() == "1" ? true : false;
                                    fields.Value.Property.SetValue(model, t5, null);
                                }
                                else
                                {
                                    fields.Value.Property.SetValue(model, "True".Equals(row[fields.Value.Name].ToString().Trim()) ? true : false, null);
                                }
                                break;
                            case FieldType.NVarChar:
                                fields.Value.Property.SetValue(model, row[fields.Value.Name].ToString(), null);
                                break;
                            default:
                                try
                                {
                                    fields.Value.Property.SetValue(model, row[fields.Value.Name].ToString(), null);
                                }
                                catch (Exception ex)
                                {

                                }
                                break;
                        }
                    }
                }
                return model;
            }
            return null;
        }

        /// <summary>
        /// 将一个数据表转化成
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> TableToModelList(DataTable dt)
        {
            var list = new List<T>();
            if (dt == null)
                return list;
            if (dt.Rows.Count < 1)
                return list;
            list.AddRange(dt.Rows.Cast<DataRow>().Select(RowToModel).Where(model => model != null));
            return list;
        }

        /// <summary>
        /// 将一个数据表转化成
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static Dictionary<string, T> TableToModelDic(string key,DataTable dt)
        {
            var list = new Dictionary<string, T>();
            if (dt == null)
                return list;
            if (dt.Rows.Count < 1)
                return list;
            foreach (DataRow row in dt.Rows)
            {
                string id = string.Empty;
                T model = RowToModel(row,key, ref id);
                if (model != null && !list.ContainsKey(id))
                {
                    list.Add(id, model);
                }
            }
            return list;
        }

        /// <summary>
        /// 将一个数据表转化成
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static Dictionary<string,T> TableToModelDic(DataTable dt)
        {
            var list = new Dictionary<string,T>();
            if (dt == null)
                return list;
            if (dt.Rows.Count < 1)
                return list;
            foreach (DataRow row in dt.Rows)
            {
                string id = string.Empty;
                T model = RowToModel(row,string.Empty, ref id);
                if(model != null && !list.ContainsKey(id))
                {
                    list.Add(id,model);
                }
            }
            return list;
        }

        /// <summary>
        /// 将一个数据表转化成
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static T TableToModelOne(DataTable dt)
        {
            var list = new List<T>();
            if (dt == null)
                return null;
            if (dt.Rows.Count < 1)
                return null;
            list.AddRange(dt.Rows.Cast<DataRow>().Select(RowToModel).Where(model => model != null));
            return list.Count > 0 ? list[0] : null;
        }
    }
}
