namespace WSOL.EktronCms.ContentMaker.Extensions
{
    using WSOL.IocContainer;

    public static class FolderDataExtensions
    {
        private static ICacheManager _CacheManager = InitializationContext.Locator.Get<ICacheManager>();

        public static Ektron.Cms.FolderData GetFolderData(this long Id, int LangaugeId, bool adminMode = false)
        {
            return _CacheManager.CacheItem
            (
                string.Format("WSOL:Cache:EktronFolderId={0}:L={1}:Admin={2}", Id, LangaugeId, adminMode),
                () =>
                {
                    var manager = Id.GetFrameworkManager<Ektron.Cms.Framework.Organization.FolderManager>(LangaugeId, adminMode);
                    return manager.GetItem(Id);
                },

                _CacheManager.QuickInterval
            );
        }
    }
}