namespace WSOL.EktronCms.ContentMaker.Extensions
{
    using Ektron.Cms;
    using Ektron.Cms.Common;
    using Ektron.Cms.Framework.Core.Content;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;
    using WSOL.EktronCms.ContentMaker.Interfaces;
    using WSOL.IocContainer;

    /// <summary>
    /// Helper extensions for Views
    /// </summary>
    public static class ContentDataExtensions
    {
        #region Cache Helpers

        private static ICacheManager _CacheManager = InitializationContext.Locator.Get<ICacheManager>();

        private static int CacheInterval = _CacheManager.ShortInterval;

        public static string ToCacheKey(this Criteria<ContentProperty> criteria)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(criteria.FilterGroups.GetFilterGroupCacheKey<ContentProperty>());
            stringBuilder.AppendFormat("OrderByField:{0},OrderDirection:{1},PageSize:{2},Page:{3}", criteria.OrderByField, criteria.OrderByDirection, criteria.PagingInfo.RecordsPerPage, criteria.PagingInfo.CurrentPage);
            
            return stringBuilder.ToString();
        }

        public static string GetFilterGroupCacheKey<P>(this List<CriteriaFilterGroup<P>> filterGroups)
        {
            StringBuilder keyBuilder = new StringBuilder();

            foreach (CriteriaFilterGroup<P> criteriaFilterGroup in filterGroups)
            {
                CriteriaFilterGroup<P> group = criteriaFilterGroup;
                keyBuilder.Append(group.Condition + "(");

                group.Filters.ForEach((Action<CriteriaFilter<P>>)(f =>
                {
                    if (f.Operator.ToString().ToLower() == "in")
                    {
                        IEnumerable local_0 = f.Value as IEnumerable;
                        StringBuilder local_1 = new StringBuilder();

                        if (local_0 != null)
                        {
                            foreach (object item_1 in local_0)
                                local_1.Append("," + item_1);
                        }

                        keyBuilder.AppendFormat("{0}{1}{2}{3}", group.Condition, f.Field, f.Operator, local_1.ToString());
                    }
                    else
                        keyBuilder.AppendFormat("{0}{1}{2}{3}", group.Condition, f.Field, f.Operator, f.Value);
                }));

                keyBuilder.Append(")");
            }
            return keyBuilder.ToString();
        }        

        public static string GetCacheKey(long Id, int Language)
        {
            return string.Format("WSOL.EktronCms.ContentMaker.ContentData:Id={0}:L={1}", Id, Language);
        }

        public static string GetCacheKey(this Criteria<ContentProperty> Criteria)
        {
            return string.Format("WSOL.EktronCms.ContentMaker.GetList:{0}", Criteria.ToCacheKey());
        }

        public static ContentData TryGetContentFromCache(long Id, int Language)
        {
            if (Id <= 0)
                return null;

            ContentData Data = HttpRuntime.Cache[GetCacheKey(Id, Language)] as ContentData;

            if (_CacheManager.EnableCache && Data != null)
                return Data;

            return null;
        }

        #endregion

        #region Ektron Framework Helpers

        public static Content GetManager(int Language, bool AdminMode)
        {
            return FrameworkFactory<Content>.Get(AdminMode, Language);
        }

        public static Criteria<ContentProperty> GetCriteria(this object o)
        {
            return GetCriteria();
        }

        public static Criteria<ContentProperty> GetCriteria()
        {
            return new Ektron.Cms.Common.Criteria<ContentProperty>();            
        }

        #endregion

        #region Public Helpers

        public static long GetDynamicContentID(this object o)
        {
            return HttpContext.Current.GetHttpContextItem<long>
            (
                "Ektron.DyanmicItemId",
                () =>
                {
                    ISiteSetup setup = InitializationContext.Locator.Get<ISiteSetup>();
                    string id = setup.DefaultContentId.ToString();
                    long i = 0;
                    HttpContext current = HttpContext.Current;

                    if (current == null)
                        return -1;

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
                    return GetDynamicContentID(o).GetContent(FrameworkFactory.CurrentLanguage(false), false);
                }
            );
        }

        public static ContentData GetEktronContentData(this long Id, int Language, bool AdminMode)
        {
            if (Id < 0)
                return null;

            return _CacheManager.CacheItem<ContentData>
            (
                GetCacheKey(Id, Language),
                () =>
                {
                    var criteria = GetCriteria();
                    criteria.AddFilter(ContentProperty.Id, CriteriaFilterOperator.EqualTo, Id);

                    var manager = GetManager(Language, AdminMode);

                    return manager.GetItem(Id);
                },
                CacheInterval
            );
        }

        public static IEnumerable<ContentData> GetEktronContentDataList(this List<long> Ids, int Language, bool AdminMode)
        {
            if (Ids == null || Ids.Count == 0)
                yield break;

            CustomTuple<long, int> Identifier;
            Dictionary<CustomTuple<long, int>, ContentData> DataItems = new Dictionary<CustomTuple<long, int>, ContentData>();

            // Adds items already cached
            foreach (long Id in Ids)
            {
                Identifier = CustomTuple.Create<long, int>(Id, Language);

                if (!DataItems.ContainsKey(Identifier))
                    DataItems.Add(Identifier, TryGetContentFromCache(Identifier.Item1, Identifier.Item2));
            }

            // Gets list of items to retrieve from the database
            var keys = from p in DataItems
                       where p.Value == null
                       select p.Key;

            List<ContentData> Data = null;

            if (keys != null && keys.Count() > 0)
            {
                var tempIds = keys.Select(x => x.Item1).ToList();

                var criteria = GetCriteria(false);
                criteria.AddFilter(ContentProperty.Id, CriteriaFilterOperator.In, tempIds);
                criteria.PagingInfo.RecordsPerPage = tempIds.Count();

                var manager = GetManager(Language, AdminMode);
                Data = manager.GetList(criteria);
            }

            ContentData item = null;

            // Returns all non empty items
            foreach (KeyValuePair<CustomTuple<long, int>, ContentData> kv in DataItems)
            {
                if (kv.Value != null)
                {
                    item = kv.Value;
                }
                else if (Data != null)
                {
                    item = _CacheManager.CacheItem<ContentData>
                        (
                            GetCacheKey(kv.Key.Item1, kv.Key.Item2),
                            () =>
                            {
                                return Data.Where(i => i.Id == kv.Key.Item1).ElementAt(0);
                            },
                            CacheInterval
                        );
                }

                // Only return when there is something to return
                if (item != null && item.Id > 0)
                    yield return item;
            }
        }

        public static IEnumerable<ContentData> GetEktronContentDataList(this string IdList, int Language, bool AdminMode)
        {
            List<ContentData> items = new List<ContentData>();

            if (string.IsNullOrEmpty(IdList))
                return items;

            // ToLong converts strings to long
            var Ids = IdList.ToEnumerable().Select(x => x.ToInt64()).ToList();

            if (Ids == null || Ids.Count == 0)
                return items;

            return GetEktronContentDataList(Ids, Language, AdminMode);
        }

        public static IEnumerable<ContentData> GetEktronContentDataList(this Criteria<ContentProperty> criteria, int Language, bool AdminMode)
        {
            if (criteria == null)
                return new List<ContentData>();

            return _CacheManager.CacheItem
            (
                GetCacheKey(criteria),
                () =>
                {
                    var manager = GetManager(Language, AdminMode);
                    return manager.GetList(criteria);
                },
                CacheInterval
            );
        }

        public static IContent GetContent(this ContentData contentData)
        {
            return ContentMaker.MakeItem(contentData);
        }

        public static IContent GetContent(this long Id, int Language, bool AdminMode)
        {
            return ContentMaker.MakeItem(GetEktronContentData(Id, Language, AdminMode));
        }

        public static IEnumerable<IContent> GetContentList(this string IdList, int Language, bool AdminMode)
        {
            return ContentMaker.MakeList(GetEktronContentDataList(IdList, Language, AdminMode));
        }

        public static IEnumerable<IContent> GetContentList(this List<long> IdList, int Language, bool AdminMode)
        {
            return ContentMaker.MakeList(GetEktronContentDataList(IdList, Language, AdminMode));
        }

        public static IEnumerable<IContent> GetContent(this IEnumerable<ContentData> contentList)
        {
            return ContentMaker.MakeList(contentList);
        }

        public static IEnumerable<IContent> GetContent(this Criteria<ContentProperty> criteria, int Language, bool AdminMode)
        {
            return ContentMaker.MakeList(criteria.GetEktronContentDataList(Language, AdminMode));
        }

        #endregion
    }
}