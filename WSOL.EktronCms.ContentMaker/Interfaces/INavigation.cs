namespace WSOL.EktronCms.ContentMaker.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface INavigation
    {
        long Id { get; set; }

        long ParentId { get; set; }

        long LanguageId { get; set; }

        string Name { get; set; }

        string Description { get; set; }

        string Link { get; set; }

        string Target { get; set; }

        bool UseImage { get; set; }

        string ImageLink { get; set; }

        IEnumerable<INavigationItem> Items { get; set; }
    }
}
