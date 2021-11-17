using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class GuessWord
    {
        public static string UNKNOWN = "-";
        public string word;
        public string description;
        public string currentWord;

        public GuessWord(string _word, string _description)
        {
            _word = _word.ToLower().Trim();
            word = _word;
            description = _description;
            for (int i = 0; i < word.Length; i++)
            {
                currentWord += UNKNOWN;
            }
        }
    }
}
