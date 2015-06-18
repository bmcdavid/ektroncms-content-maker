<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="Default.ascx.cs" Inherits="WSOL.Custom.ContentMaker.Samples.Views.BlogAuthor.Default" %>

<script runat="server">
    protected Ektron.Cms.Content.ContentCriteria Criteria;

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // make short work of getting blog posts and news releases using extensions
        Criteria = this.GetContentCriteria<ContentMakerBlogSamples.Models.SummaryBase>(); // this will get us a content criteria with XmlConfigID filters for BlogPost and NewRelease set
        Criteria.AddFilter(Ektron.Cms.Common.ContentProperty.Id, Ektron.Cms.Common.CriteriaFilterOperator.GreaterThan, 30); // apply additional filters

        this.DataBind(); // binds server controls with dynamic data
    }
</script>

<h1><%: CurrentData.FullName %></h1>
<div class="float-left"><%= CurrentData.Image %></div>
<div>
    <%= CurrentData.Bio %>
</div>
<h4><%: CurrentData.FirstName %>'s Posts</h4>
<WSOL:ObjectRenderer ID="wRenderer" runat="server" TagsString="ListView" ItemList="<%# CurrentData.BlogPosts %>" />
<hr />
<h4>More news....</h4>
<WSOL:ObjectRenderer runat="server" ID="rArticleList" TagsString="ListView" WrapTag="ul" CssClass="vlist" ItemWrapTag="li"
    ItemList="<%# Criteria.GetContent<ContentMakerBlogSamples.Models.SummaryBase>() %>" />
