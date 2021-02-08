using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Renci.SshNet;
using Renci.SshNet.Common;
using WinSCP;
using System.IO;

namespace MikRobi
{
    class Client
    {
        public SshClient sshClient;
        private ShellStream sshStream;
        private StreamReader reader;
        private StreamWriter writer;
        public Thread thread;

        LogFile logFile;

        public string status; // disconnected, connecting, connected
        public Client(string host, string username, string password)
        {
            sshClient = new SshClient(host, username, password);
            logFile = new LogFile(host + ".log");
        }

        public void Connect()
        {
            status = "connecting";
            thread = new Thread(() => Connect2(sshClient));
            thread.Start();
        }

        public void Disconnect()
        {
            sshClient.Disconnect();
            status = "disconnected";
        }

        public void Send(string command)
        {
            //status = "sending command";
            thread = new Thread(() => Send2(sshClient, command));
            thread.Start();
        }

        void Connect2(SshClient client)
        {
            try 
            {
                var terminalMode = new Dictionary<TerminalModes, uint>();
                terminalMode.Add(TerminalModes.ECHO, 53);
                client.Connect();
                sshStream = sshClient.CreateShellStream("term", 80, 60, 800, 600, 65535, terminalMode);
                reader = new StreamReader(sshStream, Encoding.UTF8, true, 1024, true);
                writer = new StreamWriter(sshStream) { AutoFlush = true };
            }
            catch { status = "disconnected"; return; }
            status = "connected";
            //client.RunCommand("mkdir ~/mikrobika");
            //client.Disconnect();  
        }

        void Send2(SshClient client, string command)
        {
            try 
            { 
                if (status == "connected")
                {
                    status = "receiving";
                    writer.WriteLine(command.Trim());
                    while (sshStream.Length == 0)
                    {
                        Thread.Sleep(500);
                    }
                    ThreadStart threadStart = ReceiveData;
                    Thread thread = new Thread(threadStart) { IsBackground = true };
                    thread.Start();
                }
            }
            catch (Exception e)
            {
                ;
            }
        }

        private void ReceiveData()
        {
            while (status != "disconnected")
            {
                try
                {
                    if (reader != null)
                    {
                        StringBuilder result = new StringBuilder();

                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            result.AppendLine(line);
                        }

                        if (!string.IsNullOrEmpty(result.ToString()))
                        {
                            // TODO - Parse data at this point
                            //Console.WriteLine(result.ToString());
                            logFile.Write(result.ToString());
                        }
                        
                        status = "connected";
                        //break;
                    }
                }
                catch (Exception e)
                {
                    // TODO
                    Console.WriteLine(e);
                }

                Thread.Sleep(200);
            }
        }
    }

    class SSH
    {
        public List<Client> clients = new List<Client>();

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
