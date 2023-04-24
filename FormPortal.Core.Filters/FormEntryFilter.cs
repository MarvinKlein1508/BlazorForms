using FormPortal.Core.Filters.Abstract;

namespace FormPortal.Core.Filters
{
    public class FormEntryFilter : PageFilterBase
    {
        public int UserId { get; set; }
        public bool SearchAssigned { get; set; }
        public int StatusId { get; set; }
    }
}
