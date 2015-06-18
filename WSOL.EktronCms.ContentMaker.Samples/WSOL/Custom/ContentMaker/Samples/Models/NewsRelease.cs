using System;
using System.Linq;
using WSOL.EktronCms.ContentMaker.Attributes;
using WSOL.EktronCms.ContentMaker.Interfaces;
using WSOL.IocContainer;

namespace WSOL.Custom.ContentMaker.Samples.Models
{
    // XmlConfigId should match definition ID in workarea
    [ContentDescriptor(XmlConfigId = -126, Description = "Model to represent news article")]
    public class NewsRelease : SummaryBase<ContentTypes.NewsRelease.root>
    {
        public NewsRelease()
            : base()
        {
        }

        public NewsRelease(IContent c, string xml)
            : base(c, xml)
        {
        }

        public string Body
        {
            get
            {
                return SmartFormData.Body.Any.ToArray().ToRichString();
            }
            set
            {
                SmartFormData.Body.Any = value.FromRichString().ToList();
            }
        }

        public override DateTime ArticleDate
        {
            get
            {
                return SmartFormData.Date.ToDateTime();
            }
            set
            {
                SmartFormData.Date = value.ToString();
            }
        }

        private WSOL.IocContainer.HtmlImage _ArticleImage;

        /// <summary>
        /// Example of how to do null checks within model to create a friendly type for front-end display
        /// </summary>
        public override WSOL.IocContainer.HtmlImage ArticleImage
        {
            get
            {
                if (_ArticleImage == null)
                {
                    if (SmartFormData.Image == null || SmartFormData.Image.img == null)
                        return null;

                    _ArticleImage = new HtmlImage() { Alt = SmartFormData.Image.img.alt, Src = SmartFormData.Image.img.src };
                }

                return _ArticleImage;
            }
            set
            {
                _ArticleImage = value;

                if (SmartFormData.Image == null)
                    SmartFormData.Image = new ContentTypes.NewsRelease.rootImage();

                if (SmartFormData.Image.img == null)
                    SmartFormData.Image.img = new ContentTypes.NewsRelease.imgDesignType();

                SmartFormData.Image.img.src = _ArticleImage.Src;
                SmartFormData.Image.img.alt = _ArticleImage.Alt;
            }
        }

        private string _ArticleDescription;

        /// <summary>
        /// News release content type doesn't contain description, so we'll just make one from the body...
        /// </summary>
        public override string ArticleDescription
        {
            get
            {
                _ArticleDescription = StripHtml(SmartFormData.Body.Any.ToArray().ToRichString());

                if (_ArticleDescription.Length > 100)
                    return _ArticleDescription.Substring(0, 100);

                return _ArticleDescription;
            }
            set
            {
                _ArticleDescription = value;
            }
        }
    }
}