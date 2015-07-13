namespace WSOL.EktronCms.ContentMaker.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
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

        /// <summary>
        /// Gets child folders from folder ID. Important, this data is not cached!
        /// </summary>
        /// <param name="FolderID"></param>
        /// <param name="Recursive"></param>
        /// <returns></returns>
        public static List<Ektron.Cms.FolderData> GetChildFolders(this long folderID, int languageID = -1, bool adminMode = false, bool recursive = false, int pageSize = 500)
        {
            languageID = languageID > 0 ? languageID : LanguageExtensions.GetLanguageId(folderID);
            List<Ektron.Cms.FolderData> folders = new List<Ektron.Cms.FolderData>();
            Ektron.Cms.FolderCriteria criteria = new Ektron.Cms.FolderCriteria();
            criteria.AddFilter(Ektron.Cms.Common.FolderProperty.ParentId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, folderID);
            criteria.PagingInfo.RecordsPerPage = pageSize;
            var folderManager = folderID.GetFrameworkManager<Ektron.Cms.Framework.Organization.FolderManager>(languageID, adminMode);
            List<Ektron.Cms.FolderData> childFolders = folderManager.GetList(criteria);

            if (childFolders != null)
            {
                foreach (var childFolder in childFolders)
                {
                    if (childFolder.Id > 0)
                    {
                        if (!folders.Contains(childFolder))
                            folders.Add(childFolder);

                        if (recursive && childFolder.HasChildren)
                            folders = folders.Union(GetChildFolders(childFolder.Id, languageID, adminMode, recursive, pageSize)).ToList();
                    }
                }
            }

            return folders.OrderBy(x => x.FolderIdWithPath).ToList();
        }

        /// <summary>
        /// Gets child folder IDs from folder. Important, this data is not cached!
        /// </summary>
        /// <param name="FolderID"></param>
        /// <returns></returns>
        public static List<long> GetChildFolderIDs(this long folderID, int languageID = -1, bool adminMode = false, bool recursive = false)
        {
            return GetChildFolders(folderID, languageID, adminMode, recursive).Select(x => x.Id).ToList();
        }

        /// <summary>
        /// Get parent folders from given folder ID
        /// </summary>
        /// <param name="FolderID"></param>
        /// <param name="languageID"></param>
        /// <param name="adminMode"></param>
        /// <returns></returns>
        public static List<Ektron.Cms.FolderData> GetParentFolders(this long FolderID, int languageID = -1, bool adminMode = false)
        {
            List<Ektron.Cms.FolderData> parents = new List<Ektron.Cms.FolderData>();
            Ektron.Cms.FolderData currentFolder = GetFolderData(FolderID, languageID > 0 ? languageID : LanguageExtensions.GetLanguageId(FolderID), adminMode);

            while (currentFolder.ParentId > 0)
            {
                if (FolderID != 0)
                    parents.Add(currentFolder);
                
                FolderID = currentFolder.ParentId;
                currentFolder = GetFolderData(FolderID, languageID > 0 ? languageID : LanguageExtensions.GetLanguageId(FolderID), adminMode);
            }

            return parents.OrderBy(x => x.FolderIdWithPath).ToList();
        }

        /// <summary>
        /// Gets parent folder IDs from folder ID
        /// </summary>
        /// <param name="FolderID"></param>
        /// <returns></returns>
        public static List<long> GetParentFolderIDs(this long FolderID, int languageID = -1, bool adminMode = false)
        {
            return GetParentFolders(FolderID, languageID, adminMode).Select(x => x.Id).ToList();
        }
    }
}