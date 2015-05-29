namespace WSOL.EktronCms.ContentMaker.Interfaces
{
    using System.Web;
    using System.Web.Configuration;

    public interface IErrorPageModuleSetup
    {
        bool EnableModule { get; }

        IContent GetIContent(HttpContext context);

        bool ExecuteErrorPage(HttpContext context, CustomErrorsSection customErrorsSection);
    }
}
