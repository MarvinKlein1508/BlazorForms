using BlazorForms.Core.Models.FormElements;
using Microsoft.Extensions.Localization;

namespace BlazorForms.Core.Validators.Admin
{
    public class FormTextareaElementValidator : FormTextElementBaseValidator<FormTextareaElement>
    {
        public FormTextareaElementValidator(IStringLocalizer<FormTextareaElement> localizer) : base(localizer)
        {

        }
    }


}
