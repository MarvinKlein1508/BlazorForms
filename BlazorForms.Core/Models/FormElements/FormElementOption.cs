namespace BlazorForms.Core.Models.FormElements;

public class FormElementOption : IDbModel<int?>
{
    public int ElementOptionId { get; set; }
    public int ElementId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsDefaultValue { get; set; }
    public int? GetIdentifier()
    {
        return ElementOptionId > 0 ? ElementOptionId : null;
    }
    public virtual Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "ELEMENT_OPTION_ID", ElementOptionId },
            { "ELEMENT_ID", ElementId },
            { "NAME", Name },
            { "IS_DEFAULT_VALUE", IsDefaultValue },
        };
    }
}
