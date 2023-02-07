using FluentValidation;
using FormularPortal.Core.Models;

namespace FormularPortal.Core.Validators.Admin
{
    public class FormRadioElementValidator : FormElementValidator<FormRadioElement>
    {
        public FormRadioElementValidator() : base()
        {
            RuleFor(x => x.Options)
               .Must(x => x.Any())
               .WithMessage("Bitte geben Sie dem Element mindestens eine Option");
        }
    }


}
