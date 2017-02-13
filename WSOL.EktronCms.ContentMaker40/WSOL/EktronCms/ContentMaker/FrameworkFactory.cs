namespace WSOL.EktronCms.ContentMaker
{
    using Ektron.Cms.Framework;
    using System;
    using System.Configuration;
    using System.Web;
    using WSOL.IocContainer;
    using Api = Ektron.Cms.Framework;    

    public enum FrameworkModeType
    {
        //WCF - this would be used if you want the Framework APIs to communicate using Web Services to a secondary server.
        WCF,
        //BusinessObjects - this the default value and should be used for most systems.
        BusinessObjects,
        //Cache - this would be used if you want the Framework APIs to utilize the caching layer.  
        Cache,
        //FullView - this is the default container that utilizes only Search Cache with a BusinessObjects fallback.
        Default
    }

    public class FrameworkFactory
    {
        private static ICacheManager _CacheManager = InitializationContext.Locator.Get<ICacheManager>();

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
                () => { return new Api.Settings.LocaleManager().ContentLanguage; }
            );
        }

        public static int FallbackLanguage
        {
            get
            {
                string key = string.Format("WSOL:Cache:FallbackLanguage:ID={0}", CurrentLanguage());

                return _CacheManager.CacheItem<int>
                (
                    key,
                    () =>
                    {
                        var api = Get<Api.Settings.LocaleManager>();
                        var item = api.GetItem(CurrentLanguage());

                        if (item != null)
                        {
                            return item.FallbackId;
                        }

                        return DefaultLanguage;
                    },
                    _CacheManager.ShortInterval
                );
            }
        }

        public static long CurrentUserId(bool refresh = false)
        {
            return HttpContext.Current.GetHttpContextItem<long>
            (
                "Ektron.CurrentUserId",
                () => { return new Api.Settings.LocaleManager().UserId; },
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

        protected static FrameworkModeType FrameworkMode
        {
            get
            {
                Ektron.Cms.FrameworkConfigurationSection config = ConfigurationManager.GetSection("ektron.framework.services/framework") as Ektron.Cms.FrameworkConfigurationSection;

                if (config != null && (config.ChildContainer == "WCF" || config.DefaultContainer == "WCF"))
                {
                    return FrameworkModeType.WCF;
                }

                return FrameworkModeType.BusinessObjects;
            }
        }

        protected static string AccessToken
        {
            get
            {
                return _CacheManager.CacheItem<string>
                (
                    "WSOL.CurrentApiAccessToken",
                    () =>
                    {
                        if (FrameworkMode == FrameworkModeType.BusinessObjects)
                            return string.Empty;

                        if (CurrentUserId() > 0 && !IsLoggedIn)
                        {
                            string
                                userName = ConfigurationManager.AppSettings["WSOL.EktronCms.ContentMaker.Username"],
                                password = ConfigurationManager.AppSettings["WSOL.EktronCms.ContentMaker.Password"],
                                domain = ConfigurationManager.AppSettings["WSOL.EktronCms.ContentMaker.Domain"];

                            var userManager = new Ektron.Cms.Framework.User.UserManager();

                            return userManager.Authenticate(userName, password, domain);
                        }

                        return string.Empty;
                    },
                    1
                );
            }
        }

        public static bool IsLoggedIn
        {
            get
            {
                string key = "WSOL:CurrentlyLoggedIn:" + CurrentUserId();

                return _CacheManager.CacheItem<bool>
                (
                    key,
                    () =>
                    {
                        if (CurrentUserId() > 0)
                        {
                            try
                            {
                                var userManager = new Ektron.Cms.Framework.User.UserManager();
                                var userDetails = userManager.GetItem(userManager.UserId, false, false);

                                if (userDetails != null)
                                    return (userDetails.LoginIdentification == userManager.RequestInformation.UniqueId.ToString());
                            }
                            catch { }
                        }

                        return false;
                    },
                    1
                );
            }
        }

        public static T Get<T>(bool Admin = false, int LanguageId = 0) where T : new()
        {
            LanguageId = LanguageId == 0 ? CurrentLanguage() : LanguageId;
            string accessToken = FrameworkFactory.AccessToken;
            string key = string.Concat(typeof(T).FullName, LanguageId, accessToken, Admin);

            if (Admin && FrameworkMode == FrameworkModeType.BusinessObjects)
            {
                return HttpContext.Current.GetHttpContextItem<T>
                (
                    key,
                    () =>
                    {
                        Type t = typeof(T);
                        T apiIntance = t.NewInstance<T>();// Activator.CreateInstance<T>();

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
                    Type t = typeof(T);
                    T apiIntance = t.NewInstance<T>();// Activator.CreateInstance<T>();

                    var api = apiIntance as CmsApi<T>;

                    if (api != null)
                    {
                        api.ApiMode = ApiAccessMode.LoggedInUser;
                        api.ContentLanguage = LanguageId;
                        api.RequestInformation.AuthenticationToken = Admin ? accessToken : string.Empty;
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
        public static T Get(bool Admin = false, int LanguageId = 0)
        {
            return FrameworkFactory.Get<T>(Admin, LanguageId);
        }
    }
}