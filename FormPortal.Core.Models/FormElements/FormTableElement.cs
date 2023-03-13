using DatabaseControllerProvider;
using FormPortal.Core.Constants;
using FormPortal.Core;

namespace FormPortal.Core.Models.FormElements
{
    public class FormTableElement : FormElement
    {
        [CompareField("allow_add_rows")]
        public bool AllowAddRows { get; set; }
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

        public override Dictionary<string, object?> GetParameters()
        {
            var parameters = base.GetParameters();

            parameters.Add("ALLOW_ADD_ROWS", AllowAddRows);

            return parameters;
        }

        /// <summary>
        /// Adds a new row to fill out for the user.
        /// </summary>
        public List<FormElement> NewRow()
        {
            var tmp = Elements.DeepCopyByExpressionTree();
            ElementValues.Add(tmp);
            return tmp;
        }
    }
}
