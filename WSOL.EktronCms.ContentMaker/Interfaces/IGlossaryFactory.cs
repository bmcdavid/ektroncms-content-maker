namespace WSOL.EktronCms.ContentMaker.Interfaces
{
    using System.Collections.Generic;

    public interface IGlossaryFactory
    {
        IEnumerable<IGlossary> GetGlossaries<T>(int pageSize = 1000) where T : IContent, IGlossary;

        IEnumerable<IGlossary> GetGlossaries(List<long> glossaryConfigTypes = null, int pageSize = 1000);

        string TranslateKey<T>(string key, int languageId, bool returnKeyOnFail = false, IEnumerable<IGlossary> glossaries = null) where T : IContent, IGlossary;

        string TranslateKey(string key, int languageId, bool returnKeyOnFail = false, IEnumerable<IGlossary> glossaries = null);
    }
}
