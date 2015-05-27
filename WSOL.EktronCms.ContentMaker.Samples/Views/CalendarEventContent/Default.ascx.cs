namespace WSOL.Custom.ContentMaker.Samples.Views.CalendarEventContent
{
    using System;
    using WSOL.EktronCms.ContentMaker.Attributes;
    using WSOL.EktronCms.ContentMaker.WebControls;

    [TemplateDescriptor(Default = true, Path = "~/Views/CalendarEventContent/Default.ascx")]
    public partial class Default : ControlBase<WSOL.Custom.ContentMaker.Samples.Models.CalendarEventContent>
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected DateTimeOffset GetTimeOffset(DateTime d, string timezone)
        {
            TimeZoneInfo t = TimeZoneInfo.Local;

            foreach (TimeZoneInfo tz in TimeZoneInfo.GetSystemTimeZones())
            {
                if (tz.DisplayName.Contains(timezone))
                {
                    t = tz;
                    break;
                }
            }

            var utcOffset = new DateTimeOffset(d, TimeSpan.Zero);
            return utcOffset.ToOffset(t.GetUtcOffset(utcOffset));
        }
    }
}