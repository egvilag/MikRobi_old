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

        public SQL()
        {
            connstr = "Server=" + Program.dbServer + ";Port=" + Program.dbPort + ";Database=" + Program.dbDatabase +
                ";Uid=" + Program.dbUser + ";Pwd=" + Program.dbPassword + ";CharSet=utf8;";
            myConn = new MySqlConnection(connstr);
        }
    }
}
