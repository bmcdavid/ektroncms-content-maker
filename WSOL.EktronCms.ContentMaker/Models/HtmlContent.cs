namespace WSOL.EktronCms.ContentMaker.Models
{
    using Extensions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;
    using WSOL.EktronCms.ContentMaker.Attributes;
    using WSOL.EktronCms.ContentMaker.Interfaces;
    using WSOL.IocContainer;

    /// <summary>
    /// Base Class for which all content is derived
    /// </summary>
    [Serializable]
    [KnownType(typeof(Enums.ArchiveAction))]
    [KnownType(typeof(Enums.ContentStatus))]
    [KnownType(typeof(List<WSOL.EktronCms.ContentMaker.Models.MetaData>))]
    [KnownType(typeof(WSOL.IocContainer.HtmlHyperlink))]
    [KnownType(typeof(WSOL.IocContainer.HtmlImage))]
    [ContentDescriptor(XmlConfigId = 0, Description = "HTML Content", BackendContent = false)]
    public class HtmlContent : IContent, ISerializable
    {
        #region Constructors

        // FullView
        public HtmlContent()
        {
            SmartFormData = null;
            MetaData = new List<MetaData>();
            Status = Enums.ContentStatus.Empty;
            ArchiveAction = Enums.ArchiveAction.Remain;
        }

        // For inherited conversions
        public HtmlContent(IContent content)
        {
            SmartFormData = null;
            Id = content.Id;
            LanguageId = content.LanguageId;
            FolderId = content.FolderId;
            UserId = content.UserId;
            Title = content.Title;
            Url = content.Url;
            Html = content.Html;
            Description = content.Description;
            DateCreated = content.DateCreated;
            DateModified = content.DateModified;
            StartDate = content.StartDate;
            EndDate = content.EndDate;
            IsPrivate = content.IsPrivate;
            IsForm = content.IsForm;
            Status = content.Status;
            ArchiveAction = content.ArchiveAction;
            MetaData = content.MetaData.OfType<MetaData>().ToList();
            XmlConfigId = content.XmlConfigId;
        }

        #endregion

        #region Properties

        public virtual long Id { get; set; }

        public virtual int LanguageId { get; set; }

        public virtual long FolderId { get; set; }

        public long UserId { get; set; }

        public virtual long XmlConfigId { get; set; }

        public virtual string Title { get; set; }

        public virtual string Url { get; set; }

        private string html;
        public virtual string Html
        {
            get
            {
                if (SmartFormData != null)
                    return SmartFormData.ToXml();

                return html;
            }
            set
            {
                html = value;
            }
        }

        public virtual string Description { get; set; }

        public virtual DateTime DateCreated { get; set; }

        public virtual DateTime DateModified { get; set; }

        public virtual DateTime StartDate { get; set; }

        public virtual DateTime EndDate { get; set; }

        public virtual bool IsPrivate { get; set; }

        public virtual bool IsForm { get; set; }

        [Browsable(false), XmlIgnore]
        public virtual object SmartFormData { get; protected set; }

        public virtual Enums.ContentStatus Status { get; set; }

        public virtual Enums.ArchiveAction ArchiveAction { get; set; }

        public Enums.ContentType ContentType { get; set; }

        public Enums.ContentSubtype ContentSubType { get; set; }

        public virtual List<MetaData> MetaData { get; set; }
        
        IEnumerable<IMetaData> IContent.MetaData
        {
            get
            {
                return MetaData.Cast<IMetaData>();
            }
            set
            {
                MetaData = value.Cast<MetaData>().ToList();
            }
        }

        #endregion

        #region Serialization Methods

        // Serialization Info Mapper, only mapped fields get serialized
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id, typeof(long));
            info.AddValue("LanguageId", LanguageId, typeof(int));
            info.AddValue("FolderId", FolderId, typeof(long));
            info.AddValue("UserId", UserId, typeof(long));
            info.AddValue("XmlConfigId", XmlConfigId, typeof(long));
            info.AddValue("Title", Title, typeof(string));
            info.AddValue("Url", Url, typeof(string));
            info.AddValue("Html", Html.ToCDATA(), typeof(string));
            info.AddValue("Description", Description.ToCDATA(), typeof(string));
            info.AddValue("DateCreated", DateCreated, typeof(DateTime));
            info.AddValue("DateModified", DateModified, typeof(DateTime));
            info.AddValue("StartDate", StartDate, typeof(DateTime));
            info.AddValue("EndDate", EndDate, typeof(DateTime));
            info.AddValue("IsPrivate", IsPrivate, typeof(bool));
            info.AddValue("IsForm", IsForm, typeof(bool));
            info.AddValue("Status", Status, typeof(Enums.ContentStatus));
            info.AddValue("ArchiveAction", ArchiveAction, typeof(Enums.ArchiveAction));
            info.AddValue("MetaData", MetaData, typeof(List<MetaData>));
        }

        // Serialization Contructor
        protected HtmlContent(SerializationInfo info, StreamingContext context)
        {
            Id = info.GetInt64("Id");
            LanguageId = info.GetInt32("LanguageId");
            FolderId =  info.GetInt64("FolderId");
            UserId = info.GetInt64("UserId");
            XmlConfigId = info.GetInt64("XmlConfigId");
            Title = info.GetString("Title");
            Url = info.GetString("Url");
            Html = info.GetString("Html").FromWrapCDATA();
            Description = info.GetString("Description").FromWrapCDATA();
            DateCreated = info.GetValue("DateCreated", typeof(object)).FromSerializedDateTime();
            DateModified = info.GetValue("DateModified", typeof(object)).FromSerializedDateTime();
            StartDate = info.GetValue("StartDate", typeof(object)).FromSerializedDateTime();
            EndDate = info.GetValue("EndDate", typeof(object)).FromSerializedDateTime();
            IsPrivate = info.GetBoolean("IsPrivate");
            IsForm = info.GetBoolean("IsForm");
            Status = (Enums.ContentStatus)info.GetValue("Status", typeof(Enums.ContentStatus));
            ArchiveAction = (Enums.ArchiveAction)info.GetValue("Status", typeof(Enums.ArchiveAction));

            // Needed for JS deserializations
            var metaData = info.GetValue("MetaData", typeof(object));

            if (metaData is Array)
            {
                MetaData = (metaData as Array).OfType<MetaData>().ToList();
            }
            else if (metaData is List<MetaData>)
                MetaData = metaData as List<MetaData>;
            
        }

        #endregion
    }
}