namespace WSOL.EktronCms.ContentMaker
{
    using DryIoc;
    using System;
    using WSOL.EktronCms.ContentMaker.Interfaces;
    using WSOL.EktronCms.ContentMaker.Models;
    using WSOL.IocContainer;

    public class ContentMakerConfigureContainer : IConfigureContainer
    {
        public void ConfigureContainer(DryIoc.IRegistry registry)
        {
            registry.Register<IContent, HtmlContent>(Reuse.Transient, (Type t) => typeof(HtmlContent).GetConstructor(Type.EmptyTypes));
        }
    }
}