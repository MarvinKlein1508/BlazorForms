using FluentValidation;
using FormularPortal.Core.Validators.Admin;

namespace FormularPortal.Core.Models
{
    public class FormTableElement : FormElement
    {
        private static FormTableElementValidator _validator = new();
        public override ElementType GetElementType() => ElementType.Table;
        public override string GetDefaultName() => "Table";
        public override IValidator GetValidator() => _validator;
    }
}
