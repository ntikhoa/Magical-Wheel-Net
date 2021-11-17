using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class ServerHandle
    {
        public static void Register(int fromClient, Packet packet)
        {
            int id = packet.ReadInt();
            string username = packet.ReadString();

            //check username exist
            for (int i = 0; i < Server.clients.Count; i++)
            {
                if (Server.clients[i].player.username == username)
                {
                    ServerSender.UsernameAlreadyExits(fromClient, PktMsg.USERNAME_ALREADY_EXIST);
                    return;
                }
            }

            Server.InitPlayer(fromClient, username);
            GameLogic.SetState(STATE.Waiting_Player);

            //check if all player already register
            bool allReady = true;
            for (int i = 0; i < Server.clients.Count; i++)
            {
                if (Server.clients[i].player.id == -1 
                    || Server.clients[i].player.username == "")
                {
                    allReady = false;
                }
            }

            if (allReady)
            {
                GameLogic.SetState(STATE.Waiting_Server);
            }
        }

        public static void Answer(int fromClient, Packet packet)
        {
            string guessChar = packet.ReadString();
            string guessWord = packet.ReadString();

            if (Server.clients[fromClient].player.turn > 2 
                && guessWord != "")
            {
                if (guessWord == GameLogic.guessWord.word)
                {
                    Server.clients[fromClient].player.scoreGet = 5;
                    Server.clients[fromClient].player.score += 5;
                    //turn -= 1 and then turn += 1 at STATE Turn_End => player get another turn
                    GameLogic.turn -= 1;

                    //TODO set game end
                }
                else
                {
                    Server.clients[fromClient].player.disqualify = true;
                    ServerSender.Disqualify(fromClient);
                }
            }
            else
            {
                bool isCorrect = false;
                String newCurrentWord = "";
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
                    //turn -= 1 and then turn += 1 at STATE Turn_End => player get another turn
                    GameLogic.turn -= 1;
                }
                else
                {
                    Server.clients[fromClient].player.scoreGet = 0;
                }
                
            }
           
            GameLogic.SetState(STATE.Turn_End);
        }
    }
}
