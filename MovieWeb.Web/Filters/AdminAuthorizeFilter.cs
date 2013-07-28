using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MovieWeb.Web.Models;

namespace MovieWeb.Web.Filters
{
    /// <summary>
    /// 权限验证属性。
    /// </summary>
    public class AuthorizeExAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 初始化权限验证类。
        /// </summary>
        /// <param name="permissionName">权限名称。</param>
        public AuthorizeExAttribute(string permissionName = "")
        {
            this.PermissionName = permissionName;
        }

        /// <summary>
        /// 获取权限名称。
        /// </summary>
        public string PermissionName { get; private set; }

        /// <summary>
        /// 验证授权。
        /// </summary>
        /// <param name="httpContext">HTTP 上下文，它封装有关单个 HTTP 请求的所有 HTTP 特定的信息。</param>
        /// <returns>如果用户已经过授权，则为 true；否则为 false。</returns>
        // 只需重载此方法，模拟自定义的角色授权机制  
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (UserSessionData.IsInRole(httpContext.User.Identity.Name, "admins"))
                return true;
            return false;
        }

        /// <summary>
        /// 重写验证。
        /// </summary>
        /// <param name="filterContext">验证信息上下文。</param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (filterContext.HttpContext.Response.StatusCode == 403)
            {
                if (filterContext.HttpContext.User.Identity.IsAuthenticated)
                    filterContext.Result = new RedirectResult("/AccessError");
                else
                    filterContext.Result =
                        new RedirectResult(FormsAuthentication.LoginUrl + "?returnUrl=" +filterContext.HttpContext.Request.UrlReferrer);
            }
        }
    }
}