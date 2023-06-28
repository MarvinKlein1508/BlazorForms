using BlazorForms.Core.Constants;

namespace BlazorForms.Core.Models.FormElements
{
    public class FormSelectElement : FormElementWithOptions
    {
        public override ElementType GetElementType() => ElementType.Select;
        public override string GetDefaultName() => "Select";
    }
}
