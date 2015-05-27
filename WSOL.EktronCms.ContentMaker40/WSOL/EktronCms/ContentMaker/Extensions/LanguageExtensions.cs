namespace WSOL.EktronCms.ContentMaker.Extensions
{
    using Ektron.Cms.Common;
    using Ektron.Cms.Content;
    using Ektron.Cms.Framework.Content;
    using Ektron.Cms.Localization;
    using System.Collections.Generic;
    using System.Linq;
    using WSOL.EktronCms.ContentMaker.Enums;
    using WSOL.EktronCms.ContentMaker.Models;
    using WSOL.IocContainer;

    public static class LanguageExtensions
    {
        private static ICacheManager _CacheManager = InitializationContext.Locator.Get<ICacheManager>();

        public static IEnumerable<LocaleData> GetLanguages(this object o, bool siteEnabled = true, bool enableCache = true)
        {
            return _CacheManager.CacheItem<IEnumerable<LocaleData>>
            (
                string.Format("WSOL:Cache:Languages:SiteEnabled={0}", siteEnabled),
                () =>
                {
                    var api = FrameworkFactory.Get<Ektron.Cms.Framework.Settings.LocaleManager>(true);

                    var items = api.GetEnabledLocales();

                    if (items != null)
                    {
                        return items.Where(x => x.SiteEnabled == siteEnabled);
                    }

                    return null;
                },
                enableCache ? _CacheManager.ShortInterval : 0
            );

        }

        public static IEnumerable<MetaData> GetLangaugeMetadata(this int languageId, bool enableCache = true)
        {
            var items = _CacheManager.CacheItem<IEnumerable<MetaData>>
                (
                    string.Format("WSOL:Cache:MetaTypes:L={0}", languageId),
                    () =>
                    {
                        var meta = FrameworkFactory<MetadataTypeManager>.Get(true, languageId);
                        var criteria = new MetadataTypeCriteria();
                        List<MetaData> metaItems = new List<MetaData>();
                        criteria.AddFilter(MetadataTypeProperty.Id, CriteriaFilterOperator.GreaterThan, 0);
                        criteria.AddFilter(MetadataTypeProperty.Language, CriteriaFilterOperator.EqualTo, languageId);

                        var list = meta.GetList(criteria);

                        if (list != null && list.Any())
                        {
                            foreach (var i in list)
                            {
                                metaItems.Add(new MetaData()
                                {
                                    Id = i.Id,
                                    LanguageId = i.Language,
                                    Name = i.Name,
                                    Type = i.Type.ToInt32().ToEnum<ContentMetaDataType>(ContentMetaDataType.SearchableProperty)
                                }
                                );
                            }
                        }

                        return metaItems;
                    },
                    enableCache ? _CacheManager.ShortInterval : 0
                );

            return items;
        }

        public static int GetLanguageId(this object o, LanguageType languageType = LanguageType.CurrentLanguage)
        {
            switch (languageType)
            {
                case LanguageType.CurrentLanguage:
                    return FrameworkFactory.CurrentLanguage();
                case LanguageType.FallbackLangauge:
                    return FrameworkFactory.FallbackLanguage;
                case LanguageType.DefaultLanguage:
                default:
                    return FrameworkFactory.DefaultLanguage;
            }
        }
    }
}