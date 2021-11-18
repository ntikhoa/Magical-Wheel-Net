using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class RegisterHandle
    {
        public static void Handle(int fromClient, string username)
        {
            if (IsUsernameExist(fromClient, username))
            {
                ServerSender.UsernameAlreadyExits(fromClient, PktMsg.USERNAME_ALREADY_EXIST);
                return;
            }

            Server.InitPlayer(fromClient, username);
            GameLogic.SetState(STATE.Waiting_Player);

            if (IsAllReady())
            {
                GameLogic.SetState(STATE.Waiting_Server);
            }
        }

        private static bool IsUsernameExist(int fromClient, string username)
        {
            for (int i = 0; i < Server.clients.Count; i++)
            {
                if (Server.clients[i].player.username == username)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsAllReady()
        {
            for (int i = 0; i < Server.clients.Count; i++)
            {
                if (Server.clients[i].player.id == -1
                    || Server.clients[i].player.username == "")
                {
                    return false;
                }
            }
            return true;
        }
    }
}
