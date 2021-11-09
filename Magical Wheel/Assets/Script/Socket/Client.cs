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

    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;


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
        tcp.Connect();
    }

    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.welcome, ClientHandle.Welcome }
        };
        Debug.Log("Initialized packets");
    }

    public void Disconnect()
    {
        if (tcp.socket.Connected)
        {
            tcp.socket.Close();
            Debug.Log("Close Connection");
        }
    }

    public class TCP
    {
        public TcpClient socket;

        private NetworkStream stream;
        private Packet receiveData;
        private byte[] receiveBuffer;

        public void Connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = dataSize,
                SendBufferSize = dataSize

            };
            receiveBuffer = new byte[dataSize];
            Debug.Log("Begin Connect");
            socket.BeginConnect(instance.ip, instance.port, ConnectCallBack, socket);
        }

        public void SendData(Packet _packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to server via TCP: {_ex}");
            }
        }

        private void ConnectCallBack(IAsyncResult _res)
        {
            Debug.Log("Connect Call Back");
            socket.EndConnect(_res);
            if (!socket.Connected)
            {
                return;
            }
            stream = socket.GetStream();
            receiveData = new Packet();
            Debug.Log("Begin Read");
            stream.BeginRead(receiveBuffer, 0, dataSize, ReceiveCallBack, null);
        }

        private void ReceiveCallBack(IAsyncResult _res)
        {
            Debug.Log("Receive Call Back");
            try
            {
                int _bytelength = stream.EndRead(_res);
                if(_bytelength == 0)
                {
                    Debug.Log("Length = 0");
                    return;
                }

                byte[] _data = new byte[_bytelength];
                Array.Copy(receiveBuffer, _data, _bytelength);
                receiveData.Reset(CheckInvalidData(_data));
                stream.BeginRead(receiveBuffer, 0, dataSize, ReceiveCallBack, null);
            }
            catch
            {
                if (socket.Connected)
                {
                    socket.Close();
                }
                Debug.LogError("Error Connection Exception");
            }
        }

        private bool CheckInvalidData(byte[] _data)
        {
            int _packetLen = 0;
            receiveData.SetBytes(_data);
            if(receiveData.UnreadLength() >= 4)
            {
                _packetLen = receiveData.ReadInt();
                if(_packetLen <= 0)
                {
                    return true;
                }
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
                if (receiveData.UnreadLength() >= 4)
                {
                    _packetLen = receiveData.ReadInt();
                    if (_packetLen <= 0)
                    {
                        return true;
                    }
                }
            }

            return (_packetLen <= 1);
        }
    }
}
