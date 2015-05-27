namespace WSOL.Custom.ContentMaker.Samples.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using WSOL.EktronCms.ContentMaker.Attributes;
    using WSOL.EktronCms.ContentMaker.Extensions;
    using WSOL.EktronCms.ContentMaker.Interfaces;
    using WSOL.EktronCms.ContentMaker.Models;
    using WSOL.IocContainer;

    [Serializable]
    [ContentDescriptor(XmlConfigId = -130, Description = "Accordions or Tabs Example")]
    public class AccordionsOrTabs : ContentType<ContentTypes.AccordionsOrTabs.root>
    {
        /// <summary>
        /// Used to get content instances from content selector references
        /// </summary>
        private int _CurrentLanguage;

        #region Constructors

        public AccordionsOrTabs()
        {
            _CurrentLanguage = this.GetLanguageId();
        }

        public AccordionsOrTabs(IContent c, string xml)
            : base(c, xml)
        {
            _CurrentLanguage = this.GetLanguageId();
        }

        #endregion Constructors

        #region Serialization Helpers

        protected AccordionsOrTabs(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _CurrentLanguage = this.GetLanguageId();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        #endregion Serialization Helpers

        #region Properties

        public string HtmlAbove
        {
            get
            {
                return SmartFormData.HtmlAbove.Any.ToRichString();
            }
            set
            {
                SmartFormData.HtmlAbove.Any = value.FromRichString();
            }
        }

        public string HtmlBelow
        {
            get
            {
                return SmartFormData.HtmlBelow.Any.ToRichString();
            }
            set
            {
                SmartFormData.HtmlBelow.Any = value.FromRichString();
            }
        }

        public bool IsAccordion
        {
            get
            {
                return SmartFormData.Style == ContentTypes.AccordionsOrTabs.rootStyle.Accordion;
            }
            set
            {
                SmartFormData.Style = value ? ContentTypes.AccordionsOrTabs.rootStyle.Accordion : ContentTypes.AccordionsOrTabs.rootStyle.Tabs;
            }
        }

        private List<AccordionOrTabItem> _Items;

        public List<AccordionOrTabItem> Items
        {
            get
            {
                if (_Items != null)
                    return _Items;

                if (_Items == null)
                    _Items = new List<AccordionOrTabItem>();

                if (SmartFormData.Item == null)
                    SmartFormData.Item = new ContentTypes.AccordionsOrTabs.rootItem[] { };

                foreach (var i in SmartFormData.Item)
                    _Items.Add(MakeItem(i));

                return _Items;
            }
            set
            {
                _Items = value;

                if (_Items == null)
                    return;

                List<ContentTypes.AccordionsOrTabs.rootItem> items = new List<ContentTypes.AccordionsOrTabs.rootItem>();

                foreach (var i in _Items)
                    items.Add(UnMakeItem(i));

                SmartFormData.Item = items.ToArray();
            }
        }

        #endregion

        #region Makers and UnMakers

        private AccordionOrTabItem MakeItem(ContentTypes.AccordionsOrTabs.rootItem item)
        {
            AccordionOrTabItem i = new AccordionOrTabItem();

            if (item == null)
                return i;

            i.Teaser = item.ItemTeaser;
            i.Title = item.ItemTitle;
            var list = new List<HtmlContent>();

            foreach (var x in item.ItemContent)
            {
                if (x.ItemType == ContentTypes.AccordionsOrTabs.rootItemItemContentItemType.RichText)
                    list.Add(new HtmlContent() { Html = x.RichText.HTML.Any.ToRichString(), Id = int.MinValue });
                else if (x.ContentBlock.ContentItemSpecified)
                    list.Add(x.ContentBlock.ContentItem.GetContent(_CurrentLanguage) as HtmlContent);
            }

            i.Items = list;

            return i;
        }

        private ContentTypes.AccordionsOrTabs.rootItem UnMakeItem(AccordionOrTabItem item)
        {
            ContentTypes.AccordionsOrTabs.rootItem i = new ContentTypes.AccordionsOrTabs.rootItem();

            if (item == null)
                return i;

            i.ItemTitle = item.Title;
            i.ItemTeaser = item.Teaser;
            var list = new List<ContentTypes.AccordionsOrTabs.rootItemItemContent>();

            ContentTypes.AccordionsOrTabs.rootItemItemContent newItem;

            foreach (var x in item.Items)
            {
                newItem = new ContentTypes.AccordionsOrTabs.rootItemItemContent();

                if (x.Id == int.MinValue)
                {
                    newItem.ItemType = ContentTypes.AccordionsOrTabs.rootItemItemContentItemType.RichText;
                    newItem.RichText = new ContentTypes.AccordionsOrTabs.rootItemItemContentRichText();
                    newItem.RichText.HTML.Any = x.Html.FromRichString();
                }
                else
                {
                    newItem.ItemType = ContentTypes.AccordionsOrTabs.rootItemItemContentItemType.ContentBlock;
                    newItem.ContentBlock = new ContentTypes.AccordionsOrTabs.rootItemItemContentContentBlock() { ContentItem = x.Id, ContentItemSpecified = true };
                }

                list.Add(newItem);
            }

            i.ItemContent = list.ToArray();

            return i;
        }

        #endregion Makers and UnMakers

    }

    /// <summary>
    /// Simple Model for Tab or Accordion Content
    /// </summary>
    [Serializable]
    public class AccordionOrTabItem : ISerializable
    {
        public AccordionOrTabItem() { Items = new List<HtmlContent>(); }

        public string Title { get; set; }
        public string Teaser { get; set; }
        public List<HtmlContent> Items { get; set; }

        public AccordionOrTabItem(SerializationInfo info, StreamingContext context)
        {
            Title = info.GetString("Title");
            Teaser = info.GetString("Teaser").FromWrapCDATA();
            Items = info.GetValue("Html", typeof(object)) as List<HtmlContent>;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Title", Title, typeof(string));
            info.AddValue("Teaser", Teaser.ToCDATA(), typeof(string));
            info.AddValue("Html", Items, typeof(IEnumerable<IContent>));
        }
    }
}