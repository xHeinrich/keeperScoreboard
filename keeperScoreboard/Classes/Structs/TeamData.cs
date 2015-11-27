using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace keeperScoreboard.Classes.Structs
{
    public class TeamData
    {
        public string TeamName { get; set; }
        public List<TeamMember> TeamMembers { get; set; }
        public string TeamLogoSquare { get; set; }
        public string TeamLogoRectangle { get; set; }
    }
    public class TeamMember
    {
        public enum CoreOrSub { core, substitute };
        public string MemberName { get; set; }
        public CoreOrSub MemberIsCoreOrSub {get;set;}
        public string MemberImage { get; set; }
    }
}
