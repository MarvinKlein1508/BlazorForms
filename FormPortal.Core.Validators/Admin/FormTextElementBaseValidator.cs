using FluentValidation;
using FormPortal.Core.Constants;
using FormPortal.Core.Models.FormElements;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FormPortal.Core.Validators.Admin
{
    public abstract class FormTextElementBaseValidator<T> : FormElementValidator<T> where T : FormTextElementBase
    {
        public FormTextElementBaseValidator() : base()
        {
            RuleFor(x => x.Value)
                .Must(ValidateValue);

            RuleFor(x => x.MinLength)
                .Must(ValidateMinLength)
                .WithMessage("Die Mindestlänge kann nicht größer sein, als der Maximallänge");

            RuleFor(x => x.RegexPattern)
                .Must(ValidateRegexPattern)
                .WithMessage("Der Reguläre Ausdruck ist ungültig");

        }

        protected bool ValidateMinLength(FormTextElementBase element, int minLength)
        {
            return element.MaxLength > minLength;
        }
        protected bool ValidateRegexPattern(string regexPattern)
        {
            if (string.IsNullOrWhiteSpace(regexPattern)) return false;

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


        public bool ValidateValue(FormTextElementBase element, string text)
        {
            bool result = true;
            if (element.MinLength > 0)
            {
                result = result && text.Length >= element.MinLength;
            }

            if (element.MaxLength > 0)
            {
                result = result && text.Length <= element.MaxLength;
            }

            if (element.IsRequired)
            {
                result = result && text.Length > 0;
            }
            else if (element.RuleType is RuleType.Required)
            {
                result = result && element.Rules.ValidateRules();
            }

            return result;
        }
    }
}
