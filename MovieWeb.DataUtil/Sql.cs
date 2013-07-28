using System.Collections.Generic;
using System.Data.SqlClient;

namespace MovieWeb.DataUtil
{
    /// <summary>
    /// 功能：描述数据库SQL相关信息
    /// 作者：莫文
    /// 时间：2011-11-1
    /// </summary>
    public class Sql
    {
        /// <summary>
        /// SQL语句
        /// </summary>
        public string SqlString { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public List<Dictionary<Field,List<SqlParameter>>> Paras { get; set; }
    }
}
