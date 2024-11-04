using IndexedDB.Blazor;
using JusGiveawayWebApp.Models;
using JusGiveawayWebApp.Pages;
using JusGiveawayWebApp.Services;
using Microsoft.AspNetCore.Components;
using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks;
using static JusGiveawayWebApp.Pages.CustomAlertDialog;

namespace JusGiveawayWebApp.Helpers
{
    public class AdminFunctions
    {
        private readonly FirebaseService _firebaseService;
        public bool ShowDialog { get; set; } = false;
        public Action YesClickedAction { get; set; }
        public Action NoClickedAction { get; set; }
        private TaskCompletionSource<bool> _dialogResult;

        public AdminFunctions(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        #region Firebase admin functions
        
        public async Task<string> GetTestEmailsFromFirebase()
        {
            return await _firebaseService.ReadDataAsync<string>($"TestEmails", needsAuthToken: true);
        }

        public async Task<AdminCredentials> GetAdminCredentialsFromFirebase()
        {
            AdminCredentials adminCredentials = await _firebaseService.ReadDataAsync<AdminCredentials>($"AdminCredentials", needsAuthToken: true);

            if (adminCredentials == null)
                return new AdminCredentials();

            return adminCredentials;
        }

        public async Task<Dictionary<string, CashOutDetails>> GetCashOutDataFromFirebase(string giveawayName)
        {
            return await _firebaseService.ReadDataAsync<Dictionary<string,CashOutDetails>>($"CashOuts", needsAuthToken: true);
        }

        public async Task<Dictionary<string, int>> GetBingoDataFromFirebase(string giveawayName) 
        {
            return await _firebaseService.ReadDataAsync<Dictionary<string, int>>($"BingoRounds", needsAuthToken: true);
        }

        public async Task<Dictionary<string, UserGamePlayData>> GetAllUsersFromFirebase()
        {
            var allUsersData = await _firebaseService.ReadDataAsync<Dictionary<string, UserCombinedData>>($"AllUsers", needsAuthToken: true);

            var uids = allUsersData.Select(s => s.Key).ToList();
            Dictionary<string, UserGamePlayData> allUserGamePlayData = new Dictionary<string, UserGamePlayData>();
            foreach (string uid in uids)
            {
                var data = await _firebaseService.ReadDataAsync<UserGamePlayData>($"AllUsers/{uid}/GamePlayData", needsAuthToken: true);
                if (data != null)
                {
                    allUserGamePlayData.Add(uid, data);
                }
            }
            return allUserGamePlayData;
        }
        #endregion


        public async Task<List<WinnerData>> GetCashoutWinners(
        Func<CashOutDetails, int> fieldSelector)
        {
            List<WinnerData> winnersList = new List<WinnerData>();
            Dictionary<string, CashOutDetails> cashOutDataFromDB = await GetCashOutDataFromFirebase("A");
            
            foreach (var entry in cashOutDataFromDB)
            {
                var fullUsername = entry.Key.Split('-').Last();
                string displayName = fullUsername.Substring(0, 3) + "...";

                winnersList.Add(new WinnerData()
                {
                    WinnerName = displayName,
                    WinningAmount = fieldSelector(entry.Value)
                });
            }
            return winnersList;
        }

        public async Task<List<BingoData>> GetBingoWinners()
        {
            List<BingoData> bingoList = new List<BingoData>();
            Dictionary<string, int> bingoDataFromDB = await GetBingoDataFromFirebase("A");

            foreach (var entry in bingoDataFromDB)
            {
                var fullUsername = entry.Key.Split('-').Last();
                string displayName = fullUsername.Substring(0, 3) + "...";

                bingoList.Add(new BingoData()
                {
                    PlayerName = displayName,
                    BingoCount = entry.Value
                });
            }
            // Sort the list by BingoCount in descending order
            return bingoList.OrderByDescending(b => b.BingoCount).ToList();
        }

        public async Task<TotalHeadsOrTailsData> GetTotalHeadsOrTailsCounts()
        {
            TotalHeadsOrTailsData totalHeadsOrTailsData = new TotalHeadsOrTailsData();
            Dictionary<string, UserGamePlayData> allUsersFromDB = await GetAllUsersFromFirebase();

            foreach (var user in allUsersFromDB)
            {
                //var userInfo = user.Value.UserInfo;
                int playerTotalFlips = user.Value.HeadsCount + user.Value.LifetimeHeadsCount + user.Value.TailsCount + user.Value.LifetimeTailsCount;

                totalHeadsOrTailsData.TotalHeads += user.Value.HeadsCount;
                totalHeadsOrTailsData.TotalHeads += user.Value.LifetimeHeadsCount;
                totalHeadsOrTailsData.TotalTails += user.Value.TailsCount;
                totalHeadsOrTailsData.TotalTails += user.Value.LifetimeTailsCount;
                
                var userName = await _firebaseService.ReadDataAsync<string>($"AllUsers/{user.Value.UID}/UserInfo/name", needsAuthToken: true);

                if (userName != null)
                {
                    string displayName = "";
                    if(userName.Length < 3)
                    {
                        displayName = userName + "...";
                    }
                    else
                    {
                        displayName = userName.Substring(0, 3) + "...";
                    }
                    totalHeadsOrTailsData.PlayersWithHighestFlips.Add(displayName, playerTotalFlips);

                    if (totalHeadsOrTailsData.PlayersWithHighestFlips.Count > 10)
                    {
                        // Find the key with the smallest flip count
                        var playerWithMinFlips = totalHeadsOrTailsData.PlayersWithHighestFlips.Aggregate((x, y) => x.Value < y.Value ? x : y).Key;

                        // Remove the player with the smallest flip count
                        totalHeadsOrTailsData.PlayersWithHighestFlips.Remove(playerWithMinFlips);
                    }
                }
                else
                {
                    totalHeadsOrTailsData.PlayersWithHighestFlips.Add(user.Value.UID.Substring(0, 3) + "...", playerTotalFlips);

                    if (totalHeadsOrTailsData.PlayersWithHighestFlips.Count > 10)
                    {
                        // Find the key with the smallest flip count
                        var playerWithMinFlips = totalHeadsOrTailsData.PlayersWithHighestFlips.Aggregate((x, y) => x.Value < y.Value ? x : y).Key;

                        // Remove the player with the smallest flip count
                        totalHeadsOrTailsData.PlayersWithHighestFlips.Remove(playerWithMinFlips);
                    }
                }
            }
            //totalHeadsOrTailsData.PlayersWithHighestFlips = totalHeadsOrTailsData.PlayersWithHighestFlips.OrderByDescending(b => b.Value).ToList();
            return totalHeadsOrTailsData;
        }
    
    }

    /// <summary>
    /// Represents the admin credentials for accessing admin page
    /// </summary>
    public class AdminCredentials
    {
        public List<string> AdminEmails { get; set; } = new List<string>();
        public int AdminPassword { get; set; } = 0;

    }
    public class WinnerData
    {
        public string WinnerName { get; set; }
        public int WinningAmount { get; set; }
    }
    public class BingoData
    {
        public string PlayerName { get; set; }
        public int BingoCount { get; set; }
    }
    public class TotalHeadsOrTailsData
    {
        public int TotalHeads { get; set; }
        public int TotalTails { get; set; }
        public Dictionary<string, int> PlayersWithHighestFlips { get; set; } = new Dictionary<string, int>();
        public int HighestPlayerFlips { get; set; } = 0;
    }
    public class UserCombinedData
    {
        public UserInfo UserInfo { get; set; }
        public UserGamePlayData UserGamePlayData { get; set; }
    }

}
