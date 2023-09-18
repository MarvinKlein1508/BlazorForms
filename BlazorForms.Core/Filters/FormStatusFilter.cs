using BlazorForms.Core.Filters.Abstract;
using System.Globalization;

namespace BlazorForms.Core.Filters
{
    public record FormStatusFilter : PageFilterBase
    {
        public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;
        public override FilterTypes FilterType => FilterTypes.FormStatus;
    }
}
