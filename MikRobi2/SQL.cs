﻿using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace MikRobi2
{
    static class SQL
    {
        static string connstr;
        static MySqlConnection myConn;

        public static string dbServer;
        public static string dbPort;
        public static string dbUser;
        public static string dbPassword;
        public static string dbDatabase;

        // Make database connection with the actual credentials
        public static void MakeConnection()
        {
            connstr = "Server=" + dbServer + ";Port=" + dbPort + ";Database=" + dbDatabase + ";Uid=" + dbUser + ";Pwd=" + dbPassword + ";CharSet=utf8;";
            myConn = new MySqlConnection(connstr);
        }

        // Execute SQL command and return affected row count
        static long SQLCommandCount(string command)
        {
            long result = -1;
            myConn = new MySqlConnection(connstr);
            MySqlCommand cmd = new MySqlCommand(command, myConn);
            try
            {
                myConn.Open();
                result = Convert.ToInt64(cmd.ExecuteScalar());
                myConn.Close();
            }
            catch (Exception e)
            {
                Log.WriteLog("SQLCommandCount: " + e.Message, true);
                myConn.Close();
            }
            return result;
        }

        // Look for IP ban in the database
        public static bool CheckBlacklist(string IP)
        {
            string command = "SELECT ID COUNT FROM blacklist WHERE IP='"
                + IP + "' AND "
                + "Datestamp<'" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day
                + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "' AND "
                + "Expire>'" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day
                + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "'";
            switch (SQLCommandCount(command))
            {
                case -1:
                    Log.WriteLog("Nem sikerült lekérdezni az adatbázisból a blacklist táblából! -> " + command, true);
                    return true;
                case 0: return false;
                case 1:
                    Log.WriteLog("IP szerepel a tiltólistán!", true);
                    return true;
                default: return true;
            }
        }
    }
}
