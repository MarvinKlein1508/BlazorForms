using FluentValidation;
using FormPortal.Core.Models.FormElements;

namespace FormPortal.Core.Validators.Admin
{
    public class FormNumberElementValidator : FormElementValidator<FormNumberElement>
    {
        public FormNumberElementValidator() : base()
        {
            RuleFor(x => x.MinValue)
                .Must((x, y) => x.MaxValue >= x.MinValue)
                .WithMessage("Der Mindestwert kann nicht größer sein, als der Maximalwert");

            RuleFor(x => x.DecimalPlaces)
                .GreaterThanOrEqualTo(0);


            RuleFor(x => x.Value)
                .GreaterThanOrEqualTo(x => x.MinValue)
                .When(IsEntryMode);

            RuleFor(x => x.Value)
                .LessThanOrEqualTo(x => x.MaxValue)
                .When((x) => x.MaxValue > 0 && IsEntryMode(x));

            RuleFor(x => x.Value)
                .Must(IsNumberSet)
                .When(x => IsValueRequired(x) && IsEntryMode(x));
        }

        private bool IsNumberSet(FormNumberElement element, decimal number)
        {
            bool result = true;
            if(element.MinValue != 0)
            {
                result = result && number >= element.MinValue;
            }

            if(element.MaxValue != 0)
            {
                result = result && number <= element.MaxValue;
            }

            return result;
        }
    }


}
