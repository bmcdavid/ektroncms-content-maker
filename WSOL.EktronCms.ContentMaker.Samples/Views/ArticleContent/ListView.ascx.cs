namespace WSOL.Custom.ContentMaker.Samples.Views.ArticleContent
{
    using WSOL.EktronCms.ContentMaker;
    using WSOL.EktronCms.ContentMaker.Attributes;
    using WSOL.EktronCms.ContentMaker.WebControls;

    [TemplateDescriptor(Path = "~/Views/ArticleContent/ListView.ascx", Default = false, RequireTags = true, Inherited = true,
        Tags = new string[] { Tags.ListView })]
    public partial class ListView : ControlBase<WSOL.Custom.ContentMaker.Samples.Models.ArticleContent>
    {
         
    }
}