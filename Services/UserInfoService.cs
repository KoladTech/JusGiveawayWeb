namespace JusGiveawayWebApp.Services
{
    public class UserInfoService
    {
        private string _uid;
        private bool _userSignedIn;
        public string? PlayerUID
        {
            get => _uid;
            set
            {
                _uid = value;
                NotifyStateChanged(); // Notify when UID changes
            }
        }
        public bool UserSignedIn
        {
            get => _userSignedIn;
            set
            {
                _userSignedIn = value;
                NotifyStateChanged(); // Notify when sign in changes
            }
        }

        // Event to notify state change
        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
