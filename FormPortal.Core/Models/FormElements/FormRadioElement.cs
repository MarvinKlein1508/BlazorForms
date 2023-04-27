using FormPortal.Core.Constants;

namespace FormPortal.Core.Models.FormElements
{
    public class FormRadioElement : FormElementWithOptions
    {
        public override ElementType GetElementType() => ElementType.Radio;
        public override string GetDefaultName() => "Radio";
    }
}
