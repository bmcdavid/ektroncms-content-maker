namespace WSOL.Custom.ContentMaker.Samples.Views.ArticleContent
{
    using WSOL.ObjectRenderer;
    using WSOL.ObjectRenderer.Attributes;
    using WSOL.ObjectRenderer.WebControls;
    
    [TemplateDescriptor(Path = "~/Views/ArticleContent/ListView.ascx", Default = false, RequireTags = true, Inherited = true,
        Tags = new string[] { Tags.ListView })]
    public partial class ListView : ControlBase<WSOL.Custom.ContentMaker.Samples.Models.ArticleContent>
    {
         
    }
}