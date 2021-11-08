using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class ServerSender
    {

        public static void Welcome(int toClientId, string message)
        {
            using (Packet packet = new Packet((int)ServerPackets.welcome))
            {
                packet.Write(message);
                packet.Write(toClientId);
                SendTcpData(toClientId, packet);
            }
        }

        private static void SendTcpData(int toClientId, Packet packet)
        {
            packet.WriteLength();
            Server.clients[toClientId].SendData(packet);
        }

        private static void SendTCPDataToAll(Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].SendData(packet);
            }
        }
        private static void SendTCPDataToAll(int exceptClient, Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != exceptClient)
                {
                    Server.clients[i].SendData(packet);
                }
            }
        }
    }
}
