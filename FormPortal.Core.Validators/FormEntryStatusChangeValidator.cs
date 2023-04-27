using FluentValidation;
using FormPortal.Core.Models;

namespace FormPortal.Core.Validators
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
