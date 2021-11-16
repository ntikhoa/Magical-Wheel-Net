using System;
using System.Collections.Generic;
using System.Threading;

namespace Server
{

    class Program
    {
        private static bool isRunning = false;
        private static string command = "";
        private static bool commandToExecute = false;
        private static Dictionary<string, Action> commandActions;
        private static Thread mainThread;
        static void Main(string[] args)
        {
            commandActions = new Dictionary<string, Action>
            {
                {"end", EndProgram }
            };

            Console.Title = "Magical Wheel Server";

            GameLogic.InitGame();

            isRunning = true;
            mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();

            Server.Start(2, 26950);


            //read command from server
            while (isRunning)
            {
                if (!commandToExecute)
                {
                    command = Console.ReadLine();
                    commandToExecute = commandActions.ContainsKey(command);

                    GameLogic.SetState(STATE.Waiting_Server);
                }
            }
        }

        private static void MainThread()
        {
            Console.WriteLine($"Main thread started. Running at {Constants.TICKS_PER_SEC} ticks per second.");
            DateTime nextLoop = DateTime.Now;

            while (isRunning)
            {
                while (nextLoop < DateTime.Now)
                {
                    if (command != "")
                    {
                        lock (command)
                        {
                            command.ToLower();
                            if (commandActions.ContainsKey(command))
                            {
                                Console.WriteLine("Please wait for the execution...");
                                ThreadManager.ExecuteOnMainThread(commandActions[command]);
                            }
                            command = "";
                        }
                    }
                    // If the time for the next loop is in the past, aka it's time to execute another tick
                    GameLogic.Update(); // Execute game logic

                    nextLoop = nextLoop.AddMilliseconds(Constants.MS_PER_TICK); // Calculate at what point in time the next tick should be executed

                    if (nextLoop > DateTime.Now)
                    {
                        // If the execution time for the next tick is in the future, aka the server is NOT running behind
                        Thread.Sleep(nextLoop - DateTime.Now); // Let the thread sleep until it's needed again.
                    }
                }
            }
            Console.WriteLine("Program Ended.");
            Server.End();
        }

        private static void EndProgram()
        {
            isRunning = false;
            Console.WriteLine("Ending Program...");
            commandToExecute = false;
        }
    }
}
