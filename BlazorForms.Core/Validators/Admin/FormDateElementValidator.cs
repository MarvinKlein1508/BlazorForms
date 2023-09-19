using FluentValidation;
using FluentValidation.Results;
using BlazorForms.Core.Models.FormElements;
using Microsoft.Extensions.Localization;

namespace BlazorForms.Core.Validators.Admin
{
    public class FormDateElementValidator : FormElementValidator<FormDateElement>
    {
        public FormDateElementValidator(IStringLocalizer<FormDateElement> localizer) : base(localizer)
        {
            RuleFor(x => x.MaxDate)
                .Must((x, y) => x.MaxDate.Date >= x.MinDate.Date)
                .WithMessage(_localizer["VALIDATION_MIN_MAX"])
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
                context.AddFailure(new ValidationFailure(context.PropertyPath, String.Format(_localizer["VALIDATION_REQUIRED"], element.Name)));
            }
            else if (element.MinDate != default && date.Date < element.MinDate.Date)
            {
                context.AddFailure(new ValidationFailure(context.PropertyPath, String.Format(_localizer["VALIDATION_NOT_MIN_DATE"], element.Name, element.MinDate.ToShortDateString())));
            }
            else if (element.MaxDate != default && date.Date > element.MaxDate.Date)
            {
                context.AddFailure(new ValidationFailure(context.PropertyPath, String.Format(_localizer["VALIDATION_OVER_MAX_DATE"], element.Name, element.MaxDate.ToShortDateString())));
            }
        }
    }


}
