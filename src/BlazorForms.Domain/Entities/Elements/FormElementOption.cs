namespace BlazorForms.Domain.Entities.Elements;

public sealed class FormElementOption
{
    public int ElementOptionId { get; set; }
    public int ElementId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsDefaultValue { get; set; }
}
