using FluentValidation;
using BlazorForms.Core.Models;

namespace BlazorForms.Core.Validators.Admin
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
