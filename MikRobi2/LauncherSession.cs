using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MikRobi2
{
    class LauncherSession
    {
        public TcpClient kliens;

        public string address;
        public bool connected;
        public DateTime lastActivity;

        public void Start()
        {
            Listener.launcherSessions.Add(this);

            ////ListenConnection = true;
            //this.thFogad = new Thread(tFogad);
            //this.thFogad.Start();
        }

        public void CloseCon()
        {
            Listener.launcherSessions.Remove(this);
            try { kliens.Close(); } catch { }
            //try { r.Close(); } catch { }
            //try { w.Close(); } catch { }
            //try { clientLog.Close(); } catch { }
            //thFogad.Abort();
        }
    }
}
