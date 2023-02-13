namespace FormPortal.Core.Models.FormElements
{
    public class FormLabelElement : FormElement
    {
        public override ElementType GetElementType() => ElementType.Label;
        public override string GetDefaultName() => "Label";
    }
}
