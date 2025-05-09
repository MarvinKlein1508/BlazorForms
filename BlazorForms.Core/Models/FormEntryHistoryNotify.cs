using BlazorForms.Core.Interfaces;

namespace BlazorForms.Core.Models;

public class FormEntryHistoryNotify : IDbParameterizable
{
    public int HistoryId { get; set; }
    public int UserId { get; set; }
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
