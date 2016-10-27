using DAL;
using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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

            string[] mgs;

            server.Start(socket =>
                {
                    socket.OnOpen = () =>
                        {
                            //链接成功                          
                        };
                    socket.OnClose = () =>
                        {
                            allOnlineUser.Remove(socket);
                            Servers.updateuserlist(allOnlineUser);

                            //关闭链接
                        };
                    socket.OnMessage = message =>
                        {
                            mgs = message.Split('|');
                            switch (mgs[0])
                            {
                                case "login":
                                    allOnlineUser.Add(socket, mgs[1]);
                                    Servers.updateuserlist(allOnlineUser);
                                    Console.WriteLine("{0} login", mgs[1]);
                                    break;
                                case "talk":
                                    Servers.SendSingleMsg(allOnlineUser, mgs);   
                                    break;
                                case "findFriend":
                                    Servers.findMyFriend(allOnlineUser, mgs);
                                    break;
                                case "addFriend":
                                    Servers.addFriend(allOnlineUser, mgs);
                                    Servers.updateuserlist(allOnlineUser);
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
