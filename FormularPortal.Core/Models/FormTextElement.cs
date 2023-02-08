using FluentValidation;
using FormularPortal.Core.Validators.Admin;

namespace FormularPortal.Core.Models
{
    public class FormTextElement : FormTextElementBase
    {
        public override ElementType GetElementType() => ElementType.Text;
        public override string GetDefaultName() => "Text";
    }
}
