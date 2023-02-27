namespace FormPortal.Core.Models.FormElements
{
    public class FormCheckboxElement : FormElement
    {
        public override ElementType GetElementType() => ElementType.Checkbox;
        public bool Value { get; set; }
        public override string GetDefaultName() => "Checkbox";

        public override Dictionary<string, object?> GetParameters()
        {
            var parameters = base.GetParameters();
            parameters["VALUE_BOOLEAN"] = Value;
            return parameters;
        }
    }
}
