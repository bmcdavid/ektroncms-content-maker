<%@ Control Language="C#" AutoEventWireup="false" Inherits="WSOL.Custom.ContentMaker.Samples.Views.ArticleContent.ListView" Codebehind="ListView.ascx.cs" %>

<a href="<%= CurrentData.Url  %>"><%= CurrentData.Title %></a> (<%= CurrentData.Date.ToShortDateString() %>)