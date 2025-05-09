using BlazorForms.Core.Filters;
using BlazorForms.Core.Interfaces;

namespace BlazorForms.Core.Models;

public class SavedFilter : IDbModel<int?>
{
    public int UserFilterId { get; set; }
    public int UserId { get; set; }
    public FilterTypes FilterType { get; set; }
    public string Page { get; set; } = string.Empty;
    public string Json { get; set; } = string.Empty;
    public int? GetIdentifier()
    {
        return UserFilterId > 0 ? UserFilterId : null;
    }
    public Dictionary<string, object?> GetParameters()
    {
        return new()
        {
            { "USER_FILTER_ID", UserFilterId },
            { "user_id", UserId },
            { "filter_type", FilterType.ToString() },
            { "page", Page },
            { "serialized", Json },
        };
    }
}
