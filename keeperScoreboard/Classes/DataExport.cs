using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
namespace keeperScoreboard.Classes
{
    class DataExport
    {
        List<Classes.CustomSnapshotRoot> PotentialSave;
        CustomSnapshotRoot checkR;
        public DataExport(List<Classes.CustomSnapshotRoot> root, CustomSnapshotRoot check)
        {
            PotentialSave = root;
            checkR = check;
        }
       
        public string saveOrNot(string description = "")
        {
            if (checkR.snapshot.roundTime == 0)
            {
                if (PotentialSave.Count != 0)
                {
                    long saveTicks = DateTime.Now.Ticks;
                    string fileName = Directory.GetCurrentDirectory() + "\\reports\\scoreboard_" + saveTicks.ToString() + ".scbrd";
                    Classes.Logger.addLog("DataExport() - Reached log save -> " + fileName);
                    if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\reports"))
                    {
                        Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\reports");
                    }
                    PotentialSave.RemoveAt(PotentialSave.Count - 1);
                    string json = JsonConvert.SerializeObject(PotentialSave.ToArray());
                    System.IO.File.WriteAllText(fileName, json);
                    Classes.Logger.addLog("exportData() - Wrote report: " + fileName);
                    Classes.DescriptionLoader.addDescription(saveTicks, description);
                    return fileName;
                    //exportData export = new exportData(fileName);
                    //MessageBox.Show("Saved current game to " + Directory.GetCurrentDirectory() + "\\reports\\\\scoreboard_" + DateTime.Now.Ticks.ToString() + ".scbrd");
                    //Scoreboard.extendedStats.Clear();
                }
                return "";
            }
            return "";
        }
    }
}
