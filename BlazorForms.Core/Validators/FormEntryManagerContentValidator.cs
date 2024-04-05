using BlazorForms.Core.Models;
using FluentValidation;

namespace BlazorForms.Core.Validators;

public class FormEntryManagerContentValidator : AbstractValidator<FormEntryManagerContent>
{
    public FormEntryManagerContentValidator()
    {
        RuleFor(x => x.Note)
            .MaximumLength(5000);
    }
}
