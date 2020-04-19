using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Renci.SshNet;

namespace MikRobi
{
    class Program
    {
        static string ProgramName = "ÉGvilág Mikrobi v0.1";

        static SSH ssh;

        static void Main(string[] args)
        {
            Console.WriteLine(ProgramName);
            Console.WriteLine("Spurikutya (c) 2020.");
            Console.WriteLine();
            Thread cli = new Thread(ReadCLICommand);
            cli.Start();
            ssh = new SSH();
            //ssh.sshClients.Add(new SshClient("web.egvilag.net", "mc-private", "Sw2FfJ"));
            //ssh.Connect();

            ssh.clients.Add(new Client("192.168.0.92", "mc-private", "Sw2FfJ"));
            ssh.clients[0].Connect();
        }
#region CLI
        static void ReadCLICommand()
        {
            while (true)
            {
                string cmd = "";
                do
                {
                    cmd = Console.ReadLine();
                } while (ExecuteCommand(cmd));
                Console.WriteLine("Kilépés...");
                break;
            }
            Console.WriteLine("Bye.");
        }

        static bool ExecuteCommand(string command)
        {
            bool contWork = true; ;
            string[] splittedCommand = command.Split(' ');

            switch (splittedCommand[0].ToLower())
            {
                // Segítség
                case "segítség":
                case "segitseg":
                case "?": ShowHelp(); break;
                // Szerver leállítás
                case "stop":
                case "s": StopServer(); contWork = false; break;
      

                // Kliens státusz
                case "stat":
                case "statusz": ClientStatus(); break;
                
                case "kuld" : 
                    if (splittedCommand.Count() > 2)
                    {
                        for (int i = 2; i < splittedCommand.Count(); i++)
                            splittedCommand[1] += " " + splittedCommand[i];
                    }
                    Send(splittedCommand[1]);
                    break;

                default: Console.WriteLine("Ismeretlen parancs."); Console.WriteLine(); break;
            }
            return contWork;
        }

        static void ShowHelp()
        {
            Console.WriteLine();
            Console.WriteLine("MikRobi helyi parancsainak listája:");
            Console.WriteLine();
            Console.WriteLine("?, Segítség      -- Ennek a listának a megjelenítése");
            Console.WriteLine("S, Stop          -- Szerver leállítása");                // 
            Console.WriteLine("UU, ÜtemÚjra     -- Ütemezőlista újraolvasása");         // kidolgozandó

            Console.WriteLine();
        }

#endregion

        static void StopServer()
        {
            Console.WriteLine("Szerver leállítása...");
            ssh.clients[0].Disconnect();
        }

        static void ClientStatus()
        {
            int i = 0;
            foreach (Client c in ssh.clients)
            {
                Console.WriteLine(i + " : " + c.status);
                i++;
            }
        }

        static void Send(string command)
        {
            ssh.clients[0].Send(command);
        }
    }
}
