using FluentValidation;
using FormularPortal.Core.Validators.Admin;

namespace FormularPortal.Core.Models
{
    public class FormRadioElement : FormElementWithOptions
    {
        private static FormRadioElementValidator _validator = new();
        public override ElementType GetElementType() => ElementType.Radio;

        public override IValidator GetValidator() => _validator;
    }
}
