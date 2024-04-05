using DbController;

namespace BlazorForms.Core.Models;

public class FormEntryManagerContent : IDbParameterizable
{
    [CompareField("entry_id")]
    public int EntryId { get; set; }
    [CompareField("work_user_id")]
    public int? WorkUserId { get; set; } = null;
    [CompareField("note")]
    public string Note { get; set; } = string.Empty;

    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "ENTRY_ID", EntryId },
            { "WORK_USER_ID", WorkUserId },
            { "NOTE", Note },
        };
    }
}
