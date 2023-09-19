using BlazorForms.Core.Models.FormElements;
using Microsoft.Extensions.Localization;

namespace BlazorForms.Core.Validators.Admin
{
    public class FormTextElementValidator : FormTextElementBaseValidator<FormTextElement>
    {
        public FormTextElementValidator(IStringLocalizer<FormTextElement> localizer) : base(localizer)
        {

        }
    }
}
