using DbController;

namespace BlazorForms.Core.Models
{
    public class FormEntryHistoryNotify : IDbParameterizable
    {
        [CompareField("history_id")]
        public int HistoryId { get; set; }
        [CompareField("user_id")]
        public int UserId { get; set; }
        [CompareField("notify")]
        public bool Notify { get; set; }

        public Dictionary<string, object?> GetParameters()
        {
            return new Dictionary<string, object?>
            {
                { "HISTORY_ID", HistoryId },
                { "USER_ID", UserId },
                { "NOTIFY", Notify }
            };
        }
    }
}
