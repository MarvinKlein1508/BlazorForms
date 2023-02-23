using FluentValidation;
using FormPortal.Core.Models.FormElements;

namespace FormPortal.Core.Validators.Admin
{
    public class FormTextElementValidator : FormTextElementBaseValidator<FormTextElement>
    {
        public FormTextElementValidator() : base()
        {
            RuleFor(x => x.Value)
                .Must(ValidateValue);
        }
    }
}
