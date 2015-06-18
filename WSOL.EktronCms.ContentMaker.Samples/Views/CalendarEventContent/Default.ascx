<%@ Control Language="C#" AutoEventWireup="false" CodeFile="Default.ascx.cs" Inherits="WSOL.Custom.ContentMaker.Samples.Views.CalendarEventContent.Default" %>

<h2><%: CurrentData.SmartFormData.DisplayTitle %></h2>
<div class="date-time">
    <span>Date:</span>
    <%: CurrentData.SmartFormData.StartTime.GetTimeOffset(CurrentData.SmartFormData.OriginalTimeZone).ToString("f")%> -
        <%: CurrentData.SmartFormData.EndTime.GetTimeOffset(CurrentData.SmartFormData.OriginalTimeZone).ToString("f")%> -
</div>
<div class="location">
    <span>Location:</span>
    <%: CurrentData.SmartFormData.Location %>
</div>
<div><%= CurrentData.SmartFormData.Description %></div>
