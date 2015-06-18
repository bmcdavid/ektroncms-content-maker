namespace WSOL.EktronCms.ContentMaker.Views.HtmlContent
{
    using WSOL.ObjectRenderer.Attributes;
    using WSOL.ObjectRenderer.WebControls;

    [TemplateDescriptor(Path = "~/Views/HtmlContent/ListView.ascx", Default = false, RequireTags = false, Inherited = true, Tags = new string[] { Tags.ListView })]
    public partial class ListView : ControlBase<Models.HtmlContent>
    {

    }
}