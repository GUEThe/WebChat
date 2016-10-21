using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    class sendMsg
    {
        //群发消息
        public void sendMessageToAllUser(List<IWebSocketConnection> allSockets,
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
        public int SendSingleMsg(List<IWebSocketConnection> allSockets,
                                 Dictionary<IWebSocketConnection, String> allOnlineUsers,
                                 string[] array, string message)
        {
            int sign = 0;
            string sendMsg; //获取要发送到客户端的文本
            foreach (var user in allOnlineUsers)
            {
                if (array[2] == user.Value)//匹配用户名
                {
                    string username = array[1];
                    sendMsg = username + ":" + message;//构建发送的消息
                    foreach (var socket in allSockets)
                    {
                        if (socket == user.Key)
                        {
                            socket.Send(sendMsg);  //向指定的用户发送消息
                            sign = 1;
                            break;
                        }
                    }
                }
            }
            return sign;
        }
    }
}
