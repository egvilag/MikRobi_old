using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace EGvilagServerCLI2
{
    class Client
    {
        public string name;
        public IPEndPoint ep;
        public TcpClient kliens;
        Thread Tfogad;
        StreamWriter w;
        StreamReader r;
        string user;
        string uzenet;
        long ping;
        bool timeout;
        bool connected;
        bool checking;
        System.Timers.Timer heartBeat;
        System.Timers.Timer timeoutTimer;
        Stopwatch pingTimeWatcher;

        public Client(TcpClient tcpkliens)
        {
            kliens = tcpkliens;
            ep = new IPEndPoint(((IPEndPoint)kliens.Client.RemoteEndPoint).Address, ((IPEndPoint)kliens.Client.RemoteEndPoint).Port);
            r = new StreamReader(kliens.GetStream(), Encoding.Default);
            w = new StreamWriter(kliens.GetStream(), Encoding.Default);
            pingTimeWatcher = new Stopwatch();
            heartBeat = new System.Timers.Timer();
            //heartBeat.Elapsed += new System.Timers.ElapsedEventHandler(PingIt);
            heartBeat.Elapsed += new System.Timers.ElapsedEventHandler(IsAlive);
            heartBeat.Interval = Program.pingInterval;
            heartBeat.AutoReset = true;
            heartBeat.Start();
            timeoutTimer = new System.Timers.Timer();
            timeoutTimer.Elapsed += new System.Timers.ElapsedEventHandler(TimeoutOccured);
            timeoutTimer.Interval = Program.disconnectAfter; //pingtimeout volt előtte, 3mp. Így most 1 perc után szétkapcsol.
            timeoutTimer.AutoReset = false;

            
            Tfogad = new Thread(Fogad);
            Tfogad.Start();
        }

        //private void PingIt(object source, System.Timers.ElapsedEventArgs e)
        //{
        //    timeout = false;
        //    heartBeat.Stop();
        //    //timeoutTimer.Start();
        //    pingTimeWatcher.Reset();
        //    pingTimeWatcher.Start();
        //    long ping;
        //    try
        //    {
        //        w.WriteLine("PING");
        //        w.Flush();
        //    }
        //    catch
        //    {
        //        timeout = true;
        //        Program.ClientDisconnected(this);
        //    }


        //    Fogad();
        //    if (timeout) return;
        //    pingTimeWatcher.Stop();
        //    //timeoutTimer.Stop();
        //    ping = pingTimeWatcher.ElapsedMilliseconds;
        //    Program.WriteLog("[INFO] PING: " + ping + "ms");
        //    heartBeat.Start();
        //}

        void Fogad()
        {
            string message;
            try
            {
                while ((message = r.ReadLine()) != "")
                {
                    if (message.Length > 1)
                        if (!Program.ProcessMessage(message.Trim().Replace("\0", "")))
                            Program.WriteLog("[ERROR] Az üzenet nem érvényes: " + message);
                    //Program.WriteLog("[INFO] Bejövő üzenet: " + message);
                }
                //string message = r.ReadLine();
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                Program.WriteLog("[ERROR] Hiba az adatok olvasása közben!");
            }
        }

        void TimeoutOccured(object source, System.Timers.ElapsedEventArgs e)
        {
            //timeout = true;
            checking = false;
            timeoutTimer.Stop();
            heartBeat.Stop();
            pingTimeWatcher.Stop();
            Program.ClientDisconnected(this);
        }










        void IsAlive(object source, System.Timers.ElapsedEventArgs e)
        {
            heartBeat.Stop();
            pingTimeWatcher.Reset();
            pingTimeWatcher.Start();
            checking = true;
            bool loggedNoPing = false;
            timeoutTimer.Start();
            while (checking)
            {
                if (!SocketConnected(this.kliens.Client))
                {
                    Thread.Sleep(500);
                    if (checking && !loggedNoPing)
                    {
                        Program.WriteLog("[INFO] Nincs PING? Próbálkozom... (" + ((IPEndPoint)this.kliens.Client.RemoteEndPoint).Address + ":" + ((IPEndPoint)kliens.Client.RemoteEndPoint).Port + ")");
                        loggedNoPing = true;
                    }
                }
                else
                {
                    pingTimeWatcher.Stop();
                    heartBeat.Start();
                    checking = false;
                    loggedNoPing = false;
                    timeoutTimer.Stop();
                    Program.WriteLog("[INFO] PING: " + pingTimeWatcher.ElapsedMilliseconds + "ms (" + ((IPEndPoint)this.kliens.Client.RemoteEndPoint).Address + ":" + ((IPEndPoint)kliens.Client.RemoteEndPoint).Port + ")");
                }
            }
        }

        bool SocketConnected(Socket s)
        {
            // Exit if socket is null
            if (s == null)
                return false;
            if ((s.Poll(1000, SelectMode.SelectRead)) && (s.Available == 0))
                return false;
            else
            {
                try
                {
                    return s.Send(new byte[1], 1, 0) == 1;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
