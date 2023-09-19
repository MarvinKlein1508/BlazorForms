using FluentValidation;
using BlazorForms.Core.Models;

namespace BlazorForms.Core.Validators.Admin
{
    public class FormRowValidator : AbstractValidator<FormRow>
    {
        public FormRowValidator(IValidator<FormColumn> formColumnValidator)
        {
            RuleForEach(x => x.Columns)
                .SetValidator(formColumnValidator);
        }
    }
}
