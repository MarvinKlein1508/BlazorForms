using BlazorForms.Core.Models.FormElements;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;

namespace BlazorForms.Core.Validators.Admin
{
    public class FormNumberElementValidator : FormElementValidator<FormNumberElement>
    {
        public FormNumberElementValidator(IStringLocalizer<FormNumberElement> localizer) : base(localizer)
        {
            RuleFor(x => x.MinValue)
                .Must((x, y) => x.MaxValue >= x.MinValue)
                .WithMessage(_localizer["VALIDATION_MIN_MAX"])
                .When(x => x.MaxValue != 0);

            RuleFor(x => x.DecimalPlaces)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(5);

            RuleFor(x => x.Value)
                .Custom(ValidateValue)
                .When(IsEntryMode);
        }

        public void ValidateValue(decimal number, ValidationContext<FormNumberElement> context)
        {
            FormNumberElement element = context.InstanceToValidate;
            if (IsValueRequired(element) && number is 0)
            {
                context.AddFailure(new ValidationFailure(context.PropertyPath, String.Format(_localizer["VALIDATION_REQUIRED"], element.Name)));
            }
            else if (element.MinValue > 0 && number < element.MinValue)
            {
                context.AddFailure(new ValidationFailure(context.PropertyPath, String.Format(_localizer["VALIDATION_LESS_THAN_MIN_VALUE"], element.Name, element.MinValue)));
            }
            else if (element.MaxValue > 0 && number > element.MaxValue)
            {
                context.AddFailure(new ValidationFailure(context.PropertyPath, String.Format(_localizer["VALIDATION_GREATER_THAN_MAX_VALUE"], element.Name, element.MaxValue)));
            }

            if (number > 9999999999)
            {
                context.AddFailure(new ValidationFailure(context.PropertyPath, String.Format(_localizer["VALIDATION_VALUE_LIMIT"], element.Name)));
            }
        }
    }


}
