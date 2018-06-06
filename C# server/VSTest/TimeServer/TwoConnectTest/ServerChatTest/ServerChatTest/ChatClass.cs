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

    public delegate String StrHandler(String str);

    public class ChatClassSet
    {
        public static String ServerIp = "";
        public static int port = 8877;
    }

    class ChatClass
    {
        public Socket socket;
        public NetworkStream stream;
        public StreamReader reader;
        public StreamWriter writer;
        public StrHandler isHandler;
        public EndPoint remoteEndPoint;
        public bool isDead = false;
        public ChatClass(Socket s)
        {
            socket = s;
            stream = new NetworkStream(s);
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);
            remoteEndPoint = socket.RemoteEndPoint;
        }

        public String receive()
        {
            return reader.ReadLine(); //receive message
        }
        public ChatClass send(String line)
        {
            writer.WriteLine(line); //??
            writer.Flush();         //??
            return this;
        }
        public static ChatClass connect(String ip)
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(ip), ChatClassSet.port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipep);
            return new ChatClass(socket);
        }
        public Thread newLister(StrHandler pHandler)
        {
            isHandler = pHandler;

            Thread listenThread = new Thread(new ThreadStart(listen));
            listenThread.Start();
            return listenThread;
        }
        public void listen()
        {
            try
            {
                while (true)
                {
                    String Line = receive();
                    isHandler(Line);
                }
            }
            catch (Exception ex)
            {
                isDead = true;
                Console.WriteLine(ex.Message);
            }


        }
    }

}
