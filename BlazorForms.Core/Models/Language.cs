using BlazorForms.Core.Interfaces;

namespace BlazorForms.Core.Models;

public class Language : IDbModelWithName<int?>
{
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool Status { get; set; }

    public int? GetIdentifier()
    {
        return LanguageId > 0 ? LanguageId : null;
    }

    public string GetName()
    {
        throw new NotImplementedException();
    }

    public Dictionary<string, object?> GetParameters()
    {
        throw new NotImplementedException();
    }
}
