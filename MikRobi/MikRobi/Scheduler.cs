using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Timers;

namespace MikRobi
{
    class Task
    {
        public long timeInSeconds;
        public string scriptPath;
    }

    class Scheduler
    {
        const string taskList = "schedule.lst";
        Timer timer = new Timer();
        List<Task> tasks = new List<Task>();
        List<Timer> timers = new List<Timer>();
    }
}
