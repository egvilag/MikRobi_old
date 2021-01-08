using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MikRobi2
{
    static class Config
    {
        static string configFile = Program.path + @"/mikrobi.cfg";

        // Give values to config variables
        public static void ReadConfig()
        {
            try
            {
                Log.maxLogSize = long.Parse(ReadConfig2(configFile, "MaxLogSize"));
                Listener.launcherPort = Int32.Parse(ReadConfig2(configFile, "LauncherPort"));
                SQL.dbServer = ReadConfig2(configFile, "SQL-Server");
                SQL.dbPort = ReadConfig2(configFile, "SQL-Port");
                SQL.dbUser = ReadConfig2(configFile, "SQL-User");
                SQL.dbPassword = ReadConfig2(configFile, "SQL-Password");
                SQL.dbDatabase = ReadConfig2(configFile, "SQL-Database");

            }
            catch (Exception e)
            {
                Console.WriteLine("Nem olvasható a config fájl!" + "(" + e.Message + ")");
                Environment.Exit(0);
            }
        }

        // Low level function to read the config file
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
                Console.WriteLine("Nem olvasható a fájl: " + filename + "(" + e.Message + ")");
                Environment.Exit(0);
            }
            return value;
        }

    }
}
