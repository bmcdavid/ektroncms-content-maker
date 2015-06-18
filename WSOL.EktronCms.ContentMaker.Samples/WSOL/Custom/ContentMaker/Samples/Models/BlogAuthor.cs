using System.Collections.Generic;
using WSOL.EktronCms.ContentMaker.Attributes;
using WSOL.EktronCms.ContentMaker.Extensions;
using WSOL.EktronCms.ContentMaker.Interfaces;
using WSOL.EktronCms.ContentMaker.Models;
using WSOL.IocContainer;

namespace WSOL.Custom.ContentMaker.Samples.Models
{
    [ContentDescriptor(XmlConfigId = -128)]
    public class BlogAuthor : ContentType<ContentTypes.BlogAuthor.root>
    {
        public BlogAuthor()
            : base()
        {
        }

        public BlogAuthor(IContent c, string xml)
            : base(c, xml)
        {
        }

        public string FullName
        {
            get
            {
                return string.Concat(SmartFormData.FirstName, " ", SmartFormData.LastName);
            }
        }

        public string FirstName
        {
            get
            {
                return SmartFormData.FirstName;
            }
            set
            {
                SmartFormData.FirstName = value;
            }
        }

        public string LastName
        {
            get
            {
                return SmartFormData.LastName;
            }
            set
            {
                SmartFormData.LastName = value;
            }
        }

        public IEnumerable<BlogPost> BlogPosts
        {
            get
            {
                if (SmartFormData.BlogPostsFolder == null || SmartFormData.BlogPostsFolder.Value == 0)
                    return null;

                var criteria = this.GetContentCriteria<BlogPost>();
                criteria.AddFilter(Ektron.Cms.Common.ContentProperty.FolderId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, SmartFormData.BlogPostsFolder.Value);
                criteria.FolderRecursive = true;

                return criteria.GetContent<BlogPost>();
            }
        }

        public string AuthorDescription
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

        public string Bio
        {
            get
            {
                return SmartFormData.Bio.Any.ToRichString();
            }
            set
            {
                SmartFormData.Bio.Any = value.FromRichString();
            }
        }

        private WSOL.IocContainer.HtmlImage _Image;

        /// <summary>
        /// Example of how to do null checks within model to create a friendly type for front-end display
        /// </summary>
        public WSOL.IocContainer.HtmlImage Image
        {
            get
            {
                if (_Image == null)
                {
                    if (SmartFormData.Image == null || SmartFormData.Image.img == null)
                        return null;

                    _Image = new HtmlImage() { Alt = SmartFormData.Image.img.alt, Src = SmartFormData.Image.img.src };
                }

                return _Image;
            }
            set
            {
                _Image = value;

                if (SmartFormData.Image == null)
                    SmartFormData.Image = new ContentTypes.BlogAuthor.rootImage();

                if (SmartFormData.Image.img == null)
                    SmartFormData.Image.img = new ContentTypes.BlogAuthor.imgDesignType();

                SmartFormData.Image.img.src = _Image.Src;
                SmartFormData.Image.img.alt = _Image.Alt;
            }
        }
    }
}