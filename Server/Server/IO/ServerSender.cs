using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class ServerSender
    {
        public static void Welcome(int toClientId, string message)
        {
            using (Packet packet = new Packet((int)ServerPackets.welcome))
            {
                packet.Write(message);
                packet.Write(toClientId);
                SendTcpData(toClientId, packet);
            }
        }

        public static void UsernameAlreadyExits(int toClientId, string message)
        {
            using (Packet packet = new Packet((int)ServerPackets.username_already_exist))
            {
                packet.Write(message);
                SendTcpData(toClientId, packet);
            }
        }

        /*
        public static void InformPlayer()
        {
            using(Packet packet = new Packet((int)ServerPackets.inform_player))
            {
                packet.Write(Server.clients.Count);
                for (int i = 0; i < Server.clients.Count; i++)
                {
                    packet.Write(Server.clients[i].player.id);
                    packet.Write(Server.clients[i].player.username);
                }
                SendTCPDataToAll(packet);
            }
        }
        */

        public static void SendGuessWord(GuessWord guessWord, int timeout)
        {
            using (Packet packet = new Packet((int)ServerPackets.send_game_obj))
            {
                packet.Write(guessWord.word.Length);
                packet.Write(guessWord.description);
                packet.Write(timeout);
                
                SendTCPDataToAll(packet);
            }
        }

        public static void SendTurnStart(int playerIdTurn)
        {
            using (Packet packet = new Packet((int)ServerPackets.turn_start))
            {
                packet.Write(Server.clients[playerIdTurn].player.id);
                packet.Write(Server.clients[playerIdTurn].player.username);
                packet.Write(Server.clients[playerIdTurn].player.turn);

                SendTCPDataToAll(packet);
            }
        }

        public static void SendTurnEnd(int playerIdTurn, GuessWord guessWord)
        {
            using (Packet packet = new Packet((int)ServerPackets.turn_end))
            {
                packet.Write(Server.clients[playerIdTurn].player.id);
                packet.Write(Server.clients[playerIdTurn].player.username);
                packet.Write(Server.clients[playerIdTurn].player.scoreGet);
                packet.Write(guessWord.currentWord);

                SendTCPDataToAll(packet);
            }
        }

        public static void Disqualify(int playerId)
        {
            using (Packet packet = new Packet((int)ServerPackets.disqualify))
            {
                packet.Write(playerId);
                packet.Write(Server.clients[playerId].player.username);

                SendTCPDataToAll(packet);
            }
        }

        public static void SendRank(List<Player> rank)
        {
            using (Packet packet = new Packet((int)ServerPackets.end_rank))
            {
                packet.Write(rank.Count);
                for (int i = 0; i< rank.Count; i++)
                {
                    packet.Write(rank[i].id);
                    packet.Write(rank[i].username);
                    packet.Write(rank[i].score);
                }

                SendTCPDataToAll(packet);
            }
        }

        private static void SendTcpData(int toClientId, Packet packet)
        {
            packet.WriteLength();
            Server.clients[toClientId].SendData(packet);
        }

        private static void SendTCPDataToAll(Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].SendData(packet);
            }
        }
        private static void SendTCPDataToAll(int exceptClient, Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != exceptClient)
                {
                    Server.clients[i].SendData(packet);
                }
            }
        }
    }
}
