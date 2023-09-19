using BlazorForms.Core.Models.FormElements;
using Microsoft.Extensions.Localization;

namespace BlazorForms.Core.Validators.Admin
{
    public class FormRadioElementValidator : FormElementWithOptionsValidator<FormRadioElement>
    {
        public FormRadioElementValidator(IStringLocalizer<FormRadioElement> localizer) : base(localizer)
        {

        }
    }


}
