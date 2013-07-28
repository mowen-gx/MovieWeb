using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace MovieWeb.DataUtil
{
    /// <summary>
    /// 类型解析器
    /// 功能：通过反射将一个类或者一个对象的数据库描述信息读取出来
    /// 注：读取出来之后缓存起来，以方便后边使用
    /// 作者：莫文
    /// 时间：2011-11-1
    /// </summary>
    public class TypeParser
    {
        #region Private field
        private Type _type;
        private static object objForLock = new object();
        private List<object> _objects = new List<object>();
        private static Dictionary<Type, DbDescription> _typeDbDescription = new Dictionary<Type, DbDescription>();

        #endregion

        #region 构造函数
        /// <summary>
        /// 类型解析器
        /// </summary>
        /// <param name="type"></param>
        public TypeParser(Type type)
        {
            _type = type;
        }

        /// <summary>
        /// 类型解析器
        /// </summary>
        /// <param name="type"></param>
        /// <param name="objs"></param>
        public TypeParser(Type type, params object[] objs)
        {
            _type = type;
            foreach (object o in objs)
            {
                _objects.Add(o);
            }
        }

        #endregion

        #region 属性
        /// <summary>
        /// 解析结果
        /// </summary>
        public DbDescription Result
        {
            get
            {
                if (!_typeDbDescription.ContainsKey(_type))
                {
                    lock (objForLock)
                    {
                        ParseFields();
                        ParseTableName();
                    }
                }
                if (_objects != null)
                    _typeDbDescription[_type].Paras = ParseValue();
                return _typeDbDescription[_type];
            }
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 获取字段
        /// </summary>
        /// <returns></returns>
        private void ParseFields()
        {
            if (!_typeDbDescription.ContainsKey(_type))
            {
                _typeDbDescription.Add(_type, new DbDescription());
            }

            _typeDbDescription[_type].Fields = _typeDbDescription[_type].Fields ?? new Dictionary<string, Field>();
            //获取属性
            PropertyInfo[] myproperties = _type.GetProperties();
            foreach (PropertyInfo info in myproperties)
            {
                //获取属性Attribute
                object[] attrs = info.GetCustomAttributes(true);
                if (attrs.Count() == 0) //没有Attrbute，说明是普通字段
                {
                    //非主键
                    var pf = new Field
                    {
                        Name = info.Name,
                        DbType = FieldHelper.GetFieldType(info, null),
                        Type = FieldHelper.GetCSharpType(info, null),
                        IsPrimary = false,
                        Property = info
                    };

                    if (!_typeDbDescription[_type].Fields.ContainsKey(info.Name))
                        _typeDbDescription[_type].Fields.Add(info.Name, pf);
                    continue;
                }

                //没有对应的数据库字段，返回
                if (!attrs.Select(attr => (attr as NotFieldAttribute)).All(pfr => pfr == null))
                    continue;
                foreach (object o in attrs)
                {
                    bool isIdentification = false;
                    var ifr = (o as IdentificationFieldAttribute);
                    if (ifr != null)
                    {
                        isIdentification = true;
                    }

                    //是否为主键
                    var ipfr = (o as IdentificationPrimaryFieldAttribute);
                    if (ipfr != null)
                    {
                        var f = new Field
                        {
                            Name = info.Name,
                            DbType = FieldHelper.GetFieldType(info, ipfr),
                            Type = FieldHelper.GetCSharpType(info, ipfr),
                            IsPrimary = true,
                            IsIdentification = true,
                            Property = info
                        };

                        _typeDbDescription[_type].PrimaryField = info.Name;

                        if (!_typeDbDescription[_type].Fields.ContainsKey(info.Name))
                            _typeDbDescription[_type].Fields.Add(info.Name, f);
                        break;
                    }


                    //是否为主键
                    var pfr = (o as PrimaryFieldAttribute);
                    if (pfr != null)
                    {
                        var f = new Field
                                      {
                                          Name = info.Name,
                                          DbType = FieldHelper.GetFieldType(info, pfr),
                                          Type = FieldHelper.GetCSharpType(info, pfr),
                                          IsPrimary = true,
                                          IsIdentification = isIdentification,
                                          Property = info
                                      };

                        _typeDbDescription[_type].PrimaryField = info.Name;

                        if (!_typeDbDescription[_type].Fields.ContainsKey(info.Name))
                            _typeDbDescription[_type].Fields.Add(info.Name, f);
                        break;
                    }

                    //非主键
                    var fr = (o as FieldAttribute);
                    if (fr != null)
                    {
                        var pf = new Field
                                     {
                                         Name = info.Name,
                                         DbType = FieldHelper.GetFieldType(info, fr),
                                         Type = FieldHelper.GetCSharpType(info, fr),
                                         IsPrimary = false,
                                         IsIdentification = isIdentification,
                                         Property = info
                                     };

                        if (!_typeDbDescription[_type].Fields.ContainsKey(info.Name))
                            _typeDbDescription[_type].Fields.Add(info.Name, pf);
                        break;
                    }
                }

                var pfWithOtherAttr = new Field
                {
                    Name = info.Name,
                    DbType = FieldHelper.GetFieldType(info, null),
                    Type = FieldHelper.GetCSharpType(info, null),
                    IsPrimary = false,
                    IsIdentification = false,
                    Property = info
                };

                if (!_typeDbDescription[_type].Fields.ContainsKey(info.Name))
                    _typeDbDescription[_type].Fields.Add(info.Name, pfWithOtherAttr);

            }
        }

        /// <summary>
        /// 解析字段值时，请先解析字段，也就是ParseFields
        /// </summary>
        /// <returns></returns>
        private List<Dictionary<Field, List<SqlParameter>>> ParseValue()
        {
            List<Dictionary<Field, List<SqlParameter>>> paras = new List<Dictionary<Field, List<SqlParameter>>>();
            if (_objects == null)
            {
                return paras;
            }
            Dictionary<string, Field> fields = _typeDbDescription[_type].Fields;
            //获取属性
            PropertyInfo[] myproperties = _type.GetProperties();
            int index = 0;
            foreach (object o in _objects)
            {
                index += 1;
                Dictionary<Field, List<SqlParameter>> paradict = new Dictionary<Field, List<SqlParameter>>();
                foreach (PropertyInfo info in myproperties)
                {
                    object value = FieldHelper.GetFieldValue(info, o);
                    if (IsHaveValue(info, value))
                    {
                        if (fields[info.Name] != null)
                        {
                            if (!paradict.ContainsKey(fields[info.Name]))
                                paradict.Add(fields[info.Name], new List<SqlParameter>());
                            paradict[fields[info.Name]].Add(FieldHelper.GetFieldSqlPara(info, fields[info.Name].Type, o, index,
                                                                                        (paradict[fields[info.Name]].Count + 1)));
                        }
                    }
                }
                paras.Add(paradict);
            }
            return paras;
        }

        /// <summary>
        /// 判断字段值是否合法
        /// </summary>
        /// <param name="info">属性</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        private bool IsHaveValue(PropertyInfo info,object value)
        {
            Dictionary<string, Field> fields = _typeDbDescription[_type].Fields;
            if(fields.ContainsKey(info.Name) && value != null)
            {
                Field field = fields[info.Name];
                if(!field.IsPrimary) //非主键
                {
                    //若为时间类型，则需要进行检查，以免到数据库中不支持C#的时间范围
                    if (field.DbType == FieldType.DateTime)
                    {
                        DateTime time;
                        if (!DateTime.TryParse(value.ToString(), out time))
                        {
                            return false;
                        }
                        if (time == DateTime.MinValue || time == DateTime.MaxValue)
                        {
                            return false;
                        }
                    }
                    return true;
                }

                //主键，且值为0，则不认为有值 
                if (value.ToString() == "0" && field.DbType == FieldType.Int)
                    return false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        /// <returns></returns>
        private void ParseTableName()
        {
            _typeDbDescription[_type] = _typeDbDescription[_type] ?? new DbDescription();
            _typeDbDescription[_type].TableName = _type.Name;
            //获取Attribute
            object[] attrs = _type.GetCustomAttributes(true);
            foreach (var tr in attrs.Select(o => (o as TableAttribute)).Where(tr => tr != null))
            {
                _typeDbDescription[_type].TableName = !string.IsNullOrEmpty(tr.Name) ? tr.Name : _type.Name;
            }
        }
        #endregion
    }

    /// <summary>
    /// 数据库描述，字段表，查询参数、主键
    /// </summary>
    public class DbDescription
    {
        private string _primaryField = "";
        private Dictionary<string, Field> _fields = new Dictionary<string, Field>();
        private List<Dictionary<Field, List<SqlParameter>>> _paras = new List<Dictionary<Field, List<SqlParameter>>>();
        private string _tableName = string.Empty;

        /// <summary>
        /// 字段 
        /// </summary>
        public Dictionary<string, Field> Fields
        {
            get { return _fields; }
            set { _fields = value; }
        }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        /// <summary>
        /// 主键
        /// </summary>
        public string PrimaryField
        {
            get { return _primaryField; }
            set { _primaryField = value; }
        }

        /// <summary>
        /// 主键
        /// </summary>
        public Field Primary
        {
            get
            {
                return Fields[PrimaryField];
            }
        }

        /// <summary>
        /// SQL参数
        /// </summary>
        public List<Dictionary<Field, List<SqlParameter>>> Paras
        {
            get { return _paras; }
            set { _paras = value; }
        }
    }
}
