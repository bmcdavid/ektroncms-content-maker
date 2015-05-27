namespace WSOL.EktronCms.ContentMaker.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using WSOL.EktronCms.ContentMaker.Interfaces;

    [Serializable]
    [KnownType(typeof(Enums.NavigationItemType))]
    [KnownType(typeof(List<NavigationItem>))]
    public class NavigationItem : INavigationItem
    {
        public NavigationItem()
        {
            Items = new List<NavigationItem>();
            AssociatedFolders = new List<long>();
            AssociatedTemplates = new List<string>();
            Type = Enums.NavigationItemType.Link;
        }

        public virtual long Id { get; set; }

        public virtual long ParentId { get; set; }

        public virtual string Text { get; set; }

        public virtual string Description { get; set; }

        public virtual string Link { get; set; }

        public virtual string Target { get; set; }

        public virtual bool UseImage { get; set; }

        public virtual string ImageLink { get; set; }

        public virtual bool Selected { get; set; }

        public virtual bool ChildSelected { get; set; }

        public virtual bool HasItems
        { 
            get
            { 
                return (Items == null || Items.Count == 0) ? false : true; 
            } 
        }

        public virtual List<NavigationItem> Items { get; set; }

        public List<long> AssociatedFolders { get; set; }

        public List<string> AssociatedTemplates { get; set; }

        public virtual bool Visible { get; set; }

        public virtual Enums.NavigationItemType Type { get; set; }

        public int Level { get; set; }

        IEnumerable<INavigationItem> INavigationItem.Items
        {
            get { return Items.Cast<INavigationItem>(); }
            set { Items = value.Cast<NavigationItem>().ToList(); }
        }

        IEnumerable<long> INavigationItem.AssociatedFolders
        {
            get { return AssociatedFolders.Cast<long>(); }
            set { AssociatedFolders = value.ToList(); }
        }

        IEnumerable<string> INavigationItem.AssociatedTemplates
        {
            get { return AssociatedTemplates.Cast<string>(); }
            set { AssociatedTemplates = value.ToList(); }
        }
    }
}