using FluentValidation;
using FluentValidation.Results;
using BlazorForms.Core.Models.FormElements;
using Microsoft.Extensions.Localization;

namespace BlazorForms.Core.Validators.Admin
{
    public class FormFileElementValidator : FormElementValidator<FormFileElement>
    {
        public FormFileElementValidator(IStringLocalizer<FormFileElement> localizer) : base(localizer)
        {
            RuleFor(x => x.Values)
                .Custom(ValidateValue)
                .When(IsEntryMode);
        }

        public void ValidateValue(List<FormFileElementFile> value, ValidationContext<FormFileElement> context)
        {
            var element = context.InstanceToValidate;

            if (IsValueRequired(element) && value.Count == 0)
            {
                context.AddFailure(new ValidationFailure(context.PropertyPath, string.Format(_localizer["VALIDATION_REQUIRED"], element.Name)));
            }
        }
    }


}
