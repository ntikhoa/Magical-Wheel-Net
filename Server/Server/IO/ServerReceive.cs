using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class ServerReceive
    {
        public static void Register(int fromClient, Packet packet)
        {
            int id = packet.ReadInt();
            string username = packet.ReadString();

            RegisterHandle.Handle(fromClient, username);
        }

        public static void Answer(int fromClient, Packet packet)
        {
            string guessChar = packet.ReadString();
            string guessWord = packet.ReadString();

            AnswerHandle.Handle(fromClient, guessChar, guessWord);
        }
    }
}
