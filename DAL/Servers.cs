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
                "select * from T_Friend where username=@username",
                new SqlParameter("@username", uname)
                );
        }//获得我的好友

        public static DataTable getMyDiscussionGroup(string username)
        {
            return SqlHelper.ExecuteDataTable(
                "select discussion_group_name from V_discussion_groups where username=@username",
                new SqlParameter("@username", username)
                );
        }//获得我的讨论组

        //public static void getAllMyFriend(Dictionary<IWebSocketConnection, String> allOnlineUsers, Mge mge)
        //{
        //    DataTable AllMyFriend = GetMyFriend(mge.username);
        //    Mge sendMsg = new Mge();
        //    sendMsg.action = mge.action;
        //    sendMsg.username = mge.username;
        //    sendMsg.arrallmyfriend = new string[AllMyFriend.Rows.Count];
        //    for (int i = 0; i < AllMyFriend.Rows.Count; i++)
        //    {
        //        if (AllMyFriend.Rows[i]["username"].ToString() == mge.username)
        //        {
        //            sendMsg.arrallmyfriend[i] = AllMyFriend.Rows[i]["friendname"].ToString();
        //        }
        //        if (AllMyFriend.Rows[i]["friendname"].ToString() == mge.username)
        //        {
        //            sendMsg.arrallmyfriend[i] = AllMyFriend.Rows[i]["username"].ToString();
        //        }
        //    }
        //    foreach (KeyValuePair<IWebSocketConnection, String> kv in allOnlineUsers)
        //    {
        //        if (kv.Value == mge.username)
        //        {
        //            string json = new JavaScriptSerializer().Serialize(sendMsg);
        //            kv.Key.Send(json);
        //        }
        //    }
        //    return;
        //}

        public static DataTable getDGmember(string DGname)
        {
            return SqlHelper.ExecuteDataTable(
                "select * from V_discussion_groups where discussion_group_name=@discussion_group_name",
                new SqlParameter("@discussion_group_name", DGname)
                );
        }//获得讨论组成员

        public static void updateUserList(Dictionary<IWebSocketConnection, String> allOnlineUsers)//更新在线用户列表
        {
           
              
                DataTable myfriends;
                DataTable mydiscussiongroups;
                foreach (KeyValuePair<IWebSocketConnection, String> kv in allOnlineUsers)
                {
                    Mge mge = new Mge();
                    mge.action = "olfriend";
                    myfriends = GetMyFriend(kv.Value);
                    mydiscussiongroups = getMyDiscussionGroup(kv.Value);
                    mge.arrolfriend=new string[myfriends.Rows.Count];
                    mge.arrallmyfriend = new string[myfriends.Rows.Count];
                    mge.mydiscussiongroups = new string[mydiscussiongroups.Rows.Count];
                    for (int i = 0; i < mydiscussiongroups.Rows.Count; i++)
                    {
                        mge.mydiscussiongroups[i] = mydiscussiongroups.Rows[i]["discussion_group_name"].ToString();
                    }
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
                        if (myfriends.Rows[i]["username"].ToString() == kv.Value)
                            foreach (KeyValuePair<IWebSocketConnection, String> ky in allOnlineUsers)
                            {
                                if (myfriends.Rows[i]["friendname"].ToString() == ky.Value && myfriends.Rows[i]["friendname"].ToString() != kv.Value)
                                {
                                    mge.arrolfriend[i] = myfriends.Rows[i]["friendname"].ToString();
                                }
                            }
                        if (myfriends.Rows[i]["friendname"].ToString() == kv.Value)
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
        }//公共聊天室


        public static void sendMsgToMyFriend(Dictionary<IWebSocketConnection, String> allOnlineUsers, Mge mge)
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
        }//跟特定好友聊天

        public static void sendMsgToMyDG(Dictionary<IWebSocketConnection, String> allOnlineUsers, Mge mge)
        {
            DataTable DGmember = getDGmember(mge.chatwith);
            for (int i = 0; i < DGmember.Rows.Count; i++)
            {
                string[] t_array = new string[4];
                t_array[1] = mge.chatwith;
                if (mge.username == DGmember.Rows[i]["username"].ToString())
                {
                    t_array[2] = mge.username;
                    t_array[3] = mge.chatlog;
                }
                else
                {
                    t_array[2] = DGmember.Rows[i]["username"].ToString();
                    t_array[3] = mge.chatcontext;
                }
                saveChatLog(t_array);
                foreach (KeyValuePair<IWebSocketConnection, String> kv in allOnlineUsers)
                {
                    if (DGmember.Rows[i]["username"].ToString() == kv.Value && kv.Value!=mge.username)//匹配用户名
                    {
                        Mge sendMsg = new Mge();
                        sendMsg.action = "betold";
                        sendMsg.friendname = mge.chatwith;
                        sendMsg.chatcontext = mge.chatcontext;
                        string json = new JavaScriptSerializer().Serialize(sendMsg);//构建发送的消息
                        kv.Key.Send(json);
                    }
                }
            }
                return;
        }//讨论组聊天

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

        public static int insertAddFriend(Mge mgs)
        {
            return SqlHelper.ExecuteNonQuery("insert into T_Friend(username,friendname) values(@username,@friendname);insert into T_Friend(username,friendname) values(@friendname,@username)",
                new SqlParameter("@username", mgs.username),
                new SqlParameter("@friendname", mgs.friendname)
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
                temp = insertAddFriend(mgs);
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

        public static void delChatLog(Dictionary<IWebSocketConnection, String> allOnlineUsers, Mge mgs)
        {
            int del_id=SqlHelper.ExecuteNonQuery("Delete from T_Chatlog where owner=@owner and chatwith=@chatwith",
                new SqlParameter("@owner", mgs.username),
                new SqlParameter("@chatwith", mgs.chatwith)
                );
        }//删除聊天记录

        public static void createDiscussionGroup(Dictionary<IWebSocketConnection, String> allOnlineUsers,Mge msg)
        {
            string id_str = SqlHelper.ExecuteScalar(
                @"insert into T_Discussiongroups(discussion_group_name) 
                 values(@discussion_group_name)
                 select @@identity",
                new SqlParameter("@discussion_group_name",msg.discussion_group_name)).ToString();
            int id = int.Parse(id_str);
            for (int i = 0; i < msg.discussion_group_members.Length; i++)
            {
                SqlHelper.ExecuteNonQuery(
                    @"insert into T_discussion_groups_member(discussion_group_id,username)
                      values(@discussion_group_id,@username)",
                    new SqlParameter("@discussion_group_id", id),
                    new SqlParameter("@username", msg.discussion_group_members[i])
                    );
            }
            msg.addfriend = 1;
            foreach (KeyValuePair<IWebSocketConnection, String> kv in allOnlineUsers)
            {
                if (kv.Value == msg.username)
                {
                    string json = new JavaScriptSerializer().Serialize(msg);//构建发送的消息
                    kv.Key.Send(json);
                }
            }
            return;
        }//创建讨论组

        public static void getDGmember(Dictionary<IWebSocketConnection, String> allOnlineUsers, Mge mge)//查找到好友并回传
        {
            DataTable tmF = getDGmember(mge.friendname);
            Mge sendMsg = new Mge();
            sendMsg.action = "findFriend";
            sendMsg.username = mge.username;
            sendMsg.friendname = mge.friendname;
            if (tmF.Rows.Count == 0)
            {
                sendMsg.arrfriend = new string[1];
                sendMsg.arrfriend[0] = "";
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

        public static int insertMYmc(Mge mgs)
        {
            DateTime publishtime = DateTime.Parse(mgs.publishtime);
            return SqlHelper.ExecuteNonQuery("insert into T_MYmc(username,[context],publish_time) values(@username,@context,@publish_time)",
                new SqlParameter("@username", mgs.username),
                new SqlParameter("@context", mgs.chatcontext),
                new SqlParameter("@publish_time", publishtime)
                );
        }//发表朋友圈

        public static DataTable getMYmc(Mge mgs)
        {
            int page_n = (mgs.page_n - 1) * 3;
            return SqlHelper.ExecuteDataTable(@"select top 3 * from T_Mymc where 
              (username=@username or username in (select friendname from T_Friend where username=@username)) 
              and id not in
              (select top "+page_n.ToString()+@"id from T_Mymc where username=@username or username in 
              (select friendname from T_Friend where username=@username) order by publish_time desc)order by publish_time desc",
              new SqlParameter("@username", mgs.username));
        }//从数据库中获取朋友圈动态

        public static int getMYmcsCount(Mge mgs)
        {
            return int.Parse(SqlHelper.ExecuteScalar("select count(*) from T_Mymc where username=@username or username in (select friendname from T_Friend where username=@username ) ",
                new SqlParameter("@username", mgs.username)).ToString());
        }//获取朋友圈所有好友动态条数

        public static DataTable getSupports(int id)
        {
            return SqlHelper.ExecuteDataTable("select * from T_Support where mymc_id=@mymc_id",
                new SqlParameter("@mymc_id", id));
        }//从数据库中获取赞
        public static DataTable getComments(int id)
        {
            return SqlHelper.ExecuteDataTable("select * from T_Comment where mymc_id=@mymc_id",
                new SqlParameter("@mymc_id", id));
        }//从数据库中获取评论
        public static void getMYmc(Dictionary<IWebSocketConnection, String> allOnlineUsers, Mge mgs)
        {
            Mge sendMgs=new Mge();
            DataTable mymctable=getMYmc(mgs);
            sendMgs.action = mgs.action;
            sendMgs.username = mgs.username;

            sendMgs.MYmcs = new MYmc[mymctable.Rows.Count];
            for (int i = 0; i < mymctable.Rows.Count; i++)
            {
                sendMgs.MYmcs[i] = new MYmc();
                sendMgs.MYmcs[i].id = int.Parse(mymctable.Rows[i]["id"].ToString());
                sendMgs.MYmcs[i].username = mymctable.Rows[i]["username"].ToString();
                sendMgs.MYmcs[i].context = mymctable.Rows[i]["context"].ToString();
                sendMgs.MYmcs[i].ptime = Convert.ToDateTime(mymctable.Rows[i]["publish_time"]).ToString("yyy-MM-dd HH:mm");

                DataTable supports=getSupports(sendMgs.MYmcs[i].id);
                if(supports.Rows.Count>0)
                    sendMgs.MYmcs[i].supports = new string[supports.Rows.Count];



                for (int j = 0; j < supports.Rows.Count; j++)
                {
                    sendMgs.MYmcs[i].supports[j] = supports.Rows[j]["username"].ToString();
                }


                DataTable comments = getComments(sendMgs.MYmcs[i].id);
                if(comments.Rows.Count>=0)
                    sendMgs.MYmcs[i].comments = new comment[comments.Rows.Count];


                for (int j = 0; j < comments.Rows.Count; j++)
                {
                    if (comments.Rows.Count > 0)
                    {
                        sendMgs.MYmcs[i].comments[j] = new comment();
                        sendMgs.MYmcs[i].comments[j].username = comments.Rows[j]["username"].ToString();
                        sendMgs.MYmcs[i].comments[j].context = comments.Rows[j]["context"].ToString();
                        sendMgs.MYmcs[i].comments[j].mymcid = int.Parse(comments.Rows[j]["mymc_id"].ToString());
                    }
                }
            }
            sendMgs.page_n = mgs.page_n;
            int c = getMYmcsCount(mgs);
            sendMgs.mymcid = (getMYmcsCount(mgs) + 3 - 1) / 3;
            foreach (KeyValuePair<IWebSocketConnection, String> kv in allOnlineUsers)
            {
                if (kv.Value == mgs.username)
                {
                    string json = new JavaScriptSerializer().Serialize(sendMgs);//构建发送的消息
                    kv.Key.Send(json);
                }
            }
            return;
        }//获取朋友圈动态

        public static int publishComment(Mge mgs)
        {
            DateTime publishtime = DateTime.Parse(mgs.publishtime);
            return SqlHelper.ExecuteNonQuery("insert into T_Comment(mymc_id,username,[context],time,[read]) values(@mymc_id,@username,@context,@time,'1')",
                new SqlParameter("@mymc_id", mgs.mymcid),
                new SqlParameter("@username", mgs.username),
                new SqlParameter("@context", mgs.chatcontext),
                new SqlParameter("@time", publishtime)
                );
        }//将评论插入数据库

        public static void publishComment(Dictionary<IWebSocketConnection, String> allOnlineUsers, Mge mgs)
        {
            int c = publishComment(mgs);
            if (c == 1)
            {
                Mge sendMgs = new Mge();
                sendMgs.username = mgs.username;
                sendMgs.action = mgs.action;
                sendMgs.addfriend = 1;
                DataTable comments = getComments(mgs.mymcid);
                if (comments.Rows.Count > 0)
                {
                    sendMgs.MYmcs = new MYmc[1];
                    sendMgs.MYmcs[0] = new MYmc();
                    sendMgs.MYmcs[0].comments = new comment[comments.Rows.Count];
                    for (int i = 0; i < comments.Rows.Count; i++)
                    {
                        sendMgs.MYmcs[0].comments[i] = new comment();
                        sendMgs.MYmcs[0].comments[i].username = comments.Rows[i]["username"].ToString();
                        sendMgs.MYmcs[0].comments[i].context = comments.Rows[i]["context"].ToString();
                        //sendMgs.MYmcs[0].comments[i].mymcid = int.Parse(comments.Rows[i]["mymc_id"].ToString());
                    }
                }
                foreach (KeyValuePair<IWebSocketConnection, String> kv in allOnlineUsers)
                {
                    if (kv.Value == mgs.username)
                    {
                        string json = new JavaScriptSerializer().Serialize(sendMgs);//构建发送的消息
                        kv.Key.Send(json);
                    }
                }
            }
        }//发表评论

        public static int support(Mge mgs)
        {
            return SqlHelper.ExecuteNonQuery("insert into T_Support(mymc_id,username) values(@mymc_id,@username)",
                new SqlParameter("@mymc_id",mgs.mymcid),
                new SqlParameter("@username",mgs.username)
            );
        }//将赞记录到数据库中

        public static void support(Dictionary<IWebSocketConnection, String> allOnlineUsers, Mge mgs)
        {
            int c = support(mgs);
            if (c == 1)
            {
                Mge sendMgs = new Mge();
                sendMgs.username = mgs.username;
                sendMgs.action = mgs.action;
                sendMgs.addfriend = 1;
                DataTable supports = getSupports(mgs.mymcid);
                if (supports.Rows.Count > 0)
                {
                    sendMgs.arrallmyfriend = new string[supports.Rows.Count];
                    for (int i = 0; i < supports.Rows.Count; i++)
                    {
                        sendMgs.arrallmyfriend[i] = supports.Rows[i]["username"].ToString();
                    }
                }
                foreach (KeyValuePair<IWebSocketConnection, String> kv in allOnlineUsers)
                {
                    if (kv.Value == mgs.username)
                    {
                        string json = new JavaScriptSerializer().Serialize(sendMgs);//构建发送的消息
                        kv.Key.Send(json);
                    }
                }
            }
            return;
        }//赞

        public static int disSupport(Mge mgs)
        {
            return SqlHelper.ExecuteNonQuery("delete from T_Support where mymc_id=@mymc_id and username=@username",
                new SqlParameter("@mymc_id", mgs.mymcid),
                new SqlParameter("@username", mgs.username)
                );
        }//删除数据库中的赞记录


        public static void disSupport(Dictionary<IWebSocketConnection, String> allOnlineUsers, Mge mgs)
        {
            int c = disSupport(mgs);
            if (c == 1)
            {
                Mge sendMgs = new Mge();
                sendMgs.username = mgs.username;
                sendMgs.action = mgs.action;
                sendMgs.addfriend = 1;
                DataTable supports = getSupports(mgs.mymcid);
                if (supports.Rows.Count > 0)
                {
                    sendMgs.arrallmyfriend = new string[supports.Rows.Count];
                    for (int i = 0; i < supports.Rows.Count; i++)
                    {
                        sendMgs.arrallmyfriend[i] = supports.Rows[i]["username"].ToString();
                    }
                }
                foreach (KeyValuePair<IWebSocketConnection, String> kv in allOnlineUsers)
                {
                    if (kv.Value == mgs.username)
                    {
                        string json = new JavaScriptSerializer().Serialize(sendMgs);//构建发送的消息
                        kv.Key.Send(json);
                    }
                }
            }
            return;
        }//取消赞


    }
}
