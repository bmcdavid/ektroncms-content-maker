namespace WSOL.EktronCms.ContentMaker
{
    using Ektron.Cms.Framework;
    using System;
    using System.Web;
    using Api = Ektron.Cms.Framework;
    using WSOL.IocContainer;

    public class FrameworkFactory
    {
        private static IApplicationHelper _ApplicationHelper = InitializationContext.Locator.Get<IApplicationHelper>();

        public static int DefaultLanguage
        {
            get
            {
                return HttpContext.Current.GetApplicationItem<int>
                (
                    "Ektron.DefaultLanguage",
                    () => { return Convert.ToInt32(_ApplicationHelper.GetAppConfigValue("ek_DefaultContentLanguage")); }
                );
            }

        }

        public static int CurrentLanguage(bool refresh = false)
        {
            return HttpContext.Current.GetHttpContextItem<int>
            (
                "Ektron.CurrentLanguage",
                () => { return new Api.Core.Content.Content().ContentLanguage; },
                refresh
            );
        }

        public static long CurrentUserId(bool refresh = false)
        {
            return HttpContext.Current.GetHttpContextItem<long>
            (
                "Ektron.CurrentUserId",
                () => { return new Api.Core.Content.Content().UserId; },
                refresh
            );
        }

        public static string SitePath
        {
            get { return _ApplicationHelper.GetAppConfigValue("ek_sitePath"); }
        }

        public static string WorkareaPath
        {
            get { return string.Concat(SitePath, _ApplicationHelper.GetAppConfigValue("ek_appPath")); }
        }

        public static string XmlfilesPath
        {
            get
            {
                return string.Concat(SitePath, _ApplicationHelper.GetAppConfigValue("ek_xmlPath").TrimEnd('/'), "/");
            }
        }

        public static string WidgetPath
        {
            get
            {
                return string.Concat(SitePath, _ApplicationHelper.GetAppConfigValue("ek_widgetPath").TrimEnd('/'), "/");
            }
        }

        public static T Get<T>(bool Admin, int LanguageId) where T : new()
        {
            string key = string.Concat(typeof(T).FullName, LanguageId);

            if (Admin)
            {
                return HttpContext.Current.GetApplicationItem<T>
                (
                    key,
                    () =>
                    {
                        T apiIntance = Activator.CreateInstance<T>();

                        var api = apiIntance as CmsApi<T>;

                        if (api != null)
                        {
                            api.ApiMode = ApiAccessMode.Admin;
                            api.ContentLanguage = LanguageId;
                        }

                        return apiIntance;
                    }
                );
            }

            return HttpContext.Current.GetHttpContextItem<T>
            (
                key,
                () =>
                {
                    T apiIntance = Activator.CreateInstance<T>();

                    var api = apiIntance as CmsApi<T>;

                    if (api != null)
                    {
                        api.ApiMode = ApiAccessMode.LoggedInUser;
                        api.ContentLanguage = LanguageId;
                    }

                    return apiIntance;
                }
            );
        }
    }

    /// <summary>
    /// Summary description for FrameworkFactory
    /// </summary>
    public class FrameworkFactory<T> : FrameworkFactory where T : new()
    {
        public static T Get(bool Admin, int LanguageId)
        {
            return FrameworkFactory.Get<T>(Admin, LanguageId);
        }
    }
}