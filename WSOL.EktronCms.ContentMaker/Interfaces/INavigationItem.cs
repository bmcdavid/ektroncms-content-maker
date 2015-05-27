namespace WSOL.EktronCms.ContentMaker.Interfaces
{
    using System.Collections.Generic;
    using WSOL.EktronCms.ContentMaker.Enums;

    public interface INavigationItem
    {
        long Id { get; set; }

        long ParentId { get; set; }

        string Text { get; set; }

        string Description { get; set; }

        string Link { get; set; }

        string Target { get; set; }

        bool UseImage { get; set; }

        string ImageLink { get; set; }

        bool Selected { get; set; }

        bool ChildSelected { get; set; }

        bool HasItems { get; }

        bool Visible { get; set; }

        NavigationItemType Type { get; set; }

        int Level { get; set; }

        IEnumerable<INavigationItem> Items { get; set; }

        IEnumerable<long> AssociatedFolders { get; set; }

        IEnumerable<string> AssociatedTemplates { get; set; }
    }
}
