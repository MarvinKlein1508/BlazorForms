using FluentValidation;
using FormularPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormularPortal.Core.Validators.Admin
{
    public class FormValidator : AbstractValidator<Form>
    {
        public FormValidator(IValidator<FormRow> rowValidator)
        {
            RuleForEach(x => x.Rows)
                .SetValidator(rowValidator);

            
        }
    }

    public class FormRowValidator : AbstractValidator<FormRow>
    {
        public FormRowValidator(IValidator<FormColumn> columnValidator)
        {
            RuleForEach(x => x.Columns) 
                .SetValidator(columnValidator);
        }
    }

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
