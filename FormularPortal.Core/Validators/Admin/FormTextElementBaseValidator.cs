using FluentValidation;
using FormularPortal.Core.Models;
using System.Text.RegularExpressions;

namespace FormularPortal.Core.Validators.Admin
{
    public abstract class FormTextElementBaseValidator<T> : FormElementValidator<FormTextElementBase> where T : FormTextElementBase
    {
        public FormTextElementBaseValidator() : base()
        {
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
    }
}
