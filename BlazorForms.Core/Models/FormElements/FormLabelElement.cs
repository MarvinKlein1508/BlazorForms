using BlazorForms.Core.Constants;

namespace BlazorForms.Core.Models.FormElements;

public class FormLabelElement : FormElement
{
    public string Description { get; set; } = string.Empty;
    public bool ShowOnPdf { get; set; } = true;
    public override ElementType GetElementType() => ElementType.Label;
    public override string GetDefaultName() => "Label";

    public override Dictionary<string, object?> GetParameters()
    {
        var parameters = base.GetParameters();

        parameters.Add("DESCRIPTION", Description);
        parameters.Add("SHOW_ON_PDF", ShowOnPdf);

        return parameters;
    }

    public override void Reset()
    {
        // This element has nothing to be resetted
    }

    public override void SetValue(FormEntryElement element)
    {
        // No value to be set
    }

    public override object Clone()
    {
        return this.MemberwiseClone();
    }
}
