using DbController;

namespace BlazorForms.Core.Models
{
    public class FormEntryStatusChange : IDbModel<int?>
    {
        [CompareField("history_id")]
        public int HistoryId { get; set; }
        [CompareField("entry_id")]
        public int EntryId { get; set; }
        [CompareField("status_id")]
        public int StatusId { get; set; }
        [CompareField("user_id")]
        public int UserId { get; set; }
        [CompareField("comment")]
        public string Comment { get; set; } = string.Empty;
        [CompareField("date_added")]
        public DateTime DateAdded { get; set; }

        public List<FormEntryHistoryNotify> Notifiers { get; set; } = new();

        [CompareField("display_name")]
        public string Username { get; set; } = string.Empty;

        public int? GetIdentifier()
        {
            return HistoryId > 0 ? HistoryId : null;
        }
        public Dictionary<string, object?> GetParameters()
        {
            return new Dictionary<string, object?>
            {
                { "HISTORY_ID", HistoryId },
                { "ENTRY_ID", EntryId },
                { "STATUS_ID", StatusId },
                { "USER_ID", UserId },
                { "COMMENT", Comment },
                { "DATE_ADDED", DateAdded }
            };
        }
    }
}
