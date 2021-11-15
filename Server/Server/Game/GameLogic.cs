using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Server
{
    public enum STATE
    {
        No_Action = -1,
        Waiting_Player = 0,
        Waiting_Server = 1,
        Game_Start = 2,
        Turn_Start = 3,
        Turn_End = 4,
        Game_End = 5
    }
    class GameLogic
    {
        public static STATE state = STATE.Waiting_Player; //Current state of the game server
        private static Dictionary<int, Action> stateActions;

        public static void Update()
        {
            ServerUpdate();
            ThreadManager.UpdateMain();
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
                {(int)STATE.No_Action, NoAction },
                {(int)STATE.Waiting_Player, WaitingPlayer },
                {(int)STATE.Waiting_Server, WaitingServer },
                {(int)STATE.Game_Start, GameStart },
                {(int)STATE.Turn_Start, TurnStart },
                {(int)STATE.Turn_End, TurnEnd },
                {(int)STATE.Game_End, GameEnd }
            };
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

            state = STATE.No_Action;
        }
        private static void WaitingServer()
        {
            //Check if server want to start or quit

            Console.WriteLine("Waiting Server");
            Thread.Sleep(5000);
            state = STATE.Waiting_Player;
            return;

            for (int i = 0; i < Server.clients.Count; i++)
            {
                Console.WriteLine(Server.clients[i].player.username);
            }
            ServerSender.GameStart(PktMsg.GAME_START);

            state = STATE.No_Action;
        }

        private static void GameStart()
        {
            //SetupGame, start score
            state = STATE.No_Action;
        }
        private static void TurnStart()
        {
            //Inform a player about their turn, start timer, wait answer
            //Wait for player to take their turn
            state = STATE.No_Action;
        }
        private static void TurnEnd()
        {
            //Update result, score, turn count, player turn
            state = STATE.No_Action;
        }
        private static void GameEnd()
        {
            //display result, waiting for server
            state = STATE.No_Action;
        }
        private static void UpdateState()
        {
            //update State
            state = STATE.No_Action;
        }

        private static void NoAction()
        {
            //set to this state whenever finished consume other state
            return;
        }
    }
}
