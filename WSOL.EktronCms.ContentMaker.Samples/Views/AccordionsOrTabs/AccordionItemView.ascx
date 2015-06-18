<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="AccordionItemView.ascx.cs" Inherits="WSOL.EktronCms.ContentMaker.Samples.Views.AccordionsOrTabs.AccordionView" %>

<script runat="server">
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        this.DataBind();
    }
</script>

<div class="item clear">
    <h3><%= CurrentData.Title %></h3>
    <asp:Panel ID="Panel1" runat="server" Visible="<%# !string.IsNullOrEmpty(CurrentData.Teaser) %>" CssClass="info">
        <%= CurrentData.Teaser %>
    </asp:Panel>
    <div class="more">
        <WSOL:ObjectRenderer runat="server" ItemList="<%# CurrentData.Items %>" />
    </div>
</div>
