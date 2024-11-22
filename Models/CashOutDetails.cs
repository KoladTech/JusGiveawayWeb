namespace JusGiveawayWebApp.Models
{
    public class CashOutDetails
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string UID { get; set; }
        public required string EmailAddress { get; set; }
        public string? Sex { get; set; }
        public required string DeviceInfo { get; set; }
        public required string BankAccountNumber { get; set; }
        public required string BankName { get; set; }
        public required int CashoutAmount { get; set; }
        public string? InstagramAccount { get; set; }
        public required string CashoutTime { get; set; }
    }
}
