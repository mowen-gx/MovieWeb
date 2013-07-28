using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace MovieWeb.DataUtil
{
    /// <summary>
    /// SQL数据生成器
    /// 功能：将对象信息以及对象赋值信息转换成查询SQL
    /// 作者：莫文
    /// 时间：2011-11-1
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SqlBuilder<T> where T : class
    {
        private TypeParser _parser;
        private DbDescription _dbDescription;

        /// <summary>
        /// 设置目标对象
        /// </summary>
        public void SetType()
        {
            _parser = new TypeParser(typeof(T));
        }
        
        /// <summary>
        /// 设置目标对象
        /// </summary>
        /// <param name="ts"></param>
        public void SetTarget(params T[] ts)
        {
            _parser = new TypeParser(typeof(T), ts);
            _dbDescription = null;//需要重新获取，以获得新的值
        }

        /// <summary>
        /// 解析器
        /// </summary>
        public TypeParser Parser
        {
            get { return _parser; }
        }

        /// <summary>
        /// 数据库结构以及查询参数
        /// </summary>
        public DbDescription Description
        {
            get
            {
                _dbDescription = _dbDescription ?? _parser.Result;
                return _dbDescription; 
            }
        }

        /// <summary>
        /// 获取检索条件
        /// </summary>
        /// <returns></returns>
        private string GetCondition()
        {
            string fieldSetString = "";
            foreach (Dictionary<Field, List<SqlParameter>> dict in Description.Paras ?? new List<Dictionary<Field, List<SqlParameter>>>())
            {
                foreach (KeyValuePair<Field, List<SqlParameter>> pair in dict)
                {
                    foreach (SqlParameter parameter in pair.Value)
                    {
                        fieldSetString += string.Format(" AND [{0}]={1}", pair.Key.Name.Trim(), parameter.ParameterName);
                    }
                }
            }
            if (!string.IsNullOrEmpty(fieldSetString))
                fieldSetString = " WHERE " + fieldSetString.Substring(4);
            return fieldSetString;
        }

        /// <summary>
        /// 获取插入数据SQL
        /// </summary>
        /// <returns></returns>
        public Sql BuildCreatSql()
        {
            string fieldString = string.Empty;
            string fieldParaString = string.Empty;
            foreach (Dictionary<Field, List<SqlParameter>> dictionary in Description.Paras)
            {
                foreach (KeyValuePair<Field, List<SqlParameter>> pair in dictionary)
                {
                    if (pair.Key.IsIdentification)
                        continue;
                    foreach (SqlParameter parameter in pair.Value)
                    {
                        fieldString += ",[" + pair.Key.Name + "]";
                        fieldParaString += "," + parameter.ParameterName;
                    }
                }
            }
           
            fieldString = fieldString.Trim(',');
            fieldParaString = fieldParaString.Trim(',');

            return new Sql
                       {
                           SqlString = string.Format(
                               @"INSERT INTO {0}({1})
                                 VALUES({2});
                                 SELECT SCOPE_IDENTITY();",
                               Description.TableName, fieldString.Trim(','), fieldParaString.Trim(',')),
                           Paras = Description.Paras
                       };
        }

        /// <summary>
        /// 获取插入数据SQL
        /// </summary>
        /// <returns></returns>
        public Sql BuildBatchCreatSql()
        {
            List<FieldCondition> fieldConditions = new List<FieldCondition>();
            
            foreach (Dictionary<Field, List<SqlParameter>> dictionary in Description.Paras)
            {
                FieldCondition condition = new FieldCondition();
                foreach (KeyValuePair<Field, List<SqlParameter>> pair in dictionary)
                {
                    if (pair.Key.IsPrimary && pair.Key.DbType == FieldType.Int)
                    {
                        if (pair.Value.Count > 0)
                            condition.ExistsString = " [" + pair.Key.Name + "] = " + pair.Value[0].ParameterName;
                        else
                            condition.ExistsString = " 1=2 ";
                    }

                    if (pair.Key.IsIdentification)
                        continue;
                    condition.UpdateSetString = pair.Value.Aggregate(condition.UpdateSetString, (current, parameter) => current + (",[" + pair.Key.Name + "]=" + parameter.ParameterName));
                    foreach (SqlParameter parameter in pair.Value)
                    {
                        condition.FieldSetString += ",[" + pair.Key.Name + "]";
                        condition.FieldValueString += "," + parameter.ParameterName;
                    }
                }
                condition.FieldSetString =condition.FieldSetString.Trim(',');
                condition.FieldValueString = condition.FieldValueString.Trim(',');
                fieldConditions.Add(condition);
            }

            StringBuilder sql = new StringBuilder();
            foreach (FieldCondition condition in fieldConditions)
            {
                if(string.IsNullOrEmpty(condition.ExistsString))
                {
                    condition.ExistsString = " 1=2 ";
                }
                sql.AppendFormat("\r\n");
                sql.Append(string.Format(
                    @" IF NOT EXISTS( SELECT {0} FROM {1}(nolock) WHERE {2})
                       INSERT INTO {1}({3}) VALUES({4})
                       ELSE 
                       UPDATE {1} SET {5} WHERE {2};",
                    Description.PrimaryField, 
                    Description.TableName, 
                    condition.ExistsString, 
                    condition.FieldSetString.Trim(','), 
                    condition.FieldValueString.Trim(','),
                    condition.UpdateSetString.Trim(',')));
            }

            return new Sql
            {
                SqlString = sql.ToString(),
                Paras = Description.Paras
            };
        }

        /// <summary>
        /// 获取更新数据SQL
        /// </summary>
        /// <returns></returns>
        public Sql BuildUpdateSql()
        {
            string fieldSetString = "";
            if (Description.Paras.Count > 0)
                foreach (KeyValuePair<Field, List<SqlParameter>> pair in Description.Paras[0])
                {
                    if(pair.Key.IsIdentification)
                        continue;
                    fieldSetString = pair.Value.Aggregate(fieldSetString, (current, parameter) => current + (",[" + pair.Key.Name + "]=" + parameter.ParameterName));
                }
            fieldSetString = fieldSetString.Trim(',');
            return new Sql
            {
                SqlString = string.Format(
                    @"Update {0} SET {1} WHERE [{2}]=@{2}_1_1",
                    Description.TableName, fieldSetString, Description.PrimaryField),
                Paras = Description.Paras
            };
        }

        /// <summary>
        /// 获取删除数据SQL
        /// </summary>
        /// <returns></returns>
        public Sql BuildDeleteSql(string fieldValues)
        {
            return new Sql
            {
                SqlString = string.Format(
                    @"DELETE FROM {0} WHERE [{1}] IN ({2})",
                    Description.TableName, Description.PrimaryField, fieldValues),
                Paras = Description.Paras
            };
        }

        /// <summary>
        /// 获取删除数据SQL
        /// </summary>
        /// <returns></returns>
        public Sql BuildDeleteByConditionSql(string condition)
        {
            if (!string.IsNullOrEmpty(condition))
                if (!condition.ToLower().Contains("where"))
                    condition = " WHERE " + condition;
            return new Sql
            {
                SqlString = string.Format(
                    @"DELETE FROM {0} {1}",
                    Description.TableName, condition),
                Paras = Description.Paras
            };
        }

        /// <summary>
        /// 获取查询单个数据SQL
        /// </summary>
        /// <returns></returns>
        public Sql BuildSearchOneSql()
        {
            if (Description.Paras.Count > 0)
            {
                if (Description.Paras[0].ContainsKey(Description.Primary))
                {
                    if (Description.Paras[0][Description.Primary].Count > 0)
                    {
                        return new Sql{    SqlString = string.Format(
                                           @"SELECT TOP 1 * FROM {0}(nolock) WHERE [{1}]=" +Description.Paras[0][Description.Primary][0].ParameterName,
                                           Description.TableName, Description.PrimaryField),
                                           Paras = new List<Dictionary<Field, List<SqlParameter>>>{
                                                       new Dictionary<Field, List<SqlParameter>>{
                                                               {Description.Primary,Description.Paras[0][Description.Primary]}
                                                           }
                                                   }
                                   };
                    }
                }
            }
            return null;

        }


        /// <summary>
        /// 获取查询单个数据SQL
        /// </summary>
        /// <returns></returns>
        public Sql BuildSearchSql()
        {
            return new Sql
            {
                SqlString = string.Format(
                    @"SELECT * FROM {0}(nolock) {1}",
                    Description.TableName, GetCondition()),
                Paras = Description.Paras 
            };
        }

        /// <summary>
        /// 获取查询数据SQL
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public Sql BuildSearchSqlByCondition(string condition)
        {
            string fieldSetString = GetCondition();
            return new Sql
            {
                SqlString = string.Format(
                    @"SELECT * FROM {0}(nolock) {1} {2}",
                    Description.TableName, fieldSetString, (!string.IsNullOrEmpty(fieldSetString) ? " AND " : ((!string.IsNullOrEmpty(condition)) ? " WHERE " : "")) + condition),
                Paras = Description.Paras
            };
        }

        /// <summary>
        /// 获取查询数据记录数SQL
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public Sql BuildSearchCountSqlByCondition(string condition)
        {
            string fieldSetString = GetCondition();

            return new Sql
            {
                SqlString = string.Format(
                    @"SELECT COUNT(*) COUNT FROM {0}(nolock) {1} {2}",
                    Description.TableName, fieldSetString, (!string.IsNullOrEmpty(fieldSetString) ? " AND " : ((!string.IsNullOrEmpty(condition))? " WHERE " : "")) + condition),
                Paras = Description.Paras
            };
        }


        /// <summary>
        /// 分页查询数据SQL
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="ascending">排序</param>
        /// <returns></returns>
        public Sql BuildSearchPageingSqlByCondition(string condition, int page, int pageSize,string orderField, string ascending)
        {
            string fieldSetString = GetCondition().Replace("WHERE", " ");

            if (string.IsNullOrEmpty(orderField))
                orderField = Description.PrimaryField;

            //组装快速分页SQL
            var fp = new FastPaging
            {
                PageIndex = page,
                PageSize = pageSize,
                TableName = string.Format("{0}(nolock)", Description.TableName),
                TableReName = "",
                JoinSQL = "  ",
                QueryFields = "*",
                OverOrderBy = orderField,
                PrimaryKey = Description.PrimaryField,
                Condition = (!string.IsNullOrEmpty(fieldSetString) ? " AND " : "") + condition,
                Ascending = FastPaging.IsAscending(ascending)
            };
            return new Sql
                       {
                           SqlString = fp.Build2005(),
                           Paras = Description.Paras
                       };
        }
    }
}
