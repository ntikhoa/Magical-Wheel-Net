using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Collections;

namespace Server
{
    public enum STATE
    {
        Waiting_Player = 0,
        Waiting_Server = 1,
        Game_Start = 2,
        Turn_Start = 3,
        Turn_End = 4,
        Game_End = 5
    }
    class GameLogic
    {
        public static GuessWord guessWord;
        public static int timeout = 10; //in seconds
        public static int turn = 0;
        public static int endTurn = Server.MaxPlayers * 5 - 1;

        private static STATE state = STATE.Waiting_Player; //Current state of the game server
        private static Dictionary<int, Action> stateActions;

        public static void Update()
        {
            ThreadManager.UpdateMain();
        }

        public static void SetState(STATE _state)
        {
            state = _state;
            ServerUpdate();
        }

        private static void ServerUpdate()
        {
            ThreadManager.ExecuteOnMainThread(stateActions[(int)state]);
            ThreadManager.ExecuteOnMainThread(UpdateState);
        }
        public static void InitGame()
        {
            //Prepare game server (loading game, etc)
            stateActions = new Dictionary<int, Action>
            {
                {(int)STATE.Waiting_Player, WaitingPlayer },
                {(int)STATE.Waiting_Server, WaitingServer },
                {(int)STATE.Game_Start, GameStart },
                {(int)STATE.Turn_Start, TurnStart },
                {(int)STATE.Turn_End, TurnEnd },
                {(int)STATE.Game_End, GameEnd }
            };

            ServerUpdate();
        }
        private static void WaitingPlayer()
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
        private static void WaitingServer()
        {
            //Check if server want to start or quit
            Console.WriteLine("Waiting Server");

            //ServerSender.InformPlayer();
            Thread.Sleep(3000);
            SetState(STATE.Game_Start);
        }

        private static void GameStart()
        {
            //SetupGame, start score
            Console.WriteLine("Game Start");

            guessWord = new GuessWord("python", "Worst Programming Language!!!");
            ServerSender.SendGuessWord(guessWord, timeout);
            Thread.Sleep(2000);

            SetState(STATE.Turn_Start);
        }
        private static void TurnStart()
        {
            //Inform a player about their turn, start timer, wait answer
            //Wait for player to take their turn
            Console.WriteLine("Turn Start");

            int playerIdTurn = turn % Server.clients.Count;
            if(Server.clients[playerIdTurn].player.disqualify)
            {
                turn += 1;
                SetState(STATE.Turn_Start);
                return;
            }

            ServerSender.SendTurnStart(playerIdTurn);
        }
        private static void TurnEnd()
        {
            //Update result, score, turn count, player turn
            Console.WriteLine("Turn End");

            int playerIdTurn = turn % Server.clients.Count;

            ServerSender.SendTurnEnd(playerIdTurn, guessWord);

            Server.clients[playerIdTurn].player.turn += 1;
            turn += 1;
            
            if (turn > endTurn || guessWord.word == guessWord.currentWord)
            {
                SetState(STATE.Game_End);
                return;
            }
            SetState(STATE.Turn_Start);
        }
        private static void GameEnd()
        {
            //display result, waiting for server
            Console.WriteLine("Game End");

            var rank = new List<Player>();
            
            //init
            for (int i = 0; i < Server.clients.Count; i++)
            {
                rank.Add(Server.clients[i].player);
            }

            //sort
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

            ServerSender.SendRank(rank);
            Thread.Sleep(3000);

            //TODO: restart game, init players stat,...
        }
        private static void UpdateState()
        {
            //update State
        }
    }
}
