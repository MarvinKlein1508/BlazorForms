using FluentValidation;
using FormularPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormularPortal.Core.Validators.Admin
{
    public class FormTextElementValidator : FormElementValidator<FormTextElement>
    {
        public FormTextElementValidator()
        {
            RuleFor(x => x.MinLength)
                .GreaterThan(10);
        }
    }

    public abstract class FormElementValidator<T> : AbstractValidator<T> where T : FormElement
    {

    }
}
