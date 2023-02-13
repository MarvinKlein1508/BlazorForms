using FluentValidation;
using FormularPortal.Core.Validators.Admin;

namespace FormularPortal.Core.Models
{
    public class FormCheckboxElement : FormElement
    {
        public override ElementType GetElementType() => ElementType.Checkbox;
        public bool Value { get; set; }
        public override string GetDefaultName() => "Checkbox";
    }
}
