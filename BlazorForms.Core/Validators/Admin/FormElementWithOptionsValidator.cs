﻿using BlazorForms.Core.Models.FormElements;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;

namespace BlazorForms.Core.Validators.Admin
{
    public abstract class FormElementWithOptionsValidator<T> : FormElementValidator<T> where T : FormElementWithOptions
    {
        public FormElementWithOptionsValidator(IStringLocalizer<T> localizer) : base(localizer)
        {
            RuleFor(x => x.Options)
                .Must(x => x.Count != 0)
                .WithMessage(_localizer["VALIDATION_NO_OPTIONS"]);

            RuleFor(x => x.Value)
                .Custom(ValidateValue)
                .When(IsEntryMode);
        }

        protected virtual void ValidateValue(string value, ValidationContext<T> context)
        {
            T element = context.InstanceToValidate;
            if (IsValueRequired(element) && value.Length is 0)
            {
                context.AddFailure(new ValidationFailure(context.PropertyPath, string.Format(_localizer["VALIDATION_REQUIRED"], element.Name)));
            }
        }
    }
}
