using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace ConsoleApplication1
{
    class Program
    {
        const string ip = "192.168.1.105";
        const int _port = 8877;
        private static Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        static void Main(string[] args)
        {
            Console.Title = "Client";
            IPAddress ipAddress = IPAddress.Parse(ip);
            IPEndPoint ipPoint = new IPEndPoint(ipAddress, _port);
            LoopConnect(ipPoint);
            SendLoop();
            Console.ReadLine();
        }
        private static void SendLoop()
        {
            while (true)
            {
                Console.Write("Enter a request: ");
                string req = Console.ReadLine();
                byte[] buffer = Encoding.ASCII.GetBytes(req);
                _clientSocket.Send(buffer);

                byte[] receiveBuf = new byte[1024];
                int rec = _clientSocket.Receive(receiveBuf);
                byte[] date = new byte[rec];
                Array.Copy(receiveBuf, date, rec);
                Console.WriteLine("Received: " + Encoding.ASCII.GetString(date));
               
            }
        }
        private static void LoopConnect(IPEndPoint ipPoint)
        {
            int attempts = 0;
            while (!_clientSocket.Connected)
            {
                try
                {
                    attempts++;
                    _clientSocket.Connect(ipPoint);
                }
                catch (SocketException)
                {
                    //Console.Clear();
                    Console.WriteLine("Connect attempts : " + attempts.ToString());
                }
            }
            Console.Clear();
            Console.WriteLine("Connceted");
        }
    }
}
