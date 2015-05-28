namespace WSOL.EktronCms.ContentMaker.Samples
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using WSOL.Custom.ContentMaker.Samples.Models;
    using WSOL.Custom.ContentMaker.Samples.Resolvers;
    using WSOL.EktronCms.ContentMaker.Extensions;
    using WSOL.EktronCms.ContentMaker.Interfaces;
    using WSOL.EktronCms.ContentMaker.Models;
    using WSOL.EktronCms.ContentMaker.Resolvers;
    using WSOL.IocContainer;

    public class CodeSamples
    {
        public void Samples()
        {
            #region Basics

            bool adminMode = false;
            bool siteEnabledLanguages = true;

            // get the dynamic Id of the page looking for pageid, ekfrm, or id
            // note ISiteSetup can be used to set an ID if none of the above query parameters are found, which is good for homepages
            long id = this.GetDynamicContentID();

            // get the dynamic content fully populated
            IContent icontent = this.GetDynamicContent();

            // get current language
            int currentLanguage = this.GetLanguageId();

            // get current user
            var user = this.GetCurrentUserId();

            // get site enabled languages
            var languages = this.GetLanguages(siteEnabledLanguages);

            // any framework can be retrieved this way as to not create a new one each time.
            var folderManager = FrameworkFactory<Ektron.Cms.Framework.Organization.FolderManager>.Get(adminMode, currentLanguage);

            // or by an extension
            var folderManager2 = this.GetFrameworkManager<Ektron.Cms.Framework.Organization.FolderManager>(currentLanguage, adminMode);

            #endregion

            #region IOC Container

            // get a concrete class from the IOC Container
            ISiteSetup setup = InitializationContext.Locator.Get<ISiteSetup>();
            ICacheManager cacheManager = InitializationContext.Locator.Get<ICacheManager>();

            // gets the value defined for the default content
            long defaultID = setup.DefaultContentId;

            #endregion

            #region SmartForms Ids

            // get smart form Id from Model
            long xmlId = typeof(LanguageGlossaryContent).GetXmlConfigId();

            // get type from ID
            Type t = xmlId.GetModelType();

            #endregion

            #region Content Criteria Example

            var criteria = this.GetContentCriteria(true);
            criteria.AddFilter(Ektron.Cms.Common.ContentProperty.LanguageId, Ektron.Cms.Common.CriteriaFilterOperator.GreaterThan, 0);
            criteria.AddFilter(Ektron.Cms.Common.ContentProperty.Id, Ektron.Cms.Common.CriteriaFilterOperator.GreaterThan, 0);
            criteria.ReturnMetadata = true;

            //.GetContent is extension of ContentCriteria no ContentManager is needed, OfType filters to only HtmlContent
            // also notice no caching is written, but it is cached via extension.
            // GetContent Extension works for all types of ContentCriteria.
            var TypedItems = criteria.GetContent(adminMode).OfType<HtmlContent>();

            // can also prefilter by smart form type
            // creates content criteria and sets xml config id filter to match value set in content descriptor of Article content
            var articleCriteria = this.GetContentCriteria<ArticleContent>();
            var articles = articleCriteria.GetContent();

            #endregion

            #region Navigation
            
            IEnumerable<INavigation> allMenus = cacheManager.CacheItem("AllEktronMenus", () => NavigationFactory.GetAll(FrameworkFactory.CurrentLanguage()), cacheManager.QuickInterval);            
            long menuID = 6; // ideally set in site settings objects

            // Resolvers are optional and set properties like selected and visible
            var mainMenu = NavigationFactory.Get(menuID, FrameworkFactory.CurrentLanguage(), cacheManager.DefaultInterval,
                new INavigationResolver[] { new NavigationItemSelectedResolver(), new NavigationItemVisibleResolver() });

            if (mainMenu != null)
            {
                // can bind to a repeater or listview
                // rptNav.DataSource = mainMenu.Items;
                // rptNav.DataBind();
            }

            #endregion
            
            #region Glossary / Translate Key

            // look across all models that implement IGlossary
            var value1 = GlossaryFactory.TranslateKey("key1", FrameworkFactory.CurrentLanguage(), false, null);

            // look for a key value in a specific IGlossary model class
            var value2 = GlossaryFactory.TranslateKey<LanguageGlossaryContent>("key2", FrameworkFactory.CurrentLanguage(), true);

            #endregion

            #region Settings Example

            // the resolver can modify settings object for whatever usecase is needed.
            var siteSettings = SettingsFactory<SiteSettingscontent>.GetSiteSettings(new SiteSettingsResolver(), true); 
            var folderSettings = SettingsFactory<FolderSettingscontent>.GetFolderSettings(icontent, null, new FolderSettingsResolver());

            // Access values set in settings
            var logoImage = siteSettings.Logo;

            // allows for the meta data to be changed at runtime from folder settings data
            var resolved = folderSettings.ResolveMetaData(icontent);

            #endregion
        }
    }
}