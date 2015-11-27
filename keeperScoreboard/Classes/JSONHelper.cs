using Newtonsoft.Json;
using System;
using System.Windows;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
namespace keeperScoreboard.Classes
{
    public static class JSONHelper
    {
        static public mapId maps;
        static public modeIds modes;
        static public List<Structs.weapons> JsonHelperWeaponData = new List<Structs.weapons>();
        static public List<Structs.PlayerLoadout> JsonHelperPlayerLoadouts = new List<Structs.PlayerLoadout>();
        static public Dictionary<string, string> JsonHelperWeaponDataStrings;
        static JSONHelper()
        {
            string mapNames = Properties.Resources.mapIds.ToString();
            string modeNames = Properties.Resources.mapModes.ToString();
            string weaponString = Properties.Resources.weaponData.ToString();

            maps = JsonConvert.DeserializeObject<mapId>(mapNames);
            modes = JsonConvert.DeserializeObject<modeIds>(modeNames);
            WeaponDataNames WeaponStrings = new WeaponDataNames();
            JsonHelperWeaponDataStrings = WeaponStrings.getWeaponDataNames();
            dynamic WeaponDataFromDynamic = JsonConvert.DeserializeObject<dynamic>(weaponString);
            foreach (var weapoonData1 in WeaponDataFromDynamic["compact"]["weapons"])
            {
                string jsonText = weapoonData1.ToString();
                string front = "{";
                string back = "}";
                string final = front + jsonText + back;
                JObject JsonFromFinalText = JsonConvert.DeserializeObject<JObject>(final);
                foreach (KeyValuePair<string, JToken> x in JsonFromFinalText)
                {
                    ulong value;
                    //if the string is a ulong parse it as weapon data otherwise skip over it(image data)
                    if (ulong.TryParse(x.Key.ToString(), out value))
                    {
                            JsonHelperWeaponData.Add(new Structs.weapons
                            {
                                key = ulong.Parse(x.Key.ToString()),
                                weaponData = JsonConvert.DeserializeObject<Structs.RootObject>(x.Value.ToString())
                            });
                    }
                }
            }
        }
        static public string getImageURL(string imageString)
        {
            string returnString = imageString;
            returnString.Replace("ic-weapon", "");
            returnString.ToLower();
            return returnString;
        }
        static public string whatFaction(int factionNum)
        {
            string faction;
            switch (factionNum)
            {
                case 0:
                    faction = "US";
                    break;
                case 1:
                    faction = "RU";
                    break;
                case 2:
                    faction = "CN";
                    break;
                default:
                    faction = "Unknown";
                    break;
            }
            return faction;
        }
        static public string whatMap(string mapCode)
        {
            foreach (var mapId in maps.maps)
            {
                if (mapId.id.ToLower() == mapCode.ToLower())
                {
                    return mapId.label;
                }
            }
            Logger.addLog("Error finding map ID: " + mapCode);
            return "Error finding map ID";
        }
        static public string whatMode(long id)
        {
            foreach (var modeName in modes.modes)
            {
                if (modeName.id == id)
                {
                    return modeName.label;
                }
            }
            Logger.addLog("Error finding mode ID: " + id);
            return "Error finding mode ID";
        }
    }
}
