using FormPortal.Core.Constants;

namespace FormPortal.Core.Models.FormElements
{
    public class FormTextareaElement : FormTextElementBase
    {
        public override ElementType GetElementType() => ElementType.Textarea;
        public override string GetDefaultName() => "Textarea";
    }
}
