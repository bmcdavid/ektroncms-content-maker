namespace WSOL.Custom.ContentMaker
{
    using DryIoc;
    using WSOL.EktronCms.ContentMaker.Interfaces;
    using WSOL.IocContainer;

    public class DependencyResolver : IConfigureContainer
    {
        public void ConfigureContainer(IRegistry registry)
        {
            #region Example of how to scan assembly to register types

            //var assembly = typeof(DependencyResolver).Assembly; // scan current assembly

            //foreach (var i in assembly.GetTypes().Where(x => x.GetInterfaces().Contains(typeof(IDoSomething))))
            //{
            //    container.Register(typeof(IDoSomething), i);
            //}

            #endregion            

            registry.Register<IErrorPageModuleSetup, ErrorPageModuleSetup>(Reuse.Singleton);
            registry.Register<ISiteSetup, SiteSetup>(Reuse.Singleton);
        }
    }
}