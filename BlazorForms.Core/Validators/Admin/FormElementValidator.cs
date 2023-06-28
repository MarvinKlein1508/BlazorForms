using FluentValidation;
using BlazorForms.Core.Constants;
using BlazorForms.Core.Extensions;
using BlazorForms.Core.Models.FormElements;

namespace BlazorForms.Core.Validators.Admin
{
    public abstract class FormElementValidator<T> : AbstractValidator<T> where T : FormElement
    {
        public FormElementValidator() : base()
        {
            RuleFor(x => x.Name)
                .Must(IsNameSet)
                .WithMessage(x => $"Bitte geben Sie dem Feld '{x.Name}' einen Namen");

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
            if (element.RuleType is RuleType.Required or RuleType.VisibleRequired && element.Rules.ValidateRules())
            {
                return true;
            }

            // Validate Column rules
            if (element.Parent is not null)
            {
                if (element.Parent.RuleType is RuleType.Required or RuleType.VisibleRequired && element.Parent.Rules.ValidateRules())
                {
                    return true;
                }

                // Validate Row rules
                if (element.Parent.Parent is not null && element.Parent.Parent.RuleType is RuleType.Required or RuleType.VisibleRequired && element.Parent.Parent.Rules.ValidateRules())
                {
                    return true;
                }
            }


            return false;
        }

        protected bool IsEntryMode(FormElement element)
        {
            if (element.Form is null)
            {
                return false;
            }

            return element.Form.EntryMode;
        }
    }
}
