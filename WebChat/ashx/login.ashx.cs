using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebChat.ashx
{
    /// <summary>
    /// login 的摘要说明
    /// </summary>
    public class login : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string username = context.Request["username"];
            string password = context.Request["password"];
            User user = new User(username);
            if (user.Login(password) == 1)
            {
                context.Response.SetCookie(new HttpCookie("username", user.username));
                context.Response.Write("OK");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}