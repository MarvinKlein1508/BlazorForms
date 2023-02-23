using FluentValidation;
using FormPortal.Core.Models;
using FormPortal.Core.Validators.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormPortal.Core.Validators
{
    public class FormEntryValidator : AbstractValidator<FormEntry>
    {
        public FormEntryValidator()
        {
            RuleFor(x => x.Form)
                .SetValidator(new FormValidator());
        }
    }

    
}
