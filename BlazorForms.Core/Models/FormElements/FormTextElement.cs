using BlazorForms.Core.Constants;
namespace BlazorForms.Core.Models.FormElements
{
    public class FormTextElement : FormTextElementBase
    {
        public override ElementType GetElementType() => ElementType.Text;
        public override string GetDefaultName() => "Text";

        public override object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
