using BlazorForms.Core.Models;
using FluentValidation;

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
