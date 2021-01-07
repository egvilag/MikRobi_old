using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

namespace MikRobi2
{
    static class Listener
    {
        public static int launcherPort;

        static Socket serverSocket;
        static List<Socket> clientSockets = new List<Socket>();
        static byte[] buffer = new byte[1024];

        private static void RemoveSocket(Socket socket)
        {
            for (int i = 0; i < clientSockets.Count; i++)
            {
                if (clientSockets[i].Handle == socket.Handle)
                    clientSockets.RemoveAt(i);
            }
            return;
        }

        public static void StartListening()
        {
            try
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(new IPEndPoint(IPAddress.Any, launcherPort));
                serverSocket.Listen(10);
                serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            }
            catch (Exception e)
            {
                Log.WriteLog("Hiba a figyelésnél: " + e.Message, true);
            }
        }

        static void AcceptCallback(IAsyncResult AR)
        {
            Socket Socket = serverSocket.EndAccept(AR);
            try
            {
                clientSockets.Add(Socket);
                //buffer = new byte[clientSocket.ReceiveBufferSize];
                Socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), Socket);
                serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);

                Log.WriteLog("Kliens csatlakozva: " + Socket.RemoteEndPoint.ToString(), true);
            }
            catch (Exception e)
            {
                Log.WriteLog("Hiba a kapcsolódásnál: " + e.Message, true);
                Socket.Close();
                RemoveSocket(Socket);
            }
        }

        static void ReceiveCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            try
            {
                int received = socket.EndReceive(AR);

                if (received == 0)
                {
                    Log.WriteLog("Kliens lecsatlakozott: " + socket.RemoteEndPoint.ToString(), true);
                    RemoveSocket(socket);
                    return;
                }

                byte[] dataBuff = new byte[received];
                Array.Copy(buffer, dataBuff, received);

                string text = Encoding.ASCII.GetString(dataBuff);

                if (text=="exit")
                    socket.Close();

                Console.WriteLine(text);
                
                SendData(socket, buffer);
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            }
            catch (Exception e)
            {
                Log.WriteLog("Hiba a fogadásnál: " + e.Message, true);
                socket.Close();
                RemoveSocket(socket);
            }
        }

        static void SendCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }

        static void SendData(Socket sock, byte[] data)
        {
            sock.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), sock);
        }

        public static void StopListening()
        {
            foreach (Socket s in clientSockets)
                s.Close();
            clientSockets.Clear();
            serverSocket.Close();
        }
    }
}
