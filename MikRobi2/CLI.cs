using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Reflection;

namespace MikRobi2
{
    static class CLI
    {
        static Thread thCLI;

        // Start new thread for processing command line instructions
        public static void StartCLI()
        {
            thCLI= new Thread(ReadCLICommand);
            thCLI.Start();
        }

        // Listen for console commands until stop signal
        static void ReadCLICommand()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("MikRobi v" + Assembly.GetEntryAssembly().GetName().Version);
            for (int i = Console.CursorLeft; i<Console.WindowWidth; i++)
                Console.Write(" ");
            Console.WriteLine();
            ChangeMenu(1);
            while (true)
            {
                bool work = true;
                string cmd = "";
                ConsoleKey ck;
                do
                {
                    //cmd = Console.ReadLine();
                    ck = Console.ReadKey().Key;
                    if (ck == ConsoleKey.Enter) work = ExecuteCommand(cmd);
                    else 
                        if ((ck != ConsoleKey.F1) && (ck != ConsoleKey.F2) && (ck != ConsoleKey.F3) && (ck != ConsoleKey.F4) &&
                                (ck != ConsoleKey.F5) && (ck != ConsoleKey.F6) && (ck != ConsoleKey.F7) && (ck != ConsoleKey.F8) &&
                                (ck != ConsoleKey.F9) && (ck != ConsoleKey.F10) && (ck != ConsoleKey.F11) && (ck != ConsoleKey.F12))
                                    cmd += ck.ToString();
                        else
                            switch (ck)
                            {
                                case ConsoleKey.F1: ChangeMenu(1); break;
                                case ConsoleKey.F2: ChangeMenu(2); break;
                        }
                }
                //while (ExecuteCommand(cmd));
                while (work) ;
                break;
            }
        }

        static void ChangeMenu(int id)
        {
            Console.CursorLeft = 0;
            Console.CursorTop = 1;
            if (id == 1)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("1");
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(" CLIENTS");
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("1");
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write(" CLIENTS");
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" ");
            if (id == 2)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("2");
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(" BLABLA");
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("2");
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write(" BLABLA");
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" ");
        }

        // Process command
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

        // Show list of available commands
        static void ShowHelp()
        {
            Console.WriteLine();
            Console.WriteLine("MikRobi helyi parancsainak listája:");
            Console.WriteLine();
            Console.WriteLine("?, Segítség                      -- Ennek a listának a megjelenítése");
            Console.WriteLine("S, Stop                          -- Szerver leállítása");
            Console.WriteLine();
        }

        // Stop TCP server
        static void StopServer()
        {
            Log.WriteLog("Szerver leállítása...", true);
            //Listener.StopListening();
            //thRadio.Abort();
            //thSetOffline.Abort();
            Log.CloseLogFile();
        }
    }
}
