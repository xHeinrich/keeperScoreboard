using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
namespace StormBackend
{
    class KeeperAPI
    {
        /*-----------
        stole some dnak code from dmex ty my frend
        ------------*/
        public string keeperURL = "http://keeper.battlelog.com/snapshot/";
        public string getNumPlayersOnServerURL = "http://battlelog.battlefield.com/bf4/servers/getNumPlayersOnServer/";
        public string getNumPlayersOnServerURLhardline = "http://battlelog.battlefield.com/bfh/servers/getNumPlayersOnServer/";
        public List<getNumPlayersOnServer> ListOfEpikSort = new List<getNumPlayersOnServer>();

        public async Task<CustomSnapshotRoot> getKeeperInfo(string guid)
        {
            string keeperData = "";
            getNumPlayersOnServer playersOnServerData = getNumPlayersOnServerInfo(guid);
            SnapshotRoot snapshotData = null;
            CustomSnapshotRoot snapshotSend = null;
            List<playersData> team0Data = new List<playersData>();
            List<playersData> team1Data = new List<playersData>();
            List<playersData> team2Data = new List<playersData>();
            playersData empty = new playersData { name = "No players on the server" };
            
            try
            {
                keeperData = await HttpAsync.GetUrlAsync(keeperURL + guid);
            }
            catch (Exception)
            {
                CLogging.AddLog("The server crashed or was never available: " + guid);
            }

            snapshotData = JsonConvert.DeserializeObject<SnapshotRoot>(keeperData);

            if (snapshotData == null)
                return null;
            if (!snapshotData.snapshot.status.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                return null;
            //run update code and return null

            if (snapshotData.snapshot.teamInfo.Team0 != null)
            {
                var team0 = snapshotData.snapshot.teamInfo.Team0;
                foreach (KeyValuePair<string, JToken> x in team0.players)
                {
                    TeamPlayer playerInfo = x.Value.ToObject<TeamPlayer>();

                    team0Data.Add(
                        new playersData
                        {
                            playerId = ulong.Parse(x.Key.ToString()),
                            name = playerInfo.Name,
                            kills = playerInfo.kills,
                            deaths = playerInfo.deaths,
                            score = playerInfo.score,
                            tag = playerInfo.tag,
                            squad = playerInfo.squad,
                            role = playerInfo.role
                        });
                }

            }
            if (snapshotData.snapshot.teamInfo.Team1 != null)
            {
                var team1 = snapshotData.snapshot.teamInfo.Team1;
                foreach (KeyValuePair<string, JToken> x in team1.players)
                {
                    TeamPlayer playerInfo = x.Value.ToObject<TeamPlayer>();

                    team1Data.Add(
                        new playersData
                        {
                            playerId = ulong.Parse(x.Key.ToString()),
                            name = playerInfo.Name,
                            kills = playerInfo.kills,
                            deaths = playerInfo.deaths,
                            score = playerInfo.score,
                            tag = playerInfo.tag,
                            squad = playerInfo.squad,
                            role = playerInfo.role
                        });
                }

            }
            else
            {
                team1Data.Add(empty);
            }

            if (snapshotData.snapshot.teamInfo.Team2 != null)
            {
                var team2 = snapshotData.snapshot.teamInfo.Team2;
                foreach (KeyValuePair<string, JToken> x in team2.players)
                {
                    TeamPlayer playerInfo = x.Value.ToObject<TeamPlayer>();

                    team2Data.Add(
                        new playersData
                        {
                            playerId = ulong.Parse(x.Key.ToString()),
                            name = playerInfo.Name,
                            kills = playerInfo.kills,
                            deaths = playerInfo.deaths,
                            score = playerInfo.score,
                            tag = playerInfo.tag,
                            squad = playerInfo.squad,
                            role = playerInfo.role
                        });
                }

            }
            else
            {
                team2Data.Add(empty);
            }
            //SORT TEAM DATA BY SCORE
            team1Data = team1Data.OrderByDescending(x => x.score).ToList();
            team2Data = team2Data.OrderByDescending(x => x.score).ToList();
            //Copy snapshotData to snapshotSend
            //--------------COPY---------------
            //repackage all data into CustomSnapshotRoot
            int team1Tickets = 0;
            int team2Tickets = 0;
            int team1TicketsMax = 0;
            int team2TicketsMax = 0;
            if (snapshotData.snapshot.Conquest != null)
            {
                team1Tickets = snapshotData.snapshot.Conquest.Team1.tickets;
                team1TicketsMax = snapshotData.snapshot.Conquest.Team1.ticketsMax;
                team2Tickets = snapshotData.snapshot.Conquest.Team2.tickets;
                team2TicketsMax = snapshotData.snapshot.Conquest.Team2.ticketsMax;
            }
            else if (snapshotData.snapshot.Deathmatch != null)
            {
                team1Tickets = snapshotData.snapshot.Deathmatch.Team1.Kills;
                team1TicketsMax = snapshotData.snapshot.Deathmatch.Team1.KillsMax;
                team2Tickets = snapshotData.snapshot.Deathmatch.Team2.Kills;
                team2TicketsMax = snapshotData.snapshot.Deathmatch.Team2.KillsMax;
            }
            try
            {
                snapshotSend = new CustomSnapshotRoot
                {
                    lastUpdated = snapshotData.lastUpdated,
                    snapshot = new CustomSnapshot
                    {
                        status = snapshotData.snapshot.status,
                        gameId = snapshotData.snapshot.gameId,
                        currentMap = snapshotData.snapshot.currentMap,
                        maxPlayers = snapshotData.snapshot.maxPlayers,
                        waitingPlayers = snapshotData.snapshot.waitingPlayers,
                        roundTime = snapshotData.snapshot.roundTime,
                        gameMode = snapshotData.snapshot.gameMode,
                        mapId = playersOnServerData.map,
                        modeId = playersOnServerData.mapMode,
                        team1Tickets = new modeCounter
                        {
                            tickets = team1Tickets,
                            ticketsMax = team1TicketsMax
                        },
                        team2Tickets = new modeCounter
                        {
                            tickets = team2Tickets,
                            ticketsMax = team2TicketsMax
                        },
                        teamInfo = new CustomSnapshotTeamInfo
                        {
                            team0 = new CustomSnapshotTeam
                            {
                                faction = snapshotData.snapshot.teamInfo.Team0.faction,
                                player = team0Data
                            },
                            team1 = new CustomSnapshotTeam
                            {
                                faction = snapshotData.snapshot.teamInfo.Team1.faction,
                                player = team1Data
                            },
                            team2 = new CustomSnapshotTeam
                            {
                                faction = snapshotData.snapshot.teamInfo.Team2.faction,
                                player = team2Data
                            }
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                CLogging.AddLog(ex.ToString());
            }
            if((team0Data.Count + team1Data.Count + team2Data.Count) < 1)
            {
                return null;
            }
            return snapshotSend;
        }
        async Task<string> getMapInfo(string url, HttpClient client)
        {
            var GetNumPlayersOnServerString = await client.GetStringAsync(url);
            return GetNumPlayersOnServerString;
        }

        async Task<List<getNumPlayersOnServer>> playas(string guid)
        {
            string[] platforms = { "pc/", "xboxone/", "xbox/", "ps4/", "ps3/" };
            HttpClient client =
                        new HttpClient() { MaxResponseContentBufferSize = 1000000 };
            Task<string> pc =
                        getMapInfo(getNumPlayersOnServerURL + platforms[0] + guid, client);
            Task<string> xboxone =
                        getMapInfo(getNumPlayersOnServerURL + platforms[1] + guid, client);
            Task<string> xbox =
                        getMapInfo(getNumPlayersOnServerURL + platforms[2] + guid, client);
            Task<string> ps4 =
                        getMapInfo(getNumPlayersOnServerURL + platforms[3] + guid, client);
            Task<string> ps3 =
                        getMapInfo(getNumPlayersOnServerURL + platforms[4] + guid, client);

            string splayersOnServerpc = await pc;
            string splayersOnServerxboxone = await xboxone;
            string splayersOnServerxbox = await xbox;
            string splayersOnServerps4 = await ps4;
            string splayersOnServerps3 = await ps3;

            getNumPlayersOnServer playersOnServerpc = JsonConvert.DeserializeObject<getNumPlayersOnServer>(splayersOnServerpc);
            getNumPlayersOnServer playersOnServerxboxone = JsonConvert.DeserializeObject<getNumPlayersOnServer>(splayersOnServerxboxone);
            getNumPlayersOnServer playersOnServerxbox = JsonConvert.DeserializeObject<getNumPlayersOnServer>(splayersOnServerxbox);
            getNumPlayersOnServer playersOnServerps4 = JsonConvert.DeserializeObject<getNumPlayersOnServer>(splayersOnServerps4);
            getNumPlayersOnServer playersOnServerps3 = JsonConvert.DeserializeObject<getNumPlayersOnServer>(splayersOnServerps3);
            ListOfEpikSort.Add(playersOnServerpc);
            ListOfEpikSort.Add(playersOnServerxboxone);
            ListOfEpikSort.Add(playersOnServerxbox);
            ListOfEpikSort.Add(playersOnServerps4);
            ListOfEpikSort.Add(playersOnServerps3);
            return ListOfEpikSort;

        }
        public getNumPlayersOnServer getNumPlayersOnServerInfo(string guid)
        {
            getNumPlayersOnServer playersOnServer = null;
            try
            {
                string[] platforms = { "pc/", "xboxone/", "xbox/", "ps4/", "ps3/" };

                using (WebClient wc = new WebClient())
                {
                    foreach (var platform in platforms)
                    {
                        string url = getNumPlayersOnServerURL + platform + guid;
                        string getNumPlayersOnServerJson = wc.DownloadString(url);
                        playersOnServer = JsonConvert.DeserializeObject<getNumPlayersOnServer>(getNumPlayersOnServerJson);
                        if (playersOnServer.map != "")
                            break;
                    }
                    //hardline
                    if (playersOnServer.map == "")
                    {
                        foreach (var platform in platforms)
                        {
                            string url = getNumPlayersOnServerURLhardline + platform + guid;
                            string getNumPlayersOnServerJson = wc.DownloadString(url);
                            playersOnServer = JsonConvert.DeserializeObject<getNumPlayersOnServer>(getNumPlayersOnServerJson);
                            if (playersOnServer.map != "")
                                break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                CLogging.AddLog("Guid: " + guid + "|" + ex.ToString());
            }
            return playersOnServer;
        }
    }
}
