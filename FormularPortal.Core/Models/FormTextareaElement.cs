using DatabaseControllerProvider;

namespace FormularPortal.Core.Models
{
    public class FormTextareaElement : FormTextElementBase
    {
        public override ElementType GetElementType() => ElementType.Textarea;
    }
}
