using FluentValidation;
using FormularPortal.Core.Models;

namespace FormularPortal.Core.Validators.Admin
{
    public class FormRowValidator : AbstractValidator<FormRow>
    {
        public FormRowValidator(IValidator<FormColumn> columnValidator)
        {
            RuleForEach(x => x.Columns) 
                .SetValidator(columnValidator);
        }
    }
}
