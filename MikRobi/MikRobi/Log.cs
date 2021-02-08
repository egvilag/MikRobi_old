using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MikRobi
{
    class LogFile
    {
        string filename;
        StreamWriter stream;

        public LogFile(string filename)
        {
            this.filename = filename;
            stream = new StreamWriter(filename, true, Encoding.Default, 65535);
        }

        public void Write(string line)
        {
            stream.WriteLine(line);
            stream.Flush();
        }
    }

    class Log
    {
        
    }
}
