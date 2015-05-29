namespace WSOL.EktronCms.ContentMaker
{
    using Ektron.Cms.Framework.Core.Folder;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using WSOL.EktronCms.ContentMaker.Extensions;
    using WSOL.EktronCms.ContentMaker.Interfaces;
    using WSOL.IocContainer;

    public static class SettingsFactory<T> where T : IContent, ISettings
    {
        private static ICacheManager _CacheManager = InitializationContext.Locator.Get<ICacheManager>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settingsResolver">Settings resolver to customize settings object returned.</param>
        /// <param name="adminMode">Determines if admin mode is used or not.</param>
        /// <param name="refresh">If true, and resolver is set, the setting value gets reloaded.</param>
        /// <returns></returns>
        public static T GetSiteSettings(ISettingsResolver<T> settingsResolver, bool adminMode, bool refreshValue)
        {
            string key = string.Concat(typeof(T).FullName, adminMode);

            T resolvedSetting = HttpContext.Current.GetHttpContextItem
            (
                key,
                () =>
                {
                    IEnumerable<IContent> content = _CacheManager.CacheItem
                    (
                        key,
                        () =>
                        {
                            long id = typeof(T).GetXmlConfigId();

                            // Pull first of its kind
                            var criteria = key.GetCriteria();
                            criteria.AddFilter(Ektron.Cms.Common.ContentProperty.XmlConfigurationId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, id);
                            criteria.OrderByDirection = Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending;
                            criteria.OrderByField = Ektron.Cms.Common.ContentProperty.DateCreated;
                            criteria.PagingInfo.RecordsPerPage = 100;

                            return criteria.GetContent(-1, adminMode);
                        },
                        _CacheManager.ShortInterval
                    );

                    T setting = default(T);

                    if (content.Any())
                    {
                        // First Try Requested Language
                        foreach (var c in content.Where(x => x.LanguageId == WSOL.EktronCms.ContentMaker.FrameworkFactory.CurrentLanguage(false)))
                        {
                            setting = (T)c;
                            break;
                        }

                        // If still here try the default langauge
                        foreach (var c in content.Where(x => x.LanguageId == WSOL.EktronCms.ContentMaker.FrameworkFactory.DefaultLanguage))
                        {
                            setting = (T)c;
                            break;
                        }
                    }
                    else
                    {
                        setting.ErrorMessage = "No settings objects were found";
                    }

                    if (settingsResolver != null)
                        setting = settingsResolver.Resolve(setting);

                    return setting;
                },
                refreshValue
            );

            return resolvedSetting;
        }

        /// <summary>
        /// Gets settings for Dynamic Content Item
        /// </summary>
        /// <returns></returns>
        public static T GetFolderSettings()
        {
            return GetFolderSettings((true).GetDynamicContent(), null, null, false);
        }

        /// <summary>
        /// Gets settings for given content Item
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static T GetFolderSettings(IContent content)
        {
            return GetFolderSettings(content, null, null, false);
        }

        /// <summary>
        /// Gets settings object that implements ISettings
        /// </summary>
        /// <param name="content"></param>
        /// <param name="FolderScope"></param>
        /// <param name="settingsResolver"></param>
        /// <param name="refresh"></param>
        /// <returns></returns>
        public static T GetFolderSettings(IContent content, IEnumerable<long> FolderScope, ISettingsResolver<T> settingsResolver = null, bool refreshValue = false)
        {
            if (content == null)
            {
                content = new WSOL.EktronCms.ContentMaker.Models.HtmlContent()
                {
                    FolderId = 0,
                    LanguageId = FrameworkFactory.CurrentLanguage(false)
                };
            }

            string key = string.Format("WSOL:Cache:Settings={0}:FolderId={1}", typeof(T).FullName, content.FolderId);

            T resolvedSetting = HttpContext.Current.GetHttpContextItem
            (
                key,
                () =>
                {
                    var settingsList = GetSettingsList(FolderScope, content.LanguageId);

                    var cachedSettings = _CacheManager.CacheItem<T>
                    (
                        key,
                        () =>
                        {
                            var folderSettings = settingsList.OfType<ISettings>();

                            if (folderSettings.Any())
                            {
                                var matches = folderSettings.Where(x => x.FolderIds.Contains(content.FolderId));

                                if (matches.Any())
                                {
                                    return (T)matches.First();
                                }
                                else
                                {
                                    List<long> folderIdWithPath = _CacheManager.CacheItem
                                        (
                                            string.Format("WSOL:Cache:FolderIdPath:ID={0}", content.FolderId),
                                            () =>
                                            {
                                                // Get the Folder ID path
                                                var folderManager = WSOL.EktronCms.ContentMaker.FrameworkFactory<Folder>.Get(true, -1);
                                                var list = new List<long>();

                                                while (true)
                                                {
                                                    var folder = folderManager.GetItem(content.FolderId);

                                                    if (folder != null)
                                                    {
                                                        list.Add(folder.Id);

                                                        if (folder.ParentId == 0)
                                                        {
                                                            list.Add(folder.ParentId);
                                                            break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        list.Add(0);
                                                        break;
                                                    }

                                                }

                                                return list;
                                            },
                                            _CacheManager.ShortInterval
                                        );

                                    if (folderIdWithPath != null && folderIdWithPath.Count > 0)
                                    {
                                        foreach (long folderId in folderIdWithPath)
                                        {
                                            matches = folderSettings.Where(x => x.FolderIds.Contains(folderId));

                                            if (matches.Any())
                                            {
                                                return (T)matches.First();
                                            }
                                        }
                                    }
                                }
                            }

                            T setting = (T)Activator.CreateInstance<T>();
                            setting.ErrorMessage = "No matches found for parent ID = " + content.FolderId;

                            return setting;
                        },
                        1 // Really just need it briefly
                    );

                    if (settingsResolver != null)
                        cachedSettings = settingsResolver.Resolve(cachedSettings);

                    return cachedSettings;
                },
                refreshValue
            );

            return resolvedSetting;
        }

        /// <summary>
        /// Gets object list of all settings smart forms of Type T for given language
        /// </summary>
        /// <param name="FolderScope"></param>
        /// <param name="LanguageId"></param>
        /// <returns></returns>
        private static IEnumerable<T> GetSettingsList(IEnumerable<long> FolderScope, int LanguageId)
        {
            string FolderIdsVariant = string.Empty;

            if (FolderScope != null && FolderScope.Any())
                FolderIdsVariant = ":Scope=" + string.Join(",", FolderScope.Select(x => x.ToString()).ToArray());

            IPagedCachedData<T> cache = _CacheManager.CacheItem
            (
                string.Format("WSOL:Cache:AllSettings={0}:LanguageId={1}{2}", typeof(T).FullName, LanguageId, FolderIdsVariant),
                () =>
                {
                    var criteria = (true).GetCriteria();
                    criteria.OrderByField = Ektron.Cms.Common.ContentProperty.Id;
                    criteria.OrderByDirection = Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending;
                    criteria.PagingInfo.RecordsPerPage = int.MaxValue;

                    long typeId = typeof(T).GetXmlConfigId();
                    criteria.AddFilter(Ektron.Cms.Common.ContentProperty.XmlConfigurationId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, typeId);

                    if (FolderScope != null && FolderScope.Any())
                    {
                        criteria.AddFilter(Ektron.Cms.Common.ContentProperty.FolderId, Ektron.Cms.Common.CriteriaFilterOperator.In, FolderScope);
                    }

                    var pagedCache = InitializationContext.Locator.Get<IPagedCachedData<T>>();
                    pagedCache.Items = criteria.GetContent(LanguageId, true).OfType<T>();

                    return pagedCache;

                },
                _CacheManager.DefaultInterval
            );

            return cache.Items;
        }
    }
}