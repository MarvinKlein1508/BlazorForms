using DatabaseControllerProvider;
using FormPortal.Core.Constants;
using FormPortal.Core.Models.FormElements;

namespace FormPortal.Core.Models
{
    /// <summary>
    /// A <see cref="ElementCalcRuleSet"/> is a logical grouping of two <see cref="FormNumberElement"/> elements which can be chained together to automatically calculate values.
    /// </summary>
    public class ElementCalcRuleSet
    {
        [CompareField("calc_id")]
        public int CalcId { get; set; }
        
        [CompareField("math_operator")]
        public MathOperators MathOperator { get; set; }
        
        [CompareField("guid_element")]
        public Guid GuidElement { get; set; }
        
        [CompareField("sort_order")]
        public int SortOrder { get; set; }
    }
}
