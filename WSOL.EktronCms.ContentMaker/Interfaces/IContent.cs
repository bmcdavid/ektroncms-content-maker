namespace WSOL.EktronCms.ContentMaker.Interfaces
{
    using System;
    using System.Collections.Generic;
    using WSOL.EktronCms.ContentMaker.Enums;

    public interface IContent
    {
        long Id { get; set; }
        int LanguageId { get; set; }
        long FolderId { get; set; }
        long UserId { get; set; }
        long XmlConfigId { get; set; }
        string Title { get; set; }
        string Url { get; set; }
        string Html { get; set; }
        object SmartFormData { get; }
        string Description { get; set; }
        DateTime DateCreated { get; set; }
        DateTime DateModified { get; set; }
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
        bool IsPrivate { get; set; }
        bool IsForm { get; set; }
        ContentStatus Status { get; set; }
        ArchiveAction ArchiveAction { get; set; }
        IEnumerable<IMetaData> MetaData { get; set; }
        ContentType ContentType { get; set; }
        ContentSubtype ContentSubType { get; set; }
    }
}
