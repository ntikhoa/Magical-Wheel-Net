using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Server
{
    class TurnEndState: State
    {
        public void Action()
        {
            //Update result, score, turn count, player turn
            Console.WriteLine("Turn End");

            int playerIdTurn = GameLogic.turn % Server.clients.Count;

            if (!Server.clients[playerIdTurn].player.disqualify)
            {
                ServerSender.SendTurnEnd(playerIdTurn, GameLogic.guessWord);

                Server.clients[playerIdTurn].player.turn += 1;

                //player guess wrong and not disqualify, increment total turn by one
                if (Server.clients[playerIdTurn].player.scoreGet == 0)
                    GameLogic.turn += 1;

                if (GameLogic.turn > GameLogic.endGameTurn
                    || GameLogic.guessWord.word == GameLogic.guessWord.currentWord
                    || !IsAnyPlayerQualify())
                {
                    GameLogic.SetState(STATE.Game_End);
                    return;
                }
            }
            Thread.Sleep(2000);
            GameLogic.SetState(STATE.Turn_Start);

            Console.WriteLine("-------------------------------------");
        }

        private bool IsAnyPlayerQualify()
        {
            for (int i = 0; i < Server.clients.Count; i++)
            {
                if (!Server.clients[i].player.disqualify)
                    return true;
            }
            return false;
        }
    }
}
