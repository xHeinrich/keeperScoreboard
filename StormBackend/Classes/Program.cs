using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace StormBackend
{
    public static class Program
    {
        public static ObservableCollection<SLogging> Logs = new ObservableCollection<SLogging>();
        public static ObservableCollection<SServers> Servers = new ObservableCollection<SServers>();
        public static bool IsRunning = true;
        private static object ServerLockObject = new object();

        static Program()
        {
            System.Windows.Data.BindingOperations.EnableCollectionSynchronization(Servers, ServerLockObject);
        }

        public async static void UpdateServers()
        {
            if (!IsRunning)
                return;

            ObservableCollection<SServers> servers = await CServerCrawler.UpdateServers();
            CLogging.AddLog(String.Format("Updating {0} servers", servers.Count), LogType.Server);
            foreach (var server in servers)
            {
                if (Servers.Contains(server))
                {
                    int i = Servers.IndexOf(server);
                    Servers[i] = server;
                }
                else
                {
                    Servers.Add(server);
                    CLogging.AddLog(String.Format("Added Server {0}", server.ServerGuid.ToString()), LogType.Server);
                }
            }
            UpdateServerKeeper();
        }

        public async static void UpdateServerKeeper()
        {
             int serversSkipped = 0;
             int y;
             for(y = 0; y < Servers.Count-1; y++)
             {
                //SServers item = Servers[y];
                KeeperAPI keeper = new KeeperAPI();
                CustomSnapshotRoot root = await keeper.getKeeperInfo(Servers[y]._ServerGuid.ToString());
                if (root == null)
                    continue;
                if (Servers[y]._Snapshots == null)
                {
                    Servers[y]._Snapshots = new List<CustomSnapshotRoot>();// new ObservableCollection<CustomSnapshotRoot>();
                   // CLogging.AddLog("null");
                }
                else
                {
                    //CLogging.AddLog("not Null");
                }
                int NumPlayers = root.snapshot.teamInfo.team0.player.Count + root.snapshot.teamInfo.team1.player.Count + root.snapshot.teamInfo.team2.player.Count;
                Servers[y].PlayersPlaying = NumPlayers;
                //CLogging.AddLog(String.Format("{0} players on server", NumPlayers));
                if (root.snapshot.roundTime != 0)
                {
                    Servers[y]._Snapshots.Add(root);
                }
                else
                    serversSkipped++;
                if (Servers[y].Snapshots.Count > 0 && root.snapshot.roundTime == 0)
                {
                    CLogging.AddLog(String.Format("{0} Saved", Servers[y].ServerGuid));
                    string output = JsonConvert.SerializeObject(Servers[y]);
                    FileHandler.Save(output, Servers[y].ServerGuid.ToString());
                    Servers[y].Snapshots.Clear();
                }
                //Servers[y] = item;
            }
            CLogging.AddLog(String.Format("{0} servers updated", y), LogType.Update);
            CheckServersForRemoval();
        }
        public static void CheckServersForRemoval()
        {

            foreach (var server in Servers)
            {
                TimeSpan lastUpdate = server.LastUpdate.TimeOfDay;
                TimeSpan timeNow = DateTime.Now.TimeOfDay;
                if((timeNow.TotalSeconds - lastUpdate.TotalSeconds) > 60)
                {
                    int i = Servers.IndexOf(server);
                    Servers.RemoveAt(i);
                    CLogging.AddLog(String.Format("Removed Server {0}", server._ServerGuid), LogType.Server);
                    return;
                }
            }
        }
    }
}
