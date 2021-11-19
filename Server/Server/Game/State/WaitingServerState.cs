using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Server
{
    class WaitingServerState: State
    {
        public void Action()
        {
            //Check if server want to start or quit
            Console.WriteLine("Waiting Server");

            ServerSender.InformPlayer();
            Thread.Sleep(2000);
            GameLogic.SetState(STATE.Game_Start);
        }
    }
}
