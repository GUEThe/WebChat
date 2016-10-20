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

            var allSockets = new List<IWebSocketConnection>();
            Dictionary<IWebSocketConnection, string> allOnlineUser = new Dictionary<IWebSocketConnection, string>();

            string[] mgs;

            server.Start(socket =>
                {
                    socket.OnOpen = () =>
                        {
                            allSockets.Add(socket);
                            //链接成功
                        };
                    socket.OnClose = () =>
                        {
                            //关闭链接
                        };
                    socket.OnMessage = message =>
                        {
                            mgs = message.Split('|');
                            switch (mgs[0])
                            {
                                case "login":
                                    allOnlineUser.Add(socket, mgs[1]); 
                                    break;
                                case "talk": break;
                            }
                            Console.WriteLine("{0}",mgs[1]);
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
                foreach (var socket in allSockets.ToList())
                {
                    socket.Send(input);
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
