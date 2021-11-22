using System;
using System.Net.Sockets;

namespace Server
{
    class Client
    {
        public static int dataBufferSize = 4096;

        public int id;

        public Player player;

        public Client(int clientId)
        {
            id = clientId;
            player = new Player(-1, "");
        }

        public TcpClient socket;

        //private NetworkStream stream;
        private byte[] receiveBuffer;
        private Packet receivedData;

        public void Connect(TcpClient _socket)
        {
            socket = _socket;
            socket.ReceiveBufferSize = dataBufferSize;
            socket.SendBufferSize = dataBufferSize;

            //stream = socket.GetStream();

            receivedData = new Packet();
            receiveBuffer = new byte[dataBufferSize];

            //stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            //Non Blocking
            socket.Client.BeginReceive(receiveBuffer, 0, dataBufferSize,SocketFlags.None, ReceiveCallback, null);
            Console.WriteLine($"Welcome { socket.Client.RemoteEndPoint} as player {id}");
            ServerSender.Welcome(id, PktMsg.WELCOME);
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                //int byteLength = stream.EndRead(result);
                //Non Blocking
                int byteLength = socket.Client.EndReceive(result);
                if (byteLength <= 0)
                {
                    return;
                }

                byte[] data = new byte[byteLength];
                Array.Copy(receiveBuffer, data, byteLength);

                receivedData.Reset(HandleData(data));
                //handle data
                //stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                //Non Blocking
                socket.Client.BeginReceive(receiveBuffer, 0, dataBufferSize, SocketFlags.None, ReceiveCallback, null);

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error receiving TCP data: {e}");
                if (socket != null)
                {
                    socket.Close();
                }
                socket = null;
                player.id = -1;
                player.username = "";

                ServerSender.WelcomeAll(PktMsg.WELCOME);

                GameLogic.SetState(STATE.Waiting_Player);
            }
        }

        public void SendData(Packet packet)
        {
            try
            {
                if (socket != null)
                {
                    //stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                    //Non Blocking
                    socket.Client.BeginSend(packet.ToArray(), 0, packet.Length(), SocketFlags.None, null, null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error sending data to player {id} via TCP: {e}");
                if (socket != null)
                {
                    socket.Close();
                }
                socket = null;
                player.id = -1;
                player.username = "";

                ServerSender.WelcomeAll(PktMsg.WELCOME);

                GameLogic.SetState(STATE.Waiting_Player);
            }
        }

        private bool HandleData(byte[] data)
        {
            bool _readlen = false;
            int _packetLen = 0;
            receivedData.SetBytes(data);
            while (receivedData.UnreadLength() >= 4 && _packetLen <= 0)
            {
                _packetLen = receivedData.ReadInt();
                _readlen = true;
            }

            while (_packetLen > 0 && _packetLen <= receivedData.UnreadLength())
            {
                byte[] _packetByte = receivedData.ReadBytes(_packetLen);
                ThreadManager.ExecuteOnMainThread(() => {
                    using (Packet _p = new Packet(_packetByte))
                    {
                        int _pId = _p.ReadInt();
                        Server.packetHandlers[_pId](id, _p);
                        _readlen = false;
                    }
                });

                _packetLen = 0;
                while (receivedData.UnreadLength() >= 4 && _packetLen <= 0)
                {
                    _packetLen = receivedData.ReadInt();
                    _readlen = true;
                }
            }

            return (!_readlen);
        }
    }
}
