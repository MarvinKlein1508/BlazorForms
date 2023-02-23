using FluentValidation;
using FormPortal.Core.Models;

namespace FormPortal.Core.Validators.Admin
{
    public class FormValidator : AbstractValidator<Form>
    {
        public FormValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(5);

            RuleForEach(x => x.Rows)
                .SetValidator(new FormRowValidator());
        }
    }
}
