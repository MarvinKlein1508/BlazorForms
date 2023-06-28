using DbController;
using BlazorForms.Core.Constants;
using BlazorForms.Core.Interfaces;
using BlazorForms.Core.Models.FormElements;

namespace BlazorForms.Core.Models
{
    public class Rule : IDbModel, IHasSortableElement
    {
        [CompareField("rule_id")]
        public int RuleId { get; set; }
        [CompareField("form_id")]
        public int FormId { get; set; }
        [CompareField("row_id")]
        public int RowId { get; set; }
        [CompareField("column_id")]
        public int? ColumnId { get; set; }
        [CompareField("element_id")]
        public int? ElementId { get; set; }
        [CompareField("logical_operator")]
        public LogicalOperator LogicalOperator { get; set; }
        [CompareField("element_guid")]
        public Guid ElementGuid { get; set; }
        [CompareField("comparison_operator")]
        public ComparisonOperator ComparisonOperator { get; set; }
        [CompareField("value_boolean")]
        public bool ValueBoolean { get; set; }
        [CompareField("value_string")]
        public string ValueString { get; set; } = string.Empty;
        [CompareField("value_number")]
        public decimal ValueNumber { get; set; }
        [CompareField("value_date")]
        public DateTime ValueDate { get; set; }
        [CompareField("sort_order")]
        public int SortOrder { get; set; }

        public int Id => RuleId;
        [IgnoreModificationCheck]
        public FormElement? Element { get; set; }

        public Dictionary<string, object?> GetParameters()
        {
            return new Dictionary<string, object?>
            {
                { "RULE_ID", RuleId },
                { "FORM_ID", FormId },
                { "ROW_ID", RowId },
                { "COLUMN_ID", ColumnId is 0 ? null : ColumnId },
                { "ELEMENT_ID", ElementId is 0 ? null : ElementId },
                { "LOGICAL_OPERATOR", LogicalOperator.ToString() },
                { "ELEMENT_GUID", ElementGuid.ToString() },
                { "COMPARISON_OPERATOR", ComparisonOperator.ToString() },
                { "VALUE_BOOLEAN", ValueBoolean },
                { "VALUE_STRING", ValueString },
                { "VALUE_NUMBER", ValueNumber },
                { "VALUE_DATE", ValueDate },
                { "SORT_ORDER", SortOrder}
            };
        }
    }
}
