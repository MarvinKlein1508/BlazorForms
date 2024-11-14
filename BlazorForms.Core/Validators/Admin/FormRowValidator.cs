using BlazorForms.Core.Models;
using FluentValidation;

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
