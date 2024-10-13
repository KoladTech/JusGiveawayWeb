using JusGiveawayWebApp.Models;
using JusGiveawayWebApp.Pages;
using JusGiveawayWebApp.Services;
using Microsoft.AspNetCore.Components;
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

        public async Task<GiveawayData> GetGiveawayData()
        {
            //if null, display error
            return await _firebaseService.ReadDataAsync<GiveawayData>($"Giveaways/A");
        }

        public async Task<bool> SaveNewUserToFirebase(UserInfo newUser)
        {
            //if null, display error
            return await _firebaseService.WriteDataAsync<UserInfo>($"AllUsers/{newUser.UID}", newUser);
        }

        public async Task<string> GetUsersNameFromFirebase(string uid)
        {
            //if null, display error
            return await _firebaseService.ReadDataAsync<string>($"AllUsers/{uid}/name");
        }

        public UserInfo? GetUserInfoFromIndexedDb(JusGiveawayDB db, string uid)
        {
            return db.UserInfo.FirstOrDefault(u => u.UID == uid);
        }

        // Add more common methods as needed.
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
