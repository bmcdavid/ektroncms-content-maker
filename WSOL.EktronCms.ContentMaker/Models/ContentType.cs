namespace WSOL.EktronCms.ContentMaker.Models
{
    using Extensions;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;
    using WSOL.EktronCms.ContentMaker.Interfaces;
    using WSOL.IocContainer;

    public abstract class ContentType<T> : HtmlContent where T : new()
    {
        [XmlIgnore]
        public new T SmartFormData
        {
            get;
            protected set;
        }

        public ContentType()
        {
            Initialize(string.Empty);
        }

        public ContentType(IContent c, string xml) : base(c)
        {
            Initialize(xml);
        }

        protected ContentType(SerializationInfo info, StreamingContext context) : base(info, context)
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