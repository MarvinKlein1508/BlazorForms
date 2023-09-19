using FluentValidation;
using BlazorForms.Core.Models;
using BlazorForms.Core.Validators.Admin;

namespace BlazorForms.Core.Validators
{
    public class FormEntryValidator : AbstractValidator<FormEntry>
    {
        public FormEntryValidator(IValidator<Form> formValidator)
        {
            RuleFor(x => x.Form)
                .SetValidator(formValidator);

            RuleFor(x => x.Name)
                .MaximumLength(50)
                .NotEmpty()
                .NotNull();
        }
    }


}
