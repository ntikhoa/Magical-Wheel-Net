using System;
using System.Threading;

namespace Server
{

    class Program
    {
        private static bool isRunning = false;

        static void Main(string[] args)
        {
            Console.Title = "Magical Wheel Server";

            GameLogic.InitGame();

            isRunning = true;
            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();

            Server.Start(2, 26950);
        }

        private static void MainThread()
        {
            Console.WriteLine($"Main thread started. Running at {Constants.TICKS_PER_SEC} ticks per second.");
            DateTime nextLoop = DateTime.Now;

            while (isRunning)
            {
                while (nextLoop < DateTime.Now)
                {
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
        }
    }
}
