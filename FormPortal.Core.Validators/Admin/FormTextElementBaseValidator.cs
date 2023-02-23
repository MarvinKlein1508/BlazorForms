using FluentValidation;
using FluentValidation.Results;
using FormPortal.Core.Constants;
using FormPortal.Core.Models.FormElements;
using MySqlX.XDevAPI.Common;
using System.Configuration;
using System.Text.RegularExpressions;

namespace FormPortal.Core.Validators.Admin
{
    public abstract class FormTextElementBaseValidator<T> : FormElementValidator<T> where T : FormTextElementBase
    {
        public FormTextElementBaseValidator() : base()
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
            if (element.IsRequired && text.Length is 0)
            {
                context.AddFailure(new ValidationFailure(context.PropertyName, $"{element.Name} darf nicht leer sein."));
            }
            else if (element.RuleType is RuleType.Required && element.Rules.ValidateRules() && text.Length is 0)
            {
                context.AddFailure( new ValidationFailure(context.PropertyName, $"{element.Name} darf nicht leer sein."));
            }
            else if (element.MinLength > 0 && text.Length >= element.MinLength)
            {
                context.AddFailure(new ValidationFailure(context.PropertyName, $"{element.Name} muss mindestens {element.MinLength} Zeichen lang sein. Sie haben {text.Length} Zeichen eingegeben."));
            }
            else if (element.MaxLength > 0 && text.Length <= element.MaxLength)
            {
                context.AddFailure(new ValidationFailure(context.PropertyName, $"{element.Name} kann maximal {element.MinLength} Zeichen lang sein. Sie haben {text.Length} Zeichen eingegeben."));
            }
        }
    }
}
