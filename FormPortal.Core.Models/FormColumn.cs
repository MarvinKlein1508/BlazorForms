using DatabaseControllerProvider;
using FormPortal.Core.Constants;
using FormPortal.Core.Interfaces;
using FormPortal.Core.Models.FormElements;

namespace FormPortal.Core.Models
{
    /// <summary>
    /// Represents a column for a <see cref="FormRow"/>
    /// </summary>
    public class FormColumn :  IDbModel, IHasSortableElement, IHasRuleSet
    {
        [CompareField("column_id")]
        public int ColumnId { get; set; }
        [CompareField("form_id")]
        public int FormId { get; set; }
        [CompareField("row_id")]
        public int RowId { get; set; }
        [CompareField("is_active")]
        public bool IsActive { get; set; } = true;
        [CompareField("rule_type")]
        public RuleType RuleType { get; set; }
        [CompareField("sort_order")]
        public int SortOrder { get; set; }

        public int Id => ColumnId;
        /// <summary>
        /// Gets or sets the elements for this column
        /// </summary>
        public List<FormElement> Elements { get; set; } = new();
        public List<Rule> Rules { get; set; } = new();
        public FormRow? Parent { get; set; }
        public Form? Form { get; set; }

        public FormColumn()
        {
            
        }

        public FormColumn(Form form)
        {
            Form = form;
        }
        public Dictionary<string, object?> GetParameters()
        {
            return new Dictionary<string, object?>
            {
                { "COLUMN_ID", ColumnId },
                { "FORM_ID", FormId },
                { "ROW_ID", RowId },
                { "IS_ACTIVE", IsActive},
                { "RULE_TYPE", RuleType.ToString() },
                { "SORT_ORDER", SortOrder }
            };
        }
        public IEnumerable<FormElement> GetElements()
        {
            foreach (var element in Elements)
            {
                yield return element;
            }
        }

        public bool IsVisible()
        {
            if(!IsActive)
            {
                return false;
            }

            if (RuleType is not RuleType.Visible)
            {
                return true;
            }

            if (!Rules.Any())
            {
                return true;
            }

            return Rules.ValidateRules();
        }

      
    }
}
