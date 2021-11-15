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
            GameLogic.state = STATE.Waiting_Player;

            //check if all player already register
            bool allReady = true;
            for (int i = 0; i < Server.clients.Count; i++)
            {
                if (Server.clients[i].player.id == -1 
                    || Server.clients[i].player.username == "")
                {
                    allReady = false;
                }
            }

            if (allReady)
            {
                GameLogic.state = STATE.Game_Start;
            }
        }
    }
}
