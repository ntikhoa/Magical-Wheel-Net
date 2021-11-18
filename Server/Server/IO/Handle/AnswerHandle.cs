using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class AnswerHandle
    {
        public static void Handle(int fromClient, string guessChar, string guessWord)
        {
            if (Server.clients[fromClient].player.turn > 2
                && guessWord != "")
            {
                FullWordHandle(fromClient, guessWord);
            }
            else
            {
                OneCharHandle(fromClient, guessChar);
            }

            GameLogic.SetState(STATE.Turn_End);
        }

        private static void FullWordHandle(int fromClient, string guessWord)
        {
            GameLogic.lastGuess = guessWord;
            if (guessWord == GameLogic.guessWord.word)
            {
                Server.clients[fromClient].player.scoreGet = 5;
                Server.clients[fromClient].player.score += 5;
                GameLogic.guessWord.currentWord = guessWord;
            }
            else
            {
                Server.clients[fromClient].player.scoreGet = 0;
                Server.clients[fromClient].player.disqualify = true;
                ServerSender.Disqualify(fromClient);
            }
        }

        private static void OneCharHandle(int fromClient, string guessChar)
        {
            GameLogic.lastGuess = guessChar;
            bool isCorrect = false;
            string newCurrentWord = "";
            for (int i = 0; i < GameLogic.guessWord.word.Length; i++)
            {
                if (GameLogic.guessWord.currentWord[i].ToString() == GuessWord.UNKNOWN
                    && guessChar == GameLogic.guessWord.word[i].ToString())
                {
                    isCorrect = true;
                    newCurrentWord += guessChar;
                }
                else
                {
                    newCurrentWord += GameLogic.guessWord.currentWord[i];
                }
            }
            GameLogic.guessWord.currentWord = newCurrentWord;

            if (isCorrect)
            {
                Server.clients[fromClient].player.scoreGet = 1;
                Server.clients[fromClient].player.score += 1;
            }
            else
            {
                Server.clients[fromClient].player.scoreGet = 0;
            }
        }
    }
}
