using System;
using WSOL.EktronCms.ContentMaker.Attributes;
using WSOL.EktronCms.ContentMaker.Extensions;
using WSOL.EktronCms.ContentMaker.Interfaces;
using WSOL.IocContainer;

namespace WSOL.Custom.ContentMaker.Samples.Models
{
    // XmlConfigId should match definition ID in workarea
    [ContentDescriptor(XmlConfigId = -127)]
    public class BlogPost : SummaryBase<ContentTypes.BlogPost.root>
    {
        public BlogPost()
            : base()
        {
        }

        public BlogPost(IContent c, string xml)
            : base(c, xml)
        {
        }

        /// <summary>
        /// This property demonstrates how to change an ID reference to a fully populated data instance
        /// </summary>
        public BlogAuthor Author
        {
            get
            {
                if (SmartFormData.Author.Value > 0)
                    return SmartFormData.Author.Value.GetContent<BlogAuthor>(this.GetLanguageId());

                return null;
            }
            set
            {
                if (SmartFormData.Author == null)
                    SmartFormData.Author = new ContentTypes.BlogPost.ContentIdRefType();

                SmartFormData.Author.Value = value.Id;
            }
        }

        public string Body
        {
            get
            {
                return SmartFormData.Body.Any.ToRichString();
            }
            set
            {
                SmartFormData.Body.Any = value.FromRichString();
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

        public override string ArticleDescription
        {
            get
            {
                return SmartFormData.Description;
            }
            set
            {
                SmartFormData.Description = value;
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
                    SmartFormData.Image = new ContentTypes.BlogPost.rootImage();

                if (SmartFormData.Image.img == null)
                    SmartFormData.Image.img = new ContentTypes.BlogPost.imgDesignType();

                SmartFormData.Image.img.src = _ArticleImage.Src;
                SmartFormData.Image.img.alt = _ArticleImage.Alt;
            }
        }
    }
}