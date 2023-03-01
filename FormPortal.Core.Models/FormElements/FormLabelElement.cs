using DatabaseControllerProvider;
using FormPortal.Core.Constants;

namespace FormPortal.Core.Models.FormElements
{
    public class FormLabelElement : FormElement
    {
        [CompareField("description")]
        public string Description { get; set; } = string.Empty;
        public override ElementType GetElementType() => ElementType.Label;
        public override string GetDefaultName() => "Label";

        public override Dictionary<string, object?> GetParameters()
        {
            var parameters = base.GetParameters();

            parameters.Add("DESCRIPTION", Description);

            return parameters;
        }
    }
}
