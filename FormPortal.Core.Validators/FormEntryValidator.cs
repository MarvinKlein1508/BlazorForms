using FluentValidation;
using FormPortal.Core.Models;
using FormPortal.Core.Validators.Admin;

namespace FormPortal.Core.Validators
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
