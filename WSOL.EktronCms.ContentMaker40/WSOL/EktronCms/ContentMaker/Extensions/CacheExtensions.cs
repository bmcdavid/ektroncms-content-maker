namespace WSOL.EktronCms.ContentMaker.Extensions
{
    using Ektron.Cms;
    using Ektron.Cms.Common;
    using Ektron.Cms.Content;
    using WSOL.IocContainer;

    public static class CacheExtensions
    {
        private static ICacheManager _CacheManager = InitializationContext.Locator.Get<ICacheManager>();

        public static string GetContentCacheKey(this long Id, int Language)
        {
            return string.Format("WSOL.EktronCms.ContentMaker.ContentData:Id={0}:L={1}", Id, Language);
        }

        public static string GetCacheKey(this ContentCriteria Criteria, bool AdminMode = false)
        {
            return string.Format("WSOL.EktronCms.ContentMaker.GetList:AdminMode={0}:{1}", AdminMode ? -1 : FrameworkFactory.CurrentUserId(), Criteria.ToCacheKey());
        }

        public static string GetCacheKey<T>(this long Id, int Language = -1)
        {
            return string.Format("WSOL.EktronCms.ContentMaker.{0}:Id={1}:L={2}", typeof(T).FullName, Id, Language);
        }

        public static string GetCacheKey<T>(this Criteria<T> criteria, bool adminMode)
        {
            return string.Format("WSOL.EktronCms.ContentMaker.GetList:Type{0}:AdminMode={1}:{2}", typeof(T).FullName, adminMode ? -1 : FrameworkFactory.CurrentUserId(), criteria.ToCacheKey());
        }

        public static ContentData TryGetContentFromCache(this long Id, int Language)
        {
            if (Id <= 0)
                return null;

            ContentData Data = _CacheManager.GetItem<ContentData>(GetContentCacheKey(Id, Language));

            if (_CacheManager.EnableCache && Data != null)
            {
                return Data;
            }

            return null;
        }
    }
}