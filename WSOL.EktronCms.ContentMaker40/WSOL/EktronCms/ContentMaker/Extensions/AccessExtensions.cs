namespace WSOL.EktronCms.ContentMaker.Extensions
{
    using Ektron.Cms;
    using System;
    using WSOL.EktronCms.ContentMaker.Interfaces;
    using WSOL.IocContainer;

    public static class AccessExtensions
    {
        private static ICacheManager _CacheManager = InitializationContext.Locator.Get<ICacheManager>();

        private static ILogger _Logger = InitializationContext.Locator.Get<ILogger>();

        /// <summary>
        /// Filters out expired and private content (if user doesn't have access).
        /// </summary>
        /// <param name="content"></param>
        /// <param name="AdminMode"></param>
        /// <returns></returns>
        public static ContentData Filter(this ContentData content, bool AdminMode = false, long UserId = -1)
        {
            if (content != null && !AdminMode)
            {
                if (content.EndDateActionType == Ektron.Cms.Common.EkEnumeration.CMSEndDateAction.Archive_Expire && content.ExpireDate != DateTime.MinValue && content.ExpireDate <= DateTime.Now)
                {
                    _Logger.Log(string.Format("The following content has been ommitted from the ContentMaker. ID = {0}, Language = {1}", content.Id, content.LanguageId), typeof(AccessExtensions).FullName, null, System.Diagnostics.EventLogEntryType.Information, content);                    

                    return null;
                }

                if (content.IsPrivate)
                {
                    if (FrameworkFactory.CurrentUserId() < 1)
                        return null;

                    var viewable = IsViewable(content.Id, content.LanguageId, UserId);

                    if (viewable.HasValue && !viewable.Value)
                        return null;
                }
            }

            return content;
        }

        public static bool? IsViewable(this IContent content, long UserId = -1)
        {
            if (content == null)
                return null;

            var permission = ContentPermissions(content.Id, content.LanguageId, UserId);

            if (permission == null)
                return null;

            return permission.CanView;
        }

        public static bool? IsViewable(this long ContentId, int LanguageId = -1, long UserId = -1, bool FolderPermission = false)
        {
            UserPermissionData permission = FolderPermission ? FolderPermissions(ContentId, LanguageId, UserId) : ContentPermissions(ContentId, LanguageId, UserId);

            if (permission == null)
                return null;

            return permission.CanView;
        }

        public static UserPermissionData ContentPermissions(this long ContentId, int LanguageId = -1, long UserId = -1)
        {
            if (UserId == -1)
                UserId = FrameworkFactory.CurrentUserId();

            if (ContentId == 0)
                ContentId = ContentId.GetDynamicContentID();

            if (LanguageId == -1)
                LanguageId = FrameworkFactory.CurrentLanguage();

            return _CacheManager.CacheItem<UserPermissionData>
            (
                string.Format("WSOL:Cache:ContentPermission:User={0}:Content={1}:Language={2}", UserId, ContentId, LanguageId),
                () =>
                {
                    var permissionManager = FrameworkFactory<Ektron.Cms.Framework.Settings.PermissionManager>.Get(true, LanguageId);

                    return permissionManager.GetUserPermissionForContent(UserId, ContentId, LanguageId);
                },
                _CacheManager.QuickInterval
            );
        }

        public static UserPermissionData FolderPermissions(this long FolderId, int LanguageId = -1, long UserId = -1)
        {
            if (UserId == -1)
                UserId = FrameworkFactory.CurrentUserId();

            if (FolderId == 0)
                FolderId = FolderId.GetDynamicContentID();

            if (LanguageId == -1)
                LanguageId = FrameworkFactory.CurrentLanguage();

            return _CacheManager.CacheItem<UserPermissionData>
            (
                string.Format("WSOL:Cache:FolderPermission:User={0}:FolderId={1}:Language={2}", UserId, FolderId, LanguageId),
                () =>
                {
                    var permissionManager = FrameworkFactory<Ektron.Cms.Framework.Settings.PermissionManager>.Get(true, LanguageId);

                    return permissionManager.GetUserPermissionForFolder(UserId, FolderId, LanguageId);
                },
                _CacheManager.QuickInterval
            );
        }
    }
}