using FluentValidation;
using FluentValidation.Results;
using BlazorForms.Core.Models.FormElements;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Localization;

namespace BlazorForms.Core.Validators.Admin
{
    public abstract class FormTextElementBaseValidator<T> : FormElementValidator<T> where T : FormTextElementBase
    {
        public FormTextElementBaseValidator(IStringLocalizer<T> localizer) : base(localizer)
        {
            RuleFor(x => x.Value)
                .Custom(ValidateValue)
                .When(IsEntryMode);

            RuleFor(x => x.MinLength)
                .Must(ValidateMinLength)
                .WithMessage("Die Mindestlänge kann nicht größer sein, als der Maximallänge");

            RuleFor(x => x.RegexPattern)
                .Must(ValidateRegexPattern)
                .WithMessage("Der Reguläre Ausdruck ist ungültig");

        }

        protected bool ValidateMinLength(FormTextElementBase element, int minLength)
        {
            if (element.MaxLength is 0)
            {
                return true;
            }

            return element.MaxLength >= minLength;
        }
        protected bool ValidateRegexPattern(string regexPattern)
        {
            if (string.IsNullOrWhiteSpace(regexPattern)) return true;

            try
            {
                Regex.Match("", regexPattern);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }


        public void ValidateValue(string text, ValidationContext<T> context)
        {
            T element = context.InstanceToValidate;
            if (IsValueRequired(element) && text.Length is 0)
            {
                context.AddFailure(new ValidationFailure(context.PropertyPath, $"{element.Name} darf nicht leer sein."));
            }
            else if (element.MinLength > 0 && text.Length < element.MinLength)
            {
                context.AddFailure(new ValidationFailure(context.PropertyPath, $"{element.Name} muss mindestens {element.MinLength} Zeichen lang sein. Sie haben {text.Length} Zeichen eingegeben."));
            }
            else if (element.MaxLength > 0 && text.Length > element.MaxLength)
            {
                context.AddFailure(new ValidationFailure(context.PropertyPath, $"{element.Name} kann maximal {element.MaxLength} Zeichen lang sein. Sie haben {text.Length} Zeichen eingegeben."));
            }

            if (element is FormTextElementBase textElement && !string.IsNullOrWhiteSpace(textElement.RegexPattern) && !string.IsNullOrWhiteSpace(text))
            {
                try
                {
                    var match = Regex.Match(text, textElement.RegexPattern);

                    if(!match.Success)
                    {
                        context.AddFailure(new ValidationFailure(context.PropertyPath, $"Die Eingabe hat ein falsches Format."));
                    }
                }
                catch (ArgumentException)
                {
                    context.AddFailure(new ValidationFailure(context.PropertyPath, $"Ungültiger Regulärer Ausdruck in Element."));
                }
            }
        }
    }
}
