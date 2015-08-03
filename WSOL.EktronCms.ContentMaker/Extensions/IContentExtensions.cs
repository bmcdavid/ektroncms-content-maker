namespace WSOL.EktronCms.ContentMaker.Extensions
{
    using WSOL.EktronCms.ContentMaker.Interfaces;

    public static class IContentExtensions
    {
        /// <summary>
        /// Determines if IContent has an ID and HTML string
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this IContent content)
        {
            if (content != null)
            {
                if (content.Id != 0 && content.Html != null)
                {
                    string s = content.Html;

                    for (int index = 0; index < s.Length; ++index)
                    {
                        if (!char.IsWhiteSpace(s[index]))
                            return false;
                    }
                }
            }

            return true;
        }
    }
}