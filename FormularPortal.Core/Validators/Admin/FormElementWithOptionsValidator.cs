using FluentValidation;
using FormularPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormularPortal.Core.Validators.Admin
{
    public abstract class FormElementWithOptionsValidator<T> : FormElementValidator<T> where T : FormElementWithOptions
    {
        public FormElementWithOptionsValidator() : base() 
        {
            RuleFor(x => x.Options)
                .Must(x => x.Any())
                .WithMessage("Bitte geben Sie dem Element mindestens eine Option");
        }
    }
}
