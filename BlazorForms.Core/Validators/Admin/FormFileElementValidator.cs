using FluentValidation;
using FluentValidation.Results;
using BlazorForms.Core.Models.FormElements;

namespace BlazorForms.Core.Validators.Admin
{
    public class FormFileElementValidator : FormElementValidator<FormFileElement>
    {
        public FormFileElementValidator() : base()
        {
            RuleFor(x => x.Values)
                .Custom(ValidateValue)
                .When(IsEntryMode);
        }

        public void ValidateValue(List<FormFileElementFile> value, ValidationContext<FormFileElement> context)
        {
            var element = context.InstanceToValidate;

            if (IsValueRequired(element) && !value.Any())
            {
                context.AddFailure(new ValidationFailure(context.PropertyPath, $"{element.Name} erfordert eine Datei."));
            }
        }
    }


}
