using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace JusGiveawayWebApp.Models
{
    public class UserInfo
    {
        [Key]
        public int Id { get; set; }
        public string UID { get; set; }
        [NotNull]
        public string IdToken { get; set; }
        [NotNull]
        public string Name { get; set; }
        public string? PhoneNumber { get; set; }
        [NotNull]
        public string EmailAddress { get; set; }
        public string? DeviceInfo { get; set; }

        public UserInfo()
        {
            Id = 0;
            UID = "";
            IdToken = "";
            Name = "";
            PhoneNumber = null;
            EmailAddress = "";
            DeviceInfo = null;
        }
    }
}
