using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
namespace keeperScoreboard.Classes
{

    public class Description
    {
        public long time { get; set; }
        public string descr { get; set; }
    }
    public static class DescriptionLoader
    {
        static DescriptionLoader()
        {
            try
            {
                readDescription();
            }
            catch (Exception)
            {
                addDescription(0, "");
                readDescription();
            }
        }
        public static void addDescription(long l_time, string s_descr)
        {
            MainWindow.reportDescriptions.Add(new Description
            {
                time = l_time,
                descr = s_descr
            });
            string json = JsonConvert.SerializeObject(MainWindow.reportDescriptions.ToArray());
            System.IO.File.WriteAllText(Directory.GetCurrentDirectory() + "/descriptions.dscbrd", json);
        }
        public static void deleteReport(string location)
        {

            File.Delete(location);

        }
        public static void editDescription(long l_time, string s_descr)
        {
            int index = MainWindow.reportDescriptions.FindLastIndex(c => c.time == l_time);
            if (index != -1)
            {
                MainWindow.reportDescriptions[index] = new Description() { time = l_time, descr = s_descr };
            }
            string json = JsonConvert.SerializeObject(MainWindow.reportDescriptions.ToArray());
            System.IO.File.WriteAllText(Directory.GetCurrentDirectory() + "/descriptions.dscbrd", json);
        }
        public static void readDescription()
        {
            MainWindow.reportDescriptions.Clear();
            string textRead = File.ReadAllText(Directory.GetCurrentDirectory() + "/descriptions.dscbrd");
            var json = JsonConvert.DeserializeObject<List<Description>>(textRead);
            foreach (var output in json)
            {
                MainWindow.reportDescriptions.Add(output);
            }
        }
        public static void mergeDescription(string location)
        {
            string textRead = File.ReadAllText(location);
            var json = JsonConvert.DeserializeObject<List<Description>>(textRead);
            foreach (var output in json)
            {
                addDescription(output.time, output.descr);
            }
        }
    }
}
