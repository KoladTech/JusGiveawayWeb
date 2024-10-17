using System.Linq;

namespace JusGiveawayWebApp.Helpers
{
    public class CustomNavigationStack
    {
        public Stack<Dictionary<string, string>> Pages { get; set; } = new Stack<Dictionary<string, string>>();

        public void AddPage(string pageName, string playerUID)
        {
            Pages.Push(new Dictionary<string, string>
            {
                { "PageName", pageName },
                { "PlayerUID", playerUID }
            });
        }

        public Dictionary<string, string> PopPage()
        {
            return Pages.Pop();
        }

        public Dictionary<string, string> PeekPage()
        {
            return Pages.Peek();
        }

        // Method to find a page by its name
        public Dictionary<string, string>? FindPageByName(string pageName)
        {
            // Search through the stack for a dictionary that contains the specified page name
            return Pages.FirstOrDefault(page => page.ContainsKey("PageName") && page["PageName"] == pageName);
        }
    }
}
