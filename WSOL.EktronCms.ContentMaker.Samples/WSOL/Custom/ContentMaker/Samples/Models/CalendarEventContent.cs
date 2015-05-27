namespace WSOL.Custom.ContentMaker.Samples.Models
{
    using System;
    using System.Runtime.Serialization;
    using WSOL.EktronCms.ContentMaker.Attributes;
    using WSOL.EktronCms.ContentMaker.Interfaces;
    using WSOL.EktronCms.ContentMaker.Models;

    /// <summary>
    /// Simple model for out of the box calendar events
    /// </summary>
    [Serializable]
    [ContentDescriptor(XmlConfigId = -120, Description = "Ektron Calendar Event Example")]
    public class CalendarEventContent : ContentType<ContentTypes.CalendarEvent.root>
    {        
        public CalendarEventContent() { }

        public CalendarEventContent(IContent c, string xml) : base(c, xml) { }
        
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        protected CalendarEventContent(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
        }
    }
}