<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Default.ascx.cs" Inherits="WSOL.Custom.ContentMaker.Samples.Views.NewsRelease.Default" %>

<h1>News Release</h1>
<h3><%: CurrentData.Title %></h3>
<span><%: CurrentData.ArticleDate.ToShortDateString() %></span>
<div class="float-left"><%= CurrentData.ArticleImage %></div>
<div>
    <%= CurrentData.Body %>
</div>
