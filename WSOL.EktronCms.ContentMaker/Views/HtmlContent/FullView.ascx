<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="FullView.ascx.cs" Inherits="WSOL.EktronCms.ContentMaker.Views.HtmlContent.FullView" %>

<script runat="server">

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        this.DataBind();
    }
    
</script>

<div class="content-data">
    <asp:PlaceHolder runat="server" Visible="<%# !CurrentData.IsForm && CurrentData.ContentSubType == WSOL.EktronCms.ContentMaker.Enums.ContentSubtype.Content %>">
        <%= CurrentData.Html %>
    </asp:PlaceHolder>
    <ektron:FormControl DynamicParameter="ekfrm" runat="server" Visible="<%# CurrentData.IsForm %>" />
</div>