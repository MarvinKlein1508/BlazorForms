using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormularPortal.Core.Models
{
    public class RuleSet
    {
        public LogicalOperator LogicalOperator { get; set; }
        public FormElement? Element { get; set; }
        public ComparisonOperator CompareOperator { get; set; }
        public bool ValueBoolean { get; set; }
        public string ValueString { get; set; } = string.Empty;
        public decimal ValueNumber { get; set; }
        public DateTime ValueDate { get; set; }
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
