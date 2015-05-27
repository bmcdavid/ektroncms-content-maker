<%@ Control Language="C#" AutoEventWireup="true" Inherits="WSOL.Custom.ContentMaker.Samples.Views.SectionContent.Default" Codebehind="Default.ascx.cs" %>

<% if (CurrentData != null)
   { %>

<div class="content"><%= CurrentData.HtmlAbove %></div>

<asp:Repeater ID="rptSections" runat="server" DataSource="<%# CurrentData.Sections %>" ItemType="WSOL.EktronCms.ContentMaker.Samples.Models.SectionItem">
    <ItemTemplate>
        <div class="section <%# Item.CssClass %>">
            <asp:PlaceHolder runat="server" Visible="<%# Item.InnerWrap %>">
                <div class="inner">
            </asp:PlaceHolder>
            <h3 runat="server" visible="<%# !String.IsNullOrEmpty(Item.Title) %>">
                <%# Item.Title %>
                <%# Item.Link != null ? Item.Link.ToString() : string.Empty %>
            </h3>
            <asp:Repeater ID="rptUnits" runat="server" DataSource="<%# Item.Units %>" ItemType="WSOL.EktronCms.ContentMaker.Samples.Models.UnitItem" Visible="<%# SetSectionSize(Item.Units)  %>">
                <ItemTemplate>
                    <div class="unit 
                        <%# UnitSizeClass(Item.Size)  %> 
                        <%# Container.ItemIndex == ((IList)((Repeater)Container.Parent).DataSource).Count-1 ? " last-unit" : string.Empty%>
                    ">
                        <asp:PlaceHolder runat="server" Visible="<%# Item.UnitPadding %>"><div class="cb"></asp:PlaceHolder>
                        <WSOL:Renderer runat="server" ItemList="<%# Item.Items %>" Tags='<%# new string[] { } %>' />
                        <asp:PlaceHolder runat="server" Visible="<%# Item.UnitPadding %>"></div></asp:PlaceHolder>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <asp:PlaceHolder runat="server" Visible="<%# Item.InnerWrap %>"></div></asp:PlaceHolder>
        </div>
        
    </ItemTemplate>
</asp:Repeater>

<div><%= CurrentData.HtmlBelow %></div>

<% } %>