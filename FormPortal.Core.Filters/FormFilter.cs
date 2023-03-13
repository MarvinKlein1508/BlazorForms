using FormPortal.Core.Filters.Abstract;

namespace FormPortal.Core.Filters
{
    public class FormFilter : PageFilterBase
    {
        public bool ShowOnlyActiveForms { get; set; }
        public bool HideLoginRequired { get; set; }
    }
}
