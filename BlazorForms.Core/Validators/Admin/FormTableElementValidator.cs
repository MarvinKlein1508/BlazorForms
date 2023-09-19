using FluentValidation;
using BlazorForms.Core.Models.FormElements;
using Microsoft.Extensions.Localization;

namespace BlazorForms.Core.Validators.Admin
{
    public class FormTableElementValidator : FormElementValidator<FormTableElement>
    {
        public FormTableElementValidator
        (
            IStringLocalizer<FormCheckboxElement> checkboxLocalizer,
            IStringLocalizer<FormDateElement> dateLocalizer
        ) : base()
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
                    x.Add(new FormTextareaElementValidator());
                    x.Add(new FormTextElementValidator());
                })
               .When(x => !IsEntryMode(x));

            RuleForEach(x => x.ElementValues)
                 .ForEach(x =>
                 {
                     x.SetInheritanceValidator(x =>
                     {

                         x.Add(new FormCheckboxElementValidator(checkboxLocalizer));
                         x.Add(new FormDateElementValidator(dateLocalizer));
                         x.Add(new FormFileElementValidator());
                         x.Add(new FormLabelElementValidator());
                         x.Add(new FormNumberElementValidator());
                         x.Add(new FormRadioElementValidator());
                         x.Add(new FormSelectElementValidator());
                         x.Add(new FormTextareaElementValidator());
                         x.Add(new FormTextElementValidator());
                     });
                 })
                 .When(IsEntryMode);

            RuleFor(x => x.Elements)
                .Must(x => x.Any())
                .WithMessage(x => $"Tabelle '{x.Name}' muss mindestens ein Element beinhalten");
        }
    }
}



