using FluentValidation;
using BlazorForms.Core.Models;

namespace BlazorForms.Core.Validators.Admin
{
    public class FormColumnValidator : AbstractValidator<FormColumn>
    {
        public FormColumnValidator()
        {
            RuleForEach(x => x.Elements)
                .SetInheritanceValidator(x =>
                {
                    x.Add(new FormCheckboxElementValidator());
                    x.Add(new FormDateElementValidator());
                    x.Add(new FormFileElementValidator());
                    x.Add(new FormLabelElementValidator());
                    x.Add(new FormNumberElementValidator());
                    x.Add(new FormRadioElementValidator());
                    x.Add(new FormSelectElementValidator());
                    x.Add(new FormTableElementValidator());
                    x.Add(new FormTextareaElementValidator());
                    x.Add(new FormTextElementValidator());
                });
        }
    }
}
