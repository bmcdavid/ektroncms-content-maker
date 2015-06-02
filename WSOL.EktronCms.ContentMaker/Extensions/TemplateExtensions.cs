namespace WSOL.EktronCms.ContentMaker.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using WSOL.EktronCms.ContentMaker.Attributes;
    using WSOL.EktronCms.ContentMaker.Delegates;
    using WSOL.EktronCms.ContentMaker.Interfaces;
    using WSOL.IocContainer;

    public static class TemplateExtensions
    {
        private static ILogger _Logger = InitializationContext.Locator.Get<ILogger>();
        private static object _lock = new object();
        internal static SafeDictionary<CustomTuple<Type, string>, TemplateDescriptorAttribute> _ResolvedTemplates;
        internal static SafeDictionary<TemplateDescriptorAttribute, Type> _DescriptorList;

        /// <summary>
        /// Adds a template for given type
        /// </summary>
        /// <param name="t"></param>
        /// <param name="templateDescriptor"></param>
        /// <returns></returns>
        public static bool RegisterTemplate(this Type t, TemplateDescriptorAttribute templateDescriptor)
        {
            if (templateDescriptor == null || t == null)
            {
                return false;
            }

            if (_DescriptorList == null)
            {
                BuildDescriptorList(false);
            }

            Type checkEntry;

            if (!_DescriptorList.TryGetValue(templateDescriptor, out checkEntry))
            {
                lock (_lock)
                {
                    _DescriptorList.Add(templateDescriptor, t);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Build _DescriptorList and assign the ModelType for each TemplateDescriptor based on its generic type in ControlBase&gt;T&lt;
        /// </summary>
        internal static void BuildDescriptorList(bool regen = false)
        {
            if (_DescriptorList == null || regen)
            {
                lock (_lock)
                {
                    _DescriptorList = new SafeDictionary<TemplateDescriptorAttribute, Type>();

                    // Resets the descriptors for web sites, not really needed for webapps
                    _ResolvedTemplates = new SafeDictionary<CustomTuple<Type, string>, TemplateDescriptorAttribute>();

                    var descriptors = _DescriptorList.ScanForAttribute<TemplateDescriptorAttribute>(regen, false, false, true);

                    foreach (var type in descriptors.Where(x => x.BaseType.IsGenericType)) // only get the generics, so we can set type below 
                    {
                        var descriptor = Attribute.GetCustomAttribute(type, typeof(TemplateDescriptorAttribute)) as TemplateDescriptorAttribute;
                        descriptor.SetModelType(type.BaseType.GetGenericArguments().First()); // assign the generic T

                        try
                        {
                            _DescriptorList.Add(descriptor, type);
                        }
                        catch(ArgumentException)
                        {
                            var message = new Exception("The template descriptor paths must be unique for every instance! Multiple have been found for: " +
                                    descriptor.Path + ". If this site is compiled please add \"WSOL.EktronCms.ContentMaker.Compiled\" application setting equal to \"true\". Stored instance Type = " +
                                    _DescriptorList[descriptor].FullName + " and new instance = " + type.FullName + " for " + descriptor.ToString());

                            message.Log(typeof(TemplateExtensions).FullName, System.Diagnostics.EventLogEntryType.Information, descriptor);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Populates the loaded user control's CurrentData property with Data
        /// </summary>
        /// <param name="control"></param>
        /// <param name="data"></param>
        private static void LoadContentData(Control control, object data)
        {
            IContentControl contentDataControl = control as IContentControl;

            if (contentDataControl == null)
                return;

            contentDataControl.CurrentData = data as IContent;
        }

        /// <summary>
        /// Resolution engine for models to find the appropriate render
        /// </summary>
        /// <param name="T"></param>
        /// <param name="OnResolved"></param>
        /// <param name="Tags"></param>
        /// <returns></returns>
        internal static TemplateDescriptorAttribute ResolveTemplate(this Type T, TemplateResolverHandler OnResolved, string[] Tags, IContent Item)
        {
            TemplateDescriptorAttribute descriptor = null;

            if (Tags == null)
            {
                Tags = new string[] { };
            }

            CustomTuple<Type, string> tuple = CustomTuple.Create(T, string.Join(",", Tags));

            if (!_ResolvedTemplates.TryGetValue(tuple, out descriptor))
            {
                BuildDescriptorList(false);

                if (_DescriptorList != null && _DescriptorList.Count > 0)
                {
                    // Only get the base Types that have descriptors
                    IEnumerable<Type> baseTypes = T.GetBaseTypes().Intersect(_DescriptorList.Select(x => x.Key.ModelType));

                    lock (_lock)
                    {
                        // Check tags
                        if (Tags != null && Tags.Length > 0)
                        {
                            var matches = _DescriptorList.Where(x => x.Key.Tags.Any(y => Tags.Contains(y)));

                            if (matches.Any())
                            {
                                // Get the highest number of matches
                                var counts = (from x in matches select new { Frequency = x.Key.Tags.Count(y => Tags.Contains(y)), Descriptor = x.Key })
                                    .OrderByDescending(x => x.Frequency).ThenByDescending(x => x.Descriptor.Default);

                                #region Required Tags

                                // Check type
                                var typecheck = counts.Where(x => x.Descriptor.ModelType == T && x.Descriptor.RequireTags);//.OrderBy(x => x.Descriptor.Path);

                                if (typecheck.Any())
                                {
                                    var defaultCheck = typecheck.Where(x => x.Descriptor.Default);

                                    if (defaultCheck.Any()) // defaults should win
                                    {
                                        descriptor = defaultCheck.First().Descriptor;
                                    }
                                    else
                                    {
                                        descriptor = typecheck.First().Descriptor;
                                    }
                                }
                                else
                                {
                                    // Inheritance check, pushing require tags to top
                                    var inheritanceCheck = counts.Where(x => x.Descriptor.Inherited && x.Descriptor.RequireTags && x.Descriptor.ModelType.IsAssignableFrom(T));

                                    if (inheritanceCheck.Any())
                                    {
                                        var defaultCheck = inheritanceCheck.Where(x => x.Descriptor.Default);

                                        if (defaultCheck.Any()) // defaults should win
                                        {
                                            // check baseTypes against ModelType, sort by baseTypes order
                                            var orderedTypes = from type in baseTypes join item in defaultCheck on type equals item.Descriptor.ModelType select item;
                                            descriptor = orderedTypes.First().Descriptor;
                                        }
                                        else
                                        {
                                            var orderedTypes = from type in baseTypes join item in inheritanceCheck on type equals item.Descriptor.ModelType select item;
                                            descriptor = orderedTypes.First().Descriptor;
                                        }
                                    }
                                }

                                #endregion

                                #region Non Required Tags

                                // Check for non required tags
                                if (descriptor == null)
                                {
                                    // Only look at descriptors with no required tags
                                    var noRequiredTags = counts.Where(x => !x.Descriptor.RequireTags).OrderBy(x => x.Descriptor.Path);

                                    // Check type
                                    var typecheck2 = noRequiredTags.Where(x => x.Descriptor.ModelType == T);

                                    if (typecheck2.Any())
                                    {
                                        var defaultCheck = typecheck2.Where(x => x.Descriptor.Default).OrderBy(x => x.Descriptor.Path);

                                        if (defaultCheck.Any()) // defaults should win
                                        {
                                            descriptor = defaultCheck.First().Descriptor;
                                        }
                                        else
                                        {
                                            descriptor = typecheck2.First().Descriptor;
                                        }
                                    }
                                    else
                                    {
                                        // Inheritance check
                                        var inheritanceCheck = noRequiredTags.Where(x => x.Descriptor.Inherited && x.Descriptor.ModelType.IsAssignableFrom(T));

                                        if (inheritanceCheck.Any())
                                        {
                                            var defaultCheck = inheritanceCheck.Where(x => x.Descriptor.Default);

                                            if (defaultCheck.Any()) // defaults should win
                                            {
                                                var orderedTypes = from type in baseTypes join item in defaultCheck on type equals item.Descriptor.ModelType select item;
                                                descriptor = orderedTypes.First().Descriptor;
                                            }
                                            else
                                            {
                                                var orderedTypes = from type in baseTypes join item in inheritanceCheck on type equals item.Descriptor.ModelType select item;
                                                descriptor = orderedTypes.First().Descriptor;
                                            }
                                        }
                                    }
                                }

                                #endregion

                            }
                        }

                        #region Default Check

                        if (descriptor == null)
                        {
                            var matches = _DescriptorList.Where(x => x.Key.Default).OrderBy(x => x.Key.Path);

                            if (matches.Any())
                            {
                                // Check type
                                var typecheck = matches.Where(x => x.Key.ModelType == T);

                                if (typecheck.Any())
                                {
                                    descriptor = typecheck.First().Key;
                                }
                                else
                                {
                                    // Inheritance check
                                    var inheritanceCheck = matches.Where(x => x.Key.Inherited && x.Key.ModelType.IsAssignableFrom(T));

                                    if (inheritanceCheck.Any())
                                    {
                                        var orderedTypes = from type in baseTypes join item in inheritanceCheck on type equals item.Key.ModelType select item;
                                        descriptor = inheritanceCheck.First().Key;
                                    }
                                }
                            }
                        }

                        #endregion

                        #region Non Required Tags, final check if no defaults are located.

                        // Check for non required tags
                        if (descriptor == null)
                        {
                            // Only look at descriptors with no required tags
                            var noRequiredTags = _DescriptorList.Where(x => !x.Key.RequireTags).OrderBy(x => x.Key.Path);

                            // Check type
                            var typecheck = noRequiredTags.Where(x => x.Key.ModelType == T);

                            if (typecheck.Any())
                            {
                                var defaultCheck = typecheck.Where(x => x.Key.Default);

                                if (defaultCheck.Any()) // defaults should win
                                {                                    
                                    descriptor = defaultCheck.First().Key;
                                }
                                else
                                {
                                    descriptor = typecheck.First().Key;
                                }
                            }
                            else
                            {
                                // Inheritance check
                                var inheritanceCheck = noRequiredTags.Where(x => x.Key.Inherited && x.Key.ModelType.IsAssignableFrom(T));

                                if (inheritanceCheck.Any())
                                {
                                    var defaultCheck = inheritanceCheck.Where(x => x.Key.Default);

                                    if (defaultCheck.Any()) // defaults should win
                                    {
                                        var orderedTypes = from type in baseTypes join item in defaultCheck on type equals item.Key.ModelType select item;
                                        descriptor = orderedTypes.First().Key;
                                    }
                                    else
                                    {
                                        var orderedTypes = from type in baseTypes join item in inheritanceCheck on type equals item.Key.ModelType select item;
                                        descriptor = inheritanceCheck.First().Key;
                                    }
                                }
                            }
                        }

                        #endregion
                    }

                    // Add to dictionary for quicker lookups
                    if (descriptor != null)
                    {
                        _ResolvedTemplates.Add(tuple, descriptor);
                    }
                }
            }

            if (descriptor == null)
            {
                if (_Logger != null)
                {
                    _Logger.Log
                    (
                        "Null descriptor found, dictionary had following types defined " + string.Join(",", _ResolvedTemplates.Select(x => x.Key.Item1.FullName).ToArray()),
                        typeof(TemplateExtensions).FullName,
                        null,
                        System.Diagnostics.EventLogEntryType.Warning,
                        null
                    );
                }
            }

            // Allows a developer to change the resolved template descriptor.
            if (OnResolved != null)
            {
                descriptor = OnResolved(descriptor, Tags, Item);
            }

            return descriptor;
        }

        /// <summary>
        /// Loads the given control path and assigns the current data (renderData) to control.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="renderData"></param>
        /// <param name="bindDataAction"></param>
        /// <returns></returns>
        internal static Control Load(this string path, object renderData, Action<Control, object> bindDataAction = null)
        {
            Control control = null;

            if (bindDataAction == null)
            {
                bindDataAction = new Action<Control, object>(LoadContentData);
            }

            control = ((Page)HttpContext.Current.CurrentHandler).LoadControl(path);

            PartialCachingControl cachedControl = control as PartialCachingControl;

            if (cachedControl != null)
            {
                control.Init += (EventHandler)((sender, e) => bindDataAction(cachedControl.CachedControl, renderData));
            }
            else
            {
                bindDataAction(control, renderData);
            }

            return control;
        }
    }
}