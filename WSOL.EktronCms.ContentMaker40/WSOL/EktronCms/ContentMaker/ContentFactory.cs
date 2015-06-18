namespace WSOL.EktronCms.ContentMaker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using WSOL.EktronCms.ContentMaker.Attributes;
    using WSOL.EktronCms.ContentMaker.Enums;
    using WSOL.EktronCms.ContentMaker.Extensions;
    using WSOL.EktronCms.ContentMaker.Interfaces;
    using WSOL.EktronCms.ContentMaker.Models;
    using WSOL.IocContainer;

    public class ContentFactory : IContentFactory<Ektron.Cms.ContentData>
    {
        #region Static Methods

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
                StartDate = Item.GoLiveDate.HasValue ? (DateTime)Item.GoLiveDate : DateTime.MinValue,
                EndDate = Item.ExpireDate,
                Html = Item.Html,
                IsPrivate = Item.IsPrivate,
                Description = Item.Teaser,
                Title = Item.Title,
                Url = Item.Quicklink,
                IsForm = Item.ContType == 2 || Item.ContType == 4,
                Status = ConvertContentStatus(Item),
                ArchiveAction = Item.EndDateActionType == Ektron.Cms.Common.EkEnumeration.CMSEndDateAction.Archive_Expire ? ArchiveAction.Remove : ArchiveAction.Remain,
                MetaData = MakeMetaData(Item).ToList(),
                ContentType = Item.ContType.ToEnum<Enums.ContentType>(Enums.ContentType.Content),
                ContentSubType = Item.SubType.ToInt32().ToEnum<Enums.ContentSubtype>(Enums.ContentSubtype.Content)
            };

            if (c.XmlConfigId > 0)
            {
                var types = Extensions.CompilationExtensions.GetContentDescriptors();
                var matched = types.Where(x => x.Value.XmlConfigId == c.XmlConfigId).FirstOrDefault();

                if (!matched.Equals(default(KeyValuePair<Type, ContentDescriptorAttribute>)))
                {
                    return matched.Key.NewInstance<IContent>(c, c.Html);
                    //return (IContent)matched.First().Value.ContentMaker(c, c.Html);
                    //return (IContent)Activator.CreateInstance(matched.First().Key, new object[] { content });
                }
            }

            return c;
        }

        public static IEnumerable<IContent> MakeList(IEnumerable<Ektron.Cms.ContentData> Items)
        {
            if (Items == null)
                yield break;

            foreach (var item in Items)
                yield return MakeItem(item);
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
                GoLiveDate = Item.StartDate as DateTime?,
                ExpireDate = Item.EndDate,
                XmlConfiguration = new Ektron.Cms.XmlConfigData() { Id = Item.XmlConfigId },
                Status = Item.Status != ContentStatus.Archived && Item.Status != ContentStatus.Empty ? Item.Status.GetStringValue() : "A",
                EndDateActionType = Item.ArchiveAction == ArchiveAction.Remove ? Ektron.Cms.Common.EkEnumeration.CMSEndDateAction.Archive_Expire : Ektron.Cms.Common.EkEnumeration.CMSEndDateAction.Archive_Display,
                LanguageId = Item.LanguageId,
                ContType = Item.ContentType.ToInt32(),
                SubType = Item.ContentSubType.ToInt32().ToEnum<Ektron.Cms.Common.EkEnumeration.CMSContentSubtype>(Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.Content),
                MetaData = UnMakeMetaData(Item).ToArray()
            };

            if (c.MetaData != null && c.MetaData.Length == 0)
                c.MetaData = null;

            return c;
        }

        public static IEnumerable<Ektron.Cms.ContentData> UnMakeList(IEnumerable<IContent> Items)
        {
            if (Items == null)
                yield break;

            foreach (var item in Items)
                yield return UnMakeItem(item);
        }

        private static ContentStatus ConvertContentStatus(Ektron.Cms.ContentData content)
        {
            if (content == null)
                return ContentStatus.Empty;

            switch ((Ektron.Cms.Common.EkEnumeration.CMSContentType)content.ContType)
            {
                case Ektron.Cms.Common.EkEnumeration.CMSContentType.Archive_Assets:
                case Ektron.Cms.Common.EkEnumeration.CMSContentType.Archive_Forms:
                case Ektron.Cms.Common.EkEnumeration.CMSContentType.Archive_Content:
                case Ektron.Cms.Common.EkEnumeration.CMSContentType.Archive_Media:
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

        private static IEnumerable<MetaData> MakeMetaData(Ektron.Cms.ContentData item)
        {
            if (item == null || item.MetaData == null)
                yield break;

            foreach (var i in item.MetaData)//.Where(x => !String.IsNullOrEmpty(x.Text)))
                yield return new MetaData()
                {
                    Id = i.Id,
                    LanguageId = i.Language,
                    Name = i.Name,
                    Value = i.Text,
                    Type = i.Type.ToInt32().ToEnum<Enums.ContentMetaDataType>(Enums.ContentMetaDataType.NotSet),
                    DataType = i.DataType.ToInt32().ToEnum<Enums.ContentMetadataDataType>(ContentMetadataDataType.Text)
                };
        }

        private static IEnumerable<Ektron.Cms.ContentMetaData> UnMakeMetaData(IContent item)
        {
            if (item == null || item.MetaData == null)
                yield break;

            foreach (var i in item.MetaData)
                yield return new Ektron.Cms.ContentMetaData()
                {
                    Id = i.Id,
                    Language = i.LanguageId,
                    Text = i.Value,
                    Name = i.Name,
                    Type = i.Type.ToInt32().ToEnum<Ektron.Cms.Common.EkEnumeration.ContentMetadataType>(Ektron.Cms.Common.EkEnumeration.ContentMetadataType.SearchableProperty)
                };
        }
        #endregion

        #region Excplicit Interface Implementations

        IContent IContentFactory<Ektron.Cms.ContentData>.MakeItem(Ektron.Cms.ContentData Item)
        {
            return MakeItem(Item);
        }

        IEnumerable<IContent> IContentFactory<Ektron.Cms.ContentData>.MakeList(IEnumerable<Ektron.Cms.ContentData> Items)
        {
            return MakeList(Items);
        }

        Ektron.Cms.ContentData IContentFactory<Ektron.Cms.ContentData>.UnMakeItem(IContent Item)
        {
            return UnMakeItem(Item);
        }

        IEnumerable<Ektron.Cms.ContentData> IContentFactory<Ektron.Cms.ContentData>.UnMakeList(IEnumerable<IContent> Items)
        {
            return UnMakeList(Items);
        }

        #endregion
    }
}
