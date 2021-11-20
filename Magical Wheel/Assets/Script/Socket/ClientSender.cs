using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSender : MonoBehaviour
{
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    #region Packets
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.register))
        {
            _packet.Write(Client.instance.id);
            _packet.Write(Client.instance.userName);
            SocketDebug.Log($"Client {Client.instance.id} send {ClientPackets.register}");
            SendTCPData(_packet);
        }
    }
    public static void Answer(string _letter, string _word)
    {
        using (Packet _packet = new Packet((int)ClientPackets.answer))
        {
            _packet.Write(_letter);
            _packet.Write(_word);
            SocketDebug.Log($"Client {Client.instance.id} send {ClientPackets.answer}: _letter: {_letter} _word: {_word}");
            SendTCPData(_packet);
        }
    }
    #endregion
}
