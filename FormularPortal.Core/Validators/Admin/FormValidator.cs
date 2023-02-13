using FluentValidation;
using FormularPortal.Core.Models;

namespace FormularPortal.Core.Validators.Admin
{
    public class FormValidator : AbstractValidator<Form>
    {
        public FormValidator(IValidator<FormRow> rowValidator)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(5);

            RuleForEach(x => x.Rows)
                .SetValidator(rowValidator);
        }
    }
}
