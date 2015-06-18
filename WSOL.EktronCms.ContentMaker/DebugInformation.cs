namespace WSOL.EktronCms.ContentMaker
{
    using Extensions;
    using System;
    using System.Collections.Generic;
    using WSOL.EktronCms.ContentMaker.Attributes;

    public class DebugInformation : WSOL.ObjectRenderer.DebugInformation
    {
        public static Dictionary<Type, ContentDescriptorAttribute> ContentDescriptors
        {
            get
            {
                return new Dictionary<Type, ContentDescriptorAttribute>(CompilationExtensions._TypedContentDictionary);
            }
        }
    }
}