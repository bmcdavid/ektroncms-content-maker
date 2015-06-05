namespace Custom.ContentMaker
{
    using DryIoc;
    using System;
    using System.Collections.Generic;
    using WSOL.IocContainer;

    /// <summary>
    /// Summary description for EkXmlSerializer
    /// </summary>
    public class EktronXmlSerializer : IXmlSerializer
    {
        public object DefaultSettings
        {
            get { return null; }
        }

        public object Deserialize(string data, Type type, object settings = null, System.Text.Encoding encoding = null, IEnumerable<Type> additionalTypes = null)
        {
            return Ektron.Cms.EkXml.Deserialize(type, data);
        }

        public T Deserialize<T>(string data, object settings = null, System.Text.Encoding encoding = null, IEnumerable<Type> additionalTypes = null)
        {
            return (T)Deserialize(data, typeof(T), settings, encoding, additionalTypes);
        }

        public string Serialize(object data, object settings = null, System.Text.Encoding encoding = null, IEnumerable<Type> additionalTypes = null)
        {
            return Ektron.Cms.EkXml.Serialize(data.GetType(), data);
        }
    }

    public class CustomXsltTranformer : WSOL.IocContainer.Services.XsltTransformer
    {
        public CustomXsltTranformer() : base() { }
    }

    // this allows us to overwrite default IXmlSerializer provided by WSOL
    [InitializationDependency(typeof(WSOL.IocContainer.Services.Initialization))]
    public class RegisterXml : IConfigureContainer
    {
        public void ConfigureContainer(DryIoc.IRegistry registry)
        {
            registry.Register<IXmlSerializer, EktronXmlSerializer>(Reuse.Singleton);

            // temporary fix until WSOL.IOCContainer is updated to pick default constructor
            registry.Register<IXsltTransformer, CustomXsltTranformer>(Reuse.Singleton);
        }
    }
}