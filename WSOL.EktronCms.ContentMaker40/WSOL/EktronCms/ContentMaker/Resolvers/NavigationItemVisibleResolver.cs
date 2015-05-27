namespace WSOL.EktronCms.ContentMaker.Resolvers
{
    using Ektron.Cms.Common;
    using Ektron.Cms.Framework.Settings.UrlAliasing;
    using Ektron.Cms.Settings.UrlAliasing.DataObjects;
    using System.Web;
    using WSOL.EktronCms.ContentMaker.Extensions;
    using WSOL.EktronCms.ContentMaker.Interfaces;
    using WSOL.IocContainer;

    /// <summary>
    /// Summary description for SelectedResolver
    /// </summary>
    public class NavigationItemVisibleResolver : INavigationResolver
    {
        private static ICacheManager _CacheManager = InitializationContext.Locator.Get<ICacheManager>();

        public virtual int CacheInterval { get { return _CacheManager.QuickInterval; } }

        public virtual bool IsEnabled { get { return true; } }

        public virtual void Resolve(INavigationItem item)
        {
            if (item == null)
                return;

            item.Visible = IsVisible(item.Link, CacheInterval);
        }

        /// <summary>
        /// Determines if a menu items href is viewable or not by the current user
        /// </summary>
        /// <param name="hyperlink">hyperlink string</param>
        /// <param name="cache">False disables caching</param>
        /// <returns>true/false</returns>
        public static bool IsVisible(string hyperlink, int cacheInterval)
        {
            if (string.IsNullOrWhiteSpace(hyperlink))
                return false;

            if (hyperlink == "#")
                return true;

            return _CacheManager.CacheItem<bool>
            (
                string.Format("WSOL:Cache:MenuItemVisible:U={0}:Href={1}", FrameworkFactory.CurrentUserId(), HttpUtility.UrlEncode(hyperlink)),
                () =>
                {
                    string[] url = hyperlink.Split('?');
                    long contentID = 0;

                    if (url.Length == 2)
                    {
                        string[] parameters = url[1].Split('&');

                        foreach (string parameter in parameters)
                        {
                            if (parameter.StartsWith("id="))
                            {
                                long.TryParse(parameter.Split('=')[1], out contentID);
                                break;
                            }
                            else if (parameter.StartsWith("ekfrm="))
                            {
                                long.TryParse(parameter.Split('=')[1], out contentID);
                                break;
                            }
                            else if (parameter.StartsWith("pageid="))
                            {
                                long.TryParse(parameter.Split('=')[1], out contentID);
                                break;
                            }
                        }

                    }

                    // Still No Content ID found, try to find by alias
                    if (contentID == 0)
                    {
                        try
                        {
                            var manager = FrameworkFactory<AliasManager>.Get(true);
                            var criteria = new AliasCriteria();

                            // Prep alias, trim beginning slash and URL parameters as aliases don't store them
                            string searchAlias = hyperlink.TrimStart('/').Split('?')[0];

                            criteria.AddFilter(AliasProperty.Alias, CriteriaFilterOperator.EqualTo, searchAlias);
                            criteria.PagingInfo.RecordsPerPage = 1;
                            var items = manager.GetList(criteria);

                            if (items.Count > 0)
                                contentID = items[0].TargetId;
                        }
                        catch
                        {
                            // Catch is for framework throwing license key violations
                        }

                    }

                    // Skip permission check if content is still 0
                    if (contentID == 0)
                        return true;

                    var viewable = contentID.IsViewable();

                    return viewable.HasValue && viewable.Value;

                },
                cacheInterval
            );

        }
    }
}