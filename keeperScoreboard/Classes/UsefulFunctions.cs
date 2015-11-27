using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using Newtonsoft.Json;
using System.IO;

namespace keeperScoreboard.Classes
{
    public static class UsefulFunctions
    {
        public static long[] getTimes(List<Classes.CustomSnapshotRoot> root)
        {
            List<long> times = new List<long>();
            foreach (var timers in root)
            {
                times.Add(long.Parse(timers.snapshot.roundTime.ToString()));
            }
            return times.ToArray();
        }
 
        public static List<Classes.CustomSnapshotRoot> getFullReport(string filename)
        {
            List<Classes.CustomSnapshotRoot> returnRoot = null;
            try
            {
                string s = File.ReadAllText(filename);
                returnRoot = JsonConvert.DeserializeObject<List<Classes.CustomSnapshotRoot>>(s);
            }

            catch (Exception ex)
            {
                Classes.Logger.addLog(ex.ToString());
            }

            return returnRoot;
        }
        public static string[] getPlayerNames(List<Classes.CustomSnapshotRoot> root)
        {
            bool alreadyAdded = false;
            List<string> names = new List<string>();
            foreach (var player in root)
            {
                foreach (var playerName in player.snapshot.teamInfo.team1.player)
                {
                    foreach (var playerCheck in names)
                    {
                        if (playerCheck == playerName.name)
                        {
                            alreadyAdded = true;
                        }
                    }
                    if (!alreadyAdded)
                    {
                        names.Add(playerName.name);
                    }
                }
                foreach (var playerName in player.snapshot.teamInfo.team2.player)
                {
                    foreach (var playerCheck in names)
                    {
                        if (playerCheck == playerName.name)
                        {
                            alreadyAdded = true;
                        }
                    }
                    if (!alreadyAdded)
                    {
                        names.Add(playerName.name);

                    }
                }

            }
            return names.ToArray();
        }
        public static string getTime(double time, byte shortOrLong)
        {
            TimeSpan t = TimeSpan.FromSeconds(time);
            string str;
            switch (shortOrLong)
            {
                //Long Hours Minutes and Seconds
                case 0:
                    str = t.ToString(@"hh\:mm\:ss");
                    break;
                //Medium Minutes and Seconds
                case 1:
                    str = t.ToString(@"mm\:ss");
                    break;
                case 2: 
                    str = t.ToString(@"ss");
                    break;
                default:
                    str = t.ToString(@"hh\:mm\:ss");
                    break;
            }
            return str;
        }
        public static string checkVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            return version;
        }
        public static DateTime RetrieveLinkerTimestamp()
        {
            string filePath = System.Reflection.Assembly.GetCallingAssembly().Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;
            byte[] b = new byte[2048];
            System.IO.Stream s = null;

            try
            {
                s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                s.Read(b, 0, 2048);
            }
            finally
            {
                if (s != null)
                {
                    s.Close();
                }
            }

            int i = System.BitConverter.ToInt32(b, c_PeHeaderOffset);
            int secondsSince1970 = System.BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dt = dt.AddSeconds(secondsSince1970);
            dt = dt.ToLocalTime();
            return dt;
        }
    }
}
