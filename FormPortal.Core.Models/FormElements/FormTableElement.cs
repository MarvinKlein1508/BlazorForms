using FormularPortal.Core;

namespace FormPortal.Core.Models.FormElements
{
    public class FormTableElement : FormElement
    {
        /// <summary>
        /// Holds all elements for the FormEditor
        /// </summary>
        public List<FormElement> Elements { get; set; } = new();

        /// <summary>
        /// Holds all values for a FormEntry.
        /// </summary>
        public List<List<FormElement>> ElementValues { get; set; } = new();
        public override ElementType GetElementType() => ElementType.Table;
        public override string GetDefaultName() => "Table";

        public void NewRow()
        {
            var tmp = Elements.DeepCopyByExpressionTree();
            ElementValues.Add(tmp);
        }
    }
}
