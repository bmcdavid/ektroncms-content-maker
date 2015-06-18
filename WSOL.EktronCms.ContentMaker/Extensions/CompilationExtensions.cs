namespace WSOL.EktronCms.ContentMaker.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Web;
    using WSOL.EktronCms.ContentMaker.Attributes;
    using WSOL.EktronCms.ContentMaker.Interfaces;
    using WSOL.IocContainer;

    public static class CompilationExtensions
    {
        // Stores properties of given type for lifetime of application
        internal static SafeDictionary<Type, PropertyDescriptorCollection> _PropertyTypes;

        // Stores properties of given type for lifetime of application
        internal static SafeDictionary<Type, ContentDescriptorAttribute> _TypedContentDictionary;

        private static object _lock = new object();

        /// <summary>
        /// Specific to Ektron
        /// </summary>
        private static SafeDictionary<CustomTuple<PropertyDescriptorCollection, Type, Type>, IEnumerable<PropertyDescriptor>> _TypeDecriptors;

        /// <summary>
        /// Gets a list of back end types only
        /// </summary>
        /// <param name="types">IEnumerable of IContent Types</param>
        /// <returns></returns>
        public static IEnumerable<Type> FilterForBackEndTypes(this IEnumerable<Type> types)
        {
            if (types == null)
            {
                return types;
            }

            return HttpContext.Current.GetApplicationItem<IEnumerable<Type>>
            (
                "WSOL:IContentTypes.BackEndOnly",
                () =>
                {
                    var backendTypes = new List<Type>();

                    foreach (Type type in types)
                    {
                        // Get instance of the attribute.
                        ContentDescriptorAttribute attribute = Attribute.GetCustomAttribute(type,
                            typeof(ContentDescriptorAttribute)) as ContentDescriptorAttribute;

                        if (attribute != null && attribute.BackendContent)
                        {
                            backendTypes.Add(type);
                        }
                    }

                    return backendTypes;
                }
            );
        }

        /// <summary>
        /// Gets a listing of front end content types
        /// </summary>
        /// <param name="types">IEnumerable of IContent Types</param>
        /// <returns></returns>
        public static IEnumerable<Type> FilterForFrontEndTypes(this IEnumerable<Type> types)
        {
            if (types == null)
            {
                return types;
            }

            return types.Except(types.FilterForBackEndTypes());
        }

        /// <summary>
        /// Gets all models with the ContentDescriptor attribute
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Type, ContentDescriptorAttribute> GetContentDescriptors()
        {
            if (_TypedContentDictionary == null)
            {
                lock (_lock)
                {
                    var types = (true).ScanForAttribute<ContentDescriptorAttribute>(false, true, true);
                    _TypedContentDictionary = new SafeDictionary<Type, ContentDescriptorAttribute>();

                    foreach (var type in types)
                    {
                        // Get instance of the attribute.
                        ContentDescriptorAttribute attribute = Attribute.GetCustomAttribute(type,
                            typeof(ContentDescriptorAttribute)) as ContentDescriptorAttribute;

                        if (attribute != null)
                        {
                            _TypedContentDictionary.Add(type, attribute);
                        }
                    }
                }
            }

            return _TypedContentDictionary;
        }

        /// <summary>
        /// Gets all types utilize IContent
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Type> GetIContentTypes(this Type t)
        {
            return HttpContext.Current.GetApplicationItem<IEnumerable<Type>>
                (
                    "WSOL:IContentTypes",
                    () =>
                    {
                        var icontentTypes = new List<Type>();
                        var interfacedTypes = t.ScanForInterface<IContent>(true, true);
                        icontentTypes.AddRange(interfacedTypes);

                        WSOL.IocContainer.OjectExtensions.ScanAllTypes
                        (
                            (Type type) =>
                            {
                                foreach (Type interfaceType in interfacedTypes)
                                {
                                    if (type.IsSubclassOf(interfaceType))
                                    {
                                        icontentTypes.Add(type);
                                    }
                                }
                            },
                            false,
                            false,
                            false
                        );
                        // causes scan error if Ektron isn't installed
                        //foreach (Type interfaceType in interfacedTypes)
                        //{
                        //    foreach (Assembly assembly in BuildManager.GetReferencedAssemblies())
                        //    {
                        //        foreach (Type type in assembly.GetTypes())
                        //        {
                        //            if (type.IsSubclassOf(interfaceType))
                        //            {
                        //                icontentTypes.Add(type);
                        //            }
                        //        }
                        //    }
                        //}

                        return icontentTypes.Distinct();
                    }

                );
        }

        /// <summary>
        /// Gets the appropriate model for the given XmlConfigId
        /// </summary>
        /// <param name="Id">Ektron XmlConfigId</param>
        /// <returns></returns>
        public static Type GetModelType(this long Id)
        {
            var types = GetContentDescriptors();
            Type type = null;

            foreach (var pair in types.Where(x => x.Value.XmlConfigId == Id))
            {
                type = pair.Key;
                break;
            }

            return type;
        }

        /// <summary>
        /// Gets property descriptor collection for type
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static PropertyDescriptorCollection GetPropertyTypes(this Type t)
        {
            if (_PropertyTypes == null)
            {
                _PropertyTypes = new SafeDictionary<Type, PropertyDescriptorCollection>();
            }

            PropertyDescriptorCollection item;

            if (!_PropertyTypes.TryGetValue(t, out item))
            {
                item = TypeDescriptor.GetProperties(t);
                _PropertyTypes.Add(t, item);
            }

            return item;
        }

        /// <summary>
        /// Returns string value for enum field
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetStringValue(this Enum value)
        {
            string output = string.Empty;
            Type type = value.GetType();
            FieldInfo fi = type.GetField(value.ToString());
            StringAttribute[] attrs = fi.GetCustomAttributes(typeof(StringAttribute), false) as StringAttribute[];

            if (attrs != null && attrs.Length > 0)
            {
                output = attrs[0].Value;
            }

            return output;
        }

        /// <summary>
        /// Gets the TypedContentDescriptor for given Id
        /// </summary>
        /// <param name="Id">Ektron XmlConfigId</param>
        /// <returns>null if not found</returns>
        public static ContentDescriptorAttribute GetTypeAttribute(this long Id)
        {
            var types = GetContentDescriptors();
            ContentDescriptorAttribute attributeType = null;

            foreach (var pair in types.Where(x => x.Value.XmlConfigId == Id))
            {
                attributeType = pair.Value;
                break;
            }

            return attributeType;
        }

        /// <summary>
        /// Specific to Ektron Widget Types
        /// </summary>
        /// <typeparam name="Type1">Ektron.Cms.Widget.WidgetDataMember</typeparam>
        /// <typeparam name="Type2">Ektron.Cms.Widget.GlobalWidgetData</typeparam>
        /// <param name="p"></param>
        public static IEnumerable<PropertyDescriptor> GetWidgetPropertyTypes<Type1, Type2>(this PropertyDescriptorCollection p)
        {
            if (_TypeDecriptors == null)
            {
                _TypeDecriptors = new SafeDictionary<CustomTuple<PropertyDescriptorCollection, Type, Type>, IEnumerable<System.ComponentModel.PropertyDescriptor>>();
            }

            CustomTuple<PropertyDescriptorCollection, Type, Type> tuple = CustomTuple.Create(p, typeof(Type1), typeof(Type2));
            IEnumerable<System.ComponentModel.PropertyDescriptor> item;

            if (!_TypeDecriptors.TryGetValue(tuple, out item))
            {
                item = p.OfType<PropertyDescriptor>()
                                    .Where(x => x.Attributes.OfType<Type1>().Any() ||
                                        x.Attributes.OfType<Type2>().Any());

                _TypeDecriptors.Add(tuple, item);
            }

            return item;
        }

        /// <summary>
        /// Gets XmlConfigId for given Type T
        /// </summary>
        /// <param name="T"></param>
        /// <returns>-1 if not found</returns>
        public static long GetXmlConfigId(this Type T)
        {
            var types = GetContentDescriptors();
            ContentDescriptorAttribute attributeType;

            if (!types.TryGetValue(T, out attributeType))
            {
                return -1;
            }

            return attributeType.XmlConfigId;
        }
    }
}