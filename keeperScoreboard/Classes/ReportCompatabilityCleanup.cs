using System.Diagnostics;
using System.IO;
using System;
using System.Windows;
namespace keeperScoreboard.Classes
{
    class ReportCompatabilityCleanup
    {

        string[] reports;
        string currentDir;
        public ReportCompatabilityCleanup()
        {
            try
            {
                reports = Directory.GetFiles(Directory.GetCurrentDirectory() + "//reports");
                currentDir = Directory.GetCurrentDirectory();
            }
            catch (Exception)
            {
                return;
            }
            FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(currentDir + "\\update.exe");
            string fileVersion = myFileVersionInfo.FileVersion;
            if (fileVersion == "1.0.0.1")
            {
                if (numReports() > 0)
                {
                        renameReports();
                }

            }
        }
        public int numReports()
        {
            return reports.Length;
        }
        public void renameReports()
        {
            foreach (string s in reports)
            {
                try {
                    string[] reportName = s.Split('/');
                    string[] reportMap = reportName[reportName.Length - 1].Split('_');
                    string[] reportTime = reportMap[3].Split('.');
                    long n;
                    bool isNumeric = long.TryParse(reportTime[0], out n);
                    if (isNumeric)
                    {
                        string newName = "scoreboard_" + reportTime[0] + ".scbrd";
                        Classes.Logger.addLog("Converted to new version: " + s + " -> " + newName);
                        File.Copy(s, Directory.GetCurrentDirectory() + "//reports//" + newName);
                        File.Delete(s);
                    }
                }catch(Exception)
                {
                    //Classes.Logger.addLog(ex.ToString());
                    Classes.Logger.addLog("Already Correct Format: " + s);
                }
            }
        }
    }
}
