using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class User
    {
        public string username { get; set; }
        public string password { get; set; }
         public User(string username) //构造函数
        {
            DataTable all_info = SqlHelper.ExecuteDataTable(
                "select * from T_Users where username=@username",
                new SqlParameter("@username", username)
                );
            if (all_info.Rows.Count == 0)
            {
                return;
            }
            this.username = all_info.Rows[0]["username"].ToString();
            this.password = all_info.Rows[0]["password"].ToString();
        }

        public int Login(string paw) //登陆
        {
            if (this == null)
            {
                return 0;
            }
            if (paw == this.password)
            {
                return 1;
            }
            return 2;
        }     
    }
}
