namespace WSOL.EktronCms.ContentMaker.Attributes
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using WSOL.EktronCms.ContentMaker.Enums;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TemplateDescriptorAttribute : Attribute
    {
        /// <summary>
        /// Tags that define where to display the content item
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        /// Path to ASCX, XSLT, or Razor template
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Defines if this is the default template for this type, note: each type should only have 1 default!
        /// </summary>
        public bool Default { get; set; }

        /// <summary>
        /// Defines if the WSOL:Renderer requires tags to display this template
        /// </summary>
        public bool RequireTags { get; set; }

        /// <summary>
        /// Defines if types derived the ModelType can use this template
        /// </summary>
        public bool Inherited { get; set; }

        /// <summary>
        /// Model class to define template for
        /// </summary>
        public Type ModelType { get; protected set; }

        /// <summary>
        /// ASCX or XSLT
        /// </summary>
        public TemplateType TemplateType { get; set; }

        /// <summary>
        /// Additional class types needed to serialize content item
        /// </summary>
        public Type[] AdditionalTypes { get; set; }

        public TemplateDescriptorAttribute()
        {
            TemplateType = TemplateType.UserControl;
            Tags = new string[] { };
            AdditionalTypes = null;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetModelType(Type t)
        {
            ModelType = t;
        }

        public override int GetHashCode()
        {
            if (!String.IsNullOrEmpty(Path))
            {
                return Path.GetHashCode();
            }

            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            TemplateDescriptorAttribute a = obj as TemplateDescriptorAttribute;

            if (a == null)
            {
                return false;
            }

            return string.Compare(this.Path, a.Path, true) == 0;
        }

        public override string ToString()
        {
            return string.Format("Template Descriptor, Path = {0}, Model = {1}, FullView = {2}, Required Tags = {3}, Tags = {4}, Template Type = {5}, Additional Types = {6}",
                Path,
                ModelType != null ? ModelType.FullName : "No Model assigned!",
                Default,
                RequireTags,
                Tags != null ? string.Join(",", Tags) : string.Empty,
                TemplateType,
                AdditionalTypes != null ? string.Join(",", AdditionalTypes.Select(x => x.FullName).ToArray()) : string.Empty
            );
        }
    }
}