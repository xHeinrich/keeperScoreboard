using System;
using System.IO;
namespace StormBackend
{
    public static class FileHandler
    {
        public static void Save(string content, string guid = "")
        {
                      string CurrentDirectory = Directory.GetCurrentDirectory();
          string Filename = DateTime.Now.Ticks.ToString();

            try
            {
                File.WriteAllText(CurrentDirectory + "/" + guid + " " + Filename, content);
            }
            catch (Exception ex)
            {
                CLogging.AddLog(ex.ToString());
            }
        }
    }
}
