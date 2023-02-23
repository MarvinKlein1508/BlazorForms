using FluentValidation;
using FluentValidation.Results;
using FormPortal.Core.Constants;
using FormPortal.Core.Models.FormElements;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Numerics;

namespace FormPortal.Core.Validators.Admin
{
    public class FormDateElementValidator : FormElementValidator<FormDateElement>
    {
        public FormDateElementValidator() : base()
        {
            RuleFor(x => x.MaxDate)
                .Must((x, y) => x.MaxDate.Date >= x.MinDate.Date)
                .WithMessage("Der Mindestwert kann nicht größer sein, als der Maximalwert.")
                .When(x => x.MaxDate != default);

            RuleFor(x => x.Value)
                .Custom(ValidateValue)
                .When(IsEntryMode);
        }

        public void ValidateValue(DateTime date, ValidationContext<FormDateElement> context)
        {
            FormDateElement element = context.InstanceToValidate;
            if (IsValueRequired(element) && date == default)
            {
                context.AddFailure(new ValidationFailure(context.PropertyName, $"Bitte geben Sie für {element.Name} ein Datum an."));
            }
            else if (element.MinDate != default && date.Date < element.MinDate.Date)
            {
                context.AddFailure(new ValidationFailure(context.PropertyName, $"{element.Name} muss größer oder gleich {element.MinDate.ToShortDateString()} sein."));
            }
            else if (element.MaxDate != default && date.Date > element.MaxDate.Date)
            {
                context.AddFailure(new ValidationFailure(context.PropertyName, $"{element.Name} muss kleiner oder gleich {element.MaxDate.ToShortDateString()} sein."));
            }
        }
    }


}
