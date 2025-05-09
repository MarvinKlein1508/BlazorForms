using BlazorForms.Core.Constants;

namespace BlazorForms.Core.Models.FormElements
{
    public class FormCheckboxElement : FormElement
    {
        public override ElementType GetElementType() => ElementType.Checkbox;
        [CompareField("value_boolean")]
        public bool Value { get; set; }
        public override string GetDefaultName() => "Checkbox";

        public override Dictionary<string, object?> GetParameters()
        {
            var parameters = base.GetParameters();
            parameters["VALUE_BOOLEAN"] = Value;
            return parameters;
        }

        public override void SetValue(FormEntryElement element)
        {
            Value = element.ValueBoolean;
        }

        public override void Reset()
        {
            Value = false;
        }

        public override object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
