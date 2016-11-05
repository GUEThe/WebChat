using Fleck;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

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
           
              
                DataTable myfriends;
                
                foreach (KeyValuePair<IWebSocketConnection, String> kv in allOnlineUsers)
                {
                    Mge mge = new Mge();
                    mge.action = "olfriend";
                    myfriends = GetMyFriend(kv.Value);
                    mge.arrolfriend=new string[myfriends.Rows.Count];
                    mge.arrallmyfriend = new string[myfriends.Rows.Count];
                    for (int i = 0; i < myfriends.Rows.Count; i++)
                    {
                        if (myfriends.Rows[i]["username"].ToString() == kv.Value)
                        {
                            mge.arrallmyfriend[i] = myfriends.Rows[i]["friendname"].ToString();
                        }
                        if (myfriends.Rows[i]["friendname"].ToString() == kv.Value)
                        {
                            mge.arrallmyfriend[i] = myfriends.Rows[i]["username"].ToString();
                        }
                        if (myfriends.Rows[i]["username"].ToString()==kv.Value)
                            foreach (KeyValuePair<IWebSocketConnection, String> ky in allOnlineUsers)
                            {
                                if (myfriends.Rows[i]["friendname"].ToString() == ky.Value && myfriends.Rows[i]["friendname"].ToString() != kv.Value)
                                {
                                    mge.arrolfriend[i] = myfriends.Rows[i]["friendname"].ToString();
                                }
                            }
                        if(myfriends.Rows[i]["friendname"].ToString()==kv.Value)
                            foreach (KeyValuePair<IWebSocketConnection, String> ky in allOnlineUsers)
                            {
                                if (myfriends.Rows[i]["username"].ToString() == ky.Value && myfriends.Rows[i]["username"].ToString() != kv.Value)
                                {
                                    mge.arrolfriend[i] = myfriends.Rows[i]["username"].ToString();
                                }
                            }                         
                    }
                    string json = new JavaScriptSerializer().Serialize(mge);
                    kv.Key.Send(json);  
                }
                //allSockets.ToList().ForEach(s => s.Send(list));
                return;
        }

        //群发
        public static void sendMessageToAllUser(Dictionary<IWebSocketConnection, String> allOnlineUsers, Mge mge)
        {
            foreach (KeyValuePair<IWebSocketConnection, String> kv in allOnlineUsers)
            {
                mge.action = "talkToAll";
                mge.chatcontext = mge.chatcontext;
                string json=new JavaScriptSerializer().Serialize(mge);
                if (kv.Value != mge.username)
                {
                    kv.Key.Send(json);
                }               
            }
        }


        //向指定用户发送消息
        public static void SendSingleMsg(Dictionary<IWebSocketConnection, String> allOnlineUsers, Mge mge)
        {
            string[] t_array = new string[4];
            t_array[1] =mge.chatwith;
            t_array[2] = mge.username;
            t_array[3] = mge.chatlog;
            saveChatLog(t_array);
            t_array[1] = mge.username;
            t_array[2] = mge.chatwith;
            t_array[3] = mge.chatcontext;
            saveChatLog(t_array);
            foreach (KeyValuePair<IWebSocketConnection, String> kv in allOnlineUsers)
            {
                if (mge.chatwith == kv.Value)//匹配用户名
                {
                    Mge sendMsg = new Mge();
                    sendMsg.action = "betold";
                    sendMsg.friendname = mge.username;
                    sendMsg.chatcontext = mge.chatcontext;
                    string json = new JavaScriptSerializer().Serialize(sendMsg);//构建发送的消息
                    kv.Key.Send(json);
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

        public static void findMyFriend(Dictionary<IWebSocketConnection, String> allOnlineUsers, Mge mge)//查找到好友并回传
        {
            DataTable tmF = findFriend(mge.friendname);
            Mge sendMsg = new Mge();
            sendMsg.action = "findFriend";
            sendMsg.username = mge.username;
            sendMsg.friendname = mge.friendname;
            if (tmF.Rows.Count == 0)
            {
                sendMsg.arrfriend = new string[1];
                sendMsg.arrfriend[0]="";
            }
            else
            {
                sendMsg.arrfriend = new string[tmF.Rows.Count];
                for (int i = 0; i < tmF.Rows.Count; i++)
                {
                    sendMsg.arrfriend[i] = tmF.Rows[i]["username"].ToString();
                }
            }
            foreach (KeyValuePair<IWebSocketConnection, String> kv in allOnlineUsers)
            {
                if (kv.Value == mge.username)
                {
                    string json = new JavaScriptSerializer().Serialize(sendMsg);//构建发送的消息
                    kv.Key.Send(json);
                }
            }
            return;
        }

        public static DataTable checkIsFriendOrNot(Mge mge)//检查是否已是好友
        {
            return SqlHelper.ExecuteDataTable(@"select * from T_Friend where (username=@username and friendname=@friendname) or 
                                             (username=@friendname and friendname=@username )",
                  new SqlParameter("@username",mge.username),
                  new SqlParameter("@friendname",mge.friendname)
                  );
        }

        public static void addFriend(Dictionary<IWebSocketConnection, String> allOnlineUsers, Mge mgs)
        {
            int temp;
            Mge sendMsg=new Mge();
            sendMsg.action = "addFriend";
            sendMsg.username = mgs.username;
            sendMsg.friendname = mgs.friendname;
            DataTable chack = checkIsFriendOrNot(mgs);
            if (mgs.username == mgs.friendname)
            {
                sendMsg.addfriend = 0;

                foreach (KeyValuePair<IWebSocketConnection, String> kv in allOnlineUsers)
                {
                    if (kv.Value == mgs.username)
                    {
                        string json = new JavaScriptSerializer().Serialize(sendMsg);//构建发送的消息
                        kv.Key.Send(json);
                    }
                }
                return;
            }
            else if (chack.Rows.Count == 0)
            {
                temp = SqlHelper.ExecuteNonQuery("insert into T_Friend(username,friendname) values(@username,@friendname)",
                new SqlParameter("@username", mgs.username),
                new SqlParameter("@friendname", mgs.friendname)
                );
                if (temp == 1)
                {
                    sendMsg.addfriend = 1;
                    foreach (KeyValuePair<IWebSocketConnection, String> kv in allOnlineUsers)
                    {
                        if (kv.Value == mgs.username)
                        {
                            string json = new JavaScriptSerializer().Serialize(sendMsg);//构建发送的消息
                            kv.Key.Send(json);
                        }
                    }
                }                
            }
            else
            {
                sendMsg.addfriend = 2;
                foreach (KeyValuePair<IWebSocketConnection, String> kv in allOnlineUsers)
                {
                    if (kv.Value == mgs.username)
                    {
                        string json = new JavaScriptSerializer().Serialize(sendMsg);//构建发送的消息
                        kv.Key.Send(json);
                    }
                }
            }
            return;
        }//添加好友

        public static DataTable checkExistChatLogOrNot(string[] array)
        {
            return SqlHelper.ExecuteDataTable(
                "select * from T_Chatlog where owner=@owner and chatwith=@chatwith",
                new SqlParameter("@owner", array[2]),
                new SqlParameter("@chatwith", array[1])
                );
        }//检测数据库中是否已含有聊天记录
        public static void saveChatLog(string[] array)
        {
            DataTable c = checkExistChatLogOrNot(array);
            if (c.Rows.Count == 1){
                SqlHelper.ExecuteNonQuery(
                    "update T_Chatlog set chatlog=chatlog+@chatlog,[read]=0 where owner=@owner and chatwith=@chatwith",
                    new SqlParameter("@chatlog", array[3]),
                    new SqlParameter("@owner", array[2]),
                    new SqlParameter("@chatwith", array[1])
                    );
            }
            else if(c.Rows.Count==0)
            {
                SqlHelper.ExecuteNonQuery(
                "insert into T_Chatlog(owner,chatwith,chatlog) values(@owner,@chatwith,@chatlog)",
                new SqlParameter("@owner", array[2]),
                new SqlParameter("@chatwith", array[1]),
                new SqlParameter("@chatlog", array[3])
                );
            }
            return;
        }//保存聊天记录

        public static void getChatLog(Dictionary<IWebSocketConnection, String> allOnlineUsers, Mge mgs)
        {
            Mge sendMsg = new Mge();
            sendMsg.action = "getChatlog";
            sendMsg.username = mgs.username;
            sendMsg.chatwith = mgs.chatwith;
            if (mgs.chatwith.IndexOf("公共聊天室")>=0)
            {
                return;
            }
            DataTable Chatlog = SqlHelper.ExecuteDataTable("select * from T_Chatlog where owner=@owner and chatwith=@chatwith",
                new SqlParameter("@owner", mgs.username),
                new SqlParameter("@chatwith", mgs.chatwith)
                );
            if (Chatlog.Rows.Count == 1)
            {
                sendMsg.chatlog = Chatlog.Rows[0]["chatlog"].ToString();

                SqlHelper.ExecuteNonQuery("update T_Chatlog set [read]=1 where owner=@owner and chatwith=@chatwith",
                    new SqlParameter("@owner", mgs.username),
                    new SqlParameter("@chatwith", mgs.chatwith)
                    );
            }
            else if (Chatlog.Rows.Count == 0)
            {
                sendMsg.chatlog = "";
            }
            foreach (KeyValuePair<IWebSocketConnection, String> kv in allOnlineUsers)
            {
                if (kv.Value == mgs.username)
                {
                    string json = new JavaScriptSerializer().Serialize(sendMsg);//构建发送的消息
                    kv.Key.Send(json);
                }
            }
        }//聊天记录获取

        public static int createDiscussionGroup(Mge msg)
        {
            string id_str = SqlHelper.ExecuteScalar(
                @"insert into T_Discussiongroups(discussion_group_name) 
                 vlaue(@discussion_group_name)
                 select @@identity",
                new SqlParameter("@discussion_group_name",msg.discussion_group_name)).ToString();
            int id = int.Parse(id_str);
            for (int i = 0; i < msg.discussion_groups_member.Length; i++)
            {
                SqlHelper.ExecuteNonQuery(
                    @"insert into T_discussion_groups_member(discussion_group_id,username)
                      values(@discussion_group_id,@username)",
                    new SqlParameter("@discussion_group_id", id),
                    new SqlParameter("@username", msg.discussion_groups_member[i])
                    );
            }
            return id;
        }//创建讨论组

    }
}
