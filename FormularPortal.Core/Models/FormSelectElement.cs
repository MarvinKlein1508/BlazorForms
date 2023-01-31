namespace FormularPortal.Core.Models
{
    public class FormSelectElement : FormElementWithOptions
    {
        public override ElementType GetElementType() => ElementType.Select;
    }
}
