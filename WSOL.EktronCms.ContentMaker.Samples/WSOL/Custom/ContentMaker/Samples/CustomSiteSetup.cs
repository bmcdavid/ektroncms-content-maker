using DryIoc;
using WSOL.Custom.ContentMaker.Samples.Models;
using WSOL.EktronCms.ContentMaker.Interfaces;
using WSOL.IocContainer;

namespace WSOL.EktronCms.ContentMaker.Samples
{
    /// <summary>
    /// Custom Site Setup to change defaults to use values stored in site settings
    /// using a custom class avoids conflicts with nuget package updates.
    /// The "InitializationDependency" attribute waits until all given types have registered before 
    /// configure container runs for this instance.
    /// </summary>
    [InitializationDependency(typeof(WSOL.Custom.ContentMaker.DependencyResolver))]
    public class CustomSiteSetupInitalize : IConfigureContainer
    {
        public void ConfigureContainer(DryIoc.IRegistry registry)
        {
            registry.Register<ISiteSetup, CustomSiteSetup>(Reuse.Singleton);
        }
    }

    public class CustomSiteSetup : ISiteSetup
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
    }
}