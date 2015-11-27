using System;
using System.IO;
using System.Windows;
using System.Diagnostics;
namespace keeperScoreboard.Classes
{
    static public class Logger
    {
        static string logDir = Directory.GetCurrentDirectory() + "\\log\\";
        static Logger()
        {
            if (!Directory.Exists(logDir))
            {
                try
                {
                    Directory.CreateDirectory(logDir);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
        static public void addLog(string log, int type = 0)
        {
            //type 0 = application log
            //type 1 = exception log
            string filenames = "";
            if(type == 0)
            {
                filenames = "app_log.log";
            }
            else if(type == 1)
            {
                filenames = "exception_log.log";
            }
            // try until file is ready to be written to
            while (true)

                try
                {
                    log = DateTime.Now.ToString() + " | Build: " + Classes.UsefulFunctions.checkVersion() + " | " + log;
                    if (File.Exists(logDir + filenames))
                    {
                        using (StreamWriter sw = File.AppendText(logDir + filenames))
                        {
                            sw.WriteLine(log);
                            return;
                        }
                    }
                    else
                    {
                        File.WriteAllText(logDir + filenames, log);
                        return;
                    }
                }
                catch (Exception)
                {
                    Debug.Write(String.Format("Failed to write to file {0}", filenames));
                        }
        }
      
    }
}
