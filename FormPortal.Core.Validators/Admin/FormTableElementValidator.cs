using FluentValidation;
using FormPortal.Core.Models.FormElements;

namespace FormPortal.Core.Validators.Admin
{
    public class FormTableElementValidator : FormElementValidator<FormTableElement>
    {
        public FormTableElementValidator() : base()
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
                    x.Add(new FormTextareaElementValidator());
                    x.Add(new FormTextElementValidator());
                })
               .When(x => !IsEntryMode(x));

            RuleForEach(x => x.ElementValues)
                 .ForEach(x =>
                 {
                     x.SetInheritanceValidator(x =>
                     {

                         x.Add(new FormCheckboxElementValidator());
                         x.Add(new FormDateElementValidator());
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
        }
    }
}



