namespace WSOL.Custom.ContentMaker
{
    using WSOL.EktronCms.ContentMaker.Interfaces;

    /// <summary>
    /// Summary description for SiteSetup
    /// </summary>
    public class SiteSetup : ISiteSetup
    {
        public long DefaultContentId
        {
            get
            {
                return 0; // map to something like web.config for setting smart form.
            }           
        }

        public long DefaultFolderId
        {
            get
            {
                return 0; // map to something like web.config for setting smart form.
            } 
        }
    }
}