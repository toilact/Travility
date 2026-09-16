namespace Travility.Core.Session
{
    public sealed class NavigationItem
    {
        public string Key { get; }
        public string Title { get; }
        public string IconName { get; }

        public NavigationItem(string key, string title, string iconName = null)
        {
            Key = key;
            Title = title;
            IconName = iconName;
        }
    }
}
