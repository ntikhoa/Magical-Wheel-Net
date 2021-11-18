﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class GameEndState: State
    {
        public void Action()
        {
            //display result, waiting for server
            Console.WriteLine("Game End");

            var rank = InitRank();

            rank = SortRank(rank);

            ServerSender.SendRank(rank);

            ResetGameObject();

            GameLogic.SetState(STATE.Game_Start);
        }
        
        private List<Player> InitRank()
        {
            var rank = new List<Player>();
            for (int i = 0; i < Server.clients.Count; i++)
            {
                rank.Add(Server.clients[i].player);
            }
            return rank;
        }

        private List<Player> SortRank(List<Player> rank)
        {
            for (int i = 0; i < rank.Count; i++)
            {
                for (int j = i + 1; j < rank.Count; j++)
                {
                    if (rank[i].score < rank[j].score)
                    {
                        Player temp = rank[i];
                        rank[i] = rank[j];
                        rank[j] = temp;
                    }
                }
            }
            return rank;
        }

        private void ResetGameObject()
        {
            ResetPlayerState();
            ResetGameLogic();
        }

        private void ResetPlayerState()
        {
            for (int i = 0; i < Server.clients.Count; i++)
            {
                Server.clients[i].player.ResetPlayerStat();
            }
        }

        private void ResetGameLogic()
        {
            GameLogic.timeout = 10; //in seconds
            GameLogic.turn = 0;
            GameLogic.endTurn = Server.MaxPlayers * 5 - 1;
            GameLogic.lastGuess = "";
        }
    }
}