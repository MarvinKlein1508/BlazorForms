using BlazorForms.Core.Filters.Abstract;

namespace BlazorForms.Core.Filters
{
    public class FormEntryFilter : PageFilterBase
    {
        public int UserId { get; set; }
        public bool SearchAssigned { get; set; }
        public int StatusId { get; set; }
    }
}
