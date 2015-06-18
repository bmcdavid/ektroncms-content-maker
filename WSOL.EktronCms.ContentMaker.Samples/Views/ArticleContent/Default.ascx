<%@ Control Language="C#" AutoEventWireup="false" Inherits="WSOL.Custom.ContentMaker.Samples.Views.ArticleContent.Default" CodeBehind="Default.ascx.cs" %>

<script runat="server">
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        
        this.DataBind(); // sets DataSource for rptCallouts
    }
</script>

<h2><%: CurrentData.Heading %></h2>
<span>Date: <%: CurrentData.Date.ToShortDateString() %> </span>
<div class="image"><%= CurrentData.Image.ToString() %></div>

<% if (CurrentData.Link != null)
   { %>
<div class="link"><%= CurrentData.Link.ToString() %></div>
<% } %>
<div class="article-body">
    <%= CurrentData.Body %>
</div>
<%  if (CurrentData.Callouts.Count > 0)
    { %>
<asp:Repeater ID="rptCallouts" DataSource="<%# CurrentData.Callouts %>" runat="server" ItemType="WSOL.EktronCms.ContentMaker.Samples.Models.ArticleCallout">
    <HeaderTemplate>
        <div class="callouts">
    </HeaderTemplate>
    <FooterTemplate></div></FooterTemplate>
    <ItemTemplate>
        <div class="callout size1of4" data-alignment="<%# Item.Alignment %>">
            <a id="<%# Item.Bookmark %>"></a>
            <h4><%# Item.Heading %></h4>
            <%# Item.Image.ToString() %>
            <div class="caption">
                <%# Item.Caption %>
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>
<% } %>
