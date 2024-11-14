using DbController;
using BlazorForms.Core.Constants;
using BlazorForms.Core.Interfaces;
using BlazorForms.Core.Models.FormElements;

namespace BlazorForms.Core.Models
{
    /// <summary>
    /// A <see cref="CalcRule"/> is a logical grouping of two <see cref="FormNumberElement"/> elements which can be chained together to automatically calculate values.
    /// </summary>
    public class CalcRule : IDbModel<int?>, IHasSortableElement
    {
        [CompareField("calc_rule_id")]
        public int CalcRuleId { get; set; }
        [CompareField("element_id")]
        public int ElementId { get; set; }
        [CompareField("math_operator")]
        public MathOperators MathOperator { get; set; }

        [CompareField("guid_element")]
        public Guid GuidElement { get; set; }

        [CompareField("sort_order")]
        public int SortOrder { get; set; }

        public int? GetIdentifier()
        {
            return CalcRuleId > 0 ? CalcRuleId : null;
        }

        public Dictionary<string, object?> GetParameters()
        {
            return new Dictionary<string, object?>
            {
                {"CALC_RULE_ID", CalcRuleId },
                {"ELEMENT_ID", ElementId },
                {"MATH_OPERATOR", MathOperator.ToString() },
                {"GUID_ELEMENT", GuidElement.ToString()},
                {"SORT_ORDER", SortOrder }
            };
        }
    }
}
