using FluentValidation;
using FluentValidation.Results;
using FormPortal.Core.Models.FormElements;

namespace FormPortal.Core.Validators.Admin
{
    public class FormNumberElementValidator : FormElementValidator<FormNumberElement>
    {
        public FormNumberElementValidator() : base()
        {
            RuleFor(x => x.MinValue)
                .Must((x, y) => x.MaxValue >= x.MinValue)
                .WithMessage("Der Mindestwert kann nicht größer sein, als der Maximalwert.")
                .When(x => x.MaxValue != 0);

            RuleFor(x => x.DecimalPlaces)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Value)
                .Custom(ValidateValue)
                .When(IsEntryMode);
        }

        public void ValidateValue(decimal number, ValidationContext<FormNumberElement> context)
        {
            FormNumberElement element = context.InstanceToValidate;
            if (IsValueRequired(element) && number is 0)
            {
                context.AddFailure(new ValidationFailure(context.PropertyName, $"{element.Name} darf nicht 0 sein."));
            }
            else if (element.MinValue > 0 && number < element.MinValue)
            {
                context.AddFailure(new ValidationFailure(context.PropertyName, $"{element.Name} muss mindestens den Wert {element.MinValue} betragen."));
            }
            else if (element.MaxValue > 0 && number > element.MaxValue)
            {
                context.AddFailure(new ValidationFailure(context.PropertyName, $"{element.Name} kann maximal den Wert {element.MaxValue} betragen."));
            }
        }
    }


}
