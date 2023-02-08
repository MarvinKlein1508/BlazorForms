using FluentValidation;
using FormularPortal.Core.Validators.Admin;

namespace FormularPortal.Core.Models
{
    public class FormTableElement : FormElement
    {
        public override ElementType GetElementType() => ElementType.Table; 
        public override string GetDefaultName() => "Table";
    }
}
