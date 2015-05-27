namespace WSOL.EktronCms.ContentMaker.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using WSOL.EktronCms.ContentMaker.Attributes;
    using WSOL.EktronCms.ContentMaker.Delegates;
    using WSOL.EktronCms.ContentMaker.Enums;
    using WSOL.EktronCms.ContentMaker.Extensions;
    using WSOL.EktronCms.ContentMaker.Interfaces;
    using WSOL.IocContainer;

    /// <summary>
    /// Renders an IContent item or list of IContent items
    /// </summary>
    [DefaultProperty("Item"), ToolboxData("<{0}:Renderer runat=server></{0}:Renderer>")]
    public class Renderer : WebControl, INamingContainer
    {
        private static ILogger _Logger = InitializationContext.Locator.Get<ILogger>();

        private static ICacheManager _CacheManager = InitializationContext.Locator.Get<ICacheManager>();

        private static IXsltTransformer _XsltTransformer = InitializationContext.Locator.Get<IXsltTransformer>();

        private static IApplicationHelper _ApplicationHelper = InitializationContext.Locator.Get<IApplicationHelper>();

        #region Constructor

        public Renderer()
        {
            CacheInterval = _CacheManager.QuickInterval;
            ShowXsltErrors = false;
            Item = null;
            ItemList = null;
            Tags = null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Renders a single items, ignored if Items is set;
        /// </summary>
        public object Item { get; set; }

        /// <summary>
        /// Renders a list of items
        /// </summary>
        public IEnumerable<object> ItemList { get; set; }

        /// <summary>
        /// Comma delimited string of tags to use for this renderer, overrides Tags[] if used
        /// </summary>
        public string TagsString { get; set; }

        /// <summary>
        /// Array of tags to use for this renderer
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        /// Used for XSLT transforms only
        /// </summary>
        public bool ShowXsltErrors { get; set; }

        /// <summary>
        /// Used for XSLT transform only
        /// </summary>
        public int CacheInterval { get; set; }

        /// <summary>
        /// Wraps each item in the given tag.
        /// </summary>
        public string ItemWrapTag { get; set; }

        /// <summary>
        /// Wrapper tag for rendered items.
        /// </summary>
        public string WrapTag { get; set; }

        /// <summary>
        /// If true, executes DebugContentHandler to write out debug information
        /// </summary>
        public bool DebugMode { get; set; }

        /// <summary>
        /// Executes after the template has been looked up.
        /// </summary>
        public event TemplateResolverHandler TemplateResolved;

        /// <summary>
        /// Executes when an item wrapper is specified and added to the renderer
        /// </summary>
        public event InsertWrapperHandler InsertItemWrapper;

        /// <summary>
        /// Executes when the Item or Items is null
        /// </summary>
        public event NullContentHandler NullContent;

        /// <summary>
        /// Executes if DebugMode is true
        /// </summary>
        public event DebugContentHandler DebugWrite;

        /// <summary>
        /// Tracks when BuildRenderer has executed.
        /// </summary>
        private bool _Executed = false;

        #endregion

        #region Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            BuildRenderer();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            BuildRenderer();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            BuildRenderer();
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            BuildRenderer();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            // handle nulls
            if (ItemList == null && NullContent != null)
            {
                NullContent(writer);
                return;
            }

            base.Render(writer);
        }

        /// <summary>
        /// Writes custom HTML tag if WrapTag is set
        /// </summary>
        /// <param name="writer"></param>
        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            if (!string.IsNullOrEmpty(WrapTag))
            {
                writer.WriteBeginTag(WrapTag);

                if (!string.IsNullOrEmpty(CssClass))
                {
                    writer.WriteAttribute("class", CssClass);
                }

                if (Attributes != null && Attributes.Count > 0)
                {
                    foreach (string key in Attributes.Keys)
                    {
                        writer.WriteAttribute(key, Attributes[key]);
                    }
                }

                writer.Write(HtmlTextWriter.TagRightChar);
            }
        }

        /// <summary>
        /// Writes custom HTML tag if WrapTag is set
        /// </summary>
        /// <param name="writer"></param>
        public override void RenderEndTag(HtmlTextWriter writer)
        {
            if (!string.IsNullOrEmpty(WrapTag))
            {
                writer.WriteEndTag(WrapTag);
            }
        }

        /// <summary>
        /// Checked during several events of the page life cycle, Ideally should be one OnLoad or OnInit if form elements are used.
        /// </summary>
        private void BuildRenderer()
        {
            if (_Executed)
            {
                return;
            }

            // Create a list of 1
            if ((ItemList == null || !ItemList.Any()) && Item != null)
            {
                ItemList = new List<object>() { Item };
            }

            // Stop processing if no items are found.
            if (ItemList == null)
            {
                return;
            }

            IEnumerable<IContent> renderingItems = ItemList.OfType<IContent>();

            // stores item for debug write
            Dictionary<IContent, TemplateDescriptorAttribute> debugItems = null;

            if (renderingItems.Any())
            {
                // Keeps from executing several times in page life cycle
                _Executed = true;

                if (DebugMode)
                    debugItems = new Dictionary<IContent, TemplateDescriptorAttribute>();

                // Control to be loaded
                Control c = null;

                // Determines if Item is a backend type or not.
                var ContentDescriptors = Extensions.CompilationExtensions.GetContentDescriptors();

                if (!String.IsNullOrEmpty(TagsString))
                    Tags = TagsString.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

                if (Tags == null)
                    Tags = new string[] { };

                // Render each item
                foreach (IContent item in renderingItems)
                {
                    ContentDescriptorAttribute contentDescriptor = null;
                    Type contentType = item.GetType();

                    ContentDescriptors.TryGetValue(contentType, out contentDescriptor);

                    // Skip rendering back end content!
                    if (contentDescriptor != null && contentDescriptor.BackendContent)
                    {
                        continue;
                    }

                    HtmlGenericControl wrapper = null;

                    if (!string.IsNullOrEmpty(ItemWrapTag))
                    {
                        wrapper = new HtmlGenericControl() { TagName = ItemWrapTag };
                    }

                    TemplateDescriptorAttribute t = contentType.ResolveTemplate(TemplateResolved, Tags, item);

                    // No renderer resolved
                    if (t == null || string.IsNullOrEmpty(t.Path))
                    {
                        _Logger.Log("Template descriptor is null for ID" + item.Id, typeof(Renderer).FullName, null, System.Diagnostics.EventLogEntryType.Warning, item);
                        return;
                    }

                    // Possible opportunity to add Razor renderings
                    switch (t.TemplateType)
                    {
                        //case TemplateType.Razor:
                        //    RazorTemplates.Core.ITemplate<IContent> template = RazorTemplates.Core.Template.Compile<IContent>(_ApplicationHelper.GetFileAsString(t.Path));
                        //    c = new LiteralControl( template.Render(item));

                        //    break;

                        case TemplateType.XSLT:
                            string xml = Item.ToXml(null, t.AdditionalTypes, null);

                            c = new LiteralControl(
                                _XsltTransformer.Transform(
                                    xml,
                                    t.Path,
                                    string.Format("WSOL:Cache:IContentRenderer:ID={0}:L={1},Tags={2},Path={3}", item.Id, item.LanguageId, string.Join(",", Tags), t.Path),
                                    CacheInterval,
                                    ShowXsltErrors
                                )
                            );

                            break;

                        case TemplateType.UserControl:
                        default:
                            c = t.Path.Load(item);

                            break;
                    }

                    // Add control to renderer
                    if (c != null)
                    {
                        if (wrapper != null)
                        {
                            if (InsertItemWrapper != null)
                            {
                                wrapper = InsertItemWrapper(wrapper, item);
                            }

                            if (wrapper != null) // just in case delegate returns null
                            {
                                wrapper.Controls.Add(c);
                            }

                            this.Controls.Add(wrapper);
                        }
                        else
                        {
                            this.Controls.Add(c);
                        }
                    }

                    if (DebugMode)
                    {
                        debugItems.Add(item, t);
                    }

                }
            }
            else
            {
                _Logger.Log("Item or ItemList must implement IContent, to display in the renderer control!", typeof(Renderer).FullName, null, System.Diagnostics.EventLogEntryType.Warning, null);
                return;
            }

            if (DebugMode)
            {
                string output = string.Empty;

                if (DebugWrite == null)
                    DebugWrite += Default_DebugWrite;

                output += DebugWrite(debugItems, Tags);

                this.Controls.Add(new LiteralControl(output));
            }
        }

        /// <summary>
        /// Creates a PRE HTML tag of Item Ids, Names, XmlConfigIDs, TemplateType and Template Path
        /// </summary>
        /// <param name="items"></param>
        /// <param name="Tags"></param>
        /// <returns></returns>
        public string Default_DebugWrite(IDictionary<IContent, TemplateDescriptorAttribute> items, string[] Tags)
        {
            StringBuilder sb = new StringBuilder(200);

            if (Tags != null && Tags.Any())
            {
                sb.AppendLine(string.Format("Tags: {0}", string.Join(",", Tags)));
            }
            else
            {
                sb.AppendLine("Renderer Tags: Null or Empty!");
            }

            if (items == null || !items.Any())
            {
                sb.AppendLine("IContent Items: Null or Empty!");
            }
            else
            {
                sb.AppendLine("IContent Item Count: " + items.Count);

                foreach (var item in items)
                {
                    sb.AppendLine(string.Format("ID: {0}, Name: {1}, XmlID: {2}, FullView Template: {3}, Inherited Template: {4}, Template Path: {5}",
                        item.Key.Id,
                        item.Key.Title,
                        item.Key.XmlConfigId,
                        item.Value.Default,
                        item.Value.Inherited,
                        item.Value.Path
                    ));
                }
            }

            return string.Format("<pre>Renderer Debug Information\r\n{0}</pre>", sb.ToString());
        }

        #endregion
    }
}