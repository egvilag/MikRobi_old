using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Renci.SshNet;
using WinSCP;

namespace MikRobi
{
    class SSH
    {
        public List<SessionOptions> sessions = new List<SessionOptions>();
        public List<SshClient> sshClients = new List<SshClient>();
        List<Thread> threads = new List<Thread>();

        public void Connect()
        {
            foreach (SshClient client in sshClients)
            {
                threads.Add(new Thread(() => Connect2(client)));
                threads.Last().Start();
            }
        }

        void Connect2(SshClient client)
        {
            client.Connect();
            client.RunCommand("mkdir ~/mikrobika");
            client.Disconnect();
        }
    }
}
