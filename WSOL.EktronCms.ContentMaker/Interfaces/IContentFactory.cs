namespace WSOL.EktronCms.ContentMaker.Interfaces
{
    using System.Collections.Generic;

    public interface IContentFactory<TContent>
    {
        IEnumerable<IContent> MakeList(IEnumerable<TContent> Items);
        IContent MakeItem(TContent Item);
        IEnumerable<TContent> UnMakeList(IEnumerable<IContent> Items);
        TContent UnMakeItem(IContent Item);
    }
}
