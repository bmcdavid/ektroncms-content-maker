namespace WSOL.Custom.ContentMaker.Samples.Views.BlogPost
{
    using System;
    using WSOL.ObjectRenderer.Attributes;
    using WSOL.ObjectRenderer.WebControls;

    [TemplateDescriptor(Path = "~/Views/BlogPost/Default.ascx", Default = true, Inherited = false, Tags = new string[] { })]
    public partial class Default : ControlBase<WSOL.Custom.ContentMaker.Samples.Models.BlogPost>
    {
        // Limited logic should be done in code behind, if any. Please use extensions of the front end.
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }
    }
}