﻿using System;
using System.Collections.Generic;
using System.Text;

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

                if (GameLogic.turn > GameLogic.endTurn 
                    || GameLogic.guessWord.word == GameLogic.guessWord.currentWord)
                {
                    GameLogic.SetState(STATE.Game_End);
                    return;
                }
            }

            GameLogic.SetState(STATE.Turn_Start);
        }
    }
}
