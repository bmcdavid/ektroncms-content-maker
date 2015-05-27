using WSOL.Custom.ContentMaker.Samples.Models;
using WSOL.EktronCms.ContentMaker.Interfaces;

namespace WSOL.Custom.ContentMaker.Samples.Resolvers
{
    public class FolderSettingsResolver : ISettingsResolver<FolderSettingscontent>
    {
        public FolderSettingscontent Resolve(FolderSettingscontent settings)
        {
            // add custom code here to modify settings...

            return settings;
        }
    }
}