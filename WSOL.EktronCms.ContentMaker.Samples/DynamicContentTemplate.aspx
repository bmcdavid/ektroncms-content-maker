<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DynamicContentTemplate.aspx.cs" Inherits="WSOL.Custom.ContentMaker.Samples.DynamicContentTemplate" %>
<%@ Register Namespace="WSOL.ObjectRenderer.WebControls.ObjectRenderer" Assembly="WSOL.ObjectRenderer" TagPrefix="WSOL" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>To demonstrate displaying dynamic content blocks / forms</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <WSOL:ObjectRenderer runat="server" ID="wRenderer" />
        </div>
    </form>
</body>
</html>
