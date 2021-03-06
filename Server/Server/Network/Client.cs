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
                    socket.Client.BeginReceive(receiveBuffer, 0, dataBufferSize, SocketFlags.None, ReceiveCallback, null);
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
            catch (SocketException ex)
            {
                SocketError s_er = ex.SocketErrorCode;
                if (s_er == SocketError.WouldBlock)
                {
                    socket.Client.BeginSend(packet.ToArray(), 0, packet.Length(), SocketFlags.None, null, null);
                }
                else
                {
                    Console.WriteLine($"Error sending data to player {id} via TCP: {ex}");
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
            int packetLength = 0;

            receivedData.SetBytes(data);

            if (receivedData.UnreadLength() >= 4)
            {
                // If client's received data contains a packet
                packetLength = receivedData.ReadInt();
                if (packetLength <= 0)
                {
                    // If packet contains no data
                    return true; // Reset receivedData instance to allow it to be reused
                }
            }

            while (packetLength > 0 && packetLength <= receivedData.UnreadLength())
            {
                // While packet contains data AND packet data length doesn't exceed the length of the packet we're reading
                byte[] _packetBytes = receivedData.ReadBytes(packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        Server.packetHandlers[_packetId](id, _packet); // Call appropriate method to handle the packet
                    }
                });

                packetLength = 0; // Reset packet length
                if (receivedData.UnreadLength() >= 4)
                {
                    // If client's received data contains another packet
                    packetLength = receivedData.ReadInt();
                    if (packetLength <= 0)
                    {
                        // If packet contains no data
                        return true; // Reset receivedData instance to allow it to be reused
                    }
                }
            }

            if (packetLength <= 1)
            {
                return true; // Reset receivedData instance to allow it to be reused
            }

            return false;
        }
    }
}
