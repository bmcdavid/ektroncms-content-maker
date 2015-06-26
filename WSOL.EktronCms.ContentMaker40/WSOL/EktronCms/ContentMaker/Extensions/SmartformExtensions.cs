namespace WSOL.EktronCms.ContentMaker.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Web.Hosting;
    using System.Xml;
    using WSOL.IocContainer;

    public static class SmartformExtensions
    {
        private static ICacheManager _CacheManager = InitializationContext.Locator.Get<ICacheManager>();

        /// <summary>
        /// Gets a smartform dropdown as a dictionary
        /// </summary>
        /// <param name="SmartFormId"></param>
        /// <param name="FieldXPath"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetSmartFormDropDown(this long SmartFormId, string FieldXPath)
        {
            XmlDocument xmldoc = _CacheManager.CacheItem
                (
                    string.Format("WSOL:Cache:XmlConfig={0}", SmartFormId),
                    () =>
                    {
                        var xDoc = new XmlDocument();
                        var manager = FrameworkFactory<Ektron.Cms.Framework.Settings.SmartFormConfigurationManager>.Get(true);
                        var item = manager.GetItem(SmartFormId);

                        if (item != null)
                        {
                            //Load XML Document
                            xDoc.LoadXml(item.FieldList);
                        }

                        return xDoc;
                    },
                    _CacheManager.ShortInterval
                );

            //Get Node by Xpath
            XmlNode xnField = xmldoc.SelectSingleNode(String.Format("fieldlist/field[@xpath='{0}']", FieldXPath));

            //Create Hashtable to store results
            Dictionary<string, string> slResults = new Dictionary<string, string>();

            //verify field type
            if (xnField.Attributes.GetNamedItem("datatype").Value == "choice" || xnField.Attributes.GetNamedItem("datatype").Value == "selection" || xnField.Attributes.GetNamedItem("datatype").Value == "select-req")
            {
                //Get DataList ID
                string datalist = xnField.Attributes.GetNamedItem("datalist").Value;

                //Check to see if the requested node is a special ektron node other wise loop through and get choices
                if (xmldoc.SelectSingleNode("fieldlist/datalist[@name='" + datalist + "']").Attributes.GetNamedItem("ektdesignns_datasrc") != null)
                {
                    //Get all Choices
                    List<string> Choices = Load_Ek_XSDList(xmldoc.SelectSingleNode("fieldlist/datalist[@name='" + datalist + "']").Attributes.GetNamedItem("ektdesignns_datasrc").Value);

                    //Build Hashtable
                    foreach (string item in Choices)
                        slResults.Add(item, item);
                }
                else
                {
                    //Get all Choices
                    XmlNodeList Choices = xmldoc.SelectNodes("fieldlist/datalist[@name='" + datalist + "']/item");

                    //Build Hashtable
                    foreach (XmlNode item in Choices)
                        slResults.Add(item.InnerText, item.Attributes.GetNamedItem("value").Value);
                }
            }

            return slResults;
        }

        /// <summary>
        /// Gets a value for given Content Id and Xpath
        /// </summary>
        /// <param name="ContentID"></param>
        /// <param name="xPath"></param>
        /// <returns></returns>
        public static string GetSmartFormFieldValue(this long ContentID, string xPath)
        {
            var data = ContentID.GetEktronContentData(ContentID.GetLanguageId(), true, false);

            if (data != null)
                return GetSmartFormFieldValue(data.Html, xPath);

            return string.Empty;
        }

        /// <summary>
        /// Gets a value for given xml string and xpath query
        /// </summary>
        /// <param name="XML"></param>
        /// <param name="xPath"></param>
        /// <returns></returns>
        public static string GetSmartFormFieldValue(this string XML, string xPath)
        {
            string value = String.Empty;

            if (!String.IsNullOrEmpty(XML))
            {
                using (System.IO.StringReader x = new System.IO.StringReader(XML))
                {
                    System.Xml.XPath.XPathNavigator nav = new System.Xml.XPath.XPathDocument(x).CreateNavigator();
                    System.Xml.XPath.XPathNavigator node = nav.SelectSingleNode(xPath);

                    if (node != null)
                        value = node.InnerXml;

                }
            }

            return value;
        }
        /// <summary>
        /// Gets Data from Ektron xsd
        /// </summary>
        /// <param name="file">Path of XSD to load</param>
        /// <returns>a list of values from the provided xsd</returns>
        private static List<string> Load_Ek_XSDList(string file)
        {
            List<string> items = new List<string>();

            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(HostingEnvironment.MapPath(file));

                XmlNamespaceManager nsm = new XmlNamespaceManager(xmldoc.NameTable);
                nsm.AddNamespace("xsd", "http://www.w3.org/2001/XMLSchema");

                XmlNodeList nodelist = xmldoc.SelectNodes("xsd:schema/xsd:simpleType/xsd:restriction/xsd:enumeration", nsm);

                foreach (XmlNode node in nodelist)
                {
                    items.Add(node.Attributes.GetNamedItem("value").Value);
                }
            }
            catch (Exception ex)
            {
                ex.Log(typeof(SmartformExtensions).FullName, System.Diagnostics.EventLogEntryType.Error, file);
            }

            return items;
        }
    }
}