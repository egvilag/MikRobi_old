using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace EGvilagServerCLI2
{


    class SQL
    {
        static string connstr = "Server=89.134.53.41;Port=3306;Database=egvilag;Uid=MikRobi;Pwd=EgViMikRob2020;";
        private MySqlConnection myConn;

        public void GetUsers()
        {
            myConn = new MySqlConnection(connstr);
            try { myConn.Open(); }
            catch (Exception e) { Program.WriteLog("[ERROR] Nem sikerült kapcsolódni az adatábzishoz:" + e.Message); }
            myConn.Close();
        }
    }
}
