using FluentValidation;
using FormularPortal.Core.Validators.Admin;

namespace FormularPortal.Core.Models
{
    public class FormCheckboxElement : FormElement
    {
        private static FormCheckboxElementValidator _validator = new();
        public override ElementType GetElementType() => ElementType.Checkbox;
        public override string GetDefaultName() => "Checkbox";
        public override IValidator GetValidator() => _validator;
    }
}
