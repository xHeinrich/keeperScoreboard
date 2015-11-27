using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace keeperScoreboard.Classes.Structs
{
    public class SettingsStruct
    {
        // Save the report when logger is closed
        public bool SaveOnClose { get; set; }
        public bool ScoreboardBackgrounds { get; set; }
        public bool MinimizeToTray { get; set; }
    }
}
