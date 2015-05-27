namespace WSOL.EktronCms.ContentMaker.Interfaces
{
    using System.Collections.Generic;

    public interface INavigationResolver
    {
        bool IsEnabled { get; }

        int CacheInterval { get; }

        void Resolve(INavigationItem item);
    }
}
