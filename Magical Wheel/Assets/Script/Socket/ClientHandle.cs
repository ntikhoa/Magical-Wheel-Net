using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        //SocketDebug.Log($"Message from server: {_msg}");
        UIManager.instance.DisplayServerMessage(_msg);
        Client.instance.id = _myId;
        ClientSender.WelcomeReceived();
    }

    public static void UsernameAlreadyExits(Packet _packet)
    {
        string _msg = _packet.ReadString();
        //SocketDebug.Log($"Message from server: {_msg}");
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
        //SocketDebug.Log(_count.ToString() + " players have joined");
        UIManager.instance.DisplayServerMessage(_count.ToString() + " players have joined");
    }

    public static void SendGuessWord(Packet _packet)
    {
        int characterLimit = _packet.ReadInt();
        string hint = _packet.ReadString();
        int timeOut = _packet.ReadInt();
        //SocketDebug.Log(hint + " Get");
        UIManager.instance.GameStart(characterLimit, hint, timeOut);
    }

    public static void SendTurnStart(Packet _packet)
    {
        int _testId = _packet.ReadInt();
        //SocketDebug.Log(_testId.ToString() + " Get");
        string _testName = _packet.ReadString();
        if (Client.instance.id == _testId && Client.instance.userName == _testName)
        {
            //UIManager.instance.DisplayServerMessage(" Your turn.");
            SocketDebug.Log(_testId.ToString() + " your turn");
            UIManager.instance.State = STATE.Play_Turn;
        }
        else
        {
            UIManager.instance.State = STATE.Play_Wait;
            //SocketDebug.Log(_testId.ToString() + " turn");
        }
    }

    public static void SendTurnEnd(Packet _packet)
    {
        int _pid = _packet.ReadInt();
        string _pname = _packet.ReadString();
        int _pscore = _packet.ReadInt();
        string _pword = _packet.ReadString();
        string _pguess = _packet.ReadString();

        if(Client.instance.id != _pid)
        {
            UIManager.instance.DisplayServerMessage($"Player {_pname} guessed [{_pguess}] and scored {_pscore} points.");
        }
        else
        {
            UIManager.instance.DisplayServerMessage($"You have scored {_pscore} points.");
        }
        UIManager.instance.updateAnswer(_pword);
        UIManager.instance.State = STATE.Play_Wait;
        //SocketDebug.Log($"update to {_pword} after {_pname} guessing {_pguess}");
    }

    public static void Disqualify(Packet _packet)
    {
        int _pid = _packet.ReadInt();
        string _pname = _packet.ReadString();
        if(Client.instance.id == _pid)
        {
            UIManager.instance.DisplayServerMessage($"You have been disqualified.");
            UIManager.instance.State = STATE.Disqualify;
        }
        else
        {
            UIManager.instance.DisplayServerMessage($"Player {_pname} has been disqualified.");
        }
        //SocketDebug.Log($"{_pname} disqualified");
    }

    public static void SendRank(Packet _packet)
    {
        string _msg = " Result: \n";
        int _count = _packet.ReadInt();
        for (int i = 0; i < _count; i++)
        {
            int _pid = _packet.ReadInt();
            string _pname = _packet.ReadString();
            int _pscore = _packet.ReadInt();
            if(_pid == Client.instance.id)
            {
                _msg = _msg + $"{i}: You: {_pscore}\n";
            }
            else
            {
                _msg = _msg + $"{i}: {_pname}: {_pscore}\n";
            }
        }
        UIManager.instance.DisplayServerMessage(_msg);
        //SocketDebug.Log(_msg);
        UIManager.instance.State = STATE.End_Game;

    }
}
