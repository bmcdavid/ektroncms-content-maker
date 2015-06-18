<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="AuthorCallout.ascx.cs" Inherits="WSOL.Custom.ContentMaker.Samples.Views.BlogAuthor.AuthorCallout" %>

<h5><a href=<%= CurrentData.Url %>><%= CurrentData.FullName %></a></h5>
<p>
    <%= CurrentData.AuthorDescription %>
</p>