namespace WSOL.EktronCms.ContentMaker
{
    using Ektron.Cms.Common;
    using System.Collections.Generic;
    using System.Linq;
    using WSOL.EktronCms.ContentMaker.Enums;
    using WSOL.EktronCms.ContentMaker.Interfaces;
    using WSOL.EktronCms.ContentMaker.Models;
    using WSOL.IocContainer;
    //using WSOL.Library.Extensions;

    public class ContentMaker : IContentFactory<Ektron.Cms.ContentData>
    {
        public const string GO_LIVE_DATE_FORMAT = "yyyy-MM-dd HH:mm";

        #region Static Methods

        public static IEnumerable<IContent> MakeList(IEnumerable<Ektron.Cms.ContentData> Items)
        {
            if (Items == null)
                yield break;

            foreach (var item in Items)
                yield return MakeItem(item);
        }

        public static IContent MakeItem(Ektron.Cms.ContentData Item)
        {
            if (Item == null)
                return null;

            IContent c = new HtmlContent()
            {
                Id = Item.Id,
                LanguageId = Item.LanguageId,
                FolderId = Item.FolderId,
                UserId = Item.UserId,
                XmlConfigId = Item.XmlConfiguration != null ? Item.XmlConfiguration.Id : 0,
                DateCreated = Item.DateCreated,
                DateModified = Item.DateModified,
                StartDate = Item.GoLive.ToDateTime(GO_LIVE_DATE_FORMAT),
                EndDate = Item.EndDate.ToDateTime(GO_LIVE_DATE_FORMAT),
                Html = Item.Html,
                IsPrivate = Item.IsPrivate,
                Description = Item.Teaser,
                Title = Item.Title,
                Url = Item.Quicklink,
                IsForm = Item.ContType == 2 || Item.ContType == 4,
                Status = ConvertContentStatus(Item),
                ArchiveAction = Item.EndDateAction.ToEnum<ArchiveAction>(ArchiveAction.Remain),
                MetaData = MakeMetaData(Item).ToList(),
                ContentType = Item.ContType.ToEnum<Enums.ContentType>(Enums.ContentType.Content),
                ContentSubType = Item.SubType.ToInt32().ToEnum<Enums.ContentSubtype>(Enums.ContentSubtype.Content)
            };
            
            if (c.XmlConfigId > 0)
            {
                var types = Extensions.CompilationExtensions.GetContentDescriptors();
                var matched = types.Where(x => x.Value.XmlConfigId == c.XmlConfigId);

                if (matched.Any())
                {
                    return matched.First().Key.NewInstance<IContent>(c, c.Html);
                    //return (IContent)matched.First().Value.ContentMaker(c, c.Html);
                    //return (IContent)Activator.CreateInstance(matched.First().Key, new object[] { content });
                }
            }

            return c;
        }

        public static IEnumerable<Ektron.Cms.ContentData> UnMakeList(IEnumerable<IContent> Items)
        {
            if(Items == null)
                yield break;

            foreach (var item in Items)
                yield return UnMakeItem(item);
        }

        public static Ektron.Cms.ContentData UnMakeItem(IContent Item)
        {
            Ektron.Cms.ContentData c = new Ektron.Cms.ContentData()
            {
                Id = Item.Id,
                Title = Item.Title,
                FolderId = Item.FolderId,
                UserId = Item.UserId,
                DateCreated = Item.DateCreated,
                DateModified = Item.DateModified,
                Html = Item.Html,
                Teaser = Item.Description,
                IsPrivate = Item.IsPrivate,
                Quicklink = Item.Url,
                GoLive = Item.StartDate.ToString(GO_LIVE_DATE_FORMAT),
                EndDate = Item.EndDate.ToString(GO_LIVE_DATE_FORMAT),
                EndDateAction = Item.ArchiveAction.ToInt32(),
                XmlConfiguration = new Ektron.Cms.XmlConfigData() { Id = Item.XmlConfigId },
                LanguageId = Item.LanguageId,
                ContType = Item.ContentType.ToInt32(),
                SubType = Item.ContentSubType.ToInt32().ToEnum<EkEnumeration.CMSContentSubtype>(EkEnumeration.CMSContentSubtype.Content),
                MetaData = UnMakeMetaData(Item).ToArray()
            };

            return c;
        }

        private static IEnumerable<MetaData> MakeMetaData(Ektron.Cms.ContentData item)
        {
            if(item == null || item.MetaData == null)
                yield break;

            foreach (var i in item.MetaData)//.Where(x => !String.IsNullOrEmpty(x.Text)))
                yield return new MetaData()
                    {
                        Id = i.TypeId,
                        LanguageId = i.Language,
                        Name = i.TypeName,
                        Value = i.Text,
                        Type = ContentMetaDataType.NotSet
                    };
        }

        private static IEnumerable<Ektron.Cms.ContentMetaData> UnMakeMetaData(IContent item)
        {
            if (item == null || item.MetaData == null)
                yield break;

            foreach (var i in item.MetaData)
                yield return new Ektron.Cms.ContentMetaData()
                {
                    TypeId = i.Id,
                    Language = i.LanguageId,
                    Text = i.Value,
                    TypeName = i.Name
                };
        }

        private static ContentStatus ConvertContentStatus(Ektron.Cms.ContentData content)
        {
            if (content == null)
                return ContentStatus.Empty;

            switch ((EkEnumeration.CMSContentType)content.ContType)
            {
                case EkEnumeration.CMSContentType.Archive_Assets:
                case EkEnumeration.CMSContentType.Archive_Forms:
                case EkEnumeration.CMSContentType.Archive_Content:
                case EkEnumeration.CMSContentType.Archive_Media:
                    return ContentStatus.Archived;
            }

            switch (content.Status)
            {
                case "A":
                    return ContentStatus.Published;
                case "S":
                    return ContentStatus.Submitted;
                case "O":
                    return ContentStatus.CheckedOut;
                case "I":
                    return ContentStatus.CheckedIn;
                case "M":
                    return ContentStatus.MarkedDeleted;
                case "P":
                    return ContentStatus.PendingStartDate;
                case "D":
                    return ContentStatus.PendingDelete;
                case "T":
                    return ContentStatus.PendingTasks;
            }

            return ContentStatus.Empty;
        }    

        #endregion

        #region Excplicit Interface Implementations

        IEnumerable<IContent> IContentFactory<Ektron.Cms.ContentData>.MakeList(IEnumerable<Ektron.Cms.ContentData> Items)
        {
            return MakeList(Items);
        }

        IContent IContentFactory<Ektron.Cms.ContentData>.MakeItem(Ektron.Cms.ContentData Item)
        {
            return MakeItem(Item);
        }

        IEnumerable<Ektron.Cms.ContentData> IContentFactory<Ektron.Cms.ContentData>.UnMakeList(IEnumerable<IContent> Items)
        {
            return UnMakeList(Items);
        }

        Ektron.Cms.ContentData IContentFactory<Ektron.Cms.ContentData>.UnMakeItem(IContent Item)
        {
            return UnMakeItem(Item);
        }

        #endregion        
    }
}