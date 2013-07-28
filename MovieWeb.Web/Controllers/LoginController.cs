using System;
using System.Web.Mvc;
using System.Web.Security;
using MovieWeb.Data;
using MovieWeb.Web.Models;

namespace MovieWeb.Web.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpPost, ActionName("DoLogin")]
        public ActionResult Login(string username, string password)
        {
            UserData data = new UserData();
            if (data.Login(username, password))
            {
                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                    1,
                    username,
                    DateTime.Now,
                    DateTime.Now.AddMinutes(30),
                    false,
                    "admins"
                    );
                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                var authCookie = new System.Web.HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);
                UserSessionData.SetUserToRoleSession(username,"admins");
                ViewBag.Msg = "登录成功";
                Session["UserName"] = username.Trim();
                ViewBag.UserName = username.Trim();
            }
            else
            {
                ViewBag.Msg = "登录失败";
            }
            return View("Index");
        }

        public ActionResult Logout()
        {
            Session.Remove("UserName");
            return View();
        }
    }
}
