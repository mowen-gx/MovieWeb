using System;
using MovieWeb.DataUtil.ConnectionString;
using log4net;

namespace MovieWeb.DataUtil
{
    /// <summary>
    /// SCM只读数据库连接字符串
    /// </summary>
    public class MovieConn : ConnectionstringBase
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MovieConn));

        /// <summary>
        /// 读取数据库连接字符串
        /// </summary>
        /// <returns></returns>
        public override string Get()
        {
            try
            {

                return System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            }
            catch (Exception ex)
            {
                Logger.Error("读取电影数据库连接字符串失败,原因："+ex.Message + ex.StackTrace);
            }
            return "";
        }
    }
}
