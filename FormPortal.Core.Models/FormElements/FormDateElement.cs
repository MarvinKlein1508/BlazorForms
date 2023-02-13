using DatabaseControllerProvider;

namespace FormPortal.Core.Models.FormElements
{
    public class FormDateElement : FormElement
    {
        [CompareField("is_current_date_default")]
        public bool SetDefaultValueToCurrentDate { get; set; }
        [CompareField("min_value")]
        public DateTime MinDate { get; set; }
        [CompareField("max_value")]
        public DateTime MaxDate { get; set; }

        public DateTime Value { get; set; }
        public override ElementType GetElementType() => ElementType.Date;
        public override Dictionary<string, object?> GetParameters()
        {
            var parameters = base.GetParameters();
            parameters.Add("IS_CURRENT_DATE_DEFAULT", SetDefaultValueToCurrentDate);
            parameters.Add("MIN_VALUE", MinDate);
            parameters.Add("MAX_VALUE", MaxDate);
            return parameters;
        }

        public override string GetDefaultName() => "Date";
    }
}
