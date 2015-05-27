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
    [ContentDescriptor(XmlConfigId = -170, Description = "Glossary Example", BackendContent = true)]
    public class LanguageGlossaryContent : ContentType<ContentTypes.LanguageGlossary.root>, IGlossary
    {
        private int CurrentLanguage = WSOL.EktronCms.ContentMaker.FrameworkFactory.CurrentLanguage();    

        public LanguageGlossaryContent()
        {
            
        }

        public LanguageGlossaryContent(IContent c, string xml) : base(c, xml)
        {

        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        protected LanguageGlossaryContent(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            SmartFormData = new ContentTypes.LanguageGlossary.root();
            base.SmartFormData = SmartFormData;
        }

        public int GlossaryLanguageId
        {
            get
            {
                return base.LanguageId;
            }
            set
            {
                base.LanguageId = value;
            }
        }

        private Dictionary<string, string> _GlossarySet = null;

        public Dictionary<string, string> GlossarySet
        {
            get
            {
                if (_GlossarySet == null)
                {
                    if (SmartFormData != null && SmartFormData.LanguageSets.Any())
                    {
                        _GlossarySet = new Dictionary<string, string>();

                        SmartFormData.LanguageSets.All
                        (
                            x =>
                            {
                                if (!_GlossarySet.ContainsKey(x.Key))
                                    _GlossarySet.Add(x.Key, x.Value);

                                return true;
                            }
                        );
                    }
                }

                return _GlossarySet;
            }
            set
            {
                _GlossarySet = value;

                if (SmartFormData != null && _GlossarySet != null)
                {
                    List<ContentTypes.LanguageGlossary.rootSet> items = new List<ContentTypes.LanguageGlossary.rootSet>();
                    _GlossarySet = new Dictionary<string, string>();

                    _GlossarySet.All
                    (
                        x =>
                        {
                            items.Add(new ContentTypes.LanguageGlossary.rootSet() { Key = x.Key, Value = x.Value });

                            return true;
                        }
                    );

                    SmartFormData.LanguageSets = items.ToArray();
                }
            }
        }
    }
}