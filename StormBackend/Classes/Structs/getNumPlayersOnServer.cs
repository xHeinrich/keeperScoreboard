using Newtonsoft.Json;

namespace StormBackend
{
    public class getNumPlayersOnServer
    {
        public long mapMode { get; set; }
        public int players { get; set; }
        public Slots slots { get; set; }
        public int queued { get; set; }
        public string map { get; set; }
    }
    [JsonObject("8")]
    public class _8
    {
        public int current { get; set; }
        public int max { get; set; }
    }
    [JsonObject("1")]
    public class _1
    {
        public int current { get; set; }
        public int max { get; set; }
    }
    [JsonObject("2")]
    public class _2
    {
        public int current { get; set; }
        public int max { get; set; }
    }
    public class Slots
    {
        [JsonProperty("8")]
        public _8 __8 { get; set; }
        [JsonProperty("1")]
        public _1 __1 { get; set; }
        [JsonProperty("2")]
        public _2 __2 { get; set; }
    }
}
