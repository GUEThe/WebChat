using Fleck;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Servers
    {
        public static DataTable GetMyFriend(string uname)
        {
            return SqlHelper.ExecuteDataTable(
                "select * from T_Friend where username=@username or friendname=@friendname",
                new SqlParameter("@username", uname),
                new SqlParameter("@friendname", uname)
                );
        }


        public static void updateuserlist(Dictionary<IWebSocketConnection, String> allOnlineUsers,
                                   List<IWebSocketConnection> allSockets)
        {
           
                //以 username1|username2|username3|...的方式返回在线用户名列表
                DataTable myfriends;
                
                foreach (KeyValuePair<IWebSocketConnection, String> kv in allOnlineUsers)
                {
                    string list = "olfriend";
                    myfriends = GetMyFriend(kv.Value);
                    for (int i = 0; i < myfriends.Rows.Count; i++)
                    {
                        if (myfriends.Rows[i]["username"].ToString()==kv.Value)
                            foreach (KeyValuePair<IWebSocketConnection, String> ky in allOnlineUsers)
                            {
                                if (myfriends.Rows[i]["friendname"].ToString() == ky.Value && myfriends.Rows[i]["friendname"].ToString() != kv.Value)
                                    list += "|" + myfriends.Rows[i]["friendname"];
                            }
                        if(myfriends.Rows[i]["friendname"].ToString()==kv.Value)
                            foreach (KeyValuePair<IWebSocketConnection, String> ky in allOnlineUsers)
                            {
                                if (myfriends.Rows[i]["username"].ToString() == ky.Value && myfriends.Rows[i]["username"].ToString() != kv.Value)
                                    list += "|" + myfriends.Rows[i]["username"];
                            }                         
                    }
                    kv.Key.Send(list);  
                }
                //allSockets.ToList().ForEach(s => s.Send(list));
            
        }

        //private static DataTable GetMyFriend(string a)
        //{
        //    return SqlHelper.ExecuteDataTable("select * from T_Friend where username=@username or friend=@username",
        //        new SqlParameter("@username",a)
        //        );
        //}

        //群发
        public static void sendMessageToAllUser(List<IWebSocketConnection> allSockets,
                                         Dictionary<IWebSocketConnection, String> allOnlineUsers,
                                         string[] array, string message)
        {
            foreach (string user in allOnlineUsers.Values)
            {
                if (array[1] == user)
                {
                    string username = user;
                    allSockets.ToList().ForEach(s => s.Send(username + ":" + message));//向所有在线用户群发消息
                }
            }
        }


        //向指定用户发送消息
        public static void SendSingleMsg(Dictionary<IWebSocketConnection, String> allOnlineUsers,string[] array)
        {
            string sendMsg; //获取要发送到客户端的文本
            foreach (KeyValuePair<IWebSocketConnection, String> kv in allOnlineUsers)
            {
                if (array[2] == kv.Value)//匹配用户名
                {
                    string username = array[1];
                    sendMsg ="betold|"+ username + "|" + array[3];//构建发送的消息
                    kv.Key.Send(sendMsg);
                }
            }
        }
    }
}
