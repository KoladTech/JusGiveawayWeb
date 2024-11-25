using System.ComponentModel.DataAnnotations;

namespace JusGiveawayWebApp.Models
{
    public class UserGamePlayData
    {
        [Key] 
        public int Id { get; set; }
        public string UID { get; set; }
        public bool SelectedSides { get; set; }
        public bool PlayingHeads { get; set; }
        public int HeadsCount { get; set; }
        public int TailsCount { get; set; }
        public int MaxPossibleWinnings { get; set; }
        public int CurrentWinnings { get; set; }
        public int MinCashOut { get; set; }
        public int TotalResetsLeft { get; set; }
        public int LifetimeHeadsCount { get; set; }
        public int LifetimeTailsCount { get; set; }
        public bool GameOver { get; set; }
        public bool CashedOut { get; set; }
        public string CurrentGiveaway {  get; set; }

        public UserGamePlayData()
        {
            Id = 0;
            UID = "";
            SelectedSides = false;
            PlayingHeads = false;
            HeadsCount = 0;
            TailsCount = 0;
            MaxPossibleWinnings = 0;
            CurrentWinnings = 0;
            MinCashOut = 0;
            TotalResetsLeft = 0;
            LifetimeHeadsCount = 0;
            LifetimeTailsCount = 0;
            GameOver = false;
            CashedOut = false;
            CurrentGiveaway = "";
        }
    }
}
