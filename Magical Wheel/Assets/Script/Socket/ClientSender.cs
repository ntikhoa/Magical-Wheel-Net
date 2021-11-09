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
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.id);
            _packet.Write(UIManager.instance.inp.text);

            SendTCPData(_packet);
        }
    }
    #endregion
}
