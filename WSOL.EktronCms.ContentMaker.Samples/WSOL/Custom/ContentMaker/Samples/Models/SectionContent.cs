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
    [ContentDescriptor(XmlConfigId = -110, Description = "Section Content Example")]
    public class SectionContent : ContentType<ContentTypes.Section.root>
    {
        /// <summary>
        /// Used to get content instances from content selector references
        /// </summary>
        private int CurrentLanguage;

        #region Constructors

        public SectionContent()
        {
            CurrentLanguage = this.GetLanguageId();   
        }

        public SectionContent(IContent c, string xml)
            : base(c, xml)
        {
            CurrentLanguage = this.GetLanguageId();
        }

        #endregion Constructors

        #region Serialization Helpers

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        protected SectionContent(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
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

        private List<SectionItem> _Sections;

        public List<SectionItem> Sections
        {
            get
            {
                if (_Sections != null)
                    return _Sections;

                if (_Sections == null)
                    _Sections = new List<SectionItem>();

                if (SmartFormData.Section == null)
                    SmartFormData.Section = new ContentTypes.Section.rootSection[] { };

                foreach (var i in SmartFormData.Section)
                    _Sections.Add(MakeItem(i));

                return _Sections;
            }
            set
            {
                _Sections = value;

                if (_Sections == null)
                    return;

                List<ContentTypes.Section.rootSection> items = new List<ContentTypes.Section.rootSection>();

                foreach (var i in _Sections)
                    items.Add(UnMakeItem(i));

                SmartFormData.Section = items.ToArray();
            }
        }

        private SectionItem MakeItem(ContentTypes.Section.rootSection item)
        {
            SectionItem i = new SectionItem();

            if (item == null)
                return i;

            i.Title = item.Title;
            i.CssClass = item.CssClass;
            i.InnerWrap = item.InnerWrap;
            i.Link = (item.Link != null && item.Link.a != null) ?
                new HtmlHyperlink() { Target = item.Link.a.target, Href = item.Link.a.href, Text = item.Link.a.Any.ToRichString() } :
                new HtmlHyperlink();

            var list = new List<UnitItem>();

            foreach (var x in item.Unit)
            {
                UnitItem unit = new UnitItem()
                {
                    CssClass = x.CssClass,
                    UnitPadding = x.UnitPadding,
                    Size = x.Size
                };

                if (x.ContentItem != null)
                {
                    foreach (var u in x.ContentItem)
                        unit.Items.Add(MakeUnitItem(u));
                }

                list.Add(unit);
            }

            i.Units = list;

            return i;
        }

        #endregion Properties

        #region Makers and UnMakers

        private ContentTypes.Section.rootSection UnMakeItem(SectionItem item)
        {
            ContentTypes.Section.rootSection i = new ContentTypes.Section.rootSection();

            if (item == null)
                return i;

            //var list = new List<ContentTypes.AccordionsOrTabs.rootItemItemContent>();

            //ContentTypes.AccordionsOrTabs.rootItemItemContent newItem;

            //foreach (var x in item.Items)
            //{
            //    newItem = new ContentTypes.AccordionsOrTabs.rootItemItemContent();

            //    if (x.Id == int.MinValue)
            //    {
            //        newItem.ItemType = ContentTypes.AccordionsOrTabs.rootItemItemContentItemType.RichText;
            //        newItem.RichText = new ContentTypes.AccordionsOrTabs.rootItemItemContentRichText();
            //        newItem.RichText.HTML.Any = x.Html.GetXmlNodes();
            //    }
            //    else
            //    {
            //        newItem.ItemType = ContentTypes.AccordionsOrTabs.rootItemItemContentItemType.ContentBlock;
            //        newItem.ContentBlock = new ContentTypes.AccordionsOrTabs.rootItemItemContentContentBlock() { ContentItem = x.Id, ContentItemSpecified = true };
            //    }

            //    list.Add(newItem);
            //}

            //i.ItemContent = list.ToArray();

            return i;
        }

        private HtmlContent MakeUnitItem(ContentTypes.Section.rootSectionUnitContentItem item)
        {
            if (item.Type == ContentTypes.Section.rootSectionUnitContentItemType.ContentBlock && item.ContentBlock.ContentIDSpecified)
                return item.ContentBlock.ContentID.GetContent(CurrentLanguage) as HtmlContent;

            return (new HtmlContent() { Html = item.RichText.HTML.Any.ToRichString(), Id = int.MinValue });
        }

        #endregion

    }

    /// <summary>
    /// Builds a row
    /// </summary>
    [Serializable]
    public class SectionItem
    {
        public SectionItem()
        {
            Units = new List<UnitItem>();
        }

        public List<UnitItem> Units { get; set; }
        public string Title { get; set; }
        public string CssClass { get; set; }
        public HtmlHyperlink Link { get; set; }
        public bool InnerWrap { get; set; }
    }

    /// <summary>
    /// Builds a column
    /// </summary>
    [Serializable]
    public class UnitItem
    {
        public UnitItem()
        {
            Items = new List<HtmlContent>();
        }

        public int Size { get; set; }
        public bool UnitPadding { get; set; }
        public string CssClass { get; set; }
        public List<HtmlContent> Items { get; set; }
    }
}