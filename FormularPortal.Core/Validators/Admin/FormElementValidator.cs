using FluentValidation;
using FormularPortal.Core.Models;

namespace FormularPortal.Core.Validators.Admin
{
    public abstract class FormElementValidator<T> : AbstractValidator<T> where T : FormElement
    {
        public FormElementValidator() : base()
        {
            RuleFor(x => x.Name)
                .Must(IsNameSet)
                .WithMessage("Bitte geben Sie dem Feld einen Namen");

        }

        protected bool IsNameSet(FormElement element, string name)
        {
            return element.GetDefaultName() != name;
        }
    }
}
