namespace BlazorForms.Core.Models;

public class Notification : IDbModel<int?>
{
    public int NotificationId { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    public int? UserId { get; set; } = null;
    public string? Icon { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadTimestamp { get; set; } = null;

    public int? GetIdentifier()
    {
        return NotificationId > 0 ? NotificationId : null;
    }
    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "NOTIFICATION_ID", NotificationId },
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
