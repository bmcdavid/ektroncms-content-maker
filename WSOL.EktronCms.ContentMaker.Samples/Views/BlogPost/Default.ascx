<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Default.ascx.cs" Inherits="WSOL.Custom.ContentMaker.Samples.Views.BlogPost.Default" %>

<script runat="server">
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        this.DataBind(); // sets the Item for wAuthor
    }
</script>

<h1><%: CurrentData.Title %></h1>
<span><%: CurrentData.ArticleDate.ToShortDateString() %></span>
<WSOL:Renderer ID="wAuthor" runat="server" TagsString="AuthorCallout" Item="<%# CurrentData.Author %>" />
<div class="float-left"><%= CurrentData.ArticleImage %></div>
<div>
    <%= CurrentData.Body %>
</div>
