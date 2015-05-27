namespace WSOL.EktronCms.ContentMaker.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using WSOL.EktronCms.ContentMaker.Interfaces;

    [Serializable]    
    [KnownType(typeof(List<NavigationItem>))]
    public class Navigation : INavigation
    {
        public Navigation()
        {
            Items = new List<NavigationItem>();
        }

        public virtual long Id { get; set; }

        public virtual long ParentId { get; set; }

        public virtual long LanguageId { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public string Link { get; set; }

        public string Target { get; set; }

        public bool UseImage { get; set; }

        public string ImageLink { get; set; }

        public virtual List<NavigationItem> Items { get; set; }

        IEnumerable<INavigationItem> INavigation.Items
        {
            get
            {
                return Items.Cast<INavigationItem>();
            }
            set
            {
                Items = value.Cast<NavigationItem>().ToList();
            }
        }
    }
}