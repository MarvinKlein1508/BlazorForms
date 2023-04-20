using FormPortal.Core.Filters.Abstract;
using System.Globalization;

namespace FormPortal.Core.Filters
{
    public class FormStatusFilter : PageFilterBase
    {
        public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;
    }
}
