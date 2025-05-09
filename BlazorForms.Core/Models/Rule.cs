using BlazorForms.Core.Constants;
using BlazorForms.Core.Interfaces;
using BlazorForms.Core.Models.FormElements;

namespace BlazorForms.Core.Models;

public class Rule : IDbModel<int?>, IHasSortableElement
{
    public int RuleId { get; set; }
    public int FormId { get; set; }
    public int RowId { get; set; }
    public int? ColumnId { get; set; }
    public int? ElementId { get; set; }
    public LogicalOperator LogicalOperator { get; set; }
    public Guid ElementGuid { get; set; }
    public ComparisonOperator ComparisonOperator { get; set; }
    public bool ValueBoolean { get; set; }
    public string ValueString { get; set; } = string.Empty;
    public decimal ValueNumber { get; set; }
    public DateTime ValueDate { get; set; }
    public int SortOrder { get; set; }

    [IgnoreModificationCheck]
    public FormElement? Element { get; set; }

    public int? GetIdentifier()
    {
        return RuleId > 0 ? RuleId : null;
    }
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
