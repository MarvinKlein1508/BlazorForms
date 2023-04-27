using FormPortal.Core.Filters.Abstract;

namespace FormPortal.Core.Filters
{
    public class UserFilter : PageFilterBase
    {
        public List<int> BlockedIds { get; set; } = new();
    }
}
