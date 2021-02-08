using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Net.Sockets;
using System.Net;

// ÉGvilág Szerver

namespace EGvilagServerCLI2
{
    class Program
    {
        const int KeyCount = 6; //Beállításkulcsok száma
        public static string ServerAddress;
        public static int ServerPort;
        public static long MaxLogSize;
        public static int pingInterval;
        public static int pingTimeout;
        public static long disconnectAfter;
        static List<string> InfoLOG = new List<string>();
        static List<string> ErrorLOG = new List<string>();
        static List<string> PubCHAT = new List<string>();
        static List<string> TeamCHAT = new List<string>();
        static List<string> PM = new List<string>();

        static string ProgramName = "ÉGvilág Mikrobi v0.1";
        
        static string logDir = @"LOG";

        static List<Client> clients = new List<Client>();
        static TcpClient kliens;
        static Thread Tconnect;
        static TcpListener listener;
        static bool ListenConnection = false;

        static void Main(string[] args)
        {
            if (ReadSettings())
            {
                Console.CursorVisible = false;
                DrawTitle("Felhasználók");
                DrawMenu();
                WriteLog("[INFO] Szerver elindítva (IP: " + Program.ServerAddress + ", port: " + Program.ServerPort + ")");
                listener = new TcpListener(IPAddress.Parse(Program.ServerAddress), Program.ServerPort);
                listener.Start();
                ListenConnection = true;
                Tconnect = new Thread(Figyel);
                Tconnect.Start();

            }
        }

#region Config olvasás
        static bool ReadSettings()
        {
            StreamReader read;
            int count = 0;
            try
            {
                read = new StreamReader("server.cfg");
                string line;
                string key;
                string value;
                while ((line = read.ReadLine()) != null)
                {
                    if (!line.StartsWith("#")) //nem kommentelt sor
                    {
                        key = line.Split(new char[1] { '=' })[0];
                        value = line.Split(new char[1] { '=' })[1];
                        switch (key)
                        {
                            case "ServerIP": if (ServerAddress == null) count++; ServerAddress = value; break;
                            case "ServerPort": if (ServerPort == 0) count++; ServerPort = Convert.ToInt32(value); break;
                            case "MaxLogSize": if (MaxLogSize == 0) count++;  MaxLogSize = long.Parse(value); break;
                            case "PingInterval": if (pingInterval == 0) count++; pingInterval = Convert.ToInt32(value); break;
                            case "PingTimeout": if (pingTimeout == 0) count++; pingTimeout = Convert.ToInt32(value); break;
                            case "DisconnectAfter": if (disconnectAfter == 0) count++; disconnectAfter = long.Parse(value); break;
                        }

                    }
                }
                if (count != KeyCount)
                {
                    WriteLog("[ERROR] Hiba a config fájl olvasásakor!");
                    read.Close();
                    read.Dispose();
                    return false;
                }
            }
            catch
            {
                WriteLog("[ERROR] Hiba a config fájl olvasásakor!");
                return false;
            }
            read.Close();
            read.Dispose();
            return true;
        }
#endregion


        // Log üzenetek helyes szintaktikája:

        // [INFO] message
        // [ERROR] message
        // [PUBCHAT] <fromuser> message
        // [TEAMCHAT] <fromuser><teamname> message
        // [PM] <fromuser> -> <touser> message
#region Logolás
        public static void WriteLog(string message)
        {
            StreamWriter write;
            string logFile = "";
            try
            {
                string messageType = message.Substring(1, message.IndexOf(']') - 1);
                switch (messageType)
                {
                    case "INFO": logFile = logDir + @"/info.log"; break;
                    case "ERROR": logFile = logDir + @"/error.log"; break;
                    case "PUBCHAT": logFile = logDir + @"/pubchat.log"; break;
                    case "TEAMCHAT": logFile = logDir + @"/teamchat.log"; break;
                    case "PM": logFile = logDir + @"/pm.log"; break;
                }
                if (logFile != null)
                {
                    if (File.Exists(logFile))
                    {
                        long fileSize = new FileInfo(logFile).Length;
                        if (fileSize >= Program.MaxLogSize)
                        {
                            File.Move(logFile, logFile + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day);
                        }
                    }
                    write = new StreamWriter(logFile, true);
                    write.WriteLine(DateTime.Now + " >> " + message);
                    write.Close();
                    write.Dispose();
                }
                //Console.WriteLine(DateTime.Now + " >> " + message);
                switch (messageType)
                {
                    case "INFO": InfoLOG.Add(DateTime.Now + " >> " + message); break;
                    case "ERROR": ErrorLOG.Add(DateTime.Now + " >> " + message); break;
                    case "PUBCHAT": PubCHAT.Add(DateTime.Now + " >> " + message); break;
                    case "TEAMCHAT": TeamCHAT.Add(DateTime.Now + " >> " + message); break;
                    case "PM": PM.Add(DateTime.Now + " >> " + message); break;
                }
            }
            catch
            {
                Console.WriteLine(DateTime.Now + " >> " + "Hiba a logoláskor a következő üzenetnél:");
                Console.WriteLine(DateTime.Now + " >> " + message);
            }
        }
#endregion

#region ui

        static void DrawTitle(string menu)
        {
            Console.SetCursorPosition(0, 0);
            Console.Write(ProgramName + " >> " + menu);
        }

        static void DrawMenu()
        {
            // 1Users 2Teams 3InfoLOG 4ErrorLOG 5PubChat 6TeamChat 7PM

            Console.SetCursorPosition(0, 1);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write("1");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.Write("Users");

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(" 2");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.Write("Teams");

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(" 3");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.Write("InfoLOG");

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(" 4");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.Write("ErrorLOG");

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(" 5");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.Write("PubCHAT");

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(" 6");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.Write("TeamCHAT");

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(" 7");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.WriteLine("PM");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
            for (int i = 0; i < 80; i++)
                Console.Write("=");
            Console.WriteLine();

        }

#endregion

        static private void Figyel()
        {
            bool found = false;
            IPEndPoint ep;
            while (ListenConnection)
            {

                kliens = listener.AcceptTcpClient();
                ep = new IPEndPoint(((IPEndPoint)kliens.Client.RemoteEndPoint).Address, ((IPEndPoint)kliens.Client.RemoteEndPoint).Port);
                WriteLog("[INFO] Új bejövő kapcsolat: " + ep.Address.ToString() + ":" + ep.Port.ToString());
                foreach (Client k in clients)
                {
                    if ( ((IPEndPoint)kliens.Client.RemoteEndPoint).Address.ToString() == k.ep.Address.ToString())
                    {
                        found = true;
                        k.kliens = kliens;
                    }
                }
                if (found)
                {
                    WriteLog("[INFO] Ugyanaz az IP, cseréltem a socketet.");
                }
                else
                {
                    //ep = new IPEndPoint(((IPEndPoint)kliens.Client.RemoteEndPoint).Address, ((IPEndPoint)kliens.Client.RemoteEndPoint).Port);
                    //WriteLog("[INFO] Új bejövő kapcsolat: " + ep.Address.ToString() + ":" + ep.Port.ToString());
                    clients.Add(new Client(kliens));
                }
                found = false;
            }
        }

        static public void ClientDisconnected(Client kliens)
        {
            WriteLog("[INFO] A kapcsolat megszakadt a következővel: " + ((IPEndPoint)kliens.kliens.Client.RemoteEndPoint).Address + ":" + ((IPEndPoint)kliens.kliens.Client.RemoteEndPoint).Port);
            clients.Remove(kliens);
            kliens.kliens.Close();
            kliens.kliens = null;
            kliens = null;
            

        }

        static public bool ProcessMessage(string message)
        {
            string command = "";
            string[] args = new string[5];
            try
            {
                if (message.StartsWith("["))
                {
                    command = message.Substring(0, message.IndexOf(']') + 1);
                    args = message.Replace(command, "").Split('$');
                }
                else
                    if (message.StartsWith("+"))
                    {
                        command = message.Substring(0, message.IndexOf(' ') + 1);
                        args = message.Replace(command, "").Split(' ');
                    }
                    else return false;
            }
            catch
            {
                return false;
            }

            if (args.Count() == 0) return false;

            switch (command)
            {
                case "[CONN]":  //LL: bejelentkezés
                    break;
                case "[SEND]":  //LL: üzenet küldés
                    break;
                case "+kick":   //Admin: kirúgás + 1 perc tiltás
                    break;
                case "+ban":    //Admin: tiltás
                    break;
                case "+dban":   //Admin: tiltás feloldása
                    break;
                case "+srol":   //Admin: jogosultságok megadása
                    break;
                case "+grol":   //Admin: jogosultságok listázása
                    break;
                case "+addt":   //Admin: felhasználó hozzáadása egy csapathoz
                    break;
                case "+remt":   //Admin: felhasználó kivétele a csapatából
                    break;
                case "+arnt":   //Admin: egy csapat átnevezése
                    break;
                case "+adlt":   //Admin: egy csapat törlése
                    break;
                case "+delt":   //Team-admin: csapat törlése
                    break;
                case "+rent":   //Team-admin: csapat átnevezése
                    break;
                case "+invu":   //Team-admin: meghívás a csapatba
                    break;
                case "+dinv":   //Team-admin: meghívás visszavonása
                    break;
                case "+remu":   //Team-admin: felhasználó kidobása a csapatból
                    break;
                case "+adda":   //Team-admin: team-admin jog adása csapattagnak
                    break;
                case "+dela":   //Team-admin: team-admin jog megvonás csapattagtól
                    break;
                case "+crtt":   //Mindenki: csapat létrehozása
                    break;
                case "+mute":   //Mindenki: felhasználó némítása
                    break;
                case "+dmut":   //Mindenki: némítás feloldása
                    break;
                default:
                    return false;
            }

            return true;
        }
    }
}
