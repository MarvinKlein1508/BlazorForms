using FluentValidation;
using FluentValidation.Results;
using BlazorForms.Core.Models.FormElements;
using Microsoft.Extensions.Localization;

namespace BlazorForms.Core.Validators.Admin
{
    public class FormSignaturePadElementValidator : FormElementValidator<FormSignaturePadElement>
    {
        public FormSignaturePadElementValidator(IStringLocalizer<FormSignaturePadElement> localizer) : base(localizer)
        {

            RuleFor(x => x.Value)
                .Custom(ValidateValue)
                .When(IsEntryMode);
        }

        public void ValidateValue(byte[] signature, ValidationContext<FormSignaturePadElement> context)
        {
            FormSignaturePadElement element = context.InstanceToValidate;
            if (IsValueRequired(element) && signature.Length is 0)
            {
                context.AddFailure(new ValidationFailure(context.PropertyPath, String.Format(_localizer["VALIDATION_REQUIRED"], element.Name)));
            }
        }
    }


}
