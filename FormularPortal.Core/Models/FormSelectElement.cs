using FluentValidation;
using FormularPortal.Core.Validators.Admin;

namespace FormularPortal.Core.Models
{
    public class FormSelectElement : FormElementWithOptions
    {
        public override ElementType GetElementType() => ElementType.Select;
        public override string GetDefaultName() => "Select";
    }
}
