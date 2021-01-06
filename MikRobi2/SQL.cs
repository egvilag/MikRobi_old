using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace MikRobi2
{
    class SQL
    {
        string connstr;
        MySqlConnection myConn;

        public static string dbServer;
        public static string dbPort;
        public static string dbUser;
        public static string dbPassword;
        public static string dbDatabase;

        public SQL()
        {
            connstr = "Server=" + dbServer + ";Port=" + dbPort + ";Database=" + dbDatabase + ";Uid=" + dbUser + ";Pwd=" + dbPassword + ";CharSet=utf8;";
            myConn = new MySqlConnection(connstr);
        }
    }
}
