namespace WSOL.Custom.ContentMaker.Samples.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using WSOL.EktronCms.ContentMaker.Attributes;
    using WSOL.EktronCms.ContentMaker.Interfaces;
    using WSOL.EktronCms.ContentMaker.Models;

    [Serializable]
    [ContentDescriptor(XmlConfigId = -1117, Description = "Folder Settings Example", BackendContent = true)]
    public class FolderSettingscontent : ContentType<ContentTypes.FolderSettings.root>, ISettings
    {
        private int CurrentLanguage = WSOL.EktronCms.ContentMaker.FrameworkFactory.CurrentLanguage();

        public FolderSettingscontent() { }

        public FolderSettingscontent(IContent c, string xml) : base(c, xml) { }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        protected FolderSettingscontent(SerializationInfo info, StreamingContext context) : base(info, context) { }

        /// <summary>
        /// Gets populated by SettingsFactory if an error occurs getting settings.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Needs to be mapped to SmartFormData folder selector, used for folder settings only
        /// </summary>
        public IEnumerable<long> FolderIds
        {
            get
            {
                return SmartFormData.FolderID.Select(x => x.Value);
            }
            set
            {
                SmartFormData.FolderID = value.Select(x => new ContentTypes.FolderSettings.FolderIdRefType() { Value = x }).ToList();
            }
        }

        public string SectionTitle
        {
            get
            {
                return SmartFormData.SectionTitle;
            }
            set
            {
                SmartFormData.SectionTitle = value;
            }
        }
        
        /// <summary>
        /// Allows an icontent's metadata to be adjusted by choices in the SmartFormData object
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public IEnumerable<IMetaData> ResolveMetaData(IContent content)
        {
            return content.MetaData;
        }
    }
}