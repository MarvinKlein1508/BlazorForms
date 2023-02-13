namespace FormPortal.Core.Models.FormElements
{
    public abstract class FormElementWithOptions : FormElement
    {
        public List<FormElementOption> Options { get; set; } = new List<FormElementOption>();
        public string Value { get; set; } = string.Empty;

    }
}
