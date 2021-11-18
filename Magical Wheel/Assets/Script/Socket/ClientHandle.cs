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
        UIManager.instance.DisplayServerMessage(_msg);
        Client.instance.id = _myId;
        ClientSender.WelcomeReceived();
    }

    public static void UsernameAlreadyExits(Packet _packet)
    {
        string _msg = _packet.ReadString();
        SocketDebug.Log($"Message from server: {_msg}");
        UIManager.instance.DisplayServerMessage(_msg);
        UIManager.instance.State = STATE.Register;
    }

    public static void InformPlayer(Packet _packet)
    {
        int _count = _packet.ReadInt();
        for (int i = 0; i < _count; i++)
        {
            _packet.ReadInt();
            _packet.ReadString();
        }
        UIManager.instance.DisplayServerMessage(_count.ToString() + " players have joined");
    }

    public static void SendGuessWord(Packet _packet)
    {
        int characterLimit = _packet.ReadInt();
        string hint = _packet.ReadString();
        int timeOut = _packet.ReadInt();
        UIManager.instance.GameStart(characterLimit, hint, timeOut);
    }

    public static void SendTurnStart(Packet _packet)
    {
        int _testId = _packet.ReadInt();
        string _testName = _packet.ReadString();
        if (Client.instance.id == _testId && Client.instance.userName == _testName)
        {
            UIManager.instance.State = STATE.Play_Turn;
            UIManager.instance.DisplayServerMessage(" Your turn.");
        }
        else
        {
            UIManager.instance.State = STATE.Play_Wait;
        }
    }

    public static void SendTurnEnd(Packet _packet)
    {
        int _pid = _packet.ReadInt();
        string _pname = _packet.ReadString();
        int _pscore = _packet.ReadInt();
        string _pword = _packet.ReadString();

        if(Client.instance.id != _pid)
        {
            UIManager.instance.DisplayServerMessage($"Player {_pname} has scored {_pscore} points.");
        }
        else
        {
            UIManager.instance.DisplayServerMessage($"You have scored {_pscore} points.");
        }
        UIManager.instance.updateAnswer(_pword);
    }
}
