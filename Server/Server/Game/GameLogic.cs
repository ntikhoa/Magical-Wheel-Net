using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

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
            //display on server current number of player
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

            Thread.Sleep(5000);
            
            for (int i = 0; i < Server.clients.Count; i++)
            {
                Console.WriteLine(Server.clients[i].player.username);
            }
            ServerSender.GameStart(PktMsg.GAME_START);
            Console.WriteLine("End Waiting Server");
        }

        private static void GameStart()
        {
            //SetupGame, start score
        }
        private static void TurnStart()
        {
            //Inform a player about their turn, start timer, wait answer
            //Wait for player to take their turn
        }
        private static void TurnEnd()
        {
            //Update result, score, turn count, player turn
        }
        private static void GameEnd()
        {
            //display result, waiting for server
        }
        private static void UpdateState()
        {
            //update State
        }
    }
}
