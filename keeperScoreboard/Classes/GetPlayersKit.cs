using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows;
namespace keeperScoreboard.Classes
{
    public static class GetPlayersKit
    {
        public static List<Structs.kits> GetPlayerKitInfo(string jsonString)
        {
            List<Structs.kits> kitData = new List<Structs.kits>();
                dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(jsonString);
                byte i = 0;
                foreach (var kitInfo in jsonData["data"]["currentLoadout"]["kits"])
                {
                    
                        List<ulong> kitInformation = JsonConvert.DeserializeObject<List<ulong>>(kitInfo.ToString());
                        kitData.Add(new Structs.kits
                        {
                            kitId = byte.Parse(i.ToString()),
                            kitIdInformation = kitInformation
                        });
                    i += 1;
                }
            
            return kitData;
        }
        public static async Task<string> downloadKitData(Uri url)
        {
            using (var client = new HttpClient())
            {
                return await client.GetStringAsync(url);
            }
        }

        public static async Task<Structs.PlayerLoadout> GetWeaponInfo(ulong playerId, string playerName)
        {

            string url = "http://battlelog.battlefield.com/bf4/loadout/get/" + playerName + "/" + playerId + "/1/";
            List<Structs.currentLoadoutWeapons> WeaponData = new List<Structs.currentLoadoutWeapons>();
            List<Structs.kits> kitData = new List<Structs.kits>();
            byte selectedKit = 0;

            using (WebClient wc = new WebClient())
            {
                //string jsonString = wc.DownloadString(url);
                Uri kitUri = new Uri(url, UriKind.Absolute);
                string jsonString = await downloadKitData(kitUri);
                kitData = GetPlayerKitInfo(jsonString);
                dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(jsonString);
                selectedKit = byte.Parse(jsonData["data"]["currentLoadout"]["selectedKit"].ToString());
                foreach (var weaponInfo in jsonData["data"]["currentLoadout"]["weapons"])
                {
                    string jsonText = weaponInfo.ToString();
                    string front = "{";
                    string back = "}";
                    string final = front + jsonText + back;
                    JObject JsonFromFinalText = JsonConvert.DeserializeObject<JObject>(final);
                    foreach (KeyValuePair<string, JToken> x in JsonFromFinalText)
                    {
                        List<ulong> attachmentList = JsonConvert.DeserializeObject<List<ulong>>(x.Value.ToString());
                        WeaponData.Add(new Structs.currentLoadoutWeapons
                        {
                            weaponId = ulong.Parse(x.Key.ToString()),
                            weaponAttachments = attachmentList
                        });
                    }
                }
            }
            Structs.PlayerLoadout loadout = new Structs.PlayerLoadout { kitList = kitData, weaponList = WeaponData, playerId = playerId,selectedKit = selectedKit };
            return loadout;
        }
    }
}
