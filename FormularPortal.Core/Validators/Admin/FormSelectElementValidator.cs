using FluentValidation;
using FormularPortal.Core.Models;

namespace FormularPortal.Core.Validators.Admin
{
    public class FormSelectElementValidator : FormElementValidator<FormSelectElement>
    {
        public FormSelectElementValidator() : base()
        {
            RuleFor(x => x.Options)
                .Must(x => x.Any())
                .WithMessage("Bitte geben Sie dem Element mindestens eine Option");
        }
    }


}
