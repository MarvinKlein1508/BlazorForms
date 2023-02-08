using FluentValidation;
using FormularPortal.Core.Validators.Admin;

namespace FormularPortal.Core.Models
{
    public class FormLabelElement : FormElement
    {
        public override ElementType GetElementType() => ElementType.Label;
        public override string GetDefaultName() => "Label";
    }
}
