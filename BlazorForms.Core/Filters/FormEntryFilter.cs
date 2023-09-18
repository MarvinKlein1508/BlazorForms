using BlazorForms.Core.Filters.Abstract;

namespace BlazorForms.Core.Filters
{
    public record FormEntryFilter : PageFilterBase
    {
        public int UserId { get; set; }
        public bool SearchAssigned { get; set; }
        public int StatusId { get; set; }
        public override FilterTypes FilterType => FilterTypes.FormEntry;
    }
}
