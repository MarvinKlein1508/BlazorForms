using BlazorForms.Core.Enums;
using BlazorForms.Core.Filters.Abstract;

namespace BlazorForms.Core.Filters
{
    public record FormEntryFilter : PageFilterBase
    {
        public int UserId { get; set; }
        public bool SearchAssigned { get; set; }
        public int StatusId { get; set; }
        public Priority? Priority { get; set; } = null;
        public override FilterTypes FilterType => FilterTypes.FormEntry;
    }
}
