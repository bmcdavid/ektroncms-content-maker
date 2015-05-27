namespace WSOL.Custom.ContentMaker.Samples.Views.SectionContent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using WSOL.Custom.ContentMaker.Samples.Models;
    using WSOL.EktronCms.ContentMaker.Attributes;
    using WSOL.EktronCms.ContentMaker.Samples.Helpers;
    using WSOL.EktronCms.ContentMaker.WebControls;

    [TemplateDescriptor(Path = "~/Views/SectionContent/Default.ascx", Default = true, RequireTags = false)]
    public partial class Default : ControlBase<WSOL.Custom.ContentMaker.Samples.Models.SectionContent>
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        private int _currentSectionSize = 0;

        protected bool SetSectionSize(IEnumerable<UnitItem> units)
        {
            if (units == null)
                _currentSectionSize = 0;
            else
                _currentSectionSize = units.Sum(x => x.Size);

            return true;
        }

        protected string UnitSizeClass(int size)
        {
            var fraction = FractionHelpers.ReduceFraction(size, _currentSectionSize);

            if (fraction != null)
                return string.Format(" size{0}of{1} ", fraction.Numerator, fraction.Denominator);

            return string.Empty;
        }
        
    }
}