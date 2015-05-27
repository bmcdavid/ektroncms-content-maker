namespace WSOL.EktronCms.ContentMaker.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using WSOL.IocContainer;

    public static class XmlExtensions
    {
        private static IXmlSerializer _Serializer;

        static XmlExtensions()
        {
            _Serializer = InitializationContext.Locator.Get<IXmlSerializer>();
        }

        public static string ToXml(this object o, Encoding encoding = null, IEnumerable<Type> additionalTypes = null, object settings = null)
        {
            return _Serializer.Serialize(o, settings, encoding ?? Encoding.Default, additionalTypes);
        }

        public static object FromXml(this string s, Type t, Encoding encoding = null, IEnumerable<Type> additionalTypes = null, object settings = null)
        {
            return _Serializer.Deserialize(s, t, settings, encoding ?? Encoding.Default, additionalTypes);
        }

        public static T FromXml<T>(this string s, Encoding encoding = null, IEnumerable<Type> additionalTypes = null, object settings = null)
        {
            return _Serializer.Deserialize<T>(s, settings, encoding ?? Encoding.Default, additionalTypes);
        }

    }
}