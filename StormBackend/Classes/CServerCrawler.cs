using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
namespace StormBackend
{
    public static class CServerCrawler
    {

        public static async Task<List<SServers>> UpdateServers()
        {
            List<SServers> servers = new List<SServers>();
            string data;
            // all squad oblit servers
            string url =
                //"http://battlelog.battlefield.com/bf4/servers/pc/?filtered=1&expand=1&settings=&useLocation=1&useAdvanced=1&gameexpansions=-1&slots=16&slots=1&slots=2&slots=4&q=&gameexpansions=-1&gameexpansions=-1&gameexpansions=-1&gameexpansions=-1&gameexpansions=-1&mapRotation=-1&modeRotation=-1&password=-1&regions=64&osls=-1&vvsa=-1&vffi=-1&vaba=-1&vkca=-1&v3ca=-1&v3sp=-1&vmsp=-1&vrhe=-1&vhud=-1&vmin=-1&vnta=-1&vbdm-min=1&vbdm-max=300&vprt-min=1&vprt-max=300&vshe-min=1&vshe-max=300&vtkk-min=1&vtkk-max=99&vnit-min=30&vnit-max=86400&vtkc-min=1&vtkc-max=99&vvsd-min=0&vvsd-max=500&vgmc-min=0&vgmc-max=500";
                "http://battlelog.battlefield.com/bf4/servers/pc/?filtered=1&expand=1&settings=&useLocation=1&useAdvanced=1&gameexpansions=-1&slots=16&slots=1&slots=2&slots=4&q=&serverTypes=8&gameexpansions=-1&gameexpansions=-1&gameexpansions=-1&gameexpansions=-1&gameexpansions=-1&mapRotation=-1&modeRotation=-1&password=-1&osls=-1&vvsa=-1&vffi=-1&vaba=-1&vkca=-1&v3ca=-1&v3sp=-1&vmsp=-1&vrhe=-1&vhud=-1&vmin=-1&vnta=-1&vbdm-min=1&vbdm-max=300&vprt-min=1&vprt-max=300&vshe-min=1&vshe-max=300&vtkk-min=1&vtkk-max=99&vnit-min=30&vnit-max=86400&vtkc-min=1&vtkc-max=99&vvsd-min=0&vvsd-max=500&vgmc-min=0&vgmc-max=500";
            data = await HttpAsync.GetUrlAsync(url);
     
            //convert string to dynamic
            dynamic serverList = JsonConvert.DeserializeObject<dynamic>(data);
            //get the servers
            dynamic serverGuidList = serverList["globalContext"]["servers"];
            serverList = null;
            data = null;
            //Add guid and guid names to list
            if (serverGuidList != null)
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
                        servers.Add(new SServers
                        {
                            _ServerGuid = serverGuid,
                            _ServerName = serverName,
                            _MapID = mapID,
                            _TickRate = tickRate,
                            _PlayerSlots = playerCount,
                            _Location = countryInfo,
                            
                            LastUpdate = DateTime.Now
                        });
                    }
                }
            }
            return servers;
        }
    }
}
