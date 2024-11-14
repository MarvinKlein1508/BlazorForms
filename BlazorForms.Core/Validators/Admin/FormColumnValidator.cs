using BlazorForms.Core.Models;
using BlazorForms.Core.Models.FormElements;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BlazorForms.Core.Validators.Admin
{
    public class FormColumnValidator : AbstractValidator<FormColumn>
    {
        public FormColumnValidator(
            IStringLocalizer<FormCheckboxElement> checkboxLocalizer,
            IStringLocalizer<FormDateElement> dateLocalizer,
            IStringLocalizer<FormFileElement> fileLocalizer,
            IStringLocalizer<FormLabelElement> labelLocalizer,
            IStringLocalizer<FormNumberElement> numberLocalizer,
            IStringLocalizer<FormRadioElement> radioLocalizer,
            IStringLocalizer<FormSelectElement> selectLocalizer,
            IStringLocalizer<FormTableElement> tableLocalizer,
            IStringLocalizer<FormTextareaElement> textareaLocalizer,
            IStringLocalizer<FormTextElement> textLocalizer
        )
        {
            RuleForEach(x => x.Elements)
                .SetInheritanceValidator(x =>
                {
                    x.Add(new FormCheckboxElementValidator(checkboxLocalizer));
                    x.Add(new FormDateElementValidator(dateLocalizer));
                    x.Add(new FormFileElementValidator(fileLocalizer));
                    x.Add(new FormLabelElementValidator(labelLocalizer));
                    x.Add(new FormNumberElementValidator(numberLocalizer));
                    x.Add(new FormRadioElementValidator(radioLocalizer));
                    x.Add(new FormSelectElementValidator(selectLocalizer));
                    x.Add(new FormTableElementValidator
                        (
                            tableLocalizer,
                            checkboxLocalizer,
                            dateLocalizer,
                            fileLocalizer,
                            labelLocalizer,
                            numberLocalizer,
                            radioLocalizer,
                            selectLocalizer,
                            textareaLocalizer,
                            textLocalizer
                        )
                    );
                    x.Add(new FormTextareaElementValidator(textareaLocalizer));
                    x.Add(new FormTextElementValidator(textLocalizer));
                });
        }
    }
}
