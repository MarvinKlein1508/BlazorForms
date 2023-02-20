using FluentValidation;
using FormPortal.Core.Models.FormElements;

namespace FormPortal.Core.Validators.Admin
{
    public class FormDateElementValidator : FormElementValidator<FormDateElement>
    {
        public FormDateElementValidator() : base()
        {
            RuleFor(x => x.Value)
                .GreaterThanOrEqualTo(x => x.MinDate.Date)
                .When(IsEntryMode);

            RuleFor(x => x.Value)
                .LessThanOrEqualTo(x => x.MaxDate.Date)
                .When(IsEntryMode);

            RuleFor(x => x.Value)
                .Must(IsDateSet)
                .When(IsEntryMode);
        }

        public bool IsDateSet(FormDateElement element, DateTime date)
        {
            return date != default;
        }
    }


}
