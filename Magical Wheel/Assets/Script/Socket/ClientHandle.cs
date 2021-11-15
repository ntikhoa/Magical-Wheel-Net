using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        SocketDebug.Log("Welcome Get");
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        SocketDebug.Log($"Message from server: {_msg}");
        Client.instance.id = _myId;
        ClientSender.WelcomeReceived();
    }
}
