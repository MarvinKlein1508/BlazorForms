using DbController;

namespace BlazorForms.Core.Models
{
    public class FormEntryHistoryNotify
    {
        [CompareField("history_id")]
        public int HistoryId { get; set; }
        [CompareField("user_id")]
        public int UserId { get; set; }

        public bool Notify { get; set; }
    }
}
