using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Magical Wheel Server";

            Server.Start(50, 26950);

            Console.ReadKey();
        }
    }
}
