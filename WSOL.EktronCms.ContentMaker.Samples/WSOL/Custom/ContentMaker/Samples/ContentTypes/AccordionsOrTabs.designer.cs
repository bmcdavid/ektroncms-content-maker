namespace WSOL.Custom.ContentMaker.Samples.ContentTypes.AccordionsOrTabs
{
    using System;
    using System.Diagnostics;
    using System.Xml.Serialization;
    using System.Collections;
    using System.Xml.Schema;
    using System.ComponentModel;


    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class root
    {

        private rich htmlAboveField;

        private rich htmlBelowField;

        private rootStyle styleField;

        private rootItem[] itemField;

        public root()
        {
            this.styleField = rootStyle.Tabs;
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public rich HtmlAbove
        {
            get
            {
                return this.htmlAboveField;
            }
            set
            {
                this.htmlAboveField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public rich HtmlBelow
        {
            get
            {
                return this.htmlBelowField;
            }
            set
            {
                this.htmlBelowField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public rootStyle Style
        {
            get
            {
                return this.styleField;
            }
            set
            {
                this.styleField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute("Item", Order = 3)]
        public rootItem[] Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = true)]
    public partial class rich
    {

        private System.Xml.XmlNode[] anyField;

        [System.Xml.Serialization.XmlTextAttribute()]
        [System.Xml.Serialization.XmlAnyElementAttribute(Order = 0)]
        public System.Xml.XmlNode[] Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public enum rootStyle
    {

        /// <remarks/>
        Tabs,

        /// <remarks/>
        Accordion,
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class rootItem
    {

        private string itemTitleField;

        private string itemTeaserField;

        private rootItemItemContent[] itemContentField;

        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public string ItemTitle
        {
            get
            {
                return this.itemTitleField;
            }
            set
            {
                this.itemTitleField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public string ItemTeaser
        {
            get
            {
                return this.itemTeaserField;
            }
            set
            {
                this.itemTeaserField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute("ItemContent", Order = 2)]
        public rootItemItemContent[] ItemContent
        {
            get
            {
                return this.itemContentField;
            }
            set
            {
                this.itemContentField = value;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class rootItemItemContent
    {

        private rootItemItemContentItemType itemTypeField;

        private rootItemItemContentRichText richTextField;

        private rootItemItemContentContentBlock contentBlockField;

        public rootItemItemContent()
        {
            this.itemTypeField = rootItemItemContentItemType.RichText;
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public rootItemItemContentItemType ItemType
        {
            get
            {
                return this.itemTypeField;
            }
            set
            {
                this.itemTypeField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public rootItemItemContentRichText RichText
        {
            get
            {
                return this.richTextField;
            }
            set
            {
                this.richTextField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public rootItemItemContentContentBlock ContentBlock
        {
            get
            {
                return this.contentBlockField;
            }
            set
            {
                this.contentBlockField = value;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public enum rootItemItemContentItemType
    {

        /// <remarks/>
        RichText,

        /// <remarks/>
        ContentBlock,
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class rootItemItemContentRichText
    {

        private rich hTMLField;

        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public rich HTML
        {
            get
            {
                return this.hTMLField;
            }
            set
            {
                this.hTMLField = value;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class rootItemItemContentContentBlock
    {

        private long contentItemField;

        private bool contentItemFieldSpecified;

        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public long ContentItem
        {
            get
            {
                return this.contentItemField;
            }
            set
            {
                this.contentItemField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ContentItemSpecified
        {
            get
            {
                return this.contentItemFieldSpecified;
            }
            set
            {
                this.contentItemFieldSpecified = value;
            }
        }
    }
}
