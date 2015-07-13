namespace WSOL.EktronCms.ContentMaker
{
    using Ektron.Cms.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using WSOL.EktronCms.ContentMaker.Extensions;
    using WSOL.EktronCms.ContentMaker.Interfaces;
    using WSOL.IocContainer;

    /// <summary>
    /// Summary description for GlossaryFactory
    /// </summary>
    public class GlossaryFactory : IGlossaryFactory
    {
        protected static ICacheManager CacheManager = InitializationContext.Locator.Get<ICacheManager>();

        public static IEnumerable<IGlossary> GetGlossaries<T>(int pageSize = 1000) where T : IContent, IGlossary
        {
            long xmlId = typeof(T).GetXmlConfigId();

            if (xmlId < 1)
                return null;

            return GetGlossaries(new List<long> { xmlId }, pageSize);
        }

        public static IEnumerable<IGlossary> GetGlossaries(List<long> glossaryConfigTypes = null, int pageSize = 1000)
        {
            List<long> xmlTypes = glossaryConfigTypes ?? _GetAllGlossaryTypes();

            if (xmlTypes == null || xmlTypes.Count == 0)
            {
                return null;
            }

            return CacheManager.CacheItem // HttpContext.Current.GetHttpContextItem<IEnumerable<IGlossary>>
            (
                string.Format("WSOL:Cache:LanguageGlossaries:{0}:{1}", string.Join(",", xmlTypes), pageSize),
                () =>
                {
                    var criteria = xmlTypes.GetContentCriteria();
                    criteria.AddFilter(ContentProperty.XmlConfigurationId, CriteriaFilterOperator.In, xmlTypes);
                    criteria.AddFilter(ContentProperty.LanguageId, CriteriaFilterOperator.GreaterThan, 0); // all languages
                    criteria.OrderByField = ContentProperty.Id;
                    criteria.PagingInfo.RecordsPerPage = pageSize;

                    var items = criteria.GetContent(AdminMode: true);

                    if (items != null)
                    {
                        return items.OfType<IGlossary>().ToList();
                    }

                    return null;
                },
                CacheManager.ShortInterval
            );

        }

        public static string TranslateKey<T>(string key, int languageId, bool returnKeyOnFail = false, IEnumerable<IGlossary> glossaries = null) where T : IContent, IGlossary
        {
            glossaries = glossaries ?? GetGlossaries<T>();

            return TranslateKey(key, languageId, returnKeyOnFail, glossaries);
        }

        public static string TranslateKey(string key, int languageId, bool returnKeyOnFail = false, IEnumerable<IGlossary> glossaries = null)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                if (returnKeyOnFail)
                    return key;

                throw new Exception("Key cannot be null or whitespace");
            }

            glossaries = glossaries ?? GetGlossaries(); // get defaults if none are given

            if (glossaries == null && !glossaries.Any() && returnKeyOnFail)
            {
                return key;
            }

            string resolved = CacheManager.CacheItem
            (
                string.Format("WSOL:Cache:TranslateKey:Key={0}:Language={1},Glossaries={2}", key, languageId, string.Join(",", glossaries.Select(x => string.Concat(x.Id, ";", x.LanguageId)))),
                () =>
                {
                    string value;
                    int defaultLanguage = FrameworkFactory.DefaultLanguage;
                    int fallbackLanguage = FrameworkFactory.FallbackLanguage;

                    foreach (var glossary in glossaries)
                    {
                        // Try requested language
                        if (glossary.GlossaryLanguageId == languageId)
                        {
                            if (glossary.GlossarySet.TryGetValue(key, out value))
                                return value;
                        }
                        else if (glossary.GlossaryLanguageId == fallbackLanguage)
                        {
                            if (glossary.GlossarySet.TryGetValue(key, out value))
                                return value;
                        }
                        // Fallback to language default
                        else if (glossary.GlossaryLanguageId == defaultLanguage)
                        {
                            if (glossary.GlossarySet.TryGetValue(key, out value))
                                return value;
                        }

                    }

                    return null;
                },
                CacheManager.ShortInterval
            );

            if (resolved != null)
            {
                return resolved;
            }
            else if (returnKeyOnFail)
            {
                return key;
            }

            return string.Format("The key: {0} was not found in the dictionary for language {1}", key, languageId);
        }

        #region Helpers

        private static List<long> _GetAllGlossaryTypes()
        {
            return HttpContext.Current.GetApplicationItem<List<long>>
                (
                    "WSOL.AllGlossaryTypes",
                    () =>
                    {
                        var glossaryTypes = (true).ScanForInterface<IGlossary>(true, false);
                        var icontentTypes = (true).ScanForInterface<IContent>(true, false);
                        IEnumerable<Type> merged = icontentTypes.Intersect(glossaryTypes);
                        List<long> xmlTypes = new List<long>();

                        foreach (var type in merged)
                        {
                            xmlTypes.Add(type.GetXmlConfigId());
                        }

                        return xmlTypes;
                    }
                );
        }

        #endregion

        #region Interface

        IEnumerable<IGlossary> IGlossaryFactory.GetGlossaries(List<long> glossaryConfigTypes, int pageSize)
        {
            return GetGlossaries(glossaryConfigTypes, pageSize);
        }

        IEnumerable<IGlossary> IGlossaryFactory.GetGlossaries<T>(int pageSize)
        {
            return GetGlossaries<T>(pageSize);
        }

        string IGlossaryFactory.TranslateKey(string key, int languageId, bool returnKeyOnFail, IEnumerable<IGlossary> glossaries)
        {
            return TranslateKey(key, languageId, returnKeyOnFail, glossaries);
        }

        string IGlossaryFactory.TranslateKey<T>(string key, int languageId, bool returnKeyOnFail, IEnumerable<IGlossary> glossaries)
        {
            return TranslateKey(key, languageId, returnKeyOnFail, glossaries);
        }

        #endregion
    }
}