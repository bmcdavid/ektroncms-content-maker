namespace WSOL.EktronCms.ContentMaker.Models
{
    using WSOL.EktronCms.ContentMaker.Enums;
    using WSOL.EktronCms.ContentMaker.Interfaces;

    /// <summary>
    /// IContent MetaData
    /// </summary>
    public class MetaData : IMetaData
    {
        public long Id { get; set; }

        public int LanguageId { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public ContentMetaDataType Type { get; set; }

        public ContentMetadataDataType DataType { get; set; }

    }
}