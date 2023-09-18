using BlazorForms.Core.Filters.Abstract;

namespace BlazorForms.Core.Filters
{
    public record FormFilter : PageFilterBase
    {
        public bool ShowOnlyActiveForms { get; set; }
        public bool HideLoginRequired { get; set; }
        public int UserId { get; set; }

        public int LanguageId { get; set; }
        public override FilterTypes FilterType => FilterTypes.Form;
    }
}
