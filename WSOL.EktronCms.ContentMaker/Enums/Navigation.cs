namespace WSOL.EktronCms.ContentMaker.Enums
{
    using System.Xml.Serialization;

    public enum NavigationItemType
    {
        [XmlEnum]
        Content = 1,
        [XmlEnum]
        Library = 2,
        [XmlEnum]
        Navigation = 3,
        [XmlEnum]
        SubNavigation = 4,
        [XmlEnum]
        Link = 5,
    }
}