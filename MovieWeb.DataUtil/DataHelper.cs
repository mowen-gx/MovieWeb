﻿using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace MovieWeb.DataUtil
{
    /// <summary>
    /// 数据库执行类
    /// 功能：执行ORM转换
    /// 作者：莫文
    /// 时间：2011-11-1
    /// </summary>
    public class DataHelper
    {
        private static Dictionary<string, string> _connectionStrings = new Dictionary<string, string>();
        private static string _curconnectionStringsKey = "default";

        /// <summary>
        /// 默认自动配置数据库连接字符串
        /// </summary>
        static DataHelper()
        {
            SetConnectionString("default",new MovieConn().Get());
        }

        /// <summary>
        /// 读取数据库连接
        /// </summary>
        /// <returns></returns>
        public static void SetConnectionString(string name, string conn)
        {
            if (!string.IsNullOrEmpty(conn) && !string.IsNullOrEmpty(name))
            {
                _curconnectionStringsKey = name;
                if (!_connectionStrings.ContainsKey(name))
                    _connectionStrings.Add(name, conn);
                else
                    _connectionStrings[name] = conn;
            }
        }

        /// <summary>
        /// 获取数据库连接字符串
        /// </summary>
        /// <returns></returns>
        public static string SetCurConnectionString(string name)
        {
            _curconnectionStringsKey = name;
            if (_connectionStrings.ContainsKey(name))
                return _connectionStrings[name];
            return string.Empty;
        }

        /// <summary>
        /// 获取数据库连接字符串
        /// </summary>
        /// <returns></returns>
        public static string SetCurConnectionString()
        {
            if (string.IsNullOrEmpty(_curconnectionStringsKey))
            {
                if (_connectionStrings.ContainsKey("AppSettings_ConnectionString"))
                {
                    if (!string.IsNullOrEmpty(_connectionStrings["AppSettings_ConnectionString"]))
                    {
                        return _connectionStrings["AppSettings_ConnectionString"];
                    }
                }
                return string.Empty;
            }
            return SetCurConnectionString(_curconnectionStringsKey);
        }

        /// <summary>
        /// 返回参数数组
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        private static SqlParameter[] GetParameter(List<Dictionary<Field, List<SqlParameter>>> paras)
        {
            List<SqlParameter> ret = new List<SqlParameter>();
            foreach (Dictionary<Field, List<SqlParameter>> dictionary in paras)
            {
                foreach (KeyValuePair<Field, List<SqlParameter>> pair in dictionary)
                {
                    ret.AddRange(pair.Value.Select(parameter =>
                        new SqlParameter(parameter.ParameterName, parameter.SqlDbType, parameter.Size) { Value = parameter.Value }));
                }
            }
            return ret.ToArray();
        }

        /// <summary>
        /// 插入数据
        /// 执行过程：
        /// （1）、根据类型反射得到对象的数据信息
        /// （2）、根据类型信息生成SQL以及参数信息
        /// （3）、执行SQL返回生成的数据的ID
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        /// <returns></returns>
        public static int Creat<T>(T t) where T : class
        {
            SqlBuilder<T> builder = new SqlBuilder<T>();
            builder.SetTarget(t);
            Sql sql = builder.BuildCreatSql();
            var helper = new DbHelperSql(SetCurConnectionString());
            return helper.ExecuteSql(sql.SqlString, GetParameter(sql.Paras));
        }


        /// <summary>
        /// 更新数据
        /// 执行过程：
        /// （1）、根据类型反射得到对象的数据信息
        /// （2）、根据类型信息生成SQL以及参数信息
        /// （3）、执行SQL返回
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        /// <returns></returns>
        public static int Update<T>(T t) where T : class,new()
        {
            SqlBuilder<T> builder = new SqlBuilder<T>();
            builder.SetTarget(t);
            Sql sql = builder.BuildUpdateSql();
            var helper = new DbHelperSql(SetCurConnectionString());
            return helper.ExecuteSql(sql.SqlString, GetParameter(sql.Paras));
        }

        /// <summary>
        /// 删除数据
        /// 执行过程：
        /// （1）、根据类型反射得到对象的数据信息
        /// （2）、根据类型信息生成SQL以及参数信息
        /// （3）、执行SQL返回
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        /// <param name="fieldValues"></param>
        /// <returns></returns>
        public static int Delete<T>(string fieldValues) where T : class,new()
        {
            SqlBuilder<T> builder = new SqlBuilder<T>();
            builder.SetType();
            Sql sql = builder.BuildDeleteSql(fieldValues);
            var helper = new DbHelperSql(SetCurConnectionString());
            return helper.ExecuteSql(sql.SqlString, GetParameter(sql.Paras));
        }

        /// <summary>
        /// 删除数据
        /// 执行过程：
        /// （1）、根据类型反射得到对象的数据信息
        /// （2）、根据类型信息生成SQL以及参数信息
        /// （3）、执行SQL返回
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static int DeleteByCondition<T>(string condition) where T : class,new()
        {
            SqlBuilder<T> builder = new SqlBuilder<T>();
            builder.SetType();
            Sql sql = builder.BuildDeleteByConditionSql(condition);
            var helper = new DbHelperSql(SetCurConnectionString());
            return helper.ExecuteSql(sql.SqlString, GetParameter(sql.Paras));
        }

        /// <summary>
        /// 获取查询单个数据
        /// 执行过程：
        /// （1）、根据类型反射得到对象的数据信息
        /// （2）、根据类型信息生成SQL以及参数信息
        /// （3）、执行SQL返回
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        /// <returns></returns>
        public static T SearchOne<T>(T t) where T : class,new()
        {
            SqlBuilder<T> builder = new SqlBuilder<T>();
            builder.SetTarget(t);
            Sql sql = builder.BuildSearchOneSql();
            var helper = new DbHelperSql(SetCurConnectionString());
            return ToModel<T>.TableToModelOne(helper.Query(sql.SqlString, GetParameter(sql.Paras)).Tables[0]);
        }

        /// <summary>
        /// 获取查询单个数据
        /// 执行过程：
        /// （1）、根据类型反射得到对象的数据信息
        /// （2）、根据类型信息生成SQL以及参数信息
        /// （3）、执行SQL返回
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        /// <returns></returns>
        public static List<T> Search<T>(T t) where T : class,new()
        {
            SqlBuilder<T> builder = new SqlBuilder<T>();
            builder.SetTarget(t);
            Sql sql = builder.BuildSearchSql();
            var helper = new DbHelperSql(SetCurConnectionString());
            return ToModel<T>.TableToModelList(helper.Query(sql.SqlString, GetParameter(sql.Paras)).Tables[0]);
        }

        /// <summary>
        /// 获取查询数据
        /// 执行过程：
        /// （1）、根据类型反射得到对象的数据信息
        /// （2）、根据类型信息生成SQL以及参数信息
        /// （3）、执行SQL返回
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, T> SearchForDic<T>(T t) where T : class,new()
        {
            SqlBuilder<T> builder = new SqlBuilder<T>();
            builder.SetTarget(t);
            Sql sql = builder.BuildSearchSql();
            var helper = new DbHelperSql(SetCurConnectionString());
            return ToModel<T>.TableToModelDic(helper.Query(sql.SqlString, GetParameter(sql.Paras)).Tables[0]);
        }

        /// <summary>
        /// 获取查询数据
        /// 执行过程：
        /// （1）、根据类型反射得到对象的数据信息
        /// （2）、根据类型信息生成SQL以及参数信息
        /// （3）、执行SQL返回
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, T> SearchForDic<T>(T t, string dickey) where T : class,new()
        {
            SqlBuilder<T> builder = new SqlBuilder<T>();
            builder.SetTarget(t);
            Sql sql = builder.BuildSearchSql();
            var helper = new DbHelperSql(SetCurConnectionString());
            return ToModel<T>.TableToModelDic(dickey, helper.Query(sql.SqlString, GetParameter(sql.Paras)).Tables[0]);
        }

        /// <summary>
        /// 获取查询数据
        /// 执行过程：
        /// （1）、根据类型反射得到对象的数据信息
        /// （2）、根据类型信息生成SQL以及参数信息
        /// （3）、执行SQL返回
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        /// <returns></returns>
        public static List<T> Search<T>() where T : class,new()
        {
            SqlBuilder<T> builder = new SqlBuilder<T>();
            builder.SetType();
            Sql sql = builder.BuildSearchSql();
            var helper = new DbHelperSql(SetCurConnectionString());
            return ToModel<T>.TableToModelList(helper.Query(sql.SqlString, GetParameter(sql.Paras)).Tables[0]);
        }

        /// <summary>
        /// 获取查询数据
        /// 执行过程：
        /// （1）、根据类型反射得到对象的数据信息
        /// （2）、根据类型信息生成SQL以及参数信息
        /// （3）、执行SQL返回
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, T> SearchForDic<T>() where T : class,new()
        {
            SqlBuilder<T> builder = new SqlBuilder<T>();
            builder.SetType();
            Sql sql = builder.BuildSearchSql();
            var helper = new DbHelperSql(SetCurConnectionString());
            return ToModel<T>.TableToModelDic(helper.Query(sql.SqlString, GetParameter(sql.Paras)).Tables[0]);
        }

        /// <summary>
        /// 获取查询数据
        /// 执行过程：
        /// （1）、根据类型反射得到对象的数据信息
        /// （2）、根据类型信息生成SQL以及参数信息
        /// （3）、执行SQL返回
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, T> SearchForDicUseDicKey<T>(string dicKey) where T : class,new()
        {
            SqlBuilder<T> builder = new SqlBuilder<T>();
            builder.SetType();
            Sql sql = builder.BuildSearchSql();
            var helper = new DbHelperSql(SetCurConnectionString());
            return ToModel<T>.TableToModelDic(dicKey, helper.Query(sql.SqlString, GetParameter(sql.Paras)).Tables[0]);
        }

        /// <summary>
        /// 获取查询数据
        /// 执行过程：
        /// （1）、根据类型反射得到对象的数据信息
        /// （2）、根据类型信息生成SQL以及参数信息
        /// （3）、执行SQL返回
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, T> SearchForDic<T>(string dickey) where T : class,new()
        {
            SqlBuilder<T> builder = new SqlBuilder<T>();
            builder.SetType();
            Sql sql = builder.BuildSearchSql();
            var helper = new DbHelperSql(SetCurConnectionString());
            return ToModel<T>.TableToModelDic(dickey, helper.Query(sql.SqlString, GetParameter(sql.Paras)).Tables[0]);
        }

        /// <summary>
        /// 获取查询数据
        /// 执行过程：
        /// （1）、根据类型反射得到对象的数据信息
        /// （2）、根据类型信息生成SQL以及参数信息
        /// （3）、执行SQL返回
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        /// <returns></returns>
        public static DataSet SearchForDataSet<T>(T t) where T : class,new()
        {
            SqlBuilder<T> builder = new SqlBuilder<T>();
            builder.SetTarget(t);
            Sql sql = builder.BuildSearchSql();
            var helper = new DbHelperSql(SetCurConnectionString());
            return helper.Query(sql.SqlString, GetParameter(sql.Paras));
        }

        /// <summary>
        /// 查询数据
        /// 执行过程：
        /// （1）、根据类型反射得到对象的数据信息
        /// （2）、根据类型信息生成SQL以及参数信息
        /// （3）、执行SQL返回
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        /// <param name="t"></param>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public static List<T> SearchByCondition<T>(T t, string condition) where T : class,new()
        {
            SqlBuilder<T> builder = new SqlBuilder<T>();
            builder.SetTarget(t);
            Sql sql = builder.BuildSearchSqlByCondition(condition);
            var helper = new DbHelperSql(SetCurConnectionString());
            return ToModel<T>.TableToModelList(helper.Query(sql.SqlString, GetParameter(sql.Paras)).Tables[0]);
        }

        /// <summary>
        /// 查询数据
        /// 执行过程：
        /// （1）、根据类型反射得到对象的数据信息
        /// （2）、根据类型信息生成SQL以及参数信息
        /// （3）、执行SQL返回
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        /// <param name="t"></param>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public static Dictionary<string, T> SearchByConditionForDic<T>(T t, string condition) where T : class,new()
        {
            SqlBuilder<T> builder = new SqlBuilder<T>();
            builder.SetTarget(t);
            Sql sql = builder.BuildSearchSqlByCondition(condition);
            var helper = new DbHelperSql(SetCurConnectionString());
            return ToModel<T>.TableToModelDic(helper.Query(sql.SqlString, GetParameter(sql.Paras)).Tables[0]);
        }

        /// <summary>
        /// 查询数据
        /// 执行过程：
        /// （1）、根据类型反射得到对象的数据信息
        /// （2）、根据类型信息生成SQL以及参数信息
        /// （3）、执行SQL返回
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        /// <param name="t"></param>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public static Dictionary<string, T> SearchByConditionForDic<T>(T t, string condition, string dicKey) where T : class,new()
        {
            SqlBuilder<T> builder = new SqlBuilder<T>();
            builder.SetTarget(t);
            Sql sql = builder.BuildSearchSqlByCondition(condition);
            var helper = new DbHelperSql(SetCurConnectionString());
            return ToModel<T>.TableToModelDic(dicKey, helper.Query(sql.SqlString, GetParameter(sql.Paras)).Tables[0]);
        }

        /// <summary>
        /// 查询数据
        /// 执行过程：
        /// （1）、根据类型反射得到对象的数据信息
        /// （2）、根据类型信息生成SQL以及参数信息
        /// （3）、执行SQL返回
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public static List<T> SearchByCondition<T>(string condition) where T : class,new()
        {
            SqlBuilder<T> builder = new SqlBuilder<T>();
            builder.SetType();
            Sql sql = builder.BuildSearchSqlByCondition(condition);
            var helper = new DbHelperSql(SetCurConnectionString());
            return ToModel<T>.TableToModelList(helper.Query(sql.SqlString, GetParameter(sql.Paras)).Tables[0]);
        }

        /// <summary>
        /// 查询数据
        /// 执行过程：
        /// （1）、根据类型反射得到对象的数据信息
        /// （2）、根据类型信息生成SQL以及参数信息
        /// （3）、执行SQL返回
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public static Dictionary<string, T> SearchByConditionForDic<T>(string condition) where T : class,new()
        {
            SqlBuilder<T> builder = new SqlBuilder<T>();
            builder.SetType();
            Sql sql = builder.BuildSearchSqlByCondition(condition);
            var helper = new DbHelperSql(SetCurConnectionString());
            return ToModel<T>.TableToModelDic(helper.Query(sql.SqlString, GetParameter(sql.Paras)).Tables[0]);
        }

        /// <summary>
        /// 查询数据
        /// 执行过程：
        /// （1）、根据类型反射得到对象的数据信息
        /// （2）、根据类型信息生成SQL以及参数信息
        /// （3）、执行SQL返回
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        /// <param name="key">字典Key</param>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public static Dictionary<string, T> SearchByConditionForDic<T>(string condition, string dickey) where T : class,new()
        {
            SqlBuilder<T> builder = new SqlBuilder<T>();
            builder.SetType();
            Sql sql = builder.BuildSearchSqlByCondition(condition);
            var helper = new DbHelperSql(SetCurConnectionString());
            return ToModel<T>.TableToModelDic(dickey, helper.Query(sql.SqlString, GetParameter(sql.Paras)).Tables[0]);
        }

        /// <summary>
        /// 查询数据
        /// 执行过程：
        /// （1）、根据类型反射得到对象的数据信息
        /// （2）、根据类型信息生成SQL以及参数信息
        /// （3）、执行SQL返回
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        /// <param name="t"></param>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public static DataSet SearchByConditionForDataSet<T>(T t, string condition) where T : class,new()
        {
            SqlBuilder<T> builder = new SqlBuilder<T>();
            builder.SetTarget(t);
            Sql sql = builder.BuildSearchSqlByCondition(condition);
            var helper = new DbHelperSql(SetCurConnectionString());
            return helper.Query(sql.SqlString, GetParameter(sql.Paras));
        }

        /// <summary>
        ///查询数据记录数
        /// 执行过程：
        /// （1）、根据类型反射得到对象的数据信息
        /// （2）、根据类型信息生成SQL以及参数信息
        /// （3）、执行SQL返回
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public static int SearchCountByCondition<T>(string condition) where T : class,new()
        {
            SqlBuilder<T> builder = new SqlBuilder<T>();
            builder.SetType();
            Sql sql = builder.BuildSearchCountSqlByCondition(condition);
            var helper = new DbHelperSql(SetCurConnectionString());
            DataSet ds = helper.Query(sql.SqlString, GetParameter(sql.Paras));
            if (ds != null)
                if (ds.Tables.Count > 0)
                    if (ds.Tables[0] != null)
                        if (ds.Tables[0].Rows.Count > 0)
                            if (ds.Tables[0].Rows[0]["COUNT"] != null)
                            {
                                int i;
                                if (int.TryParse(ds.Tables[0].Rows[0]["COUNT"].ToString(), out i))
                                {
                                    return i;
                                }
                            }
            return 0;
        }

        /// <summary>
        ///查询数据记录数
        /// 执行过程：
        /// （1）、根据类型反射得到对象的数据信息
        /// （2）、根据类型信息生成SQL以及参数信息
        /// （3）、执行SQL返回
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        ///<param name="t"></param>
        ///<param name="condition">查询条件</param>
        /// <returns></returns>
        public static int SearchCountByCondition<T>(T t, string condition) where T : class,new()
        {
            SqlBuilder<T> builder = new SqlBuilder<T>();
            builder.SetTarget(t);
            Sql sql = builder.BuildSearchCountSqlByCondition(condition);
            var helper = new DbHelperSql(SetCurConnectionString());
            DataSet ds = helper.Query(sql.SqlString, GetParameter(sql.Paras));
            if (ds != null)
                if (ds.Tables.Count > 0)
                    if (ds.Tables[0] != null)
                        if (ds.Tables[0].Rows.Count > 0)
                            if (ds.Tables[0].Rows[0]["COUNT"] != null)
                            {
                                int i;
                                if (int.TryParse(ds.Tables[0].Rows[0]["COUNT"].ToString(), out i))
                                {
                                    return i;
                                }
                            }
            return 0;
        }


        /// <summary>
        /// 分页查询数据SQL
        /// 执行过程：
        /// （1）、根据类型反射得到对象的数据信息
        /// （2）、根据类型信息生成SQL以及参数信息
        /// （3）、执行SQL返回
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        /// <param name="t"></param>
        /// <param name="condition">查询条件</param>
        /// <param name="dicKey"></param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="orderField"></param>
        /// <param name="ascending">排序</param>
        /// <returns></returns>
        public static Dictionary<string, T> SearchPageingByConditionForDic<T>(T t, string condition, string dicKey, int page, int pageSize, string orderField, string ascending) where T : class,new()
        {
            SqlBuilder<T> builder = new SqlBuilder<T>();
            builder.SetTarget(t);
            Sql sql = builder.BuildSearchPageingSqlByCondition(condition, page, pageSize,orderField, ascending);
            var helper = new DbHelperSql(SetCurConnectionString());
            return ToModel<T>.TableToModelDic(dicKey, helper.Query(sql.SqlString, GetParameter(sql.Paras)).Tables[0]);
        }

        /// <summary>
        /// 分页查询数据
        /// 执行过程：
        /// （1）、根据类型反射得到对象的数据信息
        /// （2）、根据类型信息生成SQL以及参数信息
        /// （3）、执行SQL返回
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        /// <param name="t"></param>
        /// <param name="condition">查询条件</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="orderField"></param>
        /// <param name="ascending">排序</param>
        /// <returns></returns>
        public static List<T> SearchPageingByCondition<T>(T t, string condition, int page, int pageSize, string orderField, string ascending) where T : class,new()
        {
            SqlBuilder<T> builder = new SqlBuilder<T>();
            builder.SetTarget(t);
            Sql sql = builder.BuildSearchPageingSqlByCondition(condition, page, pageSize, orderField, ascending);
            var helper = new DbHelperSql(SetCurConnectionString());
            return ToModel<T>.TableToModelList(helper.Query(sql.SqlString, GetParameter(sql.Paras)).Tables[0]);
        }

        /// <summary>
        /// 分页查询数据
        /// 执行过程：
        /// （1）、根据类型反射得到对象的数据信息
        /// （2）、根据类型信息生成SQL以及参数信息
        /// （3）、执行SQL返回
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="orderField"></param>
        /// <param name="ascending">排序</param>
        /// <returns></returns>
        public static List<T> SearchPageingByCondition<T>(string condition, int page, int pageSize, string orderField, string ascending) where T : class,new()
        {
            SqlBuilder<T> builder = new SqlBuilder<T>();
            builder.SetType();
            Sql sql = builder.BuildSearchPageingSqlByCondition(condition, page, pageSize, orderField, ascending);
            var helper = new DbHelperSql(SetCurConnectionString());
            return ToModel<T>.TableToModelList(helper.Query(sql.SqlString, GetParameter(sql.Paras)).Tables[0]);
        }

        /// <summary>
        /// 分页查询数据
        /// 执行过程：
        /// （1）、根据类型反射得到对象的数据信息
        /// （2）、根据类型信息生成SQL以及参数信息
        /// （3）、执行SQL返回
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="orderField"></param>
        /// <param name="ascending">排序</param>
        /// <returns></returns>
        public static Dictionary<string, T> SearchPageingByConditionForDic<T>(string condition, int page, int pageSize, string orderField, string ascending) where T : class,new()
        {
            SqlBuilder<T> builder = new SqlBuilder<T>();
            builder.SetType();
            Sql sql = builder.BuildSearchPageingSqlByCondition(condition, page, pageSize, orderField, ascending);
            var helper = new DbHelperSql(SetCurConnectionString());
            return ToModel<T>.TableToModelDic(helper.Query(sql.SqlString, GetParameter(sql.Paras)).Tables[0]);
        }

        /// <summary>
        /// 分页查询数据
        /// 执行过程：
        /// （1）、根据类型反射得到对象的数据信息
        /// （2）、根据类型信息生成SQL以及参数信息
        /// （3）、执行SQL返回
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <param name="dicKey"></param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="orderField"></param>
        /// <param name="ascending">排序</param>
        /// <returns></returns>
        public static Dictionary<string, T> SearchPageingByConditionForDic<T>(string condition, string dicKey, int page, int pageSize, string orderField, string ascending) where T : class,new()
        {
            SqlBuilder<T> builder = new SqlBuilder<T>();
            builder.SetType();
            Sql sql = builder.BuildSearchPageingSqlByCondition(condition, page, pageSize, orderField, ascending);
            var helper = new DbHelperSql(SetCurConnectionString());
            return ToModel<T>.TableToModelDic(dicKey, helper.Query(sql.SqlString, GetParameter(sql.Paras)).Tables[0]);
        }

        /// <summary>
        /// 分页查询数据
        /// 执行过程：
        /// （1）、根据类型反射得到对象的数据信息
        /// （2）、根据类型信息生成SQL以及参数信息
        /// （3）、执行SQL返回
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="orderField"></param>
        /// <param name="ascending">排序</param>
        /// <returns></returns>
        public static DataSet SearchPageingByConditionForDataSet<T>(string condition, int page, int pageSize, string orderField, string ascending) where T : class,new()
        {
            SqlBuilder<T> builder = new SqlBuilder<T>();
            builder.SetType();
            Sql sql = builder.BuildSearchPageingSqlByCondition(condition, page, pageSize, orderField, ascending);
            var helper = new DbHelperSql(SetCurConnectionString());
            return helper.Query(sql.SqlString, GetParameter(sql.Paras));
        }

        /// <summary>
        /// 分页查询数据
        /// 执行过程：
        /// （1）、根据类型反射得到对象的数据信息
        /// （2）、根据类型信息生成SQL以及参数信息
        /// （3）、执行SQL返回
        /// 作者：莫文
        /// 时间：2011-11-1
        /// </summary>
        /// <param name="t"></param>
        /// <param name="condition">查询条件</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="orderField"></param>
        /// <param name="ascending">排序</param>
        /// <returns></returns>
        public static DataSet SearchPageingByConditionForDataSet<T>(T t, string condition, int page, int pageSize, string orderField, string ascending) where T : class,new()
        {
            SqlBuilder<T> builder = new SqlBuilder<T>();
            builder.SetTarget(t);
            Sql sql = builder.BuildSearchPageingSqlByCondition(condition, page, pageSize, orderField, ascending);
            var helper = new DbHelperSql(SetCurConnectionString());
            return helper.Query(sql.SqlString, GetParameter(sql.Paras));
        }
    }
}
