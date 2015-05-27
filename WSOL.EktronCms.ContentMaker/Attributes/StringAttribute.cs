namespace WSOL.EktronCms.ContentMaker.Attributes
{
    using System;

    /// <summary>
    /// Allows for friendlier string value for enums and other types
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class StringAttribute : System.Attribute
    {
        private string _value;

        public StringAttribute(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }
    }
}