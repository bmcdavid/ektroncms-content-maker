namespace WSOL.EktronCms.ContentMaker.Delegates
{
    using WSOL.EktronCms.ContentMaker.Interfaces;

    public delegate IContent ContentMaker(IContent content, string XmlData = null);
}