using BlazorForms.Core.Interfaces;

namespace BlazorForms.Core.Models;

public class FormStatus : LocalizationModelBase<FormStatusDescription>, ILocalizedDbModel<int?>
{
    public int StatusId { get; set; }
    public bool RequiresApproval { get; set; }
    public bool IsCompleted { get; set; }
    public int SortOrder { get; set; }

    public int? GetIdentifier()
    {
        return StatusId > 0 ? StatusId : null;
    }

    public IEnumerable<Dictionary<string, object?>> GetLocalizedParameters()
    {
        foreach (var item in Description)
        {
            yield return new Dictionary<string, object?>
            {
                { "STATUS_ID", StatusId },
                { "CODE", item.Code },
                { "NAME", item.Name },
                { "DESCRIPTION", item.Description }
            };
        }
    }

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

public class FormStatusDescription : ILocalizationHelper
{
    public int StatusId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
