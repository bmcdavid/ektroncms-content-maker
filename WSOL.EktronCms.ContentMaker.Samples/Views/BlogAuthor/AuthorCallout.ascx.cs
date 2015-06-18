namespace WSOL.Custom.ContentMaker.Samples.Views.BlogAuthor
{
    using System;
    using WSOL.ObjectRenderer.Attributes;
    using WSOL.ObjectRenderer.WebControls;

    [TemplateDescriptor(Path = "~/Views/BlogAuthor/AuthorCallout.ascx", Default = false, Inherited = false, Tags = new string[] { "AuthorCallout" })]
    public partial class AuthorCallout : ControlBase<Samples.Models.BlogAuthor>
    {
        // Limited logic should be done in code behind, if any. Please use extensions of the front end.
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }
    }
}