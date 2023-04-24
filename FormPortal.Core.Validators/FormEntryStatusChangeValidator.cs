using FluentValidation;
using FormPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormPortal.Core.Validators
{
    public class FormEntryStatusChangeValidator : AbstractValidator<FormEntryStatusChange>
    {
        public FormEntryStatusChangeValidator()
        {
            RuleFor(x => x.StatusId)
                .GreaterThan(0)
                .WithMessage("Bitte wählen Sie einen Status aus.");
        }
    }
}
