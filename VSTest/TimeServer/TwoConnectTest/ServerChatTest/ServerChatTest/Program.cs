using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Threading;
using System.Net.Sockets;
namespace ServerChatTest
{
    class Program
    {
        List<ChatClass> clientList = new List<ChatClass>(); // who connect 
        static void Main(string[] args)
        {
            Program program = new Program();
            program.run();
        }
        public void run()
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, ChatClassSet.port);
            Socket newsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            newsock.Bind(ipep);
            newsock.Listen(10);

            while (true)
            {
                Socket socket = newsock.Accept();
                Console.WriteLine("accept a connecter");
                ChatClass client = new ChatClass(socket);
                try
                {
                    clientList.Add(client);
                    client.newLister(processMsg);
                    
                }
                catch
                {



                }
            }
        }
        public String processMsg(String msg)
        {
            Console.WriteLine("re msg" + msg);
            broadCast(msg);
            return "OK";
        }
        public void broadCast(String msg)
        {
            Console.WriteLine("broad +" + msg + "inonline clinet "+clientList.Count +"person");
            foreach(ChatClass client in clientList)
            {
                if (!client.isDead)
                {
                    Console.WriteLine("Sent to" + client.remoteEndPoint.ToString() + ":" + msg);
                    client.send(msg);
                }
            }

        }
    }
}
