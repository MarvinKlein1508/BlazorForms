using FluentValidation;
using FormPortal.Core.Models;

namespace FormPortal.Core.Validators.Admin
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
