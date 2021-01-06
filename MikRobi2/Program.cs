using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Reflection;


namespace MikRobi2
{
    class Program
    {
        static string ProgramName = "ÉGvilág Mikrobi v" + Assembly.GetEntryAssembly().GetName().Version;

        public static string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        static string configFile = path + @"/mikrobi.cfg";

        public static long maxLogSize;

        public static string dbServer;
        public static string dbPort;
        public static string dbUser;
        public static string dbPassword;
        public static string dbDatabase;

        public static int launcherPort;

        static void Main(string[] args)
        {
            Console.WriteLine(ProgramName);
            Console.WriteLine("Spurikutya (c) 2019-2021.");
            Console.WriteLine();
            ReadConfig();
        }

        static void ReadConfig()
        {
            try
            {
                maxLogSize = long.Parse(ReadConfig2(configFile, "MaxLogSize"));
                launcherPort = Int32.Parse(ReadConfig2(configFile, "LauncherPort"));
                dbServer = ReadConfig2(configFile, "SQL-Server");
                dbPort = ReadConfig2(configFile, "SQL-Port");
                dbUser = ReadConfig2(configFile, "SQL-User");
                dbPassword = ReadConfig2(configFile, "SQL-Password");
                dbDatabase = ReadConfig2(configFile, "SQL-Database");

            }
            catch (Exception e)
            {
                //WriteLog("Nem olvasható a config fájl!", true);
                //CloseLogFile();
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
                //WriteLog("Nem olvasható a fájl: " + filename, true);
                //CloseLogFile();
                Environment.Exit(0);
            }
            return value;
        }
    }
}
