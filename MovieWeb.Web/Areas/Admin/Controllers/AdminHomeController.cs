using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieWeb.Web.Filters;

namespace MovieWeb.Web.Areas.Admin.Controllers
{
    public class AdminHomeController : Controller
    {
        //
        // GET: /Admin/
        [AuthorizeEx(Roles = "admins")]
        public ActionResult Index()
        {
            if (Session["UserName"] == null)
            {
                Response.Redirect("~/Login");
            }
            else
            {
                ViewBag.UserName = Session["UserName"].ToString().Trim();
            }
            return View();
        }

    }
}
