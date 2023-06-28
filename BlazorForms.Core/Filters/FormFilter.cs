using BlazorForms.Core.Filters.Abstract;

namespace BlazorForms.Core.Filters
{
    public class FormFilter : PageFilterBase
    {
        public bool ShowOnlyActiveForms { get; set; }
        public bool HideLoginRequired { get; set; }
        public int UserId { get; set; }
    }
}
