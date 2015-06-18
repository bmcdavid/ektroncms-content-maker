namespace WSOL.EktronCms.ContentMaker.Samples.Views.AccordionsOrTabs
{
    using System;
    using WSOL.ObjectRenderer.Attributes;
    using WSOL.ObjectRenderer.WebControls;    

    [TemplateDescriptor(Path = "~/Views/AccordionsOrTabs/AccordionItemView.ascx", Default = false, Inherited = false, RequireTags = true, Tags = new string[] { "AccordionItem" })]
    public partial class AccordionView : ControlBase<WSOL.Custom.ContentMaker.Samples.Models.AccordionOrTabItem>
    {
        // Limited logic should be done in code behind, if any. Please use extensions of the front end.
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }
    }
}