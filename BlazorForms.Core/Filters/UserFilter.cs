using BlazorForms.Core.Filters.Abstract;

namespace BlazorForms.Core.Filters
{
    public record UserFilter : PageFilterBase
    {
        public List<int> BlockedIds { get; set; } = new();
        public override FilterTypes FilterType => FilterTypes.User;
    }
}
