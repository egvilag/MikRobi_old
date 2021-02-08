using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Windows.Forms;

namespace EgvilagLauncher
{
    class TCP
    {
        bool connected;
        bool checking;
        TcpClient kliens;
        StreamReader r;
        StreamWriter w;
        Thread tFigyel;
        Thread tSFigyel;
        Thread tKuld;
        //System.Timers.Timer timeoutTimer;
        System.Timers.Timer heartBeat;
        bool timeout;
        string message = "";
        FormSplash FS;
        FormMain FM;

        public TCP(FormSplash FormSplash, FormMain FormMain)
        {
            FS = FormSplash;
            FM = FormMain;
            kliens = new TcpClient();
            connected = false;
            while (!connected)
            {
                try
                {
                    kliens.Connect("127.0.0.1", 8888);
                    connected = true;
                }
                catch { ; }
            }
            r = new StreamReader(kliens.GetStream(), Encoding.Default);
            w = new StreamWriter(kliens.GetStream(), Encoding.Default);
            heartBeat = new System.Timers.Timer();
            heartBeat.Elapsed += new System.Timers.ElapsedEventHandler(IsAlive);
            heartBeat.Interval = 5000;
            heartBeat.AutoReset = true;
            heartBeat.Start();
            //timeoutTimer = new System.Timers.Timer();
            //timeoutTimer.Elapsed += new System.Timers.ElapsedEventHandler(TimeoutOccured);
            //timeoutTimer.Interval = 60000;
            //timeoutTimer.AutoReset = false;
            //MessageBox.Show("Kapcsolat felépült.");
            //message = "CONN$" + "teszt" + "$" + "hess";
            //SendMessage(message);
            tFigyel = new Thread(Figyel);
            tFigyel.Start();

        }

        public void SendMessage(string message)
        {
            this.message = message;
            tKuld = new Thread(tSend);
            tKuld.Start();
        }

        void tSend()
        {
            try
            {
                w.WriteLine(message);
                w.Flush();
            }
            catch
            {
                return;
            }
        }

        void Figyel()
        {
            string message = "";
            bool loggedNoPing = false;
            while (true)
            {
                try
                {
                    if (!checking)
                    {
                        message = r.ReadLine();
                        //Console.WriteLine("Bejövő üzenet: " + message);
                    }
                }
                catch
                {
                    if (!loggedNoPing)
                    {
                        //Console.WriteLine("Megszakadt a kapcsolat...");
                        loggedNoPing = true;
                    }
                    //tFigyel.Abort();
                }
                Console.WriteLine(message);
                if (message == "PING")
                {
                    w.WriteLine("PONG");
                    w.Flush();
                }
            }
        }




        void IsAlive(object source, System.Timers.ElapsedEventArgs e)
        {
            heartBeat.Stop();
            checking = true;
            bool loggedNoPing = false;
            //timeoutTimer.Start();
            while (checking)
            {
                if (!SocketConnected(kliens.Client))
                {
                    //heartBeat.Stop();
                    //Program.ClientDisconnected(this);
                    Thread.Sleep(500);
                    if (checking && !loggedNoPing)
                    {
                        //Console.WriteLine("[INFO] Nincs PING? Próbálkozom... (" + ((IPEndPoint)kliens.Client.RemoteEndPoint).Address + ":" + ((IPEndPoint)kliens.Client.RemoteEndPoint).Port + ")");
                        loggedNoPing = true;
                    }
                }
                else
                {
                    heartBeat.Start();
                    checking = false;
                    loggedNoPing = false;
                    //timeoutTimer.Stop();
                    //Console.WriteLine("[INFO] PING: (" + ((IPEndPoint)kliens.Client.RemoteEndPoint).Address + ":" + ((IPEndPoint)kliens.Client.RemoteEndPoint).Port + ")");
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

        //void TimeoutOccured(object source, System.Timers.ElapsedEventArgs e)
        //{
        //    timeout = true;
        //    timeoutTimer.Stop();
        //    kliens.Close();
        //    kliens = null;
        //    //("Újra be kell jelentkezni...");
        //    //heartBeat.Stop();
        //    //pingTimeWatcher.Stop();
        //    //Program.ClientDisconnected(this);
        //}
    }
   
}
