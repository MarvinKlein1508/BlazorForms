namespace FormPortal.Core.Models.FormElements
{
    public class FormCheckboxElement : FormElement
    {
        public override ElementType GetElementType() => ElementType.Checkbox;
        public bool Value { get; set; }
        public override string GetDefaultName() => "Checkbox";
    }
}
