using System.ComponentModel.DataAnnotations;

namespace JusGiveawayWebApp.Models
{
    public class UserGamePlayData
    {
        [Key] 
        public int Id { get; set; }
        public string UID { get; set; }
        public int SelectedSides { get; set; }
        public int PlayingHeads { get; set; }
        public int HeadsCount { get; set; }
        public int TailsCount { get; set; }
        public decimal MaxPossibleWinnings { get; set; }
        public decimal CurrentWinnings { get; set; }
        public decimal MinCashOut { get; set; }
        public int TotalResetsUsed { get; set; }

        public UserGamePlayData()
        {
            Id = 0;
            UID = "";
            SelectedSides = 0;
            PlayingHeads = 0;
            HeadsCount = 0;
            TailsCount = 0;
            MaxPossibleWinnings = 0;
            CurrentWinnings = 0;
            MinCashOut = 0;
            TotalResetsUsed = 0;
        }
    }
}
