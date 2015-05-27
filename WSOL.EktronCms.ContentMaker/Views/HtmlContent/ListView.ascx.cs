namespace WSOL.EktronCms.ContentMaker.Views.HtmlContent
{
    using WSOL.EktronCms.ContentMaker.Attributes;
    using WSOL.EktronCms.ContentMaker.WebControls;

    [TemplateDescriptor(Path = "~/Views/HtmlContent/ListView.ascx", Default = false, RequireTags = false, Inherited = true, Tags = new string[] { Tags.ListView })]
    public partial class ListView : ControlBase<Models.HtmlContent>
    {

    }
}