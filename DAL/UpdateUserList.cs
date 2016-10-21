using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    class UpdateUserList
    {
        //向客户端发送当前在线用户IP，更新客户端在线用户列表
        public void updateuserlist(Dictionary<IWebSocketConnection, String> allOnlineUsers, 
                                   List<IWebSocketConnection> allSockets)
        {
            try
            {
                //以 username1|username2|username3|...的方式返回在线用户名列表
                string list = "";
                foreach (var socket in allOnlineUsers.Values)
                {
                    list += allOnlineUsers.Values + "|";
                }
                allSockets.ToList().ForEach(s => s.Send(list));
            }
            catch { }  
        }
    }
}
