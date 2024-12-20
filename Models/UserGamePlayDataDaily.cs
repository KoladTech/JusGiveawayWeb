namespace JusGiveawayWebApp.Models
{
    public class UserGamePlayDataDaily
    {
        public string Choices { get; set; }
        public string LastAttempt { get; set; }
        public bool Winner { get; set; }
        public string ItemWon { get; set; }

        public UserGamePlayDataDaily()
        {
            Choices = "0";
            LastAttempt = string.Empty;
            Winner = false;
            ItemWon = string.Empty;
        }
    }

}
