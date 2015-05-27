namespace WSOL.EktronCms.ContentMaker.Interfaces
{
    using System.Collections.Generic;

    public interface INavigationFactory<T>
    {
        /// <summary>
        /// Gets a navigation
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Language"></param>
        /// <param name="CacheInterval"></param>
        /// <param name="ItemResolvers"></param>
        /// <param name="adminMode"></param>
        /// <returns></returns>
        INavigation Get(long Id, int Language = 1033, int CacheInterval = 30, IEnumerable<INavigationResolver> ItemResolvers = null, bool adminMode = false);

        /// <summary>
        /// Converts source to INavigation
        /// </summary>
        /// <param name="Item"></param>
        /// <returns></returns>
        INavigation MakeItem(T Item);

        /// <summary>
        /// Doesn't populate navigation items, just gets a reference for all menus in a system for a language.
        /// </summary>
        /// <param name="Language"></param>
        /// <returns></returns>
        IEnumerable<INavigation> GetAll(int Language = -1);

        /// <summary>
        /// Gets source data (i.e. Ektron.Cms.Organization.MenuData)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="MenuID"></param>
        /// <param name="ContentLanguage"></param>
        /// <param name="CacheInterval"></param>
        /// <param name="getTree"></param>
        /// <param name="adminMode"></param>
        /// <returns></returns>
        T GetSourceData(long MenuID, int ContentLanguage = 1033, int CacheInterval = 30, bool getTree = false, bool adminMode = false);
    }
}