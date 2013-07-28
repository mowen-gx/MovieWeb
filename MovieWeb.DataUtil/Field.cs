using System.Reflection;

namespace MovieWeb.DataUtil
{
    /// <summary>
    /// 字段
    /// 功能：描述数据库字段
    /// 作者：莫文
    /// 时间：2011-11-1
    /// </summary>
    public class Field
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 字段数据库类型
        /// </summary>
        public string DbType { get; set; }

        /// <summary>
        /// 字段是否为主键
        /// </summary>
        public bool IsPrimary { get; set; }

        private bool _isIdentification = false;
        /// <summary>
        /// 字段是否为标识字段
        /// </summary>
        public bool IsIdentification
        {
            get { return _isIdentification; }
            set { _isIdentification = value; }
    }

        /// <summary>
        /// 字段值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 字段是否有值
        /// </summary>
        public bool HasValue { get; set; }

        /// <summary>
        /// 字段所属的属性
        /// </summary>
        public PropertyInfo Property { get; set; }
    }
}
