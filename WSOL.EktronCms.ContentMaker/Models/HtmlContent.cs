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
    using WSOL.ObjectRenderer.Interfaces;

    /// <summary>
    /// Base Class for which all content is derived
    /// </summary>
    [Serializable]
    [KnownType(typeof(Enums.ArchiveAction))]
    [KnownType(typeof(Enums.ContentStatus))]
    [KnownType(typeof(List<WSOL.EktronCms.ContentMaker.Models.MetaData>))]
    [KnownType(typeof(WSOL.IocContainer.HtmlHyperlink))]
    [KnownType(typeof(WSOL.IocContainer.HtmlImage))]
    [ContentDescriptor(XmlConfigId = 0, Description = "HTML Content")]
    public class HtmlContent : IContent, ISerializable, IRendererDebugString, ICacheKey, IRendererItemDisplay
    {
        #region Constructors

        // Default
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

        #endregion Constructors

        #region Properties

        private string html;

        public virtual Enums.ArchiveAction ArchiveAction { get; set; }

        /// <summary>
        /// Sets unique info if XSLT templates are used withing the WSOL:Renderer
        /// </summary>
        public virtual string CacheKey
        {
            get { return string.Format("ID={0}:L={1}", Id, LanguageId); }
        }

        public virtual Enums.ContentSubtype ContentSubType { get; set; }

        public virtual Enums.ContentType ContentType { get; set; }

        public virtual DateTime DateCreated { get; set; }

        public virtual DateTime DateModified { get; set; }

        /// <summary>
        /// Adds object specific information for the renderer debug display
        /// </summary>
        public virtual string DebugText
        {
            get
            {
                return string.Format("ID: {0}, Name: {1}, XmlID: {2}, ",
                          this.Id,
                          this.Title,
                          this.XmlConfigId
                      );
            }
        }

        public virtual string Description { get; set; }

        public virtual DateTime EndDate { get; set; }

        public virtual long FolderId { get; set; }

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

        public virtual long Id { get; set; }

        public virtual bool IsForm { get; set; }

        public virtual bool IsPrivate { get; set; }

        public virtual int LanguageId { get; set; }

        public virtual List<MetaData> MetaData { get; set; }

        [Browsable(false), XmlIgnore]
        public virtual object SmartFormData { get; protected set; }

        public virtual DateTime StartDate { get; set; }

        public virtual Enums.ContentStatus Status { get; set; }

        public virtual string Title { get; set; }

        public virtual string Url { get; set; }

        public virtual long UserId { get; set; }

        public virtual long XmlConfigId { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines if item can be displayed within the WSOL:Renderer control
        /// </summary>
        /// <param name="templateDescriptor"></param>
        /// <param name="Tags"></param>
        /// <returns></returns>
        public virtual bool DisplayItem(ObjectRenderer.Attributes.TemplateDescriptorAttribute templateDescriptor, string[] Tags)
        {
            return true;
        }

        /// <summary>
        /// Returns the contents html value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Html;
        }

        #endregion Methods

        #region Serialization Methods

        /// <summary>
        /// Serialization Contructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected HtmlContent(SerializationInfo info, StreamingContext context)
        {
            Id = info.GetInt64("Id");
            LanguageId = info.GetInt32("LanguageId");
            FolderId = info.GetInt64("FolderId");
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

        /// <summary>
        /// Serialization Info Mapper, only mapped fields get serialized
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
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

        #endregion Serialization Methods
    }
}