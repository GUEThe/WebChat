using DAL;
using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebChatServer
{
    class server
    {

        static void Main(string[] args)
        {
            FleckLog.Level = LogLevel.Debug;
            int serverport = 8181;//服务端端口号
            IPEndPoint point = new IPEndPoint(getLocalmachineIPAddress(), serverport);          
            var server = new WebSocketServer("ws://"+point);

            //var allSockets = new List<IWebSocketConnection>();
            Dictionary<IWebSocketConnection, string> allOnlineUser = new Dictionary<IWebSocketConnection, string>();


            server.Start(socket =>
                {
                    socket.OnOpen = () =>
                        {
                            //链接成功                          
                        };
                    socket.OnClose = () =>
                        {
                            string username="";
                            foreach (KeyValuePair<IWebSocketConnection, String> kv in allOnlineUser)
                            {
                                if (kv.Key == socket)
                                {
                                    username = kv.Value;
                                }
                            }
                            Console.WriteLine("{0} sign out", username);
                            allOnlineUser.Remove(socket);
                            Servers.updateUserList(allOnlineUser);

                            //关闭链接
                        };
                    socket.OnMessage = message =>
                        {
                            Mge mge = JsonConvert.DeserializeObject<Mge>(message);
                            switch (mge.action)
                            {
                                case "login":
                                    allOnlineUser.Add(socket, mge.username);
                                    Servers.updateUserList(allOnlineUser);
                                    Console.WriteLine("{0} login", mge.username);
                                    break;
                                case "talk":
                                    Servers.sendMsgToMyFriend(allOnlineUser, mge);
                                    break;
                                case "talkToAll":
                                    Servers.sendMessageToAllUser(allOnlineUser, mge);
                                    break;
                                case "findFriend":
                                    Servers.findMyFriend(allOnlineUser, mge);
                                    break;
                                case "addFriend":
                                    Servers.addFriend(allOnlineUser, mge);
                                    Servers.updateUserList(allOnlineUser);
                                    break;
                                case "getChatlog":
                                    Servers.getChatLog(allOnlineUser, mge);
                                    break;
                                //case "getAllMyFriend":
                                //    Servers.getAllMyFriend(allOnlineUser, mge);
                                //    break;
                                case "createDiscussionGroup":
                                    Servers.createDiscussionGroup(allOnlineUser, mge);
                                    Servers.updateUserList(allOnlineUser);
                                    break;
                                case "talkToDG":
                                    Servers.sendMsgToMyDG(allOnlineUser, mge);
                                    break;
                                case "delChatlog":
                                    Servers.delChatLog(allOnlineUser, mge);
                                    break;
                                case "getDGmember":
                                    Servers.getDGmember(allOnlineUser, mge);
                                    break;
                                case "getMYmc":
                                    Servers.getMYmc(allOnlineUser, mge);
                                    break;
                                case "publishMYmc":
                                    Servers.insertMYmc(mge);
                                    break;
                                case"publishComment":
                                    Servers.publishComment(allOnlineUser, mge);
                                    break;
                                case "support":
                                    Servers.support(allOnlineUser, mge);
                                    break;
                                case "disSupport":
                                    Servers.disSupport(allOnlineUser, mge);
                                    break;

                            }
                            //switch (mgs[0])
                            //{
                                
                            //}
                            //服务端接收到客户端消息后
                        };
                });
           

            //服务端广播
            var input = Console.ReadLine();
            while (input != "exit")
            {
                foreach (KeyValuePair<IWebSocketConnection, String> kv in allOnlineUser)
                {
                    kv.Key.Send(input);
                }
                input = Console.ReadLine();
            }

        }
        //获得ip服务端所在电脑ip
        public static IPAddress getLocalmachineIPAddress()
        {
            string strHostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);

            foreach (IPAddress ip in ipEntry.AddressList)
            {
                //IPV4
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip;
            }

            return ipEntry.AddressList[0];
        }

    }
}
