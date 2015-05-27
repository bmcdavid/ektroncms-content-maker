namespace WSOL.Custom.ContentMaker.Samples.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using WSOL.EktronCms.ContentMaker.Attributes;
    using WSOL.EktronCms.ContentMaker.Interfaces;
    using WSOL.EktronCms.ContentMaker.Models;
    using WSOL.IocContainer;

    [Serializable]
    [ContentDescriptor(XmlConfigId = -1120, Description = "Site Settings Example", BackendContent = true)]
    public class SiteSettingscontent : ContentType<ContentTypes.SiteSettings.root>, ISettings
    {
        private int CurrentLanguage = WSOL.EktronCms.ContentMaker.FrameworkFactory.CurrentLanguage();

        public SiteSettingscontent()
        {
        }

        public SiteSettingscontent(IContent c, string xml)
            : base(c, xml)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        protected SiteSettingscontent(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public HtmlImage Logo
        {
            get
            {
                if (SmartFormData.Logo.img == null) return null;

                return new HtmlImage() { Alt = SmartFormData.Logo.img.alt, Src = SmartFormData.Logo.img.src };
            }
            set
            {
                if (SmartFormData.Logo.img == null)
                    SmartFormData.Logo.img = new ContentTypes.SiteSettings.imgDesignType();

                SmartFormData.Logo.img.alt = value.Alt;
                SmartFormData.Logo.img.src = value.Src;
            }
        }

        public string LogoLink
        {
            get
            {
                return SmartFormData.LogoLink;
            }
            set
            {
                SmartFormData.LogoLink = value;
            }
        }

        public long DefaultItemId
        {
            get
            {
                return SmartFormData.DefaultContentID.Value;
            }
            set
            {
                if (SmartFormData.DefaultContentID == null)
                    SmartFormData.DefaultContentID = new ContentTypes.SiteSettings.ContentIdRefType();

                SmartFormData.DefaultContentID.Value = value;
            }
        }

        public long DefaultFolderId
        {
            get
            {
                return SmartFormData.DefaultFolderID.Value;
            }
            set
            {
                if (SmartFormData.DefaultFolderID == null)
                    SmartFormData.DefaultFolderID = new ContentTypes.SiteSettings.FolderIdRefType();

                SmartFormData.DefaultFolderID.Value = value;
            }
        }

        public long MainMenuId
        {
            get
            {
                return SmartFormData.MainMenuID.ToInt64();
            }
            set
            {
                SmartFormData.MainMenuID = value.ToString();
            }
        }

        public string ErrorMessage { get; set; }

        public IEnumerable<long> FolderIds { get; set; }

        public IEnumerable<IMetaData> ResolveMetaData(IContent content)
        {
            return null;
        }
    }
}