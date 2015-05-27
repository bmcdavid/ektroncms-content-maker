namespace WSOL.EktronCms.ContentMaker.Interfaces
{
    using System.Collections.Generic;

    /// <summary>
    /// Generic Settings required properties
    /// </summary>
    public interface ISettings
    {
        /// <summary>
        /// Map repeatable folder selectors to this field for matching
        /// </summary>
        IEnumerable<long> FolderIds { get; set; }

        /// <summary>
        /// Error message on failed attempts
        /// </summary>
        string ErrorMessage { get; set; }

        /// <summary>
        /// Used to modify MetaData values for current Item from ISettings instance
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        IEnumerable<IMetaData> ResolveMetaData(IContent content);
    }
}