using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Server
{
    class GameStartState: State
    {
        public void Action()
        {
            //SetupGame, start score
            Console.WriteLine("Game Start");

            GameLogic.endGameTurn = Server.MaxPlayers * Constants.MAX_GAME_TURN - 1;
            GameLogic.guessWord = GetRandomGuessWord();

            ServerSender.SendGuessWord(GameLogic.guessWord, GameLogic.timeout);
            Thread.Sleep(2000);

            GameLogic.SetState(STATE.Turn_Start);
        }

        private GuessWord GetRandomGuessWord()
        {
            var words = getGuessWordFromDB();
            Random r = new Random();
            int rInt = r.Next(0, words.Count);
            return words[rInt];
        }

        private List<GuessWord> getGuessWordFromDB()
        {
            var words = new List<GuessWord>();
            string[] lines = System.IO.File.ReadAllLines(@"database.txt");
            int count = Int32.Parse(lines[0]);
            int c = 1;
            for (int i = 0; i < count; i++)
            {
                string word = lines[c++];
                string description = lines[c++];
                words.Add(new GuessWord(word, description));
            }
            return words;
        }
    }
}
