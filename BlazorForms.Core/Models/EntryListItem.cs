using BlazorForms.Core.Services;
using DbController;
using System.Globalization;

namespace BlazorForms.Core.Models
{
    public class EntryListItem
    {
        [CompareField("entry_id")]
        public int EntryId { get; set; }
        [CompareField("form_id")]
        public int FormId { get; set; }
        [CompareField("status_id")]
        public int StatusId { get; set; }
        [CompareField("name")]
        public string Name { get; set; } = string.Empty;
        [CompareField("creation_date")]
        public DateTime CreationDate { get; set; } = DateTime.Now;
        [CompareField("creation_user_id")]
        public int? CreationUserId { get; set; }
        [CompareField("last_change")]
        public DateTime LastChange { get; set; } = DateTime.Now;
        [CompareField("last_change_user_id")]
        public int? LastChangeUserId { get; set; }
        [CompareField("form_name")]
        public string FormName { get; set; } = string.Empty;
        [CompareField("username_creator")]
        public string UsernameCreator { get; set; } = string.Empty;
        [CompareField("username_last_change")]
        public string UsernameLastChange { get; set; } = string.Empty;

        public List<int> ManagerIds { get; set; } = new();

    }
}
