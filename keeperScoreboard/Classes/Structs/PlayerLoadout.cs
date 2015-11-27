using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace keeperScoreboard.Classes.Structs
{
    public class currentLoadoutWeapons
    {
        public ulong weaponId { get; set; }
        public List<ulong> weaponAttachments { get; set; }
    }
    public class kits
    {
        public byte kitId { get; set; }
        public List<ulong> kitIdInformation { get; set; }
    }
    public class PlayerLoadout
    {
        public ulong playerId { get; set; }
        public byte selectedKit { get; set; }
        public List<currentLoadoutWeapons> weaponList { get; set; }
        public List<kits> kitList { get; set; }
    }
}
