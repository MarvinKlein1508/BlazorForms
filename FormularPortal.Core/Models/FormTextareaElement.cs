using DatabaseControllerProvider;
using FluentValidation;
using FormularPortal.Core.Validators.Admin;

namespace FormularPortal.Core.Models
{
    public class FormTextareaElement : FormTextElementBase
    {
        public override ElementType GetElementType() => ElementType.Textarea;
        public override string GetDefaultName() => "Textarea";
    }
}
