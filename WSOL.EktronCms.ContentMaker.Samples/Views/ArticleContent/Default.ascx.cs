namespace WSOL.Custom.ContentMaker.Samples.Views.ArticleContent
{
    using System;
    using WSOL.ObjectRenderer.Attributes;
    using WSOL.ObjectRenderer.WebControls;

    [TemplateDescriptor(Default = true, Path = "~/Views/ArticleContent/Default.ascx")]
    public partial class Default : ControlBase<WSOL.Custom.ContentMaker.Samples.Models.ArticleContent>
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }
    }
}