namespace WSOL.EktronCms.ContentMaker.Resolvers
{
    using Interfaces;

    public class NavigationItemFixRelativeLinkResolver : INavigationResolver
    {
        public bool IsEnabled
        {
            get { return true; }
        }

        public int CacheInterval
        {
            get { return 0; }
        }

        public void Resolve(INavigationItem item)
        {
            if (!item.Link.Contains("://") && !item.Link.StartsWith("/"))
            {
                item.Link = "/" + item.Link;
            }
        }
    }
}