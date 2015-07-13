namespace WSOL.EktronCms.ContentMaker.Interfaces
{
    using System;
    using System.Collections.Generic;
    using WSOL.EktronCms.ContentMaker.Enums;

    public interface IContent : IIdentifier
    {
        ArchiveAction ArchiveAction { get; set; }

        ContentSubtype ContentSubType { get; set; }

        ContentType ContentType { get; set; }

        DateTime DateCreated { get; set; }

        DateTime DateModified { get; set; }

        string Description { get; set; }

        DateTime EndDate { get; set; }

        long FolderId { get; set; }

        string Html { get; set; }

        bool IsForm { get; set; }

        bool IsPrivate { get; set; }

        IEnumerable<IMetaData> MetaData { get; set; }

        object SmartFormData { get; }

        DateTime StartDate { get; set; }

        ContentStatus Status { get; set; }

        string Title { get; set; }

        string Url { get; set; }

        long UserId { get; set; }        

        long XmlConfigId { get; set; }
    }
}
