using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace JusGiveawayWebApp.Models
{
    public class GiveawayData
    {
        [Key] 
        public int Id { get; set; }
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

        // Method to get size of object in bytes
        public long GetSizeInBytes(GiveawayData data)
        {
            // Serialize to JSON
            string json = JsonSerializer.Serialize(data);

            // Convert to bytes
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(json);

            // Return the size in bytes
            return byteArray.Length;
        }
        public long GetSizeInBytes(int singleData)
        {
            // Serialize to JSON
            string json = JsonSerializer.Serialize(singleData);

            // Convert to bytes
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(json);

            // Return the size in bytes
            return byteArray.Length;
        }
    }

}
