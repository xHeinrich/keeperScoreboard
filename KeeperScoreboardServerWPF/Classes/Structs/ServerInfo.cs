using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperScoreboardServerWPF.Classes.Structs
{
    public class ServerInfo
    {
        public Guid serverGuid { get; set; }
        public string serverName { get; set; }
        public int tickRate { get; set; }
        public int playerSlots { get; set; }
        public string mapID { get; set; }
        public string location { get; set; }
    }
    public class ServerInfoByTime
    {
        public DateTime time { get; set; }
        public List<ServerInfo> info { get; set; }
    }
}
