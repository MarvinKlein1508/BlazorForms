using BlazorForms.Core.Models.FormElements;
using Microsoft.Extensions.Localization;

namespace BlazorForms.Core.Validators.Admin
{
    public class FormLabelElementValidator : FormElementValidator<FormLabelElement>
    {
        public FormLabelElementValidator(IStringLocalizer<FormLabelElement> localizer) : base(localizer)
        {

        }
    }


}
