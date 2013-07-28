using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieWeb.Model;

namespace MovieWeb.Data
{
    public class UserData
    {
        public bool Login(string userName, string pwd)
        {
            bool ret = false;
            List<AdminUser> users = DataUtil.DataHelper.Search(new AdminUser() { UserName = userName });
            if (users != null)
            {
                if (users.Count > 0)
                {
                    if (users.First().UserName.ToLower().Equals(userName) && users.First().Pwd.ToLower().Equals(pwd))
                    {
                        ret = true;
                    }
                }
            }
            return ret;
        }
    }
}
