﻿using IndexedDB.Blazor;
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
            return await _firebaseService.ReadDataAsync<GiveawayData>($"Giveaways/A", needsAuthToken: true);
        }
        public async Task<string> GetGiveawayStartDateFromFirebase()
        {
            //if null, display error
            return await _firebaseService.ReadDataAsync<string>($"Giveaways/A/StartDate", needsAuthToken: false);
        }
        public async Task<int> GetLeftoverGiveawayFundsFromFirebase()
        {
            //if null, display error
            return await _firebaseService.ReadDataAsync<int>($"Giveaways/A/LeftoverGiveawayFunds", needsAuthToken: true);
        }

        public async Task<int> GetCoinFlipIncrementFromFirebase()
        {
            //if null, display error
            int coinFlipIncrement = await _firebaseService.ReadDataAsync<int>($"CoinFlipIncrement", needsAuthToken: true);
            return coinFlipIncrement == 0 ? 1 : coinFlipIncrement;
        }

        public async Task<string> GetTestEmailsFromFirebase()
        {
            //if null, display error
            return await _firebaseService.ReadDataAsync<string>($"TestEmails", needsAuthToken: true);
        }

        public async Task<string> GetAdminEmailsFromFirebase()
        {
            //if null, display error
            return await _firebaseService.ReadDataAsync<string>($"AdminEmails", needsAuthToken: true);
        }

        public async Task<string> GetAppVersionFromFirebase()
        {
            //if null, display error
            return await _firebaseService.ReadDataAsync<string>("JGVersionNumber", needsAuthToken: false);
        }

        public async Task<bool> SaveNewUserToFirebase(UserInfo newUser)
        {
            //if null, display error
            return await _firebaseService.WriteDataAsync<UserInfo>($"AllUsers/{newUser.UID}/UserInfo", newUser, needsAuthToken: true);
        }

        public async Task<bool> SaveUserGamePlayDataToFirebase(UserGamePlayData userGamePlayData)
        {
            //if null, display error
            return await _firebaseService.WriteDataAsync<UserGamePlayData>($"AllUsers/{userGamePlayData.UID}/GamePlayData", userGamePlayData, needsAuthToken: true);
        }
        public async Task<bool> SaveUserSurveyToFirebase(bool interested)
        {
            if (interested)
            {
                int interestedCount = await _firebaseService.ReadDataAsync<int>($"Survey/Interested", needsAuthToken: false);
                return await _firebaseService.WriteDataAsync<int>($"Survey/Interested", ++interestedCount, needsAuthToken: false);
            }
            else
            {
                int notInterestedCount = await _firebaseService.ReadDataAsync<int>($"Survey/NotInterested", needsAuthToken: false);
                return await _firebaseService.WriteDataAsync<int>($"Survey/NotInterested", ++notInterestedCount, needsAuthToken: false);
            }
        }
        public async Task<bool> SaveUserStatsForCountDownPageToFirebase(bool demoClicked, bool instructionsClicked)
        {
            if (demoClicked)
            {
                int demoClicks = await _firebaseService.ReadDataAsync<int>("CountdownPage/ClickedDemo", needsAuthToken: false);
                return await _firebaseService.WriteDataAsync<int>("CountdownPage/ClickedDemo", ++demoClicks, needsAuthToken: false);
            }
            if (instructionsClicked)
            {
                int instructionsClicks = await _firebaseService.ReadDataAsync<int>("CountdownPage/ClickedInstructions", needsAuthToken: false);
                return await _firebaseService.WriteDataAsync<int>("CountdownPage/ClickedInstructions", ++instructionsClicks, needsAuthToken: false);
            }
            return false;
        }

        public async Task<string> GetUsersNameFromFirebase(string uid)
        {
            //if null, display error
            return await _firebaseService.ReadDataAsync<string>($"AllUsers/{uid}/UserInfo/name", needsAuthToken: true);
        }

        public async Task<UserGamePlayData?> GetUserGamePlayDataFromFirebase(string uid)
        {
            //if null, display error
            return await _firebaseService.ReadDataAsync<UserGamePlayData>($"AllUsers/{uid}/GamePlayData", needsAuthToken: true);
        }

        public async Task<int> GetUserCashOutAmountFromFirebase(string uid)
        {
            //if null, display error
            return await _firebaseService.ReadDataAsync<int>($"AllUsers/{uid}/GamePlayData/currentWinnings", needsAuthToken: true);
        }

        public async Task<bool> GetUserCashOutStatusFromFirebase(string uid)
        {
            //if null, display error
            return await _firebaseService.ReadDataAsync<bool>($"AllUsers/{uid}/GamePlayData/cashedOut", needsAuthToken: true);
        }

        public async Task<bool> SaveUserCashOutDetailsToFirebase(UserInfo user, CashOutDetails userCashOutDetails)
        {
            //if null, display error
            return await _firebaseService.WriteDataAsync<CashOutDetails>($"CashOuts/{user.UID}-{user.Name}", userCashOutDetails, needsAuthToken: true);
        }

        public async Task<bool> SetUserHasCashedOutToFirebase(UserInfo user)
        {
            //if null, display error
            return await _firebaseService.WriteDataAsync<bool>($"AllUsers/{user.UID}/GamePlayData/cashedOut", true, needsAuthToken: true);
        }

        public async Task<bool> UpdateLeftoverGiveawayFundsInFirebase(int leftoverFunds)
        {
            return await _firebaseService.WriteDataAsync<int>("Giveaways/A/LeftoverGiveawayFunds", leftoverFunds, needsAuthToken: true);
        }
        public async Task<bool> RecordBingoRoundsToFirebase(string uid, string playerName)
        {
            var bingoCounts = await _firebaseService.ReadDataAsync<int>($"BingoRounds/{uid}-{playerName}", needsAuthToken: true);
            return await _firebaseService.WriteDataAsync<int>($"BingoRounds/{uid}-{playerName}", ++bingoCounts, needsAuthToken: true);
        }

        public async Task WriteErrorMessageToFirebase(string errorMessage, string data, string errorTime)
        {
            await _firebaseService.WriteErrorMessagesAsync(errorMessage, data, errorTime, needsAuthToken: false);
        }
        #endregion

        #region IndexedDB Functions
        public UserInfo? GetUserInfoFromIndexedDb(JusGiveawayDB db, string uid)
        {
            return db.UserInfo.FirstOrDefault(u => u.UID == uid);
        }
        public bool GetSurveyResultFromIndexedDb(JusGiveawayDB db)
        {
            var user = db.UserInfo.FirstOrDefault(u => u.TookSurvey == true);
            return user?.TookSurvey ?? false;
        }
        public bool GetTesterFromIndexedDb(JusGiveawayDB db)
        {
            var user = db.UserInfo.FirstOrDefault(u => u.Tester == true);
            return user?.Tester ?? false;
        }

        public UserGamePlayData? GetUserGamePlayDataFromIndexedDb(JusGiveawayDB db, string uid)
        {
            return db.UserGameDatas.FirstOrDefault(u => u.UID == uid);
        }

        public async Task SignOutUser(IIndexedDbFactory DbFactory, string uid)
        {
            try
            {
                //await FirebaseService.SignOutAsync();
                using (var db = await DbFactory.Create<JusGiveawayDB>())
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
            }
            catch (Exception ex)
            {
                await WriteErrorMessageToFirebase(ex.Message, "SignOutUser - "+uid, DateTime.Now.ToString());
                Console.WriteLine(ex.Message);
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

        public async Task SaveUserProgressOnExit(IIndexedDbFactory DbFactory, string playerUID, int headsCount, int tailsCount)
        {
            var userGamePlayData = new UserGamePlayData();

            //get most recent data from indexeddb
            try
            {
                using (var db = await DbFactory.Create<JusGiveawayDB>())
                {
                    userGamePlayData = GetUserGamePlayDataFromIndexedDb(db, playerUID);
                }
            }
            catch (Exception ex)
            {
                await WriteErrorMessageToFirebase(ex.Message, "SaveUserProgressOnExit - " + playerUID, DateTime.Now.ToString());
                Console.WriteLine(ex.Message);
            }

            //save usergameplaydata to firebase
            if (userGamePlayData != null)
            {
                Console.WriteLine("Saving game play data, on exiting the HeadsOrTails page...");
                userGamePlayData.HeadsCount = headsCount;
                userGamePlayData.TailsCount = tailsCount;
                try
                {
                    bool userSaved = await SaveUserGamePlayDataToFirebase(userGamePlayData);
                    if (!userSaved)
                    {
                        Console.WriteLine($"Error saving gameplay data to firebase"); 
                        await WriteErrorMessageToFirebase("Error saving gameplay data to firebase", "SaveUserProgressOnExit - " + playerUID, DateTime.Now.ToString());
                    }
                }
                catch (Exception x)
                {
                    Console.WriteLine($"Error saving gameplay data to firebase - {x.Message}");
                }
            }
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
