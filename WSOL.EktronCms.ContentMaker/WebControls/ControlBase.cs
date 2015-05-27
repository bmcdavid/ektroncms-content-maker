namespace WSOL.EktronCms.ContentMaker.WebControls
{
    using WSOL.EktronCms.ContentMaker.Interfaces;

    /// <summary>
    /// Base class to define a renderer and Item
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ControlBase<T> : System.Web.UI.UserControl, IContentControl where T : IContent
    {
        public ControlBase() { }

        public T CurrentData { get; set; }

        IContent IContentControl.CurrentData
        {
            get
            {
                return (IContent)this.CurrentData;
            }
            set
            {
                this.CurrentData = (T)value;
            }
        }
    }
}