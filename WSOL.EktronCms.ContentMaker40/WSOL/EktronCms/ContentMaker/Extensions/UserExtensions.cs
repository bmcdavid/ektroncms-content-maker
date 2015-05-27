namespace WSOL.EktronCms.ContentMaker.Extensions
{
    using Ektron.Cms;
    using WSOL.IocContainer;

    public static class UserExtensions
    {
        private static ICacheManager _CacheManager = InitializationContext.Locator.Get<ICacheManager>();

        public static UserData GetUserData(this long id, bool enableCache = true)
        {
            if (id < 1)
                return null;

            return _CacheManager.CacheItem<Ektron.Cms.UserData>
                (
                    id.GetCacheKey<Ektron.Cms.UserData>(),
                    () =>
                    {
                        var userApi = FrameworkFactory<Ektron.Cms.Framework.User.UserManager>.Get(true);

                        return userApi.GetItem(id, true); // always get the custom properties
                    },
                    enableCache ? _CacheManager.QuickInterval : 0
                );
        }

        public static long GetCurrentUserId(this object o, bool refresh = false)
        {
            return FrameworkFactory.CurrentUserId(refresh);
        }

        public static bool IsCurrentUserLoggedIn(this long id)
        {
            return FrameworkFactory.IsLoggedIn;
        }

    }
}