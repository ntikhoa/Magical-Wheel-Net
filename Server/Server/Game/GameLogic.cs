using System;
using System.Collections.Generic;

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
        public static string lastGuess = "";
        public static GuessWord guessWord;
        public static int timeout = 10; //in seconds
        public static int turn = 0;
        public static int endTurn = Server.MaxPlayers * 5 - 1;
        

        private static STATE state = STATE.Waiting_Player; //Current state of the game server
        private static Dictionary<int, State> stateActions;

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
            ThreadManager.ExecuteOnMainThread(stateActions[(int)state].Action);
            ThreadManager.ExecuteOnMainThread(UpdateState);
        }
        public static void InitGame()
        {
            //Prepare game server (loading game, etc)
            stateActions = new Dictionary<int, State>
            {
                {(int)STATE.Waiting_Player, new WaitingPlayerState() },
                {(int)STATE.Waiting_Server, new WaitingServerState() },
                {(int)STATE.Game_Start, new GameStartState() },
                {(int)STATE.Turn_Start, new TurnStartState() },
                {(int)STATE.Turn_End, new TurnEndState() },
                {(int)STATE.Game_End, new GameEndState() }
            };

            ServerUpdate();
        }
     
        private static void UpdateState()
        {
            //update State
        }
    }
}
