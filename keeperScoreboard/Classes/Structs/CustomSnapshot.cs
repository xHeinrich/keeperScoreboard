using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace keeperScoreboard.Classes
{
    public class CustomSnapshotRoot
    {
        public long lastUpdated { get; set; }
        public CustomSnapshot snapshot { get; set; }
    }
    public class CustomSnapshot
    {
        public string status { get; set; }
        public ulong gameId { get; set; }
        public string currentMap { get; set; }
        public int maxPlayers { get; set; }
        public int waitingPlayers { get; set; }
        public double roundTime { get; set; }
        public int defaultRoundTimeMultiplier { get; set; }
        public string gameMode { get; set; }
        public string mapId { get; set; }
        public long modeId { get; set; }
        public modeCounter team1Tickets { get; set; }
        public modeCounter team2Tickets { get; set; }
        public CustomSnapshotTeamInfo teamInfo { get; set; }
    }
    public class modeCounter
    {
        public int tickets { get; set; }
        public int ticketsMax { get; set; }
    }
    public class CustomSnapshotTeamInfo
    {
        public CustomSnapshotTeam team0 { get; set; }
        public CustomSnapshotTeam team1 { get; set; }
        public CustomSnapshotTeam team2 { get; set; }
    }
    public class CustomSnapshotTeam
    {
        public int faction { get; set; }
        public List<playersData> player { get; set; }
    }
    public class playersData : IComparable<playersData>
    {
        public ulong playerId { get; set; }
        public int score { get; set; }
        public string name { get; set; }
        public string tag { get; set; }
        public int rank { get; set; }
        public int kills { get; set; }
        public int deaths { get; set; }
        public int squad { get; set; }
        public int role { get; set; }
        public int bombsDetonated { get; set; }
        public List<long> bombDetonationTime { get; set; }
        public int kit { get; set; }
        public ulong primaryWeapon { get; set; }
        public ulong secondaryWeapon { get; set; }
        public int CompareTo(playersData other)
        {
            // Alphabetic sort if salary is equal. [A to Z]
            if (this.score == other.score)
            {
                return this.name.CompareTo(other.name);
            }
            // Default to salary sort. [High to low]
            return other.score.CompareTo(this.score);
        }

        public override string ToString()
        {
            // String representation.
            return this.score.ToString() + "," + this.name;
        }
    }
}
