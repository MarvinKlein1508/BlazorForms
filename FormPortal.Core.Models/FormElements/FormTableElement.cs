namespace FormPortal.Core.Models.FormElements
{
    public class FormTableElement : FormElement
    {
        public List<FormElement> Elements { get; set; } = new();
        public override ElementType GetElementType() => ElementType.Table;
        public override string GetDefaultName() => "Table";

        public void SortTableElements()
        {
            int elementCount = 1;
            foreach (var element in Elements)
            {
                element.SortOrder = elementCount++;
            }
        }
    }
}
