using FluentValidation;
using FormularPortal.Core.Models;

namespace FormularPortal.Core.Validators.Admin
{
    public class FormDateElementValidator : FormElementValidator<FormDateElement>
    {
        public FormDateElementValidator() : base()
        {
            RuleFor(x => x.MinDate)
                .GreaterThan(DateTime.Today.Date);
        }
    }


}
