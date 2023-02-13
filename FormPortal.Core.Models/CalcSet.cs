using DatabaseControllerProvider;
using FormPortal.Core.Constants;
using FormPortal.Core.Models.FormElements;

namespace FormPortal.Core.Models
{
    /// <summary>
    /// A <see cref="CalcSet"/> is a logical grouping of two <see cref="FormNumberElement"/> elements which can be chained together to automatically calculate values.
    /// </summary>
    public class CalcSet
    {
        [CompareField("calc_id")]
        public int CalcId { get; set; }
        /// <summary>
        /// The BaseOperator is being used to chain multiple CalcSets together
        /// </summary>
        [CompareField("base_operator")]
        public MathOperators BaseOperator { get; set; }
        [CompareField("guid_element_1")]
        public Guid GuidElement1 { get; set; }
        [CompareField("math_operator")]
        public MathOperators MathOperator { get; set; }
        [CompareField("guid_element_2")]
        public Guid GuidElement2 { get; set; }
        [CompareField("sort_order")]
        public int SortOrder { get; set; }
    }
}
