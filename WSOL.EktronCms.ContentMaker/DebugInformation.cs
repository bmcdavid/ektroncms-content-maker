namespace WSOL.EktronCms.ContentMaker
{
    using Extensions;
    using HttpModules;
    using System;
    using System.Collections.Generic;
    using WSOL.EktronCms.ContentMaker.Attributes;
    using WSOL.IocContainer;

    public static class DebugInformation
    {
        public static bool EnableViewWatcher { get { return FileWatcherModule.EnableViewWatcher; } }

        public static bool IsWebsite { get { return FileWatcherModule.IsWebsite; } }

        public static bool ViewWatcherSet { get { return FileWatcherModule.ViewWatcher != null; } }

        public static Dictionary<CustomTuple<Type, string>, TemplateDescriptorAttribute> ResolvedTemplates
        {
            get
            {
                return new Dictionary<CustomTuple<Type, string>, TemplateDescriptorAttribute>(TemplateExtensions._ResolvedTemplates);
            }
        }

        public static Dictionary<TemplateDescriptorAttribute, Type> TemplateDescriptors
        {
            get
            {
                return new Dictionary<TemplateDescriptorAttribute, Type>(TemplateExtensions._DescriptorList);
            }
        }

        public static Dictionary<Type, ContentDescriptorAttribute> ContentDescriptors
        {
            get
            {
                return new Dictionary<Type, ContentDescriptorAttribute>(CompilationExtensions._TypedContentDictionary);
            }
        }
    }
}