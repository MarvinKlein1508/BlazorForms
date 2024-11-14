using BlazorForms.Core.Models.FormElements;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BlazorForms.Core.Validators.Admin
{
    public class FormTableElementValidator : FormElementValidator<FormTableElement>
    {
        public FormTableElementValidator
        (
            IStringLocalizer<FormTableElement> localizer,
            IStringLocalizer<FormCheckboxElement> checkboxLocalizer,
            IStringLocalizer<FormDateElement> dateLocalizer,
            IStringLocalizer<FormFileElement> fileLocalizer,
            IStringLocalizer<FormLabelElement> labelLocalizer,
            IStringLocalizer<FormNumberElement> numberLocalizer,
            IStringLocalizer<FormRadioElement> radioLocalizer,
            IStringLocalizer<FormSelectElement> selectLocalizer,
            IStringLocalizer<FormTextareaElement> textareaLocalizer,
            IStringLocalizer<FormTextElement> textLocalizer
        ) : base(localizer)
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
                    x.Add(new FormTextareaElementValidator(textareaLocalizer));
                    x.Add(new FormTextElementValidator(textLocalizer));
                })
               .When(x => !IsEntryMode(x));

            RuleForEach(x => x.ElementValues)
                 .ForEach(x =>
                 {
                     x.SetInheritanceValidator(x =>
                     {

                         x.Add(new FormCheckboxElementValidator(checkboxLocalizer));
                         x.Add(new FormDateElementValidator(dateLocalizer));
                         x.Add(new FormFileElementValidator(fileLocalizer));
                         x.Add(new FormLabelElementValidator(labelLocalizer));
                         x.Add(new FormNumberElementValidator(numberLocalizer));
                         x.Add(new FormRadioElementValidator(radioLocalizer));
                         x.Add(new FormSelectElementValidator(selectLocalizer));
                         x.Add(new FormTextareaElementValidator(textareaLocalizer));
                         x.Add(new FormTextElementValidator(textLocalizer));
                     });
                 })
                 .When(IsEntryMode);

            RuleFor(x => x.Elements)
                .Must(x => x.Count != 0)
                .WithMessage(x => string.Format(_localizer["VALIDATION_NO_ELEMENTS"], x.Name));
        }
    }
}



