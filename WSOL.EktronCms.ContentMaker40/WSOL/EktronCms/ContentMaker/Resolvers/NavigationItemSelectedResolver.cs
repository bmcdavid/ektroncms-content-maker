namespace WSOL.EktronCms.ContentMaker.Resolvers
{
    using System.Linq;
    using System.Web;
    using WSOL.EktronCms.ContentMaker.Extensions;
    using WSOL.EktronCms.ContentMaker.Interfaces;
    using WSOL.IocContainer;

    /// <summary>
    /// Summary description for SelectedResolver
    /// </summary>
    public class NavigationItemSelectedResolver : INavigationResolver
    {
        private static ICacheManager _CacheManager = InitializationContext.Locator.Get<ICacheManager>();

        public virtual int CacheInterval { get { return _CacheManager.QuickInterval; } }

        public virtual bool IsEnabled { get { return true; } }

        public virtual void Resolve(INavigationItem item)
        {
            item.Selected = GetIsSelectedMenuItem(item);
            item.ChildSelected = item.Items != null ? item.Items.Where(y => y.Selected || y.ChildSelected).Any() : false;
        }

        /// <summary>
        /// Determines if menu item is selected
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private static bool GetIsSelectedMenuItem(INavigationItem item)
        {
            IContent content = item.GetDynamicContent();

            if (HttpContext.Current == null || content == null)
                return false;
            else if (item.Type == WSOL.EktronCms.ContentMaker.Enums.NavigationItemType.Content && item.Id == content.Id)
                return true;
            else if (!string.IsNullOrEmpty(item.Link) && item.Link.ToLower().EndsWith(HttpContext.Current.Request.Url.PathAndQuery.ToLower()))
                return true;
            // Changed from Contains since matching links like /jobs when RawUrl is /jobs/add
            else if (!string.IsNullOrEmpty(item.Link) && item.Link.ToLower() == HttpContext.Current.Request.RawUrl.ToLower().Split('?')[0])
                return true;
            else
            {
                if (item.Type == WSOL.EktronCms.ContentMaker.Enums.NavigationItemType.SubNavigation)
                {
                    if (item.AssociatedTemplates.Where(x => HttpContext.Current.Request.Url.PathAndQuery.ToLower().Contains(x.ToLower())).Any())
                    {
                        return true;
                    }

                    if (content.Id > 0)
                    {
                        return item.AssociatedFolders.Contains(content.FolderId);
                    }
                }
            }

            return false;
        }
    }
}