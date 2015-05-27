namespace WSOL.EktronCms.ContentMaker.Attributes
{
    using System;

    /// <summary>
    /// Decorates content model classes to set xml config id for smart form, and toggle back-end content which keeps it from displaying when used in a renderer web control.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ContentDescriptorAttribute : Attribute
    {
        #region Constructors

        public ContentDescriptorAttribute() 
        { 
            XmlConfigId = 0;
            Description = "Generic HTML Content";
            BackendContent = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The Ektron Smart Form XmlConfigId
        /// </summary>
        public long XmlConfigId { get; set; }
        
        /// <summary>
        /// Description of Smart Form
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// If true, this allows controls like the WSOL:Renderer to not display its contents.
        /// </summary>
        public bool BackendContent { get; set; }

        #endregion

        #region Methods

        public override string ToString()
        {
            return string.Concat(XmlConfigId, BackendContent ? "(backend type only)" : string.Empty , ", ", Description);
        }

        #endregion
    }

}