using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class TurnStartState: State
    {
        public void Action()
        {
            //Inform a player about their turn, start timer, wait answer
            //Wait for player to take their turn
            Console.WriteLine("Turn Start");

            int playerIdTurn = GameLogic.turn % Server.clients.Count;

            Console.WriteLine($"Player's turn: {playerIdTurn}");

            if (Server.clients[playerIdTurn].player.disqualify)
            {
                GameLogic.turn += 1;
                GameLogic.SetState(STATE.Turn_Start);
                return;
            }

            ServerSender.SendTurnStart(playerIdTurn);

            
        }
    }
}
