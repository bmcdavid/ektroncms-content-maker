<%@ Control Language="C#" AutoEventWireup="true" Inherits="WSOL.Custom.ContentMaker.Samples.Views.AccordionsOrTabs.Default" Codebehind="Default.ascx.cs" %>

<% if (CurrentData != null)
   { %>

<div class="content"><%= CurrentData.HtmlAbove %></div>

<asp:PlaceHolder ID="phAccordion" runat="server" Visible="false">
    <div class="accordion bd">
        <asp:Repeater ID="rptAccordionItems" runat="server" DataSource="<%# CurrentData.Items %>" ItemType="WSOL.EktronCms.ContentMaker.Samples.Models.AccordionOrTabItem">
            <ItemTemplate>
                <div class="item clear">
                    <h3><%# Item.Title %></h3>
                    <asp:Panel runat="server" Visible="<%# !string.IsNullOrEmpty(Item.Teaser) %>" CssClass="info">
                        <%# Item.Teaser %>
                    </asp:Panel>
                    <div class="more">
                        <WSOL:Renderer runat="server" ItemList="<%# Item.Items %>" />
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="phTabs" runat="server" Visible="false">
    <div class="clear">
        <div class="tabSwitcher">
            <div class="tabs">
                <!-- tabs -->
                <ul class="tabNavigation hlist clear narrow-hidden">
                    <asp:Repeater ID="rptTabNames" runat="server" DataItemTypeName="WSOL.EktronCms.ContentMaker.Samples.Models.AccordionOrTabItem">
                        <ItemTemplate>
                            <li>
                                <a href="#tab<%# Container.ItemIndex %>"><%# Item.Title %></a>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
                <!-- tab containers -->
                <div class="tabContainer clear">
                    <asp:Repeater ID="rptTabItems" runat="server" ItemType="WSOL.EktronCms.ContentMaker.Samples.Models.AccordionOrTabItem">
                        <ItemTemplate>
                            <div id="tab<%# Container.ItemIndex %>">
                                <h3 class="multi-hidden"><%# Item.Title %></h3>
                                <WSOL:Renderer runat="server" ItemList="<%# Item.Items %>" />
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </div>
</asp:PlaceHolder>

<div><%= CurrentData.HtmlBelow %></div>

<% } %>