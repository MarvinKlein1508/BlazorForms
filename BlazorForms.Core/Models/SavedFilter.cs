using BlazorForms.Core.Filters;
using DbController;

namespace BlazorForms.Core.Models
{
    public class SavedFilter : IDbModel
    {
        [CompareField("user_filter_id")]
        public int Id { get; set; }
        [CompareField("user_id")]
        public int UserId { get; set; }
        [CompareField("filter_type")]
        public FilterTypes FilterType { get; set; }
        [CompareField("page")]
        public string Page { get; set; } = string.Empty;
        [CompareField("serialized")]
        public string Json { get; set; } = string.Empty;

        public Dictionary<string, object?> GetParameters()
        {
            return new()
            {
                { "USER_FILTER_ID", Id },
                { "user_id", UserId },
                { "filter_type", FilterType.ToString() },
                { "page", Page },
                { "serialized", Json },
            };
        }
    }
}
