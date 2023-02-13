using FluentValidation;
using FormPortal.Core.Models.FormElements;

namespace FormPortal.Core.Validators.Admin
{
    public abstract class FormElementWithOptionsValidator<T> : FormElementValidator<T> where T : FormElementWithOptions
    {
        public FormElementWithOptionsValidator() : base()
        {
            RuleFor(x => x.Options)
                .Must(x => x.Any())
                .WithMessage("Bitte geben Sie dem Element mindestens eine Option");
        }
    }
}
