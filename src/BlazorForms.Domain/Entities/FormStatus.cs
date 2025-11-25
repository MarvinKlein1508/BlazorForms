namespace BlazorForms.Domain.Entities;

public class FormStatus : LocalizationModelBase<FormStatusDescription>, IDbModel<int?>, IDbParameterizable
{
    public int StatusId { get; set; }
    public bool RequiresApproval { get; set; }
    public bool IsCompleted { get; set; }
    public int SortOrder { get; set; }
    public int? GetIdentifier() => StatusId > 0 ? StatusId : null;
    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "STATUS_ID", StatusId },
            { "REQUIRES_APPROVAL", RequiresApproval },
            { "IS_COMPLETED", IsCompleted },
            { "SORT_ORDER", SortOrder }
        };
    }
}
