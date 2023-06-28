using BlazorForms.Core.Filters.Abstract;
using System.Globalization;

namespace BlazorForms.Core.Filters
{
    public class FormStatusFilter : PageFilterBase
    {
        public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;
    }
}
