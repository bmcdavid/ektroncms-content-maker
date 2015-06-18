<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="MixedItemList.aspx.cs" Inherits="WSOL.Custom.ContentMaker.Samples.MixedItemList" %>
<%@ Register Namespace="WSOL.ObjectRenderer.WebControls.ObjectRenderer" Assembly="WSOL.ObjectRenderer" TagPrefix="WSOL" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Mixed List Example</title>
</head>
<body>
    <form id="form1" runat="server">
        <%--This will display a mixed ul/li listing of blog posts and news releases using the "ListView" template tag--%>
        <%--The WSOL:Renderer is the rendering engine for content, which provides similar functionality to EPiServer tags--%>
        <WSOL:ObjectRenderer runat="server" ID="rArticleList" TagsString="ListView" WrapTag="ul" ItemWrapTag="li" OnInsertItemWrapper="ArticleList_InsertItemWrapper" />
    </form>
</body>
</html>
