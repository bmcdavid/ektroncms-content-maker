namespace WSOL.EktronCms.ContentMaker
{
    using Ektron.Cms.Organization;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using WSOL.EktronCms.ContentMaker.Interfaces;
    using WSOL.EktronCms.ContentMaker.Models;
    using WSOL.IocContainer;

    /// <summary>
    /// Summary description for NavigationMaker
    /// </summary>
    public class NavigationFactory : INavigationFactory<Ektron.Cms.Organization.MenuData>
    {
        private static ICacheManager _CacheManager = InitializationContext.Locator.Get<ICacheManager>();

        private const string CK_MenuData = "WSOL:Cache:MenuData:ID={0}:L={1}:T={2}:FullTree{3}:User{4}";

        public static INavigation Get(long Id, int Language = 1033, int CacheInterval = 30, IEnumerable<INavigationResolver> ItemResolvers = null, bool adminMode = false)
        {
            var menu = GetSourceData(Id, Language, CacheInterval, true, adminMode);
            var navigation = MakeItem(menu, Language);

            if (navigation != null && ItemResolvers != null)
            {
                _ResolveItems(navigation.Items, ItemResolvers.Where(x => x.IsEnabled));
            }

            return navigation;
        }

        public static INavigation MakeItem(Ektron.Cms.Organization.MenuData menu, int Language = 1033)
        {
            if (menu == null)
                return null;

            Navigation nav = new Navigation()
            {
                Id = menu.Id,
                LanguageId = Language,
                Name = menu.Text,
                ParentId = menu.ParentId,
                Link = menu.Href,
                UseImage = menu.ImageOverride,
                Target = menu.Target,
                ImageLink = menu.ImagePath,
                Description = menu.Description
            };

            INavigationItem topItem;

            if (menu.Items == null)
                return nav;

            try
            {
                menu.Items.All(item =>
                {
                    topItem = new NavigationItem()
                    {
                        Type = item.Type.ToInt32().ToEnum<WSOL.EktronCms.ContentMaker.Enums.NavigationItemType>(Enums.NavigationItemType.Link),
                        Text = item.Text,
                        Description = item.Description,
                        Target = item.Target,
                        Link = item.Href,
                        UseImage = item.ImageOverride,
                        ImageLink = item.ImagePath,
                        ParentId = item.ParentId,
                        Id = item.ItemId,
                        Level = 0
                    };

                    var subMenu = item as MenuData;

                    if (subMenu != null)
                    {
                        topItem.AssociatedFolders = subMenu.AssociatedFolders;
                        topItem.AssociatedTemplates = subMenu.AssociatedTemplates;
                    }

                    if (item.Type == Ektron.Cms.Common.EkEnumeration.MenuItemType.SubMenu && item.Items.Count > 0)
                    {
                        topItem.Items = MakeChildren(item.Items, 1);
                    }

                    nav.Items.Add(topItem as NavigationItem);

                    return true;
                });
            }
            catch (Exception ex)
            {
                ex.Log(typeof(NavigationFactory).FullName, System.Diagnostics.EventLogEntryType.Error, menu);
            }

            return nav;
        }

        public static IEnumerable<INavigation> GetAll(int Language = -1)
        {
            var api = FrameworkFactory.Get<Ektron.Cms.Framework.Organization.MenuManager>(true, Language);

            Ektron.Cms.Organization.MenuCriteria criteria = new Ektron.Cms.Organization.MenuCriteria(MenuProperty.Text, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
            criteria.AddFilter(Ektron.Cms.Organization.MenuProperty.Id, Ektron.Cms.Common.CriteriaFilterOperator.InSubClause, "SELECT mnu_id FROM menu_tbl WHERE mnu_type = 0");
            criteria.PagingInfo.RecordsPerPage = int.MaxValue;
            var items = api.GetMenuList(criteria);

            return items.GroupBy(p => p.Id).Select(g => g.First()).Select(x => new Navigation() { Id = x.Id, Name = x.Text });//.ToDictionary(t => t.Id, t => t.Text);
        }

        public static Ektron.Cms.Organization.MenuData GetSourceData(long MenuID, int ContentLanguage = 1033, int CacheInterval = 30, bool getTree = false, bool adminMode = false)
        {
            string CacheKey = string.Format(CK_MenuData, MenuID, ContentLanguage, CacheInterval, getTree, adminMode ? -1 : FrameworkFactory.CurrentUserId());

            return _CacheManager.CacheItem<Ektron.Cms.Organization.MenuData>
            (
                CacheKey,
                () =>
                {
                    var api = FrameworkFactory<Ektron.Cms.Framework.Organization.MenuManager>.Get(adminMode, ContentLanguage);

                    if (getTree)
                        return api.GetTree(MenuID);

                    return api.GetMenu(MenuID);
                },
                CacheInterval
            );

        }

        #region Helpers

        private static IEnumerable<INavigationItem> MakeChildren(ICollection<IMenuItemData> items, int level)
        {
            List<INavigationItem> navItems = new List<INavigationItem>();
            INavigationItem childItem;

            items.All(subItem =>
            {
                childItem = new NavigationItem()
                {
                    Type = subItem.Type.ToInt32().ToEnum<WSOL.EktronCms.ContentMaker.Enums.NavigationItemType>(Enums.NavigationItemType.Link),
                    Text = subItem.Text,
                    Description = subItem.Description,
                    Target = subItem.Target,
                    Link = subItem.Href,
                    UseImage = subItem.ImageOverride,
                    ImageLink = subItem.ImagePath,
                    ParentId = subItem.ParentId,
                    Id = subItem.ItemId,
                    Level = level
                };
                
                var subMenu = subItem as MenuData;

                if (subMenu != null)
                {
                    childItem.AssociatedFolders = subMenu.AssociatedFolders;
                    childItem.AssociatedTemplates = subMenu.AssociatedTemplates;
                }

                if (subItem.Type == Ektron.Cms.Common.EkEnumeration.MenuItemType.SubMenu && subItem.Items.Count > 0)
                    childItem.Items = MakeChildren(subItem.Items, level + 1);

                navItems.Add(childItem);

                return true;
            });

            return navItems;
        }

        private static void _ResolveItems(IEnumerable<INavigationItem> navigationItems, IEnumerable<INavigationResolver> itemResolvers)
        {
            if (navigationItems == null || itemResolvers == null)
                return;

            navigationItems.All
            (
                x =>
                {
                    // Recursive
                    if (x.Items != null)
                        _ResolveItems(x.Items, itemResolvers);

                    // Run all resolvers for each item
                    itemResolvers.All(resolver =>
                    {
                        resolver.Resolve(x);
                        return true;
                    });

                    return true;
                }
            );
        }

        #endregion

        #region Interface

        INavigation INavigationFactory<MenuData>.Get(long Id, int Language, int CacheInterval, IEnumerable<INavigationResolver> ItemResolvers, bool adminMode)
        {
            return Get(Id, Language, CacheInterval, ItemResolvers, adminMode);
        }

        IEnumerable<INavigation> INavigationFactory<MenuData>.GetAll(int Language)
        {
            return GetAll(Language);
        }

        MenuData INavigationFactory<MenuData>.GetSourceData(long MenuID, int ContentLanguage, int CacheInterval, bool getTree, bool adminMode)
        {
            return GetSourceData(MenuID, ContentLanguage, CacheInterval, getTree, adminMode);
        }

        INavigation INavigationFactory<MenuData>.MakeItem(MenuData Item)
        {
            return MakeItem(Item);
        }

        #endregion

    }
}