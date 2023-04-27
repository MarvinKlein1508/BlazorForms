using FluentValidation;
using FluentValidation.Results;
using FormPortal.Core.Models.FormElements;

namespace FormPortal.Core.Validators.Admin
{
    public class FormCheckboxElementValidator : FormElementValidator<FormCheckboxElement>
    {
        public FormCheckboxElementValidator() : base()
        {
            RuleFor(x => x.Value)
                .Custom(ValidateValue)
                .When(IsEntryMode);
        }

        private void ValidateValue(bool value, ValidationContext<FormCheckboxElement> context)
        {
            FormCheckboxElement element = context.InstanceToValidate;
            if (IsValueRequired(element) && !value)
            {
                context.AddFailure(new ValidationFailure(context.PropertyName, $"{element.Name} muss ausgewählt sein."));
            }
        }


    }


}
