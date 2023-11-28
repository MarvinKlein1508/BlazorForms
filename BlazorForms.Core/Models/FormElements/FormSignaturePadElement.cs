using BlazorForms.Core.Constants;
using DbController;

namespace BlazorForms.Core.Models.FormElements
{
    public class FormSignaturePadElement : FormElement
    {
        [CompareField("value_data")]
        public byte[] Value { get; set; } = [];
        public override string GetDefaultName() => "SignaturePad";

        public override ElementType GetElementType() => ElementType.SignaturePad;

        public override void Reset()
        {
            Value = [];
        }

        public override void SetValue(FormEntryElement element)
        {
            Value = element.ValueData;
        }

        public override Dictionary<string, object?> GetParameters()
        {
            var parameters = base.GetParameters();
            parameters["VALUE_DATA"] = Value;

            return parameters;
        }
    }
}
