using System;
using System.Reflection;
using System.Threading;
using System.IO;


namespace MikRobi2
{
    class Program
    {
        // Program name with actual major.minor.build.revision number
        static string ProgramName = "ÉGvilág Mikrobi v" + Assembly.GetEntryAssembly().GetName().Version;
        // Full path to binary
        public static string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        static void Main(string[] args)
        {
            Console.WriteLine(ProgramName);
            Console.WriteLine("Spurikutya (c) 2019-2021.");
            Console.WriteLine();
            // Initialize sub-units
            Config.ReadConfig();
            Log.CreateLogFile();
            if (SQL.MakeConnection())
            {
                Listener.StartListening();
                CLI.StartCLI();
            }
        }
    }
}
