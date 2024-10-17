using JusGiveawayWebApp.Models;
using JusGiveawayWebApp.Pages;
using JusGiveawayWebApp.Services;
using Microsoft.AspNetCore.Components;
using System.Text.Json;
using System.Threading.Tasks;
using static JusGiveawayWebApp.Pages.CustomAlertDialog;

namespace JusGiveawayWebApp.Helpers
{
    public class CommonFunctions
    {
        private readonly FirebaseService _firebaseService;
        public bool ShowDialog { get; set; } = false;
        public Action YesClickedAction { get; set; }
        public Action NoClickedAction { get; set; }
        private TaskCompletionSource<bool> _dialogResult;

        public CommonFunctions(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        #region Firebase functions
        public async Task<GiveawayData> GetGiveawayDataFromFirebase()
        {
            //if null, display error
            return await _firebaseService.ReadDataAsync<GiveawayData>($"Giveaways/A");
        }
        public async Task<int> GetLeftoverGiveawayFundsFromFirebase()
        {
            //if null, display error
            return await _firebaseService.ReadDataAsync<int>($"Giveaways/A/LeftoverGiveawayFunds");
        }


        public async Task<bool> SaveNewUserToFirebase(UserInfo newUser)
        {
            //if null, display error
            return await _firebaseService.WriteDataAsync<UserInfo>($"AllUsers/{newUser.UID}/UserInfo", newUser);
        }

        public async Task<bool> SaveUserGamePlayDataToFirebase(UserGamePlayData userGamePlayData)
        {
            //if null, display error
            return await _firebaseService.WriteDataAsync<UserGamePlayData>($"AllUsers/{userGamePlayData.UID}/GamePlayData", userGamePlayData);
        }

        public async Task<string> GetUsersNameFromFirebase(string uid)
        {
            //if null, display error
            return await _firebaseService.ReadDataAsync<string>($"AllUsers/{uid}/UserInfo/name");
        }

        public async Task<UserGamePlayData?> GetUserGamePlayDataFromFirebase(string uid)
        {
            //if null, display error
            return await _firebaseService.ReadDataAsync<UserGamePlayData>($"AllUsers/{uid}/GamePlayData");
        }

        public async Task<int> GetUserCashOutAmountFromFirebase(string uid)
        {
            //if null, display error
            return await _firebaseService.ReadDataAsync<int>($"AllUsers/{uid}/GamePlayData/currentWinnings");
        }

        public async Task<bool> SaveUserCashOutDetailsToFirebase(UserInfo user, CashOutDetails userCashOutDetails)
        {
            //if null, display error
            return await _firebaseService.WriteDataAsync<CashOutDetails>($"CashOuts/{user.UID}", userCashOutDetails);
        }

        public async Task<bool> SetUserHasCashedOutToFirebase(UserInfo user)
        {
            //if null, display error
            return await _firebaseService.WriteDataAsync<bool>($"AllUsers/{user.UID}/GamePlayData/cashedOut", true);
        }

        public async void UpdateLeftoverGiveawayFundsInFirebase(int leftoverFunds)
        {
            await _firebaseService.WriteDataAsync<int>("Giveaways/A/LeftoverGiveawayFunds", leftoverFunds);
        }
        #endregion

        #region IndexedDB Functions
        public UserInfo? GetUserInfoFromIndexedDb(JusGiveawayDB db, string uid)
        {
            return db.UserInfo.FirstOrDefault(u => u.UID == uid);
        }

        public UserGamePlayData? GetUserGamePlayDataFromIndexedDb(JusGiveawayDB db, string uid)
        {
            return db.UserGameDatas.FirstOrDefault(u => u.UID == uid);
        }

        public async Task SignOutUser(JusGiveawayDB db, string uid)
        {   
            var existingUser = GetUserInfoFromIndexedDb(db, uid);

            if (existingUser != null)
            {
                // Update the existing user's information
                existingUser.IdToken = "";

                // Save changes
                await db.SaveChanges();
            }
        }
        #endregion

        public bool SendUserBackToLogInPageIfNotLoggedIn(UserInfo currentUser)
        {
            //check if user is logged in
            if (currentUser.IdToken == "" || currentUser.IdToken == null)
            {
                //send user back to log in page
                return true;
            }
            return false;
        }

        // Add more common methods as needed.




        // Method to get size of object in bytes
        //public long GetSizeInBytes(T data)
        //{
        //    // Serialize to JSON
        //    string json = JsonSerializer.Serialize(data);

        //    // Convert to bytes
        //    byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(json);

        //    // Return the size in bytes
        //    return byteArray.Length;
        //}
        //public long GetSizeInBytes(int singleData)
        //{
        //    // Serialize to JSON
        //    string json = JsonSerializer.Serialize(singleData);

        //    // Convert to bytes
        //    byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(json);

        //    // Return the size in bytes
        //    return byteArray.Length;
        //}
    }

    /// <summary>
    /// Represents the parameters for displaying an alert dialog.
    /// </summary>
    public class AlertDialogParams
    {
        /// <summary>
        /// Gets or sets the title of the alert dialog.
        /// </summary>
        public string AlertTitle { get; set; } = "Alert Title";

        /// <summary>
        /// Gets or sets the message to be displayed in the alert dialog.
        /// This can include HTML markup.
        /// </summary>
        public MarkupString AlertMessage { get; set; } = new MarkupString("This is the alert message.");

        /// <summary>
        /// Gets or sets the text for the primary button.
        /// </summary>
        public string PrimaryBtnText { get; set; } = "Yes";

        /// <summary>
        /// Gets or sets the text for the secondary button.
        /// </summary>
        public string SecondaryBtnText { get; set; } = "No";

        /// <summary>
        /// Gets or sets the callback for when the primary button is clicked.
        /// </summary>
        public EventCallback OnPrimaryButtonClick { get; set; }

        /// <summary>
        /// Gets or sets the callback for when the secondary button is clicked.
        /// </summary>
        public EventCallback OnSecondaryButtonClick { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the primary button.
        /// </summary>
        public bool ShowPrimaryBtn { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to show the secondary button.
        /// </summary>
        public bool ShowSecondaryBtn { get; set; } = true;

        /// <summary>
        /// Gets or sets the type of alert dialog to be displayed.
        /// </summary>
        public AlertType DialogAlertType { get; set; } = AlertType.Info;
    }
}
