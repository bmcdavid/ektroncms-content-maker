namespace WSOL.EktronCms.ContentMaker.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using WSOL.EktronCms.ContentMaker.Interfaces;

    public static class MetaDataExtensions
    {
        /// <summary>
        /// Determines if metadata set has given key.
        /// </summary>
        /// <param name="metaData"></param>
        /// <param name="key">Name of metadata field to look for.</param>
        /// <param name="allowEmptyValues">If true and metaDataSet has empty value, then true is returned, otherwise false would be returned.</param>
        /// <returns></returns>
        public static bool HasMetaDataValue(this IEnumerable<IMetaData> metaData, string key, bool allowEmptyValues)
        {
            if (metaData != null && metaData.Any())
            {
                var check = metaData.Where(x => x.Name.ToLower() == key.ToLower());

                if (!allowEmptyValues && check.Any())
                    return !string.IsNullOrEmpty(check.First().Value);

                return check.Any();
            }

            return false;
        }

        /// <summary>
        /// Determines if metadata key's value matches value, ignores letter case
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool CompareMetaDataValue(this IEnumerable<IMetaData> metaData, string key, string value)
        {
            if (metaData != null && metaData.Any())
            {
                var check = metaData.Where(x => x.Name.ToLower() == key.ToLower());

                if (check.Any())
                    return check.First().Value.ToLower().Trim() == value.ToLower().Trim();
            }

            return false;
        }

        /// <summary>
        /// Gets metadata value for key, returns String.Empy if no key is found.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetMetaDataValue(this IEnumerable<IMetaData> metaData, string key)
        {
            if (metaData != null && metaData.Any())
            {
                var check = metaData.Where(x => x.Name.ToLower() == key.ToLower());

                if (check.Any())
                    return check.First().Value.Trim();
            }

            return string.Empty;
        }
    }
}