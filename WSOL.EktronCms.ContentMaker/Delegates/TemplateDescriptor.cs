namespace WSOL.EktronCms.ContentMaker.Delegates
{
    using System.Collections.Generic;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using WSOL.EktronCms.ContentMaker.Attributes;
    using WSOL.EktronCms.ContentMaker.Interfaces;

    public delegate TemplateDescriptorAttribute TemplateResolverHandler(TemplateDescriptorAttribute T, string[] Tags, IContent Item);

    public delegate HtmlGenericControl InsertWrapperHandler(HtmlGenericControl Wrapper, IContent Item);

    public delegate void NullContentHandler(HtmlTextWriter writer);

    public delegate string DebugContentHandler(IDictionary<IContent,TemplateDescriptorAttribute> items, string[] Tags);

}