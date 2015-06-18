using System;
using WSOL.EktronCms.ContentMaker.Extensions;

namespace WSOL.Custom.ContentMaker.Samples
{
    public partial class DynamicContentTemplate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // if this returns null, the renderer simply doesn't display anything, otherwise since no tags are specific,
            // the renderer looks for templates where default is set to true, then for any other templates where tags are not required
            wRenderer.Item = this.GetDynamicContent(); 
            wRenderer.DataBind();
        }
    }
}