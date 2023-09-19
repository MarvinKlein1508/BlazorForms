using FluentValidation;
using FluentValidation.Results;
using BlazorForms.Core.Models.FormElements;
using Microsoft.Extensions.Localization;

namespace BlazorForms.Core.Validators.Admin
{
    public class FormCheckboxElementValidator : FormElementValidator<FormCheckboxElement>
    {
        private readonly IStringLocalizer<FormCheckboxElement> _localizer;

        public FormCheckboxElementValidator(IStringLocalizer<FormCheckboxElement> localizer) : base()
        {
            _localizer = localizer;

            RuleFor(x => x.Value)
                .Custom(ValidateValue)
                .When(IsEntryMode);
        }

        private void ValidateValue(bool value, ValidationContext<FormCheckboxElement> context)
        {
            FormCheckboxElement element = context.InstanceToValidate;
            if (IsValueRequired(element) && !value)
            {
                context.AddFailure(new ValidationFailure(context.PropertyPath, String.Format(_localizer["VALIDATION_REQUIRED"], element.Name)));
            }
        }


    }


}
