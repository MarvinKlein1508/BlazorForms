using BlazorForms.Core.Filters.Abstract;

namespace BlazorForms.Core.Filters
{
    public class UserFilter : PageFilterBase
    {
        public List<int> BlockedIds { get; set; } = new();
    }
}
