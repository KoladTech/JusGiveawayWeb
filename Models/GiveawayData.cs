namespace JusGiveawayWebApp.Models
{
    public class GiveawayData
    {
        public int LeftoverGiveawayFunds { get; set; }
        public int MaxPossibleWinningsPerPerson { get; set; }
        public int MinCashoutPerPerson { get; set; }
        public int MonetaryResetPenalty { get; set; }
        public int NumberOfPlayers { get; set; }
        public int RoundBigLossMonetaryValue { get; set; }
        public int RoundBigWinMonetaryValue { get; set; }
        public int RoundDrawMonetaryValue { get; set; }
        public int RoundSmallLossMonetaryValue { get; set; }
        public int RoundSmallWinMonetaryValue { get; set; }
        public string Sponsor { get; set; }
        public string SponsorInstagramAccount { get; set; }
        public string StartDate { get; set; }
        public string Title { get; set; }
        public int TotalGiveAwayFunds { get; set; }
        public int TotalResetsAllowed { get; set; }
    }

}
