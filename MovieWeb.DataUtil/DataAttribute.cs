using System;

namespace MovieWeb.DataUtil
{
    /// <summary>
    /// Attribute基类
    /// 功能：描述数据库结构
    /// 作者：莫文
    /// 时间：2011-11-1
    /// </summary>
    public class DataBaseAttribute : Attribute
    {
        /// <summary>
        /// 名称
        /// </summary>
        private readonly string _name;
        public DataBaseAttribute(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
    }

    /// <summary>
    /// 数据库表
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : DataBaseAttribute
    {
        public TableAttribute(string name) : base(name)
        {
        }

        public TableAttribute()
            : base(string.Empty)
        {
        }
    }

    /// <summary>
    /// 数据库字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldAttribute : DataBaseAttribute
    {
        private readonly string _dbType = string.Empty;
        public FieldAttribute(string name) : base(name)
        {
        }

        public FieldAttribute(string name,string dbType)
            : base(name)
        {
            _dbType = dbType;
        }

        public FieldAttribute()
            : base("")
        {
        }

        /// <summary>
        /// 数据库字段类型
        /// </summary>
        public string DbType
        {
            get { return _dbType; }
        }
    }

    /// <summary>
    /// 非数据库字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NotFieldAttribute : DataBaseAttribute
    {
        public NotFieldAttribute(string name)
            : base(name)
        {
        }

        public NotFieldAttribute()
            : base("")
        {
        }
    }

    /// <summary>
    /// 数据库主键字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryFieldAttribute : FieldAttribute
    {
        public PrimaryFieldAttribute(string name)
            : base(name)
        {
        }

        public PrimaryFieldAttribute()
            : base("")
        {
        }
    }

    /// <summary>
    /// 数据库主键字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IdentificationPrimaryFieldAttribute : PrimaryFieldAttribute
    {
        public IdentificationPrimaryFieldAttribute(string name)
            : base(name)
        {
        }

        public IdentificationPrimaryFieldAttribute()
            : base("")
        {
        }
    }

    /// <summary>
    /// 数据库主键字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IdentificationFieldAttribute : FieldAttribute
    {
        public IdentificationFieldAttribute(string name)
            : base(name)
        {
        }

        public IdentificationFieldAttribute()
            : base("")
        {
        }
    }
}
