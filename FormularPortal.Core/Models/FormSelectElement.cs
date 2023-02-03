using FluentValidation;
using FormularPortal.Core.Validators.Admin;

namespace FormularPortal.Core.Models
{
    public class FormSelectElement : FormElementWithOptions
    {
        private static FormSelectElementValidator _validator = new();
        public override ElementType GetElementType() => ElementType.Select;

        public override IValidator GetValidator() => _validator;
    }
}
