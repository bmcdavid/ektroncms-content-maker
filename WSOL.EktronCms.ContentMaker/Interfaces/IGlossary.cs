namespace WSOL.EktronCms.ContentMaker.Interfaces
{
    using System.Collections.Generic;

    public interface IGlossary
    {
        int GlossaryLanguageId { get; set; }

        Dictionary<string, string> GlossarySet { get; set; }
    }
}
