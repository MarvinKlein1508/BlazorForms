using BlazorForms.Core.Constants;

namespace BlazorForms.Core.Models.FormElements
{
    public class FormTextareaElement : FormTextElementBase
    {
        public override ElementType GetElementType() => ElementType.Textarea;
        public override string GetDefaultName() => "Textarea";

        public override object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
