using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class Constants
    {
        public const int TICKS_PER_SEC = 30; // How many ticks per second
        public const float MS_PER_TICK = 1000f / TICKS_PER_SEC; // How many milliseconds per tick
        public const int MAX_GAME_TURN = 5;
    }
}
