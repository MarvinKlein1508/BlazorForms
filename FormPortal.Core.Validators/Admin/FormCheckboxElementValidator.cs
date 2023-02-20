using FluentValidation;
using FormPortal.Core.Constants;
using FormPortal.Core.Models.FormElements;

namespace FormPortal.Core.Validators.Admin
{
    public class FormCheckboxElementValidator : FormElementValidator<FormCheckboxElement>
    {
        public FormCheckboxElementValidator()
        {
            RuleFor(x => x.Value)
                .Must(x => true)
                .WithMessage(x => $"{x.Name} muss ausgewählt werden.")
                .When(IsValueRequired);
        }

        
    }


}
