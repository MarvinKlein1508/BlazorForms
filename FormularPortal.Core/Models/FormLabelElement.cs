using FluentValidation;
using FormularPortal.Core.Validators.Admin;

namespace FormularPortal.Core.Models
{
    public class FormLabelElement : FormElement
    {
        private static FormLabelElementValidator _validator = new();
        public override ElementType GetElementType() => ElementType.Label;

        public override IValidator GetValidator() => _validator;
    }
}
