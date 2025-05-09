using BlazorForms.Core.Interfaces;

namespace BlazorForms.Core.Models;

public class FormEntryStatusChange : IDbModel<int?>
{
    public int HistoryId { get; set; }
    public int EntryId { get; set; }
    public int StatusId { get; set; }
    public int UserId { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime DateAdded { get; set; }

    public List<FormEntryHistoryNotify> Notifiers { get; set; } = new();

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
