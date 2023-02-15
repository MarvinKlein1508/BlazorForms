namespace FormPortal.Core.Models.FormElements
{
    public class FormTableElement : FormElement
    {
        public List<FormElement> Elements { get; set; } = new();
        public override ElementType GetElementType() => ElementType.Table;
        public override string GetDefaultName() => "Table";
    }
}
