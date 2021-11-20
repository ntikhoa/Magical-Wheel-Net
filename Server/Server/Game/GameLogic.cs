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
        public static int endGameTurn;
        

        private static STATE state = STATE.Waiting_Player; //Current state of the game server
        private static Dictionary<STATE, State> stateActions;
        private static Dictionary<STATE, STATE[]> adjacentStates;

        public static void Update()
        {
            ThreadManager.UpdateMain();
        }

        public static void SetState(STATE _state)
        {
            Console.WriteLine($"Attempt to go to state: {_state}");

            //when player disconnect, every state can go to state Waiting Player
            if (IsValidNextState(_state) || _state == STATE.Waiting_Player)
            {
                state = _state;
                ServerUpdate();
            }
            else
            {
                Console.WriteLine($"Does not allow to go to state: {_state}");
                Console.WriteLine("-------------------------------------");
            }
        }

        private static void ServerUpdate()
        {
            ThreadManager.ExecuteOnMainThread(stateActions[state].Action);
            ThreadManager.ExecuteOnMainThread(UpdateState);
        }
        public static void InitGame()
        {
            //Prepare game server (loading game, etc)
            stateActions = new Dictionary<STATE, State>
            {
                {STATE.Waiting_Player, new WaitingPlayerState() },
                {STATE.Waiting_Server, new WaitingServerState() },
                {STATE.Game_Start, new GameStartState() },
                {STATE.Turn_Start, new TurnStartState() },
                {STATE.Turn_End, new TurnEndState() },
                {STATE.Game_End, new GameEndState() }
            };

            adjacentStates = new Dictionary<STATE, STATE[]>
            {
                {STATE.Waiting_Player, new STATE[] { STATE.Waiting_Player, STATE.Waiting_Server } },
                {STATE.Waiting_Server, new STATE[] { STATE.Game_Start } },
                {STATE.Game_Start, new STATE[] { STATE.Turn_Start } },
                {STATE.Turn_Start, new STATE[] { STATE.Turn_Start, STATE.Turn_End } },
                {STATE.Turn_End, new STATE[] { STATE.Turn_Start, STATE.Game_End } },
                {STATE.Game_End, new STATE[] { STATE.Game_Start } }
            };

            ServerUpdate();
        }

        private static bool IsValidNextState(STATE _state)
        {
            var adjacentState = adjacentStates[state];
            for (int i = 0; i < adjacentState.Length; i++)
            {
                if (_state == adjacentState[i])
                {
                    return true;
                }
            }
            return false;
        }
     
        private static void UpdateState()
        {
            //update State
        }
    }
}
