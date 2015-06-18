<%@ Control Language="C#" AutoEventWireup="true" Inherits="WSOL.Custom.ContentMaker.Samples.Views.AccordionsOrTabs.Default" Codebehind="Default.ascx.cs" %>

<script runat="server">
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        phAccordion.Visible = CurrentData.IsAccordion;
        phTabs.Visible = !phAccordion.Visible;
        
        this.DataBind();
    }
</script>

<div class="content"><%= CurrentData.HtmlAbove %></div>

<asp:PlaceHolder ID="phAccordion" runat="server" Visible="false">
    <WSOL:ObjectRenderer runat="server" ItemList="<%# CurrentData.Items %>" WrapTag="div" CssClass="accordion hd" TagsString="AccordionItem" />    
</asp:PlaceHolder>

<asp:PlaceHolder ID="phTabs" runat="server" Visible="false">
    <div class="clear">
        <div class="tabSwitcher">
            <div class="tabs">
                <!-- tabs -->
                <ul class="tabNavigation hlist clear narrow-hidden">
                    <asp:Repeater ID="rptTabNames" runat="server" ItemType="WSOL.Custom.ContentMaker.Samples.Models.AccordionOrTabItem">
                        <ItemTemplate>
                            <li>
                                <a href="#tab<%# Container.ItemIndex %>"><%# Item.Title %></a>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
                <!-- tab containers -->
                <div class="tabContainer clear">
                    <asp:Repeater ID="rptTabItems" runat="server" ItemType="WSOL.Custom.ContentMaker.Samples.Models.AccordionOrTabItem">
                        <ItemTemplate>
                            <div id="tab<%# Container.ItemIndex %>">
                                <h3 class="multi-hidden"><%# Item.Title %></h3>
                                <WSOL:ObjectRenderer runat="server" ItemList="<%# Item.Items %>" />
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </div>
</asp:PlaceHolder>

<div><%= CurrentData.HtmlBelow %></div>