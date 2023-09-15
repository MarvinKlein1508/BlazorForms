using DbController;
using BlazorForms.Core.Constants;

namespace BlazorForms.Core.Models.FormElements
{
    public class FormLabelElement : FormElement
    {
        [CompareField("description")]
        public string Description { get; set; } = string.Empty;
        [CompareField("show_on_pdf")]
        public bool ShowOnPdf { get; set; } = true;
        public override ElementType GetElementType() => ElementType.Label;
        public override string GetDefaultName() => "Label";

        public override Dictionary<string, object?> GetParameters()
        {
            var parameters = base.GetParameters();

            parameters.Add("DESCRIPTION", Description);
            parameters.Add("SHOW_ON_PDF", ShowOnPdf);

            return parameters;
        }
    }
}
