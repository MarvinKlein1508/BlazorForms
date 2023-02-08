using FluentValidation;
using FormularPortal.Core.Validators.Admin;

namespace FormularPortal.Core.Models
{
    public class FormCheckboxElement : FormElement
    {
        public override ElementType GetElementType() => ElementType.Checkbox;
        public override string GetDefaultName() => "Checkbox";
    }
}
