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
        }//获得我的好友


        public static void updateuserlist(Dictionary<IWebSocketConnection, String> allOnlineUsers)//更新在线用户列表
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
                return;
        }

        //群发
        public static void sendMessageToAllUser(Dictionary<IWebSocketConnection, String> allOnlineUsers,string[] array)
        {
            string sendMsg; //获取要发送到客户端的文本
            foreach (KeyValuePair<IWebSocketConnection, String> kv in allOnlineUsers)
            {
                sendMsg = "talkToAll|" + array[1] + "|" + array[3];
                if (kv.Value != array[1])
                {
                    kv.Key.Send(sendMsg);
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
            return;
        }

        public static int Register(string username,string password)//注册
        {
            return SqlHelper.ExecuteNonQuery(
                "insert into T_Users(username,password) values(@username,@password)",
                new SqlParameter("@username", username),
                new SqlParameter("@password", password)
                );
        }

        public static DataTable checkRegister(string username)//检测用户名是否已被占用
        {
            return SqlHelper.ExecuteDataTable(
                "select id from T_Users where username =@username",
                new SqlParameter("@username", username)
                );
        }

        public static DataTable findFriend(string username)//从数据库中查找好友
        {
            return SqlHelper.ExecuteDataTable(
               "select username from T_Users where username like '%'+@SearchString+'%' order by id desc",
               new SqlParameter("@SearchString", username)
               );
        }

        public static void findMyFriend(Dictionary<IWebSocketConnection, String> allOnlineUsers, string[] mgs)//查找到好友并回传
        {
            DataTable tmF = findFriend(mgs[2]);
            string MF=mgs[0]+"|"+mgs[1];
            if (tmF.Rows.Count == 0)
            {
                MF += "|" + "null";
            }
            else
            {
                for (int i = 0; i < tmF.Rows.Count; i++)
                {
                    MF += "|" + tmF.Rows[i]["username"];
                }
            }
            foreach (KeyValuePair<IWebSocketConnection, String> kv in allOnlineUsers)
            {
                if (kv.Value == mgs[1])
                {
                    kv.Key.Send(MF);
                }
            }
            return;
        }

        public static DataTable chackIsFriendOrNot(string[] mgs)//检查是否已是好友
        {
            return SqlHelper.ExecuteDataTable(@"select * from T_Friend where (username=@username and friendname=@friendname) or 
                                             (username=@friendname and friendname=@username )",
                  new SqlParameter("@username",mgs[1]),
                  new SqlParameter("@friendname",mgs[2])
                  );
        }

        public static void addFriend(Dictionary<IWebSocketConnection, String> allOnlineUsers, string[] mgs)
        {
            int temp;
            string m=null;
            DataTable chack = chackIsFriendOrNot(mgs);
            if (mgs[1] == mgs[2])
            {
                m = mgs[0] + "|" + mgs[1] + "|" + mgs[2] + "|" + "0";
                foreach (KeyValuePair<IWebSocketConnection, String> kv in allOnlineUsers)
                {
                    if (kv.Value == mgs[1])
                    {
                        kv.Key.Send(m);
                    }
                }
                return;
            }
            else if (chack.Rows.Count == 0)
            {
                temp = SqlHelper.ExecuteNonQuery("insert into T_Friend(username,friendname) values(@username,@friendname)",
                new SqlParameter("@username", mgs[1]),
                new SqlParameter("@friendname", mgs[2])
                );
                if (temp == 1)
                {
                    m = mgs[0] + "|" + mgs[1]+"|"+mgs[2]+"|" + "1";
                    foreach (KeyValuePair<IWebSocketConnection, String> kv in allOnlineUsers)
                    {
                        if (kv.Value == mgs[1])
                        {
                            kv.Key.Send(m);
                        }
                    }
                }                
            }
            else
            {
                m = mgs[0] + "|" + mgs[1] + "|" + mgs[2] + "|" + "2";
                foreach (KeyValuePair<IWebSocketConnection, String> kv in allOnlineUsers)
                {
                    if (kv.Value == mgs[1])
                    {
                        kv.Key.Send(m);
                    }
                }
            }
            return;
        }//添加好友


    }
}
