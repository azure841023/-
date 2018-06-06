using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace TimeServer
{
    class Program
    {
       
        const string ip = "192.168.1.105";
        const int _port = 8877;
        private static byte[] _buffer = new byte[1024];
        private static List<Socket> _clinetSockets = new List<Socket>();
        private static Socket _serverSocket = new Socket
            (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //address  for socket class of IP4

        static void Main(string[] args)
        {
            Console.Title = "Server";
            SetupServer();
            Console.ReadLine();

        }
        private static void SetupServer()
        {
            Console.WriteLine("Setting up server.....");
            //IPAddress ipAddress = IPAddress.Parse(ip);
            //IPEndPoint ipPoint = new IPEndPoint(ipAddress, _port);
            //_serverSocket.Bind(ipPoint);
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 100));
            //build socket conncet 
            //InEndPoint features network endpoint change IP address and port number; 
            _serverSocket.Listen(5);
            //Listen connect endpoint(max)  
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            //AsyncCallback asynchronous finish the result
            //second return object state(?)        
        }

        private static void AcceptCallback(IAsyncResult AR)
            {
            
            Socket socket = _serverSocket.EndAccept(AR);
            _clinetSockets.Add(socket);
            Console.WriteLine("clinets connect");
            Console.Title = "Server, " + _clinetSockets.Count.ToString() + " clients are connected";
            socket.BeginReceive(_buffer,0,_buffer.Length,SocketFlags.None,new AsyncCallback(ReceiveCallback),null);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);




            }
        private static void ReceiveCallback(IAsyncResult AR)
        {

               Socket socket = (Socket)AR.AsyncState;
                // try
                // {
                //store socke state
                int received = socket.EndReceive(AR); ///how error....?
                // EndReceive is end receive , return receive int number
                byte[] dateBuffer = new byte[received];
                Array.Copy(_buffer, dateBuffer, received);
                //(purpose, source,size)
                string text = Encoding.ASCII.GetString(dateBuffer);
                //byte change to string
                Console.WriteLine("Test received " + text);

                string respone = string.Empty;
                if (text.ToLower() != "get time")
                {
                    respone = "Invail request";
                }
                else
                {
                    respone = DateTime.Now.ToLongTimeString();
                }

            byte[] date = Encoding.ASCII.GetBytes(DateTime.Now.ToLongTimeString());
            socket.BeginSend(date, 0, date.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
        }

        private static void SendCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            //get currect socket state
            socket.EndSend(AR);



        }
    }
}
