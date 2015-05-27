namespace WSOL.EktronCms.ContentMaker.Interfaces
{
    using WSOL.EktronCms.ContentMaker.Enums;

    public interface IMetaData
    {
        long Id { get; set; }
        int LanguageId { get; set; }
        string Name { get; set; }
        string Value { get; set; }
        ContentMetaDataType Type { get; set; }
    }
}
