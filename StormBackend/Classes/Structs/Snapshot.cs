using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StormBackend
{
    class Snapshot
    {
        public string status { get; set; }
        public ulong gameId { get; set; }
        public string gameMode { get; set; }
        public int mapVariant { get; set; }
        public string currentMap { get; set; }
        public int maxPlayers { get; set; }
        public int waitingPlayers { get; set; }
        public double roundTime { get; set; }
        public int defaultRoundTimeMultiplier { get; set; }

        [JsonProperty("deathmatch")]
        public Deathmatch Deathmatch { get; set; }
        [JsonProperty("conquest")]
        public Conquest Conquest { get; set; }
        public TeamInfo teamInfo { get; set; }
    }
    class SnapshotRoot
    {
        public long lastUpdated { get; set; }
        public Snapshot snapshot { get; set; }
    }
    public class ServerTickets
    {
        public int tickets { get; set; }
        public int ticketsMax { get; set; }
    }

    public class Conquest
    {
        [JsonProperty("1")]
        public ServerTickets Team1 { get; set; }

        [JsonProperty("2")]
        public ServerTickets Team2 { get; set; }
    }


    public class DeathmatchTeamStats
    {
        [JsonProperty("kills")]
        public int Kills { get; set; }

        [JsonProperty("killsMax")]
        public int KillsMax { get; set; }
    }

    public class Deathmatch
    {
        [JsonProperty("1")]
        public DeathmatchTeamStats Team1 { get; set; }

        [JsonProperty("2")]
        public DeathmatchTeamStats Team2 { get; set; }

        [JsonProperty("3")]
        public DeathmatchTeamStats Team3 { get; set; }

        [JsonProperty("4")]
        public DeathmatchTeamStats Team4 { get; set; }
    }

    public class TeamInfo
    {
        [JsonProperty("0")]
        public Team Team0 { get; set; }

        [JsonProperty("1")]
        public Team Team1 { get; set; }

        [JsonProperty("2")]
        public Team Team2 { get; set; }

        [JsonProperty("3")]
        public Team Team3 { get; set; }

        [JsonProperty("4")]
        public Team Team4 { get; set; }
    }

    public class Team
    {
        public int faction { get; set; }
        public JObject players { get; set; }
    }

    public class TeamPlayer
    {
        [JsonProperty("tag")]
        public string tag;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("rank")]
        public int rank;

        [JsonProperty("score")]
        public int score;

        [JsonProperty("kills")]
        public int kills;

        [JsonProperty("deaths")]
        public int deaths;

        [JsonProperty("squad")]
        public int squad;

        [JsonProperty("role")]
        public int role;
    }
}
