using FluentValidation;
using FormPortal.Core.Constants;
using FormPortal.Core.Models.FormElements;

namespace FormPortal.Core.Validators.Admin
{
    public abstract class FormElementValidator<T> : AbstractValidator<T> where T : FormElement
    {
        public FormElementValidator() : base()
        {
            RuleFor(x => x.Name)
                .Must(IsNameSet)
                .WithMessage("Bitte geben Sie dem Feld einen Namen");

        }

        private bool IsNameSet(FormElement element, string name)
        {
            return element.GetDefaultName() != name;
        }

        protected bool IsValueRequired(FormElement element)
        {
            if (!element.IsActive)
            {
                return false;
            }

            if (element.IsRequired)
            {
                return true;
            }

            // ValidateRuleSets
            if (element.RuleType is RuleType.Required && element.Rules.ValidateRules())
            {
                return true;
            }

            return false;
        }

        protected bool IsEntryMode(FormElement element)
        {
            if(element.Form is null)
            {
                return false;
            }

            return element.Form.EntryMode;
        }
    }
}
