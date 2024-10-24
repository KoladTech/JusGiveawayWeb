namespace JusGiveawayWebApp.Services
{
    public class UserInfoService
    {
        private string _uid;
        public string? PlayerUID
        {
            get => _uid;
            set
            {
                _uid = value;
                NotifyStateChanged(); // Notify when UID changes
            }
        }

        // Event to notify state change
        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
