using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
public class Client : MonoBehaviour
{
    public static Client instance;
    public static int dataSize = 4096;
    public string ip = "127.0.0.1";
    public int port = 26950;
    public int id = 0;
    public string userName = "";
    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    //Hearbeat Check
    private Coroutine trackBeat = null;
    private bool rcvBeat = false;
    public IEnumerator BeatTracking()
    {
        yield return new WaitForSeconds(2);
        if (!rcvBeat)
        {
            Disconnect();
        }
        else
        {
            rcvBeat = false;
            trackBeat = StartCoroutine(BeatTracking());
        }
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    public TCP tcp;

    private void Start()
    {
        InitializeClientData();
        tcp = new TCP();

        Application.quitting += Disconnect;
    }

    public void ConnectedToServer()
    {
        userName = UIManager.instance.userName;
        tcp.Connect();
    }

    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.welcome, ClientHandle.Welcome },
            { (int)ServerPackets.username_already_exist, ClientHandle.UsernameAlreadyExits },
            { (int)ServerPackets.inform_player, ClientHandle.InformPlayer },
            { (int)ServerPackets.send_game_obj, ClientHandle.SendGuessWord },
            { (int)ServerPackets.turn_start, ClientHandle.SendTurnStart },
            { (int)ServerPackets.turn_end, ClientHandle.SendTurnEnd },
            { (int)ServerPackets.disqualify, ClientHandle.Disqualify },
            { (int)ServerPackets.end_rank, ClientHandle.SendRank },
            { (int)ServerPackets.dummy, ClientHandle.Dummy }
        };
    }

    public void Disconnect()
    {
        tcp.Disconnect();
    }

    public void UpdateBeat()
    {
        rcvBeat = true;
    }

    public void StartBeat()
    {
        trackBeat = StartCoroutine(BeatTracking());
    }
    public class TCP
    {
        public TcpClient socket;

        //private NetworkStream stream;
        private Packet receiveData;
        private byte[] receiveBuffer;

        public void Connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = dataSize,
                SendBufferSize = dataSize

            };
            socket.Client.Blocking = false;
            receiveBuffer = new byte[dataSize];
            //SocketDebug.Log("Begin Connect");
            try
            {
                socket.BeginConnect(instance.ip, instance.port, ConnectCallBack, socket);
            }
            catch (Exception e)
            {
                SocketDebug.Log($"Cannot Locate Server: {e.Message}");
                Disconnect();                
            }
        }
        public void Disconnect()
        {
            if (socket != null)
            {
                if (socket.Connected)
                {
                    socket.Close();
                    //SocketDebug.Log("Close Connection");
                }
            }
            receiveData = null;
            receiveBuffer = null;
            socket = null;
            //SocketDebug.Log("Go Register");
            UIManager.instance.State = STATE.Register;
        }

        public void SendData(Packet _packet)
        {
            try
            {
                if (socket != null)
                {
                    //stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                    //Non Blocking
                    socket.Client.BeginSend(_packet.ToArray(), 0, _packet.Length(), SocketFlags.None, null, null);
                }
            }
            catch (Exception _ex)
            {
                SocketDebug.Log($"Error sending data to server via TCP: {_ex}");
                Disconnect();
            }
        }

        private void ConnectCallBack(IAsyncResult _res)
        {
            try
            {
                //SocketDebug.Log("Connect Call Back");
                socket.EndConnect(_res);

                if (!socket.Connected)
                {
                    return;
                }

                Client.instance.StartBeat();
                //stream = socket.GetStream();
                receiveData = new Packet();
                //SocketDebug.Log("Begin Read");
                //stream.BeginRead(receiveBuffer, 0, dataSize, ReceiveCallBack, null);
                //nNon Blocking
                socket.Client.BeginReceive(receiveBuffer, 0, dataSize, SocketFlags.None, ReceiveCallBack, null);
            }
            catch(SocketException ex)
            {
                //SocketDebug.Log(ex.Message);
                SocketError s_er = ex.SocketErrorCode;
                if (s_er == SocketError.WouldBlock)
                {
                    SocketDebug.Log($"would block: {s_er}");
                    socket.BeginConnect(instance.ip, instance.port, ConnectCallBack, socket);
                }
                else
                {
                    SocketDebug.Log($"not would block: {s_er}");
                    Disconnect();
                }
            }
            catch(Exception ex)
            {
                //SocketDebug.Log(ex.Message);
                Disconnect();
            }
        }

        private void ReceiveCallBack(IAsyncResult _res)
        {
            //SocketDebug.Log("Receive Call Back");
            try
            {
                //int _bytelength = stream.EndRead(_res);
                //Non Blocking
                int _bytelength = socket.Client.EndReceive(_res);
                if(_bytelength == 0)
                {
                    //SocketDebug.Log("Length = 0");
                    return;
                }

                byte[] _data = new byte[_bytelength];
                Array.Copy(receiveBuffer, _data, _bytelength);
                receiveData.Reset(CheckInvalidData(_data));
                //stream.BeginRead(receiveBuffer, 0, dataSize, ReceiveCallBack, null);
                //Non Blocking
                socket.Client.BeginReceive(receiveBuffer, 0, dataSize, SocketFlags.None, ReceiveCallBack, null);
            }
            catch(Exception ex)
            {
                SocketDebug.Log($"Receive fail: {ex}");
                Disconnect();
            }
        }

        private bool CheckInvalidData(byte[] _data)
        {
            int _packetLen = 0;
            receiveData.SetBytes(_data);
            while (receiveData.UnreadLength() >= 4 && _packetLen <= 0)
            {
                _packetLen = receiveData.ReadInt();
            }

            while(_packetLen > 0 && _packetLen <= receiveData.UnreadLength())
            {
                byte[] _packetByte = receiveData.ReadBytes(_packetLen);
                ThreadManager.AddAction(() => {
                    using (Packet _p = new Packet(_packetByte))
                    {
                        int _pId = _p.ReadInt();
                        packetHandlers[_pId](_p);
                    }
                });

                _packetLen = 0;
                while (receiveData.UnreadLength() >= 4 && _packetLen <= 0)
                {
                    _packetLen = receiveData.ReadInt();
                }
            }

            return (_packetLen <= 0);
        }
    }
}
