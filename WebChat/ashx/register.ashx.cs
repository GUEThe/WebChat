using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace WebChat.ashx
{
    /// <summary>
    /// register 的摘要说明
    /// </summary>
    public class register : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string username = context.Request["username"];
            string password = context.Request["password"];
            DataTable c = Servers.checkRegister(username);
            if(c.Rows.Count==1){
                context.Response.Write("NO");
                return;
            }
            int r = Servers.Register(username, password);
            if (r == 1)
            {
                //context.Response.SetCookie(new HttpCookie("username", username));
                context.Response.Write("OK");
                return;
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