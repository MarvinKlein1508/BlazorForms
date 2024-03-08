using DbController;
using BlazorForms.Core.Constants;

namespace BlazorForms.Core.Models.FormElements
{
    public class FormDateElement : FormElement
    {
        [CompareField("is_current_date_default")]
        public bool SetDefaultValueToCurrentDate { get; set; }
        [CompareField("min_value")]
        public DateTime MinDate { get; set; }
        [CompareField("max_value")]
        public DateTime MaxDate { get; set; }
        [CompareField("value_date")]
        public DateTime Value { get; set; }
        public override ElementType GetElementType() => ElementType.Date;
        public override Dictionary<string, object?> GetParameters()
        {
            var parameters = base.GetParameters();
            parameters.Add("IS_CURRENT_DATE_DEFAULT", SetDefaultValueToCurrentDate);
            parameters.Add("MIN_VALUE", MinDate);
            parameters.Add("MAX_VALUE", MaxDate);

            // MSSQL cannot handle dates older than this. 
            if (Value.Year > 1930)
            {
                parameters["VALUE_DATE"] = Value;
            }

            return parameters;
        }

        public override string GetDefaultName() => "Date";

        public override void SetValue(FormEntryElement element)
        {
            Value = element.ValueDate;
        }

        public override void Reset()
        {
            if (SetDefaultValueToCurrentDate)
            {
                Value = DateTime.Now;
            }
            else
            {
                Value = default;
            }
        }

        public override object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
