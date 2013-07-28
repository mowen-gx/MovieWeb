using System.Linq;

namespace MovieWeb.DataUtil
{
    public class SqlFilter
    {
        /// <summary>
        /// 是否包含了非法字符
        /// </summary>
        /// <param name="inText">要过滤的字符串 </param>
        /// <returns>如果参数存在不安全字符，则返回true </returns>
        public static bool Isvalid(string inText)
        {
            const string word = "and|exec|insert|select|delete|update|chr|mid|master|or|truncate|char|declare|join|cmd|master|drop|create|exists|asc(|while|xp_cmdshell|add|ch|ch(|waitfor|sleep";
            if (inText == null)
                return false;
            return word.Split('|').Any(i => (inText.ToLower().IndexOf(i + " ") > -1) || (inText.ToLower().IndexOf(" " + i) > -1));
        }

        /// <summary>
        ///SQL注入过滤
        /// </summary>
        /// <param name="inText">要过滤的字符串 </param>
        /// <returns>如果参数存在不安全字符，则返回true </returns>
        public static string Filt(string inText)
        {
            const string word = "and|exec|insert|select|delete|update|chr|mid|master|or|truncate|char|declare|join|cmd|master|drop|create|exists|asc(|while|xp_cmdshell|add|ch|ch(|waitfor|sleep";
            if (inText == null)
                return string.Empty;
            foreach (string i in word.Split('|'))
            {
                if ((inText.ToLower().IndexOf(i + " ") > -1) || (inText.ToLower().IndexOf(" " + i) > -1))
                {
                    inText = inText.ToLower().Replace(i + " ", " ").Replace(" "+ i , " ");
                }
            }
            return inText;
        }
    }
}
