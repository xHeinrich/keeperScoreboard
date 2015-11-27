using System.Collections.Generic;

namespace keeperScoreboard.Classes
{
    public class mapId
    {
        public List<mapNames> maps { get; set; }
    }
    public class mapNames
    {
        public int gameExpansion { get; set; }
        public List<long> gameModes { get; set; }
        public string id { get; set; }
        public string label { get; set; }
        public List<int> platforms { get; set; }
    }
    public class modeIds
    {
        //public JObject id { get; set; }
        public List<mapModes> modes { get; set; }
    }
    public class mapModes
    {
        public long id { get; set; }
        public string label { get; set; }
        public List<long> platforms { get; set; }
    }
}
