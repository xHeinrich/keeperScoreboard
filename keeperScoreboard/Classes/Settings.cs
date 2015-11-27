using System.IO;
using Newtonsoft.Json;

namespace keeperScoreboard.Classes
{
    class Settings
    {
        public static string settingsFile = Directory.GetCurrentDirectory() + "//settings.config";
        public static Structs.SettingsStruct Load()
        {
            if (File.Exists(settingsFile))
            {
                string settingsString = File.ReadAllText(settingsFile);
                Structs.SettingsStruct settings = JsonConvert.DeserializeObject<Structs.SettingsStruct>(settingsString);
                return settings;
            }
            else
            {
                Save();
                    return null;
            }
        }
        public static void Save()
        {
            if(MainWindow.keeperSettings == null)
            {
                MainWindow.keeperSettings = new Structs.SettingsStruct
                {
                   SaveOnClose = true,
                   ScoreboardBackgrounds = true,
                   MinimizeToTray = false
                };
                string settingsSaveString = JsonConvert.SerializeObject(MainWindow.keeperSettings);
                File.WriteAllText(settingsFile , settingsSaveString);
                Load();
            }
            else
            {
                string settingsSaveString = JsonConvert.SerializeObject(MainWindow.keeperSettings);
                File.WriteAllText(settingsFile, settingsSaveString);
            }
        }
    }
}
