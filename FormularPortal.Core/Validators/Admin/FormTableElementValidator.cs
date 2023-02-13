using FluentValidation;
using FormularPortal.Core.Models;

namespace FormularPortal.Core.Validators.Admin
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
                });
        }
    }


}
