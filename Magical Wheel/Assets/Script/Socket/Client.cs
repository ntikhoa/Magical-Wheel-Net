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
        tcp = new TCP();
    }

    public void ConnectedToServer()
    {
        tcp.Connect();
    }

    public class TCP
    {
        public TcpClient socket;
        public NetworkStream stream;
        public byte[] receiveBuffer;

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

        private void ConnectCallBack(IAsyncResult _res)
        {
            Debug.Log("Connect Call Back");
            socket.EndConnect(_res);
            if (!socket.Connected)
            {
                return;
            }
            stream = socket.GetStream();
            Debug.Log("Begin Read");
            stream.BeginRead(receiveBuffer, 0, dataSize, ReceiveCallBack, null);
        }

        private void ReceiveCallBack(IAsyncResult _res)
        {
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
                stream.BeginRead(receiveBuffer, 0, dataSize, ReceiveCallBack, null);
            }
            catch
            {
                Debug.LogError("Error Connection Exception");
            }
        }
    }
}
