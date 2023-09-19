using FluentValidation;
using BlazorForms.Core.Models;
using Microsoft.Extensions.Localization;
using BlazorForms.Core.Models.FormElements;

namespace BlazorForms.Core.Validators.Admin
{
    public class FormColumnValidator : AbstractValidator<FormColumn>
    {
        public FormColumnValidator(
            IStringLocalizer<FormCheckboxElement> checkboxLocalizer,
            IStringLocalizer<FormDateElement> dateLocalizer
        )
        {
            RuleForEach(x => x.Elements)
                .SetInheritanceValidator(x =>
                {
                    x.Add(new FormCheckboxElementValidator(checkboxLocalizer));
                    x.Add(new FormDateElementValidator(dateLocalizer));
                    x.Add(new FormFileElementValidator());
                    x.Add(new FormLabelElementValidator());
                    x.Add(new FormNumberElementValidator());
                    x.Add(new FormRadioElementValidator());
                    x.Add(new FormSelectElementValidator());
                    x.Add(new FormTableElementValidator(checkboxLocalizer, dateLocalizer));
                    x.Add(new FormTextareaElementValidator());
                    x.Add(new FormTextElementValidator());
                });
        }
    }
}
