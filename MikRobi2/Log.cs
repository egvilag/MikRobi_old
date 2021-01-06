using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using LZ4;

namespace MikRobi2
{
    static class Log
    {
        static StreamWriter stream;
        public static long maxLogSize;

        public static void CreateLogFile()
        {
            try
            {
                stream = new StreamWriter(Program.path + @"/LOGS/console.log", true, Encoding.UTF8, 65535);
            }
            catch
            {
                Console.WriteLine("Nem sikerült létrehoznom a console.log fájlt!");
                Environment.Exit(0);
            }
            WriteLog("########################################", false);
            WriteLog("           Logolás indítva", false);
            WriteLog("########################################", false);
        }

        public static void WriteLog(string line, bool writeBack)
        {
            string dateTime;
            dateTime = DateTime.Today.Year.ToString();
            if (DateTime.Today.Month < 10) dateTime += "0";
            dateTime += DateTime.Today.Month.ToString();
            if (DateTime.Today.Day < 10) dateTime += "0";
            dateTime += DateTime.Today.Day.ToString() + "_";
            if (DateTime.Now.Hour < 10) dateTime += "0";
            dateTime += DateTime.Now.Hour.ToString();
            if (DateTime.Now.Minute < 10) dateTime += "0";
            dateTime += DateTime.Now.Minute.ToString();
            if (DateTime.Now.Second < 10) dateTime += "0";
            dateTime += DateTime.Now.Second.ToString();
            if (writeBack) Console.WriteLine(line);
            CheckSize(maxLogSize);
            if (stream != null)
            {
                stream.WriteLine("[" + dateTime + "] " + line);
                stream.Flush();
            }
        }

        static void CheckSize(long maxSize)
        {
            if (new FileInfo(Program.path + @"/LOGS/console.log").Length > maxSize)
            {
                string dateTime;
                CloseLogFile();
                dateTime = DateTime.Today.Year.ToString();
                if (DateTime.Today.Month < 10) dateTime += "0";
                dateTime += DateTime.Today.Month.ToString();
                if (DateTime.Today.Day < 10) dateTime += "0";
                dateTime += DateTime.Today.Day.ToString() + "_";
                if (DateTime.Now.Hour < 10) dateTime += "0";
                dateTime += DateTime.Now.Hour.ToString();
                if (DateTime.Now.Minute < 10) dateTime += "0";
                dateTime += DateTime.Now.Minute.ToString();
                if (DateTime.Now.Second < 10) dateTime += "0";
                dateTime += DateTime.Now.Second.ToString();
                Compress(Program.path + @"/LOGS/console.log", Program.path + @"/LOGS/OLD/console.log-" + dateTime + @".lz4 ");
                File.Delete(Program.path + @"/LOGS/console.log");
                try
                {
                    while (File.Exists(Program.path + @"/LOGS/console.log")) { Thread.Sleep(200); }
                    CreateLogFile();
                }
                catch
                {
                    Console.WriteLine("Nem sikerült létrehoznom a console.log fájlt!");
                    Environment.Exit(0);
                }
            }
        }

        public static void CloseLogFile()
        {
            stream.Flush();
            stream.Close();
            stream = null;
        }

        public static void Compress(string source, string destination)
        {
            WriteLog("Tömörítés...", true);

            FileStream fs = File.OpenRead(source);
            byte[] fb = new byte[fs.Length];
            fs.Read(fb, 0, fb.Length);
            fs.Close();

            byte[] fb2 = LZ4Codec.Wrap(fb);

            FileStream fs2 = new FileStream(destination, FileMode.Create);
            fs2.Write(fb2, 0, fb2.Length);
            fs2.Flush();
            fs2.Close();

            WriteLog("Tömörítés kész", true);
        }

        static void Decompress(string source, string destination)
        {
            WriteLog("Kicsomagolás...", true);

            FileStream fs = File.OpenRead(source);
            byte[] fb = new byte[fs.Length];
            fs.Read(fb, 0, fb.Length);
            fs.Close();

            byte[] fb2 = LZ4Codec.Unwrap(fb);

            FileStream fs2 = new FileStream(destination, FileMode.Create);
            fs2.Write(fb2, 0, fb2.Length);
            fs2.Flush();
            fs2.Close();
            WriteLog("Kicsomagolás kész", true);
        }
    }
}
