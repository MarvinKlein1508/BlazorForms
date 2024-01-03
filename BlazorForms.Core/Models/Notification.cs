using DbController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Core.Models
{
    public class Notification : IDbModel
    {
        [CompareField("notification_id")]
        public int Id { get; set; }
        [CompareField("created")]
        public DateTime Created { get; set; } = DateTime.Now;
        [CompareField("user_id")]
        public int? UserId { get; set; } = null;
        [CompareField("icon")]
        public string? Icon { get; set; }
        [CompareField("title")]
        public string Title { get; set; } = string.Empty;
        [CompareField("details")]
        public string Details { get; set; } = string.Empty;
        [CompareField("href")]
        public string Href { get; set; } = string.Empty;
        [CompareField("is_read")]
        public bool IsRead { get; set; }
        [CompareField("read_timestamp")]
        public DateTime? ReadTimestamp { get; set; } = null;


        public Dictionary<string, object?> GetParameters()
        {
            return new Dictionary<string, object?>
            {
                { "NOTIFICATION_ID", Id },
                { "CREATED", Created },
                { "USER_ID", UserId },
                { "ICON", Icon },
                { "TITLE", Title },
                { "DETAILS", Details },
                { "HREF", Href },
                { "IS_READ", IsRead },
                { "READ_TIMESTAMP", ReadTimestamp }
            };
        }
    }
}
