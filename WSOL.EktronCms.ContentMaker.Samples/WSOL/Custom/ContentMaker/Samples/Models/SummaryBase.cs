using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using WSOL.EktronCms.ContentMaker.Extensions;
using WSOL.EktronCms.ContentMaker.Interfaces;
using WSOL.EktronCms.ContentMaker.Models;
using WSOL.IocContainer;

namespace WSOL.Custom.ContentMaker.Samples.Models
{
    public abstract class SummaryBase : HtmlContent
    {
        public SummaryBase()
            : base()
        {
        }

        public SummaryBase(IContent c)
            : base(c)
        {
        }

        protected SummaryBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #region Common properties for content that derives from this class

        public abstract DateTime ArticleDate { get; set; }

        public abstract HtmlImage ArticleImage { get; set; }

        public abstract string ArticleDescription { get; set; }

        #endregion Common properties for content that derives from this class

        protected static string StripHtml(string s)
        {
            return Regex.Replace(s, "<.*?>", string.Empty);
        }
    }

    /// <summary>
    /// Generic simplifies creation of properly typed smart form classes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SummaryBase<T> : SummaryBase where T : new()
    {
        [XmlIgnore]
        public new T SmartFormData
        {
            get;
            protected set;
        }

        public SummaryBase()
        {
            Initialize(string.Empty);
        }

        public SummaryBase(IContent c, string xml)
            : base(c)
        {
            Initialize(xml);
        }

        protected SummaryBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Initialize(string.Empty);
        }

        protected virtual void Initialize(string xml = "")
        {
            if (string.IsNullOrEmpty(xml))
            {
                SmartFormData = typeof(T).NewInstance<T>();
            }
            else
            {
                SmartFormData = xml.FromXml<T>();
            }

            base.SmartFormData = SmartFormData;
        }
    }
}