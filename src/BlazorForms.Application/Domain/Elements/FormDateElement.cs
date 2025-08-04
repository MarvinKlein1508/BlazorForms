namespace BlazorForms.Application.Domain.Elements;

public sealed class FormDateElement : FormElementBase
{
    public bool SetDefaultValueToCurrentDate { get; set; }
    public DateTime MinDate { get; set; }
    public DateTime MaxDate { get; set; }
    public override FormElementType GetElementType() => FormElementType.Date;
}