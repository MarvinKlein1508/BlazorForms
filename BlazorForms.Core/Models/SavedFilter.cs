using BlazorForms.Core.Filters;

namespace BlazorForms.Core.Models
{
    public class SavedFilter : IDbModel<int?>
    {
        [CompareField("user_filter_id")]
        public int UserFilterId { get; set; }
        [CompareField("user_id")]
        public int UserId { get; set; }
        [CompareField("filter_type")]
        public FilterTypes FilterType { get; set; }
        [CompareField("page")]
        public string Page { get; set; } = string.Empty;
        [CompareField("serialized")]
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
}
