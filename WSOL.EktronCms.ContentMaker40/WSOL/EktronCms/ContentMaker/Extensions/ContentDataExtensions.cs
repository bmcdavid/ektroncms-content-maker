namespace WSOL.EktronCms.ContentMaker.Extensions
{
    using Ektron.Cms;
    using Ektron.Cms.Common;
    using Ektron.Cms.Content;
    using Ektron.Cms.Organization;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using WSOL.EktronCms.ContentMaker.Interfaces;
    using WSOL.IocContainer;

    /// <summary>
    /// Helper extensions for Views
    /// </summary>
    public static class ContentDataExtensions
    {
        private static ICacheManager _CacheManager = InitializationContext.Locator.Get<ICacheManager>();

        private static int CacheInterval = _CacheManager.ShortInterval;

        public static long GetDynamicContentID(this object o)
        {
            return HttpContext.Current.GetHttpContextItem<long>
            (
                "Ektron.DyanmicItemId",
                () =>
                {
                    ISiteSetup setup = InitializationContext.Locator.Get<ISiteSetup>();
                    string id = setup == null ? "0" : setup.DefaultContentId.ToString();
                    long i = 0;
                    HttpContext current = HttpContext.Current;

                    if (current == null)
                    {
                        return Convert.ToInt64(id);
                    }

                    if (current.Request.QueryString["pageid"] != null)
                    {
                        id = current.Request.QueryString["pageid"];

                        if (current.Request.QueryString["ekfrm"] != null)
                        {
                            id = current.Request.QueryString["ekfrm"];
                        }
                        else if (current.Request.QueryString["id"] != null)
                        {
                            id = current.Request.QueryString["id"];
                        }

                    }
                    else if (current.Request.QueryString["ekfrm"] != null)
                    {
                        id = current.Request.QueryString["ekfrm"];
                    }
                    else if (current.Request.QueryString["id"] != null)
                    {
                        id = current.Request.QueryString["id"];
                    }

                    long.TryParse(id, out i);

                    return i;
                }
            );

        }

        public static IContent GetDynamicContent(this object o)
        {
            return HttpContext.Current.GetHttpContextItem<IContent>
            (
                "Ektron.TypedDynamicContent",
                () =>
                {
                    return o.GetDynamicContentID().GetContent(FrameworkFactory.CurrentLanguage());
                }
            );
        }

        public static ContentData GetEktronContentData(this long Id, int Language, bool AdminMode = false, bool disableCache = false)
        {
            if (Id < 1)
                return null;

            var ektronContent = _CacheManager.CacheItem<ContentData>
            (
                Id.GetContentCacheKey(Language),
                () =>
                {
                    var criteria = Id.GetContentCriteria();
                    criteria.AddFilter(ContentProperty.Id, CriteriaFilterOperator.EqualTo, Id);

                    var manager = criteria.GetContentManager(Language, true); // always want admin mode, filter remove content if user doesn't have access

                    return manager.GetItem(Id, true); // always return metadata
                },
                disableCache ? 0 : CacheInterval
            );

            return ektronContent.Filter(AdminMode);
        }

        public static IEnumerable<ContentData> GetEktronContentDataList(this IEnumerable<long> Ids, int Language, bool AdminMode = false)
        {
            if (Ids == null || !Ids.Any())
                yield break;

            CustomTuple<long, int> Identifier;
            Dictionary<CustomTuple<long, int>, ContentData> DataItems = new Dictionary<CustomTuple<long, int>, ContentData>();

            // Adds items already cached
            foreach (long Id in Ids)
            {
                Identifier = CustomTuple.Create<long, int>(Id, Language);

                if (!DataItems.ContainsKey(Identifier))
                    DataItems.Add(Identifier, Identifier.Item1.TryGetContentFromCache(Identifier.Item2));
            }

            // Gets list of items to retrieve from the database
            var keys = from p in DataItems
                       where p.Value == null
                       select p.Key;

            List<ContentData> Data = null;

            if (keys != null && keys.Count() > 0)
            {
                var tempIds = keys.Select(x => x.Item1).ToList(); // list to support 3-Tier

                var criteria = Ids.GetContentCriteria();
                criteria.AddFilter(ContentProperty.Id, CriteriaFilterOperator.In, tempIds);
                criteria.PagingInfo.RecordsPerPage = tempIds.Count();

                var manager = criteria.GetContentManager(Language, AdminMode);
                Data = manager.GetList(criteria);
            }

            ContentData item = null;

            // Returns all non empty items
            foreach (KeyValuePair<CustomTuple<long, int>, ContentData> kv in DataItems)
            {
                if (kv.Value != null)
                {
                    item = kv.Value.Filter(AdminMode);
                }
                else if (Data != null)
                {
                    item = _CacheManager.CacheItem<ContentData>
                        (
                            kv.Key.Item1.GetContentCacheKey(kv.Key.Item2),
                            () =>
                            {
                                return Data.Where(i => i.Id == kv.Key.Item1).FirstOrDefault();
                            },
                            CacheInterval
                        ).Filter(AdminMode);
                }

                // Only return when there is something to return
                if (item != null && item.Id > 0)
                    yield return item;
            }
        }

        public static IEnumerable<ContentData> GetEktronContentDataList(this string IdList, int Language, bool AdminMode = false)
        {
            List<ContentData> items = new List<ContentData>();

            if (string.IsNullOrEmpty(IdList))
                return items;

            // ToLong converts strings to long
            var Ids = IdList.ToEnumerable().Select(x => x.ToInt64()).ToList();;

            if (Ids == null || Ids.Count == 0)
                return items;

            return GetEktronContentDataList(Ids, Language, AdminMode);
        }

        public static IEnumerable<ContentData> GetEktronContentDataList(this ContentCriteria criteria, bool AdminMode = false, bool disableCache = false)
        {
            if (criteria == null)
                return null;

            PagedCachedData<ContentData> cached = _CacheManager.CacheItem
            (
               criteria.GetCacheKey(AdminMode),
                () =>
                {
                    var pagedCached = new PagedCachedData<ContentData>();

                    var manager = criteria.GetContentManager(criteria.GetLanguageId(), AdminMode);
                    pagedCached.Items = manager.GetList(criteria);
                    pagedCached.NumberOfRecords = criteria.PagingInfo.TotalRecords;

                    return pagedCached;
                },
                disableCache ? 0 : CacheInterval
            );

            criteria.PagingInfo.TotalPages = (cached.NumberOfRecords - 1) / criteria.PagingInfo.RecordsPerPage + 1;
            criteria.PagingInfo.TotalRecords = cached.NumberOfRecords; // update criteria

            return cached.Items;
        }

        public static IContent GetContent(this ContentData contentData)
        {
            return ContentFactory.MakeItem(contentData);
        }

        public static IContent GetContent(this long Id, int Language, bool AdminMode = false)
        {
            return ContentFactory.MakeItem(GetEktronContentData(Id, Language, AdminMode));
        }

        public static T GetContent<T>(this long Id, int Language, bool AdminMode = false) where T : IContent
        {            
            return (T)ContentFactory.MakeItem(GetEktronContentData(Id, Language, AdminMode));
        }

        public static IContent GetContent(this TaxonomyItemData TaxonomyItem, bool AdminMode = false)
        {
            return GetContent(TaxonomyItem.ItemId, TaxonomyItem.ItemLanguageId, AdminMode);
        }

        public static T GetContent<T>(this TaxonomyItemData TaxonomyItem, bool AdminMode = false) where T : IContent
        {
            return GetContent<T>(TaxonomyItem.ItemId, TaxonomyItem.ItemLanguageId, AdminMode);
        }

        public static IEnumerable<IContent> GetContentList(this IEnumerable<TaxonomyItemData> TaxonomyItems, bool AdminMode = false)
        {
            return TaxonomyItems.Select(x => x.GetContent(AdminMode));
        }

        public static IEnumerable<T> GetContentList<T>(this IEnumerable<TaxonomyItemData> TaxonomyItems, bool AdminMode = false) where T : IContent
        {
            return TaxonomyItems.Select(x => GetContent(x, AdminMode)).OfType<T>();
        }

        public static IEnumerable<IContent> GetContentList(this string IdList, int Language, bool AdminMode = false)
        {
            return ContentFactory.MakeList(GetEktronContentDataList(IdList, Language, AdminMode));
        }

        public static IEnumerable<T> GetContentList<T>(this string IdList, int Language, bool AdminMode = false) where T : IContent
        {
            return FilterType<T>(ContentFactory.MakeList(GetEktronContentDataList(IdList, Language, AdminMode)));
        }

        public static IEnumerable<IContent> GetContentList(this List<long> IdList, int Language, bool AdminMode = false)
        {
            return ContentFactory.MakeList(GetEktronContentDataList(IdList, Language, AdminMode));
        }

        public static IEnumerable<T> GetContentList<T>(this List<long> IdList, int Language, bool AdminMode = false) where T : IContent
        {
            return FilterType<T>(ContentFactory.MakeList(GetEktronContentDataList(IdList, Language, AdminMode)));
        }

        public static IEnumerable<IContent> GetContent(this IEnumerable<ContentData> contentList)
        {
            return ContentFactory.MakeList(contentList);
        }

        public static IEnumerable<T> GetContent<T>(this IEnumerable<ContentData> contentList) where T : IContent
        {
            return FilterType<T>(ContentFactory.MakeList(contentList));
        }

        public static IEnumerable<IContent> GetContent(this ContentCriteria criteria, bool AdminMode = false)
        {
            return ContentFactory.MakeList(criteria.GetEktronContentDataList(AdminMode));
        }

        public static IEnumerable<T> GetContent<T>(this ContentCriteria criteria, bool AdminMode = false) where T : IContent
        {
            return FilterType<T>(ContentFactory.MakeList(criteria.GetEktronContentDataList(AdminMode)));
        }

        public static IEnumerable<T> FilterType<T>(this IEnumerable<IContent> items) where T : IContent
        {
            if (items == null) return null;

            return items.OfType<T>();
        }

        public static List<TaxonomyData> GetAssignedCategories(this IContent c, EkEnumeration.TaxonomyItemType ItemType = EkEnumeration.TaxonomyItemType.Content, PagingInfo PageInfo = null)
        {
            if (c == null)
            {
                return null;
            }

            return GetAssignedCategories(c.Id, c.LanguageId, ItemType, PageInfo);
        }

        public static List<TaxonomyData> GetAssignedCategories(this long Id, int LanguageId, EkEnumeration.TaxonomyItemType ItemType = EkEnumeration.TaxonomyItemType.Content, PagingInfo PageInfo = null)
        {
            PagedCachedData<TaxonomyData> TaxonomyItems = null;

            if (PageInfo == null)
                PageInfo = new PagingInfo() { RecordsPerPage = 100 };

            var criteria = Id.GetTaxonomyItemCriteria();
            criteria.AddFilter(TaxonomyItemProperty.ItemType, CriteriaFilterOperator.EqualTo, ItemType);
            criteria.AddFilter(TaxonomyItemProperty.ItemId, CriteriaFilterOperator.EqualTo, Id);
            criteria.AddFilter(TaxonomyItemProperty.LanguageId, CriteriaFilterOperator.EqualTo, LanguageId);
            criteria.PagingInfo = PageInfo;

            TaxonomyItems = _CacheManager.CacheItem
            (
                criteria.GetCacheKey(true),
                () =>
                {
                    var cache = new PagedCachedData<TaxonomyData>();

                    var api = FrameworkFactory<Ektron.Cms.Framework.Organization.TaxonomyItemManager>.Get(true, LanguageId);
                    var items = api.GetList(criteria);
                    cache.NumberOfRecords = criteria.PagingInfo.TotalRecords;

                    if (items != null && items.Count > 0)
                    {
                        var tApi = FrameworkFactory<Ektron.Cms.Framework.Organization.TaxonomyManager>.Get(true, LanguageId);

                        var tCriteria = Id.GetTaxonomyCriteria();
                        tCriteria.PagingInfo = PageInfo;
                        tCriteria.AddFilter(TaxonomyProperty.Id, CriteriaFilterOperator.In, items.Select(x => x.TaxonomyId).ToList());

                        cache.Items = tApi.GetList(tCriteria).OrderBy(x => x.Path).ToList();
                    }

                    return cache;

                },
                CacheInterval
            );

            PageInfo.TotalRecords = TaxonomyItems.NumberOfRecords;

            return TaxonomyItems.Items.ToList();
        }
    }
}