namespace WSOL.Custom.ContentMaker
{
    using System.Collections.Generic;
    using System.Linq;
    using WSOL.EktronCms.ContentMaker;
    using WSOL.EktronCms.ContentMaker.Extensions;
    using WSOL.EktronCms.ContentMaker.Interfaces;

    public class ErrorPageModuleSetup : IErrorPageModuleSetup
    {
        private static IEnumerable<string> reservedPages = new string[] { "default.aspx", "404.aspx", "500.aspx" };

        private static IEnumerable<string> enabledPages = new string[] { "content.aspx" };

        public IContent GetIContent(System.Web.HttpContext context)
        {
            return context.GetDynamicContent();
        }

        public bool ExecuteErrorPage(System.Web.HttpContext context, System.Web.Configuration.CustomErrorsSection customErrorsSection)
        {
            string script = context.Request["SCRIPT_NAME"].ToLower().Trim('/');

            if (reservedPages.Contains(script))
                return false;

            if (enabledPages.Contains(script))
            {
                var id = FrameworkFactory.CurrentUserId();

                if (id == 0)
                {
                    return true;
                }

                var userData = id.GetUserData(true);

                // Disable for CMS Users
                if (userData == null || userData.IsMemberShip)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
