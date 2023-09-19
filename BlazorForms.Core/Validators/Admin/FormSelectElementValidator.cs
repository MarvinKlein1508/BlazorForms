using BlazorForms.Core.Models.FormElements;
using Microsoft.Extensions.Localization;

namespace BlazorForms.Core.Validators.Admin
{
    public class FormSelectElementValidator : FormElementWithOptionsValidator<FormSelectElement>
    {
        public FormSelectElementValidator(IStringLocalizer<FormSelectElement> localizer) : base(localizer)
        {

        }
    }


}
