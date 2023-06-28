using FluentValidation;
using FluentValidation.Results;
using BlazorForms.Core.Models.FormElements;

namespace BlazorForms.Core.Validators.Admin
{
    public abstract class FormElementWithOptionsValidator<T> : FormElementValidator<T> where T : FormElementWithOptions
    {
        public FormElementWithOptionsValidator() : base()
        {
            RuleFor(x => x.Options)
                .Must(x => x.Any())
                .WithMessage("Bitte geben Sie dem Element mindestens eine Option");

            RuleFor(x => x.Value)
                .Custom(ValidateValue)
                .When(IsEntryMode);
        }

        private void ValidateValue(string value, ValidationContext<T> context)
        {
            T element = context.InstanceToValidate;
            if (IsValueRequired(element) && value.Length is 0)
            {
                context.AddFailure(new ValidationFailure(context.PropertyName, $"{element.Name} erfordert eine ausgewählte Option."));
            }
        }
    }
}
