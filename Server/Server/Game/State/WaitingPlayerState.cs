using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class WaitingPlayerState: State
    {
        public void Action()
        {
            Console.WriteLine("Waiting Player");

            for (int i = 0; i < Server.clients.Count; i++)
            {
                if (Server.clients[i].player.id != -1)
                {
                    Console.WriteLine($"{Server.clients[i].player.username} is ready! (Player's ID: {Server.clients[i].player.id})");
                }
            }
        }
    }
}
