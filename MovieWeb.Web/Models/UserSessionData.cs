using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieWeb.Web.Models
{
    public class UserSessionData
    {
        private static Dictionary<string, List<string>> _userRoles = new Dictionary<string, List<string>>();
        private static readonly object obj = new object();

        public static bool IsInRole(string userName, string role)
        {
            bool ret = false;
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(role))
            {
                if (_userRoles.ContainsKey(userName))
                {
                    if (_userRoles[userName].Contains(role))
                    {
                        ret = true;
                    }
                }
            }
            return ret;
        }

        
        public static void SetUserToRoleSession(string userName, string role)
        {
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(role))
            {
                lock (obj)
                {
                    if (_userRoles.ContainsKey(userName))
                    {
                        if (!_userRoles[userName].Contains(role))
                        {
                            _userRoles[userName].Add(role);
                        }
                    }
                    else
                    {
                        _userRoles.Add(userName, new List<string>() { role });
                    }
                }
            }
        }

    }
}