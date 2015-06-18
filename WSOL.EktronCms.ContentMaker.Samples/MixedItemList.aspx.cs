using System;
using WSOL.EktronCms.ContentMaker.Extensions;

namespace WSOL.Custom.ContentMaker.Samples
{
    public partial class MixedItemList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // make short work of this using extensions
            var criteria = this.GetContentCriteria<Models.SummaryBase>(); // this will get us a content criteria with XmlConfigID filters for BlogPost and NewRelease set
            criteria.AddFilter(Ektron.Cms.Common.ContentProperty.Id, Ektron.Cms.Common.CriteriaFilterOperator.GreaterThan, 30); // apply additional filters

            rArticleList.ItemList = criteria.GetContent(); // extensions calls underlying code to create a content manager and fills our models with data
            rArticleList.DataBind();
        }

        private int count = 0;

        protected System.Web.UI.HtmlControls.HtmlGenericControl ArticleList_InsertItemWrapper(System.Web.UI.HtmlControls.HtmlGenericControl Wrapper, WSOL.EktronCms.ContentMaker.Interfaces.IContent Item)
        {
            Wrapper.Attributes.Add("class", count % 2 == 0 ? "even" : "odd");
            count++;

            return Wrapper;            
        }
    }
}