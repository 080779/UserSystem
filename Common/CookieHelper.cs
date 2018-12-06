using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace IMS.Common
{
    public static class CookieHelper
    {
        /// <summary>
        /// 用户登陆
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="roles">权限</param>
        /// <param name="isPersistent">输入true时，Cookies存在的时间是一个月！输入false时，Cookies存在时间是一小时</param>
        public static void Login(string username, string roles, bool isPersistent)
        {

            DateTime dt = isPersistent ? DateTime.Now.AddDays(30) : DateTime.Now.AddMinutes(60);
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                                                                                1, // 票据版本号
                                                                                username, // 票据持有者
                                                                                DateTime.Now, //分配票据的时间
                                                                                dt, // 失效时间
                                                                                isPersistent, // 需要用户的 cookie 
                                                                                roles, // 用户数据，这里其实就是用户的角色
                                                                                FormsAuthentication.FormsCookiePath);//cookie有效路径


            //使用机器码machine key加密cookie，为了安全传送
            string hash = FormsAuthentication.Encrypt(ticket);
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, hash); //加密之后的cookie
            //将cookie的失效时间设置为和票据tikets的失效时间一致 
            if (ticket.IsPersistent)
            {
                cookie.Expires = ticket.Expiration;
            }
            //添加cookie到页面请求响应中
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static long GetLoginId()
        {
            if (HttpContext.Current.Request.Cookies["Platform_UserId"] != null)
            {
                return Convert.ToInt32(HttpContext.Current.Request.Cookies["Platform_UserId"]["Id"]);
            }
            else
            {
                return 0;
            }
        }

        public static string GetLoginMobile()
        {
            if (HttpContext.Current.Request.Cookies["Platform_UserId"] != null)
            {
                return HttpContext.Current.Request.Cookies["Platform_UserId"]["Mobile"];
            }
            else
            {
                return null;
            }
        }
    }
}
