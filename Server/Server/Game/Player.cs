using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class Player
    {
        public int id;
        public string username;
        public int score;
        public int turn;
        //return the score get after turn end to send to player
        public int scoreGet;
        public bool disqualify;

        public Player(int _id, string _username)
        {
            id = _id;
            username = _username;
            score = 0;
            scoreGet = 0;
            turn = 1;
            disqualify = false;
        }

        public void ResetPlayerStat()
        {
            score = 0;
            scoreGet = 0;
            turn = 1;
            disqualify = false;
        }
    }
}
