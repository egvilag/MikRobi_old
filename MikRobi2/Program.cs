using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Reflection;
using System.Threading;


namespace MikRobi2
{
    class Program
    {
        static string ProgramName = "ÉGvilág Mikrobi v" + Assembly.GetEntryAssembly().GetName().Version;

        public static string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        static string configFile = path + @"/mikrobi.cfg";

        public static int launcherPort;

        static void Main(string[] args)
        {
            Console.WriteLine(ProgramName);
            Console.WriteLine("Spurikutya (c) 2019-2021.");
            Console.WriteLine();
            Log.CreateLogFile();
            ReadConfig();
        }

        static void ReadConfig()
        {
            try
            {
                Log.maxLogSize = long.Parse(ReadConfig2(configFile, "MaxLogSize"));
                launcherPort = Int32.Parse(ReadConfig2(configFile, "LauncherPort"));
                SQL.dbServer = ReadConfig2(configFile, "SQL-Server");
                SQL.dbPort = ReadConfig2(configFile, "SQL-Port");
                SQL.dbUser = ReadConfig2(configFile, "SQL-User");
                SQL.dbPassword = ReadConfig2(configFile, "SQL-Password");
                SQL.dbDatabase = ReadConfig2(configFile, "SQL-Database");

            }
            catch (Exception e)
            {
                Log.WriteLog("Nem olvasható a config fájl!", true);
                Log.CloseLogFile();
                Environment.Exit(0);
            }
        }

        static string ReadConfig2(string filename, string key)
        {
            string value = "";
            string line;
            try
            {
                StreamReader sr = new StreamReader(filename);
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (line.Split('=')[0] == key) value = line.Split('=')[1];
                }
                sr.Close();
                sr = null;
            }
            catch (Exception e)
            {
                Log.WriteLog("Nem olvasható a fájl: " + filename, true);
                Log.CloseLogFile();
                Environment.Exit(0);
            }
            return value;
        }

        
    }
}
