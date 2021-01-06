using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MikRobi2
{
    static class Listener
    {
        public static int launcherPort;

        static TcpListener listener;
        static Thread thFigyel;
        public static List<LauncherSession> launcherSessions = new List<LauncherSession>();

        public static void StartLstening()
        {
            listener = new TcpListener(IPAddress.Any, launcherPort);
            listener.Start();
            thFigyel = new Thread(() => tFigyel());
            thFigyel.Start();
        }

        public static void StopListening()
        {
            
            thFigyel.Abort();
            thFigyel = null;
            listener.Stop();
            listener = null;
            foreach (LauncherSession lc in launcherSessions)
                lc.CloseCon();
        }

        static void tFigyel()
        {
            TcpClient kliens;
            LauncherSession lc;
            string clientaddress;
            string serveraddress = IPAddress.Parse(((IPEndPoint)listener.LocalEndpoint).Address.ToString())
                + ":" + ((IPEndPoint)listener.LocalEndpoint).Port.ToString();
            while (true)
            {
                try
                {
                    kliens = listener.AcceptTcpClient();
                    clientaddress = IPAddress.Parse(((IPEndPoint)kliens.Client.RemoteEndPoint).Address.ToString())
                    + ":" + ((IPEndPoint)kliens.Client.RemoteEndPoint).Port.ToString();
                    Log.WriteLog("Új bejövő kapcsolat észlelve: " + clientaddress, true);
                    lc = new LauncherSession();
                    lc.kliens = kliens;
                    //lc.kliens.ReceiveTimeout = 5000;
                    lc.address = clientaddress;
                    lc.connected = true;
                    lc.lastActivity = DateTime.Now;
                    if (!SQL.CheckBlacklist(clientaddress)) lc.Start();
                    else lc.CloseCon();
                }
                catch (Exception e)
                { Log.WriteLog("Hiba a kliensfigyeléskor: " + e.Message, true); }

            }
        }
        }
}
