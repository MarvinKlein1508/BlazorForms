using DatabaseControllerProvider;

namespace FormularPortal.Core.Models
{
    public class RuleSet : IDbModel
    {
        [CompareField("rule_id")]
        public int RuleId { get; set; }
        [CompareField("element_id")]
        public int ElementId { get; set; }
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
        public FormElement? Element { get; set; }
        public FormElement? Parent  { get; set; }

        public Dictionary<string, object?> GetParameters()
        {
            return new Dictionary<string, object?>
            {
                { "RULE_ID", RuleId },
                { "ELEMENT_ID", ElementId },
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

    public enum LogicalOperator
    {
        And,
        Or
    }

    public enum ComparisonOperator
    {
        Equals,
        NotEquals,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual
    }
}
