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
        }
    }


}
