namespace WSOL.EktronCms.ContentMaker.Interfaces
{
    /// <summary>
    /// Allows for customization of settings object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISettingsResolver<T> where T : IContent, ISettings
    {
        T Resolve(T settings);
    }
}
