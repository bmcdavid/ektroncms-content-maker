namespace WSOL.EktronCms.ContentMaker.HttpModules
{
    using System;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.Hosting;
    using WSOL.EktronCms.ContentMaker.Interfaces;
    using WSOL.EktronCms.ContentMaker.Models;
    using WSOL.IocContainer;

    public class ErrorPageModule : IApplicationModule
    {
        static IErrorPageModuleSetup _ErrorPageHelper;
        static IApplicationHelper _ApplicationHelper;
        static CustomErrorsSection _CustomErrorsSection = _ApplicationHelper.GetWebConfigSection("system.web/customErrors") as CustomErrorsSection;        

        public void Dispose()
        {
            return;
        }

        public void Init(HttpApplication context)
        {
            try
            {
                _ErrorPageHelper = InitializationContext.Locator.Get<IErrorPageModuleSetup>();
                _ApplicationHelper = InitializationContext.Locator.Get<IApplicationHelper>();


                context.AuthorizeRequest += (object sender, EventArgs e) => context_AuthorizeRequest(context);
            }
            catch (Exception e)
            {
                e.Log(typeof(ErrorPageModule).FullName, System.Diagnostics.EventLogEntryType.Error, context);
            }
        }

        static void context_AuthorizeRequest(HttpApplication application)
        {
            if (application.Context.Request["SCRIPT_NAME"] == "/")
                return;

            StatusCodeCheck(application.Context);

            if (_ErrorPageHelper != null && _ErrorPageHelper.ExecuteErrorPage(application.Context, _CustomErrorsSection))
            {
                // get content for Dynamic Id
                IContent content = _ErrorPageHelper.GetIContent(application.Context);

                // if we don't have a match assume empty
                if (content == null)
                {
                    content = new HtmlContent()
                    {
                        ArchiveAction = Enums.ArchiveAction.Remove,
                        Status = Enums.ContentStatus.Archived,
                        EndDate = DateTime.MinValue
                    };
                }

                if (content.Status == Enums.ContentStatus.Archived && content.ArchiveAction == Enums.ArchiveAction.Remove)
                {
                    if (content.EndDate < DateTime.Now)
                    {
                        string errorPage = _ApplicationHelper.GetCustomErrorPage(404);
                        string currentPage = application.Context.Request.ServerVariables["SCRIPT_NAME"].ToLower();

                        if (!string.IsNullOrEmpty(errorPage) && errorPage.ToLower() != currentPage)
                        {
                            _ApplicationHelper.Redirect(errorPage + "?aspxerrorpage=" + currentPage);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets correct status code and status description for aliased content that are error pages
        /// </summary>
        private static void StatusCodeCheck(HttpContext context)
        {
            int StatusCode = 0;

            if (_CustomErrorsSection != null)
            {
                string rawUrl = context.Request.RawUrl.ToLower();
                string scriptName = context.Request.ServerVariables["SCRIPT_NAME"].ToLower();

                foreach (CustomError item in _CustomErrorsSection.Errors)
                {
                    string redirect = item.Redirect.ToLower();

                    // Resolve AppPath
                    if (redirect.StartsWith("~"))
                    {
                        redirect = _ApplicationHelper.MapPathReverse(HostingEnvironment.MapPath(redirect));
                    }

                    if (rawUrl.Contains(redirect) || scriptName.Contains(redirect))
                    {
                        StatusCode = item.StatusCode;
                        break;
                    }
                }

                if (StatusCode == 0 && !string.IsNullOrEmpty(_CustomErrorsSection.DefaultRedirect))
                {
                    string defaultRedirect = _CustomErrorsSection.DefaultRedirect.ToLower();

                    // Resolve AppPath
                    if (defaultRedirect.StartsWith("~"))
                    {
                        defaultRedirect = _ApplicationHelper.MapPathReverse(_ApplicationHelper.MapPath(defaultRedirect));
                    }

                    if (rawUrl.Contains(defaultRedirect) || scriptName.Contains(defaultRedirect))
                    {
                        StatusCode = 404;
                    }
                }

                if (StatusCode > 0 && StatusCode != 200)
                {
                    context.Response.TrySkipIisCustomErrors = true;
                    context.Response.StatusCode = StatusCode;
                    context.Response.StatusDescription = (((System.Net.HttpStatusCode)StatusCode).ToString()).ToFriendlyName();
                }
            }
        }
    }
}