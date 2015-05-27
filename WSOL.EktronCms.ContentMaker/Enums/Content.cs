namespace WSOL.EktronCms.ContentMaker.Enums
{
    using System;
    using System.Xml.Serialization;
    using WSOL.EktronCms.ContentMaker.Attributes;

    public enum ContentStatus
    {
        [String("A")]
        Published,
        [String("I")]
        CheckedIn,
        [String("O")]
        CheckedOut,
        [String("Archived")]
        Archived,
        [String("S")]
        Submitted,
        [String("P")]
        PendingStartDate,
        [String("D")]
        PendingDelete,
        [String("M")]
        MarkedDeleted,
        [String("T")]
        PendingTasks,
        [String("Empty")]
        Empty
    }

    public enum ContentSubtype
    {
        [XmlEnum]
        AllTypes = -1,
        [XmlEnum]
        Content = 0,
        [XmlEnum]
        PageBuilderData = 1,
        [XmlEnum]
        WebEvent = 2,
        [XmlEnum]
        PageBuilderMasterData = 3,
        [XmlEnum]
        CTAData = 4,
        [XmlEnum]
        LandingPageData = 5,
    }

    public enum ContentType
    {
        // Summary:
        //     Any Type
        [XmlEnum]
        AllTypes = -1,
        //
        // Summary:
        //     Value is undefined.
        [XmlEnum]
        Undefined = 0,
        //
        // Summary:
        //     Content Type
        [XmlEnum]
        Content = 1,
        //
        // Summary:
        //     Form Type
        [XmlEnum]
        Forms = 2,
        //
        // Summary:
        //     Archive Content Type
        [XmlEnum]
        Archive_Content = 3,
        //
        // Summary:
        //     Archive Forms Type
        [XmlEnum]
        Archive_Forms = 4,
        //
        // Summary:
        //     Library Item Type
        [XmlEnum]
        LibraryItem = 7,
        //
        // Summary:
        //     Assets Type
        [XmlEnum]
        Assets = 8,
        //
        // Summary:
        //     Archive Assets Type. Note: Archived assets can actually be any value between
        //     1101 and 2099 depending on the actual Asset Type.
        [XmlEnum]
        Archive_Assets = 9,
        //
        // Summary:
        //     Archive Media Type
        [XmlEnum]
        Archive_Media = 12,
        //
        // Summary:
        //     Non Library Content Type
        [XmlEnum]
        NonLibraryContent = 99,
        //
        // Summary:
        //     Multimedia Type
        [XmlEnum]
        Multimedia = 104,
        //
        // Summary:
        //     Discussion Topic Type
        [XmlEnum]
        DiscussionTopic = 1111,
        //
        // Summary:
        //     Catalog Entry Type
        [XmlEnum]
        CatalogEntry = 3333,
    }

    public enum ContentMetaDataType
    {
        [XmlEnum]
        NotSet = -100,
        [XmlEnum]
        HtmlTag = 0,
        [XmlEnum]
        MetaTag = 1,
        [XmlEnum]
        CollectionSelector = 2,
        [XmlEnum]
        ListSummarySelector = 3,
        [XmlEnum]
        ContentSelector = 4,
        [XmlEnum]
        ImageSelector = 5,
        [XmlEnum]
        LinkSelector = 6,
        [XmlEnum]
        FileSelector = 7,
        [XmlEnum]
        MenuSelector = 8,
        [XmlEnum]
        UserSelector = 9,
        [XmlEnum]
        SearchableProperty = 100,
    }

    public enum ContentMetadataDataType
    {
        [XmlEnum]
        Text = 0,
        [XmlEnum]
        Number = 1,
        [XmlEnum]
        Byte = 2,
        [XmlEnum]
        Double = 3,
        [XmlEnum]
        Float = 4,
        [XmlEnum]
        Integer = 5,
        [XmlEnum]
        Long = 6,
        [XmlEnum]
        Short = 7,
        [XmlEnum]
        Date = 8,
        [XmlEnum]
        Boolean = 9,
        [XmlEnum]
        SingleSelect = 10,
        [XmlEnum]
        MultiSelect = 11,
    }

    public enum ArchiveAction 
    { 
        Remove = 1,
        Remain = 2 
    }

    public enum MetaDataAction
    {
        Ignore,
        Append,
        Prepend,
        Replace,
        ReplaceAllowNulls
    }

    public enum ContentListType
    {
        Folder,
        Taxonomy,
        Collection,
        //Metadata,
        Content
    }
}