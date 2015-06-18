<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="ContentRenderSamples.aspx.cs" Inherits="WSOL.EktronCms.ContentMaker.Samples.ContentRenderSamples" %>
<%@ Register Namespace="WSOL.ObjectRenderer.WebControls.ObjectRenderer" Assembly="WSOL.ObjectRenderer" TagPrefix="WSOL" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Content Renderer Samples</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h1>Samples of the WSOL Content Renderer</h1>
        <p>
            This is to demonstrate how to use the renderer web control, which utilizes user controls defined in the ~/Views directory with the template descriptor attribute.

            Note: Templates are found on the web site startup and since most Ektron sites run as a web site, any new templates or changes to the template descriptor attribute
             requires a restart of the site, which can easily be done by adding a white space to the web.config.
        </p>
        <h3>Dynamic Item</h3>
        <%--Note: ISiteSetup can be used to set a default content ID if nothing is present in the querystring for dynamic content. --%>
        <WSOL:ObjectRenderer ID="wRendererForDynamicItem" runat="server" />

        <h3>Article List using ListView Tag</h3>
        <%--Note: DebugMode set to true lists each items content ID, title, and view path to assist in troubleshooting, this should always be false in production/release mode.--%>
        <WSOL:ObjectRenderer ID="wRendererForArticles" runat="server" WrapTag="ul" ItemWrapTag="li" TagsString="ListView" DebugMode="true" />

        <h3>Mixed content model ist as UL listing with even / odd classes with ListView tag</h3>
        <%--Note: this listing will have content for both accordions and articles, articles will have additional element for date thanks to its custom ListView --%>
        <%-- accordions will use the htmlcontent listview as no custom view with a "ListView" tag was made for that model --%>
        <WSOL:ObjectRenderer ID="wRendererForMixed" runat="server" WrapTag="ul" ItemWrapTag="li" TagsString="ListView" OnInsertItemWrapper="wRendererForCriteria_InsertItemWrapper" />
    </div>
    </form>
</body>
</html>
