using WSOL.Custom.ContentMaker.Samples.Models;
using WSOL.EktronCms.ContentMaker.Interfaces;

namespace WSOL.Custom.ContentMaker.Samples.Resolvers
{
    public class SiteSettingsResolver : ISettingsResolver<SiteSettingscontent>
    {
        public SiteSettingscontent Resolve(SiteSettingscontent settings)
        {
            // add custom code here to modify settings...

            return settings;
        }
    }
}