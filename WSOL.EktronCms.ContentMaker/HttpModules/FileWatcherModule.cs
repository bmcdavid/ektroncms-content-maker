namespace WSOL.EktronCms.ContentMaker.HttpModules
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Web;
    using System.Web.Compilation;
    using WSOL.IocContainer;

    /// <summary>
    /// This module builds ASCX files in the ~/Views folder to be used by the WSOL:Renderer control.
    /// </summary>
    public class FileWatcherModule : IApplicationModule
    {
        private const string _AppKeyEnableFileWatcher = "WSOL.EktronCms.ContentMaker.EnableFileWatcher";
        private const string _AppKeyViewsPath = "WSOL.EktronCms.ContentMaker.ViewsPath";
        private const string _AppKeyWebApp= "WSOL.EktronCms.ContentMaker.Compiled";

        static IApplicationHelper _ApplicationHelper;

        /// <summary>
        /// Allows watch path to be changed from default of ~/Views
        /// </summary>
        internal static readonly string FolderPath = string.IsNullOrEmpty(ConfigurationManager.AppSettings[_AppKeyViewsPath]) ? "~/Views" : ConfigurationManager.AppSettings[_AppKeyViewsPath];

        internal static readonly bool IsWebsite = string.IsNullOrEmpty(ConfigurationManager.AppSettings[_AppKeyWebApp]) ? true : !bool.Parse(ConfigurationManager.AppSettings[_AppKeyWebApp]);

        /// <summary>
        /// Enables FileSystemWatcher
        /// </summary>
        internal static readonly bool EnableViewWatcher = string.IsNullOrEmpty(ConfigurationManager.AppSettings[_AppKeyEnableFileWatcher]) ? false : bool.Parse(ConfigurationManager.AppSettings[_AppKeyEnableFileWatcher]);

        internal static System.IO.FileSystemWatcher ViewWatcher = null;

        public void Dispose()
        {
            if (ViewWatcher != null)
                ViewWatcher.Dispose();
        }

        public void Init(HttpApplication context)
        {
            _ApplicationHelper = InitializationContext.Locator.Get<IApplicationHelper>();

            string path = _ApplicationHelper.MapPath(FolderPath);

            if (ViewWatcher == null && EnableViewWatcher)
            {
                ViewWatcher = new FileSystemWatcher(path);
                ViewWatcher.IncludeSubdirectories = true;
                ViewWatcher.Filter = "*.ascx";
                ViewWatcher.NotifyFilter =
                    NotifyFilters.LastWrite |
                    NotifyFilters.FileName |
                    NotifyFilters.DirectoryName;

                ViewWatcher.Changed += ViewWatcher_CUD;
                ViewWatcher.Deleted += ViewWatcher_CUD;
                ViewWatcher.Created += ViewWatcher_CUD;
                ViewWatcher.Renamed += ViewWatcher_Renamed;
            }

            BuildFiles(path, false);
        }

        private static void BuildFiles(string path, bool delete)
        {
            FileAttributes attr = File.GetAttributes(path);

            if(ViewWatcher != null)
                ViewWatcher.EnableRaisingEvents = false; // Keeps from firing twice

            if (!delete && IsWebsite)
            {
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    var files = System.IO.Directory.GetFiles(path, "*.ascx", SearchOption.AllDirectories);

                    foreach (var file in files)
                    {
                        BuildManager.GetCompiledType(_ApplicationHelper.MapPathReverse(file));
                    }
                }
                else
                {
                    if (path.ToLower().EndsWith(".ascx") || path.ToLower().EndsWith(".cs"))
                    {
                        string file = _ApplicationHelper.MapPathReverse(path.Replace(".cs", string.Empty));
                        BuildManager.GetCompiledType(file);
                    }
                }
            }

            // Rebuild the views after compiling them
            Extensions.TemplateExtensions.BuildDescriptorList(true);

            if (ViewWatcher != null)
                ViewWatcher.EnableRaisingEvents = true; // re-enable watcher after its completed
        }

        private void ViewWatcher_CUD(object sender, System.IO.FileSystemEventArgs e)
        {
            BuildFiles(e.FullPath, e.ChangeType == WatcherChangeTypes.Deleted);
        }

        private void ViewWatcher_Renamed(object sender, System.IO.RenamedEventArgs e)
        {
            BuildFiles(e.FullPath, false);
        }
    }
}