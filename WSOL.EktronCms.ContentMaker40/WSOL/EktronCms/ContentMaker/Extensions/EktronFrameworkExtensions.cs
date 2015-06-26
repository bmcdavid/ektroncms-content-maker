namespace WSOL.EktronCms.ContentMaker.Extensions
{
    using Ektron.Cms;
    using Ektron.Cms.Common;
    using Ektron.Cms.Content;
    using Ektron.Cms.Framework.Content;
    using Ektron.Cms.Organization;
    using Ektron.Cms.User;
    using System;
    using System.Linq;
    using WSOL.EktronCms.ContentMaker.Interfaces;
    using WSOL.IocContainer;

    public static class EktronFrameworkExtensions
    {
        public static ContentCriteria GetContentCriteria(this object o, bool returnExpired = false)
        {
            return GetContentCriteriaExtended<ContentCriteria>(o, returnExpired);
        }

        public static ContentCriteria GetContentCriteria<TModel>(this object o, bool returnExpired = false) where TModel : IContent
        {
            return GetContentCriteria<TModel, ContentCriteria>(o, returnExpired);
        }

        public static TCriteria GetContentCriteria<TModel, TCriteria>(this object o, bool returnExpired = false)
            where TModel : IContent
            where TCriteria : ContentCriteria
        {
            TCriteria criteria = GetContentCriteriaExtended<TCriteria>(o, returnExpired);
            Type t = typeof(TModel);

            if (t.IsAbstract || t.IsInterface) // many types
            {
                var types = t.IsInterface ? t.ScanForInterface() : t.ScanForBaseClass();

                if (types != null && types.Any())
                {
                    // add xml config ids for all types that derive from interface or abstract model
                    criteria.AddFilter(ContentProperty.XmlConfigurationId, CriteriaFilterOperator.In, types.Select(x => x.GetXmlConfigId()).ToList());
                }
            }
            else // single concrete model
            {
                // set xml config id from IContent Model
                criteria.AddFilter(ContentProperty.XmlConfigurationId, CriteriaFilterOperator.EqualTo, t.GetXmlConfigId());
            }

            return criteria as TCriteria;
        }

        public static T GetContentCriteriaExtended<T>(this object o, bool returnExpired = false) where T : ContentCriteria
        {
            T criteria = typeof(T).NewInstance<T>();
            criteria.ReturnMetadata = true;

            if (returnExpired)
            {
                criteria.AddFilter(ContentProperty.IsArchived, CriteriaFilterOperator.GreaterThan, -1);
            }

            return criteria as T;
        }

        public static ContentManager GetContentManager(this ContentCriteria o, int Language, bool AdminMode = false)
        {
            return FrameworkFactory<ContentManager>.Get(AdminMode, Language);
        }

        public static FolderCriteria GetFolderCriteria(this object o)
        {
            return new FolderCriteria();
        }

        public static TApi GetFrameworkManager<TApi>(this object o, int Langauge, bool AdminMode = false) where TApi : new()
        {
            return FrameworkFactory<TApi>.Get(AdminMode, Langauge);
        }

        public static TaxonomyCriteria GetTaxonomyCriteria(this object o)
        {
            return new TaxonomyCriteria();
        }

        public static TaxonomyItemCriteria GetTaxonomyItemCriteria(this object o)
        {
            return new TaxonomyItemCriteria();
        }

        public static UserCriteria GetUserCriteria(this object o)
        {
            return new UserCriteria();
        }
    }
}