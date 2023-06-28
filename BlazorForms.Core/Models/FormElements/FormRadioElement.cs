using BlazorForms.Core.Constants;

namespace BlazorForms.Core.Models.FormElements
{
    public class FormRadioElement : FormElementWithOptions
    {
        public override ElementType GetElementType() => ElementType.Radio;
        public override string GetDefaultName() => "Radio";
    }
}
