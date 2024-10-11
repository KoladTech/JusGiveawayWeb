using JusGiveawayWebApp.Models;
using JusGiveawayWebApp.Services;
using System.Threading.Tasks;

namespace JusGiveawayWebApp.Helpers
{
    public class CommonFunctions
    {
        private readonly FirebaseService _firebaseService;

        public CommonFunctions(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        public async Task<GiveawayData> GetGiveawayData()
        {
            //if null, display error
            return await _firebaseService.ReadDataAsync<GiveawayData>($"Giveaways/A");
        }

        // Add more common methods as needed.
    }

}
