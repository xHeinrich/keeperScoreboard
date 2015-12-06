using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;

namespace keeperScoreboard.Classes
{
    public static class BattlelogCrawler
    {
        public static List<Structs.ServerInfo> BattlelogCrawlerBegin()
        {
            List<Structs.ServerInfo> activePrivateServers = new List<Classes.Structs.ServerInfo>();
            string data;
            // all squad oblit servers
            string url2 =
                "http://battlelog.battlefield.com/bf4/servers/pc/?filtered=1&expand=1&settings=&useLocation=1&useAdvanced=1&gameexpansions=-1&gamemodes=137438953472&slots=16&slots=1&slots=2&slots=4&q=&gameexpansions=-1&gameexpansions=-1&gameexpansions=-1&gameexpansions=-1&gameexpansions=-1&mapRotation=-1&modeRotation=-1&password=-1&osls=-1&vvsa=-1&vffi=-1&vaba=-1&vkca=-1&v3ca=-1&v3sp=-1&vmsp=-1&vrhe=-1&vhud=-1&vmin=-1&vnta=-1&vbdm-min=1&vbdm-max=300&vprt-min=1&vprt-max=300&vshe-min=1&vshe-max=300&vtkk-min=1&vtkk-max=99&vnit-min=30&vnit-max=86400&vtkc-min=1&vtkc-max=99&vvsd-min=0&vvsd-max=500&vgmc-min=0&vgmc-max=500";
            using (WebClient wc = new WebClient())
            {
                //get json response from battlelog
                wc.Headers.Add("X-AjaxNavigation", "1");
                wc.Headers.Add("X-Requested-With", "XMLHttpRequest");
                wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.124 Safari/537.36");
                data = wc.DownloadString(url2);
            }
            //convert string to dynamic
            dynamic serverList = JsonConvert.DeserializeObject<dynamic>(data);
            //get the servers
            dynamic serverGuidList = serverList["globalContext"]["servers"];
            //Add guid and guid names to list
            if(serverGuidList != null)
            {
                foreach (var guid in serverGuidList)
                {
                    Guid serverGuid = Guid.Parse(guid["guid"].ToString());
                    string serverName = guid["name"].ToString();
                    int tickRate = Convert.ToInt32(guid["tickRate"]);
                    string mapID = guid["map"].ToString();
                    int playerCount = Convert.ToInt32(guid["slots"]["2"]["current"]);
                    string countryInfo = guid["country"].ToString();
                    //limit for playercount if wanted
                    if (playerCount >= 0)
                    {
                        activePrivateServers.Add(new Structs.ServerInfo
                        {
                            serverGuid = serverGuid,
                            serverName = serverName,
                            mapID = mapID,
                            tickRate = tickRate,
                            playerSlots = playerCount,
                            location = countryInfo
                        });
                    }
                }
            }
            Console.Write("");
            //order the list by playercount so that highest playercount is first and lowest is last
            activePrivateServers = activePrivateServers.OrderByDescending(o => o.playerSlots).ToList();

            return activePrivateServers;
        }
        static List<Structs.PlayerReport> getReports(int numReports, string playername, string playerUID)
        {
            string url = "http://battlelog.battlefield.com/bf4/soldier/" + playername + "/battlereports/" + playerUID + "/pc/";
                string data;
            using (WebClient wc = new WebClient())
            {
                //get json response from battlelog
                wc.Headers.Add("X-AjaxNavigation", "1");
                wc.Headers.Add("X-Requested-With", "XMLHttpRequest");
                wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.124 Safari/537.36");
                data = wc.DownloadString(url);
            }
            return null;
        }
    }
}
