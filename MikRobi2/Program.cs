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

        static void Main(string[] args)
        {
            Console.WriteLine(ProgramName);
        }
    }
}
