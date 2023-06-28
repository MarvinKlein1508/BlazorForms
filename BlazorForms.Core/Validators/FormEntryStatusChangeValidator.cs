using FluentValidation;
using BlazorForms.Core.Models;

namespace BlazorForms.Core.Validators
{
    public class FormEntryStatusChangeValidator : AbstractValidator<FormEntryStatusChange>
    {
        public FormEntryStatusChangeValidator()
        {
            RuleFor(x => x.StatusId)
                .GreaterThan(0)
                .WithMessage("Bitte wählen Sie einen Status aus.");
        }
    }
}
