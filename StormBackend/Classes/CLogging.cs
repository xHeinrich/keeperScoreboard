using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormBackend
{
    public class CLogging 
    {
        public static void AddLog(string log = "", LogType type = LogType.Error)
        {
            App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                Program.Logs.Add(new SLogging
                {
                    LogID = Program.Logs.Count + 1,
                    LogString = log,
                    LogType = type,
                    LogTime = DateTime.Now
                });
            });


        }
    }
}
