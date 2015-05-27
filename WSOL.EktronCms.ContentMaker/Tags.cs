namespace WSOL.EktronCms.ContentMaker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.Compilation;

    /// <summary>
    /// To extend tags, inherit the class and create public const strings for your custom tags
    /// </summary>
    public class Tags
    {
        public const string DetailView = "DetailView";
        public const string ListView = "ListView";
        public const string GalleryView = "GalleryView";
        public const string MobileView = "MobileView";
        public const string ComplexView = "ComplexView"; // used to keep nesting of accordions / tabs within one another.
        public const string Callout = "Callout";
        public const string SecondaryContent = "SecondaryContent";
        public const string Sidebar = "Sidebar";
        public const string RelatedContent = "RelatedContent";
        public const string RelatedContentLeft = "RelatedContentLeft";
        public const string RelatedContentRight = "RelatedContentRight";

        private static IEnumerable<string> _TagList;
        private static object lockObject = new object();

        /// <summary>
        /// Gets all tags in the system that inherit from WSOL.EktronCms.ContentMaker.Tags
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetAllTags()
        {
            if (_TagList != null)
                return _TagList;

            lock (lockObject)
            {
                _TagList = new List<string>();
                Type t = typeof(WSOL.EktronCms.ContentMaker.Tags);

                foreach (Assembly a in BuildManager.GetReferencedAssemblies())
                {
                    foreach (Type type in a.GetTypes())
                    {
                        if (type == t || type.IsSubclassOf(t))
                        {
                            foreach (FieldInfo field in type.GetFields())
                            {
                                if (field.FieldType == typeof(string) && field.IsLiteral && !field.IsInitOnly)
                                    ((List<string>)_TagList).Add(field.GetRawConstantValue().ToString());
                            }
                        }
                    }
                }

                // Order alphabetically
                _TagList = _TagList.OrderBy(x => x);

                return _TagList;
            }
        }
    }
}