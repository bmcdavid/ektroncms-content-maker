namespace WSOL.EktronCms.ContentMaker.Extensions
{
    using Ektron.Cms;
    using Ektron.Cms.Common;
    using Ektron.Cms.Content;
    using Ektron.Cms.Framework.Content;
    using Ektron.Cms.Organization;
    using Ektron.Cms.User;
    using WSOL.EktronCms.ContentMaker.Interfaces;

    public static class EktronFrameworkExtensions
    {
        public static ContentManager GetContentManager(this ContentCriteria o, int Language, bool AdminMode = false)
        {
            return FrameworkFactory<ContentManager>.Get(AdminMode, Language);
        }

        public static TApi GetFrameworkManager<TApi>(this object o, int Langauge, bool AdminMode = false) where TApi : new()
        {
            return FrameworkFactory<TApi>.Get(AdminMode, Langauge);
        }

        public static ContentCriteria GetContentCriteria(this object o, bool returnExpired = false)
        {
            ContentCriteria criteria = new ContentCriteria();
            criteria.ReturnMetadata = true;

            if (returnExpired)
            {
                criteria.AddFilter(ContentProperty.IsArchived, CriteriaFilterOperator.GreaterThan, -1);
            }

            return criteria;
        }

        public static ContentCriteria GetContentCriteria<TModel>(this object o, bool returnExpired = false) where TModel : IContent
        {
            ContentCriteria criteria = GetContentCriteria(o, returnExpired);

            // set xml config id from IContent Model
            criteria.AddFilter(ContentProperty.XmlConfigurationId, CriteriaFilterOperator.EqualTo, typeof(TModel).GetXmlConfigId());

            return criteria;
        }

        public static FolderCriteria GetFolderCriteria(this object o)
        {
            return new FolderCriteria();
        }

        public static UserCriteria GetUserCriteria(this object o)
        {
            return new UserCriteria();
        }

        public static TaxonomyCriteria GetTaxonomyCriteria(this object o)
        {
            return new TaxonomyCriteria();
        }

        public static TaxonomyItemCriteria GetTaxonomyItemCriteria(this object o)
        {
            return new TaxonomyItemCriteria();
        }

    }
}