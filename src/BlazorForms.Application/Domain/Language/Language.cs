namespace BlazorForms.Application.Domain;

public class Language : IDbModel<int?>
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
}
