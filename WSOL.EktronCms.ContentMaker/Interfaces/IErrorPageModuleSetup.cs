namespace WSOL.EktronCms.ContentMaker.Interfaces
{
    using System.Web;
    using System.Web.Configuration;

    public interface IErrorPageModuleSetup
    {
        IContent GetIContent(HttpContext context);

        bool ExecuteErrorPage(HttpContext context, CustomErrorsSection customErrorsSection);
    }
}
