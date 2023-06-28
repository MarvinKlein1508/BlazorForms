using FluentValidation;
using BlazorForms.Core.Models;
using BlazorForms.Core.Validators.Admin;

namespace BlazorForms.Core.Validators
{
    public class FormEntryValidator : AbstractValidator<FormEntry>
    {
        public FormEntryValidator()
        {
            RuleFor(x => x.Form)
                .SetValidator(new FormValidator());

            RuleFor(x => x.Name)
                .MaximumLength(50)
                .NotEmpty()
                .NotNull();
        }
    }


}
