using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class ServerHandle
    {
        public static void Register(int fromClient, Packet packet)
        {
            string username = packet.ReadString();

            //check username exist
            for (int i = 0; i < Server.clients.Count; i++)
            {
                if (Server.clients[i].player.username == username)
                {
                    ServerSender.UsernameAlreadyExits(fromClient, PktMsg.USERNAME_ALREADY_EXIST);
                    return;
                }
            }

            Server.InitPlayer(fromClient, username);

            //check if all player already register
            bool allReady = true;
            for (int i = 0; i < Server.clients.Count; i++)
            {
                if (Server.clients[i].player.username == null 
                    || Server.clients[i].player.username == "")
                {
                    allReady = false;
                }
            }

            if (allReady)
            {
                for (int i = 0; i < Server.clients.Count; i++)
                {
                    Console.WriteLine(Server.clients[i].player.username);
                }
                ServerSender.GameStart(PktMsg.GAME_START);
            }
        }
    }
}
