using FluentValidation;
using FormPortal.Core.Models;

namespace FormPortal.Core.Validators.Admin
{
    public class FormStatusValidator : AbstractValidator<FormStatus>
    {
        public FormStatusValidator()
        {
            RuleForEach(x => x.Description)
                .SetValidator(new FormStatusDescriptionValidator());
        }
    }

    public class FormStatusDescriptionValidator : AbstractValidator<FormStatusDescription>
    {
        public FormStatusDescriptionValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}
