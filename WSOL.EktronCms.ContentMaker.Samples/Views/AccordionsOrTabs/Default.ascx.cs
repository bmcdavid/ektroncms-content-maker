namespace WSOL.Custom.ContentMaker.Samples.Views.AccordionsOrTabs
{
    using System;
    using System.Linq;
    using System.Web.UI;
    using WSOL.EktronCms.ContentMaker.Attributes;
    using WSOL.EktronCms.ContentMaker.WebControls;

    [TemplateDescriptor(Path = "~/Views/AccordionsOrTabs/Default.ascx", Default = true, RequireTags = false)]
    public partial class Default : ControlBase<WSOL.Custom.ContentMaker.Samples.Models.AccordionsOrTabs>
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (CurrentData.IsAccordion)
            {
                phAccordion.Visible = true;
                rptAccordionItems.DataSource = CurrentData.Items;
                rptAccordionItems.DataBind();
            }
            else
            {
                phTabs.Visible = true;
                rptTabItems.DataSource = CurrentData.Items;
                rptTabNames.DataSource = CurrentData.Items;

                rptTabNames.DataBind();
                rptTabItems.DataBind();
            }
        }
    }
}