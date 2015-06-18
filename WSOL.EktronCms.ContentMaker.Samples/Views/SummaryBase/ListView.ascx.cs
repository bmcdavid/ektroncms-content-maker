namespace WSOL.Custom.ContentMaker.Samples.Views.SummaryBase
{
    using System;
    using WSOL.ObjectRenderer.Attributes;
    using WSOL.ObjectRenderer.WebControls;

    [TemplateDescriptor(Path = "~/Views/SummaryBase/ListView.ascx", Default = false, Inherited = true, Tags = new string[] { WSOL.EktronCms.ContentMaker.Tags.ListView })]
    public partial class ListView : ControlBase<WSOL.Custom.ContentMaker.Samples.Models.SummaryBase>
    {
        // Limited logic should be done in code behind, if any. Please use extensions of the front end.
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }
    }
}