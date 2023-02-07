using FluentValidation;
using FormularPortal.Core.Validators.Admin;

namespace FormularPortal.Core.Models
{
    public class FormTextElement : FormTextElementBase
    {
        private static IValidator _validator = new FormTextElementValidator();
        public override ElementType GetElementType() => ElementType.Text;
        public override string GetDefaultName() => "Text";
        public override IValidator GetValidator() => _validator;
    }
}
