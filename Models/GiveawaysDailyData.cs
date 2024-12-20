namespace JusGiveawayWebApp.Models
{
    public class GiveawaysDailyData
    {
        public Dictionary<string, Prize> Prizes { get; set; }
        public Dictionary<string, string> Winners { get; set; }
        public Dictionary<string, string> Sponsors { get; set; }
    }

    public class Prize
    {
        public Dictionary<int, bool> Locations { get; set; }
    }
}
