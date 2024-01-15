using FluentValidation;
using BlazorForms.Core.Models;

namespace BlazorForms.Core.Validators.Admin
{
    public class FormValidator : AbstractValidator<Form>
    {
        public FormValidator(IValidator<FormRow> rowValidator)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(5)
                .MaximumLength(50);

            RuleFor(x => x.DefaultName)
                .MaximumLength(50);

            RuleFor(x => x.DefaultStatusId)
                .GreaterThan(0)
                .WithMessage("Bitte wählen Sie einen Status aus.");

            RuleForEach(x => x.Rows)
                .SetValidator(rowValidator);
        }
    }
}
