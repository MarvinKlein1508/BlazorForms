﻿using DatabaseControllerProvider;
using FormPortal.Core.Interfaces;

namespace FormPortal.Core.Models
{
    public class FormEntryStatusChange : IDbModel
    {
        [CompareField("history_id")]
        public int Id { get; set; }
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

        public Dictionary<string, object?> GetParameters()
        {
            return new Dictionary<string, object?>
            {
                { "HISTORY_ID", Id },
                { "ENTRY_ID", EntryId },
                { "STATUS_ID", StatusId },
                { "USER_ID", UserId },
                { "COMMENT", Comment },
                { "DATE_ADDED", DateAdded }
            };
        }
    }
}