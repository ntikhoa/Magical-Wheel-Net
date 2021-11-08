using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Server
    {
        public static int MaxPlayers { get; private set; }
        public static int Port { get; private set; }
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();

        private static TcpListener tcpListener;

        public static void Start(int maxPlayers, int port)
        {
            MaxPlayers = maxPlayers;
            Port = port;

            Console.WriteLine("Starting server...");
            InitializeClient();

            tcpListener = new TcpListener(IPAddress.Loopback, Port);
            tcpListener.Start();

            tcpListener.BeginAcceptTcpClient(TcpConnectCallback, null);

            Console.WriteLine($"Server start on port {Port}");
        }

        private static void InitializeClient()
        {
            for (int i = 0; i <= MaxPlayers; i++)
            {
                clients.Add(i, new Client(i));
            }
        }

        private static void TcpConnectCallback(IAsyncResult result)
        {
            TcpClient client = tcpListener.EndAcceptTcpClient(result);
            tcpListener.BeginAcceptTcpClient(TcpConnectCallback, null);

            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (clients[i].socket == null)
                {
                    clients[i].Connect(client);
                    return;
                }
            }

            Console.WriteLine($"{client.Client.RemoteEndPoint} failed to connect: Server full!");
        }
    }
}
