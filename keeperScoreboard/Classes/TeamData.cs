using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Windows;

namespace keeperScoreboard.Classes
{
    public static class TeamData
    {
        private static string CurrentDirectory = Directory.GetCurrentDirectory();
        private static string TeamDataDirectory = "";
        private static string TeamFileName = "\\teamdata.scrbi";
        private static string FullTeamFIlePath = CurrentDirectory + TeamDataDirectory + TeamFileName;
        static TeamData()
        {
            if(!Directory.Exists(CurrentDirectory+TeamDataDirectory))
            {
                Directory.CreateDirectory(CurrentDirectory + TeamDataDirectory);

            }
            if(!File.Exists(CurrentDirectory + TeamDataDirectory + TeamFileName))
            {
                File.WriteAllText(FullTeamFIlePath, "");
            }
        }
        public static void AddMembers(string TeamName, List<Classes.Structs.TeamMember> MembersToAdd)
        {
            bool isTeamAdded = false;
            foreach (var CompareTeam in MainWindow.CompetitiveTeamData)
                if (CompareTeam != null && CompareTeam.TeamName == TeamName)
                {
                    //Modify Team
                    isTeamAdded = true;
                    CompareTeam.TeamMembers = MembersToAdd;
                    Logger.addLog(String.Format("Added Members {0} to Competitive team {1}",MembersToAdd.ToArray().ToString(), TeamName));
                    SaveTeamData();
                }
            if(isTeamAdded == false)
            {
                MessageBox.Show(String.Format("Failed to add team {0} to competitive team data", TeamName));
                //No team is available, please create a team first
            }
        }
        public static Structs.TeamData GetTeam(string TeamName)
        {
            foreach(var Team in MainWindow.CompetitiveTeamData)
            {
                if(Team.TeamName == TeamName)
                {
                    return Team;
                }
            }
            return null;
        }
        public static string[] ListOfTeam()
        {
            List<string> Teams = new List<string>();
            if (MainWindow.CompetitiveTeamData != null)
            { 
            foreach (var CompareTeam in MainWindow.CompetitiveTeamData)
            {
                Teams.Add(CompareTeam.TeamName);

            }
            return Teams.ToArray();
        }else{
                return null;
        }

        }
        public static void AddTeam(string TeamName, string TeamLogoSquarePath, string TeamLogoRectanglePath)
        {
            if (MainWindow.CompetitiveTeamData != null)
            {
                foreach (var CompareTeam in MainWindow.CompetitiveTeamData)
                    if (CompareTeam != null && CompareTeam.TeamName == TeamName)
                    {
                        MessageBox.Show("A team with this name has already been added");
                        return;
                    }
            }
            if(MainWindow.CompetitiveTeamData == null)
            {
                MainWindow.CompetitiveTeamData = new List<Structs.TeamData>();
            }
            MainWindow.CompetitiveTeamData.Add(new Structs.TeamData
            {
                TeamName = TeamName,
                TeamLogoSquare = TeamLogoSquarePath,
                TeamLogoRectangle = TeamLogoRectanglePath
            });
            SaveTeamData();
        }
        public static void SaveTeamData()
        {
            string WriteToJson = JsonConvert.SerializeObject(MainWindow.CompetitiveTeamData.ToArray());
            File.WriteAllText(CurrentDirectory + TeamDataDirectory + TeamFileName, WriteToJson);
            Logger.addLog("Saved team data");
        }
        public static void ReadTeamData()
        {
            try {
                string FileRead = File.ReadAllText(CurrentDirectory + TeamDataDirectory + TeamFileName);
                MainWindow.CompetitiveTeamData = JsonConvert.DeserializeObject<List<Classes.Structs.TeamData>>(FileRead);
                Logger.addLog("Reloaded team data");
            }catch(Exception)
            {
                Logger.addLog("Failed to write CompetitiveTeamData", 1);
            }
        }
    }
}
