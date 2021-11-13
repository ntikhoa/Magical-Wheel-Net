using System;
using System.Collections.Generic;
using System.Text;

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
            //display on server current number of player
        }
        private static void WaitingServer()
        {
            //Check if server want to start or quit
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
