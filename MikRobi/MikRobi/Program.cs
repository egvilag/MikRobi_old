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

        static void Main(string[] args)
        {
            Console.WriteLine(ProgramName);
            Console.WriteLine("Spurikutya (c) 2020.");
            Console.WriteLine();
            Thread cli = new Thread(ReadCLICommand);
            cli.Start();
            SSH ssh = new SSH();
            ssh.sshClients.Add(new SshClient("web.egvilag.net", "mc-private", "Sw2FfJ"));
            ssh.Connect();
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
                Console.WriteLine("Exit program...");
                break;
            }
            Console.WriteLine("Bye.");
        }

        static bool ExecuteCommand(string command)
        {
            bool contWork = true; ;
            switch (command.ToLower())
            {
                // Segítség
                case "segítség":
                case "segitseg":
                case "?": ShowHelp(); break;
                // Szerver leállítás
                case "stop":
                case "s": StopServer(); contWork = false; break;
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
            Console.WriteLine("S, Stop          -- Szerver leállítása");

            Console.WriteLine();
        }

#endregion

        static void StopServer()
        {
            Console.WriteLine("Szerver leállítása...");
        }
    }
}
