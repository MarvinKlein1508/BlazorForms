using FluentValidation;
using FormularPortal.Core.Validators.Admin;

namespace FormularPortal.Core.Models
{
    public class FormRadioElement : FormElementWithOptions
    {
        public override ElementType GetElementType() => ElementType.Radio;
        public override string GetDefaultName() => "Radio";
    }
}
