namespace WSOL.Custom.ContentMaker.Samples.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using WSOL.EktronCms.ContentMaker.Attributes;
    using WSOL.EktronCms.ContentMaker.Interfaces;
    using WSOL.EktronCms.ContentMaker.Models;
    using WSOL.IocContainer; // contains many extensions like ToRichString(), FromRichString(), FromWrapCDATA(), ToCDATA()

    [Serializable]
    // xmlconfig ID must be set to ID of smart form config in Ektron
    [ContentDescriptor(XmlConfigId = -100, Description = "Article Example")]
    public class ArticleContent : ContentType<ContentTypes.Article.root>
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ArticleContent() { }

        /// <summary>
        /// All smart form models need a constructor with this signature, and derive from ContentType&lt;T&gt; where T is XSD class
        /// </summary>
        /// <param name="c"></param>
        /// <param name="xml"></param>
        public ArticleContent(IContent c, string xml) : base(c, xml) { }

        #endregion Constructors

        #region Serialization Helpers

        /// <summary>
        /// Used to serialize values
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Link", Link, typeof(HtmlHyperlink));
            info.AddValue("Heading", Heading, typeof(string));
            info.AddValue("Date", Date, typeof(DateTime));
            info.AddValue("Body", Body.ToCDATA(), typeof(string));

        }

        /// <summary>
        /// Used to deserialize into instance
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ArticleContent(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

            Link = (HtmlHyperlink)info.GetValue("Link", typeof(HtmlHyperlink));
            Heading = (string)info.GetValue("Heading", typeof(string));
            Date = info.GetValue("Date", typeof(object)).FromSerializedDateTime();
            Body = ((string)info.GetValue("Body", typeof(string))).FromWrapCDATA();
        }

        #endregion Serialization Helpers

        #region Properties

        /// <summary>
        /// Simplest example exposing a field from SmartFormData
        /// </summary>
        public string Heading
        {
            get
            {
                return SmartFormData.Heading;
            }
            set
            {
                SmartFormData.Heading = value;
            }
        }

        public DateTime Date
        {
            get
            {
                return SmartFormData.Date;
            }
            set
            {
                SmartFormData.Date = value;
            }
        }

        private HtmlImage _Image;

        /// <summary>
        /// Creates an HtmlImage from an smart form image selector
        /// </summary>
        public HtmlImage Image
        {
            get
            {
                if (_Image != null)
                    return _Image;

                if (SmartFormData.Image != null && SmartFormData.Image.img != null)
                    _Image = new HtmlImage() { Src = SmartFormData.Image.img.src, Alt = SmartFormData.Image.img.src };

                if (_Image == null)
                    _Image = new HtmlImage();

                return _Image;
            }
            set
            {
                _Image = value;

                if (_Image == null)
                    _Image = new HtmlImage();

                if (SmartFormData.Image == null)
                    SmartFormData.Image = new ContentTypes.Article.rootImage();

                if (SmartFormData.Image.img == null)
                    SmartFormData.Image.img = new ContentTypes.Article.imgDesignType();

                SmartFormData.Image.img.src = _Image.Src;
                SmartFormData.Image.img.alt = _Image.Alt;
            }
        }

        private HtmlHyperlink _Link;

        /// <summary>
        /// Creates a hyperlink from a smart form link selector
        /// </summary>
        public HtmlHyperlink Link
        {
            get
            {
                if (_Link != null)
                    return _Link;

                if (SmartFormData.Link != null && SmartFormData.Link.a != null)
                {
                    _Link = new HtmlHyperlink()
                    {
                        Href = SmartFormData.Link.a.href,
                        Text = SmartFormData.Link.a.Any.ToRichString(),
                        Target = SmartFormData.Link.a.target ?? string.Empty
                    };
                }

                return _Link;
            }
            set
            {
                _Link = value;

                if (_Link == null)
                    return;

                if (SmartFormData.Link == null)
                    SmartFormData.Link = new ContentTypes.Article.rootLink();

                if (SmartFormData.Link.a == null)
                    SmartFormData.Link.a = new ContentTypes.Article.aDesignType();

                SmartFormData.Link.a.href = _Link.Href;
                SmartFormData.Link.a.target = _Link.Target;
                SmartFormData.Link.a.Any = _Link.Text.FromRichString();
            }
        }

        /// <summary>
        /// Creates a rich text string from a rich text smart form field
        /// </summary>
        public string Body
        {
            get
            {
                if (SmartFormData.Body == null)
                    SmartFormData.Body = new ContentTypes.Article.rich();

                return SmartFormData.Body.Any.ToRichString();
            }
            set
            {
                if (SmartFormData.Body == null)
                    SmartFormData.Body = new ContentTypes.Article.rich();

                SmartFormData.Body.Any = value.FromRichString();
            }
        }

        private List<ArticleCallout> _Callouts;

        /// <summary>
        /// Creates callouts from repeatable smart form group box
        /// </summary>
        public List<ArticleCallout> Callouts
        {
            get
            {
                if (_Callouts != null)
                    return _Callouts;

                if (_Callouts == null)
                    _Callouts = new List<ArticleCallout>();

                if (SmartFormData.Callout == null)
                    SmartFormData.Callout = new ContentTypes.Article.rootCallout[] { };

                foreach (var item in SmartFormData.Callout)
                    _Callouts.Add(MakeCallout(item));

                return _Callouts;
            }
            set
            {
                _Callouts = value;

                List<ContentTypes.Article.rootCallout> callouts = new List<ContentTypes.Article.rootCallout>();

                // Convert
                foreach (var item in _Callouts)
                    callouts.Add(UnMakeCallout(item));

                SmartFormData.Callout = callouts.ToArray();
            }
        }
        #endregion Properties

        #region Makers and UnMakers

        private ArticleCallout MakeCallout(ContentTypes.Article.rootCallout callout)
        {
            ArticleCallout a = new ArticleCallout();

            if (callout == null)
                return a;

            a.Alignment = callout.Alignment.ToString();
            a.Bookmark = callout.Bookmark;
            a.Caption = callout.Caption.Any.ToRichString();
            a.Heading = callout.Heading;

            if (callout.Image != null && callout.Image.img != null)
                a.Image = new HtmlImage() { Alt = callout.Image.img.alt, Src = callout.Image.img.src };
            else
                a.Image = new HtmlImage();

            if (callout.Link != null && callout.Link.a != null)
                a.Link = new HtmlHyperlink() { Href = callout.Link.a.href, Target = callout.Link.a.target, Text = callout.Link.a.Any.ToRichString() };
            else
                a.Link = new HtmlHyperlink();

            return a;
        }

        private ContentTypes.Article.rootCallout UnMakeCallout(ArticleCallout callout)
        {
            ContentTypes.Article.rootCallout c = new ContentTypes.Article.rootCallout();

            if (callout == null)
                return c;

            c.Heading = callout.Heading;
            c.Bookmark = callout.Bookmark;

            if (c.Caption == null)
                c.Caption = new ContentTypes.Article.rich();

            c.Caption.Any = callout.Caption.FromRichString();

            if (c.Image == null)
                c.Image = new ContentTypes.Article.rootCalloutImage();

            if (callout.Image != null)
                c.Image.img = new ContentTypes.Article.imgDesignType() { alt = callout.Image.Alt, src = callout.Image.Src };

            if (c.Link == null)
                c.Link = new ContentTypes.Article.rootCalloutLink();

            if (callout.Link != null)
                c.Link.a = new ContentTypes.Article.aDesignType() { Any = callout.Link.Text.FromRichString(), href = callout.Link.Href, target = callout.Link.Target };

            return c;
        }

        #endregion

    }

    /// <summary>
    /// Simple model for a callout
    /// </summary>
    [Serializable]
    public class ArticleCallout
    {
        public ArticleCallout() { }

        public string Alignment { get; set; }
        public string Bookmark { get; set; }
        public HtmlHyperlink Link { get; set; }
        public HtmlImage Image { get; set; }
        public string Heading { get; set; }
        public string Caption { get; set; }
    }
}