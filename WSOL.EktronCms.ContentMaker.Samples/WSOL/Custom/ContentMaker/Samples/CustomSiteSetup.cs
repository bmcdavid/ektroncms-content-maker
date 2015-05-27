using DryIoc;
using WSOL.Custom.ContentMaker.Samples.Models;
using WSOL.EktronCms.ContentMaker.Interfaces;
using WSOL.IocContainer;

namespace WSOL.EktronCms.ContentMaker.Samples
{
    /// <summary>
    /// Custom Site Setup to change defaults to use values stored in site settings
    /// using a custom class avoids conflicts with nuget package updates
    /// </summary>
    [InitializationDependency(typeof(WSOL.Custom.ContentMaker.DependencyResolver))]
    public class CustomSiteSetup : ISiteSetup, IConfigureContainer
    {
        private SiteSettingscontent _SiteSettings;

        public CustomSiteSetup()
        {
            _SiteSettings = SettingsFactory<SiteSettingscontent>.GetSiteSettings();
        }

        public long DefaultContentId
        {
            get { return _SiteSettings.DefaultItemId; }
        }

        public long DefaultFolderId
        {
            get { return _SiteSettings.DefaultFolderId; }
        }

        public void ConfigureContainer(DryIoc.IRegistry registry)
        {
            registry.Register<ISiteSetup, CustomSiteSetup>(Reuse.Singleton);
        }
    }
}