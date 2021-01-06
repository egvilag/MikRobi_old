using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MikRobi2
{
    static class CLI
    {
        static Thread thCLI;

        public static void StartCLI()
        {
            thCLI= new Thread(ReadCLICommand);
            thCLI.Start();
        }

        static void ReadCLICommand()
        {
            while (true)
            {
                string cmd = "";
                do
                {
                    cmd = Console.ReadLine();
                }
                while (ExecuteCommand(cmd));
                break;
            }
        }

        public static bool ExecuteCommand(string command)
        {
            Log.WriteLog(command, false);
            bool contWork = true;
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
            }
            return contWork;
        }

        static void ShowHelp()
        {
            Console.WriteLine();
            Console.WriteLine("MikRobi helyi parancsainak listája:");
            Console.WriteLine();
            Console.WriteLine("?, Segítség                      -- Ennek a listának a megjelenítése");
            Console.WriteLine("S, Stop                          -- Szerver leállítása");
            Console.WriteLine();
        }

        static void StopServer()
        {
            Log.WriteLog("Szerver leállítása...", true);
            Listener.StopListening();
            //thRadio.Abort();
            //thSetOffline.Abort();
            Log.CloseLogFile();
        }
    }
}
