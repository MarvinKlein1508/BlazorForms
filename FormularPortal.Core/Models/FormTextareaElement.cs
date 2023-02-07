using DatabaseControllerProvider;
using FluentValidation;
using FormularPortal.Core.Validators.Admin;

namespace FormularPortal.Core.Models
{
    public class FormTextareaElement : FormTextElementBase
    {
        private static FormTextareaElementValidator _validator = new();
        public override ElementType GetElementType() => ElementType.Textarea;
        public override string GetDefaultName() => "Textarea";
        public override IValidator GetValidator() => _validator;
    }
}
