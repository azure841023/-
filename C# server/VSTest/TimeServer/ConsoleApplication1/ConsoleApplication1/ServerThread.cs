using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;



namespace ConsoleApplication1
{
     class ServerThread
    {
        public Socket serverSocket;
        public Socket clientSocket;
        private Struct_Internet inter;
        public string receiveMessage;
        private string sendMessage;

        private Thread threadConnect;
        private Thread threadReceive;

        private struct Struct_Internet
        {
            public string ip;
            public int port;
        }
        public ServerThread(AddressFamily family,SocketType socketType,ProtocolType protocoltype,string ip,int port)
        {
            serverSocket = new Socket(family, socketType, protocoltype);
            inter.ip = ip;
            inter.port = port;
            receiveMessage = null;
        }
        public void Listen()
        {
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse(inter.ip), inter.port));
            serverSocket.Listen(1);
        }

        public void StartConnect()
        {
            threadConnect = new Thread(Accept);
            threadConnect.IsBackground = true;
            threadConnect.Start();
        }
        public void StopConnect()
        {
            try
            {
                clientSocket.Close();
            }
            catch
            {


            }
        }
        
        public void Send(string message)
        {
            if(message == null)
            {


            }
            else
            {
                sendMessage = message;
                SendMessage();
            }

        }
        private void SendMessage()
        {
            try
            {
                if(serverSocket.Connected == true)
                {
                    serverSocket.Send(Encoding.ASCII.GetBytes(sendMessage));
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void Receive()
        {
            if(threadReceive != null  && threadReceive.IsAlive == true)
            {
                return;
            }
            threadReceive = new Thread(ReceiveMessage);
            threadReceive.IsBackground = true;
            threadReceive.Start();

        }
        private void ReceiveMessage()
        {
            try
            {
                if (clientSocket.Connected == true)
                {
                    byte[] bytes = new byte[256];
                    long datelength = clientSocket.Receive(bytes);
                    receiveMessage = Encoding.ASCII.GetString(bytes);
                    //Encode 

                }
            }
            catch (Exception)
            {


            }


        }
        private void Accept()
        {
            try
            {
                clientSocket = serverSocket.Accept();

            }
            catch (Exception)
            {


            }


        }
    }
}
