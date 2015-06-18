namespace WSOL.Custom.ContentMaker.Samples.Views.AccordionsOrTabs
{
    using System;
    using WSOL.ObjectRenderer.Attributes;
    using WSOL.ObjectRenderer.WebControls;

    [TemplateDescriptor(Path = "~/Views/AccordionsOrTabs/Default.ascx", Default = true, RequireTags = false)]
    public partial class Default : ControlBase<WSOL.Custom.ContentMaker.Samples.Models.AccordionsOrTabs>
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }
    }
}