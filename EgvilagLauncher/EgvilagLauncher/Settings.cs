using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EgvilagLauncher
{
    public class Settings
    {
        //private string Launcher_version;
        //private string Updater_version;
        //private string Theme_version;
        //private string Launcher_date;
        //private string Updater_date;
        //private string Theme_date;
        private List<User> Users;
        private int Autologin;
        private List<Game> Games;
        private const string SettingsFile = @"Launcher.cfg";

        public bool ReadSettings()
        {
            string line;
            string setName;
            string setValue;
            //User user;
            //Game game;
            Users = new List<User>();
            Games = new List<Game>();
            int index;
            for (int i = 0; i < 10; i++)
            {
                Users.Add(new User());
                Games.Add(new Game());
            }
            try
            {
                StreamReader read = new StreamReader(SettingsFile);
                while ((line = read.ReadLine()) != null)
                {
                    setName = line.Split('=')[0];
                    setValue = line.Split('=')[1];
                    switch (setName)
                    {
                    //    case "Launcher_version":
                    //        Launcher_version = setValue;
                    //        break;
                    //    case "Updater_version":
                    //        Updater_version = setValue;
                    //        break;
                    //    case "Theme_version":
                    //        Theme_version = setValue;
                    //        break;
                    //    case "Launcher_date":
                    //        Launcher_date = setValue;
                    //        break;
                    //    case "Updater_date":
                    //        Updater_date = setValue;
                    //        break;
                    //    case "Theme_date":
                    //        Theme_date = setValue;
                    //        break;
                        case "AutoLogin":
                            Autologin = Convert.ToInt32(setValue);
                            break;
                    }
                    if (setName.StartsWith("User"))
                    {
                        index = Convert.ToInt32(setName.Substring(4, 1));
                        if (setName.EndsWith("name")) Users[index].name = setValue;
                        if (setName.EndsWith("pass")) Users[index].pass = setValue;
                        if (setName.EndsWith("ask")) Users[index].ask = (setValue == "1");

                    }
                    if (setName.StartsWith("Game"))
                    {
                        index = Convert.ToInt32(setName.Substring(4, 1));
                        if (setName.EndsWith("name")) Games[index].name = setValue;
                        if (setName.EndsWith("path")) Games[index].path = setValue;
                        if (setName.EndsWith("theme")) Games[index].theme = setValue;

                    }
                }

                return true;
            }
            catch { return false; }
        }

        //public string GetLauncherVersion() { return Launcher_version; }

        //public string GetUpdaterVersion() { return Updater_version; }

        //public string GetThemeVersion() { return Theme_version; }

        //public string GetLauncherDate() { return Launcher_date; }

        //public string GetUpdaterDate() { return Updater_date; }

        //public string GetThemeDate() { return Theme_date; }

    }

    class User
    {
        public string name;
        public string pass;
        public bool ask;
    }

    class Game
    {
        public string name;
        public string path;
        public string theme;
    }
}
