using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using WSOL.Custom.ContentMaker.Samples.Models;
using WSOL.EktronCms.ContentMaker.Extensions;

namespace WSOL.EktronCms.ContentMaker.Samples
{
    public partial class ContentRenderSamples : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // sets the item for the renderer to be the dynamic item
            // since no tags are set, templates using the default = true in the template descriptor attribute are preferred by the renderer, which should ideally be the full display
            wRendererForDynamicItem.Item = this.GetDynamicContent();

            // creates content criteria and sets xml config id filter to match value set in content descriptor of Article content
            var articleCriteria = this.GetContentCriteria<ArticleContent>();

            // with extensions, there is no need for a content manager, simply call getcontent from the criteria
            wRendererForArticles.ItemList = articleCriteria.GetContent();

            var mixedContentCriteria = this.GetContentCriteria();
            List<long> xmlTypeFilter = new List<long> { typeof(ArticleContent).GetXmlConfigId(), typeof(AccordionsOrTabs).GetXmlConfigId() };

            // restricts content to only these smart form types
            mixedContentCriteria.AddFilter(Ektron.Cms.Common.ContentProperty.XmlConfigurationId, Ektron.Cms.Common.CriteriaFilterOperator.In, xmlTypeFilter);

            // gets article and accordion or tab content
            // note, since the tag of 'ListView' is set on this control, the ~/Views/ArticleContent/ListView.ascx gets used for article items in the list
            // and for accordions, no list view is defined, so the default of ~/Views/HtmlContent/ListView.ascx gets used as its allowed for inherited types.
            wRendererForMixed.ItemList = mixedContentCriteria.GetContent();

        }

        int count = 0; // to track even / odd

        /// <summary>
        /// Runs for each item getting added to the renderer which allows for even / odd css classes to be added
        /// this can also be used to set a selected css class if desired.
        /// </summary>
        /// <param name="Wrapper"></param>
        /// <param name="Item"></param>
        /// <returns></returns>
        protected HtmlGenericControl wRendererForCriteria_InsertItemWrapper(System.Web.UI.HtmlControls.HtmlGenericControl Wrapper, Interfaces.IContent Item)
        {
            Wrapper.Attributes.Add("class", count % 2 == 0 ? "even" : "odd");

            count++;

            return Wrapper;
        }
    }
}