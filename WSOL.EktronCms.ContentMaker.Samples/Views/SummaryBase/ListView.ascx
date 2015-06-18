<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ListView.ascx.cs" Inherits="WSOL.Custom.ContentMaker.Samples.Views.SummaryBase.ListView" %>

<%--Shared Display for listings of Blog Posts and News Releases--%>
<h3><a href=<%= CurrentData.Url %>><%: CurrentData.Title %></a></h3>
<span class="date"><%: CurrentData.ArticleDate.ToShortDateString() %></span>
<p>
    <%= CurrentData.ArticleDescription %>
</p>