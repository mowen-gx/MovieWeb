using System;

namespace MovieWeb.DataUtil.ConnectionString
{
    public class ConnectionstringBase
    {
        /// <summary>
        /// 获取数据库连接字符串
        /// </summary>
        /// <returns></returns>
        public virtual string Get()
        {
            throw new NotImplementedException();
        }
    }
}
